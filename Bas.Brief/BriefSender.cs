using HtmlAgilityPack;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

            var credentials = GetCredentials();

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpAddress, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(credentials.UserName, credentials.GetDecryptedPassword());
            await client.SendAsync(message);
            await client.DisconnectAsync(true);            
        }        

        private static Credentials GetCredentials()
        {
            var credentialsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), Brief.ApplicationDataFolderName, Brief.CredentialsFileName); ;
            var jsonCredentials = File.ReadAllText(credentialsPath);
            var credentials = JsonSerializer.Deserialize<Credentials>(jsonCredentials);
            return credentials;
        }
    }
}
