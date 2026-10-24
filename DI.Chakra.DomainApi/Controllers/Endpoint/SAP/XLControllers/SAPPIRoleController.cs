using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPPIRole")]
    public class SAPPIRoleController: BaseController<DomainModel.SAPPIRole, EntityModel.SAPPIRole, Dto.SAPPIRole>
    {
        [HttpGet]
        [Route("GetAllSAPPIRoleMatrix")]
        public List<Dto.SAPPIRole> GetAllSAPPIRoleMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPPIRole.GetAllSAPPIRoleMatrix(isValidRecords, isAllRecords));
        }


        [HttpGet]
        [Route("BindSubModuleFromPIRole")]
       
        public List<string> BindSubModuleFromPIRole(string Module)
        { 
                return DomainModel.SAPPIRole.BindSubModuleFromPIRole(Module); 
        }

        [HttpGet]
        [Route("GetBusinessRoleFromPIRole")]

        public Dictionary<long, string> GetBusinessRoleFromPIRole(string searchString, string subModule, string Module)
        {
            return DomainModel.SAPPIRole.GetBusinessRoleFromPIRole(searchString, subModule, Module); 
        }

        [HttpGet]
        [Route("BindModuleFromPIRole")] 
        public List<string> BindModuleFromPIRole()
        {
            return DomainModel.SAPPIRole.BindModuleFromPIRole(); 
        }

        [HttpGet]
        [Route("BindTechnicalFromSAPPIRole")]
        public List<string> BindTechnicalFromSAPPIRole(string BusinessRole)
        {
            return DomainModel.SAPPIRole.BindTechnicalFromSAPPIRole(BusinessRole); 
        }
    }
}