using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationSnapshotActionHistory
    {
        public long TaskId { get; set; }
        public long AttestationSnapshotActionId { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public long Version { get; set; }
        public string DelegatedBy { get; set; }
        public string DelegationComments { get; set; }
        public static List<PresentationSnapshotActionHistory> GetHistoryByCampaignId(long campaignId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new AttestationSnapshotActionHistoryRepository(dataAccessTransaction)
                    .GetHistoryByCampaignId(campaignId).ToList());
            }
        }
        public static PresentationSnapshotActionHistory ORMap(EntityModel.Presentation.PresentationSnapshotActionHistory tEntity)
        {
            if (tEntity == null)
                return default(PresentationSnapshotActionHistory);

            return Mapper.Map<EntityModel.Presentation.PresentationSnapshotActionHistory, PresentationSnapshotActionHistory > (tEntity);
        }

        public static List<PresentationSnapshotActionHistory> ORMapList(IList<EntityModel.Presentation.PresentationSnapshotActionHistory> tEntities)
        {
            return tEntities.Select(ORMap).ToList();
        }
    }
}
