using AutoMapper;
using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("UserSODComplianceReport")]
    public class UserSODComplianceReportController : BaseController<DomainModel.UserSODComplianceReport, EntityModel.UserSODComplianceReport, Dto.UserSODComplianceReport>
    {

        [HttpPost]
        [Route("GetUserSODComplianceReport")]
        public List<UserSODComplianceReport> GetUserSODComplianceReport(UserSODComplianceRequest request)
        {
            return DtoMapList(DomainModel.CompliancePolicy.CheckUserCompliance(request));
        }

        public List<UserSODComplianceReport> DtoMapList(IList<DomainModel.UserSODComplianceReport> tModelList)
        {
                        return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetReportbyUserId")]
        public  List<UserSODComplianceReport> GetReportbyUserId(long userId)
        {
            return DtoMapList(DomainModel.UserSODComplianceReport.GetReportbyUserId(userId));
        }

        [HttpGet]
        [Route("GetReportByUserIdFromDiscovery")]
        public List<UserSODComplianceReport> GetReportByUserIdFromDiscovery(long userId)
        {
            return DtoMapList(DomainModel.UserSODComplianceReport.GetReportByUserIdFromDiscovery(userId));
        }
        [HttpGet]
        [Route("GetReportByUserIdFromRequest")]
        public List<UserSODComplianceReport> GetReportByUserIdFromRequest(long userId)
        {
            return DtoMapList(DomainModel.UserSODComplianceReport.GetReportByUserIdFromRequest(userId));
        }
        public UserSODComplianceReport DtoMap(DomainModel.UserSODComplianceReport tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.UserSODComplianceReport, UserSODComplianceReport>(tModel);
        }

       
    }
}