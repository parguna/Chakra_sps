using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_Linux_GroupInfo")]
    public class LinuxGroupInfo : IAggregateRoot, IEndpointEntitlementInfo
    {

        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string GroupNumber { get; set; }
        public string GroupName { get; set; }
        public string GroupPassword { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int? Version { get; set; }

        public virtual List<LinuxUserInfo> LinuxUserInfo { get; set; }

        public string EntitlementName
        {
            get { return GroupName; }
        }

        public string EntitlementDesc
        {
            get { return GroupName; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Group; }
        }
    }
}
