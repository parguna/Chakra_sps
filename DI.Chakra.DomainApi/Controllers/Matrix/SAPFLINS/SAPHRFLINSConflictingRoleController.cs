using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPHRFLINSConflictingRole")]
    public class SAPHRFLINSConflictingRoleController : BaseController<DomainModel.SAPHRFLINSConflictingRole, EntityModel.SAPHRFLINSConflictingRole, Dto.SAPHRFLINSConflictingRole>
    {
        [HttpGet]
        [Route("GetAllSAPHRFLINSConflictRoles")]
        public List<Dto.SAPHRFLINSConflictingRole> GetAllSAPHRFLINSConflictRoles(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPHRFLINSConflictingRole.GetAllSAPHRFLINSConflictRoles(isValidRecords, isAllRecords));
        }
    }
}