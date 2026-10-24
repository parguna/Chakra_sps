using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AdminModeItem")]
    public class AdminModeItemController : BaseController<DomainModel.AdminModeItem, EntityModel.AdminModeItem, AdminModeItem>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string name)
        {
            return DomainModel.AdminModeItem.DoesExists(name);
        }

        [HttpGet]
        [Route("GetAllIncludingPageUrl")]
        public  IList<AdminModeItem> GetAllIncludingPageUrl()
        {
            return DtoMapList(DomainModel.AdminModeItem.GetAllIncludingPageUrl());
        }
    }
}