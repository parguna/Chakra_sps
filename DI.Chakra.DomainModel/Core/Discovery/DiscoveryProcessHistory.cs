using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class DiscoveryProcessHistory : DomainModelBase<DiscoveryProcessHistory, EntityModel.DiscoveryProcessHistory>
    {
        public int ProcessId { get; set; }

        public int HistoryId { get; set; }

        public long EndpointSystemId { get; set; }

        public DateTime? ProcessStartAt { get; set; }

        public DateTime? ProcessEndAt { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public string RecordStatus { get; set; }

        public static List<DiscoveryProcessHistory> GetByEndPointSystemId(long endPointSystemId)      
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.DiscoveryProcessHistory>(dataAccessTransaction)
                      .Get(dishistory => dishistory.EndpointSystemId == endPointSystemId & dishistory.Status == "Completed").ToList());
            }
        }
    }
}
