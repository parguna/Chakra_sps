using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODExecutionConfig : DomainModelBase<SODExecutionConfig, EntityModel.SODExecutionConfig>
    {
        public Nullable<long> EndpointSystemTypeId { get; set; }

        public string Level1 { get; set; }

        public string Level2 { get; set; }

        public string Level3 { get; set; }

        public string Level4 { get; set; }

        public Nullable<int> SODRunPolicyId { get; set; }

        public EndPointSystemType EndpointSystemType { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static SODExecutionConfig GetByEndpointSystenTypeId(long endpointSystemTypeId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODExecutionConfig>(tranaction)
                    .Get(item => item.EndpointSystemTypeId == endpointSystemTypeId).FirstOrDefault());
            }
        }

        public static List<SODExecutionConfig> GetAllWithEndPointSystemType()
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SODExecutionConfig>(tranaction)
                    .GetAll(item => item.EndpointSystemType).ToList());
            }
        }
    }
}
