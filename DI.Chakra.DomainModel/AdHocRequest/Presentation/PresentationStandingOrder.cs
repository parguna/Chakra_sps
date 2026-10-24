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
    public class PresentationStandingOrder
    {
        public long? EndpointSystemId { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public string EntitlementDetail { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public byte[] AttachmentFile { get; set; }

    
        public static PresentationStandingOrder ORMap(EntityModel.Presentation.PresentationStandingOrder presentationStandingOrder)
        {
            if (presentationStandingOrder == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationStandingOrder, PresentationStandingOrder>(presentationStandingOrder);
        }

        public static List<PresentationStandingOrder> ORMapList(IList<EntityModel.Presentation.PresentationStandingOrder> presentationStandingOrder)
        {
            return presentationStandingOrder.Select(ORMap).ToList();
        }

    }
}
