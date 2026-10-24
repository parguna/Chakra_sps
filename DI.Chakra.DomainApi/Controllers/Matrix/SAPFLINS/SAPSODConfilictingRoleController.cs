using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPSODConflictingRole")]
    public class SAPSODConfilictingRoleController : BaseController<DomainModel.SAPSODConflictingRole, EntityModel.SAPSODConflictingRole, Dto.SAPSODConflictingRole>
    {
        [HttpGet]
        [Route("GetAllSAPSODConflictRoles")]
        public List<Dto.SAPSODConflictingRole> GetAllSAPSODConflictRoles(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPSODConflictingRole.GetAllSAPSODConflictRoles(isValidRecords, isAllRecords));
        }
        [HttpGet]
        [Route("GetAllSAPSODendpointtechConflictRoles")]
        public List<Dto.SAPSODConflictingRole> GetAllSAPSODendpointtechConflictRoles(string EndpointSystemName, string TechnicalRole, string ConflictingTechnicalRole,long id)
        {
            return DtoMapList(DomainModel.SAPSODConflictingRole.GetAllSAPSODendpointtechConflictRoles(EndpointSystemName, TechnicalRole, ConflictingTechnicalRole, id));
        }
    }
}