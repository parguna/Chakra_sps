using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.DomainModel
{
    public class AccountReconcilation : DomainModelBase<AccountReconcilation, EntityModel.AccountReconcilation>
    {
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public long? UserId { get; set; }
        public string AccountName { get; set; }
        public string FullName { get; set; }
        public DateTime? AccountCreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsResolved { get; set; }
        public string ResolvedBy { get; set; }
        public string Comments { get; set; }
        public ExceptionTypes ExceptionType { get; set; }
        public bool IsLocked { get; set; }
        public LockScenario LockScenario { get; set; }
        public AccountType AccountType { get; set; }
        public MappingType MappingType { get; set; }
        public OffboardingAccountActionType OffboardingAccountAction { get; set; }

        public ResolvedStatus ResolvedStatus { get; set; }

        public string ProvisionedThrough { get; set; }
        public EndPoint EndPoint { get; set; }
        public bool IsSODConflict { get; set; }

        public bool IsSensitiveAccess { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// Datetime at which this account was last reviewed via attestation.
        /// </summary>
        public DateTime? LastAttestedOn { get; set; }

        /// <summary>
        /// Endpoint Last Login data
        /// </summary>
        public DateTime? LastLogonDate { get; set; }

        public DateTime? ValidFromDate { get; set; }

        public DateTime? ValidToDate { get; set; }

        public long? AccessRequestId { get; set; }
        public DateTime? ExceptionOccuredDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string AdditionalInfo { get; set; }
        public AccountReconcilation()
        {
            CreatedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
        }
        public static List<AccountReconcilation> GetByUser(long userId, bool eagerLoadAllProperties = true, bool isLocked = false)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (!isLocked)
                    return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.UserId == userId && !ar.IsLocked && ar.ExceptionType != ExceptionTypes.Attribute, ar => ar.EndPoint.EndPointSystemType).OrderBy(i => i.EndPoint.DisplayName).ToList());
                else

                    return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.UserId == userId && ar.IsLocked && ar.ExceptionType != ExceptionTypes.Attribute, ar => ar.EndPoint.EndPointSystemType).ToList());
            }
        }

        public static List<AccountReconcilation> GetInActiveByUser(long userId, bool eagerLoadAllProperties = true)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.UserId == userId && (ar.IsLocked || ar.IsDeleted), ar => ar.EndPoint.EndPointSystemType).ToList());
            }
        }
        public static List<AccountReconcilation> GetActiveAccountByUser(long userId, bool eagerLoadAllProperties = true)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.UserId == userId && !ar.IsDeleted && ar.AccountType == AccountType.User, ar => ar.EndPoint.EndPointSystemType).ToList());
            }
        }


        public static List<AccountReconcilation> GetActiveAccountByUser(long userId, EndPoint ep, AccountType accountType, bool eagerLoadAllProperties = true)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get
                    (ar => ar.UserId == userId && ar.AccountType == accountType && !ar.IsDeleted && !ar.IsLocked && ar.EndpointSystemId == ep.Id, ar => ar.EndPoint.EndPointSystemType).ToList());
            }
        }

        public static List<AccountReconcilation> GetActiveAccountByAccountTypeAndEndpointSystemId(EndPoint ep, AccountType accountType, bool eagerLoadAllProperties = true)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get
                    (ar => ar.AccountType == accountType && !ar.IsDeleted && !ar.IsLocked && ar.EndpointSystemId == ep.Id, ar => ar.EndPoint.EndPointSystemType).ToList());
            }
        }

        public static List<AccountReconcilation> GetActiveAccountByUser(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.UserId == userId && !ar.IsDeleted && !ar.IsLocked, ar => ar.EndPoint
                    ).ToList());
            }
        }

        public static void ProcessAccountMapping(long AccountReconcilationId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                AccountReconcilation acctRecon = DomainModel.AccountReconcilation.GetByIdIncludingEndPoint(AccountReconcilationId);

                if (acctRecon.EndPoint == null || acctRecon.EndPoint.EndPointSystemType == null || acctRecon.EndpointUserId == 0 || acctRecon.UserId == null) return;
                string masterUser = MasterConfigHelper.ServiceUser;
                long endpointUserId = acctRecon.EndpointUserId;
                User user = User.GetById(acctRecon.UserId.Value);
                switch ((EndpointType)acctRecon.EndPoint.EndPointSystemType.Id)
                {
                    case EndpointType.SAP:
                        {
                            var sAPUserinfo = SAPUserinfo.GetById(endpointUserId);
                            sAPUserinfo.Function0 = user.EmployeeId;
                            sAPUserinfo.Save(masterUser);
                            break;
                        }
                    case EndpointType.ActiveDirectory:
                        {
                            var adsUserInfo = ADSUserInfo.GetById(endpointUserId);
                            adsUserInfo.physicalDeliveryOfficeName = user.EmployeeId;
                            adsUserInfo.Save(masterUser);
                            break;
                        }
                    case EndpointType.OneCard:
                        {
                            var sMSUserInfo = SMSUserInfo.GetById(endpointUserId);
                            sMSUserInfo.Zip = user.EmployeeId;
                            sMSUserInfo.SSNO = user.EmployeeId;
                            sMSUserInfo.Save(masterUser);
                            break;
                        }
                    //[03/11/2022] update the employee ID when the mapping to different user
                    case EndpointType.InhouseSystems:
                        {
                            var sInhouseUserInfo = InhouseUserInfo.GetById(endpointUserId);
                            sInhouseUserInfo.EmployeeId = user.EmployeeId;
                            sInhouseUserInfo.Email = user.EmailId;
                            if (sInhouseUserInfo.AccountType == AccountType.User)
                            {
                                sInhouseUserInfo.DisplayName = user.DisplayName;
                            }
                            sInhouseUserInfo.Save(masterUser);
                            break;
                        }
                    default: break;

                }

            }
        }

        public static void UpdateOffBoardingByEndpointId(long endpointId)
        {
            string updateQuery = $"Update DI_M_AccountReconcilation Set OffboardingAccountAction = CASE WHEN (AccountType = 1 OR AccountType = 4) THEN 3 ELSE 2 END WHERE IsDeleted=0 AND AccountType > 0 and MappingType > 0 and OffboardingAccountAction <=0 AND endpointsystemid = {endpointId}";
            ExecuteQueryWithCommandTimeout(updateQuery, 0);
        }

        public static List<AccountReconcilation> GetInactiveAccounts(long userId, long endpointSystemId, AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.UserId == userId && ar.AccountType == accountType && ar.EndpointSystemId == endpointSystemId).ToList());
            }
        }

        public static void UpdateException(AccountReconcilation accountReconcilation, bool IsAdded)
        {
            if (!IsAdded)
            {
                accountReconcilation.IsDeleted = true;
                accountReconcilation.IsLocked = true;
                accountReconcilation.LastAttestedOn = null;
                accountReconcilation.DeletedDate = DateTime.Now;
                accountReconcilation.ExceptionType = ExceptionTypes.Removed;
                accountReconcilation.ExceptionOccuredDate = DateTime.Now;
            }
            else
            {
                accountReconcilation.LastAttestedOn = null;
                accountReconcilation.IsDeleted = false;
                accountReconcilation.DeletedDate = null;
                accountReconcilation.IsLocked = false;
                accountReconcilation.LockScenario = LockScenario.None;
                accountReconcilation.ExceptionType = ExceptionTypes.Added;
                accountReconcilation.ExceptionOccuredDate = DateTime.Now;
            }

            accountReconcilation.IsResolved = false;
            accountReconcilation.ResolvedBy = string.Empty;
            accountReconcilation.ResolvedStatus = 0;
            accountReconcilation.ProvisionedThrough = Infrastructure.Enums.ProvisionedThrough.BackEnd.ToString();
        }
                
        public static List<AccountReconcilation> GetAllUser(bool eagerLoadAllProperties = true, bool isLocked = false)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (!isLocked)
                    return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => !ar.IsLocked, ar => ar.EndPoint.EndPointSystemType).ToList());
                else
                    return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.IsLocked, ar => ar.EndPoint.EndPointSystemType).ToList());
            }
        }
        public static IList<AccountReconcilation> GetAllByUser(long userId, bool eagerLoadAllProperties = true)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(recon => recon.UserId == userId, eagerLoadAllProperties).ToList());
            }
        }
        public static List<AccountReconcilation> GetByAccountTypeAndEndpointSystemId(long userId, AccountType accountType, long endpointSystemId)
        {
            var allAccounts = GetByEndpointSystemId(userId, endpointSystemId);
            return allAccounts.Where(t => t.AccountType == accountType).ToList();
        }

        public static List<AccountReconcilation> GetByAccountTypeAndEndpointSystemIdAndAccountName(long userId, AccountType accountType, long endpointSystemId, string accountName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new AccountReconcilationRepository(dataAccessTransaction).GetByAccountTypeAndEndpointSystemIdAndAccountName(userId, accountType, endpointSystemId, accountName));
            }
        }

        //public static List<AccountReconcilation> GetByAccountTypeAndEndpointSystemIdAndAccountName(long userId, AccountType accountType, long endpointSystemId, string accountName)
        //{
        //    var allAccounts = GetByEndpointSystemId(userId, endpointSystemId);
        //    return allAccounts.Where(t => t.AccountType == accountType && t.AccountName.Equals(accountName, StringComparison.InvariantCultureIgnoreCase)).ToList();
        //}
        public static List<AccountReconcilation> GetByAccountType(long userId, AccountType accountType)
        {
            var allAccounts = GetByUser(userId);
            return allAccounts.Where(t => t.AccountType == accountType).ToList();
        }

        public static List<AccountReconcilation> GetByAccountTypeAndApplicationType(long userId, AccountType accountType, ApplicationType applicationType)
        {
            var allAccounts = GetByUser(userId);
            return allAccounts.Where(t => t.AccountType == accountType && t.EndPoint.ApplicationType == applicationType).ToList();
        }

        public static IList<AccountReconcilation> GetSelfServiceAccountsByUserandAccountType(long userId, AccountType accountType, bool eagerLoadAllProperties = true)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                /* return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(recon => recon.UserId == userId && recon.AccountType == accountType
                 && (!recon.CanSelfService.HasValue || recon.CanSelfService.Value) && !recon.IsDeleted && recon.EndPoint.EndPointSystemType.EndpointSystemTypeName != EndpointType.InhouseSystems
                 , recon => recon.EndPoint.EndPointSystemType).ToList());*/

                 return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(recon => recon.UserId == userId && recon.AccountType == accountType
                 && !recon.IsDeleted && recon.EndPoint.EndPointSystemType.EndpointSystemTypeName != EndpointType.InhouseSystems
                , recon => recon.EndPoint.EndPointSystemType).ToList());
            }
        }

        public static IList<AccountReconcilation> GetAllSelfServiceAccountsByUserandAccountType(long userId, bool eagerLoadAllProperties = true)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
               
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(recon => recon.UserId == userId 
                && !recon.IsDeleted && (recon.EndPoint.IsPasswordResetAllowed || recon.EndPoint.IsUnlockAllowed) && recon.EndPoint.EndPointSystemType.EndpointSystemTypeName != EndpointType.InhouseSystems
               , recon => recon.EndPoint.EndPointSystemType).ToList());
            }
        }

        public static List<AccountReconcilation> GetActiveAccountsByAccountType(long userId, AccountType accountType)
        {
            var allAccounts = GetByUser(userId);
            return allAccounts.Where(t => t.AccountType == accountType && (t.IsLocked == false || t.IsDeleted == false)).ToList();
        }
        public static IList<AccountReconcilation> GetByAccountType(long userId, AccountType accountType, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(recon => recon.UserId == userId && recon.AccountType == accountType && recon.EndpointSystemId == endpointSystemId).ToList());
            }
        }

        public static long GetByAccountTypeForOLR(long userId, AccountType accountType, long endpointSystemId, long parentEpId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var accountReconList = new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(recon => recon.UserId == userId && recon.AccountType == accountType && recon.EndpointSystemId == parentEpId).ToList();
                if (accountReconList != null && accountReconList.Count > 0 && !accountReconList.Any(a => a.IsDeleted || a.IsLocked))
                {
                    return -1;
                }
                else
                {
                    var accessRequestList = AccessRequest.GetByAccountType(userId, accountType, Convert.ToInt32(endpointSystemId));
                    if (accessRequestList != null && accessRequestList.Count > 0)
                    {
                        return accessRequestList.FirstOrDefault().Id;
                    }

                    var interimList = InterimStore.GetByType(InterimStoreItemType.AccessRequest).ToList();
                    if (interimList != null && interimList.Count > 0)
                    {
                        List<AdHocAccessRequest> listadhoc = new List<AdHocAccessRequest>();
                        for (int i = 0; i < interimList.Count; i++)
                        {
                            var adhocaccessrequest = UtilityMethods.Deserialize<AdHocAccessRequest>(interimList[i].XmlData);
                            adhocaccessrequest.Id = interimList[i].Id;
                            if (adhocaccessrequest.RequesteeUserId == userId)
                                listadhoc.Add(adhocaccessrequest);
                        }

                        if (listadhoc.Any(lq => lq.AdhocActionType == AdHocActionType.NewAccount
                                     && lq.EndpointSystemId == endpointSystemId && lq.AccountType == accountType))
                        {
                            return -1;
                        }

                    }
                }
                //return new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(recon => recon.UserId == userId && recon.AccountType == accountType && recon.EndpointSystemId == endpointSystemId).ToList());
                return 0;
            }
        }

        public static List<AccountReconcilation> GetByEndpointSystemId(long userId, long endpointSystemId)
        {
            var allAccounts = GetByUser(userId);
            var returnValue = allAccounts.Where(t => t.EndpointSystemId == endpointSystemId && t.IsLocked == false && t.IsDeleted == false).ToList();
            return returnValue;
        }
        public static List<AccountReconcilation> GetByEndpointSystemIdForAttestation(long userId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var allAccounts = ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.UserId == userId && ar.EndpointSystemId == endpointSystemId && ar.IsDeleted == false && (ar.AccountType == AccountType.User || ar.AccountType == AccountType.Shared ||
            ar.AccountType == AccountType.Secondary || ar.AccountType == AccountType.Service
            ), ar => ar.EndPoint.EndPointSystemType).ToList());
                return allAccounts;
            }
                
        }
        public static List<AccountReconcilation> GetUnmappedAccountsByEndpointSystemId(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var allUnmappedAccounts = ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar =>
                    ar.EndpointSystemId == endpointSystemId
                    //&& ar.IsLocked == false
                    && ar.IsDeleted == false
                    && ar.UserId == null,
                    true).ToList());

                return allUnmappedAccounts;
            }
        }

        public static List<AccountReconcilation> GetAcountsByEndpointSystemId(long endpointSystemId, AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var accounts = ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar =>
                    ar.EndpointSystemId == endpointSystemId && ar.AccountType == accountType).ToList());

                return accounts;
            }
        }

        public static List<AccountReconcilation> GetBySystem(long systemId, MappingType mappingTypes = MappingType.None, bool employeeHasMoreAccounts = false, bool eagerLoadAllProperties = true, bool isLocked = false)
        {
            List<AccountReconcilation> list = new List<AccountReconcilation>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (employeeHasMoreAccounts)
                {
                    if (systemId > 0)
                    {

                        list = ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar =>
                        ar.EndpointSystemId == systemId
                        && ar.IsLocked == isLocked
                        && ar.IsDeleted == false,
                        eagerLoadAllProperties).ToList());
                    }
                    else
                    {
                        list = ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar =>
                       ar.IsLocked == isLocked
                      && ar.IsDeleted == false,
                       eagerLoadAllProperties).ToList());

                    }
                }
                else
                {
                    if (systemId > 0)
                    {
                        list = ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar =>
                         ar.EndpointSystemId == systemId
                         && ar.IsLocked == isLocked
                         && ar.IsDeleted == false
                         && ar.MappingType == mappingTypes,
                         eagerLoadAllProperties).ToList());
                    }
                    else
                    {
                        list = ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar =>
                       ar.IsLocked == isLocked
                        && ar.IsDeleted == false
                        && ar.MappingType == mappingTypes,
                        eagerLoadAllProperties).ToList());
                    }
                }

                if (!employeeHasMoreAccounts)
                    return list;

                var multiAcForOneOwner = (from p in list
                                          group p by p.UserId into grps
                                          where grps.Count() > 1 && grps.Key != null
                                          select grps).ToList();

                if (multiAcForOneOwner.Count <= 0)
                    return new List<AccountReconcilation>();

                List<AccountReconcilation> returnList = new List<AccountReconcilation>();

                foreach (var gAcs in multiAcForOneOwner)
                {
                    foreach (var recon in gAcs)
                    {
                        returnList.Add(recon);
                    }
                }

                return returnList;
            }

        }


        public static AccountReconcilation GetActiveByDIUserId(long userId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).
                    Get(recon => recon.UserId == userId && recon.EndpointSystemId == endpointSystemId && recon.IsLocked != true && recon.IsDeleted == false).FirstOrDefault());
            }
        }

        public static AccountReconcilation GetByDIUserId(long userId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).
                    Get(recon => recon.UserId == userId && recon.EndpointSystemId == endpointSystemId && recon.IsLocked != true).FirstOrDefault());
            }
        }
        public static AccountReconcilation GetAccountByDIUserId(long userId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).
                    Get(recon => recon.AccountType == DI.Chakra.Infrastructure.Enums.AccountType.User && recon.UserId == userId && recon.EndpointSystemId == endpointSystemId).FirstOrDefault());
            }
        }
        public static AccountReconcilation GetByDIUserIdForSFB(long userId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).
                    Get(recon => recon.UserId == userId && recon.IsDeleted == false && recon.EndpointSystemId == endpointSystemId && recon.IsLocked == false).FirstOrDefault());
            }
        }
        public static List<AccountReconcilation> GetByUserIdAndEndpointSystemId(long userId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).
                    Get(recon => recon.UserId == userId && recon.EndpointSystemId == endpointSystemId).ToList());
            }
        }

        public static List<AccountReconcilation> GetAllActiveAccountsByUserIdAndEndpointSystemId(long userId, long endpointSystemId, AccountType accountTye)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).
                    Get(recon => recon.UserId == userId && recon.EndpointSystemId == endpointSystemId
                              && !recon.IsDeleted && !recon.IsLocked && recon.AccountType == accountTye).ToList());
            }
        }

        public static AccountReconcilation GetByUser(long endpointSystemId, long endpointUserId, bool eagerLoadAllProperties = true)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.EndpointUserId == endpointUserId && ar.EndpointSystemId == endpointSystemId, eagerLoadAllProperties).FirstOrDefault());
            }
        }
        public static AccountReconcilation GetByIDMSUserByDIUserID(long endpointSystemId, long userId, bool eagerLoadAllProperties = true)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.UserId == userId && ar.EndpointSystemId == endpointSystemId && (ar.IsLocked || ar.IsDeleted), eagerLoadAllProperties).FirstOrDefault());
            }
        }
        public static AccountReconcilation GetByEndpointUserId(long endpointSystemId, long endpointUserId, long DIUserId, bool eagerLoadAllProperties = true)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.EndpointUserId == endpointUserId && ar.EndpointSystemId == endpointSystemId && ar.UserId == DIUserId, eagerLoadAllProperties).FirstOrDefault());
            }
        }

        public static AccountReconcilation GetByUser(long endpointSystemId, long endpointUserId, ExceptionTypes exceptionType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.EndpointUserId == endpointUserId && ar.EndpointSystemId == endpointSystemId && ar.ExceptionType == exceptionType).FirstOrDefault());
            }
        }
        public static AccountReconcilation GetUserByException(long endPointSystemId, long endpointUserId, ExceptionTypes exceptionType, bool eagerLoadAllProperties = true)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.EndpointUserId == endpointUserId && ar.EndpointSystemId == endPointSystemId && ar.ExceptionType == exceptionType, eagerLoadAllProperties).FirstOrDefault());
            }
        }
        public static AccountReconcilation GetExceptionComment(long endpointSystemId, long endpointUserId, long userId, int version)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.UserId == userId && ar.EndpointUserId == endpointUserId && ar.EndpointSystemId == endpointSystemId).FirstOrDefault());
            }
        }

        public static void SaveWhileUpload(InhouseUserInfo oINH, string currentUser, List<InhousePermission> allPermissions,bool canConsiderForInactiveAccount, bool mapAll)
        {
            AccountReconcilation oARedit = GetByAccountName(oINH.AccountName, oINH.EndPointSystemId);

            //var endpointConfiguration = EndpointConfiguration.GetByEndpointId(oINH.EndPointSystemId);
            var ep = EndPoint.GetById(oINH.EndPointSystemId);

            bool updateException = false;
            if (oARedit == null)
            {
                oARedit = new AccountReconcilation();
                oARedit.EndpointSystemId = oINH.EndPointSystemId;
                oARedit.EndpointUserId = Convert.ToInt32(oINH.Id);
                oARedit.AccountName = oINH.AccountName;
                oARedit.AccountType = oINH.AccountType;
                oARedit.FullName = oINH.DisplayName;
                oARedit.CreatedDate = oINH.CreatedDate;
                oARedit.CreatedBy = "IACMAdmin";
                oARedit.IsDeleted = oINH.IsDeleted;
                oARedit.IsLocked = oINH.IsLocked;
                if (oINH.ValidTo != null && oINH.IsDeleted == false)
                    oARedit.ValidToDate = oINH.ValidTo;
            }
            else
            {
                oARedit.EndpointSystemId = oINH.EndPointSystemId;
                oARedit.EndpointUserId = Convert.ToInt32(oINH.Id);
                oARedit.AccountName = oINH.AccountName;
                oARedit.FullName = oINH.DisplayName;
                oARedit.AccountType = oINH.AccountType;
                oARedit.CreatedDate = oINH.CreatedDate;
                oARedit.LastModifiedDate = DateTime.Now;
                oARedit.LastModifiedBy = "IACMAdmin";
                if(oINH.ValidTo != null && oINH.IsDeleted == false)
                    oARedit.ValidToDate = oINH.ValidTo;
                if (oINH.IsDeleted && !oARedit.IsDeleted)
                {
                    UpdateException(oARedit, false);
                }
                if (!oINH.IsDeleted && oARedit.IsDeleted)
                {
                    UpdateException(oARedit, true);
                }
            }

            oARedit.OffboardingAccountAction = (oARedit.AccountType == AccountType.User || oARedit.AccountType == AccountType.Secondary) ? OffboardingAccountActionType.Delete : OffboardingAccountActionType.Supervisor;
            oARedit.Save(oARedit, currentUser);

            if (oARedit != null && oARedit.IsDeleted == false)
            {
                if (!string.IsNullOrWhiteSpace(oINH.Permission))
                {
                    string[] entitlements = null;
                    if (ep.IsFunctionLevel)
                    {
                        entitlements = oINH.Permission.Split('|');
                    }
                    else
                    {
                        entitlements = oINH.Permission.Split(',');
                    }

                    foreach (var item in entitlements.Distinct())
                    {
                        try
                        {

                            var ent = item.Split(new string[] { Constants.SPLITTER_FUCNTIONS }, StringSplitOptions.RemoveEmptyEntries);
                            var permission = ent[0].Trim();
                            var functions = ent.Length > 1 ? ent[1] : string.Empty;

                            var permissionObj = allPermissions.Where(x => x.Name.ToLower() == permission.ToLower() && x.EndPointSystemId == oINH.EndPointSystemId).FirstOrDefault();
                            if(permissionObj == null)
                            {
                                continue;
                            }
                            MapUserPermissionFunction(oINH.Id, permissionObj, functions, ep);

                            var entRecon = EntitlementReconcilation.GetByAccountReconcilationIdAndEntitlementName(oARedit.Id, permissionObj.EntitlementName);

                            if (entRecon == null)
                            {
                                entRecon = new EntitlementReconcilation();
                                entRecon.EnteredDate = DateTime.Now;
                                entRecon.CreatedDate = DateTime.Now;
                                EntitlementReconcilation.UpdateException(entRecon, true);
                            }

                            entRecon.AccountReconcilationId = oARedit.Id;
                            entRecon.EndpointType = EndpointType.InhouseSystems;
                            entRecon.EndpointSystemId = oARedit.EndpointSystemId;
                            entRecon.EndpointUserId = oARedit.EndpointUserId;
                            entRecon.UserId = oARedit.UserId;
                            entRecon.EntitlementType = (int)EntitlementType.Permission;
                            entRecon.AccountName = oARedit.AccountName;
                            entRecon.Entitlement = permission;
                            entRecon.EntitlementDescription = permissionObj.Description;
                            entRecon.EntitlementId = permissionObj.Id;
                            entRecon.LastModifiedDate = DateTime.Now;
                            if (entRecon.IsDeleted && oARedit.IsDeleted == false)//For active account only
                            {
                                entRecon.IsDeleted = false;
                                entRecon.DeletedDate = null;
                            }
                            if (!string.IsNullOrWhiteSpace(functions))
                            {
                                if (!string.IsNullOrWhiteSpace(entRecon.Functions))
                                {
                                    foreach (var function in functions.Split(','))
                                    {
                                        if (!entRecon.Functions.Contains(function))
                                            entRecon.Functions = string.Join(",", new string[] { entRecon.Functions, functions });
                                    }
                                }
                                else
                                    entRecon.Functions = functions;
                            }

                            entRecon.LastModifiedBy = "System";
                            entRecon.Save(currentUser);

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else
                {
                    if (oARedit != null )
                    {
                        var entRecons = EntitlementReconcilation.GetByAccountReconcilationId(oARedit.Id, oARedit.EndpointSystemId);
                        foreach (var entRecon in entRecons)
                        {
                            EntitlementReconcilation.UpdateException(entRecon, false);
                            entRecon.Save(currentUser);
                        }
                    }
                }

                InhouseUserInfo.MapWithMasterUserForReconTable(oINH.EndPointSystemId, mapAll , canConsiderForInactiveAccount, null);
                InhouseUserInfo.MapWithMasterUserForEntitlementReconTable(oINH.EndPointSystemId, mapAll, null);
            }
            //dataAccessTransaction.Commit();
            //}
        }

        private static void MapUserPermissionFunction(long endpointUserId, InhousePermission permission, string functions, EndPoint endPoint)
        {
            string currentUser = "System";
            if (!string.IsNullOrWhiteSpace(functions))
            {
                foreach (var function in functions.Split(','))
                {
                    var functionObj = InhouseFunction.GetInhouseFunctionByApplicationCode(function.Trim(), endPoint.ApplicationCode, permission.Name);
                    if (functionObj == null)
                        continue;

                    var existingUPF = InhUserPermissionFunction.GetByEndpointUserIdPermissionIdFunctionId(endpointUserId, permission.Id, functionObj.Id, Convert.ToInt64(endPoint.Id));
                    if (existingUPF == null)
                    {
                        existingUPF = new InhUserPermissionFunction();
                        existingUPF.EndpointUserId = endpointUserId;
                        existingUPF.EndPointSystemId = endPoint.Id;
                        existingUPF.InhouseSystemCode = endPoint.ApplicationCode;
                        existingUPF.Permission = permission.Name;
                        existingUPF.PermissionId = permission.Id;
                        existingUPF.FunctionId = functionObj.Id;
                        existingUPF.Functions = functions;
                        existingUPF.CreatedBy = currentUser;
                        existingUPF.CreatedDate = DateTime.Now;
                        existingUPF.Save(currentUser);
                    }
                }
            }
            else
            {
                var existingUPF = InhUserPermissionFunction.GetByEndpointUserIdPermissionId(endpointUserId, permission.Id);
                if (existingUPF == null)
                {
                    existingUPF = new InhUserPermissionFunction();
                    existingUPF.EndpointUserId = endpointUserId;
                    existingUPF.EndPointSystemId = endPoint.Id;
                    existingUPF.InhouseSystemCode = endPoint.ApplicationCode;
                    existingUPF.Permission = permission.Name;
                    existingUPF.PermissionId = permission.Id;
                    existingUPF.CreatedBy = currentUser;
                    existingUPF.CreatedDate = DateTime.Now;
                    existingUPF.Save(currentUser);
                }
            }
        }

        public static AccountReconcilation GetByAccountName(string accountName, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).
                    Get(ar => ar.AccountName.Trim().ToUpper() == accountName.Trim().ToUpper() && ar.EndpointSystemId == endpointSystemId).OrderBy(i => i.CreatedDate).FirstOrDefault());
            }
        }

        public static List<AccountReconcilation> GetByAccountNames(string accountName, List<long> endpointSystemIds)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).
                    Get(ar => ar.AccountName.Trim().ToUpper() == accountName.Trim().ToUpper() && endpointSystemIds.Contains(ar.EndpointSystemId)).OrderBy(i => i.CreatedDate).ToList());
            }
        }

        public static AccountReconcilation GetByAccountName(string accountName, long endpointSystemId, long DiUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var accountRecon = ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).
                    Get(ar => ar.AccountName.Trim().ToUpper() == accountName.Trim().ToUpper() && ar.EndpointSystemId == endpointSystemId).OrderBy(i => i.CreatedDate).FirstOrDefault());

                if (accountRecon != null && (accountRecon.IsDeleted || accountRecon.IsLocked) && accountRecon.UserId == DiUserId)
                {
                    return null;
                }

                return accountRecon;
            }
        }

        public static AccountReconcilation GetByAccountNameByAccountType(string accountName, long endpointSystemId, AccountType accoutnType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).
                    Get(ar => ar.AccountName == accountName && ar.EndpointSystemId == endpointSystemId && ar.AccountType == accoutnType).FirstOrDefault());
            }
        }

        public static bool DoesExistsByAccountName(string accountName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Any(ar => ar.AccountName == accountName);
            }
        }
        public static bool DoesExistsByAccountName(string accountName, long systemId, Nullable<long> userid)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (userid == null || (userid.HasValue && userid.Value == 0))
                {
                    return new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Any(ar => ar.AccountName == accountName
                    && ar.EndpointSystemId == systemId && ar.IsDeleted == false);
                }
                else
                {
                    return new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Any(ar => ar.UserId != userid && ar.AccountName == accountName
                   && ar.EndpointSystemId == systemId);
                }

            }
        }
        public static AccountReconcilation DoesExistsByAccountNameInRecon(string accountName, long systemId, Nullable<long> userid)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (userid == null || (userid.HasValue && userid.Value == 0))
                {
                    return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.AccountName == accountName
                    && ar.EndpointSystemId == systemId).FirstOrDefault());
                }
                else
                {
                    return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(ar => ar.UserId != userid && ar.AccountName == accountName
                   && ar.EndpointSystemId == systemId).FirstOrDefault());
                }
            }
        }
        public static bool DoesAccountNameExistInTheDependentSystems(string accountName, long endpointSystemId, string permissions, AccountType accountType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var inhouseMatrix = InhousePermission.GetPermissionsByNames(permissions, endpointSystemId);
                var adExists = inhouseMatrix.Any(p => !string.IsNullOrWhiteSpace(p.ADGroups));
                var epExists = inhouseMatrix.Any(p => !string.IsNullOrWhiteSpace(p.EPGroups) || !string.IsNullOrWhiteSpace(p.EPRoles));
                var ceExists = inhouseMatrix.Any(p => !string.IsNullOrWhiteSpace(p.CEGroups) || !string.IsNullOrWhiteSpace(p.CERoles));

                bool accountNameExists = false;
                if (adExists)
                {
                    var endpoint = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                    accountNameExists = new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Any(ar => ar.AccountName == accountName
                    && ar.EndpointSystemId == endpoint.Id && ar.IsDeleted == false && ar.AccountType == accountType);
                    if (!accountNameExists)
                        return false;
                }
                if (epExists)
                {
                    var endpoint = EndPoint.GetByApplicationType(ApplicationType.SAP_EP);
                    accountNameExists = new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Any(ar => ar.AccountName == accountName
                    && ar.EndpointSystemId == endpoint.Id && ar.IsDeleted == false && ar.AccountType == accountType);
                    if (!accountNameExists)
                        return false;
                }
                if (ceExists)
                {
                    var endpoint = EndPoint.GetByApplicationType(ApplicationType.CE);
                    accountNameExists = new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Any(ar => ar.AccountName == accountName
                    && ar.EndpointSystemId == endpoint.Id && ar.IsDeleted == false && ar.AccountType == accountType);
                    if (!accountNameExists)
                        return false;
                }
                return true;
            }
        }

        public static AccountReconcilation GetByIdIncludingEndPoint(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).GetById(id, ar => ar.EndPoint, ar1 => ar1.EndPoint.EndPointSystemType));
            }
        }





        public static void DeleteAccount(long endpointSystemId, long endpointUserId, string currentUser, long? accessRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                AccountReconcilation recon = GetByUser(endpointSystemId, endpointUserId);
                if (recon == null)
                    return;

                EntitlementReconcilation.DeleteEntitlements(recon.Id, recon.EndpointSystemId, currentUser, accessRequestId.Value);

                recon.IsDeleted = true;
                recon.LastAttestedOn = null;
                recon.DeletedDate = DateTime.Now;
                recon.Save(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }

        public static void LockAccount(long endpointSystemId, long endpointUserId, LockScenario lockScenario, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                AccountReconcilation recon = GetByUser(endpointSystemId, endpointUserId);
                if (recon == null)
                    return;
                recon.IsLocked = true;
                recon.LockScenario = lockScenario;
                recon.Save(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }

        public static void UnLockAccount(long endpointSystemId, long endpointUserId, LockScenario lockScenario, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                AccountReconcilation recon = GetByUser(endpointSystemId, endpointUserId);
                if (recon == null)
                    return;
                recon.IsLocked = false;
                recon.LockScenario = LockScenario.None;
                recon.Save(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }

        public static AccountReconcilation GetByUserId(long userId, AccountType accountType, long EndpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(recon => recon.UserId == userId
                && recon.AccountType == accountType && recon.EndpointSystemId == EndpointSystemId).FirstOrDefault());
            }
        }

        public static AccountReconcilation GetByUserName(string userName, AccountType accountType, long EndpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(recon => recon.AccountName == userName
                && recon.AccountType == accountType && recon.EndpointSystemId == EndpointSystemId).FirstOrDefault());
            }
        }

        public void Save(AccountReconcilation accountReconcilation, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                AccountReconcilation reconcilation = null;
                if ((accountReconcilation.EndpointUserId != null) && (accountReconcilation.EndpointSystemId != null))
                    reconcilation = AccountReconcilation.GetByUser((long)accountReconcilation.EndpointUserId, accountReconcilation.EndpointSystemId);

                if (reconcilation == null)
                {
                    accountReconcilation.CreatedDate = DateTime.Now;
                    accountReconcilation.Save(dataAccessTransaction);
                }
                else
                {
                    accountReconcilation.Save(dataAccessTransaction);
                }
                dataAccessTransaction.Commit();
            }
        }

        #region IEndpointUserInfo
        public static void AddOrUpdateAccountReconcilation(IEndpointUserInfo endpointUserInfo, bool IsNewlyAdded, int accountTypeId, long userId, string mappingType, string currentUser, long accessRequestId = 0, bool fromAccessRequest = false)
        {
            AccessRequest accessRequest = null;
            if (accessRequestId > 0)
                accessRequest = AccessRequest.GetByIdIncludeAdHocAccessRequest(accessRequestId);

            AccountReconcilation acctRecon = GetByUser(endpointUserInfo.EndPointSystemId, endpointUserInfo.Id);
            if (acctRecon != null)
            {
                InhouseUserInfo inhUser = InhouseUserInfo.GetById(acctRecon.EndpointUserId);
                if (inhUser != null)
                {
                    acctRecon.IsDeleted = inhUser.IsDeleted;
                    acctRecon.IsLocked = inhUser.IsDisabled;
                    acctRecon.LastModifiedBy = currentUser;
                    acctRecon.LastModifiedDate = DateTime.Now;
                }
            }
            else
            {
                acctRecon = new AccountReconcilation();
                acctRecon.AccountCreatedDate = DateTime.Now;
                acctRecon.CreatedDate = DateTime.Now;
            }

            acctRecon.AccountType = (AccountType)Convert.ToInt32(accountTypeId);
            acctRecon.FullName = endpointUserInfo.DisplayName;
            acctRecon.AccountName = endpointUserInfo.AccountName;
            acctRecon.EndpointSystemId = endpointUserInfo.EndPointSystemId;
            acctRecon.EndpointUserId = endpointUserInfo.Id;
            if (IsNewlyAdded)
            {
                acctRecon.UserId = userId == 0 ? (long?)null : userId;
                if (userId != 0)
                    acctRecon.MappingType = (MappingType)Enum.Parse(typeof(MappingType), mappingType);

                acctRecon.IsResolved = false;

                if (fromAccessRequest)
                {
                    if (accessRequest != null && accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        acctRecon.IsDeleted = false;
                        acctRecon.IsLocked = false;
                        acctRecon.LockScenario = LockScenario.None;
                        acctRecon.DeletedDate = null;

                        acctRecon.ProvisionedThrough = Infrastructure.Enums.ProvisionedThrough.Requested.ToString();
                        acctRecon.AccessRequestId = accessRequestId;
                        acctRecon.ValidToDate = accessRequest.ActivationDate;
                        acctRecon.ValidToDate = accessRequest.DeactivationDate;
                    }
                    else if (accessRequest != null && accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        acctRecon.ProvisionedThrough = Infrastructure.Enums.ProvisionedThrough.Requested.ToString();
                        acctRecon.AccessRequestId = accessRequestId;
                        acctRecon.ValidToDate = DateTime.Now;
                    }
                }
                else
                {
                    UpdateException(acctRecon, true);
                }
            }
            else
            {
                if (acctRecon.UserId != userId)
                {
                    acctRecon.UserId = userId == 0 ? (long?)null : userId;
                    acctRecon.MappingType = (MappingType)Enum.Parse(typeof(MappingType), mappingType);
                }

                if (fromAccessRequest)
                {
                    if (accessRequest != null && accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        acctRecon.ProvisionedThrough = Infrastructure.Enums.ProvisionedThrough.Requested.ToString();
                        acctRecon.AccessRequestId = accessRequestId;
                        acctRecon.ValidToDate = accessRequest.ActivationDate;
                        acctRecon.ValidToDate = accessRequest.DeactivationDate;
                    }
                    else if (accessRequest != null && accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        acctRecon.ProvisionedThrough = Infrastructure.Enums.ProvisionedThrough.Requested.ToString();
                        acctRecon.AccessRequestId = accessRequestId;
                        acctRecon.ValidToDate = DateTime.Now;
                    }
                }

                acctRecon.IsResolved = acctRecon.IsResolved;
                acctRecon.ResolvedBy = acctRecon.ResolvedBy;
                acctRecon.ResolvedStatus = acctRecon.ResolvedStatus;
                acctRecon.ExceptionType = acctRecon.ExceptionType;
            }

            acctRecon.Save(currentUser);
            if (endpointUserInfo.Entitlements != null)
                AddOrUpdateEntitlementReconcilation(endpointUserInfo, acctRecon, currentUser, accessRequestId, fromAccessRequest);

            //For Extended Accessrequests - Updating Root Request ValidTo date with new date
            if (accessRequest != null && accessRequest.RequestSource == AccessRequestSource.ExtendedRequest)
            {
                var rootAceessRequest = AccessRequest.GetById(accessRequest.ParentRequestId.HasValue ? accessRequest.ParentRequestId.Value : 0);

                if (rootAceessRequest != null && rootAceessRequest.Id > 0)
                {
                    if (rootAceessRequest.AdHocAccessRequestId.HasValue)
                    {
                        var rootAdhocAccessRequest = AdHocAccessRequest.GetById(rootAceessRequest.AdHocAccessRequestId.Value);
                        if (rootAdhocAccessRequest != null)
                        {
                            rootAdhocAccessRequest.IsÉxpiryHandled = true;
                            rootAdhocAccessRequest.IsÉxpiryMailSend = true;
                            rootAdhocAccessRequest.Comments += string.Format("This AccessRequestId- {0} has been Extended with new AccessRequest Id - {1}.", rootAceessRequest.Id, accessRequest.Id);
                            rootAdhocAccessRequest.LastModifiedBy = currentUser;
                            rootAdhocAccessRequest.LastModifiedDate = DateTime.Now;
                            rootAdhocAccessRequest.Save(currentUser);
                        }
                    }
                    rootAceessRequest.Remarks = string.Format("This AccessRequestId- {0} has been Extended with new AccessRequest Id - {1}.", rootAceessRequest.Id, accessRequest.Id);
                    rootAceessRequest.Comments = accessRequest.Comments;
                    rootAceessRequest.LastModifiedBy = currentUser;
                    rootAceessRequest.LastModifiedDate = DateTime.Now;

                    DILog.ProvisioningLogger.WriteLog(null, null, Logger.LogLevelType.Info, string.Format("Parent AccessRequestId- {0} has been Extended to till - {1} with Extended AccessRequest Id - {2}.", rootAceessRequest.Id, rootAceessRequest.DeactivationDate.ToString(), accessRequest.Id));

                    rootAceessRequest.Save(currentUser);
                }
            }
        }

        public static void AddOrUpdateEntitlementReconcilation(IEndpointUserInfo endPointUserInfo, AccountReconcilation accountReconcilation, string currentUser, long accessRequestId = 0, bool fromAccessRequest = false)
        {

            InhouseUserInfo oInhouse = (InhouseUserInfo)endPointUserInfo;
            foreach (InhousePermission inhPermission in oInhouse.Permissions)
            {
                EntitlementReconcilation objEnt = EntitlementReconcilation.GetByUser(accountReconcilation.EndpointUserId, accountReconcilation.EndpointSystemId, inhPermission.Name, EntitlementType.Permission);
                if (objEnt == null)
                {
                    objEnt = new EntitlementReconcilation();
                    objEnt.Id = 0;
                    objEnt.EnteredDate = DateTime.Now;
                    objEnt.CreatedDate = DateTime.Now;
                    objEnt.CreatedBy = oInhouse.CreatedBy;

                    if (fromAccessRequest)
                    {
                        var accessRequest = AccessRequest.GetByIdIncludeAdHocAccessRequest(accessRequestId);
                        if (accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                        {
                            objEnt.IsDeleted = false;
                            objEnt.DeletedDate = null;
                            objEnt.ExceptionType = ExceptionTypes.None;
                            objEnt.ProvisionedThrough = Infrastructure.Enums.ProvisionedThrough.Requested.ToString();
                            objEnt.AccessRequestId = accessRequestId;
                            objEnt.ValidToDate = accessRequest.ActivationDate;
                            objEnt.ValidToDate = accessRequest.DeactivationDate;
                        }
                    }
                    else
                    {
                        EntitlementReconcilation.UpdateException(objEnt, true);
                    }
                }
                else
                {
                    InhouseUserInfo objExtInhouse = InhouseUserInfo.GetByIdEndPointIncludingOwner(oInhouse.Id);
                    bool alreadyExists = objExtInhouse.Permissions.Any(x => x.Name == inhPermission.Name);
                    if (!alreadyExists)
                    {
                        objEnt.IsDeleted = true;
                        objEnt.DeletedDate = DateTime.Now;
                        if (fromAccessRequest)
                        {
                            if (objEnt.ExceptionType == ExceptionTypes.Removed)
                            {
                                objEnt.ResolvedBy = "System";
                                objEnt.IsResolved = true;
                                objEnt.ResolvedDate = DateTime.Now;
                                objEnt.ResolvedStatus = ResolvedStatus.Automatic;
                                objEnt.Comments = "Resolved deleted entitlement by Ammendment-Remove adhoc access request";
                            }
                            updateReconWithAccessRequestId(accessRequestId, objEnt);
                        }
                        else
                            EntitlementReconcilation.UpdateException(objEnt, false);
                    }
                    else
                    {
                        objEnt.IsDeleted = false;
                        objEnt.DeletedDate = null;
                        if (fromAccessRequest)
                        {
                            if (objEnt.ExceptionType == ExceptionTypes.Added)
                            {
                                objEnt.ResolvedBy = "System";
                                objEnt.IsResolved = true;
                                objEnt.ResolvedDate = DateTime.Now;
                                objEnt.ResolvedStatus = ResolvedStatus.Automatic;
                                objEnt.Comments = "Resolved deleted entitlement by Ammendment-Add adhoc access request";
                            }
                            updateReconWithAccessRequestId(accessRequestId, objEnt);
                        }
                        else
                            EntitlementReconcilation.UpdateException(objEnt, true);
                    }

                }
                objEnt.AccountReconcilationId = accountReconcilation.Id;
                objEnt.EndpointType = EndpointType.InhouseSystems;
                objEnt.EndpointSystemId = oInhouse.EndPointSystemId;
                objEnt.EndpointUserId = oInhouse.Id;
                objEnt.UserId = accountReconcilation.UserId;
                objEnt.EntitlementType = (int)EntitlementType.Permission;
                objEnt.AccountName = accountReconcilation.AccountName;
                objEnt.Entitlement = inhPermission.Name;
                objEnt.Save(currentUser);
            }

            List<EntitlementReconcilation> entList = EntitlementReconcilation.GetByEndpointUserId(accountReconcilation.EndpointUserId, accountReconcilation.EndpointSystemId, EntitlementType.Permission);
            //InhouseUserInfo oExtInhousePermission = InhouseUserInfo.GetByIdEndPointIncludingOwner(accountReconcilation.EndpointUserId);
            if (entList != null)
            {
                foreach (EntitlementReconcilation oent in entList)
                {
                    bool alreadyExists = oInhouse.Permissions.Any(x => x.Name == oent.Entitlement);
                    if (!alreadyExists)
                    {
                        oent.IsDeleted = true;
                        oent.DeletedDate = DateTime.Now;

                        if (fromAccessRequest)
                        {
                            if (oent.ExceptionType == ExceptionTypes.Removed)
                            {
                                oent.ResolvedBy = "System";
                                oent.IsResolved = true;
                                oent.ResolvedDate = DateTime.Now;
                                oent.ResolvedStatus = ResolvedStatus.Automatic;
                                oent.Comments = "Resolved deleted entitlement by Ammendment-Remove adhoc access request";
                            }
                            updateReconWithAccessRequestId(accessRequestId, oent);
                        }
                        else
                        {
                            EntitlementReconcilation.UpdateException(oent, false);
                        }
                        oent.Save(currentUser);

                        string cmd = string.Format("DELETE FROM [InhouseUserInfoInhousePermission] WHERE InhouseUserInfo_Id = {0} AND InhousePermission_Id = {1}", oent.EndpointUserId, oent.EntitlementId);
                        InhouseUserInfo.ExecuteQuery(cmd);
                    }

                }
            }
        }

        static void updateReconWithAccessRequestId(long accessRequestId, EntitlementReconcilation entitlement)
        {
            var accessRequest = AccessRequest.GetByIdIncludeAdHocAccessRequest(accessRequestId);
            if (accessRequest != null && accessRequest.AdHocAccessRequest != null && accessRequest.AdHocAccessRequest.RequestedItems != null)
            {
                List<PresentationAccessRequestItem> lstRequestedItems = accessRequest.AdHocAccessRequest.RequestedItems.Where(x => x.AccessRequestItemType == AccessRequestItemType.Entitlement).ToList();
                foreach (PresentationAccessRequestItem requestItem in lstRequestedItems)
                {
                    if (entitlement.Entitlement == requestItem.ItemName)
                    {
                        entitlement.ProvisionedThrough = Infrastructure.Enums.ProvisionedThrough.Requested.ToString();
                        entitlement.AccessRequestId = accessRequestId;
                        entitlement.ValidToDate = accessRequest.ActivationDate;
                        entitlement.ValidToDate = accessRequest.DeactivationDate;
                    }
                }
            }
        }

        public static IEndpointUserInfo GetEndpointUserInfo(long endPointSytemId, long endPointUserId)
        {
            IEndpointUserInfo endpointUserInfo = null;
            DomainModel.EndPoint endPoint = DomainModel.EndPoint.GetByIdIncludingEndPointSystemType(endPointSytemId);
            EndpointType endpointType = (EndpointType)endPoint.EndPointSystemType.EndpointSystemTypeName;
            endpointUserInfo = EndPointFactory.GetEndPoint(endpointType).GetUser(endPointUserId);
            return endpointUserInfo;
        }
        public static string GetEndpointUserInfoJson(long ReconId)
        {
            AccountReconcilation acctRecon = GetByIdIncludingEndPoint(ReconId);
            var iUser = GetEndpointUserInfo(acctRecon.EndpointSystemId, acctRecon.EndpointUserId);

            string str = JsonConvert.SerializeObject(iUser, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

            return str;
        }
        private static IEndpointUserInfo GetEndpointUserInfo(long ReconId)
        {
            AccountReconcilation acctRecon = GetByIdIncludingEndPoint(ReconId);

            return GetEndpointUserInfo(acctRecon.EndpointSystemId, acctRecon.EndpointUserId);
        }

        public static IList<IEndpointUserInfo> GetEndpointUserInfoByUser(long userId, AccountType accountType, long endPointSytemId)
        {
            var listAcntRecon = GetByAccountType(userId, accountType);
            return listAcntRecon.Select(accountReconcilation => GetEndpointUserInfo(accountReconcilation.Id))
                .Where(endpointUserInfo => endpointUserInfo != null).ToList().Where(t => t.EndPointSystemId == endPointSytemId).ToList();
        }
        //This returns only Primary & Secondary Account types for a user.
        public static IList<IEndpointUserInfo> GetPrimaryEndpointsByUser(long userId, long endPointSytemId)
        {
            var listAcntRecon = GetByEndpointSystemId(userId, endPointSytemId);
            listAcntRecon = listAcntRecon.Where(t => t.AccountType == AccountType.User || t.AccountType == AccountType.Secondary).ToList();
            return listAcntRecon.Select(accountReconcilation => GetEndpointUserInfo(accountReconcilation.Id))
                .Where(endpointUserInfo => endpointUserInfo != null).ToList().ToList();
        }
        public static IList<IEndpointUserInfo> GetLockedEndpointUserInfoByUser(long userId, Infrastructure.Enums.AccountType accountType, long endPointSytemId, string accountName)
        {
            IList<IEndpointUserInfo> list = new List<IEndpointUserInfo>();
            IEnumerable<AccountReconcilation> allAccounts;
            if (string.IsNullOrEmpty(accountName))
            {
                allAccounts = GetByUser(userId, true, true).Where(t => t.AccountType == accountType && t.EndpointSystemId == endPointSytemId && !t.IsDeleted);
            }
            else
            {
                allAccounts = GetByUser(userId, true, true).Where(t => t.AccountType == accountType && t.EndpointSystemId == endPointSytemId && (string.Compare(t.AccountName, accountName, true) == 0) && !t.IsDeleted);
            }

            foreach (var item in allAccounts)
            {
                var epUser = GetEndpointUserInfo(item.EndpointSystemId, item.EndpointUserId);
                list.Add(epUser);
            }
            return list;
        }

        public static IList<IEndpointUserInfo> GetActiveEndpointUserInfoByUser(long userId, Infrastructure.Enums.AccountType accountType, long endPointSytemId, string accountName)
        {
            IList<IEndpointUserInfo> list = new List<IEndpointUserInfo>();
            IEnumerable<AccountReconcilation> allAccounts;
            if (string.IsNullOrEmpty(accountName))
            {
                allAccounts = GetByUser(userId, true, false).Where(t => t.AccountType == accountType && t.EndpointSystemId == endPointSytemId && !t.IsDeleted);
            }
            else
            {
                allAccounts = GetByUser(userId, true, false).Where(t => t.AccountType == accountType && t.EndpointSystemId == endPointSytemId && !t.IsDeleted && (string.Compare(t.AccountName, accountName, true) == 0));
            }

            foreach (var item in allAccounts)
            {
                var epUser = GetEndpointUserInfo(item.EndpointSystemId, item.EndpointUserId);
                list.Add(epUser);
            }
            return list;
        }
        #endregion

        public static AccountReconcilation GetByEndpoint(InhouseUserInfo obj)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new AccountReconcilationRepository(transaction).GetByEndpoint(obj.ORUnMap()));
            }
        }

        public static List<AccountReconcilation> GetDeletedAccounts(long EndpointId, DateTime runDatetime, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                List<AccountReconcilation> listDeletedAccounts = new List<AccountReconcilation>();

                listDeletedAccounts = ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction)
                       .Get(objUser => objUser.EndpointSystemId == EndpointId && (objUser.LastModifiedDate < runDatetime
                       || (objUser.CreatedDate < runDatetime && objUser.LastModifiedDate == null))).ToList());

                return listDeletedAccounts;
            }
        }

        public static List<AccountReconcilation> GetByAccessRequestId(List<long> accessRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction("system"))
            {
                List<AccountReconcilation> listDeletedAccounts = new List<AccountReconcilation>();

                listDeletedAccounts = ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction)
                       .Get(objUser => objUser.AccessRequestId.HasValue && accessRequestId.Contains(objUser.AccessRequestId.Value),
                       true).ToList());

                return listDeletedAccounts;
            }
        }

        public static void ProcessDeletedAccounts(long EndpointId, Process objDiscoveryProcess, string currentUser, bool considerFileTimeStamp = false, DateTime? filetimeStamp = null)
        {
            List<AccountReconcilation> listDeletedAccounts = new List<AccountReconcilation>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {

                if (objDiscoveryProcess != null)
                {

                    listDeletedAccounts = ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction)
                       .Get(objUser => objUser.EndpointSystemId == EndpointId &&
                                       ((objUser.LastModifiedDate != null && objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt) ||
                                           (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                                       )).ToList());

                }
            }

            foreach (var item in listDeletedAccounts)
            {
                if (!item.IsDeleted || !item.IsLocked)
                {
                    UpdateException(item, false);
                    item.Save(currentUser);
                }
            }
        }
        public static List<AccountReconcilation> GetProcessDeletedAccounts(long EndpointId, Process objDiscoveryProcess, string currentUser, bool considerFileTimeStamp = false, DateTime? filetimeStamp = null)
        {
            List<AccountReconcilation> listDeletedAccounts = new List<AccountReconcilation>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                if (objDiscoveryProcess != null)
                {

                    listDeletedAccounts = ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction)
                       .Get(objUser => objUser.EndpointSystemId == EndpointId &&
                                       ((objUser.LastModifiedDate != null && objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt) ||
                                           (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                                       )).ToList());

                }
            }
            return listDeletedAccounts;
        }

        public static List<AccountReconcilation> GetNonSeedAccounts(long userId, long endpointSystemId)
        {
            var ac = GetByEndpointSystemId(userId, endpointSystemId);

            return ac.Where(t => t.ProvisionedThrough != DI.Chakra.Infrastructure.Enums.ProvisionedThrough.Seed.ToString()).ToList();
        }

        public static List<AccountReconcilation> GetRequestedAccounts(long userId, long endpointSystemId)
        {
            var ac = GetByEndpointSystemId(userId, endpointSystemId);

            return ac.Where(t => t.ProvisionedThrough == DI.Chakra.Infrastructure.Enums.ProvisionedThrough.Requested.ToString()).ToList();
        }

        public static List<AccountReconcilation> GetSeedAccounts(long userId, long endpointSystemId)
        {
            var ac = GetByEndpointSystemId(userId, endpointSystemId);

            return ac.Where(t => t.ProvisionedThrough == DI.Chakra.Infrastructure.Enums.ProvisionedThrough.Seed.ToString()).ToList();
        }
        //Switch case revamped
        //public static void SaveAccount(AccountReconcilation accountReconcilation, string currentUser)
        //{
        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
        //    {
        //        accountReconcilation.Save(dataAccessTransaction);
        //        EndpointType type = accountReconcilation.EndPoint.EndPointSystemType.EndpointSystemTypeName;
        //        EndPointFactory.GetEndPoint(type).SaveUserInfo(accountReconcilation.EndpointUserId, accountReconcilation.EndpointSystemId, currentUser);
        //    }
        //}

        public static void SaveAttributeException(IEndpointUserInfo existingUserObject, IEndpointUserInfo user, long? endpointTypeId, DateTime? whenCreated, string sAMAccountName, string currentUser)
        {
            List<ExceptionAttributeChange> temp = new List<ExceptionAttributeChange>();
            bool flag = false;
            var oType = existingUserObject.GetType();
            foreach (var oProperty in oType.GetProperties())
            {
                var oOldValue = oProperty.GetValue(existingUserObject, null);
                var oNewValue = oProperty.GetValue(user, null);

                if (!object.Equals(oOldValue, oNewValue))
                {
                    string oldvalue = oOldValue == null ? "null" : oOldValue.ToString();
                    string newvalue = oNewValue == null ? "null" : oNewValue.ToString();
                    string property = oProperty.Name;

                    if (endpointTypeId != 0)
                    {
                        List<ExceptionAttribute> getByEndpointTypeId = ExceptionAttribute.GetByEndpointTypeId((long)endpointTypeId);

                        var res = getByEndpointTypeId.Find(x => x.Attributes == property);
                        if (res != null)
                        {
                            flag = true;
                            ExceptionAttributeChange tempData = new ExceptionAttributeChange();
                            tempData.Attributes = property;
                            tempData.OldValue = oldvalue;
                            tempData.NewValue = newvalue;
                            temp.Add(tempData);
                        }
                    }
                }
            }
            if (flag)
            {
                /*insert userdetail with exceptiontype 'attribute'*/
                AccountReconcilation accountReconcilation = AccountReconcilation.GetByUser(user.EndPointSystemId, existingUserObject.Id, ExceptionTypes.Attribute);
                if (accountReconcilation == null)
                    accountReconcilation = new AccountReconcilation();

                accountReconcilation.ExceptionType = ExceptionTypes.Attribute;
                accountReconcilation.ExceptionOccuredDate = DateTime.Now;
                accountReconcilation.EndpointSystemId = user.EndPointSystemId;
                accountReconcilation.EndpointUserId = existingUserObject.Id;
                if (whenCreated != null)
                    accountReconcilation.AccountCreatedDate = (DateTime)whenCreated;
                accountReconcilation.FullName = user.DisplayName;
                accountReconcilation.AccountName = sAMAccountName;
                accountReconcilation.LastModifiedDate = DateTime.Now;
                //accountReconcilation.UserId = user.DIUserId;
                accountReconcilation.IsDeleted = false;
                accountReconcilation.IsResolved = false;
                //accountReconcilation.IsLocked = user.IsLocked;
                //accountReconcilation.LockScenario = user.LockScenario;
                accountReconcilation.Save(currentUser);

                foreach (var acct in temp)
                {
                    long exceptionAttributeId = ExceptionAttribute.GetByEndpointTypeIdandAttribute((long)endpointTypeId, acct.Attributes).Id;
                    ExceptionAttributeChange exceptionAttributeChange = ExceptionAttributeChange.GetExceptionAttributes(accountReconcilation.Id, exceptionAttributeId);
                    if (exceptionAttributeChange == null)
                        exceptionAttributeChange = new ExceptionAttributeChange();
                    exceptionAttributeChange.ReconcilationId = accountReconcilation.Id;
                    exceptionAttributeChange.ExceptionAttributeId = exceptionAttributeId;
                    exceptionAttributeChange.Attributes = acct.Attributes;
                    exceptionAttributeChange.OldValue = acct.OldValue;
                    exceptionAttributeChange.NewValue = acct.NewValue;
                    exceptionAttributeChange.Save(currentUser);
                }
            }
        }

        public static List<AccountReconcilation> GetSeedAccountsUsingProvisionThrough(long userId, string provisionedThrough)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(user => user.UserId == userId && user.ProvisionedThrough == provisionedThrough, user => user.EndPoint).ToList());
            }
        }


        public static AccountReconcilation GetByEndpointUserIdEagerLoad(long endpointUserId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointUserId == endpointUserId, true).FirstOrDefault());
            }
        }

        public static List<AccountReconcilation> GetAllByEndpointIdAndAccountType(long endpointId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointSystemId == endpointId && ar.AccountType == accountType).ToList());
            }
        }

        public static List<AccountReconcilation> GetAllByEndpointId(long endpointId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointSystemId == endpointId).ToList());
            }
        }

        public static AccountReconcilation GetByEndpointUserId(long endpointUserId, long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointUserId == endpointUserId && ar.EndpointSystemId == endpointSystemId).FirstOrDefault());
            }
        }

        public static Dictionary<long, long?> GetAllAccountByEndpointSystemId(long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                Dictionary<long, long?> retValue = new AccountReconcilationRepository(transaction).GetAllAccountReconcilationByEndpointSystemId(endpointSystemId, accountType);
                return retValue;
            }
        }

        public static string CheckForDuplicateRequest(List<long> userIds, long endpointSystemId, ApplicationType applicationType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return new AccountReconcilationRepository(transaction).CheckDuplicateRequestExist(userIds, endpointSystemId, applicationType);
            }
        }
        public static Dictionary<long, string> GetByUserIdsDoesNotExist(List<long> userIds, EndpointType EndpointSystemType, long endpointSystemId, AccountType accountType, ApplicationType applicationType, bool isLocked, DI.Chakra.Infrastructure.Enums.ActionType actionType, bool considerLock)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                Dictionary<long, string> retValue = new AccountReconcilationRepository(transaction).GetByUserIdsDoesNotExist(userIds, EndpointSystemType, endpointSystemId, accountType, applicationType, isLocked, actionType, considerLock);




                return retValue;
            }
        }

        public static List<AccountReconcilation> GetAllByEndpointIdAndEndpointUserId(long endpointId, long endpointUserId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointSystemId == endpointId && ar.EndpointUserId == endpointUserId).ToList());
            }
        }
        public static AccountReconcilation GetActiveByEndpointIdAndEndpointUserId(long endpointId, long endpointUserId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointSystemId == endpointId && ar.EndpointUserId == endpointUserId && ar.IsDeleted == false).FirstOrDefault());
            }
        }

        public static AccountReconcilation GetByEndpointIdAndEndpointUserId(long endpointId, long endpointUserId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointSystemId == endpointId && ar.EndpointUserId == endpointUserId && ar.IsDeleted == false && ar.IsLocked == false).FirstOrDefault());
            }
        }

        public static AccountReconcilation GetMappedAccReconByEpuserId(long endpointId, long endpointUserId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointSystemId == endpointId && ar.EndpointUserId == endpointUserId && ar.IsDeleted == false && ar.IsLocked == false && ar.UserId.HasValue).FirstOrDefault());
            }
        }

        public static AccountReconcilation GetByEndpointIdAndEndpointUserId(long endpointId, long endpointUserId, long userId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointSystemId == endpointId && ar.EndpointUserId == endpointUserId && ar.UserId == userId).FirstOrDefault());
            }
        }
        public static AccountReconcilation GetByEndpointIdActiveAndEndpointUserId(long endpointId, long endpointUserId, long userId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointSystemId == endpointId && ar.EndpointUserId == endpointUserId && ar.UserId == userId && ar.IsDeleted == false && ar.IsLocked == false).FirstOrDefault());
            }
        }
        public static AccountReconcilation GetByEndpointIdActiveAndEndpointUserId(long endpointId, long endpointUserId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointSystemId == endpointId && ar.EndpointUserId == endpointUserId && ar.IsDeleted == false && ar.IsLocked == false).FirstOrDefault());
            }
        }
        public static AccountReconcilation GetByEndpointIdAndEndpointUserAccountName(long endpointId, string accountname, long userId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointSystemId == endpointId && ar.AccountName == accountname && ar.UserId == userId).FirstOrDefault());
            }
        }
        public static List<AccountReconcilation> GetAllActiveByEndpointId(long endpointId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointSystemId == endpointId && ar.IsDeleted == false && ar.IsLocked == false).ToList());
            }
        }

        public static List<AccountReconcilation> GetAllActiveByEndpointIdByAccountType(long endpointId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointSystemId == endpointId && ar.IsDeleted == false && ar.IsLocked == false && ar.AccountType == accountType && ar.UserId.HasValue).ToList());
            }
        }
        public static List<AccountReconcilation> GetByMultipleEndpoints(List<long> endpoints, AccountType accounttype)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => endpoints.Contains(ar.EndpointSystemId) && ar.IsDeleted == false && ar.IsLocked == false && ar.AccountType == accounttype).ToList());
            }
        }
        public static List<AccountReconcilation> GetByMultipleEndpointsByAccountType(List<long> endpoints, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => endpoints.Contains(ar.EndpointSystemId) && ar.AccountType == accountType).ToList());
            }
        }
        public static AccountReconcilation GetByEndpointWithAccountType(long endpointUserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointUserId == endpointUserId && ar.EndpointSystemId == endpointSystemId && ar.AccountType == accountType && ar.IsDeleted == false).FirstOrDefault());
            }
        }
        public static AccountReconcilation GetByDIUserIdByAccountType(long userId, long endpointSystemId)
        {
            var accountType = DI.Chakra.Infrastructure.Enums.AccountType.User;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).
                    Get(recon => recon.AccountType == accountType && recon.UserId == userId && recon.EndpointSystemId == endpointSystemId && recon.IsDeleted == false && recon.IsLocked != true).FirstOrDefault());
            }
        }

        public static List<AccountReconcilation> GetActiveAccountRecons(long diUserId, long endpointSystemId, AccountType accountType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new AccountReconcilationRepository(transaction).GetActiveAccountRecons(diUserId, endpointSystemId, accountType));
            }
        }

        public static long? GetActiveAccountRecon(string accountName, ApplicationType applicationType)
        {
            long? userId = null;

            using (var transaction = new DataAccessTransaction(false))
            {
                var accountRecon = new RepositoryBase<EntityModel.AccountReconcilation>(transaction).Get(ar => ar.AccountName.ToLower() == accountName.ToLower() && !ar.IsLocked, ar => ar.EndPoint.EndPointSystemType).Where(t => t.EndPoint.ApplicationType == applicationType).FirstOrDefault();

                userId = accountRecon != null ? accountRecon.UserId : null;
            }

            return userId;
        }

        public static List<AccountReconcilation> GetAccountRecons(List<long> endpointIds, DateTime uploadedTime)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction)
                                   .Get(objUser => endpointIds.Contains(objUser.EndpointSystemId) &&
                                                   (objUser.LastModifiedDate < uploadedTime ||
                                                       (objUser.CreatedDate < uploadedTime && objUser.LastModifiedDate == null)
                                                   )).ToList());
            }
        }

        public static void UpdateValidTo(List<long> endpointIds)
        {
            string accountReconQuery = string.Format(@"UPDATE accRecon
                                        SET accRecon.ValidToDate = CASE WHEN u.isactive=1 and (u.FullTimeEmployee = 1 OR u.TerminationDate IS NULL OR accRecon.AccountType IN(2,3)) THEN CAST('9999-12-31 23:59:59.998' AS DATETIME)
							            ELSE u.TerminationDate END
                                        FROM DI_M_AccountReconcilation accRecon
                                        INNER JOIN DI_M_User u ON accRecon.UserId = u.Id
                                        WHERE accRecon.ValidToDate IS NULL AND accRecon.endpointsystemid in ({0})", string.Join(",", endpointIds));
            string entReconQuery = string.Format(@"UPDATE entRecon
                                        SET entRecon.ValidToDate = accRecon.ValidToDate
                                        FROM DI_M_EntitlementReconcilation entRecon
                                        JOIN DI_M_AccountReconcilation accRecon ON entRecon.AccountReconcilationId = accRecon.Id
                                        WHERE entRecon.ValidToDate IS NULL AND entRecon.EndpointSystemId IN ({0})", string.Join(",", endpointIds));

            ExecuteQuery(accountReconQuery);
            ExecuteQuery(entReconQuery);
        }

        public static string GetCanConsiderInActiveFilterCondition()
        {
            return MasterConfigHelper.ConsiderInActiveAccountForReconcilation == false ? " DI_M_User.IsActive  = 1 " : " 1 = 1 ";
        }



        public static List<AccountReconcilation> GetUserByActiveEBSAccountRecon(string accountName)
        { 
            ApplicationType[] applicationTypes = new[] { ApplicationType.EBS_ISU, ApplicationType.EBS_CRM, ApplicationType.EBS };
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList( new RepositoryBase<EntityModel.AccountReconcilation>(transaction).Get(ar => ar.AccountType== AccountType.User 
                && ar.AccountName.Equals( accountName, StringComparison.InvariantCultureIgnoreCase) 
                , ar => ar.EndPoint.EndPointSystemType).Where(t => applicationTypes.Contains(t.EndPoint.ApplicationType)  ).Select(t=> t).ToList()) ; 
            } 
        }
        public static AccountReconcilation GetActiveAccountByAccountName(string accountName, long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                Get(ar => ar.IsDeleted == false && ar.AccountName.Trim().ToUpper() == accountName.Trim().ToUpper() && ar.EndpointSystemId == endpointSystemId).OrderBy(i => i.CreatedDate).FirstOrDefault());
            }
                
        }
        public static List<AccountReconcilation> GetByEndpointSystemIdForAttestation(long endpointSystemId, bool includeUnMapped, ApplicationType applicationType)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                List<AccountReconcilation> accounts = new List<AccountReconcilation>();
                if (includeUnMapped)
                {
                    accounts = ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).Get(ar =>
                    ar.EndpointSystemId == endpointSystemId
                    && ar.IsDeleted == false
                    && ar.UserId == null,
                    true).ToList());
                    
                }
                else
                {
                    accounts = ORMapList(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).Get(ar => ar.UserId != null && ar.EndpointSystemId == endpointSystemId && ar.IsDeleted == false && (ar.AccountType == AccountType.User || ar.AccountType == AccountType.Shared ||
             ar.AccountType == AccountType.Secondary || ar.AccountType == AccountType.Service
             ), ar => ar.EndPoint.EndPointSystemType).ToList());
                }
                if(applicationType == ApplicationType.NPAC)
                {
                    //try
                    //{
                        
                    //}
                    //catch
                    //{
                    //}
                }
                else if(applicationType == ApplicationType.IDMS)
                {
                    var excludeList = MasterConfigHelper.IDMSAttestationRoleToExclude;
                    var accountids = new AccountReconcilationRepository(transaction).GetIDMSAccountOnlyForAttestation(applicationType, endpointSystemId, includeUnMapped, excludeList);
                    accounts = accounts.Where(i=> accountids.Contains(i.Id)).ToList();
                    accountids = null;
                }
                return accounts;
            }
        }

        public static void SendEmailForPasswordResetFailure(long diUserId, string applicationName,string pwdString,string currentUser="System")
        {
            try
            {
                var usr = User.GetById(diUserId);
                const string tempName = "SendPasswordForSelfServiceResetFailure";
                EmailInfo emailInfo = new EmailInfo();
                MailTemplate template = MailTemplate.GetByTemplateName(tempName);
                
                StringBuilder subject = new StringBuilder(template.MailSubject);

                subject.Replace("@@APPLICATIONNAME@@", applicationName);

                StringBuilder body = new StringBuilder(template.MailContent);
                body.Replace("@@USERDISPLAYNAME@@", usr.DisplayName);
                body.Replace("@@APPLICATIONNAME@@", applicationName);
                body.Replace("@@PASSWORD@@", pwdString);
                body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                emailInfo.Body = body.ToString();
                emailInfo.Subject = subject.ToString();
                emailInfo.To = usr.EmailId;
                emailInfo.From = template.MailFrom;
                emailInfo.ActivityType = EmailActivityType.Provisioning.ToString();
                emailInfo.Save(currentUser);
            }
            catch (Exception ex)
            {
            }
        }
        public static bool UpdateOneKeyTokenForUserThroughAdminUI(long userId, long endpointSystemId, string oldToken, string newToken, string updatedBy="System")
        {
            try
            {
                using (var transaction = new DataAccessTransaction())
                {
                    // newToken/oldToken are attacker-influenced strings, so they are bound as real
                    // SqlParameters ("{0}"/"{1}" are EF tokens, not string interpolation/format
                    // substitutions) instead of being concatenated into the SQL text. userId/
                    // endpointSystemId/vpnAccountRecon.Id are longs, safe to interpolate directly.
                    if (string.IsNullOrWhiteSpace(newToken))
                    {
                        newToken = null;
                        ExecuteQuery($"Update DI_M_User set OneKeyTokenId=null where Id = {userId};");
                    }
                    else
                    {
                        ExecuteQuery($"Update DI_M_User set OneKeyTokenId={{0}} where Id = {userId};", newToken);
                    }

                    if (!string.IsNullOrWhiteSpace(oldToken) && !string.IsNullOrWhiteSpace(newToken))
                    {
                        var vpnAccountRecon = AccountReconcilation.GetActiveAccountByAccountName(oldToken, endpointSystemId);
                        if (vpnAccountRecon != null && vpnAccountRecon.UserId == userId)
                        {
                            vpnAccountRecon.AccountName = newToken;
                            vpnAccountRecon.Save(updatedBy);
                            ExecuteQuery($"Update DI_M_EntitlementReconcilation Set AccountName = {{0}} where EndpointSystemId={endpointSystemId} and AccountName={{1}} and AccountReconcilationId ={vpnAccountRecon.Id}  and UserId={userId} and IsDeleted = 0", newToken, oldToken);
                            var vpnUsr = VPNUserInfo.GetByOnekeyTokenId(oldToken);
                            if(vpnUsr != null && vpnUsr.UserId == userId)
                            {
                                vpnUsr.OneKeyTokenId = newToken;
                                vpnUsr.Save(updatedBy);
                            }
                        }
                    }
                    
                    return true;
                    //return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    //    Get(ar => ar.EndpointUserId == endpointUserId && ar.EndpointSystemId == endpointSystemId && ar.AccountType == accountType && ar.IsDeleted == false).FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsEmailIdAlreadyExists(long endpointId, string emailId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                //Ticket 3011
                var isExists = new RepositoryBase<EntityModel.AccountReconcilation>(transaction).Any(ar => ar.EndpointSystemId == endpointId && ar.AccountName == emailId);
                if(isExists == false)
                {
                    isExists = new RepositoryBase<EntityModel.User>(transaction).Any(ar => ar.EmailId == emailId);
                }
                return isExists;
            }
        }

        public static AccountReconcilation GetActiveByEndpointIdAndAccountName(long endpointId, string accountName)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.AccountReconcilation>(transaction).
                    Get(ar => ar.EndpointSystemId == endpointId && ar.AccountName == accountName && ar.IsDeleted == false).FirstOrDefault());
            }
        }
    }
}
