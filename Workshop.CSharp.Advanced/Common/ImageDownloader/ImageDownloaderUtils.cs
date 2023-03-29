using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Workshop.CSharp.Advanced
{
    public static class ImageDownloaderUtils
    {
        public static string PrepareFolder(string folderName = "DownloadedPhotos")
        {
            var folderPath = Path.Combine(Consts.ProjectFolderPath, "..", folderName);
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
            Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public static string FormatFilePath(string url, string folderPath)
        {
            var fileName = @"\/:*?""<>|".Aggregate(new StringBuilder(Path.GetFileName(url)), (agg, c) => agg.Replace(c, ' ')).ToString();
            return Path.Combine(folderPath, string.Format("f {0} {1}", Guid.NewGuid().ToString("N"), fileName));
        }
    }
}