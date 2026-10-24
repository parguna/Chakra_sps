using AutoMapper;
using DI.Chakra.DomainApi.Controllers;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
namespace DI.Chakra.DomainApi
{
    [RoutePrefix("SkypeForBusinessResponseGroupInfo")]
    public class SkypeForBusinessResponseGroupInfoController : BaseController<DomainModel.SkypeForBusinessResponseGroupInfo, EntityModel.SkypeForBusinessResponseGroupInfo, Dto.SkypeForBusinessResponseGroupInfo>
    {
        [HttpGet]
        [Route("GetSkypeForBusinessResponseGroupById")]
        public Dto.SkypeForBusinessResponseGroupInfo GetSkypeForBusinessResponseGroupById(long groupId, long endpointsystemid)
        {
            return DtoMap(DomainModel.SkypeForBusinessResponseGroupInfo.GetById(groupId));
        }
        [HttpGet]
        [Route("GetResponseGroup")]
        public Dictionary<long, string> GetResponseGroup(long endPointUserId, long endpointId, string searchString, AdHocActionType adHocActionType)
        {
            return DomainModel.SkypeForBusinessResponseGroupInfo.GetResponseGroup(endPointUserId, endpointId, searchString, adHocActionType);
        }
        [HttpGet]
        [Route("GetResponseGroupForBulkRequest")]
        public Dictionary<long, string> GetResponseGroupForBulkRequest(long endpointId, string searchString, AdHocActionType adHocActionType)
        {
            return DomainModel.SkypeForBusinessResponseGroupInfo.GetResponseGroupForBulkRequest(endpointId, searchString, adHocActionType);
        }
        [HttpGet]
        [Route("GetResponseGroupUser")]
        public List<SkypeForBusinessUserInfo> GetResponseGroupUser(string searchString)
        {
            var results = new List<SkypeForBusinessUserInfo>();
            var items = DomainModel.SkypeForBusinessResponseGroupInfo.GetResponseGroupUser(searchString);
            foreach (var entityResult in items)
            {
                results.Add(new SkypeForBusinessUserInfo()
                {
                    UserPrincipalName = entityResult.UserPrincipalName,
                    LastName = entityResult.LastName,
                    LineURI = entityResult.LineURI,
                });
            }
            return results;
        }
    }
}