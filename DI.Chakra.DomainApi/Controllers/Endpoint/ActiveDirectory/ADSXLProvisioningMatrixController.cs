using System.Collections.Generic;
using System.Data;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ADSXLProvisioningMatrix")]
    public class ADSXLProvisioningMatrixController : BaseController<DomainModel.ADSXLProvisioningMatrix, EntityModel.ADSXLProvisioningMatrix, Dto.ADSXLProvisioningMatrix>
    {
        [HttpGet]
        [Route("GetADProvisioningMatrix")]
        public List<Dto.ADSXLProvisioningMatrix> GetADProvisioningMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.ADSXLProvisioningMatrix.GetAllADSXLProvisioningMatrix(isValidRecords, isAllRecords));
        }

        [HttpGet]
        [Route("ExecuteQuery")]
        public Dto.ADSXLProvisioningMatrix ExecuteQuery(string query)
        {
            DomainModel.ADSXLProvisioningMatrix.ExecuteQuery(query);
            return null;
        }

        [HttpGet]
        [Route("GetDataTableExecuteQuery")]
        public DataTable GetDataTableExecuteQuery(string query)
        {
            try
            {
                return DomainModel.ADSXLProvisioningMatrix.GetDataTableByQuery(query);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}