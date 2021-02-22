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
            var brief = new Brief();
            brief.Load(briefFileName);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(brief.SenderName, brief.SenderEmailAddress));
            message.To.Add(MailboxAddress.Parse(recipients));
            message.Subject = brief.Subject;
            
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = brief.GetBodyHtml();
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
