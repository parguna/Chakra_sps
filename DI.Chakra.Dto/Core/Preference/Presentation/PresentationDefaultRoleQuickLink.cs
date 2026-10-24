using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationDefaultRoleQuickLink : DtoBase
    {
        public string Name { get; set; }
        //public string Title { get; set; }
        public string RoleName { get; set; }
        public string Category { get; set; }
        public int Position { get; set; }
    }
}
