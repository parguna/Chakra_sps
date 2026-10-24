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
#endregion

namespace DI.Chakra.Connector.OracleEBS
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        public EntitlementType _entitlementType;
        private EndpointConfiguration oracleEBSConfig = null;
        public EndpointConfiguration OracleEBSConfiguration
        {
            get
            {
                return oracleEBSConfig;
            }

            set
            {
                this.oracleEBSConfig = value;
            }
        }
        public EndPoint endpoint;

        private string Configuration(EndpointKey Key)
        {
            var config = OracleEBSConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {
            bool ReturnValue = true;
            try
            {
                OracleEBSUserInfo oracleEBSUserInfo = new OracleEBSUserInfo();

                string CreateLoginQuery = String.Format(@"declare
                                               begin
                                                fnd_user_pkg.createuser(x_user_name =>'{0}', x_owner => null, x_unencrypted_password => '{1}',
                                                x_session_number => {2}, x_start_date => sysdate, x_end_date => null, x_email_address => '{3}' );
                                                commit;
                                                end;", fields["USER_NAME"].Value.ToUpper(), fields["AccountPassword"].Value, 1, fields["EMAIL_ADDRESS"].Value);

                ReturnValue = (ReturnValue) ? putOracleQueryToDB(CreateLoginQuery) : ReturnValue;

            }
            catch (Exception ex)
            {
                throw;
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        private bool putOracleQueryToDB(string strQry)
        {
            bool ReturnValue = false;
            OracleConnection OrCon = null;
            OracleCommand OrCom = null;
            //OracleEBSConfiguration OracleEBSConfiguration = new OracleEBSConfiguration();
            try
            {
                // WriteDiscoveryLog(LogLevelType.Info, string.Format("Started putOracleQueryToDB Mehtod"));
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
                int Result = OrCom.ExecuteNonQuery();
                ReturnValue = true;
                // WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed putOracleQueryToDB Mehtod"));
            }
            catch (Exception ex)
            {
                // WriteDiscoveryLog(LogLevelType.Info, string.Format("Error occured while Establishing the Connection (putOracleQueryToDB Method)."));
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
        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            bool ReturnValue = false;
            try
            {
                string CreateLoginQuery = String.Format(@"BEGIN
                                           FND_USER_PKG.DisableUser('{0}');
                                       END;", fields["USER_NAME"].Value.ToUpper());//String.Format("delete from fnd_user where user_name = {0}", fields["USER_NAME"].Value.ToUpper());
                putOracleQueryToDB(CreateLoginQuery);
                ReturnValue = true;
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, string.Format("User {0} hasbeen locked Successfully", fields["USER_NAME"].Value.ToUpper()));
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format("Failed to lock User {0} ", fields["USER_NAME"].Value.ToUpper()), ex);
                return new ProvisioningResult() { Result = false, Message = string.Empty };
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {
            bool ReturnValue = false;
            try
            {

                string disableUser = String.Format(@"BEGIN
                                           FND_USER_PKG.DisableUser('{0}');
                                       END;", fields["USER_NAME"].Value.ToUpper());

                ReturnValue = putOracleQueryToDB(disableUser);

                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, string.Format("User {0} locked Successfully", fields["USER_NAME"].Value.ToUpper()));
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format("Failed to lock user {0} ", fields["USER_NAME"].Value.ToUpper()), ex);
                return new ProvisioningResult() { Result = false, Message = string.Empty };
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult UnLockAccount(ProvisioningParamCollection fields)
        {
            bool ReturnValue = false;
            try
            {
                string CreateLoginQuery = String.Format(@"BEGIN
                                           FND_USER_PKG.EnableUser('{0}');
                                       END;", fields["USER_NAME"].Value.ToUpper());//String.Format("delete from fnd_user where user_name = {0}", fields["USER_NAME"].Value.ToUpper());
                putOracleQueryToDB(CreateLoginQuery);
                ReturnValue = true;
            }
            catch (Exception)
            {
                throw;
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {

            string OracleItemQuery = "";
            bool ReturnValue = true;

            //Add responsibility / Role to a user
           
                {

                    foreach (string res in fields["Responsibility"].Value.Split(',').ToArray())
                    {
                        OracleItemQuery = string.Format(@" BEGIN
                               FND_USER_PKG.ADDRESP(
                                  USERNAME        =>  '{0}',              -- User Name -- <change it>
                                  RESP_APP        =>  '{1}',              -- Apps Short Name
                                  RESP_KEY        =>  '{2}',              -- Responsibility Key
                                  SECURITY_GROUP  =>  'STANDARD',
                                  DESCRIPTION     =>  NULL,
                                  START_DATE      =>  SYSDATE,
                                  END_DATE        =>  NULL);
                                    COMMIT;
                                    END;", fields["USER_NAME"].Value.ToUpper(), fields["APPLICATION_SHORT_NAME"].Value.ToUpper(), fields["RESPONSIBILITY_KEY"].Value.ToUpper()); //, oebsUser.Responsibilities.FirstOrDefault()
                    
                        if (OracleItemQuery != string.Empty)
                        {
                            ReturnValue = (ReturnValue) ? putOracleQueryToDB(OracleItemQuery) : ReturnValue;
                        }
                    }
                }
            
           
                foreach (string role in fields["Roles"].Value.Split(',').ToArray())
                {

                    OracleItemQuery = string.Format(@" Begin
                        wf_local_synch.PropagateUserRole(
                        p_user_name => '{0}',
                        p_role_name => '{1}');
                        commit;
                                end;", fields["USER_NAME"].Value.ToUpper(), role.ToString());
                    if (OracleItemQuery != string.Empty)
                    {
                        ReturnValue = (ReturnValue) ? putOracleQueryToDB(OracleItemQuery) : ReturnValue;
                    }

                }
            

            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
            string OracleItemQuery = "";
            bool ReturnValue = true;

            //Add responsibility / Role to a user
           

                foreach (string res in fields["Responsibility"].Value.Split(',').ToArray())
                {
                    OracleItemQuery = string.Format(@" BEGIN
                               FND_USER_PKG.DELRESP(
                                  USERNAME        =>  '{0}',              -- User Name -- <change it>
                                  RESP_APP        =>  '{1}',              -- Apps Short Name
                                  RESP_KEY        =>  '{2}',              -- Responsibility Key
                                  SECURITY_GROUP  =>  'STANDARD'
                                 );
                                    COMMIT;
                                    END;", fields["USER_NAME"].Value.ToUpper(), fields["APPLICATION_SHORT_NAME"].Value, fields["RESPONSIBILITY_KEY"].Value.ToUpper()); //, oebsUser.Responsibilities.FirstOrDefault()

                    if (OracleItemQuery != string.Empty)
                    {

                        ReturnValue = (ReturnValue) ? putOracleQueryToDB(OracleItemQuery) : ReturnValue;
                    }

                }
            
          

                foreach (string role in fields["Roles"].Value.Split(',').ToArray())
                {
                        OracleItemQuery = string.Format(@" Begin
                        wf_local_synch.PropagateUserRole(
                        p_user_name => '{0}',
                        p_role_name => '{1}');
                        commit;
                                end;", fields["USER_NAME"].Value.ToUpper(), role.ToString());
                        if (OracleItemQuery != string.Empty)
                        {

                            ReturnValue = (ReturnValue) ? putOracleQueryToDB(OracleItemQuery) : ReturnValue;
                        }
                }
            
            return new ProvisioningResult() { Result = true, Message = string.Empty };
          }
        public override ProvisioningResult UpdateAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult ResetPassword(ProvisioningParamCollection fields)
        {
            bool ReturnValue = false;
            try
            {

                string CreateLoginQuery = String.Format(@"BEGIN
                                                    FND_USER_PKG.UPDATEUSER(x_user_name            => '{0}'
                                                                           ,x_owner                => ''
                                                                           ,x_unencrypted_password => '{1}'
                                                                           ,x_password_date        => SYSDATE + 500);

                                                    END;", fields["USER_NAME"].Value.ToUpper());
                putOracleQueryToDB(CreateLoginQuery);
                ReturnValue = true;
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, string.Format("Password changed for user {0} ", fields["USER_NAME"].Value));
            }

            catch (Exception ex)
            {

                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, string.Format("Change password failed for user {0} ", fields["USER_NAME"].Value), ex);
                return new ProvisioningResult() { Result = false, Message = string.Empty };
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

    }
}
