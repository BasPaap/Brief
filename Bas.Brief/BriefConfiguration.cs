using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bas.Brief
{
    sealed class BriefConfiguration
    {
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

            throw new NotImplementedException();
            
        }
    }
}
