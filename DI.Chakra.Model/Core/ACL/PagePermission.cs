using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DI.Chakra.Model
{
    [Table("DI_M_PagePermission")]
    [Serializable]
    public class PagePermission : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public long PageId { get; set; }
        public long PermissionId { get; set; }
        public long? UrlId { get; set; }
        [ForeignKey("PageId")]
        public WebPage Page { get; set; }
        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; }
        [ForeignKey("UrlId")]
        public PageUrl Url { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }

    public class PermissionComparer : IEqualityComparer<PagePermission>
    {

        public bool Equals(PagePermission x, PagePermission y)
        {
            return x.PermissionId.Equals(y.PermissionId) && x.PageId.Equals(y.PageId) && x.UrlId.Equals(y.UrlId);
        }

        public int GetHashCode(PagePermission obj)
        {
            return obj.Id.GetHashCode();
        }

    }
}