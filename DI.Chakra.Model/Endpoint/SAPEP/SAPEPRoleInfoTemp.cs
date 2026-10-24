using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model
{
    [Table("DI_SAPEP_RoleInfoTemp")]
    public class SAPEPRoleInfoTemp : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string RoleUniqueName { get; set; } 
        public string RoleFullName { get; set; }
        public int? Version { get; set; }

    }
}
