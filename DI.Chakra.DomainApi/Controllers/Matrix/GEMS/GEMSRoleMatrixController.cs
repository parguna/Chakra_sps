using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("GEMSRoleMatrix")]
    public class GEMSRoleMatrixController : BaseController<DomainModel.GEMSRoleMatrix, EntityModel.GEMSRoleMatrix, Dto.GEMSRoleMatrix>
    {
        [HttpGet]
        [Route("GetGEMSRoleMatrices")]
        public List<Dto.GEMSRoleMatrix> GetGEMSRoleMatrices(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.GEMSRoleMatrix.GetGEMSRoleMatrices(isValidRecords, isAllRecords));
        }
        [HttpGet]
        [Route("BindRolesGEMS")]
        public List<Dto.GEMSRoleMatrix> BindRolesGEMS()
        {
            return DtoMapList(DomainModel.GEMSRoleMatrix.BindRolesGEMS());
        }

        [HttpGet]
        [Route("GetByBusinessRole")]
        public Dto.GEMSRoleMatrix GetByBusinessRole(string businessRole)
        {
            return DtoMap(DomainModel.GEMSRoleMatrix.GetByBusinessRole(businessRole));
        }


        [HttpGet]
        [Route("GetAllTables")]
        public List<string> GetAllTables(string searchString)
        {
            return DomainModel.GEMSRoleMatrix.GetAllTables(searchString);
        }

        [HttpGet]
        [Route("GetAllColumnsByTableName")]
        public List<string> GetAllColumnsByTableName(string tableName)
        {
            return DomainModel.GEMSRoleMatrix.GetAllColumnsByTableName(tableName);
        }

        [HttpGet]
        [Route("GetBusinessRoles")]
        public Dictionary<long, string> GetBusinessRoles(long endpointId, int _applicationType, int _actionType, string searchstring)
        {
            return DomainModel.GEMSRoleMatrix.GetBusinessRoles(endpointId, (ApplicationType)_applicationType, (AdHocActionType)_actionType, searchstring);
        }
    }
}