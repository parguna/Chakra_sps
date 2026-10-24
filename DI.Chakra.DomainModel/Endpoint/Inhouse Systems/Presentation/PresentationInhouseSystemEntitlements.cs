using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationInhouseSystemEntitlements
    {
        public int ID { get; set; }
        public int InhouseSystemEntitlementId { get; set; }
        public int InhouseSystemsId { get; set; }
        public int EndPointSystemID { get; set; }
        public string InhouseSystemsCode { get; set; }
        public string SystemName { get; set; }
        public string Description { get; set; }
        public string ApplicationOwner { get; set; }
        public string EntitlementName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public bool CanRequestMultiple { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }

        public static PresentationInhouseSystemEntitlements ORMap(EntityModel.Presentation.PresentationInhouseSystemEntitlements presentationTabMenuEntityModel)
        {
            if (presentationTabMenuEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationInhouseSystemEntitlements, PresentationInhouseSystemEntitlements>(presentationTabMenuEntityModel);
        }

        public static List<PresentationInhouseSystemEntitlements> ORMapList(IList<EntityModel.Presentation.PresentationInhouseSystemEntitlements> presentationInhouseGridEntityModels)
        {
            return presentationInhouseGridEntityModels.Select(ORMap).ToList();
        }
    }
}
