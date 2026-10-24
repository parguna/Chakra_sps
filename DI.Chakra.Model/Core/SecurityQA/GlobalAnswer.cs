using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DI.Chakra.Infrastructure;


namespace DI.Chakra.Model
{
    [Table("DI_M_GlobalAnswer")]
    public class GlobalAnswer
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string PlainAnswer
        {
            get
            {
                return RSAHelper.DecryptData(this.Answer);
            }
        }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
