using DI.Chakra.Infrastructure;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_GlobalAnswer")]
    public class GlobalAnswer : EntityBase,ICreatable,IUpdatable
    {
        public long UserId { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

    }
}
