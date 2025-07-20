using Common.Libraries.Services.Services;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libraries.Services.Email
{
    public class EmailMessage : IMessage
    {
        public string Message { get; set; }
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }

        public EmailMessage(string message, IEnumerable<string> to, string subject)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("",x)));
            Message = message;            
            Subject = subject;
        }
    }
    public class EmailNotification : INotificationService<EmailMessage>
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailNotification(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task<bool> SendNotification(EmailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);
            var result =  Send(emailMessage);
            return result != null;
            
        }
        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_emailConfig.FromName, _emailConfig.From));
            mimeMessage.To.AddRange(message.To);
            mimeMessage.Subject = message.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = message.Message,          
                TextBody = "Your client does not support HTML emails."
            };

            mimeMessage.Body = builder.ToMessageBody();
            return mimeMessage;
        }
        /*private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.FromName,_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Message };
            return emailMessage;
        }*/
        private string Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                    var s = client.Send(mailMessage);
					//client.Disconnect(true);
                    return s;
				}
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
              
            }
        }
    }
}
