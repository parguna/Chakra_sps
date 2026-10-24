using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.DomainModel
{
    public class EventUndoRequest : DomainModelBase<EventUndoRequest, EntityModel.EventUndoRequest>
    {

        public EndpointType EndpointType { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public EmployeeEvents EmployeeEventType { get; set; }
        public string TechnicalRoles { get; set; }
        public string TechnicalGroups { get; set; }
        public string TechnicalPositions { get; set; }

        public string AdditionalInfo { get; set; }

        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }


        public virtual EndPoint EndPoint { get; set; }
       
    }
}
