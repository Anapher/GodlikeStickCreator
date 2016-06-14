using System.Linq;
using System.Management;

namespace GodlikeStickCreator.Utilities
{
    public static class DriveUtilities
    {
        //Source: http://www.codeproject.com/Articles/115598/Formatting-a-Drive-using-C-and-WMI
        public static FormatResponse FormatDrive(string driveLetter, string fileSystem, bool quickFormat,
            int clusterSize,
            string label, bool enableCompression)
        {
            using (var searcher = new ManagementObjectSearcher
                ($"select * from Win32_Volume WHERE DriveLetter = '{driveLetter}'"))
            {
                var vi = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
                if (vi == null)
                    throw new ManagementException("Drive not found in WMI");

                var response = vi.InvokeMethod("Format", new object[]
                {fileSystem, quickFormat, clusterSize, label, enableCompression});
                return (FormatResponse) (uint) response;
            }
        }
    }

    //https://msdn.microsoft.com/en-us/library/aa390432(v=vs.85).aspx
    public enum FormatResponse
    {
        Success,
        UnsupportedFileSystem,
        IncompatibleMediaInDrive,
        AccessDenied,
        CallCanceled,
        CallCancellationRequestTooLate,
        VolumeWriteProtected,
        VolumeLockFailed,
        UnableToQuickFormat,
        InputOutputError,
        InvalidVolumeLabel,
        NoMediaInDrive,
        VolumeIsTooSmall,
        VolumeIsTooLarge,
        VolumeIsNotMounted,
        ClusterSizeIsTooSmall,
        ClusterSizeIsTooLarge,
        ClusterSizeIsBeyond32Bits,
        UnknownError
    }
}