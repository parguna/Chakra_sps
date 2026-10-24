using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_BulkRequestDetail")]
    public class BulkRequestDetail : EntityBase
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

        public string AccountName { get; set; }
        public long? EndpointUserId { get; set; }

        public string HandPhone { get; set; }
    }
}
