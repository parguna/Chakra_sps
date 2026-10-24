using System;
using System.Collections.Generic;

namespace DI.Chakra.DomainModel.Presentation
{
    [Serializable]
    public class Office365UserServicePlan
    {
        public Office365UserServicePlan()
        {
            this.PlanDetails = new List<ServicePlanDetail>();
        }

        public List<ServicePlanDetail> PlanDetails { get; set; }
    }

    [Serializable]
    public class ServicePlanDetail
    {
        public string AccountSkuId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public string ProvisioningStatus { get; set; }
    }
}
