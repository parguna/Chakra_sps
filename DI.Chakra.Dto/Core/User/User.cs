using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class User : DtoBase
    {

        public User()
        {
            this.Groups = new List<Group>();
        }
        public long EndPointSystemId { get; set; }
        public string IdentificationNo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string KnownAs { get; set; }
        public string EmployeeId { get; set; }
        public bool FullTimeEmployee { get; set; }
        public string EEGroup { get; set; }
        public string EmployeeStatus { get; set; }
        public bool IsExecutiveEmployee { get; set; }

        public Nullable<long> CompanyId { get; set; }
        public Nullable<long> DepartmentId { get; set; }
        public Nullable<long> DivisionId { get; set; }
        public Nullable<long> NOrganisationId { get; set; }
        public Nullable<long> NBusinessRoleId { get; set; }
        public Nullable<long> ManagerId { get; set; }

        public string EmailId { get; set; }
        public string TelephoneNumber { get; set; }
        public string HomephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Fax { get; set; }

        public string PostalCode { get; set; }
        public string PostalAddress { get; set; }

        public Nullable<DateTime> JoinDate { get; set; }
        public Nullable<DateTime> RehireDate { get; set; }
        public Nullable<DateTime> TerminationDate { get; set; }

        public bool IsActive { get; set; }
        public string Ethinicity { get; set; }
        public string Country { get; set; }
        public EmployeeType EmployeeType { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string CostCenter { get; set; }
        public string CostCenterText { get; set; }
        public string CostDistribution { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public string PositionCode { get; set; }
        public string PositionName { get; set; }
        public string Supervisor { get; set; }
        public string AuthenticationMode { get; set; }

        public string OrgCompany { get; set; }
        public string OrgDepartment { get; set; }
        public string OrgDivision { get; set; }
        public string OrgBranch { get; set; }
        public string OrgSection { get; set; }
        public string OrgUnit { get; set; }

        public string OneKeyTokenId { get; set; }
        public string FLINSLoginId { get; set; }

        public string ROEmployeeId { get; set; }
        public string COEmployeeId { get; set; }
        public string ProjectDescription { get; set; }
        public long ReferenceUserId { get; set; }

        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }
        public string Reserved6 { get; set; }
        public string Reserved7 { get; set; }
        public string Reserved8
        {
            get
            {
                return this.OrgUnit;
            }
            set
            {

            }
        }
        public string Reserved9 { get; set; }
        public string Reserved10 { get; set; }
        public Nullable<DateTime> LastLogonDate { get; set; }
        public bool NeedEmailUpdateToHR { get; set; }
        public bool NeedFLINSLoginIdUpdateToHR { get; set; }

        public AccessRequestStatus RequestStatus { get; set; }

        public List<Group> Groups { get; set; }
        public List<Role> Roles { get; set; }

        public NOrganisation NOrganisation { get; set; }
        public NDesignation NBusinessRole { get; set; }
        public string OrgChartLevels { get; set; }
        public string NCompany { get; set; }
        public int SortDirection;
        public bool NeedTelephoneUpdateToHR { get; set; }

        public string BranchCode { get; set; }

        public string UnitCode { get; set; }
        public string OrganisationCode { get; set; }

    }
}
