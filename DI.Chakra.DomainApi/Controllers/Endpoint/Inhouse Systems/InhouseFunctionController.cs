using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("InhouseFunction")]
    public class InhouseFunctionController: BaseController<DomainModel.InhouseFunction, EntityModel.InhouseFunction, Dto.InhouseFunction>
    {
        [HttpGet]
        [Route("GetInhouseFunctionGridData")]
        public List<Dto.InhouseFunction> GetInhouseFunctionGridData(string InhouseSystemCode)
        {
            var res = DomainModel.InhouseFunction.GetInhouseFunctionGridData(InhouseSystemCode);
            return DtoMapList(res);
        }

        [HttpGet]
        [Route("GetByEndPointSystemId")]
        public Dictionary<long, string> GetByEndPointSystemId(long endpointId, string permission, string searchString)
        {
            return DomainModel.InhouseFunction.GetByEndPointSystemId(endpointId, permission, searchString);
        }

        [HttpGet]
        [Route("GetInhouseFunctionActive")]
        public List<Dto.InhouseFunction> GetInhouseFunctionActive(long endpointId, string permission, int type, long endpointUserId = 0)
        {
            return DtoMapList(DomainModel.InhouseFunction.GetInhouseFunctionActive(endpointId, permission, type, endpointUserId));
        }
    }
}