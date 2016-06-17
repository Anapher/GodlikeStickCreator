using System.Collections.Generic;
using System.IO;
using System.Linq;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.Core.Applications;
using GodlikeStickCreator.Core.System;

namespace GodlikeStickCreator.ViewModels
{
    public class SucceededViewModel : View
    {
        public SucceededViewModel(UsbStickSettings usbStickSettings) : base(usbStickSettings)
        {
            InstalledSystems = usbStickSettings.Systems;
            InstalledApplications = usbStickSettings.ApplicationInfo.Where(x => x.Add).ToList();
            DriveInfo = usbStickSettings.Drive;
        }

        public List<SystemInfo> InstalledSystems { get; }
        public List<ApplicationInfo> InstalledApplications { get; }
        public DriveInfo DriveInfo { get; }
    }
}