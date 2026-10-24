using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto 
{
    [Serializable]
    public partial class BulkRequestDetail : DtoBase
    {

        public long BulkRequestId { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string EmployeeId { get; set; }
        public string EmailId { get; set; }
        public string Designation { get; set; }
        public string OrgCompany { get; set; }
        public string OrgDepartment { get; set; }
        public string OrgDivision { get; set; }
        public string OrgBranch { get; set; }
        public string OrgSection { get; set; }
        public string OrgUnit { get; set; }
        public string OneKey { get; set; }

        public string HandPhone { get; set; }

        public string AccountName { get; set; }
        public long? EndpointUserId { get; set; }
    }
}
