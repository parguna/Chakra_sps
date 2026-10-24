using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_RequestApprovalConfiguration")]
    public class RequestApprovalConfiguration : EntityBase, ICreatable, IUpdatable
    {
        public DI.Chakra.Infrastructure.Enums.AccessRequestSource RequestSource { get; set; }

        public bool IsApprovalRequired { get; set; }

        public Nullable<long> WorkflowConfigurationId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        [ForeignKey("WorkflowConfigurationId")]
        public WorkflowConfiguration WorkflowConfiguration { get; set; }
    }
}
