using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_RoleMatrix")]
    public class RoleMatrix : EntityBase,ICreatable,IUpdatable
    {
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public long CostCenterId { get; set; }
        public string CostCenter { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public long DivisionId { get; set; }
        public string DivisionName { get; set; }
        public long OrganisationId { get; set; }
        public string SectionName { get; set; }
        public long BusinessRoleId { get; set; }
        public string Designation { get; set; }

        public string EmployeeTerm { get; set; }
        public byte version { get; set; }
        public bool IncludeDefaultsFromAllDeptAndDsg { get; set; }
        public bool IncludeDefaultsFromSpcDeptAndAllDsg { get; set; }
        public bool IncludeDefaultsFromAllDeptAndSpcDsg { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

        public List<RoleMatrixDetail> RoleMatrixDetails { get; set; }
        public Organisation Organsation { get; set; }
        public Designation BusinessRole { get; set; }
        public Company Company { get; set; }
        public Department Department { get; set; }
        public Division Division { get; set; }
        public Dictionary<long, string> Forms { get; set; }
    }
}
