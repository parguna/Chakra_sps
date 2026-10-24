using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_SODFunction")]
    public class SODFunction : EntityBase,ICreatable,IUpdatable
    {
        public string SodFunctionCode { get; set; }

        public string SodFunctionDescription { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public  List<SODFunctionItem> SodFunctionItems { get; set; }

        public  List<SODConflictFunctions> ConflictFunctions { get; set; }

       // public  List<SODException> Exceptions { get; set; }
        
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

    }
}
