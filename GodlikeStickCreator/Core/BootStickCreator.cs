using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private readonly DriveInfo _drive;
        private readonly BootStickConfig _bootStickConfig;
        private const string DriveDirectory = "multiboot";
        private readonly Dictionary<InstallMethod, InstallerInfo> _installer;

        public BootStickCreator(DriveInfo drive, BootStickConfig bootStickConfig)
        {
            _drive = drive;
            _bootStickConfig = bootStickConfig;

            _installer = new List<InstallerInfo>
            {
                new DefaultInstaller(),
                new KasperskyRescueDiskInstaller(),
                new KonBootPurchased(),
                new Memtest86()
            }.ToDictionary(x => x.InstallMethod, x => x);
        }

        public void AddSystemToBootStick(SystemInfo systemInfo, Logger logger, SystemProgressReporter systemProgressReporter)
        {
            var configFile = GetConfigFileFromCategory(systemInfo.Category);

            logger.Status("Check config files");
            if (!configFile.Exists)
            {
                var categoryName = EnumUtilities.GetDescription(systemInfo.Category);
                WriteCategoryConfigFile(configFile.FullName, categoryName, _bootStickConfig);
                AppendCategoryToSysFile(new DirectoryInfo(Path.Combine(_drive.RootDirectory.FullName, DriveDirectory)),
                    categoryName, configFile.Name);
                logger.Success("Config file created");
            }
            else
                logger.Status("Config file is already existing");

            var systemDirectory =
                new DirectoryInfo(Path.Combine(_drive.RootDirectory.FullName, DriveDirectory, Path.GetFileName(systemInfo.Filename)));
            if (systemDirectory.Exists)
                throw new Exception("System already exists");

            logger.Status($"Create directory \"{systemDirectory.FullName}\"");
            systemDirectory.Create();

            var configDirectory = new DirectoryInfo(Path.Combine(systemDirectory.FullName, "GODLIKE"));
            logger.Status($"Create directory \"{configDirectory.FullName}\"");
            configDirectory.Create();

            var installer = _installer[systemInfo.InstallMethod];
            logger.Status($"Install method \"{installer.InstallMethod}\" selected");
            string menuItem;
            installer.Install(systemDirectory, systemInfo.Name, systemInfo.SpecialSnowflake, systemInfo.Filename, out menuItem, systemProgressReporter);
            
            File.AppendAllText(configFile.FullName, "\r\n" + menuItem + "\r\n");
        }

        private FileInfo GetConfigFileFromCategory(Category category)
        {
            switch (category)
            {
                case Category.LinuxDistributions:
                    return new FileInfo(Path.Combine(_drive.RootDirectory.FullName, DriveDirectory, "menu", "linux.cfg"));
                case Category.SystemTools:
                    return
                        new FileInfo(Path.Combine(_drive.RootDirectory.FullName, DriveDirectory, "menu", "system.cfg"));
                case Category.AntiVirus:
                    return
                        new FileInfo(Path.Combine(_drive.RootDirectory.FullName, DriveDirectory, "menu", "antivirus.cfg"));
                case Category.Multimedia:
                    return new FileInfo(Path.Combine(_drive.RootDirectory.FullName, DriveDirectory, "menu", "multimedia.cfg"));
                case Category.Other:
                    return new FileInfo(Path.Combine(_drive.RootDirectory.FullName, DriveDirectory, "menu", "other.cfg"));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void CreateBootStick(Logger logger)
        {
            var tempDirectory = FileExtensions.MakeDirectoryUnique(Path.Combine(Path.GetTempPath(), "GodlikeStick"));
            logger.Status("Create temp directory");
            tempDirectory.Create();
            try
            {
                var targetDirectory = new DirectoryInfo(Path.Combine(_drive.RootDirectory.FullName, DriveDirectory));
                targetDirectory.Create();

                logger.Status($"Create /{DriveDirectory} directory on drive");

                if (!File.Exists(Path.Combine(targetDirectory.FullName, "libcom32.c32")))
                    InstallSysLinux(_drive, tempDirectory, logger);

                logger.Status("Write syslinux config");
                WriteSysConfigFile(targetDirectory, _bootStickConfig);

                logger.Status("Copy background.png");
                var backgroundFilePath = Path.Combine(targetDirectory.FullName, "background.png");
                if (!string.IsNullOrEmpty(_bootStickConfig.ScreenBackground) && File.Exists(_bootStickConfig.ScreenBackground))
                    File.Copy(_bootStickConfig.ScreenBackground, backgroundFilePath);
                else
                    WpfUtilities.WriteResourceToFile(new Uri("pack://application:,,,/Resources/SysLinuxFiles/background.png"), backgroundFilePath);

                var randomFiles = new[] {"chain.c32", "libcom32.c32", "libutil.c32", "memdisk", "menu.c32", "vesamenu.c32"};
                var menuDirectory = new DirectoryInfo(Path.Combine(targetDirectory.FullName, "menu"));
                menuDirectory.Create();
                foreach (var randomFile in randomFiles)
                {
                    logger.Status($"Write \"{Path.Combine(targetDirectory.FullName, randomFile)}\"");
                    WpfUtilities.WriteResourceToFile(
                        new Uri($"pack://application:,,,/Resources/SysLinuxFiles/{randomFile}"),
                        Path.Combine(targetDirectory.FullName, randomFile));
                    logger.Status($"Write \"{Path.Combine(menuDirectory.FullName, randomFile)}\"");
                    WpfUtilities.WriteResourceToFile(
                        new Uri($"pack://application:,,,/Resources/SysLinuxFiles/{randomFile}"),
                        Path.Combine(menuDirectory.FullName, randomFile));
                }

                logger.Status($"Write \"{Path.Combine(targetDirectory.FullName, "grub.exe")}\"");
                WpfUtilities.WriteResourceToFile(
                        new Uri("pack://application:,,,/Resources/SysLinuxFiles/grub.exe"),
                        Path.Combine(targetDirectory.FullName, "grub.exe"));
            }
            finally
            {
                logger.Status("Delete temp directory");
                tempDirectory.Delete(true);
            }
        }

        private static void WriteCategoryConfigFile(string filename, string categoryName, BootStickConfig bootStickConfig)
        {
            var configString = $@"UI vesamenu.c32
MENU TITLE {categoryName}
# MENU BACKGROUND background.png
MENU TABMSG {bootStickConfig.ScreenMessage}
MENU WIDTH 72
MENU MARGIN 10
MENU VSHIFT 3
MENU HSHIFT 6
MENU ROWS 15
MENU TABMSGROW 20
MENU TIMEOUTROW 22

menu color title 1;36;44 #66A0FF #00000000 none
menu color hotsel 30;47 #2980b9 #DDDDDDDD
menu color sel 30;47 #000000 #FFFFFFFF
menu color border 30;44	#2980b9 #00000000 std
menu color scrollbar 30;44 #DDDDDDDD #00000000 none
  
LABEL < --Back to Main Menu
CONFIG /{DriveDirectory}/syslinux.cfg
MENU SEPARATOR
 ";
            File.WriteAllText(filename, configString);
        }

        private static void WriteSysConfigFile(DirectoryInfo directory, BootStickConfig bootStickConfig)
        {
            var configString = $@"UI vesamenu.c32
TIMEOUT 300
MENU TITLE {bootStickConfig.ScreenTitle}
MENU BACKGROUND background.png
MENU TABMSG  {bootStickConfig.ScreenMessage}
MENU WIDTH 72
MENU MARGIN 10
MENU VSHIFT 3
MENU HSHIFT 6
MENU ROWS 15
MENU TABMSGROW 20
MENU TIMEOUTROW 22
menu color title 1;36;44 #66A0FF #00000000 none
menu color hotsel 30;47 #2980b9 #DDDDDDDD
menu color sel 30;47 #000000 #FFFFFFFF
menu color border 30;44	#2980b9 #00000000 std
menu color scrollbar 30;44 #DDDDDDDD #00000000 none

LABEL Boot from first Hard Drive
MENU LABEL Continue to Boot from ^First HD (default)
KERNEL chain.c32
APPEND hd1
MENU DEFAULT";
            File.WriteAllText(Path.Combine(directory.FullName, "syslinux.cfg"), configString);
        }

        private static void AppendCategoryToSysFile(DirectoryInfo directory, string categoryName, string categoryFile)
        {
            var appendToConfigString = $@"{"\r\n"}label {categoryName}
menu label {categoryName} ->
MENU INDENT 1
CONFIG /{DriveDirectory}/menu/{categoryFile}
";
            File.AppendAllText(Path.Combine(directory.FullName, "syslinux.cfg"), appendToConfigString);
        }

        private static void InstallSysLinux(DriveInfo drive, DirectoryInfo tempDirectory, Logger logger)
        {
            var sysLinuxFile = new FileInfo(Path.Combine(tempDirectory.FullName, "syslinux.exe"));

            var resource = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Utilities/syslinux.exe"));
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

            logger.Status(output);
            logger.Success("syslinux installed successfully");
        }
    }
}