using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Logger;
using System.Data;
using DI.Chakra.Infrastructure;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure.Enums;
using System.Globalization;
using DI.Chakra.Connector;
using CMessage = DI.Chakra.Connector.Inhouse.Message;
using Microsoft.VisualBasic.FileIO;

namespace DI.Chakra.Connector
{
    public class InhouseConnector : ConnectorBase
    {
        private DomainModel.EndPoint _endpoint;
        private EndpointConfiguration _inhouseConfiguration = null;
        public InhouseConnector(EndPoint endpoint, AccessRequest currentRequest)
        {
            _endpoint = endpoint;
            this.EndPoint = endpoint;
            this.CurrentRequest = currentRequest;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);

            var inhouseConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            inhouseConfiguration.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString())
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.InhouseConfiguration = inhouseConfiguration;

            this.NetworkUserName = GetConfiguration(EndpointKey.UserName);
            this.NetworkPassword = GetConfiguration(EndpointKey.Password);
        }
        public EndpointConfiguration InhouseConfiguration
        {
            get { return _inhouseConfiguration; }
            set { this._inhouseConfiguration = value; }
        }
        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }

        private string GetConfiguration(EndpointKey Key)
        {
            var config = InhouseConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }

        public override bool Connect()
        {
            return true;
        }

        public override void Disconnect()
        {
           
        }

        public override void GetData()
        {
            try
            {
                bool canRunDiscovery = false;
                var canRunDiscoveryConfig = EndpointConfigurationDetail.GetAll(true).FirstOrDefault(x => x.KeyId == (long)EndpointKey.CanRunDiscovery).Value;
                if (!string.IsNullOrEmpty(canRunDiscoveryConfig))
                {
                    canRunDiscovery = Convert.ToBoolean(canRunDiscoveryConfig);
                }
                if (canRunDiscovery)
                {
                    long discoveryProcessId = base.DiscoveryProcess.Id;
                    List<EndpointObjects> Objects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(discoveryProcessId);
                    bool IsUsersSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_Inhouse_User) != null;
                    bool IsRolesSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_Inhouse_PERMISSION) != null;
                    bool IsFunctionSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_Inhouse_FUNCTION) != null;
                    bool IsPrivillegeAcctTransSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_Inhouse_PRIVILLEGEACCTTRANS) != null;


                    if (!MarkDiscoveryFail && IsRolesSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.Inhouse_110, GetConfiguration(EndpointKey.IPAddress)));
                        DiscoverRoles();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.Inhouse_111, GetConfiguration(EndpointKey.IPAddress)));
                    }
                    if (!MarkDiscoveryFail && IsFunctionSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.Inhouse_107, GetConfiguration(EndpointKey.IPAddress)));
                        DiscoverFunctions();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.Inhouse_108, GetConfiguration(EndpointKey.IPAddress)));
                    }
                    if (!MarkDiscoveryFail && IsUsersSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.Inhouse_105, GetConfiguration(EndpointKey.IPAddress)));
                        DiscoverUser();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.Inhouse_106, GetConfiguration(EndpointKey.IPAddress)));
                    }
                    if (!MarkDiscoveryFail && IsPrivillegeAcctTransSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.Inhouse_113, GetConfiguration(EndpointKey.IPAddress)));
                        DiscoverPrivillegeAcctTrans();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.Inhouse_114, GetConfiguration(EndpointKey.IPAddress)));
                    }
                    if (!MarkDiscoveryFail)
                    {

                    }

                    if (this.IsExceptionRaised || MarkDiscoveryFail)
                    {
                        throw new Exception(string.Format("Inhouse discovery failed {0}", GetConfiguration(EndpointKey.IPAddress)));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
  
        #region Discover Objects
        private void DiscoverUser()
        {
            try
            {
                this.GetUsers();
            }
            catch (Exception ex)
            {

            }
        }
        private void DiscoverFunctions()
        {
            try
            {
                this.GetFunctions();

            }
            catch (Exception ex)
            {

            }
        }
        private void DiscoverRoles()
        {
            try
            {
                this.GetRoles();
            }
            catch (Exception ex)
            {

            }
        }
        private void DiscoverPrivillegeAcctTrans()
        {
            try
            {
                this.GetPrivillegeAcctTrans();
            }
            catch (Exception ex)
            {

            }
        }

        private void GetUsers()
        {

            List<long> selectedUsersList = new List<long>();
            StringBuilder queryString = new StringBuilder();
            try
            {
                string mandatoryCols = CMessage.Inhouse_USERS_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.UserFilePath);
                var length = mandatoryCols.Split(',').Length;
                var dtXMLData = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0, true, false);

                if (dtXMLData != null && dtXMLData.Rows.Count > 0)
                {
                    dtXMLData.TableName = "InhouseUser";
                }
                SetValueForNull(dtXMLData);

                string result;
                using (StringWriter sw = new StringWriter())
                {
                    dtXMLData.WriteXml(sw);
                    result = sw.ToString();
                }

                object objResult = (object)result;
                

                XLTemplates getTemplate = XLTemplates.INHOUSE_USERINFO;
                var errMsg = XLData.ValidateFileColumns(objResult, filePath, getTemplate);

                if (errMsg == "")
                {
                    string errorMessage = "";
                    var xlData = XLData.ProcessXL(objResult, filePath, getTemplate, ref errorMessage, "System");

                    if (xlData != null && xlData.Count > 0)
                    {
                        List<IXLData> invalidList = new List<IXLData>();
                        bool hasValidEntry = false;

                        foreach (IXLData data in xlData)
                        {
                            string hasvalue = data.ValidationMessage;

                            if (string.IsNullOrEmpty(hasvalue))
                            {
                                hasValidEntry = true;
                            }
                            else
                            {
                                invalidList.Add(data);

                                string remarks = string.Format(" Inhouse user file rejected due to invalid data - {0}", data.ValidationMessage);
                                this.AuthenticateNetworkAndWriteDatatoFile(filePath, remarks);

                                WriteDiscoveryLog(LogLevelType.Warning, CMessage.Inhouse_120 + data.ValidationMessage);
                            }
                        }
                        if (invalidList.Count > 0)
                        {
                            string remarks = string.Format("Inhouse discovery issue for system - {0} while reading inhouse user file, please check discovery log for more details.", _endpoint.DisplayName);
                            if (hasValidEntry)
                            {
                                remarks = string.Format("Inhouse discovery issue for system - {0} inhouse user file has some invalide entry, please check discovery log for more details.", _endpoint.DisplayName);
                            }
                            DILog.AlertNotificationLogger.WriteLog((long)_endpoint.Id, null, remarks, AlertCenterNotificationType.Discovery);
                        }
                    }
                }

                base.AuthenticateNetworkAndDeleteAllFiles(filePath);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.Inhouse_120 + ex.Message, null, true);
            }
        }
        
        private void GetFunctions()
        {

            List<long> selectedUsersList = new List<long>();
            StringBuilder queryString = new StringBuilder();
            try
            {
                string mandatoryCols = CMessage.Inhouse_FUN_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.FunctionFilePath);
                var length = mandatoryCols.Split(',').Length;
                var dtXMLData = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0, true, false);

                if (dtXMLData != null && dtXMLData.Rows.Count > 0)
                {
                    dtXMLData.TableName = "InhouseFunction";
                }
                SetValueForNull(dtXMLData);

                string result;
                using (StringWriter sw = new StringWriter())
                {
                    dtXMLData.WriteXml(sw);
                    result = sw.ToString();
                }

                object objResult = (object)result;


                XLTemplates getTemplate = XLTemplates.INHOUSE_FUNCTIONS;
                var errMsg = XLData.ValidateFileColumns(objResult, filePath, getTemplate);

                if (errMsg == "")
                {
                    string errorMessage = "";
                    var xlData = XLData.ProcessXL(objResult, filePath, getTemplate, ref errorMessage, "System");

                    if (xlData != null && xlData.Count > 0)
                    {
                        List<IXLData> invalidList = new List<IXLData>();
                       bool hasValidEntry = false;

                        foreach (IXLData data in xlData)
                        {
                            string hasvalue = data.ValidationMessage;

                            if (string.IsNullOrEmpty(hasvalue))
                            {
                                hasValidEntry = true;
                            }
                            else
                            {
                                invalidList.Add(data);
                                string remarks = string.Format(" Inhouse Function file rejected due to invalid data - {0}", data.ValidationMessage);
                                this.AuthenticateNetworkAndWriteDatatoFile(filePath, remarks);

                                WriteDiscoveryLog(LogLevelType.Warning, CMessage.Inhouse_124 + data.ValidationMessage);
                            }
                        }
                        if (invalidList.Count > 0)
                        {
                            string remarks = string.Format("Inhouse discovery issue for system - {0} while reading inhouse function file, please check discovery log for more details.", _endpoint.DisplayName);
                            if (hasValidEntry)
                            {
                                remarks = string.Format("Inhouse discovery issue for system - {0} inhouse function file has some invalide entry, please check discovery log for more details.", _endpoint.DisplayName);
                            }
                            DILog.AlertNotificationLogger.WriteLog((long)_endpoint.Id, null, remarks, AlertCenterNotificationType.Discovery);
                        }
                    }
                }
                base.AuthenticateNetworkAndDeleteAllFiles(filePath);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.Inhouse_124 + ex.Message, null, true);
            }
        }
        private void GetRoles()
        {

            List<long> selectedUsersList = new List<long>();
            StringBuilder queryString = new StringBuilder();
            try
            {
                string mandatoryCols = CMessage.Inhouse_ROLES_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.RoleFilePath);
                var length = mandatoryCols.Split(',').Length;
                var dtXMLData = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0, true, false);

                if (dtXMLData != null && dtXMLData.Rows.Count > 0)
                {
                    dtXMLData.TableName = "InhouseRole";
                }
                SetValueForNull(dtXMLData);

                string result;
                using (StringWriter sw = new StringWriter())
                {
                    dtXMLData.WriteXml(sw);
                    result = sw.ToString();
                }

                object objResult = (object)result;


                XLTemplates getTemplate = XLTemplates.INHOUSE_PERMISSIONS;
                var errMsg = XLData.ValidateFileColumns(objResult, filePath, getTemplate);

                if (errMsg == "")
                {
                    string errorMessage = "";
                    var xlData = XLData.ProcessXL(objResult, filePath, getTemplate, ref errorMessage, "System");

                    if (xlData != null && xlData.Count > 0)
                    {
                        List<IXLData> invalidList = new List<IXLData>();
                        bool hasValidEntry = false;

                        foreach (IXLData data in xlData)
                        {
                            string hasvalue = data.ValidationMessage;

                            if (string.IsNullOrEmpty(hasvalue))
                            {
                                hasValidEntry = true;
                            }
                            else
                            {
                                invalidList.Add(data);
                                string remarks = string.Format(" Inhouse Role file rejected due to invalid data - {0}", data.ValidationMessage);
                                this.AuthenticateNetworkAndWriteDatatoFile(filePath, remarks);

                                WriteDiscoveryLog(LogLevelType.Warning, CMessage.Inhouse_130 + data.ValidationMessage);
                            }
                        }
                        if (invalidList.Count > 0)
                        {
                            string remarks = string.Format("Inhouse discovery issue for system - {0} while reading inhouse role file, please check discovery log for more details.", _endpoint.DisplayName);
                            if (hasValidEntry)
                            {
                                remarks = string.Format("Inhouse discovery issue for system - {0} inhouse role file has some invalide entry, please check discovery log for more details.", _endpoint.DisplayName);
                            }
                            DILog.AlertNotificationLogger.WriteLog((long)_endpoint.Id, null, remarks, AlertCenterNotificationType.Discovery);
                        }
                    }
                }

                base.AuthenticateNetworkAndDeleteAllFiles(filePath);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.Inhouse_130 + ex.Message, null, true);
            }
        }
        private void GetPrivillegeAcctTrans()
        {
            List<long> selectedUsersList = new List<long>();
            StringBuilder queryString = new StringBuilder();
            try
            {
                string mandatoryCols = CMessage.Inhouse_USERS_MANDATORY_COLS;
                string filePath = GetConfiguration(EndpointKey.PrivillegeAcctTransFilePath);
                var length = mandatoryCols.Split(',').Length;
                var dtXMLData = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0, true, false);

                if (dtXMLData != null && dtXMLData.Rows.Count > 0)
                {
                    dtXMLData.TableName = "InhousePrivAcc";
                }
                SetValueForNull(dtXMLData);

                string result;
                using (StringWriter sw = new StringWriter())
                {
                    dtXMLData.WriteXml(sw);
                    result = sw.ToString();
                }

                object objResult = (object)result;


                XLTemplates getTemplate = XLTemplates.INHOUSE_PRIVILEGES;
                var errMsg = XLData.ValidateFileColumns(objResult, filePath, getTemplate);

                if (errMsg == "")
                {
                    string errorMessage = "";
                    var xlData = XLData.ProcessXL(objResult, filePath, getTemplate, ref errorMessage, "System");

                    if (xlData != null && xlData.Count > 0)
                    {
                        List<IXLData> invalidList = new List<IXLData>();
                        bool hasValidEntry = false;

                        foreach (IXLData data in xlData)
                        {
                            string hasvalue = data.ValidationMessage;

                            if (string.IsNullOrEmpty(hasvalue))
                            {
                                hasValidEntry = true;
                            }
                            else
                            {
                                invalidList.Add(data);
                                string remarks = string.Format(" Inhouse Privillege Acctount file rejected due to invalid data - {0}", data.ValidationMessage);
                                this.AuthenticateNetworkAndWriteDatatoFile(filePath, remarks);
                                WriteDiscoveryLog(LogLevelType.Warning, CMessage.Inhouse_137 + data.ValidationMessage);
                            }
                        }
                        if (invalidList.Count > 0)
                        {
                            string remarks = string.Format("Inhouse discovery issue for system - {0} while reading inhouse privillege file, please check discovery log for more details.", _endpoint.DisplayName);
                            if (hasValidEntry)
                            {
                                remarks = string.Format("Inhouse discovery issue for system - {0} inhouse privillege file has some invalide entry, please check discovery log for more details.", _endpoint.DisplayName);
                            }
                            DILog.AlertNotificationLogger.WriteLog((long)_endpoint.Id, null, remarks, AlertCenterNotificationType.Discovery);
                        }
                    }
                }
                base.AuthenticateNetworkAndDeleteAllFiles(filePath);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, CMessage.Inhouse_137 + ex.Message, null, true);
            }
        }
        private string GetColumnValue(DataRow currentRow, string columnName)
        {
            try
            {
                if (currentRow.Table.Columns.Contains(columnName))
                {
                    var value = currentRow[columnName].ToString().Trim();

                    if (value.ToUpper() == CMessage.Inhouse_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.Inhouse_139, columnName), ex);
                return string.Empty;
            }
        }
        private string GetColumnValueByIndex(DataRow currentRow, int index)
        {
            try
            {
                if (currentRow.Table.Columns[index] != null)
                {
                    var value = currentRow[index].ToString().Trim();

                    if (value.ToUpper() == CMessage.Inhouse_138) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CMessage.Inhouse_139, index), ex);
                return string.Empty;
            }
        }

        public void SetValueForNull(DataTable dt)
        {
            int firstIndex;
            int secondIndex;
            for (firstIndex = 0; firstIndex < dt.Columns.Count; firstIndex++)
            {
                for (secondIndex = 0; secondIndex < dt.Rows.Count; secondIndex++)
                {
                    if (dt.Columns[firstIndex].DataType.ToString() == "System.Int32" || dt.Columns[firstIndex].DataType.ToString() == "System.Single" || dt.Columns[firstIndex].DataType.ToString() == "System.Double" || dt.Columns[firstIndex].DataType.ToString() == "System.Decimal")
                    {
                        if (dt.Rows[secondIndex][firstIndex] == DBNull.Value)
                            dt.Rows[secondIndex][firstIndex] = 0;
                    }
                    else if (dt.Columns[firstIndex].DataType.ToString() == "System.String")
                    {
                        if (dt.Rows[secondIndex][firstIndex] == DBNull.Value || dt.Rows[secondIndex][firstIndex].ToString().Trim() == "")
                            dt.Rows[secondIndex][firstIndex] = "";
                    }
                }
            }
        }
        #endregion


    }
}
