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
    [RoutePrefix("SAPEPGroupInfo")]
    public class SAPEPGroupInfoController : BaseController<DomainModel.SAPEPGroupInfo, EntityModel.SAPEPGroupInfo, Dto.SAPEPGroupInfo>
    {

        [HttpGet]
        [Route("GetGroupsById")]
        public  Dictionary<string, string> GetGroupsById(long Id, AdHocActionType actionType, string searchString, long endpointsystemId)
        { 
                return DomainModel.SAPEPGroupInfo.GetGroupsById(Id, actionType, searchString, endpointsystemId); 
        }

        [HttpGet]
        [Route("GetByRoleCode")]
        public Dto.SAPEPGroupInfo GetByName(long endpointSystemId, string groupName)
        {
            return DtoMap(DomainModel.SAPEPGroupInfo.GetByName(endpointSystemId, groupName));
        }

        [HttpGet]
        [Route("GetByEndPointId")]
        public List<SAPEPGroupInfo> GetByEndPointId(long endpointSystemId)
        {
            return DtoMapList(DomainModel.SAPEPGroupInfo.GetByEndPointId(endpointSystemId));
        }


        [HttpGet]
        [Route("GetPortalGroups")]
        public Dictionary<string, string> GetPortalGroups(long endpointSystemId, string searchString)
        {
            return DomainModel.SAPEPGroupInfo.GetPortalGroups(endpointSystemId, searchString);
        }
        [HttpGet]
        [Route("GetPortalGroupsByActionType")]
        public Dictionary<string, string> GetPortalGroupsByActionType(long endpointSystemId, string searchString, AdHocActionType actionType, string endpointUserId)
        {
            var endpointUserids = new List<long>();
            foreach (var item in DI.Chakra.Infrastructure.UtilityMethods.GetListFromString(endpointUserId))
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    endpointUserids.Add(Convert.ToInt64(item));
                }
            }
            
            return DomainModel.SAPEPGroupInfo.GetPortalGroupsByActionType(endpointSystemId, searchString, endpointUserids, actionType);
        }
        [HttpGet]
        [Route("GetGroupSAPEP")]
        public Dictionary<string, string> GetGroupSAPEP(long EndpointSystemId)
        {
            return DomainModel.SAPEPGroupInfo.GetGroupSAPEP(EndpointSystemId);
        }
        [HttpGet]
        [Route("GetGroupSAPEPForRemove")]
        public Dictionary<string, string> GetGroupSAPEPForRemove(int EndpointUserID)
        {
            return DomainModel.SAPEPUserInfo.GetGroupSAPEPForRemove(EndpointUserID);
        }
        [HttpGet]
        [Route("GetAllSAPEPGroups")]
        public List<SAPEPGroupInfo> GetAllSAPEPGroups(long endpointSystemId, bool isXLuploaded)
        {
            return DtoMapList(DomainModel.SAPEPGroupInfo.GetAllSAPEPGroups(endpointSystemId, isXLuploaded));
        }
    }
}
