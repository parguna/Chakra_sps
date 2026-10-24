using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using System.Net.Mail;

namespace DI.Chakra.Services.EmailingProcessor
{
    public class IACMEmailProvider : IEMailServerProvider
    {
        private EmailSender _sender;

        private EmailSave _save;

        #region IEMailServerProvider Members

        public IACMEmailProvider()
        {
            _save = new EmailSave();
            //TODO: to be moved, because this provider may be used only for inserting mails.
            MailServerConfigInfo config = GetAvailableMailServer();

            if (null == config)
                _sender = null;
            else
                _sender = new EmailSender(config);  // (config.HostName);
        }

        #endregion
        public bool TestConnection(MailServerConfigInfo _config)
        {
            try
            {
                //if (_config == null) return false;

                //if (string.IsNullOrWhiteSpace(_config.HostName))
                //    return false;

                //_sender = new EmailSender(_config); // (_config.HostName);

                ////Read from table
                //_sender.Send(new EmailInfo()
                //{
                //    From = _config.EmailID,
                //    To = _config.EmailID
                //});
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public IList<MailServerConfigInfo> GetAllMailServerInfo()
        {
            return _save.GetServerInfo();
        }
        public MailServerConfigInfo GetAvailableMailServer()
        {
            IList<MailServerConfigInfo> lst = GetAllMailServerInfo();

            foreach (MailServerConfigInfo objMailServerConfList in lst)
            {
                if (TestConnection(objMailServerConfList) == true)
                    return objMailServerConfList;
            }
            return null;
        }

        #region IEMailServerProvider Members

        private void retry(EmailInfo mailInfo)
        {

            if (mailInfo.NoOfAttempts >= 3)
                throw new EMailServerNotWorkingException();

            Send(mailInfo);
        }

        public bool Send(EmailInfo mailInfo, System.Net.Mail.Attachment attachment = null)
        {
            try
            {
                if (null == _sender)
                    throw new EMailServerNotWorkingException();

                _sender.EmailExclusionList = DomainModel.ConfigurationMaster.GetByConfigKey("EmailExclusionList", Product.IM).ConfigValue;
                _sender.EmailExlcusionReplacer = DomainModel.ConfigurationMaster.GetByConfigKey("EmailExclusionReplacer", Product.IM).ConfigValue;

                _sender.Send(mailInfo, attachment);


            }
            //TODO: Catch specific exception. 1. Wrong mail format exception 2. Server not working exception
            catch (SmtpException smtpEx)
            {
                SmtpStatusCode statusCode = smtpEx.StatusCode;

                if (statusCode == SmtpStatusCode.MailboxBusy || statusCode == SmtpStatusCode.MailboxUnavailable || statusCode == SmtpStatusCode.TransactionFailed
                    || statusCode == SmtpStatusCode.LocalErrorInProcessing || statusCode == SmtpStatusCode.ServiceNotAvailable || statusCode == SmtpStatusCode.MustIssueStartTlsFirst ||
                    statusCode == SmtpStatusCode.ServiceClosingTransmissionChannel)
                {
                    mailInfo.NoOfAttempts += 1;

                    MailServerConfigInfo config = GetAvailableMailServer();

                    if (config == null)
                        throw new EMailServerNotWorkingException();

                    _sender = new EmailSender(config);  // (config.HostName);
                    retry(mailInfo);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        public bool SendWithAttachment(EmailInfo mailInfo)
        {
            try
            {
                if (null == _sender)
                    throw new EMailServerNotWorkingException();

                _sender.SendWithAttachment(mailInfo);
            }

            //TODO: Catch specific exception. 1. Wrong mail format exception 2. Server not working exception
            catch (System.Net.Mail.SmtpException)
            {
                mailInfo.NoOfAttempts += 1;

                MailServerConfigInfo config = GetAvailableMailServer();

                if (config == null)
                    throw new EMailServerNotWorkingException();

                _sender = new EmailSender(config);  // (config.HostName);
                retry(mailInfo);
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        #endregion

        #region IEMailServerProvider Members

        public void PostSend(EmailInfo _mailInfo)
        {
            _save.Update(_mailInfo);

            IList<DomainModel.EmailInfo> lst = new List<DomainModel.EmailInfo>();

            if (string.Compare(_mailInfo.Status, EmailStatus.SENT.ToString(), true) == 0)
                _save.MoveSentItems(lst);

        }

        public void PostSend(IList<EmailInfo> _lstMailInfo)
        {
            if (_lstMailInfo == null || _lstMailInfo.Count <= 0)
                return;
            foreach (EmailInfo email in _lstMailInfo)
            {
                _save.Update(email);
            }
            IList<EmailInfo> objEmailInfo = _lstMailInfo.Where(EL => EL.Status == EmailStatus.SENT.ToString()).ToList();

            _save.MoveSentItems(objEmailInfo);


        }

        public IList<EmailInfo> GetMailsToDispatch(EmailActivityType emailActivityType)
        {
            _save = new EmailSave();

            return _save.GetEmailsToSend(emailActivityType);
        }
        #endregion

        #region IEMailServerProvider Members


        public void Insert(EmailInfo _mailInfo)
        {
            _save.Insert(_mailInfo);

        }

        public void Insert(IList<EmailInfo> _lstMailInfo)
        {
            _save.Insert(_lstMailInfo);
        }

        #endregion
    }

}
