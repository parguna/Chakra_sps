using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPGRCRole")]
    public class SAPGRCRoleController: BaseController<DomainModel.SAPGRCRole, EntityModel.SAPGRCRole, Dto.SAPGRCRole>
    {
        [HttpGet]
        [Route("GetAllSAPGRCRoleMatrix")]
        public List<Dto.SAPGRCRole> GetAllSAPGRCRoleMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPGRCRole.GetAllSAPGRCRoleMatrix(isValidRecords, isAllRecords));
        }

        [HttpGet]
        [Route("BindSubModuleFromGRCRole")]

        public List<string> BindSubModuleFromPIRole(string Module)
        {
            return DomainModel.SAPGRCRole.BindSubModuleFromGRCRole(Module);
        }

        [HttpGet]
        [Route("GetBusinessRoleFromGRCRole")]

        public Dictionary<long, string> GetBusinessRoleFromGRCRole(string searchString, string subModule, string Module)
        {
            return DomainModel.SAPGRCRole.GetBusinessRoleFromGRCRole(searchString, subModule, Module);
        }

        [HttpGet]
        [Route("BindModuleFromGRCRole")]
        public List<string> BindModuleFromPIRole()
        {
            return DomainModel.SAPGRCRole.BindModuleFromGRCRole();
        }
        
        [HttpGet]
        [Route("BindTechnicalFromSAPGRCRole")]
        public List<string> BindTechnicalFromSAPGRCRole(string BusinessRole)
        {
            return DomainModel.SAPGRCRole.BindTechnicalFromSAPGRCRole(BusinessRole);
        }

        [HttpGet]
        [Route("GetBusinessRoles")]
        public Dictionary<long, string> GetBusinessRoles(long endpointId, int _applicationType, int _actionType, string searchString, string subModule, string Module)
        {
            return DomainModel.SAPGRCRole.GetBusinessRoles(endpointId, (ApplicationType)_applicationType, (AdHocActionType)_actionType, searchString, subModule, Module);
        }

    }
}