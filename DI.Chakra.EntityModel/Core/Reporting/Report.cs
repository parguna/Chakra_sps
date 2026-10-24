using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_Report")]
    public class Report: EntityBase,ICreatable,IUpdatable
    {
        public ReportType Type { get; set; }
        public string Format { get; set; }
        public Process process { get; set; }
        public string Name { get; set; }
        public List<ReportAttributes> ReportAttributes { get; set; }
        public string ReportUrl { get; set; }
        public long UserId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        
        public string MailTo { get; set; }
        public string MailCC { get; set; }
    }
}
