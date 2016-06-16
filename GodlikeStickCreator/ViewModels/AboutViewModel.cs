using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using GodlikeStickCreator.ViewModelBase;

namespace GodlikeStickCreator.ViewModels
{
    public class AboutViewModel : PropertyChangedBase
    {
        private RelayCommand _openHyperlinkCommand;

        public AboutViewModel()
        {
            Components = new List<Component>
            {
                new Component
                {
                    Name = "SharpZipLib",
                    Url = "https://icsharpcode.github.io/SharpZipLib/",
                    Description = "#ziplib (SharpZipLib, formerly NZipLib) is a Zip, GZip, Tar and BZip2 library"
                },
                new Component
                {
                    Name = "MahApps.Metro",
                    Url = "http://mahapps.com/",
                    Description = "A toolkit for creating metro-style WPF applications."
                },
                new Component
                {
                    Name = "Ookii.Dialogs",
                    Url = "http://www.ookii.org/Software/Dialogs/",
                    Description =
                        "Ookii.Dialogs is a class library for .Net applications providing several common dialogs. "
                },
                new Component
                {
                    Name = "Extended WPF Toolkit™",
                    Url = "https://wpftoolkit.codeplex.com/",
                    Description =
                        "Extended WPF Toolkit™ is the number one collection of WPF controls, components and utilities for creating next generation Windows applications."
                },
                new Component
                {
                    Name = "SevenZipSharp",
                    Url = "https://sevenzipsharp.codeplex.com/",
                    Description =
                        "Managed 7-zip library written in C# that provides data (self-)extraction and compression"
                },
                new Component
                {
                    Name = "7-Zip",
                    Url = "http://www.7-zip.org/",
                    Description = "7-Zip is a file archiver with a high compression ratio."
                },
                new Component
                {
                    Name = "SysLinux",
                    Url = "http://www.syslinux.org/",
                    Description = "The Syslinux Project covers lightweight bootloaders"
                }
            }.OrderBy(x => x.Name).ToList();

            ImageCreators = new List<ImageCreator>
            {
                new ImageCreator
                {
                    Name = "Vecteezy",
                    Website = "http://www.vecteezy.com/",
                    License = "CC BY-SA 3.0",
                    LicenseUrl = "https://creativecommons.org/licenses/by-sa/3.0/"
                },
                new ImageCreator
                {
                    Name = "Designmodo",
                    Website = "http://designmodo.com/",
                    License = "CC 3.0",
                    LicenseUrl = "https://creativecommons.org/licenses/by/3.0/"
                }
            };

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build} (Build {version.Revision})";
        }

        public string CurrentVersion { get; }
        public List<Component> Components { get; }
        public List<ImageCreator> ImageCreators { get; }

        public RelayCommand OpenHyperlinkCommand
        {
            get
            {
                return _openHyperlinkCommand ??
                       (_openHyperlinkCommand = new RelayCommand(parameter => { Process.Start((string) parameter); }));
            }
        }
    }

    public class Component
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }

    public class ImageCreator
    {
        public string License { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string LicenseUrl { get; set; }
    }
}