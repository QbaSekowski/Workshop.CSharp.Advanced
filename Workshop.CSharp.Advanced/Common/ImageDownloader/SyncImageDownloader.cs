
using System;
using System.IO;
using System.Net.Http;

namespace Workshop.CSharp.Advanced
{
    public static class SyncImageDownloader
    {
        private static HttpClient httpClient = new HttpClient();

        public static void RunDownloading()
        {
            DownloadImages("nba", 13, true);
        }

        private static void DownloadImages(string text, int resultCount, bool allAtOnce)
        {
            var folderPath = ImageDownloaderUtils.PrepareFolder();

            try
            {
                var client = new GoogleImageSearchClient();
                var urls = client.Search(text, resultCount); // IO

                Console.WriteLine("Pobieram {0} elementow ...", urls.Length);

                foreach (var url in urls)
                {
                    DownloadImage(url, folderPath); // IO
                }

                Console.WriteLine("Zrobione!");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.Message);
            }
        }

        private static void DownloadImage(string url, string folderPath)
        {
            try
            {
                Console.WriteLine("Pobieram {0} ...", url);

                // uwaga: blokowanie Task, nie robic tego w kodzie produkcyjnym
                var data = httpClient.GetByteArrayAsync(url).Result; // IO

                var filePath = ImageDownloaderUtils.FormatFilePath(url, folderPath);

                using (var toStream = new FileStream(filePath, FileMode.CreateNew))
                {
                    toStream.Write(data, 0, data.Length); // IO
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.Message);
            }
        }
    }
}