using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_UserGridColumnInfo")]
    public class UserGridColumnInfo : EntityBase,ICreatable,IUpdatable
    {
        public string PagePath { get; set; }

        public string GridId { get; set; }

        public long UserId { get; set; }

        public string ColumnName { get; set; }

        public int OrderIndex { get; set; }

        [ForeignKey("UserId")]
        public User Users { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
