using System;


namespace DI.Chakra.Dto
{

    public class EmployeeTerm : DtoBase
    {
        public string EmployeeTermDesc { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
