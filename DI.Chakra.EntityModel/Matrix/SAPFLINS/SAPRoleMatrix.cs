using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_RoleMatrix")]
    public class SAPRoleMatrix : EntityBase, ICreatable, IUpdatable
    { 
        [MaxLength(250)]
        public string Application { get; set; }
        [MaxLength(250)]
        public string EligibilityRule { get; set; }
        [MaxLength(250)]
        public string Company { get; set; }
        [MaxLength(250)]
        public string PersonnelArea { get; set; }
        [MaxLength(250)]
        public string PersonnelSubArea { get; set; }
        [MaxLength(250)]
        public string Position { get; set; }
        [MaxLength(250)]
        public string OrgUnit { get; set; }
        [MaxLength(250)]
        public string CostCenter { get; set; }
        [MaxLength(250)]
        public string Remarks { get; set; }

        public Nullable<bool> IsModified { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }

        public Nullable<bool> IsUpdatedbyExcel { get; set; }
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
