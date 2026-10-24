using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]

    [Table("DI_M_RoleMatrix")]
    public class RoleMatrix
    {

        public RoleMatrix()
        {
            this.RoleMatrixDetails = new List<RoleMatrixDetail>();
            this.Forms = new Dictionary<long, string>();
        }

        [Key]
        public long Id { get; set; }

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

        public virtual List<RoleMatrixDetail> RoleMatrixDetails { get; set; }

        [ForeignKey("OrganisationId")]
        public virtual Organisation Organsation { get; set; }

        [ForeignKey("BusinessRoleId")]
        public virtual Designation BusinessRole { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        [ForeignKey("DivisionId")]
        public virtual Division Division { get; set; }

        public Dictionary<long, string> Forms { get; set; }

    }
}
