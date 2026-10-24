using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationTaskdelegation : TaskDelegation
    {
        public long DelegationId { get; set; }

        public string ActionUserName { get; set; }

        public long DelegatedToUserId { get; set; }

        public string DelegatedToUserName { get; set; }

        public bool IsAdvance { get; set; }
    }
}
