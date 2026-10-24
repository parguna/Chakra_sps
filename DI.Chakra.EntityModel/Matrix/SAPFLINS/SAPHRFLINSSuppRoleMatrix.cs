using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_HRFLINSSuppRoleMatrix")]
    public class SAPHRFLINSSuppRoleMatrix : EntityBase, ICreatable, IUpdatable
    { 
        [MaxLength(250)]
        public string Rule { get; set; }
        [MaxLength(250)]
        public string Company { get; set; }
        [MaxLength(250)]
        public string CostCenter { get; set; }
        [MaxLength(250)]
        public string PurchasingOrg { get; set; }
        [MaxLength(250)]
        public string PurchasingGrp { get; set; }
        [MaxLength(250)]
        public string Plant { get; set; }
        [MaxLength(250)]
        public string MRPControl { get; set; }
        [MaxLength(250)]
        public string StorageLocation { get; set; }
        [MaxLength(250)]
        public string Role { get; set; }
        [MaxLength(250)]
        public string RoleName { get; set; }

        public Nullable<bool> IsModified { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }

        public Nullable<bool> IsModifiedbyExcelUpdate { get; set; }
        public Nullable<int> ReferenceId { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
