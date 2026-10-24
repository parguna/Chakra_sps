
#region NameSpace

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.OracleClient;
using DI.Chakra.Infrastructure;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.DBConfigHelper;

#endregion

namespace DI.Chakra.Connector
{
    public class OracleEBSConnector : ConnectorBase
    {
        #region Members
        
        public EndPoint endpoint;
        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicRoles = new Dictionary<long, string>();
        private Dictionary<long, string> dicResponsibilities = new Dictionary<long, string>();
        private Dictionary<string, ArrayList> dicRoleUsers = new Dictionary<string, ArrayList>();
        private Dictionary<string, ArrayList> dicResponsibilitiesUsers = new Dictionary<string, ArrayList>();
        List<EmployeeEventTrigger> triggers = new List<EmployeeEventTrigger>();

        private EndpointConfiguration oracleEBSConfig = null;
        public EndpointConfiguration OracleEBSConfiguration
        {
            get { return oracleEBSConfig; }
            set { this.oracleEBSConfig = value; }
        }
        #endregion

        #region Core
        public OracleEBSConnector(EndPoint endpoint, AccessRequest currentRequest)
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started OracleEBSConnector Configuration"));
                this.EndPoint = endpoint;
                this.CurrentRequest = currentRequest;
                var oracleEBSConfig = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
                oracleEBSConfig.ConfigurationDetails.ForEach(lq =>
                {
                    if (lq.KeyId == Convert.ToInt64(EndpointKey.Password))
                    {
                        lq.Value = RSAHelper.DecryptData(lq.Value);
                    }
                });
                this.OracleEBSConfiguration = oracleEBSConfig;
                WriteDiscoveryLog(LogLevelType.Info, string.Format("OracleEBSConnector Configuration Established SuccessFully."));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Error occured while Establishing the Configuration (OracleEBSConnector Method)."));
            }
        }
        public override IMProcessor IMProcessor
        {
            get
            {
                OracleEBS.IMProcessor processor = new OracleEBS.IMProcessor();
                processor.endpoint = this.EndPoint;
                processor.OracleEBSConfiguration = this.OracleEBSConfiguration;
                return processor;
            }


        }

        #endregion

        #region Discovery
        public override bool Connect()
        {
            OracleConnection OrCon = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Connection Establish"));
                string OEBSConStr = string.Empty;
                string portNo = Configuration(EndpointKey.PortNo);
                string InstanceName = Configuration(EndpointKey.InstanceName);
                string Password = Configuration(EndpointKey.Password);
                OEBSConStr = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", Configuration(EndpointKey.IPAddress), portNo, InstanceName, Configuration(EndpointKey.UserName), Password);
                OrCon = new OracleConnection(OEBSConStr);
                OrCon.Open();
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Connection Established SuccessFully ", Configuration(EndpointKey.IPAddress)));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while Connecting the Connection {0} (Connect Method) ", Configuration(EndpointKey.IPAddress)));
                return false;
            }
            finally
            {
                OrCon.Close();
                OrCon.Dispose();
            }
            return true;
        }
        private string Configuration(EndpointKey Key)
        {
            var config = OracleEBSConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public override void Disconnect()
        {
            throw new NotImplementedException();
        }
        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            bool ReturnValue = false;
            try
            {
                string CreateLoginQuery = @"BEGIN
                                                    FND_USER_PKG.UPDATEUSER(x_user_name            => :P_UserName
                                                                           ,x_owner                => ''
                                                                           ,x_unencrypted_password => :P_Password
                                                                           ,x_password_date        => SYSDATE + 500);

                                                    END;";
                putOracleQueryToDB(CreateLoginQuery,
                    new OracleParameter("P_UserName", UserName),
                    new OracleParameter("P_Password", Password));
                ReturnValue = true;
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, string.Format("Password changed for user {0} ", UserName));
            }

            catch (Exception ex)
            {

                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, string.Format("Change password failed for user {0} ", UserName), ex);
                return false;
            }
            return ReturnValue;
        }
        public override void GetData()
        {
            try
            {
                long DiscoveryProcessId = base.DiscoveryProcess.Id;
                List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);
                bool IsHRMSUsersSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_OEBS_HRMSUsers) != null;
                bool IsMenuSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_OEBS_Menus) != null;
                bool IsSubmenuSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_OEBS_Submenus) != null;
                bool IsRolesSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_OEBS_Roles) != null;
                bool IsResponsibilitiesSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_OEBS_Responsibilities) != null;
                bool IsUsersSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_OEBS_Users) != null;


                if (IsHRMSUsersSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - HRMSUsers discovery started ", Configuration(EndpointKey.HostName)));

                    IList<OracleEBSHRMSUserInfo> listOracleEBSHRMSUsers_Existing = OracleEBSHRMSUserInfo.GetAll();
                    IList<OracleEBSHRMSUserInfo> HRMSusers = getHRMSUsers();
                    OracleEBSHRMSUserInfo HRMSors = null;
                    List<OracleEBSHRMSUserInfo> oracleHRMSUsersList = new List<OracleEBSHRMSUserInfo>();
                    if (HRMSusers != null)
                    {
                        foreach (OracleEBSHRMSUserInfo item in HRMSusers)
                        {
                            HRMSors = null;
                            HRMSors = listOracleEBSHRMSUsers_Existing.Where(u => u.PERSONID == item.PERSONID).FirstOrDefault();
                            if (HRMSors != null)
                            {

                                item.Id = HRMSors.Id;

                            }
                            oracleHRMSUsersList.Add(item);
                        }
                        if (oracleHRMSUsersList.Count > 0)
                        {
                            OracleEBSHRMSUserInfo.SaveList(oracleHRMSUsersList, MasterConfigHelper.ServiceUser);
                        }
                    }

                    #region "User Table"

                    IList<User> listUsers = new List<User>();
                    User usr1;
                    bool isnewuser = false;
                    if (HRMSusers != null && HRMSusers.Count > 0)
                    {
                        listUsers = User.GetAll(false);
                        IList<NOrganisation> listAllOrganisations = new List<NOrganisation>();
                        // Organisation.GetByTenantId(EndPoint.TenantId);
                        foreach (OracleEBSHRMSUserInfo trsusr in HRMSusers)
                        {
                            usr1 = listUsers.FirstOrDefault(s => s.EmployeeId == trsusr.PERSONID);
                            triggers.Clear();
                            if (usr1 == null)
                            {
                                isnewuser = true;
                                User user = new User();
                                user.IsActive = true;
                                user.UserName = trsusr.FIRST_NAME + trsusr.LAST_NAME.Replace(" ", "");
                                user.Password = "+GNcBxodDhLAhKsu0oaGDQ=="; //DI.Chakra.Infrastructure.Constants.PASSWORD; //
                                user.FirstName = trsusr.FIRST_NAME;
                                user.LastName = trsusr.LAST_NAME;
                                user.KnownAs = trsusr.FULL_NAME;// trssr.KNOWN_AS;
                                user.DisplayName = trsusr.FULL_NAME;
                                user.IdentificationNo = trsusr.NATIONAL_IDENTIFIER;
                                user.FullTimeEmployee = trsusr.PERSON_TYPE_ID == "Employee";
                                user.JoinDate = trsusr.EFFECTIVE_START_DATE;
                                user.TerminationDate = trsusr.EFFECTIVE_END_DATE;
                                user.EmployeeId = trsusr.PERSONID;

                                user.EmailId = "devteam@deepidentity.com";// trsusr.EMAIL_ADDRESS;
                                user.EmployeeType = EmployeeType.Permanent;
                                user.TelephoneNumber = trsusr.WORK_TELEPHONE;
                                user.Country = "Singapore";
                                int orgUnitPrefix = (int)trsusr.NOrganisationId;
                                if (orgUnitPrefix != 0)
                                {
                                    user.NOrganisationId = orgUnitPrefix;
                                }
                                else
                                {
                                    user.NOrganisationId = 0;
                                }

                                user.Save(MasterConfigHelper.ServiceUser);
                                int GroupID = DBConfigHelper.MasterConfigHelper.RoleUser;
                                User.InsertUserRole(user.Id, GroupID);
                                WriteDiscoveryLog(LogLevelType.Info, string.Format(" New User {0} found in Oracle EBS HRMS", user.UserName));
                                TrackEventTriggers(user, isnewuser);

                            }
                            else
                            {
                                usr1 = User.GetByEmployeeId(usr1.EmployeeId);
                                usr1.FirstName = trsusr.FIRST_NAME;
                                usr1.LastName = trsusr.LAST_NAME;
                                usr1.KnownAs = trsusr.FULL_NAME;// trsusr.KNOWN_AS;
                                usr1.DisplayName = trsusr.FULL_NAME;
                                usr1.IdentificationNo = trsusr.NATIONAL_IDENTIFIER;
                                usr1.FullTimeEmployee = trsusr.PERSON_TYPE_ID == "Employee";
                                usr1.NBusinessRoleId = 0;
                                usr1.JoinDate = trsusr.EFFECTIVE_START_DATE;
                                usr1.EmployeeId = trsusr.PERSONID;


                                usr1.EmailId = "devteam@deepidentity.com";// trsusr.EMAIL_ADDRESS;
                                                                          // usr1.DivisionCode = "50000000";//trsusr.DivisionCode;
                                                                          // usr1.DivisionName = "Power Admin"; //trsusr.DivisionName;
                                                                          //usr1.SectionCode = "50000701";// trsusr.SectionCode;
                                                                          //  usr1.SectionName = "Section A"; //trsusr.SectionName;
                                                                          //usr1.BranchCode = trsusr.BranchCode;
                                                                          //usr1.BranchName = trsusr.BranchName;

                                usr1.Reserved1 = "";
                                usr1.TelephoneNumber = trsusr.WORK_TELEPHONE;

                                usr1.Country = "Singapore";
                                if (trsusr.EFFECTIVE_END_DATE <= DateTime.Now)//(usr1.TerminationDate != trsusr.EFFECTIVE_END_DATE)
                                {
                                    usr1.TerminationDate = trsusr.EFFECTIVE_END_DATE;
                                    isnewuser = false;
                                    TrackEventTriggers(usr1, isnewuser);
                                }
                                else
                                {
                                    usr1.TerminationDate = trsusr.EFFECTIVE_END_DATE;
                                }
                                long OrganisationId = (long)trsusr.NOrganisationId;
                                NOrganisation org = listAllOrganisations.FirstOrDefault(c => c.Id == OrganisationId);
                                if (org != null)
                                {
                                    if (usr1.NOrganisationId != org.Id)
                                    {
                                        CreateTransferTriggers(usr1.Id, org.Id, DateTime.Now, (long)usr1.NOrganisationId);
                                    }
                                    usr1.NOrganisationId = (int)org.Id;// (int)OrganisationId;
                                }
                                usr1.Save(MasterConfigHelper.ServiceUser);
                            }
                            EmployeeEventTrigger.ValidateAndCreateTriggers(triggers, MasterConfigHelper.ServiceUser);
                        }
                    }

                    #endregion                    

                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - HRMSUsers discovery completed ", Configuration(EndpointKey.HostName)));
                }

                if (IsMenuSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Menu discovery started ", Configuration(EndpointKey.HostName)));
                    DiscoverMenus();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Menu discovery completed ", Configuration(EndpointKey.HostName)));
                }

                if (IsSubmenuSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - SubMenus discovery started ", Configuration(EndpointKey.HostName)));
                    DiscoverSubMenus();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - SubMenus discovery completed ", Configuration(EndpointKey.HostName)));
                }

                if (IsRolesSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Roles discovery started ", Configuration(EndpointKey.HostName)));
                    DiscoverRoles();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Roles discovery completed ", Configuration(EndpointKey.HostName)));
                }

                if (IsResponsibilitiesSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Responsibilities discovery started ", Configuration(EndpointKey.HostName)));
                    DiscoverResponsibilities();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Responsibilities discovery completed ", Configuration(EndpointKey.HostName)));
                }

                if (IsUsersSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - users discovery started ", Configuration(EndpointKey.HostName)));
                    DiscoverUsers();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - users discovery completed ", Configuration(EndpointKey.HostName)));
                }

                if (IsRolesSelected || IsUsersSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - user-Roles discovery started ", Configuration(EndpointKey.HostName)));
                    DiscoverUsersRoles();
                    OracleEBSUserInfo.SaveUserRoleMap(dicUsers, dicRoles, dicRoleUsers, this.EndPoint.Id);
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - user-Roles discovery completed ", Configuration(EndpointKey.HostName)));
                }

                if (IsResponsibilitiesSelected || IsUsersSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - user-Responsibilites discovery started ", Configuration(EndpointKey.HostName)));
                    DiscoverUsersResponsibilities();
                    OracleEBSUserInfo.SaveUserResponsibilityMap(dicUsers, dicResponsibilities, dicResponsibilitiesUsers, this.EndPoint.Id);
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - user-Responsibilites discovery completed ", Configuration(EndpointKey.HostName)));
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void CreateTransferTriggers(long userId, long newOrgId, DateTime effectiveDate, long oldOrgId)
        {
            var evnt = EmployeeEvents.OUChange;
            var oldUser = User.GetById(userId);
            if (oldUser == null)
                return;
            var oldOrg = NOrganisation.GetById(oldOrgId);
            oldUser.NOrganisation = oldOrg;
            var oldUserDetail = UtilityMethods.Serialize<User>(oldUser);
            var newUser = User.GetById(userId);
            var newOrg = NOrganisation.GetById(newOrgId);
            newUser.NOrganisation = newOrg;
            var newUserDetail = UtilityMethods.Serialize<User>(newUser);
            if (newUser == null)
                return;
            var trigger = EmployeeEventTrigger.GetByUserAndEvent(newUser.Id, evnt, null);
            if (trigger != null)
            {
                var tempTrigger = EmployeeEventTrigger.GetByUserAndEvent(newUser.Id, EmployeeEvents.OUChange, DateTime.Now.AddMinutes(-10));
                if (tempTrigger != null && trigger.IsRequestCreated)
                {
                    return;
                }
                tempTrigger.EffectiveDate = effectiveDate;
                tempTrigger.OldUserDetails = oldUserDetail;
                tempTrigger.UserDetails = newUserDetail;
                tempTrigger.Save(MasterConfigHelper.ServiceUser);
            }
            else
            {

                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = newUser.Id,
                    IsActive = true,
                    EffectiveDate = effectiveDate,
                    UserDetails = newUserDetail,
                    OldUserDetails = oldUserDetail

                };


                newTrigger.Save(MasterConfigHelper.ServiceUser);

            }
        }

        #endregion

        #region Methods

        private DataSet getDSFromOracleQuery(string strQry)
        {
            OracleConnection OrCon = null;
            OracleCommand OrCom = null;
            OracleDataAdapter OrAdapter;

            DataSet dsResults = new DataSet();
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started getDSFromOracleQuery Method"));
                string oracleConnectionString = string.Empty;
                string portNo = Configuration(EndpointKey.PortNo);
                string InstanceName = Configuration(EndpointKey.InstanceName);
                string Password = Configuration(EndpointKey.Password);
                oracleConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", Configuration(EndpointKey.IPAddress), portNo, InstanceName, Configuration(EndpointKey.UserName), Password);
                OrCon = new OracleConnection(oracleConnectionString);
                OrCon.Open();
                OrCom = new OracleCommand();
                OrCom.Connection = OrCon;
                OrCom.CommandType = CommandType.Text;
                OrCom.CommandText = strQry;
                OrAdapter = new OracleDataAdapter(OrCom.CommandText, OrCom.Connection);
                OrAdapter.Fill(dsResults);
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed getDSFromOracleQuery Method"));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Error occured while Establishing the Connection (getDSFromOracleQuery Method)."));
                if (OrCon.State == ConnectionState.Open)
                {
                    OrCon.Close();
                    OrCon.Dispose();
                }
            }
            return dsResults;
        }

        private bool putOracleQueryToDB(string strQry)
        {
            return putOracleQueryToDB(strQry, null);
        }

        private bool putOracleQueryToDB(string strQry, params OracleParameter[] queryParameters)
        {
            bool ReturnValue = false;
            OracleConnection OrCon = null;
            OracleCommand OrCom = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started putOracleQueryToDB Mehtod"));
                string oracleConnectionString = string.Empty;
                string portNo = Configuration(EndpointKey.PortNo);
                string InstanceName = Configuration(EndpointKey.InstanceName);
                string Password = Configuration(EndpointKey.Password);
                oracleConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", Configuration(EndpointKey.IPAddress), portNo, InstanceName, Configuration(EndpointKey.UserName), Password);
                OrCon = new OracleConnection(oracleConnectionString);
                OrCon.Open();
                OrCom = new OracleCommand();
                OrCom.Connection = OrCon;
                OrCom.CommandType = CommandType.Text;
                OrCom.CommandText = strQry;
                if (queryParameters != null && queryParameters.Length > 0)
                {
                    OrCom.Parameters.AddRange(queryParameters);
                }
                int Result = OrCom.ExecuteNonQuery();
                ReturnValue = true;
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed putOracleQueryToDB Mehtod"));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Error occured while Establishing the Connection (putOracleQueryToDB Method)."));
                if (OrCon.State == ConnectionState.Open)
                {
                    OrCon.Close();
                    OrCon.Dispose();
                }
            }
            finally
            {
                if (OrCon.State == ConnectionState.Open)
                {
                    OrCon.Close();
                    OrCon.Dispose();
                }
            }
            return ReturnValue;
        }

        private IList<OracleEBSHRMSUserInfo> getHRMSUsers()
        {
            OracleEBSHRMSUserInfo OEBSHRMSUser = null;
            bool deltadiscover = base.DiscoveryProcess.DisableDeltaDiscovery;

            IList<OracleEBSHRMSUserInfo> listOEBSHRMSUsers = new List<OracleEBSHRMSUserInfo>();
            try
            {

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started GetHRMSUsers Mehtod"));

                string HRMSUserSQL = @" select
                        papf.person_id PersonID, papf.Last_Name LastName, papf.First_Name FirstName, papf.Title Title, hl_sex.meaning SEX, 
                        pptt.user_person_type PersonType, papf.national_identifier SSN, papf.date_of_birth DOB, hl_nat.meaning Nationality,
                        hl_mar.meaning MaritalStatus, papf.Effective_Start_Date EffStartDate,
                        papf.Effective_End_Date EffEndDate, papf.Email_address Email, papf.original_date_of_hire DateFirstHired,
                        papf.BUSINESS_GROUP_ID BGI, papf.START_DATE StartDate, papf.FULL_NAME FULL_NAME, papf.KNOWN_AS KNOWN_AS, papf.WORK_TELEPHONE TELEPHONE,
                        haou.ORGANIZATION_ID OrganisationId,haou.NAME OrganisationName
                    FROM
                        per_all_people_f papf,
                        hr_lookups hl_sex,
                        per_person_types_tl pptt,
                        hr_lookups hl_nat,
                        hr_lookups hl_mar,
                        hr_lookups hl_eth,
                        per_all_assignments_f  paaf,
                        hr_all_organization_units haou
                    WHERE 1=1
                        AND hl_sex.lookup_code(+) = papf.sex
                        AND hl_sex.lookup_type(+) = 'SEX'
                        AND hl_nat.lookup_code(+) = papf.nationality
                        AND hl_nat.lookup_type(+) = 'NATIONALITY'
                        AND hl_mar.lookup_code(+) = papf.marital_status
                        AND hl_mar.lookup_type(+) = 'MAR_STATUS'
                        AND hl_eth.lookup_code(+) = papf.per_information1
                        AND hl_eth.lookup_type(+) = 'US_ETHNIC_GROUP'
                        AND papf.person_type_id = pptt.person_type_id
                        AND pptt.user_person_type = 'Employee'
                        AND papf.PERSON_ID = paaf.PERSON_ID
                        AND paaf.ORGANIZATION_ID = haou.ORGANIZATION_ID
                        AND TRUNC (SYSDATE) BETWEEN papf.effective_start_date AND papf.effective_end_date
                        AND TRUNC (SYSDATE) BETWEEN paaf.effective_start_date AND paaf.effective_end_date 
                        AND papf.current_employee_flag= 'Y' AND papf.effective_start_date > TRUNC(SYSDATE - 10)";
                //
                if (deltadiscover == true)
                {
                    HRMSUserSQL = HRMSUserSQL + " AND papf.effective_start_date > TRUNC(SYSDATE - 10)";
                }


                DataSet dsHRMSUserinfo = getDSFromOracleQuery(HRMSUserSQL);

                if (dsHRMSUserinfo.Tables.Count <= 0)
                    return null;

                foreach (DataRow item in dsHRMSUserinfo.Tables[0].Rows)
                {
                    OEBSHRMSUser = new OracleEBSHRMSUserInfo();
                    OEBSHRMSUser.EndPointSystemId = EndPoint.Id;
                    OEBSHRMSUser.UserName = "";
                    OEBSHRMSUser.PERSONID = CheckNull.AgainstString(item["PersonID"]);
                    OEBSHRMSUser.EFFECTIVE_START_DATE = CheckNull.AgainstNullableDatetime(item["EffStartDate"]);
                    OEBSHRMSUser.EFFECTIVE_END_DATE = CheckNull.AgainstNullableDatetime(item["EffEndDate"]);
                    OEBSHRMSUser.BUSINESS_GROUP_ID = CheckNull.AgainstString(item["BGI"]);
                    OEBSHRMSUser.PERSON_TYPE_ID = CheckNull.AgainstString(item["PersonType"]);
                    OEBSHRMSUser.START_DATE = CheckNull.AgainstNullableDatetime(item["StartDate"]);
                    OEBSHRMSUser.DATE_OF_BIRTH = CheckNull.AgainstNullableDatetime(item["DOB"]);
                    OEBSHRMSUser.EMAIL_ADDRESS = CheckNull.AgainstString(item["Email"]);
                    OEBSHRMSUser.FIRST_NAME = CheckNull.AgainstString(item["FirstName"]);
                    OEBSHRMSUser.LAST_NAME = CheckNull.AgainstString(item["LastName"]);
                    OEBSHRMSUser.FULL_NAME = CheckNull.AgainstString(item["FULL_NAME"]).Replace(",", "");//CheckNull.AgainstString(item["FirstName"]) + " " + CheckNull.AgainstString(item["LastName"]);
                    OEBSHRMSUser.KNOWN_AS = CheckNull.AgainstString(item["KNOWN_AS"]);
                    OEBSHRMSUser.WORK_TELEPHONE = CheckNull.AgainstString(item["TELEPHONE"]);
                    OEBSHRMSUser.MARTIAL_STATUS = CheckNull.AgainstString(item["MaritalStatus"]);
                    OEBSHRMSUser.NATIONALITY = CheckNull.AgainstString(item["Nationality"]);
                    OEBSHRMSUser.NATIONAL_IDENTIFIER = CheckNull.AgainstString(item["SSN"]);
                    OEBSHRMSUser.PREVIOUS_LAST_NAME = "";
                    OEBSHRMSUser.SEX = CheckNull.AgainstString(item["SEX"]);
                    OEBSHRMSUser.TITLE = CheckNull.AgainstString(item["Title"]);
                    OEBSHRMSUser.ORGINAL_DATE_OF_HIRE = CheckNull.AgainstNullableDatetime(item["DateFirstHired"]);
                    string orgUnitPrefix = CheckNull.AgainstString(item["OrganisationId"]);
                    if (orgUnitPrefix != null && orgUnitPrefix != "")
                    {
                        OEBSHRMSUser.NOrganisationId = NOrganisation.GetByOrgCode(orgUnitPrefix).Id;
                    }
                    else
                    { OEBSHRMSUser.NOrganisationId = 0; }

                    listOEBSHRMSUsers.Add(OEBSHRMSUser);
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed GetHRMSUsers Mehtod"));
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Error occured while get HRMSUser (getHRMSUsers Method)."));
                throw;
            }

            return listOEBSHRMSUsers;
        }

        private IList<OracleEBSUserInfo> GetUsers()
        {
            OracleEBSUserInfo OEBSUser = null;
            IList<OracleEBSUserInfo> listOEBSUsers = new List<OracleEBSUserInfo>();
            try
            {
                string UserSQL = @"select USER_ID, USER_NAME, LAST_UPDATE_DATE, LAST_UPDATED_BY, CREATION_DATE, CREATED_BY,
                                    SESSION_NUMBER, START_DATE, END_DATE, DESCRIPTION, LAST_LOGON_DATE, PASSWORD_DATE, 
                                    PASSWORD_ACCESSES_LEFT, PASSWORD_LIFESPAN_ACCESSES, PASSWORD_LIFESPAN_DAYS, EMPLOYEE_ID,
                                    EMAIL_ADDRESS, FAX, CUSTOMER_ID, SUPPLIER_ID, SECURITY_GROUP_ID, USER_GUID, GCN_CODE_COMBINATION_ID, PERSON_PARTY_ID
                                from fnd_user";
                DataSet dsUserinfo = getDSFromOracleQuery(UserSQL);
                if (dsUserinfo.Tables.Count <= 0)
                {
                    return null;
                }
                foreach (DataRow item in dsUserinfo.Tables[0].Rows)
                {
                    OEBSUser = new OracleEBSUserInfo();

                    OEBSUser.EndPointSystemId = EndPoint.Id;
                    OEBSUser.USER_ID = CheckNull.AgainstString(item["USER_ID"]);
                    OEBSUser.USER_NAME = CheckNull.AgainstString(item["USER_NAME"]);
                    OEBSUser.LAST_UPDATE_DATE = CheckNull.AgainstNullableDatetime(item["LAST_UPDATE_DATE"]);
                    OEBSUser.LAST_UPDATED_BY = CheckNull.AgainstString(item["LAST_UPDATED_BY"]);
                    OEBSUser.CREATION_DATE = CheckNull.AgainstNullableDatetime(item["CREATION_DATE"]);
                    OEBSUser.CREATED_BY = CheckNull.AgainstString(item["CREATED_BY"]);
                    OEBSUser.SESSION_NUMBER = CheckNull.AgainstNullableDouble(item["SESSION_NUMBER"]);
                    OEBSUser.START_DATE = CheckNull.AgainstNullableDatetime(item["START_DATE"]);
                    OEBSUser.END_DATE = CheckNull.AgainstNullableDatetime(item["END_DATE"]);
                    OEBSUser.DESCRIPTION = CheckNull.AgainstString(item["DESCRIPTION"]);
                    OEBSUser.LAST_LOGON_DATE = CheckNull.AgainstNullableDatetime(item["LAST_LOGON_DATE"]);
                    OEBSUser.PASSWORD_DATE = CheckNull.AgainstNullableDatetime(item["PASSWORD_DATE"]);
                    OEBSUser.PASSWORD_ACCESSES_LEFT = CheckNull.AgainstNullableDouble(item["PASSWORD_ACCESSES_LEFT"]);
                    OEBSUser.PASSWORD_LIFESPAN_ACCESSES = CheckNull.AgainstNullableInt(item["PASSWORD_LIFESPAN_ACCESSES"]);
                    OEBSUser.PASSWORD_LIFESPAN_DAYS = CheckNull.AgainstNullableDouble(item["PASSWORD_LIFESPAN_DAYS"]);
                    OEBSUser.EMPLOYEE_ID = CheckNull.AgainstNullableDouble(item["EMPLOYEE_ID"]);
                    OEBSUser.EMAIL_ADDRESS = CheckNull.AgainstString(item["EMAIL_ADDRESS"]);
                    OEBSUser.FAX = CheckNull.AgainstString(item["FAX"]);
                    OEBSUser.CUSTOMER_ID = CheckNull.AgainstNullableDouble(item["CUSTOMER_ID"]);
                    OEBSUser.SUPPLIER_ID = CheckNull.AgainstNullableDouble(item["SUPPLIER_ID"]);
                    OEBSUser.SECURITY_GROUP_ID = CheckNull.AgainstNullableDouble(item["SECURITY_GROUP_ID"]);
                    OEBSUser.GCN_CODE_COMBINATION_ID = CheckNull.AgainstNullableInt(item["GCN_CODE_COMBINATION_ID"]);
                    OEBSUser.PERSON_PARTY_ID = CheckNull.AgainstNullableInt(item["PERSON_PARTY_ID"]);
                    listOEBSUsers.Add(OEBSUser);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

            }
            return listOEBSUsers;
        }

        private IList<OracleEBSRoleInfo> GetRoles()
        {
            OracleEBSRoleInfo role = null;
            IList<OracleEBSRoleInfo> listRoles = new List<OracleEBSRoleInfo>();
            try
            {
                string RolesSQL = "SELECT NAME, DISPLAY_NAME, DESCRIPTION, NOTIFICATION_PREFERENCE, LANGUAGE, TERRITORY, EMAIL_ADDRESS, FAX, " +
                            "ORIG_SYSTEM, ORIG_SYSTEM_ID, START_DATE, STATUS, EXPIRATION_DATE, SECURITY_GROUP_ID, USER_FLAG, PARTITION_ID, CREATED_BY, " +
                            "CREATION_DATE, LAST_UPDATED_BY, LAST_UPDATE_DATE, LAST_UPDATE_LOGIN, OWNER_TAG, PARENT_ORIG_SYSTEM, PARENT_ORIG_SYSTEM_ID " +
                        " FROM wf_local_roles ";
                DataSet dsResults = getDSFromOracleQuery(RolesSQL);
                if (dsResults.Tables.Count <= 0)
                {
                    return null;
                }
                foreach (DataRow item in dsResults.Tables[0].Rows)
                {
                    role = new OracleEBSRoleInfo();

                    role.EndPointSystemId = (int)EndPoint.Id;
                    role.NAME = CheckNull.AgainstString(item["NAME"]);
                    role.DISPLAY_NAME = CheckNull.AgainstString(item["DISPLAY_NAME"]);
                    role.DESCRIPTION = CheckNull.AgainstString(item["DESCRIPTION"]);
                    role.NOTIFICATION_PREFERENCE = CheckNull.AgainstString(item["NOTIFICATION_PREFERENCE"]);
                    role.LANGUAGE = CheckNull.AgainstString(item["LANGUAGE"]);
                    role.TERRITORY = CheckNull.AgainstString(item["TERRITORY"]);
                    role.EMAIL_ADDRESS = CheckNull.AgainstString(item["EMAIL_ADDRESS"]);
                    role.FAX = CheckNull.AgainstString(item["FAX"]);
                    role.ORIG_SYSTEM = CheckNull.AgainstString(item["ORIG_SYSTEM"]);
                    role.ORIG_SYSTEM_ID = CheckNull.AgainstNullableDouble(item["ORIG_SYSTEM_ID"]);
                    role.START_DATE = CheckNull.AgainstNullableDatetime(item["START_DATE"]);
                    role.STATUS = CheckNull.AgainstString(item["STATUS"]);
                    role.EXPIRATION_DATE = CheckNull.AgainstNullableDatetime(item["EXPIRATION_DATE"]);
                    role.SECURITY_GROUP_ID = CheckNull.AgainstNullableDouble(item["SECURITY_GROUP_ID"]);
                    role.USER_FLAG = CheckNull.AgainstString(item["USER_FLAG"]);
                    role.PARTITION_ID = CheckNull.AgainstNullableDouble(item["PARTITION_ID"]);
                    //role.CR = CheckNull.AgainstString(item["CREATED_BY"]);
                    //role.CREATION_DATE = CheckNull.AgainstNullableDatetime(item["CREATION_DATE"]);
                    //role.LAST_UPDATED_BY = CheckNull.AgainstString(item["LAST_UPDATED_BY"]);
                    //role.LAST_UPDATE_DATE = CheckNull.AgainstNullableDatetime(item["LAST_UPDATE_DATE"]);
                    role.LAST_UPDATE_LOGIN = CheckNull.AgainstNullableDouble(item["LAST_UPDATE_LOGIN"]);
                    role.OWNER_TAG = CheckNull.AgainstString(item["OWNER_TAG"]);
                    role.PARENT_ORIG_SYSTEM = CheckNull.AgainstString(item["PARENT_ORIG_SYSTEM"]);
                    role.PARENT_ORIG_SYSTEM_ID = CheckNull.AgainstNullableDouble(item["PARENT_ORIG_SYSTEM_ID"]);
                    listRoles.Add(role);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while processing the GetRoles Method."));
                throw;
            }

            return listRoles;
        }

        private IList<OracleEBSResponsibilitiesInfo> GetResponsibilities()
        {
            OracleEBSResponsibilitiesInfo OEBSResponsibilities = null;
            IList<OracleEBSResponsibilitiesInfo> listResponsibilities = new List<OracleEBSResponsibilitiesInfo>();
            try
            {
                DataSet dsResponsibilities = getDSFromOracleQuery(@"Select 
                                r.APPLICATION_ID APPLICATION_ID, r.RESPONSIBILITY_ID RESPONSIBILITY_ID, r.LAST_UPDATED_BY LAST_UPDATED_BY, r.LAST_UPDATE_DATE LAST_UPDATE_DATE
                                , r.CREATED_BY CREATED_BY, r.CREATION_DATE CREATION_DATE,r.DATA_GROUP_APPLICATION_ID DATA_GROUP_APPLICATION_ID, r.DATA_GROUP_ID DATA_GROUP_ID
                                , r.MENU_ID MENU_ID, r.START_DATE START_DATE, r.END_DATE END_DATE, r.GROUP_APPLICATION_ID GROUP_APPLICATION_ID 
                                , r.REQUEST_GROUP_ID REQUEST_GROUP_ID, r.VERSION OEBS_VERSION, r.WEB_HOST_NAME WEB_HOST_NAME, r.WEB_AGENT_NAME WEB_AGENT_NAME
                                , r.RESPONSIBILITY_KEY RESPONSIBILITY_KEY , r.SECURITY_GROUP_ID SECURITY_GROUP_ID
                                , rt.LANGUAGE LANGUAGE,rt.RESPONSIBILITY_NAME RESPONSIBILITY_NAME, rt.DESCRIPTION DESCRIPTION, rt.LAST_UPDATE_LOGIN LAST_UPDATE_LOGIN
                                , fa.APPLICATION_SHORT_NAME APPLICATION_SHORT_NAME
                                from fnd_responsibility r
                            JOIN fnd_responsibility_tl rt on r.responsibility_id=rt.responsibility_id 
                            Join fnd_application fa on fa.application_id=rt.application_id");
                if (dsResponsibilities.Tables.Count <= 0)
                {
                    return null;
                }
                foreach (DataRow item in dsResponsibilities.Tables[0].Rows)
                {
                    OEBSResponsibilities = new OracleEBSResponsibilitiesInfo();

                    OEBSResponsibilities.EndPointSystemId = (int)EndPoint.Id;

                    OEBSResponsibilities.APPLICATION_ID = CheckNull.AgainstDouble(item["APPLICATION_ID"]);
                    OEBSResponsibilities.RESPONSIBILITY_ID = CheckNull.AgainstDouble(item["RESPONSIBILITY_ID"]);
                    OEBSResponsibilities.RESPONSIBILITY_KEY = CheckNull.AgainstString(item["RESPONSIBILITY_KEY"]);
                    //OEBSResponsibilities.LAST_UPDATE_DATE = CheckNull.AgainstNullableDatetime(item["LAST_UPDATE_DATE"]);
                    //OEBSResponsibilities.LAST_UPDATED_BY = CheckNull.AgainstString(item["LAST_UPDATED_BY"]);
                    //OEBSResponsibilities.CREATION_DATE = CheckNull.AgainstNullableDatetime(item["CREATION_DATE"]);
                    //OEBSResponsibilities.CREATED_BY = CheckNull.AgainstString(item["CREATED_BY"]);
                    OEBSResponsibilities.LAST_UPDATE_LOGIN = CheckNull.AgainstDouble(item["LAST_UPDATE_LOGIN"]);
                    OEBSResponsibilities.DATA_GROUP_APPLICATION_ID = CheckNull.AgainstDouble(item["DATA_GROUP_APPLICATION_ID"]);
                    OEBSResponsibilities.DATA_GROUP_ID = CheckNull.AgainstDouble(item["DATA_GROUP_ID"]);
                    OEBSResponsibilities.MENU_ID = CheckNull.AgainstDouble(item["MENU_ID"]);
                    if (CheckNull.AgainstNullableDatetime(item["START_DATE"]) >= Convert.ToDateTime("1755/01/01"))
                        OEBSResponsibilities.START_DATE = CheckNull.AgainstNullableDatetime(item["START_DATE"]);
                    else
                        OEBSResponsibilities.START_DATE = Convert.ToDateTime("1800/01/01");
                    OEBSResponsibilities.END_DATE = CheckNull.AgainstNullableDatetime(item["END_DATE"]);
                    OEBSResponsibilities.GROUP_APPLICATION_ID = CheckNull.AgainstInt(item["GROUP_APPLICATION_ID"]);
                    OEBSResponsibilities.REQUEST_GROUP_ID = CheckNull.AgainstInt(item["REQUEST_GROUP_ID"]);
                    OEBSResponsibilities.OEBS_VERSION = CheckNull.AgainstString(item["OEBS_VERSION"]);
                    OEBSResponsibilities.RESPONSIBILITY_NAME = CheckNull.AgainstString(item["RESPONSIBILITY_NAME"]);
                    OEBSResponsibilities.DESCRIPTION = CheckNull.AgainstString(item["DESCRIPTION"]);
                    OEBSResponsibilities.APPLICATION_SHORT_NAME = CheckNull.AgainstString(item["APPLICATION_SHORT_NAME"]);
                    OEBSResponsibilities.SECURITY_GROUP_ID = CheckNull.AgainstInt(item["SECURITY_GROUP_ID"]);
                    OEBSResponsibilities.LANGUAGE = CheckNull.AgainstString(item["LANGUAGE"]);
                    OEBSResponsibilities.DESCRIPTION = CheckNull.AgainstString(item["DESCRIPTION"]);
                    OEBSResponsibilities.LAST_UPDATE_LOGIN = CheckNull.AgainstInt(item["LAST_UPDATE_LOGIN"]);
                    OEBSResponsibilities.WEB_HOST_NAME = CheckNull.AgainstString(item["WEB_HOST_NAME"]);
                    OEBSResponsibilities.WEB_AGENT_NAME = CheckNull.AgainstString(item["WEB_AGENT_NAME"]);
                    listResponsibilities.Add(OEBSResponsibilities);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

            }
            return listResponsibilities;
        }

        private IList<OracleEBSMenusInfo> GetMenus()
        {
            OracleEBSMenusInfo OEBSMenus = null;
            IList<OracleEBSMenusInfo> listMenus = new List<OracleEBSMenusInfo>();
            try
            {
                DataSet dsMenus = getDSFromOracleQuery(@" select * from apps.fnd_menus_tl");
                if (dsMenus.Tables.Count <= 0)
                {
                    return null;
                }
                foreach (DataRow item in dsMenus.Tables[0].Rows)
                {
                    OEBSMenus = new OracleEBSMenusInfo();
                    OEBSMenus.EndPointSystemId = (int)EndPoint.Id;
                    OEBSMenus.MENU_ID = CheckNull.AgainstDouble(item["MENU_ID"]);
                    OEBSMenus.MENU_NAME = CheckNull.AgainstString(item["USER_MENU_NAME"]);
                    OEBSMenus.LAST_UPDATE_DATE = CheckNull.AgainstNullableDatetime(item["LAST_UPDATE_DATE"]);
                    OEBSMenus.LAST_UPDATED_BY = CheckNull.AgainstString(item["LAST_UPDATED_BY"]);
                    OEBSMenus.LAST_UPDATE_LOGIN = CheckNull.AgainstDouble(item["LAST_UPDATE_LOGIN"]);
                    //OEBSMenus.CREATION_DATE = CheckNull.AgainstNullableDatetime(item["CREATION_DATE"]);
                    //OEBSMenus.CREATED_BY = CheckNull.AgainstString(item["CREATED_BY"]);
                    OEBSMenus.LANGUAGE = CheckNull.AgainstString(item["LANGUAGE"]);
                    OEBSMenus.DESCRIPTION = CheckNull.AgainstString(item["DESCRIPTION"]);
                    OEBSMenus.SOURCE_LANG = CheckNull.AgainstString(item["SOURCE_LANG"]);
                    OEBSMenus.SECURITY_GROUP_ID = CheckNull.AgainstInt(item["SECURITY_GROUP_ID"]);
                    listMenus.Add(OEBSMenus);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return listMenus;
        }

        private IList<OracleEBSSubmenusInfo> GetSubMenus()
        {
            OracleEBSSubmenusInfo OEBSSubmenus = null;
            IList<OracleEBSSubmenusInfo> listsubmenus = new List<OracleEBSSubmenusInfo>();
            try
            {
                DataSet dsMenus = getDSFromOracleQuery(@" select * from apps.fnd_menu_entries_tl");
                if (dsMenus.Tables.Count <= 0)
                    return null;
                foreach (DataRow item in dsMenus.Tables[0].Rows)
                {
                    OEBSSubmenus = new OracleEBSSubmenusInfo();
                    OEBSSubmenus.EndPointSystemId = (int)EndPoint.Id;
                    OEBSSubmenus.LANGUAGE = CheckNull.AgainstString(item["LANGUAGE"]);
                    OEBSSubmenus.MENU_ID = CheckNull.AgainstDouble(item["MENU_ID"]);
                    OEBSSubmenus.ENTRY_SEQUENCE = CheckNull.AgainstInt(item["ENTRY_SEQUENCE"]);
                    OEBSSubmenus.LAST_UPDATE_DATE = CheckNull.AgainstNullableDatetime(item["LAST_UPDATE_DATE"]);
                    OEBSSubmenus.LAST_UPDATE_BY = CheckNull.AgainstString(item["LAST_UPDATED_BY"]);
                    OEBSSubmenus.LAST_UPDATE_LOGIN = CheckNull.AgainstString(item["LAST_UPDATE_LOGIN"]);
                    OEBSSubmenus.CREATION_DATE = CheckNull.AgainstNullableDatetime(item["CREATION_DATE"]);
                    OEBSSubmenus.CREATED_BY = CheckNull.AgainstString(item["CREATED_BY"]);
                    OEBSSubmenus.PROMPT = CheckNull.AgainstString(item["PROMPT"]);
                    OEBSSubmenus.DESCRIPTION = CheckNull.AgainstString(item["DESCRIPTION"]);
                    OEBSSubmenus.SOURCE_LANG = CheckNull.AgainstString(item["SOURCE_LANG"]);
                    OEBSSubmenus.SECURITY_GROUP_ID = CheckNull.AgainstInt(item["SECURITY_GROUP_ID"]);
                    listsubmenus.Add(OEBSSubmenus);
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

            }

            return listsubmenus;
        }

        private IList<OracleEBSUserRole> GetUserRoles()
        {

            OracleEBSUserRole userrole = null;
            IList<OracleEBSUserRole> userRolesList = new List<OracleEBSUserRole>();
            try
            {
                dicRoles = OracleEBSRoleInfo.GetRoleByEndpointId(this.EndPoint.Id);
                dicUsers = OracleEBSUserInfo.GetAllByEndPointSysId(this.EndPoint.Id);
                string RolesItemQuery = @"select distinct w.Name RoleName, A.user_name UserName from APPS.WF_USER_ROLE_ASSIGNMENTS A 
                                                    join wf_local_roles w on w.orig_system_id = A.role_orig_system_id and w.Name = A.Role_Name";
                DataSet dsUserRoleResults = getDSFromOracleQuery(RolesItemQuery);
                if (dsUserRoleResults.Tables.Count <= 0)
                {
                    return null;
                }
                foreach (DataRow item in dsUserRoleResults.Tables[0].Rows)
                {
                    string roleName = CheckNull.AgainstString(item["RoleName"]);
                    string userName = CheckNull.AgainstString(item["UserName"]);
                    userrole = new OracleEBSUserRole();
                    var currentRole = dicRoles.Where(c => c.Value == roleName);
                    var currentUser = dicUsers.Where(c => c.Value == userName);
                    if (currentUser.Count() == 0 || currentRole.Count() == 0)
                    { continue; }
                    else
                    {
                        userrole.OracleEBSUserInfo_Id = currentUser.First().Key;
                        userrole.OracleEBSRoleInfo_Id = currentRole.First().Key;
                    }
                    userRolesList.Add(userrole);
                }
                FilldicRoleUsers(userRolesList);

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while processing the GetUserRoles Method."));
                throw;
            }

            return userRolesList;
        }

        private IList<OracleEBSUserResponsibility> GetUserResponsibilities()
        {
            //dicRoleUsers = new Dictionary<string, ArrayList>(); string userName = string.Empty; string roleName = string.Empty; string userNames = string.Empty;

            OracleEBSUserResponsibility userRes = null;
            IList<OracleEBSUserResponsibility> userResList = new List<OracleEBSUserResponsibility>();
            try
            {
                dicResponsibilities = OracleEBSResponsibilitiesInfo.GetResponsibilitiesByEndpointId(this.EndPoint.Id);
                dicUsers = OracleEBSUserInfo.GetAllByEndPointSysId(this.EndPoint.Id);

                string ResItemQuery = @"SELECT distinct fu.user_name UserName, frt.responsibility_name ResName 
                                                FROM fnd_user_resp_groups_direct furg, applsys.fnd_user fu,
                                                    applsys.fnd_responsibility_tl frt, applsys.fnd_responsibility fr,
                                                    applsys.fnd_application_tl fat, applsys.fnd_application fa
                                                WHERE fu.user_id = furg.user_id AND  frt.responsibility_id = furg.responsibility_id  
                                   and frt.Application_ID = furg.responsibility_application_id 
                                   AND fr.responsibility_id = furg.responsibility_id  
                                   and fr.Application_ID = furg.responsibility_application_id 
                                   AND fat.application_id = furg.responsibility_application_id 
                                   AND fa.application_id = furg.responsibility_application_id ";

                DataSet dsUserResponsibilitiesResults = getDSFromOracleQuery(ResItemQuery);
                if (dsUserResponsibilitiesResults.Tables[0].Rows.Count <= 0)
                {
                    return null;
                }
                foreach (DataRow item in dsUserResponsibilitiesResults.Tables[0].Rows)
                {
                    string resName = CheckNull.AgainstString(item["ResName"]);
                    string userName = CheckNull.AgainstString(item["UserName"]);
                    userRes = new OracleEBSUserResponsibility();

                    var currentRes = dicResponsibilities.Where(c => c.Value == resName);
                    var currentUser = dicUsers.Where(c => c.Value == userName);

                    if (currentUser.Count() == 0 || currentRes.Count() == 0)
                    { continue; }
                    else
                    {
                        userRes.OracleEBSUserInfo_Id = currentUser.First().Key;
                        userRes.OracleEBSResponsibilitiesInfo_Id = currentRes.First().Key;
                    }

                    userResList.Add(userRes);
                }
                FilldicResponsibilityUsers(userResList);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while processing the GetUserRoles Method."));
                throw;
            }
            return userResList;
        }

        private void FilldicRoleUsers(IList<OracleEBSUserRole> listUserRoles)
        {
            dicRoleUsers = new Dictionary<string, ArrayList>(); string userName = string.Empty; string roleName = string.Empty;

            foreach (var item in dicUsers)
            {
                long userId = item.Key;
                userName = item.Value;
                if (!dicRoleUsers.ContainsKey(userName))
                {
                    ArrayList roles = new ArrayList();
                    var listcurrentUser = listUserRoles.Where(c => c.OracleEBSUserInfo_Id == userId);
                    foreach (var roleitems in listcurrentUser)
                    {
                        long roleId = roleitems.OracleEBSRoleInfo_Id;

                        var currentRole = dicRoles.Where(c => c.Key == roleId);
                        if (currentRole != null)
                        {
                            roleName = currentRole.First().Value;
                            if (!string.IsNullOrEmpty(roleName) && !roles.Contains(roleName))
                            {
                                roles.Add(roleName);
                            }
                        }
                    }
                    dicRoleUsers.Add(userName, roles);
                }
            }
        }

        private void FilldicResponsibilityUsers(IList<OracleEBSUserResponsibility> listUserRes)
        {
            dicResponsibilitiesUsers = new Dictionary<string, ArrayList>(); string userName = string.Empty; string resName = string.Empty;

            foreach (var item in dicUsers)
            {
                long userId = item.Key;
                userName = item.Value;
                if (!dicResponsibilitiesUsers.ContainsKey(userName))
                {
                    ArrayList responsibilities = new ArrayList();
                    var listcurrentUser = listUserRes.Where(c => c.OracleEBSUserInfo_Id == userId);
                    foreach (var resitems in listcurrentUser)
                    {
                        long resId = resitems.OracleEBSResponsibilitiesInfo_Id;
                        var currentRes = dicResponsibilities.Where(c => c.Key == resId);
                        if (currentRes != null)
                        {
                            resName = currentRes.First().Value;
                            if (!string.IsNullOrEmpty(resName) && !responsibilities.Contains(resName))
                            {
                                responsibilities.Add(resName);
                            }
                        }

                    }
                    dicResponsibilitiesUsers.Add(userName, responsibilities);
                }
            }
        }

        private void DiscoverUsers()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Users"));
                IList<OracleEBSUserInfo> discoveredUsersList = this.GetUsers();
                if (discoveredUsersList != null && discoveredUsersList.Count > 0)
                {
                    OracleEBSUserInfo.SaveUsers(discoveredUsersList, EndPoint.Id, MasterConfigHelper.ServiceUser);
                    discoveredUsersList = null;
                    AccountReconcilation.ProcessDeletedAccounts(this.EndPoint.Id, this.Process, MasterConfigHelper.ServiceUser);
                    discoveredUsersList = OracleEBSUserInfo.GetDeletedUsers(this.EndPoint.Id, this.Process);
                    if (discoveredUsersList != null && discoveredUsersList.Count > 0)
                    {
                        OracleEBSUserInfo.RemoveOracleUMUser(discoveredUsersList.ToList());
                    }


                }
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Users"));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while processing the DiscoverUsers Method."));
                throw;
            }
        }

        private void DiscoverRoles()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Roles"));
                IList<OracleEBSRoleInfo> discoveredRolesList = new List<OracleEBSRoleInfo>();
                discoveredRolesList = this.GetRoles();
                if (discoveredRolesList != null && discoveredRolesList.Count > 0)
                {
                    OracleEBSRoleInfo.SaveRoles(discoveredRolesList, MasterConfigHelper.ServiceUser);
                }
                discoveredRolesList = null;
                discoveredRolesList = OracleEBSRoleInfo.GetDeletedByEndPointSystemId(this.EndPoint.Id);
                OracleEBSRoleInfo.DeleteList(discoveredRolesList.ToList());
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Roles from {0}", EndPoint.DisplayName));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while processing the DiscoverRoles Method."));
                throw;
            }
        }

        private void DiscoverResponsibilities()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Responsibilities"));
                IList<OracleEBSResponsibilitiesInfo> discoveredResponsibilitiesList = new List<OracleEBSResponsibilitiesInfo>();
                discoveredResponsibilitiesList = this.GetResponsibilities();
                if (discoveredResponsibilitiesList != null && discoveredResponsibilitiesList.Count > 0)
                {
                    OracleEBSResponsibilitiesInfo.SaveResponsibilities(discoveredResponsibilitiesList, MasterConfigHelper.ServiceUser);
                }
                discoveredResponsibilitiesList = null;
                discoveredResponsibilitiesList = OracleEBSResponsibilitiesInfo.GetDeletedByEndPointSystemId(this.EndPoint.Id);
                OracleEBSResponsibilitiesInfo.DeleteList(discoveredResponsibilitiesList.ToList());
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Responsibilities from {0}", EndPoint.DisplayName));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while processing the DiscoverResponsibilities Method."));
                throw;
            }
        }

        private void DiscoverMenus()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Menus"));
                IList<OracleEBSMenusInfo> discoveredMenusList = new List<OracleEBSMenusInfo>();
                discoveredMenusList = this.GetMenus();
                if (discoveredMenusList != null && discoveredMenusList.Count > 0)
                {
                    OracleEBSMenusInfo.AddMenus(discoveredMenusList, MasterConfigHelper.ServiceUser);
                }
                discoveredMenusList = null;
                discoveredMenusList = OracleEBSMenusInfo.GetDeletedByEndPointSystemId(this.EndPoint.Id);
                OracleEBSMenusInfo.DeleteList(discoveredMenusList.ToList());
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Menus from {0}", EndPoint.DisplayName));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while processing the DiscoverMenus Method."));
                throw;
            }
        }

        private void DiscoverSubMenus()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering SubMenus"));
                IList<OracleEBSSubmenusInfo> discoveredMenusList = new List<OracleEBSSubmenusInfo>();
                discoveredMenusList = this.GetSubMenus();
                if (discoveredMenusList != null && discoveredMenusList.Count > 0)
                {
                    OracleEBSSubmenusInfo.AddSubMenus(discoveredMenusList, MasterConfigHelper.ServiceUser);
                }
                discoveredMenusList = null;
                discoveredMenusList = OracleEBSSubmenusInfo.GetDeletedByEndPointSystemId(this.EndPoint.Id);
                OracleEBSSubmenusInfo.DeleteList(discoveredMenusList.ToList());
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering SubMenus from {0}", EndPoint.DisplayName));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while processing the DiscoverSubMenus Method."));
                throw;
            }
        }

        private void DiscoverUsersRoles()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering UsersRoles"));
                IList<OracleEBSUserRole> discoverdUserRoleList = this.GetUserRoles();
                if (discoverdUserRoleList != null && discoverdUserRoleList.Count > 0)
                {

                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering UsersRoles"));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while processing the UsersRoles Method."));
                throw;
            }
        }

        private void DiscoverUsersResponsibilities()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering User-Responsibilities ( Method)"));
                IList<OracleEBSUserResponsibility> discoverdUserRoleList = GetUserResponsibilities();
                if (discoverdUserRoleList != null && discoverdUserRoleList.Count > 0)
                {

                }
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Users-Responsibilities ( Method)"));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while processing the Users Responsibilities Method."));
                throw;
            }
        }

        #endregion

        #region OnBoardOffBoardUser
        private void TrackEventTriggers(User user, bool isNewUser)
        {
            //trigger on board event if the join date is greater than equal to today during first time discovery
            if (user.JoinDate.HasValue)
            {
                var trigger = this.PopulateOnBoardTrigger(user, isNewUser);

                if (trigger != null && trigger.Id == 0)
                {
                    triggers.Add(trigger);
                }
            }

            //trigger off board event if the termination date is greater than equal to today during first time discovery
            if (isNewUser == false)
            {
                if (user.TerminationDate.Value.Date >= DateTime.Today && user.TerminationDate.Value.Date != Constants.SAP_MAX_DATE)
                {
                    var trigger = this.PopulateOffBoardTrigger(user);

                    if (trigger != null && trigger.Id == 0)
                    {
                        triggers.Add(trigger);
                    }
                }
            }
        }
        private EmployeeEventTrigger PopulateOnBoardTrigger(User user, bool isNewUser)
        {
            try
            {
                var evnt = EmployeeEvents.NewJoinee;
                var trigger = EmployeeEventTrigger.GetByUserIdTriggerToExecute(user.Id, evnt);    // userFacade.GetEmployeeEventTrigger(user.Id, evnt);
                double onBoardDaysBeforeJoining = DBConfigHelper.MasterConfigHelper.OnBoardDaysBeforeJoining;
                DateTime priorBoardingDate = user.JoinDate.Value.AddDays(-(onBoardDaysBeforeJoining));
                if (trigger != null)
                {
                    var tempTrigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, evnt, DateTime.Today);   //userFacade.GetEmployeeEventTrigger(user.Id, evnt, DateTime.Today);
                    if (tempTrigger != null && tempTrigger.IsRequestCreated)
                    {
                        return null;
                    }
                    if (priorBoardingDate >= DateTime.Today)
                    {
                        trigger.EffectiveDate = priorBoardingDate;
                    }
                    else
                    {
                        trigger.EffectiveDate = DateTime.Today;
                    }

                    trigger.UserDetails = trigger.EventType == EmployeeEvents.Rehire ? UtilityMethods.Serialize<User>(user) : string.Empty;
                    trigger.Save(MasterConfigHelper.ServiceUser);
                    return trigger;
                }
                else
                {
                    if (!isNewUser)
                    {
                        return null;
                    }

                    EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                    {
                        EventType = evnt,
                        IsRequestCreated = false,
                        UserId = user.Id,
                        IsActive = true,
                        IsAdminCheckRequired = false,
                        EffectiveDate = DateTime.Now

                    };

                    if (priorBoardingDate >= DateTime.Today)
                    {
                        newTrigger.EffectiveDate = priorBoardingDate;
                    }
                    else
                    {
                        newTrigger.EffectiveDate = DateTime.Today;
                    }


                    return newTrigger;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private EmployeeEventTrigger PopulateOffBoardTrigger(User user)
        {
            var evnt = EmployeeEvents.Resignation;
            var trigger = EmployeeEventTrigger.GetByUserIdTriggerToExecute(user.Id, evnt);
            if (trigger != null)
            {
                var tempTrigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, evnt, DateTime.Today);
                if (tempTrigger != null && tempTrigger.IsRequestCreated)
                {
                    return null;
                }
                trigger.EffectiveDate = user.TerminationDate.Value;

                trigger.Save(MasterConfigHelper.ServiceUser);
                return trigger;
            }
            else
            {
                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    EffectiveDate = user.TerminationDate.Value,
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true
                };
                return newTrigger;
            }
        }
        private EmployeeEventTrigger PopulateOUChangeTrigger(User user, PresentationOrgChangeTrack orgTrack)
        {
            var trigger = EmployeeEventTrigger.GetByUserIdTriggerToExecute(user.Id, orgTrack.Event);
            if (trigger != null)
            {
                var tempTrigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, orgTrack.Event, DateTime.Today);
                if (tempTrigger != null && tempTrigger.IsRequestCreated)
                {
                    return null;
                }
                trigger.EffectiveDate = orgTrack.EffectiveDate;


                trigger.UserDetails = UtilityMethods.Serialize<User>(user);
                trigger.Save(MasterConfigHelper.ServiceUser); return trigger;
            }
            else
            {
                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    EffectiveDate = DateTime.Now,
                    EventType = orgTrack.Event,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true
                };
                newTrigger.OldUserDetails = UtilityMethods.Serialize<User>(user);
                newTrigger.UserDetails = UtilityMethods.Serialize<User>(user);
                return newTrigger;
            }
        }
        #endregion
    }

}
