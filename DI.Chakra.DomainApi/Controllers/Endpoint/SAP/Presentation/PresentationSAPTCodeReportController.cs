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
    [RoutePrefix("PresentationSAPTCodeReport")]

    public class PresentationSAPTCodeReportController : ApiController
    {
        public  PresentationSAPTCodeReport DtoMap(DomainModel.Presentation.PresentationSAPTCodeReport tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPTCodeReport, PresentationSAPTCodeReport>(tModel);
        }

        public List<PresentationSAPTCodeReport> DtoMapList(IList<DomainModel.Presentation.PresentationSAPTCodeReport> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetAllSapTCodesReportData")]
        public IList<PresentationSAPTCodeReport> GetSAPUserToProfilesReportData(long endpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPTCodeReport.GetAllSapTCodesReportData(endpointSystemId));
        }
    }
}
