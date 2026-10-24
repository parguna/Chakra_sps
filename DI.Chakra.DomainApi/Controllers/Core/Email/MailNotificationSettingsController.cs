using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Services.EmailingProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("MailNotificationSettings")]
    public class MailNotificationSettingsController : BaseController<DomainModel.MailNotificationSettings, EntityModel.MailNotificationSettings, Dto.MailNotificationSettings>
    {
        [HttpGet]
        [Route("GetMailNotificationSettingsByTrigger")]
        public MailNotificationSettings GetMailNotificationSettingsByTrigger(string TriggerName)
        {
            return DtoMap(DomainModel.MailNotificationSettings.GetMailNotificationSettingsByTrigger(TriggerName));
        }
    }
}