using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema; 
using DI.Chakra.Infrastructure;

namespace DI.Chakra.EntityModel
{
    [TableInfo("DI_Office365_RoleAssignmentPolicy", "DI_Office365_RoleAssignmentPolicy")]
    [Table("DI_Office365_RoleAssignmentPolicy")]
    public class Office365RoleAssignmentPolicy : EntityBase, ICreatable,IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public string Name { get; set; }
        public string PolicyId { get; set; }
        public string DistinguishedName { get; set; }
        public bool? IsDefault { get; set; }
        public string Description { get; set; }
        public string Identity { get; set; }
        public DateTime? WhenCreated { get; set; }
        public bool? IsValid { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }

    }
}
