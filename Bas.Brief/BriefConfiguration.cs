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
    sealed class BriefConfiguration
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

            var configuration = (from root in configurationDocument.Elements()
                                 select new
                                 {
                                     Sender = new
                                     {
                                         Name = (string)root.Attribute("senderName"),
                                         EmailAddress = (string)root.Attribute("senderEmail")
                                     },
                                     Subject = (string)root.Element("Subject"),
                                     Introduction = (string)root.Element("Introduction"),
                                     Items = from item in root.Element("Items").Elements()
                                             select new
                                             {
                                                 Name = item.Name,
                                                 Parameters = from attribute in item.Attributes()
                                                              select new
                                                              {
                                                                  Name = attribute.Name,
                                                                  Value = attribute.Value
                                                              },
                                                 Content = item.Value
                                             },
                                     SignOff = (string)root.Element("SignOff")
                                 }).Single();

            Subject = configuration.Subject;
            IntroductionHtml = configuration.Introduction;
            SignOffHtml = configuration.SignOff;
            SenderName = configuration.Sender.Name;
            SenderEmailAddress = configuration.Sender.EmailAddress;

            foreach (var item in configuration.Items)
            {
                var isFirst = item == configuration.Items.First();
                var isLast = item == configuration.Items.Last();
                
                
            }
        }
    }
}
