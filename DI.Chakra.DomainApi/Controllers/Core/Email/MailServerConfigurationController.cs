using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("MailServerConfiguration")]
    public class MailServerConfigurationController : BaseController<DomainModel.MailServerConfiguration, EntityModel.MailServerConfiguration, Dto.MailServerConfiguration>
    {
        [HttpGet]
        [Route("GetAllConfig")]
        [AllowAnonymous]
        public List<MailServerConfiguration> GetAllConfig()
        {
            return DtoMapList(DomainModel.MailServerConfiguration.GetAll());
        }
    }
}
