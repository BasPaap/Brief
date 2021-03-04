﻿using System;
using System.Threading.Tasks;

namespace Bas.Brief.Application
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var arguments = new Arguments(args);

            if (arguments.SessionType == SessionType.Send)
            {
                await BriefSender.SendAsync(arguments.BriefFileName, arguments.RecipientName, arguments.Recipients, arguments.SmtpAddress, arguments.SmtpPort);
            }
            else if (arguments.SessionType == SessionType.Initialization)
            {
                BriefInitializer.Initialize(arguments.UserName, arguments.Password);
            }
            else
            {
                Console.WriteLine("Usage: Brief fileName recipientName recipients smtpAddress smtpPort");
            }

        }
    }
}
