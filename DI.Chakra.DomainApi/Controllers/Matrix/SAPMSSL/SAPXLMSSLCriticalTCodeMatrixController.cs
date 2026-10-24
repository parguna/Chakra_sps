using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPXLMSSLCriticalTCodeMatrix")]
    public class SAPXLMSSLCriticalTCodeMatrixController : BaseController<DomainModel.SAPXLMSSLCriticalTCodeMatrix, EntityModel.SAPXLMSSLCriticalTCodeMatrix, Dto.SAPXLMSSLCriticalTCodeMatrix>
    {
        [HttpGet]
        [Route("GetAllSAPMSSLXLCriticalTCodesMatrix")]
        public List<Dto.SAPXLMSSLCriticalTCodeMatrix> GetAllSAPMSSLXLCriticalTCodesMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPXLMSSLCriticalTCodeMatrix.GetAllSAPMSSLXLCriticalTCodesMatrix(isValidRecords, isAllRecords));
        }
        [HttpPost]
        [Route("IsRecordExists")]
        public bool IsRecordExists(Dto.SAPXLMSSLCriticalTCodeMatrix tCode)
        {
            return DomainModel.SAPXLMSSLCriticalTCodeMatrix.IsRecordExists(tCode.Transaction, tCode.Id);
        }
    }
}