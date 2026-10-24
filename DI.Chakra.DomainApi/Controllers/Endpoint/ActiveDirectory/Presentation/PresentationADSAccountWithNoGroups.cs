
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
    [RoutePrefix("PresentationADSAccountWithNoGroups")]
    
    public class PresentationADSAccountWithNoGroupsController : ApiController
    {
        public PresentationADSAccountWithNoGroups DtoMap(DomainModel.Presentation.PresentationADSAccountWithNoGroups tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationADSAccountWithNoGroups, PresentationADSAccountWithNoGroups>(tModel);
        }

        public  List<PresentationADSAccountWithNoGroups> DtoMapList(IList<DomainModel.Presentation.PresentationADSAccountWithNoGroups> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetADSAccountWithNoGroupsReportData")]
        public IList<PresentationADSAccountWithNoGroups> GetADSAccountWithNoGroupsReportData(long endpointsystemid)
        {
            return DtoMapList(DomainModel.Presentation.PresentationADSAccountWithNoGroups.GetByEndpointIdIncludeAccountWithoutGroups(endpointsystemid));
        }
    }
}
