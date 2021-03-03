using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.Brief.Application
{
    enum SessionType
    {
        Unknown = 0,
        Initialisation,
        Send
    }

    record Arguments
    {
        public string BriefFileName { get; init; }
        public string RecipientName { get; init; }
        public string Recipients { get; init; }
        public string SmtpAddress { get; init; }
        public int SmtpPort { get; init; }

        public SessionType SessionType { get; init; }

        public Arguments(string[] commandLineArguments)
        {
            const int numExpectedInitialisationArguments = 3;
            const int numExpectedSendArguments = 5;
            const int briefFileNameIndex = 0;
            const int recipientNameIndex = 1;
            const int recipientsIndex = 2;
            const int smtpAddressIndex = 3;
            const int smtpPortIndex = 4;
            const int initialisationCommandIndex = 0;
            const int userNameIndex = 1;
            const int passwordIndex = 2;
            const string initialisationCommand = "init";
            
            if (commandLineArguments.Length == numExpectedSendArguments)
            {
                SessionType = SessionType.Send;
                BriefFileName = commandLineArguments[briefFileNameIndex];
                RecipientName = commandLineArguments[recipientNameIndex];
                Recipients = commandLineArguments[recipientsIndex];
                SmtpAddress = commandLineArguments[smtpAddressIndex];
                SmtpPort = int.Parse(commandLineArguments[smtpPortIndex], CultureInfo.InvariantCulture);
            }
            else if (commandLineArguments.Length == numExpectedInitialisationArguments && 
                     string.Compare(commandLineArguments[initialisationCommandIndex], initialisationCommand, true) == 0)
            {
                SessionType = SessionType.Initialisation;
                var userName = commandLineArguments[userNameIndex];
                var password = commandLineArguments[passwordIndex];
                

            }
            else
            {
                SessionType = SessionType.Unknown;
            }
        }
    }
}
