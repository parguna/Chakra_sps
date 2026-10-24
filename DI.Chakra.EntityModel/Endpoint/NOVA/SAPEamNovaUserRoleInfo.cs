using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel 
{
    [Table("DI_SAP_EamNovaUserRoleInfo")]
    public class SAPEamNovaUserRoleInfo : EntityBase, ICreatable, IUpdatable
    {
        public long SAPEamNovaUserInfoId { get; set; }
        public string RoleName { get; set; }
        public string FLINS_TCodes { get; set; }
        public string FLINS_BI_TCodes { get; set; }
        public string AD_Groups { get; set; } 
        public string Company { get; set; }
        public string Section { get; set; }
        public string DepartmentBranch { get; set; }
        public NovaMatrixType NovaPositionMatrixType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
