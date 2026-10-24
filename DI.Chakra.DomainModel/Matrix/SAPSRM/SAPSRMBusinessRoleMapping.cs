using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SAPSRMBusinessRoleMapping : DomainModelBase<DomainModel.SAPSRMBusinessRoleMapping, EntityModel.SAPSRMBusinessRoleMapping>, IXLData
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public AccountType AccountType { get; set; }
        public string BusinessRole { get; set; }
        public string POrg { get; set; }
        public string PGrp { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public String DisplayName { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string EmployeeId { get; set; }
        public bool IsValid { get; set; }
        public bool IsEditMode { get; set; }
        public string ValidationMessage { get; set; }
        
        public static SAPSRMBusinessRoleMapping GetSAPSRMBusinessRoleMappingbyObject(SAPSRMBusinessRoleMapping obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction)
                        .Get(cnfg => cnfg.BusinessRole == obj.BusinessRole && cnfg.UserId == obj.UserId && cnfg.UserName == obj.UserName && cnfg.POrg == obj.POrg && cnfg.AccountType == obj.AccountType).FirstOrDefault());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_SAPSRMBusinessRoleMapping");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static List<SAPSRMBusinessRoleMapping> GetAllSAPSRMBusinessRoleMatrix(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var res = new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).GetAll().ToList();
                    var users = User.GetAll();
                    if (isAllRecords)
                        return ORMapList(res);
                    else
                    {
                        if (isValidRecords)
                        {
                            res.ForEach(item =>
                            {
                                item.EmployeeId = users.FirstOrDefault(u => u.Id == item.UserId)?.EmployeeId;
                            });
                            return ORMapList(res);
                        }
                        else
                        {
                            res.ForEach(item =>
                            {
                                item.EmployeeId = users.FirstOrDefault(u => u.Id == item.UserId)?.EmployeeId;
                            });
                            return ORMapList(res);
                        }
                    }
                }
            }
            catch { throw; }
        }

        

        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                    //string userDisplayName = User.GetByUserName(lastModifiedDate.LastModifiedBy)?.DisplayName;
                    //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                    {
                        //string userDisplayName = User.GetByUserName(lastModifiedDate.CreatedBy)?.DisplayName;
                        //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastCreatedDate.CreatedDate);
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    }
                    else
                        return string.Empty;
                }
            }
        }

        public static Dictionary<string, string> GetSAPSRMPgrps(string BuisnessRole, string POrg)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPSRMMainRoleMatrixRepository(dataAccessTransaction).GetSAPSRMPgrps(BuisnessRole, POrg);
            }
        }

        public static Dictionary<string, string> GetSAPSRMPgrp(string BuisnessRole, AdHocActionType adhocActionType, AccountType accType, long userId, string POrg, string userName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var srmEpId = EndPoint.GetByApplicationType(ApplicationType.SRM);
                return new SAPSRMMainRoleMatrixRepository(dataAccessTransaction).GetSAPSRMPgrp(BuisnessRole, adhocActionType, accType, userId, POrg, userName, srmEpId.Id);
            }
        }
        public static List<string> GetSAPSRMEndpointUsers(long requesteeuserId, AccountType acctype)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPSRMMainRoleMatrixRepository(dataAccessTransaction).GetSAPSRMEndpointUsers(requesteeuserId, acctype).Select(srm => srm.UserName).ToList();
            }
        }

        public static List<string> GetSAPSRMPgrp(long requesteeuserId, AccountType acctype)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPSRMMainRoleMatrixRepository(dataAccessTransaction).GetSAPSRMPgrp(requesteeuserId, acctype).Select(srm => srm.UserName).ToList();
            }
        }

        public static Dictionary<string, string> GetSAPSRMPorg(string BuisnessRole, AdHocActionType adhocActionType, AccountType accType, int Userid,string userName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPSRMMainRoleMatrixRepository(dataAccessTransaction).GetSAPSRMPorg(BuisnessRole, adhocActionType, accType, Userid, userName);
            }
        }

        public static Dictionary<string, string> GetSAPSRMPorgs(string role)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPSRMMainRoleMatrixRepository(dataAccessTransaction).GetSAPSRMPorgs(role);
            }
        }

        public static List<SAPSRMBusinessRoleMapping> GetMappingByUserNameAndRole(string accountName, string businessRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item => item.UserName == accountName &&
                        (item.BusinessRole ?? string.Empty) == (businessRole ?? string.Empty)).ToList());
            }
        }
        public static List<SAPSRMBusinessRoleMapping> GetMappingByAccountTypeUserNameAndRole(string accountName, string businessRole,AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item => item.AccountType == accountType && item.IsActive && item.UserName == accountName &&
                        (item.BusinessRole ?? string.Empty) == (businessRole ?? string.Empty)).ToList());
            }
        }
        public static List<SAPSRMBusinessRoleMapping> GetMappingByUserName(string accountName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item => item.UserName.Equals(accountName, StringComparison.CurrentCultureIgnoreCase)).ToList());
            }

        }
        public static List<SAPSRMBusinessRoleMapping> GetMappingByActiveUserName(string accountName,AccountType acounType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item => item.AccountType == acounType && item.IsActive && item.UserName.Equals(accountName, StringComparison.CurrentCultureIgnoreCase)).ToList());
            }

        }
        public static SAPSRMBusinessRoleMapping GetFirstMappingByUserName(string accountName)
        {

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item => item.UserName.Equals(accountName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault());

            }
        }

        public static SAPSRMBusinessRoleMapping GetByUserId(long userId, string businessRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item => item.UserId == userId && item.BusinessRole == businessRole).FirstOrDefault());
            }
        }

        public static SAPSRMBusinessRoleMapping GetByAccountName(AccountType accountType, string accountName, string businessRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item => item.UserName == accountName && item.AccountType == accountType && item.BusinessRole == businessRole).FirstOrDefault());
            }
        }

        public static List<string> GetAccountNamesByUserIdAndAccountType(long userId, AccountType accountType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item => item.UserId == userId && item.AccountType == accountType).Select(item => item.UserName).Distinct().ToList();
            }
        }
        public static void DeleteMappingByUserId(long userId, AccountType accountType)
        {
            List<SAPSRMBusinessRoleMapping> mappings = new List<SAPSRMBusinessRoleMapping>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                mappings = ORMapList(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item => item.UserId == userId && item.AccountType == accountType).ToList());
                if (mappings != null)
                {
                    foreach (SAPSRMBusinessRoleMapping item in mappings)
                    {
                        item.Delete();
                    }
                }
            }

            //if (mappings != null)
            //{
            //    foreach (SAPSRMBusinessRoleMapping item in mappings)
            //    {
            //        using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            //        {
            //            item.Delete();
            //        }
            //    }
            //}
        }

        public static void DeleteMappingByUserName(string userName, AccountType accountType)
        {
            List<SAPSRMBusinessRoleMapping> mappings = new List<SAPSRMBusinessRoleMapping>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                mappings = ORMapList(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item => item.UserName == userName && item.AccountType == accountType).ToList());
                if (mappings != null)
                {
                    foreach (SAPSRMBusinessRoleMapping item in mappings)
                    {
                        item.Delete();
                    }
                }
            }
        }
        public static void DeleteMappingByUserNameandRole(string userName, string roleName)
        {
            List<SAPSRMBusinessRoleMapping> mappings = new List<SAPSRMBusinessRoleMapping>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                mappings = ORMapList(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item => item.UserName == userName && item.BusinessRole == roleName).ToList());
                if (mappings != null)
                {
                    foreach (SAPSRMBusinessRoleMapping item in mappings)
                    {
                        item.Delete();
                    }
                }
            }

            //if (mappings != null)
            //{
            //    foreach (SAPSRMBusinessRoleMapping item in mappings)
            //    {
            //        using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            //        {
            //            item.Delete();
            //        }
            //    }
            //}
        }
        public static void DeleteMappingByUserName(string accountName, string roleName)
        {
            try
            {
                SAPSRMBusinessRoleMapping mapping = GetFirstMappingByUserName(accountName);

                SAPSRMBusinessRoleMapping.DeleteMappingByUserNameandRole(accountName, roleName);

                //var mappings = GetMappingByUserName(accountName);

                //if (mappings.Count == 0)
                //{
                //    if (mapping != null)
                //    {
                //        SAPSRMBusinessRoleMapping dummyMapping = new SAPSRMBusinessRoleMapping()
                //        {
                //            AccountType = mapping.AccountType,
                //            UserId = mapping.UserId,
                //            UserName = mapping.UserName,
                //            IsActive = true,
                //            CreatedBy = mapping.CreatedBy,
                //            CreatedDate = mapping.CreatedDate,
                //            LastModifiedBy = mapping.LastModifiedBy,
                //            LastModifiedDate = mapping.LastModifiedDate
                //        };

                //        dummyMapping.Save("System");
                //    }
                //}
            }
            catch
            {
                throw;
            }
        }
        public static List<SAPSRMBusinessRoleMapping> GetMappingByUserNameAndRole(string accountName, string businessRole, string pOrg)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item =>
                item.UserName == accountName &&
                        (item.BusinessRole ?? string.Empty) == (businessRole ?? string.Empty) &&
                         (item.POrg ?? string.Empty) == (pOrg ?? string.Empty)).ToList());
            }
        }

        public static SAPSRMBusinessRoleMapping GetMappingByUserNameAndRolepOrg(string accountName, string businessRole, string pOrg)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item =>
                item.UserName == accountName &&
                        (item.BusinessRole ?? string.Empty) == (businessRole ?? string.Empty) &&
                         (item.POrg ?? string.Empty) == (pOrg ?? string.Empty)).FirstOrDefault());
            }
        }
        public static SAPSRMBusinessRoleMapping GetMappingByUserNameAndRolepOrg(AccountType accountType, string accountName, string businessRole, string pOrg)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction).Get(item =>
                item.UserName == accountName && item.AccountType == accountType &&
                        (item.BusinessRole ?? string.Empty) == (businessRole ?? string.Empty) &&
                         (item.POrg ?? string.Empty) == (pOrg ?? string.Empty)).FirstOrDefault());
            }
        }
        public static void SaveAll(List<SAPSRMBusinessRoleMapping> domainModelList)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(domainModelList.FirstOrDefault().CreatedBy))
            {
                foreach (var domainModel in domainModelList)
                {
                    domainModel.Save(dataAccessTransaction);
                }
                dataAccessTransaction.Commit();
            }
        }

        public static List<SAPSRMBusinessRoleMapping> GetSRMDeletedUsers(Process objDiscoveryProcess)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<SAPSRMBusinessRoleMapping> listDeletedUsers = new List<SAPSRMBusinessRoleMapping>();
                if (objDiscoveryProcess != null)
                {
                    listDeletedUsers = ORMapList(new RepositoryBase<EntityModel.SAPSRMBusinessRoleMapping>(dataAccessTransaction)
                       .Get(objUser => objUser.LastModifiedDate < objDiscoveryProcess.ProcessStartAt ||
                       (objUser.CreatedDate < objDiscoveryProcess.ProcessStartAt && objUser.LastModifiedDate == null)
                        ).ToList());
                }
                return listDeletedUsers;
            }
        }
        public static List<Dto.PresentationUserBusinessRole> GetSAPSRMUserRoles(AdHocActionType adhocActionType, AccountType accType, long userId,long endpointId, string username, string businessRole = null)
        {
            List<Dto.PresentationUserBusinessRole> roles = new List<Dto.PresentationUserBusinessRole>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var repo = new SAPSRMMainRoleMatrixRepository(dataAccessTransaction);
                var BuisnessRoles = repo.GetSAPSRMRoles(adhocActionType, userId, businessRole, accType, username, endpointId);
                foreach (var BuisnessRole in BuisnessRoles)
                {
                    var role = new Dto.PresentationUserBusinessRole(BuisnessRole.Value, BuisnessRole.Key);
                    var bRole = role.Value;
                    var pOrgs = new SAPSRMMainRoleMatrixRepository(dataAccessTransaction).GetSAPSRMPorg(BuisnessRole.Value, adhocActionType, accType, Convert.ToInt32(userId), username);
                    //new SAPSRMMainRoleMatrixRepository(dataAccessTransaction).GetSAPSRMPorgs(bRole);
                    foreach (var pOrg in pOrgs)
                    {
                        var orgDetail = new Dto.PresentationSRMPOrg(pOrg.Key, pOrg.Value);
                        var pGroups = new SAPSRMMainRoleMatrixRepository(dataAccessTransaction).GetSAPSRMPgrp(bRole, adhocActionType, accType, userId, pOrg.Value,username, endpointId);
                        foreach (var pGroup in pGroups)
                        {
                            var group = new Dto.PresentationSRMPGroup(pGroup.Key, pGroup.Value);
                            orgDetail.PGroups.Add(group);
                        }
                        role.POrgs.Add(orgDetail);
                    }
                    roles.Add(role);
                }
            }
            return roles;
        }
        public static List<PresentationUserEntitlement> GetSRMBusinessRole(long accountReconId)
        {
            string[] srmDelimiter = new[] { ","};
            List<PresentationUserEntitlement> usrEntitlements = new List<PresentationUserEntitlement>();
            List<EntityModel.EntitlementReconcilation> entitlements = new List<EntityModel.EntitlementReconcilation>();
            using (DataAccessTransaction tran = new DataAccessTransaction(false))
            {
                entitlements = new EntitlementReconcilationRepository(tran).GetSRMBusinessRoles(accountReconId);
            }
            if (entitlements != null && entitlements.Count > 0)
            {
                PresentationUserEntitlement usrEntitlement = null;
                foreach (var entitlement in entitlements)
                {
                    if (!string.IsNullOrWhiteSpace(entitlement.Functions))
                    {
                        var spRoles = entitlement.Functions.Split(srmDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        if (spRoles != null && spRoles.Length > 0)
                        {
                            foreach (var spRole in spRoles)
                            {
                                if (!string.IsNullOrWhiteSpace(spRole))
                                {
                                    var name = entitlement.Entitlement;
                                    if (!string.IsNullOrWhiteSpace(entitlement.EntitlementDescription))
                                    {
                                        name = $"{name} PurchaseOrg:{entitlement.EntitlementDescription} PurchaseGroup:{spRole}";
                                    }
                                    usrEntitlement = new PresentationSAPUserEntitlement();
                                    usrEntitlement.Id = entitlement.Id;
                                    usrEntitlement.EntitlementId = entitlement.Id;
                                    usrEntitlement.EndpointUserId = entitlement.EndpointUserId;
                                    usrEntitlement.UserName = entitlement.AccountName;
                                    usrEntitlement.ValidTo = entitlement.ValidToDate;
                                    usrEntitlement.ValidFrom = entitlement.ValidFromDate;
                                    usrEntitlement.IsSensitiveAccess = entitlement.IsSensitiveAccess;
                                    usrEntitlement.IsSODConflict = entitlement.IsSODConflict;
                                    usrEntitlement.Name = name;
                                    usrEntitlement.EndpointSystemId = entitlement.EndpointSystemId;
                                    usrEntitlement.Description = entitlement.Entitlement;
                                    //POrg
                                    usrEntitlement.Remarks5 = entitlement.EntitlementDescription;
                                    //Pgrp
                                    usrEntitlement.Remarks6 = spRole;
                                    usrEntitlements.Add(usrEntitlement);
                                }
                            }
                        }
                        else
                        {
                            usrEntitlement = new PresentationSAPUserEntitlement();
                            usrEntitlement.Id = entitlement.Id;
                            usrEntitlement.EntitlementId = entitlement.EntitlementId;
                            usrEntitlement.EndpointUserId = entitlement.EndpointUserId;
                            usrEntitlement.UserName = entitlement.AccountName;
                            usrEntitlement.ValidTo = entitlement.ValidToDate;
                            usrEntitlement.ValidFrom = entitlement.ValidFromDate;
                            usrEntitlement.IsSensitiveAccess = entitlement.IsSensitiveAccess;
                            usrEntitlement.IsSODConflict = entitlement.IsSODConflict;
                            usrEntitlement.Name = entitlement.Entitlement;
                            usrEntitlement.EndpointSystemId = entitlement.EndpointSystemId;
                            usrEntitlement.Description = entitlement.Entitlement;
                            usrEntitlements.Add(usrEntitlement);
                        }
                    }
                    else
                    {
                        usrEntitlement = new PresentationSAPUserEntitlement();
                        usrEntitlement.Id = entitlement.Id;
                        usrEntitlement.EntitlementId = entitlement.EntitlementId;
                        usrEntitlement.EndpointUserId = entitlement.EndpointUserId;
                        usrEntitlement.UserName = entitlement.AccountName;
                        usrEntitlement.ValidTo = entitlement.ValidToDate;
                        usrEntitlement.ValidFrom = entitlement.ValidFromDate;
                        usrEntitlement.IsSensitiveAccess = entitlement.IsSensitiveAccess;
                        usrEntitlement.IsSODConflict = entitlement.IsSODConflict;
                        usrEntitlement.Name = entitlement.Entitlement;
                        usrEntitlement.EndpointSystemId = entitlement.EndpointSystemId;
                        usrEntitlement.Description = entitlement.Entitlement;
                        usrEntitlements.Add(usrEntitlement);
                    }
                }
            }
            return usrEntitlements.Distinct().ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="accountName"></param>
        /// <param name="businessRoleName"></param>
        /// <param name="pOrgpGroup">Value must be POrg-PGrp example:H-T1</param>
        /// <returns></returns>
        public static List<PresentationUserEntitlement> GetSRMTechnicalRole(EndPoint endPoint,string accountName, string businessRoleName,string pOrg,string pGroup)
        {
            string[] srmDelimiter = new[] { "," };
           
            List<PresentationUserEntitlement> usrEntitlements = new List<PresentationUserEntitlement>();
            List<EntityModel.EntitlementReconcilation> entitlements = new List<EntityModel.EntitlementReconcilation>();
            List<SAPRoleInfo> roles = new List<SAPRoleInfo>();
            List<EndPoint> endPoints = new List<EndPoint>();
            List<EntitlementReconcilation> adEntitlementRecons = new List<EntitlementReconcilation>();
            var epId = endPoint.Id;
            using (DataAccessTransaction tran = new DataAccessTransaction(false))
            {
                entitlements = new EntitlementReconcilationRepository(tran).GetSRMTechRoleByBusinessRoles(businessRoleName, pOrg, pGroup);
            }
            if (entitlements != null && entitlements.Count > 0)
            {
                var applnTypes = new List<ApplicationType>() { ApplicationType.SAP_SRP, ApplicationType.SAP_SUS, ApplicationType.FLINS_BI };
                endPoints = EndPoint.GetByApplicationTypes(applnTypes);
                roles = SAPRoleInfo.GetListByUserName(accountName, string.Join(",", applnTypes.Select(i => (int)i)));
                var adep = EndPoint.GetByApplicationType(ApplicationType.ADSingaporePowerLocal);
                adEntitlementRecons = EntitlementReconcilation.GetEntitlement(adep.Id, accountName);
                endPoints.Add(adep);
                string[] mEnts = new string[] { };
                foreach (var entitlement in entitlements)
                {
                    PresentationSAPUserEntitlement usrEntitlement = new PresentationSAPUserEntitlement();
                    var srpEp = endPoints.First(j => j.ApplicationType == ApplicationType.SAP_SRP);
                    var supEp = endPoints.First(j => j.ApplicationType == ApplicationType.SAP_SUS);
                    var fBIEp = endPoints.First(j => j.ApplicationType == ApplicationType.FLINS_BI);
                    //SRP Role
                    if (!string.IsNullOrWhiteSpace(entitlement.Entitlement))
                    {
                        mEnts = entitlement.Entitlement.Split(srmDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        if (mEnts != null)
                            foreach (var item in mEnts)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                var sapRole = roles.FirstOrDefault(i => i.EndPointSystemId == srpEp.Id && i.ROLE_CODE.Equals(item, StringComparison.InvariantCultureIgnoreCase));
                                var srp = ApplicationType.SAP_SRP.GetDescription();
                                usrEntitlement = new PresentationSAPUserEntitlement();
                                usrEntitlement.EndpointSystemId = epId;
                                usrEntitlement.Id = entitlement.Id;
                                usrEntitlement.EntitlementId = entitlement.Id;
                                usrEntitlement.UserName = accountName;
                                usrEntitlement.EndpointSystemId = epId;
                                usrEntitlement.EndPointSystemType = EndpointType.SRM;
                                usrEntitlement.Name = item;
                                usrEntitlement.Description = (sapRole != null && !string.IsNullOrWhiteSpace(sapRole.EntitlementDesc)) ? sapRole.EntitlementDesc : sapRole.ROLE_CODE;
                                usrEntitlement.Remarks1 = srp;
                                usrEntitlements.Add(usrEntitlement);
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(entitlement.EntitlementDescription))
                    {
                        //SUS Role
                        mEnts = entitlement.EntitlementDescription.Split(srmDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var sup = ApplicationType.SAP_SUS.GetDescription();
                        if (mEnts != null)
                            foreach (var item in mEnts)
                        {
                            var sapRole = roles.FirstOrDefault(i => i.EndPointSystemId == supEp.Id && i.ROLE_CODE.Equals(item, StringComparison.InvariantCultureIgnoreCase));

                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                usrEntitlement = new PresentationSAPUserEntitlement();
                                usrEntitlement.EndpointSystemId = epId;
                                usrEntitlement.Id = entitlement.Id;
                                usrEntitlement.EntitlementId = entitlement.Id;
                                usrEntitlement.UserName = accountName;
                                usrEntitlement.EndpointSystemId = epId;
                                usrEntitlement.EndPointSystemType = EndpointType.SRM;
                                usrEntitlement.Name = item;
                                usrEntitlement.Description = (sapRole != null && !string.IsNullOrWhiteSpace(sapRole.EntitlementDesc)) ? sapRole.EntitlementDesc : sapRole.ROLE_CODE;
                                usrEntitlement.Remarks1 = sup;
                                usrEntitlements.Add(usrEntitlement);
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(entitlement.ProvisionedThrough))
                    {
                        //ADGroups
                        mEnts = entitlement.ProvisionedThrough.Split(srmDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        var ad = ApplicationType.ADSingaporePowerLocal.GetDescription();
                        if (mEnts != null)
                            foreach (var item in mEnts)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                var adEntitlement = adEntitlementRecons.FirstOrDefault(i => i.Entitlement.Equals(item, StringComparison.InvariantCultureIgnoreCase));

                                usrEntitlement = new PresentationSAPUserEntitlement();
                                usrEntitlement.EndpointSystemId = epId;
                                usrEntitlement.Id = entitlement.Id;
                                usrEntitlement.EntitlementId = entitlement.Id;
                                usrEntitlement.UserName = accountName;
                                usrEntitlement.EndpointSystemId = epId;
                                usrEntitlement.EndPointSystemType = EndpointType.SRM;
                                usrEntitlement.Name = item;
                                usrEntitlement.Description = (adEntitlement != null && !string.IsNullOrWhiteSpace(adEntitlement.EntitlementDescription)) ? adEntitlement.EntitlementDescription : adEntitlement.Entitlement;
                                usrEntitlement.Remarks1 = ApplicationType.ADSingaporePowerLocal.GetDescription();
                                usrEntitlements.Add(usrEntitlement);
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(entitlement.Functions))
                    {
                        //BIRoles
                        mEnts = entitlement.Functions.Split(srmDelimiter, StringSplitOptions.RemoveEmptyEntries);
                        if(mEnts != null)
                        {
                            var flinBI = ApplicationType.FLINS_BI.GetDescription();
                            foreach (var item in mEnts)
                            {
                                if (!string.IsNullOrWhiteSpace(item))
                                {
                                    var sapRole = roles.FirstOrDefault(i => i.EndPointSystemId == fBIEp.Id && i.ROLE_CODE.Equals(item, StringComparison.InvariantCultureIgnoreCase));
                                    usrEntitlement = new PresentationSAPUserEntitlement();
                                    usrEntitlement.EndpointSystemId = epId;
                                    usrEntitlement.Id = entitlement.Id;
                                    usrEntitlement.EntitlementId = entitlement.Id;
                                    usrEntitlement.UserName = accountName;
                                    usrEntitlement.EndpointSystemId = epId;
                                    usrEntitlement.EndPointSystemType = EndpointType.SRM;
                                    usrEntitlement.Name = item;
                                    usrEntitlement.Description = (sapRole != null && !string.IsNullOrWhiteSpace(sapRole.EntitlementDesc)) ? sapRole.EntitlementDesc : sapRole.ROLE_CODE;
                                    usrEntitlement.Remarks1 = flinBI;
                                    usrEntitlements.Add(usrEntitlement);
                                }
                            }
                        }
                    }
                }
            }
            return usrEntitlements.Distinct().OrderBy(i=>i.Remarks1).ThenBy(i=> i.Name).ToList();
        }
    }
}
