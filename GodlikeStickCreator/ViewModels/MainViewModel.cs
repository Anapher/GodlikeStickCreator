using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.Core.Config;
using GodlikeStickCreator.Core.System;
using GodlikeStickCreator.Utilities;
using GodlikeStickCreator.ViewModelBase;
using ICSharpCode.SharpZipLib.Zip;
using SevenZip;

namespace GodlikeStickCreator.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private readonly List<View> _views;
        private bool _canGoBack;
        private bool _canGoForward;
        private View _currentView;
        private RelayCommand _goBackCommand;
        private RelayCommand _goForwardCommand;

        public MainViewModel()
        {
            var tempFolder = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "5E6C58AD-D485-4904-8193-BC26FE41BB11"));
            tempFolder.Create();

            var filename = Path.Combine(tempFolder.FullName, "7z.dll");
            WpfUtilities.WriteResourceToFile(new Uri("pack://application:,,,/Resources/Utilities/7z.dll"),
                Path.Combine(tempFolder.FullName, filename));
            SevenZipExtractor.SetLibraryPath(filename);

            UsbStickSettings = new UsbStickSettings();
            _views = new List<View>
            {
                new SelectDriveViewModel(UsbStickSettings),
                new SystemsViewModel(UsbStickSettings),
                new ModifyAppearanceViewModel(UsbStickSettings),
                new ApplicationsViewModel(UsbStickSettings)
            };
            CurrentView = _views[0];
            _views.ForEach(view => view.GoForwardChanged += (sender, args) => RefreshCanGoForward());
            RefreshCanGoForward();

            Application.Current.Exit += (sender, args) =>
            {
                _views.ForEach(x => x.Dispose());
                tempFolder.Delete(true);
            };
        }

        public View CurrentView
        {
            get { return _currentView; }
            set { SetProperty(value, ref _currentView); }
        }

        public bool CanGoBack
        {
            get { return _canGoBack; }
            set { SetProperty(value, ref _canGoBack); }
        }

        public bool CanGoForward
        {
            get { return _canGoForward; }
            set { SetProperty(value, ref _canGoForward); }
        }

        public RelayCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (_goBackCommand = new RelayCommand(parameter =>
                {
                    CurrentView = _views[_views.IndexOf(CurrentView) - 1];
                    CanGoBack = _views.IndexOf(CurrentView) > 0;
                    RefreshCanGoForward();
                }));
            }
        }

        public RelayCommand GoForwardCommand
        {
            get
            {
                return _goForwardCommand ?? (_goForwardCommand = new RelayCommand(async parameter =>
                {
                    if (CurrentViewMode == ViewMode.LastStep)
                    {
                        var processView = new ProcessViewModel(UsbStickSettings);
                        CurrentView = processView;
                        bool failed;
                        CanGoBack = false;
                        CanGoForward = false;
                        try
                        {
                            failed = await DoYourStuff(processView);
                        }
                        catch (Exception ex)
                        {
                            processView.Logger.Error(ex.ToString());
                            failed = true;
                        }

                        if (failed)
                        {
                            CanGoBack = true;
                        }
                        return;
                    }
                    if (CurrentViewMode == ViewMode.Finished)
                    {
                        return;
                    }
                    CurrentView = _views[_views.IndexOf(CurrentView) + 1];
                    CanGoBack = _views.IndexOf(CurrentView) != 0;
                    RefreshCanGoForward();
                }));
            }
        }

        public UsbStickSettings UsbStickSettings { get; }

        public ViewMode CurrentViewMode { get; set; }

        private async Task<bool> DoYourStuff(ProcessViewModel processView)
        {
            await Task.Delay(500); //wait for the view to build up and subscribe to the logger event

            processView.Logger.Status("Check if drive is still plugged in...");
            if (!UsbStickSettings.Drive.IsReady)
            {
                processView.Logger.Error("Drive is not ready. Please plug it in again");
                return false;
            }
            var stopwatch = Stopwatch.StartNew();
            //progress:
            //10 % = install syslinux etc.
            //60 % = install systems
            //30 % = download & copy applications

            processView.Logger.Success("Drive plugged in");

            if (UsbStickSettings.FormatDrive)
            {
                if (MessageBoxEx.Show(Application.Current.MainWindow,
                    $"Warning: You selected the formatting option. Formatting will erease ALL data on the drive {UsbStickSettings.Drive.Name}. If you want to continue, press ok.",
                    $"Format drive {UsbStickSettings.Drive.Name}", MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning, MessageBoxResult.Cancel) != MessageBoxResult.OK)
                    return false;

                processView.Logger.Status($"Formatting drive {UsbStickSettings.Drive.Name} with FAT32");
                var driveLetter = UsbStickSettings.Drive.Name.Substring(0, 2);

                FormatResponse formatResponse;
                if (User.IsAdministrator)
                {
                    formatResponse = await Task.Run(() => DriveUtilities.FormatDrive(driveLetter, "FAT32", true, 8192, App.DriveLabel, false));
                }
                else
                {
                    var process = new Process
                    {
                        StartInfo =
                        {
                            FileName = Assembly.GetExecutingAssembly().Location,
                            Arguments = $"/format {driveLetter}",
                            Verb = "runas"
                        }
                    };
                    if (!process.Start())
                    {
                        processView.Logger.Error(
                            "Was not able to restart application with administrator privileges to format the drive");
                        return false;
                    }

                    await Task.Run(() => process.WaitForExit());
                    formatResponse = (FormatResponse) process.ExitCode;
                }

                if (formatResponse != FormatResponse.Success)
                {
                    processView.Logger.Error("Formatting drive failed: " + formatResponse);
                    return false;
                }
                processView.Logger.Success("Successfully formatted drive");
            }

            if (UsbStickSettings.Drive.VolumeLabel != App.DriveLabel)
            {
                processView.Logger.Status("Change volume label to " + App.DriveLabel);
                UsbStickSettings.Drive.VolumeLabel = App.DriveLabel;
            }

            var bootStickCreator = new BootStickCreator(UsbStickSettings.Drive,
                new BootStickConfig {ScreenMessage = "Hello World", ScreenTitle = "Godlike Stick"});
            processView.Message = "Creating bootable stick";
            await Task.Run(() => bootStickCreator.CreateBootStick(processView.Logger));
            processView.CurrentProgress = 0.1;

            var installSystemPercentage = UsbStickSettings.ApplicationInfo.Any(x => x.Add) ? 0.6 : 0.9;
            for (int i = 0; i < UsbStickSettings.Systems.Count; i++)
            {
                var systemInfo = UsbStickSettings.Systems[i];
                var progressReporter = new SystemProgressReporter();
                progressReporter.ProgressChanged +=
                    (sender, d) =>
                        processView.CurrentProgress =
                            0.1 +
                            (installSystemPercentage/UsbStickSettings.Systems.Count*i +
                             installSystemPercentage/UsbStickSettings.Systems.Count*d);
                progressReporter.MessageChanged +=
                    (sender, s) =>
                    {
                        processView.Message =
                            $"Install {systemInfo.Name} ({i + 1} / {UsbStickSettings.Systems.Count}): " + s;
                        processView.Logger.Status(s);
                    };

                processView.Message = $"Install {systemInfo.Name} ({i + 1} / {UsbStickSettings.Systems.Count})";
                await
                    Task.Run(
                        () =>
                            bootStickCreator.AddSystemToBootStick(systemInfo, processView.Logger,
                                progressReporter));
            }

            processView.CurrentProgress = 0.7;
            processView.Logger.Success("All systems installed");

            var applicationsToInstall = UsbStickSettings.ApplicationInfo.Where(x => x.Add).ToList();
            for (int i = 0; i < applicationsToInstall.Count; i++)
            {
                var application = applicationsToInstall[i];
                processView.Message =
                    $"Install {application.Name} ({i + 1} / {applicationsToInstall.Count}): Download";

                var downloadUrl = await Task.Run(() => application.DownloadUrl.Value);
                processView.Logger.Status($"Download {downloadUrl}");
                var tempFile =
                    FileExtensions.GetFreeTempFileName(new FileInfo(new Uri(downloadUrl).AbsolutePath).Extension);
                File.Delete(tempFile);
                var webClient = new WebClient();
                webClient.DownloadProgressChanged +=
                    (sender, args) =>
                        processView.CurrentProgress =
                            0.1 + 0.6 +
                            (0.3/applicationsToInstall.Count*i +
                             0.3/applicationsToInstall.Count*(args.ProgressPercentage/100d));
                await webClient.DownloadFileTaskAsync(downloadUrl, tempFile);

                processView.Logger.Success("Application downloaded successfully");
                processView.Logger.Status("Extract zip archive");
                processView.Message =
                    $"Install {application.Name} ({i + 1} / {applicationsToInstall.Count}): Extract";
                var targetDirectory =
                    new DirectoryInfo(Path.Combine(UsbStickSettings.Drive.RootDirectory.FullName,
                        "Tools",
                        EnumUtilities.GetDescription(application.ApplicationCategory), application.Name));
                targetDirectory.Create();

                if (downloadUrl.EndsWith(".zip"))
                {
                    var fastZip = new FastZip();

                    await
                        Task.Run(
                            () =>
                                fastZip.ExtractZip(tempFile, targetDirectory.FullName,
                                    FastZip.Overwrite.Always, null,
                                    null,
                                    null, false));
                }
                else
                {
                    using (var file = new SevenZipExtractor(tempFile))
                    using (var autoResetEvent = new AutoResetEvent(false))
                    {
                        file.ExtractionFinished += (sender, args) => autoResetEvent.Set();
                        file.BeginExtractArchive(targetDirectory.FullName);
                        await Task.Run(() => autoResetEvent.WaitOne());
                    }
                }

                File.Delete(tempFile);

                var entries = targetDirectory.GetFileSystemInfos();
                if (entries.Length == 1 &&
                    (entries[0].Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    var directory = new DirectoryInfo(entries[0].FullName);
                    await Task.Run(() =>
                    {
                        directory.MoveTo(Path.Combine(directory.Parent.FullName,
                            Guid.NewGuid().ToString("D")));
                        //in case that there's a directory with the same name
                        foreach (var fileInfo in directory.GetFiles())
                            fileInfo.MoveTo(Path.Combine(targetDirectory.FullName, fileInfo.Name));
                        foreach (var directoryInfo in directory.GetDirectories())
                            directoryInfo.MoveTo(Path.Combine(targetDirectory.FullName, directoryInfo.Name));
                        directory.Delete();
                    });
                }
                processView.Logger.Success("Application successfully installed");
            }

            processView.CurrentProgress = 1;
            processView.Logger.Success($"Finished in {stopwatch.Elapsed.ToString("g")}");
            return true;
        }

        private void RefreshCanGoForward()
        {
            CanGoForward = CurrentView.CanGoForward;
            if (_views.IndexOf(CurrentView) == _views.Count - 1)
                CurrentViewMode = ViewMode.LastStep;
            else
                CurrentViewMode = ViewMode.SmallTalk;
        }
    }

    public enum ViewMode
    {
        SmallTalk,
        LastStep,
        Finished
    }
}