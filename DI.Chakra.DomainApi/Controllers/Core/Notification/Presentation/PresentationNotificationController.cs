using AutoMapper;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationNotification")]
    public class PresentationNotificationController : ApiController
    {
        [HttpGet]
        [Route("GetByUserIdStatus")]
        public List<PresentationNotification> GetByUserIdStatus(long userId, NotificationStatus status)
        {
            return DtoMapList(DomainModel.Presentation.PresentationNotification.GetByUserIdStatus(userId, status));
        }

        [HttpGet]
        [Route("GetByTaskId")]
        public List<PresentationNotification> GetByTaskId(long taskId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationNotification.GetByTaskId(taskId));
        }

        [HttpGet]
        [Route("GetByRequestId")]
        public List<PresentationNotification> GetByRequestId(long requestId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationNotification.GetByRequestId(requestId));
        }

        [HttpGet]
        [Route("GetReplyChainById")]
        public List<PresentationNotification> GetReplyChainById(long notificationId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationNotification.GetReplyChainById(notificationId));
        }
        
        [HttpGet]
        [Route("GetNotificationsById")]
        public List<PresentationNotification> GetNotificationsById(long notificationId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationNotification.GetNotificationsById(notificationId));
        }
        public PresentationNotification DtoMap(DomainModel.Presentation.PresentationNotification tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationNotification, PresentationNotification>(tModel);
        }

        public List<PresentationNotification> DtoMapList(IList<DomainModel.Presentation.PresentationNotification> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
