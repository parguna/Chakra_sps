using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
using AutoMapper;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODPolicyConfiguration")]
    public class SODPolicyConfigurationController : BaseController<DomainModel.SODPolicyConfiguration, EntityModel.SODPolicyConfiguration, Dto.SODPolicyConfiguration>
    {

        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(long risklevel)
        {
            return DomainModel.SODPolicyConfiguration.DoesExists(risklevel);
        }


        [HttpPost]
        [Route("isExists")]
        public bool isExists([FromBody]SODPolicyConfiguration sodpolicyconfig)
        {
            return DomainModel.SODPolicyConfiguration.isExists(AutoMapperHelper.DtoUnMap<Dto.SODPolicyConfiguration, DomainModel.SODPolicyConfiguration>(sodpolicyconfig));
        }
    }
}