using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class NRulesetPresentation : DomainModelBase<NRulesetPresentation, EntityModel.NRulesetPresentation>
    {
        public long RuleId { get; set; }

        public string Code { get; set; }

        public long OrgRank { get; set; }

        public long DesignationRank { get; set; }

        public long EmployeeTypeRank { get; set; }

        public long EventTriggerRank { get; set; }
        public long GradeRank { get; set; }
        public long CompanyRank { get; set; }
        public long BusinessUnitRank { get; set; }
        public long CostCenterRank { get; set; }
        public long LocationRank { get; set; }


        public long OverallRank
        {
            get
            {
                return (OrgRank + DesignationRank + EmployeeTypeRank + EventTriggerRank + GradeRank + CompanyRank + BusinessUnitRank + CostCenterRank + LocationRank);
            }
        }
    }
}
