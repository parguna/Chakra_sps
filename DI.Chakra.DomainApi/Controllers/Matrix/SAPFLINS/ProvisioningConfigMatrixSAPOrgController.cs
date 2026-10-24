using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ProvisioningConfigMatrixSAPOrg")]
    public class ProvisioningConfigMatrixSAPOrgController: BaseController<DomainModel.ProvisioningConfigMatrixSAPOrg, EntityModel.ProvisioningConfigMatrixSAPOrg, Dto.ProvisioningConfigMatrixSAPOrg>
    {
        [HttpGet]
        [Route("GetProvisioningConfigMatrixSAPOrg")]
        public List<Dto.ProvisioningConfigMatrixSAPOrg> GetProvisioningConfigMatrixSAPOrg(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.ProvisioningConfigMatrixSAPOrg.GetProvisioningConfigMatrixSAPOrgs(isValidRecords, isAllRecords));
        }
    }
}