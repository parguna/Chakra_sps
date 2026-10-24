using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel 
{
    [Table("DI_PUB_File")]
    public class PUBFile : EntityBase,ICreatable, IUpdatable
    {
        [Column(TypeName = "datetime2")]
        public Nullable<DateTime> FileDateTime { get; set; }
        public string PUBFileStatus { get; set; }
        public string PUBFileSource { get; set; }
        public string PUBFileType { get; set; }
        public string ValidationMsg { get; set; }
        public string Remarks { get; set; }
        public string AdditionalInfo { get; set; }
        [Column(TypeName = "datetime2")]
        public Nullable<DateTime> ReportedDateTime { get; set; }
        public string ReportedStatus { get; set; }
        public string FileName { get; set; }
        public string ReportFileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
