using DI.Chakra.Connector.SAPHANA;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace DI.Chakra.Connector.SAPHANA
{
    public class SAPHANAUser
    {
        private const string teleNumberPattern = "^[+]{1}65[0-9]{8}[ ]{1}[0-9]{4}$";
        string ServiceUser;

        public string SNCParamName { get; set; }
        public bool SSOEnabled { get; set; }
        public SAPHANAUser(string _serviceUser)
        {
            ServiceUser = _serviceUser;
        }
        public SAPHANAProcessorConfig ProcessorConfig { get; set; }
        Dictionary<string, long> RolesMaster { get; set; }
        EndPoint endPoint = null;
        void SetRolesMaster()
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started GetAllRoles.", DiscoveryObjectType.User);
            var roles = SAPHANARoleInfo.GetByEndpointSystemId(ProcessorConfig.EndpointId);
            RolesMaster = new Dictionary<string, long>();
            foreach (var role in roles)
            {
                if (!RolesMaster.ContainsKey(role.ROLE_CODE))
                    RolesMaster.Add(role.ROLE_CODE, role.Id);
            }
            endPoint = EndPoint.GetById(ProcessorConfig.EndpointId);
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed GetAllRoles.", DiscoveryObjectType.User);
        }
        public void StartProcess()
        {
            SetRolesMaster();
            // save current row count value
            var rowCount = ProcessorConfig.sapUtil.RowCount;

            // update to 1000 entries to be read per read
            ProcessorConfig.sapUtil.RowCount = 1000;

            if (ProcessorConfig.IsDelta)
                ProcessDeltaUsers();
            else
                ProcessUsers();

            // reset back to original count
            ProcessorConfig.sapUtil.RowCount = rowCount;
        }
        private void ProcessUsers()
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessUsers.", DiscoveryObjectType.User);
            var lstSAPUserinfo = new List<SAPHANAUserinfo>();

            ProcessorConfig.sapUtil.ResetPaging();
            DataTable dtUsers = null;
            List<string> lstUserLogonNames = new List<string>();
            try
            {
                
                int count = 0;
                SAPHANAUserinfo sapUser = null;
                //Note Stopping Paging due to issue in Paging. When we get 1000 records per page missing of account.
                //do
                {
                    lstSAPUserinfo = new List<SAPHANAUserinfo>();
                    var fields = new List<string> { "BNAME" };
                    //Set false for no paging
                    dtUsers = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USR01", null, fields, null,false);

                    lstUserLogonNames = new List<string>();

                    string UserLogonName;
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Total no.of User going to Discover as per USR01:{dtUsers.Rows.Count}", DiscoveryObjectType.User);
                    foreach (DataRow row in dtUsers.Rows)
                    {
                        UserLogonName = Convert.ToString(row["BNAME"].ToString().Trim());

                        lstUserLogonNames.Add(UserLogonName);

                        sapUser = null;
                        try
                        {
                            sapUser = ConstructUserInfo(UserLogonName);
                        }
                        catch (Exception ex)
                        {
                            DILog.DiscoveryLogger.WriteLog(LogLevelType.Warning, $"Exception occured for user:{UserLogonName}. Try retain the account if already exists.", DiscoveryObjectType.User,ex);
                            //Required when exception happened do not delete the account from IDMS end
                            sapUser = SAPHANAUserinfo.GetByUserLogonName(UserLogonName, ProcessorConfig.EndpointId);
                            if(sapUser == null)
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Warning, $"Account:{UserLogonName} doesnt exists in IDMS", ex);
                            }
                        }

                        if (sapUser != null)
                        {
                            lstSAPUserinfo.Add(sapUser);
                        }

                        if (lstSAPUserinfo.Count() % 1000 == 0 && lstSAPUserinfo.Count > 0)
                        {
                            SAPHANAUserinfo.SaveUsers(lstSAPUserinfo, ProcessorConfig.EndpointId, ServiceUser);
                            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Saved Users to DB: {lstSAPUserinfo.Count}", DiscoveryObjectType.User);
                            lstSAPUserinfo.Clear();

                            ProcessUserRoles(lstUserLogonNames);
                            lstUserLogonNames.Clear();
                        }
                        count++;
                    }

                    if (lstSAPUserinfo.Count > 0)
                    {
                        SAPHANAUserinfo.SaveUsers(lstSAPUserinfo, ProcessorConfig.EndpointId, ServiceUser);
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"Saved Users to DB: {lstSAPUserinfo.Count}", DiscoveryObjectType.User);
                        lstSAPUserinfo.Clear();
                    }
                    ProcessUserRoles(lstUserLogonNames);
                    //ProcessorConfig.sapUtil.NextPage();

                } //while (dtUsers.Rows.Count > 0);

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessUsers. Count : " + count.ToString(), DiscoveryObjectType.User);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, DiscoveryObjectType.User, ex);
                throw ex;
            }
            finally
            {
                dtUsers?.Dispose();
                dtUsers = null;

                lstSAPUserinfo?.Clear();
                lstSAPUserinfo = null;

                lstUserLogonNames?.Clear();
                lstUserLogonNames = null;
            }
            // return lstSAPUserinfo;
        }
        private IList<string> ProcessDeltaUsers()
        {
            string deltaDate = ProcessorConfig.DeltaDate.ToString("yyyyMMdd");
            ProcessorConfig.sapUtil.NextPage();

            var lstSAPUserinfo = new List<SAPHANAUserinfo>();
            var lstSAPDeltaUsers = new List<string>();
            try
            {
                string filter = "MODDATE >= '" + deltaDate + "' ";
                DataTable dtUsers = null;
                var sapUser = new SAPHANAUserinfo();
                do
                {
                    dtUsers = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USRSTAMP", null, null, filter, ProcessorConfig.sapUtil.IsPaginationEnabled);

                    foreach (DataRow row in dtUsers.Rows)
                    {
                        string UserName = row["USERNAME"].ToString().Trim();
                        lstSAPDeltaUsers.Add(UserName);
                    }

                    lstSAPDeltaUsers = lstSAPDeltaUsers.Distinct().ToList();

                    foreach (var UserLogonName in lstSAPDeltaUsers)
                    {
                        sapUser = null;
                        try
                        {
                            sapUser = ConstructUserInfo(UserLogonName);
                        }
                        catch (Exception)
                        {
                            //Required when exception happened do not delete the account from IDMS end
                            sapUser = SAPHANAUserinfo.GetByUserLogonName(UserLogonName, ProcessorConfig.EndpointId);
                        }
                        if (sapUser != null)
                        {
                            lstSAPUserinfo.Add(sapUser);
                        }
                    }

                    if (lstSAPUserinfo.Count > 0)
                    {
                        SAPHANAUserinfo.SaveUsers(lstSAPUserinfo, ProcessorConfig.EndpointId, ServiceUser);
                        lstSAPUserinfo.Clear();
                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();

                    foreach (var UserLogonName in lstSAPDeltaUsers)
                    {
                        //ProcessUserGroups(UserLogonName);
                        //ProcessUserProfiles(UserLogonName);
                        ProcessUserRoles(UserLogonName);
                    }

                } while (true && ProcessorConfig.sapUtil.IsPaginationEnabled);

            }

            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, DiscoveryObjectType.User, ex);

            }
            return lstSAPDeltaUsers;
        }

        public SAPHANAUserinfo ConstructUserInfo(string UserLogonName)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            try
            {

                //if (UserLogonName.Trim() != "SAP*")
                {
                    dicParams = new Dictionary<string, object>();
                    dicParams.Add("USERNAME", UserLogonName);

                    IRfcStructure structAddress = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_GET_DETAIL", dicParams, "ADDRESS");
                    IRfcStructure structIsLocked = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_GET_DETAIL", dicParams, "ISLOCKED");
                    IRfcStructure structDefaults = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_GET_DETAIL", dicParams, "DEFAULTS");
                    IRfcStructure structLogonData = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_GET_DETAIL", dicParams, "LOGONDATA");
                    IRfcStructure structAlias = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_GET_DETAIL", dicParams, "ALIAS");
                    IRfcStructure structSNC = null;

                    if (SSOEnabled && !string.IsNullOrWhiteSpace(SNCParamName))
                    {
                        structSNC = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_GET_DETAIL", dicParams, "SNC");
                    }
                    var alias = structAlias.GetValue("Useralias").ToString();
                    dicParams = new Dictionary<string, object>();
                    dicParams.Add("ALIASNAME", alias);
                    dicParams.Add("USERID", UserLogonName);

                    IRfcFunction fnLastLogin = null;
                    try
                    {
                        fnLastLogin = ProcessorConfig.sapUtil.getRfcFunction("SUSR_GET_ADMIN_USER_LOGIN_INFO", dicParams);
                    }
                    catch (Exception ex)
                    {
                    }
                    SAPHANAUserinfo UserInfo = new SAPHANAUserinfo();
                    UserInfo.UserLogonName = UserLogonName;
                    UserInfo.Firstname = structAddress.GetValue("Firstname").ToString().Trim();
                    UserInfo.EndPointSystemId = ProcessorConfig.EndpointId;
                    UserInfo.Lastname = structAddress.GetValue("Lastname").ToString().Trim();
                    UserInfo.Initials = structAddress.GetValue("Initials").ToString().Trim();
                    UserInfo.Department = structAddress.GetValue("Department").ToString().Trim();
                    UserInfo.E_Mail = structAddress.GetValue("E_Mail").ToString().Trim();
                    UserInfo.Pers_No = structAddress.GetValue("Pers_No").ToString().Trim();

                    var telephoneNumber = structAddress.GetValue("TEL1_EXT").ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(telephoneNumber))
                    {
                        //DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"UserLogonName:{ UserLogonName} telephone:{telephoneNumber}", DiscoveryObjectType.User);
                        if (telephoneNumber.Length >11 && Regex.IsMatch(telephoneNumber, teleNumberPattern, RegexOptions.IgnoreCase))
                        {
                            UserInfo.Tel1_Numbr = Regex.Split(telephoneNumber," ")[0];
                        }
                        else
                        {
                            UserInfo.Tel1_Numbr = telephoneNumber;
                        }
                    }
                    else
                    {
                        UserInfo.Tel1_Numbr = string.Empty;
                    }

                    UserInfo.Function0 = structAddress.GetValue("Function").ToString().Trim();


                    UserInfo.Title = structAddress.GetValue("Title").ToString().Trim();
                    UserInfo.City = structAddress.GetValue("City").ToString().Trim();
                    UserInfo.Namcountry = structAddress.GetValue("Country").ToString().Trim();
                    UserInfo.Pboxcit_No = structAddress.GetValue("Pboxcit_No").ToString().Trim();
                    UserInfo.Postl_Cod1 = structAddress.GetValue("Postl_Cod1").ToString().Trim();
                    UserInfo.Street = structAddress.GetValue("Street").ToString().Trim();
                    UserInfo.Start_Menu = structDefaults.GetValue("Start_Menu").ToString().Trim();
                    UserInfo.AccntLockNoPwd = structIsLocked.GetValue("No_User_Pw").ToString().Trim();
                    UserInfo.AccntLocDurPwd = structIsLocked.GetValue("Wrng_Logon").ToString().Trim();
                    UserInfo.SapUserType = structLogonData.GetValue("Ustyp").ToString().Trim();

                    if(fnLastLogin != null)
                        UserInfo.LastLoginTime = ProcessorConfig.sapUtil.getLastLogin(structLogonData, fnLastLogin);

                    if (structLogonData.GetValue("Gltgv") != null)
                    {
                        DateTime? vfrom = ProcessorConfig.sapUtil.getDateTime(structLogonData.GetValue("Gltgv").ToString());
                        if (vfrom.HasValue && vfrom == DateTime.MinValue)
                        {
                            UserInfo.validFromDate = null;
                        }
                        else
                        {
                            UserInfo.validFromDate = vfrom;
                        }
                    }

                    if (structLogonData.GetValue("GLTGB") != null)
                    {
                        DateTime? vto = ProcessorConfig.sapUtil.getDateTime(structLogonData.GetValue("GLTGB").ToString());
                        if (vto.HasValue && vto == DateTime.MinValue)
                        {
                            UserInfo.validToDate = null;
                        }
                        else
                        {
                            UserInfo.validToDate = vto;
                        }
                    }

                    bool userlocked = ((structIsLocked.GetValue("Glob_Lock") != null && structIsLocked.GetValue("Glob_Lock").ToString().ToUpper() == "L") || (structIsLocked.GetValue("Local_Lock") != null && structIsLocked.GetValue("Local_Lock").ToString().ToUpper() == "L"));
                    UserInfo.IsUserLocked = userlocked;
                    UserInfo.IsActive = true;
                    if (UserInfo.validToDate != null)
                        UserInfo.IsActive = UserInfo.validToDate > DateTime.Now;

                    try
                    {
                        //Read only for User
                        if(structSNC !=null && this.SSOEnabled && !string.IsNullOrWhiteSpace(this.SNCParamName) && !string.IsNullOrWhiteSpace(UserInfo.SapUserType) && UserInfo.SapUserType.Equals("A",StringComparison.InvariantCultureIgnoreCase))
                        {
                            UserInfo.SAPUserSNC = structSNC.GetValue(this.SNCParamName)?.ToString()?.Trim();
                        }
                    }
                    catch(Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.DebugStep, $"sso snc data retrieval failed: {ex.Message}", DiscoveryObjectType.User, ex);
                    }
                    return UserInfo;
                }
            }

            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"UserLogonName:{ UserLogonName} Error:{ex.Message}" , DiscoveryObjectType.User, ex);
                throw ex;
            }
        }

        public void ProcessUserGroups(string logonName)
        {
            var user = SAPHANAUserinfo.GetByUserLogonName(logonName, ProcessorConfig.EndpointId);

            if (user == null)
                return;

            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            dicParams.Add("USERNAME", logonName);
            var dtSAPUserGroups = ProcessorConfig.sapUtil.getTableFromRfcFunction("BAPI_USER_GET_DETAIL", dicParams, "GROUPS");
            SAPHANAUserinfo.DeleteSAPUserGroupMap(ProcessorConfig.EndpointId, user.Id);
            if (dtSAPUserGroups.Rows.Count > 0)
            {
                var listUserGroup = AddUserGroups(dtSAPUserGroups, user.Id);
                SAPHANAUserinfo.SaveSAPUserGroupMap(listUserGroup, ProcessorConfig.EndpointId);
                //this.UpdateGroupToEntitlementRecon(listUserGroup); // not adding group to entrecon
            }
        }
        public void ProcessUserProfiles(string logonName)
        {
            var user = SAPHANAUserinfo.GetByUserLogonName(logonName, ProcessorConfig.EndpointId);

            if (user == null)
                return;

            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            dicParams.Add("USERNAME", logonName);
            var dtSAPUserProfiles = ProcessorConfig.sapUtil.getTableFromRfcFunction("BAPI_USER_GET_DETAIL", dicParams, "PROFILES");

            SAPHANAUserinfo.DeleteSAPUserProfileMap(ProcessorConfig.EndpointId, user.Id);
            if (dtSAPUserProfiles.Rows.Count > 0)
            {
                var listUserProfile = AddUserProfiles(dtSAPUserProfiles, user.Id);
                SAPHANAUserinfo.SaveSAPUserProfileMap(listUserProfile, ProcessorConfig.EndpointId);
                //this.UpdateProfileToEntitlementRecon(listUserProfile); // not adding profile to entrecon
            }
        }
        public void ProcessUserRoles(string logonName)
        {
            var sAPUserinfo = SAPHANAUserinfo.GetByUserLogonName(logonName, ProcessorConfig.EndpointId);

            if (sAPUserinfo == null)
                return;

            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            dicParams.Add("USERNAME", logonName);
            var dtSAPUserRoleCodes = ProcessorConfig.sapUtil.getTableFromRfcFunction("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");

            SAPHANAUserinfo.DeleteSAPUserRoleMap(ProcessorConfig.EndpointId, sAPUserinfo.Id);
            if (dtSAPUserRoleCodes.Rows.Count > 0)
            {
                DataRow[] dtRolesDirectlyAttached = dtSAPUserRoleCodes.Select("Org_Flag <> 'C' AND Org_Flag <> 'X'");

                var listUserRole = AddUserRoles(dtSAPUserRoleCodes, dtRolesDirectlyAttached, sAPUserinfo.Id);

                SAPHANAUserinfo.SaveSAPUserRoleMap(listUserRole, ProcessorConfig.EndpointId);
                dtRolesDirectlyAttached = null;
                //this.UpdateRoleToEntitlementRecon(listUserRole, sAPUserinfo.Id);
            }
            dicParams?.Clear();
            dicParams = null;
        }

        public void ProcessUserRoles(List<string> lstLogonNames)
        {
            
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"ProcessUserRoles - Started, lstLogonNames:{lstLogonNames.Count}", DiscoveryObjectType.User);
            if (lstLogonNames.Count == 0)
                return;

            var sapUsers = new List<SAPHANAUserinfo>();
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            DataRow[] dtRolesDirectlyAttached = null;
            DataTable dtSAPUserRoleCodes = null;
            try
            {
                int reTry = 0;
                bool isException = false;
                sapUsers = SAPHANAUserinfo.GetByUsersLogonNames(lstLogonNames, ProcessorConfig.EndpointId);
                DILog.DiscoveryLogger.WriteLog(LogLevelType.DebugStep, $"ProcessUserRoles - users:{sapUsers.Count}", DiscoveryObjectType.User);
                foreach (var sapUser in sapUsers)
                {
                    reTry = 2;
                    isException = true;
                    while (reTry > 0 && isException)
                    {
                        isException = false;
                        try
                        {
                            if (sapUser != null)
                            {

                                dicParams = new Dictionary<string, object>();
                                dicParams.Add("USERNAME", sapUser.UserLogonName);
                                dtSAPUserRoleCodes = ProcessorConfig.sapUtil.getTableFromRfcFunction("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");
                                SAPHANAUserinfo.DeleteSAPUserRoleMap(ProcessorConfig.EndpointId, sapUser.Id);
                                if (dtSAPUserRoleCodes != null && dtSAPUserRoleCodes.Rows.Count > 0)
                                {
                                    dtRolesDirectlyAttached = dtSAPUserRoleCodes.Select("Org_Flag <> 'C' AND Org_Flag <> 'X'");
                                    var listUserRole = AddUserRoles(dtSAPUserRoleCodes, dtRolesDirectlyAttached, sapUser.Id);
                                    //SAPUserinfo.DeleteSAPUserRoleMap(ProcessorConfig.EndpointId, sapUser.Id);
                                    SAPHANAUserinfo.SaveSAPUserRoleMap(listUserRole, ProcessorConfig.EndpointId);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"User Role:{sapUser.UserLogonName} failed. {ex.Message}", DiscoveryObjectType.User, ex);
                            isException = true;
                        }
                        finally
                        {
                            reTry = reTry - 1;
                        }
                    }
                }
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, $"ProcessUserRoles - Completed", DiscoveryObjectType.User);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                dicParams?.Clear();
                dicParams = null;

                sapUsers?.Clear();
                sapUsers = null;

                dtSAPUserRoleCodes?.Dispose();
                dtSAPUserRoleCodes = null;

            }
        }
        private List<SAPHANAUserinfo.SAPHANAUserRole> AddUserRoles(DataTable dtSAPUserRoles, DataRow[] dtRolesDirectlyAttached, long sapUserId)
        {
            var listUserRole = new List<SAPHANAUserinfo.SAPHANAUserRole>();

            SAPHANAUserinfo.SAPHANAUserRole objSAPUserRole = null;
            SAPHANAUserinfo.SAPHANAUserRole objUserRole = null;
            long roleId = 0;
            DateTime fromdate;
            DateTime todate;

            for (int role = 0; role < dtSAPUserRoles.Rows.Count; role++)
            {
                objSAPUserRole = new SAPHANAUserinfo.SAPHANAUserRole();
                //objRole = SAPRoleInfo.GetByRoleCode(dtSAPUserRoles.Rows[role]["Agr_Name"].ToString().Trim(), ProcessorConfig.EndpointId);
                RolesMaster.TryGetValue(Convert.ToString(dtSAPUserRoles.Rows[role]["Agr_Name"]).Trim(), out roleId);
                bool isDirectlyAttached = dtSAPUserRoles.Rows[role]["ORG_FLAG"].ToString() != "C" && dtSAPUserRoles.Rows[role]["ORG_FLAG"].ToString() != "X";
                if (roleId > 0)
                {
                    objUserRole = listUserRole.FirstOrDefault(userrole => userrole.SAPHANARoleInfo_Id == roleId && userrole.SAPHANAUserinfo_Id == sapUserId && userrole.IsDirectlyAttached == isDirectlyAttached);
                    if (objUserRole == null)
                    {
                        objSAPUserRole.SAPHANARoleInfo_Id = roleId;
                        objSAPUserRole.SAPHANAUserinfo_Id = sapUserId;
                        objSAPUserRole.EndpointSystemId = ProcessorConfig.EndpointId;
                        objSAPUserRole.Version = Convert.ToInt16(VersionNo.Available);
                        objSAPUserRole.IsDirectlyAttached = false;
                        fromdate = DateTime.ParseExact(dtSAPUserRoles.Rows[role]["from_dat"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                        todate = DateTime.ParseExact(dtSAPUserRoles.Rows[role]["to_dat"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                        objSAPUserRole.Role_FromDate = fromdate.ToString(ProvisioningConstants.SAPDateFormater);

                        objSAPUserRole.Role_ToDate = todate.ToString(ProvisioningConstants.SAPDateFormater);
                        objSAPUserRole.IsDirectlyAttached = isDirectlyAttached;
                        //if (dtRolesDirectlyAttached.FirstOrDefault(directrole => directrole["Agr_Name"].ToString().Equals(dtSAPUserRoles.Rows[role]["Agr_Name"].ToString().Trim(), StringComparison.CurrentCultureIgnoreCase)) != null)
                        //{
                        //    objSAPUserRole.IsDirectlyAttached = true;
                        //}
                        listUserRole.Add(objSAPUserRole);
                    }
                }
            }
            return listUserRole;
        }
        private List<SAPHANAUserinfo.SAPHANAUserProfile> AddUserProfiles(DataTable dtSAPUserProfiles, long userId)
        {
            var listUserProfile = new List<SAPHANAUserinfo.SAPHANAUserProfile>();
            SAPHANAUserinfo.SAPHANAUserProfile objUserProfile = new SAPHANAUserinfo.SAPHANAUserProfile();
            var objProfile = new SAPHANAProfileInfo();
            SAPHANAUserinfo.SAPHANAUserProfile objSAPUserProfile = new SAPHANAUserinfo.SAPHANAUserProfile();
            for (int Profile = 0; Profile < dtSAPUserProfiles.Rows.Count; Profile++)
            {
                objSAPUserProfile = new SAPHANAUserinfo.SAPHANAUserProfile();
                objProfile = new SAPHANAProfileInfo();
                objProfile = SAPHANAProfileInfo.GetByProfileCode(dtSAPUserProfiles.Rows[Profile]["Bapiprof"].ToString(), ProcessorConfig.EndpointId);
                if (objProfile != null)
                {
                    objUserProfile = listUserProfile.FirstOrDefault(profile => profile.SAPHANAProfileInfo_Id == objProfile.Id && profile.SAPHANAUserinfo_Id == userId);
                    if (objUserProfile == null)
                    {
                        objSAPUserProfile.SAPHANAProfileInfo_Id = objProfile.Id;
                        objSAPUserProfile.SAPHANAUserinfo_Id = userId;
                        objSAPUserProfile.EndpointSystemId = ProcessorConfig.EndpointId;
                        objSAPUserProfile.Version = Convert.ToInt16(VersionNo.Available);
                        listUserProfile.Add(objSAPUserProfile);
                    }
                }
            }

            return listUserProfile;
        }
        private List<SAPHANAUserinfo.SAPHANAUserGroup> AddUserGroups(DataTable dtSAPUserGroups, long userId)
        {
            var listUserGroup = new List<SAPHANAUserinfo.SAPHANAUserGroup>();
            SAPHANAUserinfo.SAPHANAUserGroup objUserGroup = null;
            SAPHANAUserinfo.SAPHANAUserGroup objSAPUserGroup = new SAPHANAUserinfo.SAPHANAUserGroup();
            SAPHANAGroupInfo objgrp = new SAPHANAGroupInfo();
            for (int group = 0; group < dtSAPUserGroups.Rows.Count; group++)
            {
                objSAPUserGroup = new SAPHANAUserinfo.SAPHANAUserGroup();
                objgrp = new SAPHANAGroupInfo();
                objgrp = SAPHANAGroupInfo.GetByEndPointSystemId(ProcessorConfig.EndpointId, dtSAPUserGroups.Rows[group]["UserGroup"].ToString());
                if (objgrp != null)
                {
                    objUserGroup = listUserGroup.FirstOrDefault(usergroup => usergroup.SAPHANAGroupInfo_Id == objgrp.Id && usergroup.SAPHANAUserinfo_Id == userId);
                    if (objUserGroup == null)
                    {
                        objSAPUserGroup.SAPHANAGroupInfo_Id = objgrp.Id;
                        objSAPUserGroup.SAPHANAUserinfo_Id = userId;
                        objSAPUserGroup.EndpointSystemId = ProcessorConfig.EndpointId;
                        objSAPUserGroup.Version = Convert.ToInt16(VersionNo.Available);
                        listUserGroup.Add(objSAPUserGroup);
                    }
                }
            }

            return listUserGroup;
        }

        #region ForTool
        public List<SAPHANAUserinfo> GetUsers(string filter = null)
        {
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started GetUsers.", DiscoveryObjectType.User);

            List<SAPHANAUserinfo> lstSAPUserinfo = new List<SAPHANAUserinfo>();
            DataTable dtUsers = null;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                try
                {
                    var fields = new List<string> { "BNAME" };
                    dtUsers = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USR01", null, fields, filter, ProcessorConfig.sapUtil.IsPaginationEnabled);
                    foreach (DataRow row in dtUsers.Rows)
                    {
                        string UserLogonName = row["BNAME"].ToString().Trim();
                        var sapUser = ConstructUserInfo(UserLogonName);
                        lstSAPUserinfo.Add(sapUser);
                    }
                    return lstSAPUserinfo;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                var rowCount = ProcessorConfig.sapUtil.RowCount;
                ProcessorConfig.sapUtil.RowCount = 1000;
                Dictionary<string, object> dicParams = new Dictionary<string, object>();

                ProcessorConfig.sapUtil.ResetPaging();
                long count = 0;
                try
                {

                    do
                    {
                        var fields = new List<string> { "BNAME" };
                        dtUsers = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USR01", null, fields, null, ProcessorConfig.sapUtil.IsPaginationEnabled);

                        foreach (DataRow row in dtUsers.Rows)
                        {
                            string UserLogonName = row["BNAME"].ToString().Trim();

                            var sapUser = ConstructUserInfo(UserLogonName);

                            if (sapUser != null)
                            {
                                lstSAPUserinfo.Add(sapUser);
                                count++;
                            }

                        }
                        if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                             ProcessorConfig.sapUtil.NextPage();

                    } while (dtUsers.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);


                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed GetUsers. Count : " + count.ToString(), DiscoveryObjectType.User);
                }
                catch (Exception ex)
                {
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, DiscoveryObjectType.User, ex);
                    throw ex;
                }
                ProcessorConfig.sapUtil.RowCount = rowCount;
                return lstSAPUserinfo;
            }
        }

        public string CheckUserExists(string username)
        {
            string message = string.Empty;
            try
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", username);
                IRfcFunction fnUserExists = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_EXISTENCE_CHECK", dicParams);

                var result = ProcessorConfig.sapUtil.ExecuteStructure(fnUserExists, "RETURN");
                message = result.GetValue("MESSAGE").ToString().ToUpper();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public DataTable GetBAPI_USER_GETLIST(string functionName, string tableName, int index)
        {
            if (string.IsNullOrWhiteSpace(functionName))
                functionName = "BAPI_USER_GETLIST";
            return ProcessorConfig.sapUtil.ExecuteTable(functionName, new Dictionary<string, object>(), index);
        }

        public DataTable ReadSAPData(string tableName, List<string> fields, string filter)
        {
            return ProcessorConfig.sapUtil.ReadRFCTblAsFunction(tableName, null, fields, filter, ProcessorConfig.sapUtil.IsPaginationEnabled);
        }
        #endregion
    }
}
