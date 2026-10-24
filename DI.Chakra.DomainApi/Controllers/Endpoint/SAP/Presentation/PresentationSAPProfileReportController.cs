using AutoMapper;
using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationSAPProfileReport")]

    public class PresentationSAPProfileReportController : ApiController
    {
        public  PresentationSAPProfileReport DtoMap(DomainModel.Presentation.PresentationSAPProfileReport tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPProfileReport, PresentationSAPProfileReport>(tModel);
        }

        public  List<PresentationSAPProfileReport> DtoMapList(IList<DomainModel.Presentation.PresentationSAPProfileReport> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetSAPUserToProfilesReportData")]
        public IList<PresentationSAPProfileReport> GetSAPUserToProfilesReportData(long endpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPProfileReport.GetSAPUserToProfilesReportData(endpointSystemId));
        }
    }
}
