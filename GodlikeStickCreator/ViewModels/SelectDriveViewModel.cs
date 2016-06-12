using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.Utilities;

namespace GodlikeStickCreator.ViewModels
{
    public class SelectDriveViewModel : View
    {
        private ObservableCollection<DriveInfo> _drives;
        private DriveInfo _selectedDrive;
        private bool _showAllDrives;
        private bool _windowsInstallationFound;

        public SelectDriveViewModel(UsbStickSettings usbStickSettings) : base(usbStickSettings)
        {
            Drives =
                new ObservableCollection<DriveInfo>(DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Removable));
            if (Drives.Count == 0)
                ShowAllDrives = true;
            Application.Current.MainWindow.SourceInitialized += MainWindowOnSourceInitialized;
            CanGoForward = false;
        }

        public ObservableCollection<DriveInfo> Drives
        {
            get { return _drives; }
            set { SetProperty(value, ref _drives); }
        }

        public DriveInfo SelectedDrive
        {
            get { return _selectedDrive; }
            set
            {
                if (SetProperty(value, ref _selectedDrive))
                {
                    if (value != null &&
                        Directory.Exists(Path.Combine(value.RootDirectory.FullName, "Windows", "system32")))
                    {
                        WindowsInstallationFound = true;
                        CanGoForward = false;
                        return;
                    }

                    CanGoForward = value != null;
                    WindowsInstallationFound = false;
                    UsbStickSettings.Drive = value;
                }
            }
        }

        public bool WindowsInstallationFound
        {
            get { return _windowsInstallationFound; }
            set { SetProperty(value, ref _windowsInstallationFound); }
        }

        public bool ShowAllDrives
        {
            get { return _showAllDrives; }
            set
            {
                if (SetProperty(value, ref _showAllDrives))
                    Drives =
                        new ObservableCollection<DriveInfo>(value
                            ? DriveInfo.GetDrives()
                            : DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Removable));
            }
        }

        private void MainWindowOnSourceInitialized(object sender, EventArgs eventArgs)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(Application.Current.MainWindow).Handle);
            if (source != null)
            {
                var windowHandle = source.Handle;
                source.AddHook(Hook);
                UsbNotification.RegisterUsbDeviceNotification(windowHandle);
            }
        }

        private IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == UsbNotification.WmDevicechange)
            {
                switch ((int) wParam)
                {
                    case UsbNotification.DbtDeviceremovecomplete:
                    case UsbNotification.DbtDevicearrival:
                        var selectedDrive = SelectedDrive;
                        Drives =
                            new ObservableCollection<DriveInfo>(ShowAllDrives
                                ? DriveInfo.GetDrives()
                                : DriveInfo.GetDrives().Where(x => x.DriveType == DriveType.Removable));
                        SelectedDrive = selectedDrive;
                        break;
                }
            }

            handled = false;
            return IntPtr.Zero;
        }
    }
}