using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAPEP_RoleInfoTemp")]
    public class SAPEPRoleInfoTemp : EntityBase,ICreatable,IUpdatable
    {
        public string RoleUniqueName { get; set; }

        public string RoleFullName { get; set; }

        public int? Version { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


    }
}
