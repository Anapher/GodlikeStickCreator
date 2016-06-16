using System.IO;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.ViewModels;

namespace GodlikeStickCreator.Views
{
    /// <summary>
    ///     Interaction logic for ManageWindow.xaml
    /// </summary>
    public partial class ManageWindow
    {
        public ManageWindow(SysLinuxConfigFile sysLinuxConfigFile, DriveInfo driveInfo)
        {
            InitializeComponent();
            Title = $"Manage {driveInfo.Name} ({driveInfo.VolumeLabel})";
            DataContext = new ManageViewModel(sysLinuxConfigFile, driveInfo, this);
        }
    }
}