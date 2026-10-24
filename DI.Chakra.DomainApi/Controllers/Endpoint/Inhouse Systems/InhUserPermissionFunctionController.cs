using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("InhUserPermissionFunction")]
    public class InhUserPermissionFunctionController : BaseController<DomainModel.InhUserPermissionFunction, EntityModel.InhUserPermissionFunction, Dto.InhUserPermissionFunction>
    {
        [HttpGet]
        [Route("GetInhUserPermissionFunctionGridData")]
        public List<Dto.InhUserPermissionFunction> GetInhUserPermissionFunctionGridData(string InhouseSystemCode)
        {
            var res = DomainModel.InhUserPermissionFunction.GetInhUserPermissionFunctionGridData(InhouseSystemCode);
            return DtoMapList(res);
        }
    }
}