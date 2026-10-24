using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class PresentationSODEntitlementConflictRepository : RepositoryBase<PresentationSODEntitlementConflict>
    {
        public PresentationSODEntitlementConflictRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<PresentationSODEntitlementConflict> GetSODEntitlementConflicts(string Username,long ConflictId)
        {
            List<PresentationSODEntitlementConflict> requestList = new List<PresentationSODEntitlementConflict>();

            requestList = (from conflict in CurrentContext.SODConflictFunctions
                           join report in CurrentContext.SODPolicyReport
                           on conflict.Id equals report.ConflictId
                           join user in CurrentContext.User
                           on report.UserName equals user.UserName
                           where report.UserName == Username && report.ConflictId == ConflictId
                           select new PresentationSODEntitlementConflict
                           {
                              UserName = user.UserName,
                              Role1 = report.Role1,
                              Role2 = report.Role2
                           }).ToList();

            return requestList;
        }
    }
}
