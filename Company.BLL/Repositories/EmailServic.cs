using Company.BLL.Interfaces;
using Company.DAL.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Company.BLL.Repositories
{
    public  class EmailServic : IEmailServic
    {
        private readonly EmailSetting conf;
        public EmailServic(IOptions<EmailSetting> option)
        {
            conf = option.Value;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(conf.SmtpHost)
                {
                    Port = conf.SmtpPort,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(conf.SenderEmail, conf.SenderPassword),
                    EnableSsl = true
                };

                using var sendmessage = new MailMessage()
                {
                    From = new MailAddress(conf.SenderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                sendmessage.To.Add(to);

                await client.SendMailAsync(sendmessage);
            }
            catch (SmtpException ex)
            {
                // log details for debugging
                Console.WriteLine($"SMTP Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }

    }
}
