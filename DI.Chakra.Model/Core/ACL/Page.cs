using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DI.Chakra.Model
{
    [Table("DI_M_Page")]
    [Serializable]
    public class WebPage : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string UniqueName { get; set; }
        public string Name { get; set; }
        public long? PageUrlId { get; set; }
        [ForeignKey("PageUrlId")]
        public PageUrl PageUrl { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public override string ToString()
        {
            if (PageUrl == null)
                return null;

            if (PageUrl.Url == null)
                return null;

            return PageUrl.Url.ToString();
        }
    }
        

    }
