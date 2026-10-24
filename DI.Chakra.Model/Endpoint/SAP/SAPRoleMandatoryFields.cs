using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DeepIdentity.Model
{
    

    [Table("DI_SAP_RoleMandatoryFields")]
    public class SAPRoleMandatoryField : IAggregateRoot
    {
        [Key]
        public int Id { get; set; }
       [MaxLength(250)]
        public string BusinessRole { get; set; }
        [MaxLength(250)]
        public string MandatoryField1 { get; set; }
        [MaxLength(250)]
        public string MandatoryField2 { get; set; }

        public Nullable<bool> IsModified { get; set; }

        

        public Nullable<bool> IsDeleted { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public Nullable<DateTime> ValidFrom { get; set; }
        [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public Nullable<DateTime> ValidTo { get; set; }

        public Nullable<bool> IsModifiedbyExcelUpload{get;set;}

        public Nullable<int> ReferenceId { get; set; }
        
    }
}
