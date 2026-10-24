#region NameSpace
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Configuration;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using CheckNulls = DI.Chakra.DomainModel.CheckNull;
using System.Data.OracleClient;
using DI.Chakra.Logger;
using DI.Chakra.DBConfigHelper;
using DeepIdentity.Connector.PeopleSoft;

#endregion

namespace DI.Chakra.Connector
{
    public class PeopleSoftConnector : ConnectorBase
    {
        #region Members

        public EndpointConfiguration peoplesoftConfiguration { get; set; }
        public EndPoint endpoint;
        List<EmployeeEventTrigger> triggers = new List<EmployeeEventTrigger>();

        #endregion

        #region Core

        public PeopleSoftConnector(EndPoint _endpoint, AccessRequest currentRequest = null)
        {
            this.peoplesoftConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            EndPoint = _endpoint;
            this.CurrentRequest = currentRequest;
        }
        public override IMProcessor IMProcessor
        {
            get
            {
                PeopleSoft.IMProcessor processor = new PeopleSoft.IMProcessor();

                processor.EndPoint = this.EndPoint;
                processor.peoplesoftConfiguration = this.peoplesoftConfiguration;


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

                string PSConStr = "";
                string portNo = "";
                string InstanceName = "";
                string Password = RSAHelper.DecryptData(Configuration(EndpointKey.Password));

                if (Convert.ToInt32(Configuration(EndpointKey.PortNo)) != 0)
                {
                    portNo = Configuration(EndpointKey.PortNo);
                }
                else
                {
                    portNo = "1521";
                }

                if (Configuration(EndpointKey.InstanceName) != null && Configuration(EndpointKey.InstanceName) != "")
                {
                    InstanceName = Configuration(EndpointKey.InstanceName);
                }
                else
                {
                    InstanceName = "oracle";
                }

                PSConStr = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                  "User Id={3};Password={4};", Configuration(EndpointKey.IPAddress), portNo, InstanceName, Configuration(EndpointKey.UserName), Password);

                OrCon = new OracleConnection(PSConStr);
                OrCon.Open();
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Endpoint syetem {0} connection opened", Configuration(EndpointKey.HostName)));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Endpoint syetem {0} connection failed", Configuration(EndpointKey.HostName)), ex);
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
            var config = peoplesoftConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
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
                string CreateLoginQuery = "UPDATE PSOPRDEFN  SET OPERPSWD =:OperPswd, OPERPSWDSALT =:OperPswdSalt, ENCRYPTED = 1 Where OPRID = :OprId";

                putOracleQueryToDB(CreateLoginQuery,
                    new OracleParameter("OprId", UserName),
                    new OracleParameter("OperPswd", "{1}A4Ngxo5bzp8fp9+fxtJHogZQaXU="),
                    new OracleParameter("OperPswdSalt", "EOZJU9bhSfBUNDJK241K3w=="));
                ReturnValue = true;

            }

            catch (Exception ex)
            {

                return false;
            }
            return ReturnValue;


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
                string oracleConnectionString = "";
                string portNo = "";
                string InstanceName = "";
                string Password = RSAHelper.DecryptData(Configuration(EndpointKey.Password));
                if (Convert.ToInt32(Configuration(EndpointKey.PortNo)) != 0)
                {
                    portNo = Configuration(EndpointKey.PortNo);
                }
                else
                {
                    portNo = "1521";
                }
                if (Configuration(EndpointKey.InstanceName) != null && Configuration(EndpointKey.InstanceName) != "")
                {
                    InstanceName = Configuration(EndpointKey.InstanceName);
                }
                else
                {
                    InstanceName = "Oracle";
                }

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

            }
            catch (Exception ex)
            {
                if (OrCon.State == ConnectionState.Open)
                {
                    OrCon.Close();
                    OrCon.Dispose();
                }
                if (ex.Message != null && ex.Message.Contains("unique constraint"))
                    ReturnValue = true;
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

        public override void GetData()
        {
            try
            {
                if (!this.EndPoint.IsTrustedSource)
                {
                    long DiscoveryProcessId = base.DiscoveryProcess.Id;


                    List<EndpointObjects> Objects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);
                    bool IsRolesSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PS_Roles) != null;
                    bool IsUsersSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PS_Users) != null;
                    bool IsPermissionSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PS_PermissionList) != null;
                    bool IsMenuSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PS_Menus) != null;
                    bool IsPortalSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PS_Portal) != null;
                    bool IsRoleClassSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PS_PSClass) != null;
                    bool IsAccessLogSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpontObject_PS_AccessLog) != null;

                    if (IsRolesSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Roles discovery started ", Configuration(EndpointKey.HostName)));
                        List<PeopleSoftRoleInfo> DiscoveredRoles = getRoles();
                        if (DiscoveredRoles != null)
                        {
                            List<PeopleSoftRoleInfo> Existingroles = PeopleSoftRoleInfo.GetByEndpointId(this.EndPoint.Id);
                            List<PeopleSoftRoleInfo> Deletedroles = Existingroles.Where(c => DiscoveredRoles.Count(d => d.ROLENAME == c.ROLENAME) == 0).ToList();
                            List<PeopleSoftRoleInfo> Newroles = DiscoveredRoles.Where(c => Existingroles.Count(d => d.ROLENAME == c.ROLENAME) == 0).ToList();
                            List<PeopleSoftRoleInfo> Otherroles = DiscoveredRoles.Where(c => Existingroles.Count(d => d.ROLENAME == c.ROLENAME) > 0).ToList();
                            Otherroles.Select(c => { c.Id = Existingroles.Where(d => d.ROLENAME == c.ROLENAME).Select(e => e.Id).FirstOrDefault(); return c; }).ToList();
                            List<PeopleSoftRoleInfo> savingRoles = Otherroles.Union(Newroles).ToList();
                            savingRoles.ForEach(lq =>
                            {
                                if (lq.Id <= 0)
                                    lq.CanRequest = true;
                            });
                            PeopleSoftRoleInfo.SaveList(savingRoles, MasterConfigHelper.ServiceUser);
                            PeopleSoftRoleInfo.DeleteList(Deletedroles);
                            WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Roles discovery completed - {1} roles discovered ", Configuration(EndpointKey.HostName), DiscoveredRoles.Count));
                        }
                    }

                    if (IsPermissionSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Permoissions discovery started ", Configuration(EndpointKey.HostName)));
                        List<PeopleSoftPermissionInfo> DiscoveredPermission = getPermission();
                        if (DiscoveredPermission != null)
                        {
                            List<PeopleSoftPermissionInfo> Existingpermissions = PeopleSoftPermissionInfo.GetByEndpointId(this.EndPoint.Id);
                            List<PeopleSoftPermissionInfo> Deletedpermissions = Existingpermissions.Where(c => DiscoveredPermission.Count(d => d.CLASSID == c.CLASSID) == 0).ToList();
                            List<PeopleSoftPermissionInfo> Newpermissions = DiscoveredPermission.Where(c => Existingpermissions.Count(d => d.CLASSID == c.CLASSID) == 0).ToList();
                            List<PeopleSoftPermissionInfo> Otherpermissions = DiscoveredPermission.Where(c => Existingpermissions.Count(d => d.CLASSID == c.CLASSID) > 0).ToList();
                            Otherpermissions.Select(c => { c.Id = Existingpermissions.Where(d => d.CLASSID == c.CLASSID).Select(e => e.Id).FirstOrDefault(); return c; }).ToList();
                            List<PeopleSoftPermissionInfo> savingroles = Otherpermissions.Union(Newpermissions).ToList();
                            PeopleSoftPermissionInfo.SaveList(savingroles, MasterConfigHelper.ServiceUser);
                            PeopleSoftPermissionInfo.DeleteList(Deletedpermissions);
                            WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - permissions discovery completed - {1} permissions discovered ", Configuration(EndpointKey.HostName), DiscoveredPermission.Count));

                        }
                    }


                    if (IsMenuSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Menus discovery started ", Configuration(EndpointKey.HostName)));
                        IList<PeopleSoftMenus> DiscoveredMenus = getMenu();
                        if (DiscoveredMenus != null)
                        {

                            List<PeopleSoftMenus> Existingmenus = PeopleSoftMenus.GetByEndpointId(this.EndPoint.Id);
                            List<PeopleSoftMenus> DeletedMenus = Existingmenus.Where(c => DiscoveredMenus.Count(d => d.MENU_ID == c.MENU_ID) == 0).ToList();
                            List<PeopleSoftMenus> NewMenus = DiscoveredMenus.Where(c => Existingmenus.Count(d => d.MENU_ID == c.MENU_ID) == 0).ToList();
                            List<PeopleSoftMenus> OtherMenus = DiscoveredMenus.Where(c => Existingmenus.Count(d => d.MENU_ID == c.MENU_ID) > 0).ToList();
                            OtherMenus.Select(c => { c.Id = Existingmenus.Where(d => d.MENU_ID == c.MENU_ID).Select(e => e.Id).FirstOrDefault(); return c; }).ToList();
                            List<PeopleSoftMenus> savingMenus = OtherMenus.Union(NewMenus).ToList();
                            PeopleSoftMenus.SaveList(savingMenus, MasterConfigHelper.ServiceUser);
                            PeopleSoftMenus.DeleteList(DeletedMenus);
                            WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Menus discovery completed - {1} menus discovered ", Configuration(EndpointKey.HostName), DiscoveredMenus.Count));
                        }
                    }

                    if (IsAccessLogSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Accesslogs discovery started ", Configuration(EndpointKey.HostName)));
                        IList<PeopleSoftAccessLogInfo> Discoveredaccesslog = getAccesslog();
                        if (Discoveredaccesslog != null)
                        {
                            List<PeopleSoftAccessLogInfo> Existingaccesslogs = PeopleSoftAccessLogInfo.GetByEndpointId(this.EndPoint.Id);
                            List<PeopleSoftAccessLogInfo> Deletedaccesslogs = Existingaccesslogs.Where(c => Discoveredaccesslog.Count(d => d.OPRID == c.OPRID) == 0).ToList();
                            List<PeopleSoftAccessLogInfo> Newaccesslogs = Discoveredaccesslog.Where(c => Existingaccesslogs.Count(d => d.OPRID == c.OPRID) == 0).ToList();
                            List<PeopleSoftAccessLogInfo> Otheraccesslogs = Discoveredaccesslog.Where(c => Existingaccesslogs.Count(d => d.OPRID == c.OPRID) > 0).ToList();
                            Otheraccesslogs.Select(c => { c.Id = Existingaccesslogs.Where(d => d.OPRID == c.OPRID).Select(e => e.Id).FirstOrDefault(); return c; }).ToList();
                            List<PeopleSoftAccessLogInfo> savingals = Otheraccesslogs.Union(Newaccesslogs).ToList();
                            PeopleSoftAccessLogInfo.SaveList(savingals, MasterConfigHelper.ServiceUser);
                            PeopleSoftAccessLogInfo.DeleteList(Deletedaccesslogs);
                            WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - accesslogs discovery completed - {1} accesslogs discovered ", Configuration(EndpointKey.HostName), Discoveredaccesslog.Count));

                        }
                    }

                    if (IsRolesSelected && IsPermissionSelected)
                    {

                        IList<PeopleSoftRoleclass> Discoveredroleclass = getRoleClass();
                        if (IsRoleClassSelected && Discoveredroleclass != null)
                        {
                            WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Roleclasses discovery started ", Configuration(EndpointKey.HostName)));
                            List<PeopleSoftRoleclass> Existingroleclass = PeopleSoftRoleclass.GetByEndpointId(this.EndPoint.Id);
                            List<PeopleSoftRoleclass> Deletedroleclass = Existingroleclass.Where(c => Discoveredroleclass.Count(d => d.ROLENAME == c.ROLENAME) == 0).ToList();
                            List<PeopleSoftRoleclass> Newroleclass = Discoveredroleclass.Where(c => Existingroleclass.Count(d => d.ROLENAME == c.ROLENAME) == 0).ToList();
                            List<PeopleSoftRoleclass> Otherroleclass = Discoveredroleclass.Where(c => Existingroleclass.Count(d => d.ROLENAME == c.ROLENAME) > 0).ToList();
                            Otherroleclass.Select(c => { c.Id = Existingroleclass.Where(d => d.ROLENAME == c.ROLENAME).Select(e => e.Id).FirstOrDefault(); return c; }).ToList();
                            List<PeopleSoftRoleclass> savingroles = Otherroleclass.Union(Newroleclass).ToList();
                            PeopleSoftRoleclass.SaveList(savingroles, MasterConfigHelper.ServiceUser);
                            PeopleSoftRoleclass.DeleteList(Deletedroleclass);
                            WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - roleclasses discovery completed - {1} roleclasses discovered ", Configuration(EndpointKey.HostName), Discoveredroleclass.Count));

                        }
                        List<string> listPermissionRoles = new List<string>();
                        foreach (PeopleSoftRoleclass roleclass in Discoveredroleclass)
                        {
                            PeopleSoftRoleInfo Peoplerole = PeopleSoftRoleInfo.GetByRole(roleclass.ROLENAME, this.EndPoint.Id);
                            PeopleSoftPermissionInfo peoplePermission = PeopleSoftPermissionInfo.GetByPermissionKey(roleclass.CLASSID, this.EndPoint.Id).FirstOrDefault();
                            if (Peoplerole != null && peoplePermission != null)
                            {
                                string Query = string.Format("insert into PeopleSoftRoleInfoPeopleSoftPermissionInfo([PeopleSoftRoleInfo_Id],[PeopleSoftPermissionInfo_Id],[EndpointSystemId],[Version]) values ({0},{1},{2},{3})",
                                                                Peoplerole.Id, peoplePermission.Id, this.EndPoint.Id, (int)VersionNo.Available);
                                listPermissionRoles.Add(Query);

                            }
                        }
                        PeopleSoftRoleInfo.ExecuteQuery(listPermissionRoles, this.EndPoint.Id);
                    }



                    if (IsPortalSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - portals discovery started ", Configuration(EndpointKey.HostName)));
                        IList<PeopleSoftPortal> DiscoveredPortal = getPortal();
                        if (DiscoveredPortal != null)
                        {

                            List<PeopleSoftPortal> ExistingPortal = PeopleSoftPortal.GetByEndpointId(this.EndPoint.Id);
                            List<PeopleSoftPortal> Deletedroleclass = ExistingPortal.Where(c => DiscoveredPortal.Count(d => d.PORTAL_NAME == c.PORTAL_NAME) == 0).ToList();
                            List<PeopleSoftPortal> Newroleclass = DiscoveredPortal.Where(c => ExistingPortal.Count(d => d.PORTAL_NAME == c.PORTAL_NAME) == 0).ToList();
                            List<PeopleSoftPortal> Otherroleclass = DiscoveredPortal.Where(c => ExistingPortal.Count(d => d.PORTAL_NAME == c.PORTAL_NAME) > 0).ToList();
                            Otherroleclass.Select(c => { c.Id = ExistingPortal.Where(d => d.PORTAL_NAME == c.PORTAL_NAME).Select(e => e.Id).FirstOrDefault(); return c; }).ToList();
                            List<PeopleSoftPortal> savingroles = Otherroleclass.Union(Newroleclass).ToList();
                            PeopleSoftPortal.SaveList(savingroles, MasterConfigHelper.ServiceUser);
                            PeopleSoftPortal.DeleteList(Deletedroleclass);
                            WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - portals discovery completed - {1} portals discovered ", Configuration(EndpointKey.HostName), DiscoveredPortal.Count));


                        }
                    }

                    if (IsUsersSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - users discovery started ", Configuration(EndpointKey.HostName)));
                        IList<PeopleSoftUserInfo> DiscoveredUsers = getUsers();
                        if (DiscoveredUsers != null)
                        {
                            PeopleSoftUserInfo.SaveUsers(DiscoveredUsers, this.EndPoint.Id, MasterConfigHelper.ServiceUser);
                        }
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - users discovery completed - {1} users discovered ", Configuration(EndpointKey.HostName), DiscoveredUsers.Count));
                    }


                    if (IsUsersSelected && IsRolesSelected)
                    {
                        getRoleUser();
                        this.UpdateEntitlementReconcilation();
                    }

                    WriteDiscoveryLog(LogLevelType.Info, string.Format("User Mapping started - {0} ", Configuration(EndpointKey.HostName)));
                    //PeopleSoftUserInfo.MapWithMasterUser(EndPoint, peoplesoftConfiguration.MapAll != null ? peoplesoftConfiguration.MapAll : false);
                    PeopleSoftUserInfo.MapWithMasterUserForReconTable(EndPoint, peoplesoftConfiguration.MapAll != null ? peoplesoftConfiguration.MapAll : false);
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("User Mapping completed - {0} ", Configuration(EndpointKey.HostName)));
                }
                else
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - HRMSUsers discovery started ", Configuration(EndpointKey.HostName)));
                    IList<User> HRMSusers = getHRMSUsers();
                    List<User> oracleHRMSUsersList = new List<User>();

                    #region "User Table"

                    IList<User> listUsers = new List<User>();
                    User user;
                    bool isnewuser = false;

                    if (HRMSusers != null && HRMSusers.Count > 0)
                    {
                        listUsers = User.GetAll(false);
                        IList<NOrganisation> listAllOrganisations = new List<NOrganisation>();//Organisation.GetByTenantId(EndPoint.TenantId);
                        foreach (User trsusr in HRMSusers)
                        {
                            user = listUsers.FirstOrDefault(s => s.EmployeeId == trsusr.EmployeeId);
                            triggers.Clear();
                            if (user == null)
                            {
                                user = trsusr;
                                isnewuser = true;
                                user.Password = RSAHelper.EncryptData(MasterConfigHelper.DefaultPassword);
                                user.Save(MasterConfigHelper.ServiceUser);
                                int GroupID = DBConfigHelper.MasterConfigHelper.RoleUser;
                                User.InsertUserRole(user.Id, GroupID);
                                WriteDiscoveryLog(LogLevelType.Info, string.Format(" New User {0} found in Peoplesoft HRMS", user.UserName));
                                TrackEventTriggers(user, isnewuser);

                            }
                            else
                            {
                                user = User.GetByEmployeeId(user.EmployeeId);

                                trsusr.Id = user.Id;
                                //retain existing value from user object itself
                                trsusr.UserName = user.UserName;
                                trsusr.EmailId = user.EmailId;
                                trsusr.Password = user.Password;
                                trsusr.AuthenticationMode = user.AuthenticationMode;

                                if (trsusr.TerminationDate <= DateTime.Now)//(usr1.TerminationDate != trsusr.EFFECTIVE_END_DATE)
                                {
                                    isnewuser = false;
                                    TrackEventTriggers(trsusr, isnewuser);
                                }
                                else
                                {
                                    user.TerminationDate = null;
                                }

                                {
                                    CreateTransferTriggers(user.Id, DateTime.Now);
                                }

                                trsusr.Save(MasterConfigHelper.ServiceUser);

                            }
                            EmployeeEventTrigger.ValidateAndCreateTriggers(triggers, MasterConfigHelper.ServiceUser);
                        }
                    }

                    #endregion

                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - HRMSUsers discovery completed ", Configuration(EndpointKey.HostName)));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private DataSet getDSFromOracleQuery(string strQry)
        {
            OracleConnection OrCon = null;
            OracleCommand OrCom = null;
            OracleDataAdapter OrAdapter;
            DataSet dsResults = new DataSet();
            try
            {
                string oracleConnectionString = "";
                string portNo = "";
                string InstanceName = "";
                string Password = RSAHelper.DecryptData(Configuration(EndpointKey.Password));

                if (Convert.ToInt32(Configuration(EndpointKey.PortNo)) != 0)
                {
                    portNo = Configuration(EndpointKey.PortNo);
                }
                else
                {
                    portNo = "1521";
                }

                if (Configuration(EndpointKey.InstanceName) != null && Configuration(EndpointKey.InstanceName) != "")
                {
                    InstanceName = Configuration(EndpointKey.InstanceName);
                }
                else
                {
                    InstanceName = "Oracle";
                }

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

            }
            catch (Exception ex)
            {
                if (OrCon.State == ConnectionState.Open)
                {
                    OrCon.Close();
                    OrCon.Dispose();
                }
            }

            return dsResults;
        }

        private IList<PeopleSoftUserInfo> getUsers()
        {
            PeopleSoftUserInfo PSUser = null;
            IList<PeopleSoftUserInfo> listPSUsers = new List<PeopleSoftUserInfo>();

            try
            {

                string UserSQL = @"select OPRID, OPRDEFNDESC, EMPLID,EMAILID, OPRCLASS, ROWSECCLASS, ENCRYPTED, SYMBOLICID,
                                        LANGUAGE_CD, MULTILANG, CURRENCY_CD, LASTPSWDCHANGE, ACCTLOCK, PRCSPRFLCLS, DEFAULTNAVHP,
                                        FAILEDLOGINS, EXPENT, OPRTYPE, USERIDALIAS, LASTSIGNONDTTM, LASTUPDDTTM, LASTUPDOPRID,
                                        PTALLOWSWITCHUSER from PSOPRDEFN ";

                DataSet dsUserinfo = getDSFromOracleQuery(UserSQL);

                if (dsUserinfo.Tables.Count <= 0)
                    return null;

                foreach (DataRow item in dsUserinfo.Tables[0].Rows)
                {
                    PSUser = new PeopleSoftUserInfo();

                    PSUser.EndPointSystemId = EndPoint.Id;
                    PSUser.OPRID = CheckNulls.AgainstString(item["OPRID"]);
                    PSUser.OPRDEFNDESC = CheckNulls.AgainstString(item["OPRDEFNDESC"]);
                    PSUser.EMPLID = CheckNulls.AgainstString(item["EMPLID"]);
                    PSUser.EMAILID = CheckNulls.AgainstString(item["EMAILID"]);
                    PSUser.OPRCLASS = CheckNulls.AgainstString(item["OPRCLASS"]);
                    PSUser.ROWSECCLASS = CheckNulls.AgainstString(item["ROWSECCLASS"]);
                    PSUser.ENCRYPTED = CheckNulls.AgainstString(item["ENCRYPTED"]);
                    PSUser.SYMBOLICID = CheckNulls.AgainstString(item["SYMBOLICID"]);
                    PSUser.LANGUAGE_CD = CheckNulls.AgainstString(item["LANGUAGE_CD"]);
                    PSUser.MULTILANG = CheckNulls.AgainstString(item["MULTILANG"]);
                    PSUser.CURRENCY_CD = CheckNulls.AgainstString(item["CURRENCY_CD"]);
                    PSUser.LASTPSWDCHANGE = CheckNulls.AgainstString(item["LASTPSWDCHANGE"]);
                    PSUser.ACCTLOCK = CheckNulls.AgainstString(item["ACCTLOCK"]);
                    PSUser.PRCSPRFLCLS = CheckNulls.AgainstString(item["PRCSPRFLCLS"]);
                    PSUser.DEFAULTNAVHP = CheckNulls.AgainstString(item["DEFAULTNAVHP"]);
                    PSUser.FAILEDLOGINS = CheckNulls.AgainstString(item["FAILEDLOGINS"]);
                    PSUser.EXPENT = CheckNulls.AgainstString(item["EXPENT"]);
                    PSUser.OPRTYPE = CheckNulls.AgainstString(item["OPRTYPE"]);
                    PSUser.USERIDALIAS = CheckNulls.AgainstString(item["USERIDALIAS"]);
                    PSUser.LASTSIGNONDTTM = CheckNulls.AgainstString(item["LASTSIGNONDTTM"]);
                    PSUser.LASTUPDDTTM = CheckNulls.AgainstString(item["LASTUPDDTTM"]);
                    PSUser.PTALLOWSWITCHUSER = CheckNulls.AgainstString(item["PTALLOWSWITCHUSER"]);
                    listPSUsers.Add(PSUser);

                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

            }
            return listPSUsers;
        }

        private List<PeopleSoftRoleInfo> getRoles()
        {
            PeopleSoftRoleInfo role = null;
            List<PeopleSoftRoleInfo> listRoles = new List<PeopleSoftRoleInfo>();

            try
            {
                string RolesSQL = @"SELECT ROLENAME, VERSION, ROLETYPE, DESCR, QRYNAME, ROLESTATUS, RECNAME, FIELDNAME, 
                            PC_EVENT_TYPE, QRYNAME_SEC, PC_FUNCTION_NAME, ROLE_PCODE_RULE_ON, ROLE_QUERY_RULE_ON, LDAP_RULE_ON,
                            DESCRLONG, ALLOWNOTIFY, ALLOWLOOKUP, LASTUPDDTTM, LASTUPDOPRID from PSROLEDEFN";
                DataSet dsResults = getDSFromOracleQuery(RolesSQL);

                if (dsResults.Tables.Count <= 0)
                    return null;

                foreach (DataRow item in dsResults.Tables[0].Rows)
                {
                    role = new PeopleSoftRoleInfo();

                    role.EndPointSystemId = EndPoint.Id;
                    role.ROLENAME = CheckNulls.AgainstString(item["ROLENAME"]);
                    role.ROLETYPE = CheckNulls.AgainstString(item["ROLETYPE"]);
                    role.DESCR = CheckNulls.AgainstString(item["DESCR"]);
                    role.QRYNAME = CheckNulls.AgainstString(item["QRYNAME"]);
                    role.ROLESTATUS = CheckNulls.AgainstString(item["ROLESTATUS"]);
                    role.RECNAME = CheckNulls.AgainstString(item["RECNAME"]);
                    role.FIELDNAME = CheckNulls.AgainstString(item["FIELDNAME"]);
                    role.PC_EVENT_TYPE = CheckNulls.AgainstString(item["PC_EVENT_TYPE"]);
                    role.QRYNAME_SEC = CheckNulls.AgainstString(item["QRYNAME_SEC"]);
                    role.PC_FUNCTION_NAME = CheckNulls.AgainstString(item["PC_FUNCTION_NAME"]);
                    role.ROLE_PCODE_RULE_ON = CheckNulls.AgainstString(item["ROLE_PCODE_RULE_ON"]);
                    role.ROLE_QUERY_RULE_ON = CheckNulls.AgainstString(item["ROLE_QUERY_RULE_ON"]);
                    role.LDAP_RULE_ON = CheckNulls.AgainstString(item["LDAP_RULE_ON"]);
                    role.DESCRLONG = CheckNulls.AgainstString(item["DESCRLONG"]);
                    role.ALLOWNOTIFY = CheckNulls.AgainstString(item["ALLOWNOTIFY"]);
                    role.ALLOWLOOKUP = CheckNulls.AgainstString(item["ALLOWLOOKUP"]);
                    role.LASTUPDDTTM = CheckNulls.AgainstNullableDatetime(item["LASTUPDDTTM"]);
                    role.LASTUPDOPRID = CheckNulls.AgainstString(item["LASTUPDOPRID"]);
                    listRoles.Add(role);
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }

            return listRoles;
        }

        private List<PeopleSoftPermissionInfo> getPermission()
        {
            PeopleSoftPermissionInfo permission = null;
            List<PeopleSoftPermissionInfo> listPermission = new List<PeopleSoftPermissionInfo>();

            try
            {
                string RolesSQL = @"SELECT CLASSID, VERSION, CLASSDEFNDESC, TIMEOUTMINUTES, DEFAULTBPM, STARTAPPSERVER,
                                        ALLOWPSWDEMAIL, LASTUPDDTTM, LASTUPDOPRID from PSCLASSDEFN";
                DataSet dsResults = getDSFromOracleQuery(RolesSQL);

                if (dsResults.Tables.Count <= 0)
                    return null;

                foreach (DataRow item in dsResults.Tables[0].Rows)
                {
                    permission = new PeopleSoftPermissionInfo();

                    //role.RoleId = Convert.ToInt32(item["User_Id"]);

                    permission.EndPointSystemId = EndPoint.Id;
                    permission.CLASSID = CheckNulls.AgainstString(item["CLASSID"]);
                    permission.VERSION = CheckNulls.AgainstString(item["VERSION"]);
                    permission.CLASSDEFNDESC = CheckNulls.AgainstString(item["CLASSDEFNDESC"]);
                    permission.TIMEOUTMINUTES = CheckNulls.AgainstString(item["TIMEOUTMINUTES"]);
                    permission.DEFAULTBPM = CheckNulls.AgainstString(item["DEFAULTBPM"]);
                    permission.STARTAPPSERVER = CheckNulls.AgainstString(item["STARTAPPSERVER"]);
                    permission.ALLOWPSWDEMAIL = CheckNulls.AgainstString(item["ALLOWPSWDEMAIL"]);
                    permission.LASTUPDDTTM = CheckNulls.AgainstString(item["LASTUPDDTTM"]);
                    permission.LASTUPDOPRID = CheckNulls.AgainstString(item["LASTUPDOPRID"]);
                    listPermission.Add(permission);
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }

            return listPermission;
        }

        private IList<PeopleSoftAccessLogInfo> getAccesslog()
        {
            PeopleSoftAccessLogInfo accesslog = null;
            IList<PeopleSoftAccessLogInfo> listaccesslog = new List<PeopleSoftAccessLogInfo>();

            try
            {
                string RolesSQL = @"select * from PSaccesslog where (OPRID, LOGINDTTM) in (select OPRID, MAX(LOGINDTTM) from PSaccesslog group by OPRID)";
                DataSet dsResults = getDSFromOracleQuery(RolesSQL);
                if (dsResults.Tables.Count <= 0)
                    return null;
                foreach (DataRow item in dsResults.Tables[0].Rows)
                {
                    accesslog = new PeopleSoftAccessLogInfo();
                    accesslog.EndPointSystemId = EndPoint.Id;
                    accesslog.OPRID = CheckNulls.AgainstString(item["OPRID"]);
                    accesslog.LOGIPADDRESS = CheckNulls.AgainstString(item["LOGIPADDRESS"]);
                    accesslog.PT_SIGNON_TYPE = CheckNulls.AgainstInt(item["PT_SIGNON_TYPE"]);
                    accesslog.LOGINDTTM = CheckNulls.AgainstDatetime(item["LOGINDTTM"]);
                    accesslog.LOGOUTDTTM = CheckNulls.AgainstDatetime(item["LOGOUTDTTM"]);
                    listaccesslog.Add(accesslog);
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }

            return listaccesslog;
        }

        private void getRoleUser()
        {

            PeopleSoftRoleUser roleuser = null;
            IList<PeopleSoftRoleUser> listroleuser = new List<PeopleSoftRoleUser>();
            try
            {
                string RolesSQL = @"SELECT  ROLENAME, ROLEUSER, DYNAMIC_SW
                                         from PSRoleUser";
                DataSet dsResults = getDSFromOracleQuery(RolesSQL);

                if (dsResults.Tables.Count <= 0)
                    return;

                foreach (DataRow item in dsResults.Tables[0].Rows)
                {
                    roleuser = new PeopleSoftRoleUser();
                    roleuser.EndpointSystemId = EndPoint.Id;
                    string ROLENAME = CheckNulls.AgainstString(item["ROLENAME"]);
                    string ROLEUSER = CheckNulls.AgainstString(item["ROLEUSER"]);
                    var currentRole = PeopleSoftRoleInfo.GetbyName(ROLENAME, EndPoint.Id).FirstOrDefault();
                    var currentUser = PeopleSoftUserInfo.GetbyName(ROLEUSER, EndPoint.Id).FirstOrDefault();
                    if (currentRole != null && currentUser != null)
                    {
                        roleuser.PeopleSoftRoleInfo_Id = currentRole.Id;
                        roleuser.PeopleSoftUserInfo_Id = currentUser.Id;
                        roleuser.EndpointSystemId = currentUser.EndPointSystemId;
                        roleuser.Version = (int)VersionNo.Available;
                        if (!listroleuser.Contains(roleuser))
                            listroleuser.Add(roleuser);
                    }
                }
                PeopleSoftUserInfo.bulkInsert(listroleuser, this.EndPoint.Id);

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

            }

        }

        private IList<PeopleSoftRoleclass> getRoleClass()
        {
            PeopleSoftRoleclass roleclass = null;
            IList<PeopleSoftRoleclass> listroleclass = new List<PeopleSoftRoleclass>();

            try
            {
                string RolesSQL = @"SELECT * FROM PSRoleclass";
                DataSet dsResults = getDSFromOracleQuery(RolesSQL);

                if (dsResults.Tables.Count <= 0)
                    return null;
                foreach (DataRow item in dsResults.Tables[0].Rows)
                {
                    roleclass = new PeopleSoftRoleclass();
                    roleclass.EndPointSystemId = EndPoint.Id;
                    roleclass.ROLENAME = CheckNulls.AgainstString(item["ROLENAME"]);
                    roleclass.CLASSID = CheckNulls.AgainstString(item["CLASSID"]);
                    listroleclass.Add(roleclass);
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }

            return listroleclass;
        }

        private IList<PeopleSoftMenus> getMenu()
        {
            PeopleSoftMenus Menu = null;
            IList<PeopleSoftMenus> listmenu = new List<PeopleSoftMenus>();

            try
            {
                string RolesSQL = @"SELECT MENUNAME, BARNAME, ITEMNAME, ITEMNUM, ITEMTYPE, PNLGRPNAME,
                                        MARKET, BARLABEL, ITEMLABEL,SEARCHRECNAME from PSMENUITEM";
                DataSet dsResults = getDSFromOracleQuery(RolesSQL);

                if (dsResults.Tables.Count <= 0)
                    return null;

                foreach (DataRow item in dsResults.Tables[0].Rows)
                {
                    Menu = new PeopleSoftMenus();
                    Menu.MENUNAME = CheckNulls.AgainstString(item["MENUNAME"]);
                    Menu.BARNAME = CheckNulls.AgainstString(item["BARNAME"]);
                    Menu.ITEMNAME = CheckNulls.AgainstString(item["ITEMNAME"]);
                    Menu.ITEMNUM = CheckNulls.AgainstDecimal(item["ITEMNUM"]);
                    Menu.ITEMTYPE = CheckNulls.AgainstDecimal(item["ITEMTYPE"]);
                    Menu.PNLGRPNAME = CheckNulls.AgainstString(item["PNLGRPNAME"]);
                    Menu.MARKET = CheckNulls.AgainstString(item["MARKET"]);
                    Menu.BARLABEL = CheckNulls.AgainstString(item["BARLABEL"]);
                    Menu.ITEMLABEL = CheckNulls.AgainstString(item["ITEMLABEL"]);
                    Menu.SEARCHRECNAME = CheckNulls.AgainstString(item["SEARCHRECNAME"]);
                    Menu.EndpointSystemId = EndPoint.Id;
                    listmenu.Add(Menu);
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }

            return listmenu;
        }

        private IList<PeopleSoftPortal> getPortal()
        {
            PeopleSoftPortal Portal = null;
            IList<PeopleSoftPortal> listportal = new List<PeopleSoftPortal>();

            try
            {
                string RolesSQL = @"SELECT PORTAL_NAME, PORTAL_REFTYPE, PORTAL_OBJNAME, PORTAL_PERMNAME, PORTAL_PERMTYPE, PORTAL_ISCASCADE
                                        from PSPRSMPERM";
                DataSet dsResults = getDSFromOracleQuery(RolesSQL);

                if (dsResults.Tables.Count <= 0)
                    return null;

                foreach (DataRow item in dsResults.Tables[0].Rows)
                {
                    Portal = new PeopleSoftPortal();
                    Portal.PORTAL_NAME = CheckNulls.AgainstString(item["PORTAL_NAME"]);
                    Portal.PORTAL_REFTYPE = CheckNulls.AgainstString(item["PORTAL_REFTYPE"]);
                    Portal.PORTAL_OBJNAME = CheckNulls.AgainstString(item["PORTAL_OBJNAME"]);
                    Portal.PORTAL_PERMNAME = CheckNulls.AgainstString(item["PORTAL_PERMNAME"]);
                    Portal.PORTAL_PERMTYPE = CheckNulls.AgainstString(item["PORTAL_PERMTYPE"]);
                    Portal.PORTAL_ISCASCADE = CheckNulls.AgainstDecimal(item["PORTAL_ISCASCADE"]);
                    Portal.EndPointSystemId = this.EndPoint.Id;
                    listportal.Add(Portal);
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }

            return listportal;
        }

        private IList<User> getHRMSUsers()
        {

            IList<User> listPSUsers = new List<User>();

            try
            {

                string UserSQL = @"select A.EMPLID EMPLID, A.EMPL_RCD, A.EFFDT, A.DEPTID DEPTID, A.JOBCODE, A.POSITION_NBR, A.SuperVisor_ID SUPERVISOR_ID, A.JOB_ENTRY_DT, 
                                               A.HIRE_DT HIRE_DT, A.BUSINESS_UNIT, A.SETID_DEPT, A.SETID_JOBCODE,  A.TERMINATION_DT TERMINATION_DT, A.ACTION, 
                                               A.FULL_PART_TIME FULL_PART_TIME, A.COMPANY COMPANY, C.DESCR DEPTNAME,
                                               B.NAME NAME, B.FIRST_NAME FIRST_NAME, B.LAST_NAME LAST_NAME, B.MIDDLE_NAME MIDDLE_NAME, B.NAME_AC, 
                                               B.Address1 ADDRESS1, B.ADDRESS2 ADDRESS2, B.ADDRESS3 ADDRESS3, B.ADDRESS4 ADDRESS4, B.CITY, B.NUM1 NUM1 ,
                                               B.NUM2 NUM2, B.COUNTY COUNTRY, B.STATE, B.POSTAL POSTAL, B.PHONE HOME_PHONE, B.SEX 
                                        from PS_JOB A INNER JOIN PS_PERSONAL_DATA B on B.emplID = A.EmPLID
                                                    Inner JOIN ps_dept_tbl C on c.SETID = A.SETID_DEPT and C.DEPTID = A.DEPTID
                                              and C.ROWID in (select MAX(ROWID) from ps_dept_tbl group by SETID, DEPTID )
                                        where A.HR_STATUS in ('A','I') and EMPL_STATUS in ('A','T') and ACTION in( 'HIR', 'XFR', 'TER')
                                                and A.ROWID in (select max(ROWID) from PS_JOB Group by EMPLID)
                                                {0}
                                        order by EmplId asc, EMPL_RCD asc";

                if (this.IsFirstTimeDiscovery)
                {
                    UserSQL = string.Format(UserSQL, string.Empty);
                }
                else
                {
                    var deltaDate = this.Process.LastRunOn.Value.ToString("yyyy-MM-dd");
                    UserSQL = string.Format(UserSQL, string.Format(@"and (TO_CHAR(A.LASTUPDDTTM,'YYYY-MM-DD') >= '{0}' OR (TO_CHAR(B.LASTUPDDTTM, 'YYYY-MM-DD') >= '{0}'))", deltaDate));
                }

                DataSet dsUserinfo = getDSFromOracleQuery(UserSQL);

                if (dsUserinfo.Tables.Count <= 0)
                    return null;

                foreach (DataRow item in dsUserinfo.Tables[0].Rows)
                {
                    User PSUser = new User();
                    PSUser.IdentificationNo = CheckNulls.AgainstString(item["EMPLID"]);
                    PSUser.UserName = CheckNulls.AgainstString(item["NAME"]);
                    PSUser.FirstName = CheckNulls.AgainstString(item["FIRST_NAME"]);
                    PSUser.EmployeeId = CheckNulls.AgainstString(item["EMPLID"]);
                    PSUser.MiddleName = CheckNulls.AgainstString(item["MIDDLE_NAME"]);
                    PSUser.LastName = CheckNulls.AgainstString(item["LAST_NAME"]);
                    PSUser.DisplayName = CheckNulls.AgainstString(item["NAME"]);
                    PSUser.KnownAs = CheckNulls.AgainstString(item["NAME"]);
                    PSUser.FullTimeEmployee = "F" == CheckNulls.AgainstString(item["FULL_PART_TIME"]).ToUpper().Trim() ? true : false;
                    PSUser.EmployeeType = (bool)PSUser.FullTimeEmployee ? EmployeeType.Permanent : EmployeeType.Contractor;
                    PSUser.HomephoneNumber = CheckNulls.AgainstString(item["HOME_PHONE"]);
                    PSUser.TelephoneNumber = CheckNulls.AgainstString(item["NUM1"]);
                    PSUser.MobileNumber = CheckNulls.AgainstString(item["NUM2"]);
                    PSUser.PostalAddress = CheckNulls.AgainstString(item["ADDRESS1"]) + ", " + CheckNulls.AgainstString(item["ADDRESS2"]) + ", " +
                                            CheckNulls.AgainstString(item["ADDRESS3"]) + ", " + CheckNulls.AgainstString(item["ADDRESS4"]);
                    PSUser.PostalCode = CheckNulls.AgainstString(item["POSTAL"]);
                    PSUser.JoinDate = CheckNulls.AgainstNullableDatetime(item["HIRE_DT"]);
                    PSUser.Ethinicity = CheckNulls.AgainstString(item["COUNTRY"]);
                    PSUser.Country = CheckNulls.AgainstString(item["COUNTRY"]);
                    PSUser.TerminationDate = CheckNulls.AgainstNullableDatetime(item["TERMINATION_DT"]);
                    PSUser.IsActive = (PSUser.TerminationDate.HasValue && PSUser.TerminationDate.Value.Date < DateTime.Now.Date) ? false : true;

                    string orgUnitPrefix = CheckNull.AgainstString(item["DEPTID"]);

                    if (orgUnitPrefix != null && orgUnitPrefix != "")
                    {
                        var Organisartion = NOrganisation.GetByOrgCode(orgUnitPrefix);
                        PSUser.NOrganisationId = (Organisartion != null) ? (int)Organisartion.Id : 0;
                    }
                    else
                    {
                        PSUser.NOrganisationId = 0;
                    }

                    listPSUsers.Add(PSUser);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }
            return listPSUsers;
        }

        private void UpdateEntitlementReconcilation()
        {

            try
            {
                IList<PeopleSoftUserInfo> dicUsers = PeopleSoftUserInfo.GetByEndpointId(this.EndPoint.Id);
                foreach (PeopleSoftUserInfo adsUserInfo in dicUsers)
                {
                    if (adsUserInfo.PeoplesoftRoles != null)
                    {
                        foreach (PeopleSoftRoleInfo entitlement in adsUserInfo.PeoplesoftRoles)
                        {
                            var acctRecon = AccountReconcilation.GetByUser(adsUserInfo.EndPointSystemId, adsUserInfo.Id);
                            var entRecon = EntitlementReconcilation.GetByUser(adsUserInfo.Id, adsUserInfo.EndPointSystemId, entitlement.ROLENAME.Trim(),
                                EntitlementType.Role);

                            if (entRecon != null)
                            {
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                var entitlements = (adsUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Group, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                entRecon.Entitlement = entitlement.ROLENAME.Trim();
                                EntitlementReconcilation.UpdateException(entRecon, true);
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                    }
                }

                EntitlementReconcilation.ProcessDeletedEntitlements(this.EndPoint.Id, this.Process, MasterConfigHelper.ServiceUser);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while updating Entitlement reconciliation.", ex);
            }
        }

        private EmployeeEventTrigger PopulateOffBoardTrigger(User user)
        {
            var evnt = EmployeeEvents.Resignation;
            var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, evnt, this.Process.LastRunOn.Value.Date);

            if (trigger != null)
            {
                trigger.EffectiveDate = user.TerminationDate.Value;
                trigger.Save(MasterConfigHelper.ServiceUser);
                return trigger;
            }
            else
            {
                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    EffectiveDate = DateTime.Now,
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true
                };
                return newTrigger;
            }
        }

        private EmployeeEventTrigger PopulateOnBoardTrigger(User user, bool isNewUser)
        {
            var evnt = EmployeeEvents.NewJoinee;
            var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, evnt, null);
            double onBoardDaysBeforeJoining = MasterConfigHelper.OnBoardDaysBeforeJoining;
            DateTime priorBoardingDate = user.JoinDate.HasValue ? user.JoinDate.Value.AddDays(-(onBoardDaysBeforeJoining)) : DateTime.Now;

            if (trigger != null)
            {
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
                    UserDetails = string.Empty,
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
                if (user.TerminationDate.Value.Date <= DateTime.Today && user.TerminationDate.Value.Date != Constants.SAP_MAX_DATE)
                {
                    var trigger = this.PopulateOffBoardTrigger(user);

                    if (trigger != null && trigger.Id == 0)
                    {
                        triggers.Add(trigger);
                    }
                }
            }
        }

        private void CreateTransferTriggers(long userId, DateTime effectiveDate)
        {
            var evnt = EmployeeEvents.OUChange;

            var trigger = EmployeeEventTrigger.GetByUserAndEvent(userId, evnt, null);
            var user = User.GetById(userId);
            var OldData = user;

            if (trigger != null)
            {
                var tempTrigger = EmployeeEventTrigger.GetByUserAndEvent(userId, EmployeeEvents.OUChange, DateTime.Now.AddMinutes(-10));
                if (tempTrigger != null && trigger.IsRequestCreated)
                {
                    return;
                }
                tempTrigger.EffectiveDate = effectiveDate;
                tempTrigger.OldUserDetails = UtilityMethods.Serialize<User>(OldData);
                tempTrigger.UserDetails = UtilityMethods.Serialize<User>(user);
                tempTrigger.Save(MasterConfigHelper.ServiceUser);
            }
            else
            {

                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true,
                    EffectiveDate = effectiveDate,
                    UserDetails = UtilityMethods.Serialize<User>(user),
                    OldUserDetails = UtilityMethods.Serialize<User>(OldData)

                };


                newTrigger.Save(MasterConfigHelper.ServiceUser);

            }
        }
        #endregion

        public bool CreateUser(IEndpointUserInfo _userInfo)
        {
            var PeoplesoftUser = _userInfo as PeopleSoftUserInfo;
            bool ReturnValue = true;
            try
            {
                PeopleSoftUserInfo PSUserInfo = (PeopleSoftUserInfo)_userInfo;

                //string CreatingQuery = string.Format(@"insert into PSOPRDEFN (OPRID, VERSION, OPRDEFNDESC, EMPLID, EMAILID, OPRCLASS, ROWSECCLASS,
                //    OPERPSWD, OPERPSWDSALT, ENCRYPTED , SYMBOLICID, LANGUAGE_CD, MULTILANG , CURRENCY_CD , LASTPSWDCHANGE, ACCTLOCK , 
                //    PRCSPRFLCLS, DEFAULTNAVHP, FAILEDLOGINS, EXPENT, OPRTYPE, USERIDALIAS, LASTUPDOPRID, PTALLOWSWITCHUSER)
                //        values ('{0}', 8 , '{2}', '{3}', '{4}', 'HCPPALL','HCDPALL','{5}','{7}',1 ,'SYSADM1' ,'ENG'  ,
                //    0 ,'USD' ,'{6}',0 ,'EPEX9000','HCSPNAVHP',0,0,0,'{1}','PPLSOFT',0)",
                //PSUserInfo.OPRID.Replace("'", "''"), PSUserInfo.USERIDALIAS.Replace("'", "''"), PSUserInfo.OPRDEFNDESC.Replace("'", "''"), PSUserInfo.EMPLID,
                //PSUserInfo.EMAILID, "{1}qWsATtDNqduEyuzHq59XKTU/Hos=", DateTime.Now.ToString("dd-MMM-yy"), "3qdf+Th7w9a8e4fnTBZDpw=="); //PSUserInfo.AccountPassword

                string CreateUserQuery = Query.CreateUserQuery;
                CreateUserQuery = CreateUserQuery.Replace("?OPRID", ":P_OPRID");
                CreateUserQuery = CreateUserQuery.Replace("?USERIDALIAS", ":P_USERIDALIAS");
                CreateUserQuery = CreateUserQuery.Replace("?OPRDEFNDESC", ":P_OPRDEFNDESC");
                CreateUserQuery = CreateUserQuery.Replace("?EMPLID", ":P_EMPLID");
                CreateUserQuery = CreateUserQuery.Replace("?EMAILID", ":P_EMAILID");
                CreateUserQuery = CreateUserQuery.Replace("?OPERPSWD", ":P_OPERPSWD");
                //CreateUserQuery = CreateUserQuery.Replace("DATETIME", "dd-MMM-yy");
                //CreateUserQuery = CreateUserQuery.Replace("?ACCOUNTPASSWORD", "3qdf+Th7w9a8e4fnTBZDpw==");
                CreateUserQuery = CreateUserQuery.Replace("?OPERPSWDSALT", "''");
                ReturnValue = (ReturnValue) ? putOracleQueryToDB(CreateUserQuery,
                    new OracleParameter("P_OPRID", PeoplesoftUser.OPRID),
                    new OracleParameter("P_USERIDALIAS", PeoplesoftUser.USERIDALIAS),
                    new OracleParameter("P_OPRDEFNDESC", PeoplesoftUser.OPRDEFNDESC),
                    new OracleParameter("P_EMPLID", PeoplesoftUser.EMPLID),
                    new OracleParameter("P_EMAILID", PeoplesoftUser.EMAILID),
                    new OracleParameter("P_OPERPSWD", PeoplesoftUser.AccountPassword)) : ReturnValue;

            }

            catch (Exception)
            {
                throw;
            }
            return ReturnValue;
        }
        
    }
}

