using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public interface ILogger
    {
        string LogLevel { get; set; }

        string Message { get; set; }

        string ExceptionMessage { get; set; }

        string StackTrace { get; set; }

        string ExceptionSource { get; set; }

        string InnerExceptionMessage { get; set; }

        DateTime LogDate { get; set; }
    }
}
