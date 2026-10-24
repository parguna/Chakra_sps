using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using DI.Chakra.Connector.Common.Enum;

namespace DI.Chakra.Connector.Exchange
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        private DirectoryEntry m_root = null;
        private DirectorySearcher m_ExSearch = null;
        private bool Isoffice365 = true;
        private bool IsConnected = false;
        private bool IsRunSpaceOpened = false;
        private const int ThreadSleeperTime = 10000;
        private bool _isCrossDomain = true;
        private string _domainController = string.Empty;
        private const string _defaultDomain = "rws";
        public EndPoint EndPoint
        {
            get; set;
        }
        public EndpointConfiguration ExchangeConfiguration
        {
            get; set;
        }
        public string DomainController
        {
            get
            {
                var domainControl = ConfigurationMaster.GetByConfigKey("DOMAINCONTROLSERVER", Product.IM);
                return domainControl != null && !string.IsNullOrEmpty(domainControl.ConfigValue) ? domainControl.ConfigValue.Trim() : "dvappdom6v.rws-dev.rwsentosa.com";
            }
            set { }
        }

        public string DefaultDomain
        {
            get
            {
                var defaultDoamin = ConfigurationMaster.GetByConfigKey("DefaultDomain", Product.IACM);
                return defaultDoamin != null && !string.IsNullOrEmpty(defaultDoamin.ConfigValue) ? defaultDoamin.ConfigValue.Trim() : "rws";
            }
            set { }
        }

        public bool IsCrossDomain
        {
            get
            {
                var domainControl = ConfigurationMaster.GetByConfigKey("ISCROSSDOMAIN", Product.IM);
                return domainControl != null && !string.IsNullOrEmpty(domainControl.ConfigValue) && domainControl.ConfigValue == "1" ? true : false;
            }
            set
            {

            }
        }

        public bool Connect()
        {
            //bool bReturn = false;
            //Isoffice365 = false;
            //if (!Isoffice365)
            //{
            //    bReturn = ConnectToExchangeServer();
            //}
            return true;
        }


        private string Configuration(EndpointKey Key)
        {
            var config = ExchangeConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }

        private WSManConnectionInfo CreateConnectionForRunspace(int timeout)
        {
            WSManConnectionInfo connInfo = null;
            string hostName = string.Empty;

            try
            {

                string[] hostNames = Convert.ToString(Configuration(EndpointKey.HostName)).Split(';');

                this.ExchangeConfiguration = EndpointConfiguration.GetByEndpointSystemId(EndPoint.Id);
                string userName = Configuration(EndpointKey.UserName);
                ExchangeConfiguration.ConfigurationDetails.ForEach(lq =>
                {
                    if (lq.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString())
                    {
                        lq.Value = RSAHelper.DecryptData(lq.Value);
                    }
                });
                string password = Configuration(EndpointKey.Password);
                string ipaddress = Configuration(EndpointKey.IPAddress);

                SecureString ssRunasPassword = new SecureString();
                foreach (char x in password)
                    ssRunasPassword.AppendChar(x);
                PSCredential credentials = new PSCredential(userName, ssRunasPassword);

                foreach (string thostName in hostNames)
                {
                    hostName = thostName;
                    string url = string.Format("http://{0}{1}", hostName, "/PowerShell");

                    connInfo = new WSManConnectionInfo(new Uri(url), "http://schemas.microsoft.com/powershell/Microsoft.Exchange", credentials);
                    connInfo.AuthenticationMechanism = AuthenticationMechanism.Kerberos;
                    connInfo.OpenTimeout = timeout * 60 * 1000;
                    connInfo.OperationTimeout = timeout * 60 * 1000;
                    connInfo.IdleTimeout = timeout * 60 * 1000;
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("Trying to connect exchange box {0}", hostName));
                    if (TestConnection(connInfo))
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("Exchange box connected succesfully {0}", hostName));
                        return connInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, string.Format("Failed to Create Connection ", hostName));
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format(ex.Message));
            }

            return connInfo;
        }

        private bool TestConnection(WSManConnectionInfo connection)
        {
            using (Runspace rsp1 = RunspaceFactory.CreateRunspace((connection)))
            {

                try
                {
                    rsp1.StateChanged += rsp_StateChanged;
                    rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                    rsp1.Open();

                    while (!IsRunSpaceOpened)
                    {
                        System.Threading.Thread.Sleep(10000);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }


        ProvisioningResult CreateExchangemailBox(ProvisioningParamCollection fields)
        {
            bool isMailboxCreated = false;
            string mailBoxAccount = string.Empty;
            //   ExUserInfo _exUserInfo = new ExUserInfo();
            User usr = new User();

            try
            {
                ICollection<PSObject> gpcOutput = null;
                ICollection<ErrorRecord> errors = null;
                try
                {
                    mailBoxAccount += "Started";

                    var userName = fields["AccountName"].Value;
                    if (string.IsNullOrEmpty(userName))
                    {
                        return new ProvisioningResult() { Result = isMailboxCreated, Message = "Username coming as blank while creating new email account." };
                    }

                    gpcOutput = null;
                    using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(3)))
                    {

                        rsp1.StateChanged += rsp_StateChanged;
                        rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                        rsp1.Open();

                        while (!IsRunSpaceOpened)
                        {
                            System.Threading.Thread.Sleep(ThreadSleeperTime);
                        }

                        using (PowerShell gpc = PowerShell.Create())
                        {
                            gpc.InvocationStateChanged += powershell_InvocationStateChanged;
                            gpc.Runspace = rsp1;
                            mailBoxAccount += "Powershell created";

                            var domainName = fields["DomainName"].Value;
                            var password = fields["Password"].Value;
                            var ssPassword = new SecureString();
                            foreach (char c in password)
                                ssPassword.AppendChar(c);

                            gpc.AddCommand("New-Mailbox");
                            //   gpc.AddParameter("Shared");

                            if (IsCrossDomain)
                            {
                                gpc.AddParameter("DomainController", DomainController);
                            }

                            gpc.AddParameter("Name", fields["AccountName"].Value);
                            gpc.AddParameter("Alias", fields["AccountName"].Value);
                            gpc.AddParameter("UserPrincipalName", fields["AccountName"].Value + "@" + domainName);
                            gpc.AddParameter("FirstName", fields["givenname"].Value);
                            gpc.AddParameter("LastName", fields["sn"].Value);
                            gpc.AddParameter("DisplayName", fields["DisplayName"].Value);
                            gpc.AddParameter("Password", ssPassword);
                            gpc.AddParameter("OrganizationalUnit", fields["OU"].Value);
                            if (!string.IsNullOrEmpty(fields["Database"].Value))
                                gpc.AddParameter("Database", fields["Database"].Value);

                            gpcOutput = gpc.Invoke();

                            errors = gpc.Streams.Error;

                            if (errors.Count > 0)
                            {
                                foreach (var err in errors)
                                {
                                    mailBoxAccount += Convert.ToString(err);
                                }

                            }
                        }
                    }
                    if (gpcOutput.Count > 0)
                    {
                        isMailboxCreated = true;
                        mailBoxAccount += userName + "@" + fields["DomainName"].Value;
                    }
                }
                catch (Exception ex)
                {
                    mailBoxAccount += Convert.ToString(ex.Message) + Environment.NewLine;
                    if (ex.InnerException != null)
                        mailBoxAccount += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                    return new ProvisioningResult() { Result = false, Message = string.Format("{0}-{1}, {2}", "Error while creating user in Exchange Mail Server..", ex.Message, mailBoxAccount) };
                }
            }
            catch (Exception ex)
            {
                mailBoxAccount += Convert.ToString(ex.Message) + Environment.NewLine;
                if (ex.InnerException != null)
                    mailBoxAccount += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;

                return new ProvisioningResult() { Result = false, Message = string.Format("{0}-{1},{2}", "Error while creating user in Exchange Mail Server.", ex.Message, mailBoxAccount) };
            }

            return new ProvisioningResult() { Result = isMailboxCreated, Message = string.Format("{0} email account creataed, successfully.", mailBoxAccount) };
        }

        ProvisioningResult EnableMailBox(ProvisioningParamCollection fields)
        {
            ProvisioningResult result = new ProvisioningResult() { Result = false, Message = string.Empty };

            string message = string.Empty;
            bool isSuccess = true;
            try
            {
                ICollection<PSObject> gpcOutput = null;
                using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(3)))
                {

                    rsp1.StateChanged += rsp_StateChanged;
                    rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                    rsp1.Open();

                    while (!IsRunSpaceOpened)
                    {
                        System.Threading.Thread.Sleep(ThreadSleeperTime);
                    }

                    string smsAccountName = Convert.ToString(fields["AccountName"].Value).Trim();
                    var command = new Command("Enable-Mailbox");
                    if (IsCrossDomain)
                    {
                        command.Parameters.Add("DomainController", DomainController);
                        command.Parameters.Add("Identity", string.Format("{0}\\{1}", DefaultDomain, smsAccountName));
                    }
                    else
                    {
                        command.Parameters.Add("Identity", string.Format("{0}\\{1}", DefaultDomain, smsAccountName));
                    }

                    command.Parameters.Add("DisplayName", fields["DisplayName"].Value);

                    if (!string.IsNullOrEmpty(fields["Database"].Value))
                        command.Parameters.Add("Database", fields["Database"].Value);

                    using (var pipeline = rsp1.CreatePipeline())
                    {
                        pipeline.Commands.Add(command);
                        var results = pipeline.Invoke();

                        message += "EnableMailBox executed ";
                        if (results.Count <= 0)
                        {
                            if (pipeline.Error != null && pipeline.Error.Count > 0)
                            {
                                message += pipeline.Error.Read().ToString() + Environment.NewLine;
                                isSuccess = false;
                            }
                        }
                    }

                }
                result = new ProvisioningResult() { Result = isSuccess, Message = string.Format("{0} for account {1}.", message, fields["AccountName"].Value) };
            }
            catch (Exception ex)
            {
                result = new ProvisioningResult() { Result = false, Message = string.Format("{0}-{1}", "Error while enabling user account in Exchange Mail Server.", ex.Message) };
            }

            return result;
        }

        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {
            try
            {
                ProvisioningResult isMailboxCreated;
                if (fields["UserType"].Value == AccountType.Shared.ToString())
                {
                    isMailboxCreated = CreateExchangemailBox(fields);

                }
                else
                {
                    isMailboxCreated = EnableMailBox(fields);
                }

                if (fields["HideMailAccount"].Value != null && fields["HideMailAccount"].Value == "1")
                {
                    string respnse = HideMailBox(fields);
                    isMailboxCreated.Message += respnse;
                }
                return isMailboxCreated;
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
        }
        public override ProvisioningResult AddMailBoxPermissions(ProvisioningParamCollection fields)
        {
            string message = string.Empty;
            string emailId = string.Empty;
            try
            {
                ICollection<PSObject> gpcOutput = null;
                using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(3)))
                {
                    try
                    {
                        rsp1.StateChanged += rsp_StateChanged;
                        rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                        rsp1.Open();

                        while (!IsRunSpaceOpened)
                        {
                            System.Threading.Thread.Sleep(ThreadSleeperTime);
                        }

                        using (PowerShell gpc = PowerShell.Create())
                        {
                            try
                            {
                                gpc.InvocationStateChanged += powershell_InvocationStateChanged;
                                gpc.Runspace = rsp1;

                                if (fields["UserType"].Value == AccountType.Shared.ToString())
                                {
                                    emailId = fields["AccountName"].Value + "@" + fields["DomainName"].Value;
                                }
                                else
                                {
                                    emailId = fields["EMAILID"].Value;
                                }

                                gpc.AddCommand("Add-MailboxPermission");
                                if (IsCrossDomain)
                                {
                                    gpc.AddParameter("DomainController", DomainController);
                                    gpc.AddParameter("Identity", emailId);
                                }
                                else
                                {
                                    gpc.AddParameter("Identity", emailId);
                                }

                                gpc.AddParameter("User", fields["UserOwner"].Value);
                                gpc.AddParameter("AccessRights", "FullAccess");
                                gpc.AddParameter("InheritanceType", "All");

                                gpcOutput = gpc.Invoke();

                                message += "Added FullAccess Permission successfully";
                            }
                            catch (Exception ex)
                            {
                                message += Convert.ToString(ex.Message) + Environment.NewLine;
                                if (ex.InnerException != null)
                                    message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        message += Convert.ToString(ex.Message) + Environment.NewLine;
                        if (ex.InnerException != null)
                            message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return new ProvisioningResult() { Result = true, Message = string.Format("Full Access permission provided to the  user {0} to the account {1}", fields["UserOwner"].Value, emailId) };
        }

        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            string message = string.Empty;
            string emailId = string.Empty;
            try
            {
                ICollection<PSObject> gpcOutput = null;
                using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(3)))
                {
                    try
                    {
                        rsp1.StateChanged += rsp_StateChanged;
                        rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                        rsp1.Open();

                        while (!IsRunSpaceOpened)
                        {
                            System.Threading.Thread.Sleep(ThreadSleeperTime);
                        }

                        using (PowerShell gpc = PowerShell.Create())
                        {
                            try
                            {
                                gpc.InvocationStateChanged += powershell_InvocationStateChanged;
                                gpc.Runspace = rsp1;

                                if (fields["UserType"].Value == AccountType.Shared.ToString())
                                {
                                    emailId = fields["AccountName"].Value + "@" + fields["DomainName"].Value;
                                }
                                else
                                {
                                    emailId = fields["EMAILID"].Value;
                                }

                                gpc.AddCommand("Disable-Mailbox");
                                if (IsCrossDomain)
                                {
                                    gpc.AddParameter("DomainController", DomainController);
                                    // gpc.AddParameter("Identity", string.Format("{0}\\{1}", DefaultDomain, fields["EMAILID"].Value));
                                    gpc.AddParameter("Identity", emailId);
                                }
                                else
                                {
                                    gpc.AddParameter("Identity", emailId);
                                }
                                gpc.AddParameter("Confirm", false);
                                //gpc.AddParameter("Permanent", true);

                                gpcOutput = gpc.Invoke();

                                message += "DeleteAccount successfully done";
                            }
                            catch (Exception ex)
                            {
                                message += Convert.ToString(ex.Message) + Environment.NewLine;
                                if (ex.InnerException != null)
                                    message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        message += Convert.ToString(ex.Message) + Environment.NewLine;
                        if (ex.InnerException != null)
                            message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return new ProvisioningResult() { Result = true, Message = string.Format("Account {0} deleted in Exchange Mail Server, Message = {1}", fields["AccountName"].Value, message) };
        }

        public override ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {
            string message = string.Empty;
            string emailId = string.Empty;
            try
            {
                ICollection<PSObject> gpcOutput = null;
                using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(3)))
                {
                    try
                    {
                        rsp1.StateChanged += rsp_StateChanged;
                        rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                        rsp1.Open();

                        while (!IsRunSpaceOpened)
                        {
                            System.Threading.Thread.Sleep(ThreadSleeperTime);
                        }

                        using (PowerShell gpc = PowerShell.Create())
                        {
                            gpc.InvocationStateChanged += powershell_InvocationStateChanged;
                            gpc.Runspace = rsp1;

                            try
                            {
                                gpc.AddCommand("Disable-Mailbox");
                                if (fields["UserType"].Value == AccountType.Shared.ToString())
                                {
                                    emailId = fields["AccountName"].Value + "@" + fields["DomainName"].Value;
                                }
                                else
                                {
                                    emailId = fields["EMAILID"].Value;
                                }

                                // gpc.AddParameter("Identity", fields["AccountName"].Value + "@" + fields["DomainName"].Value);
                                if (IsCrossDomain)
                                {
                                    gpc.AddParameter("DomainController", DomainController);
                                    gpc.AddParameter("Identity", emailId);
                                }
                                else
                                {
                                    gpc.AddParameter("Identity", emailId);
                                }
                                gpc.AddParameter("Confirm", false);

                                gpcOutput = gpc.Invoke();
                                message += "LockAccount successfully done";
                            }
                            catch (Exception ex)
                            {
                                message += Convert.ToString(ex.Message) + Environment.NewLine;
                                if (ex.InnerException != null)
                                    message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        message += Convert.ToString(ex.Message) + Environment.NewLine;
                        if (ex.InnerException != null)
                            message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = string.Format("{0}-{1}", "Error while lock user account in Exchange Mail Server.", ex) };
            }
            return new ProvisioningResult() { Result = true, Message = string.Format("{0} for account {1}.", fields["AccountName"].Value) };
        }

        public override ProvisioningResult DisableAccount(ProvisioningParamCollection fields)
        {
            string message = string.Empty;
            string emailId = string.Empty;
            try
            {
                ICollection<PSObject> gpcOutput = null;
                using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(3)))
                {
                    try
                    {
                        rsp1.StateChanged += rsp_StateChanged;
                        rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                        rsp1.Open();

                        while (!IsRunSpaceOpened)
                        {
                            System.Threading.Thread.Sleep(ThreadSleeperTime);
                        }

                        using (PowerShell gpc = PowerShell.Create())
                        {
                            gpc.InvocationStateChanged += powershell_InvocationStateChanged;
                            gpc.Runspace = rsp1;

                            try
                            {
                                if (fields["UserType"].Value == AccountType.Shared.ToString())
                                {
                                    emailId = fields["AccountName"].Value + "@" + fields["DomainName"].Value;
                                }
                                else
                                {
                                    emailId = fields["EMAILID"].Value;
                                }

                                gpc.AddCommand("Disable-Mailbox");
                                //command.Parameters.Add("Identity", fields["AccountName"].Value + "@" + fields["DomainName"].Value);
                                if (IsCrossDomain)
                                {
                                    gpc.AddParameter("DomainController", DomainController);
                                    gpc.AddParameter("Identity", emailId);
                                }
                                else
                                {
                                    gpc.AddParameter("Identity", emailId);
                                }
                                gpc.AddParameter("Confirm", false);

                                gpcOutput = gpc.Invoke();

                                message += "DisableAccount successfully done";
                            }
                            catch (Exception ex)
                            {
                                message += Convert.ToString(ex.Message) + Environment.NewLine;
                                if (ex.InnerException != null)
                                    message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        message += Convert.ToString(ex.Message) + Environment.NewLine;
                        if (ex.InnerException != null)
                            message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Message = string.Format("{0} for account {1}.", message, fields["AccountName"].Value) };
        }

        public override ProvisioningResult UnLockAccount(ProvisioningParamCollection fields)
        {
            string message = string.Empty;
            string emailId = string.Empty;
            try
            {
                ICollection<PSObject> gpcOutput = null;
                using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(3)))
                {
                    try
                    {
                        rsp1.StateChanged += rsp_StateChanged;
                        rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                        rsp1.Open();

                        while (!IsRunSpaceOpened)
                        {
                            System.Threading.Thread.Sleep(ThreadSleeperTime);
                        }

                        using (PowerShell gpc = PowerShell.Create())
                        {
                            gpc.InvocationStateChanged += powershell_InvocationStateChanged;
                            gpc.Runspace = rsp1;

                            try
                            {
                                if (fields["UserType"].Value == AccountType.Shared.ToString())
                                {
                                    emailId = fields["AccountName"].Value + "@" + fields["DomainName"].Value;
                                }
                                else
                                {
                                    emailId = fields["EMAILID"].Value;
                                }

                                gpc.AddCommand("Enable-Mailbox");
                                //command.Parameters.Add("Identity", fields["AccountName"].Value + "@" + fields["DomainName"].Value);
                                if (IsCrossDomain)
                                {
                                    gpc.AddParameter("DomainController", DomainController);
                                    gpc.AddParameter("Identity", emailId);
                                }
                                else
                                {
                                    gpc.AddParameter("Identity", emailId);
                                }

                                gpcOutput = gpc.Invoke();

                                message += "UnLockAccount successfully done";
                            }
                            catch (Exception ex)
                            {
                                message += Convert.ToString(ex.Message) + Environment.NewLine;
                                if (ex.InnerException != null)
                                    message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        message += Convert.ToString(ex.Message) + Environment.NewLine;
                        if (ex.InnerException != null)
                            message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = string.Format("{0}-{1}", "Error while unlock user in Exchange Mail Server.", ex) };
            }
            return new ProvisioningResult() { Result = true, Message = string.Format("{0} for account {1}.", message, fields["AccountName"].Value) };
        }
        public override ProvisioningResult EnableAccount(ProvisioningParamCollection fields)
        {
            try
            {

                ProvisioningResult result = EnableMailBox(fields);
                if (fields["HideMailAccount"].Value != null && fields["HideMailAccount"].Value == "1")
                {
                    string respnse = HideMailBox(fields);
                    result.Message += respnse;
                }
                return result;
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
        }

        public override ProvisioningResult ResetPassword(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = false, Message = "Password change can be done through Active Directory." };
        }

        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {
            bool isAdded = false;
            string message = string.Empty;
            string emailId = string.Empty;
            try
            {
                ICollection<PSObject> gpcOutput = null;
                using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(3)))
                {
                    try
                    {
                        rsp1.StateChanged += rsp_StateChanged;
                        rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                        rsp1.Open();
                        while (!IsRunSpaceOpened)
                        {
                            System.Threading.Thread.Sleep(ThreadSleeperTime);
                        }

                        if (fields["UserType"].Value == AccountType.Shared.ToString())
                        {
                            emailId = fields["AccountName"].Value + "@" + fields["DomainName"].Value;
                        }
                        else
                        {
                            emailId = fields["EMAILID"].Value;
                        }

                        if (!string.IsNullOrEmpty(fields["Groups"].Value))
                        {

                            foreach (var grpName in fields["Groups"].Value.Split(',').ToArray())
                            {
                                gpcOutput = null;
                                using (PowerShell gpc = PowerShell.Create())
                                {
                                    gpc.InvocationStateChanged += powershell_InvocationStateChanged;
                                    gpc.Runspace = rsp1;

                                    try
                                    {
                                        gpc.AddCommand("Add-DistributionGroupMember");
                                        gpc.AddParameter("Identity", grpName);

                                        if (IsCrossDomain)
                                        {
                                            gpc.AddParameter("DomainController", DomainController);
                                            gpc.AddParameter("Member", emailId);
                                        }
                                        else
                                        {
                                            gpc.AddParameter("Member", emailId);
                                        }

                                        gpcOutput = gpc.Invoke();

                                        if (!string.IsNullOrEmpty(message))
                                        {
                                            message += ",";
                                        }
                                        else
                                        {
                                            message += "Groups: ";
                                        }

                                        message += grpName;

                                    }
                                    catch (Exception ex)
                                    {
                                        message += Convert.ToString(ex.Message) + Environment.NewLine;
                                        if (ex.InnerException != null)
                                            message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                                    }
                                }
                                isAdded = true;
                            }

                            message += string.Format("  Add Group Entitlement successfully done to the Email Id: {0}", Convert.ToString(emailId));
                        }


                        message += string.Format("Email Quota:{0}", Convert.ToString(fields["MailQuota"].Value));
                        if (!string.IsNullOrEmpty(fields["MailQuota"].Value))
                        {
                            foreach (var grpName in fields["MailQuota"].Value.Split(new string[] { ")," }, StringSplitOptions.None).ToArray())
                            {
                                gpcOutput = null;
                                using (PowerShell gpc = PowerShell.Create())
                                {
                                    gpc.InvocationStateChanged += powershell_InvocationStateChanged;
                                    gpc.Runspace = rsp1;

                                    try
                                    {

                                        gpc.AddCommand("Set-Mailbox");
                                        if (IsCrossDomain)
                                        {
                                            gpc.AddParameter("DomainController", DomainController);
                                            gpc.AddParameter("Identity", string.Format("{0}\\{1}", DefaultDomain, fields["AccountName"].Value));
                                        }
                                        else
                                        {
                                            gpc.AddParameter("Identity", string.Format("{0}\\{1}", DefaultDomain, fields["AccountName"].Value));
                                        }



                                        message += string.Format("Mailquota change for {0} to {1}", fields["AccountName"].Value, grpName);

                                        var mailQuota = ExMailBoxQuotaInfo.GetByName(grpName);

                                        bool isDefaultMailQuota = false;
                                        var defaultMailQuota = ConfigurationMaster.GetByConfigKey("DefaultExchangeMailQuota", Product.IM);

                                        if (defaultMailQuota != null && !string.IsNullOrEmpty(defaultMailQuota.ConfigValue))
                                        {
                                            if (string.Compare(grpName, defaultMailQuota.ConfigValue.Trim(), true) != 0)
                                            {
                                                gpc.AddParameter("UseDatabaseQuotaDefaults", false);
                                            }
                                            else
                                            {
                                                isDefaultMailQuota = true;
                                                gpc.AddParameter("UseDatabaseQuotaDefaults", true);
                                            }
                                        }
                                        else if (string.Compare(grpName, "300 MB", true) != 0)
                                        {
                                            gpc.AddParameter("UseDatabaseQuotaDefaults", false);
                                        }
                                        else
                                        {
                                            isDefaultMailQuota = true;
                                            gpc.AddParameter("UseDatabaseQuotaDefaults", true);
                                        }
                                        

                                        if (mailQuota != null && !isDefaultMailQuota)
                                        {
                                            string issueWarningQuota = !string.IsNullOrEmpty(mailQuota.IssueWarningQuota) ? Convert.ToString(mailQuota.IssueWarningQuota.Split('(')[0]).Trim() : "300 MB";
                                            string prohibitSendQuota = !string.IsNullOrEmpty(mailQuota.ProhibitSendQuota) ? Convert.ToString(mailQuota.ProhibitSendQuota.Split('(')[0]).Trim() : "300 MB";

                                            gpc.AddParameter("IssueWarningQuota", issueWarningQuota);
                                            gpc.AddParameter("ProhibitSendQuota", prohibitSendQuota);
                                        }

                                        gpcOutput = gpc.Invoke();
                                        if (!string.IsNullOrEmpty(message))
                                        {
                                            message += ",";
                                        }
                                        else
                                        {
                                            message += " MailQuota: ";
                                        }

                                        message += (isDefaultMailQuota ? string.Format(" Default Mail Quota({0})", grpName) : grpName);
                                    }
                                    catch (Exception ex)
                                    {
                                        message += Convert.ToString(ex.Message) + Environment.NewLine;
                                        if (ex.InnerException != null)
                                            message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                                    }
                                }
                                isAdded = true;
                            }

                            message += string.Format("Add MailQuota successfully done to the EmailId:{0}", Convert.ToString(emailId));

                        }
                    }
                    catch (Exception ex)
                    {
                        message += Convert.ToString(ex.Message) + Environment.NewLine;
                        if (ex.InnerException != null)
                            message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = string.Format("{0}-{1}", "Error while Add entitlement for user account in Exchange Mail Server.", ex) };
            }
            return new ProvisioningResult() { Result = true, Message = string.Format("{0} for account {1}.", message, fields["AccountName"].Value, message) };
        }

        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
            bool isRemoved = false;
            string message = string.Empty;
            string emailId = string.Empty;
            try
            {
                ICollection<PSObject> gpcOutput = null;
                using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(3)))
                {
                    try
                    {
                        rsp1.StateChanged += rsp_StateChanged;
                        rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                        rsp1.Open();

                        while (!IsRunSpaceOpened)
                        {
                            System.Threading.Thread.Sleep(ThreadSleeperTime);
                        }
                        if (!string.IsNullOrEmpty(fields["Groups"].Value))
                        {

                            if (fields["UserType"].Value == AccountType.Shared.ToString())
                            {
                                emailId = fields["AccountName"].Value + "@" + fields["DomainName"].Value;
                            }
                            else
                            {
                                emailId = fields["EMAILID"].Value;
                            }

                            foreach (var grpName in fields["Groups"].Value.Split(',').ToArray())
                            {

                                using (PowerShell gpc = PowerShell.Create())
                                {
                                    gpc.InvocationStateChanged += powershell_InvocationStateChanged;
                                    gpc.Runspace = rsp1;

                                    try
                                    {
                                        gpc.AddCommand("Remove-DistributionGroupMember");
                                        gpc.AddParameter("Identity", grpName);

                                        if (IsCrossDomain)
                                        {
                                            gpc.AddParameter("DomainController", DomainController);
                                            gpc.AddParameter("Member", emailId);
                                        }
                                        else
                                        {
                                            gpc.AddParameter("Member", emailId);
                                        }
                                        gpc.AddParameter("Confirm", false);

                                        gpcOutput = gpc.Invoke();

                                        if (!string.IsNullOrEmpty(message))
                                        {
                                            message += ",";
                                        }
                                        else
                                        {
                                            message += "Groups: ";
                                        }

                                        message += grpName;
                                    }
                                    catch (Exception ex)
                                    {
                                        message += Convert.ToString(ex.Message) + Environment.NewLine;
                                        if (ex.InnerException != null)
                                            message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                                    }
                                }

                                isRemoved = true;
                            }

                            message += string.Format("  RemoveEntitlement successfully done to the Email Id: {0}", Convert.ToString(emailId));
                        }
                    }
                    catch (Exception ex)
                    {
                        message += Convert.ToString(ex.Message) + Environment.NewLine;
                        if (ex.InnerException != null)
                            message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = string.Format("{0}-{1}", "Error while creating user in Exchange Mail Server.", ex) };
            }
            return new ProvisioningResult() { Result = true, Message = string.Format("{0} for account {1}", message, fields["AccountName"].Value, message) };
        }

        public override ProvisioningResult UpdateAccount(ProvisioningParamCollection fields)
        {
            string message = string.Empty;
            string emailId = string.Empty;
            try
            {
                ICollection<PSObject> gpcOutput = null;
                using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(3)))
                {
                    try
                    {
                        rsp1.StateChanged += rsp_StateChanged;
                        rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                        rsp1.Open();

                        while (!IsRunSpaceOpened)
                        {
                            System.Threading.Thread.Sleep(ThreadSleeperTime);
                        }

                        using (PowerShell gpc = PowerShell.Create())
                        {
                            gpc.InvocationStateChanged += powershell_InvocationStateChanged;
                            gpc.Runspace = rsp1;

                            try
                            {
                                if (fields["UserType"].Value == AccountType.Shared.ToString())
                                {
                                    emailId = fields["AccountName"].Value + "@" + fields["DomainName"].Value;
                                }
                                else
                                {
                                    emailId = fields["EMAILID"].Value;
                                }

                                gpc.AddCommand("Set-User");
                                // gpc.AddParameter("DisplayName", fields["DisplayName"].Value);
                                gpc.AddParameter("Department", fields["Department"].Value);
                                gpc.AddParameter("Title", fields["Title"].Value);
                                gpc.AddParameter("Company", fields["Company"].Value);
                                gpc.AddParameter("Mobile", fields["Mobile"].Value);
                                gpc.AddParameter("FirstName", fields["FirstName"].Value);
                                gpc.AddParameter("LastName", fields["LastName"].Value);
                                gpc.AddParameter("CountryOrRegion", fields["CountryOrRegion"].Value);
                                //gpc.AddParameter("Identity", fields["AccountName"].Value + "@" + fields["DomainName"].Value);
                                if (IsCrossDomain)
                                {
                                    gpc.AddParameter("DomainController", DomainController);
                                    //gpc.AddParameter("Identity", string.Format("{0}\\{1}", DefaultDomain, fields["EMAILID"].Value));
                                    gpc.AddParameter("Identity", emailId);
                                }
                                else
                                {
                                    gpc.AddParameter("Identity", emailId);
                                }

                                gpcOutput = gpc.Invoke();

                                message += "UpdateAccount successfully done";
                            }
                            catch (Exception ex)
                            {
                                message += Convert.ToString(ex.Message) + Environment.NewLine;
                                if (ex.InnerException != null)
                                    message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        message += Convert.ToString(ex.Message) + Environment.NewLine;
                        if (ex.InnerException != null)
                            message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = string.Format("{0}-{1}", "Error while updating user in Exchange Mail Server.", ex) };
            }
            return new ProvisioningResult() { Result = true, Message = string.Format("{0} for account {1}.", message, fields["AccountName"].Value) };
        }
        #region HideMailBox
        private string HideMailBox(ProvisioningParamCollection fields)
        {
            string message = string.Empty;
            try
            {
                if (fields["HideMailAccount"].Value != null && fields["HideMailAccount"].Value == "1")
                {
                    ICollection<PSObject> gpcOutput = null;
                    using (Runspace rsp1 = RunspaceFactory.CreateRunspace(CreateConnectionForRunspace(3)))
                    {
                        try
                        {
                            rsp1.StateChanged += rsp_StateChanged;
                            rsp1.AvailabilityChanged += rsp_AvailabilityChanged;
                            rsp1.Open();

                            while (!IsRunSpaceOpened)
                            {
                                System.Threading.Thread.Sleep(ThreadSleeperTime);
                            }

                            string smsAccountName = Convert.ToString(fields["AccountName"].Value).Trim();
                            var command = new Command("Set-Mailbox");
                            if (IsCrossDomain)
                            {
                                command.Parameters.Add("DomainController", DomainController);
                                command.Parameters.Add("Identity", string.Format("{0}\\{1}", DefaultDomain, smsAccountName));
                            }
                            else
                            {
                                command.Parameters.Add("Identity", string.Format("{0}\\{1}", DefaultDomain, smsAccountName));
                            }

                            if (!string.IsNullOrEmpty(fields["Database"].Value))
                                command.Parameters.Add("Database", fields["Database"].Value);

                            command.Parameters.Add("HiddenFromAddressListsEnabled", true);
                            using (var pipeline = rsp1.CreatePipeline())
                            {
                                pipeline.Commands.Add(command);
                                var results = pipeline.Invoke();

                                message += "Set-Mailbox -HiddenFromAddressListsEnabled executed ";
                                if (results.Count <= 0)
                                {
                                    if (pipeline.Error != null && pipeline.Error.Count > 0)
                                    {
                                        message += pipeline.Error.Read().ToString() + Environment.NewLine;

                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            message += string.Format("{0}-{1}", "Error while hide user account in Exchange Mail Server.", ex.Message);
                            message += Convert.ToString(ex.Message) + Environment.NewLine;
                            if (ex.InnerException != null)
                                message += Convert.ToString(ex.InnerException.Message) + Environment.NewLine;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message += string.Format("{0}-{1}", "Error while hide user account in Exchange Mail Server.", ex.Message);
            }
            return message;
        }
        #endregion

        private void rsp_AvailabilityChanged(object sender, RunspaceAvailabilityEventArgs e)
        {
            //WriteDiscoveryLog(LogLevelType.Info, string.Format("run space availability {0}", e.RunspaceAvailability));
            if (e.RunspaceAvailability == RunspaceAvailability.None)
            {
                System.Threading.Thread.Sleep(ThreadSleeperTime);
                IsRunSpaceOpened = false;
            }
        }
        private void powershell_InvocationStateChanged(object sender, PSInvocationStateChangedEventArgs e)
        {
            //  WriteDiscoveryLog(LogLevelType.Info, string.Format("Powershel state change {0}", e.InvocationStateInfo.State));

        }
        private void rsp_StateChanged(object sender, RunspaceStateEventArgs e)
        {
            //  WriteDiscoveryLog(LogLevelType.Info, string.Format("run space statechanged {0}", e.RunspaceStateInfo.State + " - " + e.RunspaceStateInfo.Reason));

            if (e.RunspaceStateInfo.State == RunspaceState.Broken || e.RunspaceStateInfo.State == RunspaceState.BeforeOpen
                || e.RunspaceStateInfo.State == RunspaceState.Opening)
            {
                IsRunSpaceOpened = false;
                System.Threading.Thread.Sleep(ThreadSleeperTime);
            }

            if (e.RunspaceStateInfo.State == RunspaceState.Opened)
            {
                IsRunSpaceOpened = true;
            }
        }
    }

}