using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class Office365ServicePlan : DomainModelBase<Office365ServicePlan, EntityModel.Office365ServicePlan>
    {
        public long EndPointSystemId { get; set; }
        public string AccountSkuId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }

        public override string ToString()
        {
            return AccountSkuId + " - " + ServiceName;
        }

        public static Office365ServicePlan GetServicePlanByServiceName(string AccountSkuId, string serviceName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Office365ServicePlan>(dataAccessTransaction)
                      .Get(t => t.AccountSkuId == AccountSkuId && t.ServiceName == serviceName).FirstOrDefault());
            }
        }

        public static List<Office365ServicePlan> GetServicePlanByAccountSkuId(string AccountSkuId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.Office365ServicePlan>(dataAccessTransaction)
                      .Get(t => t.AccountSkuId == AccountSkuId).ToList());
            }
        }
    }
}
