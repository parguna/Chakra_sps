
using DI.Chakra.Connector.SAPBase;
using DI.Chakra.DomainModel;
using DI.Chakra.DomainModel.UserHelper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using SAP.Connector;
using SAP.Middleware.Connector;
using SAPUserDetailsComponent;
using SAPUserGrp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.Connector
{
    public class SAPHRMSConnector : ConnectorBase
    {
        const string emptyOrgID = "00000000";
        readonly IdmsLogger _idmsLogger;
        readonly bool canRunPropagationTrigger;
        EndpointConfiguration SAPConfiguration { get; set; }
        DateTime discoveryStartDate { get; set; }

        string today = DateTime.Today.ToString("yyyyMMdd");
        string deltaDate = string.Empty;
        // readonly bool CanReadNRIC;
        List<EmployeeEventTrigger> triggers = new List<EmployeeEventTrigger>();

        #region DataTables

        DataTable m_dtPA0000AllEmp;

        DataTable m_dtPA0000;
        DataTable m_dtPA0001;
        DataTable m_dtPA0002;
        DataTable m_dtPA0006;
        DataTable m_dtPA0185;
        DataTable m_dtPA0077;
        DataTable m_dtPA0105;
        DataTable m_dtPA0027;
        DataTable m_dtPA0302;

        DataTable m_dtT001;
        DataTable m_dtT500P;
        DataTable m_dtT001P;
        DataTable m_dtCSKS;
        DataTable m_dtCSKT;
        DataTable m_dtHRP1000;
        DataTable m_dtHRP1001;

        #endregion

        #region Fields

        RFC_DB_FLD FLD_PERNR;
        RFC_DB_FLD FLD_ENDDA;
        RFC_DB_FLD FLD_VORNA;
        RFC_DB_FLD FLD_TITEL;
        RFC_DB_FLD FLD_NATIO;
        RFC_DB_FLD FLD_NACHN;
        RFC_DB_FLD FLD_MIDNM;
        RFC_DB_FLD FLD_RUFNM;
        RFC_DB_FLD FLD_BEGDA;
        RFC_DB_FLD FLD_ICNUM;
        RFC_DB_FLD FLD_RACKY;
        RFC_DB_FLD FLD_MASSN;
        RFC_DB_FLD FLD_STAT2;
        RFC_DB_FLD FLD_ENAME;
        RFC_DB_FLD FLD_KOSTL;
        RFC_DB_FLD FLD_BUKRS;
        RFC_DB_FLD FLD_WERKS;
        RFC_DB_FLD FLD_BTRTL;
        RFC_DB_FLD FLD_ORGEH;
        RFC_DB_FLD FLD_PLANS;
        RFC_DB_FLD FLD_PERSG;
        RFC_DB_FLD FLD_USRTY;
        RFC_DB_FLD FLD_USRID_LONG;
        RFC_DB_FLD FLD_USRID;
        RFC_DB_FLD FLD_KST01;
        RFC_DB_FLD FLD_KSTAR;
        RFC_DB_FLD FLD_AEDTM;
        RFC_DB_FLD FLD_PSTLZ;
        RFC_DB_FLD FLD_SUBTY;
        RFC_DB_FLD FLD_MASSG;

        #endregion
        List<string> PropagationFields { get; set; }
        readonly string MasterConfigHelperServiceUser;
        readonly EndPoint idms_endpoint;
        readonly int MasterConfigHelperRoleUser;
        readonly Sapv3Util _sapV3Util = null;
        public SAPHRMSConnector(EndPoint endpoint, IdmsLogger logger) : base(logger)
        {
            this.Endpoint = endpoint;
            this.SAPConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            _idmsLogger = logger;
            MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
            idms_endpoint = EndPoint.GetByApplicationType(ApplicationType.IDMS);
            MasterConfigHelperRoleUser = MasterConfigHelper.RoleUser;
            // CanReadNRIC = MasterConfigHelper.CanReadNRIC;
            _sapV3Util = GetSapv3Util();
            PropagationFields = MasterConfigHelper.PropagationFields;
            canRunPropagationTrigger = MasterConfigHelper.CanRunPropagationTrigger;
        }

        Sapv3Util GetSapv3Util()
        {
            WriteProvisioningLog(LogLevelType.DebugStep, $"Endpoint System in : SetDefaultValues{Endpoint.Id}");
            var sapUtil = new Sapv3Util
            {
                AppServer = Config(EndpointKey.HostName, SAPConfiguration),
                SystemNumber = Config(EndpointKey.SystemNumber, SAPConfiguration),
                UserId = Config(EndpointKey.UserName, SAPConfiguration),
                Password = RSAHelper.DecryptData(Config(EndpointKey.Password, SAPConfiguration)),
                Client = Config(EndpointKey.ClientNumber, SAPConfiguration),
                DestinationName = Endpoint.DisplayName.Replace(" ", ""),
                RowCount = 50000
            };

            string sncMode = Config(EndpointKey.SNCMode, SAPConfiguration);
            if (!string.IsNullOrWhiteSpace(sncMode) && Convert.ToBoolean(sncMode) == true)
            {
                sapUtil.SNCMode = Convert.ToBoolean(sncMode);
                sapUtil.SNCLib = Config(EndpointKey.SNCLib, SAPConfiguration);
                sapUtil.SNCQop = Config(EndpointKey.SNCQop, SAPConfiguration);
                sapUtil.SNCPartnerName = Config(EndpointKey.SNCPartnerName, SAPConfiguration);
            }
            return sapUtil;
        }

        string Config(EndpointKey Key, EndpointConfiguration endpointConfig)
        {
            var config = endpointConfig.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }

        #region "Connector Base Members"

        string Configuration(EndpointKey Key)
        {
            var config = SAPConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }

        public override bool Connect()
        {
            try
            {
                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    WriteDiscoveryLog(LogLevelType.Info, string.Format("Connected to SAP Server - {0}", Configuration(EndpointKey.IPAddress)));
                    return true;
                }
            }

            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format("Could not connect to SAP Server - {0}", Configuration(EndpointKey.IPAddress)), ex);
                return false;
            }
        }

        public override void Disconnect()
        {
        }

        public override void GetData()
        {
            try
            {
                int recordCount = 0;

                this.Initialize();

                this.PopulateCompanyCodes();

                //this.PopulatePersonnelAreas();

                this.PopulateOrganizations();

                //this.PopulatePersonnelSubAreas();

                this.PopulateCostCenters();

                this.PopulateCostCenterTexts();

                this.PopulateBusinessRoles();

                this.UpdateUserPAInfo();

                if (this.IsFirstTimeDiscovery)
                {
                    recordCount = this.GetCompletePAInformation();
                }
                else
                {
                    recordCount = this.GetDeltaPAInformation();
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} Users found in Trusted Source", recordCount.ToString()));

                WriteDiscoveryLog(LogLevelType.Info, "Start updating organisation heads.");

                this.PopulateOrganizationHead();

                WriteDiscoveryLog(LogLevelType.Info, "Finished updating organisation heads.");
                User.ExecuteQuery(@" UPDATE A Set A.Supervisor = B.DisplayName FROM DI_M_User A inner join DI_M_User B on A.ROEmployeeId = B.EmployeeId WHERE  A.IsActive=1");

                User.ExecuteQuery(@" UPDATE A Set A.Supervisor = ''  WHERE  A.IsActive=1 AND  ISNULL(A.ROEmployeeId,'')= ''   ");
            }
            catch
            {
                throw;
            }
        }

        public override bool ChangePassword(string userName, string password, long dIUserId, bool isReactivation)
        {
            return false;
        }

        public override bool AddEmailIdInHR(string employeeId, string emailId, DateTime beginDate)
        {
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Info, $"AddEmailIdInHR Started for User {employeeId}");
                using (SAPNEWPRO proxy = new SAPNEWPRO())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    bool isAdded = false;

                    string Lockindicator = string.Empty;
                    string Objectid = string.Empty;
                    string Recordnumber = string.Empty;
                    string Subtype = "0010";
                    string ValiditybeginLess = beginDate.AddDays(-1).ToString("yyyyMMdd");
                    string Validitybegin = beginDate.ToString("yyyyMMdd");
                    string Validityend = "99991231";

                    SAPUserGrp.BAPIRETURN1 ret = new SAPUserGrp.BAPIRETURN1();
                    BAPIPAKEY key = new BAPIPAKEY();
                    BAPIP0105NLTable p0105Table = new BAPIP0105NLTable();

                    proxy.Bapi_Emplcomm_Getdetailedlist(employeeId, Subtype, "99991231", "18000101", out ret, ref p0105Table);

                    DataTable dt = p0105Table.ToADODataTable();

                    proxy.Bapi_Employee_Enqueue(employeeId, out ret);

                    foreach (DataRow row in dt.Rows)
                    {
                        string validBegin = row["Validbegin"].ToString();
                        string validEnd = row["Validend"].ToString();
                        Recordnumber = row["Recordnr"].ToString();
                        Objectid = row["Objectid"].ToString();
                        Lockindicator = row["Lockindic"].ToString();

                        if (validBegin == Validitybegin)
                        {
                            proxy.Bapi_Emplcomm_Delete(employeeId, Lockindicator, string.Empty, Objectid, Recordnumber, Subtype, validBegin, validEnd, out ret);
                        }
                        else if (validEnd == Validityend)
                        {
                            proxy.Bapi_Emplcomm_Delimit(ValiditybeginLess, employeeId, Lockindicator, string.Empty, Objectid, Recordnumber, Subtype, validBegin, validEnd, out key, out ret);
                        }
                    }

                    proxy.Bapi_Emplcomm_Create(emailId, string.Empty, out Lockindicator, out Objectid, out Recordnumber, out ret, ref employeeId, ref Subtype, ref Validitybegin, ref Validityend);

                    isAdded = string.IsNullOrEmpty(ret.Message);

                    if (!string.IsNullOrEmpty(ret.Message))
                    {
                        DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Warning, string.Format("Error while adding Email Id for Pernr - {0}, {1}", employeeId, ret.Message));
                    }

                    proxy.Bapi_Employee_Dequeue(employeeId, out ret);

                    proxy.CommitWork();

                    DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Warning, $"AddEmailIdInHR Completed for User {employeeId}");
                    return isAdded;
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Exception, string.Format("Error while updating Email Id - {0} in HRMS for Pernr - {1}", emailId, employeeId), ex);
                return false;
            }
        }

        public override bool DelimitTelephoneInHR(string employeeId, string telephone, string extn, DateTime beginDate)
        {
            return AddTelephoneInHR(employeeId, string.Empty, string.Empty, beginDate);
        }

        /// <summary>
        /// Update telephone number in HRMS.
        /// Note: No need to update the extension number in HRMS.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="telephone"></param>
        /// <param name="extn"></param>
        /// <param name="beginDate"></param>
        /// <returns></returns>
        public override bool AddTelephoneInHR(string employeeId, string telephone, string extn, DateTime beginDate)
        {
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Info, $"AddTelephoneInHR Started for User {employeeId}");
                using (SAPNEWPRO proxy = new SAPNEWPRO())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    bool isAdded = false;

                    string Lockindicator = string.Empty;
                    string Objectid = string.Empty;
                    string Recordnumber = string.Empty;
                    string Subtype = "0020";
                    string ValiditybeginLess = beginDate.AddDays(-1).ToString("yyyyMMdd");
                    string Validitybegin = beginDate.ToString("yyyyMMdd");
                    string Validityend = "99991231";

                    //TODO: Commented the extension part. Kanchana confirmed 06 May 2020:.
                    //if (string.IsNullOrEmpty(extn))
                    extn = string.Empty;
                    string maskedtelPhone = telephone.PadRight(30, ' ') + extn?.PadRight(10, ' ');

                    SAPUserGrp.BAPIRETURN1 ret = new SAPUserGrp.BAPIRETURN1();
                    BAPIPAKEY key = new BAPIPAKEY();
                    BAPIP0105NLTable p0105Table = new BAPIP0105NLTable();

                    proxy.Bapi_Emplcomm_Getdetailedlist(employeeId, Subtype, "99991231", "18000101", out ret, ref p0105Table);

                    DataTable dt = p0105Table.ToADODataTable();

                    proxy.Bapi_Employee_Enqueue(employeeId, out ret);

                    DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Info, $"Enqueue Pernr - {employeeId}");

                    foreach (DataRow row in dt.Rows)
                    {
                        string validBegin = row["Validbegin"].ToString();
                        string validEnd = row["Validend"].ToString();
                        Recordnumber = row["Recordnr"].ToString();
                        Objectid = row["Objectid"].ToString();
                        Lockindicator = row["Lockindic"].ToString();

                        if (validBegin == Validitybegin)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Info, $"Bapi_Emplcomm_Delete Pernr - {employeeId}");
                            proxy.Bapi_Emplcomm_Delete(employeeId, Lockindicator, string.Empty, Objectid, Recordnumber, Subtype, validBegin, validEnd, out ret);
                        }
                        else if (validEnd == Validityend)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Info, $"Bapi_Emplcomm_Delimit Pernr - {employeeId}");
                            proxy.Bapi_Emplcomm_Delimit(ValiditybeginLess, employeeId, Lockindicator, string.Empty, Objectid, Recordnumber, Subtype, validBegin, validEnd, out key, out ret);
                        }
                    }

                    DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Info, $"Bapi_Emplcomm_Create Pernr - {employeeId}");
                    string refEmpld = employeeId;

                    proxy.Bapi_Emplcomm_Create(maskedtelPhone, string.Empty, out Lockindicator, out Objectid, out Recordnumber, out ret, ref refEmpld, ref Subtype, ref Validitybegin, ref Validityend);

                    DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Info, $"refEmpld Pernr - {refEmpld}");

                    isAdded = string.IsNullOrEmpty(ret.Message);

                    if (!string.IsNullOrEmpty(ret.Message))
                    {
                        DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Warning, string.Format("Error while adding AddTelephoneInHR for Pernr - {0}, {1}", employeeId, ret.Message));
                    }

                    DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Warning, $"AddTelephoneInHR Completed for User {employeeId} - {maskedtelPhone}");

                    proxy.Bapi_Employee_Dequeue(employeeId, out ret);

                    proxy.CommitWork();


                    return isAdded;
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Exception, string.Format("Error while updating Telephone No - {0} in HRMS for Pernr - {1}", telephone, employeeId), ex);
                return false;
            }
        }

        public override bool AddFLINSLogonIdInHR(string employeeId, string sapLogonId, DateTime beginDate)
        {
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Warning, $"AddFLINSLogonIdInHR Started for User {employeeId}");
                using (SAPNEWPRO proxy = new SAPNEWPRO())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    bool isAdded = false;

                    string Lockindicator = string.Empty;
                    string Objectid = string.Empty;
                    string Recordnumber = string.Empty;
                    string Subtype = "0001";
                    string Validitybegin = beginDate.ToString("yyyyMMdd");
                    string Validityend = "99991231";

                    SAPUserGrp.BAPIRETURN1 ret = new SAPUserGrp.BAPIRETURN1();

                    proxy.Bapi_Employee_Enqueue(employeeId, out ret);

                    proxy.Bapi_Emplcomm_Create(sapLogonId, string.Empty, out Lockindicator, out Objectid, out Recordnumber, out ret, ref employeeId, ref Subtype, ref Validitybegin, ref Validityend);

                    isAdded = string.IsNullOrEmpty(ret.Message);

                    if (!string.IsNullOrEmpty(ret.Message))
                    {
                        DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Warning, $"AddFLINSLogonIdInHR return Message for User:{employeeId} , Retunr Message: {ret.Message}");
                    }

                    proxy.Bapi_Employee_Dequeue(employeeId, out ret);

                    proxy.CommitWork();

                    DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Warning, $"AddFLINSLogonIdInHR Completed for User {employeeId}");

                    return isAdded;


                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Exception, string.Format("Error while updating FLINS Login Id - {0} in HRMS for Pernr - {1}", sapLogonId, employeeId), ex);
                return false;
            }
        }

        public override bool DelimitEmailIdInHR(string employeeId, DateTime delimitDate)
        {
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Info, string.Format("DelimitEmailIdInHR started for Pernr - {0}", employeeId));
                using (SAPNEWPRO proxy = new SAPNEWPRO())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    string Lockindicator = string.Empty;
                    string Objectid = string.Empty;
                    string Recordnumber = string.Empty;
                    string Subtype = "0010";
                    string ValiditybeginLess = delimitDate.ToString("yyyyMMdd");
                    string Validityend = "99991231";

                    SAPUserGrp.BAPIRETURN1 ret = new SAPUserGrp.BAPIRETURN1();
                    BAPIPAKEY key = new BAPIPAKEY();
                    BAPIP0105NLTable p0105Table = new BAPIP0105NLTable();

                    proxy.Bapi_Emplcomm_Getdetailedlist(employeeId, Subtype, "99991231", "18000101", out ret, ref p0105Table);

                    DataTable dt = p0105Table.ToADODataTable();

                    proxy.Bapi_Employee_Enqueue(employeeId, out ret);

                    foreach (DataRow row in dt.Rows)
                    {
                        string validBegin = row["Validbegin"].ToString();
                        string validEnd = row["Validend"].ToString();
                        Recordnumber = row["Recordnr"].ToString();
                        Objectid = row["Objectid"].ToString();
                        Lockindicator = row["Lockindic"].ToString();

                        if (validEnd == Validityend)
                        {
                            proxy.Bapi_Emplcomm_Delimit(ValiditybeginLess, employeeId, string.Empty, string.Empty, Objectid, Recordnumber, Subtype, validBegin, validEnd, out key, out ret);

                            if (!string.IsNullOrEmpty(ret.Message))
                            {
                                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Warning, string.Format("Error while delimiting Email Id for Pernr - {0}", employeeId));

                            }
                        }
                    }

                    proxy.Bapi_Employee_Dequeue(employeeId, out ret);
                    proxy.CommitWork();

                    DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Info, string.Format("DelimitEmailIdInHR Completed for Pernr - {0}", employeeId));
                    return true;
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Exception, string.Format("Error while delimiting Email Id in HRMS for Pernr - {0}", employeeId), ex);
                return false;
            }
        }

        public override bool DelimitFLINSLogonIdInHR(string employeeId, DateTime delimitDate)
        {
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Info, string.Format("DelimitFLINSLogonIdInHR started for Pernr - {0}", employeeId));
                using (SAPNEWPRO proxy = new SAPNEWPRO())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    string Lockindicator = string.Empty;
                    string Objectid = string.Empty;
                    string Recordnumber = string.Empty;
                    string Subtype = "0001";
                    string ValiditybeginLess = delimitDate.ToString("yyyyMMdd");
                    string Validityend = "99991231";

                    SAPUserGrp.BAPIRETURN1 ret = new SAPUserGrp.BAPIRETURN1();
                    BAPIPAKEY key = new BAPIPAKEY();
                    BAPIP0105NLTable p0105Table = new BAPIP0105NLTable();

                    proxy.Bapi_Emplcomm_Getdetailedlist(employeeId, Subtype, "99991231", "18000101", out ret, ref p0105Table);

                    DataTable dt = p0105Table.ToADODataTable();

                    proxy.Bapi_Employee_Enqueue(employeeId, out ret);

                    foreach (DataRow row in dt.Rows)
                    {
                        string validBegin = row["Validbegin"].ToString();
                        string validEnd = row["Validend"].ToString();
                        Recordnumber = row["Recordnr"].ToString();
                        Objectid = row["Objectid"].ToString();
                        Lockindicator = row["Lockindic"].ToString();

                        if (validEnd == Validityend)
                        {
                            proxy.Bapi_Emplcomm_Delimit(ValiditybeginLess, employeeId, string.Empty, string.Empty, Objectid, Recordnumber, Subtype, validBegin, validEnd, out key, out ret);

                            if (!string.IsNullOrEmpty(ret.Message))
                            {
                                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Warning, string.Format("Error while delimiting FLINS Login Id for Pernr - {0}", employeeId));

                            }
                        }
                    }

                    proxy.Bapi_Employee_Dequeue(employeeId, out ret);

                    proxy.CommitWork();

                    DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Info, string.Format("DelimitFLINSLogonIdInHR Completed for Pernr - {0}", employeeId));
                    return true;
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Exception, string.Format("Error while delimiting FLINS Login Id in HRMS for Pernr - {0}", employeeId), ex);
                return false;
            }
        }

        public override void SyncTelephoneDirectory()
        {
            try
            {
                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Info, "Sync Telphone Dir started for Pernr ");

                var users = User.GetActiveEmployees();
                var propagationFields = MasterConfigHelper.PropagationFields;
                bool canRunPropagationTrigger = MasterConfigHelper.CanRunPropagationTrigger;

                SAPUserGrp.BAPIRETURN1 return1 = new SAPUserGrp.BAPIRETURN1();
                ZBAPI_PERNRTable pernrTable = new ZBAPI_PERNRTable();
                ZBAPIPA_TEL_DIR_INFO1Table telDirInfo = new ZBAPIPA_TEL_DIR_INFO1Table();
                ZBAPI_PERNR pernrInfo = new ZBAPI_PERNR();

                //using (SAPNEWPRO proxy = new SAPNEWPRO())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                foreach (User usr in users)
                {
                    if (usr.FullTimeEmployee)
                    {
                        try
                        {
                            int employeeId;
                            bool isValidPernr = int.TryParse(usr.EmployeeId, out employeeId);
                            if (isValidPernr)
                            {
                                using (SAPNEWPRO proxy = new SAPNEWPRO())
                                {
                                    proxy.Connection = this.GetSAPConnection();
                                    proxy.Connection.Open();

                                    return1 = new SAPUserGrp.BAPIRETURN1();
                                    pernrTable = new ZBAPI_PERNRTable();
                                    telDirInfo = new ZBAPIPA_TEL_DIR_INFO1Table();

                                    pernrInfo = new ZBAPI_PERNR();
                                    pernrInfo.Sign = "I";
                                    pernrInfo.Option0 = "EQ";
                                    pernrInfo.Low = usr.EmployeeId;

                                    pernrTable.Add(pernrInfo);

                                    proxy.Z_Bapipa_Get_Tel_Dir_Info(out return1, ref pernrTable, ref telDirInfo);

                                    if (telDirInfo != null && telDirInfo.Count != 0)
                                    {
                                        usr.OrgCompany = telDirInfo[0].Company;
                                        usr.OrgDepartment = telDirInfo[0].Department;
                                        usr.OrgDivision = telDirInfo[0].Division;
                                        usr.OrgBranch = telDirInfo[0].Branch;
                                        usr.OrgSection = telDirInfo[0].Section;
                                        usr.OrgUnit = telDirInfo[0].Unit;

                                        usr.Fax = telDirInfo[0].Fax_Number;
                                        usr.TelephoneNumber = telDirInfo[0].Office_Number;
                                        usr.MobileNumber = telDirInfo[0].Mobile_Number;

                                        if (!string.IsNullOrEmpty(telDirInfo[0].Postal_Code))
                                        {
                                            usr.PostalCode = telDirInfo[0].Postal_Code;
                                        }
                                        usr.PostalAddress = telDirInfo[0].Office_Address;
                                        usr.Country = telDirInfo[0].Country;

                                        if (!string.IsNullOrEmpty(telDirInfo[0].User_Job_Title))
                                        {
                                            usr.PositionName = telDirInfo[0].User_Job_Title;
                                        }

                                        User.UpdateUserFromDiscovery(usr, propagationFields, canRunPropagationTrigger, MasterConfigHelperServiceUser);
                                    }
                                    else
                                    {
                                        DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Warning, string.Format("Telephone directory info for the user - {0}, Pernr - {1}. is missing.", usr.KnownAs, usr.EmployeeId));
                                    }
                                }
                            }
                        }

                        catch (Exception ex)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Exception, string.Format("Error while synchronising data to telephone directory for the user - {0}, Pernr - {1}.", usr.KnownAs, usr.EmployeeId), ex);
                        }
                    }
                    //  }
                }
                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Info, "Completed preparing sync data for telephone directory.");
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, Endpoint.Id, LogLevelType.Exception, "Error while synchronising data to telephone directory.", ex);
            }
        }

        public override int DownloadAllUsersFromHR(DateTime scanDate)
        {
            try
            {
                int recordCount = 0;

                this.Initialize();

                recordCount = this.GetHRMSUsersFromGivenDate(scanDate.ToString("yyyyMMdd"));

                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} Users found in Trusted Source", recordCount.ToString()));

                return recordCount;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception at DownloadAllUsersFromHR", ex));
                throw;
            }
        }
        public override void UpdateAllUserFromHRMS(List<string> perNos)
        {
            if(perNos != null && perNos.Count > 0)
            {
                try
                {
                    
                    
                    foreach (var perNo in perNos)
                    {
                        try
                        {
                            this.Initialize();
                            this.GetUserByPernr(perNo);
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, $"Exception at DownloadAllUserFromHR PerNo:{perNo} ex:{ex.Message}",ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception at DownloadAllUserFromHR", ex));
                }
            }
            else
            {
                WriteDiscoveryLog(LogLevelType.Exception, $"perNos empty");
            }
        }
        public override void DownloadSpecificUserFromHR(string pernr)
        {
            try
            {
                this.Initialize();

                this.GetUserByPernr(pernr);
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception at DownloadSpecificUserFromHR", ex));
                throw;
            }
        }

        public override void DiscoverSpecificUserFromHR(string pernr)
        {
            try
            {
                int recordCount = 0;

                this.Initialize();

                this.PopulateCompanyCodes();

                // this.PopulatePersonnelAreas();

                this.PopulateOrganizations();

                // this.PopulatePersonnelSubAreas();

                this.PopulateCostCenters();

                this.PopulateCostCenterTexts();

                this.PopulateBusinessRoles();

                recordCount = this.DiscoverUserByPernr(pernr);

                WriteDiscoveryLog(LogLevelType.Info, $"End of Specific User Discovery for the PERNR : {pernr} ");

                this.PopulateOrganizationHead();

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception at DiscoverSpecificUserFromHR", ex));
                throw;
            }
        }

        public override DateTime GetEffectiveDateByPernr(string pernr, EmployeeEvents eventType, DateTime effectiveStartDate, out bool isTransferWithRetain, EmployeeEventTriggerComment eventTriggerComment=null)
        {
            TAB512Table dt = new TAB512Table();
            RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();
            List<OrgChangeTrack> OrgChangeTrackList = new List<OrgChangeTrack>();
            string effectiveStartDateString = effectiveStartDate.ToString("yyyyMMdd");
            try
            {
                isTransferWithRetain = false;
                this.Initialize();

                WriteProvisioningLog(LogLevelType.Info, $"Checking the {eventType.ToString()} trigger existence for user :{pernr} ", null);

                DataRow[] rows;

                RFC_DB_OPT FILTER_PERNR_ENDDA = new RFC_DB_OPT();

                if (eventType == EmployeeEvents.OUChange || eventType == EmployeeEvents.ReOrganisation)
                {
                    FILTER_PERNR_ENDDA.Text = "PERNR = '" + pernr + "'";
                }
                else
                {
                    FILTER_PERNR_ENDDA.Text = "PERNR = '" + pernr + "' AND ENDDA >= '" + today + "'";
                }

                List<EventTypeReason> triggerConfigValues = null;
                DateTime dateToCompare = DateTime.MinValue;

                switch (eventType)
                {

                    case EmployeeEvents.NewJoinee:
                    case EmployeeEvents.Rehire:
                        triggerConfigValues = MasterConfigHelper.OnBoardTriggers;
                        break;
                    case EmployeeEvents.OUChange:
                        {
                            //FILTER_PERNR_ENDDA.Text = "PERNR = '" + pernr + "'";
                            triggerConfigValues = MasterConfigHelper.TranferTriggers;
                        }
                        break;
                    case EmployeeEvents.Resignation:
                        triggerConfigValues = MasterConfigHelper.OffBoardTriggers;
                        break;
                    case EmployeeEvents.ReOrganisation:
                        triggerConfigValues = MasterConfigHelper.ReOrgTriggers;
                        break;
                }


                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    #region  PA0000 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    dtFlds.Add(FLD_BEGDA);
                    dtFlds.Add(FLD_ENDDA);
                    dtFlds.Add(FLD_MASSN);
                    dtFlds.Add(FLD_MASSG);

                    dtopts.Add(FILTER_PERNR_ENDDA);

                    proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0001 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    dtFlds.Add(FLD_BEGDA);
                    dtFlds.Add(FLD_ENDDA);
                    dtFlds.Add(FLD_ENAME);
                    dtFlds.Add(FLD_KOSTL);
                    dtFlds.Add(FLD_BUKRS);
                    dtFlds.Add(FLD_WERKS);
                    dtFlds.Add(FLD_BTRTL);
                    dtFlds.Add(FLD_ORGEH);
                    dtFlds.Add(FLD_PLANS);
                    dtFlds.Add(FLD_PERSG);
                    dtFlds.Add(FLD_AEDTM);


                    dtopts.Add(FILTER_PERNR_ENDDA);

                    proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0001 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0302 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    dtFlds.Add(FLD_BEGDA);
                    dtFlds.Add(FLD_ENDDA);
                    dtFlds.Add(FLD_MASSN);
                    dtFlds.Add(FLD_MASSG);

                    dtopts.Add(FILTER_PERNR_ENDDA);

                    proxy.Rfc_Read_Table("|", null, "PA0302", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0302 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion 
                }

                if (eventType == EmployeeEvents.OUChange || eventType == EmployeeEvents.ReOrganisation)
                {
                    rows = m_dtPA0302.AsEnumerable().Where(tmpDataRow =>
                    {

                        var flag = triggerConfigValues.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim());

                        if (!string.IsNullOrWhiteSpace(effectiveStartDateString))
                        {
                            flag = UtilityMethods.GetDateFromSAPDateString(tmpDataRow.Field<string>("BEGDA")) <= UtilityMethods.GetDateFromSAPDateString(effectiveStartDateString) &&
                             (string.IsNullOrWhiteSpace(tmpDataRow.Field<string>("ENDDA")) || UtilityMethods.GetDateFromSAPDateString(tmpDataRow.Field<string>("ENDDA")) >= UtilityMethods.GetDateFromSAPDateString(effectiveStartDateString));
                        }

                        return flag;
                    }).Select(tmpDataRow => tmpDataRow).ToArray();

                    var rows_PA0001 = getPA0001(m_dtPA0001, effectiveStartDateString, null);


                    if (rows != null && rows.Length > 0 && rows_PA0001 != null && rows_PA0001.Length > 0)
                    {
                        string dtFromSystem = CheckNull.AgainstString(rows[rows.Length - 1]["BEGDA"].ToString()).Trim();
                        WriteProvisioningLog(LogLevelType.Info, $"Date From System: {dtFromSystem }");
                        if (!string.IsNullOrEmpty(dtFromSystem) && dtFromSystem.Length >= 8)
                        {
                            dateToCompare = UtilityMethods.GetDateFromSAPDateString(dtFromSystem);
                        }
                    }
                }

                else if (eventType == EmployeeEvents.Rehire || eventType == EmployeeEvents.NewJoinee || eventType == EmployeeEvents.Resignation)
                {
                    WriteProvisioningLog(LogLevelType.Info, $"m_dtPA0000 Count: {m_dtPA0000.Rows.Count }");

                    var info_m_dtPA0000 = new List<string>();
                    foreach (DataRow row in m_dtPA0000.Rows)
                    {
                        for (int i = 0; i < m_dtPA0000.Columns.Count; i++)
                        {
                            info_m_dtPA0000.Add($"ColumnName: {m_dtPA0000.Columns[i].ColumnName} - ColumnValue: {row[i]}");
                        }
                    }

                    WriteProvisioningLog(LogLevelType.DebugStep, $"m_dtPA0000" + string.Join(",", info_m_dtPA0000));

                    WriteProvisioningLog(LogLevelType.Info, $"m_dtPA0001 Count: {m_dtPA0001.Rows.Count }");

                    var info_m_dtPA0001 = new List<string>();
                    foreach (DataRow row in m_dtPA0001.Rows)
                    {
                        for (int i = 0; i < m_dtPA0001.Columns.Count; i++)
                        {
                            info_m_dtPA0001.Add($"ColumnName: {m_dtPA0001.Columns[i].ColumnName} - ColumnValue: {row[i]}");
                        }
                    }

                    WriteProvisioningLog(LogLevelType.DebugStep, $"m_dtPA0001" + string.Join(",", info_m_dtPA0001));

                    if (triggerConfigValues != null && triggerConfigValues.Count() > 0)
                    {
                        rows = m_dtPA0000.AsEnumerable().Where(tmpDataRow =>
                        {
                            return triggerConfigValues.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim());
                        }).Select(tmpDataRow => tmpDataRow).ToArray<DataRow>();

                        WriteProvisioningLog(LogLevelType.Info, $"rows: {rows.Length }");

                        if (rows != null && rows.Length > 0)
                        {
                            string dtFromSystem = CheckNull.AgainstString(rows[rows.Length - 1]["BEGDA"].ToString()).Trim();
                            WriteProvisioningLog(LogLevelType.Info, $"Date From System: {dtFromSystem }");
                            if (!string.IsNullOrEmpty(dtFromSystem) && dtFromSystem.Length >= 8)
                            {
                                dateToCompare = UtilityMethods.GetDateFromSAPDateString(dtFromSystem);
                            }
                        }
                    }
                }

                WriteProvisioningLog(LogLevelType.Info, $"DatetoCompate {dateToCompare }");
                return dateToCompare;
            }
            catch (Exception ex)
            {
                WriteProvisioningLog(LogLevelType.Error, "Error while getting specific user information", ex);
                throw;
            }
            finally
            {
                this.DisposePATables();

                dt = null;
                dtFlds = null;
                dtopts = null;
                dtTemp = null;
                dtFields = null;
            }
        }


        #endregion

        void Initialize()
        {

            //Tables
            m_dtPA0000AllEmp = new DataTable();

            m_dtPA0000 = new DataTable();
            m_dtPA0001 = new DataTable();
            m_dtPA0002 = new DataTable();
            m_dtPA0006 = new DataTable();
            m_dtPA0185 = new DataTable();
            m_dtPA0077 = new DataTable();
            m_dtPA0105 = new DataTable();
            m_dtPA0027 = new DataTable();
            m_dtPA0302 = new DataTable();

            m_dtT001 = new DataTable();
            m_dtT500P = new DataTable();
            m_dtT001P = new DataTable();
            m_dtCSKS = new DataTable();
            m_dtCSKT = new DataTable();
            m_dtHRP1000 = new DataTable();
            m_dtHRP1001 = new DataTable();

            //Fields
            FLD_PERNR = new RFC_DB_FLD();
            FLD_PERNR.Fieldname = "PERNR";

            FLD_ENDDA = new RFC_DB_FLD();
            FLD_ENDDA.Fieldname = "ENDDA";

            FLD_VORNA = new RFC_DB_FLD();
            FLD_VORNA.Fieldname = "VORNA";

            FLD_TITEL = new RFC_DB_FLD();
            FLD_TITEL.Fieldname = "TITEL";

            FLD_NATIO = new RFC_DB_FLD();
            FLD_NATIO.Fieldname = "NATIO";

            FLD_NACHN = new RFC_DB_FLD();
            FLD_NACHN.Fieldname = "NACHN";

            FLD_MIDNM = new RFC_DB_FLD();
            FLD_MIDNM.Fieldname = "MIDNM";

            FLD_RUFNM = new RFC_DB_FLD();
            FLD_RUFNM.Fieldname = "RUFNM";

            FLD_BEGDA = new RFC_DB_FLD();
            FLD_BEGDA.Fieldname = "BEGDA";

            FLD_ICNUM = new RFC_DB_FLD();
            FLD_ICNUM.Fieldname = "ICNUM";

            FLD_RACKY = new RFC_DB_FLD();
            FLD_RACKY.Fieldname = "RACKY";

            FLD_MASSN = new RFC_DB_FLD();
            FLD_MASSN.Fieldname = "MASSN";

            FLD_MASSG = new RFC_DB_FLD();
            FLD_MASSG.Fieldname = "MASSG";

            FLD_STAT2 = new RFC_DB_FLD();
            FLD_STAT2.Fieldname = "STAT2";

            FLD_ENAME = new RFC_DB_FLD();
            FLD_ENAME.Fieldname = "ENAME";

            FLD_KOSTL = new RFC_DB_FLD();
            FLD_KOSTL.Fieldname = "KOSTL";

            FLD_BUKRS = new RFC_DB_FLD();
            FLD_BUKRS.Fieldname = "BUKRS";

            FLD_WERKS = new RFC_DB_FLD();
            FLD_WERKS.Fieldname = "WERKS";

            FLD_BTRTL = new RFC_DB_FLD();
            FLD_BTRTL.Fieldname = "BTRTL";

            FLD_ORGEH = new RFC_DB_FLD();
            FLD_ORGEH.Fieldname = "ORGEH";

            FLD_PLANS = new RFC_DB_FLD();
            FLD_PLANS.Fieldname = "PLANS";

            FLD_PERSG = new RFC_DB_FLD();
            FLD_PERSG.Fieldname = "PERSG";

            FLD_USRTY = new RFC_DB_FLD();
            FLD_USRTY.Fieldname = "USRTY";

            FLD_USRID_LONG = new RFC_DB_FLD();
            FLD_USRID_LONG.Fieldname = "USRID_LONG";

            FLD_USRID = new RFC_DB_FLD();
            FLD_USRID.Fieldname = "USRID";

            FLD_KST01 = new RFC_DB_FLD();
            FLD_KST01.Fieldname = "KST01";

            FLD_KSTAR = new RFC_DB_FLD();
            FLD_KSTAR.Fieldname = "KSTAR";

            FLD_AEDTM = new RFC_DB_FLD();
            FLD_AEDTM.Fieldname = "AEDTM";

            FLD_PSTLZ = new RFC_DB_FLD();
            FLD_PSTLZ.Fieldname = "PSTLZ";

            FLD_SUBTY = new RFC_DB_FLD();
            FLD_SUBTY.Fieldname = "SUBTY";

            //delta date 
            if (!this.IsFirstTimeDiscovery)
            {
                deltaDate = this.Process.LastRunOn.Value.ToString("yyyyMMdd");
            }
        }

        SAPConnection GetSAPConnection()
        {
            Destination destination = new Destination();
            string pwd = RSAHelper.DecryptData(Configuration(EndpointKey.Password));
            destination.AppServerHost = Configuration(EndpointKey.HostName); //Configuration.IPAddress;10.168.15.24";//
            destination.Client = Convert.ToInt16(Configuration(EndpointKey.ClientNumber)); //Configuration.IntClientNum; 300;//
            destination.Password = pwd; //Configuration.Password;"orange123"; //
            destination.SystemNumber = Convert.ToInt16(Configuration(EndpointKey.SystemNumber)); //Configuration.SystemNumber;00;//
            destination.Username = Configuration(EndpointKey.UserName); //Configuration.UserName;sisuis";//"

            string sncMode = Configuration(EndpointKey.SNCMode);

            if (!string.IsNullOrWhiteSpace(sncMode) && Convert.ToBoolean(sncMode) == true)
            {
                destination.SNCMode = Convert.ToBoolean(sncMode);
                destination.SNCLib = Configuration(EndpointKey.SNCLib);
                destination.SNCQop = Convert.ToInt16(Configuration(EndpointKey.SNCQop));
                destination.SNCPartnerName = Configuration(EndpointKey.SNCPartnerName);
            }

            SAPConnection connection = new SAPConnection(destination);
            return connection;
        }

        int GetCompletePAInformation()
        {

            TAB512Table dt = new TAB512Table();
            RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();

            try
            {
                WriteDiscoveryLog(LogLevelType.Info, "Getting Complete PA Information");

                string Pernr = string.Empty;
                bool isNewUser;

                PasswordPolicy iacmPolicy = PasswordPolicy.GetById(1);

                var companies = Company.GetAll();

                var sections = NOrganisation.GetAll();
                var positions = NDesignation.GetAll();
                var costCenters = NCostCenter.GetAll();

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    #region  All Emp PA0000 Table
                    dtFlds.Add(FLD_PERNR);
                    RFC_DB_OPT filter = new RFC_DB_OPT();
                    filter.Text = "ENDDA >= '" + today + "' ";
                    dtopts.Add(filter);
                    proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp = ConvertPipesToColumns(dtFields, dtTemp);
                    #endregion

                }


                //TRUSTED SOURCE USER INFO
                Company company = null;
                NCostCenter costCenter = null;
                NOrganisation section = null;
                NDesignation position = null;

                var OnBoardTriggers = MasterConfigHelper.OnBoardTriggers;
                var OffBoardTriggers = MasterConfigHelper.OffBoardTriggers;
                var OnBoardTriggersNotToBeConsideredForJoinDate = MasterConfigHelper.OnBoardTriggersNotToBeConsideredForJoinDate;

                DataRow[] rows;

                foreach (DataRow person in m_dtPA0000AllEmp.Rows)
                {

                    try
                    {
                        Pernr = person["PERNR"].ToString().Trim();

                        RFC_DB_OPT FILTER_PERNR = new RFC_DB_OPT();
                        FILTER_PERNR.Text = "PERNR = '" + Pernr + "' ";

                        RFC_DB_OPT FILTER_PERNR_ENDDA = new RFC_DB_OPT();
                        FILTER_PERNR_ENDDA.Text = "PERNR = '" + Pernr + "' AND ENDDA >= '" + today + "'";

                        using (SAPProxy1 proxy = new SAPProxy1())
                        {
                            proxy.Connection = this.GetSAPConnection();
                            proxy.Connection.Open();

                            #region  PA0000 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_BEGDA);
                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_STAT2);
                            dtFlds.Add(FLD_MASSN);
                            dtFlds.Add(FLD_MASSG);

                            dtopts.Add(FILTER_PERNR);

                            proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0000 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0001 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_BEGDA);
                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_ENAME);
                            dtFlds.Add(FLD_KOSTL);
                            dtFlds.Add(FLD_BUKRS);
                            dtFlds.Add(FLD_WERKS);
                            dtFlds.Add(FLD_BTRTL);
                            dtFlds.Add(FLD_ORGEH);
                            dtFlds.Add(FLD_PLANS);
                            dtFlds.Add(FLD_PERSG);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0001 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0002 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_PERNR);
                            dtFlds.Add(FLD_VORNA);
                            dtFlds.Add(FLD_TITEL);
                            dtFlds.Add(FLD_NATIO);
                            dtFlds.Add(FLD_NACHN);
                            dtFlds.Add(FLD_MIDNM);
                            dtFlds.Add(FLD_RUFNM);
                            dtFlds.Add(FLD_ENDDA);

                            dtopts.Add(FILTER_PERNR);

                            proxy.Rfc_Read_Table("|", " ", "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtFields = dtFlds.ToADODataTable();
                            DataTable dtParameters = dt.ToADODataTable();
                            m_dtPA0002 = ConvertPipesToColumns(dtFields, dtParameters);

                            #endregion

                            #region  PA0006 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_PSTLZ);
                            dtFlds.Add(FLD_SUBTY);
                            dtFlds.Add(FLD_ENDDA);

                            dtopts.Add(FILTER_PERNR);

                            proxy.Rfc_Read_Table("|", null, "PA0006", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0006 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region  PA0185 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_ICNUM);
                            dtFlds.Add(FLD_ENDDA);

                            dtopts.Add(FILTER_PERNR);

                            proxy.Rfc_Read_Table("|", null, "PA0185", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0185 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0077 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_RACKY);

                            dtopts.Add(FILTER_PERNR);

                            proxy.Rfc_Read_Table("|", null, "PA0077", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0077 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0105 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_USRTY);
                            dtFlds.Add(FLD_USRID_LONG);
                            dtFlds.Add(FLD_USRID);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0105 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0027 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_KST01);
                            dtFlds.Add(FLD_KSTAR);

                            dtopts.Add(FILTER_PERNR);

                            proxy.Rfc_Read_Table("|", null, "PA0027", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0027 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0302 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_BEGDA);
                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_MASSN);
                            dtFlds.Add(FLD_MASSG);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0302", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0302 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion
                        }

                        WriteDiscoveryLog(LogLevelType.Info, $"GetCompletePAInformation: Discovery Started for the PERNR - {Pernr}");


                        User user = User.GetByEmployeeId(Pernr, this.Endpoint.Id);

                        if (user != null)
                        {
                            isNewUser = false;
                        }
                        else
                        {
                            isNewUser = true;
                            user = new User();
                            user.Reserved4 = Constants.EmployeeStatus.NewEmployee;
                            user.EmployeeId = Pernr;
                        }

                        #region Telephone Directory

                        this.Z_Bapipa_Get_Tel_Dir_Info(user, today, null);

                        UpdateOrgTrack(null, user, sections, companies);


                        #endregion

                        user.IdentificationNo = Pernr;


                        rows = (from tmpDataRow in m_dtPA0006.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today && tmpDataRow.Field<string>("SUBTY") == "9000"
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.PostalCode = CheckNull.AgainstString(rows[0]["PSTLZ"].ToString().Trim());
                        }

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && OnBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.EmployeeStatus = CheckNull.AgainstString(rows[0]["STAT2"].ToString().Trim());
                            user.IsActive = MasterConfigHelper.ActiveEmployeeFlags.Contains(user.EmployeeStatus) ? true : false;
                        }

                        // Join date of the employee - should not considered Retirement and Re-employment

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where
                                  OnBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim()) &&

                                !OnBoardTriggersNotToBeConsideredForJoinDate.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            string Joindate = CheckNull.AgainstString(rows[rows.Count() - 1]["BEGDA"].ToString()).Trim();
                            if (!string.IsNullOrEmpty(Joindate) && Joindate.Length >= 8)
                            {
                                user.JoinDate = UtilityMethods.GetDateFromString(Joindate);
                                user.EmployeeStatus = CheckNull.AgainstString(rows[0]["STAT2"].ToString().Trim());
                                user.IsActive = MasterConfigHelper.ActiveEmployeeFlags.Contains(user.EmployeeStatus) ? true : false;
                                var rows_PA0001 = getPA0001(m_dtPA0001, Joindate, null);

                                if (rows_PA0001 != null && rows_PA0001.Count() > 0)
                                {
                                    user.CostCenter = CheckNull.AgainstString(rows_PA0001[0]["KOSTL"]).Trim();
                                    user.PositionCode = CheckNull.AgainstString(rows_PA0001[0]["PLANS"]).Trim();
                                    user.EEGroup = CheckNull.AgainstString(rows_PA0001[0]["PERSG"]).Trim();
                                    user.IsExecutiveEmployee = MasterConfigHelper.ExecutiveEmployeeFlags.Contains(user.EEGroup) ? true : false;
                                }
                            }
                        }
                        else
                        {
                            WriteDiscoveryLog(LogLevelType.Info, $"GetCompletePAInformation: Join Date is Null for the PERNR - {Pernr}");

                            user.JoinDate = null;
                        }

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && OffBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            string termdate = CheckNull.AgainstString(rows[rows.Count() - 1]["BEGDA"].ToString()).Trim();
                            if (!string.IsNullOrEmpty(termdate) && termdate.Length >= 8)
                            {
                                user.TerminationDate = UtilityMethods.GetDateFromString(termdate);
                                if (user.TerminationDate != null && user.TerminationDate.Value < DateTime.Now)
                                {
                                    user.IsActive = false;
                                }
                            }
                        }
                        else
                        {
                            WriteDiscoveryLog(LogLevelType.Info, $"GetCompletePAInformation: Termination Date is Null for the PERNR - {Pernr}");

                            user.TerminationDate = null;
                        }

                        rows = (from tmpDataRow in m_dtPA0001.AsEnumerable()
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows.Count() > 0)
                        {
                            user.CostCenter = CheckNull.AgainstString(rows[0]["KOSTL"]).Trim();
                            user.PositionCode = CheckNull.AgainstString(rows[0]["PLANS"]).Trim();
                            user.EEGroup = CheckNull.AgainstString(rows[0]["PERSG"]).Trim();
                            user.IsExecutiveEmployee = MasterConfigHelper.ExecutiveEmployeeFlags.Contains(user.EEGroup) ? true : false;
                        }

                        rows = (from tmpDataRow in m_dtPA0027.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                     && tmpDataRow.Field<string>("KSTAR").Trim() == "01"
                                select tmpDataRow).ToArray<DataRow>();
                        if (null != rows && rows.Count() > 0)
                        {
                            user.CostDistribution = CheckNull.AgainstString(rows[0]["KST01"]).Trim();
                        }
                        else
                        {
                            user.CostDistribution = string.Empty;
                        }

                        rows = (from tmpDataRow in m_dtPA0002.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && tmpDataRow.Field<string>("PERNR") == Pernr
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.FirstName = CheckNull.AgainstString(rows[0]["VORNA"]).Trim();
                            user.MiddleName = CheckNull.AgainstString(rows[0]["MIDNM"]).Trim();
                            user.LastName = CheckNull.AgainstString(rows[0]["NACHN"]).Trim();
                            user.KnownAs = CheckNull.AgainstString(rows[0]["RUFNM"]).Trim();
                            user.DisplayName = user.KnownAs;
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Error, $"Demographic information is missing for the user - {Pernr}");
                            continue;
                        }

                        rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                                where tmpDataRow.Field<string>("USRTY").Trim() == "0001" &&
                                 UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.FLINSLoginId = CheckNull.AgainstString(rows[0]["USRID"]).Trim();
                        }
                        else
                        {
                            user.FLINSLoginId = null;
                        }

                        rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                                where tmpDataRow.Field<string>("USRTY").Trim() == "0010" &&
                                 UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.EmailId = CheckNull.AgainstString(rows[rows.Count() - 1]["USRID_LONG"]).Trim();
                        }
                        else
                        {
                            user.EmailId = string.Empty;
                        }

                        //Ethinicity
                        rows = (from tmpDataRow in m_dtPA0077.AsEnumerable()
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            user.Ethinicity = rows[0]["RACKY"].ToString().Trim();
                        }
                        else
                        {
                            user.Ethinicity = String.Empty;
                        }
                        costCenter = costCenters.FirstOrDefault(item => item.Code == user.CostCenter && item.CompanyId == user.CompanyId);
                        if (costCenter != null)
                        {
                            user.CostCenterText = costCenter.Description;
                        }

                        position = positions.FirstOrDefault(item => item.Code == user.PositionCode);
                        if (position != null)
                        {
                            user.NBusinessRoleId = position.Id;
                            user.PositionName = position.Description;
                        }

                        if (iacmPolicy != null)
                        {
                            user.Password = RSAHelper.EncryptData(new PasswordResolver().ConstructGeneralUserPassword
                                                    (iacmPolicy.PasswordLength.HasValue ? iacmPolicy.PasswordLength.Value : 8,
                                                    iacmPolicy.NoOfSpecialCharacters.HasValue ? iacmPolicy.NoOfSpecialCharacters.Value : 0,
                                                    iacmPolicy.NoOfLowerCaseAlphabets.HasValue ? iacmPolicy.NoOfLowerCaseAlphabets.Value : 0,
                                                    iacmPolicy.NoOfUpperCaseAlphabets.HasValue ? iacmPolicy.NoOfUpperCaseAlphabets.Value : 0,
                                                    iacmPolicy.NoOfNumericCharacters.HasValue ? iacmPolicy.NoOfNumericCharacters.Value : 0));

                        }

                        user.EmployeeId = Pernr;
                        user.FullTimeEmployee = true;
                        user.EmployeeType = EmployeeType.Permanent;
                        user.EndPointSystemId = this.Endpoint.Id;

                        bool isRejoinUser = false;
                        long userId = 0;

                        if (isNewUser)
                        {
                            var oldUser = User.GetByNRIC(user.IdentificationNo);
                            if (oldUser != null)
                            {
                                isRejoinUser = (!user.TerminationDate.HasValue || (user.TerminationDate.HasValue && user.TerminationDate.Value > DateTime.Today)) && oldUser.TerminationDate.HasValue;
                                //if older record is recent than the newer record then skip writing the newer record.
                                if (!isRejoinUser)
                                {
                                    continue;
                                }
                            }
                            if (isRejoinUser)
                            {
                                user.Id = oldUser.Id;
                                //user.UserName = oldUser.UserName;
                                user.Password = oldUser.Password;
                                user.EmailId = oldUser.EmailId;
                                user.FLINSLoginId = oldUser.FLINSLoginId;
                                user.AuthenticationMode = oldUser.AuthenticationMode;
                                user.OneKeyTokenId = oldUser.OneKeyTokenId;

                                userId = oldUser.Id;

                                user.Save(MasterConfigHelperServiceUser);
                            }
                            else
                            {
                                user.UserName = Pernr;//new logic
                                user.Save(MasterConfigHelperServiceUser);
                                User.InsertUserRole(user.Id, MasterConfigHelperRoleUser);
                            }
                        }
                        else
                        {
                            user.Save(MasterConfigHelperServiceUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Error, string.Format("Error while reading the record - {0}", Pernr), ex);
                    }
                }

                return m_dtPA0000AllEmp.Rows.Count;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Error in getPAInformation", ex);
                throw;
            }
            finally
            {
                this.DisposePATables();

                dt = null;
                dtFlds = null;
                dtopts = null;
                dtTemp = null;
                dtFields = null;
            }
        }

        int GetDeltaPAInformation()
        {

            TAB512Table dt = new TAB512Table();
            RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();

            List<string> employeeIds = new List<string>();
            List<Company> companies = new List<Company>();
            List<NCostCenter> costCenters = new List<NCostCenter>();

            List<NOrganisation> sections = new List<NOrganisation>();
            List<NDesignation> positions = new List<NDesignation>();

            triggers = new List<EmployeeEventTrigger>();

            List<long> rejoinWithSamePernrUsers = new List<long>();

            string Pernr = string.Empty;
            bool isNewUser;

            var OnBoardTriggersNotToBeConsideredForJoinDate = MasterConfigHelper.OnBoardTriggersNotToBeConsideredForJoinDate;
            var propagationFields = MasterConfigHelper.PropagationFields;
            var OnBoardTriggers = MasterConfigHelper.OnBoardTriggers;
            var OffBoardTriggers = MasterConfigHelper.OffBoardTriggers;
            var ReOrgTriggers = MasterConfigHelper.ReOrgTriggers;
            var TranferTriggers = MasterConfigHelper.TranferTriggers;
            var ExecutiveEmployeeFlags = MasterConfigHelper.ExecutiveEmployeeFlags;
            var ActiveEmployeeFlags = MasterConfigHelper.ActiveEmployeeFlags;
            var AuthenticationMode = MasterConfigHelper.AuthenticationMode;
            var RejoinTriggers = MasterConfigHelper.RejoinTriggers;
            bool canRunPropagationTrigger = MasterConfigHelper.CanRunPropagationTrigger;

            try
            {
                PasswordPolicy iacmPolicy = PasswordPolicy.GetById(1);

                WriteDiscoveryLog(LogLevelType.Info, "Getting Delta PA Information");

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    RFC_DB_OPT filter = new RFC_DB_OPT();
                    filter.Text = "AEDTM >= '" + deltaDate + "' ";

                    #region  PA0000 Table

                    dtFlds.Add(FLD_PERNR);

                    dtopts.Add(filter);

                    proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion

                    #region PA0001 Table

                    proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion

                    #region PA0002 Table

                    proxy.Rfc_Read_Table("|", null, "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion

                    #region  PA0185 Table

                    proxy.Rfc_Read_Table("|", null, "PA0185", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion

                    #region PA0077 Table

                    proxy.Rfc_Read_Table("|", null, "PA0077", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion

                    #region PA0105 Table

                    proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion

                    #region PA0027 Table

                    proxy.Rfc_Read_Table("|", null, "PA0027", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion


                    #region PA0302 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    dtFlds.Add(FLD_PERNR);
                    dtopts.Add(filter);

                    proxy.Rfc_Read_Table("|", null, "PA0302", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));
                    #endregion

                }

                foreach (DataRow person in m_dtPA0000AllEmp.Rows)
                {
                    Pernr = person["PERNR"].ToString().Trim();

                    if (!employeeIds.Contains(Pernr))
                    {
                        employeeIds.Add(Pernr);
                    }
                }

                //TRUSTED SOURCE USER INFO


                NCostCenter costCenter = null;
                NDesignation position = null;

                DataRow[] rows;

                if (employeeIds.Count > 0)
                {
                    companies = Company.GetAll();
                    sections = NOrganisation.GetAll();
                    positions = NDesignation.GetAll();
                    costCenters = NCostCenter.GetAll();
                }

                bool isA3Action = false;
                bool isUserOrgUpdated = false;
                bool isRejoinUser = false;
                bool offBoardExists = false;
                bool onBoardExists = false;
                long userId = 0;

                List<OrgChangeTrack> OrgChangeTrackList = new List<OrgChangeTrack>();

                User oldUser = null;
                User user = null;

                foreach (string person in employeeIds)
                {
                    OrgChangeTrackList = new List<OrgChangeTrack>();

                    try
                    {
                        WriteDiscoveryLog(LogLevelType.Info, $"Discovery delta information for Empoyee:{person}");
                        userId = 0;

                        isRejoinUser = false;
                        offBoardExists = false;
                        onBoardExists = false;
                        isA3Action = false;
                        isUserOrgUpdated = false;
                        oldUser = null;

                        Pernr = person;

                        user = User.GetByEmployeeId(Pernr, this.Endpoint.Id);

                        if (user != null)
                        {
                            isNewUser = false;

                        }
                        else
                        {
                            isNewUser = true;
                            user = new User();
                            user.Reserved4 = Constants.EmployeeStatus.NewEmployee;
                            user.EmployeeId = Pernr;
                        }


                        RFC_DB_OPT FILTER_DELTA_DATA = new RFC_DB_OPT();
                        FILTER_DELTA_DATA.Text = "PERNR = '" + Pernr + "' AND AEDTM >= '" + deltaDate + "'";

                        RFC_DB_OPT FILTER_PERNR_ENDDA = new RFC_DB_OPT();
                        FILTER_PERNR_ENDDA.Text = "PERNR = '" + Pernr + "' AND ENDDA >= '" + deltaDate + "'";

                        using (SAPProxy1 proxy = new SAPProxy1())
                        {
                            proxy.Connection = this.GetSAPConnection();
                            proxy.Connection.Open();

                            #region  PA0000 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_BEGDA);
                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_STAT2);
                            dtFlds.Add(FLD_MASSN);
                            dtFlds.Add(FLD_MASSG);

                            dtopts.Add(FILTER_DELTA_DATA);

                            proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0000 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0001 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_ENAME);
                            dtFlds.Add(FLD_KOSTL);
                            dtFlds.Add(FLD_BUKRS);
                            dtFlds.Add(FLD_WERKS);
                            dtFlds.Add(FLD_BTRTL);
                            dtFlds.Add(FLD_ORGEH);
                            dtFlds.Add(FLD_PLANS);
                            dtFlds.Add(FLD_PERSG);
                            dtFlds.Add(FLD_BEGDA);
                            dtFlds.Add(FLD_AEDTM);

                            dtopts.Add(new RFC_DB_OPT() { Text = "PERNR = '" + Pernr + "'" });

                            proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0001 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0002 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_PERNR);
                            dtFlds.Add(FLD_VORNA);
                            dtFlds.Add(FLD_TITEL);
                            dtFlds.Add(FLD_NATIO);
                            dtFlds.Add(FLD_NACHN);
                            dtFlds.Add(FLD_MIDNM);
                            dtFlds.Add(FLD_RUFNM);
                            dtFlds.Add(FLD_ENDDA);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", " ", "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtFields = dtFlds.ToADODataTable();
                            DataTable dtParameters = dt.ToADODataTable();
                            m_dtPA0002 = ConvertPipesToColumns(dtFields, dtParameters);

                            #endregion

                            #region  PA0006 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_PSTLZ);
                            dtFlds.Add(FLD_SUBTY);
                            dtFlds.Add(FLD_ENDDA);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0006", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0006 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region  PA0185 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_ICNUM);
                            dtFlds.Add(FLD_ENDDA);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0185", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0185 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0077 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_RACKY);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0077", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0077 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0105 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_USRTY);
                            dtFlds.Add(FLD_USRID_LONG);
                            dtFlds.Add(FLD_USRID);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0105 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0027 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_KST01);
                            dtFlds.Add(FLD_KSTAR);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0027", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0027 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0302 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_BEGDA);
                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_MASSN);
                            dtFlds.Add(FLD_MASSG);

                            dtopts.Add(FILTER_DELTA_DATA);

                            proxy.Rfc_Read_Table("|", null, "PA0302", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0302 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion 
                        }

                        user.IdentificationNo = Pernr;


                        rows = (from tmpDataRow in m_dtPA0006.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today && tmpDataRow.Field<string>("SUBTY") == "9000"
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.PostalCode = CheckNull.AgainstString(rows[0]["PSTLZ"].ToString().Trim());
                        }

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && OnBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.EmployeeStatus = CheckNull.AgainstString(rows[0]["STAT2"].ToString().Trim());
                            user.IsActive = ActiveEmployeeFlags.Contains(user.EmployeeStatus) ? true : false;
                        }

                        #region OnBoard scenario
                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && (OnBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim()))
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            string Joindate = CheckNull.AgainstString(rows[0]["BEGDA"]).Trim();
                            if (!string.IsNullOrEmpty(Joindate) && Joindate.Length >= 8)
                            {

                                user.JoinDate = UtilityMethods.GetDateFromString(Joindate);
                                onBoardExists = true;
                                isUserOrgUpdated = this.Z_Bapipa_Get_Tel_Dir_Info(user, Joindate, null);
                                this.UpdateOrgTrack(null, user, sections, companies);

                                var rows_PA0001 = getPA0001(m_dtPA0001, Joindate, null);

                                if (rows_PA0001 != null && rows_PA0001.Count() > 0)
                                {
                                    user.CostCenter = CheckNull.AgainstString(rows_PA0001[0]["KOSTL"]).Trim();
                                    user.PositionCode = CheckNull.AgainstString(rows_PA0001[0]["PLANS"]).Trim();
                                    user.EEGroup = CheckNull.AgainstString(rows_PA0001[0]["PERSG"]).Trim();
                                    user.IsExecutiveEmployee = MasterConfigHelper.ExecutiveEmployeeFlags.Contains(user.EEGroup) ? true : false;
                                }
                            }
                        }
                        #endregion OnBoard scenario

                        #region Retirement & Rejoin scenario
                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && (OnBoardTriggersNotToBeConsideredForJoinDate.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim()))
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            string strBEGDA = CheckNull.AgainstString(rows[0]["BEGDA"]).Trim();

                            
                            var orgTrack_Event = EmployeeEvents.OUChange;
                            var orgTrack_EventCode = CheckNull.AgainstString(rows[0]["MASSN"]);
                            var orgTrack_EventReason = CheckNull.AgainstString(rows[0]["MASSG"]);
                            var orgTrack_EffectiveDate = UtilityMethods.GetDateFromString(CheckNull.AgainstString(rows[0]["BEGDA"]));
                            var orgTrack = new OrgChangeTrack(orgTrack_Event, orgTrack_EventReason, orgTrack_EventCode, orgTrack_EffectiveDate);
                            orgTrack.Comment = "A3";

                            this.Z_Bapipa_Get_Tel_Dir_Info(null, strBEGDA, orgTrack);
                            this.UpdateOrgTrack(orgTrack, null, sections, companies);

                            var rows_PA0001 = getPA0001(m_dtPA0001, strBEGDA, null);

                            if (rows_PA0001 != null && rows_PA0001.Count() > 0)
                            {
                                orgTrack.CostCenter = CheckNull.AgainstString(rows_PA0001[0]["KOSTL"]).Trim();
                            }

                            OrgChangeTrackList.Add(orgTrack);
                        }

                        #endregion Retirement & Rejoin scenario

                        #region OffBoard scenario
                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && OffBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            string termdate = CheckNull.AgainstString(rows[0]["BEGDA"].ToString()).Trim();
                            if (!string.IsNullOrEmpty(termdate) && termdate.Length >= 8)
                            {
                                user.TerminationDate = UtilityMethods.GetDateFromString(termdate);
                                _idmsLogger.WriteLog(LogLevelType.Info, $"Offboard exist for user- {Pernr}");
                                offBoardExists = true;

                            }
                        }
                        #endregion OffBoard scenario

                        #region  Transfer scenario
                        var transferRows = (from tmpDataRow in m_dtPA0302.AsEnumerable()
                                            where //UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today &&

                                            TranferTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                            select tmpDataRow).ToArray<DataRow>();

                        foreach (DataRow transferRow in transferRows)
                        {
                            
                            var orgTrack_Event = EmployeeEvents.OUChange;
                            var orgTrack_EventCode = CheckNull.AgainstString(transferRow["MASSN"]);
                            var orgTrack_EventReason = CheckNull.AgainstString(transferRow["MASSG"]);
                            var orgTrack_EffectiveDate = UtilityMethods.GetDateFromString(CheckNull.AgainstString(transferRow["BEGDA"]));
                            var orgTrack = new OrgChangeTrack(orgTrack_Event, orgTrack_EventReason, orgTrack_EventCode, orgTrack_EffectiveDate);
                            var rows_PA0001 = getPA0001(m_dtPA0001, CheckNull.AgainstString(transferRow["BEGDA"]), null);

                            if (rows_PA0001 != null && rows_PA0001.Count() > 0)
                            {
                                orgTrack.CostCenter = CheckNull.AgainstString(rows_PA0001[0]["KOSTL"]).Trim();
                                this.Z_Bapipa_Get_Tel_Dir_Info(user, CheckNull.AgainstString(transferRow["BEGDA"]), orgTrack);
                                this.UpdateOrgTrack(orgTrack, null, sections, companies);
                                OrgChangeTrackList.Add(orgTrack);
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("Employee movement received from the table PA0302 for the Transfer action for the pernr:  {0}, Event Code: {1}, Event Reason: {2} and Effective Date: {3}", Pernr, orgTrack.EventCode, orgTrack.EventReason, orgTrack.EffectiveDate), DiscoveryObjectType.User);
                            }
                        }
                        #endregion  Transfer scenario

                        #region  Re-Org scenario
                        var reOrgRows = (from tmpDataRow in m_dtPA0302.AsEnumerable()
                                         where // UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today && 
                                         ReOrgTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                         select tmpDataRow).ToArray<DataRow>();

                        foreach (DataRow reorgRow in reOrgRows)
                        {

                            var orgTrack_Event = EmployeeEvents.ReOrganisation;
                            var orgTrack_EventCode = CheckNull.AgainstString(reorgRow["MASSN"]);
                            var orgTrack_EventReason = CheckNull.AgainstString(reorgRow["MASSG"]);
                            var orgTrack_EffectiveDate = UtilityMethods.GetDateFromString(CheckNull.AgainstString(reorgRow["BEGDA"]));
                            var orgTrack = new OrgChangeTrack(orgTrack_Event, orgTrack_EventReason, orgTrack_EventCode, orgTrack_EffectiveDate);

                            var rows_PA0001 = getPA0001(m_dtPA0001, CheckNull.AgainstString(reorgRow["BEGDA"]), null);

                            if (rows_PA0001 != null && rows_PA0001.Count() > 0)
                            {
                                orgTrack.CostCenter = CheckNull.AgainstString(rows_PA0001[0]["KOSTL"]).Trim();
                                this.Z_Bapipa_Get_Tel_Dir_Info(user, CheckNull.AgainstString(reorgRow["BEGDA"]), orgTrack);
                                this.UpdateOrgTrack(orgTrack, null, sections, companies);
                                OrgChangeTrackList.Add(orgTrack);
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("Employee movement received from the table PA0302 for the Transfer action for the pernr:  {0}, Event Code: {1}, Event Reason: {2} and Effective Date: {3}", Pernr, orgTrack.EventCode, orgTrack.EventReason, orgTrack.EffectiveDate), DiscoveryObjectType.User);
                            }
                        }

                        #endregion Re-Org scenario

                        if (!isUserOrgUpdated)
                        {
                            isUserOrgUpdated = this.Z_Bapipa_Get_Tel_Dir_Info(user, null);
                            this.UpdateOrgTrack(null, user, sections, companies);
                        }


                        // Update User information like CostCenter. PositionCode, EEGroup, IsExecutiveEmployee, if no OnBoard exist

                        if (onBoardExists == false)
                        {  
                            var rows_PA0001 = m_dtPA0001.AsEnumerable().Where(tmpDataRow => UtilityMethods.GetDateFromSAPDateString(tmpDataRow.Field<string>("BEGDA"))<= DateTime.Today 
                            && UtilityMethods.GetDateFromSAPDateString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            ).Select(tmpDataRow => tmpDataRow).ToArray<DataRow>();  

                            if (rows_PA0001 != null && rows_PA0001.Count() > 0)
                            {
                                user.CostCenter = CheckNull.AgainstString(rows_PA0001[0]["KOSTL"]).Trim();
                                user.PositionCode = CheckNull.AgainstString(rows_PA0001[0]["PLANS"]).Trim();
                                user.EEGroup = CheckNull.AgainstString(rows_PA0001[0]["PERSG"]).Trim();
                                user.IsExecutiveEmployee = MasterConfigHelper.ExecutiveEmployeeFlags.Contains(user.EEGroup) ? true : false;
                            }

                        }

                        rows = (from tmpDataRow in m_dtPA0027.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                      && tmpDataRow.Field<string>("KSTAR").Trim() == "01"
                                select tmpDataRow).ToArray<DataRow>();


                        if (null != rows && rows.Count() > 0)
                        {
                            int indx = rows.Count() - 1 < 0 ? 0 : rows.Count() - 1;
                            user.CostDistribution = CheckNull.AgainstString(rows[indx]["KST01"]).Trim();
                        }
                        else
                        {
                            user.CostDistribution = string.Empty;
                        }

                        rows = (from tmpDataRow in m_dtPA0002.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && tmpDataRow.Field<string>("PERNR") == Pernr
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.FirstName = CheckNull.AgainstString(rows[0]["VORNA"]).Trim();
                            user.MiddleName = CheckNull.AgainstString(rows[0]["MIDNM"]).Trim();
                            user.LastName = CheckNull.AgainstString(rows[0]["NACHN"]).Trim();
                            user.KnownAs = CheckNull.AgainstString(rows[0]["RUFNM"]).Trim();
                            user.DisplayName = user.KnownAs;
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Error, $"Demographic information is missing for the user - {Pernr}", null);
                            continue;
                        }

                        rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                                where tmpDataRow.Field<string>("USRTY").Trim() == "0010" &&
                                 UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.EmailId = CheckNull.AgainstString(rows[rows.Count() - 1]["USRID_LONG"]).Trim();
                        }
                        else
                        {
                            user.EmailId = string.Empty;
                        }

                        rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                                where tmpDataRow.Field<string>("USRTY").Trim() == "0001" &&
                                 UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.FLINSLoginId = CheckNull.AgainstString(rows[0]["USRID"]).Trim();
                        }
                        else
                        {
                            user.FLINSLoginId = null;
                        }

                        // updating ROEmployeeId

                        rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                                where tmpDataRow.Field<string>("USRTY").Trim() == "9011" &&
                                 UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.ROEmployeeId = CheckNull.AgainstString(rows[0]["USRID"]).Trim().PadLeft(8, '0');
                        }
                        else
                        {
                            user.ROEmployeeId = null;
                        }



                           //Ethinicity
                           rows = (from tmpDataRow in m_dtPA0077.AsEnumerable()
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            user.Ethinicity = rows[0]["RACKY"].ToString().Trim();
                        }
                        else
                        {
                            user.Ethinicity = String.Empty;
                        }

                        costCenter = costCenters.FirstOrDefault(item => item.Code == user.CostCenter && item.CompanyId == user.CompanyId);
                        if (costCenter != null)
                        {
                            user.CostCenterText = costCenter.Description;
                        }



                        position = positions.FirstOrDefault(item => item.Code == user.PositionCode);
                        if (position != null)
                        {
                            user.NBusinessRoleId = position.Id;
                            user.PositionName = position.Description;
                        }

                        user.EmployeeId = Pernr;
                        user.EmployeeType = EmployeeType.Permanent;
                        user.FullTimeEmployee = true;
                        user.EndPointSystemId = this.Endpoint.Id;

                        var terminationDate = user.TerminationDate;
                        var canSaveUser = true;
                        var newJoinDate = user.JoinDate;

                        if (isNewUser)
                        {
                            oldUser = User.GetByNRIC(user.IdentificationNo);
                            isRejoinUser = oldUser != null && oldUser.Id > 0 && oldUser.IsActive == false;

                            if (isRejoinUser)
                            {
                                WriteDiscoveryLog(LogLevelType.Info, $"RejoinUser : {user.EmployeeId}");
                                if (user.TerminationDate.HasValue && user.TerminationDate.Value < DateTime.Today)
                                {
                                    continue;
                                }

                                user.Id = oldUser.Id;
                                user.UserName = oldUser.UserName;
                                user.Password = oldUser.Password;
                                user.EmailId = oldUser.EmailId;
                                user.FLINSLoginId = oldUser.FLINSLoginId;
                                //user.AuthenticationMode = "AD";
                                user.AuthenticationMode = AuthenticationMode;
                                user.OneKeyTokenId = oldUser.OneKeyTokenId;

                                userId = oldUser.Id;
                            }
                            else
                            {
                                if (!user.JoinDate.HasValue)
                                {
                                    WriteDiscoveryLog(LogLevelType.Error, $"HRMS Delta Dsicovery for the PERNR - {user}: Join Date is Null. Hence IDMS cannot proceed the discovery of the user.");
                                    continue;
                                }

                                user.UserName = Pernr;//new logic

                                if (iacmPolicy != null)
                                {
                                    user.Password = RSAHelper.EncryptData(new PasswordResolver().ConstructGeneralUserPassword
                                                            (iacmPolicy.PasswordLength.HasValue ? iacmPolicy.PasswordLength.Value : 8,
                                                            iacmPolicy.NoOfSpecialCharacters.HasValue ? iacmPolicy.NoOfSpecialCharacters.Value : 0,
                                                            iacmPolicy.NoOfLowerCaseAlphabets.HasValue ? iacmPolicy.NoOfLowerCaseAlphabets.Value : 0,
                                                            iacmPolicy.NoOfUpperCaseAlphabets.HasValue ? iacmPolicy.NoOfUpperCaseAlphabets.Value : 0,
                                                            iacmPolicy.NoOfNumericCharacters.HasValue ? iacmPolicy.NoOfNumericCharacters.Value : 0));

                                }

                                user.Save(MasterConfigHelperServiceUser);
                                User.InsertUserRole(user.Id, MasterConfigHelperRoleUser);
                                userId = user.Id;

                            }
                        }
                        else
                        {
                           
                            
                            
                            //same pernr rejoin - deactivate existing off board trigger
                            if (onBoardExists)
                            {
                                RFC_DB_OPT FILTER_PERNR = new RFC_DB_OPT();
                                FILTER_PERNR.Text = "PERNR = '" + Pernr + "'";

                                using (SAPProxy1 proxy = new SAPProxy1())
                                {
                                    proxy.Connection = this.GetSAPConnection();
                                    proxy.Connection.Open();

                                    #region  PA0000 Table

                                    dt = new TAB512Table();
                                    dtFlds = new RFC_DB_FLDTable();
                                    dtopts = new RFC_DB_OPTTable();

                                    dtFlds.Add(FLD_BEGDA);
                                    dtFlds.Add(FLD_ENDDA);
                                    dtFlds.Add(FLD_MASSN);
                                    dtFlds.Add(FLD_MASSG);

                                    dtopts.Add(FILTER_PERNR);

                                    proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                                    dtTemp = dt.ToADODataTable();
                                    dtFields = dtFlds.ToADODataTable();
                                    m_dtPA0000 = ConvertPipesToColumns(dtFields, dtTemp);

                                    #endregion

                                }

                                //var onboardEntries = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                //                      where OnBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                //                      select tmpDataRow).ToArray<DataRow>();


                                //var offboardEntries = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                //                       where OffBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                //                       select tmpDataRow).ToArray<DataRow>();

                                //if (onboardEntries.Count() > 1)
                                //{
                                //    if (onboardEntries.Count() > offboardEntries.Count())
                                //    {
                                //        if (user.TerminationDate.HasValue && user.TerminationDate.Value <= DateTime.Today)
                                //        {
                                //            user.TerminationDate = null;
                                //        }
                                //        isRejoinUser = true;
                                //        canSaveUser = false;
                                //        rejoinWithSamePernrUsers.Add(user.Id);
                                //    }
                                //}

                                var rejoinEntries = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                                     where RejoinTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim()
                                                     && code.EventReason == tmpDataRow.Field<string>("MASSG"))
                                                     && UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                                     select tmpDataRow).ToArray<DataRow>();


                                if (rejoinEntries.Any())
                                {
                                    isRejoinUser = true;
                                    canSaveUser = false;
                                    rejoinWithSamePernrUsers.Add(user.Id);
                                }



                            }

                            if (OrgChangeTrackList == null || OrgChangeTrackList.Count == 0)
                            {
                                User.UpdateUserFromDiscovery(user, propagationFields, canRunPropagationTrigger, MasterConfigHelperServiceUser, canSaveUser);
                            }

                            if (offBoardExists && canSaveUser==false)
                            {
                                oldUser = User.GetByNRIC(user.IdentificationNo);
                                if (oldUser != null && oldUser.IsActive )
                                {                                     
                                    oldUser.TerminationDate = terminationDate;
                                    oldUser.Save(MasterConfigHelperServiceUser);
                                }
                            }
                            if (isRejoinUser)
                            {
                                user.JoinDate = newJoinDate;
                                user.TerminationDate = null;
                            }

                            userId = user.Id;
                        }

                        user.Id = userId;

                        TrackEventTriggers(user, OrgChangeTrackList, isRejoinUser, oldUser, isNewUser, offBoardExists, onBoardExists, rejoinWithSamePernrUsers, terminationDate);

                        if (isNewUser)
                        {
                            IACMUserInfo iACMUserInfo = IacmUserHelper.CreateAndSaveIacmUserInfo(user, null, idms_endpoint, _idmsLogger);
                            if (iACMUserInfo != null)
                            {
                                long endpointId = 0;
                                if (idms_endpoint != null)
                                    endpointId = idms_endpoint.Id;
                                IACMUserInfo.SaveUsers(new List<IACMUserInfo> { iACMUserInfo }, endpointId, MasterConfigHelperServiceUser);
                                var accRec = AccountReconcilation.GetByAccountName(user.UserName, endpointId);
                                if (accRec != null)
                                {
                                    accRec.UserId = user.Id;
                                    accRec.Save(MasterConfigHelperServiceUser);
                                }
                                //IacmUserHelper.MapUserIacmUser(idms_endpoint, true, base.DiscoveryProcess.Id, user.Id.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Error, string.Format("Error while reading the record - {0}", person), ex);
                    }
                }

                EmployeeEventTrigger.ValidateAndCreateTriggers(triggers, rejoinWithSamePernrUsers, MasterConfigHelperServiceUser);

                return employeeIds.Count;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Error in delta getPAInformation", ex);
                throw;
            }
            finally
            {
                this.DisposePATables();

                dt = null;
                dtFlds = null;
                dtopts = null;
                dtTemp = null;
                dtFields = null;
            }
        }

        int GetHRMSUsersFromGivenDate(string scanDate)
        {
            TAB512Table dt = new TAB512Table();
            RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();

            List<string> employeeIds = new List<string>();
            List<Company> companies = new List<Company>();
            List<NCostCenter> costCenters = new List<NCostCenter>();
            List<PersonnelArea> departments = new List<PersonnelArea>();
            List<PersonnelSubArea> divisions = new List<PersonnelSubArea>();
            List<NOrganisation> sections = new List<NOrganisation>();
            List<NDesignation> positions = new List<NDesignation>();

            string Pernr = string.Empty;
            bool isNewUser;

            try
            {
                var OnBoardTriggers = MasterConfigHelper.OnBoardTriggers;
                var ActiveEmployeeFlags = MasterConfigHelper.ActiveEmployeeFlags;
                var OffBoardTriggers = MasterConfigHelper.OffBoardTriggers;



                WriteDiscoveryLog(LogLevelType.Info, "Downloading users from the selected data : " + scanDate);
                string password = RSAHelper.EncryptData(MasterConfigHelper.DefaultPassword);

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    RFC_DB_OPT filter = new RFC_DB_OPT();
                    filter.Text = "AEDTM >= '" + scanDate + "' ";

                    #region  PA0000 Table

                    dtFlds.Add(FLD_PERNR);

                    dtopts.Add(filter);

                    proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion

                    #region PA0001 Table

                    proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion

                    #region PA0002 Table

                    proxy.Rfc_Read_Table("|", null, "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion

                    #region  PA0185 Table

                    proxy.Rfc_Read_Table("|", null, "PA0185", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion

                    #region PA0077 Table

                    proxy.Rfc_Read_Table("|", null, "PA0077", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion

                    #region PA0105 Table

                    proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion

                    #region PA0027 Table

                    proxy.Rfc_Read_Table("|", null, "PA0027", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion

                    #region PA0302 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    dtFlds.Add(FLD_PERNR);
                    dtopts.Add(filter);

                    proxy.Rfc_Read_Table("|", null, "PA0302", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                    #endregion
                }

                foreach (DataRow person in m_dtPA0000AllEmp.Rows)
                {
                    Pernr = person["PERNR"].ToString().Trim();

                    if (!employeeIds.Contains(Pernr))
                    {
                        employeeIds.Add(Pernr);
                    }
                }

                //TRUSTED SOURCE USER INFO


                NCostCenter costCenter = null;
                NDesignation position = null;

                DataRow[] rows;

                if (employeeIds.Count > 0)
                {
                    companies = Company.GetAll();
                    sections = NOrganisation.GetAll();
                    positions = NDesignation.GetAll();
                    costCenters = NCostCenter.GetAll();
                }


                foreach (string person in employeeIds)
                {
                    long userId;

                    bool isRejoinUser = false;

                    User oldUser = null;

                    Pernr = person;

                    var user = User.GetByEmployeeId(Pernr);
                    if (user != null)
                    {
                        isNewUser = false;
                    }
                    else
                    {
                        isNewUser = true;
                        user = new User();
                        user.Reserved4 = Constants.EmployeeStatus.NewEmployee;
                    }

                    RFC_DB_OPT FILTER_DELTA_DATA = new RFC_DB_OPT();
                    FILTER_DELTA_DATA.Text = "PERNR = '" + Pernr + "' AND AEDTM >= '" + scanDate + "'";

                    RFC_DB_OPT FILTER_PERNR_ENDDA = new RFC_DB_OPT();
                    FILTER_PERNR_ENDDA.Text = "PERNR = '" + Pernr + "' AND ENDDA >= '" + scanDate + "'";

                    using (SAPProxy1 proxy = new SAPProxy1())
                    {
                        proxy.Connection = this.GetSAPConnection();
                        proxy.Connection.Open();

                        #region  PA0000 Table

                        dt = new TAB512Table();
                        dtFlds = new RFC_DB_FLDTable();
                        dtopts = new RFC_DB_OPTTable();

                        dtFlds.Add(FLD_BEGDA);
                        dtFlds.Add(FLD_ENDDA);
                        dtFlds.Add(FLD_STAT2);
                        dtFlds.Add(FLD_MASSN);
                        dtFlds.Add(FLD_MASSG);

                        dtopts.Add(FILTER_DELTA_DATA);

                        proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                        dtTemp = dt.ToADODataTable();
                        dtFields = dtFlds.ToADODataTable();
                        m_dtPA0000 = ConvertPipesToColumns(dtFields, dtTemp);

                        #endregion

                        #region PA0001 Table

                        dt = new TAB512Table();
                        dtFlds = new RFC_DB_FLDTable();
                        dtopts = new RFC_DB_OPTTable();

                        dtFlds.Add(FLD_ENDDA);
                        dtFlds.Add(FLD_ENAME);
                        dtFlds.Add(FLD_KOSTL);
                        dtFlds.Add(FLD_BUKRS);
                        dtFlds.Add(FLD_WERKS);
                        dtFlds.Add(FLD_BTRTL);
                        dtFlds.Add(FLD_ORGEH);
                        dtFlds.Add(FLD_PLANS);
                        dtFlds.Add(FLD_PERSG);
                        dtFlds.Add(FLD_BEGDA);
                        dtFlds.Add(FLD_AEDTM);

                        dtopts.Add(FILTER_PERNR_ENDDA);

                        proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                        dtTemp = dt.ToADODataTable();
                        dtFields = dtFlds.ToADODataTable();
                        m_dtPA0001 = ConvertPipesToColumns(dtFields, dtTemp);

                        #endregion

                        #region PA0002 Table

                        dt = new TAB512Table();
                        dtFlds = new RFC_DB_FLDTable();
                        dtopts = new RFC_DB_OPTTable();

                        dtFlds.Add(FLD_PERNR);
                        dtFlds.Add(FLD_VORNA);
                        dtFlds.Add(FLD_TITEL);
                        dtFlds.Add(FLD_NATIO);
                        dtFlds.Add(FLD_NACHN);
                        dtFlds.Add(FLD_MIDNM);
                        dtFlds.Add(FLD_RUFNM);
                        dtFlds.Add(FLD_ENDDA);

                        dtopts.Add(FILTER_PERNR_ENDDA);

                        proxy.Rfc_Read_Table("|", " ", "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                        dtFields = dtFlds.ToADODataTable();
                        DataTable dtParameters = dt.ToADODataTable();
                        m_dtPA0002 = ConvertPipesToColumns(dtFields, dtParameters);

                        #endregion

                        #region  PA0185 Table

                        dt = new TAB512Table();
                        dtFlds = new RFC_DB_FLDTable();
                        dtopts = new RFC_DB_OPTTable();

                        dtFlds.Add(FLD_ICNUM);
                        dtFlds.Add(FLD_ENDDA);

                        dtopts.Add(FILTER_PERNR_ENDDA);

                        proxy.Rfc_Read_Table("|", null, "PA0185", 0, 0, ref dt, ref dtFlds, ref dtopts);
                        dtTemp = dt.ToADODataTable();
                        dtFields = dtFlds.ToADODataTable();
                        m_dtPA0185 = ConvertPipesToColumns(dtFields, dtTemp);

                        #endregion

                        #region PA0077 Table

                        dt = new TAB512Table();
                        dtFlds = new RFC_DB_FLDTable();
                        dtopts = new RFC_DB_OPTTable();

                        dtFlds.Add(FLD_RACKY);

                        dtopts.Add(FILTER_PERNR_ENDDA);

                        proxy.Rfc_Read_Table("|", null, "PA0077", 0, 0, ref dt, ref dtFlds, ref dtopts);
                        dtTemp = dt.ToADODataTable();
                        dtFields = dtFlds.ToADODataTable();
                        m_dtPA0077 = ConvertPipesToColumns(dtFields, dtTemp);

                        #endregion

                        #region PA0105 Table

                        dt = new TAB512Table();
                        dtFlds = new RFC_DB_FLDTable();
                        dtopts = new RFC_DB_OPTTable();

                        dtFlds.Add(FLD_ENDDA);
                        dtFlds.Add(FLD_USRTY);
                        dtFlds.Add(FLD_USRID_LONG);
                        dtFlds.Add(FLD_USRID);

                        dtopts.Add(FILTER_PERNR_ENDDA);

                        proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                        dtTemp = dt.ToADODataTable();
                        dtFields = dtFlds.ToADODataTable();
                        m_dtPA0105 = ConvertPipesToColumns(dtFields, dtTemp);

                        #endregion

                        #region PA0027 Table

                        dt = new TAB512Table();
                        dtFlds = new RFC_DB_FLDTable();
                        dtopts = new RFC_DB_OPTTable();

                        dtFlds.Add(FLD_ENDDA);
                        dtFlds.Add(FLD_KST01);
                        dtFlds.Add(FLD_KSTAR);

                        dtopts.Add(FILTER_PERNR_ENDDA);

                        proxy.Rfc_Read_Table("|", null, "PA0027", 0, 0, ref dt, ref dtFlds, ref dtopts);
                        dtTemp = dt.ToADODataTable();
                        dtFields = dtFlds.ToADODataTable();
                        m_dtPA0027 = ConvertPipesToColumns(dtFields, dtTemp);

                        #endregion

                        #region  PA0302 Table

                        dt = new TAB512Table();
                        dtFlds = new RFC_DB_FLDTable();
                        dtopts = new RFC_DB_OPTTable();

                        dtFlds.Add(FLD_BEGDA);
                        dtFlds.Add(FLD_ENDDA);
                        dtFlds.Add(FLD_MASSN);
                        dtFlds.Add(FLD_MASSG);

                        dtopts.Add(FILTER_DELTA_DATA);

                        proxy.Rfc_Read_Table("|", null, "PA0302", 0, 0, ref dt, ref dtFlds, ref dtopts);
                        dtTemp = dt.ToADODataTable();
                        dtFields = dtFlds.ToADODataTable();
                        m_dtPA0302 = ConvertPipesToColumns(dtFields, dtTemp);

                        #endregion PA0302
                    }

                    user.IdentificationNo = Pernr;


                    rows = (from tmpDataRow in m_dtPA0006.AsEnumerable()
                            where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today && tmpDataRow.Field<string>("SUBTY") == "9000"
                            select tmpDataRow).ToArray<DataRow>();

                    //skip users without nric
                    if (null != rows && rows.Count() > 0)
                    {
                        user.PostalCode = CheckNull.AgainstString(rows[0]["PSTLZ"].ToString().Trim());
                    }

                    rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                            where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            && OnBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                            select tmpDataRow).ToArray<DataRow>();

                    if (null != rows && rows.Count() > 0)
                    {
                        user.EmployeeStatus = CheckNull.AgainstString(rows[0]["STAT2"].ToString().Trim());
                        user.IsActive = ActiveEmployeeFlags.Contains(user.EmployeeStatus) ? true : false;
                    }

                    rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                            where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            && OnBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim()) &&
                            !MasterConfigHelper.OnBoardTriggersNotToBeConsideredForJoinDate.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                            select tmpDataRow).ToArray<DataRow>();

                    if (rows != null && rows.Count() > 0)
                    {
                        string Joindate = CheckNull.AgainstString(rows[0]["BEGDA"].ToString()).Trim();
                        if (!string.IsNullOrEmpty(Joindate) && Joindate.Length >= 8)
                        {
                            user.JoinDate = UtilityMethods.GetDateFromString(Joindate);
                        }
                    }

                    rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                            where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            && OffBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                            select tmpDataRow).ToArray<DataRow>();

                    if (rows != null && rows.Count() > 0)
                    {
                        string termdate = CheckNull.AgainstString(rows[0]["BEGDA"].ToString()).Trim();
                        if (!string.IsNullOrEmpty(termdate) && termdate.Length >= 8)
                        {
                            user.TerminationDate = UtilityMethods.GetDateFromString(termdate);
                            if (user.JoinDate >= user.TerminationDate)
                            {
                                user.TerminationDate = null;
                            }
                        }
                    }
                    else
                    {
                        user.TerminationDate = null;
                    }

                    this.Z_Bapipa_Get_Tel_Dir_Info(user, scanDate);
                    this.UpdateOrgTrack(null, user, sections, companies);
                    rows = (from tmpDataRow in m_dtPA0001.AsEnumerable()
                            select tmpDataRow).ToArray<DataRow>();

                    if (null != rows && rows.Count() > 0)
                    {
                        user.CostCenter = CheckNull.AgainstString(rows[rows.Count() - 1]["KOSTL"]).Trim();

                        user.EEGroup = CheckNull.AgainstString(rows[rows.Count() - 1]["PERSG"]).Trim();
                        user.IsExecutiveEmployee = MasterConfigHelper.ExecutiveEmployeeFlags.Contains(user.EEGroup) ? true : false;
                    }

                    rows = (from tmpDataRow in m_dtPA0027.AsEnumerable()
                            where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                  && tmpDataRow.Field<string>("KSTAR").Trim() == "01"
                            select tmpDataRow).ToArray<DataRow>();
                    if (null != rows && rows.Count() > 0)
                    {
                        user.CostDistribution = CheckNull.AgainstString(rows[0]["KST01"]).Trim();
                    }
                    else
                    {
                        user.CostDistribution = string.Empty;
                    }


                    rows = (from tmpDataRow in m_dtPA0002.AsEnumerable()
                            where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            && tmpDataRow.Field<string>("PERNR") == Pernr
                            select tmpDataRow).ToArray<DataRow>();

                    if (null != rows && rows.Count() > 0)
                    {
                        user.FirstName = CheckNull.AgainstString(rows[0]["VORNA"]).Trim();
                        user.MiddleName = CheckNull.AgainstString(rows[0]["MIDNM"]).Trim();
                        user.LastName = CheckNull.AgainstString(rows[0]["NACHN"]).Trim();
                        user.KnownAs = CheckNull.AgainstString(rows[0]["RUFNM"]).Trim();
                        user.DisplayName = user.KnownAs;
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, $"Demographic information is missing for the user - {Pernr}", null);
                        continue;
                    }

                    rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                            where tmpDataRow.Field<string>("USRTY").Trim() == "0010" &&
                             UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            select tmpDataRow).ToArray<DataRow>();

                    if (null != rows && rows.Count() > 0)
                    {
                        user.EmailId = CheckNull.AgainstString(rows[rows.Count() - 1]["USRID_LONG"]).Trim();
                    }
                    else
                    {
                        user.EmailId = string.Empty;
                    }

                    rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                            where tmpDataRow.Field<string>("USRTY").Trim() == "9011"
                            select tmpDataRow).ToArray<DataRow>();

                    if (null != rows && rows.Count() > 0)
                    {
                        user.ROEmployeeId =  CheckNull.AgainstString(rows[0]["USRID"]).Trim();
                    }
                    else
                    {
                        user.ROEmployeeId = null;
                    }

                    rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                            where tmpDataRow.Field<string>("USRTY").Trim() == "0001"
                            select tmpDataRow).ToArray<DataRow>();

                    if (null != rows && rows.Count() > 0)
                    {
                        user.FLINSLoginId = CheckNull.AgainstString(rows[0]["USRID"]).Trim();
                    }
                    else
                    {
                        user.FLINSLoginId = null;
                    }

                    //Ethinicity
                    rows = (from tmpDataRow in m_dtPA0077.AsEnumerable()
                            select tmpDataRow).ToArray<DataRow>();

                    if (rows != null && rows.Count() > 0)
                    {
                        user.Ethinicity = rows[0]["RACKY"].ToString().Trim();
                    }
                    else
                    {
                        user.Ethinicity = Ethinicity.Chinese.ToString();
                    }

                    costCenter = costCenters.FirstOrDefault(item => item.Code == user.CostCenter && item.CompanyId == user.CompanyId);
                    if (costCenter != null)
                    {
                        user.CostCenterText = costCenter.Description;
                    }

                    position = positions.FirstOrDefault(item => item.Code == user.PositionCode);
                    if (position != null)
                    {
                        user.NBusinessRoleId = position.Id;
                        user.PositionName = position.Description;
                    }

                    user.EmployeeId = Pernr;
                    user.EmployeeType = EmployeeType.Permanent;
                    user.FullTimeEmployee = true;

                    if (isNewUser)
                    {
                        oldUser = User.GetByNRIC(user.IdentificationNo);
                        if (oldUser != null)
                        {
                            isRejoinUser = (!user.TerminationDate.HasValue || (user.TerminationDate.HasValue && user.TerminationDate.Value > DateTime.Today)) && oldUser.TerminationDate.HasValue;
                            //if older record is recent than the newer record then skip writing the newer record.
                            if (!isRejoinUser)
                            {
                                continue;
                            }
                        }

                        if (isRejoinUser)
                        {
                            user.Id = oldUser.Id;
                            user.UserName = oldUser.UserName;
                            user.Password = oldUser.Password;
                            user.EmailId = oldUser.EmailId;
                            user.FLINSLoginId = oldUser.FLINSLoginId;
                            user.AuthenticationMode = oldUser.AuthenticationMode;
                            user.OneKeyTokenId = oldUser.OneKeyTokenId;

                            user.CreatedBy = Constants.CreatedBy;
                            user.CreatedDate = DateTime.Now;
                            user.LastModifiedBy = Constants.CreatedBy;
                            user.LastModifiedDate = DateTime.Now;

                            userId = oldUser.Id;
                            user.Save(MasterConfigHelperServiceUser);

                        }
                        else
                        {
                            user.UserName = Pernr;//new logic
                            user.Password = password;

                            user.CreatedBy = Constants.CreatedBy;
                            user.CreatedDate = DateTime.Now;
                            user.Save(MasterConfigHelperServiceUser);
                            userId = user.Id;
                            int RoleId = MasterConfigHelperRoleUser;
                            User.InsertUserRole(userId, RoleId);
                        }
                    }
                    else
                    {

                        user.LastModifiedBy = Constants.CreatedBy;
                        user.LastModifiedDate = DateTime.Now;
                         User.UpdateUserFromDiscovery(user, PropagationFields, canRunPropagationTrigger, MasterConfigHelperServiceUser);
                        userId = user.Id;
                    }
                    user.Id = userId;
                }

                return employeeIds.Count;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Error while downloading users from the given date.", ex);
                throw;
            }
            finally
            {
                this.DisposePATables();

                dt = null;
                dtFlds = null;
                dtopts = null;
                dtTemp = null;
                dtFields = null;
            }
        }

        void GetUserByPernr(string Pernr)
        {

            TAB512Table dt = new TAB512Table();
            RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();


            try
            {
                WriteDiscoveryLog(LogLevelType.Info, $"Getting Pernr:{Pernr} Information");

                bool isNewUser;

                string password = RSAHelper.EncryptData(MasterConfigHelper.DefaultPassword);

                var companies = Company.GetAll();

                var sections = NOrganisation.GetAll();
                var positions = NDesignation.GetAll();
                var costCenters = NCostCenter.GetAll();

                //TRUSTED SOURCE USER INFO

                NCostCenter costCenter = null;
                NDesignation position = null;

                DataRow[] rows;

                RFC_DB_OPT FILTER_PERNR = new RFC_DB_OPT();
                FILTER_PERNR.Text = "PERNR = '" + Pernr + "' ";

                RFC_DB_OPT FILTER_PERNR_ENDDA = new RFC_DB_OPT();
                FILTER_PERNR_ENDDA.Text = "PERNR = '" + Pernr + "' AND ENDDA >= '" + today + "'";

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    #region  PA0000 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    dtFlds.Add(FLD_BEGDA);
                    dtFlds.Add(FLD_ENDDA);
                    dtFlds.Add(FLD_STAT2);
                    dtFlds.Add(FLD_MASSN);
                    dtFlds.Add(FLD_MASSG);


                    dtopts.Add(FILTER_PERNR);

                    proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0001 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    dtFlds.Add(FLD_BEGDA);
                    dtFlds.Add(FLD_ENDDA);
                    dtFlds.Add(FLD_ENAME);
                    dtFlds.Add(FLD_KOSTL);
                    dtFlds.Add(FLD_BUKRS);
                    dtFlds.Add(FLD_WERKS);
                    dtFlds.Add(FLD_BTRTL);
                    dtFlds.Add(FLD_ORGEH);
                    dtFlds.Add(FLD_PLANS);
                    dtFlds.Add(FLD_PERSG);
                    dtFlds.Add(FLD_AEDTM);

                    dtopts.Add(FILTER_PERNR_ENDDA);

                    proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0001 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0002 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    dtFlds.Add(FLD_PERNR);
                    dtFlds.Add(FLD_VORNA);
                    dtFlds.Add(FLD_TITEL);
                    dtFlds.Add(FLD_NATIO);
                    dtFlds.Add(FLD_NACHN);
                    dtFlds.Add(FLD_MIDNM);
                    dtFlds.Add(FLD_RUFNM);
                    dtFlds.Add(FLD_ENDDA);

                    dtopts.Add(FILTER_PERNR);

                    proxy.Rfc_Read_Table("|", " ", "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtFields = dtFlds.ToADODataTable();
                    DataTable dtParameters = dt.ToADODataTable();
                    m_dtPA0002 = ConvertPipesToColumns(dtFields, dtParameters);

                    #endregion

                    #region  PA0185 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    dtFlds.Add(FLD_ICNUM);
                    dtFlds.Add(FLD_ENDDA);

                    dtopts.Add(FILTER_PERNR);

                    proxy.Rfc_Read_Table("|", null, "PA0185", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0185 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0077 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    dtFlds.Add(FLD_RACKY);

                    dtopts.Add(FILTER_PERNR);

                    proxy.Rfc_Read_Table("|", null, "PA0077", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0077 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0105 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    dtFlds.Add(FLD_ENDDA);
                    dtFlds.Add(FLD_USRTY);
                    dtFlds.Add(FLD_USRID_LONG);
                    dtFlds.Add(FLD_USRID);

                    dtopts.Add(FILTER_PERNR_ENDDA);

                    proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0105 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0027 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    dtFlds.Add(FLD_ENDDA);
                    dtFlds.Add(FLD_KST01);
                    dtFlds.Add(FLD_KSTAR);

                    dtopts.Add(FILTER_PERNR);

                    proxy.Rfc_Read_Table("|", null, "PA0027", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0027 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region  PA0302 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    dtFlds.Add(FLD_BEGDA);
                    dtFlds.Add(FLD_ENDDA);
                    dtFlds.Add(FLD_MASSN);
                    dtFlds.Add(FLD_MASSG);

                    dtopts.Add(FILTER_PERNR_ENDDA);

                    proxy.Rfc_Read_Table("|", null, "PA0302", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtPA0302 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion PA0302
                }

                var user = User.GetByEmployeeId(Pernr);
                if (user != null)
                {
                    isNewUser = false;
                }
                else
                {
                    isNewUser = true;
                    user = new User();
                    user.Reserved4 = Constants.EmployeeStatus.NewEmployee;
                    user.EmployeeId = Pernr;
                }

                this.Z_Bapipa_Get_Tel_Dir_Info(user, today);
                this.UpdateOrgTrack(null, user, sections, companies);

                user.IdentificationNo = Pernr;


                rows = (from tmpDataRow in m_dtPA0006.AsEnumerable()
                        where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today && tmpDataRow.Field<string>("SUBTY") == "9000"
                        select tmpDataRow).ToArray<DataRow>();


                if (null != rows && rows.Count() > 0)
                {
                    user.PostalCode = CheckNull.AgainstString(rows[0]["PSTLZ"].ToString().Trim());
                }

                #region OnBoard
                rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                        where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            && MasterConfigHelper.OnBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                        select tmpDataRow).ToArray<DataRow>();

                if (null != rows && rows.Count() > 0)
                {
                    user.EmployeeStatus = CheckNull.AgainstString(rows[0]["STAT2"].ToString().Trim());
                    user.IsActive = MasterConfigHelper.ActiveEmployeeFlags.Contains(user.EmployeeStatus) ? true : false;
                }

                rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                        where MasterConfigHelper.OnBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                        select tmpDataRow).ToArray<DataRow>();

                if (rows != null && rows.Count() > 0)
                {
                    string Joindate = CheckNull.AgainstString(rows[rows.Count() - 1]["BEGDA"].ToString()).Trim();

                    if (!string.IsNullOrEmpty(Joindate) && Joindate.Length >= 8)
                    {
                        user.JoinDate = UtilityMethods.GetDateFromString(Joindate);
                    }
                }
                else
                {
                    user.JoinDate = null;
                }

                #endregion


                #region Retirement & Rejoining


                //rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                //        where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                //            && MasterConfigHelper.OnBoardTriggersNotToBeConsideredForJoinDate.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                //        select tmpDataRow).ToArray<DataRow>();

                //if (rows != null && rows.Count() > 0)
                //{
                //    string strBEGDA = CheckNull.AgainstString(rows[rows.Count() - 1]["BEGDA"].ToString()).Trim();

                //    var orgTrack = new OrgChangeTrack();
                //    orgTrack.Event = EmployeeEvents.OUChange;
                //    orgTrack.EventCode = CheckNull.AgainstString(rows[0]["MASSN"]);
                //    orgTrack.EventReason = CheckNull.AgainstString(rows[0]["MASSG"]);
                //    orgTrack.EffectiveDate = UtilityMethods.GetDateFromString(CheckNull.AgainstString(rows[0]["BEGDA"]));
                //    orgTrack.Comment = "A3";

                //    this.Z_Bapipa_Get_Tel_Dir_Info(null, strBEGDA, orgTrack);
                //    this.UpdateOrgTrack(orgTrack, null, sections, companies);

                //    var rows_PA0001 = getPA0001(m_dtPA0001, strBEGDA, null);

                //    if (rows_PA0001 != null && rows_PA0001.Count() > 0)
                //    {
                //        orgTrack.CostCenter = CheckNull.AgainstString(rows_PA0001[0]["KOSTL"]).Trim();
                //    }

                //    OrgChangeTrackList.Add(orgTrack);

                //}



                #endregion


                rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                        where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                        && MasterConfigHelper.OffBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                        select tmpDataRow).ToArray<DataRow>();

                if (rows != null && rows.Count() > 0)
                {
                    string termdate = CheckNull.AgainstString(rows[rows.Count() - 1]["BEGDA"].ToString()).Trim();
                    if (!string.IsNullOrEmpty(termdate) && termdate.Length >= 8)
                    {
                        user.TerminationDate = UtilityMethods.GetDateFromString(termdate);
                    }
                }
                else
                {
                    user.TerminationDate = null;
                }

                rows = (from tmpDataRow in m_dtPA0001.AsEnumerable()
                        select tmpDataRow).ToArray<DataRow>();

                if (null != rows && rows.Count() > 0)
                {
                    user.CostCenter = CheckNull.AgainstString(rows[0]["KOSTL"]).Trim();
                    user.PositionCode = CheckNull.AgainstString(rows[0]["PLANS"]).Trim();
                    user.EEGroup = CheckNull.AgainstString(rows[0]["PERSG"]).Trim();
                    user.IsExecutiveEmployee = MasterConfigHelper.ExecutiveEmployeeFlags.Contains(user.EEGroup) ? true : false;
                }

                rows = (from tmpDataRow in m_dtPA0027.AsEnumerable()
                        where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                             && tmpDataRow.Field<string>("KSTAR").Trim() == "01"
                        select tmpDataRow).ToArray<DataRow>();
                if (null != rows && rows.Count() > 0)
                {
                    user.CostDistribution = CheckNull.AgainstString(rows[0]["KST01"]).Trim();
                }
                else
                {
                    user.CostDistribution = string.Empty;
                }

                rows = (from tmpDataRow in m_dtPA0002.AsEnumerable()
                        where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                        && tmpDataRow.Field<string>("PERNR") == Pernr
                        select tmpDataRow).ToArray<DataRow>();

                if (null != rows && rows.Count() > 0)
                {
                    user.FirstName = CheckNull.AgainstString(rows[0]["VORNA"]).Trim();
                    user.MiddleName = CheckNull.AgainstString(rows[0]["MIDNM"]).Trim();
                    user.LastName = CheckNull.AgainstString(rows[0]["NACHN"]).Trim();
                    user.KnownAs = CheckNull.AgainstString(rows[0]["RUFNM"]).Trim();
                    user.DisplayName = user.KnownAs;
                }
                else
                {
                    _idmsLogger.WriteLog(LogLevelType.Error, $"Demographic information is missing for the user - {Pernr}", null);
                }

                rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                        where tmpDataRow.Field<string>("USRTY").Trim() == "0010" &&
                         UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                        select tmpDataRow).ToArray<DataRow>();

                if (null != rows && rows.Count() > 0)
                {
                    user.EmailId = CheckNull.AgainstString(rows[rows.Count() - 1]["USRID_LONG"]).Trim();
                }
                else
                {
                    user.EmailId = string.Empty;
                }

                rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                        where tmpDataRow.Field<string>("USRTY").Trim() == "9011"
                        select tmpDataRow).ToArray<DataRow>();

                if (null != rows && rows.Count() > 0)
                {
                    user.ROEmployeeId =  CheckNull.AgainstString(rows[0]["USRID"]).Trim();
                }
                else
                {
                    user.ROEmployeeId =  null;
                }

            rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                        where tmpDataRow.Field<string>("USRTY").Trim() == "0001"
                        select tmpDataRow).ToArray<DataRow>();

                if (null != rows && rows.Count() > 0)
                {
                    user.FLINSLoginId = CheckNull.AgainstString(rows[0]["USRID"]).Trim();
                }
                else
                {
                    user.FLINSLoginId = null;
                }

                //Ethinicity
                rows = (from tmpDataRow in m_dtPA0077.AsEnumerable()
                        select tmpDataRow).ToArray<DataRow>();

                if (rows != null && rows.Count() > 0)
                {
                    user.Ethinicity = rows[0]["RACKY"].ToString().Trim();
                }
                else
                {
                    user.Ethinicity = Ethinicity.Chinese.ToString();
                }

                costCenter = costCenters.FirstOrDefault(item => item.Code == user.CostCenter && item.CompanyId == user.CompanyId);
                if (costCenter != null)
                {
                    user.CostCenterText = costCenter.Description;
                }


                position = positions.FirstOrDefault(item => item.Code == user.PositionCode);
                if (position != null)
                {
                    user.NBusinessRoleId = position.Id;
                    user.PositionName = position.Description;
                }

                user.FullTimeEmployee = true;
                user.EmployeeType = EmployeeType.Permanent;
                user.EmployeeId = Pernr;
                bool isRejoinUser = false;
                long userId = 0;

                if (isNewUser)
                {
                    user.Password = password;

                    var oldUser = User.GetByNRIC(user.IdentificationNo);
                    if (oldUser != null)
                    {
                        isRejoinUser = (!user.TerminationDate.HasValue || (user.TerminationDate.HasValue && user.TerminationDate.Value > DateTime.Today)) && oldUser.TerminationDate.HasValue;
                        //if older record is recent than the newer record then skip writing the newer record.
                        if (!isRejoinUser)
                        {
                            return;
                        }
                    }

                    if (isRejoinUser)
                    {
                        user.Id = oldUser.Id;
                        user.UserName = oldUser.UserName;
                        user.Password = oldUser.Password;
                        user.EmailId = oldUser.EmailId;
                        user.FLINSLoginId = oldUser.FLINSLoginId;
                        user.OneKeyTokenId = oldUser.OneKeyTokenId;
                        user.AuthenticationMode = oldUser.AuthenticationMode;

                        user.CreatedBy = Constants.CreatedBy;
                        user.CreatedDate = DateTime.Now;
                        user.LastModifiedBy = Constants.CreatedBy;
                        user.LastModifiedDate = DateTime.Now;

                        userId = oldUser.Id;

                        user.LastModifiedBy = Constants.CreatedBy;
                        user.LastModifiedDate = DateTime.Now;
                        user.Save(MasterConfigHelperServiceUser);
                    }
                    else
                    {
                        user.UserName = Pernr;//new logic
                        user.Password = password;
                        user.EmployeeId = Pernr;
                        user.CreatedBy = Constants.CreatedBy;
                        user.CreatedDate = DateTime.Now;
                        user.Save(MasterConfigHelperServiceUser);
                        userId = user.Id;
                        int RoleId = MasterConfigHelperRoleUser;
                        User.InsertUserRole(userId, RoleId);
                    }
                }
                else
                {
                    user.LastModifiedBy = Constants.CreatedBy;
                    user.LastModifiedDate = DateTime.Now;
                    //user.Save(MasterConfigHelperServiceUser);
                    User.UpdateUserFromDiscovery(user, PropagationFields, canRunPropagationTrigger, MasterConfigHelperServiceUser);
                    userId = user.Id;
                    //var referenceUsr = User.get
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, $"Error while getting specific user information. Stack Trace:{ex.StackTrace}", ex);
                throw ex;
            }
            finally
            {
                this.DisposePATables();

                dt = null;
                dtFlds = null;
                dtopts = null;
                dtTemp = null;
                dtFields = null;
            }
        }

        int DiscoverUserByPernr(string inPernr)
        {

            TAB512Table dt = new TAB512Table();
            RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();

            List<string> employeeIds = new List<string>();
            List<Company> companies = new List<Company>();
            List<NCostCenter> costCenters = new List<NCostCenter>();
            List<PersonnelArea> departments = new List<PersonnelArea>();
            List<PersonnelSubArea> divisions = new List<PersonnelSubArea>();
            List<NOrganisation> sections = new List<NOrganisation>();
            List<NDesignation> positions = new List<NDesignation>();

            triggers = new List<EmployeeEventTrigger>();

            List<long> rejoinWithSamePernrUsers = new List<long>();
            List<OrgChangeTrack> OrgChangeTrackList = new List<OrgChangeTrack>();
            string Pernr = string.Empty;
            bool isNewUser, isOrgTrackUpdated;

            var propagationFields = MasterConfigHelper.PropagationFields;
            bool canRunPropagationTrigger = MasterConfigHelper.CanRunPropagationTrigger;

            try
            {
                PasswordPolicy iacmPolicy = PasswordPolicy.GetById(1);

                _idmsLogger.WriteLog(LogLevelType.Info, $"Discovering specific user information : {inPernr}");

                employeeIds = inPernr.Split(',').ToList();





                NOrganisation section = null;
                NDesignation position = null;

                DataRow[] rows;

                if (employeeIds.Count > 0)
                {
                    companies = Company.GetAll();
                    sections = NOrganisation.GetAll();
                    positions = NDesignation.GetAll();
                    costCenters = NCostCenter.GetAll();
                }

                foreach (string person in employeeIds)
                {
                    try
                    {

                        long userId = 0;

                        bool isMovementExists = false;
                        bool isRejoinUser = false;
                        bool offBoardExists = false;
                        bool onBoardExists = false;


                        User oldUser = null;

                        Pernr = person;

                        User user = User.GetByEmployeeId(Pernr, this.Endpoint.Id);
                        isOrgTrackUpdated = false;

                        if (user != null)
                        {
                            isNewUser = false;
                        }
                        else
                        {
                            isNewUser = true;
                            user = new User();
                            user.Reserved4 = Constants.EmployeeStatus.NewEmployee;
                            user.EmployeeId = Pernr;
                        }

                        RFC_DB_OPT FILTER_DELTA_DATA = new RFC_DB_OPT();
                        FILTER_DELTA_DATA.Text = "PERNR = '" + Pernr + "' AND AEDTM >= '" + deltaDate + "'";

                        RFC_DB_OPT FILTER_PERNR_ENDDA = new RFC_DB_OPT();
                        FILTER_PERNR_ENDDA.Text = "PERNR = '" + Pernr + "' AND ENDDA >= '" + deltaDate + "'";

                        using (SAPProxy1 proxy = new SAPProxy1())
                        {
                            proxy.Connection = this.GetSAPConnection();
                            proxy.Connection.Open();

                            #region  PA0000 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_BEGDA);
                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_STAT2);
                            dtFlds.Add(FLD_MASSN);
                            dtFlds.Add(FLD_MASSG);

                            dtopts.Add(FILTER_DELTA_DATA);

                            proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0000 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0001 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_ENAME);

                            dtopts.Add(new RFC_DB_OPT() { Text = "PERNR = '" + Pernr + "'" });

                            proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0001 = ConvertPipesToColumns(dtFields, dtTemp);




                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_ENAME);
                            dtFlds.Add(FLD_KOSTL);
                            dtFlds.Add(FLD_BUKRS);
                            dtFlds.Add(FLD_WERKS);
                            dtFlds.Add(FLD_BTRTL);
                            dtFlds.Add(FLD_ORGEH);
                            dtFlds.Add(FLD_PLANS);
                            dtFlds.Add(FLD_PERSG);
                            dtFlds.Add(FLD_AEDTM);
                            dtFlds.Add(FLD_BEGDA);


                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0001 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0002 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_PERNR);
                            dtFlds.Add(FLD_VORNA);
                            dtFlds.Add(FLD_TITEL);
                            dtFlds.Add(FLD_NATIO);
                            dtFlds.Add(FLD_NACHN);
                            dtFlds.Add(FLD_MIDNM);
                            dtFlds.Add(FLD_RUFNM);
                            dtFlds.Add(FLD_ENDDA);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", " ", "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtFields = dtFlds.ToADODataTable();
                            DataTable dtParameters = dt.ToADODataTable();
                            m_dtPA0002 = ConvertPipesToColumns(dtFields, dtParameters);

                            #endregion

                            #region  PA0006 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_PSTLZ);
                            dtFlds.Add(FLD_SUBTY);
                            dtFlds.Add(FLD_ENDDA);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0006", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0006 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region  PA0185 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_ICNUM);
                            dtFlds.Add(FLD_ENDDA);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0185", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0185 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0077 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_RACKY);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0077", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0077 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0105 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_USRTY);
                            dtFlds.Add(FLD_USRID_LONG);
                            dtFlds.Add(FLD_USRID);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0105 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0027 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_KST01);
                            dtFlds.Add(FLD_KSTAR);

                            dtopts.Add(FILTER_PERNR_ENDDA);

                            proxy.Rfc_Read_Table("|", null, "PA0027", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0027 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                            #region PA0302 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            dtFlds.Add(FLD_BEGDA);
                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_MASSN);
                            dtFlds.Add(FLD_MASSG);

                            dtopts.Add(FILTER_DELTA_DATA);

                            proxy.Rfc_Read_Table("|", null, "PA0302", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_dtPA0302 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion

                        }

                        user.IdentificationNo = Pernr;

                        //skip users without nric
                        if (string.IsNullOrEmpty(user.IdentificationNo))
                        {
                            _idmsLogger.WriteLog(LogLevelType.Error, $"NRIC is empty or missing for the user - {Pernr}");
                            continue;
                        }

                        rows = (from tmpDataRow in m_dtPA0006.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today && tmpDataRow.Field<string>("SUBTY") == "9000"
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.PostalCode = CheckNull.AgainstString(rows[0]["PSTLZ"].ToString().Trim());
                        }

                        #region OnBoard

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && MasterConfigHelper.OnBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.EmployeeStatus = CheckNull.AgainstString(rows[0]["STAT2"].ToString().Trim());
                            user.IsActive = MasterConfigHelper.ActiveEmployeeFlags.Contains(user.EmployeeStatus) ? true : false;
                        }


                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && MasterConfigHelper.OnBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            string Joindate = CheckNull.AgainstString(rows[0]["BEGDA"].ToString()).Trim();
                            if (!string.IsNullOrEmpty(Joindate) && Joindate.Length >= 8)
                            {

                                user.JoinDate = UtilityMethods.GetDateFromString(Joindate);
                                onBoardExists = true;
                                this.Z_Bapipa_Get_Tel_Dir_Info(user, Joindate);
                                this.UpdateOrgTrack(null, user, sections, companies);
                                var rows_PA0001 = getPA0001(m_dtPA0001, Joindate, null);
                                if (rows_PA0001 != null && rows_PA0001.Count() > 0)
                                {
                                    user.CostCenter = CheckNull.AgainstString(rows_PA0001[0]["KOSTL"]).Trim();
                                    user.PositionCode = CheckNull.AgainstString(rows_PA0001[0]["PLANS"]).Trim();
                                    user.EEGroup = CheckNull.AgainstString(rows_PA0001[0]["PERSG"]).Trim();
                                    user.IsExecutiveEmployee = MasterConfigHelper.ExecutiveEmployeeFlags.Contains(user.EEGroup) ? true : false;
                                }

                            }
                            else
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("Join date for the PERNR - {0} is {1}", person, user.JoinDate), DiscoveryObjectType.None);

                            }
                        }

                        #endregion OnBoard


                        #region Retirement & Rejoining

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && MasterConfigHelper.OnBoardTriggersNotToBeConsideredForJoinDate.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            string strBEGDA = CheckNull.AgainstString(rows[0]["BEGDA"]).Trim();


                            var orgTrack_Event = EmployeeEvents.OUChange;
                            var orgTrack_EventCode = CheckNull.AgainstString(rows[0]["MASSN"]);
                            var orgTrack_EventReason = CheckNull.AgainstString(rows[0]["MASSG"]);
                            var orgTrack_EffectiveDate = UtilityMethods.GetDateFromString(CheckNull.AgainstString(rows[0]["BEGDA"]));
                            var orgTrack = new OrgChangeTrack(orgTrack_Event, orgTrack_EventReason, orgTrack_EventCode, orgTrack_EffectiveDate);
                            orgTrack.Comment = "A3";

                            this.Z_Bapipa_Get_Tel_Dir_Info(null, strBEGDA, orgTrack);
                            this.UpdateOrgTrack(orgTrack, null, sections, companies);

                            var rows_PA0001 = getPA0001(m_dtPA0001, strBEGDA, null);

                            if (rows_PA0001 != null && rows_PA0001.Count() > 0)
                            {
                                orgTrack.CostCenter = CheckNull.AgainstString(rows_PA0001[0]["KOSTL"]).Trim();
                            }

                            OrgChangeTrackList.Add(orgTrack);

                        }

                        #endregion  Retirement & Rejoining

                        #region OffBoard

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && MasterConfigHelper.OffBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            string termdate = CheckNull.AgainstString(rows[0]["BEGDA"].ToString()).Trim();
                            if (!string.IsNullOrEmpty(termdate) && termdate.Length >= 8)
                            {
                                user.TerminationDate = UtilityMethods.GetDateFromString(termdate);

                                offBoardExists = true;
                            }
                        }


                        #endregion OffBoard



                        #region Transfer 

                        var transferRows = (from tmpDataRow in m_dtPA0302.AsEnumerable()
                                            where MasterConfigHelper.TranferTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                            select tmpDataRow).ToArray<DataRow>();




                        if (transferRows != null && transferRows.Count() > 0)
                        {
                            foreach (DataRow transferRow in transferRows)
                            {
                                
                                var orgTrack_Event = EmployeeEvents.OUChange;
                                var orgTrack_EventCode = CheckNull.AgainstString(transferRow["MASSN"]);
                                var orgTrack_EventReason = CheckNull.AgainstString(transferRow["MASSG"]);
                                var orgTrack_EffectiveDate = UtilityMethods.GetDateFromString(CheckNull.AgainstString(transferRow["BEGDA"]));

                                var orgTrack = new OrgChangeTrack(orgTrack_Event, orgTrack_EventReason, orgTrack_EventCode, orgTrack_EffectiveDate);

                                var rows_PA0001 = getPA0001(m_dtPA0001, CheckNull.AgainstString(transferRow["BEGDA"]), null);
                                
                                if (rows_PA0001 != null && rows_PA0001.Count() > 0)
                                {
                                    orgTrack.CostCenter = CheckNull.AgainstString(rows_PA0001[0]["KOSTL"]).Trim();

                                    this.Z_Bapipa_Get_Tel_Dir_Info(user, CheckNull.AgainstString(transferRow["BEGDA"]), orgTrack);
                                    this.UpdateOrgTrack(orgTrack, null, sections, companies);
                                    OrgChangeTrackList.Add(orgTrack);
                                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("Employee movement received from the table PA0302 for the Re-org action for the pernr:  {0}, Event Code: {1}, Event Reason: {2} and Effective Date: {3}", Pernr, orgTrack.EventCode, orgTrack.EventReason, orgTrack.EffectiveDate), DiscoveryObjectType.User);
                                }
                            }
                        }

                        #endregion Transfer

                        #region Re-Org

                        var reorgRows = (from tmpDataRow in m_dtPA0302.AsEnumerable()
                                         where MasterConfigHelper.ReOrgTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG").Trim())
                                         select tmpDataRow).ToArray<DataRow>();



                        if (reorgRows != null && reorgRows.Count() > 0)
                        {
                            foreach (DataRow reorgRow in reorgRows)
                            {

                                var orgTrack_Event = EmployeeEvents.ReOrganisation;
                                var orgTrack_EventCode = CheckNull.AgainstString(reorgRow["MASSN"]);
                                var orgTrack_EventReason = CheckNull.AgainstString(reorgRow["MASSG"]);
                                var orgTrack_EffectiveDate = UtilityMethods.GetDateFromString(CheckNull.AgainstString(reorgRow["BEGDA"]));
                                var orgTrack = new OrgChangeTrack(orgTrack_Event, orgTrack_EventReason, orgTrack_EventCode, orgTrack_EffectiveDate);
                                var rows_PA0001 = getPA0001(m_dtPA0001, CheckNull.AgainstString(reorgRow["BEGDA"]), null);
                                if (rows_PA0001 != null && rows_PA0001.Count() > 0)
                                {
                                    orgTrack.CostCenter = CheckNull.AgainstString(rows_PA0001[0]["KOSTL"]).Trim();

                                    this.Z_Bapipa_Get_Tel_Dir_Info(user, CheckNull.AgainstString(reorgRow["BEGDA"]), orgTrack);
                                    this.UpdateOrgTrack(orgTrack, null, sections, companies);
                                    OrgChangeTrackList.Add(orgTrack);
                                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("Employee movement received from the table PA0302 for the Re-org action for the pernr:  {0}, Event Code: {1}, Event Reason: {2} and Effective Date: {3}", Pernr, orgTrack.EventCode, orgTrack.EventReason, orgTrack.EffectiveDate), DiscoveryObjectType.User);
                                }
                            }
                        }
                        #endregion Re-Org



                        if (onBoardExists == false && (OrgChangeTrackList == null || OrgChangeTrackList.Count == 0))
                        {
                            Z_Bapipa_Get_Tel_Dir_Info(user, DateTime.Now.ToString("yyyyMMdd"));
                            UpdateOrgTrack(null, user, sections, companies);
                        }

                        rows = (from tmpDataRow in m_dtPA0027.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                      && tmpDataRow.Field<string>("KSTAR").Trim() == "01"
                                select tmpDataRow).ToArray<DataRow>();


                        if (null != rows && rows.Count() > 0)
                        {
                            user.CostDistribution = CheckNull.AgainstString(rows[0]["KST01"]).Trim();
                        }
                        else
                        {
                            user.CostDistribution = string.Empty;
                        }

                        rows = (from tmpDataRow in m_dtPA0002.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && tmpDataRow.Field<string>("PERNR") == Pernr
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.FirstName = CheckNull.AgainstString(rows[0]["VORNA"]).Trim();
                            user.MiddleName = CheckNull.AgainstString(rows[0]["MIDNM"]).Trim();
                            user.LastName = CheckNull.AgainstString(rows[0]["NACHN"]).Trim();
                            user.KnownAs = CheckNull.AgainstString(rows[0]["RUFNM"]).Trim();
                            user.DisplayName = user.KnownAs;
                        }
                        else
                        {
                            _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Demographic information is missing for the user - {0}", Pernr), null);
                            continue;
                        }

                        rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                                where tmpDataRow.Field<string>("USRTY").Trim() == "0010" &&
                                 UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.EmailId = CheckNull.AgainstString(rows[rows.Count() - 1]["USRID_LONG"]).Trim();
                        }
                        else
                        {
                            user.EmailId = string.Empty;
                        }

                        rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                                where tmpDataRow.Field<string>("USRTY").Trim() == "0001" &&
                                 UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.FLINSLoginId = CheckNull.AgainstString(rows[0]["USRID"]).Trim();
                        }
                        else
                        {
                            user.FLINSLoginId = null;
                        }

                        //Ethinicity
                        rows = (from tmpDataRow in m_dtPA0077.AsEnumerable()
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            user.Ethinicity = rows[0]["RACKY"].ToString().Trim();
                        }
                        else
                        {
                            user.Ethinicity = String.Empty;
                        }

                        //Ethinicity
                        rows = (from tmpDataRow in m_dtPA0077.AsEnumerable()
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            user.Ethinicity = rows[0]["RACKY"].ToString().Trim();
                        }
                        else
                        {
                            user.Ethinicity = String.Empty;
                        }



                        var costCenter = costCenters.FirstOrDefault(item => item.Code == user.CostCenter && item.CompanyId == user.CompanyId);
                        if (costCenter != null)
                        {
                            user.CostCenterText = costCenter.Description;
                        }

                        position = positions.FirstOrDefault(item => item.Code == user.PositionCode);
                        if (position != null)
                        {
                            user.NBusinessRoleId = position.Id;
                            user.PositionName = position.Description;
                        }



                        user.EmployeeId = Pernr;
                        user.EmployeeType = EmployeeType.Permanent;
                        user.FullTimeEmployee = true;
                        user.EndPointSystemId = this.Endpoint.Id;

                        var canSaveUser = true;
                        var terminationDate = user.TerminationDate;
                        var newJoinDate = user.JoinDate;

                        if (isNewUser)
                        {
                            oldUser = User.GetByNRIC(user.IdentificationNo);
                            isRejoinUser = oldUser != null && oldUser.IsActive == false;

                            if (isRejoinUser)
                            {
                                if (user.TerminationDate.HasValue && user.TerminationDate.Value < DateTime.Today)
                                {
                                    continue;
                                }

                                user.Id = oldUser.Id;
                                user.UserName = oldUser.UserName;
                                user.Password = oldUser.Password;
                                user.EmailId = oldUser.EmailId;
                                user.FLINSLoginId = oldUser.FLINSLoginId;
                                user.AuthenticationMode = MasterConfigHelper.AuthenticationMode;
                                //user.AuthenticationMode = "AD";
                                user.OneKeyTokenId = oldUser.OneKeyTokenId;

                                userId = oldUser.Id;
                            }
                            else
                            {
                                if (!user.JoinDate.HasValue)
                                {
                                    continue;
                                }

                                user.UserName = Pernr;//new logic

                                if (iacmPolicy != null)
                                {
                                    user.Password = RSAHelper.EncryptData(new PasswordResolver().ConstructGeneralUserPassword
                                                            (iacmPolicy.PasswordLength.HasValue ? iacmPolicy.PasswordLength.Value : 8,
                                                            iacmPolicy.NoOfSpecialCharacters.HasValue ? iacmPolicy.NoOfSpecialCharacters.Value : 0,
                                                            iacmPolicy.NoOfLowerCaseAlphabets.HasValue ? iacmPolicy.NoOfLowerCaseAlphabets.Value : 0,
                                                            iacmPolicy.NoOfUpperCaseAlphabets.HasValue ? iacmPolicy.NoOfUpperCaseAlphabets.Value : 0,
                                                            iacmPolicy.NoOfNumericCharacters.HasValue ? iacmPolicy.NoOfNumericCharacters.Value : 0));

                                }

                                user.Save(MasterConfigHelperServiceUser);
                                User.InsertUserRole(user.Id, MasterConfigHelperRoleUser);
                                userId = user.Id;
                            }
                        }
                        else
                        {

                            //same pernr rejoin - deactivate existing off board trigger
                            if (onBoardExists)
                            {
                                RFC_DB_OPT FILTER_PERNR = new RFC_DB_OPT();
                                FILTER_PERNR.Text = "PERNR = '" + Pernr + "'";

                                using (SAPProxy1 proxy = new SAPProxy1())
                                {
                                    proxy.Connection = this.GetSAPConnection();
                                    proxy.Connection.Open();

                                    #region  PA0000 Table

                                    dt = new TAB512Table();
                                    dtFlds = new RFC_DB_FLDTable();
                                    dtopts = new RFC_DB_OPTTable();

                                    dtFlds.Add(FLD_BEGDA);
                                    dtFlds.Add(FLD_ENDDA);
                                    dtFlds.Add(FLD_MASSN);
                                    dtFlds.Add(FLD_MASSG);

                                    dtopts.Add(FILTER_PERNR);

                                    proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                                    dtTemp = dt.ToADODataTable();
                                    dtFields = dtFlds.ToADODataTable();
                                    m_dtPA0000 = ConvertPipesToColumns(dtFields, dtTemp);

                                    #endregion

                                }

                                //var onboardEntries = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                //                      where MasterConfigHelper.OnBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG"))
                                //                      select tmpDataRow).ToArray<DataRow>();


                                //var offboardEntries = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                //                       where MasterConfigHelper.OffBoardTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim() && code.EventReason == tmpDataRow.Field<string>("MASSG"))
                                //                       select tmpDataRow).ToArray<DataRow>();

                                //if (onboardEntries.Count() > 1)
                                //{
                                //    if (onboardEntries.Count() > offboardEntries.Count())
                                //    {
                                //        if (user.TerminationDate.HasValue && user.TerminationDate.Value <= DateTime.Today)
                                //        {
                                //            user.TerminationDate = null;
                                //        }
                                //        isRejoinUser = true;
                                //        canSaveUser = false;
                                //        rejoinWithSamePernrUsers.Add(user.Id);
                                //    }
                                //}


                                var rejoinEntries = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                                     where MasterConfigHelper.RejoinTriggers.Any(code => code.EventType == tmpDataRow.Field<string>("MASSN").Trim()
                                                     && code.EventReason == tmpDataRow.Field<string>("MASSG"))
                                                     && UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                                     select tmpDataRow).ToArray<DataRow>();


                                if (rejoinEntries.Any())
                                {
                                    isRejoinUser = true;
                                    canSaveUser = false;
                                    rejoinWithSamePernrUsers.Add(user.Id);
                                }

                            }


                            //if (OrgChangeTrackList == null || OrgChangeTrackList.Count == 0)
                            //{

                            User.UpdateUserFromDiscovery(user, propagationFields, canRunPropagationTrigger, MasterConfigHelperServiceUser, canSaveUser);

                            //}

                            if (offBoardExists && canSaveUser==false)
                            {
                                oldUser = User.GetByNRIC(user.IdentificationNo);
                                if (oldUser != null && oldUser.IsActive)
                                { 
                                    oldUser.TerminationDate = terminationDate;
                                    oldUser.Save(MasterConfigHelperServiceUser);
                                }
                            }
                            if (isRejoinUser)
                            {
                                user.JoinDate = newJoinDate;
                                user.TerminationDate = null;
                            }
                            userId = user.Id;
                        }

                        user.Id = userId;

                        TrackEventTriggers(user, OrgChangeTrackList, isRejoinUser, oldUser, isNewUser, offBoardExists, onBoardExists, rejoinWithSamePernrUsers,terminationDate);
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Error, string.Format("Error while reading the record - {0}", person), ex);
                    }
                }

                EmployeeEventTrigger.ValidateAndCreateTriggers(triggers, rejoinWithSamePernrUsers, MasterConfigHelperServiceUser);

                return 1;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Error in delta getPAInformation", ex);
                throw;
            }
            finally
            {
                this.DisposePATables();

                dt = null;
                dtFlds = null;
                dtopts = null;
                dtTemp = null;
                dtFields = null;
            }
        }

        void DisposePATables()
        {
            m_dtPA0000AllEmp = null;

            m_dtPA0000 = null;
            m_dtPA0001 = null;
            m_dtPA0002 = null;
            m_dtPA0006 = null;

            m_dtPA0185 = null;
            m_dtPA0077 = null;
            m_dtPA0105 = null;
            m_dtPA0027 = null;
            m_dtHRP1000 = null;

            m_dtT001 = null;
            m_dtT500P = null;
            m_dtT001P = null;
            m_dtCSKS = null;
            m_dtPA0302 = null;
        }

        void PopulateCompanyCodes()
        {
            try
            {
                Dictionary<string, string> companies = new Dictionary<string, string>();

                TAB512Table dt = new TAB512Table();
                RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                RFC_DB_FLD FLD_BUKRS = new RFC_DB_FLD();
                FLD_BUKRS.Fieldname = "BUKRS";

                RFC_DB_FLD FLD_BUTXT = new RFC_DB_FLD();
                FLD_BUTXT.Fieldname = "BUTXT";

                dtFlds.Add(FLD_BUKRS);
                dtFlds.Add(FLD_BUTXT);

                RFC_DB_OPT filter = new RFC_DB_OPT();
                filter.Text = "SPRAS = 'E' OR SPRAS = ''";
                dtopts.Add(filter);

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    proxy.Rfc_Read_Table("|", null, "T001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    DataTable dtTemp = dt.ToADODataTable();
                    DataTable dtFields = dtFlds.ToADODataTable();
                    m_dtT001 = ConvertPipesToColumns(dtFields, dtTemp);
                }

                foreach (DataRow row in m_dtT001.Rows)
                {
                    string code = row["BUKRS"].ToString().Trim();
                    string text = row["BUTXT"].ToString().Trim();

                    if (!companies.ContainsKey(code))
                    {
                        companies.Add(code, text);
                    }
                }

                foreach (KeyValuePair<string, string> company in companies)
                {
                    var comp = Company.GetByCode(company.Key);
                    if (comp == null)
                    {
                        Company cmpany = new Company()
                        {
                            CreatedBy = Constants.CreatedBy,
                            CreatedDate = DateTime.Now,
                            Code = company.Key,
                            Text = company.Value,
                            IsActive = true,
                        };

                        cmpany.Save(MasterConfigHelperServiceUser);
                    }
                    else
                    {
                        comp.Text = company.Value;
                        comp.LastModifiedBy = Constants.CreatedBy;
                        comp.LastModifiedDate = DateTime.Now;

                        comp.Save(MasterConfigHelperServiceUser);
                    }
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Trusted Source companies populated", companies.Count.ToString()));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Error Occured in populate companies", ex);
            }
        }

        void PopulatePersonnelAreas()
        {
            try
            {
                Dictionary<string, string> personnelAreas = new Dictionary<string, string>();

                TAB512Table dt = new TAB512Table();
                RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                RFC_DB_FLD FLD_PERSA = new RFC_DB_FLD();
                FLD_PERSA.Fieldname = "PERSA";

                RFC_DB_FLD FLD_NAME1 = new RFC_DB_FLD();
                FLD_NAME1.Fieldname = "NAME1";

                dtFlds.Add(FLD_PERSA);
                dtFlds.Add(FLD_NAME1);

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    proxy.Rfc_Read_Table("|", null, "T500P", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    DataTable dtTemp = dt.ToADODataTable();
                    DataTable dtFields = dtFlds.ToADODataTable();
                    m_dtT500P = ConvertPipesToColumns(dtFields, dtTemp);
                }

                foreach (DataRow row in m_dtT500P.Rows)
                {
                    string code = row["PERSA"].ToString().Trim();
                    string text = row["NAME1"].ToString().Trim();

                    if (!personnelAreas.ContainsKey(code))
                    {
                        personnelAreas.Add(code, text);
                    }
                }

                foreach (KeyValuePair<string, string> personnelArea in personnelAreas)
                {
                    var perArea = PersonnelArea.GetByCode(personnelArea.Key);
                    if (perArea == null)
                    {
                        PersonnelArea pa = new PersonnelArea()
                        {
                            CreatedBy = Constants.CreatedBy,
                            CreatedDate = DateTime.Now,
                            Code = personnelArea.Key,
                            Text = personnelArea.Value,
                            IsActive = true,
                        };

                        pa.Save(MasterConfigHelperServiceUser);
                    }
                    else
                    {
                        perArea.Text = personnelArea.Value;
                        perArea.LastModifiedBy = Constants.CreatedBy;
                        perArea.LastModifiedDate = DateTime.Now;

                        perArea.Save(MasterConfigHelperServiceUser);
                    }
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Trusted Source Personnel Areas populated", personnelAreas.Count.ToString()));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Error Occured in populate personnel areas", ex);
            }
        }

        void PopulatePersonnelSubAreas()
        {
            try
            {
                List<PersonnelSubArea> personnelSubAreas = new List<PersonnelSubArea>();
                var departments = PersonnelArea.GetAll();

                TAB512Table dt = new TAB512Table();
                RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                RFC_DB_FLD FLD_WERKS = new RFC_DB_FLD();
                FLD_WERKS.Fieldname = "WERKS";

                RFC_DB_FLD FLD_BTRTL = new RFC_DB_FLD();
                FLD_BTRTL.Fieldname = "BTRTL";

                RFC_DB_FLD FLD_BTEXT = new RFC_DB_FLD();
                FLD_BTEXT.Fieldname = "BTEXT";

                dtFlds.Add(FLD_WERKS);
                dtFlds.Add(FLD_BTRTL);
                dtFlds.Add(FLD_BTEXT);

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    proxy.Rfc_Read_Table("|", null, "T001P", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    DataTable dtTemp = dt.ToADODataTable();
                    DataTable dtFields = dtFlds.ToADODataTable();
                    m_dtT001P = ConvertPipesToColumns(dtFields, dtTemp);
                }

                foreach (DataRow row in m_dtT001P.Rows)
                {
                    string code = row["BTRTL"].ToString().Trim();
                    string text = row["BTEXT"].ToString().Trim();
                    string deptCode = row["WERKS"].ToString().Trim();

                    //replace '-' symbol with '---' to avoid UI errors.
                    text = text.Equals("-") ? "---" : text;

                    var department = departments.FirstOrDefault(item => item.Code == deptCode);
                    personnelSubAreas.Add(new PersonnelSubArea() { Code = code, PersonnelAreaId = department != null ? department.Id : 0, Text = text });
                }

                foreach (PersonnelSubArea personnelSubArea in personnelSubAreas)
                {
                    var perSubArea = PersonnelSubArea.GetByCodeAndPersonnelAreaId(personnelSubArea.Code, personnelSubArea.PersonnelAreaId);
                    if (perSubArea == null)
                    {
                        PersonnelSubArea psa = new PersonnelSubArea()
                        {
                            CreatedBy = Constants.CreatedBy,
                            CreatedDate = DateTime.Now,
                            Code = personnelSubArea.Code,
                            Text = personnelSubArea.Text,
                            PersonnelAreaId = personnelSubArea.PersonnelAreaId,
                            IsActive = true,
                        };

                        psa.Save(MasterConfigHelperServiceUser);
                    }
                    else
                    {
                        perSubArea.Text = personnelSubArea.Text;
                        perSubArea.LastModifiedBy = Constants.CreatedBy;
                        perSubArea.LastModifiedDate = DateTime.Now;

                        perSubArea.Save(MasterConfigHelperServiceUser);
                    }
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Trusted Source Personnel Sub Areas populated", personnelSubAreas.Count.ToString()));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, "Error Occured in populate personnel sub areas", ex);
            }
        }

        void PopulateCostCenters()
        {
            try
            {
                List<NCostCenter> costCenters = new List<NCostCenter>();
                var companies = Company.GetAll();

                TAB512Table dt = new TAB512Table();
                RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                RFC_DB_FLD FLD_KOSTL = new RFC_DB_FLD();
                FLD_KOSTL.Fieldname = "KOSTL";

                RFC_DB_FLD FLD_BUKRS = new RFC_DB_FLD();
                FLD_BUKRS.Fieldname = "BUKRS";

                dtFlds.Add(FLD_KOSTL);
                dtFlds.Add(FLD_BUKRS);

                RFC_DB_OPT filter = new RFC_DB_OPT();
                filter.Text = "(SPRAS = 'E' OR SPRAS = '') AND DATBI >= '" + today + "'";
                dtopts.Add(filter);

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    proxy.Rfc_Read_Table("|", null, "CSKS", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    DataTable dtTemp = dt.ToADODataTable();
                    DataTable dtFields = dtFlds.ToADODataTable();
                    m_dtCSKS = ConvertPipesToColumns(dtFields, dtTemp);
                }

                foreach (DataRow row in m_dtCSKS.Rows)
                {
                    string costCenterCode = row["KOSTL"].ToString().Trim();
                    string companyCode = row["BUKRS"].ToString().Trim();

                    var company = companies.FirstOrDefault(item => item.Code == companyCode);
                    costCenters.Add(new NCostCenter() { Code = costCenterCode, CompanyId = company != null ? company.Id : 0 });
                }


                foreach (NCostCenter costCenter in costCenters)
                {
                    var costCntr = NCostCenter.GetByCodeAndCompany(costCenter.Code, costCenter.CompanyId);

                    if (costCntr == null)
                    {

                        NCostCenter cc = new NCostCenter()
                        {
                            IsActive = true,
                            CreatedBy = Constants.CreatedBy,
                            CreatedDate = DateTime.Now,
                            LastModifiedBy = Constants.CreatedBy,
                            LastModifiedDate = DateTime.Now,
                            Code = costCenter.Code,
                            CompanyId = costCenter.CompanyId,

                        };
                        cc.Save(MasterConfigHelperServiceUser);
                    }
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Trusted Source Cost Centers populated"));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate cost centers", ex);
            }
        }

        void PopulateCostCenterTexts()
        {
            try
            {
                var companies = Company.GetAll();

                TAB512Table dt = new TAB512Table();
                RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                RFC_DB_FLD FLD_KOSTL = new RFC_DB_FLD();
                FLD_KOSTL.Fieldname = "KOSTL";

                RFC_DB_FLD FLD_KOKRS = new RFC_DB_FLD();
                FLD_KOKRS.Fieldname = "KOKRS";

                RFC_DB_FLD FLD_LTEXT = new RFC_DB_FLD();
                FLD_LTEXT.Fieldname = "LTEXT";

                dtFlds.Add(FLD_KOSTL);
                dtFlds.Add(FLD_KOKRS);
                dtFlds.Add(FLD_LTEXT);

                RFC_DB_OPT filter = new RFC_DB_OPT();
                filter.Text = "(SPRAS = 'E' OR SPRAS = '') AND DATBI >= '" + today + "'";
                dtopts.Add(filter);

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    proxy.Rfc_Read_Table("|", null, "CSKT", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    DataTable dtTemp = dt.ToADODataTable();
                    DataTable dtFields = dtFlds.ToADODataTable();
                    m_dtCSKT = ConvertPipesToColumns(dtFields, dtTemp);
                }

                foreach (DataRow row in m_dtCSKT.Rows)
                {
                    string costCenterCode = row["KOSTL"].ToString().Trim();
                    string companyCode = row["KOKRS"].ToString().Trim();
                    string costCenterText = row["LTEXT"].ToString().Trim();


                    var company = companies.FirstOrDefault(item => item.Code == companyCode);

                    if (company != null)
                    {
                        var costCntr = NCostCenter.GetByCodeAndCompany(costCenterCode, company.Id);

                        if (costCntr != null)
                        {
                            costCntr.Description = costCenterText;
                            costCntr.Save(MasterConfigHelperServiceUser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate cost centers", ex);
            }
        }

        void PopulateOrganizations()
        {
            try
            {
                Dictionary<string, string> orgUnits = new Dictionary<string, string>();

                TAB512Table dt = new TAB512Table();
                RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                RFC_DB_FLD FLD_ORGEH = new RFC_DB_FLD();
                FLD_ORGEH.Fieldname = "OBJID";

                RFC_DB_FLD FLD_ORGTX = new RFC_DB_FLD();
                FLD_ORGTX.Fieldname = "STEXT";

                RFC_DB_FLD FLD_SHORT = new RFC_DB_FLD();
                FLD_SHORT.Fieldname = "SHORT";

                dtFlds.Add(FLD_ORGEH);
                dtFlds.Add(FLD_ORGTX);
                dtFlds.Add(FLD_SHORT);

                RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "(LANGU = 'E' OR LANGU = '') AND OTYPE = 'O' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
                filter.Text = "(LANGU = 'E' OR LANGU = '') AND OTYPE = 'O' AND ENDDA >= '" + today + "' ";

                dtopts.Add(filter);

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    proxy.Rfc_Read_Table("|", null, "HRP1000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    DataTable dtTemp = dt.ToADODataTable();
                    DataTable dtFields = dtFlds.ToADODataTable();
                    m_dtHRP1000 = ConvertPipesToColumns(dtFields, dtTemp);
                }

                foreach (DataRow row in m_dtHRP1000.Rows)
                {
                    string code = row["OBJID"].ToString().Trim();
                    string text = row["STEXT"].ToString().Trim();
                    string scope = row["SHORT"].ToString().Trim();

                    if (!orgUnits.ContainsKey(code))
                    {
                        orgUnits.Add(code, text + '|' + scope);
                    }
                }

                var listScopes = NOrganisationScope.GetAll();

                foreach (KeyValuePair<string, string> orgUnit in orgUnits)
                {
                    var nOrg = NOrganisation.GetByOrgCode(orgUnit.Key);
                    string scope = orgUnit.Value.Split('|')[1];
                    long scopeId = listScopes.Where(t => t.Scope == scope).Select(l => l.Id).FirstOrDefault();

                    if (nOrg == null)
                    {
                        NOrganisation org = new NOrganisation()
                        {
                            IsActive = true,
                            CreatedBy = Constants.CreatedBy,
                            CreatedDate = DateTime.Now,
                            LastModifiedBy = orgUnit.Value.Split('|')[1],
                            LastModifiedDate = DateTime.Now,
                            Code = orgUnit.Key,
                            Name = orgUnit.Value.Split('|')[0],
                        };

                        if (scopeId > 0)
                        {
                            org.ScopeId = scopeId;
                        }

                        org.Save(MasterConfigHelperServiceUser);
                    }
                    else
                    {
                        if (scopeId > 0)
                        {
                            // update if value changed
                            if (nOrg.ScopeId != scopeId)
                            {
                                nOrg.ScopeId = scopeId;
                            }
                        }

                        nOrg.Name = orgUnit.Value.Split('|')[0];
                        nOrg.LastModifiedBy = orgUnit.Value.Split('|')[1];
                        nOrg.LastModifiedDate = DateTime.Now;
                        nOrg.Save(MasterConfigHelperServiceUser);
                    }
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Trusted Source Organisation units populated", orgUnits.Count.ToString()));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate organisation units", ex);
            }
        }

        void PopulateOrganizationHead()
        {
            try
            {

                //flush head and parent org informaation before re-populating it.
                NOrganisation.FlushHeadInformation();
                this.NPopulateRedHats();
                this.PopulateParentOrganisation();

                //Populate RO and CO

                this.PopulateRO();
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate organisation unit heads", ex);
            }
        }

        public void PopulateRO()
        {
            TAB512Table dt = new TAB512Table();
            RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();

            DataTable m_dtAllEmps = new DataTable();
            DataTable m_PA0105 = new DataTable();

            var managerRoleId = MasterConfigHelper.DefaultManagerRole;

            try
            {
                var propagationFields = MasterConfigHelper.PropagationFields;
                _idmsLogger.WriteLog(LogLevelType.Info, "Getting RO and CO information");

                string Pernr = string.Empty;

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    #region  All Emp PA0000 Table

                    RFC_DB_FLD FLD_PERNR = new RFC_DB_FLD();
                    FLD_PERNR.Fieldname = "PERNR";

                    dtFlds.Add(FLD_PERNR);

                    RFC_DB_OPT filter = new RFC_DB_OPT();
                    filter.Text = "ENDDA >= '" + today + "' ";

                    dtopts.Add(filter);

                    proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_dtAllEmps = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion
                }

                foreach (DataRow person in m_dtAllEmps.Rows)
                {
                    try
                    {
                        DataRow[] rows;

                        Pernr = person["PERNR"].ToString().Trim();

                        var user = User.GetByEmployeeId(Pernr);

                        if (user == null)
                        {
                            continue;
                        }

                        RFC_DB_OPT FILTER_PERNR = new RFC_DB_OPT();
                        FILTER_PERNR.Text = "PERNR = '" + Pernr + "' ";

                        using (SAPProxy1 proxy = new SAPProxy1())
                        {
                            proxy.Connection = this.GetSAPConnection();
                            proxy.Connection.Open();

                            #region PA0105 Table

                            dt = new TAB512Table();
                            dtFlds = new RFC_DB_FLDTable();
                            dtopts = new RFC_DB_OPTTable();

                            RFC_DB_FLD FLD_ENDDA = new RFC_DB_FLD();
                            FLD_ENDDA.Fieldname = "ENDDA";

                            RFC_DB_FLD FLD_USRTY = new RFC_DB_FLD();
                            FLD_USRTY.Fieldname = "USRTY";

                            RFC_DB_FLD FLD_USRID = new RFC_DB_FLD();
                            FLD_USRID.Fieldname = "USRID";

                            dtFlds.Add(FLD_ENDDA);
                            dtFlds.Add(FLD_USRTY);
                            dtFlds.Add(FLD_USRID);

                            dtopts.Add(FILTER_PERNR);

                            proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                            dtTemp = dt.ToADODataTable();
                            dtFields = dtFlds.ToADODataTable();
                            m_PA0105 = ConvertPipesToColumns(dtFields, dtTemp);

                            #endregion
                        }
                         rows = (from tmpDataRow in m_PA0105.AsEnumerable()
                                where tmpDataRow.Field<string>("USRTY").Trim() == "9011" &&
                                 UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            string roPernr = CheckNull.AgainstString(rows[0]["USRID"]).Trim().PadLeft(8, '0');
                            var ro = User.GetByEmployeeId(roPernr);

                            if (ro != null)
                            {
                                if (ro.IsActive)
                                {
                                    user.ROEmployeeId = roPernr;
                                }
                                else
                                {
                                    string rosROPernr = this.GetROByPernr(roPernr);

                                    if (!string.IsNullOrEmpty(rosROPernr))
                                    {
                                        var rosRO = User.GetByEmployeeId(rosROPernr);

                                        if (rosRO != null)
                                        {
                                            if (rosRO.IsActive)
                                            {
                                                user.ROEmployeeId = rosROPernr;
                                            }
                                            else
                                            {
                                                user.ROEmployeeId = null;
                                            }
                                        }
                                        else
                                        {
                                            user.ROEmployeeId = null;
                                        }
                                    }
                                    else
                                    {
                                        user.ROEmployeeId = null;
                                    }
                                }
                                User.InsertUserRole(ro.Id, managerRoleId);
                            }
                            else
                            {
                                user.ROEmployeeId = null;
                            }
                        }
                        else
                        {
                            user.ROEmployeeId = null;
                        }

                        //rows = (from tmpDataRow in m_PA0105.AsEnumerable()
                        //        where tmpDataRow.Field<string>("USRTY").Trim() == "9012" &&
                        //         UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                        //        select tmpDataRow).ToArray<DataRow>();

                        //if (null != rows && rows.Count() > 0)
                        //{
                        //    user.COEmployeeId = CheckNull.AgainstString(rows[0]["USRID"]).Trim().PadLeft(8, '0');
                        //}
                        //else
                        //{
                        //    user.COEmployeeId = null;
                        //}

                        //user.Save(MasterConfigHelperServiceUser);
                        
                        User.UpdateUserFromDiscovery(user, propagationFields, canRunPropagationTrigger, MasterConfigHelperServiceUser, true);

                        if (user.ROEmployeeId == null)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Info, string.Format("ROEmployeeId  for - {0} is Null. Hence IDMS is unable to derive COEmployeeId.", Pernr));
                        }
                        else
                        {
                            User.DeriveCOEmployeeId(user.Id);
                        }

                        //User.UpdateROCOForReferenceUsers(user.ReferenceUserId, user.ROEmployeeId, user.COEmployeeId);
                    }
                    catch (Exception ex)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Error, string.Format("Error while getting RO and CO for - {0}", Pernr), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, "Error in while discovering RO and CO information", ex);
                throw;
            }
            finally
            {
                dt = null;
                dtFlds = null;
                dtopts = null;
                dtTemp = null;
                dtFields = null;
                m_dtAllEmps = null;
                m_PA0105 = null;
            }
        }

        string GetROByPernr(string pernr)
        {
            TAB512Table dt = new TAB512Table();
            RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();

            DataTable m_PA0105 = new DataTable();

            try
            {
                DataRow[] rows;

                RFC_DB_OPT FILTER_PERNR = new RFC_DB_OPT();
                FILTER_PERNR.Text = "PERNR = '" + pernr + "' ";

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    #region PA0105 Table

                    dt = new TAB512Table();
                    dtFlds = new RFC_DB_FLDTable();
                    dtopts = new RFC_DB_OPTTable();

                    RFC_DB_FLD FLD_ENDDA = new RFC_DB_FLD();
                    FLD_ENDDA.Fieldname = "ENDDA";

                    RFC_DB_FLD FLD_USRTY = new RFC_DB_FLD();
                    FLD_USRTY.Fieldname = "USRTY";

                    RFC_DB_FLD FLD_USRID = new RFC_DB_FLD();
                    FLD_USRID.Fieldname = "USRID";

                    dtFlds.Add(FLD_ENDDA);
                    dtFlds.Add(FLD_USRTY);
                    dtFlds.Add(FLD_USRID);

                    dtopts.Add(FILTER_PERNR);

                    proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    dtTemp = dt.ToADODataTable();
                    dtFields = dtFlds.ToADODataTable();
                    m_PA0105 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion
                }

                rows = (from tmpDataRow in m_PA0105.AsEnumerable()
                        where tmpDataRow.Field<string>("USRTY").Trim() == "9011" &&
                         UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                        select tmpDataRow).ToArray<DataRow>();

                if (null != rows && rows.Count() > 0)
                {
                    return CheckNull.AgainstString(rows[0]["USRID"]).Trim().PadLeft(8, '0');
                }
                else
                {

                    return null;
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, "Error in while discovering RO and CO information", ex);
                throw;
            }
            finally
            {
                dt = null;
                dtFlds = null;
                dtopts = null;
                dtTemp = null;
                dtFields = null;
                m_PA0105 = null;
            }
        }

        void NPopulateRedHats()
        {
            try
            {
                TAB512Table dt = new TAB512Table();
                RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();
                m_dtHRP1001 = new DataTable();

                RFC_DB_FLD FLD_SOBID = new RFC_DB_FLD();
                FLD_SOBID.Fieldname = "SOBID";

                RFC_DB_FLD FLD_OBJID = new RFC_DB_FLD();
                FLD_OBJID.Fieldname = "OBJID";

                dtFlds.Add(FLD_OBJID);
                dtFlds.Add(FLD_SOBID);

                RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "RSIGN = 'B' AND RELAT = '012' AND OTYPE = 'O' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
                filter.Text = "RSIGN = 'B' AND RELAT = '012' AND OTYPE = 'O' AND ENDDA >= '" + today + "' ";

                dtopts.Add(filter);

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    proxy.Rfc_Read_Table("|", null, "HRP1001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    DataTable dtTemp = dt.ToADODataTable();
                    DataTable dtFields = dtFlds.ToADODataTable();
                    m_dtHRP1001 = ConvertPipesToColumns(dtFields, dtTemp);
                }

                DataTable dtPositions = this.GetPernrAndPositionMappings();

                if (dtPositions != null && dtPositions.Rows.Count != 0)
                {
                    foreach (DataRow row in m_dtHRP1001.Rows)
                    {
                        string positionCode = row["SOBID"].ToString().Trim();
                        string sectionCode = row["OBJID"].ToString().Trim();

                        string pernr = this.GetEmployeeIdByPosition(positionCode, dtPositions);

                        if (!string.IsNullOrEmpty(pernr))
                        {
                            var user = User.GetByEmployeeId(pernr);
                            var section = NOrganisation.GetByOrgCode(sectionCode);
                            if (user != null && section != null)
                            {

                                var nOrgHead = NOrganisationHead.GetByOrganisationId(section.Id, false);

                                if (nOrgHead != null)
                                {
                                    // Only if head id has changed, update
                                    if (nOrgHead.HeadId != user.Id)
                                    {
                                        nOrgHead.HeadId = user.Id; // set new value
                                        nOrgHead.CoordinatorId = user.Id;
                                        nOrgHead.Save(MasterConfigHelperServiceUser);
                                    }
                                }
                                else
                                {
                                    NOrganisationHead head = new NOrganisationHead();
                                    head.OrganisationId = section.Id;
                                    head.HeadId = user.Id;
                                    head.CoordinatorId = user.Id;
                                    head.Save(MasterConfigHelperServiceUser);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate organisation unit heads", ex);
            }
        }

        DataTable GetPernrAndPositionMappings()
        {
            string today = DateTime.Today.ToString("yyyyMMdd");
            try
            {
                TAB512Table dt = new TAB512Table();
                RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();
                DataTable m_dtHRP1001 = new DataTable();

                RFC_DB_FLD FLD_SOBID = new RFC_DB_FLD();
                FLD_SOBID.Fieldname = "SOBID";

                RFC_DB_FLD FLD_OBJID = new RFC_DB_FLD();
                FLD_OBJID.Fieldname = "OBJID";

                dtFlds.Add(FLD_OBJID);
                dtFlds.Add(FLD_SOBID);

                RFC_DB_OPT filter = new RFC_DB_OPT();
                filter.Text = string.Format("RSIGN = 'A' AND RELAT = '008' AND OTYPE = 'S' AND ENDDA >= '{0}'", today);

                dtopts.Add(filter);

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    proxy.Rfc_Read_Table("|", null, "HRP1001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    DataTable dtTemp = dt.ToADODataTable();
                    DataTable dtFields = dtFlds.ToADODataTable();
                    m_dtHRP1001 = ConvertPipesToColumns(dtFields, dtTemp);
                }

                return m_dtHRP1001;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate position to pernr mappings.", ex);
                return null;
            }
        }

        string GetEmployeeIdByPosition(string positionCode, DataTable dtPositions)
        {
            try
            {
                var rows = (from tmpDataRow in dtPositions.AsEnumerable()
                            where tmpDataRow.Field<string>("OBJID").Trim() == positionCode
                            select tmpDataRow).ToArray<DataRow>();

                if (rows != null && rows.Count() != 0)
                {
                    return rows[0]["SOBID"].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Error while finding pernr for the given position code - {0}.", positionCode), ex);
                return string.Empty;
            }
        }

        void PopulateParentOrganisation()
        {
            try
            {
                TAB512Table dt = new TAB512Table();
                RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();
                m_dtHRP1001 = new DataTable();

                RFC_DB_FLD FLD_SOBID = new RFC_DB_FLD();
                FLD_SOBID.Fieldname = "SOBID";

                RFC_DB_FLD FLD_OBJID = new RFC_DB_FLD();
                FLD_OBJID.Fieldname = "OBJID";

                dtFlds.Add(FLD_OBJID);
                dtFlds.Add(FLD_SOBID);

                RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "RSIGN = 'A' AND SCLAS = 'O' AND OTYPE = 'O' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
                filter.Text = "RSIGN = 'A' AND SCLAS = 'O' AND OTYPE = 'O' AND ENDDA >= '" + today + "' ";

                dtopts.Add(filter);

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    proxy.Rfc_Read_Table("|", null, "HRP1001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    DataTable dtTemp = dt.ToADODataTable();
                    DataTable dtFields = dtFlds.ToADODataTable();
                    m_dtHRP1001 = ConvertPipesToColumns(dtFields, dtTemp);
                }

                foreach (DataRow row in m_dtHRP1001.Rows)
                {
                    string parentSectionCode = row["SOBID"].ToString().Trim();
                    string sectionCode = row["OBJID"].ToString().Trim();


                    var nOrg = NOrganisation.GetByOrgCode(sectionCode);
                    //var nParentOrg = NHRMSOrganisation.GetByOrgCode(parentSectionCode);

                    if (nOrg != null)
                    {
                        nOrg.ParentOrgCode = parentSectionCode;
                        nOrg.Save(MasterConfigHelperServiceUser);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate organisation unit heads", ex);
            }
        }

        void PopulateBusinessRoles()
        {
            try
            {
                Dictionary<string, string> positions = new Dictionary<string, string>();

                TAB512Table dt = new TAB512Table();
                RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                RFC_DB_FLD FLD_PLANS = new RFC_DB_FLD();
                FLD_PLANS.Fieldname = "OBJID";

                RFC_DB_FLD FLD_STEXT = new RFC_DB_FLD();
                FLD_STEXT.Fieldname = "STEXT";

                dtFlds.Add(FLD_PLANS);
                dtFlds.Add(FLD_STEXT);

                RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "(LANGU = 'E' OR LANGU = '') AND OTYPE = 'S' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
                filter.Text = "(LANGU = 'E' OR LANGU = '') AND OTYPE = 'S' AND ENDDA >= '" + today + "' ";

                dtopts.Add(filter);

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    proxy.Rfc_Read_Table("|", null, "HRP1000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    DataTable dtTemp = dt.ToADODataTable();
                    DataTable dtFields = dtFlds.ToADODataTable();
                    m_dtHRP1000 = ConvertPipesToColumns(dtFields, dtTemp);
                }

                foreach (DataRow row in m_dtHRP1000.Rows)
                {
                    string code = row["OBJID"].ToString().Trim();
                    string text = row["STEXT"].ToString().Trim();

                    if (!positions.Keys.Contains(code))
                    {
                        positions.Add(code, text);
                    }
                }

                foreach (KeyValuePair<string, string> position in positions)
                {
                    var nPos = NDesignation.GetByCode(position.Key);
                    if (nPos == null)
                    {
                        NDesignation dsg = new NDesignation()
                        {
                            Code = position.Key,
                            Description = position.Value,
                            IsActive = true,
                            CreatedBy = Constants.CreatedBy,
                            CreatedDate = DateTime.Now,
                            LastModifiedBy = Constants.CreatedBy,
                            LastModifiedDate = DateTime.Now,
                        };
                        dsg.Save(MasterConfigHelperServiceUser);
                    }
                    else
                    {
                        nPos.Description = position.Value;
                        nPos.LastModifiedBy = "System";
                        nPos.LastModifiedDate = DateTime.Now;
                        nPos.Save(MasterConfigHelperServiceUser);
                    }
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Trusted Source Business Role populated", positions.Count.ToString()));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate Business Roles", ex);
            }
        }

        void PopulateBusinessRolesDelta()
        {
            try
            {
                Dictionary<string, string> positions = new Dictionary<string, string>();

                TAB512Table dt = new TAB512Table();
                RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                RFC_DB_FLD FLD_PLANS = new RFC_DB_FLD();
                FLD_PLANS.Fieldname = "OBJID";

                RFC_DB_FLD FLD_STEXT = new RFC_DB_FLD();
                FLD_STEXT.Fieldname = "STEXT";

                dtFlds.Add(FLD_PLANS);
                dtFlds.Add(FLD_STEXT);

                RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "(LANGU = 'E' OR LANGU = '') AND OTYPE = 'S' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
                filter.Text = "(LANGU = 'E' OR LANGU = '') AND OTYPE = 'S' AND AEDTM >= '" + deltaDate + "' ";

                dtopts.Add(filter);

                using (SAPProxy1 proxy = new SAPProxy1())
                {
                    proxy.Connection = this.GetSAPConnection();
                    proxy.Connection.Open();

                    proxy.Rfc_Read_Table("|", null, "HRP1000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    DataTable dtTemp = dt.ToADODataTable();
                    DataTable dtFields = dtFlds.ToADODataTable();
                    m_dtHRP1000 = ConvertPipesToColumns(dtFields, dtTemp);
                }

                foreach (DataRow row in m_dtHRP1000.Rows)
                {
                    string code = row["OBJID"].ToString().Trim();
                    string text = row["STEXT"].ToString().Trim();

                    if (!positions.Keys.Contains(code))
                    {
                        positions.Add(code, text);
                    }
                }

                foreach (KeyValuePair<string, string> position in positions)
                {
                    var nPos = NDesignation.GetByCode(position.Key);
                    if (nPos == null)
                    {
                        NDesignation dsg = new NDesignation()
                        {
                            Code = position.Key,
                            Description = position.Value,
                            IsActive = true,
                            CreatedBy = Constants.CreatedBy,
                            CreatedDate = DateTime.Now,
                            LastModifiedBy = Constants.CreatedBy,
                            LastModifiedDate = DateTime.Now,
                        };
                        dsg.Save(MasterConfigHelperServiceUser);
                    }
                    else
                    {
                        nPos.Description = position.Value;
                        nPos.Save(MasterConfigHelperServiceUser);
                    }
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Trusted Source Business Role populated", positions.Count.ToString()));
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} Business Role(s) populated", positions.Count.ToString()));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate Business Roles", ex);
            }
        }

        void UpdateUserPAInfo()
        {
            try
            {
                User.UpdateUserPAInfo();
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, "Error while updating user PA information", ex);
            }
        }

        void TrackEventTriggers(User user, List<OrgChangeTrack> orgTrackList, bool isRejoiningUser, User oldUser, bool isNewUser, bool offBoardExists, bool onBoardExists, List<long> rejoinWithSamePernrUsers, DateTime? TerminationDate)
        {
            //trigger on board event if the join date is greater than equal to today during first time discovery
            if (onBoardExists)
            {
                var trigger = this.PopulateOnBoardTrigger(user, isRejoiningUser, oldUser, isNewUser, onBoardExists, rejoinWithSamePernrUsers);


                if (trigger != null && trigger.Id == 0)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Employee Event Trigger Created: Event Type :{trigger.EventType.ToString()}, Effective Date :{trigger.EffectiveDate} for the PERNR - {user.EmployeeId}");

                    triggers.Add(trigger);
                }
            }

            //trigger off board event if the termination date is greater than equal to today during first time discovery
            if (offBoardExists)
            {
                user.TerminationDate = TerminationDate;
                if (user.TerminationDate.HasValue && user.TerminationDate.Value.Date != Constants.SAP_MAX_DATE)
                {
                    var trigger = this.PopulateOffBoardTrigger(user);

                    if (trigger != null && trigger.Id == 0)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Employee Event Trigger  Created: Event Type :{trigger.EventType.ToString()}, Effective Date :{trigger.EffectiveDate} for the PERNR - {user.EmployeeId}");

                        triggers.Add(trigger);
                    }
                }
            }

            //trigger ou change/re-org event
            if (orgTrackList != null && orgTrackList.Count > 0)
            {
                foreach (var orgTrack in orgTrackList)
                {

                    var trigger = this.PopulateOUChangeTrigger(user, orgTrack);


                    if (trigger != null && trigger.Id == 0)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Employee Event Trigger  Created: Event Type :{trigger.EventType.ToString()}, Effective Date :{trigger.EffectiveDate} for the PERNR - {user.EmployeeId}");

                        triggers.Add(trigger);
                    }
                }
            }

        }

        EmployeeEventTrigger PopulateOnBoardTrigger(User user, bool isRejoiningUser, User oldUser, bool isNewUser, bool onBoardExists, List<long> rejoinWithSamePernrUsers)
        {
            try
            {
                _idmsLogger.WriteLog(LogLevelType.Info, $"Onboard trigger started for user - {user.EmployeeId} and the Join date is {user.JoinDate}");

                EmployeeEvents evnt = EmployeeEvents.NewJoinee;
                if (isRejoiningUser)
                {
                    oldUser = User.GetByNRIC(user.IdentificationNo);
                    if (oldUser != null && oldUser.IsActive && user.JoinDate.Value < DateTime.Now)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Since User is already active hence rejoin trigger cannot be honored.JoinDate:{user.JoinDate} for the PERNR - {user.EmployeeId}");
                        return null;
                    }
                    evnt = EmployeeEvents.Rehire;
                }


                var trigger = EmployeeEventTrigger.GetByUserIdAndEvent(user.Id, evnt);

                double onBoardDaysBeforeJoining = MasterConfigHelper.OnBoardDaysBeforeJoining;
                DateTime priorBoardingDate = user.JoinDate.Value.AddDays(-(onBoardDaysBeforeJoining));

                if (trigger != null)
                {
                    if (!onBoardExists && !isRejoiningUser)
                    {
                        return null;
                    }

                    if (priorBoardingDate >= DateTime.Today)
                    {
                        trigger.EffectiveDate = priorBoardingDate;
                    }
                    else
                    {
                        trigger.EffectiveDate = DateTime.Today;
                    }

                    trigger.LastModifiedBy = trigger.CreatedBy;
                    trigger.LastModifiedDate = DateTime.Now;
                    trigger.UserDetails = trigger.EventType == EmployeeEvents.Rehire ? UtilityMethods.Serialize<User>(user) : string.Empty;

                    trigger.Save(MasterConfigHelperServiceUser);

                    _idmsLogger.WriteLog(LogLevelType.Info, $"Onboard trigger saved for user - {user.EmployeeId} and the Join date is {user.JoinDate}");

                    return trigger;
                }
                else
                {
                    if (!isNewUser && !rejoinWithSamePernrUsers.Contains(user.Id))
                    {
                        return null;
                    }

                    EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                    {
                        CreatedBy = "System",
                        CreatedDate = DateTime.Now,
                        LastModifiedBy = "System",
                        LastModifiedDate = DateTime.Now,
                        EventType = evnt,
                        IsRequestCreated = false,
                        UserId = user.Id,
                        IsActive = true,
                        UserDetails = evnt == EmployeeEvents.Rehire ? UtilityMethods.Serialize<User>(user) : string.Empty,
                    };

                    if (priorBoardingDate >= DateTime.Today)
                    {
                        newTrigger.EffectiveDate = priorBoardingDate;
                    }
                    else
                    {
                        newTrigger.EffectiveDate = DateTime.Today;
                    }

                    return newTrigger;
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Error while populating/verifying on-board trigger for the User: {0}, Pernr: {1}", user.KnownAs, user.EmployeeId), ex);
                return null;
            }
        }

        EmployeeEventTrigger PopulateOffBoardTrigger(User user)
        {
            try
            {
                var evnt = EmployeeEvents.Resignation;
                var trigger = EmployeeEventTrigger.GetByUserIdAndEvent(user.Id, evnt);

                if (trigger != null)
                {
                    trigger.EffectiveDate = user.TerminationDate.Value;
                    trigger.LastModifiedBy = trigger.CreatedBy;
                    trigger.LastModifiedDate = DateTime.Now;
                    trigger.Comments = string.Empty;
                    trigger.Save(MasterConfigHelperServiceUser);

                    return trigger;
                }
                else
                {
                    var tempTrigger = EmployeeEventTrigger.GetByEffectiveDate(user.Id, evnt, user.TerminationDate.Value.Date);

                    if (tempTrigger != null)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Offboarding event was already created for the pernr {0} with effective date {1}", user.EmployeeId, user.TerminationDate.Value.ToShortDateString()));
                        return null;
                    }

                    EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                    {
                        CreatedBy = "System",
                        CreatedDate = DateTime.Now,
                        LastModifiedBy = "System",
                        LastModifiedDate = DateTime.Now,
                        EffectiveDate = user.TerminationDate.Value,
                        EventType = evnt,
                        IsRequestCreated = false,
                        UserId = user.Id,
                        IsActive = true
                    };

                    return newTrigger;
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Error while populating/verifying off-board trigger for the User: {0}, Pernr: {1}", user.KnownAs, user.EmployeeId), ex);
                return null;
            }
        }

        EmployeeEventTrigger PopulateOUChangeTrigger(User user, OrgChangeTrack orgTrack)
        {
            try
            {
                var trigger = EmployeeEventTrigger.GetByUserIdAndEvent(user.Id, orgTrack.Event);

                if (trigger != null)
                {
                    trigger.EffectiveDate = orgTrack.EffectiveDate;
                    trigger.LastModifiedBy = trigger.CreatedBy;
                    trigger.LastModifiedDate = DateTime.Now;
                    trigger.Comments = orgTrack.Comment;
                    trigger.OldUserDetails = UtilityMethods.Serialize<User>(user);

                    this.FillUserOrgFromOrgTrack(user, orgTrack);
                    trigger.UserDetails = UtilityMethods.Serialize<User>(user);

                    trigger.Save(MasterConfigHelperServiceUser);

                    var msg = ProvisioningUtility.ProvisioningValidator.IsADDefaultsExists(user);
                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"AD default provisioning matrix is updated for the user PERNR {user.EmployeeId} for the event Id {trigger.Id} - Event : {orgTrack.Event.ToString()}, and the effective date is {orgTrack.EffectiveDate.ToShortDateString()}. AD Provisioning matrix message : {msg}");
                    }
                    return trigger;
                }
                else
                {
                    var tempTrigger = EmployeeEventTrigger.GetByEffectiveDate(user.Id, orgTrack.Event, orgTrack.EffectiveDate);
                    if (tempTrigger != null)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, string.Format("{0} event was already created for the pernr {1} with effective date {2}", orgTrack.Event.ToString(), user.EmployeeId, orgTrack.EffectiveDate.ToShortDateString()));
                        return null;
                    }

                    EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                    {
                        CreatedBy = "System",
                        CreatedDate = DateTime.Now,
                        EffectiveDate = orgTrack.EffectiveDate,
                        LastModifiedBy = "System",
                        LastModifiedDate = DateTime.Now,
                        EventType = orgTrack.Event,
                        IsRequestCreated = false,
                        UserId = user.Id,
                        IsActive = true,
                        Comments = orgTrack.Comment
                    };

                    newTrigger.OldUserDetails = UtilityMethods.Serialize<User>(user);
                    this.FillUserOrgFromOrgTrack(user, orgTrack);
                    newTrigger.UserDetails = UtilityMethods.Serialize<User>(user);

                    var msg = ProvisioningUtility.ProvisioningValidator.IsADDefaultsExists(user);
                    if (string.IsNullOrWhiteSpace(msg))
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"AD default provisioning matrix is updated for the user PERNR {user.EmployeeId} for the event Id {newTrigger.Id} - Event : {orgTrack.Event.ToString()}, and the effective date is {orgTrack.EffectiveDate.ToShortDateString()}. AD Provisioning matrix message : {msg}");
                    }
                    return newTrigger;
                }
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Error while populating/verifying transfer/re-org trigger for the User: {0}, Pernr: {1}", user.KnownAs, user.EmployeeId), ex);
                return null;
            }
        }

        void FillUserOrgFromOrgTrack(User user, OrgChangeTrack orgTrack)
        {
            try
            {
                user.CostCenter = orgTrack.CostCenter;
                user.CompanyId = orgTrack.CompanyId;
                user.CompanyCode = orgTrack.CompanyCode;
                user.CompanyName = orgTrack.CompanyName;
                user.DepartmentId = orgTrack.DepartmentId;
                user.DepartmentCode = orgTrack.DepartmentCode;
                user.DepartmentName = orgTrack.DepartmentName;
                user.DivisionId = orgTrack.DivisionId;
                user.DivisionCode = orgTrack.DivisionCode;
                user.DivisionName = orgTrack.DivisionName;
                user.NOrganisationId = orgTrack.OrganisationId;
                user.SectionCode = orgTrack.SectionCode;
                user.SectionName = orgTrack.SectionName;

                user.OrgUnit = orgTrack.OrgUnit;
                user.OrgBranch = orgTrack.OrgBranch;
                user.OrgCompany = orgTrack.OrgCompany;
                user.OrgDepartment = orgTrack.OrgDepartment;
                user.OrgDivision = orgTrack.OrgDivision;
                user.OrgSection = orgTrack.OrgSection;
                user.PositionName = orgTrack.PositionName;
                user.PositionCode = orgTrack.PositionName;

                user.BranchCode = orgTrack.BranchCode;
                user.UnitCode = orgTrack.UnitCode;
                user.OrganisationCode = orgTrack.OrganisationCode;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("Exception at FillUserOrgFromOrgTrack for user -{0}", user.KnownAs), ex);
                throw;
            }
        }

        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool LockAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool UnLockAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool DisableAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool EnableAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        void UpdateOrgTrack(OrgChangeTrack orgTrack, User user, List<NOrganisation> sections, List<Company> companies)
        {
            if (sections == null || sections.Count == 0) return;
            if (orgTrack != null)
            {
                var company = companies.FirstOrDefault(item => item.Code == orgTrack.CompanyCode);
                if (company != null)
                {
                    orgTrack.CompanyId = company.Id;
                    orgTrack.CompanyName = company.Text;
                }

                var department = sections.FirstOrDefault(item => item.Code == orgTrack.DepartmentCode);
                if (department != null)
                {
                    orgTrack.DepartmentId = department.Id;
                    orgTrack.DepartmentName = department.Name;
                }

                var division = sections.FirstOrDefault(item => item.Code == orgTrack.DivisionCode);
                if (division != null)
                {
                    orgTrack.DivisionId = division.Id;
                    orgTrack.DivisionName = division.Name;
                }

                var section = sections.FirstOrDefault(item => item.Code == orgTrack.SectionCode);
                if (section != null)
                {
                    orgTrack.OrganisationId = section.Id;
                    orgTrack.SectionName = section.Name;
                }
            }

            if (user != null)
            {
                var company = companies.FirstOrDefault(item => item.Code == user.CompanyCode);
                if (company != null)
                {
                    user.CompanyId = company.Id;
                    user.CompanyName = company.Text;
                }

                var department = sections.FirstOrDefault(item => item.Code == user.DepartmentCode);
                if (department != null)
                {
                    user.DepartmentId = department.Id;
                    user.DepartmentName = department.Name;
                }

                var division = sections.FirstOrDefault(item => item.Code == user.DivisionCode);
                if (division != null)
                {
                    user.DivisionId = division.Id;
                    user.DivisionName = division.Name;
                }

            }
        }

        string getOrganisationCode(User user)
        {
            if (!string.IsNullOrWhiteSpace(user.UnitCode) && user.UnitCode != emptyOrgID)
            {
                return user.UnitCode;
            }
            else if (!string.IsNullOrWhiteSpace(user.SectionCode) && user.SectionCode != emptyOrgID)
            {
                return user.SectionCode;
            }
            else if (!string.IsNullOrWhiteSpace(user.BranchCode) && user.BranchCode != emptyOrgID)
            {
                return user.BranchCode;
            }
            else if (!string.IsNullOrWhiteSpace(user.DepartmentCode) && user.DepartmentCode != emptyOrgID)
            {
                return user.DepartmentCode;
            }
            else
                return user.CompanyCode;
        }

        bool Z_Bapipa_Get_Tel_Dir_Info(User user, string sapFormatdateTimeString, OrgChangeTrack orgTrack = null)
        {
            if (user == null) return false;

            try
            {

                RfcRepository repo = _sapV3Util.RfcDestination.Repository;
                IRfcFunction function = repo.CreateFunction("Z_BAPIPA_GET_EMP_INFO");

                if (!string.IsNullOrWhiteSpace(sapFormatdateTimeString))
                {
                    function.SetValue("IM_EFF_DATE", sapFormatdateTimeString);
                }

                IRfcTable So_MATNR = function.GetTable("PERNR");

                RfcStructureMetadata pernr_metadata = repo.GetStructureMetadata("ZBAPI_PERNR");

                var struct_pernr = pernr_metadata.CreateStructure();
                struct_pernr.SetValue("SIGN", "I");
                struct_pernr.SetValue("OPTION", "EQ");
                struct_pernr.SetValue("LOW", user.EmployeeId);
                struct_pernr.SetValue("HIGH", user.EmployeeId);


                So_MATNR.Append(struct_pernr);

                function.Invoke(_sapV3Util.GetSAPDestination());

                IRfcTable returnTable = function.GetTable("TEL_DIR_INFO");


                DataTable dtTable = returnTable.ToDataTable("TEL_DIR_INFO");

                if (dtTable != null && dtTable.Rows.Count > 0)
                {
                    DataRow dr = dtTable.Rows[0];

                    if (orgTrack != null)
                    {
                        orgTrack.OrgCompany = CheckNull.AgainstString(dr["COMPANY"]);
                        orgTrack.OrgDepartment = CheckNull.AgainstString(dr["DEPARTMENT"]);
                        orgTrack.OrgDivision = CheckNull.AgainstString(dr["DIVISION"]);
                        orgTrack.OrgBranch = CheckNull.AgainstString(dr["BRANCH"]);
                        orgTrack.OrgSection = CheckNull.AgainstString(dr["SECTION"]);
                        orgTrack.OrgUnit = CheckNull.AgainstString(dr["UNIT"]);
                        orgTrack.CompanyCode = CheckNull.AgainstString(dr["COMPANYID"]);
                        orgTrack.DepartmentCode = CheckNull.AgainstString(dr["DEPARTMENTID"]);
                        orgTrack.DivisionCode = CheckNull.AgainstString(dr["DIVISIONID"]);
                        orgTrack.SectionCode = CheckNull.AgainstString(dr["SECTIONID"]);
                        orgTrack.PositionName = CheckNull.AgainstString(dr["USER_JOB_TITLE"]);
                        orgTrack.UnitCode = CheckNull.AgainstString(dr["UNITID"]);
                        orgTrack.BranchCode = CheckNull.AgainstString(dr["BRANCHID"]);

                        if (!string.IsNullOrWhiteSpace(orgTrack.UnitCode) && orgTrack.UnitCode != emptyOrgID)
                        {
                            orgTrack.OrganisationCode = orgTrack.UnitCode;
                        }
                        else if (!string.IsNullOrWhiteSpace(orgTrack.SectionCode) && orgTrack.SectionCode != emptyOrgID)
                        {
                            orgTrack.OrganisationCode = orgTrack.SectionCode;
                        }
                        else if (!string.IsNullOrWhiteSpace(orgTrack.BranchCode) && orgTrack.BranchCode != emptyOrgID)
                        {
                            orgTrack.OrganisationCode = orgTrack.BranchCode;
                        }
                        else if (!string.IsNullOrWhiteSpace(orgTrack.DepartmentCode) && orgTrack.DepartmentCode != emptyOrgID)
                        {
                            orgTrack.OrganisationCode = orgTrack.DepartmentCode;
                        }
                        else
                        {
                            orgTrack.OrganisationCode = orgTrack.CompanyCode;
                        }

                        orgTrack.PositionName = CheckNull.AgainstString(dr["USER_JOB_TITLE"]);
                    }
                    else
                    {
                        user.CompanyName = CheckNull.AgainstString(dr["COMPANY"]);
                        user.OrgCompany = CheckNull.AgainstString(dr["COMPANY"]);

                        user.CompanyCode = CheckNull.AgainstString(dr["COMPANYID"]);


                        user.DepartmentName = CheckNull.AgainstString(dr["DEPARTMENT"]);
                        user.OrgDepartment = CheckNull.AgainstString(dr["DEPARTMENT"]);
                        user.DepartmentCode = CheckNull.AgainstString(dr["DEPARTMENTID"]);


                        user.OrgBranch = CheckNull.AgainstString(dr["BRANCH"]);
                        user.BranchCode = CheckNull.AgainstString(dr["BRANCHID"]);


                        user.DivisionName = CheckNull.AgainstString(dr["DIVISION"]);
                        user.OrgDivision = CheckNull.AgainstString(dr["DIVISION"]);
                        user.DivisionCode = CheckNull.AgainstString(dr["DIVISIONID"]);


                        user.SectionName = CheckNull.AgainstString(dr["SECTION"]);
                        user.OrgSection = CheckNull.AgainstString(dr["SECTION"]);
                        user.SectionCode = CheckNull.AgainstString(dr["SECTIONID"]);

                        user.OrgUnit = CheckNull.AgainstString(dr["UNIT"]);
                        user.UnitCode = CheckNull.AgainstString(dr["UNITID"]);

                        user.OrganisationCode = getOrganisationCode(user);
                    }

                    user.Fax = CheckNull.AgainstString(dr["FAX_NUMBER"]);

                    user.TelephoneNumber = CheckNull.AgainstString(dr["OFFICE_NUMBER"]);
                    user.MobileNumber = CheckNull.AgainstString(dr["MOBILE_NUMBER"]);


                    user.PostalCode = CheckNull.AgainstString(dr["POSTAL_CODE"]);

                    user.PostalAddress = CheckNull.AgainstString(dr["OFFICE_ADDRESS"]);
                    user.Country = CheckNull.AgainstString(dr["COUNTRY"]);

                    user.PositionName = CheckNull.AgainstString(dr["USER_JOB_TITLE"]);


                    string str = string.Format(@" CompanyName : {0}; user.OrgCompany {1}; CompanyCode :{2}; DepartmentName: {3}; OrgDepartment: {4}; DepartmentCode: {5}; OrgBranch: {6}; DivisionName: {7}; OrgDivision : {8};
                                               DivisionCode: {9};   SectionName: {10}; OrgSection: {11}; SectionCode: {12}; Unit: {13}; Fax: {14}; TelephoneNumber :{15}; MobileNumber: {16}; PostalCode: {17}; 
                                               PostalAddress : {18}; Country: {19}; PositionName: {20}",

                    user.CompanyName,
                    user.OrgCompany,
                    user.CompanyCode,
                    user.DepartmentName,
                    user.OrgDepartment,
                    user.DepartmentCode,
                    user.OrgBranch,
                    user.DivisionName,
                    user.OrgDivision,
                    user.DivisionCode,
                    user.SectionName,
                    user.OrgSection,
                    user.SectionCode,
                    user.Reserved8,
                     user.Fax,
                    user.TelephoneNumber,
                    user.MobileNumber,
                    user.PostalCode,
                    user.PostalAddress,
                    user.Country,
                    user.PositionName);

                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, string.Format("Data from Z_Bapipa_Get_Tel_Dir_Info - for the user (Pernr- {0} and the details are, {1}) ", user.EmployeeId, str));

                }
                else
                {
                    var strlog = $"No Data found from Z_Bapipa_Get_Tel_Dir_Info - for the user (Pernr- {user.EmployeeId})";
                    if (!string.IsNullOrWhiteSpace(sapFormatdateTimeString))
                    {
                        strlog = strlog + $"as on date {UtilityMethods.GetDateFromString(sapFormatdateTimeString)}";
                    }
                    DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, strlog);
                    return false;
                }

            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, string.Format("Exception occured in Z_Bapipa_Get_Tel_Dir_Info - for the user (Pernr- {0}) ", user.EmployeeId), ex);

                throw ex;
            }

            return true;
        }

        DataRow[] getPA0001(DataTable dtPA0001, string effectiveStartDate, string changeDate)
        {
            return dtPA0001.AsEnumerable().Where(tmpDataRow =>
            {

                bool returnflag = true;

                if (!string.IsNullOrWhiteSpace(effectiveStartDate))
                {
                    returnflag = tmpDataRow.Field<string>("BEGDA") == effectiveStartDate;
                }


                if (!string.IsNullOrWhiteSpace(changeDate))
                {
                    returnflag = tmpDataRow.Field<string>("AEDTM") == changeDate;
                }

                return returnflag;

            }).Select(tmpDataRow => tmpDataRow).ToArray();
        }

        #region Get Tables

        public DataTable GetTablePA0002(string pernr)
        {
            this.Initialize();

            using (SAPProxy1 proxy = new SAPProxy1())
            {
                proxy.Connection = this.GetSAPConnection();
                proxy.Connection.Open();

                var dt = new TAB512Table();
                var dtFlds = new RFC_DB_FLDTable();
                var dtopts = new RFC_DB_OPTTable();
                RFC_DB_OPT FILTER_PERNR = new RFC_DB_OPT();
                FILTER_PERNR.Text = "PERNR = '" + pernr + "'";
                
                dtFlds.Add(FLD_PERNR);
                dtFlds.Add(FLD_VORNA);
                dtFlds.Add(FLD_TITEL);
                dtFlds.Add(FLD_NATIO);
                dtFlds.Add(FLD_NACHN);
                dtFlds.Add(FLD_MIDNM);
                dtFlds.Add(FLD_RUFNM);
                dtFlds.Add(FLD_ENDDA);

                dtopts.Add(FILTER_PERNR);

                proxy.Rfc_Read_Table("|", " ", "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                var dtFields = dtFlds.ToADODataTable();
                DataTable dtParameters = dt.ToADODataTable();
                m_dtPA0002 = ConvertPipesToColumns(dtFields, dtParameters);
            }
            return m_dtPA0002;
        }

        public DataTable GetTablePA0000(string pernr)
        {
            this.Initialize();

            using (SAPProxy1 proxy = new SAPProxy1())
            {
                proxy.Connection = this.GetSAPConnection();
                proxy.Connection.Open();
                var dt = new TAB512Table();
                var dtFlds = new RFC_DB_FLDTable();
                var dtopts = new RFC_DB_OPTTable();
                RFC_DB_OPT FILTER_PERNR = new RFC_DB_OPT();
                FILTER_PERNR.Text = "PERNR = '" + pernr + "'";

                dtFlds.Add(FLD_BEGDA);
                dtFlds.Add(FLD_ENDDA);
                dtFlds.Add(FLD_STAT2);
                dtFlds.Add(FLD_MASSN);
                dtFlds.Add(FLD_MASSG);
                dtFlds.Add(FLD_AEDTM);

                dtopts.Add(FILTER_PERNR);

                proxy.Rfc_Read_Table("|", " ", "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                var dtFields = dtFlds.ToADODataTable();
                DataTable dtParameters = dt.ToADODataTable();
                m_dtPA0000 = ConvertPipesToColumns(dtFields, dtParameters); 
            }
            return m_dtPA0000;
        }

        public DataTable GetTablePA0001(string pernr)
        {
            this.Initialize();

            using (SAPProxy1 proxy = new SAPProxy1())
            {
                proxy.Connection = this.GetSAPConnection();
                proxy.Connection.Open();
                var dt = new TAB512Table();
                var dtFlds = new RFC_DB_FLDTable();
                var dtopts = new RFC_DB_OPTTable();
                RFC_DB_OPT FILTER_PERNR = new RFC_DB_OPT();
                FILTER_PERNR.Text = "PERNR = '" + pernr + "'";

                dtFlds.Add(FLD_BEGDA);
                dtFlds.Add(FLD_ENDDA);
                dtFlds.Add(FLD_AEDTM);
                dtFlds.Add(FLD_ENAME);
                dtFlds.Add(FLD_KOSTL);
                dtFlds.Add(FLD_BUKRS);
                dtFlds.Add(FLD_WERKS);
                dtFlds.Add(FLD_BTRTL);
                dtFlds.Add(FLD_ORGEH);
                dtFlds.Add(FLD_PLANS);
                dtFlds.Add(FLD_PERSG);
               


                dtopts.Add(FILTER_PERNR);

                proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                var dtTemp = dt.ToADODataTable();
                var dtFields = dtFlds.ToADODataTable();
                m_dtPA0001 = ConvertPipesToColumns(dtFields, dtTemp);
            }
            return m_dtPA0001;
        }

        public DataTable GetTablePA0302(string pernr)
        {
            this.Initialize();

            using (SAPProxy1 proxy = new SAPProxy1())
            {
                proxy.Connection = this.GetSAPConnection();
                proxy.Connection.Open();
                var dt = new TAB512Table();
                var dtFlds = new RFC_DB_FLDTable();
                var dtopts = new RFC_DB_OPTTable();
                RFC_DB_OPT FILTER_PERNR = new RFC_DB_OPT();
                FILTER_PERNR.Text = "PERNR = '" + pernr + "'";

                dtFlds.Add(FLD_BEGDA);
                dtFlds.Add(FLD_ENDDA);
                dtFlds.Add(FLD_MASSN);
                dtFlds.Add(FLD_MASSG);
                dtFlds.Add(FLD_AEDTM);

                dtopts.Add(FILTER_PERNR);

                proxy.Rfc_Read_Table("|", null, "PA0302", 0, 0, ref dt, ref dtFlds, ref dtopts);
                var dtTemp = dt.ToADODataTable();
                var dtFields = dtFlds.ToADODataTable();
                m_dtPA0302 = ConvertPipesToColumns(dtFields, dtTemp);
            }
            return m_dtPA0302;
        }


        #endregion Get Tables

    }

}
