using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DI.Chakra.EntityModel
{   

    [Table("DI_SAP_RoleMandatoryFields")]
    public class SAPRoleMandatoryField : EntityBase
    { 
       [MaxLength(250)]
        public string BusinessRole { get; set; }
        [MaxLength(250)]
        public string MandatoryField1 { get; set; }
        [MaxLength(250)]
        public string MandatoryField2 { get; set; }

        public Nullable<bool> IsModified { get; set; }

        

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }

        public Nullable<bool> IsModifiedbyExcelUpload{get;set;}

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
