using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    
    public class SODConflictFunctionMap : DtoBase
    {

        public long SodConflictFunctionId { get; set; }

        public long SodFunctionId { get; set; }

        public virtual SODFunction Function { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
