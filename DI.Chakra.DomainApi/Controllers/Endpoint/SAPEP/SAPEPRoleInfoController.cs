using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPEPRoleInfo")]
    public class SAPEPRoleInfoController : BaseController<DomainModel.SAPEPRoleInfo, EntityModel.SAPEPRoleInfo, Dto.SAPEPRoleInfo>
    {
        [HttpGet]
        [Route("GetRolesById")]
        public Dictionary<string, string> GetRolesById(long Id, AdHocActionType actionType, string searchString, long endpointsystemId)
        {
            return DomainModel.SAPEPRoleInfo.GetRolesById(Id, actionType, searchString, endpointsystemId);
        }

        [HttpGet]
        [Route("GetByName")]
        public Dto.SAPEPRoleInfo GetByName(long endpointSystemId, string roleName)
        {
            return DtoMap(DomainModel.SAPEPRoleInfo.GetByName(endpointSystemId, roleName));
        }
        [HttpGet]
        [Route("GetByEndPointId")]
        public List<SAPEPRoleInfo> GetByEndPointId(long endpointSystemId)
        {
            return DtoMapList(DomainModel.SAPEPRoleInfo.GetByEndPointId(endpointSystemId));
        }


        [HttpGet]
        [Route("GetPortalRoles")]
        public Dictionary<string, string> GetPortalRoles(long endpointSystemId, string searchString)
        {
            return DomainModel.SAPEPRoleInfo.GetPortalRoles((long)endpointSystemId, searchString);
        }
        [HttpGet]
        [Route("GetPortalRolesByActionType")]
        public Dictionary<string, string> GetPortalRolesByActionType(long endpointSystemId, string searchString, AdHocActionType actionType, string endpointUserId)
        {
            var endpointUserids = new List<long>();
            foreach (var item in DI.Chakra.Infrastructure.UtilityMethods.GetListFromString(endpointUserId))
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    endpointUserids.Add(Convert.ToInt64(item));
                }
            }

            return DomainModel.SAPEPRoleInfo.GetPortalRolesByActionType(endpointSystemId, searchString, endpointUserids, actionType);
        }
        [HttpGet]
        [Route("GetRoleSAPEP")]
        public Dictionary<string, string> GetRoleSAPEP(long EndpointSystemId)
        {
            return DomainModel.SAPEPRoleInfo.GetRoleSAPEP(EndpointSystemId);
        }
        [HttpGet]
        [Route("GetRolesSAPEPForRemove")]
        public Dictionary<string, string> GetRolesSAPEPForRemove(long EndpointUserID)
        {
            return DomainModel.SAPEPUserInfo.GetRolesSAPEPForRemove(EndpointUserID);
        }

        [HttpGet]
        [Route("GetAllSAPEPRoles")]
        public List<SAPEPRoleInfo> GetAllSAPEPRoles(long endpointSystemId, bool isXLuploaded)
        {
            return DtoMapList(DomainModel.SAPEPRoleInfo.GetAllSAPEPRoles(endpointSystemId, isXLuploaded));
        }
    }
}
