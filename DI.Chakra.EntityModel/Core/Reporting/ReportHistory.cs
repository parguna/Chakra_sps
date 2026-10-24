using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_ReportHistory")]
    public class ReportHistory : EntityBase, ICreatable, IUpdatable
    {
        public long ReportId { get; set; }
        [ForeignKey("ReportId")]
        public Report ReportSchedule { get; set; }
        public byte[] Report { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public ReportStatus Status { get; set; }
    }

}
