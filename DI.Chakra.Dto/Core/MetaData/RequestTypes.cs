using System;
using System.Collections.Generic;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class RequestTypes : DtoBase
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
