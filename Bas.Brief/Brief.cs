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
        public string IntroductionHtml { get; private set; }
        public string SignOffHtml { get; private set; }
        public string SenderName { get; private set; }
        public string SenderEmailAddress { get; private set; }
        public Collection<ItemGenerator> ItemGenerators { get; private set; } = new Collection<ItemGenerator>();

        private CultureInfo culture = CultureInfo.InvariantCulture;

        private readonly Dictionary<string, string> replacements;

        public Brief(string recipientName)
        {            
            replacements = new Dictionary<string, string>
            {
                { "%RECIPIENT%", recipientName },
                { "%DATE%", DateTime.Now.ToString(culture) }
            };
        }

        public void Load(string path)
        {
            var configurationDocument = XDocument.Load(path);
            this.culture = new CultureInfo((string)configurationDocument.Root.Attribute("culture"));
            SenderName = (string)configurationDocument.Root.Attribute("senderName");
            SenderEmailAddress = (string)configurationDocument.Root.Attribute("senderEmail");
            Subject = ReplaceAllWildcards((string)configurationDocument.Root.Element("Subject"));
            IntroductionHtml = (string)configurationDocument.Root.Element("Introduction");
            SignOffHtml = (string)configurationDocument.Root.Element("SignOff");

            var items = from item in configurationDocument.Root.Element("Items").Elements()
                        select new
                        {
                            item.Name,
                            Parameters = from attribute in item.Attributes()
                                         select new KeyValuePair<string, string>(attribute.Name.ToString(), attribute.Value),
                            Content = item.Value
                        };

            ItemGenerators.Clear();

            foreach (var item in items)
            {
                var itemGenerator = ItemGeneratorFactoy.GetItemGenerator(item.Name.ToString(), item.Parameters, item.Content, culture);
                ItemGenerators.Add(itemGenerator);
            }
        }

        public async Task<string> GetBodyHtmlAsync()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(IntroductionHtml);

            foreach (var itemGenerator in ItemGenerators)
            {
                stringBuilder.Append("<p>");
                stringBuilder.Append(await itemGenerator.ToHtmlAsync());
                stringBuilder.Append("</p>");
            }

            stringBuilder.Append(SignOffHtml);
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
