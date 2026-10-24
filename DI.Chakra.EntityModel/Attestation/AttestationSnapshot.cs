using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AttestationSnapshot")]
    public class AttestationSnapshot : EntityBase, ICreatable, IUpdatable
    {
        public long AttestationCampaignId { get; set; }
        public long UserId { get; set; }
        public AccountType AccountType { get; set; }
        public long KeyId { get; set; }
        public string Entity { get; set; }
        /// <summary>
        /// Endpoint User/Entitlement Name
        /// </summary>
        [MaxLength(1024,ErrorMessage = "Name cannot exceed 1024")]
        public string Name { get; set; }
        /// <summary>
        /// Endpoint User/Entitlement Description
        /// </summary>
        public string Description { get; set; }
        [MaxLength(1024, ErrorMessage = "AccountName cannot exceed 512")]
        public string AccountName { get; set; }
        [MaxLength(1024, ErrorMessage = "DIUserName cannot exceed 512")]
        public string DIUserName { get; set; }
        public string DIUserDisplayName { get; set; }
        public long EndpointId { get; set; }
        public string EndpointName { get; set; }
        public EndpointType EndpointType { get; set; }
        public string ParentEntity { get; set; }
        public Nullable<System.DateTime> LastLogon { get; set; }
        public long Level { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public bool HasRisk { get; set; }
        public bool AccountStatus { get; set; }
        public Nullable<long> RootSnapshotId { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }
        public string Reserved6 { get; set; }
        public string Reserved7 { get; set; }
        public string Reserved8 { get; set; }
        public string Reserved9 { get; set; }
        public string Reserved10 { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        [ForeignKey("AttestationCampaignId")]
        public AttestationCampaign AttestationCampaign { get; set; }
    }
}
