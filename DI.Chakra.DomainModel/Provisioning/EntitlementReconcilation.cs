using AutoMapper;
using AutoMapper.QueryableExtensions;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using DIEntitlementType = DI.Chakra.Infrastructure.Enums.EntitlementType;
namespace DI.Chakra.DomainModel
{
    public class EntitlementReconcilation : DomainModelBase<EntitlementReconcilation, EntityModel.EntitlementReconcilation>
    {
        public long AccountReconcilationId { get; set; }
        public EndpointType EndpointType { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public long? UserId { get; set; }
        public long EntitlementId { get; set; }
        public int EntitlementType { get; set; }
        public string AccountName { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementDescription { get; set; }
        public string Functions { get; set; }
        public DateTime EnteredDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsResolved { get; set; }
        public string ResolvedBy { get; set; }
        public string Comments { get; set; }
        public ExceptionTypes ExceptionType { get; set; }

        public ResolvedStatus ResolvedStatus { get; set; }
        public string ProvisionedThrough { get; set; }
        public bool IsSODConflict { get; set; }

        public bool IsSensitiveAccess { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public DateTime? ValidFromDate { get; set; }

        public DateTime? ValidToDate { get; set; }

        public long? AccessRequestId { get; set; }

        public AccountReconcilation AccountReconcilation { get; set; }

        public EndPoint EndPoint { get; set; }

        /// <summary>
        /// Datetime at which this entitlement was last reviewed via attestation.
        /// </summary>
        public DateTime? LastAttestedOn { get; set; }
        public DateTime? ExceptionOccuredDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string PerformedBy { get; set; }

        public bool IsDefaultRole { get; set; }
        public static List<AccountEntitlementRecon> GetAccountEntitlementReconsByUserId(long AccountReconId, long? userId, List<long> endpointIds, AccountType accountType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new EntitlementReconcilationRepository(dataAccessTransaction).GetAccountEntitlementReconsByUserId(AccountReconId, userId, endpointIds, accountType).Select(acc => AutoMapper.Mapper.Map<DomainModel.AccountEntitlementRecon>(acc)).ToList();
            }
        }
        public static List<AccountEntitlementRecon> GetAccountEntitlementRecons(long AccountReconId, long userId, List<long> endpointIds, AccountType accountType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new EntitlementReconcilationRepository(dataAccessTransaction).GetAccountEntitlementRecons(AccountReconId, userId, endpointIds, accountType).Select(acc => AutoMapper.Mapper.Map<DomainModel.AccountEntitlementRecon>(acc)).ToList();
            }
        }
        //Method for "GetEntitlementReconcilationRecordByUser"
        public static EntitlementReconcilation GetByUser(long endpointUserId, long endpointSystemId, string entitlement, EntitlementType entitlementType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.EndpointUserId == endpointUserId && er.EndpointSystemId == endpointSystemId &&
                           er.Entitlement == entitlement && er.EntitlementType == (int)entitlementType).FirstOrDefault());
            }
        }

        public static List<EntitlementReconcilation> GetByUser(long endpointUserId, long endpointSystemId, EntitlementType entitlementType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.EndpointUserId == endpointUserId && er.EndpointSystemId == endpointSystemId && er.EntitlementType == (int)entitlementType).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetByEndpointUserId(long endpointUserId, long endpointSystemId, EntitlementType entitlementType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.EndpointUserId == endpointUserId && er.EndpointSystemId == endpointSystemId &&
                            er.EntitlementType == (int)entitlementType).ToList());
            }
        }
        public static List<EntitlementReconcilation> GetActiveEntitlementByEndpointUserId(long endpointUserId, long endpointSystemId, EntitlementType entitlementType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.EndpointUserId == endpointUserId && er.IsDeleted == false && er.EndpointSystemId == endpointSystemId &&
                            er.EntitlementType == (int)entitlementType).ToList());
            }
        }
        public static List<EntitlementReconcilation> GetByEndpointUserId(long endpointUserId, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.EndpointUserId == endpointUserId && er.EndpointSystemId == endpointSystemId
                           ).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                        Get(er => er.EndpointSystemId == endpointSystemId
                           ).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetAllByEndpointSystemTypeId(int endpointSystemTypeId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var endpointIds = EndPoint.GetAllByEndpointTypeId(endpointSystemTypeId).Select(p => p.Id);
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                        Get(er => endpointIds.Contains(er.EndpointSystemId)).ToList());
            }
        }

        public static List<string> GetNonExistingEntitlementsByEndpointSystemId(long endpointSystemId, long endpointUserId, List<string> entitlements)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var existingEntitlements = new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                         Get(er => er.EndpointSystemId == endpointSystemId && er.EndpointUserId == endpointUserId && !er.IsDeleted && entitlements.Contains(er.Entitlement.Trim())
                            ).Select(er => er.Entitlement).ToList();

                if (existingEntitlements == null || existingEntitlements.Count() == 0)
                    return entitlements.Select(e => e.Trim()).ToList();
                else
                    return entitlements.Where(e => !existingEntitlements.Contains(e)).Select(e => e.Trim()).ToList();
            }
        }

        public static List<string> GetExistingEntitlementsByEndpointSystemId(long endpointSystemId, long endpointUserId, List<string> entitlements)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                         Get(er => er.EndpointSystemId == endpointSystemId && er.EndpointUserId == endpointUserId && !er.IsDeleted && entitlements.Contains(er.Entitlement.Trim())
                            ).Select(er => er.Entitlement.Trim()).ToList();
            }
        }

        //Method for "GetEntitlementReconcilationsByUser"
        public static List<EntitlementReconcilation> GetByUser(long userId, long endpointSystemId, long endpointUserId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.UserId == userId &&
                           er.EndpointUserId == endpointUserId && er.EndpointSystemId == endpointSystemId, ar => ar.EndPoint, ar1 => ar1.EndPoint.EndPointSystemType).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetByUserAndReconcilationId(long userId, long endpointSystemId, long reconcilationId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.UserId == userId &&
                           er.AccountReconcilationId == reconcilationId && er.EndpointSystemId == endpointSystemId && er.IsDeleted != true, ar => ar.EndPoint, ar1 => ar1.EndPoint.EndPointSystemType).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetByReconcilationId(long endpointSystemId, long reconcilationId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er =>
                           er.AccountReconcilationId == reconcilationId && er.EndpointSystemId == endpointSystemId && er.IsDeleted != true, ar => ar.EndPoint, ar1 => ar1.EndPoint.EndPointSystemType).ToList());
            }
        }
        public static List<EntitlementReconcilation> GetByReconcilationIdAndType(long endpointSystemId, long reconcilationId, ApplicationType applicationType, EntitlementType entitlementType = DIEntitlementType.None)
        {
            List<EntitlementReconcilation> entitlements = new List<EntitlementReconcilation>();
            if (applicationType == ApplicationType.OneCard)
            {
                using (var tran = new DataAccessTransaction(false))
                {
                    entitlements = ORMapList(new EntitlementReconcilationRepository(tran).GetOneCardEntitlementWithAdditionalInfo(reconcilationId, endpointSystemId).ToList());
                }
            }
            if(applicationType == ApplicationType.NPAC)
            {
                using (var tran = new DataAccessTransaction(false))
                {
                    entitlements = ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(tran).Get(er =>
                           er.AccountReconcilationId == reconcilationId && er.EndpointSystemId == endpointSystemId && er.IsDeleted == false && er.EntitlementType == (int)entitlementType, ar => ar.EndPoint, ar1 => ar1.EndPoint.EndPointSystemType).ToList());
                    if (entitlements != null && entitlements.Count > 0)
                    {
                        var usr = NPACUserInfo.GetNPACUserForAttestation(endpointSystemId, reconcilationId);
                        if (usr != null)
                        {
                            entitlements.ForEach(i =>
                            {
                                i.Functions = $"Section: {usr.Section} / SectionName: {usr.SectionName} / Zone: {usr.Zone}";
                            });
                        }
                    }
                }
            }
            if (applicationType == ApplicationType.MSSL_CRM || applicationType == ApplicationType.MSSL_ISU ||
                applicationType == ApplicationType.EBS_CRM || applicationType == ApplicationType.EBS_ISU)
            {
                using (var tran = new DataAccessTransaction(false))
                {
                    entitlements = ORMapList(new EntitlementReconcilationRepository(tran).GetSAPPositionWithTextInfo(reconcilationId, endpointSystemId, applicationType, entitlementType).ToList());
                }
            }
            else if (applicationType == ApplicationType.EBS || applicationType == ApplicationType.MSSL)
            using (var tran = new DataAccessTransaction(false))
            {
                entitlements = ORMapList(new EntitlementReconcilationRepository(tran).GetSAPHANAPositionWithTextInfo(reconcilationId, endpointSystemId, applicationType, entitlementType).ToList());
            }
            else
            {
                using (var tran = new DataAccessTransaction(false))
                {
                    entitlements = ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(tran).Get(er =>
                           er.AccountReconcilationId == reconcilationId && er.EndpointSystemId == endpointSystemId && er.IsDeleted == false && er.EntitlementType == (int)entitlementType, ar => ar.EndPoint, ar1 => ar1.EndPoint.EndPointSystemType).ToList());
                }
            }

            entitlements.ForEach(i =>
            {
                //Unmapped account should not allow revoking.
                i.IsDefaultRole = true;
            });
            entitlements = entitlements.Where(i => !string.IsNullOrWhiteSpace(i.Entitlement)).ToList();
            if (applicationType == ApplicationType.SSIDS_SMS)
            {
                entitlements.ForEach(entitlement =>
                {
                    //Added below code for New Attestation
                    entitlement.Entitlement = "Handphone Registered";
                    entitlement.EntitlementDescription = "Handphone Registered";
                });
            }
            return entitlements;
        }
        public static List<EntitlementReconcilation> GetByUser(long userId, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.UserId == userId &&
                           er.EndpointSystemId == endpointSystemId && er.IsDeleted == false, er => er.AccountReconcilation).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetByEndpointUser(long endpointuserId, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.EndpointUserId == endpointuserId &&
                           er.EndpointSystemId == endpointSystemId && er.IsDeleted == false).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetByEndpointUsers(List<long> endpointuserIds, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => endpointuserIds.Contains(er.EndpointUserId) &&
                           er.EndpointSystemId == endpointSystemId && er.IsDeleted == false).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetByUserADIDAndEndpointSystemId(string userADID, long endpointSystemId, EntitlementType entitlementType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.AccountName == userADID && er.EntitlementType == (int)entitlementType &&
                           er.EndpointSystemId == endpointSystemId && er.IsDeleted == false).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetByEntitlementId(long entitlementId, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                    Get(er => er.EntitlementId == entitlementId && er.EndpointSystemId == endpointSystemId && !er.IsDeleted).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetByUserId(long userId, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                    Get(er => er.UserId == userId && er.EndpointSystemId == endpointSystemId && !er.IsDeleted).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetByUserId(long userId, long endpointSystemId, EntitlementType entitlementType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                    Get(er => er.UserId == userId && er.EndpointSystemId == endpointSystemId && er.EntitlementType == (int)entitlementType && !er.IsDeleted).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetEntitlementByUserId(List<long> userIds)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.UserId.HasValue && userIds.Contains(er.UserId.Value) && er.IsDeleted == false && er.ProvisionedThrough.ToLower() == "seed").ToList());
            }
        }

        public static List<EntitlementReconcilation> GetByUser(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.UserId == userId && er.IsDeleted == false && (!er.AccountReconcilation.IsDeleted && !er.AccountReconcilation.IsLocked), er => er.AccountReconcilation, er => er.EndPoint).ToList());
            }
        }
        //public static List<EntitlementReconcilation> GetByEndpoint(EndPoint endpoint)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        if (endpoint.EndpointSystemTypeId == (int)EndpointType.ActiveDirectory)
        //        {
        //            return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetActiveDirectory(endpoint.Id).ToList());
        //        }
        //        else if (endpoint.EndpointSystemTypeId == (int)EndpointType.LotusNotes)
        //        {
        //            return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetLNS(endpoint.Id).ToList());
        //        }
        //        else if (endpoint.EndpointSystemTypeId == (int)EndpointType.SAP)
        //        {
        //            return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetSAP(endpoint.Id).ToList());
        //        }
        //        else if (endpoint.EndpointSystemTypeId == (int)EndpointType.SAPEnterprisePortal)
        //        {
        //            return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetSAPEP(endpoint.Id).ToList());
        //        }
        //        else if (endpoint.EndpointSystemTypeId == (int)EndpointType.InhouseSystems)
        //        {
        //            return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetInHouse(endpoint.Id).ToList());
        //        }
        //        else if (endpoint.EndpointSystemTypeId == (int)EndpointType.Linux)
        //        {
        //            return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetLinux(endpoint.Id).ToList());
        //        }
        //        else if (endpoint.EndpointSystemTypeId == (int)EndpointType.VPN)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        public static void RemoveEntitlementsByUser(long endpointSystemId, long endpointUserId, string currentUser)
        {
            var entRecons = EntitlementReconcilation.GetByEndpointUser(endpointUserId, endpointSystemId);
            entRecons.ForEach(entRecon =>
            {
                entRecon.IsDeleted = true;
                entRecon.DeletedDate = DateTime.Now;
                entRecon.Save(currentUser);
            });
        }

        public static void DeleteEntitlements(long accountReconId, long endpointSystemId, string currentUser, long? accessRequestId)
        {
            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var recons = EntitlementReconcilation.GetByAccountReconcilationId(accountReconId, endpointSystemId);
                if (recons == null)
                    return;

                recons.ForEach(entRecon =>
                {
                    entRecon.EntitlementDescription = string.Empty;
                    entRecon.Functions = string.Empty;
                    entRecon.IsDeleted = true;
                    entRecon.DeletedDate = DateTime.Now;
                    entRecon.AccessRequestId = accessRequestId;
                    entRecon.Save(currentUser);
                });
            }
        }
        public static void DeleteEntitlementsByType(int entitlementType, long accountReconId, long endpointSystemId, string currentUser, long? accessRequestId)
        {
            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var recons = EntitlementReconcilation.GetByAccountReconcilationId(accountReconId, endpointSystemId);
                if (recons == null)
                    return;
                recons = recons.Where(i => i.IsDeleted == false).ToList();
                recons.ForEach(entRecon =>
                {
                    if (entRecon.EntitlementType == entitlementType)
                    {
                        entRecon.Functions = string.Empty;
                        entRecon.IsDeleted = true;
                        entRecon.DeletedDate = DateTime.Now;
                        entRecon.AccessRequestId = accessRequestId;
                        entRecon.Save(currentUser);
                    }

                });
            }
        }
        public static void DeleteEntitlementsById(long reconId, long endpointSystemId, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var recons = EntitlementReconcilation.GetById(reconId);
                if (recons == null)
                    return;

                recons.IsDeleted = true;
                recons.DeletedDate = DateTime.Now;
                recons.Save(currentUser);
            }
        }
        public static void DeleteEntitlementsByAcountReconId(long reconId, long endpointSystemId, string currentUser)
        {
            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var recons = EntitlementReconcilation.GetAllByAccountReconcilationId(reconId);
                foreach (var recon in recons)
                {
                    if (recon == null || recon.IsDeleted)
                        continue;

                    recon.IsDeleted = true;
                    recon.DeletedDate = DateTime.Now;
                    recon.Save(currentUser);
                }
            }
        }
        public static void DeleteEntitlements(long userId, long endpointSystemId, long endpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var recons = GetByUser(userId, endpointSystemId, endpointUserId);
                if (recons == null)
                    return;

                foreach (EntitlementReconcilation recon in recons)
                {
                    recon.Delete();
                }
            }
        }
        public static List<EntitlementReconcilation> GetNonSeedRoles(long userId, long endpointSystemId)
        {
            var ents = GetByUser(userId, endpointSystemId);

            return ents.Where(t => t.ProvisionedThrough != DI.Chakra.Infrastructure.Enums.ProvisionedThrough.Seed.ToString()).ToList();
        }
        public static List<EntitlementReconcilation> GetSeedRoles(long userId, long endpointSystemId)
        {
            var ents = GetByUser(userId, endpointSystemId);

            return ents.Where(t => t.ProvisionedThrough == DI.Chakra.Infrastructure.Enums.ProvisionedThrough.Seed.ToString()).ToList();
        }

        public static List<EntitlementReconcilation> GetSeedRolesUsingProvisionThrough(long endPointSystemId, long accountReconId, long userId, string provisionedThrough)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(user => user.EndpointSystemId == endPointSystemId && user.AccountReconcilationId == accountReconId && user.UserId == userId && user.ProvisionedThrough == provisionedThrough).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetDeletedEntitlements(long EndpointId, DateTime runDatetime, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                List<EntitlementReconcilation> listDeletedEntitlements = new List<EntitlementReconcilation>();

                listDeletedEntitlements = ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction)
                       .Get(objUser => objUser.EndpointSystemId == EndpointId && (objUser.LastModifiedDate < runDatetime
                       || (objUser.CreatedDate < runDatetime && objUser.LastModifiedDate == null))).ToList());

                return listDeletedEntitlements;
            }
        }

        public static void UpdateException(EntitlementReconcilation entitlementReconcilation, bool IsAdded)
        {
            if (!IsAdded)
            {
                entitlementReconcilation.IsDeleted = true;
                entitlementReconcilation.DeletedDate = DateTime.Now;
                entitlementReconcilation.ExceptionType = ExceptionTypes.Removed;
                entitlementReconcilation.ExceptionOccuredDate = DateTime.Now;
            }
            else
            {

                entitlementReconcilation.LastAttestedOn = null;
                entitlementReconcilation.IsDeleted = false;
                entitlementReconcilation.DeletedDate = null;
                entitlementReconcilation.ExceptionType = ExceptionTypes.Added;
                entitlementReconcilation.ExceptionOccuredDate = DateTime.Now;
            }

            entitlementReconcilation.IsResolved = false;
            entitlementReconcilation.ResolvedBy = string.Empty;
            entitlementReconcilation.ResolvedStatus = 0;
            entitlementReconcilation.ProvisionedThrough = Infrastructure.Enums.ProvisionedThrough.BackEnd.ToString();
        }
        public static void ProcessDeletedEntitlements(long EndpointId, Process objDiscoveryProcess, string currentUser, List<int> lstEntitlementTypesToBeConsidered)
        {
            //List<EntitlementReconcilation> listDeletedEntitlements = new List<EntitlementReconcilation>();

            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            //{
            //    if (objDiscoveryProcess != null)
            //    {
            //        listDeletedEntitlements = ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction)
            //           .Get(objUser => objUser.EndpointSystemId == EndpointId && lstEntitlementTypesToBeConsidered.Contains(objUser.EntitlementType) &&
            //            ((objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt) ||
            //            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null))).ToList());
            //    }
            //}
            
            if (lstEntitlementTypesToBeConsidered.Count > 0 && objDiscoveryProcess != null)
            {
                var strDate = objDiscoveryProcess.ProcessStartAt.Value.ToString("dd MMM yyyy hh:mm:ss");
                var query = $@"UPDATE DI_M_EntitlementReconcilation SET ProvisionedThrough='BackEnd',IsDeleted = 1, DeletedDate=GETDATE(),ExceptionType=2, ExceptionOccuredDate = GETDATE(),IsResolved=0,ResolvedBy = NULL,LastAttestedOn = NULL,LastModifiedBy='{currentUser}',LastModifiedDate=GETDATE() WHERE EndpointSystemId={EndpointId}   AND EntitlementType IN({string.Join(",", lstEntitlementTypesToBeConsidered)}) AND (LastModifiedDate < '{strDate}' or (CreatedDate <'{strDate}' AND LastModifiedDate is NULL)) AND IsDeleted=0";
                Logger.DILog.DiscoveryLogger.WriteLog(objDiscoveryProcess.Id, Logger.LogLevelType.Info, query);

                var dataCount = ExecuteQueryReturnValue(query);
                Logger.DILog.DiscoveryLogger.WriteLog(objDiscoveryProcess.Id, Logger.LogLevelType.Info, $"ProcessDeletedEntitlements Record count: {dataCount}");
            }
            //long totalDeleted = 0;
            //foreach (var item in listDeletedEntitlements)
            //{
            //    try
            //    {
            //        if (!item.IsDeleted)
            //        {
            //            item.IsDeleted = true;
            //            item.DeletedDate = DateTime.Now;
            //            item.ExceptionType = ExceptionTypes.Removed;
            //            item.ExceptionOccuredDate = DateTime.Now;
            //            // clearing resolved flag so that this item shows up in reconciliation again
            //            item.IsResolved = false;
            //            item.ResolvedBy = null;
            //            item.LastAttestedOn = null;
            //            item.Save(currentUser);
            //            totalDeleted++;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger.DILog.DiscoveryLogger.WriteLog(objDiscoveryProcess.Id, Logger.LogLevelType.Exception,$"ProcessDeletedEntitlements: {ex.Message}",ex);
            //    }
            //}
            //Logger.DILog.DiscoveryLogger.WriteLog(objDiscoveryProcess.Id, Logger.LogLevelType.Info, $"Total item to be removed:{listDeletedEntitlements?.Count} & actual removed:{totalDeleted}");
        }
        public static void ProcessDeletedEntitlements(long EndpointId, Process objDiscoveryProcess, string currentUser, bool considerFileTimeStamp = false, DateTime? filetimeStamp = null)
        {
            List<EntitlementReconcilation> listDeletedEntitlements = new List<EntitlementReconcilation>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (objDiscoveryProcess != null)
                {
                    if (!considerFileTimeStamp)
                    {
                        listDeletedEntitlements = ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction)
                           .Get(objUser => objUser.EndpointSystemId == EndpointId &&
                            ((objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt) ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null))).ToList());
                    }
                    else
                    {
                        listDeletedEntitlements = ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction)
                                                   .Get(objUser => objUser.EndpointSystemId == EndpointId &&
                                                    ((objUser.LastModifiedDate < filetimeStamp) ||
                                                    (objUser.CreatedDate < filetimeStamp && objUser.LastModifiedDate == null))).ToList());
                    }
                }
            }

            foreach (var item in listDeletedEntitlements)
            {
                try
                {
                    if (!item.IsDeleted)
                    {
                        item.IsDeleted = true;
                        item.DeletedDate = DateTime.Now;
                        item.ExceptionType = ExceptionTypes.Removed;
                        item.ExceptionOccuredDate = DateTime.Now;
                        // clearing resolved flag so that this item shows up in reconciliation again
                        item.IsResolved = false;
                        item.ResolvedBy = null;
                        item.LastAttestedOn = null;
                        item.Save(currentUser);
                    }
                }
                catch (Exception)
                {

                }
            }
            listDeletedEntitlements?.Clear();
            listDeletedEntitlements = null;
        }

        public static bool DoesAnyEntitlementHavingSODConflict(long endpointSystemId, long endpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Any(m => m.EndpointUserId == endpointUserId &&
                m.EndpointSystemId == endpointSystemId && m.IsSODConflict == true);
            }
        }
        public static bool DoesAnyEntitlementHavingSensitiveAccess(long endpointSystemId, long endpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Any(m => m.EndpointUserId == endpointUserId &&
                m.EndpointSystemId == endpointSystemId && m.IsSensitiveAccess == true);
            }
        }

        public override long Save(string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                this.Save(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
            // SOD related coded lines are from RWS. Since SOD is not applicable in SP. Below lines are commented - Ashok
            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            //{
            //    try
            //    {
            //        IList<UserSODComplianceReport> compReport = null;
            //        if (!this.IsDeleted)
            //            compReport = GenerateComplianceReport(this, dataAccessTransaction.User);
            //        else
            //        {
            //            AccountReconcilation accRecon = AccountReconcilation.GetById(this.AccountReconcilationId);

            //            if (accRecon.UserId.HasValue)
            //            {

            //                List<UserSODComplianceReport> report = UserSODComplianceReport.GetReportbyUserId(accRecon.UserId.Value)
            //                    .Where(x => (x.ConflictEntitlement1 == this.Entitlement && x.EndpointSystemId == this.EndpointSystemId)
            //                                ||
            //                                (x.ConflictEntitlement2 == this.Entitlement && x.EndpointSystemId == this.EndpointSystemId)
            //                               ).ToList();
            //                UserSODComplianceReport.DeleteList(report);

            //                //After deleting SOD report, get the refresh list for user
            //                compReport = UserSODComplianceReport.GetReportbyUserId(accRecon.UserId.Value);
            //            }

            //        }

            //        if (compReport != null)
            //        {

            //            this.IsSODConflict = compReport.Where(x => x.CompliancePolicyType == "SOD").ToList().Count() > 0 ? true : false;
            //            this.IsSensitiveAccess = compReport.Where(x => x.CompliancePolicyType != "SOD").ToList().Count() > 0 ? true : false;
            //            base.Save(dataAccessTransaction);
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        throw;
            //    }
            //}
            return 0;
        }

        public override long Save(DataAccessTransaction dataAccessTransaction)
        {
            base.Save(dataAccessTransaction);
            return 0;
        }

        protected IList<UserSODComplianceReport> GenerateComplianceReport(EntitlementReconcilation entRecon, string currentUser)
        {
            IList<UserSODComplianceReport> compReport = new List<UserSODComplianceReport>();

            AccountReconcilation acctRecon = AccountReconcilation.GetById(entRecon.AccountReconcilationId);

            List<DomainModel.Presentation.PresentationUserEntitlement> userEntitlement = EndPointFactory.GetEndPoint(this.EndpointType).GetEntitlementDetail(this.EndpointUserId).ToList();

            List<string> entitlements = userEntitlement.Select(x => x.Name.ToString()).ToList();

            string entitlementsWithComma = string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { });

            Dto.UserSODComplianceRequest request = new Dto.UserSODComplianceRequest(); request.PolicyId = 0;
            if (acctRecon.UserId.HasValue)
            {
                request.UserId = (long)acctRecon.UserId;
                request.IsFromAccessRequest = false;
                request.EntitlementDetails = entitlementsWithComma;
                request.EndpointSystemId = entRecon.EndpointSystemId;
                request.EndpointTypeValue = (EndpointType)(acctRecon.EndPoint != null ? acctRecon.EndPoint.EndpointSystemTypeId : 0);
                List<UserSODComplianceReport> report = UserSODComplianceReport.GetReportbyUserId((long)acctRecon.UserId);
                compReport = DomainModel.CompliancePolicy.CheckUserCompliance(request);
                List<UserSODComplianceReport> reportTosave = compReport.ToList();
                compReport.All(x => { x.UserId = (long)acctRecon.UserId; x.isFromAccessRequest = false; return true; });
                if (compReport.Count > 0)
                {
                    foreach (UserSODComplianceReport dbrpt in report)
                    {
                        foreach (UserSODComplianceReport calrept in compReport)
                        {
                            if (calrept.CompliancePolicyType == dbrpt.CompliancePolicyType)
                            {
                                if (calrept.Endpoint == dbrpt.Endpoint && calrept.ConflictEntitlement1 == dbrpt.ConflictEntitlement1 && calrept.ConflictEntitlement2 == dbrpt.ConflictEntitlement2 && calrept.UserId == dbrpt.UserId && calrept.isFromAccessRequest == dbrpt.isFromAccessRequest)
                                    reportTosave.Remove(calrept);
                            }

                        }
                    }
                    if (reportTosave.Count() > 0)
                    {
                        UserSODComplianceReport.SaveList(reportTosave, currentUser);
                        acctRecon.IsSODConflict = compReport.Where(x => x.CompliancePolicyType == "SOD").ToList().Count() > 0 ? true : false;
                        acctRecon.IsSensitiveAccess = compReport.Where(x => x.CompliancePolicyType != "SOD").ToList().Count() > 0 ? true : false;
                        acctRecon.Save(currentUser);
                    }

                }
            }
            return compReport;
            //return new EntitlementReconcilation()
            //{
            //    IsSODConflict = compReport.Where(x => x.CompliancePolicyType == "SOD").ToList().Count() > 0 ? true : false,
            //    IsSensitiveAccess = compReport.Where(x => x.CompliancePolicyType != "SOD").ToList().Count() > 0 ? true : false,
            //    AccountReconcilationId = acctRecon.Id,
            //    EndpointSystemId = acctRecon.EndpointSystemId,
            //    EndpointUserId = acctRecon.EndpointUserId,
            //    AccountName = acctRecon.AccountName,
            //    UserId = acctRecon.UserId,
            //    EnteredDate = DateTime.Now,
            //    IsDeleted = false,
            //    EntitlementType = (int)_entitlementType,


            //    EndpointType = (EndpointType)acctRecon.EndpointSystemId,
            //};
        }

        public static EntitlementReconcilation GetByAccountReconcilationId(long accountReconcilationId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(m => m.AccountReconcilationId == accountReconcilationId).FirstOrDefault());
            }
        }

        public static List<EntitlementReconcilation> GetAllByAccountReconcilationId(long accountReconcilationId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(m => m.AccountReconcilationId == accountReconcilationId).ToList());
            }
        }

        public static EntitlementReconcilation GetByAccountReconcilationIdAndEntitlementId(long accountReconcilationId, long entitlementId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(m => m.AccountReconcilationId == accountReconcilationId && m.EntitlementId == entitlementId).FirstOrDefault());
            }
        }

        public static EntitlementReconcilation GetByAccountReconcilationIdAndEntitlementName(long accountReconcilationId, string entitlementName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(m => m.AccountReconcilationId == accountReconcilationId && m.Entitlement == entitlementName).FirstOrDefault());
            }
        }

        public static List<EntitlementReconcilation> GetByAccountReconcilationId(long accountReconcilationId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(m => m.AccountReconcilationId == accountReconcilationId).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetByAccessRequestId(List<long> accessRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction("system"))
            {
                List<EntitlementReconcilation> listDeletedAccounts = new List<EntitlementReconcilation>();

                listDeletedAccounts = ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction)
                       .Get(objUser => objUser.AccessRequestId.HasValue && accessRequestId.Contains(objUser.AccessRequestId.Value),
                       true).ToList());

                return listDeletedAccounts;
            }
        }

        public static List<EntitlementReconcilation> GetActiveAccountByUser(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(ar => ar.UserId == userId && !ar.IsDeleted, ar => ar.EndPoint
                    ).ToList());
            }
        }

        public static EntitlementReconcilation CreateEntitlementReconcilationShell(AccountReconcilation acctRecon, EntitlementType _entitlementType, long endpointTypeId)
        {
            #region old code
            //IList<UserSODComplianceReport> compReport = new List<UserSODComplianceReport>();
            //if (acctRecon.UserId.HasValue)
            //{

            //    Dto.UserSODComplianceRequest request = new Dto.UserSODComplianceRequest(); request.PolicyId = 0;
            //    request.UserId = (long)acctRecon.UserId;
            //    request.IsFromAccessRequest = false;
            //    request.EntitlementDetails = Entitlements;
            //    request.EndpointSystemId = acctRecon.EndpointSystemId;
            //    request.EndpointTypeValue = (EndpointType)(acctRecon.EndPoint != null ? acctRecon.EndPoint.EndpointSystemTypeId : 0);
            //    List<UserSODComplianceReport> report = UserSODComplianceReport.GetReportbyUserId((long)acctRecon.UserId);
            //    compReport = CheckUserCompliance(request);
            //    List<UserSODComplianceReport> reportTosave = compReport.ToList();
            //    compReport.All(x => { x.UserId = (long)acctRecon.UserId; x.isFromAccessRequest = false; return true; });
            //    if (compReport.Count > 0)
            //    {
            //        foreach (UserSODComplianceReport dbrpt in report)
            //        {
            //            foreach (UserSODComplianceReport calrept in compReport)
            //            {
            //                if (calrept.CompliancePolicyType == dbrpt.CompliancePolicyType)
            //                {
            //                    if (calrept.Endpoint == dbrpt.Endpoint && calrept.ConflictEntitlement1 == dbrpt.ConflictEntitlement1 && calrept.ConflictEntitlement2 == dbrpt.ConflictEntitlement2 && calrept.UserId == dbrpt.UserId && calrept.isFromAccessRequest == dbrpt.isFromAccessRequest)
            //                        reportTosave.Remove(calrept);
            //                }

            //            }
            //        }
            //        if (reportTosave.Count() > 0)
            //        {
            //            UserSODComplianceReport.SaveList(reportTosave, "Service");
            //            acctRecon.IsSODConflict = compReport.Where(x => x.CompliancePolicyType == "SOD").ToList().Count() > 0 ? true : false;
            //            acctRecon.IsSensitiveAccess = compReport.Where(x => x.CompliancePolicyType != "SOD").ToList().Count() > 0 ? true : false;
            //            acctRecon.Save("Service");
            //        }

            //    }
            //}

            //var endpoint = EndPoint.GetByIdIncludingEndPointSystemType(acctRecon.EndpointSystemId);


            //return new EntitlementReconcilation()
            //{
            //    IsSODConflict = compReport.Where(x => x.CompliancePolicyType == "SOD").ToList().Count() > 0 ? true : false,
            //    IsSensitiveAccess = compReport.Where(x => x.CompliancePolicyType != "SOD").ToList().Count() > 0 ? true : false,
            //    AccountReconcilationId = acctRecon.Id,
            //    EndpointSystemId = acctRecon.EndpointSystemId,
            //    EndpointUserId = acctRecon.EndpointUserId,
            //    AccountName = acctRecon.AccountName,
            //    UserId = acctRecon.UserId,
            //    EnteredDate = DateTime.Now,
            //    IsDeleted = false,
            //    EntitlementType = (int)_entitlementType,
            //    EndPoint = endpoint,
            //    EndpointType = (EndpointType)endpoint.EndpointSystemTypeId,
            //};
            #endregion
            return new EntitlementReconcilation()
            {
                AccountReconcilationId = acctRecon.Id,
                EndpointSystemId = acctRecon.EndpointSystemId,
                EndpointUserId = acctRecon.EndpointUserId,
                AccountName = acctRecon.AccountName,
                UserId = acctRecon.UserId,
                EnteredDate = DateTime.Now,
                IsDeleted = false,
                DeletedDate = null,
                EntitlementType = Convert.ToInt32(_entitlementType),
                EndpointType = (EndpointType)endpointTypeId
            };
        }

        public static IList<UserSODComplianceReport> CheckUserCompliance(Dto.UserSODComplianceRequest request)
        {
            ComplianceProcess complianceProcess = null;
            AccessRequest accessRequest = null;
            List<UserSODComplianceReport> complianceRoleDetails = new List<UserSODComplianceReport>();
            try
            {
                if (request.AccessRequestId > 0) accessRequest = AccessRequest.GetByIdIncludeAdHocAccessRequest(request.AccessRequestId);
                if (request.UserId <= 0)
                {
                    if (accessRequest != null)
                    {
                        request.UserId = accessRequest.UserId;
                    }
                    else
                        return complianceRoleDetails;

                }

                if (string.IsNullOrEmpty(request.EntitlementDetails) && accessRequest != null)
                {
                    if (accessRequest.AdHocAccessRequest != null)
                        request.EntitlementDetails = accessRequest.AdHocAccessRequest.AccountType != AccountType.User ? accessRequest.AdHocAccessRequest.SharedAccountName : accessRequest.AdHocAccessRequest.Entitlement;
                }

                if ((request.EndpointSystemId == null || request.EndpointSystemId <= 0) && accessRequest != null)
                {
                    request.EndpointSystemId = (long)accessRequest.EndpointSystemId;
                    if (accessRequest.AdHocAccessRequest != null)
                        request.EndpointTypeValue = accessRequest.AdHocAccessRequest.EndpointSystemType;
                }

                var _processId = request.PolicyId;
                //try to get the available policy to validate.
                //understanding is that company will have only one policy.
                //if we have many policy, we are taking the first policy for the SOD.
                if (_processId <= 0)
                {
                    // var defaultPolicy = CompliancePolicy.GetById(2,true);
                    var defaultPolicy = CompliancePolicy.GetAll().Where(a => a.CompliancePolicyType == "SOD" && a.IsActive == true).FirstOrDefault();
                    if (defaultPolicy == null) return complianceRoleDetails;
                    var sODPolicyExecutionDefinition = SODPolicyExecutionDefinition.GetAll().Where(pe => pe.CompliancePolicyId.HasValue && pe.CompliancePolicyId.Value == defaultPolicy.Id).FirstOrDefault();
                    if (sODPolicyExecutionDefinition == null) return complianceRoleDetails;
                    _processId = sODPolicyExecutionDefinition.Id;
                }

                complianceProcess = ComplianceProcess.GetComplianceProcessByPolicyExecutionId(_processId);
                var policyExecutionDefinition = complianceProcess != null ? complianceProcess.PolicyExecutionDefinition : null;
                if (policyExecutionDefinition == null)
                {
                    complianceRoleDetails.AddRange(CompliancePolicy.CheckUserSensitiveComplianceReport(request));
                    return complianceRoleDetails;
                }
                var policy = policyExecutionDefinition.CompliancePolicy;
                if (policy == null)
                {
                    complianceRoleDetails.AddRange(CompliancePolicy.CheckUserSensitiveComplianceReport(request));
                    return complianceRoleDetails;
                }

                if (policy.CompliancePolicyType == "SOD")
                {
                    //get all user roles.
                    List<EntitlementReconcilation> userEntitlement = EntitlementReconcilation.GetByUser(request.UserId);
                    if (!string.IsNullOrEmpty(request.EntitlementDetails))
                    {
                        var entitlementDetails = request.EntitlementDetails.Trim();
                        if (!string.IsNullOrEmpty(entitlementDetails))
                        {
                            foreach (string requestedRole in entitlementDetails.Split(','))
                            {
                                userEntitlement.Add(new EntitlementReconcilation
                                {
                                    Entitlement = requestedRole,
                                    EndpointSystemId = request.EndpointSystemId,
                                    EndpointType = request.EndpointTypeValue,
                                });
                            }
                        }
                    }

                    //if its from Access Request get all the entitlements from cart as well.
                    if (request.IsFromAccessRequest)
                    {
                        var interimRequest = InterimStore.GetByUserId(request.UserId, true, Infrastructure.Enums.InterimStoreItemType.AccessRequest);

                        if (interimRequest != null)
                        {
                            for (int i = 0; i < interimRequest.Count; i++)
                            {
                                var AdhocaccessRequest = !String.IsNullOrEmpty(interimRequest[i].XmlData) ?
                                   Infrastructure.UtilityMethods.Deserialize<Dto.AdHocAccessRequest>(interimRequest[i].XmlData) : null;
                                if (AdhocaccessRequest != null)
                                {
                                    if (request.RequesteeUserId == AdhocaccessRequest.RequesteeUserId || request.UserId == AdhocaccessRequest.RequesterUserId)
                                    {
                                        userEntitlement.Add(new EntitlementReconcilation
                                        {
                                            Entitlement = AdhocaccessRequest.Entitlement,
                                            EndpointSystemId = (long)AdhocaccessRequest.EndpointSystemId,
                                            EndpointType = AdhocaccessRequest.EndpointSystemType,
                                        });
                                    }
                                }
                            }
                        }
                    }
                    foreach (SODConflictFunctions conflictfunction in policy.Conflicts)
                    {
                        List<SODConflict> lstConflicts = SODConflict.GetBySodConflictCodeIncludingSODRiskCode(conflictfunction.SodConflictCode);
                        if (lstConflicts.Count > 0)
                        {
                            foreach (SODConflict conflict in lstConflicts)
                            {
                                complianceRoleDetails.AddRange(CompliancePolicy.CheckUserConflictBetweenFunctions(userEntitlement, conflict));
                            }
                        }
                    }
                }
                foreach (UserSODComplianceReport userSOD in complianceRoleDetails)
                {
                    userSOD.CompliancePolicyCode = policy.CompliancePolicyCode;
                    userSOD.CompliancePolicyName = policy.CompliancePolicyName;
                    userSOD.CompliancePolicyType = policy.CompliancePolicyType;
                }
                complianceRoleDetails.AddRange(CompliancePolicy.CheckUserSensitiveComplianceReport(request));

                return complianceRoleDetails;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<EntitlementReconcilation> GetAllByActiveAccountReconcilationId(long accountReconcilationId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(m => m.AccountReconcilationId == accountReconcilationId && m.IsDeleted == false
                && m.EntitlementType == (int)DI.Chakra.Infrastructure.Enums.EntitlementType.Group).ToList());
            }
        }
        public static List<EntitlementReconcilation> GetByEndpointSystemIdAndAccountRecon(long endpointSystemId, long accountReconId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                        Get(er => er.EndpointSystemId == endpointSystemId && er.AccountReconcilationId == accountReconId
                           ).ToList());
            }
        }
        public static EntitlementReconcilation GetByEntitlement(long endpointSystemId, long accountReconId, string groupName, int entitlementType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                        Get(er => er.EntitlementType == entitlementType && er.EndpointSystemId == endpointSystemId && er.AccountReconcilationId == accountReconId && er.Entitlement == groupName
                           ).FirstOrDefault());
            }
        }
        public static EntitlementReconcilation GetByEntitlementType(long endpointSystemId, long accountReconId, long endpointUserId, int entitlementType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                        Get(er => er.EntitlementType == entitlementType && er.EndpointSystemId == endpointSystemId && er.AccountReconcilationId == accountReconId && er.EndpointUserId == endpointUserId
                           ).FirstOrDefault());
            }
        }
        public static List<EntitlementReconcilation> GetByEntitlementType(long endpointSystemId, long accountReconId,int entitlementType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                        Get(er => er.EntitlementType == entitlementType && er.EndpointSystemId == endpointSystemId && er.AccountReconcilationId == accountReconId
                           ).ToList());
            }
        }
        public static EntitlementReconcilation GetActiveByEntitlementType(long endpointSystemId, long accountReconId, long endpointUserId, int entitlementType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                        Get(er => er.EntitlementType == entitlementType && er.EndpointSystemId == endpointSystemId && er.AccountReconcilationId == accountReconId && er.EndpointUserId == endpointUserId && er.IsDeleted == false
                           ).FirstOrDefault());
            }
        }
        public static List<EntitlementReconcilation> GetByEntitlementTypeByExcludeGroup(long endpointSystemId, long accountReconId, long endpointUserId)
        {

            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                        Get(er => er.EntitlementType != (int)Chakra.Infrastructure.Enums.EntitlementType.ResponseGroup && er.EndpointSystemId == endpointSystemId && er.IsDeleted == false && er.AccountReconcilationId == accountReconId && er.EndpointUserId == endpointUserId
                           ).ToList());
            }
        }
        public static List<EntitlementReconcilation> GetByEndpointUserIdWithEntitlement(long endpointUserId, long endpointSystemId, EntitlementType entitlementType, string entitlement)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.Entitlement == entitlement && er.EndpointUserId == endpointUserId && er.EndpointSystemId == endpointSystemId &&
                            er.EntitlementType == (int)entitlementType).ToList());
            }
        }
        public static EntitlementReconcilation GetActiveReordByUserIdWithEntitlement(long endpointUserId, long endpointSystemId, EntitlementType entitlementType, string entitlement)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.IsDeleted == false && er.Entitlement == entitlement && er.EndpointUserId == endpointUserId && er.EndpointSystemId == endpointSystemId &&
                            er.EntitlementType == (int)entitlementType).FirstOrDefault());
            }
        }
        public static List<EntitlementReconcilation> GetActiveEntitlementByEndpointUserId(long endpointUserId, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.IsDeleted == false && er.EndpointUserId == endpointUserId && er.EndpointSystemId == endpointSystemId
                           ).ToList());
            }
        }
        public static List<EntitlementReconcilation> GetAllEntitlementByType(long endpointSystemId, int entitlementType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).
                        Get(er => er.EndpointSystemId == endpointSystemId && er.EntitlementType == entitlementType
                           ).ToList());
            }
        }
        public static void RemoveUserEntitlementByType(long EndpointId, int entitlementType, long endpointUserId, string currentUser)
        {
            List<EntitlementReconcilation> listDeletedEntitlements = new List<EntitlementReconcilation>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                listDeletedEntitlements = ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction)
                          .Get(objUser => objUser.EndpointSystemId == EndpointId && objUser.EndpointUserId == endpointUserId && objUser.EntitlementType == entitlementType).ToList());
            }
            foreach (var item in listDeletedEntitlements)
            {
                if (!item.IsDeleted)
                {
                    item.IsDeleted = true;
                    item.DeletedDate = DateTime.Now;
                    item.ExceptionType = ExceptionTypes.Removed;
                    item.ExceptionOccuredDate = DateTime.Now;
                    // clearing resolved flag so that this item shows up in reconciliation again
                    item.IsResolved = false;
                    item.ResolvedBy = null;
                    item.LastAttestedOn = null;
                    item.Save(currentUser);
                }
            }
        }
        public static void ProcessDeletedEntitlementsByType(long EndpointId, Process objDiscoveryProcess, string currentUser, int entitlementType)
        {
            List<EntitlementReconcilation> listDeletedEntitlements = new List<EntitlementReconcilation>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                if (objDiscoveryProcess != null)
                {
                    listDeletedEntitlements = ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction)
                           .Get(objUser => objUser.EndpointSystemId == EndpointId && objUser.EntitlementType == entitlementType &&
                            ((objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt) ||
                            (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null))).ToList());
                }
            }

            foreach (var item in listDeletedEntitlements)
            {
                if (!item.IsDeleted)
                {
                    item.IsDeleted = true;
                    item.DeletedDate = DateTime.Now;
                    item.ExceptionType = ExceptionTypes.Removed;
                    item.ExceptionOccuredDate = DateTime.Now;
                    // clearing resolved flag so that this item shows up in reconciliation again
                    item.IsResolved = false;
                    item.ResolvedBy = null;
                    item.LastAttestedOn = null;
                    item.Save(currentUser);
                }
            }

        }
        public static EntitlementReconcilation GetByUserEntitlement(long endpointUserId, long endpointSystemId, string entitlement)
        {
            var entitlementType = (int)DI.Chakra.Infrastructure.Enums.EntitlementType.ResponseGroup;
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.EndpointUserId == endpointUserId && er.EndpointSystemId == endpointSystemId &&
                           er.Entitlement == entitlement && er.EntitlementType != entitlementType).FirstOrDefault());
            }
        }
        public static EntitlementReconcilation GetByUserEntitlementByType(long endpointUserId, long endpointSystemId, string entitlement)
        {
            var entitlementType = (int)DI.Chakra.Infrastructure.Enums.EntitlementType.ResponseGroup;
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.EndpointUserId == endpointUserId && er.EndpointSystemId == endpointSystemId &&
                           er.Entitlement == entitlement && er.EntitlementType == entitlementType && er.IsDeleted == false).FirstOrDefault());
            }
        }
        public static List<EntitlementReconcilation> GetByUserEntitlementByType(long endpointUserId, long endpointSystemId)
        {
            var entitlementType = (int)DI.Chakra.Infrastructure.Enums.EntitlementType.ResponseGroup;
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(er => er.EndpointUserId == endpointUserId
                && er.EndpointSystemId == endpointSystemId && er.EntitlementType != entitlementType && er.IsDeleted == false).ToList());
            }
        }
        public static List<EntitlementReconcilation> GetByMultipleEndpoints(List<long> endpoints, AccountType accountType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetAllEntitlementReconcilationByEndpointSystemId(endpoints, accountType));
            }
        }
        public static EntitlementReconcilation GetByAccountReconcilationIdAndEntitlementType(long endpointId, long accountReconcilationId, int entitlementType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction).Get(m => m.EndpointSystemId == endpointId &&
                m.AccountReconcilationId == accountReconcilationId && m.EntitlementType == entitlementType).FirstOrDefault());
            }
        }
        public static List<EntitlementReconcilation> GetAllActiveByAccountReconcilationId(long accountReconcilationId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(tran).Get(m => m.AccountReconcilationId == accountReconcilationId && m.IsDeleted == false).ToList());
            }
        }

        public static List<EntitlementReconcilation> GetGemsBusinessRoleByAccount(long endpointId, long accountReconId, string accountName)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new EntitlementReconcilationRepository(tran).GetGemsBusinessRoleByAccount(endpointId, accountReconId, accountName));
            }
        }
        public static List<EntitlementReconcilation> GetTechnicalRoleFromBusinessRole(long endpointId, string accountName, string businessRole)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new EntitlementReconcilationRepository(tran).GetTechnicalRoleFromBusinessRole(endpointId, accountName, businessRole));
            }
        }
        public static List<EntitlementReconcilation> GetSAPEPBusinessRoleByAccount(long endpointId, long accountReconId, string accountName)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new EntitlementReconcilationRepository(tran).GetSAPEPBusinessRoleByAccount(endpointId, accountReconId, accountName));
            }
        }
        public static List<EntitlementReconcilation> GetSAPEPTechnicalRoleFromBusinessRole(long endpointId, string accountName, string businessRole)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new EntitlementReconcilationRepository(tran).GetSAPEPTechnicalRoleFromBusinessRole(endpointId, accountName, businessRole));
            }
        }
        public static List<EntitlementReconcilation> GetByUserAndReconcilationId(long userId, long endpointSystemId, long reconcilationId, ApplicationType applicationType, AccountType accountType, EntitlementType entitlementType = DIEntitlementType.None)
        {
            var entitlements = new List<EntitlementReconcilation>();
            switch (applicationType)
            {
                case ApplicationType.OneCard:
                    #region OneCard
                    using (var tran = new DataAccessTransaction(false))
                    {
                        entitlements = ORMapList(new EntitlementReconcilationRepository(tran).GetOneCardEntitlementWithAdditionalInfo(reconcilationId, endpointSystemId).ToList());
                    }
                    #endregion
                    break;
                case ApplicationType.MSSL_CRM:
                case ApplicationType.MSSL_ISU:
                case ApplicationType.EBS_CRM:
                case ApplicationType.EBS_ISU:
                    #region MSSL-EBS
                    using (var tran = new DataAccessTransaction(false))
                    {
                        entitlements = ORMapList(new EntitlementReconcilationRepository(tran).GetSAPPositionWithTextInfo(reconcilationId, endpointSystemId, applicationType, entitlementType).ToList());
                    }
                    #endregion
                    break;
                case ApplicationType.EAM:
                    #region EAM
                    using (var tran = new DataAccessTransaction(false))
                    {
                        entitlements = ORMapList(new EntitlementReconcilationRepository(tran).GetEAMEntitlements(reconcilationId, endpointSystemId).ToList());
                    }
                    #endregion
                    break;
                case ApplicationType.SAP_EP:
                case ApplicationType.CE:
                    #region SAP Portal EP/CE
                    using (var tran = new DataAccessTransaction(false))
                    {
                        entitlements = ORMapList(new EntitlementReconcilationRepository(tran).GetSAPPoratlEntitlements(reconcilationId, endpointSystemId, entitlementType).ToList());
                        entitlements.ForEach(entitlement  => 
                        {
                            if (!string.IsNullOrWhiteSpace(entitlement.Functions) && entitlement.Functions == "0")
                                entitlement.IsDefaultRole = true; });
                    }
                    #endregion
                    break;
                case ApplicationType.NPAC:
                    using (DataAccessTransaction tran = new DataAccessTransaction(false))
                    {
                        entitlements = ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(tran).Get(er => er.UserId == userId &&
                           er.AccountReconcilationId == reconcilationId && er.EndpointSystemId == endpointSystemId && er.EntitlementType == (int)entitlementType && er.IsDeleted == false, ar => ar.EndPoint, ar1 => ar1.EndPoint.EndPointSystemType).ToList());
                        
                        if(entitlements != null && entitlements.Count > 0)
                        {
                            var usr = NPACUserInfo.GetNPACUserForAttestation(endpointSystemId, reconcilationId);
                            if(usr != null)
                            {
                                entitlements.ForEach(i =>
                                {
                                    i.Functions = $"Section: {usr.Section} / SectionName: {usr.SectionName} / Zone: {usr.Zone}";
                                });
                            }
                        }
                    }
                    break;
                case ApplicationType.EBS:
                case ApplicationType.MSSL:
                    #region MSSL-EBS
                    using (var tran = new DataAccessTransaction(false))
                    {
                        entitlements = ORMapList(new EntitlementReconcilationRepository(tran).GetSAPHANAPositionWithTextInfo(reconcilationId, endpointSystemId, applicationType, entitlementType).ToList());
                    }
                    #endregion
                    break;
                default:
                    #region other type
                    using (DataAccessTransaction tran = new DataAccessTransaction(false))
                    {
                        entitlements = ORMapList(new RepositoryBase<EntityModel.EntitlementReconcilation>(tran).Get(er => er.UserId == userId &&
                           er.AccountReconcilationId == reconcilationId && er.EndpointSystemId == endpointSystemId && er.EntitlementType == (int)entitlementType && er.IsDeleted == false, ar => ar.EndPoint, ar1 => ar1.EndPoint.EndPointSystemType).ToList());
                    }
                    #endregion
                    break;
            }

            if (entitlements.Count > 0)
            {
                MarkDefaultRole(userId, endpointSystemId, entitlements, applicationType, accountType, entitlementType);
            }
            entitlements = entitlements.Where(i => !string.IsNullOrWhiteSpace(i.Entitlement)).ToList();
            return entitlements;
        }
        private static void MarkDefaultRole(long userId, long endpointSystemId, List<EntitlementReconcilation> entitlements, ApplicationType applicationType, AccountType accountType, EntitlementType entitlementType)
        {
            List<PresentationEndpointDefaultRole> defaultRoles = null;
            NProvisioningField provisioningFields = null;
            List<NProvisioningField> provFields = new List<NProvisioningField>();
            try
            {
                var fieldName = string.Empty;
                defaultRoles = new List<PresentationEndpointDefaultRole>();
                var user = User.GetById(userId);
                switch (applicationType)
                {
                    case ApplicationType.ADSingaporePowerLocal:
                    case ApplicationType.SAPLocal:
                        #region AD
                        provisioningFields = NProvisioningField.GetByFieldNameAndAccountType(endpointSystemId, "Groups", accountType);
                        if (applicationType == ApplicationType.ADSingaporePowerLocal && accountType == AccountType.User && user != null && user.FullTimeEmployee)
                        {
                            var exisitingGroups = (provisioningFields != null && !string.IsNullOrWhiteSpace(provisioningFields.Value)) ? provisioningFields.Value : string.Empty;
                            var matrix = ADSXLProvisioningMatrix.GetByUser(user);
                            if (matrix != null && !string.IsNullOrWhiteSpace(matrix.DefaultGroups))
                            {
                                exisitingGroups = $"{exisitingGroups},{matrix.DefaultGroups}";
                                if (provisioningFields == null)
                                {
                                    provisioningFields = new NProvisioningField();
                                    provisioningFields.FieldName = "Groups";
                                }
                                provisioningFields.Value = exisitingGroups;
                            }
                        }
                        if (provisioningFields != null && !string.IsNullOrWhiteSpace(provisioningFields.Value))
                        {
                            foreach (var item in provisioningFields.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (!string.IsNullOrWhiteSpace(item))
                                    defaultRoles.Add(new PresentationEndpointDefaultRole(GetEntitlementTypeByName(provisioningFields.FieldName), item.Trim()));
                            }
                        }
                        #endregion
                        break;
                    case ApplicationType.HRMS_FLINS:
                        break;
                    case ApplicationType.FLINS_BI:
                        break;
                    case ApplicationType.SAP_GRC:
                        break;
                    case ApplicationType.MSSL_ISU:
                    case ApplicationType.MSSL_CRM:
                    case ApplicationType.EBS_ISU:
                    case ApplicationType.EBS_CRM:
                        {
                            if (entitlementType == DIEntitlementType.Position)
                            {
                                if (entitlements.Count == 1)
                                {
                                    //MSSL EBS if its only one position then retain that
                                    entitlements[0].IsDefaultRole = true;
                                }
                            }
                        }
                        break;
                    case ApplicationType.EBS:
                    case ApplicationType.MSSL:
                        {
                            if (entitlementType == DIEntitlementType.Position)
                            {
                                if (entitlements.Count == 1)
                                {
                                    //MSSL EBS if its only one position then retain that
                                    entitlements[0].IsDefaultRole = true;
                                }
                            }
                        }
                        break;
                    case ApplicationType.SAP_EP:
                    case ApplicationType.CE:
                        #region SAPEPCE
                        fieldName = string.Empty;
                        if (entitlementType == DIEntitlementType.Role)
                        {
                            fieldName = "Roles";
                        }
                        else if (entitlementType == DIEntitlementType.Group)
                        {
                            fieldName = "Groups";
                        }
                        if (!string.IsNullOrWhiteSpace(fieldName))
                        {
                            provisioningFields = NProvisioningField.GetByFieldNameAndAccountType(endpointSystemId, fieldName, accountType);
                            if (provisioningFields != null && !string.IsNullOrWhiteSpace(provisioningFields.Value))
                            {
                                foreach (var item in provisioningFields.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    if (!string.IsNullOrWhiteSpace(item))
                                        defaultRoles.Add(new PresentationEndpointDefaultRole(GetEntitlementTypeByName(provisioningFields.FieldName), item.Trim()));
                                }
                            }
                        }
                        #endregion
                        break;
                    case ApplicationType.NPAC:
                    case ApplicationType.SRM:
                    case ApplicationType.GEMS:
                        fieldName = (applicationType == ApplicationType.GEMS || applicationType == ApplicationType.SRM) ? "Roles" : "Level";
                        provisioningFields = NProvisioningField.GetByFieldNameAndAccountType(endpointSystemId, fieldName, accountType);
                        if (provisioningFields != null && !string.IsNullOrWhiteSpace(provisioningFields.Value))
                        {
                            foreach (var item in provisioningFields.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (!string.IsNullOrWhiteSpace(item))
                                    defaultRoles.Add(new PresentationEndpointDefaultRole(GetEntitlementTypeByName(provisioningFields.FieldName), item.Trim()));
                            }
                        }
                        break;
                    case ApplicationType.VPN:
                        break;
                    case ApplicationType.Office365:
                        break;
                    case ApplicationType.OneCard:
                        #region OneCard
                        if (user != null && user.FullTimeEmployee)
                        {
                            provisioningFields = NProvisioningField.GetByFieldNameAndAccountType(endpointSystemId, "DefaultAccessLevels", accountType);
                            if (provisioningFields != null && !string.IsNullOrWhiteSpace(provisioningFields.Value))
                            {
                                foreach (var item in provisioningFields.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    if (!string.IsNullOrWhiteSpace(item))
                                        defaultRoles.Add(new PresentationEndpointDefaultRole(GetEntitlementTypeByName(provisioningFields.FieldName), item.Trim()));
                                }
                            }
                        }
                        break;
                    #endregion
                    case ApplicationType.InhouseSystems:
                        provFields = NProvisioningField.GetByFieldNamesAndAccountType(endpointSystemId, new string[] { "Permissions" }, accountType);
                        foreach (var item in provFields)
                        {
                            provisioningFields = item;
                            if (provisioningFields != null && !string.IsNullOrWhiteSpace(provisioningFields.Value))
                            {
                                foreach (var field in provisioningFields.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    if (!string.IsNullOrWhiteSpace(field))
                                        defaultRoles.Add(new PresentationEndpointDefaultRole(GetEntitlementTypeByName(provisioningFields.FieldName), field.Trim()));
                                }
                            }
                        }
                        break;
                    case ApplicationType.IDMS:
                        provFields = NProvisioningField.GetByFieldNamesAndAccountType(endpointSystemId, new string[] { "Roles", "Groups" }, accountType);
                        foreach (var item in provFields)
                        {
                            provisioningFields = item;
                            if (provisioningFields != null && !string.IsNullOrWhiteSpace(provisioningFields.Value))
                            {
                                foreach (var field in provisioningFields.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    if (!string.IsNullOrWhiteSpace(field))
                                        defaultRoles.Add(new PresentationEndpointDefaultRole(GetEntitlementTypeByName(provisioningFields.FieldName), field.Trim()));
                                }
                            }
                        }
                        var extraRoles = MasterConfigHelper.IDMSAttestationRoleToExclude;
                        extraRoles.ForEach(i=>
                        {
                            defaultRoles.Add(new PresentationEndpointDefaultRole(DIEntitlementType.Role,i));
                        });
                        break;
                    case ApplicationType.SSIDS_GUI:
                    case ApplicationType.SSIDS_SMS:
                        {
                            if (applicationType == ApplicationType.SSIDS_SMS)
                            {
                                entitlements.ForEach(entitlement =>
                                {
                                    //Added below code for New Attestation
                                    entitlement.Entitlement = "Handphone Registered";
                                    entitlement.EntitlementDescription = "Handphone Registered";
                                });
                            }
                        }
                        break;
                    case ApplicationType.EAM:
                        break;
                    case ApplicationType.MicrosoftTeams:
                        entitlements.ForEach(entitlement =>
                        {
                            if (entitlement.EntitlementType == (int)DIEntitlementType.Default)
                            {
                                entitlement.IsDefaultRole = true;
                            }
                        });
                        break;
                    default:
                        break;
                }

                if (defaultRoles != null && defaultRoles.Count > 0)
                {
                    entitlements.ForEach(entitlement =>
                    {
                        if (!string.IsNullOrWhiteSpace(entitlement.Entitlement) && defaultRoles.Exists(i => (int)i.EntitlementType == entitlement.EntitlementType &&
                        !string.IsNullOrWhiteSpace(i.DefaultRole) && i.DefaultRole.Equals(entitlement.Entitlement, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            entitlement.IsDefaultRole = true;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                provisioningFields = null;
                defaultRoles = null;
            }
        }
        private static DIEntitlementType GetEntitlementTypeByName(string fieldName)
        {
            EntitlementType type = DIEntitlementType.None;
            switch (fieldName.ToUpper())
            {
                case "ROLES":
                case "ROLE":
                    type = DIEntitlementType.Role;
                    break;
                case "GROUPS":
                case "GROUP":
                    type = DIEntitlementType.Group;
                    break;
                case "DEFAULTACCESSLEVELS":
                    type = DIEntitlementType.AccessLevel;
                    break;
                case "LEVEL":
                case "LEVELS":
                    type = DIEntitlementType.Level;
                    break;
                case "PERMISSIONS":
                    type = DIEntitlementType.Permission;
                    break;
                case "POSITION":
                    type = DIEntitlementType.Position;
                    break;
                default:
                    break;
            }
            return type;
        }

        public static List<EntitlementReconcilation> GetEntitlement(long endpointId, string accountName)
        {
            using(var tran = new DataAccessTransaction(false))
            {
                return ORMapList(new EntitlementReconcilationRepository(tran).GetEntitlements(endpointId, accountName));
            }
        }
        public static bool GetActiveAccountsForCriticalReport(long endpointId,string accountName=null)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var accounts = new EntitlementReconcilationRepository(dataAccessTransaction).GetActiveAccountsCriticalReport(endpointId, accountName).ToList();
                var rep = new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction);
                Logger.DILog.DiscoveryLogger.WriteLog(Logger.LogLevelType.Info, $"Total active User:{accounts.Count}", DiscoveryObjectType.CriticalTCodeReport);
                foreach (var username in accounts)
                {
                    //5min timeout
                    try
                    {
                        rep.ExecuteQueryWithCommandTimeOut("exec USP_CRITICALTCODEREPORT @USERNAME={0},@ENDPOINTID={1}", 300, (object)username, endpointId);
                    }
                    catch(Exception ex)
                    {
                        Logger.DILog.DiscoveryLogger.WriteLog(Logger.LogLevelType.Exception, ex.Message, DiscoveryObjectType.CriticalTCodeReport);
                    }
                }
            }
            return true;
        }

        public static bool GetActiveAccountsForSODConflictingReport(long endpointId, string accountName = null)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var accounts = new EntitlementReconcilationRepository(dataAccessTransaction).GetActiveAccountsForCriticalReport(endpointId, accountName).ToList();
                var rep = new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction);
                Logger.DILog.DiscoveryLogger.WriteLog(Logger.LogLevelType.Info, $"Total active User:{accounts.Count}", DiscoveryObjectType.ConflictingRoleReport);
                foreach (var username in accounts)
                {
                    //5min timeout
                    try
                    {
                        rep.ExecuteQueryWithCommandTimeOut("EXEC USP_SODCONFLICTING_REPORT @USERNAME={0},@ENDPOINTID={1}", 300, (object)username, endpointId);
                    }
                    catch (Exception ex)
                    {
                        Logger.DILog.DiscoveryLogger.WriteLog(Logger.LogLevelType.Exception, ex.Message, DiscoveryObjectType.ConflictingRoleReport);
                    }
                }
            }
            return true;
        }
        public static EntitlementReconcilation GetEndpointUserObjectByParameter(long endpointSystemId, long userId, string entitlement, int entitlementType)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMap(new EntitlementReconcilationRepository(tran).GetEndpointUserObjectByParameter(endpointSystemId, userId, entitlement, entitlementType));
            }
        }
        public static bool GetActiveAccountsForSAPHANACriticalReport(long endpointId, string accountName = null)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var accounts = new EntitlementReconcilationRepository(dataAccessTransaction).GetActiveAccountsCriticalReport(endpointId, accountName).ToList();
                var rep = new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction);
                Logger.DILog.DiscoveryLogger.WriteLog(Logger.LogLevelType.Info, $"Total active User:{accounts.Count}", DiscoveryObjectType.CriticalTCodeReport);
                foreach (var username in accounts)
                {
                    //5min timeout
                    try
                    {
                        rep.ExecuteQueryWithCommandTimeOut("exec USP_SAPHANA_CRITICALTCODEREPORT @USERNAME={0},@ENDPOINTID={1}", 300, (object)username, endpointId);
                    }
                    catch (Exception ex)
                    {
                        Logger.DILog.DiscoveryLogger.WriteLog(Logger.LogLevelType.Exception, ex.Message, DiscoveryObjectType.CriticalTCodeReport);
                    }
                }
            }
            return true;
        }
        public static bool GetActiveAccountsForSAPHANASODConflictingReport(long endpointId, string accountName = null)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                var accounts = new EntitlementReconcilationRepository(dataAccessTransaction).GetActiveAccountsForCriticalReport(endpointId, accountName).ToList();
                var rep = new RepositoryBase<EntityModel.EntitlementReconcilation>(dataAccessTransaction);
                Logger.DILog.DiscoveryLogger.WriteLog(Logger.LogLevelType.Info, $"Total active User:{accounts.Count}", DiscoveryObjectType.ConflictingRoleReport);
                foreach (var username in accounts)
                {
                    //5min timeout
                    try
                    {
                        rep.ExecuteQueryWithCommandTimeOut("EXEC USP_SAPHANA_SODCONFLICTING_REPORT @USERNAME={0},@ENDPOINTID={1}", 300, (object)username, endpointId);
                    }
                    catch (Exception ex)
                    {
                        Logger.DILog.DiscoveryLogger.WriteLog(Logger.LogLevelType.Exception, ex.Message, DiscoveryObjectType.ConflictingRoleReport);
                    }
                }
            }
            return true;
        }
    }
}
