using DI.Chakra.DomainModel.Presentation;
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
    public class ApproverDelegationHistory : DomainModelBase<ApproverDelegationHistory, EntityModel.ApproverDelegationHistory>
    {
        public long AccessRequestId { get; set; }

        public string ApproverName { get; set; }

        public string Level { get; set; }

        public string Remarks { get; set; }

        public DateTime DelegatedDateTime { get; set; }

        public string ApproverRole { get; set; }
        public bool IsActionOnlyRequired { get; set; }
        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public long TaskId { get; set; }

        public static List<ApproverDelegationHistory> GetApproverDelegationHistoryByRequestId(long requestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ApproverDelegationHistory>(dataAccessTransaction).Get(task => task.AccessRequestId == requestId).ToList());
            }
        }
    }
}
