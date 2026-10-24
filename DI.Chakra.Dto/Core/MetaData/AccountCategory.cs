using System;
using System.ComponentModel;


namespace DI.Chakra.Dto
{
    public class AccountCategory : DtoBase
    {
        public string AccountTypeDesc { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
