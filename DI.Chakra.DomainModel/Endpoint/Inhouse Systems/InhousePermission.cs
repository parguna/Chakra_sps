using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class InhousePermission : DomainModelBase<InhousePermission, EntityModel.InhousePermission>, IXLData, IEndpointEntitlementInfo, IApprover
    {
        public long EndPointSystemId { get; set; }
        public string InhouseSystemCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }



        public virtual EndPoint EndPoint { get; set; }

        public string BusinessRole
        {
            get { return this.Name; }
        }

        public string SystemName
        {
            get { return this.InhouseSystemCode; }
        }

        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Permission; }
        }
        public string EntitlementName
        {
            get { return Name; }
        }

        public string EntitlementDesc
        {
            get { return Description; }
        }

        public string ADGroups { get; set; }
        public string EPGroups { get; set; }
        public string EPRoles { get; set; }
        public string CEGroups { get; set; }
        public string CERoles { get; set; }
        public bool IsDeleted { get; set; }
        public static List<InhousePermission> GetByUserId(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<InhousePermission> inhousePermissions = new List<InhousePermission>();
                InhouseUserInfo inhouseUserInfo = InhouseUserInfo.GetById(userId);

                if (inhouseUserInfo != null)
                {
                    string Permission = string.Empty;
                    string[] Permissions = Permission.Split(',');
                    foreach (string p in Permissions)
                    {
                        long id = string.IsNullOrEmpty(p) ? 0 : Convert.ToInt64(p);
                        var inhPermission = ORMap(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).GetById(id));
                        if (inhPermission != null)
                        {
                            inhousePermissions.Add(inhPermission);
                        }
                    }
                }
                return inhousePermissions;
            }
        }

        public static IList<InhousePermission> GetDeletedByEndPointSystemId(long EndpointId, DateTime runDatetime, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                List<InhousePermission> listDeletedPermissions = new List<InhousePermission>();

                listDeletedPermissions = ORMapList(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction)
                       .Get(objUser => objUser.EndPointSystemId == EndpointId && (objUser.LastModifiedDate < runDatetime ||
                            (objUser.CreatedDate < runDatetime && objUser.LastModifiedDate == null)
                        )).ToList());

                return listDeletedPermissions;
            }
        }

        public static List<InhousePermission> GetInhousePermissionGridData(string inhouseSystemCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (inhouseSystemCode == "ALL")
                {
                    return ORMapList(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction)
                           .GetAll().ToList());
                }
                else
                    return ORMapList(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction)
                           .Get(objUser => objUser.InhouseSystemCode == inhouseSystemCode).ToList());
            }
        }

        public static InhousePermission GetByRoleIdIncludeFunctions(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction(false))
            {
                var inhousePermissionList = InhousePermission.GetByIdIncludeEndpointFunction(id);
                return inhousePermissionList;
            }
        }

        public static List<InhousePermission> GetINHRolesByDIUserId(long DIUserId)
        {
            List<InhousePermission> inhPermissionsList = new List<InhousePermission>();
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                List<InhouseUserInfo> inhuser = InhouseUserInfo.GetByDIUserIdIncludePermissions(DIUserId);
                if (inhuser != null)
                {
                    foreach (InhouseUserInfo info in inhuser)
                    {
                        inhPermissionsList.AddRange(info.Permissions);
                    }
                }
                return inhPermissionsList;
            }
        }

        public static Dictionary<long, string> GetByEndPointSystemId(long endPointSystemId, string searchString)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new InhousePermissionsRepository(dataAccessTransaction).GetByEndPointSystemId(endPointSystemId, CheckNull.AsteriskToString(searchString));
            }
        }

        public static Dictionary<long, string> GetByEndPointSystemIdExisting(long endPointSystemId, string searchString, long endpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new InhousePermissionsRepository(dataAccessTransaction).GetByEndPointSystemIdExisting(endPointSystemId, CheckNull.AsteriskToString(searchString), endpointUserId);
            }
        }

        public static Dictionary<long, string> GetAllPermissionsExceptExisting(long endPointSystemId, string searchString, long endpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<string> endpointAllowed = new List<string>();
                var endpoint = EndPoint.GetById(endPointSystemId);
                var data = MasterConfigHelper.InhouseAllowSamePermissionForAmendmentAdd;
                if (!string.IsNullOrWhiteSpace(data))
                {
                    var getDataByInhosueCode = data.Split(new []{ "|"}, StringSplitOptions.RemoveEmptyEntries);
                    if(getDataByInhosueCode != null && getDataByInhosueCode.Length > 0)
                    {
                        foreach (var item in getDataByInhosueCode)
                        {
                            if (!string.IsNullOrWhiteSpace(item) && endpoint.ApplicationCode.Equals(item.Trim(), StringComparison.InvariantCultureIgnoreCase))
                            {
                                endpointAllowed.Add(endpoint.ApplicationCode);
                                break;
                            }
                        }
                    }
                }
                
                return new InhousePermissionsRepository(dataAccessTransaction).GetAllPermissionsExceptExisting(endPointSystemId, CheckNull.AsteriskToString(searchString), endpointUserId, endpoint.IsFunctionLevel, endpointAllowed);
            }
        }

        public static List<InhousePermission> GetByEndPointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction)
                    .Get(ip => ip.EndPointSystemId == endPointSystemId).ToList());
            }
        }


        public static List<InhousePermission> GetPermissionWithNoAccountsReportData(long endPointSysemId)
        {

            using (var transaction = new DataAccessTransaction(false))
            {
                var permissionWithoutAccount = ORMapList(new RepositoryBase<EntityModel.InhousePermission>(transaction)
                    .Get(group => group.EndPointSystemId == endPointSysemId && group.IsActive).ToList());
                //&& group.INHUsers.Count == 0).ToList());

                return permissionWithoutAccount;
            }

        }

        public static InhousePermission GetByIdIncludesUser(long permissionId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                //return ORMap(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).
                //    Get(item => item.Id == permissionId, item => item.INHUsers).FirstOrDefault());
                return null;
            }
        }

        public static List<InhousePermission> GetByName(string permissionName, string systemCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).
                    Get(item => item.Name.ToLower().Trim() == permissionName.ToLower() && item.InhouseSystemCode.ToLower() == systemCode.ToLower()).ToList());
            }
        }

        public static InhousePermission GetByName(string permissionName, long endpointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).
                    Get(item => item.Name.ToLower().Trim() == permissionName.ToLower() && item.EndPointSystemId == endpointId).FirstOrDefault());
            }
        }

        public static InhousePermission GetByPermissionName(string permissionName, string systemCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).Get(item => item.Name.ToLower() == permissionName.ToLower() && item.InhouseSystemCode.ToLower() == systemCode.ToLower()).FirstOrDefault());
            }
        }
        public static InhousePermission GetByPermissionName(string permissionName, long epId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).Get(item => item.Name.ToLower() == permissionName.ToLower() && item.EndPointSystemId == epId).FirstOrDefault());
            }
        }
        public static List<InhousePermission> GetPermissionsByNames(string permissions, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lstPermissions = permissions.Split(',');
                return ORMapList(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).Get(item => lstPermissions.Contains(item.Name) && item.EndPointSystemId == endpointSystemId).ToList());
            }
        }
        
        public static InhousePermission GetByNameAndEndpointSystemId(string permission, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).
                    Get(inhPermission => inhPermission.Name.ToLower() == permission.ToLower()
                    && inhPermission.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public static List<InhousePermission> GetPermissionsByEndpointSystemId(string permissions, long endPointSystemId)
        {
            List<InhousePermission> PermissionList = new List<InhousePermission>();
            string[] permissionsArray = permissions.Split(',');
            foreach (string permissionName in permissionsArray)
            {
                InhousePermission permission = GetByNameAndEndpointSystemId(permissionName, endPointSystemId);
                if (permission != null)
                    PermissionList.Add(permission);
            }
            return PermissionList;
        }

        public static InhousePermission GetByObject(InhousePermission obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).Get(cnfg => cnfg.Name == obj.Name
                        && cnfg.InhouseSystemCode == obj.InhouseSystemCode).FirstOrDefault());
            }
        }

        public static InhousePermission GetByIdIncludeEndpoint(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).GetById(id, cnfg => cnfg.EndPoint));
            }
        }

        public static InhousePermission GetByCode(string code)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).Get(i => i.Name == code, cnfg => cnfg.EndPoint).FirstOrDefault());
            }
        }

        public static InhousePermission GetByIdIncludeEndpointFunction(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var roleWithFunctions = new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).GetById(id);
                return ORMap(roleWithFunctions);
            }
        }

        public static List<InhouserEntitlementCustom> GetPermissionAndFunctionWithDescription(long endpointsystemId, string entitlement)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var endpoint = EndPoint.GetById(endpointsystemId);

                if (endpoint.IsFunctionLevel)
                {
                    List<InhouserEntitlementCustom> response = new List<InhouserEntitlementCustom>();
                    var rows = entitlement.Split('|');
                    foreach (var row in rows)
                    {
                        var permissionAndFunctions = row.Split(new string[] { Constants.SPLITTER_FUCNTIONS }, StringSplitOptions.RemoveEmptyEntries);
                        if (permissionAndFunctions.Length > 0)
                        {
                            response.AddRange(new InhousePermissionsRepository(dataAccessTransaction).GetPermissionAndFunctionWithDescription(endpointsystemId, permissionAndFunctions[0], permissionAndFunctions[1].Split(',')));
                        }
                    }
                    return response;
                }
                else
                {
                    var permssions = entitlement.Split(',');
                    if (permssions.Length > 0)
                    {
                        return new InhousePermissionsRepository(dataAccessTransaction).GetPermissionWithDescription(endpointsystemId, permssions);
                    }
                }
                return null;
            }
        }

        public static void TruncateDI_INH_Permission()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                InhousePermission.DeleteAll();
            }
        }

        public static InhousePermission GetByEndPoint(InhousePermission obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new InhousePermissionsRepository(dataAccessTransaction).GetByEndPoint(obj.ORUnMap()));
            }
        }

        public static bool DoesExists(string value)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).
                    Any(ihPerm => ihPerm.Name.ToLower() == value.ToLower());
            }
        }

        public static bool DoesExistsByEndPointSystemId(string value, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).Any(ihPerm => ihPerm.Name.ToLower() == value.ToLower() && ihPerm.EndPointSystemId == endPointSystemId);
            }
        }

        public static long GetAllCount()
        {
            string cmd = $"Select count(1) FROM [DI_INH_Permission] WITH(NOLOCK)";
            var iacmOperator = new IACMDbOperator(cmd, CommandType.Text, null);
            var executor = new QueryExecutor(iacmOperator);
            object maxId = executor.ExecuteQueryObj();
            return maxId == null ? 0 : Convert.ToInt64(maxId);
        }

        public static string DeletePermissionById(long permissionId, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var inhPer = InhousePermission.GetById(permissionId);
                var endPoint = inhPer.EndPointSystemId;
                var recon = EntitlementReconcilation.GetByEntitlementId(permissionId, endPoint);
                if (recon.Count > 0)
                {
                    return "Permission can't be deleted, as user has assigned with this permission.";
                }
                else
                {
                    string cmd = string.Format("DELETE FROM [DI_INH_UserPermissionFunction] WHERE PermissionId = {0} ", permissionId);
                    ExecuteQuery(cmd);
                    Delete(permissionId);
                    return "Inhouse Permission deleted Sucessfully.";
                }

                //var permissionInfo = InhousePermission.GetByIdIncludesUser(permissionId);
                //if (permissionInfo != null && permissionInfo..Count == 0)
                //{
                //    string cmd = string.Format("DELETE FROM [InhouseUserInfoInhousePermission] WHERE InhousePermission_Id = {0} ", permissionId);
                //    InhousePermission.ExecuteQuery(cmd);

                //    cmd = string.Format("DELETE FROM [InhousePermissionInhouseFunction] WHERE InhousePermission_Id = {0} ", permissionId);
                //    InhousePermission.ExecuteQuery(cmd);

                //    InhousePermission.Delete(permissionId);

                //    var recon = EntitlementReconcilation.GetByEntitlementId(permissionId, permissionInfo.EndPointSystemId);

                //    if (recon != null && recon.Count > 0)
                //        recon.ForEach(a => { EntitlementReconcilation.DeleteEntitlementsById(a.Id, a.EndpointSystemId, currentUser); });

                //    return "Inhouse Permission deleted Sucessfully.";
                //}
                //else
                //{
                //    return "Permission can't be deleted, as user has assigned with this permission.";
                //}
            }
        }


        public static List<InhousePermission> GetByEndpointSystem(long endPointSysemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InhousePermission>(transaction)
                    .Get(entitlement => entitlement.EndPointSystemId == endPointSysemId && entitlement.IsActive == true).ToList());
            }

        }

        public static List<InhousePermission> GetByEndpointIdIncludeGroupWithNoAccounts(long endPointSysemId)
        {

            using (var transaction = new DataAccessTransaction())
            {
                //return ORMapList(new RepositoryBase<EntityModel.InhousePermission>(transaction)
                //    .Get(group => group.EndPointSystemId == endPointSysemId
                //    && group.IsActive == true
                //    && group.INHUsers.Count == 0).ToList());
                return null;
            }

        }

        public override long Save(string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    this.Id = new InhousePermissionsRepository(transaction).Insert(ORUnMap());
                }
                else
                {
                    new InhousePermissionsRepository(transaction).Update(ORUnMap());
                }

                transaction.Commit();
            }
            return 0;
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
                        // return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastCreatedDate.CreatedDate);
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    }
                    else
                        return string.Empty;
                }
            }
        }
        public static string CheckAccountExistsInDependentSystems(long permissionId, long userId, AccountType accountType, string accountName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var permission = GetById(permissionId);
                if (accountType == AccountType.User || accountType == AccountType.Secondary)
                {
                    if (!string.IsNullOrWhiteSpace(permission.ADGroups))
                    {
                        var adAccounts = AccountReconcilation.GetByAccountTypeAndApplicationType(userId, accountType, ApplicationType.ADSingaporePowerLocal);
                        if (adAccounts.Count == 0)
                            return $"This entitlement ({permission.Name}) requires account in {ApplicationType.ADSingaporePowerLocal.GetDescription()}.";
                    }
                    else if (!string.IsNullOrWhiteSpace(permission.EPGroups) || !string.IsNullOrWhiteSpace(permission.EPRoles))
                    {
                        var epAccounts = AccountReconcilation.GetByAccountTypeAndApplicationType(userId, accountType, ApplicationType.SAP_EP);
                        if (epAccounts.Count == 0)
                            return $"This entitlement ({permission.Name}) requires account in {ApplicationType.HRMS_FLINS.GetDescription()}.";
                    }
                    else if (!string.IsNullOrWhiteSpace(permission.CEGroups) || !string.IsNullOrWhiteSpace(permission.CERoles))
                    {
                        var ceAccounts = AccountReconcilation.GetByAccountTypeAndApplicationType(userId, accountType, ApplicationType.CE);
                        if (ceAccounts.Count == 0)
                            return $"This entitlement ({permission.Name}) requires account in {ApplicationType.HRMS_FLINS.GetDescription()}.";
                    }
                }
                else
                {
                    long? ownerId = null;

                    if (!string.IsNullOrWhiteSpace(permission.ADGroups))
                    {
                        ownerId = AccountReconcilation.GetActiveAccountRecon(accountName, ApplicationType.ADSingaporePowerLocal);

                        if (ownerId == null || !ownerId.HasValue)
                        {
                            return $"This entitlement ({permission.Name}) requires account in {ApplicationType.ADSingaporePowerLocal.GetDescription()}.";
                        }

                        if (!User.GetSameOUUserIDs(userId).Contains(ownerId.Value))
                        {
                            return $"This entitlement ({permission.Name}) requires account in {ApplicationType.ADSingaporePowerLocal.GetDescription()}.";
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(permission.EPGroups) || !string.IsNullOrWhiteSpace(permission.EPRoles))
                    {
                        ownerId = AccountReconcilation.GetActiveAccountRecon(accountName, ApplicationType.SAP_EP);

                        if (ownerId == null || !ownerId.HasValue)
                        {
                            return $"This entitlement ({permission.Name}) requires account in {ApplicationType.HRMS_FLINS.GetDescription()}.";
                        }

                        if (!User.GetSameOUUserIDs(userId).Contains(ownerId.Value))
                        {
                            return $"This entitlement ({permission.Name}) requires account in {ApplicationType.HRMS_FLINS.GetDescription()}.";
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(permission.CEGroups) || !string.IsNullOrWhiteSpace(permission.CERoles))
                    {
                        ownerId = AccountReconcilation.GetActiveAccountRecon(accountName, ApplicationType.CE);

                        if (ownerId == null || !ownerId.HasValue)
                        {
                            return $"This entitlement ({permission.Name}) requires account in {ApplicationType.HRMS_FLINS.GetDescription()}.";
                        }

                        if (!User.GetSameOUUserIDs(userId).Contains(ownerId.Value))
                        {
                            return $"This entitlement ({permission.Name}) requires account in {ApplicationType.HRMS_FLINS.GetDescription()}.";
                        }
                    }
                }
                return string.Empty;
            }
        }

        public static List<InhousePermission> GetActiveRolesWithADGroups()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new InhousePermissionsRepository(dataAccessTransaction).GetActiveRolesWithADGroups());
            }
        }
        public static List<InhousePermission> GetOtherSystemPermissionByEndPointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InhousePermission>(dataAccessTransaction)
                    .Get(ip => ip.EndPointSystemId == endPointSystemId && ip.IsActive == true && ip.ADGroups != null).ToList());
            }
        }
    }
}
