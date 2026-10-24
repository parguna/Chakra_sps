using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class SQLUMDatabaseLoginRoleReportData
    {
        public int Id { get; set; }
        public string LoginName { get; set; }
        public string DatabaseName { get; set; }
        public string RoleName { get; set; }
        public string Permissions { get; set; }
        public int ParentId { get; set; }

    }

}
