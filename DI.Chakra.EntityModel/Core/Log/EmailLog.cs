using System; 
using System.ComponentModel.DataAnnotations.Schema; 

namespace DI.Chakra.EntityModel 
{
    [Table("DI_Log_Email")]
    public class EmailLog : EntityBase, ICreatable, IUpdatable
    { 
        public Nullable<long> EmailId { get; set; }
        public string EmailSubject { get; set; }
        public string LogLevel { get; set; }
        public string LogType { get; set; }
        public string Reviewer { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string ExceptionSource { get; set; }
        public string InnerExceptionMessage { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
