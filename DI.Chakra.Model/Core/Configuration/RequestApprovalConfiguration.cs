using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_RequestApprovalConfiguration")]
    public class RequestApprovalConfiguration
    {
        [Key]
        public long Id { get; set; }
        public AccessRequestSource RequestSource { get; set; }
        public bool IsApprovalRequired { get; set; }
        public Nullable<long> WorkflowConfigurationId { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        [ForeignKey("WorkflowConfigurationId")]
        public virtual WorkflowConfiguration WorkflowConfiguration { get; set; }

    }
}
