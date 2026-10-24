using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SFB_ExternalAccessPolicy")]
    public class SkypeForBusinessExternalAccessPolicy : EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public string Identity { get; set; }
        public string Description { get; set; }
        public bool EnableFederationAccess { get; set; } = false;
        public bool EnableXmppAccess { get; set; } = false;
        public bool EnablePublicCloudAccess { get; set; } = false;
        public bool EnablePublicCloudAudioVideoAccess { get; set; } = false;
        public bool EnableOutsideAccess { get; set; } = false;
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}
