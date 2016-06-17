using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.Utilities;
using GodlikeStickCreator.ViewModelBase;
using GodlikeStickCreator.Views;

namespace GodlikeStickCreator.ViewModels
{
    public class ManageViewModel : PropertyChangedBase
    {
        private readonly Window _window;
        private RelayCommand _openSysLinuxDesignerCommand;
        private RelayCommand _removeSystemCommand;

        public ManageViewModel(SysLinuxConfigFile sysLinuxConfigFile, DriveInfo drive, Window window)
        {
            _window = window;
            SysLinuxConfigFile = sysLinuxConfigFile;
            Drive = drive;
        }

        public SysLinuxConfigFile SysLinuxConfigFile { get; private set; }
        public DriveInfo Drive { get; }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(value, ref _isLoading); }
        }

        public RelayCommand OpenSysLinuxDesignerCommand
        {
            get
            {
                return _openSysLinuxDesignerCommand ?? (_openSysLinuxDesignerCommand = new RelayCommand(parameter =>
                {
                    var window = new AppearanceDesignerWindow(SysLinuxConfigFile.SysLinuxAppearance)
                    {
                        Owner = _window
                    };
                    var currentBackgroundPath = SysLinuxConfigFile.SysLinuxAppearance.ScreenBackground;
                    window.ShowDialog();
                    if (currentBackgroundPath != SysLinuxConfigFile.SysLinuxAppearance.ScreenBackground)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(SysLinuxConfigFile.SysLinuxAppearance.ScreenBackground))
                                File.Copy(SysLinuxConfigFile.SysLinuxAppearance.ScreenBackground,
                                    Path.Combine(Drive.RootDirectory.FullName, "multiboot", "background.png"), true);
                            else
                                WpfUtilities.WriteResourceToFile(
                                    new Uri("pack://application:,,,/Resources/SysLinuxFiles/background.png"),
                                    Path.Combine(Drive.RootDirectory.FullName, "multiboot", "background.png"));
                        }
                        catch (IOException ex)
                        {
                            MessageBoxEx.Show(_window, "Failed to replace background.png: " + ex.Message,
                                "Error replacing file", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    SysLinuxConfigFile.Save();
                }));
            }
        }

        public RelayCommand RemoveSystemCommand
        {
            get
            {
                return _removeSystemCommand ?? (_removeSystemCommand = new RelayCommand(async parameter =>
                {
                    var system = parameter as SystemEntry;
                    if (system == null)
                        return;

                    if (
                        MessageBoxEx.Show(_window,
                            $"Are you sure that you want to remove {system.Name} from the drive {Drive.Name}?",
                            $"Remove {system.Name}", MessageBoxButton.OKCancel, MessageBoxImage.Warning,
                            MessageBoxResult.Cancel) != MessageBoxResult.OK)
                        return;

                    IsLoading = true;

                    try
                    {
                        SysLinuxConfigFile.RemoveSystem(system);
                        SysLinuxConfigFile.Save();

                        var systemDirectory =
                            new DirectoryInfo(Path.Combine(Drive.RootDirectory.FullName, "multiboot", system.Directory));
                        if (systemDirectory.Exists)
                            await Task.Run(() => FileSystemExtensions.DeleteDirectory(systemDirectory.FullName));
                    }
                    catch (Exception ex)
                    {
                        MessageBoxEx.Show(_window, "An error occured: " + ex.Message, "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error, MessageBoxResult.OK);
                        return;
                    }
                    finally
                    {
                        IsLoading = false;
                    }

                    MessageBoxEx.Show(_window, "The system was successfully removed from the drive.", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }));
            }
        }
    }
}