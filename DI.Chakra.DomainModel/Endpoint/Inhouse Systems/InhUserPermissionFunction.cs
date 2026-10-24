using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class InhUserPermissionFunction : DomainModelBase<InhUserPermissionFunction, EntityModel.InhUserPermissionFunction>, IXLData
    {
        public long EndpointUserId { get; set; }
        public long EndPointSystemId { get; set; }
        public string InhouseSystemCode { get; set; }
        public string EmployeeId { get; set; }
        //public string NRIC { get; set; }
        public string UserName { get; set; }
        public string Permission { get; set; }
        public string Functions { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public long PermissionId { get; set; }
        public long FunctionId { get; set; }

        public static InhUserPermissionFunction GetByObject(InhUserPermissionFunction obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhUserPermissionFunction>(dataAccessTransaction).Get(cnfg => cnfg.EndpointUserId == obj.EndpointUserId && cnfg.PermissionId == obj.PermissionId).FirstOrDefault());
            }
                
        }

        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.InhUserPermissionFunction>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.InhUserPermissionFunction>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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

        public static void TruncateDI_INH_UserPermissionFunction()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DeleteAll();
            }
        }
        public static List<InhUserPermissionFunction> GetInhUserPermissionFunctionGridData(string inhouseSystemCode)
        {
                List<InhUserPermissionFunction> lstItems = null;
                if (inhouseSystemCode == "ALL")
                {
                    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                    {
                        lstItems = ORMapList(new RepositoryBase<EntityModel.InhUserPermissionFunction>(dataAccessTransaction)
                           .GetAll().ToList());
                    }
                    
                }
            else
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                {
                    lstItems = ORMapList(new RepositoryBase<EntityModel.InhUserPermissionFunction>(dataAccessTransaction)
                           .Get(objUser => objUser.InhouseSystemCode == inhouseSystemCode).ToList());
                }
            }
                    

                if(lstItems.Count > 0)
                {
                    var permissions = InhousePermission.GetAll().ToDictionary(k => k.Id, v => v.EntitlementName);
                    var inhUsers = InhouseUserInfo.GetAll().ToDictionary(k => k.Id, v => v);
                    InhouseUserInfo userInfo;
                    bool userExists = false;
                    foreach (var item in lstItems)
                    {
                        item.Permission = permissions[item.PermissionId];
                        userExists = inhUsers.TryGetValue(item.EndpointUserId, out userInfo);
                        if (userExists)
                        {
                            item.EmployeeId = userInfo.EmployeeId;
                            item.UserName = userInfo.UserName;
                        }
                    }
                }
                return lstItems;
        }
        public static bool SaveAccountReconcilation(string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                try
                {
                    var query = $"insert into DI_M_AccountReconcilation(EndpointSystemId, EndpointUserId, UserId, AccountName, FullName, CreatedDate, CreatedBy) " +
                        $"select UPF.EndpointSystemId, EndpointUserId, U.UserId, U.UserName AccountName, U.DisplayName FullName, GETDATE() CreatedDate,{currentUser} CreatedBy from DI_INH_UserPermissionFunction UPF inner join DI_INH_UserInfo U on UPF.EndpointUserId = U.Id where not exists(select 1 from DI_M_AccountReconcilation AR where AR.EndpointSystemId = UPF.EndpointSystemId and AR.EndpointUserId = UPF.EndpointUserId)";
                    new RepositoryBase<EntityModel.InhUserPermissionFunction>(dataAccessTransaction).ExecuteQuery(query);
                    return true;
                }
                catch (Exception ex)
                {
                    dataAccessTransaction.Rollback();
                    return false;
                }
            }
        }

        public static void SaveEntitlementReconcilation(string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                try
                {
                    var inhouseEndpoints = EndPoint.GetByEndpointType(EndpointType.InhouseSystems, string.Empty);
                    if (inhouseEndpoints != null && inhouseEndpoints.Count > 0)
                    {
                        var inhouseEndpointIds = inhouseEndpoints.Select(p => p.Id);
                        var accountRecons = new RepositoryBase<EntityModel.AccountReconcilation>(dataAccessTransaction).Get(p => inhouseEndpointIds.Contains(p.EndpointSystemId));
                        var inhUserPermissionFunction = GetAll();

                        foreach (var accRec in accountRecons)
                        {
                            var ep = inhouseEndpoints.Where(p => p.Id == accRec.EndpointSystemId).FirstOrDefault();
                            if (ep == null)
                                continue;

                            var permissions = inhUserPermissionFunction.Where(p => p.EndpointUserId == accRec.EndpointUserId);
                            foreach (var permission in permissions)
                            {
                                if (permission == null)
                                    continue;

                                var entRecon = EntitlementReconcilation.GetByAccountReconcilationIdAndEntitlementName(accRec.Id, permission.Permission);
                                if (entRecon == null)
                                {
                                    entRecon = new EntitlementReconcilation();
                                }

                                entRecon.AccountReconcilationId = accRec.Id;
                                entRecon.EndpointSystemId = accRec.EndpointSystemId;
                                entRecon.EndpointUserId = accRec.EndpointUserId;
                                entRecon.AccountName = accRec.AccountName;
                                entRecon.UserId = accRec.UserId;
                                entRecon.EnteredDate = DateTime.Now;
                                entRecon.IsDeleted = false;
                                entRecon.EntitlementType = Convert.ToInt32(EntitlementType.Permission);
                                entRecon.EndpointType = EndpointType.InhouseSystems;
                                entRecon.Entitlement = permission.Permission;
                                entRecon.EntitlementId = permission.PermissionId;
                                if (ep.IsFunctionLevel)
                                {
                                    entRecon.Functions = permission.Functions;
                                }

                                entRecon.Save(dataAccessTransaction);
                            }
                        }
                    }
                    //var endpointType = Convert.ToInt32(EndpointType.InhouseSystems);
                    //var entitlementType = Convert.ToInt32(EntitlementType.Permission);
                    //var query = $"insert into DI_M_EntitlementReconcilation (AccountReconcilationId, EndpointType, EndpointSystemId, EndpointUserId, UserId, EntitlementId, EntitlementType, AccountName, Entitlement, EntitlementDescription, Functions, EnteredDate, CreatedDate, CreatedBy)" +
                    //    $"select AR.Id AccountReconcilationId, {endpointType} EndpointType, AR.EndpointSystemId, AR.EndpointUserId, UserId, P.Id, {entitlementType} EntitlementType, " +
                    //    $"AccountName, P.Name Entitlement,P.Name EntitlementDescription,UPF.Functions, GETDATE() EnteredDate, GETDATE() CreatedDate, {currentUser} CreatedBy " +
                    //    $"from DI_M_AccountReconcilation AR inner join DI_INH_UserPermissionFunction UPF on AR.EndpointSystemId = UPF.EndpointSystemId and " +
                    //    $"AR.EndpointUserId = UPF.EndpointUserId inner join DI_M_EndpointSystem ES on ES.Id = AR.EndpointSystemId inner join DI_INH_Permission P " +
                    //    $"on P.EndPointSystemId = AR.EndpointSystemId and P.Id = UPF.PermissionId where ES.EndpointSystemTypeId = {endpointType} " +
                    //    $"and not exists (select 1 from DI_M_EntitlementReconcilation ER where ER.AccountReconcilationId = AR.Id)";
                    //new RepositoryBase<EntityModel.InhUserPermissionFunction>(dataAccessTransaction).ExecuteQuery(query);
                }
                catch (Exception ex)
                {
                    dataAccessTransaction.Rollback();
                }
            }
        }
        public static List<InhUserPermissionFunction> GetByEndpointUserId(long endpointUserId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InhUserPermissionFunction>(dataAccessTransaction)
                       .Get(objUser => objUser.EndpointUserId == endpointUserId && objUser.EndPointSystemId == endpointSystemId).ToList());
            }
                
        }

        public static List<InhUserPermissionFunction> GetByEndpointUserId(long endpointUserId, long endpointSystemId, long permissionId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.InhUserPermissionFunction>(dataAccessTransaction)
                       .Get(objUser => objUser.EndpointUserId == endpointUserId && objUser.EndPointSystemId == endpointSystemId && objUser.PermissionId == permissionId).ToList());
            }
                
        }

        public static InhUserPermissionFunction GetByEndpointUserIdPermissionIdFunctionId(long endpointUserId, long permissionId, long functionId, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhUserPermissionFunction>(dataAccessTransaction)
                       .Get(obj => obj.EndpointUserId == endpointUserId && obj.PermissionId == permissionId && obj.FunctionId == functionId && obj.EndPointSystemId == endpointSystemId).FirstOrDefault());
            }
                
        }

        public static InhUserPermissionFunction GetByEndpointUserIdPermissionId(long endpointUserId, long permissionId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.InhUserPermissionFunction>(dataAccessTransaction)
                       .Get(obj => obj.EndpointUserId == endpointUserId && obj.PermissionId == permissionId).FirstOrDefault());
            }
                
        }

        public static void DeleteByEndpointUserId(long endpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new RepositoryBase<EntityModel.InhUserPermissionFunction>(dataAccessTransaction).ExecuteQuery($"DELETE FROM DI_INH_UserPermissionFunction WHERE EndpointUserId = {endpointUserId}");
            }
                
        }
    }
}
