using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeepIdentity.Model;
using DeepIdentity.Infrastructure;
using System.Collections;
using DeepIdentity.DataAccess;

using DeepIdentity.Infrastructure.Enums;

namespace DeepIdentity.Facade
{
    public class LoggerFacade
    {
        public LoggerDA loggerDA { get; set; }
        public LoggerFacade()
        {
            loggerDA = new LoggerDA();
        }

        public IList<AttestationLog> GetAttestationLog()
        {
            try
            {
                return loggerDA.GetAttestationLog();
            }
           
            catch (Exception)
            {

                throw;
            }
        }

        public IList<ProvisioningLog> GetProvisioningLog()
        {
            try
            {
                return loggerDA.GetProvisioningLog();
            }

            catch (Exception)
            {

                throw;
            }
        }


        public IList<DiscoveryLog> GetDiscoveryLog()
        {
            try
            {
                return loggerDA.GetDiscoveryLog();
            }

            catch (Exception)
            {

                throw;
            }
        }



        public IList<EmailLog> GetEmailLog()
        {
            try
            {
                return loggerDA.GetEmailLog();
            }

            catch (Exception)
            {

                throw;
            }
        }



        public IList<SchedulerLog> GetSchedulerLog()
        {
            try
            {
                return loggerDA.GetSchedulerLog();
            }

            catch (Exception)
            {

                throw;
            }
        }


        public IList<UILog> GetUILog()
        {
            try
            {
                return loggerDA.GetUILog();
            }

            catch (Exception)
            {

                throw;
            }
        }



        public void SaveLogIntoDB(ILogger LogInfo)
        {
            try
            {
                loggerDA.SaveLogIntoDB(LogInfo);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<EndPointSystemTypeLog> getEndpoitSystemType()
        {
            try
            {
                return loggerDA.getEndpoitSystemType();
            }
            catch (Exception)
            {
                throw;
            }
        }
        
       
        public List<EmailLog> GetEmailLogger(string Loglevel)
        {
            try
            {
                return loggerDA.GetEmailLogger(Loglevel);
            }
            catch
            {
                throw;
            }
        }
    }
}
