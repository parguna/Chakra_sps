using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_ProvisioningConfigMatrixSAPEEGroup")]
    public class ProvisioningConfigMatrixSapEEGroup : EntityBase, ICreatable, IUpdatable
    {       
        public string EmployeeGroupNo { get; set; }
        public string EmployeeGroup { get; set; }
        public string PArea { get; set; }
        public string DefaultRole { get; set; }
        public string Remark { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

    }
}
