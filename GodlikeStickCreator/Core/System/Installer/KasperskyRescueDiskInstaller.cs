using System.IO;
using SevenZip;

namespace GodlikeStickCreator.Core.System.Installer
{
    public class KasperskyRescueDiskInstaller : InstallerInfo
    {
        public override InstallMethod InstallMethod { get; } = InstallMethod.KasperskyRescueDisk;

        public override void Install(DirectoryInfo systemDirectory, string systemName, SpecialSnowflake specialSnowflake, string filename, out MenuItemInfo menuItem, SystemProgressReporter progressReporter)
        {
            progressReporter.ReportStatus(InstallationStatus.ExtractFile);

            using (var file = new SevenZipExtractor(filename))
            {
                file.Extracting += (sender, args) => { progressReporter.ReportProgress(args.PercentDone / 100d); };
                file.ExtractArchive(systemDirectory.FullName);
            }

            menuItem = new MenuItemInfo($@"MENU BEGIN Kaspersky Rescue Disk
    LABEL LIVE
    MENU LABEL ^Run Kaspersky Rescue Disk from this USB
    kernel /multiboot/{systemDirectory.Name}/boot/rescue
    append root=live:LABEL={App.DriveLabel} live_dir=/multiboot/{systemDirectory.Name}/rescue/LiveOS/ subdir=/multiboot/{systemDirectory.Name}/rescue/LiveOS/ rootfstype=auto vga=791 init=/linuxrc loop=/multiboot/{systemDirectory.Name}/rescue/LiveOS/squashfs.img initrd=/multiboot/{systemDirectory.Name}/boot/rescue.igz kav_rescue_10_lang=en udev liveimg splash quiet doscsi nomodeset

    label text
    menu label ^Run Kaspersky Rescue Disk - Text Mode
    kernel /multiboot/{systemDirectory.Name}/boot/rescue
    append root=live:LABEL={App.DriveLabel} live_dir=/multiboot/{systemDirectory.Name}/rescue/LiveOS/ subdir=/multiboot/{systemDirectory.Name}/rescue/LiveOS/ rootfstype=auto vga=791 init=/linuxrc loop=/multiboot/{systemDirectory.Name}/rescue/LiveOS/squashfs.img initrd=/multiboot/{systemDirectory.Name}/boot/rescue.igz kav_rescue_10_lang=en udev liveimg quiet nox kav_rescue_10shell noresume doscsi nomodeset

    label hwinfo
    menu label ^Run Hardware Info
    kernel /multiboot/{systemDirectory.Name}/boot/rescue
    append root=live:LABEL={App.DriveLabel} live_dir=/multiboot/{systemDirectory.Name}/rescue/LiveOS/ subdir=/multiboot/{systemDirectory.Name}/rescue/LiveOS/ rootfstype=auto vga=791 init=/linuxrc loop=/multiboot/{systemDirectory.Name}/rescue/LiveOS/squashfs.img initrd=/multiboot/{systemDirectory.Name}/boot/rescue.igz kav_rescue_10_lang=en udev liveimg quiet softlevel=boot nox hwinfo noresume doscsi nomodeset

  	MENU SEPARATOR

	LABEL exit_kaspersky
	MENU LABEL Return to Antivirus Tools
	MENU EXIT
MENU END", true);
        }
    }
}