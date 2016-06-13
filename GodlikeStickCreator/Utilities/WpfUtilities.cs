using System;
using System.IO;
using System.Windows;

namespace GodlikeStickCreator.Utilities
{
    public class WpfUtilities
    {
        public static void WriteResourceToFile(Uri resourceUri, string fileName)
        {
            var resource = Application.GetResourceStream(resourceUri);
            if (resource == null)
                throw new FileNotFoundException();

            using (var fileStream = File.OpenWrite(fileName))
                resource.Stream.CopyTo(fileStream);
        }
    }
}