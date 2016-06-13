using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
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
            Systems = new[]
   {
                new SystemInfo
                {
                    Name = "Kali Linux",
                    Description =
                        "Kali Linux is an Advanced Penetration Testing Linux distribution used for Penetration Testing, Ethical Hacking and network security assessments.",
                    Category = Category.SystemTools,
                    InstallMethod = InstallMethod.Other,
                    Website = "https://www.kali.org/",
                    DownloadUrl = "https://www.kali.org/downloads/",
                    Thumbnail = new BitmapImage(new Uri("/Resources/Thumbnails/KaliLinux.png", UriKind.Relative)),
                    IsoFileMatch = "^kali.*\\.iso$"
                },
                new SystemInfo
                {
                    Name = "MemTest86+",
                    Description =
                        "MemTest86 is the original, free, stand alone memory testing software for x86 computers. ",
                    Category = Category.SystemTools,
                    InstallMethod = InstallMethod.Memtest86,
                    Website = "http://www.memtest86.com/",
                    DownloadUrl = "http://www.memtest86.com/downloads/memtest86-usb.zip",
                    Thumbnail = new BitmapImage(new Uri("/Resources/Thumbnails/MemTest86.png", UriKind.Relative)),
                    IsoFileMatch = "^memtest86\\+-.*\\.zip$"
                },
                new SystemInfo
                {
                    Name = "Kaspersky Rescue Disk",
                    Description =
                        "Kaspersky Rescue Disk is a free tool for disinfecting computers from malware which does not allow the operating system to start.",
                    Category = Category.AntiVirus,
                    InstallMethod = InstallMethod.KasperskyRescueDisk,
                    Website = "https://www.kaspersky.com",
                    DownloadUrl = "https://support.kaspersky.com/us/viruses/rescuedisk",
                    Thumbnail =
                        new BitmapImage(new Uri("/Resources/Thumbnails/KasperskyRescueDisk.png", UriKind.Relative)),
                    IsoFileMatch = @"^kav_rescue_10.iso$"
                },
                new SystemInfo
                {
                    Name = "Kodibuntu",
                    Description =
                        "Kodi (formerly XBMC) is a free and open-source media player software application developed by the XBMC Foundation, a non-profit technology consortium.",
                    Category = Category.Multimedia,
                    InstallMethod = InstallMethod.Other,
                    Website = "http://kodi.wiki/view/kodibuntu",
                    DownloadUrl = "https://kodi.tv/download/",
                    Thumbnail = new BitmapImage(new Uri("/Resources/Thumbnails/Kodi.png", UriKind.Relative)),
                    IsoFileMatch = "^kodibuntu.*\\.iso$"
                },
                new SystemInfo
                {
                    Name = "Tails",
                    Description =
                        "Tails is a live operating system which aims at preserving your privacy and anonymity and helps you to use the Internet anonymously and circumvent censorship.",
                    Category = Category.LinuxDistributions,
                    InstallMethod = InstallMethod.Other,
                    Website = "https://tails.boum.org/",
                    DownloadUrl = "http://dl.amnesia.boum.org/tails/stable/",
                    Thumbnail = new BitmapImage(new Uri("/Resources/Thumbnails/Tails.png", UriKind.Relative)),
                    IsoFileMatch = "^tails.*\\.iso$"
                },
                new SystemInfo
                {
                    Name = "Ubuntu Studio",
                    Description =
                        "Ubuntu Studio is a free and open operating system for creative people. It comes preinstalled with a selection of the most common free multimedia applications available.",
                    Category = Category.Multimedia,
                    InstallMethod = InstallMethod.Other,
                    Website = "https://ubuntustudio.org/",
                    DownloadUrl = "https://ubuntustudio.org/download/",
                    Thumbnail = new BitmapImage(new Uri("/Resources/Thumbnails/UbuntuStudio.png", UriKind.Relative)),
                    IsoFileMatch = "^ubuntustudio.*\\.iso$"
                },
                new SystemInfo
                {
                    Name = "System Rescue CD",
                    Description =
                        "SystemRescueCd is a Linux system rescue disk for administrating or repairing your system and data after a crash.",
                    Category = Category.SystemTools,
                    InstallMethod = InstallMethod.Other,
                    Website = "https://www.system-rescue-cd.org/",
                    DownloadUrl = "https://sourceforge.net/projects/systemrescuecd/files/latest/download",
                    Thumbnail = new BitmapImage(new Uri("/Resources/Thumbnails/SystemRescueCd.png", UriKind.Relative)),
                    IsoFileMatch = "^systemrescuecd.*\\.iso$"
                },
                new SystemInfo
                {
                    Name = "Redo Backup and Recovery",
                    Description =
                        "Redo Backup and Recovery is so simple that anyone can use it. It is the easiest, most complete disaster recovery solution available. It allows bare-metal restore.",
                    Category = Category.SystemTools,
                    InstallMethod = InstallMethod.Other,
                    Website = "http://redobackup.org/",
                    DownloadUrl = "http://redobackup.org/download.php",
                    Thumbnail = new BitmapImage(new Uri("/Resources/Thumbnails/RedoBackup.png", UriKind.Relative)),
                    IsoFileMatch = "^redobackup-livecd.*\\.iso$"
                },
                new SystemInfo
                {
                    Name = "DBAN",
                    Description =
                        "Darik’s Boot and Nuke CD is the easiest way to permanently and totally destroy every bit of personal information on that drive—nobody is going to recover a thing once this is done.",
                    Category = Category.SystemTools,
                    InstallMethod = InstallMethod.Other,
                    Website = "http://www.dban.org/",
                    DownloadUrl = "http://www.dban.org/download",
                    Thumbnail = new BitmapImage(new Uri("/Resources/Thumbnails/DBAN.png", UriKind.Relative)),
                    IsoFileMatch = "^dban.*\\.iso$"
                },
                new SystemInfo
                {
                    Name = "Offline NT Password & Registry Editor",
                    Description =
                        "This is a utility to reset the password of any user that has a valid local account on your Windows system.",
                    Category = Category.SystemTools,
                    InstallMethod = InstallMethod.Other,
                    Website = "https://pogostick.net/~pnh/ntpasswd/",
                    DownloadUrl = "https://pogostick.net/~pnh/ntpasswd/",
                    Thumbnail = new BitmapImage(new Uri("/Resources/Thumbnails/ntpaswd.png", UriKind.Relative)),
                    IsoFileMatch = "^usb.*\\.zip$"
                }
            }.OrderBy(x => x.Name).ToList();

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
                var system = Systems.FirstOrDefault(x => Regex.IsMatch(file.Name, x.IsoFileMatch));
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