using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DBConfigHelper;
using System.Data.SqlClient;
using DI.Chakra.Logger;
using System.Data;
using System.Collections;
using DI.Chakra.DomainModel;
using System;
using System.Linq;

namespace DI.Chakra.Connector.CMS
{
   public  class IMProcessor: DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        SqlConnection sqlConnect = null;
        string connectionString = "";
        string databaseName = "";


        public EndPoint EndPoint
        {
            get; set;
        }
        public EndpointConfiguration CMSConfiguration
        {
            get; set;
        }

        private string Configuration(EndpointKey Key)
        {
            var config = CMSConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }

        public  bool Connect()
        {
            connectionString = "Data Source=" + Configuration(EndpointKey.ServerName) + "; Initial Catalog=" + Configuration(EndpointKey.DatabaseName) + ";Persist Security Info= True; User ID=" + Configuration(EndpointKey.UserName) + ";Password=" + Configuration(EndpointKey.Password) + ";Connect Timeout=100";

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

        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)

        {

            try
            {
                this.Connect();


                string existingUserQuery = @"SELECT * FROM DI_ERP_User WHERE UserName = '{0}';";
                SqlCommand cmd = null;
                cmd = new SqlCommand();
                cmd.Connection = sqlConnect;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = string.Format(existingUserQuery, fields["UserName"].Value);
                int Result = cmd.ExecuteNonQuery();

                if (Result <= 0)
                {
                    string newUserQuery = @"INSERT INTO DI_ERP_User(IDType,IdentificationNo,UserName,FirstName,MiddleName,LastName,DisplayName,KnownAs,EmployeeId,OrganisationId,BusinessRoleId,ManagerId,EmailId,TelephoneNumber,HomephoneNumber,MobileNumber,Fax,PostalCode,PostalAddress,Ethinicity,Country,IsActive,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate, LastLogonDate, IsLocked, Password) 
                                                VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9},{10},{11},'{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}',{21}, '{22}', '{23}', '{24}', '{25}', '{26}', {27}, '{28}')";
                    cmd = null;
                    cmd = new SqlCommand();
                    cmd.Connection = sqlConnect;
                    cmd.CommandType = CommandType.Text;

                    string queryToExecute = string.Format(newUserQuery, fields["IDType"].Value,
                        fields["IdentificationNo"].Value,
                        fields["UserName"].Value,
                        fields["FirstName"].Value,
                        fields["MiddleName"].Value,
                        fields["LastName"].Value,
                        fields["DisplayName"].Value,
                        fields["KnownAs"].Value,
                        fields["EmployeeId"].Value,
                        fields["EmailId"].Value,
                        fields["TelephoneNumber"].Value,
                        fields["PostalCode"].Value,
                        fields["Ethinicity"].Value,
                        fields["PostalAddress"].Value,
                         DateTime.Now.Date.ToString("MM/dd/yyyy"),
                        fields["NewUsrPassword"].Value);

                    queryToExecute = queryToExecute.Replace("null", "NULL").Replace(",,", ",NULL,");
                    cmd.CommandText = queryToExecute;
                    Result = cmd.ExecuteNonQuery();


                }

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

                string diableUserQuery = @"Declare @UserId INT;
                                            SELECT @UserId = Id FROM DI_ERP_User WHERE UserName = '{0}';
                                            IF(@UserId IS NOT NULL)
                                            BEGIN
                                                DELETE FROM ERPUserRole WHERE ERPUser_Id = @UserId;
                                            END;
                                            UPDATE DI_ERP_User SET IsActive=0, IsLocked=0 WHERE UserName = '{0}';";

                SqlCommand cmd = null;
                cmd = new SqlCommand();
                cmd.Connection = sqlConnect;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = string.Format(diableUserQuery, fields["UserName"].Value);
                int Result = cmd.ExecuteNonQuery();



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


                foreach (var role in fields["Roles"].Value)
                {
                    //table name needs to be configured in the configuration screen.
                    string ChangeUserPasswordQuery = @"Declare @UserId INT, @RoleId INT;
                                                        SELECT @UserId = Id FROM DI_ERP_User WHERE UserName = '{0}';
                                                        SELECT @RoleId = Id FROM DI_ERP_Role WHERE RoleName = '{1}';
                                                        IF(@UserId IS NOT NULL AND @RoleId IS NOT NULL)
                                                        BEGIN
                                                        IF NOT EXISTS(SELECT * FROM ERPUserRole WHERE ERPUser_Id = @UserId AND Role_Id = @RoleId)
                                                        BEGIN
                                                            INSERT INTO ERPUserRole(ERPUser_Id, Role_Id) VALUES(@UserId, @RoleId)
                                                        END
                                                        END";
                    SqlCommand cmd = null;
                    cmd = new SqlCommand();
                    cmd.Connection = sqlConnect;
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = string.Format(ChangeUserPasswordQuery, fields["UserName"].Value, role);
                    int Result = cmd.ExecuteNonQuery();

                   
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



                foreach (var role in fields["Roles"].Value)
                {
                    //table name needs to be configured in the configuration screen.
                    string ChangeUserPasswordQuery = @"Declare @UserId INT, @RoleId INT;
                                                        SELECT @UserId = Id FROM DI_ERP_User WHERE UserName = '{0}';
                                                        SELECT @RoleId = Id FROM DI_ERP_Role WHERE RoleName = '{1}';
                                                        IF(@UserId IS NOT NULL AND @RoleId IS NOT NULL)
                                                        BEGIN
                                                        DELETE FROM ERPUserRole WHERE ERPUser_Id = @UserId AND Role_Id = @RoleId)
                                                        END";
                    SqlCommand cmd = null;
                    cmd = new SqlCommand();
                    cmd.Connection = sqlConnect;
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = string.Format(ChangeUserPasswordQuery, fields["UserName"].Value, role);
                    int Result = cmd.ExecuteNonQuery();


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


                string ChangeUserPasswordQuery = @"UPDATE DI_ERP_User SET Password = '{0}' WHERE UserName = '{1}'";
                SqlCommand cmd = null;
                cmd = new SqlCommand();
                if (sqlConnect == null) Connect();
                cmd.Connection = sqlConnect;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = string.Format(ChangeUserPasswordQuery, fields["Password"].Value, fields["UserName"].Value);
                int Result = cmd.ExecuteNonQuery();


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
