using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using DI.Chakra.Logger;
using System.Configuration;
using DI.Chakra.Infrastructure.Enums;
using System.IO;
using System.Net.Mime;
using System.Linq;

namespace DI.Chakra.Services.EmailingProcessor
{
    public class EmailSender
    {

        public MailServerConfigInfo ServerConfig { get; set; }
        public string EmailExclusionList { get; set; }
        public string EmailExlcusionReplacer { get; set; }
        public EmailSender(MailServerConfigInfo _server)
        {
            ServerConfig = _server;
        }

        MailPriority GetPriority(string priority)
        {
            if (Enum.TryParse(priority, out MailPriority mailPriority))
                return mailPriority;
            return MailPriority.Normal;
        }

        public bool Send(DomainModel.EmailInfo _info, Attachment attachment = null)
        {

            if (_info == null) return false;

            MailMessage mMailMessage = null;
            SmtpClient mSmtpClient = null;

            try
            {


                mMailMessage = new MailMessage();

                //FROM
                mMailMessage.From = new MailAddress(_info.From);

                //TO
                if (!string.IsNullOrWhiteSpace(_info.To))
                {
                    string[] toAddlist = _info.To.Split(',');
                    foreach (string tmpToAddress in toAddlist)
                    {
                        if (!string.IsNullOrWhiteSpace(tmpToAddress) && !string.IsNullOrWhiteSpace(tmpToAddress.Trim()))
                        {
                            string toMail = EmailExclusionListCheck(tmpToAddress.Trim());
                            if (!string.IsNullOrWhiteSpace(toMail))
                                mMailMessage.To.Add(new MailAddress(toMail.Trim()));
                        }
                    }
                }

                //CC
                if (!string.IsNullOrWhiteSpace(_info.Cc))
                {
                    string[] toAddlist = _info.Cc.Split(',');
                    foreach (string tmpCCAddress in toAddlist)
                    {
                        if (!string.IsNullOrWhiteSpace(tmpCCAddress) && !string.IsNullOrWhiteSpace(tmpCCAddress.Trim()))
                        {
                            string ccMail = EmailExclusionListCheck(tmpCCAddress.Trim());

                            if (!string.IsNullOrWhiteSpace(ccMail))
                                mMailMessage.CC.Add(new MailAddress(ccMail.Trim()));
                        }
                    }
                }

                //BCC
                if (!string.IsNullOrWhiteSpace(_info.Bcc))
                {
                    string[] toAddlist = _info.Bcc.Split(',');
                    foreach (string tmpbccAddress in toAddlist)
                    {
                        if (!string.IsNullOrWhiteSpace(tmpbccAddress) && !string.IsNullOrWhiteSpace(tmpbccAddress.Trim()))
                        {
                            string bccMail = EmailExclusionListCheck(tmpbccAddress.Trim());
                            if (!string.IsNullOrWhiteSpace(bccMail))
                                mMailMessage.Bcc.Add(new MailAddress(bccMail.Trim()));
                        }
                    }
                }

                long attachmentId = 0;
                if (!string.IsNullOrWhiteSpace(_info.Attachment) && long.TryParse(_info.Attachment, out attachmentId))
                {
                    try
                    {
                        var attachmentContent = DomainModel.Attachment.GetById(attachmentId);
                        if (attachmentContent != null)
                        {
                            Attachment attach = new Attachment(new MemoryStream(attachmentContent.AttachmentFile), $"{attachmentContent.FileName}.csv");
                            mMailMessage.Attachments.Add(attach);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.EmailLogger.WriteLog(LogLevelType.Exception, "Exception occured during construction of attachment in EmailSender.Send()", ex);
                    }
                }

                if (attachment != null)
                {
                    Attachment attach = attachment;
                    mMailMessage.Attachments.Add(attach);
                }
                mMailMessage.Subject = _info.Subject;
                mMailMessage.Body = _info.Body;
                mMailMessage.IsBodyHtml = _info.IsHtml;
                mMailMessage.Priority = GetPriority(_info.Priority);

                mSmtpClient = new SmtpClient();

                mSmtpClient.Host = ServerConfig.HostName;
                mSmtpClient.Port = int.Parse(ServerConfig.PortNumber);
                mSmtpClient.EnableSsl = ServerConfig.EnableSSL;

                if (!string.IsNullOrWhiteSpace(ServerConfig.UserName) && !string.IsNullOrWhiteSpace(ServerConfig.Password))
                    mSmtpClient.Credentials = new System.Net.NetworkCredential(ServerConfig.UserName, ServerConfig.Password);
                else
                    mSmtpClient.UseDefaultCredentials = false;

                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                if (mMailMessage != null &&
                    ((mMailMessage.To != null && mMailMessage.To.Count > 0) ||
                     (mMailMessage.CC != null && mMailMessage.CC.Count > 0) ||
                     (mMailMessage.Bcc != null && mMailMessage.Bcc.Count > 0)))
                {
                    mSmtpClient.Send(mMailMessage);
                }

                //DILog.EmailLogger.WriteLog((int)_info.Id,LogLevelType.Info, "Email Sent Successfully");
                return true;
            }
            catch (Exception ex)
            {
                //DILog.EmailLogger.WriteLog((int)_info.Id, LogLevelType.Exception, string.Empty, ex);
                throw;
            }
            finally
            {
                mMailMessage = null;
                mSmtpClient = null;
            }
        }

        public bool SendWithAttachment(DomainModel.EmailInfo _info)
        {
            MailMessage mMailMessage = null;
            SmtpClient mSmtpClient = null;
            try
            {
                mMailMessage = new MailMessage();
                mMailMessage.From = new MailAddress(_info.From);

                //TO
                if (!string.IsNullOrWhiteSpace(_info.To))
                {
                    string[] toAddlist = _info.To.Split(',');
                    foreach (string tmpToAddress in toAddlist)
                    {
                        if (!string.IsNullOrWhiteSpace(tmpToAddress) && !string.IsNullOrWhiteSpace(tmpToAddress.Trim()))
                        {
                            string toMail = EmailExclusionListCheck(tmpToAddress.Trim());
                            if (!string.IsNullOrWhiteSpace(toMail))
                                mMailMessage.To.Add(new MailAddress(toMail));
                        }
                    }
                }

                //CC
                if (!string.IsNullOrWhiteSpace(_info.Cc))
                {
                    string[] toAddlist = _info.Cc.Split(',');
                    foreach (string tmpCCAddress in toAddlist)
                    {
                        if (!string.IsNullOrWhiteSpace(tmpCCAddress) && !string.IsNullOrWhiteSpace(tmpCCAddress.Trim()))
                        {
                            string ccMail = EmailExclusionListCheck(tmpCCAddress.Trim());
                            if (!string.IsNullOrWhiteSpace(ccMail))
                                mMailMessage.CC.Add(new MailAddress(ccMail.Trim()));
                        }
                    }
                }

                //BCC
                if (!string.IsNullOrWhiteSpace(_info.Bcc))
                {
                    string[] toAddlist = _info.Bcc.Split(',');
                    foreach (string tmpBCcAddress in toAddlist)
                    {
                        if (!string.IsNullOrWhiteSpace(tmpBCcAddress) && !string.IsNullOrWhiteSpace(tmpBCcAddress.Trim()))
                        {
                            string bccMail = EmailExclusionListCheck(tmpBCcAddress.Trim());
                            if (!string.IsNullOrWhiteSpace(bccMail))
                                mMailMessage.Bcc.Add(new MailAddress(bccMail.Trim()));
                        }
                    }
                }

                mMailMessage.Subject = _info.Subject;
                mMailMessage.Body = _info.Body;
                mMailMessage.IsBodyHtml = _info.IsHtml;

                Attachment attach = new Attachment(_info.Attachment);
                mMailMessage.Attachments.Add(attach);
                mMailMessage.Priority = GetPriority(_info.Priority);

                mSmtpClient = new SmtpClient();

                mSmtpClient.Host = ServerConfig.HostName;
                mSmtpClient.Port = int.Parse(ServerConfig.PortNumber);
                mSmtpClient.EnableSsl = ServerConfig.EnableSSL;

                if (!string.IsNullOrWhiteSpace(ServerConfig.UserName) && !string.IsNullOrWhiteSpace(ServerConfig.Password))
                    mSmtpClient.Credentials = new System.Net.NetworkCredential(ServerConfig.UserName, ServerConfig.Password);
                else
                    mSmtpClient.UseDefaultCredentials = false;

                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                if (mMailMessage != null && (mMailMessage.To == null || mMailMessage.To.Count <= 0))
                // &&
                //(mMailMessage.CC == null || mMailMessage.CC.Count <= 0) && (mMailMessage.Bcc == null || mMailMessage.Bcc.Count <= 0))
                {
                    return true;
                }

                mSmtpClient.Send(mMailMessage);

                DILog.EmailLogger.WriteLog((int)_info.Id, LogLevelType.Info, "Email Sent Successfully", null);
                return true;
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog((int)_info.Id, LogLevelType.Exception, string.Empty, ex);
                throw;
            }
            finally
            {
                mMailMessage = null;
                mSmtpClient = null;
            }
        }

        private string EmailExclusionListCheck(string emailId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(emailId) && !string.IsNullOrWhiteSpace(this.EmailExclusionList))
                {
                    var exclusionList = this.EmailExclusionList.ToLower().Split(',').ToList();

                    if (exclusionList.Any(a => a.Trim() == emailId.Trim().ToLower()))
                    {
                        if (!string.IsNullOrWhiteSpace(this.EmailExlcusionReplacer))
                        {
                            emailId = this.EmailExlcusionReplacer;
                        }
                        else
                        {
                            emailId = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                emailId = "";
            }
            return emailId;
        }
    }
}
