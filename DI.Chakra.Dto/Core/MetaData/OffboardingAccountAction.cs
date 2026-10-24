using DI.Chakra.Infrastructure.Enums;
using System;


namespace DI.Chakra.Dto
{
    public class OffboardingAccountAction : DtoBase
    {
        public string Desc { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
