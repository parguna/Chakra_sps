using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
   
    [RoutePrefix("SAPProfileInfo")]
    public class SAPProfileInfoController : BaseController<DomainModel.SAPProfileInfo, EntityModel.SAPProfileInfo, Dto.SAPProfileInfo>
    {
        [HttpGet]
        [Route("GetByProfileCode")]
        public SAPProfileInfo GetByProfileCode(string profileCode, long endPointSystemId)
        {
            return DtoMap(DomainModel.SAPProfileInfo.GetByProfileCode(profileCode, endPointSystemId));
        }

        [HttpGet]
        [Route("GetByEndpointSystemId")]
        public List<SAPProfileInfo> GetByEndpointSystemId(long endpointId)
        {
            return DtoMapList(DomainModel.SAPProfileInfo.GetByEndpointSystemId(endpointId));
        }
        [HttpGet]
        [Route("GetSAPProfileBySearchString")]
        public Dictionary<long, string> GetSAPProfileBySearchString(long endpointId, string searchString)
        {
            return DomainModel.SAPProfileInfo.GetSAPProfileBySearchString(endpointId, searchString);
        }
    }
}
