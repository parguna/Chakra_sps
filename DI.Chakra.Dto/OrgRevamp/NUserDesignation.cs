using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Dto;

namespace DI.Chakra.Dto
{
    public class NUserDesignation : DtoBase
    {
        public long UserId { get; set; }

        public long DesignationId { get; set; }

        public string Category { get; set; }

        public Nullable<System.DateTime> ValidFrom { get; set; }

        public Nullable<System.DateTime> ValidTo { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public User User { get; set; }

        public NDesignation Designation { get; set; }
    }
}
