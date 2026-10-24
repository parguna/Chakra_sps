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
    [RoutePrefix("PresentationSAPEPUserToGroups")]

    public class PresentationSAPEPUserToGroupsReportController
    {
        public  PresentationSAPEPUserToGroups DtoMap(DomainModel.Presentation.PresentationSAPEPUserToGroups tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPEPUserToGroups, PresentationSAPEPUserToGroups>(tModel);
        }

        public  List<PresentationSAPEPUserToGroups> DtoMapList(IList<DomainModel.Presentation.PresentationSAPEPUserToGroups> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetSAPEPUserToGroupsReportData")]
        public IList<PresentationSAPEPUserToGroups> GetSAPEPUserToGroupsReportData(long endpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPEPUserToGroups.GetSAPEPUserToGroupsReportData(endpointSystemId));
        }
    }
}
