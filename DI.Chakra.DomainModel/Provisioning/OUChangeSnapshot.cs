using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class OUChangeSnapshot : DomainModelBase<OUChangeSnapshot, EntityModel.OUChangeSnapshot>
    {
        public string BatchId { get; set; }
        public long UserId { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public string AccountName { get; set; }
        public AccountType AccountType { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public DateTime EnteredDate { get; set; }
        public EmployeeEvents EmployeeeEventType { get; set; }

        public string AdditionalInfo { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
        public long? RequestId { get; set; }
        public static List<OUChangeSnapshot> GetByUserId(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new OUChangeSnapshotsRepository(dataAccessTransaction).GetByUserId(userId));
            }
        }

        public static OUChangeSnapshot  GetByUserIdAccessRequestId(long userId, long accessRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new OUChangeSnapshotsRepository(dataAccessTransaction).GetByUserIdAccessRequestId(userId,accessRequestId));
            }
        }
    }
}
