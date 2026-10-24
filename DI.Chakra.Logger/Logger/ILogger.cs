using System;

namespace DI.Chakra.Logger
{
    public interface IdmsLogger
    {
       void WriteLog(LogLevelType loglevel, string messageText, Exception ex);
       void WriteLog(LogLevelType loglevel, string messageText);



    }
}