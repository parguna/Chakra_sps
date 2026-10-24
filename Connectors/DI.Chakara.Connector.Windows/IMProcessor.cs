using DI.Chakra.Connector;
using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using System.DirectoryServices.AccountManagement;
using System.Management;
using System.DirectoryServices;
using DI.Chakra.DomainModel;
using System.Collections;
using System.Net;
using System.Security.Principal;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Connector.Windows
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        private EndpointConfiguration m_winConfig = null;
        private Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        private Dictionary<long, string> dicGroups = new Dictionary<long, string>();
        private Dictionary<string, ArrayList> dicGroupUsers = new Dictionary<string, ArrayList>();
        ManagementScope mgmtScope;
        public DomainModel.EndPoint endpoint;
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

        public DomainModel.EndPoint EndPoint
        {
            get; set;
        }
        private string Configuration(EndpointKey Key)
        {
            var config = WindowsConfiguraiton.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public bool Connect()
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
        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {
            try
            {
                if (this.Connect() == false)
                    return new ProvisioningResult() { Result = false, Message = "Connection could not be made" };
                PrincipalContext principalCntx = new PrincipalContext(ContextType.Machine, Configuration(EndpointKey.IPAddress), Configuration(EndpointKey.UserName), Configuration(EndpointKey.Password));
                UserPrincipal usrPrincipal = new UserPrincipal(principalCntx);
                usrPrincipal.Name = fields["AccountName"].Value;
                usrPrincipal.DisplayName = fields["FullName"].Value;
                usrPrincipal.SetPassword(fields["Password"].Value);
                usrPrincipal.Description = fields["Description"].Value;
                usrPrincipal.UserCannotChangePassword = false;
                usrPrincipal.PasswordNeverExpires = true;
                usrPrincipal.Enabled = true;
                usrPrincipal.Save();
            }
            catch (Exception ex)
            {

                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            try
            {
                this.Connect();
                PrincipalContext principalCntx = new PrincipalContext(ContextType.Machine, Configuration(EndpointKey.IPAddress), Configuration(EndpointKey.UserName), Configuration(EndpointKey.Password));
                UserPrincipal usrPrincipal = UserPrincipal.FindByIdentity(principalCntx, IdentityType.Name, fields["AccountName"].Value);
                usrPrincipal.Delete();
            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = true, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {
            try
            {
                this.Connect();
                PrincipalContext principalCntx = new PrincipalContext(ContextType.Machine, Configuration(EndpointKey.IPAddress), Configuration(EndpointKey.UserName), Configuration(EndpointKey.Password));
                UserPrincipal usrPrincipal = UserPrincipal.FindByIdentity(principalCntx, IdentityType.Name, fields["AccountName"].Value);
                usrPrincipal.Enabled = false;
                usrPrincipal.Save();
            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = true, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }        
        public override ProvisioningResult UnLockAccount(ProvisioningParamCollection fields)
        {
            try
            {
                this.Connect();
                PrincipalContext principalCntx = new PrincipalContext(ContextType.Machine, Configuration(EndpointKey.IPAddress), Configuration(EndpointKey.UserName), Configuration(EndpointKey.Password));
                UserPrincipal usrPrincipal = UserPrincipal.FindByIdentity(principalCntx, IdentityType.Name, fields["AccountName"].Value);
                usrPrincipal.Enabled = true;
                usrPrincipal.Save();
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = true, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }   
        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {
            try
            {
                this.Connect();

                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpContext.Current.Request.LogonUserIdentity.Impersonate();
                }
                PrincipalContext principalCntx = new PrincipalContext(ContextType.Machine, Configuration(EndpointKey.IPAddress), Configuration(EndpointKey.UserName), Configuration(EndpointKey.Password));

                UserPrincipal usrPrincipal = UserPrincipal.FindByIdentity(principalCntx, fields["AccountName"].Value);
                foreach (string strGroup in fields["Groups"].Value.Split(',').ToArray())
                {
                    GroupPrincipal groups = GroupPrincipal.FindByIdentity(principalCntx, strGroup);

                    if (!usrPrincipal.GetGroups().Contains(groups))
                    {
                        groups.Members.Add(usrPrincipal);
                        groups.Save();
                    }
                }
            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            finally
            {
               
            }

            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
            try
            {
                this.Connect();
                PrincipalContext principalCntx = new PrincipalContext(ContextType.Machine, Configuration(EndpointKey.IPAddress), Configuration(EndpointKey.UserName), Configuration(EndpointKey.Password));
                UserPrincipal usrPrincipal = UserPrincipal.FindByIdentity(principalCntx, IdentityType.Name, fields["AccountName"].Value);
                foreach (string groupName in fields["Groups"].Value.Split(',').ToArray())
                {
                    foreach (var grpName in usrPrincipal.GetGroups())
                    {
                        if (groupName == grpName.ToString())
                        {
                            GroupPrincipal grp = GroupPrincipal.FindByIdentity(principalCntx, IdentityType.Name, groupName);
                            grp.Members.Remove(principalCntx, IdentityType.Name, groupName);
                            grp.Save();
                            
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            finally
            {
                
            }

            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult UpdateAccount(ProvisioningParamCollection fields)
        {
            try
            {
                if (this.Connect() == false)
                    return new ProvisioningResult() { Result = false, Message = "Connection could not be made" };
                PrincipalContext principalCntx = new PrincipalContext(ContextType.Machine, Configuration(EndpointKey.IPAddress), Configuration(EndpointKey.UserName), Configuration(EndpointKey.Password));
                UserPrincipal usrPrincipal = UserPrincipal.FindByIdentity(principalCntx, IdentityType.Name, fields["AccountName"].Value);

                usrPrincipal.DisplayName = fields["FullName"].Value;
                usrPrincipal.Description = fields["Description"].Value;
                usrPrincipal.Save();
            }
            catch (Exception Ex)
            {
                return new ProvisioningResult() { Result = true, Message = Ex.Message };
            }

            return new ProvisioningResult() { Result = true, Message = string.Empty };
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
        public override object RunTheMethod(Action methodName)
        {
            methodName();
            return null;

        }
        public static object InvokeMethod(Delegate method, params object[] args)
        {
            return method.DynamicInvoke(args);
        }
        

    }
}
