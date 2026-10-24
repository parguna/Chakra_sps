using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ConfigurationMasterPasswordManagement")]
    public class ConfigurationMasterPasswordManagementController : BaseController<DomainModel.ConfigurationMasterPasswordManagement, EntityModel.ConfigurationMasterPasswordManagement, Dto.ConfigurationMasterPasswordManagement>
    {
    }
}
