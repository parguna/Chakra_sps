using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Dto;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class NCostCenter : DtoBase
    {
        public string Code { get; set; }

        public long CompanyId { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
