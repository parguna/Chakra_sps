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
    [RoutePrefix("PresentationDiscoveryProcess")]
    public class PresentationDiscoveryProcessController : DiscoveryProcessController
    {
        [HttpGet]
        [Route("GetDiscoveryProcessJobs")]
        public List<PresentationDiscoveryProcess> GetDiscoveryProcessJobs(int startRow, int PageSize, string filter)
        {
            return DtoMapList(DomainModel.Presentation.PresentationDiscoveryProcess.GetDiscoveryProcessJobs(startRow,PageSize,filter));
        }

        [HttpGet]
        [Route("GetDiscoveryProcessJobsByEndPointSystemId")]
        public List<PresentationDiscoveryProcess> GetDiscoveryProcessJobsByEndPointSystemId(int endPointSystemId, int startRow, int PageSize, string filter)
        {
            return DtoMapList(DomainModel.Presentation.PresentationDiscoveryProcess.GetDiscoveryProcessJobsByEndPointSystemId(endPointSystemId,startRow, PageSize, filter));
        }

        public  PresentationDiscoveryProcess DtoMap(DomainModel.Presentation.PresentationDiscoveryProcess tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationDiscoveryProcess, PresentationDiscoveryProcess>(tModel);
        }

        public  List<PresentationDiscoveryProcess> DtoMapList(IList<DomainModel.Presentation.PresentationDiscoveryProcess> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
