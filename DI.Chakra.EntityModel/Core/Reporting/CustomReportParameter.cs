using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_CustomReportParameter")]
    public class CustomReportParameter : EntityBase, ICreatable, IUpdatable
    {
        public long CustomReportId { get; set; }
        [ForeignKey("CustomReportId")]
        public CustomReport CustomReport { get; set; }
        public string ParameterName { get; set; }
        public string Description { get; set; }
        public CustomReportParameterVariant ParameterVariant { get; set; }
        public CustomReportParameterTypes ParameterType { get; set; }
        public long? Sequence { get; set; }
        public string DefaultValue { get; set; }
        public string DataFormatString { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; }

    }
}
