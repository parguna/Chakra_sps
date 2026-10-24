using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SAPHANARoleInfoRepository : RepositoryBase<SAPHANARoleInfo>
    {
        public SAPHANARoleInfoRepository(DataAccessTransaction dataAccessTransaction) : base(dataAccessTransaction) { }
        public void UpdateInfo(long endpointSystemId, long userId)
        {
            string query = "UPDATE [SAPHANARoleInfoSAPHANAUserinfo] SET EndpointSystemId = " + endpointSystemId + " WHERE SAPHANAUserinfo_Id = " + userId;
            CurrentContext.Database.ExecuteSqlCommand(query);

        }

        public List<SAPHANARoleInfo> GetListByUserId(long userId)
        {
            string query = string.Format("Select  rl.* from DI_SAPHANA_ROLEINFO rl INNER JOIN   SAPHANARoleInfoSAPHANAUserinfo userrole ON rl.Id=userrole.SAPHANARoleInfo_Id where userrole.SAPHANARoleInfo_Id={0} and IsDirectlyAttached ={1} and convert(datetime, role_todate, 102) >= convert(datetime, {2}, 131)  ", userId, 1, DateTime.Now.Date.ToShortDateString());
            return CurrentContext.Database.SqlQuery<SAPHANARoleInfo>(query).ToList();
        }


        public List<PresentationSAPHANAUsersToRolesReport> GetUserRoles(long sap_userId, long endpointSystemId)
        {
            string query = $@"select u.Id as UserId, u.UserLogonName, '' DisplayName ,'' as Department,
                         u.IsActive, r.ROLE_CODE as RoleCode,r.ROLE_DESC as RoleDescription,
	                        Role_FromDate, Role_ToDate , r.Id as RoleId

                     from SAPHANARoleInfoSAPHANAUserinfo ur
                     inner join DI_SAPHANA_UserInfo u on ur.SAPHANAUserinfo_Id = u.Id
                     inner join DI_SAPHANA_ROLEINFO r on ur.SAPHANARoleInfo_Id = r.Id
                     where u.Id = {sap_userId} and u.EndPointSystemId = {endpointSystemId}   and ur.IsDirectlyAttached = 1 ";

            return CurrentContext.Database.SqlQuery<PresentationSAPHANAUsersToRolesReport>(query).ToList();
        }

        public IList<SAPHANARoleInfo> GetByEndPointSystemId(long endpointId)
        {
            IList<SAPHANARoleInfo> sapRoleList = new List<SAPHANARoleInfo>();

            // TODO: //ORDER BY ROLE_CODE COLLATE Latin1_General_BIN
            var _list1 = (from sr in CurrentContext.SAPHANARoleInfos
                          where sr.EndPointSystemId == endpointId
                          orderby sr.ROLE_CODE
                          select new
                          {
                              SAP_ROLEID = sr.Id,
                              sr.ROLE_DESC,
                              sr.ROLE_CODE,

                          }).Distinct();
            return sapRoleList;
        }

        public Dictionary<long, string> GetSAPRoleBySearchString(long endpointId, string searchString)
        {
            Dictionary<long, string> sapRoles = (from sapRole in CurrentContext.SAPHANARoleInfos
                                                 where sapRole.ROLE_CODE.Contains(searchString) && sapRole.EndPointSystemId == endpointId
                                                 select new { sapRole.Id, sapRole.ROLE_CODE }).Distinct().Take(100).OrderBy(s => s.ROLE_CODE)
                         .ToDictionary(c => c.Id, c => c.ROLE_CODE);
            return sapRoles;
        }

        public List<PresentationAccessRequestItem> GetSAPAccessRequestItemIncludingentitlrment(string searchString, long? DIUserId)
        {
            return (from groups in CurrentContext.SAPHANARoleInfos
                    where groups.ROLE_CODE.Contains(searchString)
                    orderby groups.ROLE_CODE
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Entitlement,
                        EndpointType = EndpointType.SAPHANA,
                        KeyId = groups.Id,
                        EndpointSystemId = groups.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.SAPHANA,
                        ItemName = groups.ROLE_CODE,
                        ItemDescription = groups.ROLE_DESC
                    }).ToList();
        }

        public List<PresentationAccessRequestItem> GetAccessRequestItemForRemoveRequestSpecailACR(string SearchKey, long? DIUserId)
        {
            return (from groups in CurrentContext.SAPHANARoleInfos
                    where groups.ROLE_CODE.Contains(SearchKey)
                    orderby groups.ROLE_CODE
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Entitlement,
                        EndpointType = EndpointType.SAPHANA,
                        KeyId = groups.Id,
                        EndpointSystemId = groups.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.SAPHANA,
                        ItemName = groups.ROLE_CODE,
                        ItemDescription = groups.ROLE_DESC
                    }).ToList();
        }

        public List<PresentationSAPHANARoleWithNoUsers> GetSAPRolesWithNoUsers(long endPointSystemId)
        {
            return (from sapRole in CurrentContext.SAPHANARoleInfos
                    where sapRole.EndPointSystemId == endPointSystemId && sapRole.SAPUserinfo.Count == 0
                    select new PresentationSAPHANARoleWithNoUsers

                    {
                        SAP_ROLEID = (int)sapRole.Id,
                        ROLE_CODE = sapRole.ROLE_CODE,
                        ROLE_DESC = sapRole.ROLE_DESC
                    }
                    )
                    .ToList();
        }

        public List<PresentationSAPHANARoleToUsers> GetSAPRolesToUsers(long endPointSystemId)
        {
            return (from sapRole in CurrentContext.SAPHANARoleInfos
                    from sapUser in sapRole.SAPUserinfo
                    where sapRole.EndPointSystemId == endPointSystemId && sapRole.ROLE_DESC != string.Empty
                    select new PresentationSAPHANARoleToUsers

                    {
                        RoleId = sapRole.Id,
                        RoleCode = sapRole.ROLE_CODE,
                        RoleDescription = sapRole.ROLE_DESC,
                        UserName = sapUser.UserLogonName,
                        Department = sapUser.Department,
                        DisplayName = sapUser.Firstname + " " + sapUser.Lastname

                    }
                    )
                    .ToList();
        }
        

        public Dictionary<long, string> GetBusinessRoles(long endpointId, long EndpointUserId, string module = null, string subModule = null)
        {
            try
            {
                var endpoint = CurrentContext.EndPoint.FirstOrDefault(i => i.Id == endpointId);

                if (endpoint.ApplicationType == ApplicationType.EBS)
                {

                    string sapRolesQuery = string.Format("SELECT rinfo.ROLE_CODE FROM SAPHANARoleInfoSAPHANAUserinfo ruinfo JOIN DI_SAPHANA_ROLEINFO rinfo ON ruinfo.SAPHANARoleInfo_Id= rinfo.Id AND ruinfo.IsDirectlyAttached=1 WHERE ruinfo.SAPHANAUserinfo_Id={0}", EndpointUserId);

                    List<string> SapRoles = CurrentContext.Database.SqlQuery<string>(sapRolesQuery).ToList();

                    return (from businessRole in CurrentContext.SAPHANAXLRolesMatrices
                            join tRoles in SapRoles on businessRole.TechnicalRole equals tRoles
                            where businessRole.EndpointSystemId == endpoint.Id
                            select businessRole)
                           .Select(role => new { key = role.Id, value = role.RoleName }).OrderBy(item => item.value)
                           .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }

                else if (endpoint.ApplicationType == ApplicationType.MSSL)
                {
                    string sapRolesQuery = string.Format("SELECT rinfo.ROLE_CODE FROM SAPHANARoleInfoSAPHANAUserinfo ruinfo join DI_SAPHANA_ROLEINFO rinfo ON ruinfo.SAPHANARoleInfo_Id= rinfo.Id AND ruinfo.IsDirectlyAttached=1 WHERE ruinfo.SAPHANAUserinfo_Id={0}", EndpointUserId);

                    List<string> SapRoles = CurrentContext.Database.SqlQuery<string>(sapRolesQuery).ToList();

                    return (from businessRole in CurrentContext.SAPHANAXLRolesMatrices
                            join tRoles in SapRoles on businessRole.TechnicalRole equals tRoles
                            where businessRole.EndpointSystemId == endpoint.Id
                            select businessRole)
                           .Select(role => new { key = role.Id, value = role.RoleName }).OrderBy(item => item.value)
                           .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public List<SAPHANARoleToAuthorizationReportData> GetSapRolesToAuthorizationData(string RoleCode, int EndpointSystemId)
        {
            try
            {
                string query = "select distinct roleauth.Id, roleauth.RoleName, roleauth.AuthCode,  roleauth.ObjectCode, roleauth.FieldCode, case when fieldvalue.FromValue is not null then fieldvalue.FromValue else roleauth.fromvalue end as FromValue, roleauth.ToValue from DI_SAPHANA_RoleToAuthMap roleauth  left join DI_SAPHANA_FIELDVALUEMAPPING fieldvalue on roleauth.FromValue = fieldvalue.FieldCode and roleauth.rolename = fieldvalue.RoleCode and left(roleauth.fromvalue, 1) = '$'	where roleauth.endpointsystemid = @EndpointSystemId and roleauth.RoleName = @RoleCode and roleauth.ObjectCode != 'S_TCODE' and roleauth.ProfileGeneratorVariant != 'X'";
                var queryParameters = new[]
                {
                    new System.Data.SqlClient.SqlParameter("@EndpointSystemId", EndpointSystemId),
                    new System.Data.SqlClient.SqlParameter("@RoleCode", (object)RoleCode ?? DBNull.Value)
                };
                var RoleAuth = CurrentContext.Database.SqlQuery<SAPHANARoleToAuthorizationReportData>(query, queryParameters);

                return RoleAuth.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PresentationBusinessRoleDetail> GetSAPBusinessRole(long endpointSystemId, long endpointUserId, ApplicationType applicationType)
        {
            CurrentContext.Database.CommandTimeout = 0;
            string sapRolesQuery = string.Empty;
            var sapRoles = new List<PresentationBusinessRoleDetail>();
            #region EBS
            if (applicationType == ApplicationType.EBS)
            {
                sapRolesQuery = $@"SELECT EndpointSystemId,EndpointUserId,BusinessRole,Description,MAX(ValidToDate) as ValidToDate from dbo.VW_SAPHANA_EBS_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}
                    GROUP BY EndpointSystemId,EndpointUserId,BusinessRole,Description ORDER BY BusinessRole";
            }
            #endregion
            #region MSSL
            else if (applicationType == ApplicationType.MSSL)
            {
                sapRolesQuery = $@"SELECT EndpointSystemId,EndpointUserId,BusinessRole,Description,MAX(ValidToDate) as ValidToDate from dbo.VW_SAPHANA_MSSL_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}
                    GROUP BY EndpointSystemId,EndpointUserId,BusinessRole,Description ORDER BY BusinessRole";
            }
            #endregion
            
            if (!string.IsNullOrWhiteSpace(sapRolesQuery))
                sapRoles = CurrentContext.Database.SqlQuery<PresentationBusinessRoleDetail>(sapRolesQuery).ToList();

            return sapRoles;
        }

        public List<PresentationSAPHANARoleReport> GetTechnicalRolesFromBusinessRole(long endpointSystemId, long endpointUserId, string businessrole, ApplicationType applicationType)
        {
            var sapRoles = new List<PresentationSAPHANARoleReport>();
            string sapRolesQuery = null;
            switch (applicationType)
            {
                case ApplicationType.MSSL:
                    #region
                    sapRolesQuery = $@"select SAP_ROLEID,EndpointSystemId,EndpointUserId,ROLE_CODE,ROLE_DESC,MAX(ValidToDate) as ValidToDate from dbo.VW_SAPHANA_MSSL_ROLES WHERE ENDPOINTSYSTEMID={endpointSystemId} AND EndpointUserId={endpointUserId} AND BusinessRole = '{businessrole.Replace("'", "''")}' GROUP BY SAP_ROLEID,EndpointSystemId,EndpointUserId,ROLE_CODE,ROLE_DESC ORDER BY ROLE_CODE";
                    #endregion
                    break;
                case ApplicationType.EBS:
                    #region
                    sapRolesQuery = $@"SELECT SAP_ROLEID,EndpointSystemId,EndpointUserId,ROLE_CODE,ROLE_DESC,MAX(ValidToDate) as ValidToDate from dbo.VW_SAPHANA_EBS_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}  AND BusinessRole = '{businessrole.Replace("'", "''")}'
                    GROUP BY SAP_ROLEID,EndpointSystemId,EndpointUserId,ROLE_CODE,ROLE_DESC ORDER BY ROLE_CODE";

                    #endregion
                    break;
                default: break;
            }
            if (!string.IsNullOrWhiteSpace(sapRolesQuery))
                sapRoles = CurrentContext.Database.SqlQuery<PresentationSAPHANARoleReport>(sapRolesQuery).ToList();

            return sapRoles;
        }

        public PresentationBusinessRoleDetail GetSAPBusinessRole(long endpointSystemId, long endpointUserId, ApplicationType applicationType, string businessRole)
        {
            string sapRolesQuery = string.Empty;
            var sapRole = new PresentationBusinessRoleDetail();
            #region EBS
             if (applicationType == ApplicationType.EBS)
            {
                sapRolesQuery = $@"SELECT EndpointSystemId,EndpointUserId,BusinessRole,MAX(ValidToDate) as ValidToDate from dbo.VW_SAPHANA_EBS_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}
                    GROUP BY EndpointSystemId,EndpointUserId,BusinessRole ORDER BY BusinessRole";
            }
            #endregion
            #region MSSL
            else if (applicationType == ApplicationType.MSSL)
            {
                sapRolesQuery = $@"SELECT EndpointSystemId,EndpointUserId,BusinessRole,MAX(ValidToDate) as ValidToDate from dbo.VW_SAPHANA_MSSL_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}
                    GROUP BY EndpointSystemId,EndpointUserId,BusinessRole ORDER BY BusinessRole";
            }
            #endregion
            if (!string.IsNullOrWhiteSpace(sapRolesQuery))
                sapRole = CurrentContext.Database.SqlQuery<PresentationBusinessRoleDetail>(sapRolesQuery).FirstOrDefault();

            return sapRole;
        }
        public List<PresentationBusinessRoleDetail> GetSAPHANAIndirectRole(long endpointSystemId, long accountReconId)
        {
            var query = $@"EXEC DBO.USP_SAPHANA_INDIRECTROLE @ENDPOINTSYSTEMID,@ACCOUNTRECONID";
            var epParam = new SqlParameter("@ENDPOINTSYSTEMID", endpointSystemId);
            var acctReconParam = new SqlParameter("@ACCOUNTRECONID", accountReconId);
            object[] param = { epParam, acctReconParam };
            return CurrentContext.Database.SqlQuery<PresentationBusinessRoleDetail>(query, param).ToList();
        }
        public List<PresentationBusinessRoleDetail> GetSAPHANAIndirectRoleByPosition(long endpointSystemId, string userAccountName, string positionCode)
        {
            var query = $@"EXEC DBO.[USP_SAPHANA_INDIRECTROLE_BYPOSITION] @ENDPOINTSYSTEMID,@USERNAME,@POSITIONCODE";
            var epParam = new SqlParameter("@ENDPOINTSYSTEMID", endpointSystemId);
            var usrNameParam = new SqlParameter("@USERNAME", System.Data.SqlDbType.VarChar, 128);
            var positionParam = new SqlParameter("@POSITIONCODE", positionCode);

            if (!string.IsNullOrWhiteSpace(userAccountName))
            {
                usrNameParam.Value = userAccountName;
            }
            else
            {
                usrNameParam.Value = DBNull.Value;
            }

            if (!string.IsNullOrWhiteSpace(positionCode))
            {
                positionParam.Value = positionCode;
            }
            else
            {
                positionParam.Value = DBNull.Value;
            }
            object[] param = { epParam, usrNameParam, positionParam };
            return CurrentContext.Database.SqlQuery<PresentationBusinessRoleDetail>(query, param).ToList();
        }
        public List<SAPHANARoleInfo> GetListByUserName(string userName, string applicationtypes)
        {
            string query = string.Format("Select  rl.* from DI_SAPHANA_ROLEINFO rl INNER JOIN   SAPHANARoleInfoSAPHANAUserinfo userrole ON rl.Id=userrole.SAPHANARoleInfo_Id   INNER JOIN DI_SAPHANA_UserInfo U on userrole.SAPHANAUserinfo_Id = U.Id inner join DI_M_EndpointSystem E on rl.EndPointSystemId = E.id where U.UserLogonName='{0}' AND E.ApplicationType IN({3})  AND IsDirectlyAttached ={1} and convert(datetime, role_todate, 102) >= convert(datetime, {2}, 131)  ", userName, 1, DateTime.Now.Date.ToShortDateString(), applicationtypes);
            return CurrentContext.Database.SqlQuery<SAPHANARoleInfo>(query).ToList();
        }
       
        
    }
}
