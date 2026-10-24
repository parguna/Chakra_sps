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

namespace DI.Chakra.Connector.LotusNotes
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        private NotesSession _lotusNotesServerSession = null;
        private NotesDatabase _serverDatabase = null;
        private NotesView _LotusNotesView = null;
        public EndPoint EndPoint
        {
            get; set;
        }
        public EndpointConfiguration LNSConfiguration
        {
            get; set;
        }

        private string Configuration(EndpointKey Key)
        {
            var config = LNSConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }

        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {
            object[] documentItems = null;
            string OwnerValue = null;
            try
            {

                if (this.Connect())
                {
                    _LotusNotesView = _serverDatabase.GetView("$Users");

                    NotesViewEntry entry = _LotusNotesView.GetEntryByKey(fields["AccountName"].Value);

                    if (entry == null)
                    {
                        return new ProvisioningResult() { Result = false, Message = "Add Entitlement In LNS could not find Notes Entry" };
                    }
                    NotesDocument document = entry.Document;

                    if (document.HasItem("Owner"))
                    {
                        documentItems = (object[])document.GetItemValue("Owner");

                        if (documentItems != null && documentItems.Count() > 0)
                            OwnerValue = documentItems.First().ToString();

                    }

                    NotesAdministrationProcess nap = _lotusNotesServerSession.CreateAdministrationProcess(Configuration(EndpointKey.IPAddress));

                    foreach (string GroupName in fields["Groups"].Value.Split(','))
                    {
                        nap.AddGroupMembers(GroupName, documentItems);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return new ProvisioningResult() { Result = true, Message = string.Format("Group(s) {0} added to aacount {1} in Lotus Notes.", fields["Groups"].Value, fields["AccountName"].Value) };

        }

        private bool Connect()
        {
            string DominoServerName = string.Empty;
            try
            {
                DominoServerName = Configuration(EndpointKey.IPAddress);
                string NotesPassword = Configuration(EndpointKey.Password);

                _lotusNotesServerSession = new Domino.NotesSession();
                _lotusNotesServerSession.Initialize(NotesPassword);

                _serverDatabase = _lotusNotesServerSession.GetDatabase(DominoServerName, "names.nsf", false);

                if (_serverDatabase == null)
                {
                    return false;
                }
                return true;

            }

            catch (Exception ex)
            {

                throw ex;

            }
        }
        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {
            try
            {
                if (this.Connect())
                {
                    _LotusNotesView = _serverDatabase.GetView("$Users");

                    NotesViewEntry entry = _LotusNotesView.GetEntryByKey(fields["AccountName"].Value);

                    if (entry != null)
                    {
                        return new ProvisioningResult() { Result = false, Message = string.Format("account {0} already exist in Lotus Notes.", fields["AccountName"].Value) };
                    }

                    string userIdFile = string.Format("{0}{1}.id", fields["UserIdFilePath"].Value, fields["UserIdFileName"].Value);

                    NotesRegistration notesRegistration = _lotusNotesServerSession.CreateRegistration();
                    notesRegistration.MailInternetAddress = string.Format("{0}@{1}", fields["EmailAddress"].Value, fields["MailDomain"].Value) ;
                    notesRegistration.MailACLManager = fields["MailACLManager"].Value;
                    notesRegistration.CertifierIDFile = fields["CertifierIDFile"].Value;
                    notesRegistration.MailTemplateName = fields["MailTemplateName"].Value;
                    notesRegistration.ShortName = fields["AccountName"].Value;
                    notesRegistration.StoreIDInMailfile = false;
                    notesRegistration.StoreIDInAddressBook = false;
                    notesRegistration.CreateMailDb = true;
                    notesRegistration.UpdateAddressBook = true;
                    notesRegistration.IDType = Domino.REG_TYPE.ID_HIERARCHICAL;
                    notesRegistration.Expiration = DateTime.Now.AddYears(Convert.ToInt32(fields["AccountExpiryDays"].Value));
                    notesRegistration.RegistrationServer = fields["RegistrationMailServer"].Value;

                    if (!string.IsNullOrEmpty(fields["PolicyName"].Value))
                    {
                        notesRegistration.PolicyName = fields["PolicyName"].Value;
                    }
                    notesRegistration.MailQuotaSizeLimit = Convert.ToInt32(fields["MailQuota"].Value);
                    notesRegistration.MailQuotaWarningThreshold = Convert.ToInt32(fields["MailQuotaThreshhold"].Value);

                    if (fields["ReplicaServer"] != null)
                    {
                        notesRegistration.MailReplicaServers = fields["ReplicaServer"].Value;
                    }
                    notesRegistration.RegisterNewUser(fields["LastName"].Value,
                                                      userIdFile,
                                                      fields["RegistrationMailServer"].Value,
                                                      fields["FirstName"].Value,
                                                      fields["MiddleName"].Value,
                                                      fields["CertifierPassword"].Value,
                                                      fields["Location"].Value,
                                                      fields["Comment"].Value,
                                                      string.Format (@"mail\{0}.nsf",fields["MailFile"].Value),
                                                      string.Empty,
                                                      fields["Password"].Value,
                                                      fields["AltName"].Value,
                                                      fields["AltLang"].Value,
                                                      Domino.USER_TYPE.NOTES_FULL_CLIENT);

                    UpdatePostAccount(fields);

                    return new ProvisioningResult() { Result = true, Message = string.Format("account {0} created in Lotus Notes.", fields["AccountName"].Value) };
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return new ProvisioningResult() { Result = false, Message = "account creation failed in Lotus Notes." };
        }

        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            object[] documentItems = null;
            string OwnerValue = null;

            try
            {


                if (this.Connect())
                {
                    _LotusNotesView = _serverDatabase.GetView("$Users");

                    NotesViewEntry entry = _LotusNotesView.GetEntryByKey(fields["AccountName"].Value);

                    if (entry == null)
                    {
                        return new ProvisioningResult() { Result = false, Message = "Entry not found" };
                    }
                    NotesDocument document = entry.Document;

                    if (document.HasItem("Owner"))
                    {
                        documentItems = (object[])document.GetItemValue("Owner");

                        if (documentItems != null && documentItems.Count() > 0)
                            OwnerValue = documentItems.First().ToString();

                    }

                    if (string.IsNullOrEmpty(OwnerValue))
                    {
                        return new ProvisioningResult() { Result = false, Message = "Entry not found in Lotus Notes." };

                    }
                    NotesAdministrationProcess nap = _lotusNotesServerSession.CreateAdministrationProcess(Configuration(EndpointKey.IPAddress));
                    string msg = nap.DeleteUser(OwnerValue, true, ADMP_MAILDEL.MAILDELALL, fields["DenyGroup"].Value, false);

                    //nap.ApproveMailFileDeletion(msg);

                    deleteIdFile(fields["UserIdFilePath"].Value, fields["UserIdFileName"].Value);
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };

            }
            return new ProvisioningResult() { Result = true, Message = string.Format("account {0} deleted in Lotus Notes.", fields["AccountName"].Value) };
        }

        private void deleteIdFile(string filePath, string fileName)
        {
            try
            {
                string userIdFile = string.Format("{0}{1}.id", filePath, fileName);


                if (System.IO.File.Exists(userIdFile))
                {
                    System.IO.File.Delete(userIdFile);
                }
            }
            catch (Exception)
            {

            }
        }
        public override ProvisioningResult DisableAccount(ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override ProvisioningResult EnableAccount(ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {
            object[] documentItems = null;
            string OwnerValue = null;
            try
            {
                if (this.Connect())
                {
                    _LotusNotesView = _serverDatabase.GetView("$Users");
                    NotesViewEntry entry = _LotusNotesView.GetEntryByKey(fields["AccountName"].Value);
                    if (entry == null)
                    {
                        return new ProvisioningResult() { Result = false, Message = "Entry not found in Lotus Notes." };
                    }
                    NotesDocument document = entry.Document;
                    if (document.HasItem("Owner"))
                    {
                        documentItems = (object[])document.GetItemValue("Owner");
                        if (documentItems != null && documentItems.Count() > 0)
                            OwnerValue = documentItems.First().ToString();
                    }
                    NotesAdministrationProcess nap = _lotusNotesServerSession.CreateAdministrationProcess(Configuration(EndpointKey.IPAddress));


                    nap.AddGroupMembers(fields["DenyGroup"].Value, documentItems);
                    return new ProvisioningResult() { Result = true, Message = string.Format("account {0} locked in Lotus Notes.", fields["AccountName"].Value) };
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = false, Message = "Entry not found in Lotus Notes." };

        }

        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
            NotesViewEntry entry = null;

            NotesDocument document = null;

            NotesItem notesItem = null;

            string strMembers = string.Empty;

            string ownerValue = string.Empty;

            object[] userDocumentItems = null;

            try
            {

                this.Connect();

                _LotusNotesView = _serverDatabase.GetView("$Users");

                NotesViewEntry userEntry = _LotusNotesView.GetEntryByKey(fields["AccountName"].Value);

                if (userEntry == null)
                {
                    return new ProvisioningResult() { Result = false, Message = "Entry not found in Lotus Notes." };
                }
                NotesDocument userDocument = userEntry.Document;

                if (userDocument.HasItem("Owner"))
                {
                    userDocumentItems = (object[])userDocument.GetItemValue("Owner");

                    if (userDocumentItems != null && userDocumentItems.Count() > 0)
                        ownerValue = userDocumentItems.First().ToString();

                }

                if (ownerValue == string.Empty)
                    return new ProvisioningResult() { Result = false, Message = "Entry not found in Lotus Notes." };


                foreach (string strGrp in fields["Groups"].Value.Split(','))
                {

                    _LotusNotesView = _serverDatabase.GetView("Groups");

                    entry = _LotusNotesView.GetEntryByKey(strGrp);

                    if (entry == null)
                        continue;

                    document = entry.Document;

                    object documentItems = document.Items;
                    Array arrayDocItems = (System.Array)documentItems;

                    for (int itemCnt = 0; itemCnt < arrayDocItems.Length; itemCnt++)
                    {
                        notesItem = (Domino.NotesItem)arrayDocItems.GetValue(itemCnt);

                        if (notesItem.Name == "Members")
                        {
                            object[] objValues = notesItem.Values;
                            var v = objValues.ToList().Where(a => a != null).ToList();

                            if (objValues.Contains(ownerValue))
                            {
                                v.Remove(ownerValue);
                                if (v.Count > 0)
                                    notesItem.Values = (object[])v.ToArray<object>();
                                else
                                    notesItem.Values = new object[0];
                                document.Save(true, false, false);

                            }

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return new ProvisioningResult() { Result = true, Message = string.Format("Group(s) {0} removed  from aacount {1} in Lotus Notes.", fields["Groups"].Value, fields["AccountName"].Value) };
        }
        public string GetOwner(string shortName)
        {
            object[] documentItems = null;
            string OwnerValue = null;
            try
            {

                if (this.Connect())
                {
                    _LotusNotesView = _serverDatabase.GetView("$Users");

                    NotesViewEntry entry = _LotusNotesView.GetEntryByKey(shortName);

                    if (entry == null)
                    {
                        return string.Empty;
                    }
                    NotesDocument document = entry.Document;

                    if (document.HasItem("Owner"))
                    {
                        documentItems = (object[])document.GetItemValue("Owner");

                        if (documentItems != null && documentItems.Count() > 0)
                            OwnerValue = documentItems.First().ToString();

                    }
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return OwnerValue;
        }
        public ProvisioningResult RemoveExternalEmailAccess(ProvisioningParamCollection fields)
        {
            string mailExtAccess = "No";
            try
            {

                this.Connect();


                _LotusNotesView = _serverDatabase.GetView("$Users");

                NotesViewEntry entry = _LotusNotesView.GetEntryByKey(fields["AccountName"].Value);

                if (entry == null)
                {
                    return new ProvisioningResult() { Result = false, Message = "Entry not found in Lotus Notes." };
                }
                NotesDocument document = entry.Document;
                NotesItem notesItem = null;
                object documentItems = document.Items;
                Array arrayDocItems = (System.Array)documentItems;

                for (int itemCnt = 0; itemCnt < arrayDocItems.Length; itemCnt++)
                {
                    notesItem = (Domino.NotesItem)arrayDocItems.GetValue(itemCnt);

                    if (notesItem.Name == "MailExtAccess")
                    {
                        notesItem.Values = mailExtAccess;
                        document.Save(true, false, false);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return new ProvisioningResult() { Result = true, Message = string.Format("External email access removed  from aacount {0} in Lotus Notes.", fields["AccountName"].Value) };
        }
        public override ProvisioningResult UpdateAccount(ProvisioningParamCollection fields)
        {
            try
            {
                if (this.Connect())
                {
                    _LotusNotesView = _serverDatabase.GetView("$Users");

                    NotesViewEntry entry = _LotusNotesView.GetEntryByKey(fields["AccountName"].Value);

                    if (entry == null)
                    {
                    }
                    NotesDocument document = entry.Document;
                    NotesItem notesItem = null;
                    object documentItems = document.Items;
                    Array arrayDocItems = (System.Array)documentItems;

                    for (int itemCnt = 0; itemCnt < arrayDocItems.Length; itemCnt++)
                    {
                        notesItem = (Domino.NotesItem)arrayDocItems.GetValue(itemCnt);

                        if (notesItem.Name == "JobTitle")
                            notesItem.Values = fields["JobTitle"].Value;
                        if (notesItem.Name == "CompanyName")
                            notesItem.Values = fields["CompanyName"].Value;
                        if (notesItem.Name == "DepartmentName")
                            notesItem.Values = fields["DepartmentName"].Value;
                        if (notesItem.Name == "OfficePhoneNumber")
                            notesItem.Values = fields["OfficePhoneNumber"].Value;
                        if (notesItem.Name == "NRIC")
                            notesItem.Values = fields["NRIC"].Value;

                    }
                    document.Save(true, false, false);
                    return new ProvisioningResult() { Result = true, Message = string.Format("account {0} updated in Lotus Notes.", fields["AccountName"].Value) };
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Message = "Entry not found in Lotus Notes." };
        }
        public ProvisioningResult MoveOU(ProvisioningParamCollection fields)
        {
            try
            {
                if (this.Connect())
                {
                    NotesAdministrationProcess nap = _lotusNotesServerSession.CreateAdministrationProcess(Configuration(EndpointKey.IPAddress));

                    string owner = this.GetOwner(fields["AccountName"].Value);

                    if (string.IsNullOrEmpty(owner))
                        return new ProvisioningResult() { Result = true, Message = "Owner Could not be found" };


                    nap.CertifierFile = string.Format(@"{0}", fields["CertifierIDFile"].Value);
                    nap.CertifierPassword = fields["CertifierPassword"].Value;
                    nap.UseCertificateAuthority = false;
                    string noteidReq = nap.MoveUserInHierarchyRequest(owner, fields["TargetCertifier"].Value, false);
                    if (noteidReq == null)
                    {
                        MoveCertifierWhenRequestIdIsNULL(fields["AdminUserIdFile"].Value, fields["TargetCertifierPassword"].Value);
                    }
                    else
                    {
                        nap.CertifierFile = string.Format(@"{0}", fields["TargetCertifierIdFile"].Value);
                        nap.CertifierPassword = fields["TargetCertifierPassword"].Value;


                        string noteidComplete = nap.MoveUserInHierarchyComplete(noteidReq);
                        if (null == noteidComplete)
                        {
                        }

                       // ProcessAdminP(fields["AdminUserIdFile"].Value);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return new ProvisioningResult() { Result = true, Message = string.Format("account {0} moved to {1} in Lotus Notes.", fields["AccountName"].Value, fields["TargetCertifier"].Value) };
        }
        public bool IsProcessed(string _requestUNID)
        {

            _serverDatabase = _lotusNotesServerSession.GetDatabase(Configuration(EndpointKey.IPAddress), "admin4.nsf", false);

            NotesDateTime dbTime = _lotusNotesServerSession.CreateDateTime("Yesterday");


            _LotusNotesView = _serverDatabase.GetView("Name Move Requests");

            string proxyAuthor = string.Format(" & ProxyAuthor = \"{0}\"", _lotusNotesServerSession.UserName);

            string SearchFormula = string.Concat("Form = \"AdminLog\""
                                    , " & ProxyAction = \"6\""
                                    , proxyAuthor
                                    );



            NotesDocumentCollection collection = _serverDatabase.Search(SearchFormula, dbTime, 0);

            if (collection.Count == 0)
                return false;

            var document = collection.GetFirstDocument();
            while (document != null)
            {

                if (document.ParentDocumentUNID == _requestUNID)
                {
                    return true;
                }


                document = collection.GetNextDocument(document);

            }

            return false;
        }

        private void MoveCertifierWhenRequestIdIsNULL(string adminUserIdPath,string certifierPassword)
        {

            string _targetCertifier = string.Empty;
            object documentItems = null;
            string FieldName = string.Empty;

            try
            {
                _serverDatabase = _lotusNotesServerSession.GetDatabase(Configuration(EndpointKey.IPAddress), "admin4.nsf", false);

                NotesDateTime dbTime = _lotusNotesServerSession.CreateDateTime("Yesterday");


                _LotusNotesView = _serverDatabase.GetView("Name Move Requests");

                string proxyAuthor = string.Format(" & ProxyAuthor = \"{0}\"", _lotusNotesServerSession.UserName);

                string SearchFormula = string.Concat("Form = \"AdminRequest\""
                                        , " & ProxyAction = \"6\""
                                        , proxyAuthor
                                        );

                NotesDocumentCollection collection = _serverDatabase.Search(SearchFormula, dbTime, 0);

                var document = collection.GetFirstDocument();
                while (document != null)
                {

                    if (IsProcessed(document.UniversalID))
                    {
                        document = collection.GetNextDocument(document);
                        continue;
                    }

                    documentItems = document.Items;
                    Array itemArrayDocs = (System.Array)documentItems;


                    for (int itemCount = 0; itemCount < itemArrayDocs.Length; itemCount++)
                    {
                        NotesItem notesItem =
                        (Domino.NotesItem)itemArrayDocs.GetValue(itemCount);

                        if (notesItem.Text != null)
                        {
                            NotesItem searchedNotesItem = (Domino.NotesItem)itemArrayDocs.GetValue(itemCount);
                            FieldName = searchedNotesItem.Name.ToString();


                            if (FieldName == "ProxyTargetCertifier")
                                _targetCertifier = searchedNotesItem.Text;

                        }
                    }

                    if (string.IsNullOrEmpty(_targetCertifier))
                        continue;

                    MoveOUComplete(document.NoteID, _targetCertifier, certifierPassword,adminUserIdPath);

                    document = collection.GetNextDocument(document);

                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public void MoveOUComplete(string _requestNotesId, string targetCertifier, string certifierPassword,string adminUserIdFile)
        {
            try
            {

               
                NotesAdministrationProcess napComplete = _lotusNotesServerSession.CreateAdministrationProcess(Configuration(EndpointKey.IPAddress));
                napComplete.CertifierFile = string.Format(@"{0}", targetCertifier);
                napComplete.CertifierPassword = certifierPassword;
                string noteidComplete = napComplete.MoveUserInHierarchyComplete(_requestNotesId);

                ProcessAdminP(adminUserIdFile);

            }
            catch (Exception ex)
            {
                return;
            }

        }
        public override ProvisioningResult ResetPassword(ProvisioningParamCollection fields)
        {
            bool isPasswordChanged = false;
            try
            {

                if (this.Connect())
                {
                    string tempIdFilePath = MasterConfigHelper.LNSTempIdPath;

                    tempIdFilePath = string.Format(@"{0}\{1}.id", tempIdFilePath, fields["AccountName"].Value);

                    string owner = this.GetOwner(fields["AccountName"].Value);
                    LNPasswordHelper lnPwdReset = new LNPasswordHelper();
                    lnPwdReset.DBName = string.Empty;
                    lnPwdReset.ServerName = fields["Server"].Value;
                    lnPwdReset.UserName = owner;
                    lnPwdReset.TempIdFilePath = tempIdFilePath;
                    lnPwdReset.AdminIdPath = fields["AdminIdPath"].Value;
                    lnPwdReset.AdminUserName = fields["AdminUserName"].Value;
                    lnPwdReset.AdminPassword = fields["AdminPassword"].Value;
                    lnPwdReset.NewPasswordToBeChanged = fields["Password"].Value;

                    string tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8);

                    isPasswordChanged = lnPwdReset.ChangePassword_Client(tempPassword);



                    if (isPasswordChanged == false)

                        try
                        {
                            this.Connect();

                            NotesAdministrationProcess nap = _lotusNotesServerSession.CreateAdministrationProcess(fields["MailServer"].Value);


                            string httpRetMsg = nap.ChangeHTTPPassword(owner, tempPassword, fields["Password"].Value);



                        }
                        catch (Exception ex)
                        {
                            return new ProvisioningResult() { Result = false, Message = ex.Message };
                        }
                }
            }
            catch (Exception exx)
            {
                return new ProvisioningResult() { Result = false, Message = exx.Message };

            }


            return new ProvisioningResult() { Result = true, Message = "account password changed in Lotus Notes." };
        }

        public override ProvisioningResult UnLockAccount(ProvisioningParamCollection fields)
        {
            NotesViewEntry entry = null;
            NotesDocument document = null;
            NotesItem notesItem = null;
            string strMembers = string.Empty;
            string denyGroupName = string.Empty;
            string ownerValue = string.Empty;
            object[] userDocumentItems = null;
            try
            {
                this.Connect();
                _LotusNotesView = _serverDatabase.GetView("$Users");
                NotesViewEntry userEntry = _LotusNotesView.GetEntryByKey(fields["AccountName"].Value);
                if (userEntry == null)
                {
                    return new ProvisioningResult() { Result = true, Message = "Entry Not Found in Lotus Notes." };
                }
                NotesDocument userDocument = userEntry.Document;
                if (userDocument.HasItem("Owner"))
                {
                    userDocumentItems = (object[])userDocument.GetItemValue("Owner");
                    if (userDocumentItems != null && userDocumentItems.Count() > 0)
                        ownerValue = userDocumentItems.First().ToString();
                }
                if (ownerValue == string.Empty)
                    return new ProvisioningResult() { Result = true, Message = "Entry Not Found in Lotus Notes." };

                //_serverDatabase.GetView("Groups");
                _LotusNotesView = _serverDatabase.GetView("Groups");
                entry = _LotusNotesView.GetEntryByKey(fields["DenyGroup"].Value);
                if (entry == null)
                    return new ProvisioningResult() { Result = true, Message = "Entry Not Found in Lotus Notes." };

                document = entry.Document;
                object documentItems = document.Items;
                Array arrayDocItems = (System.Array)documentItems;
                for (int itemCnt = 0; itemCnt < arrayDocItems.Length; itemCnt++)
                {
                    notesItem = (Domino.NotesItem)arrayDocItems.GetValue(itemCnt);
                    if (notesItem.Name == "Members")
                    {
                        object[] objValues = notesItem.Values;
                        var v = objValues.ToList().Where(a => a != null).ToList();
                        if (objValues.Contains(ownerValue))
                        {
                            v.Remove(ownerValue);
                            if (v.Count > 0)
                                notesItem.Values = (object[])v.ToArray<object>();
                            else
                                notesItem.Values = new object[0];
                            document.Save(true, false, false);
                        }
                    }
                    //if (notesItem.Name == "Members") As this code save string as it is the logic is changed
                    //{
                    //    strMembers = notesItem.Text;

                    //    if (strMembers.Contains(ownerValue))  // (strMembers.Contains(fields["Owner"].Value)) There is  no provisioning field called owner for  LNS, So unlocking not happening
                    //    {
                    //        strMembers = strMembers.Replace(ownerValue, string.Empty);
                    //        notesItem.Values = strMembers;
                    //        document.Save(true, false, false);
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Message = string.Format("account {0} unlocked in Lotus Notes.", fields["AccountName"].Value) };
        }
        private void ProcessAdminP(string adminUserIdFile)
        {
            ProcessAdminPReq.ProcessAdminRequest(Configuration(EndpointKey.IPAddress), adminUserIdFile, _lotusNotesServerSession.UserName, Configuration(EndpointKey.Password));
        }
        private ProvisioningResult UpdatePostAccount(ProvisioningParamCollection fields)
        {
            
            try
            {

                if (this.Connect())
                {

                    _LotusNotesView = _serverDatabase.GetView("$Users");

                    NotesViewEntry entry = _LotusNotesView.GetEntryByKey(fields["AccountName"].Value);

                    if (entry == null)
                    {
                    }
                    NotesDocument document = entry.Document;
                    NotesItem notesItem = null;
                    object documentItems = document.Items;
                    Array arrayDocItems = (System.Array)documentItems;

                    for (int itemCnt = 0; itemCnt < arrayDocItems.Length; itemCnt++)
                    {
                        notesItem = (Domino.NotesItem)arrayDocItems.GetValue(itemCnt);

                        if (notesItem.Name == "MailDomain")
                            notesItem.Values = fields["MailDomain"].Value;
                        
                        if (notesItem.Name == "AvailableForDirSync")
                            notesItem.Values = fields["AvailableForDirSync"].Value;
                        if (notesItem.Name == "MailExtAccess")
                            notesItem.Values = fields["MailExtAccess"].Value;
                        if (notesItem.Name == "MailChargeable")
                            notesItem.Values = fields["MailChargeable"].Value;
                        if (notesItem.Name == "MailEnforceACL")
                            notesItem.Values = "Yes";
                        if (notesItem.Name == "IDMPolicy")
                            notesItem.Values = fields["IDMPolicy"].Value;
                        if (notesItem.Name == "NRIC")
                            notesItem.Values = fields["NRIC"].Value;
                        if (notesItem.Name == "CheckPassword")
                            notesItem.Values = fields["CheckPassword"].Value;
                        if (notesItem.Name == "PasswordChangeInterval")
                            notesItem.Values = fields["PasswordChangeInterval"].Value;
                        if (notesItem.Name == "PasswordGracePeriod")
                            notesItem.Values = fields["PasswordGracePeriod"].Value;
                        if (notesItem.Name == "LocalAdmin")
                            notesItem.Values = fields["LocalAdmin"].Value;

                        //Notes license change
                        if (notesItem.Name == "ClientType")
                        {
                            notesItem.Values = fields["ClientType"].Value;
                        }

                    }
                    document.Save(true, false, false);
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
                return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
    }
}