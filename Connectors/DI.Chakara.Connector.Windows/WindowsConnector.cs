
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Threading;
using System.DirectoryServices.AccountManagement;
using System.Management;
using System.DirectoryServices;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Connector
{
    public class WindowsConnector : DI.Chakra.Connector.ConnectorBase
    {

        private EndpointConfiguration m_winConfig = null;
        /// <summary>
        /// constructor
        /// </summary>
        public WindowsConnector(DomainModel.EndPoint _endpoint, AccessRequest currentRequest)
        {
            this.EndPoint = _endpoint;
            this.CurrentRequest = currentRequest;
            var adsConfig = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            adsConfig.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.KeyId == Convert.ToInt64(EndpointKey.Password))
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.WindowsConfiguraiton = adsConfig;
        }

        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicGroups = new Dictionary<long, string>();
        private Dictionary<string, ArrayList> dicGroupUsers = new Dictionary<string, ArrayList>();
        ManagementScope mgmtScope;

        private class folderDetailsCollection
        {
            public string folderIPPath { get; set; }
            public string folderDrivePath { get; set; }
            public string folderName { get; set; }
            public string resType { get; set; }
        }

        public EndpointConfiguration WindowsConfiguraiton
        {
            get
            {
                return m_winConfig;
            }

            set
            {
                this.m_winConfig = value;
            }
        }

        #region Connect
        public override bool Connect()
        {
            bool bReturn = false;
            try
            {
                mgmtScope = getMgmtScope();
                if (mgmtScope.IsConnected == true)
                {
                    bReturn = true;
                }
                return bReturn;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region GET DATA

        private string Configuration(EndpointKey Key)
        {
            var config = WindowsConfiguraiton.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public override void GetData()
        {
            try
            {

                long DiscoveryProcessId = base.DiscoveryProcess.Id;

                List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

                bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_WIN_User) != null;
                bool IsGroupSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_WIN_Group) != null;
                bool IsFolderSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_WIN_Folder) != null;

                if (IsUserSelected || IsGroupSelected)
                {
                    DiscoverUsers();
                    DiscoverGroups();
                    SaveUserGroups();
                    this.UpdateEntitlementRecon();
                }


                if (IsFolderSelected)
                {
                    // DiscoverFolders();
                }
                //WindowsUserInfo.MapWithMasterUser(EndPoint, WindowsConfiguraiton.MapAll != null ? WindowsConfiguraiton.MapAll : false);
                WindowsUserInfo.MapWithMasterUserForReconTable(EndPoint, WindowsConfiguraiton.MapAll != null ? WindowsConfiguraiton.MapAll : false);

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, ex.InnerException.ToString(), ex);

                throw;
            }

        }
        private void SaveUserGroups()
        {
            dicGroupUsers = new Dictionary<string, ArrayList>();

            try
            {

                foreach (var wingroupInfo in dicGroups)
                {
                    using (DirectoryEntry search = new DirectoryEntry("WinNT://" + Configuration(EndpointKey.IPAddress) + "/" + wingroupInfo.Value + ",group"))
                    {
                        ArrayList groupMembers = new ArrayList();
                        foreach (object member in (IEnumerable)search.Invoke("Members"))
                        {
                            DirectoryEntry memberEntry = new DirectoryEntry(member);
                            groupMembers.Add(memberEntry.Name);
                        }
                        dicGroupUsers.Add(wingroupInfo.Value, groupMembers);

                    }

                }

            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message);
            }

            WindowsUserInfo.SaveUserGroupMap(dicUsers, dicGroups, dicGroupUsers, this.EndPoint.Id);
        }
        private void DiscoverUsers()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Users"));
            IList<WindowsUserInfo> userlist = this.GetUsers();
            WindowsUserInfo.SaveUsers(userlist, EndPoint.Id, MasterConfigHelper.ServiceUser);
            userlist = null;
            userlist = WindowsUserInfo.GetDeletedUsers(this.EndPoint.Id, this.Process);
            WindowsUserInfo.RemoveWinUser(userlist.ToList());
            dicUsers = WindowsUserInfo.GetAllByEndPointSystemId(this.EndPoint.Id);
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Users"));
        }
        private void DiscoverGroups()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Groups"));

            IList<WindowsGroupInfo> listAdGroups = this.GetGroupInfos();
            WindowsGroupInfo.saveWinGroupInfo(listAdGroups.ToList(), EndPoint.Id, MasterConfigHelper.ServiceUser);
            dicGroups = WindowsGroupInfo.GetByEndpointId(this.EndPoint.Id);
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Groups"));
        }
        #endregion

        private IList<WindowsGroupInfo> GetGroupInfos()
        {
            IList<WindowsGroupInfo> list = new List<WindowsGroupInfo>();
            ArrayList members;
            WindowsGroupInfo winGroupInfo = null;
            GroupInfoOrchestrator orchestrator = new GroupInfoOrchestrator();
           
            orchestrator.EndpointSystemId = this.EndPoint.Id;

            using (DirectoryEntry DE = new DirectoryEntry("WinNT://" + Configuration(EndpointKey.IPAddress) + ",computer", Configuration(EndpointKey.UserName), Configuration(EndpointKey.Password)))
            {
                try
                {
                    foreach (DirectoryEntry deChild in DE.Children)
                    {
                        try
                        {
                            DirectoryEntry dirEntry = null;
                            if ((deChild.SchemaClassName == "Group"))
                            {
                                using (dirEntry = DE.Children.Find(deChild.Name, "Group"))
                                {
                                    DirectoryEntry obDirEntry = null;
                                    obDirEntry = new DirectoryEntry("WinNT://" + Configuration(EndpointKey.IPAddress) + "/" + dirEntry.Name);
                                    System.DirectoryServices.PropertyCollection coll = obDirEntry.Properties;

                                    members = new ArrayList();
                                    winGroupInfo = new WindowsGroupInfo();
                                    if (coll.Contains("objectSid"))
                                    {
                                        winGroupInfo.objectSid = ConvertByteToStringSid((byte[])coll["objectSid"].Value);
                                    }
                                    if (coll.Contains("groupType"))
                                    {
                                        winGroupInfo.groupType = Convert.ToString(coll["groupType"].Value);
                                    }
                                    if (coll.Contains("Name"))
                                    {
                                        winGroupInfo.Name = Convert.ToString(coll["Name"].Value);
                                    }
                                    if (coll.Contains("Description"))
                                    {
                                        winGroupInfo.Description = Convert.ToString(coll["Description"].Value);
                                    }
                                    winGroupInfo.Version = Convert.ToInt16(VersionNo.Available);
                                    winGroupInfo.EndPointSystemId = Convert.ToInt64(WindowsConfiguraiton.EndpointSystemId);
                                    list.Add(winGroupInfo);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message);
                }
                DE.Close();
            }
            return list;
        }
        private IList<WindowsUserInfo> GetUsers()
        {
            string sid = "";
            IList<WindowsUserInfo> list = new List<WindowsUserInfo>();
            WindowsUserInfo winUserInfo = null;
            UserInfoOrchestrator orchestrator = new UserInfoOrchestrator();
           
            orchestrator.EndpointSystemId = this.EndPoint.Id;
            try
            {
                using (DirectoryEntry DE = new DirectoryEntry("WinNT://" + Configuration(EndpointKey.IPAddress) + ",computer", Configuration(EndpointKey.UserName), Configuration(EndpointKey.Password)))
                {
                    try
                    {
                        foreach (DirectoryEntry deChild in DE.Children)
                        {
                            DirectoryEntry dirEntry = null;

                            if ((deChild.SchemaClassName == "User"))
                            {
                                using (dirEntry = DE.Children.Find(deChild.Name, "user"))
                                {
                                    DirectoryEntry obDirEntry = null;
                                    obDirEntry = new DirectoryEntry("WinNT://" + Configuration(EndpointKey.IPAddress) + "/" + dirEntry.Name);
                                    System.DirectoryServices.PropertyCollection coll = obDirEntry.Properties;
                                    winUserInfo = new WindowsUserInfo();
                                    if (coll.Contains("UserFlags"))
                                    {
                                        winUserInfo.UserFlags = Convert.ToString(coll["UserFlags"].Value);
                                    }

                                    if (coll.Contains("MaxStorage"))
                                    {
                                        winUserInfo.MaxStorage = Convert.ToString(coll["MaxStorage"].Value);
                                    }

                                    if (coll.Contains("PasswordAge"))
                                    {
                                        winUserInfo.PasswordAge = Convert.ToString(coll["PasswordAge"].Value);

                                    }

                                    if (coll.Contains("PasswordExpired"))
                                    {
                                        winUserInfo.PasswordExpired = Convert.ToString(coll["PasswordExpired"].Value);

                                    }
                                    if (coll.Contains("LoginHours"))
                                    {
                                        winUserInfo.LoginHours = ConvertByteToStringSid((byte[])coll["LoginHours"].Value); //Convert.ToString(coll["LoginHours"].Value);
                                    }
                                    if (coll.Contains("FullName"))
                                    {
                                        winUserInfo.FullName = Convert.ToString(coll["FullName"].Value);
                                    }
                                    if (coll.Contains("Description"))
                                    {
                                        winUserInfo.Description = Convert.ToString(coll["Description"].Value);
                                    }
                                    if (coll.Contains("BadPasswordAttempts"))
                                    {
                                        winUserInfo.BadPasswordAttempts = Convert.ToString(coll["BadPasswordAttempts"].Value);
                                    }
                                    if (coll.Contains("HomeDirectory"))
                                    {
                                        winUserInfo.HomeDirectory = Convert.ToString(coll["HomeDirectory"].Value);
                                    }
                                    if (coll.Contains("LoginScript"))
                                    {
                                        winUserInfo.LoginScript = Convert.ToString(coll["LoginScript"].Value);
                                    }
                                    if (coll.Contains("Profile"))
                                    {
                                        winUserInfo.Profile = Convert.ToString(coll["Profile"].Value);
                                    }
                                    if (coll.Contains("HomeDirDrive"))
                                    {
                                        winUserInfo.HomeDirDrive = Convert.ToString(coll["HomeDirDrive"].Value);
                                    }
                                    if (coll.Contains("Parameters"))
                                    {
                                        winUserInfo.Parameters = Convert.ToString(coll["Parameters"].Value);
                                    }
                                    if (coll.Contains("PrimaryGroupID"))
                                    {
                                        winUserInfo.PrimaryGroupID = Convert.ToString(coll["PrimaryGroupID"].Value);
                                    }
                                    if (coll.Contains("Name"))
                                    {
                                        winUserInfo.Name = Convert.ToString(coll["Name"].Value);
                                    }
                                    if (coll.Contains("MinPasswordLength"))
                                    {
                                        winUserInfo.MinPasswordLength = Convert.ToString(coll["MinPasswordLength"].Value);
                                    }
                                    if (coll.Contains("MaxPasswordAge"))
                                    {
                                        winUserInfo.MaxPasswordAge = Convert.ToString(coll["MaxPasswordAge"].Value);
                                    }
                                    if (coll.Contains("MinPasswordAge"))
                                    {
                                        winUserInfo.MinPasswordAge = Convert.ToString(coll["MinPasswordAge"].Value);
                                    }
                                    if (coll.Contains("PasswordHistoryLength"))
                                    {
                                        winUserInfo.PasswordHistoryLength = Convert.ToString(coll["PasswordHistoryLength"].Value);
                                    }
                                    if (coll.Contains("AutoUnlockInterval"))
                                    {
                                        winUserInfo.AutoUnlockInterval = Convert.ToString(coll["AutoUnlockInterval"].Value);
                                    }
                                    if (coll.Contains("LockoutObservationInterval"))
                                    {
                                        winUserInfo.LockoutObservationInterval = Convert.ToString(coll["LockoutObservationInterval"].Value);
                                    }
                                    if (coll.Contains("MaxBadPasswordsAllowed"))
                                    {
                                        winUserInfo.MaxBadPasswordsAllowed = Convert.ToString(coll["MaxBadPasswordsAllowed"].Value);
                                    }
                                    if (coll.Contains("objectSid"))
                                    {
                                        winUserInfo.objectSid = ConvertByteToStringSid((byte[])coll["objectSid"].Value);
                                    }
                                   // winUserInfo.Version = Convert.ToInt16(VersionNo.Available);
                                    list.Add(winUserInfo);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message);
                    }
                    DE.Close();
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message);
            }

            return list;
        }
        private string ConvertByteToStringSid(Byte[] sidBytes)
        {
            StringBuilder strSid = new StringBuilder();
            strSid.Append("S-");
            try
            {
                // Add SID revision.

                strSid.Append(sidBytes[0].ToString());
                // Next six bytes are SID authority value.

                if (sidBytes[6] != 0 || sidBytes[5] != 0)
                {
                    string strAuth = String.Format
                        ("0x{0:2x}{1:2x}{2:2x}{3:2x}{4:2x}{5:2x}",
                        (Int16)sidBytes[1],
                        (Int16)sidBytes[2],
                        (Int16)sidBytes[3],
                        (Int16)sidBytes[4],
                        (Int16)sidBytes[5],
                        (Int16)sidBytes[6]);
                    strSid.Append("-");
                    strSid.Append(strAuth);
                }
                else
                {
                    Int64 iVal = (Int32)(sidBytes[1]) +
                        (Int32)(sidBytes[2] << 8) +
                        (Int32)(sidBytes[3] << 16) +
                        (Int32)(sidBytes[4] << 24);
                    strSid.Append("-");
                    strSid.Append(iVal.ToString());
                }

                // Get sub authority count...

                int iSubCount = Convert.ToInt32(sidBytes[7]);
                int idxAuth = 0;
                for (int i = 0; i < iSubCount; i++)
                {
                    idxAuth = 8 + i * 4;
                    UInt32 iSubAuth = BitConverter.ToUInt32(sidBytes, idxAuth);
                    strSid.Append("-");
                    strSid.Append(iSubAuth.ToString());
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            return strSid.ToString();
        }
        private ManagementScope getMgmtScope()
        {


            ManagementScope Scope;
            string ComputerName = Configuration(EndpointKey.IPAddress);
            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                if (!ComputerName.Equals("localhost", StringComparison.OrdinalIgnoreCase) && !ComputerName.Equals(host.AddressList[2].ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    Scope = new ManagementScope(String.Format("\\\\{0}\\root\\CIMV2", ComputerName));
                    Scope.Options.Authority = "ntlmdomain:DOMAIN";
                    Scope.Options.Username = Configuration(EndpointKey.UserName); // @"senokopower\idamsadmin";
                    Scope.Options.Password = Configuration(EndpointKey.Password);// WindowsConfiguration.Password;  //@"pass@idams";
                }
                else
                {
                    Scope = new ManagementScope(String.Format("\\\\{0}\\root\\CIMV2", ComputerName), null);
                }

                Scope.Connect();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Scope;
        }
        public override void Disconnect()
        {
            throw new NotImplementedException();
        }
        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }

        private void UpdateEntitlementRecon()
        {
            try
            {
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    var winUserInfo = WindowsUserInfo.GetByIdIncludeADSGroups(item.Key);

                    if (winUserInfo.windowsGroups != null)
                    {
                        foreach (WindowsGroupInfo entitlement in winUserInfo.windowsGroups)
                        {
                            var acctRecon = AccountReconcilation.GetByUser(winUserInfo.EndPointSystemId, winUserInfo.Id);
                            var entRecon = EntitlementReconcilation.GetByUser(winUserInfo.Id, winUserInfo.EndPointSystemId, entitlement.Name.Trim(), EntitlementType.Group);

                            if (entRecon != null)
                            {
                                //entRecon.LastModifiedDate = DateTime.Now;
                                //entRecon.LastModifiedBy = "System";
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                var entitlements = (winUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Group,string.Join(",", entitlements!=null?entitlements.ToArray():new string[] {}));
                                entRecon.Entitlement = entitlement.Name.Trim();
                                entRecon.ExceptionType = ExceptionTypes.Added;
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

        public override IMProcessor IMProcessor
        {
            get
            {
                Windows.IMProcessor processor = new Windows.IMProcessor();
                processor.EndPoint = this.EndPoint;
                processor.WindowsConfiguraiton = this.WindowsConfiguraiton;
                return processor;
            }
        }
    }
}
