using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LinkStorage.Utility
{
    public static class HelperMail
    {
        public static async Task<bool> SendMail(string mail, string subject, string message)
        {
            try
            {
                MailMessage mailMessage = new MailMessage("mailiniz@gmail.com", mail, subject, message);

                mailMessage.IsBodyHtml = true;
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential("mailiniz@gmail.com", "dzjv xsya fbpy svpv");


                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}