using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_IACM_GroupInfo")]
    public class IACMGroupInfo : EntityBase,ICreatable,IUpdatable, IRoleOwnerExtension
    {
        public long EndPointSystemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
        public List<IACMUserInfo> Users { get; set; }

        public string UserDefinedDisplayName { get; set; }

        public string UserDefinedDesc { get; set; }

        public bool? CanRequest { get; set; }

        public long? Level1Approver { get; set; }

        public long? Level2Approver { get; set; }

        public bool? IsLevel1Group { get; set; }

        public bool? IsLevel2Group { get; set; }
    }
}
