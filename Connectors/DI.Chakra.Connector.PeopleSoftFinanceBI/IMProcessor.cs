#region NameSpace
using System;
using System.Data;
using System.Linq;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.Text;
using Oracle.ManagedDataAccess.Client;
//using System.Data.OracleClient;
#endregion


namespace DI.Chakra.Connector.PeopleSoftFinanceBI
{
    public class IMProcessor : DI.Chakra.Connector.IMProcessor, IIMProcessor
    {

        public EndpointConfiguration _peopleSoftFinanceBIConfiguration { get; set; }
        private string EnvironmentFBI { get; set; }
        public object CommandType { get; private set; }
        public EndPoint EndPoint
        {
            get;
            set;
        }
        private string Configuration(EndpointKey Key)
        {
            var config = _peopleSoftFinanceBIConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }

        public IMProcessor()
        {
            this.EnvironmentFBI = ConfigurationMaster.GetByConfigKey("EnvironmentFBI", Product.IM).ConfigValue;
        }

        public bool Connect()
        {
            OracleConnection orCon = null;
            try
            {
                string psConStr = "";
                string portNo = "";
                string instanceName = "";
                //string IPAddress = "";
                string password = RSAHelper.DecryptData(Configuration(EndpointKey.Password));

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
                    instanceName = Configuration(EndpointKey.InstanceName);
                }
                else
                {
                    instanceName = "oracle";
                }

                psConStr = string.Format("Data Source=={2};" +
                    "User Id={3};Password={4};", Configuration(EndpointKey.IPAddress), portNo, instanceName, Configuration(EndpointKey.UserName), password);               
                orCon = new OracleConnection(psConStr);
                orCon.Open();

            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                orCon.Close();
                orCon.Dispose();
            }
            return true;
        }
        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = "" };
        }
        private string PutOracleQueryToDB(string strQry)
        {
            bool ReturnValue = false;

            string returMsg = string.Empty;

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


                using (OracleConnection connection = new OracleConnection(oracleConnectionString))
                {
                    connection.Open();

                    OracleCommand command = connection.CreateCommand();
                    OracleTransaction transaction;

                    transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    command.Transaction = transaction;

                    try
                    {
                        command.CommandText = strQry;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                        ReturnValue = true;
                    }
                    catch (Exception e)
                    {
                        //  transaction.Rollback();
                        ReturnValue = false;
                        returMsg = e.InnerException != null ? e.InnerException.ToString() + Convert.ToString(e.Message) : Convert.ToString(e.Message);
                    }
                }
            }
            catch (Exception ex)
            {                //if (ex.Message != null && ex.Message.Contains("unique constraint"))

                returMsg = ex.InnerException != null ? ex.InnerException.ToString() + Convert.ToString(ex.Message) : Convert.ToString(ex.Message);
                ReturnValue = false;
            }

            return returMsg;
        }
        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            string executionmsg = string.Empty;
            try
            {
                if (this.Connect())
                {
                    string adid = Convert.ToString(fields["OPRID"].Value);

                    if (!string.IsNullOrEmpty(adid))
                    {
                        StringBuilder deleteQuery = new StringBuilder();

                        deleteQuery.AppendLine(string.Format("DELETE FROM {0}_DW.GROUPMEMBERS WHERE G_MEMBER = '{1}'", EnvironmentFBI, adid));
                        executionmsg = executionmsg + PutOracleQueryToDB(deleteQuery.ToString()) + deleteQuery.ToString();
                        deleteQuery.Clear();

                        deleteQuery.AppendLine(string.Format("DELETE FROM {0}_DW.RWS_SECURITY_MEM_CC WHERE DOMAIN_ID = '{1}'", EnvironmentFBI, adid));
                        executionmsg = executionmsg + PutOracleQueryToDB(deleteQuery.ToString()) + deleteQuery.ToString();
                        deleteQuery.Clear();

                        //deleteQuery.AppendLine(string.Format("DELETE FROM {1} WHERE USER_ACCOUNT = '{0}'", adid, "SIT_DW.SEC_BU_OPR"));
                        //PutOracleQueryToDB(deleteQuery.ToString());
                    }
                    else
                        return new ProvisioningResult() { Result = false, Message = string.Format("ADID empty. Reference TX-Id:{0}", Convert.ToString(fields["IDASTnxID"].Value)) };
                }
                else
                {
                    return new ProvisioningResult() { Result = false, Message = string.Format("End Point data base not available. Reference TX-Id:{0}", Convert.ToString(fields["IDASTnxID"].Value)) };
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Message = string.Format("FBI Account deleted successfully for  the user {0}. Query Execution Details : {1}", Convert.ToString(fields["OPRID"].Value), executionmsg) };
        }
        public override ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {

            return new ProvisioningResult() { Result = false, Message = string.Empty };
        }
        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {
            string message = string.Empty;
            string executionmsg = string.Empty;
            try
            {
                if (this.Connect())
                {
                    StringBuilder InsertEntlQuery = new StringBuilder();
                    string adid = Convert.ToString(fields["OPRID"].Value);
                    string costCenters = Convert.ToString(fields["CostCenter"].Value);
                    string roles = Convert.ToString(fields["Role"].Value);

                    string[] roleList = !string.IsNullOrEmpty(roles) ? Convert.ToString(roles).Split(',') : null;
                    string[] costCentersList = !string.IsNullOrEmpty(costCenters) ? Convert.ToString(costCenters).Split(',') : null;

                    DomainModel.EndPoint endpoint = DomainModel.EndPoint.GetByEndpointSystemTypeIdIsActive((long)EndpointType.PSFBI, true);

                    //Role Entitlement
                    if (roleList != null && roleList.Count() > 0)
                    {
                        var existingRoles = EntitlementReconcilation.GetByUserADIDAndEndpointSystemId(adid, endpoint.Id, EntitlementType.Role);
                        message = "Roles: ";
                        foreach (var role in roleList)
                        {
                            if (!string.IsNullOrEmpty(role))
                            {
                                ///Sample request provided by RWS FBI
                                ///INSERT INTO <schema_env>_DW.GROUPMEMBERS VALUES ('<ADID>','<RoleID>');
                                ///COMMIT;
                                ///

                                if (!existingRoles.Any(e => e.Entitlement.ToLower() == role.ToLower()))
                                {
                                    message = !string.IsNullOrEmpty(message) ? message + "," : "";
                                    InsertEntlQuery.AppendLine(string.Format("INSERT INTO {0}_DW.GROUPMEMBERS VALUES('{1}', '{2}')", EnvironmentFBI, adid, role));
                                    executionmsg = executionmsg + PutOracleQueryToDB(InsertEntlQuery.ToString()) + InsertEntlQuery.ToString();
                                    InsertEntlQuery.Clear();
                                    message = message + role;
                                }
                            }
                        }
                    }

                    //CostCenter Entitlement
                    if (costCentersList != null)
                    {
                        var existingCostCnters = EntitlementReconcilation.GetByUserADIDAndEndpointSystemId(adid, endpoint.Id, EntitlementType.CostCenter);
                        message = message + " CostCenters: ";
                        foreach (var cc in costCentersList)
                        {
                            DomainModel.PeopleSoftFinanceBICostCenterGroup ccObj = DomainModel.PeopleSoftFinanceBICostCenterGroup.GetByCCCode(cc);
                            if (ccObj != null)
                            {
                                /// Sample code provided by RWS FBI
                                ///INSERT INTO <schema_env>_DW.RWS_SECURITY_MEM_CC VALUES ('<ADID>','<CostCenterCode>', '<CostCenterTree>',(SELECT MAX(MEM_CC_ID) FROM <schema_env>_DW.RWS_SECURITY_MEM_CC) + 1);
                                ///COMMIT;
                                ///

                                if (!existingCostCnters.Any(a => a.Entitlement.ToLower() == ccObj.CostCenterCode.ToLower()))
                                {
                                    message = !string.IsNullOrEmpty(message) ? message + "," : "";
                                    InsertEntlQuery.AppendLine(string.Format("INSERT INTO {0}_DW.RWS_SECURITY_MEM_CC VALUES('{1}', '{2}', '{3}', (SELECT MAX(MEM_CC_ID) FROM {0}_DW.RWS_SECURITY_MEM_CC) + 1)", EnvironmentFBI, adid, ccObj.CostCenterCode, ccObj.CosCenterTree));
                                    executionmsg = executionmsg + PutOracleQueryToDB(InsertEntlQuery.ToString()) + InsertEntlQuery.ToString();
                                    InsertEntlQuery.Clear();
                                    message = message + cc;
                                }
                            }
                        }
                    }

                }
                else
                {
                    return new ProvisioningResult() { Result = false, Result_TX_ID = Convert.ToString(fields["IDASTnxID"].Value), Message = string.Format("End Point data base not available. Reference TX-Id:{0}", Convert.ToString(fields["IDASTnxID"].Value)) };
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Result_TX_ID = Convert.ToString(fields["IDASTnxID"].Value), Message = string.Format("FBI - Entitlements has been added sucessfully. {0}, Query Execution Details : {1}", message, executionmsg) };
        }
        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
            string message = string.Empty;
            string executionmsg = string.Empty;
            try
            {
                if (this.Connect())
                {
                    StringBuilder removeEntlQuery = new StringBuilder();
                    string adid = Convert.ToString(fields["OPRID"].Value);
                    string costCenters = Convert.ToString(fields["CostCenter"].Value);
                    string roles = Convert.ToString(fields["Role"].Value);

                    string[] roleList = !string.IsNullOrEmpty(roles) ? Convert.ToString(roles).Split(',') : null;
                    string[] costCentersList = !string.IsNullOrEmpty(costCenters) ? Convert.ToString(costCenters).Split(',') : null;

                    //Role Entitlement
                    if (roleList != null && roleList.Count() > 0)
                    {
                        message = "Roles: ";
                        foreach (var role in roleList)
                        {
                            if (!string.IsNullOrEmpty(role))
                            {
                                /// Sample request provided by RWS FBI
                                /// DELETE FROM <schema_env>_DW.GROUPMEMBERS WHERE DOMAIN_ID = '<ADID>' AND G_NAME = '<RoleID>';
                                /// COMMIT;
                                /// 
                                message = !string.IsNullOrEmpty(message) ? message + "," : "";
                                removeEntlQuery.AppendLine(string.Format("DELETE FROM {0}_DW.GROUPMEMBERS WHERE G_MEMBER = '{1}' AND G_NAME = '{2}'", EnvironmentFBI, adid, role));
                                executionmsg = executionmsg + PutOracleQueryToDB(removeEntlQuery.ToString()) + removeEntlQuery.ToString();
                                removeEntlQuery.Clear();
                                message = message + role;
                            }
                        }
                    }

                    //CostCenter Entitlement
                    if (costCentersList != null)
                    {
                        message = message + " CostCenters: ";
                        foreach (var cc in costCentersList)
                        {
                            DomainModel.PeopleSoftFinanceBICostCenterGroup ccObj = DomainModel.PeopleSoftFinanceBICostCenterGroup.GetByCCCode(cc);
                            if (ccObj != null)
                            {
                                ///Sample request provided by RWS FBI
                                /// DELETE FROM <schema_env>_DW.RWS_SECURITY_MEM_CC WHERE DOMAIN_ID = '<ADID>' AND COST_CENTER_GROUP = '<CostCenterCode>' AND HIERARCHY_ID = '<CostCenterTree>';
                                /// COMMIT;
                                /// 
                                message = !string.IsNullOrEmpty(message) ? message + "," : "";
                                removeEntlQuery.AppendLine(string.Format("DELETE FROM {0}_DW.RWS_SECURITY_MEM_CC WHERE DOMAIN_ID = '{1}' AND COST_CENTER_GROUP = '{2}' AND HIERARCHY_ID = '{3}'", EnvironmentFBI, adid, ccObj.CostCenterCode, ccObj.CosCenterTree));
                                executionmsg = executionmsg + PutOracleQueryToDB(removeEntlQuery.ToString()) + removeEntlQuery.ToString();
                                removeEntlQuery.Clear();
                                message = message + cc;
                            }
                        }
                    }

                }
                else
                {
                    return new ProvisioningResult() { Result = false, Message = string.Format("End Point data base not available. Reference TX-Id:{0}", Convert.ToString(fields["IDASTnxID"].Value)) };
                }
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Result_TX_ID = Convert.ToString(fields["IDASTnxID"].Value), Message = string.Format("FBI - Entitlements has been removed sucessfully. {0}, Query Execution Details : {1}", message, executionmsg) };
        }
        public override ProvisioningResult ResetPassword(ProvisioningParamCollection fields)
        {

            return new ProvisioningResult() { Result = false, Message = string.Empty };
        }

        public override ProvisioningResult UnLockAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult EnableAccount(ProvisioningParamCollection fields)
        {
            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
        public override ProvisioningResult DisableAccount(ProvisioningParamCollection fields)
        {

            return new ProvisioningResult() { Result = true, Message = string.Empty };
        }
    }
}




