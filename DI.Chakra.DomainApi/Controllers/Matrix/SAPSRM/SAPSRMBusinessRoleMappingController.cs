using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPSRMBusinessRoleMapping")]
    public class SAPSRMBusinessRoleMappingController : BaseController<DomainModel.SAPSRMBusinessRoleMapping, EntityModel.SAPSRMBusinessRoleMapping, Dto.SAPSRMBusinessRoleMapping>
    {
        [HttpGet]
        [Route("GetAllSAPSRMBusinessRoleMatrix")]
        public List<Dto.SAPSRMBusinessRoleMapping> GetAllSAPSRMBusinessRoleMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPSRMBusinessRoleMapping.GetAllSAPSRMBusinessRoleMatrix(isValidRecords, isAllRecords));
        }

        [HttpGet]
        [Route("GetSAPSRMPgrp")]
        public   Dictionary<string, string> GetSAPSRMPgrp(string BuisnessRole, AdHocActionType adhocActionType, AccountType accType, long userId, string POrg,string userName)
        {
            return DomainModel.SAPSRMBusinessRoleMapping.GetSAPSRMPgrp(BuisnessRole, adhocActionType, accType, userId, POrg,userName);
        }
        [HttpGet]
        [Route("GetSAPSRMEndpointUsers")]
        public List<string> GetSAPSRMEndpointUsers(long requesteeuserId, AccountType acctype)
        {

            return DomainModel.SAPSRMBusinessRoleMapping.GetSAPSRMEndpointUsers(requesteeuserId, acctype);
        }
        [HttpGet]
        [Route("GetSAPSRMPgrpEndpointUsers")]
        public List<string> GetSAPSRMPgrpEndpointUsers(long requesteeuserId, AccountType acctype)
        {
            return DomainModel.SAPSRMBusinessRoleMapping.GetSAPSRMPgrp(requesteeuserId, acctype);
        }
        [HttpGet]
        [Route("GetSAPSRMPorg")]
        public  Dictionary<string, string> GetSAPSRMPorg(string BuisnessRole, AdHocActionType adhocActionType, AccountType accType, int Userid, string userName)
        { 
            return DomainModel.SAPSRMBusinessRoleMapping.GetSAPSRMPorg(BuisnessRole, adhocActionType, accType, Userid, userName); 
        }


        [HttpGet]
        [Route("GetSAPSRMPgrps")]
        public Dictionary<string, string> GetSAPSRMPgrps(string BuisnessRole,  string POrg)
        {
            return DomainModel.SAPSRMBusinessRoleMapping.GetSAPSRMPgrps(BuisnessRole, POrg);
        }

        [HttpGet]
        [Route("GetSAPSRMPorgs")]
        public Dictionary<string, string> GetSAPSRMPorgs(string role)
        { 
            return DomainModel.SAPSRMBusinessRoleMapping.GetSAPSRMPorgs(role); 
        }
        [HttpGet]
        [Route("GetSAPSRMUserRoles")]
        public List<Dto.PresentationUserBusinessRole> GetSAPSRMUserRoles(AdHocActionType adhocActionType, AccountType accType, long userId,string userName, long endpointId, string businessRole=null)
        {
            return DomainModel.SAPSRMBusinessRoleMapping.GetSAPSRMUserRoles(adhocActionType,accType,userId, endpointId, userName, businessRole);
        }
    }
}