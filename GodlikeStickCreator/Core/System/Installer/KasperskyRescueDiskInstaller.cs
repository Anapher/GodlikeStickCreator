using System;
using System.IO;
using GodlikeStickCreator.Utilities;
using ICSharpCode.SharpZipLib.Zip;

namespace GodlikeStickCreator.Core.System.Installer
{
    public class KasperskyRescueDiskInstaller : InstallerInfo
    {
        public override InstallMethod InstallMethod { get; } = InstallMethod.KasperskyRescueDisk;

        public override void Install(DirectoryInfo systemDirectory, string systemName, SpecialSnowflake specialSnowflake, string filename, out string menuItem, SystemProgressReporter progressReporter)
        {
            progressReporter.ReportStatus(InstallationStatus.ExtractZipFile);
            using (var zipFilestream = File.OpenRead(filename))
            {
                var fastZip = new FastZip(new FastZipEvents
                {
                    Progress = (sender, args) =>
                    {
                        progressReporter.ReportProgress(args.PercentComplete);
                        progressReporter.SetMessage($"{args.Name} ({args.Processed} / {args.Target})");
                    }
                });
                fastZip.ExtractZip(zipFilestream, systemDirectory.FullName, FastZip.Overwrite.Always, null, null, "-^boot$", false, true);
            }

            progressReporter.ReportStatus(InstallationStatus.WriteConfig);
            
            var sysLinuxDirectory = new DirectoryInfo(Path.Combine(systemDirectory.FullName, "syslinux"));
            sysLinuxDirectory.Create();
            WpfUtilities.WriteResourceToFile(new Uri("pack://application:,,,/Resources/SysLinuxFiles/vesamenu.c32"),
                        Path.Combine(sysLinuxDirectory.FullName, "vesamenu.c32"));
            var configFile =
                string.Format(@"menu title Kaspersky Rescue Disk (Antivirus Scanner) Boot Menu
# menu background background.png
MENU MARGIN 10
MENU VSHIFT 4

MENU color disabled   	1;30;44 #000000 #000000 std
MENU color hotsel       30;47   #C00000 #DDDDDD std
MENU color scrollbar    30;44   #000000 #000000 std
MENU color border       30;44   #D00000 #000000 std
MENU color title        1;36;44 #66A0FF #000000 std
MENU color sel          7;37;40 #000000 #FFFFFF all
MENU color unsel        37;44   #FFFFFF #000000 std
MENU color help         37;40   #FFFFFF #000000 std
MENU color timeout_msg  37;40   #FFFFFF #000000 std
MENU color timeout      1;37;40 #FFFFFF #000000 std
MENU color tabmsg       31;40   #FFFF00 #000000 std
MENU color screen       37;40   #000000 #000000 std
  
default live

label live
  menu label ^Run Kaspersky Rescue Disk from this USB
  kernel /multiboot/{0}/boot/rescue
  append root=live:LABEL=MULTIBOOT live_dir=/multiboot/{0}/rescue/LiveOS/ subdir=/multiboot/{0}/rescue/LiveOS/ rootfstype=auto vga=791 init=/linuxrc loop=/multiboot/{0}/rescue/LiveOS/squashfs.img initrd=/multiboot/{0}/boot/rescue.igz SLUG_lang=en udev liveimg splash quiet doscsi nomodeset
label text
  menu label ^Run Kaspersky Rescue Disk - Text Mode
  kernel /multiboot/{0}/boot/rescue
  append root=live:LABEL=MULTIBOOT live_dir=/multiboot/{0}/rescue/LiveOS/ subdir=/multiboot/{0}/rescue/LiveOS/ rootfstype=auto vga=791 init=/linuxrc loop=/multiboot/{0}/rescue/LiveOS/squashfs.img initrd=/multiboot/{0}/boot/rescue.igz SLUG_lang=en udev liveimg quiet nox SLUGshell noresume doscsi nomodeset
label hwinfo
  menu label ^Run Hardware Info
  kernel /multiboot/{0}/boot/rescue
  append root=live:LABEL=MULTIBOOT live_dir=/multiboot/{0}/rescue/LiveOS/ subdir=/multiboot/{0}/rescue/LiveOS/ rootfstype=auto vga=791 init=/linuxrc loop=/multiboot/{0}/rescue/LiveOS/squashfs.img initrd=/multiboot/{0}/boot/rescue.igz SLUG_lang=en udev liveimg quiet softlevel=boot nox hwinfo noresume doscsi nomodeset 
MENU SEPARATOR  
label <-- Back to Main Menu
 kernel vesamenu.c32
 append /multiboot/syslinux.cfg", sysLinuxDirectory.Name);
            File.WriteAllText(Path.Combine(sysLinuxDirectory.FullName, "syslinux.cfg"), configFile);
            var systemPathWithoutDrive = RemoveDriveFromPath(systemDirectory.FullName);
            var bootInfo = GetSystemBootInfo(systemDirectory.FullName);
            var isoName = Path.GetFileName(filename);
            menuItem =
                $@"#start {isoName}
LABEL Kaspersky Rescue Disk
MENU LABEL Kaspersky Rescue Disk
CONFIG {Path.Combine
                    (systemPathWithoutDrive, bootInfo.ConfigPath, bootInfo.ConfigFile)}
APPEND {Path.Combine(
                        systemPathWithoutDrive, bootInfo.ConfigPath)}
#end {isoName}";

        }
    }
}