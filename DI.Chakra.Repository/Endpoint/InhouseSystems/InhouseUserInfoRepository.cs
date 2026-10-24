using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Logger;
using System.Data.SqlClient;
using System.Data;
using System.Linq.Dynamic;

namespace DI.Chakra.Repository
{
    public class InhouseUserInfoRepository : RepositoryBase<InhouseUserInfo>
    {
        public InhouseUserInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<PresentationInhouseUserInfo> GetInhouseUserInfoBySystemCode(string inhousesystemcode)
        {
            try
            {
                var endpoint = from ep in CurrentContext.EndPoint //.Include("EndPointSystemType")
                               where ep.EndpointSystemTypeId == (int)EndpointType.InhouseSystems
                               select ep;

                var List = (from INHUsr in CurrentContext.InhouseUserInfos
                            join accRecon in CurrentContext.AccountReconcilations on INHUsr.Id equals accRecon.EndpointUserId into accRecons
                            from accRecon in accRecons.DefaultIfEmpty()
                            join ep in endpoint
                            on accRecon.EndpointSystemId equals ep.Id
                            where accRecon.IsDeleted == false && accRecon.IsLocked == false && INHUsr.EndPointSystemId == accRecon.EndpointSystemId
                            select new PresentationInhouseUserInfo
                            {
                                ReconciliationId = accRecon.Id,
                                Id = INHUsr.Id,
                                UserName = INHUsr.UserName,
                                DisplayName = INHUsr.DisplayName,
                                InhouseSystemCode = INHUsr.InhouseSystemCode,
                                NRIC = INHUsr.NRIC,
                                Email = INHUsr.Email,
                                EmployeeId = INHUsr.EmployeeId,
                                AccountCreatedDate = INHUsr.AccountCreatedDate,
                                CreatedBy = INHUsr.CreatedBy,
                                CreatedDate = INHUsr.CreatedDate,
                                LastLoginDate = INHUsr.LastLoginDate,
                                LastModifiedBy = INHUsr.LastModifiedBy,
                                IsDisabled = INHUsr.IsDisabled,
                                DisabledDate = INHUsr.DisabledDate,
                                IsDeleted = accRecon.IsDeleted,
                                IsLocked = accRecon.IsLocked,
                                DeletedDate = accRecon.DeletedDate,
                                AccountType = accRecon.AccountType,
                                LastModifiedDate = INHUsr.LastModifiedDate,
                                Remarks1 = INHUsr.Remarks1,
                                Remarks2 = INHUsr.Remarks2,
                                Remarks3 = INHUsr.Remarks2,
                                Remarks4 = INHUsr.Remarks2,
                                Remarks5 = INHUsr.Remarks2,
                                IsMapped = (accRecon.UserId != null) ? "Yes" : "No",
                            });

                if (!string.IsNullOrWhiteSpace(inhousesystemcode))
                {
                    return List.Where(t => t.InhouseSystemCode == inhousesystemcode).ToList();
                }
                return List.ToList();
            }
            catch
            {
                throw;
            }
        }
        public List<PresentationInhouseUserInfo> GetByInhousesystemcode(string inhousesystemcode)
        {

            var list = (from INHUsr in CurrentContext.InhouseUserInfos
                        join accountRecon in CurrentContext.AccountReconcilations on INHUsr.Id equals accountRecon.EndpointUserId
                        join endpointSystem in CurrentContext.EndPoint on accountRecon.EndpointSystemId equals endpointSystem.Id
                        join diUsers in CurrentContext.User on accountRecon.UserId equals diUsers.Id into owners
                        from o in owners.DefaultIfEmpty()
                        where INHUsr.IsDeleted == false
                        select new PresentationInhouseUserInfo
                        {
                            UserId = INHUsr.Id,
                            DIUserID = o.Id,
                            UserName = INHUsr.UserName,
                            DisplayName = INHUsr.DisplayName,
                            NRIC = INHUsr.NRIC,
                            Email = INHUsr.Email,
                            EmployeeId = o.EmployeeId,
                            AccountCreatedDate = INHUsr.AccountCreatedDate,
                            ApplicationOwner = INHUsr.DisplayName,
                            SystemCode = INHUsr.InhouseSystemCode,
                            CreatedBy = INHUsr.CreatedBy,
                            CreatedDate = INHUsr.CreatedDate,
                            LastLoginDate = INHUsr.LastLoginDate,
                            LastModifiedBy = INHUsr.LastModifiedBy,
                            IsDisabled = INHUsr.IsDisabled,
                            IsDeleted = INHUsr.IsDeleted,
                            LastModifiedDate = INHUsr.LastModifiedDate,
                            IsMapped = (o.Id != null) ? "Yes" : "No",
                            Owner = (o.Id != null) ? o.DisplayName : ""
                        }).Where(t => t.SystemCode == inhousesystemcode).ToList();

            return list;
        }

        public List<PresentationInhouseUserInfo> GetByEndpointIdAndAccountWithNoGroups(long endPointSystemId)
        {
            return (from INHUsr in CurrentContext.InhouseUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on INHUsr.Id equals accountRecon.EndpointUserId
                    join endpointSystem in CurrentContext.EndPoint on accountRecon.EndpointSystemId equals endpointSystem.Id
                    join diUsers in CurrentContext.User on accountRecon.UserId equals diUsers.Id into owners
                    from o in owners.DefaultIfEmpty()
                    where INHUsr.EndPointSystemId == endPointSystemId && accountRecon.EndpointSystemId == endPointSystemId
                    && !INHUsr.IsDisabled && !INHUsr.IsDeleted && INHUsr.Permissions.Count == 0
                    select new PresentationInhouseUserInfo
                    {
                        UserId = INHUsr.Id,
                        DIUserID = o.Id,
                        UserName = INHUsr.UserName,
                        DisplayName = INHUsr.DisplayName,
                        NRIC = INHUsr.NRIC,
                        Email = INHUsr.Email,
                        EmployeeId = o.EmployeeId,
                        AccountCreatedDate = INHUsr.AccountCreatedDate,
                        ApplicationOwner = INHUsr.DisplayName,
                        SystemCode = INHUsr.InhouseSystemCode,
                        CreatedBy = INHUsr.CreatedBy,
                        CreatedDate = INHUsr.CreatedDate,
                        LastLoginDate = INHUsr.LastLoginDate,
                        LastModifiedBy = INHUsr.LastModifiedBy,
                        IsDisabled = INHUsr.IsDisabled,
                        IsDeleted = INHUsr.IsDeleted,
                        LastModifiedDate = INHUsr.LastModifiedDate,
                        IsMapped = (o.Id != null) ? "Yes" : "No",
                        Owner = (o.Id != null) ? o.DisplayName : ""
                    }).ToList();
        }

        public List<PresentationInhouseUserInfo> GetByEndpointSystemIdIncludeEndPointSystem(long endPointSystemId)
        {
            return (from INHUsr in CurrentContext.InhouseUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on INHUsr.Id equals accountRecon.EndpointUserId
                    join endpointSystem in CurrentContext.EndPoint on accountRecon.EndpointSystemId equals endpointSystem.Id
                    join diUsers in CurrentContext.User on accountRecon.UserId equals diUsers.Id into owners
                    from o in owners.DefaultIfEmpty()
                    join nOrg in CurrentContext.NOrganisations on o.NOrganisationId equals nOrg.Id into org
                    from ownerOrg in org.DefaultIfEmpty()
                    where INHUsr.EndPointSystemId == endPointSystemId && accountRecon.EndpointSystemId == endPointSystemId
                    select new PresentationInhouseUserInfo
                    {
                        UserId = INHUsr.Id,
                        DIUserID = o.Id,
                        UserName = INHUsr.UserName,
                        DisplayName = INHUsr.DisplayName,
                        NRIC = INHUsr.NRIC,
                        Email = INHUsr.Email,
                        EmployeeId = o.EmployeeId,
                        AccountCreatedDate = INHUsr.AccountCreatedDate,
                        ApplicationOwner = INHUsr.DisplayName,
                        SystemCode = INHUsr.InhouseSystemCode,
                        CreatedBy = INHUsr.CreatedBy,
                        CreatedDate = INHUsr.CreatedDate,
                        LastLoginDate = INHUsr.LastLoginDate,
                        LastModifiedBy = INHUsr.LastModifiedBy,
                        IsDisabled = INHUsr.IsDisabled,
                        IsDeleted = INHUsr.IsDeleted,
                        LastModifiedDate = INHUsr.LastModifiedDate,
                        IsMapped = (o.Id != null) ? "Yes" : "No",
                        Owner = (o.Id != null) ? o.DisplayName : ""
                    }).ToList();

        }

        public void TruncateDI_INH_UserInfo()
        {
            string cmd = "DELETE FROM DI_INH_UserInfo ";
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
            CurrentContext.SaveChanges();

        }
        public InhouseUserInfo GetByEndpoint(InhouseUserInfo obj)
        {
            var tmpInhUser = (from inhUsr in CurrentContext.InhouseUserInfos.Include("Permissions")
                              where inhUsr.InhouseSystemCode == obj.InhouseSystemCode
                              && inhUsr.UserName == obj.UserName
                              select inhUsr).FirstOrDefault();

            var inhUser = (from inhUsr in this.CurrentContext.InhouseUserInfos.Include("Permissions")
                           join ePoint in this.CurrentContext.EndPoint on inhUsr.EndPointSystemId equals ePoint.Id
                           where inhUsr.InhouseSystemCode == obj.InhouseSystemCode && ePoint.ApplicationCode == obj.InhouseSystemCode
                           && inhUsr.UserName == obj.UserName
                           select inhUsr).FirstOrDefault();

            return inhUser;
        }

        public override long Insert(InhouseUserInfo inhouseUser)
        {
            //if (inhouseUser.Permissions.Count > 0)
            //{
            //    var selectedPermission = inhouseUser.Permissions.ToList();
            //    inhouseUser.Permissions.Clear();
            //    foreach (var per in selectedPermission)
            //    {
            //        InhousePermission permission = this.CurrentContext.InhousePermissions.Find(per.Id);
            //        if (permission != null)
            //            inhouseUser.Permissions.Add(permission);
            //    }
            //}
            return base.Insert(inhouseUser);

        }
        public override void Update(InhouseUserInfo inhouseUser)
        {
            InhouseUserInfo dbinhouseUser = this.GetById(inhouseUser.Id, r => r.Permissions);

            //uncomment the below line, if you are looking for single row for user and its permission
            //List<InhousePermission> deletedPermission = dbinhouseUser.Permissions.Except(inhouseUser.Permissions, new IdBaseEqualityComparer<InhousePermission>()).ToList();
            //deletedPermission.ForEach(epobj => dbinhouseUser.Permissions.Remove(epobj));

            List<InhousePermission> addedPermission = inhouseUser.Permissions.Except(dbinhouseUser.Permissions, new IdBaseEqualityComparer<InhousePermission>()).ToList();

            foreach (InhousePermission permission in addedPermission)
            {
                if (CurrentContext.Entry(permission).State == System.Data.Entity.EntityState.Detached)
                    CurrentContext.InhousePermissions.Attach(permission);

                dbinhouseUser.Permissions.Add(permission);
            }

            if (inhouseUser.Permissions == null || inhouseUser.Permissions.Count == 0)
            {
                dbinhouseUser.Permissions = new List<InhousePermission>();
            }

            base.Update(inhouseUser);
        }

        public List<InhouseUserInfo> GetByDIUserId(long diUserId)
        {
            return (from inhouseUser in CurrentContext.InhouseUserInfos.Include("EndPoint").Include("EndPointSystemType")
                    join accountRecon in CurrentContext.AccountReconcilations on inhouseUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diUserId && inhouseUser.IsDeleted == false && inhouseUser.IsDisabled == false && accountRecon.IsLocked == false
                    select inhouseUser).ToList();
        }

        public List<InhouseUserInfo> GetByDIUserIdIncludePermissions(long id)
        {
            return (from inhouseUser in CurrentContext.InhouseUserInfos
                    from inusr in inhouseUser.Permissions
                    join accountRecon in CurrentContext.AccountReconcilations on inhouseUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == id && inhouseUser.IsDisabled == false
                    select inhouseUser).ToList();
        }

        public List<InhouseUserInfo> GetUnlockedAccountsByDIUserId(long userId)
        {
            return (from inhouseUser in CurrentContext.InhouseUserInfos.Include("Permissions")
                    join accountRecon in CurrentContext.AccountReconcilations on inhouseUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == userId && accountRecon.IsLocked == false && inhouseUser.EndPointSystemId == accountRecon.EndpointSystemId
                    select inhouseUser).ToList();
        }

        public List<InhouseUserInfo> GetActiveAccountsByDIUserId(long userId)
        {
            return (from inhouseUser in CurrentContext.InhouseUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on inhouseUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == userId && inhouseUser.IsDisabled == false && accountRecon.IsLocked == false
                    select inhouseUser).ToList();
        }
        public List<InhouseUserInfo> GetByDIUserId(long userId, long endpointSystemId)
        {
            return (from inhouseUser in CurrentContext.InhouseUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on inhouseUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == userId && accountRecon.EndpointSystemId == endpointSystemId && accountRecon.IsLocked == false
                    select inhouseUser).ToList();
        }

        public List<InhouseUserInfo> GetByUserIdAndAccountType(long userId, long endpointSystemId, AccountType accountType)
        {
            return (from inhouseUser in CurrentContext.InhouseUserInfos
                    join accountRecon in CurrentContext.AccountReconcilations on inhouseUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == userId && accountRecon.EndpointSystemId == endpointSystemId && accountRecon.IsLocked == false && accountRecon.AccountType == accountType
                    select inhouseUser).ToList();
        }


        public InhouseUserInfo GetByDIUserIdAndEndPointSystemId(long diUserId, long endpointSystemId)
        {
            return (from inhouseUser in CurrentContext.InhouseUserInfos
                    from inusr in inhouseUser.Permissions
                    join accountRecon in CurrentContext.AccountReconcilations on inhouseUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diUserId && accountRecon.EndpointSystemId == endpointSystemId
                    select inhouseUser).FirstOrDefault();
        }

        public IList<PresentationInhouseUserToPermission> GetUserToPermissionsReportData(long endPointSystemId)
        {
            var userPermissions = (from inhUser in CurrentContext.InhouseUserInfos
                                   from inhGrp in inhUser.Permissions
                                   where inhUser.EndPointSystemId == endPointSystemId && inhGrp.EndPointSystemId == endPointSystemId
                                   select new PresentationInhouseUserToPermission
                                   {
                                       UserName = inhUser.UserName,
                                       DisplayName = inhUser.DisplayName,
                                       PermissionName = inhGrp.Name,
                                       PermissionDescription = inhGrp.Description,
                                   }
                    ).ToList();

            return userPermissions;
        }

        public List<PresentationUserAccessDetail> GetPermissions()
        {
            return (from inhperm in CurrentContext.InhousePermissions
                    select new PresentationUserAccessDetail
                    {
                        ParentId = inhperm.Id,
                        Name = inhperm.Name, // adsGrp.description
                        Description = inhperm.Description,
                        Type = "Permission",
                        EndpointSystemID = inhperm.EndPointSystemId
                    }).ToList();
        }

        public InhouseUserInfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {

            return (from inhUser in CurrentContext.InhouseUserInfos
                    join user in CurrentContext.User on inhUser.EmployeeId equals user.EmployeeId
                    where user.Id == dIUserId && inhUser.EndPointSystemId == endPointSystemId
                    && !inhUser.IsLocked
                    select inhUser).FirstOrDefault();

        }

        public List<PresentationOtherSystemData> GetPresentationOtherSystemDatas(long endPointSystemId, long adEndPointSystemId)
        {
            CurrentContext.Database.CommandTimeout = 0;
            var parameters = new object[]
            {
                new System.Data.SqlClient.SqlParameter("@ADEndpointSystemId", adEndPointSystemId),
                new System.Data.SqlClient.SqlParameter("@EndpointSystemId", endPointSystemId)
            };
            return CurrentContext.Database.SqlQuery<PresentationOtherSystemData>("EXEC USP_GETNEWUSERFOROTHERSYSTEM @ADEndpointSystemId,@EndpointSystemId", parameters).ToList();
        }
        public List<PresentationOtherSystemUser> GetOtherSystemDataNoADGroups(string accountName, long endPointSystemId, long adEndPointSystemId)
        {
            SqlParameter sqlParamUserName = new SqlParameter();
            sqlParamUserName.ParameterName = "@AccountName";
            sqlParamUserName.SqlDbType = SqlDbType.NVarChar;
            sqlParamUserName.Value = accountName;

            SqlParameter sqlParamEndpoinSystemId = new SqlParameter();
            sqlParamEndpoinSystemId.ParameterName = "@EndpointSystemId";
            sqlParamEndpoinSystemId.SqlDbType = SqlDbType.BigInt;
            sqlParamEndpoinSystemId.Value = endPointSystemId;

            SqlParameter sqlParamADEndpoinSystemId = new SqlParameter();
            sqlParamADEndpoinSystemId.ParameterName = "@ADEndpointId";
            sqlParamADEndpoinSystemId.SqlDbType = SqlDbType.BigInt;
            sqlParamADEndpoinSystemId.Value = adEndPointSystemId;

            object[] parameters = { sqlParamUserName, sqlParamEndpoinSystemId, sqlParamADEndpoinSystemId };
            CurrentContext.Database.CommandTimeout = 0;
            return CurrentContext.Database.SqlQuery<PresentationOtherSystemUser>($"EXEC USP_GETOtherSystemUserADGroupNotExists @AccountName,@EndpointSystemId,@ADEndpointId", parameters).ToList();
        }

        public List<PresentationOtherSystemUser> GetOtherSystemDataNoADGroups(long endPointSystemId)
        {
            SqlParameter sqlParamEndpoinSystemId = new SqlParameter();
            sqlParamEndpoinSystemId.ParameterName = "@EndpointSystemId";
            sqlParamEndpoinSystemId.SqlDbType = SqlDbType.BigInt;
            sqlParamEndpoinSystemId.Value = endPointSystemId;

            object[] parameters = { sqlParamEndpoinSystemId };
            CurrentContext.Database.CommandTimeout = 0;
            return CurrentContext.Database.SqlQuery<PresentationOtherSystemUser>($"SELECT A.Id as AccountReconcilationId,A.EndpointUserId, A.AccountName as UserName,NULL as Permission,U.DisplayName,A.AccountType FROM DI_M_AccountReconcilation A LEFT JOIN DI_M_User U on A.UserId = U.Id  WHERE A.EndpointSystemId=@EndpointSystemId AND A.IsDeleted = 0 AND NOT A.Id IN(select AccountReconcilationId from DI_M_EntitlementReconcilation WHERE EndpointSystemId = @EndpointSystemId and IsDeleted = 0)", parameters).ToList();
        }
        public List<PresentationOtherSystemUser> GetInhouseUsersWithoutADAccount(long endPointSystemId, long adEndPointSystemId)
        {
            SqlParameter sqlParamEndpoinSystemId = new SqlParameter();
            sqlParamEndpoinSystemId.ParameterName = "@EndpointSystemId";
            sqlParamEndpoinSystemId.SqlDbType = SqlDbType.BigInt;
            sqlParamEndpoinSystemId.Value = endPointSystemId;

            SqlParameter sqlParamADEndpoinSystemId = new SqlParameter();
            sqlParamADEndpoinSystemId.ParameterName = "@ADEndpointId";
            sqlParamADEndpoinSystemId.SqlDbType = SqlDbType.BigInt;
            sqlParamADEndpoinSystemId.Value = adEndPointSystemId;

            object[] parameters = { sqlParamEndpoinSystemId, sqlParamADEndpoinSystemId };
            CurrentContext.Database.CommandTimeout = 0;
            return CurrentContext.Database.SqlQuery<PresentationOtherSystemUser>($"SELECT B.Id as AccountReconcilationId,B.EndpointUserId, B.AccountName as UserName,NULL as Permission,U.DisplayName,B.AccountType from DI_INH_UserInfo A inner join DI_M_AccountReconcilation B on A.Id = B.EndpointUserId LEFT JOIN DI_M_User U on B.UserId = U.Id WHERE A.EndPointSystemId=@EndpointSystemId AND B.EndPointSystemId=@EndpointSystemId AND B.ISDELETED = 0 AND A.UserName NOT IN(SELECT AccountName FROM DI_M_AccountReconcilation WHERE EndpointSystemId = @ADEndpointId AND IsDeleted = 0)", parameters).ToList();

        }
        public PresentationGridPagedData<PresentationAccountEntitlementAuditTrial> GetLazyLoadOtherSystemAuditTrial(int startRowIndex, int maximumRows, string filterExpression, string sortExpression, List<long> ids, DateTime? FromDate, DateTime? ToDate)
        {
            PresentationGridPagedData<PresentationAccountEntitlementAuditTrial> result = new PresentationGridPagedData<PresentationAccountEntitlementAuditTrial>();

            try
            {
                IQueryable<PresentationAccountEntitlementAuditTrial> userList = null;
                CurrentContext.Database.CommandTimeout = 300;
                userList = (from usr in CurrentContext.AccountEntitlementAuditTrials
                            join ep in CurrentContext.EndPoint on usr.EndPointSystemId equals ep.Id
                            select new PresentationAccountEntitlementAuditTrial
                            {
                                EndpointName = ep.DisplayName,
                                UserName = usr.UserName,
                                Permission = usr.Permission,
                                Remarks = usr.Remarks,
                                CreatedDate = usr.CreatedDate,
                                EndPointSystemId = usr.EndPointSystemId,
                                DisplayName = usr.DisplayName,
                                AccountType = usr.AccountType
                            });

                if (ids != null && ids.Count > 0)
                {
                    userList = userList.Where(i => ids.Contains(i.EndPointSystemId));
                }
                if (FromDate.HasValue && ToDate.HasValue)
                {
                    userList = userList.Where(i => i.CreatedDate >= FromDate && i.CreatedDate <= ToDate);
                }
                if (!string.IsNullOrWhiteSpace(filterExpression))
                {
                    userList = userList.Where(filterExpression);
                }
                result.TotalCount = userList.Count();
                if (!string.IsNullOrWhiteSpace(sortExpression))
                {
                    userList = userList.OrderBy(sortExpression);
                }
                else
                {
                    userList = userList.OrderByDescending(a => a.CreatedDate).ThenBy(i => i.EndpointName);
                }

                if (maximumRows == -1)
                    result.Data = userList.ToList();
                else
                    result.Data = userList.Skip(startRowIndex).Take(maximumRows).ToList();
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public List<PresentationOtherSystemData> GetOtherSystemDataGetMissingPermission(string accountName, long endPointSystemId, long adEndPointSystemId)
        {
            SqlParameter sqlParamUserName = new SqlParameter();
            sqlParamUserName.ParameterName = "@AccountName";
            sqlParamUserName.SqlDbType = SqlDbType.NVarChar;
            sqlParamUserName.Value = accountName;

            SqlParameter sqlParamEndpoinSystemId = new SqlParameter();
            sqlParamEndpoinSystemId.ParameterName = "@EndpointSystemId";
            sqlParamEndpoinSystemId.SqlDbType = SqlDbType.BigInt;
            sqlParamEndpoinSystemId.Value = endPointSystemId;

            SqlParameter sqlParamADEndpoinSystemId = new SqlParameter();
            sqlParamADEndpoinSystemId.ParameterName = "@ADEndpointId";
            sqlParamADEndpoinSystemId.SqlDbType = SqlDbType.BigInt;
            sqlParamADEndpoinSystemId.Value = adEndPointSystemId;

            object[] parameters = { sqlParamUserName, sqlParamEndpoinSystemId, sqlParamADEndpoinSystemId };
            CurrentContext.Database.CommandTimeout = 0;
            return CurrentContext.Database.SqlQuery<PresentationOtherSystemData>($"EXEC USP_OTHERSYSTEM_ADDMISSINGPERMISSION @AccountName,@EndpointSystemId,@ADEndpointId", parameters).ToList();
        }
        /// <summary>
        /// Used to update the accounttype,mapping type and offboarding action type
        /// </summary>
        /// <param name="endPointSystemId"></param>
        /// <param name="adEndPointSystemId"></param>
        public void UpdateADMappingAccountType(long endPointSystemId, long adEndPointSystemId)
        {
            SqlParameter sqlParamEndpoinSystemId = new SqlParameter();
            sqlParamEndpoinSystemId.ParameterName = "@endpointid";
            sqlParamEndpoinSystemId.SqlDbType = SqlDbType.BigInt;
            sqlParamEndpoinSystemId.Value = endPointSystemId;

            SqlParameter sqlParamADEndpoinSystemId = new SqlParameter();
            sqlParamADEndpoinSystemId.ParameterName = "@ADEndpointId";
            sqlParamADEndpoinSystemId.SqlDbType = SqlDbType.BigInt;
            sqlParamADEndpoinSystemId.Value = adEndPointSystemId;

            object[] parameters = { sqlParamEndpoinSystemId, sqlParamADEndpoinSystemId };
            CurrentContext.Database.CommandTimeout = 0;
            CurrentContext.Database.SqlQuery<PresentationOtherSystemData>($"USP_UPDATEOTHERSYSTEM_ACCOUNTYYPEFROMAD @endpointid,@ADEndpointId", parameters).ToList();
        }
        public Dictionary<string, List<string>> GetAuditTrailForEmail(DateTime fromDate, DateTime toDate)
        {
            return (from auditTrail in CurrentContext.AccountEntitlementAuditTrials
                    join ep in CurrentContext.EndPoint on auditTrail.EndPointSystemId equals ep.Id
                    where auditTrail.CreatedDate >= fromDate && auditTrail.CreatedDate <= toDate
                    select new { ep.DisplayName, auditTrail.Remarks }
                       ).GroupBy(i => i.DisplayName).ToDictionary(group => group.Key,
        group => group.Select(a => a.Remarks).Distinct().ToList());
        }

        public void UpdateINHReconEntitlementDescription(long endPointSystemId)
        {
            SqlParameter sqlParamEndpoinSystemId = new SqlParameter();
            sqlParamEndpoinSystemId.ParameterName = "@endPointSystemId";
            sqlParamEndpoinSystemId.SqlDbType = SqlDbType.BigInt;
            sqlParamEndpoinSystemId.Value = endPointSystemId;

            object[] parameters = { sqlParamEndpoinSystemId };
            CurrentContext.Database.CommandTimeout = 0;
            CurrentContext.Database.SqlQuery<PresentationOtherSystemData>($"DI_USP_UpdateINHReconEntitlementDescription @endPointSystemId", parameters).ToList();
        }
        public void UpdateAllINHReconEntitlementDescription()
        {
            try
            {
                var endPoints = CurrentContext.Database.SqlQuery<EndPoint>("select Distinct E.* from DI_M_EndpointSystem E  join DI_INH_Permission P on E.Id = P.EndPointSystemId where E.IsActive = 1 and E.EndpointSystemTypeId = 9 and P.IsActive = 1 AND P.ADGroups IS NOT NULL and LEN(RTRIM(LTRIM(ISNULL(P.ADGroups,''))))> 0").ToList();
                var restOfEndpoints = CurrentContext.Database.SqlQuery<EndPoint>("" +
                    "SELECT * FROM DI_M_EndpointSystem WHERE EndpointSystemTypeId = 9 AND IsActive = 1 AND ID NOT IN(" +
                    "select Distinct E.ID from DI_M_EndpointSystem E  join DI_INH_Permission P on E.Id = P.EndPointSystemId where E.IsActive = 1 and E.EndpointSystemTypeId = 9 and P.IsActive = 1 AND P.ADGroups IS NOT NULL and LEN(RTRIM(LTRIM(ISNULL(P.ADGroups,''))))> 0"
                    +")"
                    ).ToList();

                long endPointSystemId = 0;

                object[] parameters = new object []{ };
                SqlParameter sqlParamEndpoinSystemId = null;
                foreach (var restOfEndpoint in restOfEndpoints)
                {
                    try
                    {
                        endPointSystemId = restOfEndpoint.Id;
                        sqlParamEndpoinSystemId = new SqlParameter();
                        sqlParamEndpoinSystemId.ParameterName = "@endPointSystemId";
                        sqlParamEndpoinSystemId.SqlDbType = SqlDbType.BigInt;
                        sqlParamEndpoinSystemId.Value = endPointSystemId;
                        parameters = new object[] { sqlParamEndpoinSystemId };
                        CurrentContext.Database.CommandTimeout = 0;
                        CurrentContext.Database.SqlQuery<PresentationOtherSystemData>($"DI_USP_UpdateINHReconEntitlementDescription @endPointSystemId", parameters).ToList();
                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, $"UpdateAllINHReconEntitlementDescription: {restOfEndpoint.DisplayName}:{ex}", null);
                    }
                }
                
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(LogLevelType.Exception, $"UpdateAllINHReconEntitlementDescription {ex}", null);
            }
        }
    }
}