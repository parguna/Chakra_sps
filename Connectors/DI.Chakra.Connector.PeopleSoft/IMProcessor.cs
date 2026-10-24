#region NameSpace
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
using CheckNulls = DI.Chakra.DomainModel.CheckNull;
using System.Data.OracleClient;
using DI.Chakra.Logger;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Connector;
using DeepIdentity.Connector.PeopleSoft;

#endregion


namespace DI.Chakra.Connector.PeopleSoft
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {
        public EndpointConfiguration peoplesoftConfiguration { get; set; }
        public object CommandType { get; private set; }
        public EndPoint EndPoint
        {
            get;
            set;
        }

        private string Configuration(EndpointKey Key)
        {
            var config = peoplesoftConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }

        public bool Connect()
        {
            OracleConnection OrCon = null;
            try
            {

                string PSConStr = "";
                string portNo = "";
                string InstanceName = "";
                //string IPAddress = "";
                string Password = RSAHelper.DecryptData(Configuration(EndpointKey.Password));

                if (Convert.ToInt32(Configuration(EndpointKey.PortNo)) != 0)
                {
                    portNo = Configuration(EndpointKey.PortNo);
                }
                else
                {
                    portNo = "1521";
                }

                if (Configuration(EndpointKey.InstanceName) != null && Configuration(EndpointKey.InstanceName) != "")
                {
                    InstanceName = Configuration(EndpointKey.InstanceName);
                }
                else
                {
                    InstanceName = "oracle";
                }

                PSConStr = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", Configuration(EndpointKey.IPAddress), portNo, InstanceName, Configuration(EndpointKey.UserName), Password);

                //PSConStr = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.16.211)(PORT=1521))(CONNECT_DATA=(SID=FSCMDM92))); User Id=sysadm;Password=sysadm");
                OrCon = new OracleConnection(PSConStr);
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


        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {

            try
            {
                this.Connect();
                bool ReturnValue = true;

                //string CreatingQuery = string.Format(@"insert into PSOPRDEFN (OPRID, VERSION, OPRDEFNDESC, EMPLID, EMAILID, OPRCLASS, ROWSECCLASS,
                //    OPERPSWD, OPERPSWDSALT, ENCRYPTED , SYMBOLICID, LANGUAGE_CD, MULTILANG , CURRENCY_CD , LASTPSWDCHANGE, ACCTLOCK , 
                //    PRCSPRFLCLS, DEFAULTNAVHP, FAILEDLOGINS, EXPENT, OPRTYPE, USERIDALIAS, LASTUPDOPRID, PTALLOWSWITCHUSER)
                //        values ('{0}', 8 , '{2}', '{3}', '{4}', 'HCPPALL','HCDPALL','{5}','{7}',1 ,'SYSADM1' ,'ENG'  ,
                //    0 ,'USD' ,'{6}',1 ,'EPEX9000','HCSPNAVHP',0,0,0,'{1}','PPLSOFT',0)",
                //fields["OPRID"].Value.Replace("'", "''"), fields["USERIDALIAS"].Value.Replace("'", "''"), fields["OPRDEFNDESC"].Value.Replace("'", "''"), fields["EMPLID"].Value,
                //fields["EMAILID"].Value, "{1}qWsATtDNqduEyuzHq59XKTU/Hos=", DateTime.Now.ToString("dd-MMM-yy"), "3qdf+Th7w9a8e4fnTBZDpw=="); //PSUserInfo.AccountPassword

                //ReturnValue = (ReturnValue) ? putOracleQueryToDB(CreatingQuery) : ReturnValue;
                string CreateUserQuery = Query.CreateUserQuery;
                CreateUserQuery = CreateUserQuery.Replace("?OPRID", fields["OPRID"].Value.Replace("'", "''"));
                CreateUserQuery = CreateUserQuery.Replace("?USERIDALIAS", fields["USERIDALIAS"].Value.Replace("'", "''"));
                CreateUserQuery = CreateUserQuery.Replace("?OPRDEFNDESC", fields["OPRDEFNDESC"].Value.Replace("'", "''"));
                CreateUserQuery = CreateUserQuery.Replace("?EMPLID", fields["EMPLID"].Value);
                CreateUserQuery = CreateUserQuery.Replace("?EMAILID", fields["EMAILID"].Value == "" ? "change_me@oracle.com" : fields["EMAILID"].Value);
                CreateUserQuery = CreateUserQuery.Replace("?OPERPSWD", fields["Password"].Value);
                //CreateUserQuery = CreateUserQuery.Replace("DATETIME", "dd-MMM-yy");
                //CreateUserQuery = CreateUserQuery.Replace("?ACCOUNTPASSWORD", "3qdf+Th7w9a8e4fnTBZDpw==");
                CreateUserQuery = CreateUserQuery.Replace("?OPERPSWDSALT", "''");
                ReturnValue = (ReturnValue) ? putOracleQueryToDB(CreateUserQuery) : ReturnValue;

            }

            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }


            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }

        private bool putOracleQueryToDB(string strQry)
        {
            bool ReturnValue = false;
            OracleConnection OrCon = null;
            OracleCommand OrCom = null;
            try
            {

                string oracleConnectionString = "";
                string portNo = "";
                string InstanceName = "";
                string Password = RSAHelper.DecryptData(Configuration(EndpointKey.Password));
                if (Convert.ToInt32(Configuration(EndpointKey.PortNo)) != 0)
                {
                    portNo = Configuration(EndpointKey.PortNo);
                }
                else
                {
                    portNo = "1521";
                }
                if (Configuration(EndpointKey.InstanceName) != null && Configuration(EndpointKey.InstanceName) != "")
                {
                    InstanceName = Configuration(EndpointKey.InstanceName);
                }
                else
                {
                    InstanceName = "Oracle";
                }

                oracleConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", Configuration(EndpointKey.IPAddress), portNo, InstanceName, Configuration(EndpointKey.UserName), Password);


                OrCon = new OracleConnection(oracleConnectionString);

                OrCon.Open();
                OrCom = new OracleCommand();
                OrCom.Connection = OrCon;
                // OrCom.CommandType = CommandType.Text;
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
                if (ex.Message != null && ex.Message.Contains("unique constraint"))
                    ReturnValue = true;
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
                this.Connect();
                string CreateLoginQuery = String.Format("update PSOPRDEFN set acctlock=0 where  OPRID= {0}", fields["OPRID"].Value);
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

                string CreateLoginQuery = String.Format("update PSOPRDEFN set acctlock=1 where  OPRID= {0}", fields["OPRID"].Value);
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
                foreach (var permission in fields["Roles"].Value.Split(','))
                {
                    CreatingQuery = string.Format(@"Insert into PSRoleUser VALUES('{0}'  , '{1}' ,'N')", fields["AccountName"].Value, permission);
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

                    foreach (var permission in fields["Roles"].Value.Split(','))
                    {
                        CreatingQuery = string.Format(@"delete PSRoleUser where  ROLEUSER='{0}' and ROLENAME='{1}'", fields["AccountName"].Value, permission);
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
                //this.Connect();


                //string CreateLoginQuery = String.Format(@"UPDATE PSOPRDEFN  SET OPERPSWD ={1}, OPERPSWDSALT ={2}, ENCRYPTED = 1 Where OPRID = {0}",
                //                          fields["UserName"].Value, "{1}A4Ngxo5bzp8fp9+fxtJHogZQaXU=", "EOZJU9bhSfBUNDJK241K3w==");

                //putOracleQueryToDB(CreateLoginQuery);



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

                string CreateLoginQuery = String.Format("update PSOPRDEFN set acctlock=0 where  OPRID= {1}", fields["OPRID"].Value);
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

                string CreateLoginQuery = String.Format("update PSOPRDEFN set acctlock=0 where  OPRID= {0}", fields["OPRID"].Value);
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
                string CreateLoginQuery = String.Format("update PSOPRDEFN set acctlock=1 where  OPRID= '{0}'", fields["OPRID"].Value);
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




