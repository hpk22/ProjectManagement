using System.Net;
using System.Net.Mail;

namespace ProjectManagement.Utility
{
    public static class EmailSender
    {
        public static void Send(string toEmail, string subject, string body)
        {
            using (var smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new NetworkCredential("hrutvijkakade123@gmail.com", "prqv aqpz bsne iaaq");
                smtp.EnableSsl = true;

                var mail = new MailMessage("hrutvijkakade123@gmail.com", toEmail, subject, body);
                smtp.Send(mail);
            }
        }
    }
}
