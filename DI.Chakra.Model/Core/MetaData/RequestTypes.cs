using DI.Chakra.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_RequestTypes")]
    public class RequestTypes : IAggregateRoot
    {
        public RequestTypes()
        {
            this.WorkflowConfigurations = new List<WorkflowConfiguration>();
        }

        [Key]
        public long Id { get; set; }
        public string RequestTypeDesc { get; set; }
        public bool IsActive { get; set; }

        public virtual List<WorkflowConfiguration> WorkflowConfigurations { get; set; }
    }

}

