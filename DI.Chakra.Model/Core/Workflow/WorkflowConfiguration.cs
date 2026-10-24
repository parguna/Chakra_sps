using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_M_WorkflowConfiguration")]
    [Serializable]
    public class WorkflowConfiguration
    {
        public WorkflowConfiguration()
        {
            this.WorkflowConfigurationDetails = new List<WorkflowConfigurationDetail>();
        }

        [Key]
        public long Id { get; set; }
        public string WorkflowName { get; set; }
        public string Description { get; set; }
        public long RequestTypeId { get; set; }
        public long? EndpointSystemId { get; set; }

        public string ApplicableUserTypes { get; set; }
        public string ApplicableAccountTypes { get; set; }
        public string ApplicableActionTypes { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

        public virtual List<WorkflowConfigurationDetail> WorkflowConfigurationDetails { get; set; }
        [ForeignKey("EndpointSystemId")]
        public virtual EndPoint Endpoint { get; set; }

        [ForeignKey("RequestTypeId")]
        public virtual RequestTypes RequestTypes { get; set; } 
    }

}
