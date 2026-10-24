using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODConflictFunctionMap : DomainModelBase<SODConflictFunctionMap, EntityModel.SODConflictFunctionMap>
    {
        public long SodConflictFunctionId { get; set; }

        public long SodFunctionId { get; set; }

        public SODFunction Function { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
