using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationOUChangeSnapshots
    {
        public long SnapshotId { get; set; }
        public string UserName { get; set; }
        public string EmployeeNo { get; set; }
        public string EmailID { get; set; }
        public long EndpointSystemTypeID { get; set; }
        public string EndpointSystemType { get; set; }
        public string EndpointsystemName { get; set; }
        public long EndpointUserID { get; set; }
        public string AccountName { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public DateTime CreatedDate { get; set; }
        public EmployeeEvents EmployeeEventType { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public static List<PresentationOUChangeSnapshots> GetByUserId(List<long> listUserID, bool includeOldRecord = false)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new OUChangeSnapshotsRepository(dataAccessTransaction).GetByUserActionUserId(listUserID, includeOldRecord));
            }
        }


        public static PresentationOUChangeSnapshots ORMap(EntityModel.Presentation.PresentationOUChangeSnapshots presentationOUChangeSnapshotEntityModel)
        {
            if (presentationOUChangeSnapshotEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationOUChangeSnapshots, PresentationOUChangeSnapshots>(presentationOUChangeSnapshotEntityModel);
        }

        public static List<PresentationOUChangeSnapshots> ORMapList(IList<EntityModel.Presentation.PresentationOUChangeSnapshots> presentationOUChangeSnapshotEntityModels)
        {
            return presentationOUChangeSnapshotEntityModels.Select(ORMap).ToList();
        }


    }
}
