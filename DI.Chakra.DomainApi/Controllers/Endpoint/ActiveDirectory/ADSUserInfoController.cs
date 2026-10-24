using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("ADSUserInfo")]
    public class ADSUserInfoController : BaseController<DomainModel.ADSUserInfo, EntityModel.ADSUserInfo, Dto.ADSUserInfo>
    {
        [HttpGet]
        [Route("GetByEndPointSystemId")]
        public ADSUserInfo GetByEndPointSystemId(long id, long endPointSystemId)
        {
            return DtoMap(DomainModel.ADSUserInfo.GetByEndPointSystemId(id, endPointSystemId));
        }

        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string sAMAccountName, long endpointsystemId)
        {
            return DomainModel.ADSUserInfo.DoesExists(sAMAccountName, endpointsystemId);
        }

        [HttpGet]
        [Route("DoesExistActiveADSUserInfoByAccountType")]
        public  bool DoesExistActiveADSUserInfoByAccountType(long userId, AccountType accounttype)
        {
            return DomainModel.ADSUserInfo.DoesExistActiveADSUserInfoByAccountType(userId, accounttype);
        }


        [HttpGet]
        [Route("GetByIdIncludeADSGroups")]
        public ADSUserInfo GetByIdIncludeADSGroups(long id)
        {
            return DtoMap(DomainModel.ADSUserInfo.GetByIdIncludeADSGroups(id));
        }

        [HttpGet]
        [Route("GetBysAMAccountName")]
        [AllowAnonymous]
        public ADSUserInfo GetBysAMAccountName(string sAMAccountName)
        {
            return DtoMap(DomainModel.ADSUserInfo.GetBysAMAccountName(sAMAccountName));
        }

        [HttpGet]
        [Route("GetFLINSAccountByDIUserId")]
        public AccountReconDto GetFLINSAccountByDIUserId(long userid, long endpointsystemid, int accountType)
        {
            AccountReconDto dto = new AccountReconDto();
            dto.AccountReconDictionary = new Dictionary<long, string>();
            dto.AccountReconDictionary =   DomainModel.ADSUserInfo.GetFLINSByDIUserId(userid, endpointsystemid, (AccountType)accountType);
            return dto;
        }
        [HttpGet]
        [Route("GetADUsersByDIUserId")]
        public AccountReconDto GetADUsersByDIUserId(long userid, long endpointsystemid, int accountType)
        {
            AccountReconDto dto = new AccountReconDto();
            dto.AccountReconDictionary = new Dictionary<long, string>();
            dto.EntitlementReconcilations = new List<EntitlementReconcilation>();
            dto.AccountReconDictionary =   DomainModel.ADSUserInfo.GetADUsersByDIUserId(userid, endpointsystemid, (AccountType)accountType);
            if (dto.AccountReconDictionary != null && dto.AccountReconDictionary.Count > 0)
            {
                List<long> endPointUsers = dto.AccountReconDictionary.Keys.ToList();
                dto.EntitlementReconcilations = DomainModel.EntitlementReconcilation.GetByEndpointUsers(endPointUsers, endpointsystemid).Where(ent=> ent !=null ).Select(entitlementReconcilation => AutoMapper.Mapper.Map<Dto.EntitlementReconcilation>(entitlementReconcilation)).ToList();
            }
            return dto;
        }

        [HttpGet]
        [Route("GetAccountReconDtoByEndpointUserId")]
        public AccountReconDto GetADUsersByDIUserId(long userid, long endpointsystemid, int accountType, long endpointUserId)
        {
            AccountReconDto dto = new AccountReconDto();
            dto.AccountReconDictionary = new Dictionary<long, string>();
            dto.EntitlementReconcilations = new List<EntitlementReconcilation>();
            dto.AccountReconDictionary = DomainModel.ADSUserInfo.GetADUsersByEndpointUserId(userid, endpointsystemid, (AccountType)accountType, endpointUserId);

            if (dto.AccountReconDictionary != null && dto.AccountReconDictionary.Count > 0)
            {
                List<long> endPointUsers = dto.AccountReconDictionary.Keys.ToList();
                dto.EntitlementReconcilations = DomainModel.EntitlementReconcilation.GetByEndpointUsers(endPointUsers, endpointsystemid).Select(entitlementReconcilation => AutoMapper.Mapper.Map<Dto.EntitlementReconcilation>(entitlementReconcilation)).ToList();
            }
            return dto;
        }

        [HttpGet]
        [Route("IsAccountLocked")]
        public AccountStatus IsAccountLocked(long userID, long endPointSystemID, int adhocAccountType, int endpointSystemType)
        {
            return DomainModel.ADSUserInfo.IsAccountLockedByEndPointId(userID, new List<long> { endPointSystemID }, adhocAccountType, endpointSystemType);
        }

        [HttpGet]
        [Route("GetByDIUserId")]
        public List<ADSUserInfo> GetByDIUserId(long DIUserId)
        {
            return DtoMapList(DomainModel.ADSUserInfo.GetByDIUserId(DIUserId));
        }

        [HttpGet]
        [Route("GetSharedADSUserInfoByName")]
        public ADSUserInfo GetSharedADSUserInfoByName(string userName, AccountType accountType)
        {
            return DtoMap(DomainModel.ADSUserInfo.GetSharedADSUserInfoByName(userName, accountType));
        }
        [HttpGet]
        [Route("GetActiveADAccountForVPN")]
        public Dto.ADSUserInfo GetActiveADAccount(long userId)
        {
            long endPointSystemID = DomainModel.EndPoint.GetEndPointByApplicationType(ApplicationType.ADSingaporePowerLocal).Id;
            return DtoMap(DomainModel.ADSUserInfo.GetByDIUserIdByEndPointId(userId, endPointSystemID));
        }

        [HttpGet]
        [Route("IsAllwedExternalEmailAccess")]
        public bool IsAllowedExternalEmailAccess(long userId, string mail)
        {
            return DomainModel.ADSUserInfo.IsAllowedExternalEmailAccess(userId, mail);
        }
    }
}
