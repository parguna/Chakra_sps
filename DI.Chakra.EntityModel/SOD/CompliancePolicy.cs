using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_CompliancePolicy")]
    public class CompliancePolicy : EntityBase,ICreatable,IUpdatable
    {
        public string CompliancePolicyCode { get; set; }

        public string CompliancePolicyName { get; set; }

        public string CompliancePolicyDescription { get; set; }

        public string CompliancePolicyType { get; set; }

        public string Status { get; set; }

        public Nullable<long> OwnerId { get; set; }

        public Nullable<bool> IsActive { get; set; }

        [ForeignKey("OwnerId")]
        public  User Owner { get; set; }

        public  List<SODConflictFunctions> Conflicts { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

    }
}
