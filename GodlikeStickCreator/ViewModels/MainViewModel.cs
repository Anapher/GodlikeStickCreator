using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.Core.Config;
using GodlikeStickCreator.Core.System;
using GodlikeStickCreator.Utilities;
using GodlikeStickCreator.ViewModelBase;
using ICSharpCode.SharpZipLib.Zip;

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
            UsbStickSettings = new UsbStickSettings();
            _views = new List<View>
            {
                new SelectDriveViewModel(UsbStickSettings),
                new SystemsViewModel(UsbStickSettings),
                new ApplicationsViewModel(UsbStickSettings)
            };
            CurrentView = _views[0];
            _views.ForEach(view => view.GoForwardChanged += (sender, args) => RefreshCanGoForward());
            RefreshCanGoForward();
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
                        CanGoBack = false;
                        CanGoForward = false;

                        processView.Logger.Status("Check if drive is still plugged in...");
                        if (!UsbStickSettings.Drive.IsReady)
                        {
                            processView.Logger.Error("Drive is not ready. Please plug it in again");
                            CanGoBack = true;
                            return;
                        }
                        var stopwatch = Stopwatch.StartNew();
                        //progress:
                        //10 % = install syslinux etc.
                        //60 % = install systems
                        //30 % = download & copy applications

                        processView.Logger.Success("Drive plugged in");
                        var bootStickCreator = new BootStickCreator(UsbStickSettings.Drive,
                            new BootStickConfig {ScreenMessage = "Hello World", ScreenTitle = "Godlike Stick"});
                        processView.Message = "Creating bootable stick";
                        await Task.Run(() => bootStickCreator.CreateBootStick(processView.Logger));
                        processView.CurrentProgress = 0.3;

                        for (int i = 0; i < UsbStickSettings.Systems.Count; i++)
                        {
                            var systemInfo = UsbStickSettings.Systems[i];
                            var progressReporter = new SystemProgressReporter();
                            progressReporter.ProgressChanged +=
                                (sender, d) => processView.CurrentProgress = 0.1 + 0.6/((0.6/i + 1)*i + (0.6/i + 1)*d);
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

                        processView.CurrentProgress = 0.6;
                        processView.Logger.Success("All systems installed");

                        var applicationsToInstall = UsbStickSettings.ApplicationInfo.Where(x => x.Add).ToList();
                        for (int i = 0; i < applicationsToInstall.Count; i++)
                        {
                            var application = applicationsToInstall[i];
                            processView.Message =
                                $"Install {application.Name} ({i + 1} / {applicationsToInstall.Count}): Download";

                            var downloadUrl = await Task.Run(() => application.DownloadUrl.Value);
                            processView.Logger.Status($"Download {downloadUrl}");
                            var tempFile = Path.GetTempFileName();
                            File.Delete(tempFile);
                            var webClient = new WebClient();
                            webClient.DownloadProgressChanged +=
                                (sender, args) =>
                                    processView.CurrentProgress =
                                        0.1 + 0.6 + 0.3/((0.3/i + 1)*i + (0.3/i + 1)*args.BytesReceived);
                            await webClient.DownloadFileTaskAsync(downloadUrl, downloadUrl);

                            processView.Logger.Success("Application downloaded successfully");
                            processView.Logger.Status("Extract zip archive");
                            var fastZip = new FastZip();

                            var targetDirectory =
                                new DirectoryInfo(Path.Combine(UsbStickSettings.Drive.RootDirectory.FullName, "Tools",
                                    EnumUtilities.GetDescription(application.ApplicationCategory), application.Name));
                            targetDirectory.Create();

                            fastZip.ExtractZip(tempFile, targetDirectory.FullName, FastZip.Overwrite.Always, null, null,
                                null, false);

                            var entries = targetDirectory.GetFileSystemInfos();
                            if (entries.Length == 1 &&
                                (entries[0].Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                            {
                                var directory = new DirectoryInfo(entries[0].FullName);
                                directory.MoveTo(Path.Combine(directory.Parent.FullName, Guid.NewGuid().ToString("D"))); //in case that there's a directory with the same name
                                foreach (var fileInfo in directory.GetFiles())
                                    fileInfo.MoveTo(Path.Combine(targetDirectory.FullName, fileInfo.Name));
                                foreach (var directoryInfo in directory.GetDirectories())
                                    directoryInfo.MoveTo(Path.Combine(targetDirectory.FullName, directoryInfo.Name));
                                directory.Delete();
                            }
                            processView.Logger.Success("Application successfully installed");
                        }

                        processView.CurrentProgress = 1;
                        processView.Logger.Success($"Finished in {stopwatch.Elapsed.ToString("g")}");
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