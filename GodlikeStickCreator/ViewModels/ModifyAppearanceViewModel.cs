using GodlikeStickCreator.Core;
using GodlikeStickCreator.Core.Config;

namespace GodlikeStickCreator.ViewModels
{
    public class ModifyAppearanceViewModel : View
    {
        public ModifyAppearanceViewModel(UsbStickSettings usbStickSettings) : base(usbStickSettings)
        {
            SysLinuxAppearance = usbStickSettings.SysLinuxAppearance;
        }

        public SysLinuxAppearance SysLinuxAppearance { get; }
    }
}