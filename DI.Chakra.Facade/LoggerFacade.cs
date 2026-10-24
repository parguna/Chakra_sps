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

namespace DI.Chakra.Facade
{
    public class LoggerFacade
    {
        public LoggerDA loggerDA { get; set; }
        public LoggerFacade()
        {
            loggerDA = new LoggerDA();
        }

        //public IList<UILog> GetUILog()
        //{
        //    try
        //    {
        //        return loggerDA.GetUILog();
        //    }

        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
       
        public IList<ServiceLog> GetLogWithoutDate(LogReferenceType logRefType,LogLevelType logleveltype)
        {
            try
            {
                if (logleveltype == LogLevelType.All)
                {
                    return loggerDA.GetLog(false).Where(x => x.ReferenceType == logRefType).ToList();
                }
                else
                {
                    return loggerDA.GetLog(false).Where(x => x.ReferenceType == logRefType && (LogLevelType)Enum.Parse(typeof(LogLevelType), x.LogLevel) == logleveltype).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }       
         public IList<ServiceLog> GetLogWithDate(LogReferenceType? logRefType,DateTime? dtFrom,DateTime? dtTo,LogLevelType logleveltype)
        {
            try
            {
                if (logleveltype == LogLevelType.All)
                {
                    return loggerDA.GetLog(false).Where(x => x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value).ToList();
                }
                else
                {
                    return loggerDA.GetLog(false).Where(x => x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value && (LogLevelType)Enum.Parse(typeof(LogLevelType), x.LogLevel) == logleveltype).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public IList<ServiceLog> GetAllLog(bool Initial,LogLevelType logleveltype)
        {
            try
            {
                if (logleveltype == LogLevelType.All)
                {
                    return loggerDA.GetLog(Initial);
                }
                else
                {
                    return loggerDA.GetLog(Initial).Where(x => (LogLevelType)Enum.Parse(typeof(LogLevelType), x.LogLevel) == logleveltype).ToList();
                 
                }
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


        public List<ServiceLog> GetEmailLogger(string Loglevel)
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
