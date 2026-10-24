using DI.Chakra.DataAccess;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using DI.Chakra.Model.ReportData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Facade
{
    public class ReconcilationFacade
    {
        ReconcilationDA _reconcilationDA = new ReconcilationDA();

        public long CreateAccountReconcilationRecord(AccountReconcilation acctRecon)
        {
            try
            {
                return _reconcilationDA.CreateAccountReconcilationRecord(acctRecon);
            }
            catch
            {
                throw;
            }
        }
        public void DeleteAccount(long id)
        {
            _reconcilationDA.DeleteReconAccount(id);
        }
        public void DeleteAccount(long EndpointSystemId, long EndpointUserId)
        {
            AccountReconcilation recon = GetAccountReconcilationRecordByUser(EndpointSystemId, EndpointUserId);

            if (recon == null)
                return;

            DeleteAccount(recon.Id);
        }
        public void LockAccount(long EndpointSystemId, long EndpointUserId, LockScenario lockScenario)
        {
            AccountReconcilation recon = GetAccountReconcilationRecordByUser(EndpointSystemId, EndpointUserId);

            if (recon == null)
                return;

            recon.IsLocked = true;
            recon.LockScenario = lockScenario;
            AddOrUpdateAccountInReconTable(recon);
        }
        public void UnLockAccount(int EndpointSystemId, long EndpointUserId)
        {
            AccountReconcilation recon = GetAccountReconcilationRecordByUser(EndpointSystemId, EndpointUserId);

            if (recon == null)
                return;

            recon.IsLocked = false;
            recon.LockScenario = LockScenario.None;
            AddOrUpdateAccountInReconTable(recon);
        }
        public void AddOrUpdateAccountInReconTable(AccountReconcilation acctRecon)
        {
            _reconcilationDA.AddOrUpdateAccountInReconTable(acctRecon);  
        }
        public void AddOrUpdateAccountInReconTable(IEndpointUserInfo endpointUserInfo, bool IsNewlyAdded)
        {

            AccountReconcilation acctRecon =
                GetAccountReconcilationRecordByUser(endpointUserInfo.EndPointSystemId,
                    endpointUserInfo.Id);

            if (acctRecon == null)
            {
                acctRecon = new AccountReconcilation();
            }
            acctRecon.FullName = endpointUserInfo.DisplayName;
            acctRecon.AccountType = endpointUserInfo.AccountType;
            acctRecon.AccountName = endpointUserInfo.AccountName;
            acctRecon.EndpointSystemId = endpointUserInfo.EndPointSystemId;
            acctRecon.EndpointUserId = endpointUserInfo.Id;
            //acctRecon.Version = VersionNo.Available;
            acctRecon.IsDeleted = false;
            acctRecon.ExceptionType = IsNewlyAdded ? ExceptionTypes.Added : acctRecon.ExceptionType;
            AddOrUpdateAccountInReconTable(acctRecon);
        }

        public IList<AccountReconcilation> GetLockedAccountsByUser(int userId)
        {
            var listAcntRecon = GetLockedAccountReconcilations(userId);

            return listAcntRecon.Where(t => t.IsLocked == true).ToList();

        }
        public IList<AccountReconcilation> GetLockedAccountsByUser(long userId, long endpointSystemId)
        {
            return GetLockedAccountReconcilations(userId,endpointSystemId);
        }
        public IList<IEndpointUserInfo> GetLockedEndpointUserInfoByUser(long userId, AccountType accountType, long endpointSystemId)
        {
            IList<IEndpointUserInfo> list = new List<IEndpointUserInfo>();

            var accounts = GetLockedAccountsByUser(userId, endpointSystemId).Where(t => t.AccountType == accountType).ToList();

            foreach (var item in accounts)
            {
                var epUser = this.GetEndpointUserInfo(item.EndpointSystemId, item.EndpointUserId);

                list.Add(epUser);
            }

            return list;
        }
        public IList<IEndpointUserInfo> GetEndpointUserInfoByUser(long userId, int endpointSystemId)
        {
            var accounts = GetEndpointUserInfoByUser(userId);

            return accounts.Where(t => t.EndPointSystemId == endpointSystemId).ToList();
        }
        
        public IList<IEndpointUserInfo> GetEndpointUserInfoByUser(long userId, AccountType accountType, int endpointSystemId)
        {
            var accounts = GetEndpointUserInfoByUser(userId, accountType);

            return accounts.Where(t => t.EndPointSystemId == endpointSystemId).ToList();
        }
        public IList<IEndpointUserInfo> GetEndpointUserInfoByUser(long userId, AccountType accountType)
        {
            var listAcntRecon = GetAccountReconcilations(userId,accountType);

            return GetEndpointUserInfoFromAccountRecon(listAcntRecon);
        }

        public IList<IEndpointUserInfo> GetEndpointUserInfoByUser(long userId)
        {
           var listAcntRecon = GetAccountReconcilations(userId);

           return GetEndpointUserInfoFromAccountRecon(listAcntRecon);

        }


        public IList<IEndpointUserInfo> GetEndpointUserInfoFromAccountRecon(IList<AccountReconcilation> listAccountRecon)
        {
            return listAccountRecon.Select(accountReconcilation => GetEndpointUserInfo(accountReconcilation.Id))
                .Where(endpointUserInfo => endpointUserInfo != null).ToList();
        }
        public IEndpointUserInfo  GetEndpointUserInfo(long ReconId)
        {
            AccountReconcilation acctRecon = GetAccountReconcilationRecordById(ReconId);

            return GetEndpointUserInfo(acctRecon.EndpointSystemId, acctRecon.EndpointUserId);
        }
        public IEndpointUserInfo GetEndpointUserInfo(long EndpointSystemId, long EndpointUserId)
        {

            var Endpoint = new EndPointFacade().GetEndPointSystemByKey(EndpointSystemId);

            if (Endpoint == null)
                return null;

            IEndpointFacade epFacade = GetEndpointFacade(Endpoint.EndPointSystemType.EndpointSystemTypeName);

            return epFacade.GetEndpointUserInfo(EndpointUserId);
        }

        public void SaveReconcilationRecord(AccountReconcilation _accountReconcilation)
        {
            try
            {
                _reconcilationDA.SaveReconcilationRecord(_accountReconcilation);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveEntitlementReconcilationRecord(EntitlementReconcilation _entitlementReconcilation)
        {
            try
            {
                _reconcilationDA.SaveEntitlementReconcilationRecord(_entitlementReconcilation);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void AddReconcilationRecord(AccountReconcilation _accountReconcilation)
        {
            try
            {
                _reconcilationDA.AddReconcilationRecord(_accountReconcilation);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateAccountReconcilationRecord(AccountReconcilation acctRecon)
        {
            try
            {
                _reconcilationDA.UpdateAccountReconcilationRecord(acctRecon);
            }
            catch
            {
                throw;
            }
        }
        public void AddEntitlementReconcilationRecord(EntitlementReconcilation _entitlementReconcilation)
        {
            try
            {
                _reconcilationDA.AddEntitlementReconcilationRecord(_entitlementReconcilation);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void UpdateEntitlementReconcilationRecord(EntitlementReconcilation entRecon)
        {
            try
            {
                _reconcilationDA.UpdateEntitlementReconcilationRecord(entRecon);
            }
            catch
            {
                throw;
            }
        }
        public void CreateEntitlementReconcilationRecord(EntitlementReconcilation entRecon)
        {
            try
            {
                _reconcilationDA.CreateEntitlementReconcilationRecord(entRecon);
            }
            catch
            {
                throw;
            }
        }
        

        public AccountReconcilation GetAccountReconcilationRecordById(long acctReconId)
        {
            try
            {
                return _reconcilationDA.GetAccountReconcilationRecordById(acctReconId);
            }
            catch
            {
                throw;
            }
        }

        public EntitlementReconcilation GetEntitlementReconcilationRecordById(long entReconId)
        {
            try
            {
                return _reconcilationDA.GetEntitlementReconcilationRecordById(entReconId);
            }
            catch
            {
                throw;
            }
        }
        public IList<AccountReconcilation> GetAccountReconcilationsBySystemId(int systemId)
        {
            return _reconcilationDA.GetAccountReconcilationsBySystemId(systemId);
        }
        public IList<AccountReconcilation> GetAccountReconcilations(long userId, AccountType accountType, int endpointSystemId)
        {
            var allAccounts = GetAccountReconcilations(userId, endpointSystemId);

            return allAccounts.Where(t => t.AccountType == accountType).ToList();
        }
        public IList<AccountReconcilation> GetAccountReconcilations(long userId, AccountType accountType)
        {
           var allAccounts = GetAccountReconcilations(userId);

           return allAccounts.Where(t => t.AccountType == accountType).ToList();
        }
        public IList<AccountReconcilation> GetAccountReconcilations(long userId, int endpointSystemId)
        {
            var allAccounts = GetAccountReconcilations(userId);
            var returnValue = allAccounts.Where(t => t.EndpointSystemId == endpointSystemId).ToList();
            return returnValue;
        }
        public IList<AccountReconcilation> GetAccountReconcilations(long userId)
        {
            return _reconcilationDA.GetAccountReconcilationByUser(userId);
        }


        public IList<AccountReconcilation> GetLockedAccountReconcilationsBySystemId(int systemId)
        {
            return _reconcilationDA.GetAccountReconcilationsBySystemId(systemId,true);
        }
        public IList<AccountReconcilation> GetLockedAccountReconcilations(long userId, AccountType accountType, long endpointSystemId)
        {
            var allAccounts = GetLockedAccountReconcilations(userId, endpointSystemId);

            return allAccounts.Where(t => t.AccountType == accountType).ToList();
        }
        public IList<AccountReconcilation> GetLockedAccountReconcilations(int userId, AccountType accountType)
        {
            var allAccounts = GetLockedAccountReconcilations(userId);

            return allAccounts.Where(t => t.AccountType == accountType).ToList();
        }
        public IList<AccountReconcilation> GetLockedAccountReconcilations(long userId, long endpointSystemId)
        {
            var allAccounts = GetLockedAccountReconcilations(userId);

            return allAccounts.Where(t => t.EndpointSystemId == endpointSystemId).ToList();
        }
        public IList<AccountReconcilation> GetLockedAccountReconcilations(long userId)
        {
            return _reconcilationDA.GetAccountReconcilationByUser(userId,true);
        }



        public IList<EntitlementReconcilation> GetEntitlementReconcilations(long userId, AccountType accountType, int endpointSystemId)
        {
            IList<EntitlementReconcilation> entList = new List<EntitlementReconcilation>();

            var allAccounts = GetAccountReconcilations(userId, endpointSystemId)
                .Where(t=>t.AccountType == accountType).ToList();

            foreach (var item in allAccounts)
            {
                IEndpointFacade epFacade = GetEndpointFacade(item.EndPoint.EndPointSystemType.EndpointSystemTypeName);

                var entRecon = epFacade.GetEndpointUserInfo(item.EndpointUserId).Entitlements
                    .Select(t=> new EntitlementReconcilation
                    {
                        EndpointSystemId = t.EndPointSystemId,
                        EndpointUserId = item.EndpointUserId,
                        Entitlement = t.EntitlementName,
                    }).ToList();

                entList = entList.Union(entRecon).ToList();
            }

            return entList;
        }
        public IList<EntitlementReconcilation> GetEntitlementReconcilations(long userId, AccountType accountType)
        {
            IList<EntitlementReconcilation> entList = new List<EntitlementReconcilation>();

            var allAccounts = GetAccountReconcilations(userId,accountType)
                .Where(t => t.AccountType == accountType).ToList();

            foreach (var item in allAccounts)
            {
                IEndpointFacade epFacade = GetEndpointFacade(item.EndPoint.EndPointSystemType.EndpointSystemTypeName);

                var entRecon = epFacade.GetEndpointUserInfo(item.EndpointUserId).Entitlements
                    .Select(t => new EntitlementReconcilation
                    {
                        EndpointSystemId = t.EndPointSystemId,
                        EndpointType = item.EndPoint.EndPointSystemType.EndpointSystemTypeName,
                        EndpointUserId = item.EndpointUserId,
                        Entitlement = t.EntitlementName,
                    }).ToList();

                entList = entList.Union(entRecon).ToList();
            }

            return entList;
        }

        public AccountReconcilation GetAccountReconcilationRecordByUser(long endpointSystemId, long endpointUserId)
        {
            try
            {
                return _reconcilationDA.GetAccountReconcilationRecordByUser(endpointSystemId, endpointUserId);
            }
            catch
            {
                throw;
            }
        }

        public EntitlementReconcilation GetEntitlementReconcilationRecordByUser(long endpointSystemId, long endpointUserId, string entitlement, EntitlementType entitlementType)
        {
            try
            {
                return _reconcilationDA.GetEntitlementReconcilationRecordByUser(endpointSystemId, endpointUserId, entitlement, entitlementType);
            }
            catch
            {
                throw;
            }
        }

        public void RemoveAllEntitlementsByUser(long userId, long endpointSystemId, long endpointUserId)
        {
            try
            {
                var entRecons = _reconcilationDA.GetEntitlementReconcilationsByUser(userId, endpointSystemId, endpointUserId);

                foreach (EntitlementReconcilation entRecon in entRecons)
                {
                    entRecon.IsDeleted = true;
                    entRecon.DeletedDate = DateTime.Now;

                    _reconcilationDA.UpdateEntitlementReconcilationRecord(entRecon);
                }
            }
            catch
            {
                throw;
            }
        }

        public void CreateOUChangeSnapshot(OUChangeSnapshot snapshot)
        {
            try
            {
                _reconcilationDA.CreateOUChangeSnapshot(snapshot);
            }
            catch
            {
                throw;
            }
        }

        public void ResolveAllExceptionsForFirstTimeDiscovery(EndPoint _endpoint)
        {
            try
            {
                _reconcilationDA.ResolveAllExceptionsForFirstTimeDiscovery(_endpoint);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ReconcilationExceptionReport> GetAccountReconcilationExceptions(int _EndpointSystemId, ExceptionTypes exceptionType)
        {
            try
            {
                if (exceptionType == ExceptionTypes.Added)
                {
                    return _reconcilationDA.GetAccountReconcilationAddedExceptions(_EndpointSystemId);
                }
                else
                {
                    return _reconcilationDA.GetAccountReconcilationRemovedExceptions(_EndpointSystemId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ReconcilationExceptionForEntitlmentsReport> GetEntitlementReconcilationExceptions(int _EndpointSystemId, ExceptionTypes exceptionType)
        {
            try
            {
                if (exceptionType == ExceptionTypes.Added)
                {
                    return _reconcilationDA.GetEntitlementReconcilationAddedExceptions(_EndpointSystemId);
                }
                else
                {
                    return _reconcilationDA.GetEntitlementReconcilationRemovedExceptions(_EndpointSystemId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<OUTransferSnapshotGridData> GetOUChangeSnapshotByUser(List<int> actionUserIds)
        {
            try
            {
                return _reconcilationDA.GetOUChangeSnapshotByUser(actionUserIds);
            }
            catch
            {
                throw;
            }
        }

        public List<AccountReconciliationReport> GetResolvedAccountExceptions()
        {
            try
            {
                return _reconcilationDA.GetResolvedAccountExceptions();
            }
            catch
            {
                throw;
            }
        }

        public List<EntitlementReconciliationReport> GetResolvedEntitlementExceptions()
        {
            try
            {
                return _reconcilationDA.GetResolvedEntitlementExceptions();
            }
            catch
            {
                throw;
            }
        }

        public static IEndpointFacade GetEndpointFacade(EndpointType endpointType)
        {

            IEndpointFacade endpointFacade = null;

            switch (endpointType)
            {
                    case EndpointType.ActiveDirectory:
                    endpointFacade = new ActiveDirectoryFacade();
                    break;

                    case EndpointType.SAP:
                    endpointFacade = new SAPFacade();
                    break;

                    case EndpointType.LotusNotes:
                    endpointFacade = new LotusNotesFacade();
                    break;

                default:
                    break;
            }
            return endpointFacade;
        }

    }
}
