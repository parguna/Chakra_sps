using DI.Chakra.DBClient;
using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DI.Chakra.Logger
{


    public class DILog
    {
        static bool? _TurnOffDebugStep { get; set; }
        static bool TurnOffDebugStep { get; set; }

        static DiscoveryLogger discoveryLogger;
        static ProvisioningLogger provisioningLogger;
        public static SchedulerLogger SchedulerLogger;
        public static AttestationLogger AttestationLogger;
        public static EmailLogger EmailLogger;
        public static WorkflowLogger WorkflowLogger;
        public static UILogger UILogger;
        public static ComplianceLogger ComplianceLogger;
        public static HouseKeepingLogger HouseKeepingLogger;
        public static GeneralLogger GeneralLogger;
        public static ReportingLogger ReportingLogger;
        public static PasswordChangeLogger PasswordChangeLogger;
        public static PasswordResetLogger PasswordResetLogger;
        public static AccountUnlockLogger AccountUnlockLogger;
        public static AuthenticationLogger AuthenticationLogger;

        public static DiscoveryLogger DiscoveryLogger
        {
            get
            {
                if (discoveryLogger == null)
                    discoveryLogger = new DiscoveryLogger(0, 0, EndpointType.None,"");
                return discoveryLogger;
            }
            set
            {
                discoveryLogger = value;
            }
        }

        public static ProvisioningLogger ProvisioningLogger
        {
            get
            {
                if (provisioningLogger == null)
                    provisioningLogger = new ProvisioningLogger(null, null,null,string.Empty);
                return provisioningLogger;
            }
            set
            {
                provisioningLogger = value;
            }
        }

        #region Private Members and Constructor

        static DILog()
        {
            discoveryLogger = new DiscoveryLogger(0, 0, EndpointType.None,string.Empty);
            provisioningLogger = new ProvisioningLogger(null, null,null,string.Empty);
            SchedulerLogger = new SchedulerLogger();
            AttestationLogger = new AttestationLogger();
            EmailLogger = new EmailLogger();
            WorkflowLogger = new WorkflowLogger();
            UILogger = new UILogger();
            ComplianceLogger = new ComplianceLogger();
            HouseKeepingLogger = new HouseKeepingLogger();
            GeneralLogger = new GeneralLogger();
            ReportingLogger = new ReportingLogger();
            PasswordChangeLogger = new PasswordChangeLogger();
            PasswordResetLogger = new PasswordResetLogger(null, null, string.Empty, null);
            AccountUnlockLogger = new AccountUnlockLogger(null,null,string.Empty,null);
            AuthenticationLogger = new AuthenticationLogger();
            SetDebugStep();
        }

        static void SetDebugStep()
        {
            try
            {
                if (_TurnOffDebugStep != null)
                    return;

                using (IACMDBContext _context = new IACMDBContext())
                {
                    var configurationMaster = _context.ConfigurationMastersIACM.Where(p => p.ConfigKey == "TurnOffDebugStep").FirstOrDefault();
                    if (configurationMaster != null)
                    {
                        _TurnOffDebugStep = Convert.ToBoolean(configurationMaster.ConfigValue);
                        TurnOffDebugStep = Convert.ToBoolean(_TurnOffDebugStep);
                    }
                    else
                    {
                        TurnOffDebugStep = true;
                    }
                }
            }
            catch (Exception e)
            {
                //Swallow exception
            }
        }

        #endregion

        #region Public Methods

        #region UI Methods

        public static void WriteProvisioningLogToDB(ProvisioningLog provisioningLog)
        {
            if (TurnOffDebugStep && provisioningLog.LogLevel == LogLevelType.DebugStep.ToString())
                return;

            using (IACMDBContext _context = new IACMDBContext())
            {
                provisioningLog.LogDate = DateTime.Now;
                provisioningLog.CreatedBy = "System";
                _context.ProvisioningLog.Add(provisioningLog);
                _context.SaveChanges();
            }
        }

        public static void WriteAttestationLogToDB(AttestationLog attestationLog)
        {
            if (TurnOffDebugStep && attestationLog.LogLevel == LogLevelType.DebugStep.ToString())
                return;

            using (IACMDBContext _context = new IACMDBContext())
            {
                attestationLog.CreatedDate = DateTime.Now;
                attestationLog.CreatedBy = "System";
                _context.AttestationLog.Add(attestationLog);
                _context.SaveChanges();
            }
        }

        public static void WriteDiscoveryLogToDB(DiscoveryLog discoveryLog)
        {
            if (TurnOffDebugStep && discoveryLog.LogLevel == LogLevelType.DebugStep.ToString())
                return;

            using (IACMDBContext _context = new IACMDBContext())
            {
                discoveryLog.CreatedDate = DateTime.Now;
                discoveryLog.CreatedBy = "System";
                _context.DiscoveryLog.Add(discoveryLog);
                _context.SaveChanges();
            }
        }

        public static void WriteEmailLogToDB(EmailLog emailLog)
        {
            if (TurnOffDebugStep && emailLog.LogLevel == LogLevelType.DebugStep.ToString())
                return;

            using (IACMDBContext _context = new IACMDBContext())
            {
                emailLog.CreatedDate = DateTime.Now;
                emailLog.CreatedBy = "System";
                _context.EmailLog.Add(emailLog);
                _context.SaveChanges();
            }
        }
        public static void WriteServiceLogToDB(ServiceLog serviceLog)
        {
            if (TurnOffDebugStep && serviceLog.LogLevel == LogLevelType.DebugStep.ToString())
                return;

            using (IACMDBContext _context = new IACMDBContext())
            {
                serviceLog.CreatedDate = DateTime.Now;
                serviceLog.CreatedBy = "System";
                _context.ServiceLog.Add(serviceLog);
                _context.SaveChanges();
            }
        }

        public static void WriteUILogToDB(UILog uILog)
        {
            //TODO - Move DomainApiClient in Web project to seperate Library and replace DomainApiClient for below code.
            string baseUrl = AppSettingsHelper.DomainApiBaseUrl;

            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null
                && System.Web.HttpContext.Current.Session["LoginUserName"] != null && System.Web.HttpContext.Current.Session["LoginPassword"] != null)
            {
                var credentials = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", System.Web.HttpContext.Current.Session["LoginUserName"],
                    System.Web.HttpContext.Current.Session["LoginPassword"]));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
            }

            HttpResponseMessage response = httpClient.PostAsJsonAsync("UILog", uILog).Result;

            response.EnsureSuccessStatusCode();
        }

        internal static ServiceLog GetServiceLog(long? referenceId, LogReferenceType logReferenceType, LogLevelType loglevel, string messageText, Exception ex)
        {
            ServiceLog serviceLog = new ServiceLog();
            serviceLog.ReferenceId = referenceId;
            serviceLog.ReferenceType = logReferenceType;
            serviceLog.LogLevel = loglevel.ToString();
            serviceLog.Message = messageText;
            serviceLog.LogDate = DateTime.Now;

            if (null != ex)
            {
                serviceLog.ExceptionMessage = ex.Message;
                serviceLog.ExceptionSource = ex.Source;
                serviceLog.StackTrace = ex.StackTrace;
                if (null != ex.InnerException)
                    serviceLog.InnerExceptionMessage = string.Format(@"Source={0}\nStackTrace={1}\nMessage={2}", ex.InnerException.Source,
                        ex.InnerException.StackTrace, ex.InnerException.Message);
            }

            return serviceLog;
        }
        internal static ProvisioningLog GetProvisioningLog(long? accessRequestId, long? endpointId, LogLevelType loglevel, string messageText, Exception ex)
        {

            ProvisioningLog log = new ProvisioningLog();
            log.LogLevel = loglevel.ToString();
            log.Message = messageText;

            log.EndpointSystemId = endpointId;
            log.AccessRequestId = accessRequestId;

            if (null != ex)
            {
                log.ExceptionMessage = ex.Message;
                log.ExceptionSource = ex.Source;
                log.StackTrace = ex.StackTrace;
                if (null != ex.InnerException)
                {
                    var innerEx = ex.InnerException;
                    log.InnerExceptionMessage = string.Format(@"Source={0}\nStackTrace={1}\nMessage={2}", innerEx.Source, innerEx.StackTrace, innerEx.Message);

                    if (null != innerEx.InnerException)
                    {
                        innerEx = innerEx.InnerException;
                        log.InnerExceptionMessage = string.Format(@"Source={0}\nStackTrace={1}\nMessage={2}", innerEx.Source, innerEx.StackTrace, innerEx.Message);

                        if (null != innerEx.InnerException)
                        {
                            innerEx = innerEx.InnerException;
                            log.InnerExceptionMessage = string.Format(@"Source={0}\nStackTrace={1}\nMessage={2}", innerEx.Source, innerEx.StackTrace, innerEx.Message);
                        }
                    }
                }
            }

            return log;
        }

        internal static AttestationLog GetAttestationLog(long? attestationId, string attestationName, string reviewer, string reviewerType, string reviewee,
             LogLevelType loglevel, string messageText, Exception ex)
        {

            var log = new AttestationLog();
            log.LogLevel = loglevel.ToString();
            log.Message = messageText;
            log.LogDate = DateTime.Now;
            log.Reviewee = reviewee;
            log.Reviewer = reviewer;
            log.ReviewerType = reviewerType;
            log.AttestationName = attestationName;

            if (null != ex)
            {
                log.ExceptionMessage = ex.Message;
                log.ExceptionSource = ex.Source;
                log.StackTrace = ex.StackTrace;
                if (null != ex.InnerException)
                    log.InnerExceptionMessage = string.Format(@"Source={0}\nStackTrace={1}\nMessage={2}", ex.InnerException.Source,
                        ex.InnerException.StackTrace, ex.InnerException.Message);
            }

            if (attestationId.HasValue)
                log.AttestationId = attestationId.Value;
            else
                log.AttestationId = null;



            return log;
        }

        internal static DiscoveryLog GetDiscoveryLog(long? EndpointSystemId, EndpointType EndpointSystemType, DiscoveryObjectType DiscoveryObjectType,
            LogLevelType loglevel, string messageText, Exception ex)
        {
            var log = new DiscoveryLog();
            log.LogLevel = loglevel.ToString();
            log.Message = messageText;
            log.LogDate = DateTime.Now;
            log.EndpointSystemType = EndpointSystemType;
            log.DiscoveryObjectType = DiscoveryObjectType;

            if (null != ex)
            {
                log.ExceptionMessage = ex.Message;
                log.ExceptionSource = ex.Source;
                log.StackTrace = ex.StackTrace;
                if (null != ex.InnerException)
                    log.InnerExceptionMessage = string.Format(@"Source={0}\nStackTrace={1}\nMessage={2}", ex.InnerException.Source,
                        ex.InnerException.StackTrace, ex.InnerException.Message);
            }

            if (EndpointSystemId.HasValue)
                log.EndpointSystemId = EndpointSystemId.Value;

            return log;
        }


        internal static EmailLog GetEmailLog(long? emailProcessorId, LogLevelType loglevel, string messageText, Exception ex, long? emailId, string subject)
        {
            var log = new EmailLog();
            log.LogLevel = loglevel.ToString();
            log.Message = messageText;
            log.EmailId = emailId;
            log.EmailSubject = subject;
            if (emailProcessorId.HasValue)
                log.EmailId = emailProcessorId.Value;

            if (null != ex)
            {
                log.ExceptionMessage = ex.Message;
                log.ExceptionSource = ex.Source;
                log.StackTrace = ex.StackTrace;
                if (null != ex.InnerException)
                    log.InnerExceptionMessage = string.Format(@"Source={0}\nStackTrace={1}\nMessage={2}", ex.InnerException.Source,
                        ex.InnerException.StackTrace, ex.InnerException.Message);
            }


            return log;
        }

        internal static EmailLog GetEmailLog(long? emailProcessorId, LogLevelType loglevel, string messageText, Exception ex  )
        {
            var log = new EmailLog();
            log.LogLevel = loglevel.ToString();
            log.Message = messageText;
            
            if (emailProcessorId.HasValue)
                log.EmailId = emailProcessorId.Value;

            if (null != ex)
            {
                log.ExceptionMessage = ex.Message;
                log.ExceptionSource = ex.Source;
                log.StackTrace = ex.StackTrace;
                if (null != ex.InnerException)
                    log.InnerExceptionMessage = string.Format(@"Source={0}\nStackTrace={1}\nMessage={2}", ex.InnerException.Source,
                        ex.InnerException.StackTrace, ex.InnerException.Message);
            }


            return log;
        }


        #endregion

        #region Service Methods




        #endregion

        #endregion

        #region  Helper Methods

        #region Public Methods
        public static string GetExceptionText(Exception ex)
        {
            string msg = string.Empty;

            if (ex == null)
                return msg;
            msg = string.Format("<Message>{0}</Message><StackTrace>{1}</StackTrace>", ex.Message, ex.StackTrace);

            return msg;
        }

        public static void WriteUILogToDB(object eRROR, string v, Exception ex)
        {
            throw new NotImplementedException();
        }
        #endregion


        #endregion
    }
}
