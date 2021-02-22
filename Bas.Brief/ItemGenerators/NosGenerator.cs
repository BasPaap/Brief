using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bas.Brief.ItemGenerators
{
    public sealed class NosGenerator : ItemGenerator
    {
        public NosGenerator(IEnumerable<KeyValuePair<string, string>> parameters, string content)
            : base(parameters, content)
        {
        }

        public override async Task<string> ToHtmlAsync()
        {
            Collection<RssItem> rssItems = await GetRssItemsAsync();

            if (rssItems == null)
            {
                return "Er ging iets mis met het ophalen van het nieuws.";
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"<p>{Content}<p>");

            const string paragraphOpenTag = "<p>";
            const string paragraphCloseTag = "</p>";
            foreach (var rssItem in rssItems)
            {
                var startIndex = rssItem.Description.IndexOf(paragraphOpenTag) + paragraphOpenTag.Length;
                var length = rssItem.Description.IndexOf(paragraphCloseTag) - startIndex;
                var itemText = rssItem.Description.Substring(startIndex, length);

                stringBuilder.Append("<div>");
                stringBuilder.Append($"<p style=\"float: left;\"><img src=\"{rssItem.Enclosure}\" height=\"72px\" width=\"128px\"></p>");
                stringBuilder.Append($"<p><a href=\"{rssItem.Link}\"><strong>{rssItem.Title}</strong></a><br />");
                stringBuilder.Append($"{itemText}</p>");
                stringBuilder.Append("</div>");
            }

            return stringBuilder.ToString();
        }

        private async Task<Collection<RssItem>> GetRssItemsAsync()
        {
            var httpClient = new HttpClient();
            var rssResponse = await httpClient.GetStringAsync(Parameters["url"]);

            if (string.IsNullOrWhiteSpace(rssResponse))
            {
                return null;
            }

            var numItemsToTake = int.Parse(Parameters["numItems"]);

            var rssDocument = XDocument.Parse(rssResponse);
            var rssItems = (from item in rssDocument.Root.Element("channel").Elements("item")
                            orderby DateTime.Parse((string)item.Element("pubDate")) descending
                            select new RssItem
                            {
                                Title = (string)item.Element("title"),
                                Link = (string)item.Element("link"),
                                Enclosure = (string)item.Element("enclosure").Attribute("url"),
                                Description = (string)item.Element("description"),
                                PubDate = DateTime.Parse((string)item.Element("pubDate")),
                                Guid = (string)item.Element("guid")
                            }).Take(numItemsToTake);
            
            return new Collection<RssItem>(rssItems.ToList());
        }
    }
}
