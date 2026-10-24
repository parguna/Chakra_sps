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
    [RoutePrefix("PresentationSAPEPGroupToUsersReport")]

    public class PresentationSAPEPGroupToUsersReportController
    {
        public  PresentationSAPEPGroupToUsersReport DtoMap(DomainModel.Presentation.PresentationSAPEPGroupToUsersReport tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPEPGroupToUsersReport, PresentationSAPEPGroupToUsersReport>(tModel);
        }

        public  List<PresentationSAPEPGroupToUsersReport> DtoMapList(IList<DomainModel.Presentation.PresentationSAPEPGroupToUsersReport> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetByEndpointSystemId")]
        public IList<PresentationSAPEPGroupToUsersReport> GetByEndpointSystemId(long endpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPEPGroupToUsersReport.GetByEndpointSystemId(endpointSystemId));
        }
    }
}
