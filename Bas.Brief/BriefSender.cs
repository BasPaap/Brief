using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bas.Brief
{
    public static class BriefSender
    {
        public static void Send(string briefFileName, string recipients)
        {
            var configuration = new BriefConfiguration();
            configuration.Load(briefFileName);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(configuration.SenderName, configuration.SenderEmailAddress));
            message.To.Add(MailboxAddress.Parse(recipients));
            message.Subject = configuration.Subject;

            var bodyBuilder = new BodyBuilder();
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.foo.com", 587, false);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
