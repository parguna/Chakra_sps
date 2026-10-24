using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{

    [Table("DI_M_Attestation")]
    public class Attestation : EntityBase, ICreatable, IUpdatable
    {
        public string AttestationName { get; set; }

        public string AttestationDescription { get; set; }

        public string CampaignNameFormat { get; set; }

        public bool RequiresAttestationRemediation { get; set; }

        public long WorkflowConfigurationId { get; set; }
        public bool IncludeUnmappedAccount { get; set; }
        public bool IncludeDormantAccount { get; set; }
        public long DormantAccountDays { get; set; }
        public bool CreateTaskOnNoReviewer { get; set; }
        public Nullable<long> TaskActionUserId { get; set; }
        public bool IsGroupTask { get; set; }
        public bool SendEmailOnNoReviewer { get; set; }
        public Nullable<long> EmailEntityId { get; set; }
        public bool IsGroupMail { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("WorkflowConfigurationId")]
        public  WorkflowConfiguration WorkflowConfiguration { get; set; }

        public  List<AttestationProcess> AttestationProcesses { get; set; }

        public  List<AttestationEndpoint> AttestationEndpoint { get; set; }

        public List<AttestationScope> AttestationScopes { get; set; } 


        public AttestationDefaultReviewStatus DefaultReviewStatus { get; set; }

        public List<AttestationHierarchy> AttestationHierarchy { get; set; }
        public AttestationType AttestationType { get; set; }
        public AttestationScopeEnum SplitCampaignUsing { get; set; }
    }

}
