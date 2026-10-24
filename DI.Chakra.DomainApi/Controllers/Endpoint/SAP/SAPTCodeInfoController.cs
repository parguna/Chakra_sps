using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPTCodeInfo")]
    public class SAPTCodeInfoController : BaseController<DomainModel.SAPTCodeInfo, EntityModel.SAPTCodeInfo, Dto.SAPTCodeInfo>
    {
        [HttpGet]
        [Route("GetByRoleCode")]
        public List<Dto.SAPTCodeInfo> GetByRoleCode(string roleCode, long endpointSystemId)
        {
            return DtoMapList(DomainModel.SAPTCodeInfo.GetByRoleCode(roleCode, endpointSystemId));
        }
        [HttpGet]
        [Route("GetByTCode")]
        public  SAPTCodeInfo GetByTCode(string tCode)
        {
            return DtoMap(DomainModel.SAPTCodeInfo.GetByTCode(tCode));
        }

        [HttpGet]
        [Route("GetSapRolesToTCodeData")]
        public  List<Dto.SAPTCodeInfo> GetSapRolesToTCodeData(string roleCode, long endpointSystemId)
        {
            return DtoMapList(DomainModel.SAPTCodeInfo.GetSapRolesToTCodeData(roleCode, endpointSystemId));
        }

        [HttpGet]
        [Route("GetByRoleCodeIncludingRoles")]
        public  List<Dto.SAPTCodeInfo> GetByRoleCodeIncludingRoles(string roleCode, long endpointSystemId)
        {
            return DtoMapList(DomainModel.SAPTCodeInfo.GetByRoleCodeIncludingRoles(roleCode, endpointSystemId));
        }        

        [HttpGet]
        [Route("GetByEndpointId")]

        public List<Dto.SAPTCodeInfo> GetByEndpointId(long endpointId)
        {
            return DtoMapList(DomainModel.SAPTCodeInfo.GetByEndpointId(endpointId));
        }

        [HttpGet]
        [Route("GetSAPTCodeBySearchString")]
        public Dictionary<long, string> GetSAPTCodeBySearchString(long endpointId, string searchString)
        {
            return DomainModel.SAPTCodeInfo.GetSAPTCodeBySearchString(endpointId, searchString);
        }

        [HttpGet]
        [Route("GenerateSAPTCodeReportByEndpoint")]
        public Dictionary<long, string> GenerateSAPTCodeReportByEndpoint(long endpointId)
        {
            return DomainModel.SAPTCodeInfo.GetSapTCodes(endpointId);
        }
    }
}
