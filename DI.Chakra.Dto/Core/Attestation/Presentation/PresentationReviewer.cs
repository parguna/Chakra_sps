using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationReviewer
    {
        public string ReviewerName { get; set; }
        public string DisplayName { get; set; }
        public string EmailId { get; set; }

        public long TaskId { get; set; }

        
    }
}
