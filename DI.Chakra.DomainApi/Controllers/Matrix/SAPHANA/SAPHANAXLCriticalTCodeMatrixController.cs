using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPHANAXLCriticalTCodeMatrix")]
    public class SAPHANAXLCriticalTCodeMatrixController : BaseController<DomainModel.SAPHANAXLCriticalTCodeMatrix, EntityModel.SAPHANAXLCriticalTCodeMatrix, Dto.SAPHANAXLCriticalTCodeMatrix>
    {
        [HttpGet]
        [Route("GetAllSAPHANAXLCriticalTCodeMatrix")]
        public List<Dto.SAPHANAXLCriticalTCodeMatrix> GetAllSAPHANAXLCriticalTCodeMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPHANAXLCriticalTCodeMatrix.GetAllSAPHANAXLCriticalTCodesMatrix(isValidRecords, isAllRecords));
        }

        [HttpPost]
        [Route("IsRecordExists")]
        public bool IsRecordExists(Dto.SAPHANAXLCriticalTCodeMatrix tCode)
        {
            return DomainModel.SAPHANAXLCriticalTCodeMatrix.IsRecordExists(tCode.Transaction, tCode.Id);
        }
    }
}