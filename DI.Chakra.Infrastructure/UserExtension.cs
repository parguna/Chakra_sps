using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure
{
    public class UserExtension
    {
        public string EEGroup { get; set; }
        public string EmployeeStatus { get; set; }
        public bool IsExecutiveEmployee { get; set; }

        public Nullable<long> CompanyId { get; set; }
        public Nullable<long> DepartmentId { get; set; }
        public Nullable<long> DivisionId { get; set; }

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

        public string OrgCompany { get; set; }
        public string OrgDepartment { get; set; }
        public string OrgDivision { get; set; }
        public string OrgBranch { get; set; }
        public string OrgSection { get; set; }

        public string OneKeyTokenId { get; set; }
        public string FLINSLoginId { get; set; }

        public bool NeedEmailUpdateToHR { get; set; }
        public bool NeedFLINSLoginIdUpdateToHR { get; set; }

        public string Serialize()
        {
            return UtilityMethods.Serialize<UserExtension>(this);
        }
    }
}
