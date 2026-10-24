using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class UILog : DtoBase, ILogger
    {
        public Nullable<long> UserId { get; set; }

        public string LogLevel { get; set; }

        public string UserName { get; set; }

        public string IP { get; set; }

        public string Message { get; set; }

        public string ExceptionMessage { get; set; }

        public string StackTrace { get; set; }

        public string ExceptionSource { get; set; }

        public string InnerExceptionMessage { get; set; }

        public DateTime LogDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
