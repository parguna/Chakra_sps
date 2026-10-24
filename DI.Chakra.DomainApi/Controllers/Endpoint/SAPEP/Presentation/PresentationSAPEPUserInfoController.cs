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
    [RoutePrefix("PresentationSAPEPUserInfo")]

    public class PresentationSAPEPUserInfoController
    {
        public  PresentationSAPEPUserInfo DtoMap(DomainModel.Presentation.PresentationSAPEPUserInfo tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationSAPEPUserInfo, PresentationSAPEPUserInfo>(tModel);
        }

        public  List<PresentationSAPEPUserInfo> DtoMapList(IList<DomainModel.Presentation.PresentationSAPEPUserInfo> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetAllSAPEPUsersReportData")]
        public IList<PresentationSAPEPUserInfo> GetAllSAPEPUsersReportData(long endpointSystemId)
        {
            return DtoMapList(DomainModel.Presentation.PresentationSAPEPUserInfo.GetAllSAPEPUsersReportData(endpointSystemId));
        }
    }
}

