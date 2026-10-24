using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class TabMenu : DtoBase
    {
        public string UniqueName { get; set; }

        public string MenuName { get; set; }

        public long? ParentTabMenuId { get; set; }

        public long? PageId { get; set; }

        public WebPage WebPage { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string PageUrl { get; set; }
    }
}
