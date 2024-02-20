using Microsoft.Extensions.Options;
using MimeKit;
using test_platform.Configuration;
using test_platform.Models;
using MailKit.Net.Smtp;
using MimeKit.Text;

namespace test_platform.Services
{
    public interface IMailService
    {
        string SendMail(MailData mailData);
    }
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettingsOptions)
        {
            _mailSettings = mailSettingsOptions.Value;
        }

        public string SendMail(MailData mailData)
        {
            try
            {
                using (MimeMessage emailMessage = new MimeMessage())
                {
                    MailboxAddress emailFrom = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.From);
                    emailMessage.From.Add(emailFrom);
                    emailMessage.To.Add(new MailboxAddress(mailData.EmailToName, mailData.EmailToId));


                    emailMessage.Subject = mailData.EmailSubject;
                    var builder = new BodyBuilder();
                    builder.HtmlBody = mailData.EmailBody;
                    emailMessage.Body = builder.ToMessageBody();

                    //this is the SmtpClient from the Mailkit.Net.Smtp namespace, not the System.Net.Mail one
                    using (SmtpClient mailClient = new SmtpClient())
                    {
                        
                        mailClient.Connect(_mailSettings.Host, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                        mailClient.Authenticate(_mailSettings.UserName, _mailSettings.Password);
                        mailClient.Send(emailMessage);
                        mailClient.Disconnect(true);
                    }
                }

                return "true";
            }
            catch (Exception ex)
            {
                
                return ex.Message;
            }
        }
    }
}
