using System.IO;

namespace GodlikeStickCreator.Utilities
{
    public static class FileExtensions
    {
        /// <summary>
        ///     If the directory <see cref="path" /> already exists, add a number at the end to make it non existing
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DirectoryInfo MakeDirectoryUnique(string path)
        {
            var info = new DirectoryInfo(path);
            string dir = info.Parent.FullName;
            string name = info.Name;

            for (int i = 1;; ++i)
            {
                if (!Directory.Exists(path))
                    return new DirectoryInfo(path);

                path = Path.Combine(dir, name + " " + i);
            }
        }
    }
}