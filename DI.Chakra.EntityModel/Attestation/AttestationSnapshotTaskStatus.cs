using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AttestationSnapshotTaskStatus")]
    public class AttestationSnapshotTaskStatus : EntityBase, ICreatable, IUpdatable
    {
        public long AttestationId { get; set; }
        public long CampaignId { get; set; }
        public long TaskId { get; set; }    
        public bool UnSavedChangesAvailable { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }        
    }
}
