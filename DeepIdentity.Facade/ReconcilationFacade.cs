using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using DeepIdentity.Model.ReportData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeepIdentity.Facade
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
        public void DeleteAccount(int EndpointSystemId, int EndpointUserId)
        {
            AccountReconcilation recon = GetAccountReconcilationRecordByUser(EndpointSystemId, EndpointUserId);

            if (recon == null)
                return;

            DeleteAccount(recon.AccountReconcilationId);
        }
        public void LockAccount(int EndpointSystemId, int EndpointUserId, LockScenario lockScenario)
        {
            AccountReconcilation recon = GetAccountReconcilationRecordByUser(EndpointSystemId, EndpointUserId);

            if (recon == null)
                return;

            recon.IsLocked = true;
            recon.LockScenario = lockScenario;
            AddOrUpdateAccountInReconTable(recon);
        }
        public void UnLockAccount(int EndpointSystemId, int EndpointUserId)
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
                    endpointUserInfo.UserId);

            if (acctRecon == null)
            {
                acctRecon = new AccountReconcilation();
            }
            acctRecon.FullName = endpointUserInfo.DisplayName;
            acctRecon.AccountName = endpointUserInfo.AccountName;
            acctRecon.EndpointSystemId = endpointUserInfo.EndPointSystemId;
            acctRecon.EndpointUserId = endpointUserInfo.UserId;
            acctRecon.Version = (byte) endpointUserInfo.Version;
            acctRecon.IsDeleted = false;
            acctRecon.ExceptionType = IsNewlyAdded ? ExceptionTypes.Added : acctRecon.ExceptionType;
            AddOrUpdateAccountInReconTable(acctRecon);
        }
        public IList<IEndpointUserInfo> GetEndpointUserInfoByUser(int userId)
        {
           var listAcntRecon = GetAccountReconcilations(userId);

            IList<IEndpointUserInfo> listEndpointUserInfos = listAcntRecon.Select(accountReconcilation => GetEndpointUserInfo(accountReconcilation.AccountReconcilationId))
                .Where(endpointUserInfo => endpointUserInfo != null).ToList();

            return listEndpointUserInfos;
        }
        public IEndpointUserInfo  GetEndpointUserInfo(long ReconId)
        {
            AccountReconcilation acctRecon = GetAccountReconcilationRecordById(ReconId);

            return GetEndpointUserInfo(acctRecon.EndpointSystemId, acctRecon.EndpointUserId);
        }
        public IEndpointUserInfo GetEndpointUserInfo(int EndpointSystemId, int EndpointUserId)
        {

            var Endpoint = new EndPointFacade().GetEndPointSystemByKey(EndpointSystemId);

            if (Endpoint == null)
                return null;

            IEndpointFacade epFacade = GetEndpointFacade(Endpoint.EndPointSystemType.EndpointSystemTypeName);

            return epFacade.GeEndpointUserInfo(EndpointUserId);
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
        public IList<AccountReconcilation> GetAccountReconcilations(int userId)
        {
            return _reconcilationDA.GetAccountReconcilationByUser(userId);
        } 
        public AccountReconcilation GetAccountReconcilationRecordByUser(int endpointSystemId, int endpointUserId)
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

        public EntitlementReconcilation GetEntitlementReconcilationRecordByUser(int endpointSystemId, int endpointUserId, string entitlement, EntitlementType entitlementType)
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

        public void RemoveAllEntitlementsByUser(int userId, int endpointSystemId, int endpointUserId)
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
