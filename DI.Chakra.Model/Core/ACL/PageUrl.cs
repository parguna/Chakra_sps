using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DI.Chakra.Model
{
    [Table("DI_M_PageUrl")]
    [Serializable]
    public class PageUrl
    {
        [Key]
        public long Id { get; set; }
        public string Url { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public override string ToString()
        {
            if (Url == null)
                return null;

            return Url.ToString();
        }
    }
}
