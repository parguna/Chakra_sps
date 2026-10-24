using DI.Chakra.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationAttestationSnapshotActionsForTask
    {
        public List<AttestationSnapshotAction> AttestationSnapshotActions { get; set; }
        public long? TaskID { get; set; }
        public bool IsUnSavedChangesAvailable { get; set; }
    }
}
