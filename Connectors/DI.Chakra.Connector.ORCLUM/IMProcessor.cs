using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Configuration;
using System.Data.OracleClient;
using DI.Chakra.DomainModel;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.Connector.ORCLUM
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        private EndpointConfiguration oraUMConfig = null;
        public EndPoint endpoint;



        public EndpointConfiguration oracleUMConfiguration
        {
            get
            {
                return oraUMConfig;
            }

            set
            {
                this.oraUMConfig = value;
            }
        }

        private string Configuration(EndpointKey Key)
        {
            var config = oracleUMConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public  bool Connect()
        {
            string oracleConnectionString = string.Empty;
            OracleConnection OrCon = null;
            try
            {

                string portNo = Configuration(EndpointKey.PortNo);
                string InstanceName = Configuration(EndpointKey.InstanceName);
                string Password = Configuration(EndpointKey.Password);
                oracleConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", Configuration(EndpointKey.IPAddress), portNo, InstanceName, Configuration(EndpointKey.UserName), Password);
                OrCon = new OracleConnection(oracleConnectionString);
                OrCon.Open();
               
            }
            catch (Exception ex)
            {
               
                return false;
            }
            finally
            {
                OrCon.Close();
                OrCon.Dispose();
            }
            return true;
        }


        private bool putOracleQueryToDB(string strQry)
        {
            bool ReturnValue = false;
            OracleConnection OrCon = null;
            OracleCommand OrCom = null;
            try
            {
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
            }
            catch (Exception ex)
            {
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
        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)

        {
            bool ReturnValue = false;
            try
            {
                this.Connect();

                string CreateLoginQuery = String.Format("CREATE USER {0} IDENTIFIED BY {1}", fields["UserName"].Value, fields["AccountPassword"].Value);
                ReturnValue = putOracleQueryToDB(CreateLoginQuery);




            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)

        {
            bool ReturnValue = false;
            try
            {
                this.Connect();
                string CreateLoginQuery = String.Format("ALTER USER {0} ACCOUNT LOCK", fields["UserName"].Value);
                ReturnValue = putOracleQueryToDB(CreateLoginQuery);
            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public override ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {
            bool ReturnValue = false;
            try
            {
                this.Connect();
                string CreateLoginQuery = String.Format("ALTER USER {0} ACCOUNT LOCK", fields["UserName"].Value);
                ReturnValue = putOracleQueryToDB(CreateLoginQuery);

            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public override ProvisioningResult UnLockAccount(ProvisioningParamCollection fields)
        {
            bool ReturnValue = false;
            try
            {
                this.Connect();
                string CreateLoginQuery = String.Format("ALTER USER {0} ACCOUNT UNLOCK", fields["UserName"].Value);
                ReturnValue = putOracleQueryToDB(CreateLoginQuery);

            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {

            try
            {
                this.Connect();
                string CreateQuery = "";
               
                foreach (string Role in fields["Roles"].Value.Split(',').ToArray())//oracleUMUserInfo.Roles)
                {
                    CreateQuery = String.Format("GRANT {0} TO {1}", Role, fields["UserName"].Value);
                    putOracleQueryToDB(CreateQuery);
                }
            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
           
            try
            {
                this.Connect();
                string CreateQuery = "";

                foreach (string Role in fields["Roles"].Value.Split(',').ToArray())
                {
                    CreateQuery = String.Format("REVOKE {0} FROM {1}", Role, fields["UserName"].Value);
                    putOracleQueryToDB(CreateQuery);
                }



            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
}
            

            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }


        public override ProvisioningResult ResetPassword(ProvisioningParamCollection fields)
        {
            try
            {
                this.Connect();

                string CreateLoginQuery = String.Format("ALTER USER {0} IDENTIFIED BY {1}",fields["UserName"].Value,fields["Password"].Value);// UserName, Password);
                putOracleQueryToDB(CreateLoginQuery);

            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
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
