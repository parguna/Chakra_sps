using DI.Chakra.Infrastructure;
using System.Collections.Generic;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationAttestationSnapshotActionsForTask:DtoBase
    {
        public List<AttestationSnapshotAction> AttestationSnapshotActions { get; set; }
        public long? TaskID { get; set; }
        public bool IsUnSavedChangesAvailable { get; set; }
    }
}
