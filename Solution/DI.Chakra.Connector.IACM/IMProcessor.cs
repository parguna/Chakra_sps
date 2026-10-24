using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Configuration;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.Data.SqlClient;
using CheckNulls = DI.Chakra.DomainModel.CheckNull;
using DI.Chakra.Logger;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Connector;


namespace DI.Chakra.Connector.IACM
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        public EndpointConfiguration IacmConfiguration { get; set; }
        public EndPoint EndPoint
        {
            get;
            set;
        }

        private string Configuration(EndpointKey Key)
        {
            var config = IacmConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }


        public bool Connect()
        {
            SqlConnection sqlCon = null;
            try
            {

                string IADsConStr = "";
                string UserName = "";
                string InstanceName = "";
                string Password = RSAHelper.DecryptData(Configuration(EndpointKey.Password));
                string DataSource = Configuration(EndpointKey.IPAddress);

                if (Convert.ToInt32(Configuration(EndpointKey.UserName)) != 0)
                {
                    UserName = Configuration(EndpointKey.UserName);
                }
                else
                {
                    UserName = "sa";
                }

                if (!String.IsNullOrEmpty(Configuration(EndpointKey.InstanceName)))
                {
                    InstanceName = Configuration(EndpointKey.InstanceName);
                }

                IADsConStr = string.Format("Data Source ={0}; Initial Catalog = {1}; Persist Security Info = True; User Id = {2}; Password = {3};" +
                    "Connect Timeout = 1000; providerName =System.Data.SqlClient", DataSource, InstanceName, Configuration(EndpointKey.UserName), Password);

                sqlCon = new SqlConnection(IADsConStr);
            }
            catch (Exception ex)
            {

                return false;
            }
            finally
            {
                sqlCon.Close();
                sqlCon.Dispose();
            }
            return true;

        }

        private bool putOracleQueryToDB(string strQry)
        {
            bool ReturnValue = false;
            SqlConnection sqlCon = null;
            SqlCommand SqlComd = null;

            try
            {
                string IADsConStr = "";
                string userName = "";
                string InstanceName = "";
                string Password = RSAHelper.DecryptData(Configuration(EndpointKey.Password));
                string DataSource = Configuration(EndpointKey.IPAddress);
                if (Convert.ToInt32(Configuration(EndpointKey.UserName)) != 0)
                {
                    userName = Configuration(EndpointKey.UserName);
                }
                else
                {
                    userName = "sa";
                }
                if (!String.IsNullOrEmpty(Configuration(EndpointKey.InstanceName)))
                {
                    InstanceName = Configuration(EndpointKey.InstanceName);
                }

                IADsConStr = string.Format("Data Source ={0}; Initial Catalog = {1}; Persist Security Info = True; User Id = {2}; Password = {3};" +
                      "Connect Timeout = 1000; providerName =System.Data.SqlClient", DataSource, InstanceName, Configuration(EndpointKey.UserName), Password);
                sqlCon = new SqlConnection(IADsConStr);
                sqlCon.Open();
                SqlComd = new SqlCommand();
                SqlComd.Connection = sqlCon;
                SqlComd.CommandType = CommandType.Text;
                SqlComd.CommandText = strQry;
                int Result = SqlComd.ExecuteNonQuery();
                ReturnValue = true;

            }
            catch (Exception ex)
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                    sqlCon.Dispose();
                }
                if (ex.Message != null && ex.Message.Contains("unique constraint"))
                    ReturnValue = true;
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                    sqlCon.Dispose();
                }
            }

            return ReturnValue;
        }


        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            bool ReturnValue = false;
            try
            {
                this.Connect();
                string CreateLoginQuery = String.Format("update DI_M_User set IsActive=0 where  Id= {0}", fields["Id"].Value);
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

            try
            {
                this.Connect();

                string CreateLoginQuery = String.Format("update DI_M_User set IsActive=1 where  Id= {0}", fields["Id"].Value);
                putOracleQueryToDB(CreateLoginQuery);

            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {
            bool ReturnValue = true;
            try
            {
                this.Connect();
                string CreatingQuery = "";
                var user = User.GetByName(fields["UserName"].ToString());
                foreach (var role in fields["Roles"].Value.Split(','))
                {
                    var idasRole = Role.GetByRoleName(role);
                    CreatingQuery = string.Format(@"Insert into UserRole VALUES('{0}'  , '{1}')", idasRole.Id, user.Id);
                    ReturnValue = (ReturnValue) ? putOracleQueryToDB(CreatingQuery) : ReturnValue;
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
                bool ReturnValue = true;
                this.Connect();
                string CreatingQuery = "";
                {
                    var user = User.GetByName(fields["UserName"].ToString());
                    foreach (var role in fields["Roles"].Value.Split(','))
                    {
                        var idasRole = Role.GetByRoleName(role);
                        CreatingQuery = string.Format(@"delete  UserRole where  Role_RoleId='{0}' and User_UserId='{1}')", idasRole.Id, user.Id);
                        ReturnValue = (ReturnValue) ? putOracleQueryToDB(CreatingQuery) : ReturnValue;
                    }
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


                string CreateLoginQuery = String.Format(@"UPDATE DI_M_User  SET Password ={1}  Where Id = {0}",
                                         fields["UserName"].Value, "{1}A4Ngxo5bzp8fp9+fxtJHogZQaXU=", "EOZJU9bhSfBUNDJK241K3w==");

                putOracleQueryToDB(CreateLoginQuery);


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

                string CreateLoginQuery = String.Format("update DI_M_User set IsActive=0 where  Id= {0}", fields["Id"].Value);
                ReturnValue = putOracleQueryToDB(CreateLoginQuery);
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }



        public override ProvisioningResult EnableAccount(ProvisioningParamCollection fields)
        {
            bool ReturnValue = false;
            try
            {
                this.Connect();

                string CreateLoginQuery = String.Format("update DI_M_User set IsActive=0 where  OPRID= {0}", fields["Id"].Value);
                ReturnValue = putOracleQueryToDB(CreateLoginQuery);



            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        public override ProvisioningResult DisableAccount(ProvisioningParamCollection fields)
        {
            bool ReturnValue = false;
            try
            {
                this.Connect();
                string CreateLoginQuery = String.Format("update DI_M_User set IsActive=1 whereId= {0}", fields["Id"].Value);
                ReturnValue = putOracleQueryToDB(CreateLoginQuery);
            }


            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }



            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }


    }
}