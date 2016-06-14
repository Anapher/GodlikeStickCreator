using System;
using System.Linq;
using System.Windows;
using GodlikeStickCreator.Utilities;

namespace GodlikeStickCreator
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public const string DriveLabel = "GODLIKE";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (e.Args.Contains("/format"))
            {
                var drive = e.Args[Array.IndexOf(e.Args, "/format") + 1];
                if (drive.Length != 2)
                    Environment.Exit(100);

                Environment.Exit((int) DriveUtilities.FormatDrive(drive, "FAT32", true, 8192, DriveLabel, false));
            }

            new MainWindow().Show();
        }
    }
}