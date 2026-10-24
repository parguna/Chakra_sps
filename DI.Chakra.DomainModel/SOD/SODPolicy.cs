using System;

namespace DI.Chakra.DomainModel
{
    public class SODPolicy : DomainModelBase<SODPolicy, EntityModel.SODPolicy>
    {
        public string SODPolicyName { get; set; }

        public string SODPolicyDescription { get; set; }

        public long PolicyTypeId { get; set; }

        public int BindType { get; set; }

        public ComplianceType policytype { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

    }
}
