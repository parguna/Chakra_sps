using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DI.Chakra.EntityModel;

namespace DI.Chakra.Logger
{
    public class UILogger
    {
        public void WriteLog(long? userId,string UserName, LogLevelType logLevel, string messageText, Exception ex)
        {
            try
            {
                UILog uIlog = new UILog();
                uIlog.UserName = UserName;
                uIlog.UserId = userId;
                uIlog.IP = GetIpAddress();
                uIlog.LogLevel = logLevel.ToString();
                uIlog.Message = messageText;
                uIlog.LogDate = DateTime.Now; 
                if (null != ex)
                {
                    uIlog.ExceptionMessage = ex.Message;
                    uIlog.ExceptionSource = ex.Source;
                    uIlog.StackTrace = ex.StackTrace;
                    if (null != ex.InnerException)
                        uIlog.InnerExceptionMessage = string.Format(@"Source={0}\nStackTrace={1}\nMessage={2}", ex.InnerException.Source,
                            ex.InnerException.StackTrace, ex.InnerException.Message);
                }

                DILog.WriteUILogToDB(uIlog);
            }
            catch (Exception)
            {
                //Swallow Exception
            }
        }

        public void WriteLog(long? userId, LogLevelType logLevel, string messageText, Exception ex)
        {
            try
            {
                UILog uIlog = new UILog(); 
                uIlog.UserId = userId;
                uIlog.IP = GetIpAddress();
                uIlog.LogLevel = logLevel.ToString();
                uIlog.Message = messageText;
                uIlog.LogDate = DateTime.Now; 
                if (null != ex)
                {
                    uIlog.ExceptionMessage = ex.Message;
                    uIlog.ExceptionSource = ex.Source;
                    uIlog.StackTrace = ex.StackTrace;
                    if (null != ex.InnerException)
                        uIlog.InnerExceptionMessage = string.Format(@"Source={0}\nStackTrace={1}\nMessage={2}", ex.InnerException.Source,
                            ex.InnerException.StackTrace, ex.InnerException.Message);
                }

                DILog.WriteUILogToDB(uIlog);
            }
            catch (Exception)
            {
                //Swallow Exception
            }
        }
        public string  GetIpAddress()
        {
           return Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
        }
        public void WriteLog(long? userId,  LogLevelType loglevel, string messageText)
        {
            this.WriteLog(userId, loglevel, messageText, null);
        }

        
    }
}
