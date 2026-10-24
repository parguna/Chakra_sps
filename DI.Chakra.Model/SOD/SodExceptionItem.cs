using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_SODExceptionItem")]
    public class SodExceptionItem
    {
        [Key]
        public long Id { get; set; }

        public Nullable<long> ExceptionId { get; set; }
        public Nullable<long> UserId { get; set; }
        public Nullable<long> FunctionId1 { get; set; }
        public Nullable<long> FunctionItemId1 { get; set; }
        public Nullable<long> FunctionId2 { get; set; }
        public Nullable<long> FunctionItemId2 { get; set; }
        [ForeignKey("UserId")]
        public virtual User user { get; set; }

        [ForeignKey("FunctionId1")]
        public virtual SodFunction function1 { get; set; }

        [ForeignKey("FunctionItemId1")]
        public virtual SodFunctionItem functionitem1 { get; set; }

        [ForeignKey("FunctionId2")]
        public virtual SodFunction function2 { get; set; }

        [ForeignKey("FunctionItemId2")]
        public virtual SodFunctionItem functionitem2 { get; set; }

        [ForeignKey("ExceptionId")]
        public virtual SODException SODException { get; set; }
    }
}
