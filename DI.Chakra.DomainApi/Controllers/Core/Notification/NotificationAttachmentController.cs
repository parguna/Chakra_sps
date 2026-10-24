using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NotificationAttachment")]
    public class NotificationAttachmentController : BaseController<DomainModel.NotificationAttachment, EntityModel.NotificationAttachment, NotificationAttachment>
    {
    }
}
