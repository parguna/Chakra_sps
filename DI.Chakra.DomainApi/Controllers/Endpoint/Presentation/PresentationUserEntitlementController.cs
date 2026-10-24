using AutoMapper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{

    [RoutePrefix("PresentationUserEntitlement")]
    public class PresentationUserEntitlementController : ApiController
    {
        [HttpGet]
        [Route("GetUserAccountDetail")]
        public List<Dto.Presentation.PresentationUserEntitlement> GetUserAccountDetail(long userId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserEntitlement.GetUserAccountDetail(userId));
        }

        [HttpGet]
        [Route("GetUserAccountDetailInactiveAccounts")]
        public List<Dto.Presentation.PresentationUserEntitlement> GetUserAccountDetailInactiveAccounts(long userId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserEntitlement.GetUserAccountDetailInactiveAccounts(userId));
        }

        [HttpGet]
        [Route("GetUserAccountEntitlment")]
        public List<Dto.Presentation.PresentationUserEntitlement> GetUserAccountEntitlment(long userId, long endpointUserId, long endpointSytemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserEntitlement.GetUserAccountEntitlment(userId, endpointUserId, endpointSytemId));
        }

        [HttpGet]
        [Route("GetEntitlementDetail")]
        public List<Dto.Presentation.PresentationUserEntitlement> GetEntitlementDetail(long endpointUserId, EndpointType applicationType)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserEntitlement.GetEntitlementDetail(endpointUserId, applicationType));
        }

        [HttpGet]
        [Route("GetEntitlementHierachyDetail")]
        public List<Dto.Presentation.PresentationUserEntitlement> GetEntitlementHierachyDetail(long endpointUserId, EndpointType applicationType)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserEntitlement.GetEntitlementHierachyDetail(endpointUserId, applicationType));
        }

        [Route("GetDetailById")]

        public List<Dto.Presentation.PresentationUserEntitlement> GetDetailById(long userID)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserEntitlement.GetDetailById(userID));
        }

        [HttpGet]
        [Route("GetEntitlementDetailByApplicationType")]
        public List<Dto.Presentation.PresentationUserEntitlement> GetEntitlementDetailByApplicationType(long endpointsystemId, EndpointType applicationType)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserEntitlement.GetEntitlementDetailByApplicationType(endpointsystemId, applicationType));
        }

        [HttpGet]
        [Route("GetEntitlementDetailByApplicationTypeWithAllCanRequest")]
        public List<Dto.Presentation.PresentationUserEntitlement> GetEntitlementDetailByApplicationTypeWithAllCanRequest(long endpointsystemId, EndpointType applicationType)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserEntitlement.GetEntitlementDetailByApplicationTypeWithAllCanRequest(endpointsystemId, applicationType));
        }

        [HttpGet]
        [Route("GetEntitlement")]
        public Dto.Presentation.PresentationUserEntitlement GetEntitlement(long id, EndpointType applicationType, string entitlementType)
        {
            return DtoMap(DomainModel.Presentation.PresentationUserEntitlement.GetEntitlement(id, applicationType, entitlementType));
        }

        [HttpGet]
        [Route("GetEntitlement")]
        public string GetEntitlement(long id, long endpointSystemId, string entitlementType)
        {
            return DomainModel.Presentation.PresentationUserEntitlement.GetEntitlement(id, endpointSystemId, entitlementType);
        }

        [HttpPost]
        [Route("UpdateEntitlement")]
        public void UpdateEntitlement([FromBody]Dto.Presentation.PresentationUserEntitlement entitlement)
        {
            try
            {
                DomainModel.Presentation.PresentationUserEntitlement.UpdateEntitlement(AutoMapperHelper.DtoUnMap<Dto.Presentation.PresentationUserEntitlement, DomainModel.Presentation.PresentationUserEntitlement>(entitlement), User.Identity.Name);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        [Route("GetEntitlementList")]
        public List<EntitlementType> GetEntitlementList(EndpointType applicationType)
        {
            return DomainModel.Presentation.PresentationUserEntitlement.GetEntitlementList(applicationType);
        }

        [HttpGet]
        [Route("GetEntitlementName")]
        public Dictionary<long, string> GetEntitlementName(EndpointType applicationType, long endpointSystemId, string entitlementType, string searchString)
        {
            return DomainModel.Presentation.PresentationUserEntitlement.GetEntitlementName(applicationType, endpointSystemId, entitlementType, searchString);
        }
        public List<Dto.Presentation.PresentationUserEntitlement> DtoMapList(IList<DomainModel.Presentation.PresentationUserEntitlement> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        public  Dto.Presentation.PresentationUserEntitlement DtoMap(DomainModel.Presentation.PresentationUserEntitlement tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationUserEntitlement, Dto.Presentation.PresentationUserEntitlement>(tModel);
        }
        public DomainModel.Presentation.PresentationUserEntitlement DtoUnMap(Dto.Presentation.PresentationUserEntitlement tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<Dto.Presentation.PresentationUserEntitlement, DomainModel.Presentation.PresentationUserEntitlement>(tModel);
        }
    }
}
