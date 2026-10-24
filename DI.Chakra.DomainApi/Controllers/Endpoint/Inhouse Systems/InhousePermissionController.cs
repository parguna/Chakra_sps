using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("InhousePermission")]
    public class InhousePermissionController : BaseController<DomainModel.InhousePermission, EntityModel.InhousePermission, Dto.InhousePermission>
    {
        [HttpGet]
        [Route("GetByIdIncludeEndpoint")]
        public InhousePermission GetByIdIncludeEndpoint(long id)
        {
            return DtoMap(DomainModel.InhousePermission.GetByIdIncludeEndpoint(id));
        }

        [HttpGet]
        [Route("GetByIdIncludeEndpointFunction")]
        public InhousePermission GetByIdIncludeEndpointFunction(long id)
        {
            return DtoMap(DomainModel.InhousePermission.GetByIdIncludeEndpointFunction(id));
        }

        [HttpGet]
        [Route("GetPermissionAndFunctionWithDescription")]
        public List<InhouserEntitlementCustom> GetPermissionAndFunctionWithDescription(long endpointsystemId, string entitlement)
        {
            return DomainModel.InhousePermission.GetPermissionAndFunctionWithDescription(endpointsystemId, entitlement);
        }

        [HttpGet]
        [Route("GetByUserId")]
        public List<InhousePermission> GetByUserId(long userId)
        {
            return DtoMapList(DomainModel.InhousePermission.GetByUserId(userId));
        }

        [HttpGet]
        [Route("GetByEndPointSystemId")]
        public Dictionary<long, string> GetByEndPointSystemId(long endpointId, string searchString)
        {
            return DomainModel.InhousePermission.GetByEndPointSystemId(endpointId, searchString);
        }

        [HttpGet]
        [Route("GetByEndPointSystemIdExisting")]
        public Dictionary<long, string> GetByEndPointSystemIdExisting(long endPointSystemId, string searchString, long endpointUserId)
        {
            return DomainModel.InhousePermission.GetByEndPointSystemIdExisting(endPointSystemId, searchString, endpointUserId);
        }

        [HttpGet]
        [Route("GetAllPermissionsExceptExisting")]
        public Dictionary<long, string> GetAllPermissionsExceptExisting(long endPointSystemId, string searchString, long endpointUserId)
        {
            return DomainModel.InhousePermission.GetAllPermissionsExceptExisting(endPointSystemId, searchString, endpointUserId);
        }

        [HttpGet]
        [Route("GetByEndPointSystemId")]
        public List<InhousePermission> GetByEndPointSystemId(long endPointSystemId)
        {
            return DtoMapList(DomainModel.InhousePermission.GetByEndPointSystemId(endPointSystemId));
        }

        [HttpGet]
        [Route("DoesExistsByEndPointSystemId")]
        public bool DoesExistsByEndPointSystemId(string value, long endPointSystemId)
        {
            return DomainModel.InhousePermission.DoesExistsByEndPointSystemId(value, endPointSystemId);
        }

        [HttpGet]
        [Route("GetPermissionWithNoAccountsReportData")]
        public List<InhousePermission> GetPermissionWithNoAccountsReportData(long endPointSystemId)
        {
            return DtoMapList(DomainModel.InhousePermission.GetPermissionWithNoAccountsReportData(endPointSystemId));
        }

        [HttpGet]
        [Route("GetPermissionsByEndpointSystemId")]
        public List<InhousePermission> GetPermissionsByEndpointSystemId(string permissions, long endPointSystemId)
        {
            return DtoMapList(DomainModel.InhousePermission.GetPermissionsByEndpointSystemId(permissions, endPointSystemId));
        }

        [HttpGet]
        [Route("GetByNameAndEndpointSystemId")]
        public InhousePermission GetByNameAndEndpointSystemId(string permission, long endPointSystemId)
        {
            return DtoMap(DomainModel.InhousePermission.GetByNameAndEndpointSystemId(permission, endPointSystemId));
        }

        [HttpGet]
        [Route("DeleteByPermissionId")]
        public string DeleteByPermissionId(long permissionId, string currentUser)
        {
            return DomainModel.InhousePermission.DeletePermissionById(permissionId, currentUser);
        }

        [HttpGet]
        [Route("GetInhousePermissionGridData")]
        public List<Dto.InhousePermission> GetInhousePermissionGridData(string InhouseSystemCode)
        {
            var res = DomainModel.InhousePermission.GetInhousePermissionGridData(InhouseSystemCode);
            return DtoMapList(res);
        }

        [HttpGet]
        [Route("GetPermissionsByNames")]
        public List<InhousePermission> GetPermissionsByNames(string permissions, long endPointSystemId)
        {
            return DtoMapList(DomainModel.InhousePermission.GetPermissionsByNames(permissions, endPointSystemId));
        }
        [HttpGet]
        [Route("CheckAccountExistsInDependentSystems")]
        public string CheckAccountExistsInDependentSystems(long permissionId, long userId, AccountType accountType, string accountName = "")
        {
            return DomainModel.InhousePermission.CheckAccountExistsInDependentSystems(permissionId, userId, accountType, accountName);
        }
        [HttpPost]
        [Route("GetBulkPermissionsByNames")]
        public List<InhousePermission> GetBulkPermissionsByNames(long endPointSystemId, [FromBody]List<string> permissions)
        {
            if(permissions == null)
            {
                permissions = new List<string>();
            }
            return DtoMapList(DomainModel.InhousePermission.GetPermissionsByNames(string.Join(",", permissions), endPointSystemId));
        }
    }
}
