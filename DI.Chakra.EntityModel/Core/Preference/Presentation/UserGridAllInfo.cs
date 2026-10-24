using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class UserGridAllInfo : EntityBase
    {
        public string PagePath { get; set; }
        public string GridId { get; set; }
        public long UserId { get; set; }
        public UserGridInfo UserGridInfo { get; set; }
        public List<UserGridFilterInfo> UserGridFilterInfo { get; set; }
        public List<UserGridColumnInfo> UserGridColumnInfo { get; set; }
    }
}
