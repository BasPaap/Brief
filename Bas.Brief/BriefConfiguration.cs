﻿using System;
using System.Collections.Generic;
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
                                     DataComponents = from dataComponent in root.Element("DataComponents").Elements()
                                             select new
                                             {
                                                 Name = dataComponent.Name,
                                                 Parameters = from attribute in dataComponent.Attributes()
                                                              select new
                                                              {
                                                                  Name = attribute.Name,
                                                                  Value = attribute.Value
                                                              },
                                                 Content = dataComponent.Value
                                             },
                                     SignOff = (string)root.Element("SignOff")
                                 }).Single();

            Subject = configuration.Subject;
            IntroductionHtml = configuration.Introduction;
            SignOffHtml = configuration.SignOff;
            SenderName = configuration.Sender.Name;
            SenderEmailAddress = configuration.Sender.EmailAddress;            
        }
    }
}
