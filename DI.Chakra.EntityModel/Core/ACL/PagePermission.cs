using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_PagePermission")]
    public class PagePermission : EntityBase, ICreatable, IUpdatable
    {
        public long PageId { get; set; }

        public long PermissionId { get; set; }

        public long? UrlId { get; set; }

        [ForeignKey("PageId")]
        public WebPage WebPage { get; set; }

        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; }

        [ForeignKey("UrlId")]
        public PageUrl PageUrl { get; set; }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
