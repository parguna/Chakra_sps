using DI.Chakra.Infrastructure;
using System;
using System.ComponentModel;


namespace DI.Chakra.Dto
{
    public class GlobalAnswer : DtoBase
    {
        public long UserId { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public User User { get; set; }

    }
}
