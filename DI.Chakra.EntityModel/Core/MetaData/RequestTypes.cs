using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
     [Table("DI_M_RequestTypes")]
    public class RequestTypes : EntityBase,ICreatable,IUpdatable
    {
        public RequestTypes()
        {
            this.WorkflowConfigurations = new List<WorkflowConfiguration>();
        }

        public string RequestTypeDesc { get; set; }

        public bool IsActive { get; set; }

        public List<WorkflowConfiguration> WorkflowConfigurations { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

    }
}
