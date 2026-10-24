using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ProvisioningConfigMatrixSAPPosition")]
    public class ProvisioningConfigMatrixSAPPositionController: BaseController<DomainModel.ProvisioningConfigMatrixSAPPosition, EntityModel.ProvisioningConfigMatrixSAPPosition, Dto.ProvisioningConfigMatrixSAPPosition>
    {
        [HttpGet]
        [Route("GetAllSAPRoleBasedOnPosition")]
        public List<Dto.ProvisioningConfigMatrixSAPPosition> GetAllSAPRoleBasedOnPosition(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.ProvisioningConfigMatrixSAPPosition.GetAllSAPRoleBasedOnPosition(isValidRecords, isAllRecords));
        }
    }
}