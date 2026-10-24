using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ProvisioningConfigMatrixSapEEGroup")]
    public class ProvisioningConfigMatrixSapEEGroupController : BaseController<DomainModel.ProvisioningConfigMatrixSapEEGroup, EntityModel.ProvisioningConfigMatrixSapEEGroup, Dto.ProvisioningConfigMatrixSapEEGroup>
    {
        [HttpGet]
        [Route("GetAllSAPRoleBasedOnEEGroups")]
        public List<Dto.ProvisioningConfigMatrixSapEEGroup> GetAllSAPRoleBasedOnEEGroups(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.ProvisioningConfigMatrixSapEEGroup.GetAllSAPRoleBasedOnEEGroups(isValidRecords, isAllRecords));
        }
    }
}