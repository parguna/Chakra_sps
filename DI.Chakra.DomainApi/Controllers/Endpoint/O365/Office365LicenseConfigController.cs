using AutoMapper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers.Endpoint.O365
{

    [RoutePrefix("Office365LicenseConfig")]

    public class Office365LicenseConfigController : BaseController<DomainModel.Office365LicenseConfig, EntityModel.Office365LicenseConfig, Dto.Office365LicenseConfig>
    {
        [HttpPost]
        [Route("GetAdhocOffice365LicenseInfos")]
        public Dictionary<string, string> GetAdhocOffice365LicenseInfos(string SearchString, AdHocActionType adhocActionType, [FromBody] Office365LicenseConfigDto office365LicenseConfigDto)
        {
            try
            {
                List<DomainModel.AdHocLicenseInfo> AdHocLicenseInfo = new List<DomainModel.AdHocLicenseInfo>();
                List<DomainModel.ServicePlanDetail> ExistingServicePlanDetail = new List<DomainModel.ServicePlanDetail>();
                if (office365LicenseConfigDto != null)
                {
                    if (office365LicenseConfigDto.AdHocLicenseInfo != null)
                        AdHocLicenseInfo = office365LicenseConfigDto.AdHocLicenseInfo.Select(info => Mapper.Map<Dto.AdHocLicenseInfo, DomainModel.AdHocLicenseInfo>(info)).ToList();
                    if (office365LicenseConfigDto.ExistingServicePlanDetail != null)
                        ExistingServicePlanDetail = office365LicenseConfigDto.ExistingServicePlanDetail.Select(detail => Mapper.Map<Dto.ServicePlanDetail, DomainModel.ServicePlanDetail>(detail)).ToList();
                }
                return DomainModel.Office365LicenseConfig.GetAdhocOffice365LicenseInfos(SearchString, AdHocLicenseInfo, ExistingServicePlanDetail, adhocActionType);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("GetOffice365ServicePlansforBulkReq")]
        public List<Office365LicenseConfig> GetOffice365ServicePlansforBulkReq(string License)
        {
            return DtoMapList(DomainModel.Office365LicenseConfig.GetOffice365ServicePlansforBulkReq(License));

        }

        [HttpPost]
        [Route("GetLicenseInfoForBulkReq")]
        public Dictionary<string, string> GetLicenseInfoForBulkReq(string SearchString, [FromBody]Office365LicenseConfigDto office365LicenseConfigDto)
        {
            List<DomainModel.AdHocLicenseInfo> AdHocLicenseInfo = new List<DomainModel.AdHocLicenseInfo>();
            if (office365LicenseConfigDto != null)
            {
                AdHocLicenseInfo = office365LicenseConfigDto.AdHocLicenseInfo.Select(info => Mapper.Map<Dto.AdHocLicenseInfo, DomainModel.AdHocLicenseInfo>(info)).ToList();
            }
            return DomainModel.Office365LicenseConfig.GetLicenseInfoForBulkReq(SearchString, AdHocLicenseInfo);

        }

        [HttpGet]
        [Route("GetO365Users")]
        public Dictionary<string, string> GetO365Users(string searchString, long endpointsystemId)
        {
            return DomainModel.Office365LicenseConfig.GetO365Users(searchString, endpointsystemId);
        }
    }
}