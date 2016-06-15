using System.Collections.Generic;
using System.IO;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.Core.System;

namespace GodlikeStickCreator.ViewModels
{
    public class SucceededViewModel : View
    {
        public SucceededViewModel(UsbStickSettings usbStickSettings) : base(usbStickSettings)
        {
            InstalledSystems = usbStickSettings.Systems;
            DriveInfo = usbStickSettings.Drive;
        }

        public List<SystemInfo> InstalledSystems { get; }
        public DriveInfo DriveInfo { get; }
    }
}