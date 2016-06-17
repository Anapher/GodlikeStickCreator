using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GodlikeStickCreator.Annotations;
using GodlikeStickCreator.Core.Config;
using GodlikeStickCreator.ViewModelBase;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;

namespace GodlikeStickCreator.Controls
{
    /// <summary>
    ///     Interaction logic for ChangeSysLinuxInterfaceControl.xaml
    /// </summary>
    public partial class ChangeSysLinuxInterfaceControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty SysLinuxAppearanceProperty = DependencyProperty.Register(
            "SysLinuxAppearance", typeof (SysLinuxAppearance), typeof (ChangeSysLinuxInterfaceControl),
            new PropertyMetadata(default(SysLinuxAppearance), PropertyChangedCallback));

        private BitmapImage _backgroundImage;

        private RelayCommand _changeBackgroundImageCommand;

        public ChangeSysLinuxInterfaceControl()
        {
            InitializeComponent();
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
                new ColorItem(Color.FromRgb(192, 57, 43), "Pomegranate")
            };
        }

        public BitmapImage BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                if (value != _backgroundImage)
                {
                    _backgroundImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ColorItem> StandardColors { get; }

        public SysLinuxAppearance SysLinuxAppearance
        {
            get { return (SysLinuxAppearance) GetValue(SysLinuxAppearanceProperty); }
            set { SetValue(SysLinuxAppearanceProperty, value); }
        }

        public RelayCommand ChangeBackgroundImageCommand
        {
            get
            {
                return _changeBackgroundImageCommand ?? (_changeBackgroundImageCommand = new RelayCommand(parameter =>
                {
                    var ofd = new OpenFileDialog {Filter = "Image files|*.png"};
                    if (ofd.ShowDialog(Application.Current.MainWindow) == true)
                    {
                        BackgroundImage = new BitmapImage(new Uri(ofd.FileName, UriKind.RelativeOrAbsolute));
                        SysLinuxAppearance.ScreenBackground = ofd.FileName;
                        OnPropertyChanged(nameof(SysLinuxAppearance));
                    }
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static void PropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (ChangeSysLinuxInterfaceControl) dependencyObject;
            var sysLinuxAppearance = dependencyPropertyChangedEventArgs.NewValue as SysLinuxAppearance;
            control.BackgroundImage = string.IsNullOrEmpty(sysLinuxAppearance?.ScreenBackground)
                ? new BitmapImage(new Uri("/Resources/SysLinuxFiles/background.png", UriKind.Relative))
                : LoadBitmapImageIntoMemory(new Uri(sysLinuxAppearance.ScreenBackground, UriKind.RelativeOrAbsolute));
        }

        private static BitmapImage LoadBitmapImageIntoMemory(Uri uri)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.UriSource = uri;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}