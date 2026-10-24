using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPEBSXLPositionsMatrix")]
    public class SAPEBSXLPositionsMatrixController : BaseController<DomainModel.SAPEBSXLPositionsMatrix, EntityModel.SAPEBSXLPositionsMatrix, Dto.SAPEBSXLPositionsMatrix>
    {
        [HttpGet]
        [Route("GetAllSAPEBSXLPositionMatrix")]
        public List<Dto.SAPEBSXLPositionsMatrix> GetAllSAPEBSXLPositionMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPEBSXLPositionsMatrix.GetAllSAPEBSXLPositionMatrix(isValidRecords, isAllRecords));
        }

        [HttpGet]
        [Route("GetPositionEBS")]
        public string GetPositionEBS(long ID)
        {
            return DomainModel.SAPEBSXLPositionsMatrix.GetPositionCode(ID);
        }

        [HttpGet]
        [Route("GetEBSRolesByPosition")]
        public List<string> GetEBSRolesByPosition(string position, long endpointSystemId)
        {
            return DomainModel.SAPHRP1001.GetSAPRolesByPosition(position, endpointSystemId);
        }
        [HttpGet]
        [Route("GetEBSPositionMatrixByPosition")]
        public SAPEBSXLPositionsMatrix GetEBSPositionMatrixByPosition(string position)
        {
            return DtoMap(DomainModel.SAPEBSXLPositionsMatrix.GetEBSPositionMatrixByPosition(position));
        }

        [HttpGet]
        [Route("BindPositionsEBS")]
        public IList<SAPEBSXLPositionsMatrix> BindPositionsEBS(ApplicationType Entitlement, string prefix, EmployeeType employeeType)
        {
            return DtoMapList(DomainModel.SAPEBSXLPositionsMatrix.BindPositionsEBS(Entitlement, prefix, employeeType)); 
        }
        [HttpGet]
        [Route("GetPrefixEBS")]
        public string GetPrefixEBS(long ID)
        {
            return DomainModel.SAPEBSXLPositionsMatrix.GetPrefixEBS(ID) ;  
        }
    }
}