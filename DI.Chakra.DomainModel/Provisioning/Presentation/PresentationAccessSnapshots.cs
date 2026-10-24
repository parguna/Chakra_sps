using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationAccessSnapshots
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string EmailID { get; set; }
        public string EndpointsystemName { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementType { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string ProvisionedThrough { get; set; }
        public bool IsResolved { get; set; }
        public string ResolvedBy { get; set; }
        public string ResolvedStatus { get; set; }
        public DateTime? LastLogonDate { get; set; }
        public DateTime? ValidFromDate { get; set; }
        public DateTime? ValidToDate { get; set; }
        public long? AccessRequestId { get; set; }

        public static List<PresentationAccessSnapshots> GetByUserId(long listUserID)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationAccessSnapshotsRepository(dataAccessTransaction).GetByUserActionUserId(listUserID));
            }
        }


        public static PresentationAccessSnapshots ORMap(EntityModel.Presentation.PresentationAccessSnapshots presentationOUChangeSnapshotEntityModel)
        {
            if (presentationOUChangeSnapshotEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAccessSnapshots, PresentationAccessSnapshots>(presentationOUChangeSnapshotEntityModel);
        }

        public static List<PresentationAccessSnapshots> ORMapList(IList<EntityModel.Presentation.PresentationAccessSnapshots> presentationOUChangeSnapshotEntityModels)
        {
            return presentationOUChangeSnapshotEntityModels.Select(ORMap).ToList();
        }


    }
}
