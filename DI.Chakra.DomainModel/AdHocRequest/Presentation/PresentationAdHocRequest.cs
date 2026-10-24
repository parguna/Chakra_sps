using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
   public class PresentationAdHocRequest
    {
        public AdHocAccessRequest AdHocAccessRequests { get; set; }


        public static PresentationAdHocRequest ORMap(EntityModel.Presentation.PresentationAdHocRequest presentationadhocRequestEntityModel)
        {
            if (presentationadhocRequestEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAdHocRequest, PresentationAdHocRequest>(presentationadhocRequestEntityModel);
        }

        public static List<PresentationAdHocRequest> ORMapList(IList<EntityModel.Presentation.PresentationAdHocRequest> presentationadhocRequestEntityModel)
        {
            return presentationadhocRequestEntityModel.Select(ORMap).ToList();
        }

    }
}
