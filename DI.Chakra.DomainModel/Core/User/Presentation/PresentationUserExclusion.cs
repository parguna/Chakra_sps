using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationUserExclusion : DomainModelBase<PresentationUserExclusion, EntityModel.Presentation.PresentationUserExclusion>
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string EventType { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string FileName { get; set; }
        public byte[] AttachmentFile { get; set; }
        public string ContentType { get; set; }
        public string Remarks { get; set; }
        public bool HasAttachments { get; set; }
        public string DisplayName { get; set; }
        public bool IsDeleted { get; set; }
        public static PresentationUserExclusion ORMap(EntityModel.Presentation.PresentationUserExclusion presentationUserExclusion)
        {
            if (presentationUserExclusion == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationUserExclusion, PresentationUserExclusion>(presentationUserExclusion);
        }

        public static List<PresentationUserExclusion> ORMapList(IList<EntityModel.Presentation.PresentationUserExclusion> presentationUserExclusion)
        {
            return presentationUserExclusion.Select(ORMap).ToList();
        }

        public static List<Presentation.PresentationUserExclusion> GetAllUserExclusion()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationUserExclusionRepository(dataAccessTransaction).GetAllUserExclusion());
            }
        }
    }
}
