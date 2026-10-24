using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;

using SAP.Connector;
using SAPUserDetailsComponent;
using SAPUserGrp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using DI.Chakra.DomainModel.Presentation;

namespace DI.Chakra.Connector
{
    public class SAPHRMSOrgRevampConnector : ConnectorBase
    {

        EndpointConfiguration SAPConfiguration { get; set; }
        string today = DateTime.Today.ToString("yyyyMMdd");
        string deltaDate = string.Empty;        

        #region DataTables

        DataTable m_dtPA0000AllEmp;

        DataTable m_dtPA0000;
        DataTable m_dtPA0001;
        DataTable m_dtPA0002;
        DataTable m_dtPA0185;
        DataTable m_dtPA0077;
        DataTable m_dtPA0105;
        DataTable m_dtPA0027;

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

        #endregion

        public SAPHRMSOrgRevampConnector(EndPoint endpoint)
        {
            this.EndPoint = endpoint;
            this.SAPConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
        }

        #region "Connector Base Members"


        private string Configuration(EndpointKey Key)
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

                ////this.PopulateCompanyCodes();

                ////this.PopulatePersonnelAreas();

                this.PopulateOrganizations();

                ////this.PopulatePersonnelSubAreas();

                this.PopulateCostCenters();

                this.PopulateCostCenterTexts();

                this.PopulateBusinessRoles();

                this.PopulateOrganizationHead();

                if (this.IsFirstTimeDiscovery)
                {
                    //this.PopulateBusinessRoles();
                    recordCount = this.GetCompletePAInformation();
                }
                else
                {
                    //this.PopulateBusinessRolesDelta();
                    recordCount = this.GetDeltaPAInformation();
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} Users found in Trusted Source", recordCount.ToString()));

                WriteDiscoveryLog(LogLevelType.Info, "Start updating Manager");


                //User.UpdateManagerForUsers();

                WriteDiscoveryLog(LogLevelType.Info, "Finished updating Manager");

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

        /*
        public override bool AddEmailIdInHR(string employeeId, string emailId, DateTime beginDate)
        {
            try
            {
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
                        DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Warning, string.Format("Error while adding Email Id for Pernr - {0}", employeeId));
                        //DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Warning, ret.Message);
                    }

                    proxy.Bapi_Employee_Dequeue(employeeId, out ret);

                    proxy.CommitWork();

                    return isAdded;
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format("Error while updating Email Id - {0} in HRMS for Pernr - {1}", emailId, employeeId), ex);
                return false;
            }
        }

        public override bool AddFLINSLogonIdInHR(string employeeId, string sapLogonId, DateTime beginDate)
        {
            try
            {
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
                        //.ProvisioningLogger.WriteLog(null,  LogLevelType.Warning, string.Format("Error while adding FLINS Login Id for Pernr - {0}", employeeId));
                        //DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Warning, ret.Message);
                    }

                    proxy.Bapi_Employee_Dequeue(employeeId, out ret);

                    proxy.CommitWork();

                    return isAdded;
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format("Error while updating FLINS Login Id - {0} in HRMS for Pernr - {1}", sapLogonId, employeeId), ex);
                return false;
            }
        }

        public bool DelimitEmailIdInHR(string employeeId, DateTime delimitDate)
        {
            try
            {
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
                                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Warning, string.Format("Error while delimiting Email Id for Pernr - {0}", employeeId));
                                //DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Warning, ret.Message);
                            }
                        }
                    }

                    proxy.Bapi_Employee_Dequeue(employeeId, out ret);

                    proxy.CommitWork();

                    return true;
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format("Error while delimiting Email Id in HRMS for Pernr - {0}", employeeId), ex);
                return false;
            }
        }

        public bool DelimitFLINSLogonIdInHR(string employeeId, DateTime delimitDate)
        {
            try
            {
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
                                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Warning, string.Format("Error while delimiting FLINS Login Id for Pernr - {0}", employeeId));
                                //DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Warning, ret.Message);
                            }
                        }
                    }

                    proxy.Bapi_Employee_Dequeue(employeeId, out ret);

                    proxy.CommitWork();

                    return true;
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format("Error while delimiting FLINS Login Id in HRMS for Pernr - {0}", employeeId), ex);
                return false;
            }
        }

        public override void SyncTelephoneDirectory()
        {
            try
            {
                var users = User.GetActiveEmployees();
                var propagationFields = MasterConfigHelper.PropagationFields;
                bool canRunPropagationTrigger = MasterConfigHelper.CanRunPropagationTrigger;

                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, "Started preparing sync data for telephone directory.");

                foreach (User usr in users)
                {
                    if (usr.FullTimeEmployee.HasValue && usr.FullTimeEmployee.Value)
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

                                    SAPUserGrp.BAPIRETURN1 return1 = new SAPUserGrp.BAPIRETURN1();
                                    ZBAPI_PERNRTable pernrTable = new ZBAPI_PERNRTable();
                                    ZBAPIPA_TEL_DIR_INFO1Table telDirInfo = new ZBAPIPA_TEL_DIR_INFO1Table();

                                    ZBAPI_PERNR pernrInfo = new ZBAPI_PERNR();
                                    pernrInfo.Sign = "I";
                                    pernrInfo.Option0 = "EQ";
                                    pernrInfo.Low = usr.EmployeeId;

                                    pernrTable.Add(pernrInfo);

                                    proxy.Z_Bapipa_Get_Tel_Dir_Info(out return1, ref pernrTable, ref telDirInfo);

                                    if (telDirInfo != null && telDirInfo.Count != 0)
                                    {
                                        usr.Fax = telDirInfo[0].Fax_Number;
                                        usr.TelephoneNumber = telDirInfo[0].Office_Number;
                                        usr.MobileNumber = telDirInfo[0].Mobile_Number;

                                        usr.PostalCode = telDirInfo[0].Postal_Code;
                                        usr.PostalAddress = telDirInfo[0].Office_Address;
                                        usr.Country = telDirInfo[0].Country;
                                                                                
                                        User.UpdateUserFromDiscovery(usr, propagationFields, canRunPropagationTrigger, MasterConfigHelper.ServiceUser);
                                    }
                                    else
                                    {
                                        DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Warning, string.Format("Telephone directory info for the user - {0}, Pernr - {1}. is missing.", usr.KnownAs, usr.EmployeeId));
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format("Error while synchronising data to telephone directory for the user - {0}, Pernr - {1}.", usr.KnownAs, usr.EmployeeId), ex);
                        }
                    }
                }
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, "Completed preparing sync data for telephone directory.");
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Error while synchronising data to telephone directory.", ex);
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
            catch
            {
                throw;
            }
        }

        public override void DownloadSpecificUserFromHR(string pernr)
        {
            try
            {
                this.Initialize();
                this.GetUserByPernr(pernr);
            }
            catch
            {
                throw;
            }
        }
        */
        #endregion

        private void Initialize()
        {

            //Tables
            m_dtPA0000AllEmp = new DataTable();

            m_dtPA0000 = new DataTable();
            m_dtPA0001 = new DataTable();
            m_dtPA0002 = new DataTable();
            m_dtPA0185 = new DataTable();
            m_dtPA0077 = new DataTable();
            m_dtPA0105 = new DataTable();
            m_dtPA0027 = new DataTable();

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
            //delta date 
            if (!this.IsFirstTimeDiscovery)
            {
                deltaDate = this.Process.LastRunOn.Value.ToString("yyyyMMdd");
            }
        }

        private SAPConnection GetSAPConnection()
        {
            Destination destination = new Destination();
            string pwd = RSAHelper.DecryptData(Configuration(EndpointKey.Password));
            destination.AppServerHost = Configuration(EndpointKey.IPAddress); //Configuration.IPAddress;10.168.15.24";//
            destination.Client = Convert.ToInt16(Configuration(EndpointKey.ClientNumber)); //Configuration.IntClientNum; 300;//
            destination.Password = pwd; //Configuration.Password;"orange123"; //
            destination.SystemNumber = Convert.ToInt16(Configuration(EndpointKey.SystemNumber)); //Configuration.SystemNumber;00;//
            destination.Username = Configuration(EndpointKey.UserName); //Configuration.UserName;sisuis";//"
            SAPConnection connection = new SAPConnection(destination);
            return connection;
        }

        private int GetCompletePAInformation()
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

                        }

                        NHRMSUser user = NHRMSUser.GetByEmployeeId(Pernr, this.EndPoint.Id) ?? new NHRMSUser();

                        rows = (from tmpDataRow in m_dtPA0185.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                select tmpDataRow).ToArray<DataRow>();

                        //skip users without nric
                        if (null != rows && rows.Count() > 0)
                        {
                            user.IdentificationNo = CheckNull.AgainstString(rows[0]["ICNUM"].ToString().Trim());
                        }

                        //temp fix
                        if (string.IsNullOrEmpty(user.IdentificationNo))
                        {
                            user.IdentificationNo = Pernr;
                        }

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && MasterConfigHelper.OnBoardTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {                            
                            user.IsActive = MasterConfigHelper.ActiveEmployeeFlags.Contains(CheckNull.AgainstString(rows[0]["STAT2"].ToString().Trim())) ? true : false;
                        }

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where MasterConfigHelper.OnBoardTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
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

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && MasterConfigHelper.OffBoardTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
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

                        if (rows.Count() > 0)
                        {
                            user.OrgUnitCode = CheckNull.AgainstString(rows[0]["ORGEH"]).Trim();
                            user.PositionCode = CheckNull.AgainstString(rows[0]["PLANS"]).Trim();
                        }

                        rows = (from tmpDataRow in m_dtPA0027.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                     && tmpDataRow.Field<string>("KSTAR").Trim() == "01"
                                select tmpDataRow).ToArray<DataRow>();                        

                        rows = (from tmpDataRow in m_dtPA0002.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && tmpDataRow.Field<string>("PERNR") == Pernr
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.FirstName = CheckNull.AgainstString(rows[0]["VORNA"]).Trim();
                            user.MiddleName = CheckNull.AgainstString(rows[0]["MIDNM"]).Trim();
                            user.LastName = CheckNull.AgainstString(rows[0]["NACHN"]).Trim();
                            //user.KnownAs = CheckNull.AgainstString(rows[0]["RUFNM"]).Trim();
                            user.KnownAs = user.FirstName + " " + user.LastName;
                            user.DisplayName = user.KnownAs;
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

                        rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                                where tmpDataRow.Field<string>("USRTY").Trim() == "0001"
                                select tmpDataRow).ToArray<DataRow>();

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

                        user.EmployeeId = Pernr;
                        user.FullTimeEmployee = true;
                        user.EmployeeType = EmployeeType.Permanent;

                        user.EndpointSystemId = this.EndPoint.Id;
                        user.Save(MasterConfigHelper.ServiceUser);
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

        private int GetDeltaPAInformation()
        {

            TAB512Table dt = new TAB512Table();
            RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();

            List<string> employeeIds = new List<string>();            

            string Pernr = string.Empty;

            var propagationFields = MasterConfigHelper.PropagationFields;
            bool canRunPropagationTrigger = MasterConfigHelper.CanRunPropagationTrigger;

            try
            {
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

                }

                foreach (DataRow person in m_dtPA0000AllEmp.Rows)
                {
                    Pernr = person["PERNR"].ToString().Trim();

                    if (!employeeIds.Contains(Pernr))
                    {
                        employeeIds.Add(Pernr);
                    }
                }

                DataRow[] rows;

                foreach (string person in employeeIds)
                {
                    try
                    {

                        Pernr = person;

                        NHRMSUser user = NHRMSUser.GetByEmployeeId(Pernr, this.EndPoint.Id) ?? new NHRMSUser();
                        List<NHRMSUserEvent> userEvent = new List<NHRMSUserEvent>();

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

                        }

                        rows = (from tmpDataRow in m_dtPA0185.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                select tmpDataRow).ToArray<DataRow>();

                        //skip users without nric
                        if (null != rows && rows.Count() > 0)
                        {
                            user.IdentificationNo = CheckNull.AgainstString(rows[0]["ICNUM"].ToString().Trim());
                        }

                        //temp fix
                        if (string.IsNullOrEmpty(user.IdentificationNo))
                        {
                            user.IdentificationNo = Pernr;
                        }

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && MasterConfigHelper.OnBoardTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.IsActive = MasterConfigHelper.ActiveEmployeeFlags.Contains(CheckNull.AgainstString(rows[0]["STAT2"].ToString().Trim())) ? true : false;
                        }

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && MasterConfigHelper.OnBoardTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            string Joindate = CheckNull.AgainstString(rows[0]["BEGDA"].ToString()).Trim();
                            if (!string.IsNullOrEmpty(Joindate) && Joindate.Length >= 8)
                            {
                                userEvent.Add(new NHRMSUserEvent()
                                {
                                    EmployeeEvent = EmployeeEvents.NewJoinee
                                });
                                user.JoinDate = UtilityMethods.GetDateFromString(Joindate);
                            }
                        }

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && MasterConfigHelper.OffBoardTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            string termdate = CheckNull.AgainstString(rows[0]["BEGDA"].ToString()).Trim();
                            if (!string.IsNullOrEmpty(termdate) && termdate.Length >= 8)
                            {
                                userEvent.Add(new NHRMSUserEvent()
                                {
                                    EmployeeEvent = EmployeeEvents.Resignation
                                });
                                user.TerminationDate = UtilityMethods.GetDateFromString(termdate);
                            }
                        }

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && MasterConfigHelper.TranferTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            DateTime tempBeginDate = UtilityMethods.GetDateFromString(CheckNull.AgainstString(rows[0]["BEGDA"].ToString()).Trim());

                            userEvent.Add(new NHRMSUserEvent()
                            {
                                EmployeeEvent = EmployeeEvents.OUChange,
                                EffectiveDate = tempBeginDate
                            });
                        }

                        rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && MasterConfigHelper.ReOrgTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows != null && rows.Count() > 0)
                        {
                            DateTime tempBeginDate = UtilityMethods.GetDateFromString(CheckNull.AgainstString(rows[0]["BEGDA"].ToString()).Trim());

                            userEvent.Add(new NHRMSUserEvent()
                            {
                                EmployeeEvent = EmployeeEvents.ReOrganisation,
                                EffectiveDate = tempBeginDate
                            });
                        }

                        rows = (from tmpDataRow in m_dtPA0001.AsEnumerable()
                                select tmpDataRow).ToArray<DataRow>();

                        if (rows.Count() > 0)
                        {
                            user.OrgUnitCode = CheckNull.AgainstString(rows[0]["ORGEH"]).Trim();
                            user.PositionCode = CheckNull.AgainstString(rows[0]["PLANS"]).Trim();
                        }

                        rows = (from tmpDataRow in m_dtPA0027.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                      && tmpDataRow.Field<string>("KSTAR").Trim() == "01"
                                select tmpDataRow).ToArray<DataRow>();
                        
                        rows = (from tmpDataRow in m_dtPA0002.AsEnumerable()
                                where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                && tmpDataRow.Field<string>("PERNR") == Pernr
                                select tmpDataRow).ToArray<DataRow>();

                        if (null != rows && rows.Count() > 0)
                        {
                            user.FirstName = CheckNull.AgainstString(rows[0]["VORNA"]).Trim();
                            user.MiddleName = CheckNull.AgainstString(rows[0]["MIDNM"]).Trim();
                            user.LastName = CheckNull.AgainstString(rows[0]["NACHN"]).Trim();
                            //user.KnownAs = CheckNull.AgainstString(rows[0]["RUFNM"]).Trim();
                            user.KnownAs = user.FirstName + " " + user.LastName;
                            user.DisplayName = user.KnownAs;
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

                        rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                                where tmpDataRow.Field<string>("USRTY").Trim() == "0001"
                                select tmpDataRow).ToArray<DataRow>();

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
                        
                        user.EmployeeId = Pernr;
                        user.EmployeeType = EmployeeType.Permanent;
                        user.FullTimeEmployee = true;

                        user.EndpointSystemId = this.EndPoint.Id;
                        user.Save(MasterConfigHelper.ServiceUser);

                        userEvent.ForEach(evt => evt.UserId = user.Id);
                        NHRMSUserEvent.SaveList(userEvent, MasterConfigHelper.ServiceUser);
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Error, string.Format("Error while reading the record - {0}", person), ex);
                    }
                }

                return m_dtPA0000AllEmp.Rows.Count;
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

        private int GetHRMSUsersFromGivenDate(string scanDate)
        {

            TAB512Table dt = new TAB512Table();
            RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();

            List<string> employeeIds = new List<string>();            
            List<NOrganisation> sections = new List<NOrganisation>();
            List<NHRMSOrganisation> nOrgs = new List<NHRMSOrganisation>();
            List<NDesignation> nDesignations = new List<NDesignation>();

            string Pernr = string.Empty;
            bool isNewUser;

            try
            {
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
                NOrganisation section = null;

                DataRow[] rows;

                if (employeeIds.Count > 0)
                {                    
                    sections = NOrganisation.GetAll();
                    nOrgs = NHRMSOrganisation.GetAll();
                    nDesignations = NDesignation.GetAll();
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

                    }

                    rows = (from tmpDataRow in m_dtPA0185.AsEnumerable()
                            where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            select tmpDataRow).ToArray<DataRow>();

                    //skip users without nric
                    if (null != rows && rows.Count() > 0)
                    {
                        user.IdentificationNo = CheckNull.AgainstString(rows[0]["ICNUM"].ToString().Trim());
                    }
                    else
                    {
                        WriteDiscoveryLog(LogLevelType.Error, string.Format("NRIC is empty or missing for the user - {0}", Pernr), null);
                        continue;
                    }

                    if (string.IsNullOrEmpty(user.IdentificationNo))
                    {
                        WriteDiscoveryLog(LogLevelType.Error, string.Format("NRIC is empty or missing for the user - {0}", Pernr), null);
                        continue;
                    }

                    rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                            where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            && MasterConfigHelper.OnBoardTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
                            select tmpDataRow).ToArray<DataRow>();

                    if (null != rows && rows.Count() > 0)
                    {
                        user.IsActive = MasterConfigHelper.ActiveEmployeeFlags.Contains(CheckNull.AgainstString(rows[0]["STAT2"].ToString().Trim())) ? true : false;
                    }

                    rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                            where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            && MasterConfigHelper.OnBoardTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
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
                            && MasterConfigHelper.OffBoardTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
                            select tmpDataRow).ToArray<DataRow>();

                    if (rows != null && rows.Count() > 0)
                    {
                        string termdate = CheckNull.AgainstString(rows[0]["BEGDA"].ToString()).Trim();
                        if (!string.IsNullOrEmpty(termdate) && termdate.Length >= 8)
                        {
                            user.TerminationDate = UtilityMethods.GetDateFromString(termdate);
                        }
                    }

                    rows = (from tmpDataRow in m_dtPA0001.AsEnumerable()
                            select tmpDataRow).ToArray<DataRow>();

                    string sectionCode = string.Empty;
                    string positionCode = string.Empty;
                    if (rows.Count() > 0)
                    {
                        sectionCode = CheckNull.AgainstString(rows[0]["ORGEH"]).Trim();
                        positionCode = CheckNull.AgainstString(rows[0]["PLANS"]).Trim();
                    }

                    rows = (from tmpDataRow in m_dtPA0027.AsEnumerable()
                            where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                                  && tmpDataRow.Field<string>("KSTAR").Trim() == "01"
                            select tmpDataRow).ToArray<DataRow>();

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

                    rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                            where tmpDataRow.Field<string>("USRTY").Trim() == "0001"
                            select tmpDataRow).ToArray<DataRow>();

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
                                        
                    section = sections.FirstOrDefault(item => item.Code == sectionCode);
                    if (section != null)
                    {
                        user.NOrganisationId = Convert.ToInt32(section.Id);
                    }

                    var nOrg = nOrgs.FirstOrDefault(item => item.Code == sectionCode);
                    if (nOrg != null)
                    {
                        user.NOrganisationId = Convert.ToInt32(nOrg.Id);
                    }
                                        
                    var nDesignation = nDesignations.FirstOrDefault(item => item.Code == positionCode);
                    if (nDesignation != null)
                    {
                        user.NBusinessRoleId = Convert.ToInt32(nDesignation.Id);
                    }

                    user.EmployeeId = Pernr;
                    user.EmployeeType = EmployeeType.Permanent;
                    user.FullTimeEmployee = true;
                    if (isNewUser)
                    {
                        oldUser = User.GetByNRIC(user.IdentificationNo, user.IdentificationType, user.IdentificationCountry);
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
                            user.AuthenticationMode = oldUser.AuthenticationMode;

                            user.CreatedBy = Constants.CreatedBy;
                            user.CreatedDate = DateTime.Now;
                            user.LastModifiedBy = Constants.CreatedBy;
                            user.LastModifiedDate = DateTime.Now;

                            userId = oldUser.Id;
                            userId = oldUser.Id;
                            user.Save(MasterConfigHelper.ServiceUser);

                        }
                        else
                        {
                            user.UserName = Pernr;//new logic
                            user.Password = password;

                            user.CreatedBy = Constants.CreatedBy;
                            user.CreatedDate = DateTime.Now;
                            user.Save(MasterConfigHelper.ServiceUser);
                            userId = user.Id;
                            int RoleId = MasterConfigHelper.RoleUser;
                            User.InsertUserRole(userId, RoleId);
                        }
                    }
                    else
                    {

                        user.LastModifiedBy = Constants.CreatedBy;
                        user.LastModifiedDate = DateTime.Now;
                        user.Save(MasterConfigHelper.ServiceUser);
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

        private void GetUserByPernr(string Pernr)
        {

            TAB512Table dt = new TAB512Table();
            RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();

            try
            {
                WriteDiscoveryLog(LogLevelType.Info, "Getting specific user Information");

                bool isNewUser;

                string password = RSAHelper.EncryptData(MasterConfigHelper.DefaultPassword);

                var sections = NOrganisation.GetAll();
                
                var nOrgs = NHRMSOrganisation.GetAll();

                var nDesignations = NDesignation.GetAll();


                //TRUSTED SOURCE USER INFO                
                NOrganisation section = null;

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
                }

                rows = (from tmpDataRow in m_dtPA0185.AsEnumerable()
                        where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                        select tmpDataRow).ToArray<DataRow>();

                //skip users without nric
                if (null != rows && rows.Count() > 0)
                {
                    user.IdentificationNo = CheckNull.AgainstString(rows[0]["ICNUM"].ToString().Trim());
                }

                if (string.IsNullOrEmpty(user.IdentificationNo))
                {
                    WriteDiscoveryLog(LogLevelType.Error, string.Format("NRIC is empty or missing for the user - {0}", Pernr), null);
                    return;
                }

                rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                        where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            && MasterConfigHelper.OnBoardTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
                        select tmpDataRow).ToArray<DataRow>();

                if (null != rows && rows.Count() > 0)
                {
                    user.IsActive = MasterConfigHelper.ActiveEmployeeFlags.Contains(CheckNull.AgainstString(rows[0]["STAT2"].ToString().Trim())) ? true : false;
                }

                rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                        where MasterConfigHelper.OnBoardTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
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

                rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                        where MasterConfigHelper.OffBoardTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
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

                string sectionCode = string.Empty;
                string positionCode = string.Empty;
                if (rows.Count() > 0)
                {
                    sectionCode = CheckNull.AgainstString(rows[0]["ORGEH"]).Trim();
                    positionCode = CheckNull.AgainstString(rows[0]["PLANS"]).Trim();
                }

                rows = (from tmpDataRow in m_dtPA0027.AsEnumerable()
                        where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                             && tmpDataRow.Field<string>("KSTAR").Trim() == "01"
                        select tmpDataRow).ToArray<DataRow>();

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

                rows = (from tmpDataRow in m_dtPA0105.AsEnumerable()
                        where tmpDataRow.Field<string>("USRTY").Trim() == "0001"
                        select tmpDataRow).ToArray<DataRow>();

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
                
                section = sections.FirstOrDefault(item => item.Code == sectionCode);
                if (section != null)
                {
                    user.NOrganisationId = Convert.ToInt32(section.Id);
                }
                                             
                var nDesignation = nDesignations.FirstOrDefault(item => item.Code == positionCode);
                if (nDesignation != null)
                {
                    user.NBusinessRoleId = Convert.ToInt32(nDesignation.Id);
                }
                
                user.FullTimeEmployee = true;
                user.EmployeeType = EmployeeType.Permanent;
                user.EmployeeId = Pernr;
                bool isRejoinUser = false;
                long userId = 0;

                if (isNewUser)
                {
                    user.Password = password;

                    var oldUser = User.GetByNRIC(user.IdentificationNo, user.IdentificationType, user.IdentificationCountry);
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
                        user.AuthenticationMode = oldUser.AuthenticationMode;

                        user.CreatedBy = Constants.CreatedBy;
                        user.CreatedDate = DateTime.Now;
                        user.LastModifiedBy = Constants.CreatedBy;
                        user.LastModifiedDate = DateTime.Now;

                        userId = oldUser.Id;

                        user.LastModifiedBy = Constants.CreatedBy;
                        user.LastModifiedDate = DateTime.Now;
                        user.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        user.UserName = Pernr;//new logic
                        user.Password = password;
                        user.EmployeeId = Pernr;
                        user.CreatedBy = Constants.CreatedBy;
                        user.CreatedDate = DateTime.Now;
                        user.Save(MasterConfigHelper.ServiceUser);
                        userId = user.Id;
                        int RoleId = MasterConfigHelper.RoleUser;
                        User.InsertUserRole(userId, RoleId);

                    }
                }
                else
                {
                    user.LastModifiedBy = Constants.CreatedBy;
                    user.LastModifiedDate = DateTime.Now;
                    user.Save(MasterConfigHelper.ServiceUser);
                    userId = user.Id;
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Error while getting specific user information", ex);
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

        private void DisposePATables()
        {
            m_dtPA0000AllEmp = null;

            m_dtPA0000 = null;
            m_dtPA0001 = null;
            m_dtPA0002 = null;

            m_dtPA0185 = null;
            m_dtPA0077 = null;
            m_dtPA0105 = null;
            m_dtPA0027 = null;
            m_dtHRP1000 = null;

            m_dtT001 = null;
            m_dtT500P = null;
            m_dtT001P = null;
            m_dtCSKS = null;
        }

        private void PopulateCostCenters()
        {
            try
            {
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

                    var costCntr = NHRMSOrganisation.GetByOrgCode(costCenterCode);
                    
                    if (costCntr == null)
                    {
                        
                        NHRMSOrganisation cc = new NHRMSOrganisation()
                        {
                            IsActive = true,
                            EndpointSystemId = this.EndPoint.Id,
                            CreatedBy = Constants.CreatedBy,
                            CreatedDate = DateTime.Now,
                            LastModifiedBy = Constants.CreatedBy,
                            LastModifiedDate = DateTime.Now,
                            Scope = "CostCentre",
                            Code = costCenterCode,
                        };
                        cc.Save(MasterConfigHelper.ServiceUser);
                    }
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Trusted Source Cost Centers populated"));
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Cost Center(s) populated"));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate cost centers", ex);
            }
        }

        private void PopulateCostCenterTexts()
        {
            try
            {
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

                    if (!string.IsNullOrEmpty(costCenterCode))
                    {
                        
                        var nCstCntr = NHRMSOrganisation.GetByOrgCode(costCenterCode);

                        if (nCstCntr != null)
                        {
                            nCstCntr.Name = costCenterText;
                            nCstCntr.Description = costCenterText;
                            nCstCntr.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate cost centers", ex);
            }
        }

        private void PopulateOrganizations()
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

                foreach (KeyValuePair<string, string> orgUnit in orgUnits)
                { 
                    var nOrg = NHRMSOrganisation.GetByOrgCode(orgUnit.Key);
                    if (nOrg == null)
                    {
                        NHRMSOrganisation org = new NHRMSOrganisation()
                        {
                            IsActive = true,
                            EndpointSystemId = this.EndPoint.Id,
                            CreatedBy = Constants.CreatedBy,
                            CreatedDate = DateTime.Now,
                            LastModifiedBy = orgUnit.Value.Split('|')[1],
                            LastModifiedDate = DateTime.Now,
                            Code = orgUnit.Key,
                            Name = orgUnit.Value.Split('|')[0],
                            Scope = orgUnit.Value.Split('|')[1]
                        };
                        org.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        nOrg.Name = orgUnit.Value.Split('|')[0];
                        nOrg.Scope = orgUnit.Value.Split('|')[1];
                        nOrg.LastModifiedBy = orgUnit.Value.Split('|')[1];
                        nOrg.LastModifiedDate = DateTime.Now;
                        nOrg.Save(MasterConfigHelper.ServiceUser);
                    }
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Trusted Source Organisation units populated", orgUnits.Count.ToString()));
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} Organisation unit(s) populated", orgUnits.Count.ToString()));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate organisation units", ex);
            }
        }

        private void PopulateOrganizationHead()
        {
            try
            {

                //flush head and parent org informaation before re-populating it.
               // Organisation.FlushHeadInformation();
                this.NPopulateRedHats();
                this.PopulateParentOrganisation();
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate organisation unit heads", ex);
            }
        }


        //private void PopulateRedHats()
        //{
        //    try
        //    {
        //        TAB512Table dt = new TAB512Table();
        //        RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
        //        RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();
        //        m_dtHRP1001 = new DataTable();

        //        RFC_DB_FLD FLD_SOBID = new RFC_DB_FLD();
        //        FLD_SOBID.Fieldname = "SOBID";

        //        RFC_DB_FLD FLD_OBJID = new RFC_DB_FLD();
        //        FLD_OBJID.Fieldname = "OBJID";

        //        dtFlds.Add(FLD_OBJID);
        //        dtFlds.Add(FLD_SOBID);

        //        RFC_DB_OPT filter = new RFC_DB_OPT();
        //        //filter.Text = "RSIGN = 'B' AND RELAT = '012' AND OTYPE = 'O' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
        //        filter.Text = "RSIGN = 'B' AND RELAT = '012' AND OTYPE = 'O' AND ENDDA >= '" + today + "' ";

        //        dtopts.Add(filter);

        //        using (SAPProxy1 proxy = new SAPProxy1())
        //        {
        //            proxy.Connection = this.GetSAPConnection();
        //            proxy.Connection.Open();

        //            proxy.Rfc_Read_Table("|", null, "HRP1001", 0, 0, ref dt, ref dtFlds, ref dtopts);
        //            DataTable dtTemp = dt.ToADODataTable();
        //            DataTable dtFields = dtFlds.ToADODataTable();
        //            m_dtHRP1001 = ConvertPipesToColumns(dtFields, dtTemp);
        //        }

        //        DataTable dtPositions = this.GetPernrAndPositionMappings();

        //        if (dtPositions != null && dtPositions.Rows.Count != 0)
        //        {
        //            foreach (DataRow row in m_dtHRP1001.Rows)
        //            {
        //                string positionCode = row["SOBID"].ToString().Trim();
        //                string sectionCode = row["OBJID"].ToString().Trim();

        //                var section = Organisation.GetByOrgUnitPrefix(sectionCode);

        //                if (section != null)
        //                {
        //                    string pernr = this.GetEmployeeIdByPosition(positionCode, dtPositions);

        //                    if (!string.IsNullOrEmpty(pernr))
        //                    {
        //                        var user = User.GetByEmployeeId(pernr);

        //                        if (user != null)
        //                        {
        //                            section.Head = Convert.ToInt32(user.Id);

        //                            NOrganisationHead head = new NOrganisationHead();
        //                            head.OrganisationId = section.Id;
        //                            head.HeadId = user.Id;
        //                            head.CoordinatorId = user.Id;
        //                            head.CreatedBy = Constants.CreatedBy;
        //                            head.CreatedDate = DateTime.Now;
        //                            head.LastModifiedBy = Constants.CreatedBy;
        //                            head.LastModifiedDate = DateTime.Now;
        //                            head.Save();
        //                        }
        //                        section.Save();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate organisation unit heads", ex);
        //    }
        //}
        private void NPopulateRedHats()
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

                            var nOrg = NHRMSOrganisation.GetByOrgCode(sectionCode);

                            if (nOrg != null)
                            {
                                nOrg.Head = pernr;
                                nOrg.Save(MasterConfigHelper.ServiceUser);
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate organisation unit heads", ex);
            }
        }

        private DataTable GetPernrAndPositionMappings()
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

        private string GetEmployeeIdByPosition(string positionCode, DataTable dtPositions)
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

        private void PopulateParentOrganisation()
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

                  
                    var nOrg = NHRMSOrganisation.GetByOrgCode(sectionCode);
                    //var nParentOrg = NHRMSOrganisation.GetByOrgCode(parentSectionCode);

                    if (nOrg != null)
                    {
                        nOrg.ParentOrgCode = parentSectionCode;
                        nOrg.Save(MasterConfigHelper.ServiceUser);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate organisation unit heads", ex);
            }
        }

        private void PopulateBusinessRoles()
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
                    var nPos = NHRMSDesignation.GetByCode(position.Key, this.EndPoint.Id);
                    if (nPos == null)
                    {
                        NHRMSDesignation dsg = new NHRMSDesignation()
                        {
                            EndpointSystemId = this.EndPoint.Id,
                            Code = position.Key,
                            Description = position.Value,
                            IsActive = true,
                            CreatedBy = Constants.CreatedBy,
                            CreatedDate = DateTime.Now,
                            LastModifiedBy = Constants.CreatedBy,
                            LastModifiedDate = DateTime.Now,
                        };
                        dsg.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        nPos.Description = position.Value;
                        nPos.Save(MasterConfigHelper.ServiceUser);
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

        private void PopulateBusinessRolesDelta()
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
                        dsg.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        nPos.Description = position.Value;
                        nPos.Save(MasterConfigHelper.ServiceUser);
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
    }

}
