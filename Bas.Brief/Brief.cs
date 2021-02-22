using Bas.Brief.ItemGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public void Load(string fileName)
        {
            var configurationDocument = XDocument.Load(fileName);
            SenderName = (string)configurationDocument.Root.Attribute("senderName");
            SenderEmailAddress = (string)configurationDocument.Root.Attribute("senderEmail");
            Subject = (string)configurationDocument.Root.Element("Subject");
            IntroductionHtml = (string)configurationDocument.Root.Element("Introduction");
            SignOffHtml = (string)configurationDocument.Root.Element("SignOff");

            var items = from item in configurationDocument.Root.Element("Items").Elements()
                        select new
                        {
                            Name = item.Name,
                            Parameters = from attribute in item.Attributes()
                                         select new KeyValuePair<string, string>(attribute.Name.ToString(), attribute.Value),                                         
                            Content = item.Value
                        };

            ItemGenerators.Clear();

            foreach (var item in items)
            {
                var isFirst = item == items.First();
                var isLast = item == items.Last();

                var itemGenerator = ItemGeneratorFactoy.GetItemGenerator(item.Name.ToString(), item.Parameters, item.Content, isFirst, isLast);
                ItemGenerators.Add(itemGenerator);
            }
        }

        public string GetBodyHtml()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(IntroductionHtml);

            foreach (var itemGenerator in ItemGenerators)
            {
                stringBuilder.Append(itemGenerator.ToHtml());
            }

            stringBuilder.Append(SignOffHtml);

            return stringBuilder.ToString();
        }
    }
}
