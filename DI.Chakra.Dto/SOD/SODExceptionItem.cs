using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SODExceptionItem : DtoBase
    {
        public Nullable<long> ExceptionId { get; set; }

        public Nullable<long> UserId { get; set; }

        public Nullable<long> FunctionId1 { get; set; }

        public Nullable<long> FunctionItemId1 { get; set; }

        public Nullable<long> FunctionId2 { get; set; }

        public Nullable<long> FunctionItemId2 { get; set; }

        public  User user { get; set; }

        public  SODFunction function1 { get; set; }

        public  SODFunctionItem functionitem1 { get; set; }

        public  SODFunction function2 { get; set; }

        public  SODFunctionItem functionitem2 { get; set; }

        public  SODException SODException { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
