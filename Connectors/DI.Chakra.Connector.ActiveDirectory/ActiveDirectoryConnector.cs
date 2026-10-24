using DI.Chakra.Connector.ActiveDirectory;

using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace DI.Chakra.Connector
{
    public enum objectClass
    {
        user, group, computer
    }
    public enum returnType
    {
        distinguishedName, ObjectGUID
    }

    public class ActiveDirectoryConnector : ConnectorBase
    {

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        protected readonly IdmsLogger _logger;
        #region Members 

        #region private

        private const string OBJECTEXISTS = "The Object Already Exists";
        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicGroups = new Dictionary<long, string>();
        private Dictionary<string, ArrayList> dicGroupUsers = new Dictionary<string, ArrayList>();
        private DataTable dtWhole = new DataTable();
        private const Int64 iSeconds = 864000000000;
        private static Int64 MAXPWDAGE = Int64.MaxValue;
        private DirectoryEntry m_root = null;
        private DirectorySearcher m_adSearch = null;
        private string IPAddress = string.Empty;
        private bool IsSecondaryIPChecking = false;

        #endregion

        #region public
        public IList<ADSComputerGroup> ComputerGroups { get; set; }
        IList<SearchResultCollection> srList = new List<SearchResultCollection>();

        #endregion

        #endregion

        #region Discovery
        readonly string MasterConfigHelperServiceUser;
        public ActiveDirectoryConnector(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
        {
            this.Endpoint = endpoint;
            this.CurrentRequest = currentRequest;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            this.IPAddress = Convert.ToString(Configuration(EndpointKey.IPAddress));
            _logger = logger;
            MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }
        public override bool IsActiveAccount(IEndpointUserInfo _userInfo)
        {
            ADSUserInfo ADSUserInfo = (ADSUserInfo)_userInfo;
            try
            {
                if (this.Connect())
                {

                    using (DirectoryEntry dir = GetDirectoryEntry(objectClass.user, ADSUserInfo.sAMAccountName, string.Empty))
                    {

                        int userControl = (int)dir.Properties["userAccountControl"].Value;
                        int CheckIfUserIsDisabled = userControl & (int)ADUserControl.ACCOUNTDISABLE;

                        if (ADAccount.IsAccountDisabled(dir))
                        {
                            UnLockChangePasswordAPIMessage = $"{ADSUserInfo.sAMAccountName} account is disabled in {this.Endpoint.DisplayName}. Please contact IAM Team.";
                            _logger.WriteLog(LogLevelType.Info, ADSUserInfo.sAMAccountName + " account is disabled in " + this.Endpoint.DisplayName);
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Exception, $"Error while checking the account {ADSUserInfo.sAMAccountName} is active or not", ex);
                return false;
            }
        }
        public bool IsAccountLocked(IEndpointUserInfo _userInfo)
        {
            ADSUserInfo ADSUserInfo = (ADSUserInfo)_userInfo;
            try
            {
                if (this.Connect())
                {
                    using (DirectoryEntry dir = GetDirectoryEntry(objectClass.user, ADSUserInfo.sAMAccountName, string.Empty))
                    {
                        if (ADAccount.IsAccountLocked(dir))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Exception, string.Format("Error while checking whether the account {0} is in Locked state.", ADSUserInfo.sAMAccountName), ex);
                return false;
            }
        }
        DirectoryEntry GetDirectoryEntry(objectClass objectCls, string objectName, string LdapDomain)
        {
            DirectorySearcher mySearcher = new DirectorySearcher(m_root);

            switch (objectCls)
            {
                case objectClass.user:
                    mySearcher.Filter = "(&(objectClass=user)(|(cn=" + objectName.ToSecureLDAPFilter() + ")(sAMAccountName=" + objectName.ToSecureLDAPFilter() + ")))";
                    break;
                case objectClass.group:
                    mySearcher.Filter = "(&(objectClass=group)(|(cn=" + objectName.ToSecureLDAPFilter() + ")(dn=" + objectName.ToSecureLDAPFilter() + ")))";
                    break;
                case objectClass.computer:
                    mySearcher.Filter = "(&(objectClass=computer)(|(cn=" + objectName.ToSecureLDAPFilter() + ")(dn=" + objectName.ToSecureLDAPFilter() + ")))";
                    break;
            }
            SearchResult result = mySearcher.FindOne();

            if (result == null)
            {
                throw new NullReferenceException
                ("unable to locate the distinguishedName for the object " +
                objectName + " in the " + LdapDomain + " domain");
            }
            DirectoryEntry directoryObject = result.GetDirectoryEntry();
            return directoryObject;
        }
        public override bool Connect()
        {
            bool isSucc = connectwithoutlog();
            if (isSucc)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Active Directory {0} Connected", this.IPAddress));
            }
            else
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Failed to connect Active Directory {0}", this.IPAddress));
            }

            return isSucc;
        }

        private bool connectwithoutlog()
        {
            bool bReturn = false;
            List<string> lstDomain = null;
            try
            {
                m_root = new DirectoryEntry();
                if (ADHelper.Configuration(EndpointKey.DomainName, this.EndpointConfiguration) != string.Empty)
                {
                    lstDomain = ADHelper.Configuration(EndpointKey.DomainName, this.EndpointConfiguration).Split(new char[] { '.' }).ToList();
                }
                StringBuilder bldr = new StringBuilder();
                int index = 0;
                string domainString = string.Empty;

                if (!string.IsNullOrEmpty(ADHelper.Configuration(EndpointKey.Ou, this.EndpointConfiguration)))
                    bldr.AppendFormat("OU={0},", Configuration(EndpointKey.Ou));

                foreach (string str in lstDomain)
                {
                    bldr.AppendFormat("DC={0},", str);
                    index = index + 1;
                    if (lstDomain.Count == index)
                        domainString = bldr.ToString().Remove(bldr.ToString().Length - 1);
                }

                m_root.Path = string.Format("LDAP://{0}/{1}", this.IPAddress, domainString);
                m_root.Username = ADHelper.Configuration(EndpointKey.UserName, this.EndpointConfiguration);
                m_root.Password = ADHelper.Configuration(EndpointKey.Password, this.EndpointConfiguration);
                m_adSearch = new DirectorySearcher(m_root);
                m_adSearch.Filter = (" (objectClass=" + "user" + ")");

                if (null != m_adSearch.FindOne())
                    bReturn = true;

            }
            catch (DirectoryServicesCOMException de)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Could not connect to AD {0}{2} with User : {1},Password:", this.IPAddress, ADHelper.Configuration(EndpointKey.UserName, this.EndpointConfiguration), m_root.Path), de);

                bReturn = IsSecondaryIPChecking ? false : ConnectSecondaryIP();
            }
            catch (System.Runtime.InteropServices.COMException ce)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Could not connect to AD {0}{2} with User : {1},Password:", this.IPAddress, ADHelper.Configuration(EndpointKey.UserName, this.EndpointConfiguration), m_root.Path), ce);

                bReturn = IsSecondaryIPChecking ? false : ConnectSecondaryIP();
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Could not connect to AD {0}{2} with User : {1},Password:", this.IPAddress, ADHelper.Configuration(EndpointKey.UserName, this.EndpointConfiguration), m_root.Path), ex);

                bReturn = IsSecondaryIPChecking ? false : ConnectSecondaryIP();
            }

            return bReturn;
        }

        private bool ConnectSecondaryIP()
        {
            this.IsSecondaryIPChecking = true;
            bool bReturn = false;
            string secondaryIpAddress = Convert.ToString(Configuration(EndpointKey.SecondaryIPAddress));
            string[] IPAddresses = secondaryIpAddress.Split(',');

            if (IPAddresses.Count() > 0)
            {
                foreach (string secondIp in IPAddresses)
                {

                    if (!bReturn)
                    {
                        if (string.IsNullOrEmpty(secondIp))
                            continue;
                        this.IPAddress = Convert.ToString(secondIp);
                        bReturn = this.Connect();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                bReturn = false;
            }

            return bReturn;
        }
        public override void Disconnect()
        {

        }
        public override void GetData()
        {
            try
            {
                long DiscoveryProcessId = base.DiscoveryProcess.Id;
                List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);
                bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_AD_User) != null;
                bool IsGroupSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_AD_Group) != null;
                bool IsOuSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_AD_OU) != null;
                bool IsComputerSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_AD_Computer) != null;
                bool IsPrinterSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_AD_Printer) != null;
                bool IsFolderSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_AD_Folder) != null;

                if (IsUserSelected || IsGroupSelected)
                {
                    DiscoverUsers();
                    //if (!(Convert.ToBoolean(ADHelper.Configuration(EndpointKey.IsWOG, this.EndpointConfiguration))))
                    {
                        DiscoverGroups(Process.Id);
                        GetListOfAdUsersByGroup();
                        SaveUserGroups();
                        //Update recon table
                        this.UpdateEntitlementRecon();
                    }
                }

                if (IsOuSelected)
                {
                    DiscoverOUs();
                }

                if (IsComputerSelected)
                {
                    DiscoverComputers();

                }
                if (IsPrinterSelected)
                {
                    DiscoverPrinters();
                }

                if (IsFolderSelected)
                {
                    DiscoverFolders();
                }
                //ADSUserInfo.MapWithMasterUser(EndPoint, ActiveDirectoryConfiguration.MapAll != null ? ActiveDirectoryConfiguration.MapAll : false);
                ADSUserInfo.MapWithMasterUserForReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
                ADSUserInfo.MapWithMasterUserForEntitlementReconTable(Endpoint, EndpointConfiguration.MapAll != null ? EndpointConfiguration.MapAll : false, DiscoveryProcessId);
                ADSUserInfo.PopulateMasterUserFromAD(Endpoint);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, ex.InnerException.ToString(), ex);
                throw;
            }
            finally
            {
                dicUsers?.Clear();
                dicUsers = null;
                dicGroups?.Clear();
                dicGroups = null;
                try
                {
                    m_adSearch?.Dispose();
                    m_adSearch = null;

                    m_root?.Dispose();
                    m_root = null;
                    GC.Collect();
                }
                catch
                {
                }
            }
        }
        public void GetListOfAdUsersByGroup()
        {

            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering GetListOfAdUsersByGroup"));

                m_adSearch = new DirectorySearcher(m_root);

                m_adSearch.PageSize = 2000;

                string query = string.Format("(objectCategory={0})", ADObjectCategory.Group);

                m_adSearch.Filter = query;

                using (System.DirectoryServices.SearchResultCollection mySearchResultColl = m_adSearch.FindAll())
                {
                    foreach (SearchResult result in mySearchResultColl)
                    {
                        using (DirectoryEntry de = result.GetDirectoryEntry())
                        {
                            int count = de.Properties["member"].Count;
                            var membersList = ADHelper.GetProperty_Array(de, "member");

                            var ADGroupObjectSId = ADHelper.GetProperty_SID(de, ADProperties.objectSid);
                            var ADGroupName = ADHelper.GetProperty(de, ADProperties.distinguishedName);

                            const int increment = 999;
                            var groupEntry = de;
                            int from = 0;
                            ArrayList mem = new ArrayList();
                            ArrayList objectIdList = new ArrayList();
                            string result2 = string.Empty;
                            for (int i = 0; i < membersList.Count; ++i)
                            {
                                string cn = membersList[i].ToString();
                                string cnValue = "";
                                string cnPattern = @"^CN=(?<cn>.+?)(?<!\\),";
                                var match = Regex.Match(cn, cnPattern);
                                //string objectId = strArr[0].ToString();
                                if (match.Success)
                                {
                                    cnValue = match.Groups[1].Value;
                                }

                                if (!string.IsNullOrEmpty(cnValue))
                                {
                                    if (cn.Contains("=ForeignSecurityPrincipals,"))
                                    {
                                        objectIdList.Add(cnValue);
                                    }
                                    else
                                    {
                                        using (DirectoryEntry entry = ADHelper.GetDirectoryEntries(cnValue, m_root))
                                        {
                                            if (entry != null)
                                            {
                                                string objectsId = ADHelper.GetProperty_SID(entry, ADProperties.objectSid);
                                                objectIdList.Add(objectsId);
                                            }
                                        }
                                    }
                                }
                            }
                            if (membersList.Count >= 1500)
                            {
                                while (true)
                                {
                                    // End of the range.
                                    int to = from + increment - 1;

                                    // Attach a range option to the properties to load,
                                    // for example: range=0-999.
                                    var properties = new[] { string.Format("member;range={0}-{1}", @from, to) };

                                    // Perform a search using the group entry as the base.
                                    var filter = "(objectClass=*)";

                                    var memberSearcher = new DirectorySearcher(groupEntry, filter, properties, SearchScope.Subtree);

                                    using (memberSearcher)
                                    {
                                        try
                                        {
                                            using (SearchResultCollection memberResults = memberSearcher.FindAll())
                                            {
                                                foreach (SearchResult memberResult in memberResults)
                                                {
                                                    var membersProperties = memberResult.Properties;
                                                    var membersPropertyNames = membersProperties.PropertyNames.OfType<string>().Where(n => n.StartsWith("member;"));

                                                    foreach (var propertyName in membersPropertyNames)
                                                    {
                                                        // Get all members from the ranged result.
                                                        var members = membersProperties[propertyName];

                                                        foreach (string memberDn in members)
                                                        {
                                                            string cn = memberDn;
                                                            string cnValue = "";
                                                            string cnPattern = @"^CN=(?<cn>.+?)(?<!\\),";
                                                            var match = Regex.Match(cn, cnPattern);
                                                            if (match.Success)
                                                            {
                                                                cnValue = match.Groups[1].Value;
                                                            }
                                                            if (!string.IsNullOrEmpty(cnValue))
                                                            {
                                                                if (cn.Contains("=ForeignSecurityPrincipals,"))
                                                                {
                                                                    mem.Add(cnValue);
                                                                }
                                                                else
                                                                {
                                                                    DirectoryEntry entry = ADHelper.GetDirectoryEntries(cnValue, m_root);
                                                                    if (entry != null)
                                                                    {
                                                                        string objectsId = ADHelper.GetProperty_SID(entry, ADProperties.objectSid);
                                                                        mem.Add(objectsId);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (DirectoryServicesCOMException ex)
                                        {
                                            WriteDiscoveryLog(LogLevelType.Info, $"DirectoryServicesCOMException Exception occured in GetListOfAdUsersByGroup. Message:{ ex.Message} Extended ErrorMessage:{ex.ExtendedErrorMessage} for Group:{ADGroupName}");
                                            // When the start of the range exceeds the number
                                            // of available results, an exception is thrown
                                            // and we exit the loop.
                                            break;
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteDiscoveryLog(LogLevelType.Info, $"DirectoryServicesCOMException Exception occured in GetListOfAdUsersByGroup. Message:{ ex.Message} for Group:{ADGroupName}");
                                            break;
                                        }
                                    }
                                    // Increment for the next range.
                                    from += increment;
                                }
                                dicGroupUsers.Add(ADGroupObjectSId, mem);
                            }
                            if (!dicGroupUsers.ContainsKey(ADGroupObjectSId))
                                dicGroupUsers.Add(ADGroupObjectSId, objectIdList);
                        }
                    }
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed GetListOfAdUsersByGroup"));
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, $"GetListOfAdUsersByGroup failed.Message: { ex.Message}");
            }
        }
        private void DiscoverGroups(long processId)
        {
            IList<ADSGroupInfo> listAdGroups = new List<ADSGroupInfo>();
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Groups"), DiscoveryObjectType.Group);
                this.GetADGroups();
                listAdGroups = ADSGroupInfo.GetDeletedByEndPointSystemId(this.Endpoint.Id, this.DiscoveryProcess.Id);
                WriteDiscoveryLog(LogLevelType.DebugStep, $"ADGroup Delete list: {listAdGroups.Count}", DiscoveryObjectType.Group);
                if (listAdGroups != null && listAdGroups.Any())
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Delete Groups started.Total AdGroups:{0}", listAdGroups.Count), DiscoveryObjectType.Group);
                    var Ids = listAdGroups.Select(i => i.Id).Distinct().ToList();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Deleting from UserGroup started.Total AdGroups:{0}", Ids.Count), DiscoveryObjectType.Group);
                    ADSUserInfo.RemoveADGroupFromUserGroup(Ids);
                    Ids?.Clear();
                    Ids = null;
                    WriteDiscoveryLog(LogLevelType.Info, "Deleting from UserGroup completed.", DiscoveryObjectType.Group);
                    ADSGroupInfo.DeleteList(listAdGroups.ToList());
                    WriteDiscoveryLog(LogLevelType.Info, "Delete Groups completed", DiscoveryObjectType.Group);
                }
                dicGroups = ADSGroupInfo.GetByEndpointId(this.Endpoint.Id);
                this.FindNestedGroups();
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Groups"), DiscoveryObjectType.Group);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, $"Deleting from UserGroup failed. {ex.StackTrace}", DiscoveryObjectType.Group, ex);
            }
            finally
            {
                listAdGroups?.Clear();
                listAdGroups = null;
            }
        }

        private void FindNestedGroups()
        {
            try
            {
                IList<ADSGroupInfo> list = new List<ADSGroupInfo>();
                ADSGroupInfo adsInfo = null;
                DirectoryEntry de = null;
                m_adSearch.Filter = string.Format("(objectCategory={0})", ADObjectCategory.Group);
                m_adSearch.PageSize = 1000;
                GroupInfoOrchestrator orchestrator = new GroupInfoOrchestrator();
                SearchResultCollection seCol = m_adSearch.FindAll();
                orchestrator.EndpointSystemId = (int)this.Endpoint.Id;
                foreach (SearchResult result in seCol)
                {

                    adsInfo = new ADSGroupInfo();
                    de = result.GetDirectoryEntry();
                    adsInfo = orchestrator.PopulateNestedGroup(de, m_adSearch);
                    if (null != adsInfo && adsInfo.memberOf.Count() > 0)
                    {

                        list.Add(adsInfo);
                    }
                }
                if (list.Count > 0)
                {
                    ADSNestedGroup.SaveNestedGroups(list, MasterConfigHelperServiceUser, this.Endpoint.Id);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveUserGroups()
        {
            ADSUserInfo.SaveUserGroupMap(dicUsers, dicGroups, dicGroupUsers, this.Endpoint.Id);
        }
        private void UpdateEntitlementRecon()
        {
            List<AccountReconcilation> exsitngAccountrecon = null;
            List<EntitlementReconcilation> existingEntilements = null;
            try
            {

                exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(this.Endpoint.Id).ToList();
                existingEntilements = EntitlementReconcilation.GetAllByEndpointSystemId(this.Endpoint.Id);
                AccountReconcilation acctRecon = null;
                EntitlementReconcilation entRecon = null;
                // List<ADSUserInfo> listAdsUserInfo = ADSUserInfo.GetAllEndPointSystemId(this.EndPoint.Id);

                ADSUserInfo adsUserInfo = null;
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    adsUserInfo = null;
                    try
                    {
                        adsUserInfo = ADSUserInfo.GetByIdIncludeADSGroups(item.Key);
                        // adsUserInfo = listAdsUserInfo.Where(a => a.Id == item.Key).FirstOrDefault();

                        if (adsUserInfo != null && adsUserInfo.ADSGroups != null)
                        {
                            foreach (ADSGroupInfo entitlement in adsUserInfo.ADSGroups)
                            {
                                // acctRecon = AccountReconcilation.GetByUser(adsUserInfo.EndPointSystemId, adsUserInfo.Id);
                                acctRecon = exsitngAccountrecon.Where(a => a.EndpointUserId == adsUserInfo.Id).FirstOrDefault();
                                if (acctRecon == null) continue;

                                //entRecon = EntitlementReconcilation.GetByUser(adsUserInfo.Id, adsUserInfo.EndPointSystemId, entitlement.name.Trim(), EntitlementType.Group);

                                entRecon = existingEntilements.Where(e => e.EndpointUserId == adsUserInfo.Id &&
                                                                            e.Entitlement == Convert.ToString(entitlement.name).Trim() &&
                                                                            e.EntitlementType == (int)EntitlementType.Group).FirstOrDefault();

                                if (entRecon == null)
                                {
                                    entRecon = new EntitlementReconcilation();
                                    var entitlements = (adsUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                    entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Group, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                    entRecon.Entitlement = entitlement.name.Trim();
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = entitlement.description;
                                    EntitlementReconcilation.UpdateException(entRecon, true);
                                    entRecon.CreatedDate = DateTime.Now;
                                    entRecon.CreatedBy = "System";
                                    entRecon.LastAttestedOn = null;
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.Save(MasterConfigHelperServiceUser);
                                    existingEntilements.Add(entRecon);
                                }
                                else
                                {
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.LastModifiedDate = DateTime.Now;
                                    entRecon.LastModifiedBy = "System";
                                    entRecon.Entitlement = entitlement.name.Trim();
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = entitlement.description;

                                    if (entRecon.IsDeleted)
                                    {
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                    }
                                    entRecon.Save(MasterConfigHelperServiceUser);
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while updating Entitlement reconciliation. ADS SAMAccount -{0}", adsUserInfo.sAMAccountName), ex);
                    }
                }

                try
                {
                    EntitlementReconcilation.ProcessDeletedEntitlements(this.Endpoint.Id, this.Process, MasterConfigHelperServiceUser);
                }
                catch (Exception ex)
                {
                    this.WriteDiscoveryLog(LogLevelType.Exception, "Error while ProcessDelete Entitlements Recon.", ex);
                }
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Process Entitlements Recon.", ex);
            }
            finally
            {
                exsitngAccountrecon?.Clear();
                exsitngAccountrecon = null;

                existingEntilements?.Clear();
                existingEntilements = null;
            }
        }

        #region Discover Objects

        private void DiscoverUsers()
        {
            IList<ADSUserInfo> userlist = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Users"));
                userlist = this.GetUsers();
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Discovered {0} Users", userlist.Count));
                bool isOneWayTrust = false;
                var isOnewayConfigured = ADHelper.Configuration(EndpointKey.IsOneWayTrust, this.EndpointConfiguration);
                if (!string.IsNullOrEmpty(isOnewayConfigured))
                    isOneWayTrust = Convert.ToBoolean(isOnewayConfigured);
                if (isOneWayTrust)
                    userlist = userlist.Concat(this.GetWOGUsers()).ToList();

                ADSUserInfo.SaveUsers(userlist, Endpoint.Id, MasterConfigHelperServiceUser);
                userlist = null;

                AccountReconcilation.ProcessDeletedAccounts(this.Endpoint.Id, this.Process, MasterConfigHelperServiceUser);
                userlist = ADSUserInfo.GetDeletedUsers(this.Endpoint.Id, this.Process);
                ADSUserInfo.RemoveADsUser(userlist.ToList());

                dicUsers = ADSUserInfo.GetAllByEndPointSystemId(this.Endpoint.Id);
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Users"));
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while Discovery Users.", ex);
                throw ex;
            }
            finally
            {
                userlist?.Clear();
                userlist = null;
            }
        }

        private IList<ADSUserInfo> GetWOGUsers()
        {
            var trustedAD = Convert.ToString(ADHelper.Configuration(EndpointKey.TrustSource, this.EndpointConfiguration));
            if (string.IsNullOrEmpty(trustedAD)) return new List<ADSUserInfo>();
            long endpointSystemId = EndPoint.GetByApplicationCode(trustedAD, this.Endpoint.EndpointSystemTypeId.Value).Id;
            if (endpointSystemId == 0) return new List<ADSUserInfo>();
            var domainName = EndpointConfigurationDetail.GetKeyValue(endpointSystemId, (long)EndpointKey.DomainName);
            if (string.IsNullOrEmpty(domainName)) domainName = "WOG";
            IList<ADSUserInfo> adsWogUserLst = ADSUserInfo.GetAll().Where(x => x.EndPointSystemId == endpointSystemId).ToList();
            foreach (ADSUserInfo wogUser in adsWogUserLst)
            {
                wogUser.sAMAccountName = string.Format("{0}\\{1}", domainName.Split('.')[0], wogUser.sAMAccountName);
                wogUser.WogUserId = wogUser.Id;
                wogUser.Id = 0;
                wogUser.EndPointSystemId = Endpoint.Id;
                wogUser.EndPointSystem = EndPoint.GetById(Endpoint.Id);
            }
            return adsWogUserLst;
        }

        private IList<ADSUserInfo> GetUsers()
        {
            try
            {
                IList<ADSUserInfo> list = new List<ADSUserInfo>();
                ADSUserInfo adsUserInfo = null;
                DirectoryEntry de = null;
                m_adSearch.Filter = string.Format("(objectCategory={0})", ADObjectCategory.User);
                m_adSearch.PageSize = 1000;

                UserInfoOrchestrator orchestrator = new UserInfoOrchestrator();

                orchestrator.EndpointSystemId = (int)this.Endpoint.Id;
                using (SearchResultCollection seCol = m_adSearch.FindAll())
                {
                    WriteDiscoveryLog(LogLevelType.Info, "Loading latest Login information from All Domain Controllers");
                    MAXPWDAGE = this.MaxPasswordAgeForDomain();
                    dtWhole = LoadLastLogonForAllDC();
                    WriteDiscoveryLog(LogLevelType.Info, "Completed latest Last Login information from All Domain Controllers");

                    foreach (SearchResult result in seCol)
                    {
                        adsUserInfo = new ADSUserInfo();
                        de = result.GetDirectoryEntry();
                        adsUserInfo = orchestrator.PopulateUser(de, m_adSearch);

                        if (adsUserInfo != null)
                        {
                            int index = adsUserInfo.distinguishedName.IndexOf("OU=");
                            if (index != -1)
                            {
                                adsUserInfo.ou = adsUserInfo.distinguishedName.Substring(index, adsUserInfo.distinguishedName.Length - index);
                            }

                            if (dtWhole.Rows.Count != 0)
                                adsUserInfo.lastLogon = this.getLatestLastLoginFromAllDC(adsUserInfo.sAMAccountName);

                            if (adsUserInfo.pwdLastSet.HasValue)
                            {
                                adsUserInfo.PasswordExpirationDate = adsUserInfo.pwdLastSet.Value.AddDays(MAXPWDAGE);
                            }
                            if (null != adsUserInfo)
                                list.Add(adsUserInfo);
                        }
                    }
                    return list;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Int64 MaxPasswordAgeForDomain()
        {
            try
            {
                return Math.Abs(ADHelper.GetInt64FromLargeInteger(m_root.Properties["maxPwdAge"].Value) / iSeconds);
            }
            catch (Exception) { throw; }
        }
        public DataTable LoadLastLogonForAllDC()
        {
            srList = new List<SearchResultCollection>();

            DataTable results = new DataTable();
            results.Columns.Add("sAMAccountName");
            results.Columns.Add("lastLogon");

            DirectoryContext context = new DirectoryContext(DirectoryContextType.Domain,
               ADHelper.Configuration(EndpointKey.DomainName, this.EndpointConfiguration), ADHelper.Configuration(EndpointKey.UserName, this.EndpointConfiguration), ADHelper.Configuration(EndpointKey.Password, this.EndpointConfiguration));

            DateTime latestLogon = DateTime.MinValue;

            DomainControllerCollection dcc =
              DomainController.FindAll(context);
            string sam = string.Empty;
            foreach (DomainController dc in dcc)
            {
                DirectorySearcher ds;

                using (dc)
                using (ds = dc.GetDirectorySearcher())
                {
                    ds.Filter = string.Format("(objectCategory={0})", "User");
                    //ds.Filter = string.Format("(&(sAMAccountName={0})(lastLogon=*)(objectCategory=User))", sam);
                    ds.PropertiesToLoad.Add(ADProperties.sAMAccountName);
                    ds.PropertiesToLoad.Add(ADProperties.lastLogon);
                    ds.PropertiesToLoad.Add(ADProperties.lastLogonTimestamp);
                    ds.PageSize = 1000;

                    SearchResultCollection seCol = null;
                    try
                    {
                        seCol = ds.FindAll();
                        foreach (SearchResult sr in seCol)
                        {
                            try
                            {
                                if (sr.Properties.Contains(ADProperties.sAMAccountName) &&
                                    (sr.Properties.Contains(ADProperties.lastLogon) || sr.Properties.Contains(ADProperties.lastLogonTimestamp)))
                                {

                                    DataRow rw = results.NewRow();
                                    rw[ADProperties.sAMAccountName] = sr.Properties[ADProperties.sAMAccountName][0].ToString();
                                    var lastLogon = DateTime.MinValue;

                                    if (sr.Properties.Contains(ADProperties.lastLogon))
                                    {
                                        lastLogon = DateTime.FromFileTime((long)sr.Properties[ADProperties.lastLogon][0]);
                                    }

                                    if (lastLogon == DateTime.MinValue && sr.Properties.Contains(ADProperties.lastLogonTimestamp))
                                    {
                                        lastLogon = DateTime.FromFileTime((long)sr.Properties[ADProperties.lastLogonTimestamp][0]);
                                    }

                                    rw["lastLogon"] = lastLogon;

                                    results.Rows.Add(rw);
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }
            return results;
        }
        private DateTime? getLatestLastLoginFromAllDC(string sAMAccountName)
        {
            DateTime latestLogon = DateTime.MinValue;
            string filter = string.Format("sAMAccountName='{0}'", sAMAccountName.Replace("'", "''"));
            DataRow[] dRows = dtWhole.Select(filter);

            foreach (DataRow dRow in dRows)
            {
                DateTime lastLogon = DateTime.MinValue;
                lastLogon = Convert.ToDateTime(dRow["lastLogon"]);
                if (DateTime.Compare(lastLogon, latestLogon) > 0)
                {
                    latestLogon = lastLogon;
                }
            }

            if (latestLogon == DateTime.MinValue)
                return null;

            if (latestLogon.CompareTo(DateTime.Today.AddYears(-30)) < 1)
                return null;

            return latestLogon;
        }



        private void GetADGroups()
        {
            IList<ADSGroupInfo> list = null;
            IList<ADSGroupInfo> listExistingADgroups = null;
            try
            {
                list = new List<ADSGroupInfo>();
                ADSGroupInfo adsInfo = null;
                //DirectoryEntry de = null;

                m_adSearch.Filter = string.Format("(objectCategory={0})", ADObjectCategory.Group);
                m_adSearch.PageSize = 1000;

                GroupInfoOrchestrator orchestrator = new GroupInfoOrchestrator();
                using (SearchResultCollection seCol = m_adSearch.FindAll())
                {
                    orchestrator.EndpointSystemId = (int)this.Endpoint.Id;
                    foreach (SearchResult result in seCol)
                    {
                        adsInfo = new ADSGroupInfo();
                        using (DirectoryEntry de = result.GetDirectoryEntry())
                        {
                            adsInfo = orchestrator.PopulateGroupLive(de, m_adSearch);
                            if (null != adsInfo)
                                list.Add(adsInfo);
                        }
                    }
                    if (list.Count > 0)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("Discovered {0} Groups", list.Count), DiscoveryObjectType.Group);
                        listExistingADgroups = new List<ADSGroupInfo>();
                        listExistingADgroups = ADSGroupInfo.GetAllByEndpointSystemId(this.Endpoint.Id);
                        WriteDiscoveryLog(LogLevelType.Info, $"Exisiting Groups :{listExistingADgroups.Count}", DiscoveryObjectType.Group);
                        foreach (ADSGroupInfo grp in list)
                        {
                            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Group:{grp.sAMAccountName}");
                            ADSGroupInfo adexistingGroups = null;
                            try
                            {
                                adexistingGroups = listExistingADgroups.Where(t => t.sAMAccountName == grp.sAMAccountName).FirstOrDefault();
                                if (adexistingGroups == null)
                                {
                                    grp.CanRequest = true;
                                    grp.Save(MasterConfigHelperServiceUser);
                                }
                                else
                                {
                                    grp.Id = adexistingGroups.Id;
                                    grp.CanRequest = adexistingGroups.CanRequest;
                                    grp.UserDefinedDesc = adexistingGroups.UserDefinedDesc;
                                    grp.CreatedBy = adexistingGroups.CreatedBy;
                                    grp.CreatedDate = adexistingGroups.CreatedDate;
                                    grp.Save(MasterConfigHelperServiceUser);
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteDiscoveryLog(LogLevelType.Exception, $"Save Groups failed for GroupName:{grp.sAMAccountName}", DiscoveryObjectType.Group, ex);
                                throw ex;
                            }
                        }
                        WriteDiscoveryLog(LogLevelType.DebugStep, $"Saving AD Groups completed.", DiscoveryObjectType.Group);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, $"GetADGroups failed.{ex.Message}", DiscoveryObjectType.Group, ex);
                throw ex;
            }
            finally
            {
                listExistingADgroups?.Clear();
                listExistingADgroups = null;

                list?.Clear();
                list = null;
            }
        }


        private void DiscoverPrinters()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Printers"), DiscoveryObjectType.Printer);
            IList<ADSPrinterInfo> listPrinters = this.GetPrinters();
            if (listPrinters.Count > 0)
                ADSPrinterInfo.AddADSPrinterInfoBulk(listPrinters.ToList());
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Printers"), DiscoveryObjectType.Printer);
        }
        private IList<ADSPrinterInfo> GetPrinters()
        {
            IList<ADSPrinterInfo> list = new List<ADSPrinterInfo>();
            ADSPrinterInfo adsInfo = null;
            DirectoryEntry de = null;
            m_adSearch.Filter = string.Format("(objectCategory={0})", ADObjectCategory.PrintQueue);
            m_adSearch.PageSize = 1000;
            PrinterOrchestrator orchestrator = new PrinterOrchestrator();
            SearchResultCollection seCol = m_adSearch.FindAll();
            orchestrator.EndpointSystemId = (int)this.Endpoint.Id;

            foreach (SearchResult result in seCol)
            {
                adsInfo = new ADSPrinterInfo();
                de = result.GetDirectoryEntry();
                adsInfo = orchestrator.PopulatePrinter(de);
                if (null != adsInfo)
                    list.Add(adsInfo);
            }
            return list;
        }


        private void DiscoverComputers()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Computers"), DiscoveryObjectType.ADComputerGroup);
            IList<ADSComputerInfo> listComputers = this.GetComputers();
            if (listComputers.Count > 0)
                ADSComputerInfo.AddADSComputerInfoBulk(listComputers.ToList());

            if (this.ComputerGroups.Count > 0)
                ADSComputerGroup.AddADSComputerGroupBulk(this.ComputerGroups);
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Computers"), DiscoveryObjectType.ADComputerGroup);
        }
        private IList<ADSComputerInfo> GetComputers()
        {
            IList<ADSComputerInfo> list = new List<ADSComputerInfo>();
            ADSComputerInfo adsInfo = null;
            IList<ADSComputerGroup> listComputerGroups = new List<ADSComputerGroup>();
            DirectoryEntry de = null;

            m_adSearch.Filter = string.Format("(objectCategory={0})", ADObjectCategory.Computer);
            m_adSearch.PageSize = 1000;


            ComputerOrchestrator orchestrator = new ComputerOrchestrator();
            using (SearchResultCollection seCol = m_adSearch.FindAll())
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Found {0} computer(s) objects ", seCol.Count.ToString()), DiscoveryObjectType.ADComputerGroup);

                orchestrator.EndpointSystemId = (int)this.Endpoint.Id;
                foreach (SearchResult result in seCol)
                {
                    adsInfo = new ADSComputerInfo();
                    de = result.GetDirectoryEntry();
                    adsInfo = orchestrator.PopulateComputer(de, m_adSearch, ref listComputerGroups);

                    if (null != adsInfo)
                        list.Add(adsInfo);

                }

                this.ComputerGroups = listComputerGroups;
                return list;
            }
        }

        private void DiscoverFolders()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Shared Folders"), DiscoveryObjectType.Folder);
            IList<ADSSharedFolderInfo> listSharedFolder = this.GetSharedFolders();
            if (listSharedFolder.Count > 0)
                ADSSharedFolderInfo.AddADSSharedFolderInfoBulk(listSharedFolder);
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Shared Folders"), DiscoveryObjectType.Folder);
        }
        private IList<ADSSharedFolderInfo> GetSharedFolders()
        {
            IList<ADSSharedFolderInfo> list = new List<ADSSharedFolderInfo>();
            ADSSharedFolderInfo adsInfo = null;
            DirectoryEntry de = null;

            m_adSearch.Filter = string.Format("(objectCategory={0})", ADObjectCategory.Volume);
            m_adSearch.PageSize = 1000;

            SharedFolderOrchestrator orchestrator = new SharedFolderOrchestrator();
            using (SearchResultCollection seCol = m_adSearch.FindAll())
            {


                orchestrator.EndpointSystemId = (int)this.Endpoint.Id;
                foreach (SearchResult result in seCol)
                {
                    adsInfo = new ADSSharedFolderInfo();
                    de = result.GetDirectoryEntry();
                    adsInfo = orchestrator.PopulateSharedFolder(de);

                    if (null != adsInfo)
                        list.Add(adsInfo);
                }
                return list;
            }
        }


        private void DiscoverOUs()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering OUs"), DiscoveryObjectType.OrganizationalUnit);
            this.GetOrganizationalUnits();
            IList<ADSOuInfo> listAdOus = new List<ADSOuInfo>();
            listAdOus = ADSOuInfo.GetDeletedByEndPointSystemId(this.Endpoint.Id);
            ADSOuInfo.DeleteList(listAdOus.ToList());
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering OUs"), DiscoveryObjectType.OrganizationalUnit);
        }
        private IList<ADSOuInfo> GetOrganizationalUnits()
        {
            IList<ADSOuInfo> list = new List<ADSOuInfo>();
            ADSOuInfo adsInfo = null;
            DirectoryEntry de = null;

            m_adSearch.Filter = string.Format("(objectCategory={0})", ADObjectCategory.OrganizationalUnit);
            m_adSearch.PageSize = 1000;

            OrganizationalUnitOrchestrator orchestrator = new OrganizationalUnitOrchestrator();
            using (SearchResultCollection seCol = m_adSearch.FindAll())
            {
                orchestrator.EndpointSystemId = (int)this.Endpoint.Id;
                foreach (SearchResult result in seCol)
                {
                    adsInfo = new ADSOuInfo();
                    de = result.GetDirectoryEntry();
                    adsInfo = orchestrator.PopulateOrganizationalUnit(de, m_adSearch);

                    if (null != adsInfo)
                        list.Add(adsInfo);
                }
                if (list.Count > 0)
                {
                    ADSOuInfo.SaveOUs(list, MasterConfigHelperServiceUser, this.Endpoint.Id);
                }
                return list;
            }
        }

        #endregion

        #endregion

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword for User:{UserName} started.");
            if (this.Connect())
            {
                System.DirectoryServices.DirectorySearcher pADSearcher = default(System.DirectoryServices.DirectorySearcher);
                System.DirectoryServices.SearchResult pADResult = default(System.DirectoryServices.SearchResult);

                using (DirectoryEntry dir = GetDirectoryEntry(objectClass.user, UserName, string.Empty))
                {
                    bool pFunctionResult = false;
                    int userControl = (int)dir.Properties["userAccountControl"].Value;
                    int CheckIfUserIsDisabled = userControl & (int)ADUserControl.ACCOUNTDISABLE;

                    bool IsLocked = ADAccount.IsAccountLocked(dir);

                    if (ADAccount.IsAccountDisabled(dir))
                    {
                        UnLockChangePasswordAPIMessage = $"Account:{UserName} disabled. Please contact IAM Team.";
                        throw new Exception($"Account:{UserName} disabled. Please contact IAM Team.");
                    }

                    if (IsLocked)
                    {
                        ADSUserInfo adsUser = ADSUserInfo.GetBysAMAccountName(UserName);
                        UnLockAccount(adsUser);
                    }

                    try
                    {
                        pADSearcher = new System.DirectoryServices.DirectorySearcher(m_root);
                        using (pADSearcher)
                        {
                            //'Setting the search parameters for the Active directory search 
                            pADSearcher.Filter = "sAMAccountName=" + UserName.ToSecureLDAPFilter();
                            pADSearcher.PropertiesToLoad.Add("cn");
                            pADSearcher.PropertiesToLoad.Add("givenName");
                            pADSearcher.PropertiesToLoad.Add("SN");

                            //'This statement will throw an error if the Username or Password are incorrect 
                            pADResult = pADSearcher.FindOne();
                            if (pADResult != null)
                            {
                                DirectoryEntry objLoginEntry = (pADResult != null) ? pADResult.GetDirectoryEntry() : null;
                                if (objLoginEntry != null)
                                {
                                    object obj = objLoginEntry.Invoke("SetPassword", new object[] { Password });
                                    objLoginEntry.CommitChanges();
                                    EndPoint.UpdatePasswordresetcount(this.Endpoint.Id, UserName, MasterConfigHelperServiceUser);
                                    obj = null;
                                    _idmsLogger.WriteLog(LogLevelType.Info, $"ChangePassword for User:{UserName} successfully completed.");
                                    return true;
                                }
                            }
                            else
                            {
                                UnLockChangePasswordAPIMessage = $"User {UserName} not found.  Please contact IAM Team.";
                                _logger.WriteLog(LogLevelType.Info, "User " + UserName + "not found;");
                                return false;
                            }
                            return pFunctionResult;
                        }
                    }
                    catch (Exception ex)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Exception, $"ChangePassword failed for user: {UserName}", ex);
                        throw ex;
                    }
                }
            }

            return false;
        }

        public override bool ChangePasswordUsingOldPassword(string UserName, string Password, string oldPassword)
        {
            //EndpointInformation EndpointInformation = EndPointFacade.GetEndPointSystemInformation(EndpointId);

            //LDAP Related objects 
            if (this.Connect())
            {
                System.DirectoryServices.DirectorySearcher pADSearcher = default(System.DirectoryServices.DirectorySearcher);
                System.DirectoryServices.SearchResult pADResult = default(System.DirectoryServices.SearchResult);


                bool pFunctionResult = false;

                try
                {
                    pADSearcher = new System.DirectoryServices.DirectorySearcher(m_root);
                    using (pADSearcher)
                    {
                        //'Setting the search parameters for the Active directory search 
                        pADSearcher.Filter = "sAMAccountName=" + UserName.ToSecureLDAPFilter();
                        pADSearcher.PropertiesToLoad.Add("cn");
                        pADSearcher.PropertiesToLoad.Add("givenName");
                        pADSearcher.PropertiesToLoad.Add("SN");

                        //'This statement will throw an error if the Username or Password are incorrect 
                        pADResult = pADSearcher.FindOne();
                        if (pADResult != null)
                        {
                            DirectoryEntry objLoginEntry = (pADResult != null) ? pADResult.GetDirectoryEntry() : null;
                            if (objLoginEntry != null)
                            {

                                object obj = objLoginEntry.Invoke("ChangePassword", new object[] { oldPassword, Password });
                                //  string result = obj.ToString();
                                objLoginEntry.CommitChanges();
                                obj = null;
                                return true;

                            }
                        }
                        else
                        {
                            _logger.WriteLog(LogLevelType.Info, "User " + UserName + "not found;", null);
                            return false;
                        }
                        return pFunctionResult;
                    }

                }
                catch (Exception ex)
                {

                    _logger.WriteLog(LogLevelType.Error, "Error: " + ex.InnerException.ToString(), ex);
                    return false;

                }

            }
            return false;
        }

        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            try
            {
                ADSUserInfo _adsUserInfo = (ADSUserInfo)_userInfo;

                string domainString = this.getDomainString(Endpoint);

                if (this.Connect())
                {
                    _adsUserInfo.cn = _adsUserInfo.cn.Replace(",", @"\,");

                    ADHelper.SetDirectoryPath(m_root, _adsUserInfo.ou, EndpointConfiguration);

                    //Check for User Already Exists or Not.
                    using (DirectorySearcher ds = new DirectorySearcher(m_root))
                    {
                        ds.SearchScope = SearchScope.Subtree;
                        ds.Filter = "(&(objectClass=User)(sAMAccountName=" + _adsUserInfo.sAMAccountName.ToSecureLDAPFilter() + "))";
                        if (ds.FindOne() == null)
                        {
                            string upn = string.Empty;
                            if (this.Endpoint.ApplicationType == ApplicationType.SAPLocal)
                            {
                                upn = string.Format("{0}@{1}", _adsUserInfo.sAMAccountName, "SAP.LOCAL");
                            }
                            else
                            {
                                upn = string.Format("{0}@{1}", _adsUserInfo.sAMAccountName, MasterConfigHelper.ADUPNDomainName);
                            }

                            DirectoryEntry newUser = m_root.Children.Add("CN=" + _adsUserInfo.cn, "user");
                            newUser.Properties["samAccountName"].Value = _adsUserInfo.sAMAccountName;
                            if (!string.IsNullOrEmpty(_adsUserInfo.company))
                                newUser.Properties["company"].Value = _adsUserInfo.company;
                            if (!string.IsNullOrEmpty(_adsUserInfo.department))
                                newUser.Properties["department"].Value = _adsUserInfo.department;
                            if (!string.IsNullOrEmpty(_adsUserInfo.description))
                                newUser.Properties["description"].Value = _adsUserInfo.description;
                            if (!string.IsNullOrEmpty(_adsUserInfo.displayName))
                                newUser.Properties["displayName"].Value = _adsUserInfo.displayName;

                            if (!string.IsNullOrEmpty(_adsUserInfo.facsimileTelephoneNumber))
                                newUser.Properties["facsimileTelephoneNumber"].Value = _adsUserInfo.facsimileTelephoneNumber;

                            if (!string.IsNullOrEmpty(_adsUserInfo.givenName))
                                newUser.Properties["givenName"].Value = _adsUserInfo.givenName;
                            if (!string.IsNullOrEmpty(_adsUserInfo.homePhone))
                                newUser.Properties["homePhone"].Value = _adsUserInfo.homePhone;
                            if (!string.IsNullOrEmpty(_adsUserInfo.sn))
                                newUser.Properties["sn"].Value = _adsUserInfo.sn;
                            if (!string.IsNullOrEmpty(_adsUserInfo.st))
                                newUser.Properties["st"].Value = _adsUserInfo.st;
                            if (!string.IsNullOrEmpty(_adsUserInfo.postalAddress))
                                newUser.Properties["postalAddress"].Value = _adsUserInfo.postalAddress;
                            if (!string.IsNullOrEmpty(_adsUserInfo.postalCode))
                                newUser.Properties["postalCode"].Value = _adsUserInfo.postalCode;
                            if (!string.IsNullOrEmpty(_adsUserInfo.postOfficeBox))
                                newUser.Properties["postOfficeBox"].Value = _adsUserInfo.postOfficeBox;
                            if (!string.IsNullOrEmpty(_adsUserInfo.physicalDeliveryOfficeName))
                                newUser.Properties["physicalDeliveryOfficeName"].Value = _adsUserInfo.physicalDeliveryOfficeName;
                            if (!string.IsNullOrEmpty(_adsUserInfo.telephoneNumber))
                                newUser.Properties["telephoneNumber"].Value = _adsUserInfo.telephoneNumber;
                            if (!string.IsNullOrEmpty(_adsUserInfo.info))
                                newUser.Properties["info"].Value = _adsUserInfo.info;
                            if (!string.IsNullOrEmpty(_adsUserInfo.co))
                                newUser.Properties["co"].Value = _adsUserInfo.co;
                            if (!string.IsNullOrEmpty(_adsUserInfo.manager))
                                newUser.Properties["manager"].Value = _adsUserInfo.manager;
                            if (!string.IsNullOrEmpty(_adsUserInfo.streetAddress))
                                newUser.Properties["streetAddress"].Value = _adsUserInfo.streetAddress;
                            if (!string.IsNullOrEmpty(_adsUserInfo.wWWHomePage))
                                newUser.Properties["wWWHomePage"].Value = _adsUserInfo.wWWHomePage;
                            if (!string.IsNullOrEmpty(_adsUserInfo.name))
                                newUser.Properties["name"].Value = _adsUserInfo.name;
                            if (!string.IsNullOrEmpty(_adsUserInfo.ipPhone))
                                newUser.Properties["ipPhone"].Value = _adsUserInfo.ipPhone;
                            if (!string.IsNullOrEmpty(_adsUserInfo.mobile))
                                newUser.Properties["mobile"].Value = _adsUserInfo.mobile;
                            if (!string.IsNullOrEmpty(_adsUserInfo.pager))
                                newUser.Properties["pager"].Value = _adsUserInfo.pager;

                            if (!string.IsNullOrEmpty(_adsUserInfo.mail))
                            {
                                newUser.Properties["mail"].Value = _adsUserInfo.mail;
                                //This must be for onboarding because for NonEmployee they might enter non domain email or sharedmailbox email. This cause issue in sync. Hence handling only for Onboarding
                                //For NonEmployee when they apply office365 it updates traget address, proxyaddress and mailNickName (UpdatePOBoxAttribute method).
                                if (this.CurrentRequest.RequestSource == AccessRequestSource.OnBoarding && this.CurrentRequest.AccountType == AccountType.User 
                                    && this.CurrentRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal
                                    )
                                {
                                    newUser.Properties["targetAddress"].Value = _adsUserInfo.mail;
                                    try
                                    {
                                        string SMTP = string.Format("SMTP:{0}", _adsUserInfo.mail);
                                        string sip = string.Format("sip:{0}", upn);

                                        newUser.Properties["proxyAddresses"].Value = new object[] { sip, SMTP };

                                        newUser.Properties["mailNickName"].Value = _adsUserInfo.mail.Split('@')[0];
                                    }
                                    catch { }
                                }
                            }

                            if (!string.IsNullOrEmpty(_adsUserInfo.countryCode))
                                newUser.Properties["l"].Value = _adsUserInfo.countryCode;
                            if (!string.IsNullOrEmpty(_adsUserInfo.title))
                                newUser.Properties["title"].Value = _adsUserInfo.title;
                            if (!string.IsNullOrEmpty(_adsUserInfo.sAMAccountName))
                                newUser.Properties["userPrincipalName"].Value = upn;
                            if (!string.IsNullOrEmpty(_adsUserInfo.homeDirectory))
                                newUser.Properties["homeDirectory"].Value = _adsUserInfo.homeDirectory;
                            if (!string.IsNullOrEmpty(_adsUserInfo.profilePath))
                                newUser.Properties["profilePath"].Value = _adsUserInfo.profilePath;
                            if (!string.IsNullOrEmpty(_adsUserInfo.homeDrive))
                                newUser.Properties["homeDrive"].Value = _adsUserInfo.homeDrive;
                            //if (!string.IsNullOrEmpty(_adsUserInfo.division))
                            //    newUser.Properties["division"].Value = _adsUserInfo.division;
                            if (!string.IsNullOrEmpty(_adsUserInfo.businessCategory))
                            {
                                newUser.Properties["businessCategory"].Value = _adsUserInfo.businessCategory;
                                newUser.Properties["extensionAttribute1"].Value = _adsUserInfo.businessCategory;
                            }

                            if (!string.IsNullOrEmpty(_adsUserInfo.otherPager))
                                newUser.Properties["extensionAttribute2"].Value = _adsUserInfo.otherPager;
                            if (!string.IsNullOrEmpty(_adsUserInfo.otherFascimileTelephoneNumber))
                                newUser.Properties["extensionAttribute3"].Value = _adsUserInfo.otherFascimileTelephoneNumber;
                            if (!string.IsNullOrEmpty(_adsUserInfo.extensionAttribute4))
                                newUser.Properties["extensionAttribute4"].Value = _adsUserInfo.extensionAttribute4;

                            if (!string.IsNullOrEmpty(_adsUserInfo.extensionAttribute6))
                                newUser.Properties["extensionAttribute6"].Value = _adsUserInfo.extensionAttribute6;

                            if (!string.IsNullOrEmpty(_adsUserInfo.extensionAttribute10))
                                newUser.Properties["extensionAttribute10"].Value = _adsUserInfo.extensionAttribute10;

                            if (_adsUserInfo.accountExpires.HasValue)
                            {
                                DateTime expiryDate = _adsUserInfo.accountExpires.Value;
                                if (expiryDate.Date != DateTime.MaxValue.Date)
                                {
                                    expiryDate = _adsUserInfo.accountExpires.Value.AddDays(1);
                                }

                                int year = expiryDate.Year;
                                int month = expiryDate.Month;
                                int day = expiryDate.Day;

                                newUser.InvokeSet("AccountExpirationDate", new object[] { new DateTime(year, month, day) });
                            }
                            else
                            {
                                newUser.Properties["AccountExpires"].Value = "0";
                            }

                            User ownerusr = User.GetById(this.CurrentRequest.UserId);

                            //extensionAttribute11 - Account Type
                            if (!string.IsNullOrWhiteSpace(_adsUserInfo.extensionAttribute11))
                                newUser.Properties["extensionAttribute11"].Value = _adsUserInfo.extensionAttribute11;
                            else
                                newUser.Properties["extensionAttribute11"].Value = this.CurrentRequest.AccountType.GetDescription();

                            //extensionAttribute12 - Account Owner
                            if (!string.IsNullOrWhiteSpace(_adsUserInfo.extensionAttribute12))
                                newUser.Properties["extensionAttribute12"].Value = _adsUserInfo.extensionAttribute12;
                            else if (ownerusr != null)
                                newUser.Properties["extensionAttribute12"].Value = !string.IsNullOrWhiteSpace(ownerusr.KnownAs) ? ownerusr.KnownAs : ownerusr.FirstName + ownerusr.LastName;

                            if (!string.IsNullOrWhiteSpace(_adsUserInfo.info))
                                newUser.Properties["info"].Value = _adsUserInfo.info;
                            else
                                newUser.Properties["info"].Value = $"Created on {DateTime.Now.ToString("dd MMM yyyy")} by IDMS. Request Id {this.CurrentRequest.Id};";

                            //extensionAttribute13 
                            if (!string.IsNullOrWhiteSpace(_adsUserInfo.extensionAttribute13))
                                newUser.Properties["extensionAttribute13"].Value = _adsUserInfo.extensionAttribute13;


                            newUser.CommitChanges();

                            //*********Start***********************
                            //1.Do not change or uncomment the below lines of code as it will impact on security violaiton by setting 
                            //2.userAccountControl values is 546- Disabled|Password Not Required instead of 512-Normal Account
                            //3.We have to mandate to setPassword irrespective of Provisioning Form parameter, otherwise won't able to set vlaue 512 for userAccountControl
                            newUser.Invoke("SetPassword", new object[] { _adsUserInfo.NewUsrPassword });
                            newUser.CommitChanges();
                            newUser.Properties["userAccountControl"].Value = (int)ADUserControl.NORMAL_ACCOUNT;
                            newUser.CommitChanges();
                            //*********End***********************


                            int val = (int)newUser.Properties["userAccountControl"].Value;

                            System.DirectoryServices.DirectorySearcher pADSearcher = default(System.DirectoryServices.DirectorySearcher);
                            System.DirectoryServices.SearchResult pADResult = default(System.DirectoryServices.SearchResult);
                            pADSearcher = new System.DirectoryServices.DirectorySearcher(m_root);

                            using (pADSearcher)
                            {
                                pADSearcher.Filter = "sAMAccountName=" + _adsUserInfo.sAMAccountName.ToSecureLDAPFilter();
                                pADResult = pADSearcher.FindOne();
                                if (pADResult != null)
                                {
                                    using (DirectoryEntry objLoginEntry = (pADResult != null) ? pADResult.GetDirectoryEntry() : null)
                                    {
                                        val = (int)objLoginEntry.Properties["userAccountControl"].Value;
                                        objLoginEntry.Properties["userAccountControl"].Value = val & ~(int)ADUserControl.ACCOUNTDISABLE;
                                        objLoginEntry.CommitChanges();

                                        if (fields != null)
                                        {
                                            //objLoginEntry.Invoke("SetPassword", new object[] { _adsUserInfo.NewUsrPassword });
                                            //objLoginEntry.CommitChanges();

                                            if (fields["PasswordChangeOnNextLogon"].Value == "Yes")
                                            {
                                                objLoginEntry.Properties["pwdLastSet"].Value = 0;
                                            }

                                            val = (int)objLoginEntry.Properties["userAccountControl"].Value;

                                            if (fields["PasswordNeverExpire"].Value == "Yes")
                                                objLoginEntry.Properties["userAccountControl"].Value = val | (int)ADUserControl.DONT_EXPIRE_PASSWORD;


                                            objLoginEntry.CommitChanges();
                                        }

                                        m_root.Close();
                                        objLoginEntry.Close();

                                        _logger.WriteLog(LogLevelType.Info, string.Format("User {0} Created Successfully in Active Directory.", _adsUserInfo.sAMAccountName), null);
                                    }
                                }
                                if (fields["CreateHomeFolder"].Value == "Yes")
                                {
                                    _logger.WriteLog(LogLevelType.Info, string.Format("CreateHomeFolder Started for User {0}.", _adsUserInfo.sAMAccountName), null);

                                    bool bStatus = this.CreateHomeFolder(fields["HomeFolderPath"].Value, _adsUserInfo.AccountName);

                                    _logger.WriteLog(LogLevelType.Info, string.Format("CreateHomeFolder Completed for User {0}.", _adsUserInfo.sAMAccountName), null);
                                }
                            }
                        }
                        else
                        {
                            _logger.WriteLog(LogLevelType.Info, $"{_adsUserInfo.sAMAccountName} already exists", null);
                            return false;
                        }
                    }
                }
                else
                {
                    _logger.WriteLog(LogLevelType.Error, "Connection Failed", null);

                    return false;
                }
            }
            catch (Exception Ex)
            {
                if (Ex.Message.Contains("The object already exists."))
                    throw;

                _logger.WriteLog(LogLevelType.Error, Ex.ToString(), Ex);
                return false;
            }

            return true;
        }

        public override bool PostReactivationProcedure(IEndpointUserInfo _userInfo)
        {
            try
            {
                ADSUserInfo _adsUserInfo = (ADSUserInfo)_userInfo;

                if (this.Connect())
                {
                    using (DirectorySearcher ds = new DirectorySearcher(m_root))
                    {

                        ds.SearchScope = SearchScope.Subtree;
                        ds.Filter = "(&(objectClass=User)(sAMAccountName=" + _adsUserInfo.sAMAccountName.ToSecureLDAPFilter() + "))";
                        if (ds.FindOne() == null)
                        {
                            _logger.WriteLog(LogLevelType.Info, String.Format(" User: {0} Not found .", _adsUserInfo.sAMAccountName), null);
                        }
                        SearchResult result = ds.FindOne();
                        if (result == null)
                        {
                            _logger.WriteLog(LogLevelType.Info, String.Format(" User: {0} Not found  in SearchResult.", _adsUserInfo.sAMAccountName), null);
                        }
                        using (DirectoryEntry rejoinUser = result.GetDirectoryEntry())
                        {
                            _logger.WriteLog(LogLevelType.Info, $"PostReactivationProcedure Set AccountExpires");
                            if (_adsUserInfo.accountExpires.HasValue)
                            {
                                DateTime expiryDate = _adsUserInfo.accountExpires.Value;
                                if (expiryDate.Date != DateTime.MaxValue.Date)
                                {
                                    expiryDate = _adsUserInfo.accountExpires.Value.AddDays(1);
                                }

                                int year = expiryDate.Year;
                                int month = expiryDate.Month;
                                int day = expiryDate.Day;

                                rejoinUser.InvokeSet("AccountExpirationDate", new object[] { new DateTime(year, month, day) });
                            }
                            else
                            {
                                rejoinUser.Properties["AccountExpires"].Value = "0";
                            }

                            rejoinUser.CommitChanges();

                            int val = (int)rejoinUser.Properties["userAccountControl"].Value;

                            System.DirectoryServices.DirectorySearcher pADSearcher = default(System.DirectoryServices.DirectorySearcher);
                            System.DirectoryServices.SearchResult pADResult = default(System.DirectoryServices.SearchResult);

                            pADSearcher = new System.DirectoryServices.DirectorySearcher(m_root);
                            using (pADSearcher)
                            {
                                pADSearcher.Filter = "sAMAccountName=" + _adsUserInfo.sAMAccountName.ToSecureLDAPFilter();

                                pADResult = pADSearcher.FindOne();
                                if (pADResult != null)
                                {
                                    DirectoryEntry objLoginEntry = (pADResult != null) ? pADResult.GetDirectoryEntry() : null;
                                    val = (int)objLoginEntry.Properties["userAccountControl"].Value;

                                    objLoginEntry.Properties["userAccountControl"].Value = val & ~(int)ADUserControl.ACCOUNTDISABLE;
                                    objLoginEntry.CommitChanges();

                                    objLoginEntry.Properties["pwdLastSet"].Value = 0;
                                    val = (int)objLoginEntry.Properties["userAccountControl"].Value;

                                    objLoginEntry.CommitChanges();

                                    objLoginEntry.Close();

                                    _logger.WriteLog(LogLevelType.Info, string.Format("PostReactivationProcedure - User {0} Created Successfully in Active Directory.", _adsUserInfo.sAMAccountName), null);
                                }

                                m_root.Close();
                                rejoinUser.Close();
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception Ex)
            {
                _logger.WriteLog(LogLevelType.Error, Ex.ToString(), Ex);
                return false;
            }
        }
        public override bool UpdateAccountAttribute(IEndpointUserInfo _userInfo)
        {
            try
            {
                ADSUserInfo ADSUserInfo = (ADSUserInfo)_userInfo;

                if (this.Connect() && m_root != null)
                {
                    using (DirectorySearcher ds = new DirectorySearcher(m_root))
                    {

                        ds.SearchScope = SearchScope.Subtree;
                        ds.Filter = "(&(objectClass=User)(sAMAccountName=" + ADSUserInfo.sAMAccountName.ToSecureLDAPFilter() + "))";
                        if (ds.FindOne() == null)
                        {
                            _logger.WriteLog(LogLevelType.Info, String.Format(" User: {0} Not found .", ADSUserInfo.sAMAccountName), null);
                        }
                        SearchResult result = ds.FindOne();
                        if (result == null)
                        {
                            _logger.WriteLog(LogLevelType.Info, String.Format(" User: {0} Not found  in SearchResult.", ADSUserInfo.sAMAccountName), null);
                        }
                        using (DirectoryEntry objLoginEntry = result.GetDirectoryEntry())
                        {
                            if (!string.IsNullOrWhiteSpace(ADSUserInfo.extensionAttribute12))
                                objLoginEntry.Properties["extensionAttribute12"].Value = ADSUserInfo.extensionAttribute12;

                            objLoginEntry.CommitChanges();

                            m_root.Close();
                            objLoginEntry.Close();
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception Ex)
            {
                WriteDiscoveryLog(LogLevelType.Error,$"UpdateAccountAttribute failed:{Ex.ToString()}", Ex);
                return false;
            }
        }
        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            try
            {
                ADSUserInfo ADSUserInfo = (ADSUserInfo)_userInfo;

                if (this.Connect() && m_root != null)
                {
                    using (DirectorySearcher ds = new DirectorySearcher(m_root))
                    {
                        ds.SearchScope = SearchScope.Subtree;
                        ds.Filter = "(&(objectClass=User)(sAMAccountName=" + ADSUserInfo.sAMAccountName.ToSecureLDAPFilter() + "))";
                        if (ds.FindOne() == null)
                        {
                            _logger.WriteLog(LogLevelType.Info, String.Format(" User: {0} Not found .", ADSUserInfo.sAMAccountName), null);
                        }
                        SearchResult result = ds.FindOne();
                        if (result == null)
                        {
                            _logger.WriteLog(LogLevelType.Info, String.Format(" User: {0} Not found  in SearchResult.", ADSUserInfo.sAMAccountName), null);
                        }
                        using (DirectoryEntry objLoginEntry = result.GetDirectoryEntry())
                        {
                            var company = !string.IsNullOrEmpty(ADSUserInfo.company) ? ADSUserInfo.company : null;
                            var department = !string.IsNullOrEmpty(ADSUserInfo.department) ? ADSUserInfo.department : null;
                            var title = !string.IsNullOrEmpty(ADSUserInfo.title) ? ADSUserInfo.title : null;
                            var pager = !string.IsNullOrEmpty(ADSUserInfo.pager) ? ADSUserInfo.pager : null;
                            var division = !string.IsNullOrEmpty(ADSUserInfo.division) ? ADSUserInfo.division : null;
                            var businessCategory = !string.IsNullOrEmpty(ADSUserInfo.businessCategory) ? ADSUserInfo.businessCategory : null;

                            var sn = !string.IsNullOrEmpty(ADSUserInfo.sn) ? ADSUserInfo.sn : null;
                            var displayName = !string.IsNullOrEmpty(ADSUserInfo.displayName) ? ADSUserInfo.displayName : null;
                            var description = !string.IsNullOrEmpty(ADSUserInfo.description) ? ADSUserInfo.description : null;

                            var mail = !string.IsNullOrEmpty(ADSUserInfo.mail) ? ADSUserInfo.mail : null;
                            var telephoneNumber = !string.IsNullOrEmpty(ADSUserInfo.telephoneNumber) ? ADSUserInfo.telephoneNumber : null;
                            var mobile = !string.IsNullOrEmpty(ADSUserInfo.mobile) ? ADSUserInfo.mobile : null;
                            var facsimileTelephoneNumber = !string.IsNullOrEmpty(ADSUserInfo.facsimileTelephoneNumber) ? ADSUserInfo.facsimileTelephoneNumber : null;

                            var postalAddress = !string.IsNullOrEmpty(ADSUserInfo.postalAddress) ? ADSUserInfo.postalAddress : null;
                            var postalCode = !string.IsNullOrEmpty(ADSUserInfo.postalCode) ? ADSUserInfo.postalCode : null;

                            var physicalDeliveryOfficeName = !string.IsNullOrEmpty(ADSUserInfo.physicalDeliveryOfficeName) ? ADSUserInfo.physicalDeliveryOfficeName : null;

                            var manager = !string.IsNullOrEmpty(ADSUserInfo.manager) ? ADSUserInfo.manager : null;

                            var extensionAttribute6 = !string.IsNullOrEmpty(ADSUserInfo.extensionAttribute6) ? ADSUserInfo.extensionAttribute6 : null;
                            var otherPager = !string.IsNullOrEmpty(ADSUserInfo.otherPager) ? ADSUserInfo.otherPager : null;
                            var otherFascimileTelephoneNumber = !string.IsNullOrEmpty(ADSUserInfo.otherFascimileTelephoneNumber) ? ADSUserInfo.otherFascimileTelephoneNumber : null;
                            var extensionAttribute10 = !string.IsNullOrEmpty(ADSUserInfo.extensionAttribute10) ? ADSUserInfo.extensionAttribute10 : null;

                            string mailNickName = string.Empty;
                            string targetAddress = string.Empty;
                            string proxyAddress = string.Empty;
                            objLoginEntry.Properties["company"].Value = company;
                            objLoginEntry.Properties["department"].Value = department;
                            objLoginEntry.Properties["title"].Value = title;
                            objLoginEntry.Properties["pager"].Value = pager;
                            //objLoginEntry.Properties["division"].Value = division;
                            objLoginEntry.Properties["businessCategory"].Value = businessCategory;
                            objLoginEntry.Properties["extensionAttribute1"].Value = businessCategory;

                            objLoginEntry.Properties["sn"].Value = sn;
                            objLoginEntry.Properties["displayName"].Value = displayName;
                            objLoginEntry.Properties["description"].Value = description;

                            objLoginEntry.Properties["mail"].Value = mail;
                            objLoginEntry.Properties["telephoneNumber"].Value = telephoneNumber;
                            objLoginEntry.Properties["mobile"].Value = mobile;
                            objLoginEntry.Properties["facsimileTelephoneNumber"].Value = facsimileTelephoneNumber;

                            objLoginEntry.Properties["postalAddress"].Value = postalAddress;
                            objLoginEntry.Properties["postalCode"].Value = postalCode;

                            objLoginEntry.Properties["physicalDeliveryOfficeName"].Value = physicalDeliveryOfficeName;

                            objLoginEntry.Properties["manager"].Value = manager;

                            objLoginEntry.Properties["extensionAttribute6"].Value = extensionAttribute6;
                            objLoginEntry.Properties["extensionAttribute2"].Value = otherPager;
                            objLoginEntry.Properties["extensionAttribute3"].Value = otherFascimileTelephoneNumber;
                            objLoginEntry.Properties["extensionAttribute10"].Value = extensionAttribute10;
                            objLoginEntry.Properties["extensionAttribute8"].Value = !string.IsNullOrEmpty(ADSUserInfo.extensionAttribute8) ? ADSUserInfo.extensionAttribute8 : null;

                            //extensionAttribute13 
                            if (!string.IsNullOrWhiteSpace(ADSUserInfo.extensionAttribute13))
                                objLoginEntry.Properties["extensionAttribute13"].Value = ADSUserInfo.extensionAttribute13;

                            try
                            {
                                targetAddress = ADSUserInfo.mail;
                                if (!string.IsNullOrWhiteSpace(ADSUserInfo.mail) && this.CurrentRequest.AccountType == AccountType.User && this.CurrentRequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
                                {
                                    var ep = EndPoint.GetByApplicationType(ApplicationType.Office365);
                                    //Possiblity that For NonEmployee they might add mailid as SharedMailBox
                                    var sharedAccount = AccountReconcilation.GetByAccountNameByAccountType(ADSUserInfo.mail, ep.Id, AccountType.Shared);
                                    if (sharedAccount == null)
                                    {
                                        objLoginEntry.Properties["targetAddress"].Value = targetAddress;
                                        string upn = string.Empty;
                                        if (this.Endpoint.ApplicationType == ApplicationType.SAPLocal)
                                        {
                                            upn = string.Format("{0}@{1}", ADSUserInfo.sAMAccountName, "SAP.LOCAL");
                                        }
                                        else
                                        {
                                            upn = string.Format("{0}@{1}", ADSUserInfo.sAMAccountName, MasterConfigHelper.ADUPNDomainName);
                                        }

                                        string SMTP = string.Format("SMTP:{0}", ADSUserInfo.mail);
                                        string sip = string.Format("sip:{0}", upn);
                                        proxyAddress = $"{sip},{SMTP}";
                                        objLoginEntry.Properties["proxyAddresses"].Value = new object[] { sip, SMTP };
                                        mailNickName = ADSUserInfo.mail.Split('@')[0];
                                        objLoginEntry.Properties["mailNickName"].Value = mailNickName;
                                    }
                                }
                                else
                                {
                                    objLoginEntry.Properties["proxyAddresses"].Value = null;
                                    objLoginEntry.Properties["mailNickName"].Value = null;
                                    objLoginEntry.Properties["targetAddress"].Value = null;
                                }
                            }
                            catch { }

                            _logger.WriteLog(LogLevelType.Info, $"Propagation Detail Update:company:{company} department:{department} title:{title}" +
                                $"pager:{pager} division:{division} businessCategory:{businessCategory} extensionAttribute1:{businessCategory} sn:{sn} displayName:{displayName}" +
                                $"description:{description} mail:{mail} telephoneNumber:{telephoneNumber} mobile:{mobile} facsimileTelephoneNumber:{facsimileTelephoneNumber}" +
                                $"postalAddress:{postalAddress} postalCode:{postalCode} physicalDeliveryOfficeName:{physicalDeliveryOfficeName} manager:{manager} extensionAttribute6:{extensionAttribute6} extensionAttribute8:{ADSUserInfo?.extensionAttribute8}" +
                                $"otherPager:{otherPager} otherFascimileTelephoneNumber:{otherFascimileTelephoneNumber} extensionAttribute10:{extensionAttribute10} extensionAttribute13:{ADSUserInfo.extensionAttribute13}");

                            objLoginEntry.CommitChanges();
                            
                           
                            objLoginEntry.Close();
                        }
                        result = null;
                    }
                    _logger.WriteLog(LogLevelType.Info, $"Fetch the updated account:{ADSUserInfo.sAMAccountName} distinguishedName");
                    using (DirectorySearcher ds = new DirectorySearcher(m_root))
                    {
                        //Fetch Distinguishedname after update. This is required because during Reorg Manager distinguishedName should be referenced to Non-Employee it get failed. To avoid that we should get the distinguishedName.
                        //Error will be Constraint violation occurred
                        ds.SearchScope = SearchScope.Subtree;
                        ds.Filter = "(&(objectClass=User)(sAMAccountName=" + ADSUserInfo.sAMAccountName.ToSecureLDAPFilter() + "))";
                        if (ds.FindOne() == null)
                        {
                            _logger.WriteLog(LogLevelType.Info, String.Format(" User: {0} Not found .", ADSUserInfo.sAMAccountName), null);
                        }
                        var result = ds.FindOne();
                        using (DirectoryEntry objLoginEntry = result.GetDirectoryEntry())
                        {
                            ADSUserInfo.distinguishedName = objLoginEntry.Properties["distinguishedName"].Value.ToString();
                            objLoginEntry.Close();
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception Ex)
            {
                _logger.WriteLog(LogLevelType.Error, Ex.ToString(), Ex);
                return false;
            }
            finally
            {
                m_root?.Close();
            }
        }

        public override bool LockAccount(IEndpointUserInfo _userInfo)
        {
            try
            {
                this.Connect();

                using (DirectoryEntry dir = GetDirectoryEntry(objectClass.user, _userInfo.AccountName, string.Empty))
                {
                    int val = (int)dir.Properties["userAccountControl"].Value;

                    dir.Properties["userAccountControl"].Value = val | (int)ADUserControl.LOCKOUT;

                    dir.CommitChanges();

                    _logger.WriteLog(LogLevelType.Info, _userInfo.AccountName + " is Locked", null);

                    base.LockAccount(_userInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Error, ex.Message, ex);
                return false;
            }
            return true;
        }

        public override bool UnLockAccount(IEndpointUserInfo _userInfo)
        {
            try
            {
                _logger.WriteLog(LogLevelType.Info, $"UnlockAccount for User:{_userInfo.AccountName} started.");
                this.Connect();

                using (DirectoryEntry dir = GetDirectoryEntry(objectClass.user, _userInfo.AccountName, string.Empty))
                {
                    int val = (int)dir.Properties["userAccountControl"].Value;

                    dir.Properties["userAccountControl"].Value = val & ~(int)ADUserControl.LOCKOUT;

                    dir.CommitChanges();

                    _logger.WriteLog(LogLevelType.Info, _userInfo.AccountName + " is Unlocked", null);

                    base.UnLockAccount(_userInfo);
                }
            }
            catch (Exception ex)
            {
                UnLockChangePasswordAPIMessage = $"UnLock Account failed for User:{_userInfo.AccountName}. Please contact IAM Team.";
                _logger.WriteLog(LogLevelType.Error, ex.Message, ex);
                return false;
            }
            _logger.WriteLog(LogLevelType.Info, $"UnlockAccount for User:{_userInfo.AccountName} successfully completed.");
            return true;
        }

        public override bool DisableAccount(IEndpointUserInfo _userInfo)
        {
            try
            {
                ADSUserInfo adsUserinfo = _userInfo as ADSUserInfo;
                this.Connect();

                using (DirectoryEntry dir = GetDirectoryEntry(objectClass.user, _userInfo.AccountName, string.Empty))
                {
                    int val = (int)dir.Properties["userAccountControl"].Value;

                    dir.Properties["userAccountControl"].Value = val | (int)ADUserControl.ACCOUNTDISABLE;
                    if (this.CurrentRequest != null && this.CurrentRequest.AccountType == AccountType.Service)
                    {
                        dir.CommitChanges();
                        val = (int)dir.Properties["userAccountControl"].Value;
                        dir.Properties["userAccountControl"].Value = val & ~(int)ADUserControl.DONT_EXPIRE_PASSWORD;
                    }
                    dir.Properties["postOfficeBox"].Value = null;
                    dir.Properties["manager"].Value = null;

                    dir.Properties["mail"].Value = null;
                    dir.Properties["mailNickName"].Value = null;
                    dir.Properties["proxyAddresses"].Value = null;
                    dir.Properties["targetAddress"].Value = null;
                    try
                    {
                        _logger.WriteLog(LogLevelType.Info, $"Reset additional attribute(msRTCSIP-DeploymentLocator,telephoneNumber,msRTCSIP-Line)");
                        dir.Properties["msRTCSIP-DeploymentLocator"].Value = null;
                        dir.Properties["telephoneNumber"].Value = null;
                        dir.Properties["msRTCSIP-Line"].Value = null;
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteLog(LogLevelType.Warning, $"Warning while reset the attribute(msRTCSIP-DeploymentLocator,telephoneNumber,msRTCSIP-Line) {ex.Message} {ex.StackTrace}");
                    }
                    
                    if (!string.IsNullOrWhiteSpace(adsUserinfo.info))
                        dir.Properties["info"].Value = adsUserinfo.info;
                    else
                        dir.Properties["info"].Value = $"Disabled on {DateTime.Now.ToString("dd MMM yyyy")} by IDMS. Request Id {this.CurrentRequest?.Id ?? 0};";

                    dir.Properties["pwdLastSet"].Value = 0;

                    dir.CommitChanges();

                    _logger.WriteLog(LogLevelType.Info, _userInfo.AccountName + " is Disabled", null);
                    base.LockAccount(_userInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Error, ex.ToString(), ex);
                return false;
            }
            return true;
        }

        public override bool EnableAccount(IEndpointUserInfo _userInfo)
        {
            try
            {
                ADSUserInfo adsUserInfo = _userInfo as ADSUserInfo;
                this.Connect();

                using (DirectoryEntry dir = GetDirectoryEntry(objectClass.user, _userInfo.AccountName, string.Empty))
                {
                    int val = (int)dir.Properties["userAccountControl"].Value;

                    dir.Properties["userAccountControl"].Value = val & ~(int)ADUserControl.ACCOUNTDISABLE;

                    if (this.CurrentRequest != null && this.CurrentRequest.AccountType == AccountType.Service)
                    {
                        dir.CommitChanges();
                        val = (int)dir.Properties["userAccountControl"].Value;
                        dir.Properties["userAccountControl"].Value = val | (int)ADUserControl.DONT_EXPIRE_PASSWORD;
                    }

                    if (!string.IsNullOrWhiteSpace(adsUserInfo.info))
                        dir.Properties["info"].Value = adsUserInfo.info;
                    else
                        dir.Properties["info"].Value = $"Enabled on {DateTime.Now.ToString("dd MMM yyyy")} by IDMS. Request Id {this.CurrentRequest?.Id ?? 0};";

                    dir.Properties["pwdLastSet"].Value = 0;

                    dir.CommitChanges();

                    _logger.WriteLog(LogLevelType.Info, _userInfo.AccountName + " is Enabled", null);

                    base.UnLockAccount(_userInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Error, ex.ToString(), ex);
                return false;
            }
            return true;
        }

        public override bool AddEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            bool bret = true;

            try
            {
                this.Connect();

                ADSUserInfo ADSUserInfo = (ADSUserInfo)_userInfo;

                if (IsAccountMarkedAsLocked(ADSUserInfo.Id, Endpoint.Id))
                {
                    if (this.CurrentRequest.AdHocAccessRequest != null && (this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                    {
                        _logger.WriteLog(LogLevelType.Error, string.Format("Account - {0} is locked.", ADSUserInfo.AccountName), null);
                        return false;
                    }
                }

                string usSt = GetObjectDistinguishedName(objectClass.user, returnType.distinguishedName, ADSUserInfo.sAMAccountName, string.Empty);
                _logger.WriteLog(LogLevelType.Info, $"AD Connector: AddEntitlement:started");
                List<string> exceptionalGroups = new List<string>();
                foreach (var grpName in ADSUserInfo.Groups)
                {
                    try
                    {
                        _logger.WriteLog(LogLevelType.Info, $"AD Connector: AddEntitlement: {grpName} started");
                        this.Connect();
                        string grst = GetObjectDistinguishedName(objectClass.group, returnType.distinguishedName, grpName, string.Empty);
                        ADHelper.SetDirectoryPath(m_root, grst, EndpointConfiguration);
                        m_root.Properties["member"].Add(usSt);
                        m_root.CommitChanges();
                        //ResetPath();
                        Thread.Sleep(1000);
                        _logger.WriteLog(LogLevelType.Info, $"AD Connector: AddEntitlement: {grpName} completed");
                    }
                    catch (COMException ex)
                    {
                        _logger.WriteLog(LogLevelType.Exception, "Add AddEntitlement COMException", ex);
                        if (ex != null && !string.IsNullOrWhiteSpace(ex.Message) && ex.Message.ToUpper().Contains(OBJECTEXISTS.ToUpper()))
                        {
                            //Dont throw exception.
                            _logger.WriteLog(LogLevelType.Exception, $"AD Connector: AddEntitlement: Group:Member already exists in {grpName}.", ex);
                        }
                        else
                        {
                            bret = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        bret = false;
                        _logger.WriteLog(LogLevelType.Exception, "Add AddEntitlement Exception", ex);
                        if (ex != null && ex.Message != null && ex.Message.Contains("unable to locate the distinguishedName for the object", StringComparison.InvariantCultureIgnoreCase))
                        {
                            _logger.WriteLog(LogLevelType.Exception, $"AD AddEntitlement:unable to locate the distinguishedName for the object.{ex.Message}", ex);
                        }
                    }
                }
                _logger.WriteLog(LogLevelType.DebugStep, $"AD Connector: AddEntitlement:Success");
            }
            catch (COMException ex)
            {
                _logger.WriteLog(LogLevelType.Exception, $"AD AddEntitlement: {ex.Message}", ex);
                if (ex.Message.ToUpper().Contains(OBJECTEXISTS.ToUpper()))
                {
                    bret = true;
                }
                else
                {
                    bret = false;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Error, ex.ToString(), ex);
                return false;
            }
            finally
            {
                m_root.Close();
            }

            if (bret == true)
                base.AddEntitlement(_userInfo, _entitlementType);

            _logger.WriteLog(LogLevelType.DebugStep, $"AD AddEntitlement:Status {bret}");
            return bret;
        }

        public override bool RemoveEntitlement(IEndpointUserInfo _userInfo, EntitlementType _entitlementType)
        {
            bool bret = false;
            try
            {
                this.Connect();

                ADSUserInfo ADSUserInfo = (ADSUserInfo)_userInfo;

                if (IsAccountMarkedAsLocked(ADSUserInfo.Id, Endpoint.Id))
                {
                    if (this.CurrentRequest.AdHocAccessRequest != null && (this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentAdd || this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.AmendmentRemove))
                    {
                        _logger.WriteLog(LogLevelType.Error, string.Format("Account - {0} is locked.", ADSUserInfo.AccountName), null);
                        return false;
                    }
                }

                string usSt = GetObjectDistinguishedName(objectClass.user, returnType.distinguishedName, ADSUserInfo.sAMAccountName, string.Empty);

                foreach (var grpName in ADSUserInfo.Groups)
                {
                    _logger.WriteLog(LogLevelType.Info, $"AD RemoveEntitlement:{grpName} started");
                    this.Connect();
                    string grst = GetObjectDistinguishedName(objectClass.group, returnType.distinguishedName, grpName, string.Empty);
                    ADHelper.SetDirectoryPath(m_root, grst, EndpointConfiguration);
                    m_root.Properties["member"].Remove(usSt);
                    m_root.CommitChanges();
                    _logger.WriteLog(LogLevelType.Info, $"AD RemoveEntitlement:{grpName} completed");
                    //ResetPath();
                }
                bret = true;
            }
            catch (COMException ex)
            {
                _logger.WriteLog(LogLevelType.Error, ex.Message, ex);

                if (string.Compare(ex.Message, OBJECTEXISTS, true) == 0)
                    bret = true;
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Error, ex.Message, ex);
                return false;
            }
            finally
            {
                m_root.Close();
            }

            if (bret == true)

                base.RemoveEntitlement(_userInfo, _entitlementType);

            return bret;
        }

        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            string domainString = string.Empty;
            ADSUserInfo ADSUserInfo = (ADSUserInfo)_userInfo;

            try
            {
                if (this.Connect())
                {
                    try
                    {
                        System.DirectoryServices.DirectorySearcher pADSearcher = default(System.DirectoryServices.DirectorySearcher);
                        System.DirectoryServices.SearchResult pADResult = default(System.DirectoryServices.SearchResult);
                        pADSearcher = new System.DirectoryServices.DirectorySearcher(m_root);
                        using (pADSearcher)
                        {
                            //'Setting the search parameters for the Active directory search 
                            pADSearcher.Filter = "sAMAccountName=" + ADSUserInfo.sAMAccountName.ToSecureLDAPFilter();
                            pADResult = pADSearcher.FindOne();
                            using (DirectoryEntry objLoginEntry = (pADResult != null) ? pADResult.GetDirectoryEntry() : null)
                            {
                                objLoginEntry.Properties["company"].Value = !string.IsNullOrEmpty(ADSUserInfo.company) ? ADSUserInfo.company : null;
                                objLoginEntry.Properties["department"].Value = !string.IsNullOrEmpty(ADSUserInfo.department) ? ADSUserInfo.department : null;
                                objLoginEntry.Properties["title"].Value = !string.IsNullOrEmpty(ADSUserInfo.title) ? ADSUserInfo.title : null;
                                objLoginEntry.Properties["pager"].Value = !string.IsNullOrEmpty(ADSUserInfo.pager) ? ADSUserInfo.pager : null;
                                //objLoginEntry.Properties["division"].Value = !string.IsNullOrEmpty(ADSUserInfo.division) ? ADSUserInfo.division : null;
                                objLoginEntry.Properties["businessCategory"].Value = !string.IsNullOrEmpty(ADSUserInfo.businessCategory) ? ADSUserInfo.businessCategory : null;
                                objLoginEntry.Properties["extensionAttribute1"].Value = !string.IsNullOrEmpty(ADSUserInfo.businessCategory) ? ADSUserInfo.businessCategory : null;

                                objLoginEntry.Properties["sn"].Value = !string.IsNullOrEmpty(ADSUserInfo.sn) ? ADSUserInfo.sn : null;
                                objLoginEntry.Properties["displayName"].Value = !string.IsNullOrEmpty(ADSUserInfo.displayName) ? ADSUserInfo.displayName : null;
                                objLoginEntry.Properties["description"].Value = !string.IsNullOrEmpty(ADSUserInfo.description) ? ADSUserInfo.description : null;

                                objLoginEntry.Properties["mail"].Value = !string.IsNullOrEmpty(ADSUserInfo.mail) ? ADSUserInfo.mail : null;
                                objLoginEntry.Properties["telephoneNumber"].Value = !string.IsNullOrEmpty(ADSUserInfo.telephoneNumber) ? ADSUserInfo.telephoneNumber : null;
                                objLoginEntry.Properties["mobile"].Value = !string.IsNullOrEmpty(ADSUserInfo.mobile) ? ADSUserInfo.mobile : null;
                                objLoginEntry.Properties["facsimileTelephoneNumber"].Value = !string.IsNullOrEmpty(ADSUserInfo.facsimileTelephoneNumber) ? ADSUserInfo.facsimileTelephoneNumber : null;

                                objLoginEntry.Properties["postalAddress"].Value = !string.IsNullOrEmpty(ADSUserInfo.postalAddress) ? ADSUserInfo.postalAddress : null;
                                objLoginEntry.Properties["postalCode"].Value = !string.IsNullOrEmpty(ADSUserInfo.postalCode) ? ADSUserInfo.postalCode : null;

                                objLoginEntry.Properties["physicalDeliveryOfficeName"].Value = !string.IsNullOrEmpty(ADSUserInfo.physicalDeliveryOfficeName) ? ADSUserInfo.physicalDeliveryOfficeName : null;

                                objLoginEntry.Properties["manager"].Value = !string.IsNullOrEmpty(ADSUserInfo.manager) ? ADSUserInfo.manager : null;
                                objLoginEntry.Properties["postOfficeBox"].Value = !string.IsNullOrEmpty(ADSUserInfo.postOfficeBox) ? ADSUserInfo.postOfficeBox : null;

                                objLoginEntry.Properties["extensionAttribute6"].Value = !string.IsNullOrEmpty(ADSUserInfo.extensionAttribute6) ? ADSUserInfo.extensionAttribute6 : null;
                                objLoginEntry.Properties["extensionAttribute2"].Value = !string.IsNullOrEmpty(ADSUserInfo.otherPager) ? ADSUserInfo.otherPager : null;
                                objLoginEntry.Properties["extensionAttribute3"].Value = !string.IsNullOrEmpty(ADSUserInfo.otherFascimileTelephoneNumber) ? ADSUserInfo.otherFascimileTelephoneNumber : null;
                                objLoginEntry.Properties["extensionAttribute8"].Value = !string.IsNullOrEmpty(ADSUserInfo.extensionAttribute8) ? ADSUserInfo.extensionAttribute8 : null;
                                objLoginEntry.Properties["extensionAttribute10"].Value = !string.IsNullOrEmpty(ADSUserInfo.extensionAttribute10) ? ADSUserInfo.extensionAttribute10 : null;
                                //extensionAttribute13 
                                if (!string.IsNullOrWhiteSpace(ADSUserInfo.extensionAttribute13))
                                    objLoginEntry.Properties["extensionAttribute13"].Value = ADSUserInfo.extensionAttribute13;

                                objLoginEntry.CommitChanges();

                                _logger.WriteLog(LogLevelType.Info, "Updated the department/OU in AD.", null);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteLog(LogLevelType.Exception, "Error while updating the department/OU in AD.", ex);
                        throw ex;
                    }

                    string edn = GetObjectDistinguishedName(objectClass.user, returnType.distinguishedName, ADSUserInfo.sAMAccountName, string.Empty);

                    using (DirectoryEntry eLocation = new DirectoryEntry())
                    {
                        ADHelper.SetDirectoryPath(eLocation, edn, EndpointConfiguration);

                        eLocation.Username = Configuration(EndpointKey.UserName);
                        eLocation.Password = Configuration(EndpointKey.Password);
                        eLocation.AuthenticationType = AuthenticationTypes.Secure | AuthenticationTypes.ServerBind;
                        using (DirectoryEntry nLocation = new DirectoryEntry())
                        {
                            ADHelper.SetDirectoryPath(nLocation, ADSUserInfo.ou, EndpointConfiguration);
                            nLocation.Username = Configuration(EndpointKey.UserName);
                            nLocation.Password = Configuration(EndpointKey.Password);
                            nLocation.AuthenticationType = AuthenticationTypes.Secure | AuthenticationTypes.ServerBind;
                            string newName = eLocation.Name;
                            eLocation.MoveTo(nLocation, newName);
                            nLocation.Close();
                        }
                        eLocation.Close();
                    }
                    using (DirectorySearcher ds = new DirectorySearcher(m_root))
                    {
                        //Fetch Distinguishedname after update. This is required because during Reorg Manager distinguishedName should be referenced to Non-Employee it get failed. To avoid that we should get the distinguishedName.
                        //Error will be Constraint violation occurred
                        ds.SearchScope = SearchScope.Subtree;
                        ds.Filter = "(&(objectClass=User)(sAMAccountName=" + ADSUserInfo.sAMAccountName.ToSecureLDAPFilter() + "))";
                        if (ds.FindOne() == null)
                        {
                            _logger.WriteLog(LogLevelType.Info, String.Format(" User: {0} Not found .", ADSUserInfo.sAMAccountName), null);
                        }
                        var result = ds.FindOne();
                        using (DirectoryEntry objLoginEntry = result.GetDirectoryEntry())
                        {
                            ADSUserInfo.distinguishedName = objLoginEntry.Properties["distinguishedName"].Value.ToString();
                            objLoginEntry.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Error, string.Format("Error occured while moving user {0} to OU {1}", ADSUserInfo.sAMAccountName, ADSUserInfo.ou), ex);
                return false;
            }
            _logger.WriteLog(LogLevelType.Info, string.Format("user {0} Moved to OU {1} successfully", ADSUserInfo.sAMAccountName, ADSUserInfo.ou), null);
            return true;
        }

        public override void MoveOU(IEndpointUserInfo _userInfo, string _ouName)
        {
            string domainString = string.Empty;
            ADSUserInfo ADSUserInfo = (ADSUserInfo)_userInfo;

            try
            {
                if (this.Connect())
                {
                    string edn = GetObjectDistinguishedName(objectClass.user, returnType.distinguishedName, ADSUserInfo.sAMAccountName, string.Empty);

                    using (DirectoryEntry eLocation = new DirectoryEntry())
                    {

                        ADHelper.SetDirectoryPath(eLocation, edn, EndpointConfiguration);

                        eLocation.Username = Configuration(EndpointKey.UserName);
                        eLocation.Password = Configuration(EndpointKey.Password);
                        eLocation.AuthenticationType = AuthenticationTypes.Secure | AuthenticationTypes.ServerBind;

                        using (DirectoryEntry nLocation = new DirectoryEntry())
                        {

                            ADHelper.SetDirectoryPath(nLocation, _ouName, EndpointConfiguration);
                            nLocation.Username = Configuration(EndpointKey.UserName);
                            nLocation.Password = Configuration(EndpointKey.Password);
                            nLocation.AuthenticationType = AuthenticationTypes.Secure | AuthenticationTypes.ServerBind;

                            string newName = eLocation.Name;
                            eLocation.MoveTo(nLocation, newName);
                            nLocation.Close();
                            eLocation.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Error, string.Format("Error occured while moving user {0} to OU {1}", ADSUserInfo.sAMAccountName, ADSUserInfo.ou), ex);
                throw ex;
            }
        }

        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            try
            {
                ADSUserInfo _adsUserInfo = (ADSUserInfo)_userInfo;

                if (this.Connect())
                {
                    using (DirectorySearcher ds = new DirectorySearcher(m_root))
                    {
                        ds.SearchScope = SearchScope.Subtree;
                        ds.Filter = "(&(objectClass=User)(sAMAccountName=" + _adsUserInfo.sAMAccountName.ToSecureLDAPFilter() + "))";
                        if (ds.FindOne() == null)
                        {
                            _logger.WriteLog(LogLevelType.Info, String.Format(" User: {0} Not found .", _adsUserInfo.sAMAccountName), null);
                        }
                        SearchResult result = ds.FindOne();
                        if (result == null)
                        {
                            _logger.WriteLog(LogLevelType.Info, String.Format(" User: {0} Not found  in SearchResult.", _adsUserInfo.sAMAccountName), null);
                        }
                        using (DirectoryEntry rejoinUser = result.GetDirectoryEntry())
                        {


                            if (_adsUserInfo.accountExpires.HasValue)
                            {
                                DateTime expiryDate = _adsUserInfo.accountExpires.Value;
                                if (expiryDate.Date != DateTime.MaxValue.Date)
                                {
                                    expiryDate = _adsUserInfo.accountExpires.Value.AddDays(1);
                                }

                                int year = expiryDate.Year;
                                int month = expiryDate.Month;
                                int day = expiryDate.Day;

                                rejoinUser.InvokeSet("AccountExpirationDate", new object[] { new DateTime(year, month, day) });
                            }
                            else
                            {
                                rejoinUser.Properties["AccountExpires"].Value = "0";
                            }

                            rejoinUser.CommitChanges();

                            m_root.Close();
                            rejoinUser.Close();
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception Ex)
            {
                _logger.WriteLog(LogLevelType.Error, Ex.ToString(), Ex);
                return false;
            }
        }

        public override bool UpdateADManagerAttribute(IEndpointUserInfo _userInfo)
        {
            try
            {
                ADSUserInfo ADSUserInfo = (ADSUserInfo)_userInfo;

                if (connectwithoutlog())
                {
                    using (DirectorySearcher ds = new DirectorySearcher(m_root))
                    {
                        ds.SearchScope = SearchScope.Subtree;
                        ds.Filter = "(&(objectClass=User)(sAMAccountName=" + ADSUserInfo.sAMAccountName.ToSecureLDAPFilter() + "))";
                        if (ds.FindOne() == null)
                        {
                            WriteProvisioningLog(LogLevelType.Info, String.Format(" User: {0} Not found .", ADSUserInfo.sAMAccountName), null);
                        }
                        SearchResult result = ds.FindOne();
                        if (result == null)
                        {
                            WriteProvisioningLog(LogLevelType.Info, String.Format(" User: {0} Not found  in SearchResult.", ADSUserInfo.sAMAccountName), null);
                        }
                        using (DirectoryEntry objLoginEntry = result.GetDirectoryEntry())
                        {
                            objLoginEntry.Properties["manager"].Value = !string.IsNullOrEmpty(ADSUserInfo.manager) ? ADSUserInfo.manager : null;
                            objLoginEntry.Properties["userPrincipalName"].Value = !string.IsNullOrEmpty(ADSUserInfo.sAMAccountName) ? string.Format("{0}@{1}", ADSUserInfo.sAMAccountName, MasterConfigHelper.ADUPNDomainName) : null;

                            objLoginEntry.Properties["extensionAttribute1"].Value = !string.IsNullOrEmpty(ADSUserInfo.businessCategory) ? ADSUserInfo.businessCategory : null;
                            objLoginEntry.Properties["extensionAttribute2"].Value = !string.IsNullOrEmpty(ADSUserInfo.otherPager) ? ADSUserInfo.otherPager : null;
                            objLoginEntry.Properties["extensionAttribute3"].Value = !string.IsNullOrEmpty(ADSUserInfo.otherFascimileTelephoneNumber) ? ADSUserInfo.otherFascimileTelephoneNumber : null;
                            objLoginEntry.Properties["extensionAttribute6"].Value = !string.IsNullOrEmpty(ADSUserInfo.extensionAttribute6) ? ADSUserInfo.extensionAttribute6 : null;
                            objLoginEntry.Properties["extensionAttribute8"].Value = !string.IsNullOrEmpty(ADSUserInfo.extensionAttribute8) ? ADSUserInfo.extensionAttribute8 : null;
                            //objLoginEntry.Properties["extensionAttribute10"].Value = !string.IsNullOrEmpty(ADSUserInfo.extensionAttribute10) ? ADSUserInfo.extensionAttribute10 : null;
                            //objLoginEntry.Properties["extensionAttribute13"].Value = !string.IsNullOrEmpty(ADSUserInfo.extensionAttribute13) ? ADSUserInfo.extensionAttribute13 : null;

                            if (!string.IsNullOrEmpty(ADSUserInfo.extensionAttribute10))
                            {
                                objLoginEntry.Properties["extensionAttribute10"].Value = ADSUserInfo.extensionAttribute10;
                            }

                            if (!string.IsNullOrEmpty(ADSUserInfo.extensionAttribute13))
                            {
                                objLoginEntry.Properties["extensionAttribute13"].Value = ADSUserInfo.extensionAttribute13;
                            }


                            //WriteProvisioningLog(LogLevelType.Info, "extensionAttribute10" + objLoginEntry.Properties["extensionAttribute10"].Value, null);
                            //don't cleanup the badge info if it is missing (can be due to onecard discovery issue, etc)
                            if (!string.IsNullOrEmpty(ADSUserInfo.extensionAttribute7))
                            {
                                objLoginEntry.Properties["extensionAttribute7"].Value = ADSUserInfo.extensionAttribute7;
                            }

                            objLoginEntry.Properties["company"].Value = !string.IsNullOrEmpty(ADSUserInfo.company) ? ADSUserInfo.company : null;
                            objLoginEntry.Properties["department"].Value = !string.IsNullOrEmpty(ADSUserInfo.department) ? ADSUserInfo.department : null;
                            objLoginEntry.Properties["title"].Value = !string.IsNullOrEmpty(ADSUserInfo.title) ? ADSUserInfo.title : null;
                            objLoginEntry.Properties["pager"].Value = !string.IsNullOrEmpty(ADSUserInfo.pager) ? ADSUserInfo.pager : null;
                            //objLoginEntry.Properties["division"].Value = !string.IsNullOrEmpty(ADSUserInfo.division) ? ADSUserInfo.division : null;
                            objLoginEntry.Properties["businessCategory"].Value = !string.IsNullOrEmpty(ADSUserInfo.businessCategory) ? ADSUserInfo.businessCategory : null;

                            objLoginEntry.Properties["sn"].Value = !string.IsNullOrEmpty(ADSUserInfo.sn) ? ADSUserInfo.sn : null;
                            objLoginEntry.Properties["displayName"].Value = !string.IsNullOrEmpty(ADSUserInfo.displayName) ? ADSUserInfo.displayName : null;
                            objLoginEntry.Properties["description"].Value = !string.IsNullOrEmpty(ADSUserInfo.description) ? ADSUserInfo.description : null;

                            objLoginEntry.Properties["mail"].Value = !string.IsNullOrEmpty(ADSUserInfo.mail) ? ADSUserInfo.mail : null;
                            objLoginEntry.Properties["telephoneNumber"].Value = !string.IsNullOrEmpty(ADSUserInfo.telephoneNumber) ? ADSUserInfo.telephoneNumber : null;
                            objLoginEntry.Properties["mobile"].Value = !string.IsNullOrEmpty(ADSUserInfo.mobile) ? ADSUserInfo.mobile : null;
                            objLoginEntry.Properties["facsimileTelephoneNumber"].Value = !string.IsNullOrEmpty(ADSUserInfo.facsimileTelephoneNumber) ? ADSUserInfo.facsimileTelephoneNumber : null;

                            objLoginEntry.Properties["postalAddress"].Value = !string.IsNullOrEmpty(ADSUserInfo.postalAddress) ? ADSUserInfo.postalAddress : null;
                            objLoginEntry.Properties["postalCode"].Value = !string.IsNullOrEmpty(ADSUserInfo.postalCode) ? ADSUserInfo.postalCode : null;

                            objLoginEntry.Properties["physicalDeliveryOfficeName"].Value = !string.IsNullOrEmpty(ADSUserInfo.physicalDeliveryOfficeName) ? ADSUserInfo.physicalDeliveryOfficeName : null;

                            objLoginEntry.CommitChanges();
                            //WriteProvisioningLog(LogLevelType.Info, $"displayName:{ADSUserInfo?.displayName} Manager:{ADSUserInfo?.manager} sAMAccountName:{ ADSUserInfo.sAMAccountName}");
                            objLoginEntry.Close();
                            m_root.Close();
                        }

                    }
                    try
                    {
                        m_root.Dispose();
                        m_adSearch.Dispose();
                        m_root = null;
                        m_adSearch = null;
                    }
                    catch (Exception)
                    {
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception Ex)
            {
                WriteProvisioningLog(LogLevelType.Error, "Error while updating manager attribute for " + _userInfo.AccountName, Ex);
                return false;
            }
        }

        public override bool UpdatePOBoxAttribute(IEndpointUserInfo _userInfo, bool add)
        {
            try
            {
                ADSUserInfo _adsUserInfo = (ADSUserInfo)_userInfo;

                if (connectwithoutlog())
                {
                    using (DirectoryEntry dir = GetDirectoryEntry(objectClass.user, _adsUserInfo.sAMAccountName, string.Empty))
                    {
                        if (add)
                        {
                            dir.Properties["postOfficeBox"].Value = MasterConfigHelper.ADPostOfficeBoxValue;
                            if (!string.IsNullOrWhiteSpace(_adsUserInfo.mail))
                            {
                                dir.Properties["mail"].Value = _adsUserInfo.mail;
                                dir.Properties["targetAddress"].Value = _adsUserInfo.mail;
                                try
                                {
                                    string SMTP = string.Format("SMTP:{0}", _adsUserInfo.mail);
                                    string sip = string.Format("sip:{0}", dir.Properties["userPrincipalName"].Value);

                                    dir.Properties["proxyAddresses"].Value = new object[] { sip, SMTP };

                                    dir.Properties["mailNickName"].Value = _adsUserInfo.mail.Split('@')[0];
                                }
                                catch (Exception ex)
                                {
                                    _logger.WriteLog(LogLevelType.Error, $"Error while updating the attribute proxyAddresses and mailNickName for { _userInfo.AccountName}", ex);
                                }
                            }
                        }
                        else
                        {
                            dir.Properties["postOfficeBox"].Value = null;
                            dir.Properties["manager"].Value = null;
                        }

                        dir.CommitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Error, "Error while updating the PO Box attribute for " + _userInfo.AccountName, ex);
                return false;
            }
            return true;
        }

        public override bool UpdateExterEmailAccecss(string accountName, string access)
        {
            try
            {
                if (connectwithoutlog())
                {
                    using (DirectoryEntry dir = GetDirectoryEntry(objectClass.user, accountName, string.Empty))
                    {
                        dir.Properties["extensionAttribute4"].Value = access;
                        dir.CommitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Error, "Error while updating the external email access for " + accountName, ex);
                return false;
            }
            return true;
        }

        string GetObjectDistinguishedName(objectClass objectCls, returnType returnValue, string objectName, string LdapDomain)
        {
            string distinguishedName = string.Empty;


            using (DirectorySearcher mySearcher = new DirectorySearcher(m_root))
            {
                switch (objectCls)
                {
                    case objectClass.user:
                        mySearcher.Filter = "(&(objectClass=user)(|(cn=" + objectName.ToSecureLDAPFilter() + ")(sAMAccountName=" + objectName.ToSecureLDAPFilter() + ")))";
                        break;
                    case objectClass.group:
                        mySearcher.Filter = "(&(objectClass=group)(|(cn=" + objectName.ToSecureLDAPFilter() + ")(dn=" + objectName.ToSecureLDAPFilter() + ")))";
                        break;
                    case objectClass.computer:
                        mySearcher.Filter = "(&(objectClass=computer)(|(cn=" + objectName.ToSecureLDAPFilter() + ")(dn=" + objectName.ToSecureLDAPFilter() + ")))";
                        break;
                }
                SearchResult result = mySearcher.FindOne();

                if (result == null)
                {
                    throw new NullReferenceException
                    ("unable to locate the distinguishedName for the object " +
                    objectName + " in the " + LdapDomain + " domain");
                }
                using (DirectoryEntry directoryObject = result.GetDirectoryEntry())
                {
                    if (returnValue.Equals(returnType.distinguishedName))
                    {
                        distinguishedName = directoryObject.Properties["distinguishedName"].Value.ToString();
                    }
                    if (returnValue.Equals(returnType.ObjectGUID))
                    {
                        distinguishedName = directoryObject.Guid.ToString();
                    }
                    return distinguishedName;
                }
            }
        }

        void ResetPath()
        {
            try
            {

                List<string> lstDomain = null;

                m_root = new DirectoryEntry();
                if (Configuration(EndpointKey.DomainName) != string.Empty)
                {
                    lstDomain = Configuration(EndpointKey.DomainName).Split(new char[] { '.' }).ToList();
                }

                StringBuilder bldr = new StringBuilder();
                int index = 0;
                string domainString = string.Empty;

                if (!string.IsNullOrEmpty(Configuration(EndpointKey.Ou)))
                    bldr.AppendFormat("OU={0},", Configuration(EndpointKey.Ou));


                foreach (string str in lstDomain)
                {

                    //bldr.AppendFormat("cn=Users;DC={0},", str);
                    bldr.AppendFormat("DC={0},", str);

                    index = index + 1;

                    if (lstDomain.Count == index)
                        domainString = bldr.ToString().Remove(bldr.ToString().Length - 1);
                }
                ADHelper.SetDirectoryPath(m_root, domainString, EndpointConfiguration);


                m_root.Username = Configuration(EndpointKey.IMUserName);
                m_root.Password = Configuration(EndpointKey.IMUserPassword);
            }
            catch (Exception)
            {
                //Log the exception.
                throw;
            }
        }

        string getDomainString(EndPoint _ep)
        {
            StringBuilder bldr = new StringBuilder();
            int index = 0;
            string domainString = string.Empty;
            try
            {
                List<string> lstDomain = null;


                if (Configuration(EndpointKey.DomainName) != string.Empty)
                {
                    lstDomain = Configuration(EndpointKey.DomainName).Split(new char[] { '.' }).ToList();


                }
                foreach (string str in lstDomain)
                {

                    bldr.AppendFormat("DC={0},", str);

                    index = index + 1;

                    if (lstDomain.Count == index)
                        domainString = bldr.ToString().Remove(bldr.ToString().Length - 1);
                }
            }
            catch { }
            return domainString;
        }

        string getDomainString(ADSUserInfo adsUserInfo)
        {
            string domainString = string.Empty;
            try
            {
                domainString = adsUserInfo != null ? adsUserInfo.distinguishedName : string.Empty;
                if (!string.IsNullOrEmpty(domainString))
                {
                    int startIndx = domainString.IndexOf("OU", 0, domainString.Length, StringComparison.CurrentCultureIgnoreCase);
                    if (startIndx >= 0 && domainString.Length > startIndx)
                    {
                        domainString = domainString.Substring(startIndx, domainString.Length - startIndx);
                    }
                }
            }
            catch { }
            return domainString;
        }

        public override bool AssignRightsToHomeFolder(string samAccountName)
        {
            try
            {
                WriteProvisioningLog(LogLevelType.Info, $"Home Folder Rights Assign Started for User { samAccountName}", null);

                ADSUserInfo adUser = ADSUserInfo.GetBysAMAccountName(samAccountName);

                if (adUser == null)
                {
                    WriteProvisioningLog(LogLevelType.Warning, "Could Not find User in AD to Assign Rights To Home Folder:" + samAccountName, null);
                    return false;
                }

                DirectoryInfo Info = new DirectoryInfo(adUser.homeDirectory);
                DirectorySecurity Security = Info.GetAccessControl(AccessControlSections.Access);

                InheritanceFlags iFlags = InheritanceFlags.ObjectInherit;
                iFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
                FileSystemRights Rights = (FileSystemRights)0;
                Rights = FileSystemRights.FullControl;
                AccessControlType aceType = AccessControlType.Allow;

                FileSystemAccessRule AccessRule = new FileSystemAccessRule(samAccountName, Rights,
                                                           iFlags,
                                                           PropagationFlags.NoPropagateInherit,
                                                           aceType);

                Security.AddAccessRule(AccessRule);
                Info.SetAccessControl(Security);

                WriteProvisioningLog(LogLevelType.Info, "Home Folder Rights Assigned for User :" + samAccountName, null);

            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Exception, "Error While Assigning Rights to Home Folder", ex);

                return false;
            }
            return true;
        }
        public bool CreateHomeFolder(string homefolderPath, string samaccountName)
        {
            try
            {
                _logger.WriteLog(LogLevelType.Info, $"User:{samaccountName}, HomeFolderPath: {homefolderPath}", null);

                string folderName = samaccountName;
                string username = Configuration(EndpointKey.UserName);
                string password = Configuration(EndpointKey.Password);

                IntPtr admin_token = default(IntPtr);
                WindowsIdentity wid_current = WindowsIdentity.GetCurrent();
                WindowsIdentity wid_admin = null;
                WindowsImpersonationContext wic = null;

                if (LogonUser(username, Configuration(EndpointKey.DomainName), password, 9, 0, ref admin_token) != 0)
                {
                    wid_admin = new WindowsIdentity(admin_token);
                    wic = wid_admin.Impersonate();
                    try
                    {
                        string directoryName = string.Format(@"{0}\{1}", homefolderPath, folderName);

                        _logger.WriteLog(LogLevelType.Info, "Trying to Create Directory" + directoryName, null);

                        Directory.CreateDirectory(directoryName);

                        _logger.WriteLog(LogLevelType.Info, "Completed to Creating Directory" + directoryName, null);

                        if (Directory.Exists(directoryName))
                        {
                            DisableInheritancePermission(directoryName);
                        }

                        _logger.WriteLog(LogLevelType.Info, "Home Folder Created for User :" + samaccountName, null);
                    }
                    catch (Exception fex)
                    {
                        _logger.WriteLog(LogLevelType.Exception, "Error While Creating Home Folder", fex);
                        return false;
                    }
                }
                else
                {
                    _logger.WriteLog(LogLevelType.Error, string.Format("Logon Failed for User {0} while Home Folder Creation", username), null);
                }

                try
                {
                    _logger.WriteLog(LogLevelType.Info, "Started Home Folder Rights Assigned for User :" + samaccountName, null);
                    var sid = GetObjectSidByAccountName(samaccountName);
                    if (!string.IsNullOrWhiteSpace(sid))
                    {
                        SecurityIdentifier secIdentifierSid = new SecurityIdentifier(sid);
                        DirectoryInfo Info = new DirectoryInfo(string.Format(@"{0}\{1}", homefolderPath, folderName));
                        DirectorySecurity Security = Info.GetAccessControl(AccessControlSections.Access);


                        InheritanceFlags iFlags = InheritanceFlags.ObjectInherit;
                        iFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
                        FileSystemRights Rights = (FileSystemRights)0;
                        Rights = FileSystemRights.FullControl;
                        AccessControlType aceType = AccessControlType.Allow;

                        FileSystemAccessRule AccessRule = new FileSystemAccessRule(secIdentifierSid, Rights,
                                                                   iFlags,
                                                                   PropagationFlags.NoPropagateInherit,
                                                                   aceType);

                        Security.AddAccessRule(AccessRule);

                        Info.SetAccessControl(Security);

                        _logger.WriteLog(LogLevelType.Info, "Home Folder Rights Assigned for User :" + samaccountName, null);
                    }
                    else
                    {
                        _logger.WriteLog(LogLevelType.Info, "Home Folder Rights not Assigned for User.objectSid is not found in AD account :" + samaccountName, null);
                    }

                }
                catch (Exception aex)
                {
                    _logger.WriteLog(LogLevelType.Exception, "Error While Assigning rights to Home Folder", aex);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(LogLevelType.Error, $"Exception in CreateHomeFolder for the user {samaccountName}", ex);
                return false;
            }

            return true;
        }
        private string GetObjectSidByAccountName(string accountName)
        {
            try
            {
                if (this.Connect())
                {
                    using (DirectoryEntry dir = GetDirectoryEntry(objectClass.user, accountName, string.Empty))
                    {
                        string objectSid = ADHelper.GetProperty_SID(dir, ADProperties.objectSid);
                        return objectSid;
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Error, "Error while getting ObjectSid on Home Folder Creation", ex);
                return string.Empty;
            }
        }
        bool DisableInheritancePermission(string folderPath)
        {
            var isInheritencePermissionMaintained = false; // remove all inheritance permission if set to "false"
            var isProtected = true;
            DirectorySecurity directorySecurity = Directory.GetAccessControl(folderPath);
            directorySecurity.SetAccessRuleProtection(isProtected, isInheritencePermissionMaintained);
            Directory.SetAccessControl(folderPath, directorySecurity);
            return true;
        }

        protected override bool UnLockAccountRecon(IEndpointUserInfo _userInfo)
        {
            try
            {
                var acctRecon = AccountReconcilation.GetByUser(_userInfo.EndPointSystemId, _userInfo.Id);
                if (null == acctRecon)
                    return false;
                else
                {
                    if (this.CurrentRequest != null && this.CurrentRequest.AdHocAccessRequest != null && this.CurrentRequest.AdHocAccessRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        acctRecon.UserId = this.CurrentRequest.AdHocAccessRequest.RequesteeUserId;
                        var user = DI.Chakra.DomainModel.User.GetById(this.CurrentRequest.AdHocAccessRequest.RequesteeUserId);
                        var aduserInfo = (ADSUserInfo)_userInfo;
                        if (aduserInfo != null && user != null && this.CurrentRequest.AdHocAccessRequest.AccountType != AccountType.User)
                        {
                            aduserInfo.physicalDeliveryOfficeName = user.EmployeeId;
                        }
                    }

                    acctRecon.IsDeleted = false;
                    acctRecon.IsLocked = false;
                    acctRecon.FullName = !string.IsNullOrWhiteSpace(_userInfo.DisplayName) ? _userInfo.DisplayName : _userInfo.AccountName;
                    if(CurrentRequest != null)
                    {
                        acctRecon.ValidFromDate = CurrentRequest.ActivationDate;
                        acctRecon.ValidToDate = GetValidTo(CurrentRequest.UserId);
                    }
                    acctRecon.Save(MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error occured while unlocking the account - {0} in system - {1}", _userInfo.AccountName, this.Endpoint.DisplayName), ex);
            }

            return true;
        }

        public override bool IsUserPropertyExists(string propertyName, string propertyValue, string accountName)
        {
            bool isExists = false;
            try
            {

                if (!string.IsNullOrWhiteSpace(propertyName) && !string.IsNullOrWhiteSpace(propertyValue))
                {
                    if (this.Connect() && m_root != null)
                    {
                        using (DirectorySearcher search = new DirectorySearcher(m_root))
                        {
                            search.Filter = "(distinguishedName=" + propertyValue.ToSecureLDAPFilter() + ")";
                            search.PropertiesToLoad.Add("distinguishedName");
                            SearchResult result = search.FindOne();
                            if (result != null)
                            {
                                isExists = true;
                            }
                            else
                            {
                                throw new Exception($"OU {propertyName}:{propertyValue} does not exists");
                            }
                        }
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"unable to connect to endpoint");
                    }
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Propertry - {propertyName}:{propertyValue} empty");
                }
            }
            catch (Exception ex)
            {
                isExists = false;
                _idmsLogger.WriteLog(LogLevelType.Error, $"IsUserPropertyExists {propertyName}:{propertyValue}", ex);
            }
            return isExists;
        }
        public override bool UpdateSpecificAttribute(IEndpointUserInfo userInfo, List<KeyValuePair<string, string>> keyValuePair)
        {
            bool isUpdated = false;
            try
            {
                ADSUserInfo adsUserinfo = userInfo as ADSUserInfo;
                this.Connect();
                using (DirectoryEntry dir = GetDirectoryEntry(objectClass.user, userInfo.AccountName, string.Empty))
                {
                    foreach (var item in keyValuePair)
                    {
                        string propName = item.Key;
                        try
                        {
                            if (dir.Properties[propName] != null)
                            {
                                dir.Properties[propName].Value = item.Value;
                                _idmsLogger.WriteLog(LogLevelType.Info, $"UpdateSpecificAttribute Property: {propName} Value:{item.Value} for Account:{adsUserinfo.AccountName}");
                            }
                            else
                            {
                                _idmsLogger.WriteLog(LogLevelType.Info, $"UpdateSpecificAttribute Property: {propName} does not exists for Account:{adsUserinfo.AccountName}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Exception, $"UpdateSpecificAttribute exception.{ex.Message} for Account:{adsUserinfo.AccountName}", ex);
                        }
                    }
                    dir.CommitChanges();
                    isUpdated = true;
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"UpdateSpecificAttribute exception occured.{ex.ToString()}", ex);
            }
            return isUpdated;
        }

        public override bool IsUserExists(string userName)
        {
            try
            {
                if (this.Connect())
                {
                    using (DirectorySearcher mySearcher = new DirectorySearcher(m_root))
                    {
                        mySearcher.Filter = "(&(objectClass=user)(|(cn=" + userName.ToSecureLDAPFilter() + ")(sAMAccountName=" + userName.ToSecureLDAPFilter() + ")))";
                        return mySearcher.FindOne() != null;
                    }
                }
                else
                {
                    throw new Exception("Cannot connect to AD");
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"IsUserExists exception occured.{ex.ToString()}", ex);
                return false;
            }
        }
    }
}
