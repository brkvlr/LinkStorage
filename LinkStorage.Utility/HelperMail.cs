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
            Console.WriteLine("E-posta gönderme işlemi simüle ediliyor...");
            Console.WriteLine($"Gönderilen E-posta: {mail}, Konu: {subject}, Mesaj: {message}");

            await Task.Delay(100); // Asenkron bir metot olduğu için bekleme ekledik
            return true; // Simüle edilen başarılı gönderim
            //try
            //{
            //    MailMessage mailMessage = new MailMessage("info@runawaytr.com", mail, subject, message);

            //    mailMessage.IsBodyHtml = true;
            //    SmtpClient smtpClient = new SmtpClient("smtp.yandex.com", 465);
            //    smtpClient.EnableSsl = true;
            //    smtpClient.Credentials = new NetworkCredential("info@runawaytr.com", "xxx xxx xxx xxx");


            //    await smtpClient.SendMailAsync(mailMessage);
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}

        }
    }
}