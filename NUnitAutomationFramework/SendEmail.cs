using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NUnitAutomationFramework
{
    public class SendEmail
    {
        public static void SendMail(string filePath)
        {
            Utils.WriteErrorLog("Starting to send email");
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient("smtp.mandrillapp.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = (ICredentialsByHost)new NetworkCredential("neacsu_drag0sh@yahoo.com", "5yQU9V76hLFa2ier0aixHw");
            smtpClient.EnableSsl = Convert.ToBoolean(true);
            message.From = new MailAddress("raluca.nanea@gmail.com", "PandaTeam", Encoding.UTF8);
            message.To.Add("raluca.nanea@gmail.com");
            message.Subject = "Test result VePrompt";
            message.IsBodyHtml = true;
            message.Body = "Please find attached the test results!";
            Attachment attachment = new Attachment(filePath);
            message.Attachments.Add(attachment);
            smtpClient.Send(message);
        }
    }
}
