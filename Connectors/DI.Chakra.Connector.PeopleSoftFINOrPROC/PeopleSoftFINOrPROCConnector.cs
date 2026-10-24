#region NameSpace
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DBConfigHelper;
using CMessage = DI.Chakra.Connector.PeopleSoftFINOrPROC.Message;
using CQuery = DI.Chakra.Connector.PeopleSoftFINOrPROC.Query;
using CEnum = DI.Chakra.Connector.PeopleSoftFINOrPROC.EnumPeopleSoftRoleDefinitions;
using DI.Chakra.DomainModel.Presentation;
using System.IO;
using System.Text;
using System.Globalization;
using Oracle.ManagedDataAccess.Client;
//using System.Data.OracleClient;
#endregion
namespace DI.Chakra.Connector
{
    public class PeopleSoftFINOrPROCConnector : ConnectorBase
    {
        #region Variable Declaration
        public OracleConnection _oracleConnection = null;
        public string _connectionString = string.Empty;
        public EndpointConfiguration _peoplesoftFINOrPROCConfiguration { get; set; }
        public EndPoint _endpoint;
        private Dictionary<long, string> _dicUsers = new Dictionary<long, string>();
        private bool doUser2RoleRecon = true;
        private bool doUser2BURecon = true;
        private bool doUser2BankRecon = true;

        private IList<PeopleSoftFINOrPROCUserInfo> discoveredUsers = new List<PeopleSoftFINOrPROCUserInfo>();
        #endregion
        #region Constructor
        public PeopleSoftFINOrPROCConnector(EndPoint endpoint, AccessRequest currentRequest = null)
        {
            try
            {
                this._endpoint = endpoint;

                if (endpoint.IsChildEpSplitForm.HasValue && endpoint.IsChildEpSplitForm.Value && endpoint.ParentEndpointId.HasValue && endpoint.ParentEndpointId.Value > 0)
                {
                    var parentEndpoint = EndPoint.GetById(endpoint.ParentEndpointId.Value);
                    if (parentEndpoint != null)
                        this._endpoint = parentEndpoint;
                }

                this.CurrentRequest = currentRequest;
                var _eoplesoftFINOrPROCConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);               
                _eoplesoftFINOrPROCConfiguration.ConfigurationDetails.ForEach(lq =>
                {
                    if (lq.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString())
                    {
                        lq.Value = RSAHelper.DecryptData(lq.Value);
                    }
                });
                this.PeopleSoftFINOrPROCConfiguration = _eoplesoftFINOrPROCConfiguration;
                this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id); ;

                this.NetworkUserName = GetConfiguration(EndpointKey.BizServiceUserName);
                this.NetworkPassword = GetConfiguration(EndpointKey.BizServicePassword);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_102, GetConfiguration(EndpointKey.IPAddress)), ex);
            }
        }
        public EndpointConfiguration PeopleSoftFINOrPROCConfiguration
        {
            get { return _peoplesoftFINOrPROCConfiguration; }
            set { this._peoplesoftFINOrPROCConfiguration = value; }
        }
        private string GetConfiguration(EndpointKey Key)
        {
            var config = _peoplesoftFINOrPROCConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }

        private string GetFileNameByDateTime(string sharedPath, string moduleName)
        {
            if (!string.IsNullOrEmpty(sharedPath) && !string.IsNullOrEmpty(moduleName))
            {
                string todaydateTime = DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                return string.Concat(sharedPath, "\\" + "FSCM_" + moduleName + "_" + todaydateTime + ".csv");
            }

            return string.Empty;
        }
        public override IMProcessor IMProcessor
        {
            get
            {
                PeopleSoftFINOrPROC.IMProcessor processor = new PeopleSoftFINOrPROC.IMProcessor();
                processor._endpoint = this.EndPoint;
                processor.PeopleSoftFINOrPROCConfiguration = this._peoplesoftFINOrPROCConfiguration;
                return processor;
            }
        }
        #endregion
        #region IConnector Members
        public override bool Connect()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_103, GetConfiguration(EndpointKey.IPAddress)));
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_103 + "{0}", RSAHelper.DecryptData(GetConfiguration(EndpointKey.Password)) + GetConfiguration(EndpointKey.UserName)));
                string portNo = GetConfiguration(EndpointKey.PortNo);
                string instanceName = GetConfiguration(EndpointKey.InstanceName);
                string Password = GetConfiguration(EndpointKey.Password);
                _connectionString = string.Format("Data Source={2};" +
                    "User Id={3};Password={4};", GetConfiguration(EndpointKey.IPAddress), portNo, instanceName, GetConfiguration(EndpointKey.UserName), Password);

                _oracleConnection = new OracleConnection(_connectionString);
                _oracleConnection.Open();
                WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_104, GetConfiguration(EndpointKey.IPAddress)));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_105, GetConfiguration(EndpointKey.IPAddress)), ex);
                return false;
            }
            finally
            {
                //_oracleConnection.Close();
                //_oracleConnection.Dispose();
            }
            return true;
        }
        public override void Disconnect()
        {
            if (_oracleConnection != null && _oracleConnection.State == ConnectionState.Open)
            {
                _oracleConnection.Close();
                _oracleConnection.Dispose();
            }
        }
        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }
        public override void GetData()
        {
            try
            {
                this.MarkDiscoveryFail = false;
                DateTime starttime = DateTime.Now;
                long discoveryProcessId = base.DiscoveryProcess.Id;
                List<EndpointObjects> Objects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(discoveryProcessId);

                bool isUsr2RolesSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PLFINPROC_User2Role) != null;
                bool isFinanceSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PLFINPROC_Finance) != null;
                bool isProcurementSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PLFINPROC_Procurement) != null;
                bool isPriviligedTransSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PLFINPROC__PriviligedTransaction) != null;
                bool isBankDescSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PLFINPROC__BankDesc) != null;
                bool isBusinessUnitSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PLFINPROC__BusinessUnitDesc) != null;
                bool isCC2OUSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PLFINPROC_CC2Ou) != null;
                bool isOU2taggingtSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PLFINPROC_OU2Tagging) != null;
                bool isRole2FunSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PLFINPROC_Role2Function) != null;
                bool isUser2BankSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PLFINPROC_User2Bank) != null;
                bool isUser2BUSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_PLFINPROC_User2BU) != null;

                if (!this.MarkDiscoveryFail && isUsr2RolesSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_109, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverRoles();
                    //DiscoverDefaultRoles();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_110, GetConfiguration(EndpointKey.IPAddress)));
                }

                if (!this.MarkDiscoveryFail && isBusinessUnitSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_118, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverBUDescription();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_119, GetConfiguration(EndpointKey.IPAddress)));
                }

                if (!this.MarkDiscoveryFail && isBankDescSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_121, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverBankDescription();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_122, GetConfiguration(EndpointKey.IPAddress)));
                }

                if (!this.MarkDiscoveryFail && isCC2OUSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_124, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverCCtoOU();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_125, GetConfiguration(EndpointKey.IPAddress)));
                }

                if (!this.MarkDiscoveryFail && isOU2taggingtSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_127, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverOUtoTagging();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_128, GetConfiguration(EndpointKey.IPAddress)));
                }

                if (!this.MarkDiscoveryFail && isRole2FunSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_137, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverRoleFunction();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_138, GetConfiguration(EndpointKey.IPAddress)));
                }

                if (!this.MarkDiscoveryFail && isUsr2RolesSelected && isUsr2RolesSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_106, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverUserRole();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_107, GetConfiguration(EndpointKey.IPAddress)));
                }

                if (!this.MarkDiscoveryFail && isUser2BankSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("FSCM-User2BankAccount Discovery Started {0}", GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverUser2BankAccount();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("FSCM-User2BankAccount Discovery Ended {0}", GetConfiguration(EndpointKey.IPAddress)));
                }

                if (!this.MarkDiscoveryFail && isUser2BUSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("FSCM-User2BusinessUnit Discovery Started {0}", GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverUser2BusinessUnit();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("FSCM - User2BusinessUnit Discovery Ended {0}", GetConfiguration(EndpointKey.IPAddress)));
                }

                if (!this.MarkDiscoveryFail && isUsr2RolesSelected && isFinanceSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_133, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverRoleRoleDefinitionFinance();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_134, GetConfiguration(EndpointKey.IPAddress)));
                }
                if (!this.MarkDiscoveryFail && isUsr2RolesSelected && isProcurementSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_135, GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverRoleRoleDefinitionProcurement();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftFINOrPROC_136, GetConfiguration(EndpointKey.IPAddress)));
                }

                if (!this.MarkDiscoveryFail && isPriviligedTransSelected)
                {
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("FSCM Priviliage Transaction discovery started {0}", GetConfiguration(EndpointKey.IPAddress)));
                    DiscoverPrivTrans();
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("FSCM Priviliage Transaction discovery ended {0}", GetConfiguration(EndpointKey.IPAddress)));
                }

                if (!this.MarkDiscoveryFail)
                {
                    if (doUser2RoleRecon && doUser2BankRecon && doUser2BURecon)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("FSCM Account recon started {0}", GetConfiguration(EndpointKey.IPAddress)));
                        FSCMAccountReconcilation();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("FSCM Account recon Ended {0}", GetConfiguration(EndpointKey.IPAddress)));

                        WriteDiscoveryLog(LogLevelType.Info, string.Format("FSCM Entitlement recon started {0}", GetConfiguration(EndpointKey.IPAddress)));
                        this.UpdateEntitlementReconcilation();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format("FSCM Entitlement recon Ended {0}", GetConfiguration(EndpointKey.IPAddress)));
                    }

                    base.DeleteLocalCopyFiles();
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("FSCM Mapping recon started {0}", GetConfiguration(EndpointKey.IPAddress)));
                PeopleSoftFINOrPROCUserInfo.MapWithMasterUserForReconTable(_endpoint, _peoplesoftFINOrPROCConfiguration.MapAll != null ? _peoplesoftFINOrPROCConfiguration.MapAll : false);
                PeopleSoftFINOrPROCUserInfo.MapWithMasterUserForEntitlementReconTable(_endpoint, _peoplesoftFINOrPROCConfiguration.MapAll != null ? _peoplesoftFINOrPROCConfiguration.MapAll : false, discoveryProcessId);

                WriteDiscoveryLog(LogLevelType.Info, string.Format("FSCM Mapping recon Ended {0}", GetConfiguration(EndpointKey.IPAddress)));

                DateTime endtime = DateTime.Now;

                if (this.IsExceptionRaised || this.MarkDiscoveryFail)
                {
                    throw new Exception(string.Format("FSCM -FinorProc Discovery failed {0}", GetConfiguration(EndpointKey.IPAddress)));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Discover Objects    
        private void DiscoverRoles()
        {
            try
            {
                IList<PeopleSoftFINOrPROCRoleInfo> discoveredRoles = new List<PeopleSoftFINOrPROCRoleInfo>();
                IList<PeopleSoftFINOrPROCRoleInfo> deleteRoles = new List<PeopleSoftFINOrPROCRoleInfo>();

                discoveredRoles = this.GetRoles();

                if (discoveredRoles != null && discoveredRoles.Count > 0)
                {
                    PeopleSoftFINOrPROCRoleInfo.SaveRoles(discoveredRoles, _endpoint.Id, MasterConfigHelper.ServiceUser);
                    deleteRoles = PeopleSoftFINOrPROCRoleInfo.GetDeletedByEndPointSystemId(this._endpoint.Id, this.Process);

                    var childSplitFormEps = EndPoint.GetAllActiveChildSplitForm(_endpoint.Id);
                    if (childSplitFormEps != null)
                    {
                        foreach (var childSplitFormEp in childSplitFormEps)
                        {
                            PeopleSoftFINOrPROCRoleInfo.SaveRoles(discoveredRoles, childSplitFormEp.Id, MasterConfigHelper.ServiceUser);

                            var tempRoles = PeopleSoftFINOrPROCRoleInfo.GetDeletedByEndPointSystemId(this._endpoint.Id, this.Process).ToList();

                            tempRoles.ForEach(t => deleteRoles.Add(t));
                        }
                    }
                }
                if (doUser2RoleRecon)
                {
                    PeopleSoftFINOrPROCRoleInfo.DeleteRoles(deleteRoles.ToList(), MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_111, GetConfiguration(EndpointKey.IPAddress)), ex, true);
            }
        }

        //private void DiscoverDefaultRoles()
        //{
        //    try
        //    {
        //        PeopleSoftFINOrPROCRoleInfo.SaveDefaultRoles(_endpoint.Id, MasterConfigHelper.ServiceUser);

        //        var childSplitFormEps = EndPoint.GetAllActiveChildSplitForm(_endpoint.Id);
        //        if (childSplitFormEps != null)
        //        {
        //            foreach (var childSplitFormEp in childSplitFormEps)
        //            {
        //                PeopleSoftFINOrPROCRoleInfo.SaveDefaultRoles(childSplitFormEp.Id, MasterConfigHelper.ServiceUser);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_111, GetConfiguration(EndpointKey.IPAddress)), ex, true);
        //    }
        //}

        private void DiscoverBUDescription()
        {
            try
            {
                IList<PeopleSoftFINOrPROCBUDescriptionInfo> discoveredBUDescription = new List<PeopleSoftFINOrPROCBUDescriptionInfo>();
                List<PeopleSoftFINOrPROCBUDescriptionInfo> deletebuscriptions = new List<PeopleSoftFINOrPROCBUDescriptionInfo>();
                discoveredBUDescription = this.GetBUDescription();
                if (discoveredBUDescription != null && discoveredBUDescription.Count > 0)
                {
                    PeopleSoftFINOrPROCBUDescriptionInfo.SaveBUDescriptions(discoveredBUDescription, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    deletebuscriptions = PeopleSoftFINOrPROCBUDescriptionInfo.GetDeletedByEndPointSystemId(this._endpoint.Id, this.Process).ToList();

                    var childSplitFormEps = EndPoint.GetAllActiveChildSplitForm(_endpoint.Id);
                    if (childSplitFormEps != null)
                    {
                        foreach (var childSplitFormEp in childSplitFormEps)
                        {
                            PeopleSoftFINOrPROCBUDescriptionInfo.SaveBUDescriptions(discoveredBUDescription, childSplitFormEp.Id, MasterConfigHelper.ServiceUser);

                            var tempList = PeopleSoftFINOrPROCBUDescriptionInfo.GetDeletedByEndPointSystemId(this._endpoint.Id, this.Process).ToList();

                            tempList.ForEach(t => deletebuscriptions.Add(t));
                        }
                    }
                }

                if (doUser2BURecon)
                {
                    PeopleSoftFINOrPROCBUDescriptionInfo.DeleteBUSByEndPointSystemId(deletebuscriptions.ToList(), _endpoint.Id, MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_120, GetConfiguration(EndpointKey.IPAddress)), ex, true);
            }
        }
        private void DiscoverBankDescription()
        {
            try
            {
                IList<PeopleSoftFINOrPROCBankDescriptionInfo> discoveredBankDescriptions = new List<PeopleSoftFINOrPROCBankDescriptionInfo>();
                IList<PeopleSoftFINOrPROCBankDescriptionInfo> deleteBankDescriptions = new List<PeopleSoftFINOrPROCBankDescriptionInfo>();
                discoveredBankDescriptions = this.GetBankDescription();
                if (discoveredBankDescriptions != null && discoveredBankDescriptions.Count > 0)
                {
                    PeopleSoftFINOrPROCBankDescriptionInfo.SaveBankDescriptions(discoveredBankDescriptions, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    deleteBankDescriptions = PeopleSoftFINOrPROCBankDescriptionInfo.GetDeletedByEndPointSystemId(this._endpoint.Id, this.Process);

                    var childSplitFormEps = EndPoint.GetAllActiveChildSplitForm(_endpoint.Id);
                    if (childSplitFormEps != null)
                    {
                        foreach (var childSplitFormEp in childSplitFormEps)
                        {
                            PeopleSoftFINOrPROCBankDescriptionInfo.SaveBankDescriptions(discoveredBankDescriptions, childSplitFormEp.Id, MasterConfigHelper.ServiceUser);

                            var tempList = PeopleSoftFINOrPROCBankDescriptionInfo.GetDeletedByEndPointSystemId(this._endpoint.Id, this.Process).ToList();

                            tempList.ForEach(t => deleteBankDescriptions.Add(t));
                        }
                    }
                }

                if (doUser2BankRecon)
                {
                    PeopleSoftFINOrPROCBankDescriptionInfo.DeleteBankAccountsByEndPointSystemId(deleteBankDescriptions.ToList(), _endpoint.Id, MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_123, GetConfiguration(EndpointKey.IPAddress)), ex, true);

            }
        }
        private void DiscoverCCtoOU()
        {
            try
            {
                IList<PeopleSoftFINOrPROCCCtoOUInfo> cc2OUs = new List<PeopleSoftFINOrPROCCCtoOUInfo>();
                cc2OUs = this.GetCCtoOU();
                if (cc2OUs != null && cc2OUs.Count > 0)
                {
                    PeopleSoftFINOrPROCCCtoOUInfo.SaveCCtoOU(cc2OUs, _endpoint.Id, MasterConfigHelper.ServiceUser);
                }
                cc2OUs = null;
                cc2OUs = PeopleSoftFINOrPROCCCtoOUInfo.GetDeletedByEndPointSystemId(this._endpoint.Id);
                PeopleSoftFINOrPROCCCtoOUInfo.DeleteList(cc2OUs.ToList());
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_126, GetConfiguration(EndpointKey.IPAddress)), ex, true);

            }
        }
        private void DiscoverOUtoTagging()
        {
            try
            {
                IList<PeopleSoftFINOrPROCOUtoTaggingInfo> discoveredOUtoTaggings = new List<PeopleSoftFINOrPROCOUtoTaggingInfo>();
                discoveredOUtoTaggings = this.GetOUtoTagging();
                if (discoveredOUtoTaggings != null && discoveredOUtoTaggings.Count > 0)
                {
                    PeopleSoftFINOrPROCOUtoTaggingInfo.SaveOUtoTagging(discoveredOUtoTaggings, _endpoint.Id, MasterConfigHelper.ServiceUser);
                }
                discoveredOUtoTaggings = null;
                discoveredOUtoTaggings = PeopleSoftFINOrPROCOUtoTaggingInfo.GetDeletedByEndPointSystemId(this._endpoint.Id);
                PeopleSoftFINOrPROCOUtoTaggingInfo.DeleteList(discoveredOUtoTaggings.ToList());
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_129, GetConfiguration(EndpointKey.IPAddress)), ex, true);
            }
        }
        private void DiscoverRoleFunction()
        {
            try
            {
                this.GetRole2Function();

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_111, GetConfiguration(EndpointKey.IPAddress)), ex, true);
                // throw;
            }
        }
        private void DiscoverUserRole()
        {
            try
            {
                var discoveredUserRoles = this.GetUserRoles();
                // SaveUsers();
                string sqlQuery = string.Empty;
                if (discoveredUserRoles != null && discoveredUserRoles.Count > 0)
                {
                    discoveredUserRoles = discoveredUserRoles.Distinct().ToList();
                    sqlQuery = string.Format(CQuery.DeleteUserRoleTable, _endpoint.Id);
                    PeopleSoftFINOrPROCUserInfo.ExecuteQuery(sqlQuery); sqlQuery = string.Empty;
                    List<string> insertQueries = discoveredUserRoles.Select(c => string.Format(CQuery.InsertUserRoleTable,
                        c.EmployeeAD_ID, c.RoleId, _endpoint.Id, Convert.ToInt16(VersionNo.Available))).ToList();
                    List<string> tempQueries = new List<string>();
                    foreach (var query in insertQueries)
                    {
                        tempQueries.Add(query);
                        if (tempQueries.Count() % 10 == 0)
                        {
                            PeopleSoftFINOrPROCUserInfo.ExecuteQuery(tempQueries.Distinct().ToList());
                            tempQueries.Clear();
                            tempQueries = new List<string>();
                        }
                    }
                    if (tempQueries.Count() > 1)
                    {
                        PeopleSoftFINOrPROCUserInfo.ExecuteQuery(tempQueries.Distinct().ToList());
                        tempQueries.Clear();
                    }

                    insertQueries.Clear();
                }

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_111, GetConfiguration(EndpointKey.IPAddress)), ex, true);
            }
        }

        private void DiscoverUser2BusinessUnit()
        {
            try
            {
                var discoveredUser2BU = this.GetUser2BU();
                List<PeopleSoftFINOrPROCUser2BusinessUnit> user2BusinessList = new List<PeopleSoftFINOrPROCUser2BusinessUnit>();

                if (discoveredUser2BU != null && discoveredUser2BU.Count() > 0)
                {

                    var userList = (from table in discoveredUser2BU.AsEnumerable()
                                    group table by new
                                    {
                                        ADID = table.ADID
                                    } into g
                                    select new PeopleSoftFINOrPROCUserInfo
                                    {
                                        EmployeeAD_ID = g.Key.ADID
                                    }).ToList();

                    //Save Users
                    PeopleSoftFINOrPROCUserInfo.SaveUsers(userList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    List<PeopleSoftFINOrPROCUserInfo> existingUserList = PeopleSoftFINOrPROCUserInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();

                    List<PeopleSoftFINOrPROCBUDescriptionInfo> existingBUList = PeopleSoftFINOrPROCBUDescriptionInfo.GetByEndpointSystemId(_endpoint.Id).ToList();
                    PeopleSoftFINOrPROCUserInfo userInfo = null;
                    PeopleSoftFINOrPROCBUDescriptionInfo buInfo = null;

                    string executeQuery = string.Empty;
                    executeQuery = string.Format(CMessage.DELETE_USER2BU_QUERY);
                    PeopleSoftFinanceBIUserInfo.ExecuteQuery(executeQuery);
                    executeQuery = null;

                    foreach (var item in discoveredUser2BU)
                    {
                        try
                        {
                            userInfo = existingUserList.Where(u => u.EmployeeAD_ID == item.ADID).FirstOrDefault();
                            buInfo = existingBUList.Where(b => b.BUID == item.BusinessUnit).FirstOrDefault();


                            if (buInfo == null)
                            {
                                buInfo = new PeopleSoftFINOrPROCBUDescriptionInfo();
                                buInfo.BUDescription = item.BusinessUnit;
                                buInfo.BUID = item.BusinessUnit;
                                buInfo.EndPointSystemId = this._endpoint.Id;
                                buInfo.CreatedBy = MasterConfigHelper.ServiceUser;
                                buInfo.CreatedDate = DateTime.Now;
                                buInfo.CanRequest = true;

                                buInfo.Save(MasterConfigHelper.ServiceUser);
                                existingBUList.Add(buInfo);
                            }
                            else
                            {
                                buInfo.CanRequest = true;
                                buInfo.LastModifiedBy = MasterConfigHelper.ServiceUser;
                                buInfo.LastModifiedDate = DateTime.Now;
                                buInfo.EndPointSystemId = this._endpoint.Id;
                                buInfo.Save(MasterConfigHelper.ServiceUser);
                            }


                            if (buInfo != null && userInfo != null)
                            {
                                executeQuery = string.Format(CMessage.INSERT_USER2BU_QUERY, buInfo.Id, userInfo.Id);
                                PeopleSoftFINOrPROCUserInfo.ExecuteQuery(executeQuery);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception @Record User2BusinessUnit Mapping - ADID:{0}, BusinessUnit:{1}", item.ADID, item.BusinessUnit), ex, true);
                        }
                    }
                }
                else
                {
                    this.DoRecon = false;
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_111, GetConfiguration(EndpointKey.IPAddress)), ex, true);
            }
        }

        private void DiscoverUser2BankAccount()
        {
            try
            {
                var discoveredUser2Bank = this.GetUser2Banks();

                if (discoveredUser2Bank != null && discoveredUser2Bank.Count() > 0)
                {

                    List<PeopleSoftFINOrPROCUserInfo> existingUserList = new List<PeopleSoftFINOrPROCUserInfo>();

                    var userList = (from table in discoveredUser2Bank.AsEnumerable()
                                    group table by new
                                    {
                                        ADID = table.ADID
                                    } into g
                                    select new PeopleSoftFINOrPROCUserInfo
                                    {
                                        EmployeeAD_ID = g.Key.ADID
                                    }).ToList();

                    //Save Users
                    PeopleSoftFINOrPROCUserInfo.SaveUsers(userList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    existingUserList = PeopleSoftFINOrPROCUserInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();

                    List<PeopleSoftFINOrPROCBankDescriptionInfo> existingBankList = PeopleSoftFINOrPROCBankDescriptionInfo.GetByEndpointSystem(_endpoint.Id).ToList();
                    PeopleSoftFINOrPROCUserInfo userInfo = null;
                    PeopleSoftFINOrPROCBankDescriptionInfo bankInfo = null;

                    string executeQuery = string.Empty;
                    executeQuery = string.Format(CMessage.DELETE_USER2BU_QUERY);
                    PeopleSoftFinanceBIUserInfo.ExecuteQuery(executeQuery);
                    executeQuery = null;

                    foreach (var item in discoveredUser2Bank)
                    {
                        try
                        {
                            userInfo = existingUserList.Where(u => u.EmployeeAD_ID == item.ADID).FirstOrDefault();
                            bankInfo = existingBankList.Where(b => b.BankAccount == item.BankAccountNumber).FirstOrDefault();

                            if (bankInfo == null)
                            {
                                bankInfo = new PeopleSoftFINOrPROCBankDescriptionInfo();
                                bankInfo.BankAccount = item.BankAccountNumber;
                                bankInfo.BankAccountDesc = item.BankAccountNumber;
                                bankInfo.EndPointSystemId = this._endpoint.Id;
                                bankInfo.CreatedBy = MasterConfigHelper.ServiceUser;
                                bankInfo.CreatedDate = DateTime.Now;
                                bankInfo.CanRequest = true;

                                bankInfo.Save(MasterConfigHelper.ServiceUser);
                                existingBankList.Add(bankInfo);
                            }
                            else
                            {
                                bankInfo.CanRequest = true;
                                bankInfo.LastModifiedBy = MasterConfigHelper.ServiceUser;
                                bankInfo.LastModifiedDate = DateTime.Now;
                                bankInfo.EndPointSystemId = this._endpoint.Id;

                                bankInfo.Save(MasterConfigHelper.ServiceUser);
                            }


                            if (bankInfo != null && userInfo != null)
                            {
                                executeQuery = string.Format(CMessage.INSERT_USER2BANK_QUERY, userInfo.Id, bankInfo.Id);
                                PeopleSoftFINOrPROCUserInfo.ExecuteQuery(executeQuery);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception @Record User2Bank Mapping - ADID:{0}, Bank Account:{1}", item.ADID, item.BankAccountNumber), ex, true);
                        }
                    }
                }
                else
                {
                    this.DoRecon = false;
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_111, GetConfiguration(EndpointKey.IPAddress)), ex, true);
            }
        }
        private void DiscoverRoleRoleDefinitionFinance()
        {
            try
            {
                this.GetRoleDefinitionFinORProcurement(true);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_111, GetConfiguration(EndpointKey.IPAddress)), ex, true);
            }
        }
        private void DiscoverRoleRoleDefinitionProcurement()
        {
            try
            {
                this.GetRoleDefinitionFinORProcurement(false);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_111, GetConfiguration(EndpointKey.IPAddress)), ex, true);
                // throw;
            }
        }
        private void DiscoverPrivTrans()
        {
            try
            {
                IList<PeopleSoftFINOrPROCPrivTransInfo> discoveredPrivTran = new List<PeopleSoftFINOrPROCPrivTransInfo>();
                discoveredPrivTran = this.GetPrivTrans();
                if (discoveredPrivTran != null && discoveredPrivTran.Count > 0)
                {
                    PeopleSoftFINOrPROCPrivTransInfo.SavePrivTrans(discoveredPrivTran, _endpoint.Id, MasterConfigHelper.ServiceUser);
                }
                //discoveredPrivTran = null;
                //discoveredPrivTran = PeopleSoftFINOrPROCPrivTransInfo.GetDeletedByEndPointSystemId(this._endpoint.Id);
                //PeopleSoftFINOrPROCPrivTransInfo.DeleteList(discoveredPrivTran.ToList());
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftFINOrPROC_111, GetConfiguration(EndpointKey.IPAddress)), ex, true);
                // throw;
            }
        }

        #endregion
        #region Dicovery objects Methods
        private IList<PeopleSoftFINOrPROCRoleInfo> GetRoles()
        {
            IList<PeopleSoftFINOrPROCRoleInfo> peoplesoftFINOrPROCRole = new List<PeopleSoftFINOrPROCRoleInfo>();
            PeopleSoftFINOrPROCRoleInfo peoplesoftFINOrPROCRoles = null;
            IDataReader reader = null;
            try
            {
                if (this.Connect())
                {
                    string query = CQuery.roleDescriptionSQL;
                    OracleCommand oracleCommand = null;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Connection = _oracleConnection;
                    oracleCommand.CommandText = query;
                    oracleCommand.CommandType = CommandType.Text;
                    reader = oracleCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        peoplesoftFINOrPROCRoles = new PeopleSoftFINOrPROCRoleInfo();
                        peoplesoftFINOrPROCRoles.EndPointSystemId = _endpoint.Id;
                        peoplesoftFINOrPROCRoles.RoleId = CheckNull.AgainstString(reader[CQuery.roleId]);
                        peoplesoftFINOrPROCRoles.RoleDescription = CheckNull.AgainstString(reader[CQuery.roleDescription]);
                        peoplesoftFINOrPROCRole.Add(peoplesoftFINOrPROCRoles);
                    }

                }
            }
            catch (Exception ex)
            {
                doUser2RoleRecon = false;
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while saving Roles from file path:{0}", GetConfiguration(EndpointKey.RoleFilePath)), ex);
            }
            finally
            {
                this.Disconnect();
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }

            return peoplesoftFINOrPROCRole;
        }
        private IList<PeopleSoftFINOrPROCBUDescriptionInfo> GetBUDescription()
        {
            IList<PeopleSoftFINOrPROCBUDescriptionInfo> peoplesoftFINOrPROCBUDescription = new List<PeopleSoftFINOrPROCBUDescriptionInfo>();
            PeopleSoftFINOrPROCBUDescriptionInfo buDescription = null;
            IDataReader reader = null;
            try
            {
                if (this.Connect())
                {
                    string query = CQuery.BUISINESSUNIT_QUERY;
                    OracleCommand oracleCommand = null;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Connection = _oracleConnection;
                    oracleCommand.CommandText = query;
                    oracleCommand.CommandType = CommandType.Text;
                    reader = oracleCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        buDescription = new PeopleSoftFINOrPROCBUDescriptionInfo();
                        buDescription.EndPointSystemId = _endpoint.Id;
                        buDescription.BUID = CheckNull.AgainstString(reader[CQuery.buId]);
                        buDescription.BUDescription = CheckNull.AgainstString(reader[CQuery.buDescription]);
                        peoplesoftFINOrPROCBUDescription.Add(buDescription);
                    }
                }
            }
            catch (Exception ex)
            {
                this.doUser2BURecon = false;
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while saving FSCM - BUDescription"), ex);
            }
            finally
            {
                this.Disconnect();
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }

            return peoplesoftFINOrPROCBUDescription;
        }
        private IList<PeopleSoftFINOrPROCBankDescriptionInfo> GetBankDescription()
        {
            IList<PeopleSoftFINOrPROCBankDescriptionInfo> peoplesoftFINOrPROCBankDescription = new List<PeopleSoftFINOrPROCBankDescriptionInfo>();
            PeopleSoftFINOrPROCBankDescriptionInfo peoplesoftFINOrPROCBankDescriptions = null;
            IDataReader reader = null;
            try
            {
                if (this.Connect())
                {
                    string query = CQuery.bankDescriptionSQL;
                    OracleCommand oracleCommand = null;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Connection = _oracleConnection;
                    oracleCommand.CommandText = query;
                    oracleCommand.CommandType = CommandType.Text;
                    reader = oracleCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        peoplesoftFINOrPROCBankDescriptions = new PeopleSoftFINOrPROCBankDescriptionInfo();
                        peoplesoftFINOrPROCBankDescriptions.EndPointSystemId = _endpoint.Id;
                        peoplesoftFINOrPROCBankDescriptions.BankAccount = CheckNull.AgainstString(reader[CQuery.bankAccount]);
                        peoplesoftFINOrPROCBankDescriptions.BankAccountDesc = CheckNull.AgainstString(reader[CQuery.bankAccountDesc]);
                        peoplesoftFINOrPROCBankDescription.Add(peoplesoftFINOrPROCBankDescriptions);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while saving FSCM - Bank Description"), ex);
            }
            finally
            {
                this.Disconnect();
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }

            return peoplesoftFINOrPROCBankDescription;
        }
        private IList<PeopleSoftFINOrPROCCCtoOUInfo> GetCCtoOU()
        {
            IList<PeopleSoftFINOrPROCCCtoOUInfo> peoplesoftFINOrPROCCCtoOU = new List<PeopleSoftFINOrPROCCCtoOUInfo>();
            PeopleSoftFINOrPROCCCtoOUInfo CCtoOU = null;
            IDataReader reader = null;
            try
            {
                if (this.Connect())
                {
                    string query = CQuery.cctoOUSQL;
                    OracleCommand oracleCommand = null;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Connection = _oracleConnection;
                    oracleCommand.CommandText = query;
                    oracleCommand.CommandType = CommandType.Text;
                    reader = oracleCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        CCtoOU = new PeopleSoftFINOrPROCCCtoOUInfo();
                        CCtoOU.EndPointSystemId = _endpoint.Id;
                        CCtoOU.CostCenter = CheckNull.AgainstString(reader[CQuery.costCenter]);
                        CCtoOU.OperatingUnit = CheckNull.AgainstString(reader[CQuery.operatingUnit]);
                        peoplesoftFINOrPROCCCtoOU.Add(CCtoOU);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while saving FSCM - CC2OU"), ex);
            }
            finally
            {
                this.Disconnect();
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
            return peoplesoftFINOrPROCCCtoOU;
        }
        private IList<PeopleSoftFINOrPROCOUtoTaggingInfo> GetOUtoTagging()
        {
            IList<PeopleSoftFINOrPROCOUtoTaggingInfo> peoplesoftFINOrPROCOUtoTaggings = new List<PeopleSoftFINOrPROCOUtoTaggingInfo>();
            IDataReader reader = null;
            try
            {
                if (this.Connect())
                {
                    string query = CQuery.outoTaggingSQL;
                    OracleCommand oracleCommand = null;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Connection = _oracleConnection;
                    oracleCommand.CommandText = query;
                    oracleCommand.CommandType = CommandType.Text;
                    reader = oracleCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        PeopleSoftFINOrPROCOUtoTaggingInfo peoplesoftFINOrPROCOUtoTagging = new PeopleSoftFINOrPROCOUtoTaggingInfo();
                        peoplesoftFINOrPROCOUtoTagging.EndPointSystemId = _endpoint.Id;
                        peoplesoftFINOrPROCOUtoTagging.OperatingUnit = CheckNull.AgainstString(reader[CQuery.operatingUnit]);
                        peoplesoftFINOrPROCOUtoTagging.TaggingRole = CheckNull.AgainstString(reader[CQuery.taggingRole]);
                        peoplesoftFINOrPROCOUtoTaggings.Add(peoplesoftFINOrPROCOUtoTagging);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while saving FSCM - OU2Tagging"), ex);
            }
            finally
            {
                this.Disconnect();
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
            return peoplesoftFINOrPROCOUtoTaggings;
        }

        private IList<PeopleSoftFINOrPROCUserRole> GetUserRoles()
        {
            IList<PeopleSoftFINOrPROCUserRole> peoplesoftFINOrPROCUserRole = new List<PeopleSoftFINOrPROCUserRole>();
            try
            {

                string mandatoryCols = CMessage.FIN_U2R_MANDATORY;
                string filePath = GetConfiguration(EndpointKey.UserRoleFilePath);
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols, 0, true, false);
                if (dataTable != null)
                {
                    var userList = (from table in dataTable.AsEnumerable()
                                    group table by new
                                    {
                                        ADID = table.Field<string>(CQuery.ROLEUSER)
                                    } into g
                                    select new PeopleSoftFINOrPROCUserInfo
                                    {
                                        EmployeeAD_ID = g.Key.ADID
                                    }).ToList();

                    var roleList = (from table in dataTable.AsEnumerable()
                                    group table by new
                                    {
                                        RoleName = table.Field<string>(CQuery.ROLENAME)
                                    } into g
                                    select new PeopleSoftFINOrPROCRoleInfo
                                    {
                                        RoleId = g.Key.RoleName,
                                        RoleDescription = g.Key.RoleName
                                    }).ToList();

                    //Save Users
                    PeopleSoftFINOrPROCUserInfo.SaveUsers(userList, _endpoint.Id, MasterConfigHelper.ServiceUser);

                    //Save Roles
                    PeopleSoftFINOrPROCRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser, false);

                    List<PeopleSoftFINOrPROCUserInfo> existingUserList = PeopleSoftFINOrPROCUserInfo.GetByEndpointSystemIdIncludeEndPointSystem(_endpoint.Id).ToList();

                    List<PeopleSoftFINOrPROCRoleInfo> existingRoleList = PeopleSoftFINOrPROCRoleInfo.GetByEndpointSystemId(_endpoint.Id).ToList();

                    PeopleSoftFINOrPROCRoleInfo roleItem = null;
                    PeopleSoftFINOrPROCUserInfo userItem = null;
                    PeopleSoftFINOrPROCUserRole userRoles = null;
                    foreach (DataRow currentWorkingRow in dataTable.Rows)
                    {
                        try
                        {
                            var adId = Convert.ToString(GetColumnValue(currentWorkingRow, CQuery.ROLEUSER)).Trim();
                            var roleId = Convert.ToString(GetColumnValue(currentWorkingRow, CQuery.ROLENAME)).Trim();

                            if (string.IsNullOrEmpty(adId) || string.IsNullOrEmpty(roleId))
                            {
                                continue;
                            }

                            userItem = existingUserList.Where(u => u.EmployeeAD_ID == adId).FirstOrDefault();
                            roleItem = existingRoleList.Where(r => r.RoleId == roleId).FirstOrDefault();

                            if (userItem != null && roleItem != null)
                            {
                                userRoles = new PeopleSoftFINOrPROCUserRole();
                                userRoles.EmployeeAD_ID = CheckNull.AgainstString(userItem.Id);
                                userRoles.RoleId = CheckNull.AgainstString(roleItem.Id);
                                peoplesoftFINOrPROCUserRole.Add(userRoles);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while saving FSCM - User2Role"), ex);
                        }

                    }

                    existingUserList = null; existingRoleList = null;
                    this.AuthenticateNetworkAndDeleteAllFiles(filePath);
                }
                else
                {
                    doUser2RoleRecon = false;
                }
            }
            catch (Exception ex)
            {
                this.DoRecon = false;
            }

            return peoplesoftFINOrPROCUserRole;
        }

        private IList<PeopleSoftFINOrPROCUser2BusinessUnit> GetUser2BU()
        {
            IList<PeopleSoftFINOrPROCUser2BusinessUnit> user2BuList = new List<PeopleSoftFINOrPROCUser2BusinessUnit>();
            IDataReader reader = null;
            PeopleSoftFINOrPROCUser2BusinessUnit user2Bu = null;
            try
            {
                if (this.Connect())
                {
                    string query = CQuery.USER2BU_QUERY;
                    OracleCommand oracleCommand = null;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Connection = _oracleConnection;
                    oracleCommand.CommandText = query;
                    oracleCommand.CommandType = CommandType.Text;
                    reader = oracleCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        user2Bu = new PeopleSoftFINOrPROCUser2BusinessUnit();
                        user2Bu.EndpointSystemId = _endpoint.Id;
                        user2Bu.ADID = CheckNull.AgainstString(reader[CQuery.OprIdADID]);
                        user2Bu.BusinessUnit = CheckNull.AgainstString(reader[CQuery.BusinessUnit]);
                        if (!string.IsNullOrEmpty(user2Bu.ADID))
                            user2BuList.Add(user2Bu);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while saving FSCM - GetUser2BU"), ex);
                this.DoRecon = false;
            }
            finally
            {
                this.Disconnect();
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }

            return user2BuList;
        }

        private IList<PeopleSoftFINOrPROCUser2BankAccount> GetUser2Banks()
        {
            IList<PeopleSoftFINOrPROCUser2BankAccount> userBankList = new List<PeopleSoftFINOrPROCUser2BankAccount>();
            IDataReader reader = null;
            PeopleSoftFINOrPROCUser2BankAccount user2Bank = null;
            try
            {
                if (this.Connect())
                {
                    string query = CQuery.USER2BANK_QUERY;
                    OracleCommand oracleCommand = null;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Connection = _oracleConnection;
                    oracleCommand.CommandText = query;
                    oracleCommand.CommandType = CommandType.Text;
                    reader = oracleCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        user2Bank = new PeopleSoftFINOrPROCUser2BankAccount();
                        user2Bank.ADID = CheckNull.AgainstString(reader[CQuery.OprIdADID]);
                        user2Bank.BankAccountNumber = CheckNull.AgainstString(reader[CQuery.bankAccount]);
                        if (!string.IsNullOrEmpty(user2Bank.ADID))
                            userBankList.Add(user2Bank);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while saving FSCM - GetUser2Bank"), ex);
                this.DoRecon = false;
            }
            finally
            {
                this.Disconnect();
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }

            return userBankList;
        }


        private void GetRoleDefinitionFinORProcurement(bool isFinanace)
        {
            string mandatoryCols = string.Empty;
            string filePath = string.Empty;
            filePath = isFinanace == true ? Convert.ToString(GetConfiguration(EndpointKey.FinanceFilePath)) : Convert.ToString(GetConfiguration(EndpointKey.ProcurementFilePath));
            string readFileName = string.Empty;
            var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols);
            if (dataTable != null)
            {
                //Get Distinct roles 
                //var roleList = (from table in dataTable.AsEnumerable()
                //                group table by new
                //                {
                //                    RoleName = table.Field<string>(CQuery.Role)
                //                } into g
                //                select new PeopleSoftFINOrPROCRoleInfo
                //                {
                //                    RoleId = g.Key.RoleName,
                //                    RoleDescription = g.Key.RoleName,
                //                }).ToList();

                ////save Roles - If not exist will insert
                //PeopleSoftFINOrPROCRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser);
                var childSplitFormEps = EndPoint.GetAllActiveChildSplitForm(_endpoint.Id);
                //if (childSplitFormEps != null)
                //{
                //    foreach (var childSplitFormEp in childSplitFormEps)
                //    {
                //        PeopleSoftFINOrPROCRoleInfo.SaveRoles(roleList, childSplitFormEp.Id, MasterConfigHelper.ServiceUser);
                //    }
                //}

                var catalogueList = (from table in dataTable.AsEnumerable()
                                     group table by new
                                     {
                                         Module = table.Field<string>(CQuery.Module),
                                         CatalogueName = table.Field<string>(CQuery.CatalogueName),
                                         RoleName = table.Field<string>(CQuery.ROLENAME),
                                     } into g
                                     select new NRoleGroup
                                     {
                                         RoleGroup = g.Key.CatalogueName,
                                         Description = g.Key.CatalogueName,
                                         CreatedBy = MasterConfigHelper.ServiceUser,
                                         CreatedDate = DateTime.Now,
                                         dummyRoleName = g.Key.RoleName,
                                         dummyModuleName = g.Key.Module,
                                     }).ToList();

                catalogueList = catalogueList.Where(c => !string.IsNullOrEmpty(c.RoleGroup)).ToList();

                //Save RoleGroup and Details
                NRoleGroup.SaveRoleGroups(catalogueList, _endpoint.Id, MasterConfigHelper.ServiceUser);
                NRoleGroup.DeleteRoleGroups(_endpoint.Id, MasterConfigHelper.ServiceUser, this.Process);
                if (childSplitFormEps != null)
                {
                    foreach (var childSplitFormEp in childSplitFormEps)
                    {
                        NRoleGroup.SaveRoleGroups(catalogueList, childSplitFormEp.Id, MasterConfigHelper.ServiceUser);
                        NRoleGroup.DeleteRoleGroups(childSplitFormEp.Id, MasterConfigHelper.ServiceUser, this.Process);
                    }
                }
                int counter = 0;
                List<PeopleSoftFINOrPROCRoleInfo> exisitngRoles = PeopleSoftFINOrPROCRoleInfo.GetByEndpointSystemId(_endpoint.Id).ToList();
                //Update UserDefinaed Desc
                foreach (DataRow currentWorkingRow in dataTable.Rows)
                {
                    try
                    {
                        var module = GetColumnValue(currentWorkingRow, CQuery.Module);
                        var catalogueName = GetColumnValue(currentWorkingRow, CQuery.CatalogueName);
                        var role = GetColumnValue(currentWorkingRow, CQuery.Role);

                        if (string.IsNullOrEmpty(role))
                        {
                            continue;
                        }

                        var peoplesoftFINOrPROCRole = exisitngRoles.Where(r => r.RoleId == role).FirstOrDefault();

                        if (peoplesoftFINOrPROCRole != null)
                        {
                            //update the userdefineddisplayname = module name - role name
                            //can request needs to be true.
                            if (string.IsNullOrEmpty(catalogueName))
                            {
                                peoplesoftFINOrPROCRole.CanRequest = true;
                            }

                            peoplesoftFINOrPROCRole.UserDefinedDisplayName = Convert.ToString(module).Trim() + '-' + Convert.ToString(role).Trim();
                            peoplesoftFINOrPROCRole.LastModifiedBy = MasterConfigHelper.ServiceUser;
                            peoplesoftFINOrPROCRole.LastModifiedDate = DateTime.Now;
                            peoplesoftFINOrPROCRole.EndPointSystemId = _endpoint.Id;
                            peoplesoftFINOrPROCRole.Save(MasterConfigHelper.ServiceUser);
                        }
                        else if (peoplesoftFINOrPROCRole == null)
                        {
                            peoplesoftFINOrPROCRole = new PeopleSoftFINOrPROCRoleInfo();
                            //update the userdefineddisplayname = module name - role name
                            //can request needs to be true.
                            if (string.IsNullOrEmpty(catalogueName))
                            {
                                peoplesoftFINOrPROCRole.CanRequest = true;
                                counter++;
                            }

                            peoplesoftFINOrPROCRole.RoleId = role;
                            peoplesoftFINOrPROCRole.RoleDescription = role;
                            peoplesoftFINOrPROCRole.CreatedBy = MasterConfigHelper.ServiceUser;
                            peoplesoftFINOrPROCRole.CreatedDate = DateTime.Now;
                            peoplesoftFINOrPROCRole.UserDefinedDisplayName = Convert.ToString(module).Trim() + '-' + Convert.ToString(role).Trim();

                            peoplesoftFINOrPROCRole.EndPointSystemId = _endpoint.Id;
                            peoplesoftFINOrPROCRole.Save(MasterConfigHelper.ServiceUser);

                            exisitngRoles.Add(peoplesoftFINOrPROCRole);

                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while saving FSCM - Roledefination"), ex);
                    }
                }

                if (counter > 0)
                {
                    string remarks = string.Format("{0} - New RoleDefinations Entitlements has been discovered. Endpoint System - {1}", counter, _endpoint.Id);
                    DILog.AlertNotificationLogger.WriteLog((long)_endpoint.Id, null, remarks, AlertCenterNotificationType.Discovery);
                }
                counter = 0;

                if (childSplitFormEps != null)
                {
                    foreach (var childSplitFormEp in childSplitFormEps)
                    {
                        List<PeopleSoftFINOrPROCRoleInfo> exisitngChildEpRoles = PeopleSoftFINOrPROCRoleInfo.GetByEndpointSystemId(childSplitFormEp.Id).ToList();
                        //Update UserDefinaed Desc
                        foreach (DataRow currentWorkingRow in dataTable.Rows)
                        {
                            try
                            {
                                var module = GetColumnValue(currentWorkingRow, CQuery.Module);
                                var catalogueName = GetColumnValue(currentWorkingRow, CQuery.CatalogueName);
                                var role = GetColumnValue(currentWorkingRow, CQuery.Role);

                                if (string.IsNullOrEmpty(role) && string.IsNullOrEmpty(catalogueName))
                                {
                                    continue;
                                }

                                var peoplesoftFINOrPROCRole = exisitngChildEpRoles.Where(r => r.RoleId == role).FirstOrDefault();

                                if (peoplesoftFINOrPROCRole != null)
                                {
                                    //update the userdefineddisplayname = module name - role name
                                    //can request needs to be true.
                                    if (string.IsNullOrEmpty(catalogueName))
                                    {
                                        peoplesoftFINOrPROCRole.CanRequest = true;
                                    }
                                    peoplesoftFINOrPROCRole.UserDefinedDisplayName = Convert.ToString(module).Trim() + '-' + Convert.ToString(role).Trim();
                                    peoplesoftFINOrPROCRole.LastModifiedBy = MasterConfigHelper.ServiceUser;
                                    peoplesoftFINOrPROCRole.LastModifiedDate = DateTime.Now;
                                    peoplesoftFINOrPROCRole.EndPointSystemId = childSplitFormEp.Id;
                                    peoplesoftFINOrPROCRole.Save(MasterConfigHelper.ServiceUser);

                                }
                                else if (peoplesoftFINOrPROCRole == null)
                                {
                                    peoplesoftFINOrPROCRole = new PeopleSoftFINOrPROCRoleInfo();
                                    //update the userdefineddisplayname = module name - role name
                                    //can request needs to be true.
                                    if (string.IsNullOrEmpty(catalogueName))
                                    {
                                        peoplesoftFINOrPROCRole.CanRequest = true;
                                        counter++;
                                    }
                                    peoplesoftFINOrPROCRole.RoleId = role;
                                    peoplesoftFINOrPROCRole.RoleDescription = role;
                                    peoplesoftFINOrPROCRole.CreatedBy = MasterConfigHelper.ServiceUser;
                                    peoplesoftFINOrPROCRole.CreatedDate = DateTime.Now;
                                    peoplesoftFINOrPROCRole.UserDefinedDisplayName = Convert.ToString(module).Trim() + '-' + Convert.ToString(role).Trim();

                                    peoplesoftFINOrPROCRole.EndPointSystemId = childSplitFormEp.Id;
                                    peoplesoftFINOrPROCRole.Save(MasterConfigHelper.ServiceUser);

                                    exisitngChildEpRoles.Add(peoplesoftFINOrPROCRole);
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while saving FSCM - Roledefination"), ex);
                            }
                        }

                        if (counter > 0)
                        {
                            string remarks = string.Format("{0} - New RoleDefinations Entitlements has been discovered. Endpoint System - {1}", counter, childSplitFormEp.Id);
                            DILog.AlertNotificationLogger.WriteLog((long)childSplitFormEp.Id, null, remarks, AlertCenterNotificationType.Discovery);
                        }
                        counter = 0;
                    }
                }

                this.AuthenticateNetworkAndDeleteAllFiles(filePath);
            }

        }
        private string GetColumnValue(DataRow currentRow, string columnName)
        {
            try
            {
                if (currentRow.Table.Columns.Contains(columnName))
                {
                    var value = currentRow[columnName].ToString().Trim();

                    if (value.ToUpper() == CQuery.NULL) return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format(CQuery.NULL, columnName), ex);
                return string.Empty;
            }
        }
        private void GetRole2Function()
        {
            List<PeopleSoftFINOrPROCFunctionInfo> functionList = new List<PeopleSoftFINOrPROCFunctionInfo>();
            PeopleSoftFINOrPROCFunctionInfo psFunc = null;
            string mandatoryCols = string.Empty;
            string filePath = GetConfiguration(EndpointKey.Role2FunctionFilePath);
            var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, mandatoryCols);
            if (dataTable != null)
            {
                var lstFuncs = (from table in dataTable.AsEnumerable()
                                group table by new
                                {
                                    PermissionName = table.Field<string>(CQuery.PermissionName),
                                    MenuName = table.Field<string>(CQuery.MenuName),
                                    BarName = table.Field<string>(CQuery.BarName),
                                    ContentReference = table.Field<string>(CQuery.ContentReference),
                                    Display = table.Field<string>(CQuery.Display),
                                    Description = table.Field<string>(CQuery.Description)
                                } into g
                                select new PeopleSoftFINOrPROCFunctionInfo
                                {
                                    BarName = g.Key.BarName,
                                    ContentReference = g.Key.ContentReference,
                                    Description = g.Key.Description,
                                    IsDisplay = g.Key.Display == "0" ? false : true,
                                    PermissionName = g.Key.PermissionName,
                                    MenuName = g.Key.MenuName,
                                    dummyModifiedDate = g.Select(a => a.Field<string>(CQuery.ModifiedAt)).FirstOrDefault()
                                }).ToList();


                if (lstFuncs != null && lstFuncs.Count() > 0)
                {
                    string executeQuery = string.Format(CQuery.FlushQuery, _endpoint.Id);
                    PeopleSoftFINOrPROCFunctionInfo.ExecuteQuery(executeQuery);
                    executeQuery = null;
                }

                PeopleSoftFINOrPROCFunctionInfo.SaveFunctions(lstFuncs, _endpoint.Id, MasterConfigHelper.ServiceUser);

                IList<PeopleSoftFINOrPROCRoleInfo> existingroleList = PeopleSoftFINOrPROCRoleInfo.GetByEndpointSystem(_endpoint.Id).ToList();

                var roleList = (from table in dataTable.AsEnumerable()
                                group table by new
                                {
                                    RoleName = table.Field<string>(CQuery.ROLENAME)
                                } into g
                                select new PeopleSoftFINOrPROCRoleInfo
                                {
                                    RoleId = g.Key.RoleName,
                                    RoleDescription = g.Key.RoleName,
                                }).ToList();

                PeopleSoftFINOrPROCRoleInfo.SaveRoles(roleList, _endpoint.Id, MasterConfigHelper.ServiceUser, false);

                PeopleSoftFINOrPROCRoleInfo roleItem = null;
                StringBuilder queryBuilder = new StringBuilder();
                int batchCount = 0;

                existingroleList = PeopleSoftFINOrPROCRoleInfo.GetByEndpointSystem(_endpoint.Id).ToList();
                List<PeopleSoftFINOrPROCFunctionInfo> existingfunList = PeopleSoftFINOrPROCFunctionInfo.GetByEndpointSystem((long)_endpoint.Id).ToList();

                foreach (DataRow currentWorkingRow in dataTable.Rows)
                {
                    try
                    {
                        var permissionName = Convert.ToString(GetColumnValue(currentWorkingRow, CQuery.PermissionName)).Trim();
                        var menuName = Convert.ToString(GetColumnValue(currentWorkingRow, CQuery.MenuName)).Trim();
                        var barName = Convert.ToString(GetColumnValue(currentWorkingRow, CQuery.BarName)).Trim();
                        var contentReference = Convert.ToString(GetColumnValue(currentWorkingRow, CQuery.ContentReference)).Trim();
                        var description = GetColumnValue(currentWorkingRow, CQuery.Description);
                        var isDisplay = Convert.ToString(GetColumnValue(currentWorkingRow, CQuery.Display)).Trim() == "0" ? false : true;
                        var modifiedAt = Convert.ToString(GetColumnValue(currentWorkingRow, CQuery.ModifiedAt)).Trim();
                        var roleId = Convert.ToString(GetColumnValue(currentWorkingRow, CQuery.roleId)).Trim();

                        psFunc = existingfunList.Where(f => f.PermissionName == permissionName && f.MenuName == menuName &&
                                                            f.BarName == barName && f.ContentReference == contentReference &&
                                                            f.Description == description && f.IsDisplay == isDisplay).FirstOrDefault();

                        roleItem = existingroleList.Where(r => r.RoleId == roleId).FirstOrDefault();

                        if (psFunc != null && roleItem != null)
                        {
                            batchCount++;
                            queryBuilder.AppendLine(string.Format(CQuery.InsertRolFunQuery, roleItem.Id, psFunc.Id));

                            if (batchCount == 500)
                            {
                                PeopleSoftFINOrPROCFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                                queryBuilder = new StringBuilder();
                                batchCount = 0;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while saving FSCM - Role2Function"), ex);
                    }
                }

                if (!string.IsNullOrEmpty(queryBuilder.ToString()))
                {
                    PeopleSoftFINOrPROCFunctionInfo.ExecuteQuery(queryBuilder.ToString());
                }
                this.AuthenticateNetworkAndDeleteAllFiles(filePath);

                IList<PeopleSoftFINOrPROCFunctionInfo> discoveredFunctions = new List<PeopleSoftFINOrPROCFunctionInfo>();
                discoveredFunctions = PeopleSoftFINOrPROCFunctionInfo.GetDeletedByEndPointSystemId(this._endpoint.Id);
                PeopleSoftFINOrPROCFunctionInfo.DeleteList(discoveredFunctions.ToList());
            }
        }
        private IList<PeopleSoftFINOrPROCPrivTransInfo> GetPrivTrans()
        {
            IList<PeopleSoftFINOrPROCPrivTransInfo> peoplesoftFINOrPROCPrivTran = new List<PeopleSoftFINOrPROCPrivTransInfo>();

            string filePath = GetConfiguration(EndpointKey.PrivillegeAcctTransFilePath);
            string manadatory = string.Empty;
            var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, manadatory, 0, false, true);
            if (dataTable != null)
            {
                PeopleSoftFINOrPROCPrivTransInfo PrivillegeAcctTrans = null;
                foreach (DataRow currentWorkingRow in dataTable.Rows)
                {
                    try
                    {
                        var operatorId = GetColumnValue(currentWorkingRow, CQuery.AUDITUSER);
                        var ADId = GetColumnValue(currentWorkingRow, CQuery.ROLEUSER);
                        var dateTime = GetColumnValue(currentWorkingRow, CQuery.AUDITSTAMP);
                        var action = GetColumnValue(currentWorkingRow, CQuery.ACTION);
                        var roleUserID = GetColumnValue(currentWorkingRow, CQuery.ROLENAME);
                        var IDASRefNo = GetColumnValue(currentWorkingRow, CQuery.REFCODE);
                        DateTime tempdateTime;
                        if (!string.IsNullOrEmpty(dateTime) && dateTime.Split('.').Count() > 3)
                        {
                            int lastIndex = dateTime.LastIndexOf('.');

                            if (lastIndex > 0)
                            {
                                dateTime = dateTime.Substring(0, lastIndex);
                                if (!string.IsNullOrEmpty(dateTime))
                                {
                                    tempdateTime = DateTime.ParseExact(dateTime, "yyyy-MM-dd HH.mm.ss", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    tempdateTime = CheckNull.AgainstDatetime(dateTime);
                                }
                            }
                            else
                            {
                                tempdateTime = CheckNull.AgainstDatetime(dateTime);
                            }
                        }
                        else
                        {
                            tempdateTime = CheckNull.AgainstDatetime(dateTime);
                        }

                        PrivillegeAcctTrans = new PeopleSoftFINOrPROCPrivTransInfo();
                        PrivillegeAcctTrans.OperatorId = CheckNull.AgainstString(operatorId);
                        PrivillegeAcctTrans.ADId = CheckNull.AgainstString(ADId);
                        PrivillegeAcctTrans.DateTime = tempdateTime;
                        PrivillegeAcctTrans.Action = CheckNull.AgainstString(action);
                        PrivillegeAcctTrans.RoleUserID = CheckNull.AgainstString(roleUserID);
                        PrivillegeAcctTrans.RefNo = CheckNull.AgainstString(IDASRefNo);
                        PrivillegeAcctTrans.EndPointSystemId = _endpoint.Id;
                        PrivillegeAcctTrans.ProcessStatus = PrivilegeProcessStatusType.UnProcessed;
                        peoplesoftFINOrPROCPrivTran.Add(PrivillegeAcctTrans);

                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error while saving FSCM - Priviliage Trans"), ex);
                    }
                }

                this.AuthenticateNetworkAndDeleteAllFiles(filePath);
            }


            return peoplesoftFINOrPROCPrivTran;
        }
        #endregion
        #region EntitlementReconcilation
        private void UpdateEntitlementReconcilation()
        {
            var accountReconcilationRole = AccountReconcilation.GetAllByEndpointId(this._endpoint.Id);

            _dicUsers = PeopleSoftFINOrPROCUserInfo.GetAllByEndPointSysId(this._endpoint.Id);
            PeopleSoftFINOrPROCUserInfo peoplesoftFINOrPROCUser = null;
            AccountReconcilation accon = null;
            EntitlementReconcilation entitlementReconcilationRole = null;
            List<EntitlementReconcilation> userentitlements = new List<EntitlementReconcilation>();
            foreach (KeyValuePair<long, string> item in _dicUsers)
            {
                try
                {
                    peoplesoftFINOrPROCUser = null; accon = null;
                    userentitlements = new List<EntitlementReconcilation>();
                    peoplesoftFINOrPROCUser = PeopleSoftFINOrPROCUserInfo.GetByIdIncludePeopleSoftFINOrPRORoles(item.Key, this._endpoint.Id);
                    if (peoplesoftFINOrPROCUser == null) return;

                    accon = accountReconcilationRole.Where(a => a.AccountName.ToLower() == peoplesoftFINOrPROCUser.EmployeeAD_ID.ToLower()).FirstOrDefault();

                    if (accon == null) continue;

                    userentitlements = EntitlementReconcilation.GetByEndpointUser(peoplesoftFINOrPROCUser.Id, this._endpoint.Id);
                    //Roles
                    if (doUser2RoleRecon && peoplesoftFINOrPROCUser != null && peoplesoftFINOrPROCUser.peoplesoftFINOrPROCRoles != null)
                    {
                        foreach (PeopleSoftFINOrPROCRoleInfo entitlementRole in peoplesoftFINOrPROCUser.peoplesoftFINOrPROCRoles)
                        {
                            entitlementReconcilationRole = null;
                            entitlementReconcilationRole = userentitlements.Where(ue => ue.Entitlement == entitlementRole.RoleId.Trim() && ue.EntitlementType == (int)EntitlementType.Role).FirstOrDefault();

                            if (entitlementReconcilationRole != null)
                            {
                                entitlementReconcilationRole.LastModifiedBy = MasterConfigHelper.ServiceUser;
                                entitlementReconcilationRole.LastModifiedDate = DateTime.Now;
                                entitlementReconcilationRole.Entitlement = entitlementRole.RoleId.Trim();
                                entitlementReconcilationRole.EntitlementDescription = entitlementRole.RoleDescription.Trim();
                                entitlementReconcilationRole.EndpointUserId = peoplesoftFINOrPROCUser.Id;
                                entitlementReconcilationRole.AccountName = peoplesoftFINOrPROCUser.EmployeeAD_ID;
                                entitlementReconcilationRole.AccountReconcilationId = accon.Id;
                                entitlementReconcilationRole.EndpointSystemId = this._endpoint.Id;
                                if (entitlementReconcilationRole.IsDeleted)
                                {
                                    EntitlementReconcilation.UpdateException(entitlementReconcilationRole, true);
                                }
                            }
                            else
                            {
                                var entitlementRoles = (peoplesoftFINOrPROCUser.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entitlementReconcilationRole = this.CreateEntitlementReconcilationShell(accon, EntitlementType.Role, string.Join(",", entitlementRoles != null ? entitlementRoles.ToArray() : new string[] { }));
                                entitlementReconcilationRole.Entitlement = entitlementRole.RoleId.Trim();
                                entitlementReconcilationRole.EntitlementId = entitlementRole.Id;
                                entitlementReconcilationRole.EntitlementDescription = entitlementRole.RoleDescription.Trim();
                                entitlementReconcilationRole.EndpointSystemId = this._endpoint.Id;
                                entitlementReconcilationRole.EndpointUserId = peoplesoftFINOrPROCUser.Id;
                                entitlementReconcilationRole.AccountName = peoplesoftFINOrPROCUser.EmployeeAD_ID;
                                EntitlementReconcilation.UpdateException(entitlementReconcilationRole, true);
                            }
                            entitlementReconcilationRole.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                    //Bank accounts
                    if (doUser2BankRecon && peoplesoftFINOrPROCUser != null && peoplesoftFINOrPROCUser.peoplesoftFINOrPROCBankAccounts != null)
                    {
                        foreach (PeopleSoftFINOrPROCBankDescriptionInfo entitlementRole in peoplesoftFINOrPROCUser.peoplesoftFINOrPROCBankAccounts)
                        {
                            entitlementReconcilationRole = null;
                            entitlementReconcilationRole = userentitlements.Where(ue => ue.Entitlement == entitlementRole.BankAccount.Trim() && ue.EntitlementType == (int)EntitlementType.BankAccount).FirstOrDefault();
                            // entitlementReconcilationRole = EntitlementReconcilation.GetByUser(peoplesoftFINOrPROCUser.Id, peoplesoftFINOrPROCUser.EndPointSystemId, entitlementRole.BankAccount.Trim(), EntitlementType.BankAccount);

                            if (entitlementReconcilationRole != null && entitlementReconcilationRole.Entitlement == entitlementRole.BankAccount.Trim())
                            {
                                entitlementReconcilationRole.LastModifiedBy = MasterConfigHelper.ServiceUser;
                                entitlementReconcilationRole.LastModifiedDate = DateTime.Now;
                                entitlementReconcilationRole.Entitlement = entitlementRole.BankAccount.Trim();
                                entitlementReconcilationRole.EntitlementDescription = entitlementRole.BankAccountDesc.Trim();
                                entitlementReconcilationRole.EndpointUserId = peoplesoftFINOrPROCUser.Id;
                                entitlementReconcilationRole.AccountName = peoplesoftFINOrPROCUser.EmployeeAD_ID;
                                entitlementReconcilationRole.AccountReconcilationId = accon.Id;
                                if (entitlementReconcilationRole.IsDeleted)
                                {
                                    EntitlementReconcilation.UpdateException(entitlementReconcilationRole, true);
                                }
                            }
                            else
                            {
                                var entitlementRoles = (peoplesoftFINOrPROCUser.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entitlementReconcilationRole = this.CreateEntitlementReconcilationShell(accon, EntitlementType.BankAccount, string.Join(",", entitlementRoles != null ? entitlementRoles.ToArray() : new string[] { }));
                                entitlementReconcilationRole.Entitlement = entitlementRole.BankAccount.Trim();
                                entitlementReconcilationRole.EntitlementId = entitlementRole.Id;
                                entitlementReconcilationRole.EndpointSystemId = this._endpoint.Id;
                                entitlementReconcilationRole.EntitlementDescription = entitlementRole.BankAccountDesc.Trim();
                                entitlementReconcilationRole.EndpointUserId = peoplesoftFINOrPROCUser.Id;
                                entitlementReconcilationRole.AccountName = peoplesoftFINOrPROCUser.EmployeeAD_ID;
                                EntitlementReconcilation.UpdateException(entitlementReconcilationRole, true);
                            }
                            entitlementReconcilationRole.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                    //Business Units
                    if (doUser2BURecon && peoplesoftFINOrPROCUser != null && peoplesoftFINOrPROCUser.peoplesoftFINOrPROCBUs != null)
                    {
                        foreach (PeopleSoftFINOrPROCBUDescriptionInfo entitlementRole in peoplesoftFINOrPROCUser.peoplesoftFINOrPROCBUs)
                        {
                            entitlementReconcilationRole = null;
                            entitlementReconcilationRole = userentitlements.Where(ue => ue.Entitlement == entitlementRole.BUID.Trim() && ue.EntitlementType == (int)EntitlementType.BusinessUnit).FirstOrDefault();
                            //entitlementReconcilationRole = EntitlementReconcilation.GetByUser(peoplesoftFINOrPROCUser.Id, peoplesoftFINOrPROCUser.EndPointSystemId, entitlementRole.BUID.Trim(), EntitlementType.BusinessUnit);

                            if (entitlementReconcilationRole != null && entitlementReconcilationRole.Entitlement == entitlementRole.BUID.Trim())
                            {
                                entitlementReconcilationRole.LastModifiedBy = MasterConfigHelper.ServiceUser;
                                entitlementReconcilationRole.LastModifiedDate = DateTime.Now;
                                entitlementReconcilationRole.Entitlement = entitlementRole.BUID.Trim();
                                entitlementReconcilationRole.EntitlementDescription = entitlementRole.BUDescription.Trim();
                                entitlementReconcilationRole.EndpointUserId = peoplesoftFINOrPROCUser.Id;
                                entitlementReconcilationRole.AccountName = peoplesoftFINOrPROCUser.EmployeeAD_ID;
                                entitlementReconcilationRole.AccountReconcilationId = accon.Id;
                                if (entitlementReconcilationRole.IsDeleted)
                                {
                                    EntitlementReconcilation.UpdateException(entitlementReconcilationRole, true);
                                }
                            }
                            else
                            {
                                var entitlementRoles = (peoplesoftFINOrPROCUser.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entitlementReconcilationRole = this.CreateEntitlementReconcilationShell(accon, EntitlementType.BusinessUnit, string.Join(",", entitlementRoles != null ? entitlementRoles.ToArray() : new string[] { }));
                                entitlementReconcilationRole.Entitlement = entitlementRole.BUID.Trim();
                                entitlementReconcilationRole.EntitlementId = entitlementRole.Id;
                                entitlementReconcilationRole.EndpointSystemId = this._endpoint.Id;
                                entitlementReconcilationRole.EntitlementDescription = entitlementRole.BUDescription.Trim();
                                entitlementReconcilationRole.EndpointUserId = peoplesoftFINOrPROCUser.Id;
                                entitlementReconcilationRole.AccountName = peoplesoftFINOrPROCUser.EmployeeAD_ID;
                                EntitlementReconcilation.UpdateException(entitlementReconcilationRole, true);
                            }
                            entitlementReconcilationRole.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.WriteDiscoveryLog(LogLevelType.Exception, "Error while updating Entitlement reconciliation.", ex, true);
                }
            }

            EntitlementReconcilation.ProcessDeletedEntitlements(this._endpoint.Id, this.Process, MasterConfigHelper.ServiceUser, true, this.FileTimeStamp);
        }

        #endregion
        #region AccountReconcilation
        private void FSCMAccountReconcilation()
        {
            try
            {
                AccountReconcilation.ProcessDeletedAccounts(this._endpoint.Id, this.Process, MasterConfigHelper.ServiceUser, true, this.FileTimeStamp);
                List<PeopleSoftFINOrPROCUserInfo> userList = PeopleSoftFINOrPROCUserInfo.GetDeletedUsers(this._endpoint.Id, this.Process, this.FileTimeStamp).ToList();
                if (userList != null && userList.Count > 0)
                {
                    PeopleSoftFINOrPROCUserInfo.RemovePeopleSoftFINOrPROCUser(userList.ToList());
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception at FSCM Account Recon", GetConfiguration(EndpointKey.IPAddress)), ex, true);

            }
        }
        #endregion

    }
}

