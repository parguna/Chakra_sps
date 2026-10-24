using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("DefaultRoleDashboardPreference")]
    public class DefaultRoleDashboardPreferenceController : BaseController<DomainModel.DefaultRoleDashboardPreference, EntityModel.DefaultRoleDashboardPreference, Dto.DefaultRoleDashboardPreference>
    {
        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(long roleId, long dashBoardId)
        {
            return DomainModel.DefaultRoleDashboardPreference.DoesExists(roleId, dashBoardId);
        }

        [HttpGet]
        [Route("GetByIdWithNavigaton")]
        public DefaultRoleDashboardPreference GetByIdWithNavigaton(long id)
        {
            return DtoMap(DomainModel.DefaultRoleDashboardPreference.GetByIdWithNavigaton(id));
        }

        [HttpGet]
        [Route("GetAllOrderedWithNavigaton")]
        public List<DefaultRoleDashboardPreference> GetAllOrderedWithNavigaton()
        {
            return DtoMapList(DomainModel.DefaultRoleDashboardPreference.GetAllOrderedWithNavigaton());
        }

        [HttpGet]
        [Route("DoesExistsByPosition")]
        public bool DoesExistsByPosition(long dbPosition, long roleID)
        {
            return DomainModel.DefaultRoleDashboardPreference.DoesExistsByPosition(dbPosition, roleID);
        }
    }
}
