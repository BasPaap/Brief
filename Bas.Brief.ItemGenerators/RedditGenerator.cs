using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bas.Brief.ItemGenerators
{
    [ItemGenerator("Reddit")]
    public sealed class RedditGenerator : ItemGenerator
    {
        private record RedditItem
        {
            public string Title { get; init; }
            public string Url { get; init; }            
        }

        public RedditGenerator(IEnumerable<KeyValuePair<string, string>> parameters, string content, CultureInfo culture)
            : base(parameters, content, culture)
        {
        }

        public override async Task<string> ToHtmlAsync()
        {
            var redditItem = await GetRedditItemAsync();

            var stringBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(Content))
            {
                stringBuilder.Append(Content);
            }
                                   
            stringBuilder.Append($"<a href=\"{redditItem.Url}\"><strong>{redditItem.Title}</strong></a><br />");
            
            return stringBuilder.ToString();            
        }

        private async Task<RedditItem> GetRedditItemAsync()
        {
            var jsonResponse = await GetJsonAsync();

            var jsonDocument = JsonDocument.Parse(jsonResponse);
            var dataProperty = jsonDocument.RootElement.GetProperty("data").GetProperty("children")[0].GetProperty("data");
            var redditItem = new RedditItem
            {
                Title = dataProperty.GetProperty("title").GetString(),
                Url = $"https://reddit.com{dataProperty.GetProperty("permalink").GetString()}"
            };
            return redditItem;
        }

        private async Task<string> GetJsonAsync()
        {
            string url = $"https://www.reddit.com/r/{Parameters["sub"]}/top.json?limit={Parameters["numItems"]}&t={Parameters["period"]}";

            using var httpClient = new HttpClient();
            var jsonResponse = await httpClient.GetStringAsync(url);

            return jsonResponse;
        }
    }
}
