using Bas.Brief.ItemGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bas.Brief
{
    sealed class Brief
    {
        public string Subject { get; private set; }
        public string SenderName { get; private set; }
        public string SenderEmailAddress { get; private set; }
        public string BodyHtml { get; private set; }

        private CultureInfo culture = CultureInfo.InvariantCulture;
        private readonly Dictionary<string, string> replacements;

        public Brief(string recipientName)
        {            
            replacements = new Dictionary<string, string>
            {
                { "{RECIPIENT}", recipientName },
                { "{DATE}", DateTime.Now.ToString(culture) }
            };
        }

        public async Task LoadAsync(string path)
        {
            var configurationDocument = XDocument.Load(path);
                        
            this.culture = new CultureInfo((string)configurationDocument.Root.Attribute("culture"));
            SenderName = (string)configurationDocument.Root.Attribute("senderName");
            SenderEmailAddress = (string)configurationDocument.Root.Attribute("senderEmail");
            Subject = ReplaceAllWildcards((string)configurationDocument.Root.Element("Subject"));
            BodyHtml = await GetBodyHtmlAsync(configurationDocument);
        }

        private List<ItemGenerator> GetItemGenerators(XDocument configurationDocument)
        {
            var items = from item in configurationDocument.Root.Element("Items").Elements()
                        select new
                        {
                            item.Name,
                            Parameters = from attribute in item.Attributes()
                                         select new KeyValuePair<string, string>(attribute.Name.ToString(), attribute.Value),
                            Content = item.Value
                        };

            List<ItemGenerator> itemGenerators = new List<ItemGenerator>();

            foreach (var item in items)
            {
                var itemGenerator = ItemGeneratorFactoy.GetItemGenerator(item.Name.ToString(), item.Parameters, item.Content, culture);

                if (itemGenerator != null)
                {
                    itemGenerators.Add(itemGenerator);
                }
            }

            return itemGenerators;
        }

        private async Task<string> GetBodyHtmlAsync(XDocument configurationDocument)
        {
            var stringBuilder = new StringBuilder();
            
            foreach (var itemGenerator in GetItemGenerators(configurationDocument))
            {
                stringBuilder.Append(await itemGenerator.ToHtmlAsync());
                stringBuilder.Append(' ');  // Add a space after the item to make the text version of this brief more readable.
            }

            var html = stringBuilder.ToString();
            return ReplaceAllWildcards(html);
        }

        private string ReplaceAllWildcards(string text)
        {
            string replacedText = text;
            foreach (var replacement in replacements)
            {
                replacedText = replacedText.Replace(replacement.Key, replacement.Value);
            }
            return replacedText;
        }
    }
}
