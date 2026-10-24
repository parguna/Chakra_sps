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
using CMessage = DI.Chakra.Connector.PeopleSoftHRMS.Message;
using CQuery = DI.Chakra.Connector.PeopleSoftHRMS.Query;
//using System.Data.OracleClient;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Transaction;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
#endregion
namespace DI.Chakra.Connector
{
    public class PeopleSoftHRMSConnector : ConnectorBase
    {
        #region Variable Declaration
        public OracleConnection _oracleConnection = null;
        public string _connectionString = string.Empty;
        public EndpointConfiguration _peopleSoftHRMSConfiguration { get; set; }
        public EndPoint _endpoint;
        IList<PeopleSoftHRMSUserInfo> _offboardTriggerView = new List<PeopleSoftHRMSUserInfo>();
        IList<PeopleSoftHRMSUserInfo> _onboardTriggerView = new List<PeopleSoftHRMSUserInfo>();
        IList<PeopleSoftHRMSUserInfo> _ouTransferTriggerView = new List<PeopleSoftHRMSUserInfo>();
        IList<PeopleSoftHRMSUserInfo> _promodemoTriggerView = new List<PeopleSoftHRMSUserInfo>();
        IList<PeopleSoftHRMSUserInfo> _cselTriggerView = new List<PeopleSoftHRMSUserInfo>();
        IList<PeopleSoftSalaryGrade> _peopleSoftSalaryGrade = new List<PeopleSoftSalaryGrade>();
        bool isOnBoardSelected; bool isOffBoardSelected; bool isOffBoardROSSelected;
        bool isPromotionSelected; bool isTransferSelected; bool isOneTimeUserSelected;
        bool isUserSelected; bool isCSELSelected; bool isWithDrawnSelected;
        bool isErrOnboard = false; bool isErrOffboard = false; bool isErrTransfer = false; bool isErrPromotion = false;
        #endregion

        #region Constructor
        public PeopleSoftHRMSConnector(EndPoint endpoint, AccessRequest currentRequest = null)
        {
            try
            {
                _endpoint = endpoint;
                this.CurrentRequest = currentRequest;
                this._peopleSoftHRMSConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
                this.EndpointConfiguration = _peopleSoftHRMSConfiguration;
                this.NetworkUserName = GetConfiguration(EndpointKey.BizServiceUserName);
                this.NetworkPassword = GetConfiguration(EndpointKey.BizServicePassword);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_102, GetConfiguration(EndpointKey.IPAddress)), ex);
            }
        }
        public EndpointConfiguration PeopleSoftHRMSConfiguration
        {
            get { return _peopleSoftHRMSConfiguration; }
            set { this._peopleSoftHRMSConfiguration = value; }
        }
        private string GetConfiguration(EndpointKey Key)
        {
            var config = PeopleSoftHRMSConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }
        public override IMProcessor IMProcessor
        {
            get
            {
                PeopleSoftHRMS.IMProcessor processor = new PeopleSoftHRMS.IMProcessor();
                processor.EndPoint = this.EndPoint;
                processor._peoplesoftHRMSConfiguration = this._peopleSoftHRMSConfiguration;
                return processor;
            }
        }
        #endregion
        #region IConnector Members
        public override bool Connect()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_103, GetConfiguration(EndpointKey.IPAddress)));

            try
            {
                string portNo = GetConfiguration(EndpointKey.PortNo);
                string instanceName = GetConfiguration(EndpointKey.InstanceName);
                string Password = RSAHelper.DecryptData(GetConfiguration(EndpointKey.Password));
                _connectionString = string.Format("Data Source={2};" +
                    "User Id={3};Password={4};", GetConfiguration(EndpointKey.IPAddress), portNo, instanceName, GetConfiguration(EndpointKey.UserName), Password);
                _oracleConnection = new OracleConnection(_connectionString);
                _oracleConnection.Open();
                WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_104, GetConfiguration(EndpointKey.IPAddress)));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftHRMS_105, GetConfiguration(EndpointKey.IPAddress)), ex);
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
            if (_oracleConnection != null && (_oracleConnection.State == ConnectionState.Open))
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
                long discoveryProcessId = base.DiscoveryProcess.Id;
                List<EndpointObjects> Objects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(discoveryProcessId);

                isOnBoardSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_HRMS_OnBoard) != null;
                isOffBoardSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_HRMS_OffBoard) != null;
                isOffBoardROSSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_HRMS_OffBoardROS) != null;
                isPromotionSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_HRMS_Promotion) != null;
                isTransferSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_HRMS_Transfer) != null;
                isOneTimeUserSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_HRMS_OneTimeUser) != null;
                isUserSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_HRMS_USER) != null;
                isCSELSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_HRMS_CSEL) != null;
                isWithDrawnSelected = Objects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_HRMS_WithDrawn) != null;

                if (_endpoint.IsTrustedSource)
                {
                    #region HRMS User & Trigger View
                    //On-Board Discovery
                    if (isOnBoardSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_208, GetConfiguration(EndpointKey.IPAddress)));
                        DiscoverOnBoardusers();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_209, GetConfiguration(EndpointKey.IPAddress)));
                    }

                    //Off-Board Discovery
                    if (isOffBoardROSSelected || isOffBoardSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_219, GetConfiguration(EndpointKey.IPAddress)));
                        DiscoverOffBoardusers();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_220, GetConfiguration(EndpointKey.IPAddress)));
                    }

                    //Transfer Discovery
                    if (isTransferSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_221, GetConfiguration(EndpointKey.IPAddress)));
                        DiscoverTransferusers();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_222, GetConfiguration(EndpointKey.IPAddress)));
                    }
                    //Promotion Discovery
                    if (isPromotionSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_223, GetConfiguration(EndpointKey.IPAddress)));
                        DiscoverPromotionusers();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_224, GetConfiguration(EndpointKey.IPAddress)));
                    }
                    //CSEL Discovery
                    if (isCSELSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_227, GetConfiguration(EndpointKey.IPAddress)));
                        DiscoverCSEL();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_228, GetConfiguration(EndpointKey.IPAddress)));
                    }
                    //Withdrawn Discovery
                    if (isWithDrawnSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_307, GetConfiguration(EndpointKey.IPAddress)));
                        Withdrawn();
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_308, GetConfiguration(EndpointKey.IPAddress)));
                    }

                    //User Discovery
                    if (isUserSelected || isOneTimeUserSelected)
                    {
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_106, GetConfiguration(EndpointKey.IPAddress)));
                        DiscoverHRMSUsers(isUserSelected);
                        WriteDiscoveryLog(LogLevelType.Info, string.Format(CMessage.PeopleSoftHRMS_107, GetConfiguration(EndpointKey.IPAddress)));
                    }

                    if (this.DoRecon && isUserSelected)
                    {
                        ProcessHRMSAccountRecon();
                    }
                    #endregion
                }

                if (this.IsExceptionRaised)
                {
                    throw new Exception(string.Format("HCM Discovery failed {0}", GetConfiguration(EndpointKey.IPAddress)));
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception occured in HRMS {0}", GetConfiguration(EndpointKey.IPAddress)), ex);
                throw;
            }
        }

        private void ProcessHRMSAccountRecon()
        {
            try
            {
                IList<PeopleSoftHRMSUserInfo> existingUserList = PeopleSoftHRMSUserInfo.GetAllByEndPointId(_endpoint.Id).Where(p => p.EmploymentStatus == "A").ToList();
                AccountReconcilation acctRecon = null;
                foreach (PeopleSoftHRMSUserInfo hrmsusr in existingUserList)
                {
                    try
                    {
                        acctRecon = null;
                        if (string.IsNullOrEmpty(hrmsusr.EmployeeAD_Id))
                            continue;

                        acctRecon = AccountReconcilation.GetByUser(hrmsusr.EndPointSystemId, hrmsusr.Id);
                        if (acctRecon == null)
                        {
                            acctRecon = new AccountReconcilation();
                            AccountReconcilation.UpdateException(acctRecon, true);
                            acctRecon.EndpointUserId = hrmsusr.Id;
                            acctRecon.AccountCreatedDate = hrmsusr.CreatedDate;
                        }
                        else
                        {

                            acctRecon.LastModifiedDate = DateTime.Now;
                            acctRecon.LastModifiedBy = MasterConfigHelper.ServiceUser;
                        }

                        acctRecon.AccountType = AccountType.User;
                        acctRecon.EndpointSystemId = hrmsusr.EndPointSystemId;
                        acctRecon.FullName = hrmsusr.DisplayName;
                        acctRecon.AccountName = hrmsusr.AccountName;
                        acctRecon.Save(MasterConfigHelper.ServiceUser);
                    }
                    catch (Exception ex)
                    {
                        string remarks = string.Format("Exception raised @ProcessHRMSAccountRecon, UserId: {0}", hrmsusr.Id);
                        DI.Chakra.Logger.DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, remarks, ex);
                    }
                }

                if (this.DoRecon)
                {
                    AccountReconcilation.ProcessDeletedAccounts(_endpoint.Id, this.Process, MasterConfigHelper.ServiceUser);
                }

            }
            catch (Exception ex)
            {
                string remarks = string.Format("Exception raised @ProcessHRMSAccountRecon");
                DI.Chakra.Logger.DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, remarks, ex);
            }
        }
        #endregion
        #region Discover Objects           
        private void DiscoverCSEL()
        {
            try
            {
                IList<PeopleSoftHRMSCSEL> discoveredPeopleSoftHRMSCSEL = new List<PeopleSoftHRMSCSEL>();
                discoveredPeopleSoftHRMSCSEL = this.GetPeopleSoftHRMSCSEL();
                if (discoveredPeopleSoftHRMSCSEL != null && discoveredPeopleSoftHRMSCSEL.Count > 0)
                {
                    SaveCSEL(discoveredPeopleSoftHRMSCSEL, _endpoint.Id, MasterConfigHelper.ServiceUser);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftHRMS_229, GetConfiguration(EndpointKey.IPAddress)), ex);
            }
        }
        private void DiscoverHRMSUsers(bool isUserSelected)
        {
            IList<PeopleSoftHRMSUserInfo> discoveredHRMSUsers = new List<PeopleSoftHRMSUserInfo>();

            if (isOneTimeUserSelected)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("OnetTime User List Started {0}", GetConfiguration(EndpointKey.IPAddress)));
                try
                {
                    discoveredHRMSUsers = this.GetOneTimeHRMSUsers();

                    if (discoveredHRMSUsers != null && discoveredHRMSUsers.Count > 0)
                    {
                        //foreach (PeopleSoftHRMSUserInfo usr in discoveredHRMSUsers)
                        //{
                        //    usr.JoinDate = Convert.ToDateTime(usr.JoinDateString);
                        //    usr.DOB = string.IsNullOrEmpty(usr.DOBString) ? (DateTime?)null : DateTime.Parse(usr.DOBString);
                        //    usr.TerminationDate = string.IsNullOrEmpty(usr.TerminationDateString) ? (DateTime?)null : DateTime.Parse(usr.TerminationDateString);

                        //}

                        ProcessUserMetaData(discoveredHRMSUsers.ToList());
                        this.DoRecon = PeopleSoftHRMSUserInfo.SaveUsers(_endpoint.Id, MasterConfigHelper.ServiceUser, discoveredHRMSUsers, true);
                    }

                    this.IsExceptionRaised = false;
                }
                catch (Exception ex)
                {
                    WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftHRMS_303, GetConfiguration(EndpointKey.IPAddress)), ex);
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("OnetTime User List Completed {0}", GetConfiguration(EndpointKey.IPAddress)));
            }

            if (isUserSelected)
            {
                try
                {
                    discoveredHRMSUsers = new List<PeopleSoftHRMSUserInfo>();

                    discoveredHRMSUsers = this.GetHRMSUsers(CMessage.PEOPLESOFTHRMS_USERLIST, false, false, EmployeeEvents.None);

                    if (discoveredHRMSUsers != null && discoveredHRMSUsers.Count > 0)
                    {
                        this.DoRecon = PeopleSoftHRMSUserInfo.SaveUsers(_endpoint.Id, MasterConfigHelper.ServiceUser, discoveredHRMSUsers, false);
                    }
                }
                catch (Exception ex)
                {
                    WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftHRMS_108, GetConfiguration(EndpointKey.IPAddress)), ex);
                }
            }
            else
            {
                this.DoRecon = false;
            }
        }
        private void DiscoverOffBoardusers()
        {
            try
            {
                List<PeopleSoftHRMSUserInfo> offboardRegView = null;

                if (isOffBoardSelected)
                {
                    offboardRegView = this.GetOffBoardUsers(CMessage.PeopleSoftHRMS_OffBoard_REG);
                    offboardRegView.ForEach(user =>
                    {
                        if (user.EffectiveDate.HasValue)
                            user.EffectiveDate = user.EffectiveDate.Value;
                        _offboardTriggerView.Add(user);
                    });

                    base.WriteCSV(offboardRegView, "OffBoardRegular");
                    if (offboardRegView != null && offboardRegView.Count > 0)
                    {
                        bool isSuccess = PeopleSoftHRMSUserInfo.OffBoardUserProcess(offboardRegView, _endpoint.Id, MasterConfigHelper.ServiceUser, false);
                        isErrOffboard = !isSuccess;
                    }
                }

                if (isOffBoardROSSelected)
                {
                    var offboardRoasView = this.GetOffBoardUsers(CMessage.PeopleSoftHRMS_OffBoard_ROS);
                    offboardRoasView.ForEach(user =>
                    {
                        if (user.EffectiveDate.HasValue)
                            user.EffectiveDate = user.EffectiveDate.Value;
                        _offboardTriggerView.Add(user);

                        try
                        {
                            User dimUser = User.GetByADID(user.EmployeeAD_Id);
                            if (dimUser != null)
                            {
                                dimUser.EmpClass = EmployeeClass.Roster;
                                dimUser.LastModifiedBy = "System";
                                dimUser.LastModifiedDate = DateTime.Now;
                                dimUser.Save("System");
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, string.Format("OffBoard Roaster - EmplClass Update Exception. User ADID - {0} ", user.EmployeeAD_Id), ex);
                        }
                    });

                    base.WriteCSV(offboardRoasView, "OffBoardRoaster");
                    if (offboardRoasView != null && offboardRoasView.Count > 0)
                    {
                        bool isSuccess = PeopleSoftHRMSUserInfo.OffBoardUserProcess(offboardRoasView, _endpoint.Id, MasterConfigHelper.ServiceUser, true);
                        isErrOffboard = !isSuccess;
                    }
                }
            }
            catch (Exception ex)
            {
                isErrOffboard = true;
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftHRMS_304, GetConfiguration(EndpointKey.IPAddress)), ex);
            }
        }
        private void DiscoverOnBoardusers()
        {
            try
            {
                _onboardTriggerView = this.GetOnBoardUsers(CMessage.PeopleSoftHRMS_153);
                if (_onboardTriggerView != null && _onboardTriggerView.Count > 0)
                {
                    ProcessUserMetaData(_onboardTriggerView.ToList());
                    base.WriteCSV(_onboardTriggerView, "OnBoard");
                    bool isSuccess = PeopleSoftHRMSUserInfo.SaveOnboardTrigger(_onboardTriggerView, _endpoint.Id, MasterConfigHelper.ServiceUser);
                    isErrOnboard = !isSuccess;
                }
            }
            catch (Exception ex)
            {
                isErrOnboard = true;
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftHRMS_210, GetConfiguration(EndpointKey.IPAddress)), ex);
            }
        }
        private void DiscoverTransferusers()
        {
            try
            {
                _ouTransferTriggerView = this.GetTransferUsers(CMessage.PeopleSoftHRMS_155);
                if (_ouTransferTriggerView != null && _ouTransferTriggerView.Count > 0)
                {
                    ProcessUserMetaData(_ouTransferTriggerView.ToList());
                    base.WriteCSV(_ouTransferTriggerView, "Transfer");
                    bool isSuccess = PeopleSoftHRMSUserInfo.TransferUserProcess(_ouTransferTriggerView, _endpoint.Id, MasterConfigHelper.ServiceUser);
                    isErrTransfer = !isSuccess;
                }
            }
            catch (Exception ex)
            {
                isErrTransfer = true;
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftHRMS_305, GetConfiguration(EndpointKey.IPAddress)), ex);
            }
        }
        private void DiscoverPromotionusers()
        {
            try
            {
                _promodemoTriggerView = this.GetPromotionUsers(CMessage.PeopleSoftHRMS_154);
                if (_promodemoTriggerView != null && _promodemoTriggerView.Count > 0)
                {
                    ProcessUserMetaData(_promodemoTriggerView.ToList());
                    base.WriteCSV(_promodemoTriggerView, "PromotionDemotion");
                    bool isSuccess = PeopleSoftHRMSUserInfo.PromotionUserProcess(_promodemoTriggerView, _endpoint.Id, MasterConfigHelper.ServiceUser);
                    isErrPromotion = !isSuccess;
                }
            }
            catch (Exception ex)
            {
                isErrPromotion = true;
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftHRMS_306, GetConfiguration(EndpointKey.IPAddress)), ex);
            }
        }
        #endregion

        #region Dicovery objects Methods
        private void SaveNHRMSOrganisation(IList<NHRMSOrganisation> hrmsOrganisation, long endpointSystemId, string currentUser, string objectName)
        {
            NHRMSOrganisation organisation = null;
            foreach (NHRMSOrganisation org in hrmsOrganisation)
            {
                if (string.IsNullOrEmpty(org.Code)) continue;

                try
                {
                    organisation = NHRMSOrganisation.GetByOrgCode(org.Code);
                    if (organisation == null)
                    {
                        org.CreatedBy = Constants.CreatedBy;
                        org.CreatedDate = DateTime.Now;
                        org.LastModifiedDate = null;
                        org.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        org.LastModifiedBy = Constants.CreatedBy;
                        org.LastModifiedDate = DateTime.Now;
                        org.Code = org.Code;
                        org.Id = organisation.Id;
                        org.Save(MasterConfigHelper.ServiceUser);
                    }
                }
                catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - SaveNHRMSOrganisation", ex); }
            }
        }
        private void SaveNGrade(IList<NGrade> ngrade, long endpointSystemId, string currentUser)
        {
            foreach (NGrade nGrade in ngrade)
            {
                if (string.IsNullOrEmpty(nGrade.Grade)) continue;
                try
                {
                    var grade = NGrade.GetByCode(nGrade.Grade);

                    var gradeRef = NGradeReference.GetByGrade(nGrade.Grade);
                    nGrade.SequenceNumber = gradeRef == null ? 0 : gradeRef.Sequence;

                    if (grade == null)
                    {
                        nGrade.CreatedBy = Constants.CreatedBy;
                        nGrade.CreatedDate = DateTime.Now;
                        nGrade.LastModifiedDate = null;
                        nGrade.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        nGrade.LastModifiedBy = Constants.IACMADMIN;
                        nGrade.LastModifiedDate = DateTime.Now;
                        nGrade.Grade = nGrade.Grade;
                        nGrade.Id = grade.Id;
                        nGrade.Save(MasterConfigHelper.ServiceUser);
                    }
                }
                catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - SaveNGrade", ex); }
            }
        }

        private void SaveNPlan(IList<NPlan> nplan, long endpointSystemId, string currentUser)
        {

            foreach (NPlan nPlan in nplan)
            {
                if (string.IsNullOrEmpty(nPlan.Plan)) continue;
                try
                {
                    var grade = NPlan.GetByPlan(nPlan.Plan);
                    if (grade == null)
                    {
                        nPlan.CreatedBy = Constants.CreatedBy;
                        nPlan.CreatedDate = DateTime.Now;
                        nPlan.LastModifiedDate = null;
                        nPlan.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        nPlan.LastModifiedBy = Constants.IACMADMIN;
                        nPlan.LastModifiedDate = DateTime.Now;
                        nPlan.Plan = nPlan.Plan;
                        nPlan.Id = grade.Id;
                        nPlan.Save(MasterConfigHelper.ServiceUser);
                    }
                }
                catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - SaveNPlan", ex); }
            }
        }
        private void SaveNHRMSDesignation(IList<NHRMSDesignation> ndesignation, long endpointSystemId, string currentUser)
        {
            foreach (NHRMSDesignation nDesignation in ndesignation)
            {
                if (string.IsNullOrEmpty(nDesignation.Code)) continue;
                try
                {
                    var designation = NHRMSDesignation.GetByCode(nDesignation.Code, endpointSystemId);
                    if (designation == null)
                    {
                        nDesignation.CreatedBy = Constants.CreatedBy;
                        nDesignation.CreatedDate = DateTime.Now;
                        nDesignation.LastModifiedDate = null;
                        nDesignation.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        nDesignation.LastModifiedBy = Constants.IACMADMIN;
                        nDesignation.LastModifiedDate = DateTime.Now;
                        nDesignation.Code = nDesignation.Code;
                        nDesignation.Id = designation.Id;
                        nDesignation.Save(MasterConfigHelper.ServiceUser);
                    }
                }
                catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - SaveNHRMSDesignation", ex); }
            }
        }

        private void SaveNHRMSJobcode(IList<NHRMSJobcode> njobcode, long endpointSystemId, string currentUser)
        {
            foreach (NHRMSJobcode nJobcode in njobcode)
            {
                if (string.IsNullOrEmpty(nJobcode.Code)) continue;
                try
                {
                    var jobcode = NHRMSJobcode.GetByCode(nJobcode.Code, endpointSystemId);
                    if (jobcode == null)
                    {
                        nJobcode.CreatedBy = Constants.CreatedBy;
                        nJobcode.CreatedDate = DateTime.Now;
                        nJobcode.LastModifiedDate = null;
                        nJobcode.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        nJobcode.LastModifiedBy = Constants.IACMADMIN;
                        nJobcode.LastModifiedDate = DateTime.Now;
                        nJobcode.Code = nJobcode.Code;
                        nJobcode.Id = jobcode.Id;
                        nJobcode.Save(MasterConfigHelper.ServiceUser);
                    }
                }
                catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - SaveNHRMSJobcode", ex); }
            }
        }

        private void SaveNHRMSCostCenter(IList<NHRMSCostCenter> ncostCenter, long endpointSystemId, string currentUser)
        {
            foreach (NHRMSCostCenter nCostCenter in ncostCenter)
            {
                if (string.IsNullOrEmpty(nCostCenter.Code)) continue;
                try
                {
                    var costCenter = NHRMSCostCenter.GetByCode(nCostCenter.Code, endpointSystemId);
                    if (costCenter == null)
                    {
                        nCostCenter.CreatedBy = Constants.CreatedBy;
                        nCostCenter.CreatedDate = DateTime.Now;
                        nCostCenter.LastModifiedDate = null;
                        nCostCenter.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        nCostCenter.LastModifiedBy = Constants.IACMADMIN;
                        nCostCenter.LastModifiedDate = DateTime.Now;
                        nCostCenter.Code = nCostCenter.Code;
                        nCostCenter.Id = costCenter.Id;
                        nCostCenter.Save(MasterConfigHelper.ServiceUser);
                    }
                }
                catch (Exception ex)
                {
                    WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - SaveNHRMSCostCenter", ex);
                }
            }
        }
        private void SaveCSEL(IList<PeopleSoftHRMSCSEL> csel, long endpointSystemId, string currentUser)
        {

            if (csel != null && csel.Count() > 0)
            {
                User.ExecuteQuery("UPDATE DI_M_User SET IsCSEL = 0, CSELDescription = NULL, CSELLicNo = NULL, StartDate = NULL, ExpiryDate = NULL ");
                User.ExecuteQuery("UPDATE DI_HRMS_User SET CSELDescription = NULL, CSELLicNo = NULL, StartDate = NULL, ExpiryDate = NULL ");
                NHRMSUser nHRMSUser = null;
                User diMSUser = null;

                List<NHRMSUser> hrmsUserList = NHRMSUser.GetAllActiveEmps();
                List<User> diMUserList = User.GetActiveEmployees();

                foreach (PeopleSoftHRMSCSEL peopleSoftHRMSCSEL in csel)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(peopleSoftHRMSCSEL.EmployeeId))
                        {
                            continue;
                        }
                        nHRMSUser = hrmsUserList.Where(n => n.EmployeeId == peopleSoftHRMSCSEL.EmployeeId).FirstOrDefault();

                        if (nHRMSUser != null)
                        {
                            nHRMSUser.CSELLicNo = peopleSoftHRMSCSEL.CSELLicNo;
                            nHRMSUser.CSELDescription = peopleSoftHRMSCSEL.CSELDescription;
                            nHRMSUser.StartDate = (peopleSoftHRMSCSEL.StartDate != DateTime.MinValue && peopleSoftHRMSCSEL.StartDate != DateTime.MaxValue) ? peopleSoftHRMSCSEL.StartDate : null;
                            nHRMSUser.ExpiryDate = (peopleSoftHRMSCSEL.ExpiryDate != DateTime.MinValue && peopleSoftHRMSCSEL.ExpiryDate != DateTime.MaxValue) ? peopleSoftHRMSCSEL.ExpiryDate : null;

                            nHRMSUser.Save(MasterConfigHelper.ServiceUser);
                        }

                        diMSUser = diMUserList.Where(d => d.EmployeeId == peopleSoftHRMSCSEL.EmployeeId).FirstOrDefault();

                        if (diMSUser != null)
                        {
                            diMSUser.IsCSEL = true;
                            diMSUser.CSELLicNo = peopleSoftHRMSCSEL.CSELLicNo;
                            diMSUser.CSELDescription = peopleSoftHRMSCSEL.CSELDescription;
                            diMSUser.StartDate = (peopleSoftHRMSCSEL.StartDate != DateTime.MinValue && peopleSoftHRMSCSEL.StartDate != DateTime.MaxValue) ? peopleSoftHRMSCSEL.StartDate : null;
                            diMSUser.ExpiryDate = (peopleSoftHRMSCSEL.ExpiryDate != DateTime.MinValue && peopleSoftHRMSCSEL.ExpiryDate != DateTime.MaxValue) ? peopleSoftHRMSCSEL.ExpiryDate : null;

                            diMSUser.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - SaveCSEL", ex);
                    }
                }
            }
        }
        IList<NHRMSOrganisation> GetCompany()
        {
            string query = CMessage.PeopleSoftHRMS_153;
            // CMessage.PeopleSoftHRMS_124;
            IDataReader reader = null;
            IList<NHRMSOrganisation> nhrmsOrganisation = new List<NHRMSOrganisation>();
            try
            {
                OracleCommand oracleCommand = null;
                oracleCommand = new OracleCommand();
                oracleCommand.Connection = _oracleConnection;
                oracleCommand.CommandText = query;
                oracleCommand.CommandType = CommandType.Text;
                reader = oracleCommand.ExecuteReader();
                NHRMSOrganisation hrmsOrganisation = null;
                while (reader.Read())
                {
                    try
                    {
                        hrmsOrganisation = new NHRMSOrganisation();
                        hrmsOrganisation.EndpointSystemId = _endpoint.Id;
                        hrmsOrganisation.Code = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_168]);
                        hrmsOrganisation.Name = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_168]);
                        hrmsOrganisation.Description = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_COMPANY_DESC]);
                        hrmsOrganisation.IsActive = true;
                        hrmsOrganisation.Scope = CMessage.PeopleSoftHRMS_125;
                        hrmsOrganisation.ParentOrgCode = null;

                        nhrmsOrganisation.Add(hrmsOrganisation);
                    }
                    catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetCompany", ex); }
                }
            }
            catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetCompany", ex); }
            finally
            {
                reader.Close();
                reader.Dispose();
            }

            return nhrmsOrganisation;
        }
        private DataSet GetManagerID(string strQry)
        {
            DataSet dsResults = new DataSet();
            try
            {
                if (_oracleConnection.State == ConnectionState.Closed)
                {
                    this.Connect();
                }
                OracleDataAdapter oracleDataAdapter;
                OracleCommand oracleCommand = new OracleCommand();
                oracleCommand.Connection = _oracleConnection;
                oracleCommand.CommandType = CommandType.Text;
                oracleCommand.CommandText = strQry;
                oracleDataAdapter = new OracleDataAdapter(oracleCommand.CommandText, oracleCommand.Connection);
                oracleDataAdapter.Fill(dsResults);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format(CMessage.PeopleSoftHRMS_146, GetConfiguration(EndpointKey.IPAddress)), ex);
            }
            return dsResults;
        }
        private string GetManagerFromPosition(string PositionNumber)
        {
            string ManagerID = string.Empty;
            string ManagerQuery = CMessage.PeopleSoftHRMS_147;
            ManagerQuery = string.Format(ManagerQuery, PositionNumber);
            DataSet dsManager = GetManagerID(ManagerQuery);
            if (dsManager.Tables.Count > 0 && dsManager.Tables[0].Rows.Count > 0)
            {
                ManagerID = dsManager.Tables[0].Rows[0][CMessage.PeopleSoftHRMS_148].ToString();
            }
            return ManagerID;
        }
        private IList<NHRMSOrganisation> GetDepartment()
        {
            string query = CMessage.PeopleSoftHRMS_153;// CMessage.PeopleSoftHRMS_126;
            IDataReader reader = null;
            IList<NHRMSOrganisation> nhrmsOrganisation = new List<NHRMSOrganisation>();
            try
            {
                OracleCommand oracleCommand = null;
                oracleCommand = new OracleCommand();
                oracleCommand.Connection = _oracleConnection;
                oracleCommand.CommandText = query;
                oracleCommand.CommandType = CommandType.Text;
                reader = oracleCommand.ExecuteReader();
                NHRMSOrganisation hrmsOrganisation = null;
                while (reader.Read())
                {
                    try
                    {
                        hrmsOrganisation = new NHRMSOrganisation();
                        hrmsOrganisation.EndpointSystemId = _endpoint.Id;
                        hrmsOrganisation.Code = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_142]);
                        hrmsOrganisation.Name = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_142]);
                        hrmsOrganisation.Description = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_DEPT_DESC]);
                        hrmsOrganisation.IsActive = true;
                        hrmsOrganisation.Scope = CMessage.PeopleSoftHRMS_127;
                        nhrmsOrganisation.Add(hrmsOrganisation);
                    }
                    catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetDepartment", ex); }
                }
            }
            catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetDepartment", ex); }
            finally
            {
                reader.Close();
                reader.Dispose();
            }

            return nhrmsOrganisation;
        }
        IList<NHRMSOrganisation> GetBusinessUnit()
        {
            string query = CMessage.PeopleSoftHRMS_153;// CMessage.PeopleSoftHRMS_128;
            IDataReader reader = null;
            IList<NHRMSOrganisation> nhrmsOrganisation = new List<NHRMSOrganisation>();
            try
            {
                OracleCommand oracleCommand = null;
                oracleCommand = new OracleCommand();
                oracleCommand.Connection = _oracleConnection;
                oracleCommand.CommandText = query;
                oracleCommand.CommandType = CommandType.Text;
                reader = oracleCommand.ExecuteReader();
                NHRMSOrganisation hrmsOrganisation = null;
                while (reader.Read())
                {
                    try
                    {
                        hrmsOrganisation = new NHRMSOrganisation();
                        hrmsOrganisation.EndpointSystemId = _endpoint.Id;
                        hrmsOrganisation.Code = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_188]);
                        hrmsOrganisation.Name = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_188]);
                        hrmsOrganisation.Description = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_BUSINESSUNIT_DESC]);
                        // var parentOrgCode = NHRMSOrganisation.GetByOrgCode(CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_132]));
                        //hrmsOrganisation.ParentOrgCode = parentOrgCode != null ? parentOrgCode.Code : null;
                        hrmsOrganisation.ParentOrgCode = null;
                        hrmsOrganisation.IsActive = true;
                        hrmsOrganisation.Scope = CMessage.PeopleSoftHRMS_129;
                        nhrmsOrganisation.Add(hrmsOrganisation);
                    }
                    catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetBusinessUnit", ex); }

                }

            }
            catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetBusinessUnit", ex); }
            finally
            {
                reader.Close();
                reader.Dispose();
            }

            return nhrmsOrganisation;
        }
        IList<NHRMSDesignation> GetDesignation()
        {
            string query = CMessage.PeopleSoftHRMS_153;// CMessage.PeopleSoftHRMS_134;
            IDataReader reader = null;
            IList<NHRMSDesignation> hrmsDesignation = new List<NHRMSDesignation>();
            try
            {
                OracleCommand oracleCommand = null;
                oracleCommand = new OracleCommand();
                oracleCommand.Connection = _oracleConnection;
                oracleCommand.CommandText = query;
                oracleCommand.CommandType = CommandType.Text;
                reader = oracleCommand.ExecuteReader();
                NHRMSDesignation nhrmsDesignation = null;
                while (reader.Read())
                {
                    try
                    {
                        nhrmsDesignation = new NHRMSDesignation();
                        nhrmsDesignation.EndpointSystemId = _endpoint.Id;
                        nhrmsDesignation.Code = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_203]);
                        nhrmsDesignation.Description = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_DESIGN_DESC]);
                        nhrmsDesignation.IsActive = true;
                        hrmsDesignation.Add(nhrmsDesignation);
                    }
                    catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetDesignation", ex); }
                }

            }
            catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetDesignation", ex); }
            finally
            {
                reader.Close();
                reader.Dispose();
            }

            return hrmsDesignation;
        }
        IList<NHRMSJobcode> GetJobcode()
        {
            string query = CMessage.PeopleSoftHRMS_153;// CMessage.PeopleSoftHRMS_134;
            IDataReader reader = null;
            IList<NHRMSJobcode> hrmsJobcode = new List<NHRMSJobcode>();
            try
            {
                OracleCommand oracleCommand = null;
                oracleCommand = new OracleCommand();
                oracleCommand.Connection = _oracleConnection;
                oracleCommand.CommandText = query;
                oracleCommand.CommandType = CommandType.Text;
                reader = oracleCommand.ExecuteReader();
                NHRMSJobcode nhrmsJobcode = null;
                while (reader.Read())
                {
                    try
                    {
                        nhrmsJobcode = new NHRMSJobcode();
                        nhrmsJobcode.EndpointSystemId = _endpoint.Id;
                        nhrmsJobcode.Code = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_171]);
                        nhrmsJobcode.Description = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_JOBCODE_DESC]);
                        nhrmsJobcode.IsActive = true;
                        hrmsJobcode.Add(nhrmsJobcode);
                    }
                    catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetJobcode", ex); }
                }

            }
            catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetJobcode", ex); }
            finally
            {
                reader.Close();
                reader.Dispose();
            }

            return hrmsJobcode;
        }
        IList<NHRMSCostCenter> GetCostCenter()
        {
            string query = CMessage.PeopleSoftHRMS_153;// CMessage.PeopleSoftHRMS_134;
            IDataReader reader = null;
            IList<NHRMSCostCenter> hrmsCostCenter = new List<NHRMSCostCenter>();
            try
            {
                OracleCommand oracleCommand = null;
                oracleCommand = new OracleCommand();
                oracleCommand.Connection = _oracleConnection;
                oracleCommand.CommandText = query;
                oracleCommand.CommandType = CommandType.Text;
                reader = oracleCommand.ExecuteReader();
                NHRMSCostCenter nhrmsCostCenter = null;
                while (reader.Read())
                {
                    try
                    {
                        nhrmsCostCenter = new NHRMSCostCenter();
                        nhrmsCostCenter.EndpointSystemId = _endpoint.Id;
                        nhrmsCostCenter.Code = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_172]);
                        nhrmsCostCenter.Description = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_COSTCENTER_DESC]);
                        nhrmsCostCenter.IsActive = true;
                        hrmsCostCenter.Add(nhrmsCostCenter);
                    }
                    catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetCostCenter", ex); }
                }

            }
            catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetCostCenter", ex); }
            finally
            {
                reader.Close();
                reader.Dispose();
            }

            return hrmsCostCenter;
        }
        private IList<NGrade> GetSalaryGrade()
        {
            string query = CMessage.PeopleSoftHRMS_153;// CMessage.PeopleSoftHRMS_139;
            IDataReader reader = null;
            IList<NGrade> ngrade = new List<NGrade>();
            try
            {
                OracleCommand oracleCommand = null;
                oracleCommand = new OracleCommand();
                oracleCommand.Connection = _oracleConnection;
                oracleCommand.CommandText = query;
                oracleCommand.CommandType = CommandType.Text;
                reader = oracleCommand.ExecuteReader();
                NGrade grade = null;
                while (reader.Read())
                {
                    try
                    {
                        grade = new NGrade();
                        grade.Grade = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_140]);
                        grade.Description = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_GRADE_DESC]);
                        //var parentOrgCode = NHRMSOrganisation.GetByOrgCode(CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_141]));
                        //grade.SetID = parentOrgCode != null ? parentOrgCode.Code : null;
                        ngrade.Add(grade);
                    }
                    catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetSalaryGrade", ex); }
                }
            }
            catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetSalaryGrade", ex); }
            finally
            {
                reader.Close();
                reader.Dispose();
            }

            return ngrade;
        }
        private IList<NPlan> GetSalaryPlan()
        {
            string query = CMessage.PeopleSoftHRMS_153;// CMessage.PeopleSoftHRMS_139;
            IDataReader reader = null;
            IList<NPlan> nplan = new List<NPlan>();
            try
            {
                OracleCommand oracleCommand = null;
                oracleCommand = new OracleCommand();
                oracleCommand.Connection = _oracleConnection;
                oracleCommand.CommandText = query;
                oracleCommand.CommandType = CommandType.Text;
                reader = oracleCommand.ExecuteReader();
                NPlan plan = null;
                while (reader.Read())
                {
                    try
                    {
                        plan = new NPlan();
                        plan.Plan = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_SAL_ADMIN_PLAN]);
                        plan.Description = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_PLAN_DESCR]);
                        //var parentOrgCode = NHRMSOrganisation.GetByOrgCode(CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_141]));
                        //grade.SetID = parentOrgCode != null ? parentOrgCode.Code : null;
                        nplan.Add(plan);
                    }
                    catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetSalaryPlan", ex); }
                }

            }
            catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetSalaryPlan", ex); }
            finally
            {
                reader.Close();
                reader.Dispose();
            }

            return nplan;
        }
        public void GetPeopleSoftSalaryGrade(string grade, string employeeADId, IList<PeopleSoftSalaryGrade> salaryGrade)
        {
            string query = string.Format(CMessage.PeopleSoftHRMS_204, grade);
            IDataReader reader = null;
            try
            {
                OracleCommand oracleCommand = null;
                oracleCommand = new OracleCommand();
                oracleCommand.Connection = _oracleConnection;
                oracleCommand.CommandText = query;
                oracleCommand.CommandType = CommandType.Text;
                reader = oracleCommand.ExecuteReader();
                PeopleSoftSalaryGrade peopleSoftSalaryGrade = null;
                while (reader.Read())
                {
                    try
                    {
                        peopleSoftSalaryGrade = new PeopleSoftSalaryGrade();
                        peopleSoftSalaryGrade.EndpointSystemId = _endpoint.Id;
                        peopleSoftSalaryGrade.EmployeeAD_Id = employeeADId;
                        peopleSoftSalaryGrade.SalaryGrade = grade;
                        salaryGrade.Add(peopleSoftSalaryGrade);
                    }
                    catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetPeopleSoftSalaryGrade", ex); }
                }
            }
            catch (Exception ex) { WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetPeopleSoftSalaryGrade", ex); }
            finally
            {
                reader.Close();
                reader.Dispose();
            }
        }
        IList<PeopleSoftHRMSUserInfo> GetHRMSUsers(string ViewQuery, bool isEffectiveDate, bool isPromotion, EmployeeEvents eventType)
        {
            // string query = CMessage.PeopleSoftHRMS_151;
            IDataReader reader = null;
            IList<PeopleSoftHRMSUserInfo> peoplesoftHRMSUser = new List<PeopleSoftHRMSUserInfo>();
            try
            {
                if (this.Connect())
                {
                    OracleCommand oracleCommand = null;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Connection = _oracleConnection;
                    oracleCommand.CommandText = ViewQuery;
                    oracleCommand.CommandType = CommandType.Text;
                    reader = oracleCommand.ExecuteReader();
                    PeopleSoftHRMSUserInfo pplSoftHRMSUser = null;
                    while (reader.Read())
                    {
                        try
                        {
                            pplSoftHRMSUser = new PeopleSoftHRMSUserInfo();
                            pplSoftHRMSUser.EndPointSystemId = _endpoint.Id;
                            //pplSoftHRMSUser.FirstName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_158]);
                            //pplSoftHRMSUser.MiddleName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_159]);
                            //pplSoftHRMSUser.LastName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_160]);
                            //pplSoftHRMSUser.PreferredName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_161]);
                            pplSoftHRMSUser.EmployeeId = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_162]);
                            // pplSoftHRMSUser.NRIC = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_163]);
                            pplSoftHRMSUser.EmployeeAD_Id = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_164]);
                            pplSoftHRMSUser.EmployeeType = EmployeeType.Permanent.ToString();
                            pplSoftHRMSUser.EmploymentType = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_EMPLNTTYPE]);
                            pplSoftHRMSUser.EmploymentTypeDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_EMPLNTYPE_DESC]);
                            //pplSoftHRMSUser.OrganizationId = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_166]);
                            //pplSoftHRMSUser.BusinessUnit = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_188]);
                            //pplSoftHRMSUser.CompanyCode = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_168]);
                            //pplSoftHRMSUser.SalaryPlan = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_169]);
                            //pplSoftHRMSUser.SalaryGrade = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_170]);
                            //pplSoftHRMSUser.Designation = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_DESIGNATION]);
                            //pplSoftHRMSUser.CostCenter = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_172]);
                            pplSoftHRMSUser.Email = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_173]);

                            // pplSoftHRMSUser.DOB = CheckNull.AgainstNullableDatetime(reader[CMessage.PeopleSoftHRMS_DOB]);
                            pplSoftHRMSUser.IdentificationNo = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_IDENTITY_NO]);
                            pplSoftHRMSUser.IdentificationType = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_IDENTITY_TYPE]);
                            pplSoftHRMSUser.IdentificationCountry = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_IDENTITY_COUNTRY]);
                            pplSoftHRMSUser.SuperVisorADID = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_SUPERVISOR_ADID]);
                            pplSoftHRMSUser.SuperVisorEMPId = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_SUPERVISOR_EMPID]);
                            //pplSoftHRMSUser.PositionCode = CheckNull.AgainstString(reader[CMessage.PEOPLESOFTHRMS_POSITIONCODE]);
                            pplSoftHRMSUser.LocationCode = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_Location_Code]);
                            pplSoftHRMSUser.LocationDesciption = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_Location_Desc]);
                            // pplSoftHRMSUser.EmployeeName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_157]);
                            pplSoftHRMSUser.EmploymentStatus = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_HR_STATUS]);
                            if (eventType != EmployeeEvents.None)
                            {
                                pplSoftHRMSUser.EffectiveDate = CheckNull.AgainstNullableDatetime(reader[CMessage.PeopleSoftHRMS_202]);
                            }
                            else
                            {
                                pplSoftHRMSUser.TerminationDate = CheckNull.AgainstNullableDatetime(reader[CMessage.PeopleSoftHRMS_225]);
                                pplSoftHRMSUser.JoinDate = CheckNull.AgainstDatetime(reader[CMessage.PeopleSoftHRMS_226]);
                                pplSoftHRMSUser.JoinDateString = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_226]);
                                pplSoftHRMSUser.TerminationDateString = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_225]);
                            }

                            //if (isPromotion == true)
                            //{
                            //    //PeopleSoftUserLookUp - Model
                            //    var nhrmsUser = NHRMSUser.GetByEmployeeId(pplSoftHRMSUser.EmployeeAD_Id, _endpoint.Id);
                            //    if (nhrmsUser != null)
                            //    {
                            //        GetPeopleSoftSalaryGrade(nhrmsUser.Grade, pplSoftHRMSUser.EmployeeAD_Id, _peopleSoftSalaryGrade);
                            //    }
                            //}
                            peoplesoftHRMSUser.Add(pplSoftHRMSUser);
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetHRMSUsers", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetHRMSUsers", ex);
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

            return peoplesoftHRMSUser;
        }
        private IList<PeopleSoftHRMSUserInfo> GetOneTimeHRMSUsers()
        {
            string filePath = GetConfiguration(EndpointKey.OneTimeUserListFilePath);

            try
            {
                string manadatoryCols = string.Empty;
                var dataTable = base.AuthenticateNetworkAndGetFileData(filePath, manadatoryCols);
                if (dataTable != null)
                {
                    var lstFuncs = (from table in dataTable.AsEnumerable()
                                    group table by new
                                    {
                                        EndPointSystemId = _endpoint.Id,
                                        EmployeeName = table.Field<string>(CQuery.NameDispaly),
                                        FirstName = table.Field<string>(CQuery.FirstName),
                                        MiddleName = table.Field<string>(CQuery.MiddleName),
                                        LastName = table.Field<string>(CQuery.LastName),
                                        DOB = table.Field<string>(CQuery.DOB),
                                        PreferredName = table.Field<string>(CQuery.PreferredName),
                                        EmployeeId = table.Field<string>(CQuery.EmployeeId),
                                        IdentificationNo = table.Field<string>(CQuery.IdentityNo),
                                        IdentificationType = table.Field<string>(CQuery.IdentityType),
                                        IdentificationCountry = table.Field<string>(CQuery.IdentityCountry),
                                        LocationCode = table.Field<string>(CQuery.LocationCode),
                                        LocationDescription = table.Field<string>(CQuery.LocationCodeDescr),
                                        EmployeeAD_Id = table.Field<string>(CQuery.ADID),
                                        EmployeeType = EmployeeType.Permanent.ToString(),
                                        EmploymentType = table.Field<string>(CQuery.EmploymenttType),
                                        EmploymentTypeDescription = table.Field<string>(CQuery.EmploymentTypeDescr),
                                        OrganizationId = table.Field<string>(CQuery.DeptId),
                                        DepartmentDescription = table.Field<string>(CQuery.DepartmentDesc),
                                        BusinessUnit = table.Field<string>(CQuery.BusinessUnit),
                                        BusinessUnitDescription = table.Field<string>(CQuery.BusinessUnitDesc),
                                        CompanyCode = table.Field<string>(CQuery.Company),
                                        CompanyDescription = table.Field<string>(CQuery.CompanyCodeDesc),
                                        SalaryPlan = table.Field<string>(CQuery.SalaryPlan),
                                        SalaryPlanDescription = table.Field<string>(CQuery.SalaryPlanDesc),
                                        SalaryGrade = table.Field<string>(CQuery.SalaryGrade),
                                        SalaryGradeDescription = table.Field<string>(CQuery.SalaryGradeDesc),
                                        Designation = table.Field<string>(CQuery.Designation),
                                        DesignationDescription = table.Field<string>(CQuery.DesignationDesc),
                                        Jobcode = table.Field<string>(CQuery.JobCode),
                                        JobcodeDescription = table.Field<string>(CQuery.JobCodeDescr),
                                        CostCenter = table.Field<string>(CQuery.CostCenter),
                                        CostCenterDescription = table.Field<string>(CQuery.CostCenterDesc),
                                        Email = table.Field<string>(CQuery.Email),
                                        SuperVisorADID = table.Field<string>(CQuery.SupervisorADID),
                                        SuperVisorEMPId = table.Field<string>(CQuery.SupervisorEmpID),
                                        JoinDate = table.Field<string>(CQuery.JoinDate),
                                        EmploymentStatus = table.Field<string>(CQuery.Status),
                                        TerminationDate = table.Field<string>(CQuery.TerminationDate),


                                    } into g
                                    select new PeopleSoftHRMSUserInfo
                                    {
                                        EndPointSystemId = g.Key.EndPointSystemId,
                                        EmployeeName = g.Key.EmployeeName,
                                        FirstName = g.Key.FirstName,
                                        MiddleName = g.Key.MiddleName,
                                        LastName = g.Key.LastName,
                                        PreferredName = g.Key.PreferredName,
                                        EmployeeId = g.Key.EmployeeId,
                                        EmployeeAD_Id = g.Key.EmployeeAD_Id,

                                        EmployeeType = g.Key.EmployeeType,
                                        EmploymentType = g.Key.EmploymentType,
                                        EmploymentTypeDescription = g.Key.EmploymentTypeDescription,
                                        OrganizationId = g.Key.OrganizationId,
                                        BusinessUnit = g.Key.BusinessUnit,
                                        CompanyCode = g.Key.CompanyCode,
                                        SalaryPlan = g.Key.SalaryPlan,
                                        SalaryGrade = g.Key.SalaryGrade,
                                        Designation = g.Key.Designation,
                                        Jobcode = g.Key.Jobcode,
                                        CostCenter = g.Key.CostCenter,
                                        Email = g.Key.Email,

                                        DOBString = g.Key.DOB,
                                        IdentificationNo = g.Key.IdentificationNo,
                                        IdentificationType = g.Key.IdentificationType,
                                        IdentificationCountry = g.Key.IdentificationCountry,
                                        SuperVisorADID = g.Key.SuperVisorADID,
                                        SuperVisorEMPId = g.Key.SuperVisorEMPId,
                                        LocationCode = g.Key.LocationCode,
                                        LocationDesciption = g.Key.LocationDescription,

                                        EmploymentStatus = g.Key.EmploymentStatus,
                                        TerminationDateString = g.Key.TerminationDate,
                                        JoinDateString = g.Key.JoinDate,

                                        CompanyDescription = g.Key.CompanyDescription,
                                        DepartmentDescription = g.Key.DepartmentDescription,
                                        BusinessUnitDescription = g.Key.BusinessUnitDescription,
                                        DesignationDescription = g.Key.DesignationDescription,
                                        JobcodeDescription = g.Key.JobcodeDescription,
                                        CostCenterDescription = g.Key.CostCenterDescription,
                                        SalaryGradeDescription = g.Key.SalaryGradeDescription,
                                        SalaryPlanDescription = g.Key.SalaryPlanDescription

                                    }).ToList();
                    return lstFuncs;
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetOneTimeHRMSUsers", ex);
            }
            finally
            {
            }

            return null;
        }
        private IList<PeopleSoftHRMSUserInfo> GetOnBoardUsers(string ViewQuery)
        {
            IDataReader reader = null;
            IList<PeopleSoftHRMSUserInfo> peoplesoftHRMSUser = new List<PeopleSoftHRMSUserInfo>();
            try
            {
                if (this.Connect())
                {
                    OracleCommand oracleCommand = null;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Connection = _oracleConnection;
                    oracleCommand.CommandText = ViewQuery;
                    oracleCommand.CommandType = CommandType.Text;
                    reader = oracleCommand.ExecuteReader();
                    PeopleSoftHRMSUserInfo pplSoftHRMSUser = null;
                    while (reader.Read())
                    {
                        try
                        {
                            pplSoftHRMSUser = new PeopleSoftHRMSUserInfo();
                            pplSoftHRMSUser.EndPointSystemId = _endpoint.Id;
                            pplSoftHRMSUser.EmployeeName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_157]);
                            pplSoftHRMSUser.FirstName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_158]);
                            pplSoftHRMSUser.MiddleName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_159]);
                            pplSoftHRMSUser.LastName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_160]);
                            pplSoftHRMSUser.PreferredName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_161]);
                            pplSoftHRMSUser.EmployeeId = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_162]);
                            pplSoftHRMSUser.EmployeeAD_Id = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_164]);
                            pplSoftHRMSUser.EmployeeType = EmployeeType.Permanent.ToString();
                            pplSoftHRMSUser.EmploymentType = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_EMPLNTTYPE]);
                            pplSoftHRMSUser.EmploymentTypeDescription = CheckNull.AgainstString(reader[CMessage.RWSEMPLCLSDESC]);

                            pplSoftHRMSUser.OrganizationId = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_166]);
                            pplSoftHRMSUser.BusinessUnit = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_188]);
                            pplSoftHRMSUser.CompanyCode = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_168]);
                            pplSoftHRMSUser.SalaryPlan = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_169]);
                            pplSoftHRMSUser.SalaryGrade = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_170]);
                            pplSoftHRMSUser.Designation = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_DESIGNATION]);
                            pplSoftHRMSUser.Jobcode = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_171]);
                            pplSoftHRMSUser.CostCenter = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_172]);
                            pplSoftHRMSUser.Email = CheckNull.AgainstString(reader[CMessage.EMAILADDR]);

                            pplSoftHRMSUser.DOB = Convert.ToDateTime((reader[CMessage.PeopleSoftHRMS_DOB]));
                            pplSoftHRMSUser.IdentificationNo = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_IDENTITY_NO]);
                            pplSoftHRMSUser.IdentificationType = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_IDENTITY_TYPE]);
                            pplSoftHRMSUser.IdentificationCountry = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_IDENTITY_COUNTRY]);
                            pplSoftHRMSUser.SuperVisorADID = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_SUPERVISOR_ADID]);
                            pplSoftHRMSUser.SuperVisorEMPId = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_SUPERVISOR_EMPID]);
                            // pplSoftHRMSUser.PositionCode = CheckNull.AgainstString(reader[CMessage.PEOPLESOFTHRMS_POSITIONCODE]);
                            pplSoftHRMSUser.LocationCode = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_Location_Code]);
                            pplSoftHRMSUser.LocationDesciption = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_Location_Desc]);
                            pplSoftHRMSUser.EffectiveDate = Convert.ToDateTime((reader[CMessage.PeopleSoftHRMS_202]));
                            pplSoftHRMSUser.JoinDate = Convert.ToDateTime((reader[CMessage.PeopleSoftHRMS_202]));

                            pplSoftHRMSUser.CompanyDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_COMPANY_DESC]);
                            pplSoftHRMSUser.DepartmentDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_DEPT_DESC]);
                            pplSoftHRMSUser.BusinessUnitDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_BUSINESSUNIT_DESC]);
                            pplSoftHRMSUser.DesignationDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_DESIGN_DESC]);
                            pplSoftHRMSUser.JobcodeDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_JOBCODE_DESC]);
                            pplSoftHRMSUser.CostCenterDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_COSTCENTER_DESC]);
                            pplSoftHRMSUser.SalaryGradeDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_GRADE_DESC]);
                            pplSoftHRMSUser.SalaryPlanDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_PLAN_DESCR]);

                            peoplesoftHRMSUser.Add(pplSoftHRMSUser);

                            // WriteDiscoveryLog(LogLevelType.Info, string.Format("@record - Effective Date {0}, User {1}", Convert.ToString(reader[CMessage.PeopleSoftHRMS_202]), JsonConvert.SerializeObject(pplSoftHRMSUser).ToString()));

                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetOnBoardUsers", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isErrOnboard = true;
                WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetOnBoardUsers", ex);
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
            return peoplesoftHRMSUser;
        }
        List<PeopleSoftHRMSUserInfo> GetOffBoardUsers(string ViewQuery)
        {
            IDataReader reader = null;
            IList<PeopleSoftHRMSUserInfo> peopleSoftUsers = new List<PeopleSoftHRMSUserInfo>();
            try
            {
                if (this.Connect())
                {
                    OracleCommand oracleCommand = null;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Connection = _oracleConnection;
                    oracleCommand.CommandText = ViewQuery;
                    oracleCommand.CommandType = CommandType.Text;
                    reader = oracleCommand.ExecuteReader();
                    PeopleSoftHRMSUserInfo pplSoftHRMSUser = null;
                    while (reader.Read())
                    {
                        try
                        {
                            pplSoftHRMSUser = new PeopleSoftHRMSUserInfo();
                            var ADID = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_164]);
                            var empID = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_162]);
                            if (string.IsNullOrEmpty(ADID))
                            {
                                WriteDiscoveryLog(LogLevelType.Warning, string.Format("Discover OffBoard Users, ADID NULL/BLANK - EMPLID:{0} ", empID), null, true);
                                continue;
                            }


                            pplSoftHRMSUser = PeopleSoftHRMSUserInfo.GetByEmployeeADIdEndpointSystemId(ADID, _endpoint.Id);
                            if (pplSoftHRMSUser != null && pplSoftHRMSUser.EmployeeId == empID)
                            {
                                pplSoftHRMSUser.Email = CheckNull.AgainstString(reader[CMessage.EMAILADDR]);
                                pplSoftHRMSUser.EffectiveDate = CheckNull.AgainstNullableDatetime(reader[CMessage.PeopleSoftHRMS_LASTDAYATWORK]);
                                peopleSoftUsers.Add(pplSoftHRMSUser);

                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetOffBoardUsers", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isErrOffboard = true;
                WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetOffBoardUsers", ex);
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

            return peopleSoftUsers.ToList();
        }
        IList<PeopleSoftHRMSUserInfo> GetTransferUsers(string ViewQuery)
        {
            IDataReader reader = null;
            IList<PeopleSoftHRMSUserInfo> peopleSoftUsers = new List<PeopleSoftHRMSUserInfo>();
            try
            {
                if (this.Connect())
                {
                    OracleCommand oracleCommand = null;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Connection = _oracleConnection;
                    oracleCommand.CommandText = ViewQuery;
                    oracleCommand.CommandType = CommandType.Text;
                    reader = oracleCommand.ExecuteReader();
                    PeopleSoftHRMSUserInfo pplSoftHRMSUser = null;
                    while (reader.Read())
                    {
                        try
                        {
                            pplSoftHRMSUser = new PeopleSoftHRMSUserInfo();
                            var ADID = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_164]);
                            var empID = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_162]);
                            if (string.IsNullOrEmpty(ADID) && string.IsNullOrEmpty(empID))
                            {
                                WriteDiscoveryLog(LogLevelType.Warning, string.Format("Discover Transfer Users, ADID NULL/BLANK - EMPLID:{0} ", empID), null, true);
                                continue;
                            }
                            pplSoftHRMSUser = PeopleSoftHRMSUserInfo.GetByEmployeeADIdEndpointSystemId(ADID, _endpoint.Id);

                            if (pplSoftHRMSUser != null && pplSoftHRMSUser.EmployeeId == empID)
                            {
                                pplSoftHRMSUser.Email = CheckNull.AgainstString(reader[CMessage.EMAILADDR]);
                                pplSoftHRMSUser.EmployeeType = EmployeeType.Permanent.ToString();
                                pplSoftHRMSUser.EmploymentType = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_EMPLNTTYPE]);
                                pplSoftHRMSUser.EmploymentTypeDescription = CheckNull.AgainstString(reader[CMessage.RWSEMPLCLSDESC]);
                                pplSoftHRMSUser.OrganizationId = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_166]);
                                pplSoftHRMSUser.BusinessUnit = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_188]);
                                pplSoftHRMSUser.CompanyCode = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_168]);
                                pplSoftHRMSUser.CostCenter = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_172]);
                                pplSoftHRMSUser.SuperVisorADID = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_SUPERVISOR_ADID]);
                                pplSoftHRMSUser.SuperVisorEMPId = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_SUPERVISOR_EMPID]);


                                pplSoftHRMSUser.CompanyDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_COMPANY_DESC]);
                                pplSoftHRMSUser.DepartmentDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_DEPT_DESC]);
                                pplSoftHRMSUser.BusinessUnitDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_BUSINESSUNIT_DESC]);
                                pplSoftHRMSUser.CostCenterDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_COSTCENTER_DESC]);


                                pplSoftHRMSUser.EffectiveDate = CheckNull.AgainstNullableDatetime(reader[CMessage.PeopleSoftHRMS_202]);
                                peopleSoftUsers.Add(pplSoftHRMSUser);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetTransferUsers", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isErrTransfer = true;
                WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetTransferUsers", ex);
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
            return peopleSoftUsers;
        }
        IList<PeopleSoftHRMSUserInfo> GetPromotionUsers(string ViewQuery)
        {
            IDataReader reader = null;
            IList<PeopleSoftHRMSUserInfo> peopleSoftUsers = new List<PeopleSoftHRMSUserInfo>();
            try
            {
                if (this.Connect())
                {
                    OracleCommand oracleCommand = null;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Connection = _oracleConnection;
                    oracleCommand.CommandText = ViewQuery;
                    oracleCommand.CommandType = CommandType.Text;
                    reader = oracleCommand.ExecuteReader();
                    PeopleSoftHRMSUserInfo pplSoftHRMSUser = null;
                    while (reader.Read())
                    {
                        try
                        {

                            pplSoftHRMSUser = new PeopleSoftHRMSUserInfo();
                            var ADID = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_164]);
                            var empID = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_162]);

                            if (string.IsNullOrEmpty(ADID))
                            {
                                WriteDiscoveryLog(LogLevelType.Warning, string.Format("Discover Promotion Users, ADID NULL/BLANK - EMPLID:{0} ", empID), null, true);
                                continue;
                            }

                            pplSoftHRMSUser = PeopleSoftHRMSUserInfo.GetByEmployeeADIdEndpointSystemId(ADID, _endpoint.Id);

                            if (pplSoftHRMSUser != null && pplSoftHRMSUser.EmployeeId == empID)
                            {
                                pplSoftHRMSUser.Email = CheckNull.AgainstString(reader[CMessage.EMAILADDR]);
                                pplSoftHRMSUser.EmployeeType = EmployeeType.Permanent.ToString();
                                pplSoftHRMSUser.EmploymentType = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_EMPLNTTYPE]);
                                pplSoftHRMSUser.EmploymentTypeDescription = CheckNull.AgainstString(reader[CMessage.RWSEMPLCLSDESC]);
                                pplSoftHRMSUser.SalaryPlan = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_169]);
                                pplSoftHRMSUser.SalaryGrade = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_170]);
                                pplSoftHRMSUser.Designation = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_DESIGNATION]);
                                pplSoftHRMSUser.Jobcode = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_171]);
                                pplSoftHRMSUser.SuperVisorADID = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_SUPERVISOR_ADID]);
                                pplSoftHRMSUser.SuperVisorEMPId = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_SUPERVISOR_EMPID]);

                                pplSoftHRMSUser.DesignationDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_DESIGN_DESC]);
                                pplSoftHRMSUser.JobcodeDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_JOBCODE_DESC]);
                                pplSoftHRMSUser.SalaryGradeDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_GRADE_DESC]);
                                pplSoftHRMSUser.SalaryPlanDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_PLAN_DESCR]);

                                pplSoftHRMSUser.EffectiveDate = CheckNull.AgainstNullableDatetime(reader[CMessage.PeopleSoftHRMS_202]);
                                peopleSoftUsers.Add(pplSoftHRMSUser);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetPromotionUsers", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isErrPromotion = true;
                WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetPromotionUsers", ex);
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

            return peopleSoftUsers;
        }
        IList<PeopleSoftHRMSUserInfo> GetOnboardTrigger()
        {
            string query = CMessage.PeopleSoftHRMS_153;
            IDataReader reader = null;
            IList<PeopleSoftHRMSUserInfo> peoplesoftHRMSUser = new List<PeopleSoftHRMSUserInfo>();
            try
            {
                OracleCommand oracleCommand = null;
                oracleCommand = new OracleCommand();
                oracleCommand.Connection = _oracleConnection;
                oracleCommand.CommandText = query;
                oracleCommand.CommandType = CommandType.Text;
                reader = oracleCommand.ExecuteReader();
                PeopleSoftHRMSUserInfo pplSoftHRMSUser = null;

                while (reader.Read())
                {
                    try
                    {
                        pplSoftHRMSUser = new PeopleSoftHRMSUserInfo();
                        pplSoftHRMSUser.EndPointSystemId = _endpoint.Id;
                        pplSoftHRMSUser.EmployeeName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_157]);
                        pplSoftHRMSUser.FirstName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_158]);
                        pplSoftHRMSUser.MiddleName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_159]);
                        pplSoftHRMSUser.LastName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_160]);
                        pplSoftHRMSUser.PreferredName = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_161]);
                        pplSoftHRMSUser.EmployeeId = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_162]);
                        // pplSoftHRMSUser.NRIC = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_163]);
                        pplSoftHRMSUser.EmployeeAD_Id = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_164]);
                        pplSoftHRMSUser.EmployeeType = EmployeeType.Permanent.ToString();
                        pplSoftHRMSUser.EmploymentType = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_EMPLNTTYPE]);
                        pplSoftHRMSUser.EmploymentTypeDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_EMPLNTYPE_DESC]);
                        pplSoftHRMSUser.OrganizationId = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_166]);
                        pplSoftHRMSUser.BusinessUnit = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_188]);
                        pplSoftHRMSUser.CompanyCode = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_168]);
                        pplSoftHRMSUser.SalaryPlan = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_169]);
                        pplSoftHRMSUser.SalaryGrade = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_170]);
                        pplSoftHRMSUser.Designation = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_DESIGNATION]);
                        pplSoftHRMSUser.Jobcode = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_171]);
                        pplSoftHRMSUser.CostCenter = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_172]);
                        pplSoftHRMSUser.Email = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_173]);
                        pplSoftHRMSUser.EffectiveDate = CheckNull.AgainstNullableDatetime(reader[CMessage.PeopleSoftHRMS_202]);

                        pplSoftHRMSUser.DOB = CheckNull.AgainstNullableDatetime(reader[CMessage.PeopleSoftHRMS_DOB]);
                        pplSoftHRMSUser.IdentificationNo = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_IDENTITY_NO]);
                        pplSoftHRMSUser.IdentificationType = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_IDENTITY_TYPE]);
                        pplSoftHRMSUser.IdentificationCountry = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_IDENTITY_COUNTRY]);
                        pplSoftHRMSUser.SuperVisorADID = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_SUPERVISOR_ADID]);
                        pplSoftHRMSUser.SuperVisorEMPId = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_SUPERVISOR_EMPID]);
                        pplSoftHRMSUser.PositionCode = CheckNull.AgainstString(reader[CMessage.PEOPLESOFTHRMS_POSITIONCODE]);
                        pplSoftHRMSUser.LocationCode = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_Location_Code]);
                        pplSoftHRMSUser.LocationDesciption = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_Location_Desc]);
                        pplSoftHRMSUser.JoinDate = CheckNull.AgainstDatetime(reader[CMessage.PeopleSoftHRMS_202]);

                        peoplesoftHRMSUser.Add(pplSoftHRMSUser);
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetOnboardTrigger", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                isErrOnboard = true;
                WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetOnboardTrigger", ex);
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
            return peoplesoftHRMSUser;
        }
        IList<PeopleSoftHRMSCSEL> GetPeopleSoftHRMSCSEL()
        {
            string query = CMessage.PeopleSoftHRMS_156;
            IDataReader reader = null;
            IList<PeopleSoftHRMSCSEL> peopleSoftHRMSCSEL = new List<PeopleSoftHRMSCSEL>();
            try
            {
                if (this.Connect())
                {
                    OracleCommand oracleCommand = null;
                    oracleCommand = new OracleCommand();
                    oracleCommand.Connection = _oracleConnection;
                    oracleCommand.CommandText = query;
                    oracleCommand.CommandType = CommandType.Text;
                    reader = oracleCommand.ExecuteReader();
                    PeopleSoftHRMSCSEL peoplesoftHRMSCSEL = null;
                    while (reader.Read())
                    {
                        try
                        {
                            peoplesoftHRMSCSEL = new PeopleSoftHRMSCSEL();
                            peoplesoftHRMSCSEL.EmployeeId = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_EMPLOYEE_ID]);
                            peoplesoftHRMSCSEL.CSELLicNo = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_CSEL_LICNO]);
                            peoplesoftHRMSCSEL.CSELDescription = CheckNull.AgainstString(reader[CMessage.PeopleSoftHRMS_CSEL_DESC]);
                            peoplesoftHRMSCSEL.StartDate = CheckNull.AgainstNullableDatetime(reader[CMessage.PeopleSoftHRMS_START_DATE]);
                            peoplesoftHRMSCSEL.ExpiryDate = CheckNull.AgainstNullableDatetime(reader[CMessage.PeopleSoftHRMS_EXPIRY_DATE]);
                            peopleSoftHRMSCSEL.Add(peoplesoftHRMSCSEL);
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, "Exception @record - GetPeopleSoftHRMSCSEL", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Exception @GetPeopleSoftHRMSCSEL", ex);
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

            return peopleSoftHRMSCSEL;
        }
        private void Withdrawn()
        {
            try
            {
                if (isOnBoardSelected && !isErrOnboard)
                {
                    WithDrawnByEvent(EmployeeEvents.NewJoinee, AlertCenterNotificationType.OnBoardWithDrawn);
                    WithDrawnByEvent(EmployeeEvents.Rehire, AlertCenterNotificationType.RehireWithDrawn);
                }

                if (isOffBoardROSSelected && isOffBoardSelected && !isErrOffboard)
                {
                    WithDrawnByEvent(EmployeeEvents.Resignation, AlertCenterNotificationType.OffBoardWithDrawn);
                }

                if (isTransferSelected && !isErrTransfer)
                {
                    WithDrawnByEvent(EmployeeEvents.OUChange, AlertCenterNotificationType.TransferWithDrawn);
                }

                if (isPromotionSelected && !isErrPromotion)
                {
                    WithDrawnByEvent(EmployeeEvents.Promotion, AlertCenterNotificationType.PromotionWithDrawn);
                    WithDrawnByEvent(EmployeeEvents.Demotion, AlertCenterNotificationType.DemotionWithDrawn);
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Exception @Withdrawn", ex);
            }
        }
        private void WithDrawnByEvent(EmployeeEvents eventType, AlertCenterNotificationType alertType)
        {
            try
            {
                var currentUser = MasterConfigHelper.ServiceUser;
                var configEventType = eventType;
                if (eventType == EmployeeEvents.Demotion)
                    configEventType = EmployeeEvents.Promotion;
                else if (eventType == EmployeeEvents.Rehire)
                    configEventType = EmployeeEvents.NewJoinee;
                int backdatedViewperiodDays = Convert.ToInt32(ConfigurationMaster.GetByConfigKey(configEventType.ToString() + "ViewBackDatedWindowPeriodDays", Product.IM).ConfigValue);
                int futuredatedViewperiodDays = Convert.ToInt32(ConfigurationMaster.GetByConfigKey(configEventType.ToString() + "ViewFutureWindowperiodDays", Product.IM).ConfigValue);

                PeopleSoftHRMSUserInfo user = null;
                List<AlertCenterNotification> saveAlertList = null;
                DateTime userListBackdatedWindowperiod = DateTime.Now.Date.Subtract(TimeSpan.FromDays(backdatedViewperiodDays));
                DateTime userListFutureWindowperiod = DateTime.Now.Date.Add(TimeSpan.FromDays(futuredatedViewperiodDays));

                var userEvents = NHRMSUserEvent.GetAll().Where(o => o.EmployeeEvent == eventType &&
                (o.CreatedDate.Date <= userListFutureWindowperiod.Date && o.CreatedDate.Date >= userListBackdatedWindowperiod.Date));

                foreach (var userEvent in userEvents)
                {
                    var usr = NHRMSUser.GetById(userEvent.UserId);
                    user = null;
                    switch (userEvent.EmployeeEvent)
                    {
                        case EmployeeEvents.NewJoinee:
                        case EmployeeEvents.Rehire:
                            if (_onboardTriggerView == null || _onboardTriggerView.Count() == 0)
                                continue;
                            user = _onboardTriggerView.Where(u => (u.IdentificationNo == usr.IdentificationNo || u.EmployeeAD_Id == usr.ADID) && u.EffectiveDate <= userListFutureWindowperiod && u.EffectiveDate >= userListBackdatedWindowperiod).FirstOrDefault();
                            break;
                        case EmployeeEvents.Resignation:
                            if (_offboardTriggerView == null || _offboardTriggerView.Count() == 0)
                                continue;
                            user = _offboardTriggerView.Where(u => (u.IdentificationNo == usr.IdentificationNo || u.EmployeeAD_Id == usr.ADID) && u.EffectiveDate <= userListFutureWindowperiod && u.EffectiveDate >= userListBackdatedWindowperiod).FirstOrDefault();
                            break;
                        case EmployeeEvents.Promotion:
                        case EmployeeEvents.Demotion:
                            if (_promodemoTriggerView == null || _promodemoTriggerView.Count() == 0)
                                continue;
                            user = _promodemoTriggerView.Where(u => (u.IdentificationNo == usr.IdentificationNo || u.EmployeeAD_Id == usr.ADID) && u.EffectiveDate <= userListFutureWindowperiod && u.EffectiveDate >= userListBackdatedWindowperiod).FirstOrDefault();
                            break;
                        case EmployeeEvents.OUChange:
                            if (_ouTransferTriggerView == null || _ouTransferTriggerView.Count() == 0)
                                continue;
                            user = _ouTransferTriggerView.Where(u => (u.IdentificationNo == usr.IdentificationNo || u.EmployeeAD_Id == usr.ADID) && u.EffectiveDate <= userListFutureWindowperiod && u.EffectiveDate >= userListBackdatedWindowperiod).FirstOrDefault();
                            break;
                    }

                    if (user == null)
                    {
                        saveAlertList = new List<AlertCenterNotification>();
                        AlertCenterNotification alert = null;
                        List<AlertCenterNotification> alertList = AlertCenterNotification.GetAlertCenterNotificationsByType(alertType);

                        AlertCenterNotification existingAlert = alertList.Where(e => (e.AlertCenterNotificationType == alertType) &&
                        (e.Remarks.Contains(usr.EmployeeId) || e.Remarks.Contains(usr.ADID) || e.Remarks.Contains(usr.IdentificationNo)) &&
                        (e.CreatedDate.Date <= userListFutureWindowperiod.Date && e.CreatedDate.Date >= userListBackdatedWindowperiod.Date) &&
                        e.EndpointSystemId == _endpoint.Id).FirstOrDefault();

                        if (existingAlert == null)
                        {
                            string remarks = string.Format("IDAS discovered {4} event. User Id: {0}, AD-ID: {1}, Identification Number: {2}, Event : {3} ", usr.EmployeeId, usr.ADID, usr.IdentificationNo, userEvent.EmployeeEvent.ToString(), alertType.ToString());
                            alert = new AlertCenterNotification()
                            {
                                AlertCenterNotificationStatus = AlertCenterNotificationStatus.Pending,
                                AlertCenterNotificationType = alertType,
                                CreatedBy = currentUser,
                                CreatedDate = DateTime.Today,
                                EndpointSystemId = _endpoint.Id,
                                EntityId = null,
                                Remarks = remarks
                            };

                            saveAlertList.Add(alert);
                        }

                        userEvent.IsEventProcessed = true;
                        EmployeeEventTrigger trigger = EmployeeEventTrigger.GetTriggerByEventId(userEvent.Id);


                        using (Transaction.DataAccessTransaction datatransaction = new Transaction.DataAccessTransaction("System"))
                        {
                            if (trigger != null)
                            {
                                trigger.IsActive = false;
                                trigger.EffectiveDate = Convert.ToDateTime("01-01-1900");
                                trigger.IsRequestCreated = true;
                                trigger.Save(currentUser);
                            }

                            AlertCenterNotification.SaveList(saveAlertList, currentUser);

                            userEvent.IsEventProcessed = true;
                            userEvent.EffectiveDate = Convert.ToDateTime("01-01-1900");
                            userEvent.Save(currentUser);

                            if (trigger != null)
                            {
                                if (trigger.EventType == EmployeeEvents.Resignation)
                                {
                                    var employeeTriggerList = EmployeeEventTrigger.GetByUserIdTrigger(trigger.UserId);

                                    var resignation2 = employeeTriggerList.Where(item => item.UserId == trigger.UserId &&
                                                        item.EventType == EmployeeEvents.Resignation2 &&
                                                        item.IsRequestCreated == false && item.IsActive).FirstOrDefault();
                                    if (resignation2 != null)
                                    {
                                        resignation2.IsActive = false;
                                        resignation2.EffectiveDate = Convert.ToDateTime("01-01-1900");
                                        resignation2.IsRequestCreated = true;
                                        resignation2.Save(currentUser);
                                    }

                                    var resignation3 = employeeTriggerList.Where(item => item.UserId == trigger.UserId &&
                                                       item.EventType == EmployeeEvents.Resignation3 &&
                                                       item.IsRequestCreated == false && item.IsActive).FirstOrDefault();
                                    if (resignation3 != null)
                                    {
                                        resignation3.IsActive = false;
                                        resignation3.EffectiveDate = Convert.ToDateTime("01-01-1900");
                                        resignation3.IsRequestCreated = true;
                                        resignation3.Save(currentUser);
                                    }
                                }
                            }
                            datatransaction.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Exception @WithDrawnByEvent", ex);
            }
        }
        #endregion
        private void ProcessUserMetaData(List<PeopleSoftHRMSUserInfo> nUserList)
        {

            //Comapny
            var companyList = nUserList.GroupBy(g => new { g.CompanyCode, g.CompanyDescription }).
                Select(s => new NHRMSOrganisation()
                {
                    Code = s.Key.CompanyCode,
                    Description = string.IsNullOrEmpty(s.Key.CompanyDescription) ? s.Key.CompanyCode : s.Key.CompanyDescription,
                    Name = string.IsNullOrEmpty(s.Key.CompanyDescription) ? s.Key.CompanyCode : s.Key.CompanyDescription,
                    EndpointSystemId = _endpoint.Id,
                    IsActive = true,
                    Scope = CMessage.PeopleSoftHRMS_125,
                    ParentOrgCode = null
                }).ToList();

            SaveNHRMSOrganisation(companyList, _endpoint.Id, MasterConfigHelper.ServiceUser, CMessage.PeopleSoftHRMS_125);

            //Departments
            var departmentList = nUserList.GroupBy(g => new { g.OrganizationId, g.DepartmentDescription }).
                   Select(s => new NHRMSOrganisation()
                   {
                       Code = s.Key.OrganizationId,
                       Description = string.IsNullOrEmpty(s.Key.DepartmentDescription) ? s.Key.OrganizationId : s.Key.DepartmentDescription,
                       Name = string.IsNullOrEmpty(s.Key.DepartmentDescription) ? s.Key.OrganizationId : s.Key.DepartmentDescription,
                       EndpointSystemId = _endpoint.Id,
                       IsActive = true,
                       Scope = CMessage.PeopleSoftHRMS_127,
                       ParentOrgCode = null
                   }).ToList();

            SaveNHRMSOrganisation(departmentList, _endpoint.Id, MasterConfigHelper.ServiceUser, CMessage.PeopleSoftHRMS_127);

            //BusinessUnit
            var buisnessUnitList = nUserList.GroupBy(g => new { g.BusinessUnit, g.BusinessUnitDescription }).
                   Select(s => new NHRMSOrganisation()
                   {
                       Code = s.Key.BusinessUnit,
                       Description = string.IsNullOrEmpty(s.Key.BusinessUnitDescription) ? s.Key.BusinessUnit : s.Key.BusinessUnitDescription,
                       Name = string.IsNullOrEmpty(s.Key.BusinessUnitDescription) ? s.Key.BusinessUnit : s.Key.BusinessUnitDescription,
                       EndpointSystemId = _endpoint.Id,
                       IsActive = true,
                       Scope = CMessage.PeopleSoftHRMS_129,
                       ParentOrgCode = null
                   }).ToList();

            SaveNHRMSOrganisation(buisnessUnitList, _endpoint.Id, MasterConfigHelper.ServiceUser, CMessage.PeopleSoftHRMS_129);

            //Location
            var locationList = nUserList.GroupBy(g => new { g.LocationCode, g.LocationDesciption }).
                Select(s => new NHRMSOrganisation()
                {
                    Code = s.Key.LocationCode,
                    Description = string.IsNullOrEmpty(s.Key.LocationDesciption) ? s.Key.LocationCode : s.Key.LocationDesciption,
                    Name = string.IsNullOrEmpty(s.Key.LocationDesciption) ? s.Key.LocationCode : s.Key.LocationDesciption,
                    EndpointSystemId = _endpoint.Id,
                    IsActive = true,
                    Scope = CMessage.PeopleSoftHRMS_130,
                    ParentOrgCode = null
                }).ToList();

            SaveNHRMSOrganisation(locationList, _endpoint.Id, MasterConfigHelper.ServiceUser, CMessage.PeopleSoftHRMS_130);

            //Designation
            var designationList = nUserList.GroupBy(g => new { g.Designation, g.DesignationDescription }).
              Select(s => new NHRMSDesignation()
              {
                  Code = s.Key.Designation,
                  Description = string.IsNullOrEmpty(s.Key.DesignationDescription) ? s.Key.Designation : s.Key.DesignationDescription,
                  EndpointSystemId = _endpoint.Id,
                  IsActive = true,
              }).ToList();

            SaveNHRMSDesignation(designationList, _endpoint.Id, MasterConfigHelper.ServiceUser);

            //JobCode

            var jobCodeList = nUserList.GroupBy(g => new { g.Jobcode, g.JobcodeDescription }).
               Select(s => new NHRMSJobcode()
               {
                   Code = s.Key.Jobcode,
                   Description = string.IsNullOrEmpty(s.Key.JobcodeDescription) ? s.Key.Jobcode : s.Key.JobcodeDescription,
                   EndpointSystemId = _endpoint.Id,
                   IsActive = true,
               }).ToList();

            SaveNHRMSJobcode(jobCodeList, _endpoint.Id, MasterConfigHelper.ServiceUser);

            //CostCenter
            var costCenterList = nUserList.GroupBy(g => new { g.CostCenter, g.CostCenterDescription }).
                Select(s => new NHRMSCostCenter()
                {
                    Code = s.Key.CostCenter,
                    Description = string.IsNullOrEmpty(s.Key.CostCenterDescription) ? s.Key.CostCenter : s.Key.CostCenterDescription,
                    EndpointSystemId = _endpoint.Id,
                    IsActive = true,
                }).ToList();

            SaveNHRMSCostCenter(costCenterList, _endpoint.Id, MasterConfigHelper.ServiceUser);


            //Grade
            var gradeList = nUserList.GroupBy(g => new { g.SalaryGrade, g.SalaryGradeDescription }).
              Select(s => new NGrade()
              {
                  Grade = s.Key.SalaryGrade,
                  Description = string.IsNullOrEmpty(s.Key.SalaryGradeDescription) ? s.Key.SalaryGrade : s.Key.SalaryGradeDescription,
              }).ToList();

            SaveNGrade(gradeList, _endpoint.Id, MasterConfigHelper.ServiceUser);

            //Grade
            var salaryplanList = nUserList.GroupBy(g => new { g.SalaryPlan, g.SalaryPlanDescription }).
              Select(s => new NPlan()
              {
                  Plan = s.Key.SalaryPlan,
                  Description = string.IsNullOrEmpty(s.Key.SalaryPlanDescription) ? s.Key.SalaryPlan : s.Key.SalaryPlanDescription,
              }).ToList();

            SaveNPlan(salaryplanList, _endpoint.Id, MasterConfigHelper.ServiceUser);

        }
    }
}

