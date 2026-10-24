using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
  public class UserPreference : DtoBase
    {

        public long TabMenuId { get; set; }

        public string MenuName { get; set; }

        public int Position { get; set; }

        public string TargetUrl { get; set; }
    }
}
