using DI.Chakra.Connector.LotusNotes;
using DI.Chakra.Connector.LotusNotes.Orchestrators;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.DomainModel;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using Domino;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;

namespace DI.Chakra.Connector
{
    public class LotusNotesConnector : ConnectorBase
    {
        #region Members

        private NotesSession _lotusNotesServerSession = null;
        private NotesDatabase _serverDatabase = null;
        private NotesView _LotusNotesView = null;
        private EndpointConfiguration m_lnsConfig = null;

        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicGroups = new Dictionary<long, string>();
        private Dictionary<string, List<object>> dicGroupUsers = new Dictionary<string, List<object>>();

        public EndpointConfiguration LotusNotesConfiguration
        {
            get { return m_lnsConfig; }
            set { this.m_lnsConfig = value; }
        }

        #endregion

        #region Core

        public LotusNotesConnector(EndPoint endpoint, AccessRequest currentRequest)
        {
            this.EndPoint = endpoint;
            this.CurrentRequest = currentRequest;
            var lnsConfig = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            lnsConfig.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.KeyId == Convert.ToInt64(EndpointKey.Password))
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.LotusNotesConfiguration = lnsConfig;
        }

        public override IMProcessor IMProcessor
        {
            get
            {
                LotusNotes.IMProcessor processor = new LotusNotes.IMProcessor();
                processor.EndPoint = this.EndPoint;
                processor.LNSConfiguration = this.LotusNotesConfiguration;

                return processor;
            }


        }

        #endregion

        #region Discovery

        public override bool Connect()
        {
            string DominoServerName = string.Empty;
            try
            {
                DominoServerName = Configuration(EndpointKey.IPAddress); // Server name is replaced as IPaddress;
                string NotesPassword = Configuration(EndpointKey.Password);

                _lotusNotesServerSession = new Domino.NotesSession();
                _lotusNotesServerSession.Initialize(NotesPassword);

                _serverDatabase = _lotusNotesServerSession.GetDatabase(DominoServerName, "names.nsf", false);

                if (_serverDatabase == null)
                {
                    WriteDiscoveryLog(LogLevelType.Fatal, string.Format("Lotus Notes Server {0} is not pinging", DominoServerName));
                    return false;
                }
                return true;
            }

            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while connecting to domino server {0}", DominoServerName), ex);
                throw ex;
            }
        }
        private string Configuration(EndpointKey Key)
        {
            var config = LotusNotesConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }
        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            bool isPasswordChanged = false;
            try
            {

                #region Get UserInfo
                LotusUserInfo lnsUsr = null;
                User mUsr = User.GetById(DIUserId);
                List<LotusUserInfo> usrLst = LotusUserInfo.GetAll() as List<LotusUserInfo>;
                if (UserName.Contains("@"))
                    UserName = UserName.Split('@')[0];
                if (usrLst != null)
                {
                    usrLst = (from tmpusr in usrLst
                              where tmpusr.ShortName == UserName && tmpusr.EndPointSystemId == this.EndPoint.Id
                              select tmpusr).ToList<LotusUserInfo>();

                    lnsUsr = usrLst != null && usrLst.Count > 0 ? usrLst[0] : null;
                }

                if (mUsr == null || lnsUsr == null)
                    return isPasswordChanged;
                #endregion

                if (this.Connect())
                {
                    if (EndPoint == null)
                    {
                        EndPoint = DI.Chakra.DomainModel.EndPoint.GetById(lnsUsr.EndPointSystemId);
                    }

                    string tempIdFilePath = MasterConfigHelper.LNSTempIdPath;

                    tempIdFilePath = string.Format(@"{0}\{1}.id", tempIdFilePath, lnsUsr.ShortName);

                    LNPasswordHelper lnPwdReset = new LNPasswordHelper();
                    lnPwdReset.DBName = string.Empty;
                    lnPwdReset.ServerName = lnsUsr.MailServer;
                    lnPwdReset.UserName = lnsUsr.Owner;
                    lnPwdReset.TempIdFilePath = tempIdFilePath;
                    lnPwdReset.AdminIdPath = Configuration(EndpointKey.IDVaultAdminFile);
                    lnPwdReset.AdminUserName = Configuration(EndpointKey.IDVaultAdmin);
                    lnPwdReset.AdminPassword = Configuration(EndpointKey.Password);
                    lnPwdReset.NewPasswordToBeChanged = Password;

                    string tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8);

                    isPasswordChanged = lnPwdReset.ChangePassword_Client(tempPassword);

                    //if (isPasswordChanged = false)
                    //{
                    //    bool bDigestCleared = ClearPasswordDigest(lnsUsr.ShortName);

                    //    if (bDigestCleared)
                    //    {
                    //        tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
                    //        isPasswordChanged = lnPwdReset.ChangePassword_Client(tempPassword);
                    //    }
                    //}
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Info, string.Format("Client Password Changed"));

                    if (isPasswordChanged == false)
                        return false;

                    try
                    {
                        this.Connect(lnsUsr.MailServer);

                        NotesAdministrationProcess nap = _lotusNotesServerSession.CreateAdministrationProcess(lnsUsr.MailServer);


                        string httpRetMsg = nap.ChangeHTTPPassword(lnsUsr.Owner, tempPassword, Password);


                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Error, string.Format("HTTP Password changed Return Code : {0}", httpRetMsg));

                    }
                    catch (Exception ex)
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Error, string.Format("HTTP Password change Failed"), ex);

                    }
                }
            }
            catch (Exception exx)
            {

                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Error, string.Format("HTTP Password change Failed"), exx);

                return false;
            }


            return isPasswordChanged;
        }
        public bool Connect(string serverName)
        {
            string DominoServerName = string.Empty;
            try
            {
                this.LotusNotesConfiguration = EndpointConfiguration.GetByEndpointSystemId(EndPoint.Id);
                LotusNotesConfiguration.ConfigurationDetails.ForEach(lq =>
                {
                    if (lq.KeyId == Convert.ToInt64(EndpointKey.Password))
                    {
                        lq.Value = RSAHelper.DecryptData(lq.Value);
                    }
                });
                DominoServerName = serverName;
                string NotesPassword = Configuration(EndpointKey.Password);

                _lotusNotesServerSession = new Domino.NotesSession();
                _lotusNotesServerSession.Initialize(NotesPassword);

                _serverDatabase = _lotusNotesServerSession.GetDatabase(DominoServerName, "names.nsf", false);

                if (_serverDatabase == null)
                {
                    WriteDiscoveryLog(LogLevelType.Fatal, string.Format("Lotus Notes Server {0} is not pinging", DominoServerName));
                    return false;
                }
                return true;

            }

            catch (Exception ex)
            {

                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while connecting to domino server {0}", DominoServerName), ex);
                throw ex;

            }
        }

        public override void GetData()
        {
            IList<LotusUserInfo> listLnsUsers = null;
            try
            {
                if (this.Connect())
                {
                    long DiscoveryProcessId = base.DiscoveryProcess.Id;

                    List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);
                    bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_LNS_User) != null;
                    bool IsGroupSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_LNS_Group) != null;
                    bool IsCertifierSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_LNS_Certifier) != null;

                    if (IsUserSelected || IsGroupSelected)
                    {
                        DiscoverUsers();
                        DiscoverGroups();
                        SaveUserGroup();

                        //Update recon table
                        this.UpdateEntitlementRecon();
                    }

                    if (IsCertifierSelected)
                        DiscoverCertifiers();

                    listLnsUsers = LotusUserInfo.GetAll();
                    //LotusUserInfo.GetAllMasterUsers(listLnsUsers, LotusNotesConfiguration.MapAll.Value);
                    //TODO: listLnsUsers should be retrieved regardless of populate.
                    //LotusUserInfo.MapWithMasterUser(EndPoint, LotusNotesConfiguration.MapAll != null ? LotusNotesConfiguration.MapAll : false);
                    LotusUserInfo.MapWithMasterUserForReconTable(EndPoint, LotusNotesConfiguration.MapAll != null ? LotusNotesConfiguration.MapAll : false, DiscoveryProcessId);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, ex.InnerException.ToString(), ex);
            }

        }

        private void DiscoverUsers()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Users"));

            IList<LotusUserInfo> list = this.GetUsers(false);
            LotusUserInfo.SaveUsers(list, EndPoint, MasterConfigHelper.ServiceUser);
            AccountReconcilation.ProcessDeletedAccounts(this.EndPoint.Id, this.Process, MasterConfigHelper.ServiceUser);
            var userlist = LotusUserInfo.GetDeletedUsers(this.EndPoint.Id, this.Process);

            LotusUserInfo.RemoveLNSUser(userlist);
            dicUsers = LotusUserInfo.GetByEndpointSystemIdWithOwner(this.EndPoint.Id);
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Users"));
        }
        private IList<LotusUserInfo> GetUsers(bool forDelUser)
        {
            NotesViewEntryCollection notesViewCollection = null;

            IList<LotusUserInfo> list = new List<LotusUserInfo>();

            // LotusUserHistoryInfo lotusUserInfo = null;
            UserInfoOrchestrator orchestrator = new UserInfoOrchestrator();

            orchestrator.EndpointSystemId = (int)this.EndPoint.Id;
            try
            {
                _LotusNotesView = _serverDatabase.GetView("$People");

                notesViewCollection = _LotusNotesView.AllEntries;



                for (int rowCount = 1; rowCount <= notesViewCollection.Count; rowCount++)
                {
                    NotesViewEntry viewEntry = notesViewCollection.GetNthEntry(rowCount);
                    NotesDocument document = viewEntry.Document;
                    object documentItems = document.Items;
                    Array itemArray1 = (System.Array)documentItems;
                    LotusUserInfo lotusUserInfo = new LotusUserInfo();
                    List<string> filds = new List<string>();
                    for (int itemCount = 0; itemCount < itemArray1.Length; itemCount++)
                    {
                        NotesItem notesItem =
                        (Domino.NotesItem)itemArray1.GetValue(itemCount);

                        //compare field value with specific value entered by user
                        if (notesItem.Text != null)
                        {


                            NotesItem searchedNotesItem = (Domino.NotesItem)itemArray1.GetValue(itemCount);
                            string FieldName = searchedNotesItem.Name.ToString();

                            lotusUserInfo.EndPointSystemId = EndPoint.Id;
                            if (!forDelUser)
                            {
                                if (searchedNotesItem.Name == "FullName")
                                {
                                    string fullname = searchedNotesItem.Text.Trim();
                                    if (fullname.Contains("CN="))
                                    {
                                        if (fullname.IndexOf('/') != -1)
                                        {
                                            lotusUserInfo.FullName = fullname.Substring(3, fullname.IndexOf('/') - 3);
                                        }
                                    }
                                    else
                                    {
                                        lotusUserInfo.FullName = fullname;
                                    }
                                }
                            }
                            else
                            {
                                if (searchedNotesItem.Name == "FullName")
                                {
                                    lotusUserInfo.FullName = searchedNotesItem.Text.Trim();
                                }
                            }
                            if (searchedNotesItem.Name == "FirstName")
                                lotusUserInfo.Name = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "InternetAddress")
                                lotusUserInfo.InternetAddress = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "LastName")
                                lotusUserInfo.LastName = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "ListName")
                                lotusUserInfo.ListName = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "Members")
                                lotusUserInfo.Members = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "MailServer")
                                lotusUserInfo.MailServer = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "MailDomain")
                                lotusUserInfo.MailDomain = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "MailAddress")
                                lotusUserInfo.MailAddress = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "ShortName")
                                lotusUserInfo.ShortName = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "Location")
                                lotusUserInfo.Location = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "EncryptIncomingMail")
                                lotusUserInfo.EncryptIncomingMail = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "CalendarDomain")
                                lotusUserInfo.CalendarDomain = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "CompanyName")
                                lotusUserInfo.CompanyName = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "HTTPPassword")
                                lotusUserInfo.HTTPPassword = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "Department")
                                lotusUserInfo.Department = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "JobTitle")
                                lotusUserInfo.JobTitle = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "Owner")
                                lotusUserInfo.Owner = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "PasswordChangeDate")
                                lotusUserInfo.PasswordChangedDate = Convert.ToDateTime(searchedNotesItem.Text.Trim().ToString());
                            if (searchedNotesItem.Name == "NewMailServer")
                                lotusUserInfo.NewMailServer = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "MailFile")
                                lotusUserInfo.MailFile = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "MessageStorage")
                                lotusUserInfo.MessageStorage = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "ServerName")
                                lotusUserInfo.ServerName = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "AltFullNameLanguage")
                                lotusUserInfo.AltFullNameLanguage = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "QuickPlace")
                                lotusUserInfo.QuickPlace = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "SametimeServer")
                                lotusUserInfo.SametimeServer = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "Type")
                                lotusUserInfo.Type = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "MailSystem")
                                lotusUserInfo.MailSystem = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "AdminpOldWebName")
                                lotusUserInfo.AdminpOldWebName = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "HTTPPasswordChangeDate")
                                lotusUserInfo.HTTPPasswordChangeDate = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "HTTPPasswordForceChange")
                                lotusUserInfo.HTTPPasswordForceChange = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "Policy")
                                lotusUserInfo.Policy = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "LTPA_UsrNm")
                                lotusUserInfo.LTPA_UsrNm = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "DB2UserName")
                                lotusUserInfo.DB2UserName = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "CryptoCap")
                                lotusUserInfo.CryptoCap = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "OfficePhoneNumber")
                                lotusUserInfo.OfficePhoneNumber = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "OfficeFAXPhoneNumber")
                                lotusUserInfo.OfficeFAXPhoneNumber = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "AvailableForDirSync")
                                lotusUserInfo.AvailableForDirSync = string.Compare(searchedNotesItem.Text, "1", true) == 0 ? true : false;
                            if (searchedNotesItem.Name == "MailExtAccess")
                                lotusUserInfo.MailExternalAccess = string.Compare(searchedNotesItem.Text, "No", true) == 0 ? false : true;
                            if (searchedNotesItem.Name == "MailChargeable")
                                lotusUserInfo.MailChargeable = string.Compare(searchedNotesItem.Text, "No", true) == 0 ? false : true;
                            if (searchedNotesItem.Name == "Comment")
                                lotusUserInfo.Comment = searchedNotesItem.Text.Trim().ToString();
                            if (searchedNotesItem.Name == "IDMPolicy")
                                lotusUserInfo.IDMPolicy = string.Compare(searchedNotesItem.Text, "Yes", true) == 0 ? false : true;
                            if (searchedNotesItem.Name == "EmployeeID")
                                lotusUserInfo.NRIC = searchedNotesItem.Text.Trim().ToString();

                        }
                    }

                    list.Add(lotusUserInfo);

                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Discovery Failed", ex);

                throw;
            }
            finally
            {

            }
            return list.Distinct().ToList();
        }

        private void DiscoverGroups()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  Groups"));
            IList<LotusGroupInfo> listgroup = this.GetGroups();
            LotusGroupInfo.SaveGroups(listgroup, EndPoint, MasterConfigHelper.ServiceUser);
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering  Groups"));
            dicGroups = LotusGroupInfo.GetWithOwner(this.EndPoint.Id);
        }
        private IList<LotusGroupInfo> GetGroups()
        {

            object[] documentItems = null;

            IList<LotusGroupInfo> list = new List<LotusGroupInfo>();
            LotusGroupInfo lotusGroupInfo = null;
            GroupInfoOrchestrator orchestrator = new GroupInfoOrchestrator();

            orchestrator.EndpointSystemId = (int)this.EndPoint.Id;

            _LotusNotesView = _serverDatabase.GetView("Groups");
            NotesViewEntryCollection notesViewCollectionGroup = _LotusNotesView.AllEntries;

            for (int rowCount = 1; rowCount <= notesViewCollectionGroup.Count; rowCount++)
            {
                NotesViewEntry viewEntry = notesViewCollectionGroup.GetNthEntry(rowCount);
                NotesDocument document = viewEntry.Document;
                try
                {
                    lotusGroupInfo = new LotusGroupInfo();
                    lotusGroupInfo = orchestrator.PopulateGroup(viewEntry);
                    list.Add(lotusGroupInfo);

                    if (document.HasItem("Members"))
                    {
                        documentItems = (object[])document.GetItemValue("Members");

                        if (documentItems.Count() > 0)
                        {
                            if (!dicGroupUsers.ContainsKey(lotusGroupInfo.Group))
                            {
                                dicGroupUsers.Add(lotusGroupInfo.Group, documentItems.ToList());
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {

                }
            }

            return list;
        }

        private void DiscoverCertifiers()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Certifiers"));
            _LotusNotesView = _serverDatabase.GetView(@"Server\Certificates");
            NotesViewEntryCollection notesViewCollection = _LotusNotesView.AllEntries;
            IList<LotusCertifierInfo> certifierList = this.GetCertifiers(notesViewCollection);
            LotusCertifierInfo.SaveCertifiers(certifierList, EndPoint, MasterConfigHelper.ServiceUser);
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Certifiers"));
        }
        private IList<LotusCertifierInfo> GetCertifiers(NotesViewEntryCollection notesViewCollection)
        {
            IList<LotusCertifierInfo> list = new List<LotusCertifierInfo>();
            try
            {
                for (int rowCount = 1; rowCount <= notesViewCollection.Count; rowCount++)
                {
                    NotesViewEntry viewEntry = notesViewCollection.GetNthEntry(rowCount);
                    NotesDocument document = viewEntry.Document;
                    object documentItems = document.Items;
                    Array itemArray1 = (System.Array)documentItems;
                    LotusCertifierInfo lotusCertifierInfo = new LotusCertifierInfo();
                    List<string> filds = new List<string>();
                    for (int itemCount = 0; itemCount < itemArray1.Length; itemCount++)
                    {
                        NotesItem notesItem = (Domino.NotesItem)itemArray1.GetValue(itemCount);

                        if (notesItem.Text != null)
                        {
                            NotesItem searchedNotesItem = (Domino.NotesItem)itemArray1.GetValue(itemCount);
                            string FieldName = searchedNotesItem.Name.ToString();
                            string value = searchedNotesItem.Text.ToString();

                            //lotusCertifierInfo.CertifierId = 1
                            if (searchedNotesItem.Name == "Type")
                                lotusCertifierInfo.CertifierType = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "$UpdatedBy")
                                lotusCertifierInfo.Owner = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "FullName")
                                lotusCertifierInfo.FullName = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "IssuedTo")
                                lotusCertifierInfo.FormIssuedBy = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "MailSystem")
                                lotusCertifierInfo.MailDomain = searchedNotesItem.Text.Trim();
                            if (searchedNotesItem.Name == "CertificateType")
                                lotusCertifierInfo.CertificateType = (int)Convert.ChangeType(searchedNotesItem.Text.Trim(), typeof(int));
                        }
                    }
                    lotusCertifierInfo.EndPointSystemId = (int)EndPoint.Id;
                    lotusCertifierInfo.Version = Convert.ToInt16(VersionNo.Available);
                    list.Add(lotusCertifierInfo);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return list.Distinct().ToList();
        }

        private void SaveUserGroup()
        {
            try
            {
                LotusUserInfo.SaveUserGroupMap(dicUsers, dicGroups, dicGroupUsers, this.EndPoint.Id);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Error while saving user group mapping.", ex);
            }
        }
        private void UpdateEntitlementRecon()
        {
            try
            {
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    var lnsUserInfo = LotusUserInfo.GetByIdIncludeLNSGroups(item.Key);

                    if (lnsUserInfo.LotusGroupInfos != null)
                    {
                        foreach (LotusGroupInfo entitlement in lnsUserInfo.LotusGroupInfos)
                        {
                            var acctRecon = AccountReconcilation.GetByUser(lnsUserInfo.EndPointSystemId, lnsUserInfo.Id);
                            var entRecon = EntitlementReconcilation.GetByUser(lnsUserInfo.Id, lnsUserInfo.EndPointSystemId, entitlement.Group.Trim(), EntitlementType.Group);

                            if (entRecon == null)
                            {
                                var entitlements = (lnsUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Group, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                entRecon.EntitlementId = entitlement.Id;
                                entRecon.EntitlementDescription = entitlement.Description;
                                entRecon.Entitlement = entitlement.Group.Trim();
                                entRecon.ExceptionType = ExceptionTypes.Added;
                                entRecon.EndpointType = (EndpointType)this.EndPoint.EndpointSystemTypeId;
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

        #endregion

    }
}
