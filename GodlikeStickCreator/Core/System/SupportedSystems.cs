using System;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace GodlikeStickCreator.Core.System
{
    public static class SupportedSystems
    {
        static SupportedSystems()
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
                    IsoFileMatch = new Regex(@"^kali.*\.iso$")
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
                    IsoFileMatch = new Regex(@"^memtest86\+-.*\.zip$")
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
                    IsoFileMatch = new Regex(@"^kav_rescue_10\.iso$")
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
                    IsoFileMatch = new Regex(@"^kodibuntu.*\.iso$")
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
                    IsoFileMatch = new Regex(@"^tails.*\.iso$")
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
                    IsoFileMatch = new Regex(@"^ubuntustudio.*\.iso$")
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
                    IsoFileMatch = new Regex(@"^systemrescuecd.*\.iso$"),
                    SpecialSnowflake = SpecialSnowflake.SystemRescueDisk
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
                    IsoFileMatch = new Regex(@"^redobackup-livecd.*\.iso$")
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
                    IsoFileMatch = new Regex(@"^dban.*\.iso$"),
                    SpecialSnowflake = SpecialSnowflake.IsoLinuxPrompt0
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
                    IsoFileMatch = new Regex(@"^usb.*\.zip$"),
                    SpecialSnowflake = SpecialSnowflake.IsoLinuxPrompt0
                },
                new SystemInfo
                {
                    Name = "Kon-Boot Purchased",
                    Description =
                        "Kon-Boot is one of the best tools around which can log you into Windows without knowing the password.",
                    Category = Category.SystemTools,
                    InstallMethod = InstallMethod.KonBootPurchased,
                    Website = "http://www.piotrbania.com/all/kon-boot/",
                    DownloadUrl = "http://www.piotrbania.com/all/kon-boot/",
                    Thumbnail = new BitmapImage(new Uri("/Resources/Thumbnails/KonBoot.png", UriKind.Relative)),
                    IsoFileMatch = new Regex(@"^kon-boot-installer\.exe$")
                }
            };
        }

        public static SystemInfo[] Systems { get; set; }
    }
}