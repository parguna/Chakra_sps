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
    [RoutePrefix("SkypeForBusinessUserInfo")]
    public class SkypeForBusinessUserInfoController : BaseController<DomainModel.SkypeForBusinessUserInfo, EntityModel.SkypeForBusinessUserInfo, Dto.SkypeForBusinessUserInfo>
    {
        [HttpGet]
        [Route("GetSkypeForBusinessUserById")]
        public Dto.SkypeForBusinessUserInfo GetSkypeForBusinessUserById(long userId,long endpointSystemId)
        {
            return DtoMap(DomainModel.SkypeForBusinessUserInfo.GetPrimaryByDIUserId(userId, endpointSystemId));
        }
        [HttpGet]
        [Route("GetUserWithResponseGroup")]
        public Dto.SkypeForBusinessUserInfo GetUserWithResponseGroup(long userId, long endpointSystemId)
        {
            return DtoMap(DomainModel.SkypeForBusinessUserInfo.GetByIdIncludeGroups(userId, endpointSystemId));
        }
        [HttpGet]
        [Route("IsDCCNumberExists")]
        public bool IsDCCExists(long endpointId, long number, AdHocActionType actionType,long accessRequestId)
        {
            return DomainModel.SkypeForBusinessUserInfo.IsDCCExists(endpointId, number, actionType, accessRequestId);
        }
        
            [HttpGet]
        [Route("HasUserHoldAnyResponseGroup")]
        public bool HasUserHoldAnyResponseGroup(long userId, long endpointSystemId)
        {
            var isExists = DomainModel.SkypeForBusinessUserInfo.GetByIdIncludeGroups(userId, endpointSystemId)?.SkypeForBusinessResponseGroup.Count > 0;
            return isExists;
        }

    }
}