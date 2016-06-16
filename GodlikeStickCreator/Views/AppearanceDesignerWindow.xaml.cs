using GodlikeStickCreator.Core.Config;

namespace GodlikeStickCreator.Views
{
    /// <summary>
    ///     Interaction logic for AppearanceDesignerWindow.xaml
    /// </summary>
    public partial class AppearanceDesignerWindow
    {
        public AppearanceDesignerWindow(SysLinuxAppearance sysLinuxAppearance)
        {
            SysLinuxAppearance = sysLinuxAppearance;
            InitializeComponent();
        }

        public SysLinuxAppearance SysLinuxAppearance { get; }
    }
}