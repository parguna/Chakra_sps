using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ADSOuInfo")]
    public class ADSOuInfoController : BaseController<DomainModel.ADSOuInfo, EntityModel.ADSOuInfo, Dto.ADSGroupInfo>
    {
        [HttpGet]
        [Route("GetBySearchString")]
        public Dictionary<long, string> GetBySearchString(string searchString, long endpointsystemid)
        {
            return DomainModel.ADSOuInfo.GetBySearchString(searchString, endpointsystemid);
        }

        [HttpGet]
        [Route("GetDefaultOU")]
        public Dto.ADSXLProvisioningMatrix GetDefaultOU(long DiUserId)
        {
            var user = DomainModel.User.GetById(DiUserId);

            return AutoMapper.Mapper.Map<Dto.ADSXLProvisioningMatrix>(DomainModel.ADSXLProvisioningMatrix.GetByUser(user));
        }

        
    }
}
