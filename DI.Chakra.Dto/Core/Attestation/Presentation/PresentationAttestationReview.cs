using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationAttestationReview
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string EmailId { get; set; }

        public long ReviewId { get; set; }
    }
}
