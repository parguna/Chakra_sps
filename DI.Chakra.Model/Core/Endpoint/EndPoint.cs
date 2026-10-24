using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_EndpointSystem")]
    public class EndPoint : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string Description { get; set; }
        public string TenantId { get; set; }
        public Nullable<long> EndpointSystemTypeId { get; set; }
        public string DisplayName { get; set; }
        public long? OwnerUserId { get; set; }        
        public string ApplicationCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsTrustedSource { get; set; }
        public bool IsPasswordResetAllowed { get; set; }
        public bool IncludeInAttestation { get; set; }
        public string UserObjectName { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [XmlIgnore]
        [ForeignKey("OwnerUserId")]
        public virtual User Owner { get; set; }
        public virtual IList<AttributeMappingColumns> ListAttributeMappingColumns { get; set; }

        [XmlIgnore]
        [ForeignKey("EndpointSystemTypeId")]
        public virtual EndPointSystemType EndPointSystemType { get; set; }
     
    }



}
