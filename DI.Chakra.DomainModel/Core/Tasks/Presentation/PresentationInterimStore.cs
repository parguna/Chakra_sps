using AutoMapper;
using DI.Chakra.Infrastructure;
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
    public class PresentationInterimStore 
    {
        public long Id { get; set; }
        public InterimStoreItemType InterimStoreItemType { get; set; }

        public long? TaskId { get; set; }

        public long? UserId { get; set; }

        public string XmlData { get; set; }

        public bool IsSaved { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
        public bool HasAttachment { get; set; }
        public string AttachmentFileName { get; set; }

        public static IList<PresentationInterimStore> GetPresentationByUserId(long userId, bool isSaved, InterimStoreItemType itemType)
        {
            IList<PresentationInterimStore> interim = new List<PresentationInterimStore>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                interim = ORMapList(new InterimStoreRepository(dataAccessTransaction).GetInterimStoreAccessRequestForUser(userId, isSaved, itemType).ToList());
            }
            return interim;
        }

        public static PresentationInterimStore ORMap(EntityModel.Presentation.PresentationInterimStore presentationTaskGridDataEntityModel)
        {
            if (presentationTaskGridDataEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationInterimStore, PresentationInterimStore>(presentationTaskGridDataEntityModel);
        }

        public static List<PresentationInterimStore> ORMapList(IList<EntityModel.Presentation.PresentationInterimStore> presentationTaskGridDataEntityModels)
        {
            return presentationTaskGridDataEntityModels.Select(ORMap).ToList();
        }
    }
}
