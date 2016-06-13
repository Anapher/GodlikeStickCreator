using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace GodlikeStickCreator.Core.System.Installer
{
    public class Memtest86 : InstallerInfo
    {
        public override InstallMethod InstallMethod { get; } = InstallMethod.Memtest86;

        public override void Install(DirectoryInfo systemDirectory, string systemName, SpecialSnowflake specialSnowflake, string filename, out MenuItemInfo menuItem, SystemProgressReporter progressReporter)
        {
            FileInfo memtestFile = null;
            progressReporter.ReportStatus(InstallationStatus.ExtractZipFile);
            using (var fileStream = File.OpenRead(filename))
            {
                var zipFile = new ZipFile(fileStream);
                foreach (ZipEntry zipEntry in zipFile)
                {
                    if (zipEntry.Name.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
                    {
                        memtestFile = new FileInfo(Path.Combine(systemDirectory.FullName, zipEntry.Name));
                        using (var zipStream = zipFile.GetInputStream(zipEntry))
                        using (var memetestFileStream = memtestFile.OpenWrite())
                        {
                            var buffer = new byte[4096];
                            var totalRead = 0d;
                            var length = zipEntry.Size;
                            int read;
                            while ((read = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                memetestFileStream.Write(buffer, 0, read);
                                totalRead += read;
                                progressReporter.ReportProgress(totalRead / length);
                            }
                        }
                        break;
                    }
                }
            }

            if (memtestFile == null)
                throw new FileNotFoundException();

            var pathWithoutDrive = RemoveDriveFromPath(memtestFile.FullName).Replace('\\', '/');
            menuItem = new MenuItemInfo($"LINUX {pathWithoutDrive}");
        }
    }
}