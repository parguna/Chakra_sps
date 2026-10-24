using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using DI.Chakra.Logger;

namespace DI.Chakra.Services.EmailingProcessor
{
    public class EmailHelper
    {
        private string m_hostName = string.Empty;
        private int m_port = 25;

        public EmailHelper(string hostName, int port)
        {
            m_hostName = hostName;
            m_port = port;
        }

        public bool SendMailMessage(string from, string to, string bcc, string cc, string subject, string body)
        {
            try
            {
                MailMessage mMailMessage = new MailMessage();
                mMailMessage.From = new MailAddress(from);
     
                string[] toAddlist = to.Split(',');
                foreach (string tmpToAddress in toAddlist)
                {
                    if (!string.IsNullOrEmpty(tmpToAddress.Trim()))
                    {
                        mMailMessage.To.Add(new MailAddress(tmpToAddress.Trim()));
                    }
                }
                if ((bcc != null) && (bcc != string.Empty))
                {
                    mMailMessage.Bcc.Add(new MailAddress(bcc));
                }

                if ((cc != null) && (cc != string.Empty))
                {
                    mMailMessage.CC.Add(new MailAddress(cc));
                }
                mMailMessage.Subject = subject;
                mMailMessage.Body = body;
                mMailMessage.IsBodyHtml = true;
                mMailMessage.Priority = MailPriority.Normal;
                SmtpClient mSmtpClient = new SmtpClient();
                mSmtpClient.Host = m_hostName;
                mSmtpClient.Port = m_port;
                mSmtpClient.Send(mMailMessage);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(LogLevelType.Info, "Email Sent Successfully", null);
                throw;
            }
        }

    }
}
