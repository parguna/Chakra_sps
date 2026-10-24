using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationOrgChangeTrack
    {
        public string CostCenter { get; set; }

        public long CompanyId { get; set; }

        public long OrganisationId { get; set; }

        public string CompanyCode { get; set; }

        public string DepartmentCode { get; set; }

        public string DivisionCode { get; set; }

        public string SectionCode { get; set; }

        public string CompanyName { get; set; }

        public string DepartmentName { get; set; }

        public string DivisionName { get; set; }

        public string SectionName { get; set; }

        public string OrgCompany { get; set; }

        public string OrgDepartment { get; set; }

        public string OrgDivision { get; set; }

        public string OrgBranch { get; set; }

        public string OrgSection { get; set; }

        public string PositionName { get; set; }

        public DateTime EffectiveDate { get; set; }

        public EmployeeEvents Event { get; set; }
    }
}
