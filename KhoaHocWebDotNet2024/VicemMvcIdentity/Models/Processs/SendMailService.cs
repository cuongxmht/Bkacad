using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace VicemMvcIdentity.Models.Process
{
    public class SendMailService: IEmailSender
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<SendMailService> _logger;
        public SendMailService(IOptions<MailSettings> mailSettings,ILogger<SendMailService> logger)
        {
            _mailSettings=mailSettings.Value;
            _logger=logger;
            _logger.LogInformation("Create SendMailServices");
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message=new MimeMessage();
            message.Sender = new MailboxAddress(_mailSettings.DisplayName,_mailSettings.Mail);
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName,_mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            var builder=new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            message.Body=builder.ToMessageBody();
            using var smtp=new MailKit.Net.Smtp.SmtpClient();
            try {
                smtp.Connect(_mailSettings.Host,_mailSettings.Port,MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(message);
            }
            catch(Exception ex){
                _logger.LogError($"Loi gui mail: {ex.Message}");

            }
            smtp.Disconnect(true);
            _logger.LogInformation("Send mail to: " + email);

        }

    }
}