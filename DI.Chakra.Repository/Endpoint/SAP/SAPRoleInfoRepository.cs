using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Logger;
using System.Data.SqlClient;

namespace DI.Chakra.Repository
{
    public class SAPRoleInfoRepository : RepositoryBase<SAPRoleInfo>
    {
        public SAPRoleInfoRepository(DataAccessTransaction dataAccessTransaction) : base(dataAccessTransaction) { }
        
        public List<PresentationSAPRoleReport> GetAllSAPRolesReportData(long endPointSystemId)
        {
            return (from sapRole in CurrentContext.SAPRoleInfos
                    where sapRole.EndPointSystemId == endPointSystemId
                    select new PresentationSAPRoleReport

                    {
                        SAP_ROLEID = (int)sapRole.Id,
                        ROLE_CODE = sapRole.ROLE_CODE,
                        ROLE_DESC = sapRole.ROLE_DESC
                    }
                    )
                    .ToList();
        }

        public List<PresentationAccessRequestItem> GetAccessRequestItem(string searchString)
        {
            return (from roles in CurrentContext.SAPRoleInfos
                    where roles.ROLE_CODE.Contains(searchString) && roles.CanRequest == true
                    orderby roles.ROLE_CODE
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Entitlement,
                        EntitlementType = Infrastructure.Enums.EntitlementType.Role,
                        EndpointType = EndpointType.SAP,
                        KeyId = roles.Id,
                        EndpointSystemId = roles.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.SAP,
                        ItemName = roles.ROLE_CODE,
                        ItemDescription = roles.ROLE_DESC,
                        ItemDetail = roles.UserDefinedDesc
                    }).ToList();
        }

        public void UpdateInfo(long endpointSystemId, long userId)
        {
            string query = "UPDATE [SAPRoleInfoSAPUserinfo] SET EndpointSystemId = " + endpointSystemId + " WHERE SAPUserinfo_Id = " + userId;
            CurrentContext.Database.ExecuteSqlCommand(query);

        }

        public List<SAPRoleInfo> GetListByUserId(long userId)
        {
            string query = string.Format("Select  rl.* from DI_SAP_ROLEINFO rl INNER JOIN   SAPRoleInfoSAPUserinfo userrole ON rl.Id=userrole.SAPRoleInfo_Id where userrole.SAPUserinfo_Id={0} and IsDirectlyAttached ={1} and convert(datetime, role_todate, 102) >= convert(datetime, {2}, 131)  ", userId, 1, DateTime.Now.Date.ToShortDateString());
            return CurrentContext.Database.SqlQuery<SAPRoleInfo>(query).ToList();
        }


        public List<PresentationSAPUsersToRolesReport> GetUserRoles(long sap_userId, long endpointSystemId)
        {
                                string query = $@"select u.Id as UserId, u.UserLogonName, '' DisplayName ,'' as Department,
                         u.IsActive, r.ROLE_CODE as RoleCode,r.ROLE_DESC as RoleDescription,
	                        Role_FromDate, Role_ToDate , r.Id as RoleId

                     from SAPRoleInfoSAPUserinfo ur
                     inner join DI_SAP_UserInfo u on ur.SAPUserinfo_Id = u.Id
                     inner join DI_SAP_ROLEINFO r on ur.SAPRoleInfo_Id = r.Id
                     where u.Id = {sap_userId} and u.EndPointSystemId = {endpointSystemId}   and ur.IsDirectlyAttached = 1 ";

            return CurrentContext.Database.SqlQuery<PresentationSAPUsersToRolesReport>(query).ToList(); 
        }

        public IList<SAPRoleInfo> GetByEndPointSystemId(long endpointId)
        {
            IList<SAPRoleInfo> sapRoleList = new List<SAPRoleInfo>();

            // TODO: //ORDER BY ROLE_CODE COLLATE Latin1_General_BIN
            var _list1 = (from sr in CurrentContext.SAPRoleInfos
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
            Dictionary<long, string> sapRoles = (from sapRole in CurrentContext.SAPRoleInfos
                                                 where sapRole.ROLE_CODE.Contains(searchString) && sapRole.EndPointSystemId == endpointId
                                                 select new { sapRole.Id, sapRole.ROLE_CODE }).Distinct().Take(100).OrderBy(s => s.ROLE_CODE)
                         .ToDictionary(c => c.Id, c => c.ROLE_CODE);
            return sapRoles;
        }

        public List<PresentationAccessRequestItem> GetSAPAccessRequestItemIncludingentitlrment(string searchString, long? DIUserId)
        {
            return (from groups in CurrentContext.SAPRoleInfos
                    where groups.ROLE_CODE.Contains(searchString)
                    orderby groups.ROLE_CODE
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Entitlement,
                        EndpointType = EndpointType.SAP,
                        KeyId = groups.Id,
                        EndpointSystemId = groups.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.SAP,
                        ItemName = groups.ROLE_CODE,
                        ItemDescription = groups.ROLE_DESC
                    }).ToList();
        }

        public List<PresentationAccessRequestItem> GetAccessRequestItemForRemoveRequestSpecailACR(string SearchKey, long? DIUserId)
        {
            return (from groups in CurrentContext.SAPRoleInfos
                    where groups.ROLE_CODE.Contains(SearchKey)
                    orderby groups.ROLE_CODE
                    select new PresentationAccessRequestItem
                    {
                        AccessRequestItemType = AccessRequestItemType.Entitlement,
                        EndpointType = EndpointType.SAP,
                        KeyId = groups.Id,
                        EndpointSystemId = groups.EndPointSystemId,
                        EndpointSystemTypeId = (int)EndpointType.SAP,
                        ItemName = groups.ROLE_CODE,
                        ItemDescription = groups.ROLE_DESC
                    }).ToList();
        }

        public List<PresentationSAPRoleWithNoUsers> GetSAPRolesWithNoUsers(long endPointSystemId)
        {
            return (from sapRole in CurrentContext.SAPRoleInfos
                    where sapRole.EndPointSystemId == endPointSystemId && sapRole.SAPUserinfo.Count == 0
                    select new PresentationSAPRoleWithNoUsers

                    {
                        SAP_ROLEID = (int)sapRole.Id,
                        ROLE_CODE = sapRole.ROLE_CODE,
                        ROLE_DESC = sapRole.ROLE_DESC
                    }
                    )
                    .ToList();
        }

        public List<PresentationSAPRoleToUsers> GetSAPRolesToUsers(long endPointSystemId)
        {
            return (from sapRole in CurrentContext.SAPRoleInfos
                    from sapUser in sapRole.SAPUserinfo
                    where sapRole.EndPointSystemId == endPointSystemId && sapRole.ROLE_DESC != string.Empty
                    select new PresentationSAPRoleToUsers

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

        

        public Dictionary<long, string> GetBusinessRoles(string searchString, long EndpointUserId, ApplicationType _applicationType, AdHocActionType actionType,string subModule,string module)
        {

            long key = 0;

            string sapRolesQuery = string.Format("SELECT rinfo.ROLE_CODE FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id= rinfo.Id AND ruinfo.IsDirectlyAttached=1 WHERE ruinfo.SAPUserinfo_Id={0}", EndpointUserId);

            List<string> SapRoles = CurrentContext.Database.SqlQuery<string>(sapRolesQuery).ToList();

            var businessRolesQuery = CurrentContext.SAPBIRoleCategory.Select(b => b);

            if (!string.IsNullOrWhiteSpace(module))
            {
                businessRolesQuery = businessRolesQuery.Where(b => b.Module == module);
            }

            if (!string.IsNullOrWhiteSpace(subModule))
            {
                businessRolesQuery = businessRolesQuery.Where(b => b.SubModule == subModule);
            }


            if (actionType == AdHocActionType.AmendmentRemove)
            {
                businessRolesQuery = businessRolesQuery.Where(businessRole => SapRoles.Contains(businessRole.TechnicalRole)).Select(businessRole => businessRole);
            }
            else
            {
                businessRolesQuery = businessRolesQuery.Where(businessRole => !SapRoles.Contains(businessRole.TechnicalRole)).Select(businessRole => businessRole); 
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                businessRolesQuery = businessRolesQuery.Where(item => item.BusinessRole.StartsWith(searchString) || item.BusinessRole.Contains(searchString));
            }

            DILog.ProvisioningLogger.WriteLog(Logger.LogLevelType.DebugStep, $"FLINS BI : Get Business Roles Query - {actionType.ToString()}:{businessRolesQuery.ToTraceQuery()}");


            return businessRolesQuery.Select(b => b.BusinessRole).Distinct().OrderBy(item => item).ToDictionary(item => key++, item => item);
        }

        public Dictionary<long, string> GetBusinessRoles(long EndpointUserId, ApplicationType _applicationType, string module = null, string subModule = null)
        {
            try
            {
                if (_applicationType == ApplicationType.HRMS_FLINS)
                {
                    string sapRolesQuery = string.Format("SELECT rinfo.ROLE_CODE FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id= rinfo.Id AND ruinfo.IsDirectlyAttached=1 WHERE ruinfo.SAPUserinfo_Id={0}", EndpointUserId);

                    List<string> SapRoles = CurrentContext.Database.SqlQuery<string>(sapRolesQuery).ToList();

                    var businessRoles = (from businessRole in CurrentContext.SAPHRFLINSRoleMatrix
                                         where SapRoles.Contains(businessRole.TechnicalRule) && (businessRole.MainSupp == "main" 
                                         )
                                         select businessRole);

                    if (!string.IsNullOrWhiteSpace(module))
                    {
                        businessRoles = businessRoles.Where(i => i.Module == module);
                    }
                    if (!string.IsNullOrWhiteSpace(subModule))
                    {
                        businessRoles = businessRoles.Where(i => i.SubModule == subModule);
                    }

                    var resultBusinessRoles = businessRoles.Select(i => i.BusinessRole).ToList().Distinct().OrderBy(item => item);
                    //add dummy key and return
                    long key = 1;
                    return resultBusinessRoles.ToDictionary(item => key++, item => item);

                }
                else if (_applicationType == ApplicationType.FLINS_BI)
                {
                    List<string> SapRoles = (from user in CurrentContext.SAPUserinfos
                                             where user.Id == EndpointUserId
                                             from role in user.SAPRoleInfo
                                             select role.ROLE_CODE).ToList();

                    return (from businessRole in CurrentContext.SAPBIRoleCategory
                            join tRoles in SapRoles on businessRole.TechnicalRole equals tRoles
                            select businessRole)
                           .Select(role => new { key = role.Id, value = role.BusinessRole }).OrderBy(item => item.value)
                           .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }

                else if (_applicationType == ApplicationType.EBS_CRM || _applicationType == ApplicationType.EBS_ISU)//|| _applicationType == ApplicationType.EBS_BI)
                {
                    var applicationName = _applicationType == ApplicationType.EBS_ISU ? Constants.EBSISP : _applicationType == ApplicationType.EBS_CRM ? Constants.EBSCRP : Constants.EBSBI;

                    string sapRolesQuery = string.Format("SELECT rinfo.ROLE_CODE FROM SAPRoleInfoSAPUserinfo ruinfo JOIN DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id= rinfo.Id AND ruinfo.IsDirectlyAttached=1 WHERE ruinfo.SAPUserinfo_Id={0}", EndpointUserId);

                    List<string> SapRoles = CurrentContext.Database.SqlQuery<string>(sapRolesQuery).ToList();

                    return (from businessRole in CurrentContext.SAPEBSXLRolesMatrices
                            join tRoles in SapRoles on businessRole.TechnicalRole equals tRoles
                            where businessRole.Application == applicationName
                            select businessRole)
                           .Select(role => new { key = role.Id, value = role.RoleName }).OrderBy(item => item.value)
                           .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }

                else if (_applicationType == ApplicationType.MSSL_CRM || _applicationType == ApplicationType.MSSL_ISU)
                {
                    var applicationName = _applicationType == ApplicationType.MSSL_ISU ? Constants.MSSLMSP : _applicationType == ApplicationType.MSSL_CRM ? Constants.MSSLMCP : Constants.MSSLBI;

                    string sapRolesQuery = string.Format("SELECT rinfo.ROLE_CODE FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id= rinfo.Id AND ruinfo.IsDirectlyAttached=1 WHERE ruinfo.SAPUserinfo_Id={0}", EndpointUserId);

                    List<string> SapRoles = CurrentContext.Database.SqlQuery<string>(sapRolesQuery).ToList();

                    return (from businessRole in CurrentContext.SAPXLMSSLRolesMatrices
                            join tRoles in SapRoles on businessRole.TechnicalRole equals tRoles
                            where businessRole.Application == applicationName
                            select businessRole)
                           .Select(role => new { key = role.Id, value = role.RoleName }).OrderBy(item => item.value)
                           .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }

                else if (_applicationType == ApplicationType.SAP_GRC)
                {
                    List<string> SapRoles = (from user in CurrentContext.SAPUserinfos
                                             where user.Id == EndpointUserId
                                             from role in user.SAPRoleInfo
                                             select role.ROLE_CODE).ToList();

                    return (from businessRole in CurrentContext.SAPGRCRole
                            join tRoles in SapRoles on businessRole.TechnicalRole equals tRoles
                            select businessRole)
                           .Select(role => new { key = role.Id, value = role.BusinessRoleName }).OrderBy(item => item.value)
                           .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }

                else if (_applicationType == ApplicationType.SAP_PI)
                {
                    List<string> SapRoles = (from user in CurrentContext.SAPUserinfos
                                             where user.Id == EndpointUserId
                                             from role in user.SAPRoleInfo
                                             select role.ROLE_CODE).ToList();


                    return (from businessRole in CurrentContext.SAPPIRoles
                            join tRoles in SapRoles on businessRole.TechnicalRole equals tRoles
                            select businessRole)
                           .Select(role => new { key = role.Id, value = role.BusinessRoleName }).OrderBy(item => item.value)
                           .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }
                else if (_applicationType == ApplicationType.GEMS)
                {
                    List<string> techRoles = (from user in CurrentContext.GEMSUserInfo.Include("GEMSRoles")
                                              where user.Id == EndpointUserId
                                              from role in user.GEMSRoles
                                              select role.Display_Name).ToList();

                    long? DI_User_Id = CurrentContext.GEMSUserInfo.Join(CurrentContext.AccountReconcilations,
                                                                        gems => gems.Id,
                                                                        acconRecon => acconRecon.EndpointUserId,
                                                                        (gems, acconRecon) => new { GEMS = gems, AccountRecon = acconRecon })
                                                                        .Where(GemsAccountRecon => GemsAccountRecon.AccountRecon.UserId.HasValue && GemsAccountRecon.GEMS.Id == EndpointUserId)
                                                                        .Select(GemsAccountRecon => GemsAccountRecon.AccountRecon.UserId.Value).FirstOrDefault();


                    List<ApplicationType> AD_applicationTypes = new List<ApplicationType> { ApplicationType.ADSDCLocal, ApplicationType.ADSingaporePowerLocal };

                    List<string> adGroups = CurrentContext.EntitlementReconcilations.Join(CurrentContext.EndPoint,
                                                                                            ER => ER.EndpointSystemId,
                                                                                            EP => EP.Id,
                                                                                            (ER, EP) => new { ER = ER, EP = EP })
                                                                                            .Where(EREP => AD_applicationTypes.Contains(EREP.EP.ApplicationType)
                                                                                             && !EREP.ER.IsDeleted && EREP.ER.EntitlementType == (int)EntitlementType.Group
                                                                                             && EREP.ER.UserId == DI_User_Id)
                                                                                             .Select(EREP => EREP.ER.Entitlement).ToList();

                    List<string> bussRolesByAD = (from gemsMatrixItem in CurrentContext.GEMSRoleMatrix
                                                  from adGroup in adGroups
                                                  where gemsMatrixItem.ADGroups.Contains(adGroup)
                                                  select gemsMatrixItem.BusinessRoleName).ToList();

                    List<string> bussRolesByTechRole = (from gemsMatrixItem in CurrentContext.GEMSRoleMatrix
                                                        from techRole in techRoles
                                                        where gemsMatrixItem.TechnicalRole.Contains(techRole)
                                                        select gemsMatrixItem.BusinessRoleName).ToList();

                    List<string> bussRoles = bussRolesByAD.Union(bussRolesByTechRole).Distinct().ToList();

                    return (from matrix in CurrentContext.GEMSRoleMatrix
                            where bussRoles.Contains(matrix.BusinessRoleName) // == bussRole
                            && matrix.ValidFrom <= DateTime.Now && matrix.ValidTo >= DateTime.Now
                            select matrix)
                           .Select(role => new { key = role.Id, value = role.BusinessRoleName }).OrderBy(item => item.value)
                           .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }
                else if (_applicationType == ApplicationType.EBS || _applicationType == ApplicationType.MSSL)//|| _applicationType == ApplicationType.EBS_BI)
                {
                    var ep = CurrentContext.EndPoint.FirstOrDefault(i=>i.ApplicationType == _applicationType);
                    var applicationName = ep.ApplicationCode;

                    string sapRolesQuery = string.Format("SELECT rinfo.ROLE_CODE FROM SAPHANARoleInfoSAPHANAUserinfo ruinfo JOIN DI_SAPHANA_ROLEINFO rinfo ON ruinfo.SAPHANARoleInfo_Id= rinfo.Id AND ruinfo.IsDirectlyAttached=1 WHERE ruinfo.SAPUserinfo_Id={0}", EndpointUserId);

                    List<string> SapRoles = CurrentContext.Database.SqlQuery<string>(sapRolesQuery).ToList();

                    return (from businessRole in CurrentContext.SAPHANAXLRolesMatrices
                            join tRoles in SapRoles on businessRole.TechnicalRole equals tRoles
                            where businessRole.Application == applicationName
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

        public List<SapRoleToAuthorizationReportData> GetSapRolesToAuthorizationData(string RoleCode, int EndpointSystemId)
        {
            try
            {
                string query = "select distinct roleauth.Id, roleauth.RoleName, roleauth.AuthCode,  roleauth.ObjectCode, roleauth.FieldCode, case when fieldvalue.FromValue is not null then fieldvalue.FromValue else roleauth.fromvalue end as FromValue, roleauth.ToValue from DI_SAP_RoleToAuthMap roleauth  left join DI_SAP_FIELDVALUEMAPPING fieldvalue on roleauth.FromValue = fieldvalue.FieldCode and roleauth.rolename = fieldvalue.RoleCode and left(roleauth.fromvalue, 1) = '$'	where roleauth.endpointsystemid = @EndpointSystemId and roleauth.RoleName = @RoleCode and roleauth.ObjectCode != 'S_TCODE' and roleauth.ProfileGeneratorVariant != 'X'";
                var queryParameters = new[]
                {
                    new System.Data.SqlClient.SqlParameter("@EndpointSystemId", EndpointSystemId),
                    new System.Data.SqlClient.SqlParameter("@RoleCode", (object)RoleCode ?? DBNull.Value)
                };
                var RoleAuth = CurrentContext.Database.SqlQuery<SapRoleToAuthorizationReportData>(query, queryParameters);

                return RoleAuth.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, string> GetBusinessRoles(long EndpointUserId)
        {
            long key = 0;
            string sapRolesQuery = string.Format("SELECT rinfo.ROLE_CODE FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id= rinfo.Id AND ruinfo.IsDirectlyAttached=1 WHERE ruinfo.SAPUserinfo_Id={0}", EndpointUserId);
            List<string> SapRoles = CurrentContext.Database.SqlQuery<string>(sapRolesQuery).ToList();
            var businessRolesQuery = CurrentContext.SAPBIRoleCategory.Select(b => b);
            businessRolesQuery = businessRolesQuery.Where(businessRole => SapRoles.Contains(businessRole.TechnicalRole)).Select(businessRole => businessRole);
            return businessRolesQuery.Select(b => b.BusinessRole).Distinct().OrderBy(item => item).ToDictionary(item => key++, item => item);
        }
        public List<PresentationBusinessRoleDetail> GetSAPBusinessRole(long endpointSystemId, long endpointUserId, ApplicationType applicationType)
        {
            CurrentContext.Database.CommandTimeout = 0;
            string sapRolesQuery = string.Empty;
            var sapRoles = new List<PresentationBusinessRoleDetail>();
            if (applicationType == ApplicationType.HRMS_FLINS)
            {
                sapRolesQuery =$@"SELECT EndpointSystemId,EndpointUserId,AccountName,BusinessRole,BusinessRole as Description,Company,ValidToDate FROM [dbo].[VWHRMSFLIN_USER_MAIN_BUSINESSROLE] WHERE EndpointSystemId = {endpointSystemId} and EndpointUserId={endpointUserId}";
            }
            #region EBS
            else if (applicationType == ApplicationType.EBS_CRM || applicationType == ApplicationType.EBS_ISU)
            {
                sapRolesQuery = $@"SELECT EndpointSystemId,EndpointUserId,BusinessRole,Description,MAX(ValidToDate) as ValidToDate from dbo.VW_EBSISU_CRM_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}
                    GROUP BY EndpointSystemId,EndpointUserId,BusinessRole,Description ORDER BY BusinessRole";
            }
            #endregion
            #region MSSL
            else if (applicationType == ApplicationType.MSSL_CRM || applicationType == ApplicationType.MSSL_ISU)
            {
                sapRolesQuery = $@"SELECT EndpointSystemId,EndpointUserId,BusinessRole,Description,MAX(ValidToDate) as ValidToDate from dbo.VW_MSSLISU_CRM_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}
                    GROUP BY EndpointSystemId,EndpointUserId,BusinessRole,Description ORDER BY BusinessRole";
            }
            #endregion
            else if(applicationType == ApplicationType.SAP_GRC)
            {
                sapRolesQuery = $@"SELECT EndpointSystemId,EndpointUserId,BusinessRole,Description,MAX(ValidToDate) as ValidToDate from dbo.VW_SAPGRC_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}
                    GROUP BY EndpointSystemId,EndpointUserId,BusinessRole,Description ORDER BY BusinessRole";
            }
            else if (applicationType == ApplicationType.FLINS_BI)
            {
                sapRolesQuery = $@"SELECT EndpointSystemId,EndpointUserId,BusinessRole,Description,MAX(ValidToDate) as ValidToDate from dbo.VW_SAPFLINSBI_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}
                    GROUP BY EndpointSystemId,EndpointUserId,BusinessRole,Description ORDER BY BusinessRole";
            }
            if(!string.IsNullOrWhiteSpace(sapRolesQuery))
                sapRoles = CurrentContext.Database.SqlQuery<PresentationBusinessRoleDetail>(sapRolesQuery).ToList();

            return sapRoles;
        }

        public List<PresentationSAPRoleReport> GetTechnicalRolesFromBusinessRole(long endpointSystemId, long endpointUserId, string businessrole, ApplicationType applicationType)
        {
            var sapRoles = new List<PresentationSAPRoleReport>();
            string sapRolesQuery = null;
            switch (applicationType)
            {
                case ApplicationType.MSSL_ISU:
                case ApplicationType.MSSL_CRM:
                    #region
                    sapRolesQuery = $@"select SAP_ROLEID,EndpointSystemId,EndpointUserId,ROLE_CODE,ROLE_DESC,MAX(ValidToDate) as ValidToDate from dbo.VW_MSSLISU_CRM_ROLES WHERE ENDPOINTSYSTEMID={endpointSystemId} AND EndpointUserId={endpointUserId} AND BusinessRole = '{businessrole.Replace("'","''")}' GROUP BY SAP_ROLEID,EndpointSystemId,EndpointUserId,ROLE_CODE,ROLE_DESC ORDER BY ROLE_CODE";
                    #endregion
                    break;
                case ApplicationType.EBS_CRM:
                case ApplicationType.EBS_ISU:
                    #region
                    sapRolesQuery = $@"SELECT SAP_ROLEID,EndpointSystemId,EndpointUserId,ROLE_CODE,ROLE_DESC,MAX(ValidToDate) as ValidToDate from dbo.VW_EBSISU_CRM_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}  AND BusinessRole = '{businessrole.Replace("'", "''")}'
                    GROUP BY SAP_ROLEID,EndpointSystemId,EndpointUserId,ROLE_CODE,ROLE_DESC ORDER BY ROLE_CODE";

                    #endregion
                    break;
                case ApplicationType.SAP_GRC:
                    #region
                    sapRolesQuery = $@"SELECT SAP_ROLEID,EndpointSystemId,EndpointUserId,ROLE_CODE,ROLE_DESC,MAX(ValidToDate) as ValidToDate from dbo.VW_SAPGRC_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}  AND BusinessRole = '{businessrole.Replace("'", "''")}'
                    GROUP BY SAP_ROLEID,EndpointSystemId,EndpointUserId,ROLE_CODE,ROLE_DESC ORDER BY ROLE_CODE";
                    #endregion
                    break;
                case ApplicationType.FLINS_BI:
                    #region
                    sapRolesQuery = $@"SELECT SAP_ROLEID,EndpointSystemId,EndpointUserId,ROLE_CODE,ROLE_DESC,MAX(ValidToDate) as ValidToDate from dbo.VW_SAPFLINSBI_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId} AND BusinessRole = '{businessrole.Replace("'", "''")}'
                    GROUP BY SAP_ROLEID,EndpointSystemId,EndpointUserId,ROLE_CODE,ROLE_DESC ORDER BY ROLE_CODE";
                    #endregion
                    break;
                default: break;
            }
            if(!string.IsNullOrWhiteSpace(sapRolesQuery))
                sapRoles = CurrentContext.Database.SqlQuery<PresentationSAPRoleReport>(sapRolesQuery).ToList();

            return sapRoles;
        }

        public PresentationBusinessRoleDetail GetSAPBusinessRole(long endpointSystemId, long endpointUserId, ApplicationType applicationType,string businessRole)
        {
            string sapRolesQuery = string.Empty;
            var sapRole = new PresentationBusinessRoleDetail();
            if (applicationType == ApplicationType.HRMS_FLINS)
            {
                sapRolesQuery = $@"select EndpointSystemId,EndpointUserId,BusinessRole,MAX(ValidToDate) as ValidToDate from dbo.VW_HRMSFLINS WHERE ENDPOINTSYSTEMID={endpointSystemId} AND EndpointUserId={endpointUserId} GROUP BY EndpointSystemId,EndpointUserId,BusinessRole ORDER BY BusinessRole";
            }
            #region EBS
            else if (applicationType == ApplicationType.EBS_CRM || applicationType == ApplicationType.EBS_ISU)
            {
                sapRolesQuery = $@"SELECT EndpointSystemId,EndpointUserId,BusinessRole,MAX(ValidToDate) as ValidToDate from dbo.VW_EBSISU_CRM_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}
                    GROUP BY EndpointSystemId,EndpointUserId,BusinessRole ORDER BY BusinessRole";
            }
            #endregion
            #region MSSL
            else if (applicationType == ApplicationType.MSSL_CRM || applicationType == ApplicationType.MSSL_ISU)
            {
                sapRolesQuery = $@"SELECT EndpointSystemId,EndpointUserId,BusinessRole,MAX(ValidToDate) as ValidToDate from dbo.VW_MSSLISU_CRM_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}
                    GROUP BY EndpointSystemId,EndpointUserId,BusinessRole ORDER BY BusinessRole";
            }
            #endregion
            else if (applicationType == ApplicationType.SAP_GRC)
            {
                sapRolesQuery = $@"SELECT EndpointSystemId,EndpointUserId,BusinessRole,MAX(ValidToDate) as ValidToDate from dbo.VW_SAPGRC_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}
                    GROUP BY EndpointSystemId,EndpointUserId,BusinessRole ORDER BY BusinessRole";
            }
            else if (applicationType == ApplicationType.FLINS_BI)
            {
                sapRolesQuery = $@"SELECT EndpointSystemId,EndpointUserId,BusinessRole,MAX(ValidToDate) as ValidToDate from dbo.VW_SAPFLINSBI_ROLES WHERE ENDPOINTSYSTEMID = {endpointSystemId} AND EndpointUserId={endpointUserId}
                    GROUP BY EndpointSystemId,EndpointUserId,BusinessRole ORDER BY BusinessRole";
            }
            if (!string.IsNullOrWhiteSpace(sapRolesQuery))
                sapRole = CurrentContext.Database.SqlQuery<PresentationBusinessRoleDetail>(sapRolesQuery).FirstOrDefault();

            return sapRole;
        }
        public List<PresentationBusinessRoleDetail> GetMSSLEBSIndirectRole(long endpointSystemId, long accountReconId)
        {
            var query = $@"EXEC DBO.USP_MSSL_EBS_INDIRECTROLE @ENDPOINTSYSTEMID,@ACCOUNTRECONID";
            var epParam = new SqlParameter("@ENDPOINTSYSTEMID", endpointSystemId);
            var acctReconParam = new SqlParameter("@ACCOUNTRECONID", accountReconId);
            object[] param = { epParam, acctReconParam };
            return CurrentContext.Database.SqlQuery<PresentationBusinessRoleDetail>(query, param).ToList();
        }
        public List<PresentationBusinessRoleDetail> GetMSSLEBSIndirectRoleByPosition(long endpointSystemId,string userAccountName, string positionCode)
        {
            var query = $@"EXEC DBO.USP_MSSL_EBS_INDIRECTROLE_BYPOSITION @ENDPOINTSYSTEMID,@USERNAME,@POSITIONCODE";
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
        public List<PresentationBusinessRoleDetail> GetSAPIndirectRole(ApplicationType applnType, string userAccountName)
        {

            List<PresentationBusinessRoleDetail> result = new List<PresentationBusinessRoleDetail>();
            
            var applnParam = new SqlParameter("@APPLICATIONTYPE", (int)applnType);
            var usrNameParam = new SqlParameter("@USERNAME",System.Data.SqlDbType.VarChar,128);
            if (!string.IsNullOrWhiteSpace(userAccountName))
            {
                usrNameParam.Value = userAccountName;
            }
            else
            {
                usrNameParam.Value = DBNull.Value;
            }
            object[] param = { applnParam , usrNameParam };
            var query = string.Empty;
            if (applnType == ApplicationType.FLINS_BI)
            {
                query = $@"EXEC DBO.USP_SAPGRC_SAPFLINSBI_TECHNICALROLEWITHOUTBUSINESSROLE @APPLICATIONTYPE,@USERNAME";
            }
            else if (applnType == ApplicationType.SAP_GRC)
            {
                query = $@"EXEC DBO.USP_SAPGRC_SAPFLINSBI_TECHNICALROLEWITHOUTBUSINESSROLE @APPLICATIONTYPE,@USERNAME";
            }
            if (!string.IsNullOrWhiteSpace(query))
            {
                return CurrentContext.Database.SqlQuery<PresentationBusinessRoleDetail>(query, param).ToList();
            }
            return result;
        }

        public List<SAPRoleInfo> GetListByUserName(string userName, string applicationtypes)
        {
            string query = string.Format("Select  rl.* from DI_SAP_ROLEINFO rl INNER JOIN   SAPRoleInfoSAPUserinfo userrole ON rl.Id=userrole.SAPRoleInfo_Id   INNER JOIN DI_SAP_UserInfo U on userrole.SAPUserinfo_Id = U.Id inner join DI_M_EndpointSystem E on rl.EndPointSystemId = E.id where U.UserLogonName='{0}' AND E.ApplicationType IN({3})  AND IsDirectlyAttached ={1} and convert(datetime, role_todate, 102) >= convert(datetime, {2}, 131)  ", userName, 1, DateTime.Now.Date.ToShortDateString(), applicationtypes);
            return CurrentContext.Database.SqlQuery<SAPRoleInfo>(query).ToList();
        }

        public List<PresentationFlinsIndirectRole> GetSAPFlinsIndirctRole(ApplicationType applnType, long accountReconId)
        {
            List<PresentationFlinsIndirectRole> result = new List<PresentationFlinsIndirectRole>();

            CurrentContext.Database.CommandTimeout = 0;
            var applnParam = new SqlParameter("@APPLICATIONTYPE", (int)applnType);
            var usrNameParam = new SqlParameter("@AccountReconId", System.Data.SqlDbType.BigInt);
            usrNameParam.Value = accountReconId;

            object[] param = { applnParam, usrNameParam };
            var query = string.Empty;

            query = $@"EXEC DBO.USP_HRMSFLINS_INDIRECTROLE @AccountReconId,@APPLICATIONTYPE";
            result = CurrentContext.Database.SqlQuery<PresentationFlinsIndirectRole>(query, param).ToList();
            return result;
        }
        public List<PresentationFlinsTechnicalRole> GetSAPFlinsTechnicalRole(long endpointId, string accountName,string businessrole,string company)
        {
            List<PresentationFlinsTechnicalRole> result = new List<PresentationFlinsTechnicalRole>();

            CurrentContext.Database.CommandTimeout = 0;
            var endpointParam = new SqlParameter("@ENDPOINTSYSTEMID", System.Data.SqlDbType.BigInt);
            endpointParam.Value = endpointId;

            var accntParam = new SqlParameter("@AccountReconId", System.Data.SqlDbType.BigInt);
            accntParam.Value = 0;

            var bsRoleParam = new SqlParameter("@BUSINESSROLE", System.Data.SqlDbType.VarChar);
            bsRoleParam.Value = businessrole;

            var cmpParam = new SqlParameter("@COMPANY", System.Data.SqlDbType.VarChar);
            cmpParam.Value = company;

            var accNameParam = new SqlParameter("@AccountName", System.Data.SqlDbType.VarChar,512);
            accNameParam.Value = accountName;

            object[] param = { endpointParam, accntParam, bsRoleParam, cmpParam, accNameParam };
            var query = $@"EXEC DBO.USP_HRMSFLINS_GETTECHNICALROLEFROM_BUSINESSROLE @ENDPOINTSYSTEMID,@AccountReconId,@BUSINESSROLE,@COMPANY,@AccountName";
            result = CurrentContext.Database.SqlQuery<PresentationFlinsTechnicalRole>(query, param).ToList();
            return result;
        }

    }
}
