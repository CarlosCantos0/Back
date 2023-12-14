using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using sdCommon.DTO;
using sdCommon.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace sdDomain.Clases
{
    public class sdEmailSender: IEmailSender
    {
        private readonly EmailSettingsDTO _emailSettingsDTO;

        public sdEmailSender(IConfiguration configuracion)
        {
            _emailSettingsDTO = configuracion.GetSection("EmailSettings").Get<EmailSettingsDTO>();
        }

        public Task SendEmailAsync(string emailReceptor, string subject, string htmlMessage)
        {
            return Execute(subject, htmlMessage, emailReceptor);
        }

        public Task SendEmailAsync(string emailReceptor, string copiaOculta, string subject, string htmlMessage)
        {
            return Execute(subject, htmlMessage, emailReceptor);
        }

        public Task SendEmailAsync(string emailReceptor, string subject, string htmlMessage, Attachment[] adjuntos)
        {
            return Execute(subject, htmlMessage, emailReceptor, "", adjuntos);
        }


        private Task Execute(string subject, string message, string emailReceptor, string copiaOculta = "", Attachment[] adjuntos = null)
        {
            try
            {

                // Mail message
                var mail = new MailMessage()
                {
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                mail.From = new MailAddress(_emailSettingsDTO.EmailSender, _emailSettingsDTO.SenderName);
                foreach (var email in emailReceptor.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.To.Add(email);
                }

                foreach (var email in copiaOculta.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.Bcc.Add(email);
                }


                if (adjuntos != null)
                {
                    foreach (Attachment adjunto in adjuntos)
                        mail.Attachments.Add(adjunto);
                }

                // Smtp client
                using (var smtp = new SmtpClient(_emailSettingsDTO.MailServer, _emailSettingsDTO.MailPort))
                {
                    smtp.UseDefaultCredentials = false;
                    var credentials = new NetworkCredential(_emailSettingsDTO.User, _emailSettingsDTO.Password);
                    smtp.Credentials = credentials;
                    //smtp.EnableSsl = _emailSettings.EnableSsl;

                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}
