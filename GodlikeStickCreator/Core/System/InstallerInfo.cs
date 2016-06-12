﻿using System.Collections.Generic;
using System.IO;

namespace GodlikeStickCreator.Core.System
{
    public abstract class InstallerInfo
    {
        public abstract InstallMethod InstallMethod { get; }
        public abstract void Install(DirectoryInfo systemDirectory, string systemName, SpecialSnowflake specialSnowflake, string filename, out string menuItem, SystemProgressReporter progressReporter);

        protected SystemBootInfo GetSystemBootInfo(string directory)
        {
            if (File.Exists(Path.Combine(directory, "liberte", "boot", "syslinux", "syslinux.cfg"))) //Liberte

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"liberte/boot/syslinux",
                    CopyPath = @"liberte\boot\syslinux",
                    ConfigFile = @"syslinux.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "boot", "i386", "loader", "isolinux.cfg"))) //OpenSuse based 32bit

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"boot/i386/loader",
                    CopyPath = @"boot\i386\loader",
                    ConfigFile = @"isolinux.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "boot", "x86_64", "loader", "isolinux.cfg"))) //OpenSuse based 32bit

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"boot/x86_64/loader",
                    CopyPath = @"boot\x86_64\loader",
                    ConfigFile = @"isolinux.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "syslinux", "syslinux.cfg")))

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"syslinux",
                    CopyPath = @"syslinux",
                    ConfigFile = @"syslinux.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "isolinux", "isolinux.cfg")))

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"isolinux",
                    CopyPath = @"isolinux",
                    ConfigFile = @"isolinux.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "isolinux", "syslinux.cfg"))) //AVG

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"isolinux",
                    CopyPath = @"isolinux",
                    ConfigFile = @"syslinux.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "isolinux", "txt.cfg"))) //Probably Ubuntu based

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"isolinux",
                    CopyPath = @"isolinux",
                    ConfigFile = @"txt.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "isolinux", "text.cfg"))) //Probably Ubuntu based

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"isolinux",
                    CopyPath = @"isolinux",
                    ConfigFile = @"text.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "syslinux", "txt.cfg")))

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"syslinux",
                    CopyPath = @"syslinux",
                    ConfigFile = @"txt.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "syslinux", "text.cfg")))

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"syslinux",
                    CopyPath = @"syslinux",
                    ConfigFile = @"text.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "system", "isolinux", "isolinux.cfg"))) //AOSS

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"system/isolinux",
                    CopyPath = @"system\isolinux",
                    ConfigFile = @"isolinux.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "isolinux.cfg"))) //Probably Puppy based

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"",
                    CopyPath = @"",
                    ConfigFile = @"isolinux.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "syslinux.cfg")))

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"",
                    CopyPath = @"",
                    ConfigFile = @"syslinux.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "slax", "boot", "syslinux.cfg"))) //Slax based

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"slax/boot",
                    CopyPath = @"slax\boot",
                    ConfigFile = @"syslinux.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "boot", "syslinux", "syslinux.cfg")))

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"boot/syslinux",
                    CopyPath = @"boot\syslinux",
                    ConfigFile = @"syslinux.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "boot", "isolinux", "isolinux.cfg")))

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"boot/isolinux",
                    CopyPath = @"boot\isolinux",
                    ConfigFile = @"isolinux.cfg"
                };
            }

            if (File.Exists(Path.Combine(directory, "boot", "isolinux", "syslinux.cfg")))

            {
                return new SystemBootInfo
                {
                    ConfigPath = @"boot/isolinux",
                    CopyPath = @"boot\isolinux",
                    ConfigFile = @"syslinux.cfg"
                };
            }

            throw new FileNotFoundException();
        }

        protected void ReplaceStringInFile(string filename, IDictionary<string, string> replacementStrings)
        {
            if (!File.Exists(filename))
                return;

            var content = File.ReadAllText(filename);
            foreach (var replacementString in replacementStrings)
            {
                content = content.Replace(replacementString.Key, replacementString.Value);
            }
            File.WriteAllText(filename, content);
        }

        protected string RemoveDriveFromPath(string filename)
        {
            return filename.Substring(2);
        }

        protected class SystemBootInfo
        {
            public string ConfigPath { get; set; }
            public string CopyPath { get; set; }
            public string ConfigFile { get; set; }
        }
    }

    public enum InstallMethod
    {
        KasperskyRescueDisk,
        Memtest86,
        KonBootPurchased,
        Other
    }

    public enum SpecialSnowflake
    {
        None,
        SystemRescueDisk
    }
}