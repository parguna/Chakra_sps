using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationInhouseUserInfo")]
    public class PresentationInhouseUserInfoController : ApiController
    {
        [HttpGet]
        [Route("GetByInhousesystemcode")]
        public List<Dto.Presentation.PresentationInhouseUserInfo> GetByInhouseSystemsCode(string inhouseSystemsCode)
        {
            return DtoMapList(DomainModel.Presentation.PresentationInhouseUserInfo.GetByInhousesystemcode(inhouseSystemsCode));
        }

        [HttpGet]
        [Route("GetByEndPointSystemId")]
        public List<Dto.Presentation.PresentationInhouseUserInfo> GetByEndPointSystemId(long endpointSystemid)
        {
            return DtoMapList(DomainModel.Presentation.PresentationInhouseUserInfo.GetByEndpointSystemId(endpointSystemid));
        }

        [HttpGet]
        [Route("GetAccountWithNoPermissionsReportData")]
        public List<Dto.Presentation.PresentationInhouseUserInfo> GetAccountWithNoPermissionsReportData(long endpointSystemid)
        {
            return DtoMapList(DomainModel.Presentation.PresentationInhouseUserInfo.GetByEndpointIdAndAccountWithNoGroups(endpointSystemid));
        }

        public  Dto.Presentation.PresentationInhouseUserInfo DtoMap(DomainModel.Presentation.PresentationInhouseUserInfo tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationInhouseUserInfo, Dto.Presentation.PresentationInhouseUserInfo>(tModel);
        }

        public  List<Dto.Presentation.PresentationInhouseUserInfo> DtoMapList(IList<DomainModel.Presentation.PresentationInhouseUserInfo> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
        [HttpGet]
        [Route("GetInhouseUserInfoBySystemCode")]
        public List<Dto.Presentation.PresentationInhouseUserInfo> GetInhouseUserInfoBySystemCode(string inhousesystemcode)
        {
            return DtoMapList(DomainModel.Presentation.PresentationInhouseUserInfo.GetInhouseUserInfoBySystemCode(inhousesystemcode));
        }
    }
}
