using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.Core.Applications;
using GodlikeStickCreator.Utilities;
using GodlikeStickCreator.ViewModelBase;

namespace GodlikeStickCreator.ViewModels
{
    public class ApplicationsViewModel : View
    {
        private RelayCommand _deselectAllCommand;
        private RelayCommand _selectAllCommand;
        private RelayCommand _selectMissingCommand;

        public ApplicationsViewModel(UsbStickSettings usbStickSettings) : base(usbStickSettings)
        {
            Applications = new List<ApplicationInfo>(new[]
            {
                new ApplicationInfo
                {
                    Name = "ILSpy",
                    DownloadUrl =
                        new Lazy<string>(
                            () => GetNewestReleaseDownloadUrl("https://github.com/icsharpcode/ILSpy/releases")),
                    ApplicationCategory = ApplicationCategory.Decompiler,
                    Description = "ILSpy is the open-source .NET assembly browser and decompiler."
                },
                new ApplicationInfo
                {
                    Name = "FFmpeg 32 Bit",
                    DownloadUrl = new Lazy<string>(() => GetFFmpegDownloadUrl(true)),
                    ApplicationCategory = ApplicationCategory.Multimedia,
                    Description =
                        "FFmpeg is a complete, cross-platform solution to record, convert and stream audio and video."
                },
                new ApplicationInfo
                {
                    Name = "FFmpeg 64 Bit",
                    DownloadUrl = new Lazy<string>(() => GetFFmpegDownloadUrl(false)),
                    ApplicationCategory = ApplicationCategory.Multimedia,
                    Description =
                        "FFmpeg is a complete, cross-platform solution to record, convert and stream audio and video."
                },
                new ApplicationInfo
                {
                    Name = "Process Explorer",
                    DownloadUrl = new Lazy<string>(() => "https://download.sysinternals.com/files/ProcessExplorer.zip"),
                    ApplicationCategory = ApplicationCategory.SystemTools,
                    Description =
                        "Process Explorer is a freeware task manager and system monitor."
                },
                new ApplicationInfo
                {
                    Name = "Autoruns",
                    DownloadUrl = new Lazy<string>(() => "https://download.sysinternals.com/files/Autoruns.zip"),
                    ApplicationCategory = ApplicationCategory.SystemTools,
                    Description =
                        "Autoruns is an app that shows you what apps are configured to run during your system bootup or login."
                },
                new ApplicationInfo
                {
                    Name = "HxD",
                    DownloadUrl = new Lazy<string>(() => "http://mh-nexus.de/downloads/HxDen.zip"),
                    ApplicationCategory = ApplicationCategory.Editors,
                    Description =
                        "HxD is a carefully designed and fast hex editor which, additionally to raw disk editing and modifying of main memory (RAM)"
                },
                new ApplicationInfo
                {
                    Name = "Notepad++",
                    DownloadUrl = new Lazy<string>(GetNotepadPlusPlusDownloadLink),
                    ApplicationCategory = ApplicationCategory.Editors,
                    Description =
                        "Notepad++ is a free source code editor and Notepad replacement that supports several languages."
                },
                new ApplicationInfo
                {
                    Name = "MultiHasher",
                    DownloadUrl = new Lazy<string>(() => "http://hostsman.it-mate.co.uk/MultiHasher_2.8.2_win.zip"),
                    ApplicationCategory = ApplicationCategory.FileTools,
                    Description = "MultiHasher is a freeware file hash calculator."
                },
                new ApplicationInfo
                {
                    Name = "PeaZip",
                    DownloadUrl =
                        new Lazy<string>(
                            () =>
                                "http://liquidtelecom.dl.sourceforge.net/project/peazip/6.0.2/peazip_portable-6.0.2.WINDOWS.zip"),
                    ApplicationCategory = ApplicationCategory.FileTools,
                    Description =
                        "PeaZip is free file archiver utility, based on Open Source technologies of 7-Zip, p7zip, FreeArc, PAQ, and PEA projects."
                },
                new ApplicationInfo
                {
                    Name = "PEStudio",
                    DownloadUrl = new Lazy<string>(() => "https://www.winitor.com/tools/pestudio/current/pestudio.zip"),
                    ApplicationCategory = ApplicationCategory.FileTools,
                    Description =
                        "pestudio is a tool that is used in many Cyber Emergency Response Teams worldwide in order to perform malware initial assessment."
                },
                new ApplicationInfo
                {
                    Name = "CPU-Z",
                    DownloadUrl = new Lazy<string>(() => "http://download.cpuid.com/cpu-z/cpu-z_1.76-en.zip"),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Description =
                        "CPU-Z is a freeware that gathers information on some of the main devices of your system."
                },
                new ApplicationInfo
                {
                    Name = "LaZagne",
                    DownloadUrl =
                        new Lazy<string>(
                            () => GetNewestReleaseDownloadUrl("https://github.com/AlessandroZ/LaZagne/releases")),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Description =
                        "The LaZagne project is used to retrieve lots of passwords stored on a local computer from about 22 programs."
                },
                new ApplicationInfo
                {
                    Name = "NotMyFault",
                    DownloadUrl = new Lazy<string>(() => "https://live.sysinternals.com/files/NotMyFault.zip"),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Description =
                        "The NotMyFault tool is a great way to crash Windows systems in a controlled manner to test various tools and analyze crashes."
                },
                new ApplicationInfo
                {
                    Name = "Prime95 32 Bit",
                    DownloadUrl = new Lazy<string>(() => "http://www.mersenne.org/ftp_root/gimps/p95v289.win32.zip"),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Description =
                        "Prime95 is a small and easy to use application that allows you to find Mersenne Prime numbers designed for overclockers."
                },
                new ApplicationInfo
                {
                    Name = "Prime95 64 Bit",
                    DownloadUrl = new Lazy<string>(() => "http://www.mersenne.org/ftp_root/gimps/p95v289.win64.zip"),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Description =
                        "Prime95 is a small and easy to use application that allows you to find Mersenne Prime numbers designed for overclockers."
                },
                new ApplicationInfo
                {
                    Name = "CoreTemp 32 Bit",
                    DownloadUrl = new Lazy<string>(() => "http://www.alcpu.com/CoreTemp/php/download.php?id=2"),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Extension = "zip",
                    Description =
                        "Core Temp is a compact, no fuss, small footprint, yet powerful program to monitor processor temperature and other vital information."
                },
                new ApplicationInfo
                {
                    Name = "CoreTemp 64 Bit",
                    DownloadUrl = new Lazy<string>(() => "http://www.alcpu.com/CoreTemp/php/download.php?id=3"),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Extension = "zip",
                    Description =
                        "Core Temp is a compact, no fuss, small footprint, yet powerful program to monitor processor temperature and other vital information."
                },
                new ApplicationInfo
                {
                    Name = "ImgBurn",
                    DownloadUrl =
                        new Lazy<string>(() => "http://filessjc01.dddload.net/static/Portable_ImgBurn_2.5.8.0.exe"),
                    ApplicationCategory = ApplicationCategory.Multimedia,
                    Extension = "7z",
                    Description =
                        "ImgBurn is a lightweight CD / DVD / HD DVD / Blu-ray burning application that everyone should have in their toolkit!"
                },
                new ApplicationInfo
                {
                    Name = "FileZilla",
                    DownloadUrl =
                        new Lazy<string>(
                            () =>
                                "http://vorboss.dl.sourceforge.net/project/filezilla/FileZilla_Client/3.18.0/FileZilla_3.18.0_win32.zip"),
                    ApplicationCategory = ApplicationCategory.Network,
                    Description = "FileZilla is a cross-platform graphical FTP, SFTP, and FTPS file management tool."
                },
                new ApplicationInfo
                {
                    Name = "Putty",
                    DownloadUrl = new Lazy<string>(() => "https://the.earth.li/~sgtatham/putty/latest/x86/putty.zip"),
                    ApplicationCategory = ApplicationCategory.Network,
                    Description = "PuTTY is a free implementation of SSH and Telnet."
                },
                new ApplicationInfo
                {
                    Name = "WinSCP",
                    DownloadUrl = new Lazy<string>(GetWinSCPDownloadUrl),
                    ApplicationCategory = ApplicationCategory.Network,
                    Description = "WinSCP is an open source free SFTP client and FTP client for Windows."
                },
                new ApplicationInfo
                {
                    Name = "PsTools",
                    DownloadUrl = new Lazy<string>(() => "https://download.sysinternals.com/files/PSTools.zip"),
                    ApplicationCategory = ApplicationCategory.SystemTools,
                    Description =
                        "The PsTools suite includes command-line utilities that help you administer your Windows NT/2K systems."
                },
                new ApplicationInfo
                {
                    Name = "SharpDevelop",
                    DownloadUrl =
                        new Lazy<string>(
                            () =>
                                "http://netix.dl.sourceforge.net/project/sharpdevelop/SharpDevelop%205.x/5.1/SharpDevelop_5.1.0.5216_Xcopyable.zip"),
                    ApplicationCategory = ApplicationCategory.Editors,
                    Description =
                        "#develop (short for SharpDevelop) is a free IDE for C# projects on Microsoft's .NET platform"
                },
                new ApplicationInfo
                {
                    Name = "TrueCrypt 7.1a",
                    DownloadUrl =
                        new Lazy<string>(GetTrueCryptUrl),
                    ApplicationCategory = ApplicationCategory.FileTools,
                    Description =
                        "TrueCrypt creates encrypted volumes on your computer, or encrypts entire disks - including your system disk"
                },
                new ApplicationInfo
                {
                    Name = "Process Monitor",
                    DownloadUrl =
                        new Lazy<string>(() => "https://download.sysinternals.com/files/ProcessMonitor.zip"),
                    ApplicationCategory = ApplicationCategory.SystemTools,
                    Description =
                        "Process Monitor is an advanced monitoring tool for Windows that shows real-time file system, Registry and process/thread activity"
                },
                new ApplicationInfo
                {
                    Name = "Sysinternals Suite",
                    DownloadUrl =
                        new Lazy<string>(() => "https://download.sysinternals.com/files/SysinternalsSuite.zip"),
                    ApplicationCategory = ApplicationCategory.SystemTools,
                    Description =
                        "This suite contains all Sysinternals Troubleshooting Utilities (Process Explorer, Autoruns, PsTools, ...)"
                }
            }.OrderBy(x => x.Name));
            foreach (var application in Applications)
                application.Add = true;

            CanGoForward = true;
            UsbStickSettings.ApplicationInfo = Applications;
        }

        public List<ApplicationInfo> Applications { get; set; }

        public RelayCommand SelectAllCommand
        {
            get
            {
                return _selectAllCommand ?? (_selectAllCommand = new RelayCommand(parameter =>
                {
                    foreach (var applicationInfo in Applications)
                        applicationInfo.Add = true;
                }));
            }
        }

        public RelayCommand DeselectAllCommand
        {
            get
            {
                return _deselectAllCommand ?? (_deselectAllCommand = new RelayCommand(parameter =>
                {
                    foreach (var applicationInfo in Applications)
                        applicationInfo.Add = false;
                }));
            }
        }

        public RelayCommand SelectMissingCommand
        {
            get
            {
                return _selectMissingCommand ?? (_selectMissingCommand = new RelayCommand(parameter =>
                {
                    foreach (var application in Applications)
                        application.Add =
                            !Directory.Exists(Path.Combine(UsbStickSettings.Drive.RootDirectory.FullName, "Tools",
                                EnumUtilities.GetDescription(application.ApplicationCategory), application.Name));
                }));
            }
        }

        private static string GetNewestReleaseDownloadUrl(string githubReleaseUrl)
        {
            var websiteSource = new WebClient().DownloadString(githubReleaseUrl);
            var url =
                Regex.Match(websiteSource, @"<ul class=""release-downloads"">\s*<li>\s*<a href=""(?<url>(.+?))""")
                    .Groups
                    ["url"].Value;
            return "https://www.github.com" + url;
        }

        private static string GetFFmpegDownloadUrl(bool bit32)
        {
            var source = new WebClient().DownloadString("https://ffmpeg.zeranoe.com/builds/");
            return $"https://ffmpeg.zeranoe.com/builds/win{(bit32 ? "32" : "64")}/static/" +
                   Regex.Match(source,
                       $@"<a class=""latest"" href=""\./win{(bit32 ? "32" : "64")}/static/(?<filename>(.*?))""").Groups[
                           "filename"].Value;
        }

        private static string GetNotepadPlusPlusDownloadLink()
        {
            var source = new WebClient().DownloadString("https://notepad-plus-plus.org/download");
            return "https://notepad-plus-plus.org" +
                   Regex.Match(source, @"<a href=""(?<url>(.*?))"">Notepad\+\+ zip package").Groups["url"].Value;
        }

        /*
        private static string Get7ZipDownloadUrl()
        {
            var source = new WebClient().DownloadString("http://www.7-zip.org/download.html");
            return "http://www.7-zip.org/" +
                   Regex.Match(source, @"<A href=""(?<url>(.*?))-extra\.7z"">Download<\/A>").Groups["url"].Value +
                   "-extra.7z";
        }
        */

        private static string GetWinSCPDownloadUrl()
        {
            using (var webClient = new WebClient())
            {
                var source = webClient.DownloadString("https://winscp.net/eng/download.php");
                source =
                    webClient.DownloadString("https://winscp.net" +
                                             Regex.Match(source, @"<a href=""\.\.(?<downloadUrl>(.*?))\.zip""").Groups[
                                                 "downloadUrl"].Value + ".zip");
                return Regex.Match(source, @"href='(?<url>(.*?))'>\[Direct download\]<\/a>").Groups["url"].Value;
            }
        }

        private static string GetTrueCryptUrl()
        {
            var source =
                new WebClient().DownloadString(
                    "http://www.heise.de/download/product/truecrypt-25104/download/danke?id=25104-5");
            return Regex.Match(source, @"<a href=""(?<url>(.*?))truecrypt\.zip""").Groups["url"].Value + "truecrypt.zip";
        }
    }
}