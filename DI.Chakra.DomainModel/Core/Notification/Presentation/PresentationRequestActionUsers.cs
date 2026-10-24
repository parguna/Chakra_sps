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
    public class PresentationRequestActionUsers
    {
        public long? TaskId { get; set; }
        public long? RequestId { get; set; }
        public string ActionUser { get; set; }
        public long? ActionUserId { get; set; }


        public static List<PresentationRequestActionUsers> GetActionUsersByRequestId(long requestId, string source)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if(!string.IsNullOrEmpty(source) &&  source == DI.Chakra.Infrastructure.Constants.BulkRequest)
                    return ORMapList(new PresentationNotificationRepository(dataAccessTransaction).GetActionUsersByBulkRequestId(requestId));
                else
                   return ORMapList(new PresentationNotificationRepository(dataAccessTransaction).GetActionUsersByRequestId(requestId));
            }
        }


        public static PresentationRequestActionUsers ORMap(EntityModel.Presentation.PresentationRequestActionUsers presentationNotificationEntityModel)
        {
            if (presentationNotificationEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationRequestActionUsers, PresentationRequestActionUsers>(presentationNotificationEntityModel);
        }

        public static List<PresentationRequestActionUsers> ORMapList(IList<EntityModel.Presentation.PresentationRequestActionUsers> presentationNotificationEntityModel)
        {
            return presentationNotificationEntityModel.Select(ORMap).ToList();
        }



    }
}
