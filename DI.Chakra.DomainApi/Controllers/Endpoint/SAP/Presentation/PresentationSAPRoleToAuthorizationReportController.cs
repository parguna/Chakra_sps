using AutoMapper;
using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers.Endpoint.SAP.Presentation
{
    [RoutePrefix("PresentationSAPRoleToAuthorizationReport")]
    public class PresentationSAPRoleToAuthorizationReportController : ApiController
    {
        public PresentationSAPRoleToAuthorizationReport DtoMap(DomainModel.Presentation.PresentationSAPRoleToAuthorizationReport tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPRoleToAuthorizationReport, PresentationSAPRoleToAuthorizationReport>(tModel);
        }

        public List<PresentationSAPRoleToAuthorizationReport> DtoMapList(IList<DomainModel.Presentation.PresentationSAPRoleToAuthorizationReport> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
        [HttpGet]
        [Route("GetSapRolesToAuthorizationData")]
        public List<PresentationSAPRoleToAuthorizationReport> GetSapRolesToAuthorizationData(string roleCode, long endPointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPRoleToAuthorizationReport.GetSapRolesToAuthorizationData(roleCode, endPointSystemId));

        }
    }
}