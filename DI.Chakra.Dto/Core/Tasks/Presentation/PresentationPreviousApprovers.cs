using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationPreviousApprovers : Task
    {
        public string UserName { get; set; }

        public DateTime ApprovedDate { get; set; }
    }
}
