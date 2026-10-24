using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.Core
{
    [Table("DI_M_UserImage")]
    public class UserImage
    {
        [Key]
        public long Id { get; set; }
        public byte[] Image { get; set; }
        public long? UserId { get; set; }
    }
}
