using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_SODExceptionItem")]
    public class SODExceptionItem : EntityBase,ICreatable,IUpdatable
    {
        public Nullable<long> ExceptionId { get; set; }

        public Nullable<long> UserId { get; set; }

        public Nullable<long> FunctionId1 { get; set; }

        public Nullable<long> FunctionItemId1 { get; set; }

        public Nullable<long> FunctionId2 { get; set; }

        public Nullable<long> FunctionItemId2 { get; set; }

        [ForeignKey("UserId")]
        public  User user { get; set; }

        [ForeignKey("FunctionId1")]
        public  SODFunction function1 { get; set; }

        [ForeignKey("FunctionItemId1")]
        public  SODFunctionItem functionitem1 { get; set; }

        [ForeignKey("FunctionId2")]
        public  SODFunction function2 { get; set; }

        [ForeignKey("FunctionItemId2")]
        public  SODFunctionItem functionitem2 { get; set; }

        [ForeignKey("ExceptionId")]
        public  SODException SODException { get; set; }


        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


    }
}
