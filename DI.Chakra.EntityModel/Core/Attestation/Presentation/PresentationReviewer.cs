using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    public class PresentationReviewer : EntityBase
    {
        public string ReviewerName { get; set; }
        public string DisplayName { get; set; }
        public string EmailId { get; set; }

        public long TaskId { get; set; }
    }
}
