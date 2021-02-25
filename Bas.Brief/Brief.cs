using Bas.Brief.ItemGenerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        private Dictionary<string, Dictionary<string, string>> persistedItemData;

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
            var briefFileName = Path.GetFileName(path);
            LoadPersistedItemData(briefFileName);   // Load any persisted data that was saved earlier by ItemGenerators.

            var configurationDocument = XDocument.Load(path);
            this.culture = new CultureInfo((string)configurationDocument.Root.Attribute("culture"));
            SenderName = (string)configurationDocument.Root.Attribute("senderName");
            SenderEmailAddress = (string)configurationDocument.Root.Attribute("senderEmail");
            Subject = ReplaceAllWildcards((string)configurationDocument.Root.Element("Subject"));
            BodyHtml = await GetBodyHtmlAsync(configurationDocument);

            SavePersistedItemData(briefFileName); // Save any changes that the ItemGenerators may have made to their persisted data.
        }

        private void LoadPersistedItemData(string briefFileName)
        {
            var persistedItemDataPath = GetPersistedItemDataPath(briefFileName);
            
            if (!Directory.Exists(Path.GetDirectoryName(persistedItemDataPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(persistedItemDataPath));
            }

            if (!File.Exists(persistedItemDataPath))
            {
                persistedItemData = new Dictionary<string, Dictionary<string, string>>();
                return;
            }

            var persistedItemJson = File.ReadAllText(persistedItemDataPath);
            persistedItemData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(persistedItemJson);
        }

        private void SavePersistedItemData(string briefFileName)
        {
            var persistedItemDataPath = GetPersistedItemDataPath(briefFileName);

            if (!Directory.Exists(Path.GetDirectoryName(persistedItemDataPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(persistedItemDataPath));
            }

            var persistedItemDataJson = JsonSerializer.Serialize(persistedItemData);
            File.WriteAllText(persistedItemDataPath, persistedItemDataJson);
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

        private static string GetPersistedItemDataPath(string briefFileName) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "Bas.Brief", $"{briefFileName}.data");

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
                    var identifier = item.Name + itemGenerator.GetUniqueIdentifier();

                    if (!persistedItemData.ContainsKey(identifier))
                    {
                        persistedItemData.Add(identifier, new Dictionary<string, string>());
                    }

                    itemGenerator.PersistentItemData = persistedItemData[identifier];
                                        
                    itemGenerators.Add(itemGenerator);
                }
            }

            return itemGenerators;
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
