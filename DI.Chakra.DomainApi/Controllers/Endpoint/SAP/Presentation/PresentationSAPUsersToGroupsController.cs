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
    [RoutePrefix("PresentationSAPUsersToGroups")]

    public class PresentationSAPUserToGroupsController : ApiController
    {
        public  PresentationSAPUsersToGroups DtoMap(DomainModel.Presentation.PresentationSAPUsersToGroups tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPUsersToGroups, PresentationSAPUsersToGroups>(tModel);
        }

        public  List<PresentationSAPUsersToGroups> DtoMapList(IList<DomainModel.Presentation.PresentationSAPUsersToGroups> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetSAPUserToGroupsReportData")]
        public IList<PresentationSAPUsersToGroups> GetSAPUserToGroupsReportData()
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPUsersToGroups.GetSAPUserToGroupsReportData());
        }
    }
}
