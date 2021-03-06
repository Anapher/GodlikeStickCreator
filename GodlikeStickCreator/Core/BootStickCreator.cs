﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using GodlikeStickCreator.Controls;
using GodlikeStickCreator.Core.Config;
using GodlikeStickCreator.Core.System;
using GodlikeStickCreator.Core.System.Installer;
using GodlikeStickCreator.Utilities;

namespace GodlikeStickCreator.Core
{
    public class BootStickCreator
    {
        private const string DriveDirectory = "multiboot";
        private readonly SysLinuxAppearance _sysLinuxAppearance;
        private readonly DriveInfo _drive;
        private readonly Dictionary<InstallMethod, InstallerInfo> _installer;

        public BootStickCreator(DriveInfo drive, SysLinuxAppearance sysLinuxAppearance)
        {
            _drive = drive;
            _sysLinuxAppearance = sysLinuxAppearance;

            _installer = new List<InstallerInfo>
            {
                new DefaultInstaller(),
                new KasperskyRescueDiskInstaller(),
                new KonBootPurchased(),
                new Memtest86()
            }.ToDictionary(x => x.InstallMethod, x => x);
        }

        public SysLinuxConfigFile SysLinuxConfigFile { get; private set; }

        public void AddSystemToBootStick(SystemInfo systemInfo, Logger logger,
            SystemProgressReporter systemProgressReporter)
        {
            if (SysLinuxConfigFile == null)
                throw new InvalidOperationException();

            var systemDirectory =
                new DirectoryInfo(Path.Combine(_drive.RootDirectory.FullName, DriveDirectory,
                    Path.GetFileNameWithoutExtension(systemInfo.Filename)));
            if (systemDirectory.Exists)
                throw new Exception("System already exists");

            logger.Status($"Create directory \"{systemDirectory.FullName}\"");
            systemDirectory.Create();

            var installer = _installer[systemInfo.InstallMethod];
            logger.Status($"Install method \"{installer.InstallMethod}\" selected");
            MenuItemInfo menuItem;
            installer.Install(systemDirectory, systemInfo.Name, systemInfo.SpecialSnowflake, systemInfo.Filename,
                out menuItem, systemProgressReporter);

            logger.Status("Add to config file");
            SysLinuxConfigFile.AddSystem(systemInfo, menuItem, systemDirectory.Name);
            SysLinuxConfigFile.Save();

            logger.Status("Update syslinux");
            UpdateSysLinux(systemDirectory);
        }

        public void RemoveSystem(SystemInfo systemInfo)
        {
            var directory = SysLinuxConfigFile.GetSystemDirectory(systemInfo);
            SysLinuxConfigFile.RemoveSystem(systemInfo);

            var systemDirectory =
                new DirectoryInfo(Path.Combine(_drive.RootDirectory.FullName, DriveDirectory, directory));

            if (!systemDirectory.Exists)
                return;

            FileSystemExtensions.DeleteDirectory(systemDirectory.FullName);
        }

        public void CreateBootStick(Logger logger)
        {
            var tempDirectory = FileSystemExtensions.MakeDirectoryUnique(Path.Combine(Path.GetTempPath(), "GodlikeStick"));
            logger.Status("Create temp directory");
            tempDirectory.Create();
            try
            {
                var targetDirectory = new DirectoryInfo(Path.Combine(_drive.RootDirectory.FullName, DriveDirectory));

                if (!targetDirectory.Exists)
                {
                    logger.Status($"Create /{DriveDirectory} directory on drive");
                    targetDirectory.Create();
                }

                if (!File.Exists(Path.Combine(targetDirectory.FullName, "libcom32.c32")))
                    InstallSysLinux(_drive, tempDirectory, logger);

                logger.Status("Write syslinux config");
                var sysLinuxFile = new FileInfo(Path.Combine(targetDirectory.FullName, "syslinux.cfg"));
                if (sysLinuxFile.Exists)
                    SysLinuxConfigFile = SysLinuxConfigFile.OpenFile(sysLinuxFile.FullName, _sysLinuxAppearance);
                else
                    SysLinuxConfigFile = SysLinuxConfigFile.Create(sysLinuxFile.FullName, _sysLinuxAppearance);

                logger.Status("Copy background.png");
                var backgroundFilePath = Path.Combine(targetDirectory.FullName, "background.png");
                if (!string.IsNullOrEmpty(_sysLinuxAppearance.ScreenBackground) &&
                    File.Exists(_sysLinuxAppearance.ScreenBackground))
                    File.Copy(_sysLinuxAppearance.ScreenBackground, backgroundFilePath);
                else if (!File.Exists(backgroundFilePath))
                    //only copy if the file does not exist because the user could have already installed syslinux with a different background image
                    WpfUtilities.WriteResourceToFile(
                        new Uri("pack://application:,,,/Resources/SysLinuxFiles/background.png"), backgroundFilePath);

                var randomFiles = new[]
                {"chain.c32", "libcom32.c32", "libutil.c32", "memdisk", "menu.c32", "vesamenu.c32", "grub.exe"};
                foreach (var randomFile in randomFiles)
                {
                    var resource =
                        Application.GetResourceStream(
                            new Uri($"pack://application:,,,/Resources/SysLinuxFiles/{randomFile}"));

                    if (resource == null)
                        throw new FileNotFoundException();

                    var targetFile = Path.Combine(targetDirectory.FullName, randomFile);
                    if (File.Exists(targetFile))
                    {
                        byte[] resourceHash;
                        byte[] fileHash;
                        using (var md5 = new MD5CryptoServiceProvider())
                        {
                            resourceHash = md5.ComputeHash(resource.Stream);
                            using (var fs = new FileStream(targetFile, FileMode.Open, FileAccess.Read))
                                fileHash = md5.ComputeHash(fs);
                        }

                        if (resourceHash.SequenceEqual(fileHash))
                        {
                            logger.Status($"{randomFile} already exists");
                            continue;
                        }
                    }

                    logger.Status($"Write \"{Path.Combine(targetDirectory.FullName, randomFile)}\"");
                    WpfUtilities.WriteResourceToFile(
                        new Uri($"pack://application:,,,/Resources/SysLinuxFiles/{randomFile}"),
                        Path.Combine(targetDirectory.FullName, randomFile));
                }
            }
            finally
            {
                logger.Status("Delete temp directory");
                tempDirectory.Delete(true);
            }
        }

        private static void UpdateSysLinux(DirectoryInfo directoryInfo)
        {
            foreach (var file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
            {
                switch (file.Name)
                {
                    case "chain.c32":
                        break;
                    case "vesamenu.c32":
                        break;
                    case "menu.c32":
                        break;
                    case "libutil.c32":
                        break;
                    case "libcom32.c32":
                        break;
                    case "ifcpu64.c32":
                        break;
                    default:
                        continue;
                }

                file.Delete();
                WpfUtilities.WriteResourceToFile(
                    new Uri($"pack://application:,,,/Resources/SysLinuxFiles/{file.Name}"), file.FullName);
            }
        }

        private static void InstallSysLinux(DriveInfo drive, DirectoryInfo tempDirectory, Logger logger)
        {
            var sysLinuxFile = new FileInfo(Path.Combine(tempDirectory.FullName, "syslinux.exe"));

            var resource =
                Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Utilities/syslinux.exe"));
            if (resource == null)
                throw new FileNotFoundException();

            logger.Status("Extract syslinux.exe");
            using (var fileStream = sysLinuxFile.OpenWrite())
                resource.Stream.CopyTo(fileStream);

            var process = new Process
            {
                StartInfo =
                {
                    FileName = sysLinuxFile.FullName,
                    Arguments = $"-maf -d /multiboot {drive.Name.Substring(0, drive.Name.Length - 1)}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            logger.Status("Run syslinux.exe");
            if (!process.Start())
                throw new Exception("Could not create syslinux process");

            var output = process.StandardOutput.ReadToEnd();
            var errorOutput = process.StandardError.ReadToEnd();

            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new Exception($"Output:\r\n{output}\r\nError:\r\n{errorOutput}");

            logger.Success("syslinux installed successfully");
        }
    }
}