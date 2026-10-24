using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPXLMSSLRolesMatrix")]
    public class SAPXLMSSLRolesMatrixController: BaseController<DomainModel.SAPXLMSSLRolesMatrix, EntityModel.SAPXLMSSLRolesMatrix, Dto.SAPXLMSSLRolesMatrix>
    {
        [HttpGet]
        [Route("GetAllSAPMSSLXLRoleMatrix")]
        public List<Dto.SAPXLMSSLRolesMatrix> GetAllSAPMSSLXLRoleMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPXLMSSLRolesMatrix.GetAllSAPMSSLXLRoleMatrix(isValidRecords, isAllRecords));
        }

        [HttpGet]
        [Route("BindRoles")]
        public IList<SAPXLMSSLRolesMatrix> BindRoles(ApplicationType ApplicationName)
        {
            return DtoMapList(DomainModel.SAPXLMSSLRolesMatrix.BindRoles(ApplicationName));
        }
        [HttpGet]
        [Route("BindViewDetails")]
        public IList<SAPXLMSSLRolesMatrix> BindViewDetails(string Role)
        {
            return DtoMapList(DomainModel.SAPXLMSSLRolesMatrix.BindViewDetails(Role));
        }
    }
}