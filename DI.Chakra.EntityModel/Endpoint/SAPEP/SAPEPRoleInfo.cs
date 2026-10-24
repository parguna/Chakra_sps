using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAPEP_RoleInfo")]
    public class SAPEPRoleInfo : EntityBase,ICreatable,IUpdatable
    {
        public Nullable<long> EndPointSystemId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string UniqueRoleName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public int? Version { get; set; }
        public bool IsXLUploaded { get; set; }
        public string UserDefinedDesc { get; set; }
        public bool? CanRequest { get; set; }
        public List<SAPEPUserInfo> SAPEPUserInfos { get; set; }

    }
}
