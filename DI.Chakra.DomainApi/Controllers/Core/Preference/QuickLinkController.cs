using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("QuickLink")]
    public class QuickLinkController : BaseController<DomainModel.QuickLink, EntityModel.QuickLink, Dto.QuickLink>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string qlName)
        {
           return DomainModel.QuickLink.DoesExists(qlName);
        }

        [HttpGet]
        [Route("GetByRoleId")]
        public  IList<QuickLink> GetByRoleId(long roleId)
        {
            return DtoMapList(DomainModel.QuickLink.GetByRoleId(roleId));
        }

        [HttpGet]
        [Route("GetByCategory")]
        public List<QuickLink> GetByCategory(string category)
        {
            return DtoMapList(DomainModel.QuickLink.GetByCategory(category));
        }

        [HttpGet]
        [Route("GetByName")]
        public QuickLink GetByName(string id)
        {
            return DtoMap(DomainModel.QuickLink.GetByName(id));
        }
    }
}
