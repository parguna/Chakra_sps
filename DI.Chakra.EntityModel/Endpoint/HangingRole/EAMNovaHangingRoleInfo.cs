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
    [Table("DI_M_EAMNovaHangingRoleInfo")]
    public class EAMNovaHangingRoleInfo : EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }

        [MaxLength(200)]
        public string AccountName { get; set; }
        public AccountType AccountType { get; set; }

        public string ADGroups { get; set; }

        public string FLINSRoles { get; set; }
        public string BIRoles { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
