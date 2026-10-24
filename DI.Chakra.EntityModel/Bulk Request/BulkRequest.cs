using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_BulkRequest")]
    public class BulkRequest : EntityBase, ICreatable, IUpdatable
    {

        public BulkRequest()
        {
            this.BulkRequestDetails = new List<BulkRequestDetail>();
        }

        public long RequestedBy { get; set; }
        public EndpointType EndpointType { get; set; }
        public long EndpointSystemId { get; set; }
        public string Description { get; set; }
        public AdHocActionType AdhocActionType { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string EntitlementDetail { get; set; }

        public bool IsEmployee { get; set; }

        public long? ActualSupervisor { get; set; }
        public long? SelectedSupervisor { get; set; }
        public bool SupervisorChanged { get; set; }

        public AccessRequestStatus RequestStatus { get; set; }
        public AccessRequestProgress ProgressStatus { get; set; }
        public AccessRequestSource RequestSource { get; set; }

        public Nullable<DateTime> ActivationDate { get; set; }
        public Nullable<DateTime> DeactivationDate { get; set; }
        public Nullable<DateTime> ExecutionTime { get; set; }

        public string Remarks { get; set; }
        public string AdditionalInfo { get; set; }
        public string Comments { get; set; }
        public bool IsCurrentLevelApproved { get; set; }
        public bool IsRequestedToRemove { get; set; }
        public string ApproverType { get; set; }

        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

        [ForeignKey("EndpointSystemId")]
        public EndPoint EndPoint { get; set; }

        public string ConflictsRemarks { get; set; }
        public List<BulkRequestDetail> BulkRequestDetails { get; set; }

    }
}