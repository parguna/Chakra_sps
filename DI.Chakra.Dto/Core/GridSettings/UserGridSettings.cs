using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class UserGridSettings : DtoBase 
    {
        public string PagePath { get; set; }

        public string GridID { get; set; }

        public string SortColumnName { get; set; }

        public string SortDirection { get; set; }

        public string FilterCondition { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public long UserId { get; set; }

        public User User { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
