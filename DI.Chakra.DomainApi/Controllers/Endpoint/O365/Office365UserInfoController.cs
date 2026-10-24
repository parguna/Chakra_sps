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
    [RoutePrefix("Office365UserInfo")]
    public class Office365UserInfoController : BaseController<DomainModel.Office365UserInfo, EntityModel.Office365UserInfo, Dto.Office365UserInfo>
    {
        [HttpGet]
        [Route("GetO365UserByUserId")]
        public Dto.Office365UserInfo GetO365UserByUserId(long userId, AccountType accountType, long endpointsystemid)
        {
            return DtoMap(DomainModel.Office365UserInfo.GetO365UserByUserId(userId, accountType, endpointsystemid));
        }
        [HttpGet]
        [Route("GetAdhocOffice365ServicePlans")]
        public List<Office365LicenseConfig> GetAdhocOffice365ServicePlans(string License, long endpointUser, AccountType accountType, long EndpointSystemId, AdHocActionType adhocActionType)
        {
            List<DomainModel.ServicePlanDetail> _existingServicePlanDetails = new List<DomainModel.ServicePlanDetail>();
            DomainModel.Office365UserInfo userinfo = DomainModel.Office365UserInfo.GetO365UserByUserId(endpointUser, accountType, EndpointSystemId);
            if (userinfo != null)
            {
                DomainModel.Office365UserServicePlan obj = userinfo.ServicePlanObject;
                _existingServicePlanDetails = obj == null ? new List<DomainModel.ServicePlanDetail>() : obj.PlanDetails.Where(item => item.ProvisioningStatus != "Disabled").ToList();
            }
            return DomainModel.Office365UserInfo.GetAdhocOffice365ServicePlans(License, _existingServicePlanDetails, adhocActionType).Select(item => Mapper.Map<Dto.Office365LicenseConfig>(item)).ToList(); ;
        }
             
        
    }
}