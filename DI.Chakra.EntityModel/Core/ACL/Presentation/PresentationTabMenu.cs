using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationTabMenu : TabMenu
    {
        public string ParentMenuName { get; set; }
        public string PageName { get; set; }
    }
}
