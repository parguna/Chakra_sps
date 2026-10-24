using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_UserGridFilterInfo")]
   public  class UserGridFilterInfo:EntityBase,ICreatable,IUpdatable
    {
        public string PagePath { get; set; }

        public string GridId { get; set; }

        public long UserId { get; set; }

        public string Operand { get; set; }

        public string ColumnName { get; set; }

        public string FilterValue { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("UserId")]
        public  User Users { get; set; }
    }
}
