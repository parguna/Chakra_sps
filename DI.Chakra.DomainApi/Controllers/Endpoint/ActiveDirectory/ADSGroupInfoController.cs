using System.Collections.Generic;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ADSGroupInfo")]
    public class ADSGroupInfoController : BaseController<DomainModel.ADSGroupInfo, EntityModel.ADSGroupInfo, Dto.ADSGroupInfo>
    {
        [HttpGet]
        [Route("GetByName")]
        public ADSGroupInfo GetByName(string groupName)
        {
            return DtoMap(DomainModel.ADSGroupInfo.GetByName(groupName));
        }

        [HttpGet]
        [Route("GetAllByGroupTypeAndEndpointId")]
        public List<ADSGroupInfo> GetAllByGroupTypeAndEndpointId(string paramgroupType, long endpointSystemId)
        {
            return DtoMapList(DomainModel.ADSGroupInfo.GetAllByGroupTypeAndEndpointId(paramgroupType, endpointSystemId));
        }

        [HttpGet]
        [Route("GetByCn")]
        public ADSGroupInfo GetByCn(string cn)
        {
            return DtoMap(DomainModel.ADSGroupInfo.GetByCn(cn));
        }

        [HttpGet]
        [Route("GetADGroupsByUserId")]
        public  List<ADSGroupInfo> GetADGroupsByUserId(long userId)
        {
            return DtoMapList(DomainModel.ADSGroupInfo.GetADGroupsByUserId(userId));
        }

        [HttpGet]
        [Route("GetADGroupsByUserIdAndEndpointId")]
        public List<ADSGroupInfo> GetADGroupsByUserIdAndEndpointId(long userId, long EndpointId)
        {
            return DtoMapList(DomainModel.ADSGroupInfo.GetADGroupsByUserIdAndEndpointId(userId, EndpointId));
        }

        [HttpGet]
        [Route("GetAllByName")]
        public Dictionary<long, string> GetAllByName(string searchString)
        {
                return DomainModel.ADSGroupInfo.GetAllByName(searchString);
        }

        [HttpGet]
        [Route("GetADSGroupBySearchString")]
        public Dictionary<long, string> GetADSGroupBySearchString(string searchString, long EndpointId, long accessRequestId=0,bool isBulkRequest=false)
        {
            return DomainModel.ADSGroupInfo.GetADSGroupBySearchString(searchString,EndpointId, accessRequestId, isBulkRequest);
        }
        [HttpGet]
        [Route("CheckADGroupExistsInOtherSystemMatrix")]
        public string CheckADGroupExistsInOtherSystemMatrix(string selectedADGroup)
        {
            return DomainModel.ADSGroupInfo.CheckADGroupExistsInOtherSystemMatrix(selectedADGroup);
        }
    }
}
