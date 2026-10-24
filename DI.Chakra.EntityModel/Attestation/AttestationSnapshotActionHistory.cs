using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_AttestationSnapshotAction_History")]
    public class AttestationSnapshotActionHistory : EntityBase
    {
        public long Version { get; set; }
        public long AttestationSnapshotActionId { get; set; }
        public long UserId { get; set; }
        public long TaskId { get; set; }
        public bool Approve { get; set; }
        public bool Revoke { get; set; }
        public string Comment { get; set; }
        public Nullable<DateTime> EffectiveDate { get; set; }
        public Nullable<long> ReviewerId { get; set; }        
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
