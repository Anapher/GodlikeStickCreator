using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.Core.Config;
using Xceed.Wpf.Toolkit;

namespace GodlikeStickCreator.ViewModels
{
    public class ModifyAppearanceViewModel : View
    {
        private BitmapImage _backgroundImage;

        public ModifyAppearanceViewModel(UsbStickSettings usbStickSettings) : base(usbStickSettings)
        {
            BootStickConfig = usbStickSettings.BootStickConfig;
            BackgroundImage =
                new BitmapImage(string.IsNullOrEmpty(usbStickSettings.BootStickConfig.ScreenBackground)
                    ? new Uri("/Resources/SysLinuxFiles/background.png", UriKind.Relative)
                    : new Uri(usbStickSettings.BootStickConfig.ScreenBackground, UriKind.RelativeOrAbsolute));

            StandardColors = new ObservableCollection<ColorItem>
            {
                new ColorItem(Color.FromArgb(0, 0, 0, 0), "Transparent"),
                new ColorItem(Color.FromRgb(255, 255, 255), "White"),
                new ColorItem(Color.FromRgb(0, 0, 0), "Black"),
                new ColorItem(Color.FromRgb(52, 152, 219), "Peter River"),
                new ColorItem(Color.FromRgb(41, 128, 185), "Belize Hole"),
                new ColorItem(Color.FromRgb(52, 73, 94), "Wet Asphalt"),
                new ColorItem(Color.FromRgb(46, 204, 113), "Emerald"),
                new ColorItem(Color.FromRgb(22, 160, 133), "Green Sea"),
                new ColorItem(Color.FromRgb(142, 68, 173), "Wisteria"),
                new ColorItem(Color.FromRgb(211, 84, 0), "Pumpkin"),
                new ColorItem(Color.FromRgb(231, 76, 60), "Alizarin"),
                new ColorItem(Color.FromRgb(192, 57, 43), "Pomegranate"),
            };
        }

        public ObservableCollection<ColorItem> StandardColors { get; }

        public BootStickConfig BootStickConfig { get; }

        public BitmapImage BackgroundImage
        {
            get { return _backgroundImage; }
            set { SetProperty(value, ref _backgroundImage); }
        }
    }
}