using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeepIdentity.Model
{
    [Table("DI_SAP_RoleMatrix")]
    public class SAPRoleMatrix
    {
        [Key]
        public int Id { get; set; }
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
    }
}
