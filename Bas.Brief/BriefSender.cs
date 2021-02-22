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
        public static async Task SendAsync(string briefFileName, string recipientName, string recipients)
        {
            var brief = new Brief(recipientName);
            brief.Load(briefFileName);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(brief.SenderName, brief.SenderEmailAddress));
            message.To.Add(MailboxAddress.Parse(recipients));
            message.Subject = brief.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = await brief.GetBodyHtmlAsync()
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.foo.com", 587, false);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);            
        }
    }
}
