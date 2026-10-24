using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPEBSXLCriticalTCodesMatrix")]
    public class SAPEBSXLCriticalTCodesMatrixController : BaseController<DomainModel.SAPEBSXLCriticalTCodesMatrix, EntityModel.SAPEBSXLCriticalTCodesMatrix, Dto.SAPEBSXLCriticalTCodesMatrix>
    {
        [HttpGet]
        [Route("GetAllSAPEBSXLCriticalTCodesMatrix")]
        public List<Dto.SAPEBSXLCriticalTCodesMatrix> GetAllSAPEBSXLCriticalTCodesMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPEBSXLCriticalTCodesMatrix.GetAllSAPEBSXLCriticalTCodesMatrix(isValidRecords, isAllRecords));
        }

        [HttpPost]
        [Route("IsRecordExists")]
        public bool IsRecordExists(Dto.SAPEBSXLCriticalTCodesMatrix tCode)
        {
            return DomainModel.SAPEBSXLCriticalTCodesMatrix.IsRecordExists(tCode.Transaction, tCode.Id);
        }
    }
}