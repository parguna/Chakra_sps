using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationUser : EntityBase
    {
        public string EmployeeId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string EmailId { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public EmployeeType? EmployeeType { get; set; }
        public bool? isActiveUser { get; set; }
        public string OrganisationName { get; set; }
        public string Ethinicity { get; set; }
        public string Country { get; set; }
        public string BusinessRoleName { get; set; }
        public string ManagerName { get; set; }

        public string Grade { get; set; }

        public string CSEL { get; set; }

        public bool GoldBar { get; set; }

        public string Company { get; set; }
        public string NCompany { get; set; }

        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }
        public bool? IsActive { get; set; }
        public string GroupName { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Department { get; set; }
        public string CostCenter { get; set; }
        public string Division { get; set; }
        public string OrganisationUnit { get; set; }
        public string ReportingOfficer { get; set; }
        public string ProjectDescription { get; set; }
        public string CreatedBy { get; set; }
        public string ReferenceUser { get; set; }
        public string Status { get; set; }
        public string SectionName { get; set; }
        public string CompanyCode { get; set; }
        public string DepartmentCode { get; set; }
        public string BranchCode { get; set; }
        public string SectionCode { get; set; }
        public string UnitCode { get; set; }
        public string OrganisationCode { get; set; }
        public string Branch { get; set; }
        public string EmployeeTypeString
        {
            get;
            set;
        }
        public string CounterSigningOfficer { get; set; }
    }
}
