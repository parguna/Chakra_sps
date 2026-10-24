using DI.Chakra.Connector;

using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using DI.Chakra.Services.EmailingProcessor;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace DI.Chakra.Services.DiscoveryProcessor
{
    public class DiscoveryService
    {
        readonly Process process;
        private EndPoint m_endpoint;

        public DiscoveryService(Process _process)
        {
            process = _process;

        }

        private void ChangeDiscoveryStatus(SchedulerStatus status, IdmsLogger logger)
        {
            var discoveryProcess = DiscoveryProcess.GetById(process.ProcessTypeKeyId);
            var endpoint = EndPoint.GetById(discoveryProcess.Endpointsystemid);
            var now = DateTime.Now;

            if (status == SchedulerStatus.InProgress)
            {
                process.ProcessStartAt = DateTime.Now;
                process.ProcessEndAt = null;
                process.Status = "InProgress";
            }

            else if (status == SchedulerStatus.Failed)
            {
                process.ProcessEndAt = DateTime.Now;
                process.Status = "Failed";
                SendEmail(discoveryProcess.ProcessName, logger);
                process.Save(MasterConfigHelper.ServiceUser);
                return;
            }
            else if (status == SchedulerStatus.Pending)
                process.Status = "Pending";
            else
            {
                process.ProcessEndAt = now;
                process.LastRunOn = process.ProcessStartAt;
                process.Status = "Completed";
                if (endpoint.IsTrustedSource)
                {
                    // Following region being implemented to make sure that all instances of the HRMS (Trusted Source) endpoint share the same delta date (Last Run On)
                    // i.e. if there are multiple discovery processes e.g. scheduled and ad hoc process, any instance will have the latest delta date to run from  
                    #region [Update Last Run On field for all instances of discovery process of Trusted Source Endpoint]
                    // get all discovery process for this endpoint except current process
                    var allTrustedSourceDiscovery = DiscoveryProcess.GetByEndpointsystemId(endpoint.Id).Where(x => x.Id != discoveryProcess.Id).ToList();

                    // update Last Run On field to curreent value for all instances.
                    foreach (var discProc in allTrustedSourceDiscovery)
                    {
                        var prc = Process.GetByProcessTypeKeyId(discProc.Id, ProcessType.Discovery);
                        if (prc != null)
                        {
                            prc.LastRunOn = process.ProcessStartAt;
                            prc.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                    #endregion
                    if (!discoveryProcess.Default)
                    {
                        DiscoveryProcess defaultDiscoveryProcess = DiscoveryProcess.GetDefaultDiscoveryProcess(endpoint.Id);
                        if (defaultDiscoveryProcess != null)
                            defaultDiscoveryProcess.Save(MasterConfigHelper.ServiceUser);

                    }
                }
            }
            process.Save(MasterConfigHelper.ServiceUser);
        }

        public void Start()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 |  System.Net.SecurityProtocolType.Ssl3 | System.Net.SecurityProtocolType.Tls;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object obj, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };
            List<EmailInfo> emails = new List<EmailInfo>();
            EndPointSystemType epType = null;
            ConnectorBase connector = null;
            ProcessHistory history = null;
            DiscoveryProcess discoveryProcess = null;
            IdmsLogger logger = new DiscoveryLogger(0, 0, EndpointType.None,string.Empty);
            try
            {
                discoveryProcess = DiscoveryProcess.GetById(process.ProcessTypeKeyId);
                if (null == discoveryProcess)
                {
                    logger.WriteLog(LogLevelType.Error, $"DiscoveryProcess not found for ProcessId - {Convert.ToString(process.ProcessTypeKeyId)}");
                    throw new EntityNotFoundException($"DiscoveryProcess not found for ProcessId - {Convert.ToString(process.ProcessTypeKeyId)}");
                }

                m_endpoint = EndPoint.GetByIdIncludingEndPointSystemType(discoveryProcess.Endpointsystemid);
                if(m_endpoint != null && !m_endpoint.IsActive)
                {
                    logger.WriteLog(LogLevelType.Error, $"Endpoint System:{m_endpoint.DisplayName} is Inactive");
                    return;
                }
                epType = EndPointSystemType.GetByEndpointSystemTypeName(m_endpoint.EndpointSystemTypeId.Value);
                EndpointType endpointtype = epType != null ? epType.EndpointSystemTypeName : EndpointType.None;

                logger = new DiscoveryLogger((int)discoveryProcess.Id, discoveryProcess.Endpointsystemid, endpointtype,m_endpoint.DisplayName);


                logger.WriteLog(LogLevelType.Info, $"Changing ProcessId:{Convert.ToString(discoveryProcess.Id)} to InProgress");
                ChangeDiscoveryStatus(SchedulerStatus.InProgress, logger);

                connector = ConnectorFactory.GetConnector(m_endpoint, null, logger);
                connector.DiscoveryProcess = discoveryProcess;
                connector.Process = process;
                logger.WriteLog(LogLevelType.Info, $"Trying to connect {endpointtype.ToString()}");

                if (!connector.Connect())
                {
                    logger.WriteLog(LogLevelType.Info, $"Failed to connect {((EndpointType)m_endpoint.EndpointSystemTypeId.Value).ToString()}");

                    throw new Exception($"Failed to connect {((EndpointType)m_endpoint.EndpointSystemTypeId.Value).ToString()}");
                }

                logger.WriteLog(LogLevelType.Info, $"GetData for ProcessId :{discoveryProcess.Id.ToString()}");
                connector.GetData();

                ChangeDiscoveryStatus(SchedulerStatus.Completed, logger);
                history = GetHistory(process.Id, "Success", "");
                history.Save(MasterConfigHelper.ServiceUser);
               // connector.Disconnect();
                logger.WriteLog(LogLevelType.Info, $"Completed ProcessId:{discoveryProcess.Id.ToString()}");

                logger.WriteLog(LogLevelType.Info, $"Post Discovery Validation Started:{discoveryProcess.Id.ToString()}");

                try
                {

                    emails = XLDataValidator.PostDiscoveryValidation(m_endpoint, logger, discoveryProcess);

                    foreach (var email in emails)
                        EmailHandler.Send(email);

                    logger.WriteLog(LogLevelType.Info, $"Post Discovery Validation Ended:{discoveryProcess.Id.ToString()}");

                }
                catch (Exception ee)
                {
                    logger.WriteLog(LogLevelType.Exception, $"Exception Occured in Post Discovery Validation for the ProcessId:{discoveryProcess.Id.ToString()}", ee);
                }
              
            }
            catch (Exception ex)
            {
                process.ErrorMessage = ex.Message;
                ChangeDiscoveryStatus(SchedulerStatus.Failed, logger);

                logger.WriteLog(LogLevelType.Error, "Discovery Error Traced", ex);

                SaveProcessHistory();
                //throw ex;
            }
            finally
            {
                if(m_endpoint != null)
                {
                    try
                    {
                        //logger.WriteLog(LogLevelType.Info, $"UpdateOffBoardingAccountAction {discoveryProcess?.Id} started");
                        AccountReconcilation.UpdateOffBoardingByEndpointId(m_endpoint.Id);
                       //logger.WriteLog(LogLevelType.Info, $"UpdateOffBoardingAccountAction {discoveryProcess?.Id} completed");
                        GenereateTCodeReportForSAP(m_endpoint);
                        
                    }
                    catch (Exception ex)
                    {
                        logger.WriteLog(LogLevelType.Exception, $"UpdateOffBoardingAccountAction {discoveryProcess?.Id} failed.",ex);
                    }
                }
                discoveryProcess = null;
                connector = null;
                epType = null;
                history = null;
                m_endpoint = null;
                emails?.Clear();
                emails = null;
            }
        }
        private void GenereateTCodeReportForSAP(EndPoint endpoint)
        {
            try
            {
                if (endpoint.EndpointSystemTypeId.HasValue && endpoint.EndpointSystemTypeId.Value == (int)EndpointType.SAPHANA)
                {
                    SAPHANATCodeInfo.GenerateRoleTCodeForReport(endpoint.Id);
                }
                else if (endpoint.EndpointSystemTypeId.HasValue && endpoint.EndpointSystemTypeId.Value == (int)EndpointType.SAP)
                {
                    SAPTCodeInfo.GenerateRoleTCodeForReport(endpoint.Id);
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(endpoint?.Id, LogLevelType.Warning, $"Genereating TCodeReport ForSAP failed", ex);
            }
        }
        private ProcessHistory GetHistory(long processId, string status, string errMessage)
        {
            ProcessHistory history = new ProcessHistory();
            history.ProcessType = ProcessType.Discovery;
            history.ProcessId = processId;
            history.Status = status;
            history.ErrorMessage = errMessage;
            return history;
        }

        private void SaveProcessHistory()
        {
            ProcessHistory ph = new ProcessHistory()
            {
                Id = 0,
                Process = process,
                ProcessId = process.Id,
                ProcessType = process.ProcessType,
                ErrorMessage = process.ErrorMessage,
                ProcessStartAt = process.ProcessStartAt,
                ProcessEndAt = process.ProcessEndAt,
                Status = process.Status,
            };

            ph.Save(MasterConfigHelper.ServiceUser);
        }

        private void SendEmail(string processName, IdmsLogger logger)
        {
            string strMessage = string.Empty;
            StringBuilder strBldr = new StringBuilder();
            DiscoveryProcess discoveryProcess = null;
            DomainModel.MailTemplate template = null;
            StringBuilder subject = null;
            StringBuilder body = null;
            EmailInfo emailInfo = null;
            try
            {
                discoveryProcess = DiscoveryProcess.GetById(process.ProcessTypeKeyId);
                emailInfo = new EmailInfo();
                template = MailTemplate.GetByTemplateName(Constants.DiscoveryJobFailed);
                subject = new StringBuilder(template.MailSubject);
                subject.Replace("@@ENDPOINTNAME@@", m_endpoint.DisplayName);
                subject.Replace("@@PROCESSNAME@@", processName);
                body = new StringBuilder(template.MailContent);
                body.Replace("@@ERRORMESSAGE@@", process.ErrorMessage);
                body.Replace("@@ENDPOINTTYPE@@", m_endpoint.EndPointSystemType.DisplayName);
                body.Replace("@@ENDPOINTNAME@@", m_endpoint.DisplayName);
                body.Replace("@@PROCESSNAME@@", processName);
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = string.Format("{0}", string.Join(",", MasterConfigHelper.FailureNotificationReceivers));
                emailInfo.From = template.MailFrom;
                if(!string.IsNullOrWhiteSpace(template.MailCC))
                    emailInfo.Cc = template.MailCC;
                if (!string.IsNullOrWhiteSpace(template.MailBcc))
                    emailInfo.Bcc = template.MailBcc;
                emailInfo.ActivityType = EmailActivityType.Discovery.ToString();
                EmailHandler.Send(emailInfo);
            }
            catch (Exception ex)
            {
                logger.WriteLog(LogLevelType.Exception, "Failed in composing mail from template", ex);
            }
            finally
            {
                discoveryProcess = null;
                template = null;
                strBldr?.Clear();
                subject?.Clear();
                subject = null;
                strBldr = null;
                body?.Clear();
                body = null;
                emailInfo = null;
            }
        }
    }
}

