using HtmlAgilityPack;
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
        public static async Task SendAsync(string briefPath, string recipientName, string recipients, string smtpAddress, int smtpPort)
        {
            var brief = new Brief(recipientName);
            await brief.LoadAsync(briefPath);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(brief.SenderName, brief.SenderEmailAddress));
            message.To.Add(MailboxAddress.Parse(recipients));
            message.Subject = brief.Subject;

            var entitizedBodyHtml = HtmlEntity.Entitize(brief.BodyHtml);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(brief.BodyHtml);            
            var bodyText = htmlDocument.DocumentNode.InnerText;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = entitizedBodyHtml,
                TextBody = bodyText
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpAddress, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("username", "password");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);            
        }
    }
}
