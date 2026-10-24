using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class UserGridFilterSettings : DtoBase
    {
        public long UserId { get; set; }

        public string PagePath { get; set; }

        public string GridID { get; set; }

        public string Operand { get; set; }

        public string ColumnName { get; set; }

        public string FilterValue { get; set; }

        public User User { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
