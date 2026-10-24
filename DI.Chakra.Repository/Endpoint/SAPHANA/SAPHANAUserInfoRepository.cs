using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SAPHANAUserInfoRepository : RepositoryBase<SAPHANAUserinfo>
    {
        public SAPHANAUserInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        private IACMDbOperator m_iacmOperator;
        private QueryExecutor m_executor;

        public void InsertSAPUserRoleMapping(long endPointSystemId, long userId, long roleId, string fromDate, string toDate)
        {
            string cmd = string.Format("IF NOT EXISTS (SELECT 1 FROM [dbo].[SAPHANARoleInfoSAPHANAUserinfo] WHERE SAPHANAUserinfo_Id = {0} AND SAPHANARoleInfo_Id = {1}) INSERT INTO [dbo].[SAPHANARoleInfoSAPHANAUserinfo] ([SAPHANAUserinfo_Id],[SAPHANARoleInfo_Id],[EndpointSystemId],[IsDirectlyAttached],[Role_FromDate],[Role_ToDate]) VALUES({0},{1},{2},{3},'{4}','{5}')", userId, roleId, endPointSystemId, 1, fromDate, toDate);
            //DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, cmd);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void UpdateSAPRoleInfoSAPUserinfoByEndPointSystemId(long endPointSystemId, long id)
        {
            string cmd = "UPDATE [SAPHANARoleInfoSAPHANAUserinfo] SET EndpointSystemId = " + endPointSystemId + " WHERE SAPHANAUserinfo_Id = " + id;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
            CurrentContext.SaveChanges();
        }

        public List<PresentationSAPHANAUserinfo> GetByEndPointSystemId(long diUserId, long EndpointSystemId)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos
                    where sapUser.Id == diUserId && sapUser.EndPointSystemId == EndpointSystemId
                    from sapRole in sapUser.SAPRoleInfo
                    where sapRole.EndPointSystemId == EndpointSystemId
                    select new PresentationSAPHANAUserinfo
                    {
                        EndpointUserId = sapUser.Id,
                        UserName = sapUser.UserLogonName,
                        FullName = sapUser.Firstname + sapUser.Lastname,
                        RoleCode = sapRole.ROLE_CODE,
                        RoleDescription = sapRole.ROLE_DESC,
                        IsActive = sapUser.IsActive,
                        Department = sapUser.Department

                    }).ToList();
        }

        public IList<PresentationSAPHANAUsersToGroups> GetSAPUserToGroupsReportData()
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos
                    from sapGroup in sapUser.SAPGroupInfo
                    select new PresentationSAPHANAUsersToGroups
                    {
                        UserId = sapUser.Id,
                        UserName = sapUser.UserLogonName,
                        DisplayName = sapUser.Firstname + sapUser.Lastname,
                        GroupCode = sapGroup.GROUP_CODE,
                        GroupDescription = sapGroup.GROUP_DESC,
                        IsActive = sapUser.IsActive ? "YES" : "NO",
                        Department = sapUser.Department


                    }
                    ).ToList();
        }


        public IList<PresentationSAPHANAUsersToRolesReport> GetSAPUserToRolesReportData(long EndpointSystemId)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos
                    from sapRole in sapUser.SAPRoleInfo
                    where (!string.IsNullOrEmpty(sapRole.ROLE_CODE) && sapUser.EndPointSystemId == EndpointSystemId)
                    select new PresentationSAPHANAUsersToRolesReport
                    {
                        UserId = sapUser.Id,
                        UserName = sapUser.UserLogonName,
                        DisplayName = sapUser.Firstname + sapUser.Lastname,
                        Department = sapUser.Department,

                        RoleCode = sapRole.ROLE_CODE,
                        RoleDescription = sapRole.ROLE_DESC
                    }
                         ).ToList();
        }

        public void DeleteSAPUserGroupMap(long endpointSystemId)
        {
            string query = string.Format("Delete from SAPHANAUserinfoSAPHANAGroupInfo where EndpointSystemId={0}", endpointSystemId);
            ExecuteQuery(query);
        }

        public void DeleteSAPUserProfileMap(long endpointSystemId)
        {
            string query = string.Format("Delete from SAPHANAProfileInfoSAPHANAUserinfo where EndpointSystemId={0}", endpointSystemId);
            ExecuteQuery(query);
        }

        public void DeleteSAPUserRoleMap(long endpointSystemId)
        {
            string query = string.Format("Delete from SAPHANARoleInfoSAPHANAUserinfo where EndpointSystemId={0}", endpointSystemId);
            ExecuteQuery(query);
        }

        public void DeleteSAPUserGroupMap(long endpointSystemId, long userId)
        {
            string query = string.Format("Delete from SAPHANAUserinfoSAPHANAGroupInfo where EndpointSystemId={0} and SAPHANAUserinfo_Id={1}", endpointSystemId, userId);
            ExecuteQuery(query);
        }

        public void DeleteSAPUserProfileMap(long endpointSystemId, long userId)
        {
            string query = string.Format("Delete from SAPHANAProfileInfoSAPHANAUserinfo where EndpointSystemId={0} and SAPHANAUserinfo_Id={1}", endpointSystemId, userId);
            ExecuteQuery(query);
        }

        public void DeleteSAPUserRoleMap(long endpointSystemId, long userId)
        {
            string query = string.Format("Delete from SAPHANARoleInfoSAPHANAUserinfo where EndpointSystemId={0} and SAPHANAUserinfo_Id={1}", endpointSystemId, userId);
            ExecuteQuery(query);
        }

        public void DeleteSAPTCodeSingleRoleMap(long endpointSystemId)
        {
            string query = string.Format("Delete from SAPHANATCodeInfoSAPHANASingleRoleInfo where EndpointSystemId={0}", endpointSystemId);
            ExecuteQuery(query);
        }

        public void DeleteSAPTCodeSingleRoleMap(long endpointSystemId, long singleRoleId)
        {
            string query = string.Format("Delete from SAPTCodeInfoSAPSingleRoleInfo where EndpointSystemId={0} and SAPHANASingleRoleInfo_Id={1}", endpointSystemId, singleRoleId);
            ExecuteQuery(query);
        }

        public void DeleteSAPCompositeRoleSingleRoleMap(long endpointSystemId)
        {
            string query = string.Format("Delete from SAPHANASingleRoleInfoSAPHANACompositeRoleInfo where EndpointSystemId={0}", endpointSystemId);
            ExecuteQuery(query);
        }

        public void ExecuteQuery(string query)
        {
            CurrentContext.Database.ExecuteSqlCommand(query);
        }

        public List<PresentationAccessRequestItem> GetSAPAccessRequestItemForDIUserId(string searchString, long? DIUserId)
        {
            return (from apps in CurrentContext.SAPHANAUserinfos
                    where apps.UserLogonName.Contains(searchString)
                    orderby apps.UserLogonName
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Application,
                        EndpointType = EndpointType.SAPHANA,
                        KeyId = apps.EndPointSystemId,
                        EndpointSystemId = apps.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.SAPHANA,
                        ItemName = apps.UserLogonName,
                        ItemDescription = apps.Lastname
                    }).ToList();

        }

        public List<PresentationAccessRequestItem> GetAccessRequestItemForRemoveRequestSpecailACR(string SearchKey, long? DIUserId)
        {
            return (from apps in CurrentContext.SAPHANAUserinfos
                    where apps.UserLogonName.Contains(SearchKey)
                    orderby apps.UserLogonName
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Application,
                        EndpointType = EndpointType.SAPHANA,
                        KeyId = apps.EndPointSystemId,
                        EndpointSystemId = apps.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.SAPHANA,
                        ItemName = apps.UserLogonName,
                        ItemDescription = apps.Lastname
                    }).ToList();
        }

        public void DeleteSAPUserInfoSAPRoleInfo(long userId, long roleid)
        {
            string cmd = "DELETE FROM SAPHANARoleInfoSAPHANAUserinfo WHERE SAPHANAUserinfo_Id = " + userId + "AND SAPHANARoleInfo_Id=" + roleid;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public List<SAPHANAUserinfo> GetActiveAccounts(long usrId)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos
                    from sapgrp in sapUser.SAPRoleInfo
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == usrId //&& accountRecon.IsLocked == false
                    && sapUser.EndPointSystemId == accountRecon.EndpointSystemId
                    select sapUser).ToList();
        }

        public List<SAPHANAUserinfo> GetByDIUserID(long diUserId)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos.Include("EndPointSystem").Include("EndPointSystemType")
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diUserId && accountRecon.IsLocked == false
                    select sapUser).ToList();
        }
        public List<SAPHANAUserinfo> GetByDIUserID(long diUserId, AccountType accType)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos.Include("EndPointSystem").Include("EndPointSystemType")
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diUserId && accountRecon.IsLocked == false && accountRecon.IsDeleted == false
                    && accountRecon.AccountType == accType
                    select sapUser).ToList();
        }

        public SAPHANAUserinfo GetByDIUserIdIncludeSAPRoleInfo(long id)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos
                    from sapusr in sapUser.SAPRoleInfo
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == id
                    select sapUser).FirstOrDefault();
        }

        public List<SAPHANAUserinfo> GetByDIUserId(long DIUserId, long endpointSystemId)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos
                    from sapusr in sapUser.SAPRoleInfo
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == DIUserId && accountRecon.AccountType == AccountType.User && accountRecon.EndpointSystemId == endpointSystemId && !accountRecon.IsLocked
                    select sapUser).ToList();
        }

        public List<string> GetSAPRolesByEndpointUserId(long endpointUserId)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos.Include("SAPRoleInfo")
                    from sapRole in sapUser.SAPRoleInfo
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointUserId == endpointUserId
                    select sapRole.ROLE_CODE).ToList();
        }

        public List<SAPHANAUserinfo> GetByEndPointSystemIdIsLockedIncludingEndpointSystem(long endpointSystemId)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos.Include("EndPointSystem").Include("EndPointSystemType")
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.IsLocked == false
                    select sapUser).ToList();
        }

        public IList<SAPHANAUserinfo> GetSAPUserInfoByDIUserId(long DIUserId, AccountType accountType)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos.Include("EndPointSystem").Include("EndPointSystemType")
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == DIUserId && accountRecon.IsLocked == false && accountRecon.AccountType == accountType
                    select sapUser).OrderBy(sapUsr => sapUsr.CreatedDate).ToList();
        }

        public SAPHANAUserinfo GetByAccountType(long diUserId, AccountType accountType)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos
                    from sapusr in sapUser.SAPRoleInfo
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diUserId && accountRecon.AccountType == accountType
                    select sapUser).FirstOrDefault();
        }

        public SAPHANAUserinfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == AccountType.User && (!accountRecon.IsLocked || !accountRecon.IsDeleted)
                    select sapUser).FirstOrDefault();
        }

        public List<SAPHANAUserinfo> GetActiveAccounts(long dIUserId, long endPointSystemId, AccountType accountType)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == accountType && (!accountRecon.IsLocked)
                    select sapUser).ToList();
        }

        public SAPHANAUserinfo GetLockedAccount(long diuserId, long endpointSystemId, AccountType accountType)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.UserId == diuserId &&
                    accountRecon.AccountType == accountType && sapUser.IsUserLocked && (accountRecon.IsLocked || accountRecon.IsDeleted)
                    select sapUser).FirstOrDefault();
        }

        public SAPHANAUserinfo GetUserAccountByUser(long dIUserId, long endPointSystemId)
        {
            return (from sapUser in CurrentContext.SAPHANAUserinfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId && accountRecon.AccountType == AccountType.User
                    select sapUser).FirstOrDefault();
        }
        public SAPHANAUserinfo GetPositionPrefixMatchedUser(long dIUserId, long endPointSystemId, string positionPrefix)
        {
            var result = (from sapUser in CurrentContext.SAPHANAUserinfos
                          join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                          where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId && accountRecon.AccountType == AccountType.User
                          select sapUser).ToList();
            if (result != null && result.Count > 0)
            {
                return result.FirstOrDefault(i => i.UserLogonName.StartsWith(positionPrefix, StringComparison.InvariantCultureIgnoreCase));
            }
            return null;
        }

        public SAPHANAUserinfo GetLockedAccountByName(string accountName, long di_userId, long endpointSystemId, AccountType accountType)
        {

            long? diuserId = di_userId == 0 ? null : (long?)di_userId;

            var accountReconQuery = CurrentContext.AccountReconcilations.Where(accountRecon =>
                          accountRecon.EndpointSystemId == endpointSystemId && accountRecon.AccountType == accountType && (accountRecon.IsLocked || accountRecon.IsDeleted) &&
                         accountRecon.AccountName.Equals(accountName, StringComparison.CurrentCultureIgnoreCase)).AsQueryable();

            if ((accountType == AccountType.Service || accountType == AccountType.Shared) && diuserId.HasValue)
            {
                var UserIds = CurrentContext.User.Join(CurrentContext.User, otheruser => otheruser.OrganisationCode, usrid => usrid.OrganisationCode, (otheruser, usrid) => new { UserList = otheruser, currentUser = usrid })
                                          .Where(bothuser => bothuser.currentUser.Id == diuserId).Select(bothuser => bothuser.UserList.Id).ToList();

                accountReconQuery = accountReconQuery.Where(a => UserIds.Contains(a.UserId.Value)).Select(a => a);
            }
            else
            {
                accountReconQuery = accountReconQuery.Where(a => a.UserId == diuserId).Select(a => a);
            }

            var query = (from sapUser in CurrentContext.SAPHANAUserinfos
                         join accountRecon in accountReconQuery on sapUser.Id equals accountRecon.EndpointUserId
                         where sapUser.IsUserLocked
                         select sapUser);



            return query.FirstOrDefault();
        }

        public Dictionary<string, string> GetActivePrimarySAPUserByDIUserId(long dIUserId, ApplicationType applicationType, AccountType accountType)
        {
            var Endpoint = this.CurrentContext.EndPoint.Where(ep => ep.ApplicationType == applicationType && !ep.IsTrustedSource).Select(ep => ep).FirstOrDefault();

            if (Endpoint == null) return new Dictionary<string, string>();

            return (from sapUser in CurrentContext.SAPHANAUserinfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == Endpoint.Id
                 && accountRecon.AccountType == accountType && !accountRecon.IsLocked
                    select sapUser.UserLogonName).Distinct().ToDictionary(Key => Key, Value => Value);
        }

        public List<string> GetSAPSODConflicts(long dIUserId, long requestId, bool isBulkRequest, string applicationType)
        {
            var isbulkRequest = isBulkRequest ? "1" : "0";
            string flins_Sod_Conflicts_query = "exec USP_GetSAPHANA_SODConflicts @UserId, @RequestId, @IsBulkRequest, @ApplicationType";
            var sodParameters = new[]
            {
                new SqlParameter("@UserId", dIUserId),
                new SqlParameter("@RequestId", requestId),
                new SqlParameter("@IsBulkRequest", isbulkRequest),
                new SqlParameter("@ApplicationType", (object)applicationType ?? DBNull.Value)
            };

            DILog.ProvisioningLogger.WriteLog(null, 4, LogLevelType.DebugStep, $"Checking SAPHANA SOD Conflict : User Id:{dIUserId},  RequestId:{requestId}, IsBulkRequest :{isbulkRequest}  is {flins_Sod_Conflicts_query}");

            return CurrentContext.Database.SqlQuery<string>(flins_Sod_Conflicts_query, sodParameters).ToList();

        }
        public List<PresentationConflictRemarks> GetSAPSODConflictsShowingrid(long dIUserId, long requestId, bool isBulkRequest, string applicationType)
        {
            var isbulkRequest = isBulkRequest ? "1" : "0";
            string sap_Sod_Conflicts_query = "exec USP_GetSAPHANA_SODConflicts @UserId, @RequestId, @IsBulkRequest, @ApplicationType,1,0";
            var sodParameters = new[]
            {
                new SqlParameter("@UserId", dIUserId),
                new SqlParameter("@RequestId", requestId),
                new SqlParameter("@IsBulkRequest", isbulkRequest),
                new SqlParameter("@ApplicationType", (object)applicationType ?? DBNull.Value)
            };
            DILog.ProvisioningLogger.WriteLog(null, 4, LogLevelType.DebugStep, $"Checking SAPHANA SOD Conflict : User Id:{dIUserId},  RequestId:{requestId}, IsBulkRequest :{isbulkRequest}  is {sap_Sod_Conflicts_query}");

            return CurrentContext.Database.SqlQuery<PresentationConflictRemarks>(sap_Sod_Conflicts_query, sodParameters).ToList();

        }
        public List<PresentationSAPUserDetails> GetSAPUsersToAssignEPGroup(long endpointSystemId)
        {
            return (from sapUsr in CurrentContext.SAPHANAUserinfos
                    where sapUsr.EndPointSystemId == endpointSystemId && sapUsr.NeedEPGroupAssignment
                    select new PresentationSAPUserDetails()
                    {
                        Id = sapUsr.Id,
                        UserLogonName = sapUsr.UserLogonName
                    }).ToList();
        }

        public List<PresentationSAPHANAUserDetails> GetAllUserByEndpointSystemId(long endpointSystemId)
        {
            return (from sapUsr in CurrentContext.SAPHANAUserinfos
                    where sapUsr.EndPointSystemId == endpointSystemId
                    select new PresentationSAPHANAUserDetails()
                    {
                        Id = sapUsr.Id,
                        UserLogonName = sapUsr.UserLogonName,
                        EndpointSystemId = endpointSystemId

                    }).ToList();
        }
       
        public List<string> GetRoleFromPosition(long endpointId, string position)
        {
            SqlParameter sqlParamUserLogon = new SqlParameter();
            sqlParamUserLogon.ParameterName = "@POSITION";
            sqlParamUserLogon.SqlDbType = SqlDbType.NVarChar;
            sqlParamUserLogon.Value = position;

            SqlParameter sqlParamEndpoinSystemId = new SqlParameter();
            sqlParamEndpoinSystemId.ParameterName = "@ENDPOINTID";
            sqlParamEndpoinSystemId.SqlDbType = SqlDbType.BigInt;
            sqlParamEndpoinSystemId.Value = endpointId;

            object[] parameters = { sqlParamEndpoinSystemId, sqlParamUserLogon };

            return CurrentContext.Database.SqlQuery<string>("SELECT Role FROM [dbo].[fnSAPHANAGetRoleFromPosition] (@ENDPOINTID,@POSITION)", parameters).ToList();
        }
        public DataSet GetAccessByUserId(long sapUserId)
        {
            DataSet ds = null;

            var aSAPUser = CurrentContext.SAPHANAUserinfos.FirstOrDefault(usr => usr.Id == sapUserId);

            string query = "DI_spGetUserAccess";

            SqlParameter sqlParamUserLogon = new SqlParameter();
            sqlParamUserLogon.ParameterName = "@userid";
            sqlParamUserLogon.SqlDbType = SqlDbType.Int;
            sqlParamUserLogon.Value = sapUserId;

            SqlParameter sqlParamEndpoinSystemId = new SqlParameter();
            sqlParamEndpoinSystemId.ParameterName = "@EndpointSystemId";
            sqlParamEndpoinSystemId.SqlDbType = SqlDbType.Int;
            sqlParamEndpoinSystemId.Value = aSAPUser.EndPointSystemId;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(sqlParamUserLogon);
            parameters.Add(sqlParamEndpoinSystemId);

            m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

            m_executor = new QueryExecutor(m_iacmOperator);
            ds = m_executor.ExecuteQueryDS();

            return ds;
        }

        public DataSet GetObjectsByUserId(long id)
        {
            DataSet ds = null;

            var aSAPUser = CurrentContext.SAPHANAUserinfos.FirstOrDefault(usr => usr.Id == id);

            string query = "DI_spGetSapObjectFieldData";

            SqlParameter sqlParamUserLogon = new SqlParameter();
            sqlParamUserLogon.ParameterName = "@UserLogonName";
            sqlParamUserLogon.SqlDbType = SqlDbType.NVarChar;
            sqlParamUserLogon.Value = aSAPUser.UserLogonName;

            SqlParameter sqlParamEndpoinSystemId = new SqlParameter();
            sqlParamEndpoinSystemId.ParameterName = "@EndpointSystemId";
            sqlParamEndpoinSystemId.SqlDbType = SqlDbType.NVarChar;
            sqlParamEndpoinSystemId.Value = aSAPUser.EndPointSystemId;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(sqlParamUserLogon);
            parameters.Add(sqlParamEndpoinSystemId);

            m_iacmOperator = new IACMDbOperator(query, CommandType.StoredProcedure, parameters);

            m_executor = new QueryExecutor(m_iacmOperator);
            ds = m_executor.ExecuteQueryDS();

            return ds;
        }
    }
}
