using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;


namespace DI.Chakra.Services.EmailingProcessor
{
    public class EmailHandler
    {
        private static IEMailServerProvider _serverProvider;

        static EmailHandler()
        {
            _serverProvider = GetEmailProvider();
        }

        private static IEMailServerProvider GetEmailProvider()
        {
            if (_serverProvider == null)
                return new IACMEmailProvider();

            return _serverProvider;
        }

        public static void Send(EmailInfo _info)
        {
            try
            {
               _serverProvider.Insert(_info);
            }
            catch (Exception)
            {                
                throw;
            }
        }

        public static void Send(IList<EmailInfo> _lstInfo)
        {
            try
            {
                _serverProvider.Insert(_lstInfo);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void Dispatch(EmailInfo _info)
        {
            try
            {
                _serverProvider.Send(_info);
            }
            catch (EMailServerNotWorkingException esx)
            {
                _info.Status = EmailStatus.FAILED.ToString();
            }
            catch (Exception)
            {
                _info.Status = EmailStatus.FAILED.ToString();               
            }
            _serverProvider.PostSend(_info);
        }

        public static void Dispatch(IList<EmailInfo> _lstInfo)
        {
            List<EmailInfo> _sentList = new List<EmailInfo>();
            List<EmailInfo> _failedList = new List<EmailInfo>();
            try
            {
                foreach (var info in _lstInfo)
                {
                    if (MasterConfigHelper.IsDeploymentInProgress)
                    {
                        DILog.EmailLogger.WriteLog(LogLevelType.Info, $"EmailService - Deployment InProgress");
                        return;
                    }
                    try
                    {
                        _serverProvider.Send(info);

                        info.Status = EmailStatus.SENT.ToString();
                        info.SentDateTime = DateTime.Now;
                        info.LastModifiedDate = DateTime.Now;
                        info.LastModifiedBy = DomainModel.MasterConfigHelper.ServiceUser;
                        _sentList.Add(info);
                    }
                    catch (EMailServerNotWorkingException esx)
                    {
                        info.Status = EmailStatus.FAILED.ToString();
                        info.LastModifiedDate = DateTime.Now;
                        info.LastModifiedBy = DomainModel.MasterConfigHelper.ServiceUser;

                        _failedList.Add(info);

                        //BeforeStopMailService(_sentList, _failedList);

                        DILog.EmailLogger.WriteLog(null, LogLevelType.Exception, "Email server is not responding or not working properly", esx, info.Id, info.Subject);

                        //throw;

                    }
                    catch (Exception esx)
                    {
                        info.Status = EmailStatus.FAILED.ToString();
                        _failedList.Add(info);
                        DILog.EmailLogger.WriteLog(null, LogLevelType.Exception, esx.ToString(), esx, info.Id, info.Subject);
                    }
                }
                _serverProvider.PostSend(_sentList);
                _serverProvider.PostSend(_failedList);
                //if (_sentList.Count > 0)
                //    DILog.EmailLogger.WriteLog(null, LogLevelType.Info, string.Format("Email Successfully Sent : Count {0}", _sentList.Count.ToString()), null, string.Empty);
                if (_failedList.Count > 0)
                    DILog.EmailLogger.WriteLog(null, LogLevelType.Info, string.Format("Failed Sending Email : Count {0}", _failedList.Count.ToString()), null, string.Empty);
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null, LogLevelType.Exception, $"Dispatch email failed",ex,null,ex.Message);
            }
            finally
            {
                _sentList?.Clear();
                _sentList = null;

                _failedList?.Clear();
                _failedList = null;
            }
        }

        private static void BeforeStopMailService(IList<EmailInfo> sentList, IList<EmailInfo> failedList)
        {
            if (sentList.Count > 0)
            {
                _serverProvider.PostSend(sentList);
            }

            if (failedList.Count > 0)
            {
                _serverProvider.PostSend(failedList);
            }
        }

        public static void StartSendingMails( EmailActivityType emailActivityType)
        {
            IList<EmailInfo> lstEmails = null;
            try
            {
                lstEmails = _serverProvider.GetMailsToDispatch(emailActivityType);
                if (0 == lstEmails.Count)
                    return;

                //DILog.EmailLogger.WriteLog(null, LogLevelType.Info, string.Format("Start Sending Emails : Count {0}", lstEmails.Count.ToString()), null, string.Empty);
                Dispatch(lstEmails);
            }
            catch
            {
            }
            finally
            {
                lstEmails?.Clear();
                lstEmails = null;
            }
        }
      
    }
}
