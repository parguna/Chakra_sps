using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("Dashboard")]
    public class DashboardController : BaseController<DomainModel.Dashboard, EntityModel.Dashboard, Dto.Dashboard>
    {
        [HttpGet]
        [Route("GetByDashBoardCategory")]
        public List<Dashboard> GetByDashBoardCategory(string dashboardCategory)
        {
            return DtoMapList(DomainModel.Dashboard.GetByDashBoardCategory(dashboardCategory));
        }

        [HttpGet]
        [Route("GetDashboardCategoryByRoleId")]
        public List<string> GetDashboardCategoryByRoleId(long roleId)
        {
            return DomainModel.Dashboard.GetDashboardCategoryByRoleId(roleId);
        }

        [HttpGet]
        [Route("GetDashboardType")]
        public DashboardType GetDashboardType(long id)
        {
            return DomainModel.Dashboard.GetDashboardType(id);
        }

        [HttpGet]
        [Route("GetByRoleId")]
        public List<Dashboard> GetByRoleId(long roleId)
        {
            return DtoMapList(DomainModel.Dashboard.GetByRoleId(roleId));
        }
        [HttpGet]
        [Route("GetByRoleIdAndCategory")]
        public List<Dashboard> GetByRoleIdAndCategory(long roleId, string category)
        {
            return DtoMapList(DomainModel.Dashboard.GetByRoleIdAndCategory(roleId, category));
        }

        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string name,long id)
        {
            return DomainModel.Dashboard.DoesExists(name,id);
        }
    }
}
