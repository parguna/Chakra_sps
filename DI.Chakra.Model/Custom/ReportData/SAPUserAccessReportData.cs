using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class SAPUserAccessReportData
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string RoleCode { get; set; }
        public string TCode { get; set; }
        public string ActivityDescription { get; set; }
    }
}
