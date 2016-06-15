using System.IO;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.ViewModelBase;

namespace GodlikeStickCreator.ViewModels
{
    public class ManageViewModel : PropertyChangedBase
    {
        public ManageViewModel(SysLinuxConfigFile sysLinuxConfigFile, DriveInfo drive)
        {
            SysLinuxConfigFile = sysLinuxConfigFile;
            Drive = drive;
        }

        public SysLinuxConfigFile SysLinuxConfigFile { get; }
        public DriveInfo Drive { get; }
    }
}