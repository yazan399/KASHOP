using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace KASHOP.BLL.Common
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("yoyoyazan22@gmail.com", "viom xvbl mmly ddao")
            };
            return client.SendMailAsync(
                new MailMessage(from: "yoyoyazan22@gmail.com",
                    to: email,
                    subject: subject,
                    body: htmlMessage)
                {
                    IsBodyHtml = true
                });


        }
    }
}
