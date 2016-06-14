using System.Collections.Generic;
using System.IO;
using GodlikeStickCreator.Core.Applications;
using GodlikeStickCreator.Core.Config;
using GodlikeStickCreator.Core.System;

namespace GodlikeStickCreator.Core
{
    public class UsbStickSettings
    {
        public UsbStickSettings()
        {
            BootStickConfig = new BootStickConfig();
        }

        public DriveInfo Drive { get; set; }
        public List<SystemInfo> Systems { get; set; }
        public List<ApplicationInfo> ApplicationInfo { get; set; }
        public BootStickConfig BootStickConfig { get; set; }
        public bool FormatDrive { get; set; }
    }
}