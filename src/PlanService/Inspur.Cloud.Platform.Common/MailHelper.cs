using Inspur.ECP.Rtf.Extensions;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspur.ECP.Rtf.Common
{
    public class MailHelper
    {
        private static object lockObj = new object();
        private static MailServer mailServer;

        private static MailServer MailServer
        {
            get
            {
                if (mailServer == null)
                {
                    lock (lockObj)
                    {
                        if (mailServer == null)
                            mailServer = Utility.GetJsonConfig("configs/message.json").GetCinfig<MailServer>("MailServers", "default");
                    }
                }
                return mailServer;
            }
        }

        #region Smtp 方式

        public static void SendBySmtp(List<string> mails, string subject, string body, bool isHtml = false)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(MailServer.Account));
            foreach (var item in mails)
            {
                message.To.Add(new MailboxAddress(item));
            }

            message.Subject = subject;
            TextFormat format = isHtml == true ? TextFormat.Html : TextFormat.Plain;

            message.Body = new TextPart(format)
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(MailServer.Host, MailServer.Port, MailServer.UserSSL);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(Encoding.UTF8, MailServer.Account, MailServer.Password);

                client.Send(message);
                client.Disconnect(true);
            }
        }

        public static async Task SendBySmtpAsync(List<string> mails, string subject, string body, bool isHtml = false)
        {
            if (mails == null || mails.Count == 0)
            {
                return;
            }
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(MailServer.Account));
            foreach (var item in mails)
            {
                message.To.Add(new MailboxAddress(item));
            }

            message.Subject = subject;
            TextFormat format = isHtml == true ? TextFormat.Html : TextFormat.Plain;

            message.Body = new TextPart(format)
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(MailServer.Host, MailServer.Port, MailServer.UserSSL);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                await client.AuthenticateAsync(Encoding.UTF8, MailServer.Account, MailServer.Password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        #endregion
    }
}
