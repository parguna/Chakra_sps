using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class UserGridInfo:DtoBase
    {
        public string PagePath { get; set; }

        public string GridId { get; set; }

        public long UserId { get; set; }

        public string SortDirection { get; set; }

        public string SortColumnName { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public  User Users { get; set; }
    }
}
