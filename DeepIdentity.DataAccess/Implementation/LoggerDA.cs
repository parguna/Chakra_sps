using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeepIdentity.Model;
using DeepIdentity.Infrastructure;
using System.Collections;
using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure.Enums;

namespace DeepIdentity.DataAccess
{
    public class LoggerDA
    {
        public IList<AttestationLog> GetAttestationLog()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return c.AttestaionLog.OrderByDescending(x => x.LogDate).ToList();

                //    OrderBy(x => x.GroupName).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Getting the Attestation Logs", ex);
                throw ex;
            }
        }



        public IList<ProvisioningLog> GetProvisioningLog()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return c.ProvisioningLog.OrderByDescending(x => x.LogDate).ToList();
                  
                }
                
                }
            
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Getting the Provisioning Logs", ex);
                throw ex;
            }
        }



        public IList<DiscoveryLog> GetDiscoveryLog()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {

                    return c.DiscoveryLog.OrderByDescending(x => x.LogDate).ToList();

                 

                }
                
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Getting the Discovery Logs", ex);
                throw ex;
            }
        }



        public IList<EmailLog> GetEmailLog()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return c.EmailLog.Take(1000).OrderByDescending(x => x.LogDate).ToList();
          
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Getting the Mail Logs", ex);
                throw ex;
            }
        }




        public IList<SchedulerLog> GetSchedulerLog()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return c.SchedulerLog.OrderByDescending(x => x.LogDate).ToList();
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Getting the Scheduler Logs", ex);
                throw ex;
            }
        }




        public IList<UILog> GetUILog()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return c.UILog.OrderByDescending(x => x.LogDate).ToList();
                  
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Getting the Ui Logs", ex);
                throw ex;
            }
        }



        public void SaveLogIntoDB(ILogger LogInfo)
        {
            try
            {
                if (LogInfo.GetType().Name == "DiscoveryLog")
                {
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        _context.DiscoveryLog.Add((DiscoveryLog)LogInfo);
                        _context.SaveChanges();
                    }
                }
                else if (LogInfo.GetType().Name == "ProvisioningLog")
                {
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        _context.ProvisioningLog.Add((ProvisioningLog)LogInfo);
                        _context.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<EndPointSystemTypeLog> getEndpoitSystemType()
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                return (from epsys in oIACMDBContext.EndpointSystemTypes
                        select new EndPointSystemTypeLog
                        {
                            EndpointSystemTypeId = epsys.EndpointSystemTypeId,
                            EndpointSystemTypeName = epsys.DisplayName
                        }).ToList();
            }
        }
       
   
        public List<EmailLog> GetEmailLogger(string Loglevel)
        {
            List<EmailLog> objEmailLog = new List<EmailLog>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    objEmailLog = _context.EmailLog.Where(ei => (ei.LogLevel == Loglevel)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return objEmailLog;
        }
    }
}
