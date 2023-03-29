
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Workshop.CSharp.Advanced
{
    public static class AsyncImageDownloader
    {
        private static HttpClient httpClient = new HttpClient();

        public static void RunDownloading()
        {
            var task = DownloadImagesAsync("nba", 13, true);
            task.Wait();
        }

        private static async Task DownloadImagesAsync(string text, int resultCount, bool allAtOnce = false)
        {
            var folderPath = ImageDownloaderUtils.PrepareFolder();

            try
            {
                var client = new GoogleImageSearchClient();
                var urls = client.Search(text, resultCount);

                Console.WriteLine("Pobieram {0} elementow ...", urls.Length);

                if (allAtOnce)
                {
                    var tasks = urls.Select(url => DownloadImageAsync(url, folderPath));
                    await Task.WhenAll(tasks);
                }
                else
                {
                    foreach (var url in urls)
                    {
                        await DownloadImageAsync(url, folderPath);
                    }
                }

                Console.WriteLine("Zrobione!");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.Message);
            }
        }

        private static async Task DownloadImageAsync(string url, string folderPath)
        {
            try
            {
                Console.WriteLine("Pobieram {0} ...", url);

                var data = await httpClient.GetByteArrayAsync(url);

                var filePath = ImageDownloaderUtils.FormatFilePath(url, folderPath);

                using (var toStream = new FileStream(filePath, FileMode.CreateNew))
                {
                    await toStream.WriteAsync(data, 0, data.Length);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception: " + exception.Message);
            }
        }
    }
}