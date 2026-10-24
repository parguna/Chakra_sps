using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers.Endpoint.OneCard
{
    [RoutePrefix("SMSAccessLevel")]
    public class SMSAccessLevelController : BaseController<DomainModel.SMSAccessLevel, EntityModel.SMSAccessLevel, Dto.SMSAccessLevel>
    {
        [HttpGet]
        [Route("GetAllBuildings")]
        public Dictionary<string, string> GetAllBuildings()
        {
            return DomainModel.SMSXLRoleCatalogue.GetAllBuildings();
        }
        [HttpGet]
        [Route("GetSectionByBuilding")]
        public Dictionary<string, string> GetSectionByBuilding(string Building)
        {
            return DomainModel.SMSXLRoleCatalogue.GetSectionByBuilding(Building);
        }
        [HttpGet]
        [Route("GetTechRolesByBuildingandSection")]
        public Dictionary<string, string> GetTechRolesByBuildingandSection(string Building, string section)
        {
            return DomainModel.SMSXLRoleCatalogue.GetTechRolesByBuildingandSection(Building, section);
        }
        [HttpGet]
        [Route("GetExistingTechRoles")]
        public Dictionary<string, string> GetExistingTechRoles(long userid,long endpointUserId,long endpointSystemId)
        {
            return DomainModel.SMSUserInfo.GetExistingTechRoles(userid, endpointUserId, endpointSystemId);
        }
        [HttpGet]
        [Route("GetExistingRolesByEndpointUserId")]
        public Dictionary<string, string> GetExistingRolesByEndpointUserId(long endpointUserID, long endpointsystemId)
        {
            return DomainModel.SMSUserInfo.GetExistingRolesByEndpointUserId(endpointUserID, endpointsystemId);
        }

        [HttpGet]
        [Route("GetDescription")]
        public string GetDescription(string techRole)
        {
            return DomainModel.SMSXLRoleCatalogue.GetDescription(techRole);
        }
        [HttpGet]
        [Route("GetBuildingandSectionByTechRoles")]
        public Dto.SMSXLRoleCatalogue GetBuildingandSectionByTechRoles(string techRole)
        {
            return AutoMapper.Mapper.Map<Dto.SMSXLRoleCatalogue>(DomainModel.SMSXLRoleCatalogue.GetBuildingandSectionByTechRoles(techRole));
        }
        [HttpGet]
        [Route("GetDistinctTechRoles")]
        public Dictionary<string, string> GetDistinctTechRoles()
        {
            return DomainModel.SMSXLRoleCatalogue.GetDistinctTechRoles();
        }
    }
}