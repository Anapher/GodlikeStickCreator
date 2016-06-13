using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.Core.Applications;
using GodlikeStickCreator.ViewModelBase;

namespace GodlikeStickCreator.ViewModels
{
    public class ApplicationsViewModel : View
    {
        private RelayCommand _deselectAllCommand;
        private RelayCommand _selectAllCommand;

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
                        "HxD is a carefully designed and fast hex editor which, additionally to raw disk editing and modifying of main memory (RAM), handles files of any size."
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
                    Name = "7-Zip (console)",
                    DownloadUrl = new Lazy<string>(Get7ZipDownloadUrl),
                    ApplicationCategory = ApplicationCategory.FileTools,
                    Description = "7-Zip is a file archiver with a high compression ratio."
                }
            }.OrderBy(x => x.Name));

            foreach (var applicationInfo in Applications)
                applicationInfo.Add = true;

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

        private static string Get7ZipDownloadUrl()
        {
            var source = new WebClient().DownloadString("http://www.7-zip.org/download.html");
            return "http://www.7-zip.org/" +
                   Regex.Match(source, @"<A href=""(?<url>(.*?))-extra\.7z"">Download<\/A>").Groups["url"].Value +
                   "-extra.7z";
        }
    }
}