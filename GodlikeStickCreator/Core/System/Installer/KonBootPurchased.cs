﻿using System.IO;

namespace GodlikeStickCreator.Core.System.Installer
{
    public class KonBootPurchased : InstallerInfo
    {
        public override InstallMethod InstallMethod { get; } = InstallMethod.KonBootPurchased;

        public override void Install(DirectoryInfo systemDirectory, string systemName, SpecialSnowflake specialSnowflake,
            string filename, out MenuItemInfo menuItem, SystemProgressReporter progressReporter)
        {
            progressReporter.ReportStatus(InstallationStatus.CopyFiles);
            var fileDirectory =
                new DirectoryInfo(Path.Combine(Path.GetDirectoryName(filename), "kon-bootUSB", "USBFILES"));
            if (!fileDirectory.Exists)
                throw new FileNotFoundException($"Could not find kon boot USB directory: \"{fileDirectory.FullName}\"");

            File.Copy(Path.Combine(fileDirectory.FullName, "grldr"), Path.Combine(systemDirectory.FullName, "grldr"));
            progressReporter.ReportProgress(.4);
            File.Copy(Path.Combine(fileDirectory.FullName, "konboot.img"),
                Path.Combine(systemDirectory.FullName, "konboot.img"));
            progressReporter.ReportProgress(.8);
            File.Copy(Path.Combine(fileDirectory.FullName, "konbootOLD.img"),
                Path.Combine(systemDirectory.FullName, "konbootOLD.img"));
            progressReporter.ReportProgress(1);

            progressReporter.ReportStatus(InstallationStatus.WriteConfig);
            var pathWithoutDrive = RemoveDriveFromPath(systemDirectory.FullName).Replace("\\", "/");

            var konbootVersionSelectionFile =
                $@"title Kon-Boot (CURRENT VERSION)
map --mem {pathWithoutDrive}/konboot.img (fd0)
map --hook
chainloader (fd0)+1
map (hd1) (hd0)
map --hook
rootnoverify (fd0)
title Kon-Boot v2.1 (OLD VERSION)
map --mem {pathWithoutDrive}/konbootOLD.img (fd0)
map --hook
chainloader (fd0)+1
map (hd1) (hd0)
map --hook
rootnoverify (fd0)";

            File.WriteAllText(Path.Combine(systemDirectory.FullName, "konboot.lst"), konbootVersionSelectionFile);

            menuItem =
                new MenuItemInfo($@"KERNEL /multiboot/grub.exe
APPEND --config-file={pathWithoutDrive}/konboot.lst");
        }
    }
}