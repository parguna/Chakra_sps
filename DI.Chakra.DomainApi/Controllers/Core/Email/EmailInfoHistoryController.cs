using DI.Chakra.DomainModel;
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
    [RoutePrefix("EmailInfoHistory")]
    public class EmailInfoHistoryController : BaseController<DomainModel.EmailInfoHistory, EntityModel.EmailInfoHistory, Dto.EmailInfoHistory>
    {
        //[HttpGet]
        //[Route("SendOTPEmail")]
        //[AllowAnonymous]
        //public bool SendOTPEmail()
        //{
        //    EmailAgent EA = new EmailAgent();
        //    EA.SendEmails(EmailActivityType.OTP);
        //    return true;
        //}

        //[HttpGet]
        //[Route("EmailCompose")]
        //[AllowAnonymous]
        //public bool EmailCompose(long userId)
        //{
        //    new EmailComposer().Compose(EmailTemplate.OTPNotification, userId);
        //    return true;
        //}
    }
}
