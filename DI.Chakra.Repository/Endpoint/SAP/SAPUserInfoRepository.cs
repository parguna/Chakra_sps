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

namespace DI.Chakra.Repository
{
    public class SAPUserInfoRepository : RepositoryBase<SAPUserinfo>
    {
        public SAPUserInfoRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        private IACMDbOperator m_iacmOperator;
        private QueryExecutor m_executor;

        public void InsertSAPUserRoleMapping(long endPointSystemId, long userId, long roleId, string fromDate, string toDate)
        {
            string cmd = string.Format("IF NOT EXISTS (SELECT 1 FROM [dbo].[SAPRoleInfoSAPUserinfo] WHERE SAPUserinfo_Id = {0} AND SAPRoleInfo_Id = {1}) INSERT INTO [dbo].[SAPRoleInfoSAPUserinfo] ([SAPUserinfo_Id],[SAPRoleInfo_Id],[EndpointSystemId],[IsDirectlyAttached],[Role_FromDate],[Role_ToDate]) VALUES({0},{1},{2},{3},'{4}','{5}')", userId, roleId, endPointSystemId, 1, fromDate, toDate);
            //DILog.ProvisioningLogger.WriteLog(LogLevelType.DebugStep, cmd);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void UpdateSAPRoleInfoSAPUserinfoByEndPointSystemId(long endPointSystemId, long id)
        {
            string cmd = "UPDATE [SAPRoleInfoSAPUserinfo] SET EndpointSystemId = " + endPointSystemId + " WHERE SAPUserinfo_Id = " + id;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
            CurrentContext.SaveChanges();
        }



        public List<PresentationSAPUsersToProfileReport> GetSAPUserToProfilesReportData(long endPointSystemId)
        {
            return (from sapUserprofile in CurrentContext.SapUserProfile
                    where sapUserprofile.EndPointSystemId == endPointSystemId
                    select new PresentationSAPUsersToProfileReport
                    {
                        UserName = sapUserprofile.UserName,
                        ProfileCode = sapUserprofile.ProfileName
                    })
                      .ToList();
        }

        public DataSet GetAccessByUserId(long sapUserId)
        {
            DataSet ds = null;

            SAPUserinfo aSAPUser = CurrentContext.SAPUserinfos.FirstOrDefault(usr => usr.Id == sapUserId);

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

            SAPUserinfo aSAPUser = CurrentContext.SAPUserinfos.FirstOrDefault(usr => usr.Id == id);

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

        //public List<PresentationSAPUserinfo> GetByDIUserId(long diUserId)
        //{
        //    return (from sapusr in CurrentContext.SAPUserinfos.
        //            Where(sapusr => sapusr.DIUserId == diUserId && sapusr.IsLocked == false)
        //            select new PresentationSAPUserinfo
        //            {
        //                EndpointUserId = sapusr.Id,
        //                UserName = sapusr.UserLogonName,
        //                FullName = sapusr.Firstname + " " + sapusr.Lastname,
        //                AccountType = sapusr.AccountType,
        //                LastLogonDate = sapusr.LastLoginTime,
        //                EndpointSystemId = sapusr.EndPointSystemId,
        //                EndPointSystemType = sapusr.EndPointSystem.EndPointSystemType.EndpointSystemTypeName.ToString(),
        //                EndpointSystemName = sapusr.EndPointSystem.DisplayName

        //            }).ToList();
        //}

        public List<PresentationSAPUserinfo> GetByEndPointSystemId(long diUserId, long EndpointSystemId)
        {
            return (from sapUser in CurrentContext.SAPUserinfos
                    where sapUser.Id == diUserId && sapUser.EndPointSystemId == EndpointSystemId
                    from sapRole in sapUser.SAPRoleInfo
                    where sapRole.EndPointSystemId == EndpointSystemId
                    select new PresentationSAPUserinfo
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

        //public bool HashUserAccountExist(long diUserId, int endPointSystemID, EndpointType EndpointSystemType)
        //{
        //    var result = false;

        //    Dictionary<long, string> data = CurrentContext.SAPUserinfos.Where(item => item.EndPointSystemId == endPointSystemID && item.DIUserId == diUserId && item.AccountType == AccountType.User && item.IsLocked == false)
        //                                          .Select(x => new { Key = x.Id, Value = x.UserLogonName })
        //                                          .Distinct().ToDictionary(o => o.Key, o => o.Value);
        //    result = data.Count > 0 ? true : false;

        //    return result;
        //}

        //public dynamic GetAllSAPUser(long endPtId)
        //{
        //    return (from usr in CurrentContext.User
        //            join endUser in CurrentContext.SAPUserinfos on usr.Id equals endUser.DIUserId
        //            where endUser.EndPointSystemId == endPtId && endUser.DIUserId != null && endUser.DIUserId != -1
        //            select new
        //            {
        //                UserId = usr.Id,
        //                usr.FirstName,
        //                usr.LastName,
        //                UserLogOnName = endUser.UserLogonName,
        //                usr.EmailId,
        //                usr.DisplayName
        //            }).ToList();
        //}

        public IList<PresentationSAPUsersToGroups> GetSAPUserToGroupsReportData()
        {
            return (from sapUser in CurrentContext.SAPUserinfos
                    from sapGroup in sapUser.SAPGroupInfo
                    select new PresentationSAPUsersToGroups
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


        public IList<PresentationSAPUsersToRolesReport> GetSAPUserToRolesReportData(long EndpointSystemId)
        {
            return (from sapUser in CurrentContext.SAPUserinfos
                    from sapRole in sapUser.SAPRoleInfo
                    where (!string.IsNullOrEmpty(sapRole.ROLE_CODE) && sapUser.EndPointSystemId == EndpointSystemId)
                    select new PresentationSAPUsersToRolesReport
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
            string query = string.Format("Delete from SAPUserinfoSAPGroupInfo where EndpointSystemId={0}", endpointSystemId);
            ExecuteQuery(query);
        }

        public void DeleteSAPUserProfileMap(long endpointSystemId)
        {
            string query = string.Format("Delete from SAPProfileInfoSAPUserinfo where EndpointSystemId={0}", endpointSystemId);
            ExecuteQuery(query);
        }

        public void DeleteSAPUserRoleMap(long endpointSystemId)
        {
            string query = string.Format("Delete from SAPRoleInfoSAPUserinfo where EndpointSystemId={0}", endpointSystemId);
            ExecuteQuery(query);
        }

        public void DeleteSAPUserGroupMap(long endpointSystemId, long userId)
        {
            string query = string.Format("Delete from SAPUserinfoSAPGroupInfo where EndpointSystemId={0} and SAPUserinfo_Id={1}", endpointSystemId, userId);
            ExecuteQuery(query);
        }

        public void DeleteSAPUserProfileMap(long endpointSystemId, long userId)
        {
            string query = string.Format("Delete from SAPProfileInfoSAPUserinfo where EndpointSystemId={0} and SAPUserinfo_Id={1}", endpointSystemId, userId);
            ExecuteQuery(query);
        }

        public void DeleteSAPUserRoleMap(long endpointSystemId, long userId)
        {
            string query = string.Format("Delete from SAPRoleInfoSAPUserinfo where EndpointSystemId={0} and SAPUserinfo_Id={1}", endpointSystemId, userId);
            ExecuteQuery(query);
        }

        public void DeleteSAPTCodeSingleRoleMap(long endpointSystemId)
        {
            string query = string.Format("Delete from SAPTCodeInfoSAPSingleRoleInfo where EndpointSystemId={0}", endpointSystemId);
            ExecuteQuery(query);
        }

        public void DeleteSAPTCodeSingleRoleMap(long endpointSystemId, long singleRoleId)
        {
            string query = string.Format("Delete from SAPTCodeInfoSAPSingleRoleInfo where EndpointSystemId={0} and SAPSingleRoleInfo_Id={1}", endpointSystemId, singleRoleId);
            ExecuteQuery(query);
        }

        public void DeleteSAPCompositeRoleSingleRoleMap(long endpointSystemId)
        {
            string query = string.Format("Delete from SAPSingleRoleInfoSAPCompositeRoleInfo where EndpointSystemId={0}", endpointSystemId);
            ExecuteQuery(query);
        }

        public void ExecuteQuery(string query)
        {
            CurrentContext.Database.ExecuteSqlCommand(query);
        }

        public List<PresentationAccessRequestItem> GetSAPAccessRequestItemForDIUserId(string searchString, long? DIUserId)
        {
            return (from apps in CurrentContext.SAPUserinfos
                    where apps.UserLogonName.Contains(searchString)
                    orderby apps.UserLogonName
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Application,
                        EndpointType = EndpointType.SAP,
                        KeyId = apps.EndPointSystemId,
                        EndpointSystemId = apps.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.SAP,
                        ItemName = apps.UserLogonName,
                        ItemDescription = apps.Lastname
                    }).ToList();

        }

        public List<PresentationAccessRequestItem> GetAccessRequestItemForRemoveRequestSpecailACR(string SearchKey, long? DIUserId)
        {
            return (from apps in CurrentContext.SAPUserinfos
                    where apps.UserLogonName.Contains(SearchKey)
                    orderby apps.UserLogonName
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Application,
                        EndpointType = EndpointType.SAP,
                        KeyId = apps.EndPointSystemId,
                        EndpointSystemId = apps.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.SAP,
                        ItemName = apps.UserLogonName,
                        ItemDescription = apps.Lastname
                    }).ToList();
        }

        public void DeleteSAPUserInfoSAPRoleInfo(long userId, long roleid)
        {
            string cmd = "DELETE FROM SAPRoleInfoSAPUserinfo WHERE SAPUserinfo_Id = " + userId + "AND SAPRoleInfo_Id=" + roleid;
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public List<SAPUserinfo> GetActiveAccounts(long usrId)
        {
            return (from sapUser in CurrentContext.SAPUserinfos
                    from sapgrp in sapUser.SAPRoleInfo
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == usrId //&& accountRecon.IsLocked == false
                    && sapUser.EndPointSystemId == accountRecon.EndpointSystemId
                    select sapUser).ToList();
        }

        public List<SAPUserinfo> GetByDIUserID(long diUserId)
        {
            return (from sapUser in CurrentContext.SAPUserinfos.Include("EndPointSystem").Include("EndPointSystemType")
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diUserId && accountRecon.IsLocked == false
                    select sapUser).ToList();
        }
        public List<SAPUserinfo> GetByDIUserID(long diUserId, AccountType accType)
        {
            return (from sapUser in CurrentContext.SAPUserinfos.Include("EndPointSystem").Include("EndPointSystemType")
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diUserId && accountRecon.IsLocked == false && accountRecon.IsDeleted == false
                    && accountRecon.AccountType == accType
                    select sapUser).ToList();
        }

        public SAPUserinfo GetByDIUserIdIncludeSAPRoleInfo(long id)
        {
            return (from sapUser in CurrentContext.SAPUserinfos
                    from sapusr in sapUser.SAPRoleInfo
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == id
                    select sapUser).FirstOrDefault();
        }

        public List<SAPUserinfo> GetByDIUserId(long DIUserId, long endpointSystemId)
        {
            return (from sapUser in CurrentContext.SAPUserinfos
                    from sapusr in sapUser.SAPRoleInfo
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == DIUserId && accountRecon.AccountType == AccountType.User && accountRecon.EndpointSystemId == endpointSystemId && !accountRecon.IsLocked
                    select sapUser).ToList();
        }

        public List<string> GetSAPRolesByEndpointUserId(long endpointUserId)
        {
            return (from sapUser in CurrentContext.SAPUserinfos.Include("SAPRoleInfo")
                    from sapRole in sapUser.SAPRoleInfo
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointUserId == endpointUserId
                    select sapRole.ROLE_CODE).ToList();
        }

        public List<SAPUserinfo> GetByEndPointSystemIdIsLockedIncludingEndpointSystem(long endpointSystemId)
        {
            return (from sapUser in CurrentContext.SAPUserinfos.Include("EndPointSystem").Include("EndPointSystemType")
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.IsLocked == false
                    select sapUser).ToList();
        }

        public IList<SAPUserinfo> GetSAPUserInfoByDIUserId(long DIUserId, AccountType accountType)
        {
            return (from sapUser in CurrentContext.SAPUserinfos.Include("EndPointSystem").Include("EndPointSystemType")
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == DIUserId && accountRecon.IsLocked == false && accountRecon.AccountType == accountType
                    select sapUser).OrderBy(sapUsr => sapUsr.CreatedDate).ToList();
        }

        public SAPUserinfo GetByAccountType(long diUserId, AccountType accountType)
        {
            return (from sapUser in CurrentContext.SAPUserinfos
                    from sapusr in sapUser.SAPRoleInfo
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == diUserId && accountRecon.AccountType == accountType
                    select sapUser).FirstOrDefault();
        }

        public SAPUserinfo GetPrimaryByDIUserId(long dIUserId, long endPointSystemId)
        {
            return (from sapUser in CurrentContext.SAPUserinfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == AccountType.User && (!accountRecon.IsLocked || !accountRecon.IsDeleted)
                    select sapUser).FirstOrDefault();
        }

        public List<SAPUserinfo> GetActiveAccounts(long dIUserId, long endPointSystemId, AccountType accountType)
        {
            return (from sapUser in CurrentContext.SAPUserinfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId
                 && accountRecon.AccountType == accountType && (!accountRecon.IsLocked)
                    select sapUser).ToList();
        }

        public SAPUserinfo GetLockedAccount(long diuserId, long endpointSystemId, AccountType accountType)
        {
            return (from sapUser in CurrentContext.SAPUserinfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.EndpointSystemId == endpointSystemId && accountRecon.UserId == diuserId &&
                    accountRecon.AccountType == accountType && sapUser.IsUserLocked && (accountRecon.IsLocked || accountRecon.IsDeleted)
                    select sapUser).FirstOrDefault();
        }

        public SAPUserinfo GetUserAccountByUser(long dIUserId, long endPointSystemId)
        {
            return (from sapUser in CurrentContext.SAPUserinfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId && accountRecon.AccountType == AccountType.User
                    select sapUser).FirstOrDefault();
        }
        public SAPUserinfo GetPositionPrefixMatchedUser(long dIUserId, long endPointSystemId, string positionPrefix)
        {
            var result = (from sapUser in CurrentContext.SAPUserinfos
                          join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                          where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == endPointSystemId && accountRecon.AccountType == AccountType.User
                          select sapUser).ToList();
            if (result != null && result.Count > 0)
            {
                return result.FirstOrDefault(i => i.UserLogonName.StartsWith(positionPrefix, StringComparison.InvariantCultureIgnoreCase));
            }
            return null;
        }

        public SAPUserinfo GetLockedAccountByName(string accountName, long di_userId, long endpointSystemId, AccountType accountType)
        {

            long? diuserId = di_userId == 0 ? null : (long?)di_userId;

            var accountReconQuery = CurrentContext.AccountReconcilations.Where(accountRecon =>
                          accountRecon.EndpointSystemId == endpointSystemId && accountRecon.AccountType == accountType && (accountRecon.IsLocked || accountRecon.IsDeleted) &&
                         accountRecon.AccountName.Equals(accountName, StringComparison.CurrentCultureIgnoreCase)).AsQueryable();

            if ((accountType == AccountType.Service || accountType == AccountType.Shared) && diuserId.HasValue )
            {
                var UserIds = CurrentContext.User.Join(CurrentContext.User, otheruser => otheruser.OrganisationCode, usrid => usrid.OrganisationCode, (otheruser, usrid) => new { UserList = otheruser, currentUser = usrid })
                                          .Where(bothuser => bothuser.currentUser.Id == diuserId).Select(bothuser => bothuser.UserList.Id).ToList();

                accountReconQuery = accountReconQuery.Where(a => UserIds.Contains(a.UserId.Value)).Select(a => a);
            }
            else
            {
                accountReconQuery = accountReconQuery.Where(a => a.UserId == diuserId).Select(a => a);
            }

            var query = (from sapUser in CurrentContext.SAPUserinfos
                         join accountRecon in accountReconQuery on sapUser.Id equals accountRecon.EndpointUserId
                         where sapUser.IsUserLocked
                         select sapUser);



            return query.FirstOrDefault();
        }

        public Dictionary<string, string> GetActivePrimarySAPUserByDIUserId(long dIUserId, ApplicationType applicationType, AccountType accountType)
        {
            var Endpoint = this.CurrentContext.EndPoint.Where(ep => ep.ApplicationType == applicationType && !ep.IsTrustedSource).Select(ep => ep).FirstOrDefault();

            if (Endpoint == null) return new Dictionary<string, string>();

            return (from sapUser in CurrentContext.SAPUserinfos
                    join accountRecon in CurrentContext.AccountReconcilations on sapUser.Id equals accountRecon.EndpointUserId
                    where accountRecon.UserId == dIUserId && accountRecon.EndpointSystemId == Endpoint.Id
                 && accountRecon.AccountType == accountType && !accountRecon.IsLocked
                    select sapUser.UserLogonName).Distinct().ToDictionary(Key => Key, Value => Value);
        }

        public List<string> GetConflictRemarks(long dIUserId, long requestId, bool isBulkRequest)
        {
            var isbulkRequest = isBulkRequest ? "1" : "0";
            #region flins_Sod_Conflicts_query

            string flins_Sod_Conflicts_query = $@"
SET NOCOUNT ON;

declare @userId int
declare @flinsApplicationType int 
declare @AdhocActionType int
declare @progressStatus bigint

declare @tblCurrentRequestTabContent table(id int identity,    xmlcontent xml)
declare @tblExistingXmlContent table(id int identity,RequestId int,[source] nvarchar(200), xmlcontent XML NULL)
declare @tblExistingRequestEntitlements table(id int identity,[source] nvarchar(200),RequestId int,Entitlement nvarchar(1000))
declare @tblCurrentRequestEntitlements table(id int identity,Entitlement  nvarchar(max))

declare @tblConflictBRoles table(id int identity, BRole  nvarchar(max),TRole  nvarchar(max),CBRole  nvarchar(max),CTRole  nvarchar(max) )
  
declare @endpointUserId int = 0
declare @i int =  0
declare @splitter nvarchar(1) = ','
declare @currentRequestId int
declare @isCurrentBulkRequest bit
declare @XMLCONTENT XML 
declare @count int
 
  set @userId ={dIUserId}
  set @flinsApplicationType = 3  
  set @AdhocActionType = 2  
  set @progressStatus  = 1  
  set @isCurrentBulkRequest = {isbulkRequest}
  set @currentRequestId = {requestId}      
  set @i=0  
  set @endpointUserId   = 0 

 
  
IF @isCurrentBulkRequest = 0
begin
insert into @tblCurrentRequestTabContent (xmlcontent) 
( 
   select  cast( adhoc.EntitlementDetail as XML)  from DI_M_AccessRequest as accrrst join DI_M_AdHocAccessRequest as adhoc on accrrst.AdHocAccessRequestId = adhoc.Id 
                                  where  adhoc.Id = @currentRequestId    and  
								  adhoc.ApplicationType = @flinsApplicationType and  
								  adhoc.AdhocActionType = @AdhocActionType 
)
end
else
begin
 insert into @tblCurrentRequestTabContent (xmlcontent) 
(	 
	select  cast(bulkReq.EntitlementDetail as XML)  from DI_M_BulkRequest as bulkReq  
                                    where  bulkReq.Id =  @currentRequestId  and  
								        bulkReq.ApplicationType = @flinsApplicationType and  
								        bulkReq.AdhocActionType = @AdhocActionType 
)
end 

 select  @XMLCONTENT =  xmlcontent from @tblCurrentRequestTabContent 
 
 insert into @tblCurrentRequestEntitlements  
    Select distinct   B.Item from 
	(select Field.N.value('(Entitlement/text())[1]', 'nvarchar(100)') As Entitlement from 
	@XMLCONTENT.nodes('/AdHocSAPHRFLINSRequest/AdHocSAPHRFLINSRequestDetails/AdHocSAPHRFLINSRequestDetail') as Field (N) ) A CROSS APPLY dbo.split(A.Entitlement,@splitter) B
 

  Select @endpointUserId =  E.EndpointUserId from 
	 (
	select Field.N.value('(text())[1]', 'nvarchar(100)') As EndpointUserId from 
	@XMLCONTENT.nodes('/AdHocSAPHRFLINSRequest/EndpointUserId') as Field (N) ) E
   
IF (@isCurrentBulkRequest = 1 AND @AdhocActionType = 2  )
 BEGIN
		SELECT @endpointUserId =  E.EndpointUserId FROM DI_M_BulkRequestDetail E INNER JOIN DI_M_BulkRequest B on E.BulkRequestId = B.Id WHERE B.Id = @currentRequestId AND E.UserId = @userId
 END

  insert into @tblExistingXmlContent([source], [RequestId] ,xmlcontent) 
 select Source,RequestId, CAST(EntitlementDetail As XML)  EntitlementDetails  from 
(
 
 select 'Adhoc Request'  as Source, accrrst.Id  As RequestId, adhoc.EntitlementDetail  from DI_M_AccessRequest as accrrst join DI_M_AdHocAccessRequest as adhoc on accrrst.AdHocAccessRequestId = adhoc.Id 
                                  where   adhoc.RequesteeUserId = @userId and  
								  adhoc.ApplicationType = @flinsApplicationType and  
								  adhoc.AdhocActionType = @AdhocActionType and 
								  accrrst.ProgressStatus = @progressStatus  and  adhoc.Id <> @currentRequestId
 union							  
 select 'Bulk Request' as Source, bulkReq.Id  As RequestId,  bulkReq.EntitlementDetail  from DI_M_AccessRequest as acrqst join DI_M_BulkRequest as bulkReq on acrqst.BulkRequestId = bulkReq.Id 
                                   join DI_M_BulkRequestDetail bulkdetail on bulkReq.Id = bulkdetail.BulkRequestId
                                    where   bulkdetail.UserId = @userId and
								  bulkReq.ApplicationType = @flinsApplicationType and  
								  bulkReq.AdhocActionType = @AdhocActionType and 
								  bulkReq.ProgressStatus = @progressStatus and  bulkReq.Id <> @currentRequestId						    
 
 ) C


 declare @RequestId int
 declare @source nvarchar(200)
   
set @i=0 
select @count = count(1) from @tblExistingXmlContent
 while(@i < @count)
	BEGIN
	
	select    @XMLCONTENT = xmlcontent,@RequestId = RequestId, @source = source  from @tblExistingXmlContent where id = @i+1 

	insert into @tblExistingRequestEntitlements(Entitlement,source,RequestId)
	  Select distinct   B.Item,@source, @RequestId from  (
	 select Field.N.value('(Entitlement/text())[1]', 'nvarchar(100)') As Entitlement from
	 @XMLCONTENT.nodes('/AdHocSAPHRFLINSRequest/AdHocSAPHRFLINSRequestDetails/AdHocSAPHRFLINSRequestDetail') as Field (N) ) A CROSS APPLY dbo.split(A.Entitlement,@splitter) B

	set @i = @i + 1

   END

    

   insert into @tblConflictBRoles (BRole,TRole,CBRole)
   select ConflictingBusinessRole BRole , ConflictingTechnicalRole TechnicalRole, BusinessRole ConflictingBusinessRole  from DI_SAP_SAPHRFLINSConflictingRole where BusinessRole in (select Entitlement collate SQL_Latin1_General_CP1_CI_AS  from @tblCurrentRequestEntitlements) 
   union 
   select BusinessRole BRole , TechnicalRole, ConflictingBusinessRole   from DI_SAP_SAPHRFLINSConflictingRole where ConflictingBusinessRole  in   (select Entitlement  collate SQL_Latin1_General_CP1_CI_AS from @tblCurrentRequestEntitlements) 
 
  
  select CONCAT('The Requested Business Role [',A.CBRole,'] Conflicts with [',B.Entitlement,'] (Conflicts Source : Current Request)') from @tblConflictBRoles A inner join @tblCurrentRequestEntitlements B on A.BRole = B.Entitlement
  union
  select CONCAT('The Requested Business Role [',A.CBRole,'] Conflicts with [',B.Entitlement,'] (Conflicts Source :',B.source,' Request - Id:',B.RequestId,' )') from  @tblConflictBRoles A inner join @tblExistingRequestEntitlements B on A.BRole = B.Entitlement
  union 
  select CONCAT('The Requested Business Role [',A.CBRole,'] Conflicts with [',A.BRole,'] (Conflicts Source : The User has in the existing.)') from @tblConflictBRoles A , DI_M_EntitlementReconcilation E
  where   E.Entitlement like replace(A.TRole,'*','%') collate SQL_Latin1_General_CP1_CI_AS and E.UserId=@userId and E.IsDeleted = 0 
  and E.AccountReconcilationId in (SELECT ID from DI_M_AccountReconcilation where IsLocked=0 and IsDeleted = 0 and EndpointUserId = @endpointUserId )

  ";

            #endregion flins_Sod_Conflicts_query

            DILog.ProvisioningLogger.WriteLog(null, 4, LogLevelType.DebugStep, $"Checking FLINS Conflict : User Id:{dIUserId},  RequestId:{requestId}, IsBulkRequest :{isbulkRequest}  is {flins_Sod_Conflicts_query}");

            return CurrentContext.Database.SqlQuery<string>(flins_Sod_Conflicts_query).ToList();

        }

        public List<string> GetSAPSODConflicts(long dIUserId, long requestId, bool isBulkRequest, string applicationType)
        {
            var isbulkRequest = isBulkRequest ? "1" : "0";
            #region flins_Sod_Conflicts_query

            string flins_Sod_Conflicts_query = "exec USP_GetSAPSODConflicts @UserId, @RequestId, @IsBulkRequest, @ApplicationType";
            var sodConflictParameters = new[]
            {
                new System.Data.SqlClient.SqlParameter("@UserId", dIUserId),
                new System.Data.SqlClient.SqlParameter("@RequestId", requestId),
                new System.Data.SqlClient.SqlParameter("@IsBulkRequest", isbulkRequest),
                new System.Data.SqlClient.SqlParameter("@ApplicationType", (object)applicationType ?? DBNull.Value)
            };

            #endregion flins_Sod_Conflicts_query

            DILog.ProvisioningLogger.WriteLog(null, 4, LogLevelType.DebugStep, $"Checking SAP SOD Conflict : User Id:{dIUserId},  RequestId:{requestId}, IsBulkRequest :{isbulkRequest}  is {flins_Sod_Conflicts_query}");

            return CurrentContext.Database.SqlQuery<string>(flins_Sod_Conflicts_query, sodConflictParameters).ToList();

        }
        public List<PresentationConflictRemarks> GetSAPSODConflictsShowingrid(long dIUserId, long requestId, bool isBulkRequest, string applicationType)
        {
            var isbulkRequest = isBulkRequest ? "1" : "0";
            #region flins_Sod_Conflicts_query

            string sap_Sod_Conflicts_query = "exec USP_GetSAPSODConflicts @UserId, @RequestId, @IsBulkRequest, @ApplicationType,1,0";
            var sodConflictParameters = new[]
            {
                new System.Data.SqlClient.SqlParameter("@UserId", dIUserId),
                new System.Data.SqlClient.SqlParameter("@RequestId", requestId),
                new System.Data.SqlClient.SqlParameter("@IsBulkRequest", isbulkRequest),
                new System.Data.SqlClient.SqlParameter("@ApplicationType", (object)applicationType ?? DBNull.Value)
            };

            #endregion sap_Sod_Conflicts_query

            DILog.ProvisioningLogger.WriteLog(null, 4, LogLevelType.DebugStep, $"Checking SAP SOD Conflict : User Id:{dIUserId},  RequestId:{requestId}, IsBulkRequest :{isbulkRequest}  is {sap_Sod_Conflicts_query}");

            return CurrentContext.Database.SqlQuery<PresentationConflictRemarks>(sap_Sod_Conflicts_query, sodConflictParameters).ToList();

        }
        public List<PresentationSAPUserDetails> GetSAPUsersToAssignEPGroup(long endpointSystemId)
        {
            return (from sapUsr in CurrentContext.SAPUserinfos
                    where sapUsr.EndPointSystemId == endpointSystemId && sapUsr.NeedEPGroupAssignment
                    select new PresentationSAPUserDetails(){
                        Id = sapUsr.Id,
                        UserLogonName = sapUsr.UserLogonName
                    }).ToList();
        }

        public List<PresentationSAPUserDetails> GetAllUserByEndpointSystemId(long endpointSystemId)
        {
            return (from sapUsr in CurrentContext.SAPUserinfos
                    where sapUsr.EndPointSystemId == endpointSystemId
                    select new PresentationSAPUserDetails()
                    {
                        Id = sapUsr.Id,
                        UserLogonName = sapUsr.UserLogonName,
                        EndpointSystemId = endpointSystemId

                    }).ToList();
        }

        public List<string> GetParentPositionWithOrgPosition(long endpointId, string position)
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

            return CurrentContext.Database.SqlQuery<string>("EXEC USP_GETPARENTANDORGPOSITION @ENDPOINTID,@POSITION", parameters).ToList();
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

            return CurrentContext.Database.SqlQuery<string>("SELECT Role FROM [dbo].[getRoleFromPosition] (@ENDPOINTID,@POSITION)", parameters).ToList();
        }
    }
}