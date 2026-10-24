using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace DI.Chakra.Connector.MSSQL
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        private DomainModel.EndpointConfiguration m_msqlConfig = null;
        MSSQLUMUserInfo _msqlUserInfo = new MSSQLUMUserInfo();
        MSSQLUMLoginInfo _msqlLoginInfo = new MSSQLUMLoginInfo();
        SqlConnection sqlConnect = null;
        string connectionString = "";
        public EndPoint endpoint;
        string databaseName = "";
        string _connectionString = "";
        public EndPoint EndPoint
        {
            get; set;
        }
        public EndpointConfiguration MSSQLUMConfiguraiton
        {
            get
            {
                return m_msqlConfig;
            }

            set
            {
                this.m_msqlConfig = value;
            }
        }

        private string Configuration(EndpointKey Key)
        {
            var config = MSSQLUMConfiguraiton.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }

        public bool Connect()
        {
            connectionString = "Data Source=" + Configuration(EndpointKey.IPAddress) + "; Initial Catalog=" + Configuration(EndpointKey.InstanceName) + ";Persist Security Info= True; User ID=" + Configuration(EndpointKey.UserName) + ";Password=" + Configuration(EndpointKey.Password) + ";Connect Timeout=100";
            try
            {
                sqlConnect = new SqlConnection(connectionString);
                sqlConnect.Open();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }


        public bool SqlCon()
        {
            SqlCommand cmd = null;
            //     con.Open();
            cmd = new SqlCommand();
            cmd.Connection = sqlConnect;
            cmd.CommandType = CommandType.Text;
            return true;
        }




        public bool CreateMssqlUser(ProvisioningParamCollection fields)
        {
            bool isSqlcreated = false;
            try
            {
                string LoginName = fields["Name"].Value;
                string Password = fields["Password"].Value;
                string Database = Configuration(EndpointKey.InstanceName);
                string CreateLoginQuery = String.Format("CREATE LOGIN {0} WITH PASSWORD = '{1}', CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF; USE {2}", LoginName, Password, Database);
                string CreateUserQuery = String.Format("CREATE USER {0} FOR LOGIN {0}", LoginName);
                this.Connect();
                SqlCommand cmd = null;
                cmd = new SqlCommand();
                cmd.Connection = sqlConnect;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = CreateLoginQuery;
                cmd.CommandText = CreateUserQuery;
                int Result = cmd.ExecuteNonQuery();
                cmd.CommandText = string.Format("select sid from sys.syslogins where loginname='{0}'", LoginName);
                byte[] binaryString = (byte[])CheckNull.AgainstByte(cmd.ExecuteScalar());
                if (binaryString != null)
                {
                    // if the original encoding was ASCII
                    string sid = Convert.ToBase64String(binaryString); // BitConverter.ToString(binaryString);
                    StringBuilder hex = new StringBuilder(binaryString.Length * 2);
                    foreach (byte b in binaryString)
                        hex.AppendFormat("{0:X2}", b);
                }
           }
            catch (Exception ex)
            {
                isSqlcreated = false;

            }
            return isSqlcreated;
        }



        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {
            try
            {
                this.Connect();
                bool isMSSQLCreated = CreateMssqlUser(fields);
                isMSSQLCreated = true;
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = true, Message = string.Empty };
            }
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }


        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            bool bReturn = false;
            this.Connect();
            bReturn = DisableUser(fields);
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public bool DisableUser(ProvisioningParamCollection fields)
        {
            bool ReturnValue = false;
            Connect();
            string Username = fields["Name"].Value;
            try
            {
                string DeleteUserQuery = "ALTER LOGIN {0} Disable";
                SqlCommand cmd = null;
                // con.Open();
                cmd = new SqlCommand();
                Connect();
                cmd.Connection = sqlConnect;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = string.Format(DeleteUserQuery, Username);
                int Result = cmd.ExecuteNonQuery();
                ReturnValue = true;

            }
            catch (Exception)
            {
                throw;
            }
            return ReturnValue;
        }

        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {

            try
            {
                this.Connect();
                string Database = Configuration(EndpointKey.InstanceName);
                foreach (var role in fields["Roles"].Value.Split(',').ToArray())
                {
                    if (Database != null)
                    {
                        AddLoginToRole(fields["LoginName"].Value, role, Database);

                    }
                    else
                    {
                        AddLoginToRole(fields["LoginName"].Value, role, "");
                    }

                }
            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }


















        public bool AddLoginToRole(string LoginName, string RoleName, string DatabaseName)
        {
            bool ReturnValue = false;
            DatabaseName = Configuration(Infrastructure.Enums.EndpointKey.InstanceName);
            try
            {
                string AddRoleQuery = "Use {2} EXEC sp_addrolemember '{0}', {1};";
                if (DatabaseName == "")
                {
                    AddRoleQuery = "EXEC sp_addrolemember '{0}', {1};";
                }
                SqlCommand cmd = null;
                //     con.Open();
                cmd = new SqlCommand();
                cmd.Connection = sqlConnect;
                cmd.CommandType = CommandType.Text;


                cmd.CommandText = string.Format(AddRoleQuery, RoleName, LoginName, DatabaseName);
                int Result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
               
                return false;
            }


            ReturnValue = true;

            return true;
        }


        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
            try
            {

                this.Connect();
                foreach (var role in fields["Roles"].Value.Split(',').ToArray())
                {


                    RemoveLoginFromRole(fields["Name"].Value, role);
                }
            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }






        public bool RemoveLoginFromRole(string LoginName, string RoleName)
        {

            bool ReturnValue = false;
            try
            {
                string AddRoleQuery = "EXEC sp_droprolemember '{0}', {1};";
                SqlCommand cmd = null;
                //     con.Open();
                cmd = new SqlCommand();
                cmd.Connection = sqlConnect;
                cmd.CommandType = CommandType.Text;


                cmd.CommandText = string.Format(AddRoleQuery, RoleName, LoginName);
                int Result = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }


            ReturnValue = true;

            return true;

        }


        public override ProvisioningResult ResetPassword(ProvisioningParamCollection fields)
        {
            try
            {
                this.Connect();
                string ChangeUserPasswordQuery = @"ALTER LOGIN {0} WITH PASSWORD = '{1}';";
                SqlCommand cmd = null;
                //     con.Open();
                cmd = new SqlCommand();
                cmd.Connection = sqlConnect;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = string.Format(ChangeUserPasswordQuery, fields["LoginName"].Value, fields["password"].Value);
                int Result = cmd.ExecuteNonQuery();
            }
            catch (Exception Ex)
            {
                return new ProvisioningResult() { Result = true, Message = Ex.Message };
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





