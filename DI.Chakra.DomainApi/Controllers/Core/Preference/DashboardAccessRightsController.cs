using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("DashboardAccessRights")]
    public class DashboardAccessRightsController : BaseController<DomainModel.DashboardAccessRights, EntityModel.DashboardAccessRights, Dto.DashboardAccessRights>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(long roleId, long dashboardId)
        {
            return DomainModel.DashboardAccessRights.DoesExists(roleId, dashboardId);
        }

        [HttpGet]
        [Route("GetAllWithNavigation")]
        public List<DashboardAccessRights> GetAllWithNavigation()
        {
            return DtoMapList(DomainModel.DashboardAccessRights.GetAllWithNavigation());
        }

        [HttpGet]
        [Route("Delete")]
        public  new void Delete(long id)
        {
            DomainModel.DashboardAccessRights.Delete(id);
        }
    }
}
