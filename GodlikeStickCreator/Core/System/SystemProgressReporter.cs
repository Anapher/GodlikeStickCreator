using System;

namespace GodlikeStickCreator.Core.System
{
    public class SystemProgressReporter
    {
        public event EventHandler<double> ProgressChanged;
        public event EventHandler<string> MessageChanged;

        public void ReportProgress(double progress)
        {
            ProgressChanged?.Invoke(this, progress);
        }

        public void ReportStatus(InstallationStatus installationStatus)
        {
            switch (installationStatus)
            {
                case InstallationStatus.ExtractFile:
                    MessageChanged?.Invoke(this, "Extract file");
                    break;
                case InstallationStatus.WriteConfig:
                    MessageChanged?.Invoke(this, "Write config");
                    break;
                case InstallationStatus.CopyFiles:
                    MessageChanged?.Invoke(this, "Copy files");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(installationStatus), installationStatus, null);
            }
        }

        public void SetMessage(string message)
        {
            MessageChanged?.Invoke(this, message);
        }
    }

    public enum InstallationStatus
    {
        ExtractFile,
        WriteConfig,
        CopyFiles
    }
}