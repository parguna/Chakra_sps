using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AccountReconcilation")]
    public class AccountReconcilationController : BaseController<DomainModel.AccountReconcilation, EntityModel.AccountReconcilation, AccountReconcilation>
    {
        [HttpGet]
        [Route("GetByIdIncludingEndPoint")]
        public AccountReconcilation GetByIdIncludingEndPoint(long id)
        {
            return DtoMap(DomainModel.AccountReconcilation.GetByIdIncludingEndPoint(id));
        }

        [HttpGet]
        [Route("DoesExistByAccountNameAndEndpointSystemId")]
        public bool DoesExistByAccountNameAndEndpointSystemId(string accountName, long EndpointSystemId, Nullable<long> userid = null)
        {
            return DomainModel.AccountReconcilation.DoesExistsByAccountName(accountName, EndpointSystemId, userid);
        }
        [HttpGet]
        [Route("DoesAccountExistInAccountRecon")]
        public Dto.AccountReconcilation DoesAccountExistInAccountRecon(string accountName, long EndpointSystemId, Nullable<long> userid = null)
        {
            return DtoMap(DomainModel.AccountReconcilation.DoesExistsByAccountNameInRecon(accountName, EndpointSystemId, userid));
        }

        [HttpGet]
        [Route("DoesAccountNameExistInTheDependentSystems")]
        public bool DoesAccountNameExistInTheDependentSystems(string accountName, long endpointSystemId, string permissions, AccountType accountType)
        {
            return DomainModel.AccountReconcilation.DoesAccountNameExistInTheDependentSystems(accountName, endpointSystemId, permissions, accountType);
        }

        [HttpGet]
        [Route("GetByAccountType")]
        public long GetByAccountType(long userId, Infrastructure.Enums.AccountType accountType, int endpointSystemId, int parentEpId)
        {
            return DomainModel.AccountReconcilation.GetByAccountTypeForOLR(userId, accountType, endpointSystemId, parentEpId);
        }

        [HttpGet]
        [Route("GetByUser")]
        public AccountReconcilation GetByUser(long endpointUserId, long endpointSystemId)
        {
            return DtoMap(DomainModel.AccountReconcilation.GetByUser(endpointSystemId, endpointUserId));
        }

        [HttpGet]
        [Route("GetByUser")]
        public List<AccountReconcilation> GetByUser(long userId, bool eagerLoadAllProperties, bool isLocked = false)
        {
            return DtoMapList(DomainModel.AccountReconcilation.GetByUser(userId, eagerLoadAllProperties, isLocked = false));
        }
        [HttpGet]
        [Route("GetEndpointUserInfoJson")]
        public string GetEndpointUserInfoJson(long accountReconId)
        {
            return DomainModel.AccountReconcilation.GetEndpointUserInfoJson(accountReconId);
        }
        [HttpGet]
        [Route("GetUserByException")]
        public AccountReconcilation GetUserByException(long endPointSystemId, long endpointUserId, ExceptionTypes exceptionType, bool eagerLoadAllProperties = true)
        {
            return DtoMap(DomainModel.AccountReconcilation.GetUserByException(endPointSystemId, endpointUserId, exceptionType, eagerLoadAllProperties));
        }
        [HttpGet]
        [Route("GetByAccountTypeAndEndpointSystemId")]
        public List<AccountReconcilation> GetByAccountTypeAndEndpointSystemId(long userId, AccountType accountType, long endpointSystemId)
        {
            return DtoMapList(DomainModel.AccountReconcilation.GetByAccountTypeAndEndpointSystemId(userId, accountType, endpointSystemId));
        }

        [HttpGet]
        [Route("GetByAccountType")]
        public List<AccountReconcilation> GetByAccountType(long userId, AccountType accountType)
        {
            return DtoMapList(DomainModel.AccountReconcilation.GetByAccountType(userId, accountType));
        }


        [HttpGet]
        [Route("GetByAccountTypeAndApplicationType")]
        public List<AccountReconcilation> GetByAccountTypeAndApplicationType(long userId, AccountType accountType, ApplicationType applicationType)
        {
            return DtoMapList(DomainModel.AccountReconcilation.GetByAccountTypeAndApplicationType(userId, accountType, applicationType));
        }

        [HttpGet]
        [Route("GetInactiveAccounts")]
        public List<AccountReconcilation> GetInactiveAccounts(long userId, long endpointSystemId, AccountType accountType)
        {
            return DtoMapList(DomainModel.AccountReconcilation.GetInactiveAccounts(userId, endpointSystemId, accountType));
        }

        [HttpGet]
        [Route("GetActiveAccountsByAccountType")]
        public List<AccountReconcilation> GetActiveAccountsByAccountType(long userId, AccountType accountType)
        {
            return DtoMapList(DomainModel.AccountReconcilation.GetActiveAccountsByAccountType(userId, accountType));
        }


        [HttpGet]
        [Route("GetByUserId")]
        public AccountReconcilation GetByUserId(long userId, Infrastructure.Enums.AccountType accountType, long EndpointSystemId)
        {
            return DtoMap(DomainModel.AccountReconcilation.GetByUserId(userId, accountType, EndpointSystemId));
        }


        [HttpPost]
        [Route("AddOrUpdateAccountReconcilation")]
        public void AddOrUpdateAccountReconcilation(IEndpointUserInfo endpointUserInfo, bool IsNewlyAdded, int accountTypeId, string mappingType, long userId, bool fromAccessRequest, long accessRequestId)
        {
            var model = AutoMapper.Mapper.Map<Dto.IEndpointUserInfo, DomainModel.InhouseUserInfo>(endpointUserInfo);
            DomainModel.AccountReconcilation.AddOrUpdateAccountReconcilation(model, IsNewlyAdded, accountTypeId, userId, mappingType, User.Identity.Name, accessRequestId, fromAccessRequest);
        }

        [HttpPost]
        [Route("SaveAccount")]
        public void SaveAccount([FromBody]AccountReconcilation accountReconcilation)
        {
            //DomainModel.AccountReconcilation.SaveAccount(AutoMapper.Mapper.Map<Dto.AccountReconcilation, DomainModel.AccountReconcilation>(accountReconcilation),User.Identity.Name);
        }
        [HttpGet]
        [Route("GetByUserIdAndEndpointSystemId")]
        public List<AccountReconcilation> GetByUserIdAndEndpointSystemId(long userId, long endpointSystemId)
        {
            return DtoMapList(DomainModel.AccountReconcilation.GetByUserIdAndEndpointSystemId(userId, endpointSystemId));
        }

        [HttpGet]
        [Route("GetByEndpointUserId")]
        public   AccountReconcilation GetByEndpointUserId(long endpointSystemId, long endpointUserId, long DIUserId)
        {
            return DtoMap(DomainModel.AccountReconcilation.GetByEndpointUserId(endpointSystemId, endpointUserId, DIUserId));
        }

        [HttpGet]
        [Route("ProcessAccountMapping")]
        public void ProcessAccountMapping(long AccountReconcilationId)
        {
            DomainModel.AccountReconcilation.ProcessAccountMapping(AccountReconcilationId);
        }

        [HttpPost]
        [Route("GetByUserIdsDoesNotExist")]
        public Dictionary<long, string> GetByUserIdsDoesNotExist([FromBody]BulkRequestParameter param)
        {
            bool considerLock = true;
            if (param.keyValuePairs.ContainsKey("considerLock"))
            {
                considerLock = Convert.ToBoolean(param.keyValuePairs["considerLock"]);
            }
            return DomainModel.AccountReconcilation.GetByUserIdsDoesNotExist(param.UserIds,
                                                                             param.EndpointSystemTypeId,
                                                                             (long)param.EndpointSystemId,
                                                                             param.AccountType,
                                                                             param.ApplicationType, param.Locked, param.ActionType, considerLock);
        }

        [HttpPost]
        [Route("CheckDuplicateRequestExist")]
        public string CheckDuplicateRequestExist([FromBody]BulkRequestParameter param)
        {

            return DomainModel.AccountReconcilation.CheckForDuplicateRequest(param.UserIds,
                                                                             param.EndpointSystemId, param.ApplicationType);
        }
        [HttpGet]
        [Route("GetByAccountTypeAndEndpointSystemIdAndAccountName")]
        public List<AccountReconcilation> GetByAccountTypeAndEndpointSystemIdAndAccountName(long userId, AccountType accountType, long endpointSystemId,string accountName)
        {
            return DtoMapList(DomainModel.AccountReconcilation.GetByAccountTypeAndEndpointSystemIdAndAccountName(userId, accountType, endpointSystemId, accountName));
        }
        [HttpPost]
        [Route("DoesAccountNameExistInTheDependentSystemsWithPermission")]
        public bool DoesAccountNameExistInTheDependentSystemsWithPermission(string accountName, long endpointSystemId,AccountType accountType,[FromBody]List<string> permissions)
        {
            if(permissions == null)
            {
                permissions = new List<string>();
            }
            var perm = string.Join(",", permissions);
            return DomainModel.AccountReconcilation.DoesAccountNameExistInTheDependentSystems(accountName, endpointSystemId, perm, accountType);
        }
        [HttpGet]
        [Route("UpdateOneKeyTokenForUserThroughAdminUI")]
        public bool UpdateOneKeyTokenForUserThroughAdminUI(long userId, long endpointSystemId, string oldToken, string newToken, string updatedBy = "System")
        {
            return DomainModel.AccountReconcilation.UpdateOneKeyTokenForUserThroughAdminUI(userId, endpointSystemId, oldToken, newToken, updatedBy);
        }
    }
}
