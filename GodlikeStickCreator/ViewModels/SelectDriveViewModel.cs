using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.Utilities;
using GodlikeStickCreator.ViewModelBase;
using GodlikeStickCreator.Views;

namespace GodlikeStickCreator.ViewModels
{
    public class SelectDriveViewModel : View
    {
        private ObservableCollection<DriveInfo> _drives;

        private SysLinuxConfigFile _driveSysLinuxConfigFile;
        private bool _formatDrive;
        private bool _formatDriveChangedByUser;
        private DriveInfo _selectedDrive;
        private bool _showAllDrives;
        private bool _windowsInstallationFound;

        public SelectDriveViewModel(UsbStickSettings usbStickSettings) : base(usbStickSettings)
        {
            Drives =
                new ObservableCollection<DriveInfo>(
                    DriveInfo.GetDrives().Where(x => x.IsReady && x.DriveType == DriveType.Removable));
            if (Drives.Count == 0)
                ShowAllDrives = true;
            Application.Current.MainWindow.SourceInitialized += MainWindowOnSourceInitialized;
            CanGoForward = false;
            _formatDrive = usbStickSettings.FormatDrive;
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
                    if (!_formatDriveChangedByUser && value != null && value.IsReady)
                    {
                        _formatDrive = value.DriveFormat != "FAT32";
                        UsbStickSettings.FormatDrive = _formatDrive;
                        OnPropertyChanged(nameof(FormatDrive));
                    }
                    if (value != null)
                    {
                        var sysLinuxFile =
                            new FileInfo(Path.Combine(value.RootDirectory.Root.FullName, "multiboot", "syslinux.cfg"));
                        if (sysLinuxFile.Exists)
                        {
                            SysLinuxConfigFile.TryParse(sysLinuxFile.FullName, out _driveSysLinuxConfigFile);
                            OnPropertyChanged(nameof(DriveSysLinuxConfigFile));
                        }
                        else
                            DriveSysLinuxConfigFile = null;
                    }
                    else
                        DriveSysLinuxConfigFile = null;
                }
            }
        }

        public SysLinuxConfigFile DriveSysLinuxConfigFile
        {
            get { return _driveSysLinuxConfigFile; }
            set { SetProperty(value, ref _driveSysLinuxConfigFile); }
        }

        public bool FormatDrive
        {
            get { return _formatDrive; }
            set
            {
                if (SetProperty(value, ref _formatDrive))
                {
                    UsbStickSettings.FormatDrive = value;
                    _formatDriveChangedByUser = true;
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
                {
                    var currentItem = SelectedDrive;
                    Drives =
                        new ObservableCollection<DriveInfo>(value
                            ? DriveInfo.GetDrives().Where(x => x.IsReady)
                            : DriveInfo.GetDrives().Where(x => x.IsReady && x.DriveType == DriveType.Removable));
                    SelectedDrive = Drives.FirstOrDefault(x => x.Name == currentItem.Name);
                }
            }
        }

        private RelayCommand _manageDriveCommand;

        public RelayCommand ManageDriveCommand
        {
            get
            {
                return _manageDriveCommand ?? (_manageDriveCommand = new RelayCommand(parameter =>
                {
                    if (SelectedDrive == null)
                        return;

                    new ManageWindow(DriveSysLinuxConfigFile, SelectedDrive) {Owner = Application.Current.MainWindow}.ShowDialog();
                }));
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            UsbNotification.UnregisterUsbDeviceNotification();
        }

        private void MainWindowOnSourceInitialized(object sender, EventArgs eventArgs)
        {
            var source = HwndSource.FromHwnd(new WindowInteropHelper(Application.Current.MainWindow).Handle);
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