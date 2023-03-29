using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Workshop.CSharp.Advanced
{
    public class GoogleImageSearchClient
    {
        private static HttpClient httpClient = new HttpClient();

        public string[] Search(string keyword, int resultCount)
        {
            // uwaga: tutaj blokujemy Task, nie powinnismy tak robic w kodzie produkcyjnym
            return SearchAsync(keyword, resultCount).Result;
        }

        public async Task<string[]> SearchAsync(string keyword, int resultCount)
        {
            string cacheFilePath = Path.Combine(Consts.ProjectFolderPath, "..", " __GimageSearchCache.json");
            string cacheKey = string.Format("{0}_{1}", keyword, resultCount);

            var cache = new Dictionary<string, string[]>();

            if (File.Exists(cacheFilePath))
            {
                var cacheContent = File.ReadAllText(cacheFilePath);
                cache = JsonSerializer.Deserialize<Dictionary<string, string[]>>(cacheContent) ?? new Dictionary<string, string[]>();
                string[] urls;
                if (cache.TryGetValue(cacheKey, out urls))
                {
                    return urls;
                }
            }

            GoogleImageResults googleResults = await ExecuteCall(keyword, resultCount);
            var result = googleResults.Items.Select(a => a.Link).ToArray();

            cache[cacheKey] = result;
            File.WriteAllText(cacheFilePath, JsonSerializer.Serialize(cache));

            return result;
        }

        private async Task<GoogleImageResults> ExecuteCall(string keyword, int resultCount)
        {
            const int pageSize = 10;
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

            try
            {
                var googleResultsList = new List<GoogleImageResults>();

                for (int start = 1; ; start += pageSize)
                {
                    var url = string.Format(
                        "https://www.googleapis.com/customsearch/v1?key=AIzaSyC1iFsZVNQUdTI8U9ETAEj0IWCIKSotlfk&cx=012667624178776984641:zei_o0qxidm&q={0}&searchType=image&imgSize=xlarge&alt=json&num={1}&start={2}",
                        keyword, Math.Min(resultCount, pageSize), start);

                    var httpResponseMessage = await httpClient.GetAsync(url);
                    var data = await httpClient.GetFromJsonAsync<GoogleImageResults>(url, options);
                    googleResultsList.Add(data);

                    resultCount -= pageSize;
                    if (resultCount <= 0)
                    {
                        break;
                    }
                }

                return new GoogleImageResults { Items = googleResultsList.SelectMany(r => r.Items).ToArray() };
            }
            catch (HttpRequestException exception)
            {
                Console.WriteLine("HttpRequestException:" + exception);
                throw;
            }
        }


        private class GoogleImageResults
        {
            public Image[] Items { get; set; }

            public GoogleImageResults()
            {
                Items = new Image[0];
            }
        }

        private class Image
        {
            public string Link { get; set; }
        }
    }
}


