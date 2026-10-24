using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPHANAXLPositionsMatrix")]
    public class SAPHANAXLPositionsMatrixController : BaseController<DomainModel.SAPHANAXLPositionsMatrix, EntityModel.SAPHANAXLPositionsMatrix, Dto.SAPHANAXLPositionsMatrix>
    {
        [HttpGet]
        [Route("GetAllSAPHANAXLPositionMatrix")]
        public List<Dto.SAPHANAXLPositionsMatrix> GetAllSAPHANAXLPositionMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPHANAXLPositionsMatrix.GetAllSAPHANAXLPositionMatrix(isValidRecords, isAllRecords));
        }

        [HttpGet]
        [Route("GetPositionSAPHANA")]
        public string GetPositionSAPHANA(long ID)
        {
            return DomainModel.SAPHANAXLPositionsMatrix.GetPositionCode(ID);
        }

        [HttpGet]
        [Route("GetSAPHANARolesByPosition")]
        public List<string> GetSAPHANARolesByPosition(string position, long endpointSystemId)
        {
            return DomainModel.SAPHANAHRP1001.GetSAPRolesByPosition(position, endpointSystemId);
        }
        [HttpGet]
        [Route("GetSAPHANAPositionMatrixByPosition")]
        public SAPHANAXLPositionsMatrix GetSAPHANAPositionMatrixByPosition(long endpointSystemId, string position)
        {
            return DtoMap(DomainModel.SAPHANAXLPositionsMatrix.GetSAPHANAPositionMatrixByPosition(endpointSystemId,position));
        }

        [HttpGet]
        [Route("BindPositionsSAPHANA")]
        public IList<SAPHANAXLPositionsMatrix> BindPositionsSAPHANA(long endpointSystemId, string prefix, EmployeeType employeeType)
        {
            return DtoMapList(DomainModel.SAPHANAXLPositionsMatrix.BindPositionsSAPHANA(endpointSystemId, prefix, employeeType)); 
        }
        [HttpGet]
        [Route("GetPrefixSAPHANA")]
        public string GetPrefixSAPHANA(long ID)
        {
            return DomainModel.SAPHANAXLPositionsMatrix.GetPrefixSAPHANA(ID) ;  
        }
    }
}