using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_EmployeeTerm")]
    public class EmployeeTerm : EntityBase,ICreatable,IUpdatable
    {
        public string EmployeeTermDesc { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

    }
}
