using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class AccessRightAuditData
    {
        public string RoleName { get; set; }
        public string MenuName { get; set; }
        public string Action { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? ChangedDate { get; set; }
    }

    public class MasterConfigAuditData
    {
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? ChangedDate { get; set; }
    }

    public class GroupAuditData
    {
        public string GroupName { get; set; }
        public string GroupOwner { get; set; }
        public string Description { get; set; }
        public bool IsDefaultGroup { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? ChangedDate { get; set; }
    }

    public class RoleAuditData
    {
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsDefaultRole { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? ChangedDate { get; set; }
    }

}
