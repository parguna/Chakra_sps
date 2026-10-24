using System;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainModel
{
    public interface IOrgChangeTrack
    {
        string EmployeeId { get; set; }
        string BranchCode { get; set; }

        string CompanyCode { get; set; }
        Nullable<long> CompanyId { get; set; }
        string CompanyName { get; set; }
        string CostCenter { get; set; }
        string CostCenterText { get; set; }
        string DepartmentCode { get; set; }
        Nullable<long> DepartmentId { get; set; }
        string DepartmentName { get; set; }
        string DivisionCode { get; set; }
        Nullable<long> DivisionId { get; set; }
        string DivisionName { get; set; }

        bool IsActive { get; set; }
        string OrganisationCode { get; set; }

        string OrgBranch { get; set; }
        string OrgCompany { get; set; }
        string OrgDepartment { get; set; }
        string OrgDivision { get; set; }
        string OrgSection { get; set; }
        string OrgUnit { get; set; }
        string PositionCode { get; set; }
        string PositionName { get; set; }
        string SectionCode { get; set; }
        string SectionName { get; set; }
        string UnitCode { get; set; }
        string ROEmployeeId { get; set; }

        string EEGroup { get; set; }
        string EmployeeStatus { get; set; }
        bool IsExecutiveEmployee { get; set; }

          Nullable<DateTime> JoinDate { get; set; }
          Nullable<DateTime> RehireDate { get; set; }
          Nullable<DateTime> TerminationDate { get; set; }
        string ToString();
    }

    public interface IEmployeeEventReason
    {
        DateTime EffectiveDate { get;   }
        EmployeeEvents Event { get;   }
        string EventCode { get;   }
        string EventReason { get;   }
        string Comment { get;   }
    }
}