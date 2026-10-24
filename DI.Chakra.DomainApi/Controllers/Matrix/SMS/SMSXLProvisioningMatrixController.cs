using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SMSXLProvisioningMatrix")]
    public class SMSXLProvisioningMatrixController : BaseController<DomainModel.SMSXLProvisioningMatrix, EntityModel.SMSXLProvisioningMatrix, Dto.SMSXLProvisioningMatrix>
    {
        [HttpGet]
        [Route("GetAllSMSProvisioningMatrix")]
        public List<Dto.SMSXLProvisioningMatrix> GetAllSMSProvisioningMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SMSXLProvisioningMatrix.GetAllSMSProvisioningMatrix(isValidRecords, isAllRecords));
        }
    }
}