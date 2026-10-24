using System;


namespace DI.Chakra.DomainModel
{
    public class ServiceAccountQuestion : DomainModelBase<ServiceAccountQuestion, EntityModel.ServiceAccountQuestion>
    {
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
