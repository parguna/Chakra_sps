using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPXLMSSLPositionsMatrix")]
    public class SAPXLMSSLPositionsMatrixController : BaseController<DomainModel.SAPXLMSSLPositionsMatrix, EntityModel.SAPXLMSSLPositionsMatrix, Dto.SAPXLMSSLPositionsMatrix>
    {
        [HttpGet]
        [Route("GetAllSAPMSSLXLPositionMatrix")]
        public List<Dto.SAPXLMSSLPositionsMatrix> GetAllSAPMSSLXLPositionMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPXLMSSLPositionsMatrix.GetAllSAPMSSLXLPositionMatrix(isValidRecords, isAllRecords));
        }

        [HttpGet]
        [Route("GetMSSLPositionMatrixByPosition")]
        public Dto.SAPXLMSSLPositionsMatrix GetMSSLPositionMatrixByPosition(string position)
        {
            return DtoMap(DomainModel.SAPXLMSSLPositionsMatrix.GetMSSLPositionMatrixByPosition(position));
        }

        [HttpGet]
        [Route("GetPrefix")]
        public string GetPrefix(long Id)
        {
            return DomainModel.SAPXLMSSLPositionsMatrix.GetPrefix(Id);
        }
        [HttpGet]
        [Route("GetPositionCode")]
        public string GetPositionCode(long ID)
        {
            return DomainModel.SAPXLMSSLPositionsMatrix.GetPositionCode(ID);
        }
        [HttpGet]
        [Route("GetRolesByPosition")]
        public List<string> GetRolesByPosition(string position, long endpointSystemId)
        { 
           return DomainModel.SAPHRP1001.GetSAPRolesByPosition(position, endpointSystemId); 
        }
        [HttpGet]
        [Route("BindPositions")]
        public IList<Dto.SAPXLMSSLPositionsMatrix> BindPositions(ApplicationType Entitlement, string prefix, EmployeeType employeeType)
        {
            return DtoMapList(DomainModel.SAPXLMSSLPositionsMatrix.BindPositions(Entitlement, prefix, employeeType));
        }

    }
}