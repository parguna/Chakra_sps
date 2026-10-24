using System.Collections.Generic;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPParameterHistoryInfo")]
    public class SAPParameterHistoryInfoController : BaseController<DomainModel.SAPParameterHistoryInfo, EntityModel.SAPParameterHistoryInfo, Dto.SAPParameterHistoryInfo>
    {
        [HttpPost]
        [Route("GetSAPParameters")]
        public IList<SAPParameterHistoryInfo> GetSAPParameters([FromBody] Dto.EndPoint endPoint)
        {
            return DtoMapList(DomainModel.SAPParameterHistoryInfo.GetSAPParameters(AutoMapperHelper.DtoUnMap<Dto.EndPoint, DomainModel.EndPoint>(endPoint)));
        }
    }
}
