using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationTabMenu : TabMenu
    {
        public string ParentMenuName { get; set; }
        public string PageName { get; set; }
    }
}
