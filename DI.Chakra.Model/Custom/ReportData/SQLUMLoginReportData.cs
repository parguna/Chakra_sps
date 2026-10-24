using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.ReportData
{
    public class SQLUMLoginReportData
    {
        public string LoginName { get; set; }
        public string DefaultDatabase { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? updatedate { get; set; }
        public DateTime? accdate { get; set; }
        public bool SysAdmin { get; set; }
        public bool SecurityAdmin { get; set; }
        public bool ServerAdmin { get; set; }
        public bool ProcessAdmin { get; set; }
        public bool DiskAdmin { get; set; }
        public bool DbCreator { get; set; }
        public bool BulkAdmin { get; set; }
    }

}
