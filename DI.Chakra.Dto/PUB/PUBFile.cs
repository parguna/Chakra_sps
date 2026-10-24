using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{ 
    public class PUBFile : DtoBase
    {        
        public Nullable<DateTime> FileDateTime { get; set; }
        public string PUBFileStatus { get; set; }
        public string PUBFileSource { get; set; }
        public string PUBFileType { get; set; }
        public string ValidationMsg { get; set; }
        public string Remarks { get; set; }
        public string AdditionalInfo { get; set; }
        public Nullable<DateTime> ReportedDateTime { get; set; }
        public string FileName { get; set; }
        public string ReportFileName { get; set; }
        public string ReportedStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        
    }
}
