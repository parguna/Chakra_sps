using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_INH_Permission")]
    public class InhousePermission : IXLData, IEndpointEntitlementInfo
    {
        public InhousePermission()
        {
            this.INHUsers = new List<InhouseUserInfo>();

        }
        [Key]
        public long Id
        {
            get;
            set;
        }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string InhouseSystemCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("EndPointSystemId")]
        public virtual EndPoint EndPoint { get; set; }
        public virtual List<InhouseUserInfo> INHUsers { get; set; }

        [NotMapped]
        public DateTime? ValidFrom { get; set; }
        [NotMapped]
        public DateTime? ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }

        public string EntitlementName
        {
            get { return Name; }
        }

        public string EntitlementDesc
        {
            get { return Description; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Group; }
        }

    }
}
