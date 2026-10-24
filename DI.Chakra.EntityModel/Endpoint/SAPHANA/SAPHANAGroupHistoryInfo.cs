using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAPHANA_GROUPHISTORYINFO")]
    public class SAPHANAGroupHistoryInfo : EntityBase,ICreatable,IUpdatable
    {
        public Nullable<int> HistoryId { get; set; }

        public Nullable<int> EndPointSystemId { get; set; }

        public string GROUP_CODE { get; set; }

        public string GROUP_DESC { get; set; }

        public Nullable<int> TargetSysId { get; set; } 

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }
    }
}
