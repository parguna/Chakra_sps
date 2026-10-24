using DI.Chakra.Connector.Linux;
using DI.Chakra.Infrastructure;
using DI.Chakra.DomainModel;
using Tamir.SharpSsh;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure.Enums;
namespace DI.Chakra.Connector.Unix
{
   public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        SshConnectionInfo input;
        SshExec exec;

        public EndPoint EndPoint
        {
            get; set;
        }
        public AccessRequest CurrentRequest { get; set; }
        public EndpointConfiguration LinuxConfig 
        {
            get; set;
        }
        private string Configuration(EndpointKey Key)
        {
            var config = LinuxConfig.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }

        public  bool Connect()
        {


            try
            {
                input = LinuxUtil.GetInput();
                exec = new SshExec(Configuration(EndpointKey.IPAddress), Configuration(EndpointKey.UserName));
                if (Configuration(EndpointKey.Password) != null) exec.Password = RSAHelper.DecryptData(Configuration(EndpointKey.Password));

                exec.Connect();

                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Connection Established SuccessFully."));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {
              
            try
            {
                if (this.Connect())
                {

                    string Groupcommand = "";
                    if (this.LinuxConfig.EndpointSystem.EndPointSystemType.Id == (int)DI.Chakra.Infrastructure.Enums.EndpointType.Linux)
                    {
                        Groupcommand = string.Format("adduser -c \"{0}\" {1}", fields["UserIdInfo"].Value, fields["UserName"].Value);
                    }
                    else
                    {
                        Groupcommand = string.Format("useradd -c \"{0}\" {1}", fields["UserIdInfo"].Value, fields["UserName"].Value);
                    }

                    string Groupoutput = exec.RunCommand(Groupcommand);
                    exec.Close();
                    ChangePassword(fields["UserName"].Value, fields["AccountPassword"].Value, true);
                }

                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Info, string.Format("New User {0} hasbeen created", fields["UserName"].Value));

                return new ProvisioningResult() { Result = true, Message = string.Empty };
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Exception, string.Format("New User {0} creation failed", fields["UserName"].Value), ex);
                return new ProvisioningResult() { Result = true, Message = string.Empty };
            }
        }

        public  bool ChangePassword(string UserName, string Password,  bool isReactivation)
        {
            try
            {
                if (this.Connect())
                {

                    string groupcommand = "";
                    if (this.LinuxConfig.EndpointSystem.EndPointSystemType.Id == (int)DI.Chakra.Infrastructure.Enums.EndpointType.Linux)
                    {
                        groupcommand = string.Format("echo -e \"{1}\n{1}\" | passwd {0}", UserName, Password);
                    }
                    else
                    {
                        groupcommand = string.Format("echo -e \"{1}\n{1}\" | passwd {0}", UserName, Password);
                    }

                    string groupoutput = exec.RunCommand(groupcommand);

                    exec.Close();
                  return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            
            try
            {
                if (this.Connect())
                {

                    string Deletecommand = "userdel -r  " + fields["UserName"].Value;
                   
                    string Deleteoutput = exec.RunCommand(Deletecommand);
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Info, string.Format("User {0} hasbeen deleted Successfully", fields["UserName"].Value));

                    exec.Close();

                    return new ProvisioningResult() { Result = true, Message = string.Empty };
                }
                return new ProvisioningResult() { Result = true, Message = string.Empty };
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Exception, string.Format("Deleting User {0} failed", fields["UserName"].Value), ex);
                return new ProvisioningResult() { Result = true, Message = string.Empty };
            }
        }

        public override ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {

           try
            {
                if (this.Connect())
                {

           string Deletecommand = "passwd -l  " + fields["UserName"].Value;
        string Deleteoutput = exec.RunCommand(Deletecommand);
        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id,  LogLevelType.Info, string.Format("User {0} hasbeen locked Successfully", fields["UserName"].Value));

              exec.Close();

              return new ProvisioningResult() { Result = true, Message = string.Empty };
                }

                return new ProvisioningResult() { Result = true, Message = string.Empty };
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Exception, string.Format("locking User {0} failed", fields["UserName"].Value),ex);
                return new ProvisioningResult() { Result = true, Message = string.Empty };
            }
}

        public override ProvisioningResult UnLockAccount(ProvisioningParamCollection fields)
        {
           
            try
            {
                if (this.Connect())
                {
                    string Deletecommand = "passwd -u  " + fields["UserName"].Value;
                    string Deleteoutput = exec.RunCommand(Deletecommand);
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Info, string.Format("User {0} hasbeen unlocked Successfully", fields["UserName"].Value));

                    exec.Close();

                    return new ProvisioningResult() { Result = true, Message = string.Empty };
                }
                return new ProvisioningResult() { Result = false, Message = string.Empty };
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Exception, string.Format("unlocking User {0} failed", fields["UserName"].Value), ex);
                return new ProvisioningResult() { Result = false, Message = string.Empty };
            }
        }
        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {
           
            try
            {
                if (this.Connect())
                {
                    if (fields["Groups"].Value == null)
                        return new ProvisioningResult() { Result = true, Message = string.Empty };
                    
                    foreach (var group in fields["Groups"].Value .Split(','))
                    {
                        
                            string Groupcommand = Groupcommand = "gpasswd -a  " + fields["UserName"].Value + " " + group;
                            string Groupoutput = exec.RunCommand(Groupcommand);
                            base.AddEntitlement(fields);
                          //  DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Info, string.Format("User {0} hasbeen added to the group {1}", fields["UserName"].Value, group));
                        
                    }
                    exec.Close();

                    return new ProvisioningResult() { Result = true, Message = string.Empty };
                }
                return new ProvisioningResult() { Result = true, Message = string.Empty };
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Exception, string.Format("Adding user {0} to groups failed", fields["UserName"].Value), ex);
                return new ProvisioningResult() { Result = true, Message = string.Empty };
            }
        }

        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
          
            try
            {
                if (this.Connect())
                {
                    foreach (var group in fields["Groups"].Value)
                    {
                        string Groupcommand = Groupcommand = "gpasswd -d  " + fields["UserName"].Value + " " + group;
                        string Groupoutput = exec.RunCommand(Groupcommand);
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Info, string.Format("User {0} hasbeen deleted from the group {1}", fields["UserName"].Value, group));
                    }
                    exec.Close();

                    return new ProvisioningResult() { Result = true, Message = string.Empty };
                }
                return new ProvisioningResult() { Result = false, Message = string.Empty };
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Exception, string.Format("Deleting user {0} from groups failed", fields["UserName"].Value), ex);
                return new ProvisioningResult() { Result = false, Message = string.Empty };
            }
        }

        public override ProvisioningResult UpdateAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

    }
}
