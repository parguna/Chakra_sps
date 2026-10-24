using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPEBSXLRoleMatrix")]
    public class SAPEBSXLRoleMatrixController : BaseController<DomainModel.SAPEBSXLRoleMatrix, EntityModel.SAPEBSXLRoleMatrix, Dto.SAPEBSXLRoleMatrix>
    {
        [HttpGet]
        [Route("GetAllSAPEBSXLRoleMatrix")]
        public List<Dto.SAPEBSXLRoleMatrix> GetAllSAPEBSXLRoleMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPEBSXLRoleMatrix.GetAllSAPEBSXLRoleMatrix(isValidRecords, isAllRecords));
        }
        [HttpGet]
        [Route("BindRolesEBS")]
        public IList<Dto.SAPEBSXLRoleMatrix> BindRolesEBS(ApplicationType ApplicationName)
        {
            return DtoMapList(DomainModel.SAPEBSXLRoleMatrix.BindRolesEBS(ApplicationName)); 
        }

        [HttpGet]
        [Route("BindViewDetailsEBS")]
        public IList<Dto.SAPEBSXLRoleMatrix> BindViewDetailsEBS(string Role)
        {
            return DtoMapList(DomainModel.SAPEBSXLRoleMatrix.BindViewDetailsEBS(Role));
        }
    }
}