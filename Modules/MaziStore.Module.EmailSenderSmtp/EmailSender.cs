using MaziStore.Module.Core.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;
using System.Threading.Tasks;

namespace MaziStore.Module.EmailSenderSmtp
{
   public class EmailSender : IEmailSender
   {
      private readonly EmailConfig _emailConfig = new EmailConfig();

      public EmailSender(IConfiguration configuration)
      {
         _emailConfig.SmtpServer = configuration.GetValue<string>("SmtpServer");
         _emailConfig.SmtpUsername = configuration.GetValue<string>("SmtpUsername");
         _emailConfig.SmtpPassword = configuration.GetValue<string>("SmtpPassword");
         _emailConfig.SmtpPort = configuration.GetValue<int>("SmtpPort");
      }

      public async Task SendEmailAsync(
         string email,
         string subject,
         string body,
         bool isHtml = false
      )
      {
         var message = new MimeMessage();
         message.From.Add(new MailboxAddress(_emailConfig.SmtpUsername));
         message.To.Add(new MailboxAddress(email));
         message.Subject = subject;

         var textFormat = isHtml ? TextFormat.Html : TextFormat.Plain;
         message.Body = new TextPart(textFormat) { Text = body };

         using (var client = new SmtpClient())
         {
            client.ServerCertificateValidationCallback = (
               obj,
               x509cert,
               x509chain,
               sslErrors
            ) => true;

            await client.ConnectAsync(
               _emailConfig.SmtpServer,
               _emailConfig.SmtpPort,
               false
            );
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(
               _emailConfig.SmtpUsername,
               _emailConfig.SmtpPassword
            );

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
         }
      }
   }
}
