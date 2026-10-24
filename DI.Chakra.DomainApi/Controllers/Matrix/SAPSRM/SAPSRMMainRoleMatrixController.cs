using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SAPSRMMainRoleMatrix")]
    public class SAPSRMMainRoleMatrixController : BaseController<DomainModel.SAPSRMMainRoleMatrix, EntityModel.SAPSRMMainRoleMatrix, Dto.SAPSRMMainRoleMatrix>
    {
        [HttpGet]
        [Route("GetAllSAPSRMEMainRoleMatrix")]
        public List<Dto.SAPSRMMainRoleMatrix> GetAllSAPSRMEMainRoleMatrix(bool isValidRecords, bool isAllRecords)
        {
            return DtoMapList(DomainModel.SAPSRMMainRoleMatrix.GetAllSAPSRMEMainRoleMatrix(isValidRecords, isAllRecords));
        }


        [HttpGet]
        [Route("IsAdGroupsExist")]
        public bool IsAdGroupsExist(string RoleName,AdHocActionType actionType,AccountType accountType,long DIUserID, string AccountName)
        {
            return DomainModel.SAPSRMMainRoleMatrix.IsAdGroupsExist(RoleName, actionType, accountType, DIUserID, AccountName);
        }


        [HttpGet]
        [Route("GetSAPSRMRoles")]
        public Dictionary<string, string> GetSAPSRMRoles(AdHocActionType adhocActiontype, long userId, string searchString, AccountType accountType, 
            string srmUserName,long endpointId)
        {
            return DomainModel.SAPSRMMainRoleMatrix.GetSAPSRMRoles(adhocActiontype, userId, searchString, accountType, srmUserName, endpointId);
        }

        [HttpGet]
        [Route("GetAllSAPSRMRoles")]
        public Dictionary<string, string> GetAllSAPSRMRoles(string searchString)
        {
            return DomainModel.SAPSRMMainRoleMatrix.GetAllSAPSRMRoles(searchString);
        }


    }
}