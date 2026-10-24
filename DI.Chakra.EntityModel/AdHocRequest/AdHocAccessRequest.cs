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
    [Table("DI_M_AdHocAccessRequest")]
    public class AdHocAccessRequest : EntityBase,ICreatable,IUpdatable
    {
        public long RequesterUserId { get; set; }

        public long RequesteeUserId { get; set; }

        public EndpointType EndpointSystemType { get; set; }

        public long? EndpointSystemId { get; set; }

        public AdHocActionType AdhocActionType { get; set; }

        public AccountType AccountType { get; set; }

        public bool IsSelfRequest { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public string Comments { get; set; }

        public string EntitlementDetail { get; set; }

        public ApplicationType ApplicationType { get; set; }
        public string SecurityQuestions { get; set; }
        public int? ActualSupervisor { get; set; }
        public int? SelectedSupervisor { get; set; }
        public bool SupervisorChanged { get; set; }


        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }
        
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("EndpointSystemId")]
        public  EndPoint EndPoint { get; set; }

        [NotMapped]
        public bool IsAddAccess { get; set; }

        public bool IsÉxpiryMailSend { get; set; }

        public bool IsÉxpiryHandled { get; set; }
       
    }
}
