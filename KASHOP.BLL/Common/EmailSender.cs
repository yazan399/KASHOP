using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;


namespace KASHOP.BLL.Common
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_configuration["EmailSettings:Host"], int.Parse(_configuration["EmailSettings:Port"]))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_configuration["EmailSettings:Email"], _configuration["EmailSettings:Password"])
            };
            return client.SendMailAsync(
                new MailMessage(from: _configuration["EmailSettings:Email"],
                    to: email,
                    subject: subject,
                    body: htmlMessage)
                {
                    IsBodyHtml = true
                });


        }
    }
}
