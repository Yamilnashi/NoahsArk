using System.IO;

namespace NoahsArk.Utilities
{
    public class DirectoryPathHelper
    {
        public static string GetFormattedFilePath(string filePath)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            return Path.Combine(Path.GetDirectoryName(filePath), fileNameWithoutExtension); ;
        }
    }
}
