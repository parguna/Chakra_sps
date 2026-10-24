using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("NOVAManagementRoleMatrix")]
    public class NOVAManagementRoleMatrixController : BaseController<DomainModel.NOVAManagementRoleMatrix, EntityModel.NOVAManagementRoleMatrix, Dto.NOVAManagementRoleMatrix>
    {
        [HttpGet]
        [Route("GetNOVAManagementRoleMatrices")]
        public List<Dto.NOVAManagementRoleMatrix> GetNOVAManagementRoleMatrices(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.NOVAManagementRoleMatrix.GetNOVAManagementRoleMatrices(isValidRecords, isAllRecords));
        }
        [HttpGet]
        [Route("GetManagementDepartment")]
        public List<string> GetManagementDepartment(long endpointsystemId)
        {
            return DomainModel.NOVAManagementRoleMatrix.GetManagementDepartment(endpointsystemId);
        }
        [HttpGet]
        [Route("GetManagementMatrixByDepartemnt")]
        public List<Dto.NOVAManagementRoleMatrix> GetManagementMatrixByDepartemnt(long endpointsystemId,string departmentCode)
        {
            return DtoMapList(DomainModel.NOVAManagementRoleMatrix.GetManagementMatrixByDepartemnt(endpointsystemId, departmentCode));
        }
    }
}