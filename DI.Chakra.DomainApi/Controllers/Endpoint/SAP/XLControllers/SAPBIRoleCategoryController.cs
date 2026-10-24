using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPBIRoleCategory")]
    public class SAPBIRoleCategoryController: BaseController<DomainModel.SAPBIRoleCategory, EntityModel.SAPBIRoleCategory, Dto.SAPBIRoleCategory>
    {
        [HttpGet]
        [Route("GetAllSAPBIRoleCategories")]
        public List<Dto.SAPBIRoleCategory> GetAllSAPBIRoleCategories(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPBIRoleCategory.GetAllSAPBIRoleCategories(isValidRecords, isAllRecords));
        }

        [HttpGet]
        [Route("BindSubModuleFromFLINSBI")]
        public List<string> BindSubModuleFromFLINSBI(string moduleName)
        {
            return DomainModel.SAPBIRoleCategory.BindSubModuleFromFLINSBI(moduleName);
        }

        [HttpGet]
        [Route("BindModuleFromFLINSBI")]
        public List<string> BindModuleFromFLINSBI()
        {
            return DomainModel.SAPBIRoleCategory.BindModuleFromFLINSBI();
        }

        [HttpGet]
        [Route("GetBusinessRoleFromFlinsBIRole")]
        public Dictionary<long, string> GetBusinessRoleFromFlinsBIRole(string searchString, string subModule, string Module)
        {
            try
            {
                return DomainModel.SAPBIRoleCategory.GetBusinessRoleFromFlinsBIRole(searchString, subModule, Module);

            }
            catch
            {
                throw;
            }
        }


        [HttpGet]
        [Route("GetBusinessRoles")]
        public Dictionary<long, string> GetBusinessRoles(string searchString, long endpointUserId, long actionType, string subModule, string Module)
        { 
            try
            {
                return DomainModel.SAPBIRoleCategory.GetBusinessRoles(searchString, endpointUserId, (DI.Chakra.Infrastructure.Enums.AdHocActionType)actionType, subModule, Module);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        [Route("BindTechnicalFromFLINSBI")]
        public List<string> BindTechnicalFromFLINSBI(string BusinessRole)
        {
            return DomainModel.SAPBIRoleCategory.BindTechnicalFromFLINSBI(BusinessRole);
        }

    }
}