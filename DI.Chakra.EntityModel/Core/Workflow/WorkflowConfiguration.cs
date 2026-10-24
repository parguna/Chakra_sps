using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_WorkflowConfiguration")]
    public class WorkflowConfiguration : EntityBase,ICreatable,IUpdatable
    {
        public WorkflowConfiguration()
        {
            this.WorkflowConfigurationDetails = new List<WorkflowConfigurationDetail>();
        }
        public string WorkflowName { get; set; }

        public string Description { get; set; }

        public long RequestTypeId { get; set; }

        public long? EndpointSystemId { get; set; }

        public string ApplicableUserTypes { get; set; }

        public string ApplicableAccountTypes { get; set; }

        public string ApplicableActionTypes { get; set; }

        public bool IsActive { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public List<WorkflowConfigurationDetail> WorkflowConfigurationDetails { get; set; }

        [ForeignKey("EndpointSystemId")]
        public EndPoint Endpoint { get; set; }

        [ForeignKey("RequestTypeId")]
        public RequestTypes RequestTypes { get; set; }
        public string EntitlementType { get; set; }
    }
}
