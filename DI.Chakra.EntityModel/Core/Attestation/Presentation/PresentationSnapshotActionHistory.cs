using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationSnapshotActionHistory
    {
        public long TaskId { get; set; }
        public long AttestationSnapshotActionId { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public long Version { get; set; }
        public string DelegatedBy { get; set; }
        public string DelegationComments { get; set; }
    }
}
