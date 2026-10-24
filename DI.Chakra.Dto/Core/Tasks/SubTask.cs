using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SubTask : DtoBase
    {
        public string RoleName { get; set; }

        public string TCode { get; set; }

        public string TCodeDescription { get; set; }

        public string Remarks { get; set; }

        public bool IsActive { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public long OwnerId;
    }
}
