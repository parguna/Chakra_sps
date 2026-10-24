using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAPEP_RoleInfoHistory")]
    public class SAPEPRoleInfoHistory : EntityBase,ICreatable,IUpdatable
    {
        public int HistoryId { get; set; }

        public Nullable<int> EndPointSystemId { get; set; }

        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public string RoleDescription { get; set; }

        public string Extar1 { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string RecordStatus { get; set; }

        public int? Version { get; set; }
    }
}
