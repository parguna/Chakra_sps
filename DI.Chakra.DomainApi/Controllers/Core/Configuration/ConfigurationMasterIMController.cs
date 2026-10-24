using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ConfigurationMasterIM")]
    public class ConfigurationMasterIMController : BaseController<DomainModel.ConfigurationMasterIM, EntityModel.ConfigurationMasterIM, Dto.ConfigurationMasterIM>
    {

    }
}
