using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.Core.System;
using GodlikeStickCreator.ViewModelBase;
using Ookii.Dialogs.Wpf;

namespace GodlikeStickCreator.ViewModels
{
    public class SystemsViewModel : View
    {
        private RelayCommand _changeIsosPathCommand;
        private FileSystemWatcher _fileSystemWatcher;
        private string _isoPath;
        private RelayCommand _openDownloadUrlCommand;
        private RelayCommand _openIsosPathCommand;
        private RelayCommand _openSystemWebsiteUrlCommand;

        public SystemsViewModel(UsbStickSettings usbStickSettings) : base(usbStickSettings)
        {
            Systems = SupportedSystems.Systems.OrderBy(x => x.Name).ToList();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ISOs");
            Directory.CreateDirectory(path);
            IsoPath = path;
        }

        public override void Dispose()
        {
            base.Dispose();
            _fileSystemWatcher.Dispose();
        }

        public List<SystemInfo> Systems { get; }

        public string IsoPath
        {
            get { return _isoPath; }
            set
            {
                if (SetProperty(value, ref _isoPath))
                {
                    _fileSystemWatcher?.Dispose();
                    _fileSystemWatcher = new FileSystemWatcher(IsoPath) { IncludeSubdirectories = false };
                    _fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
                    _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
                    _fileSystemWatcher.Renamed += FileSystemWatcherOnRenamed;
                    _fileSystemWatcher.Deleted += FileSystemWatcherOnDeleted;
                    _fileSystemWatcher.EnableRaisingEvents = true;
                    CheckAllIsos();
                }
            }
        }

        public RelayCommand ChangeIsosPathCommand
        {
            get
            {
                return _changeIsosPathCommand ?? (_changeIsosPathCommand = new RelayCommand(parameter =>
                {
                    var folderBrowser = new VistaFolderBrowserDialog { ShowNewFolderButton = true };
                    if (folderBrowser.ShowDialog(Application.Current.MainWindow) == true)
                    {
                        IsoPath = folderBrowser.SelectedPath;
                    }
                }));
            }
        }

        public RelayCommand OpenIsosPathCommand
        {
            get
            {
                return _openIsosPathCommand ??
                       (_openIsosPathCommand =
                           new RelayCommand(parameter => { Process.Start("explorer.exe", "\"" + IsoPath + "\""); }));
            }
        }

        public RelayCommand OpenSystemWebsiteUrlCommand
        {
            get
            {
                return _openSystemWebsiteUrlCommand ?? (_openSystemWebsiteUrlCommand = new RelayCommand(parameter =>
                {
                    var system = parameter as SystemInfo;
                    if (system == null)
                        return;

                    Process.Start(system.Website);
                }));
            }
        }

        public RelayCommand OpenDownloadUrlCommand
        {
            get
            {
                return _openDownloadUrlCommand ?? (_openDownloadUrlCommand = new RelayCommand(parameter =>
                {
                    var system = parameter as SystemInfo;
                    if (system == null)
                        return;

                    Process.Start(system.DownloadUrl);
                }));
            }
        }

        private void FileSystemWatcherOnDeleted(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            CheckAllIsos();
        }

        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            CheckAllIsos();
        }

        private void FileSystemWatcherOnRenamed(object sender, RenamedEventArgs renamedEventArgs)
        {
            CheckAllIsos();
        }

        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            CheckAllIsos();
        }

        private void CheckAllIsos()
        {
            var alreadyAdded = Systems.Where(x => x.IsAdded).ToList();
            foreach (var file in new DirectoryInfo(IsoPath).GetFiles())
            {
                var system = Systems.FirstOrDefault(x => x.IsoFileMatch.IsMatch(file.Name));
                if (system == null)
                    continue;

                if (alreadyAdded.Contains(system))
                {
                    alreadyAdded.Remove(system);
                    continue;
                }

                system.IsAdded = true;
                system.Filename = file.FullName;
            }

            foreach (var systemInfo in alreadyAdded)
            {
                systemInfo.IsAdded = false;
            }

            UsbStickSettings.Systems = Systems.Where(x => x.IsAdded).ToList();
        }
    }
}