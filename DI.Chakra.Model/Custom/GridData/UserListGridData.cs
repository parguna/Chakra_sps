using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class UserListGridData
    {
        public long UserId { get; set; }
        public string TenantId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string EmployeeId { get; set; }
        public string OrganisationName { get; set; }
        public long OrganisationId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public bool IsActive { get; set; }
        public bool FullTimeEmployee { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public string Supervisor { get; set; }
        

        public Nullable<long> CompanyId { get; set; }
        public Nullable<long> DepartmentId { get; set; }
        public Nullable<long> DivisionId { get; set; }
        public Nullable<int> BusinessRoleId { get; set; }
        public string CostCenter { get; set; }

        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string DivisionName { get; set; }
        public string BusinessRole {get;set;}

 
    }
}
