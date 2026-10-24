using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;


namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("MailTemplate")]
    public class MailTemplateController : BaseController<DomainModel.MailTemplate, EntityModel.MailTemplate, Dto.MailTemplate>
    {
        [HttpGet]
        [Route("GetByTemplateName")]
        public MailTemplate GetByTemplateName(string templateName)
        {
            return DtoMap (DomainModel.MailTemplate.GetByTemplateName(templateName));
        }

        [HttpGet]
        [Route("DoesNameAlreadyExists")]
        public bool DoesNameAlreadyExists(string templateName)
        {
            return DomainModel.MailTemplate.DoesNameAlreadyExists(templateName);
        }
      
    }
}
