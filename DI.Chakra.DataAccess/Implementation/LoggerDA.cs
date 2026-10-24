using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Model;
using DI.Chakra.Infrastructure;
using System.Collections;
using DI.Chakra.DataAccess;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;

namespace DI.Chakra.DataAccess
{
    public class LoggerDA
    {

        //public IList<UILog> GetUILog()
        //{
        //    try
        //    {
        //        using (IACMDBContext c = new IACMDBContext())
        //        {
        //            return c.UILog.OrderByDescending(x => x.LogDate).ToList();

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the Ui Logs", ex);
        //        throw ex;
        //    }
        //}
        public IList<ServiceLog> GetLog(bool Initial)
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                      //For Show All Logs
                        if (Initial == true)
                            return c.ServiceLog.OrderByDescending(x => x.LogDate).Take(100).ToList();
                        else
                            return c.ServiceLog.OrderByDescending(x => x.LogDate).ToList(); 
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the Discovery Logs", ex);
                throw ex;
            }
        }
        
        public IList<EndPointSystemTypeLog> getEndpoitSystemType()
        {
            using (IACMDBContext oIACMDBContext = new IACMDBContext())
            {
                return (from epsys in oIACMDBContext.EndpointSystemTypes
                        select new EndPointSystemTypeLog
                        {
                            EndpointSystemTypeId = epsys.Id,
                            EndpointSystemTypeName = epsys.DisplayName
                        }).ToList();
            }
        }


        public List<ServiceLog> GetEmailLogger(string Loglevel)
        {
            List<ServiceLog> objEmailLog = new List<ServiceLog>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    objEmailLog = _context.ServiceLog.Where(ei => (ei.ReferenceType == LogReferenceType.Email && ei.LogLevel == Loglevel)).ToList();
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
