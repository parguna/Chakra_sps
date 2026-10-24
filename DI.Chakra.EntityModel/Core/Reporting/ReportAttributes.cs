using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_ReportAttributes")]
    public class ReportAttributes : EntityBase, ICreatable, IUpdatable
    {
        public long ReportId { get; set; }
        [ForeignKey("ReportId")]
        public Report ReportSchedule { get; set; }
        public string ParamKey { get; set; }
        public string ParamValue { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
