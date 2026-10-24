using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class RoleAssignmentReportData
    {
        public int roleId { get; set; }
        public string roleName { get; set; }
        public string attributeType { get; set; }
        public string attributeValue { get; set; }
        public string GroupName { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
