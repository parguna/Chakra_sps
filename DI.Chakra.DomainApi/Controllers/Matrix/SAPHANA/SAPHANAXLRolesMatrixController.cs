using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPHANAXLRolesMatrix")]
    public class SAPHANAXLRolesMatrixController : BaseController<DomainModel.SAPHANAXLRolesMatrix, EntityModel.SAPHANAXLRolesMatrix, Dto.SAPHANAXLRolesMatrix>
    {
        [HttpGet]
        [Route("GetAllSAPHANAXLRolesMatrix")]
        public List<Dto.SAPHANAXLRolesMatrix> GetAllSAPHANAXLRolesMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPHANAXLRolesMatrix.GetAllSAPHANAXLRoleMatrix(isValidRecords, isAllRecords));
        }
        [HttpGet]
        [Route("BindRolesSAPHANA")]
        public IList<Dto.SAPHANAXLRolesMatrix> BindRolesSAPHANA(long endpointSystemId)
        {
            return DtoMapList(DomainModel.SAPHANAXLRolesMatrix.BindRolesSAPHANA(endpointSystemId)); 
        }

        [HttpGet]
        [Route("BindViewDetailsSAPHANA")]
        public IList<Dto.SAPHANAXLRolesMatrix> BindViewDetailsSAPHANA(string Role)
        {
            return DtoMapList(DomainModel.SAPHANAXLRolesMatrix.BindViewDetailsEBS(Role));
        }
    }
}