using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    public class MetaData : EntityBase,ICreatable,IUpdatable
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string Type { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

    }
}
