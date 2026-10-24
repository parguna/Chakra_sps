using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class PagePermission : DtoBase
    {
        public long PageId { get; set; }

        public long PermissionId { get; set; }

        public long? UrlId { get; set; }

        public WebPage WebPage { get; set; }

        public Permission Permission { get; set; }

        public PageUrl PageUrl { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string Url { get; set; }
    }
}
