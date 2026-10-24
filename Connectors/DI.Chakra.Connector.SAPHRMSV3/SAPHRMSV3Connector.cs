using DI.Chakra.DBConfigHelper;
using DI.Chakra.DomainModel;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.Connector
{
    public class SAPHRMSV3Connector : ConnectorBase
    {

        EndpointConfiguration SAPConfiguration { get; set; }
        RfcDestination SAPdestination = null;
        List<EmployeeEventTrigger> triggers = new List<EmployeeEventTrigger>();
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

        //RFC_DB_FLD FLD_PERNR;
        //RFC_DB_FLD FLD_ENDDA;
        //RFC_DB_FLD FLD_VORNA;
        //RFC_DB_FLD FLD_TITEL;
        //RFC_DB_FLD FLD_NATIO;
        //RFC_DB_FLD FLD_NACHN;
        //RFC_DB_FLD FLD_MIDNM;
        //RFC_DB_FLD FLD_RUFNM;
        //RFC_DB_FLD FLD_BEGDA;
        //RFC_DB_FLD FLD_ICNUM;
        //RFC_DB_FLD FLD_RACKY;
        //RFC_DB_FLD FLD_MASSN;
        //RFC_DB_FLD FLD_STAT2;
        //RFC_DB_FLD FLD_ENAME;
        //RFC_DB_FLD FLD_KOSTL;
        //RFC_DB_FLD FLD_BUKRS;
        //RFC_DB_FLD FLD_WERKS;
        //RFC_DB_FLD FLD_BTRTL;
        //RFC_DB_FLD FLD_ORGEH;
        //RFC_DB_FLD FLD_PLANS;
        //RFC_DB_FLD FLD_PERSG;
        //RFC_DB_FLD FLD_USRTY;
        //RFC_DB_FLD FLD_USRID_LONG;
        //RFC_DB_FLD FLD_USRID;
        //RFC_DB_FLD FLD_KST01;
        //RFC_DB_FLD FLD_KSTAR;
        //RFC_DB_FLD FLD_AEDTM;


        #endregion

        RfcConfigParameters configParameters = new RfcConfigParameters();
        RfcDestination rfcSAPDestination = null;


        public SAPHRMSV3Connector(EndPoint endpoint)
        {
            this.EndPoint = endpoint;
            this.SAPConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
        }

        #region "Connector Base Members"

        public override bool Connect()
        {
            try
            {
                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                //    WriteDiscoveryLog(LogLevelType.Info, string.Format("Connected to SAP Server - {0}", this.SAPConfiguration.IPAddress));
                //    return true;
                //}
                return true;
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
        private string Configuration(EndpointKey Key)
        {
            var config = SAPConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public override void GetData()
        {
            try
            {
                int recordCount = 0;

                this.Initialize();

                this.PopulateCompanyCodes();

                this.PopulatePersonnelAreas();

                this.PopulateOrganizations();

                this.PopulatePersonnelSubAreas();

                this.PopulateCostCenters();

                this.PopulateCostCenterTexts();

                this.PopulateBusinessRoles();

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

                this.PopulateOrganizationHead();

                User.UpdateManagerForUsers();

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

        public bool DelimitEmailIdInHR(string employeeId, DateTime delimitDate)
        {
            try
            {
                //using (SAPNEWPRO proxy = new SAPNEWPRO())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                    string Lockindicator = string.Empty;
                    string Objectid = string.Empty;
                    string Recordnumber = string.Empty;
                    string Subtype = "0010";
                    string ValiditybeginLess = delimitDate.ToString("yyyyMMdd");
                    string Validityend = "99991231";

                    //SAPUserGrp.BAPIRETURN1 ret = new SAPUserGrp.BAPIRETURN1();
                    //BAPIPAKEY key = new BAPIPAKEY();
                    //BAPIP0105NLTable p0105Table = new BAPIP0105NLTable();

                    //proxy.Bapi_Emplcomm_Getdetailedlist(employeeId, Subtype, "99991231", "18000101", out ret, ref p0105Table);

                    //DataTable dt = p0105Table.ToADODataTable();

                    DataTable dt;

                    //proxy.Bapi_Emplcomm_Getdetailedlist(employeeId, Subtype, "99991231", "18000101", out ret, ref p0105Table);

                    //DataTable dt = p0105Table.ToADODataTable();


                    SAPdestination = GetSAPHRMSDestination();
                    RfcRepository bapiRepository = SAPdestination.Repository;

                    IRfcFunction readUsersFunction = bapiRepository.CreateFunction("BAPI_EMPLCOMM_GETDETAILEDLIST");

                    readUsersFunction.SetValue("EMPLOYEENUMBER", employeeId);
                    readUsersFunction.SetValue("SUBTYPE", Subtype);


                    readUsersFunction.Invoke(SAPdestination);

                    IRfcTable COMMUNICATION = readUsersFunction.GetTable("COMMUNICATION");
                    dt = COMMUNICATION.ToDataTable("COMMUNICATION");



                    //proxy.Bapi_Employee_Enqueue(employeeId, out ret);

                    IRfcFunction employeeEnqueueFunction = bapiRepository.CreateFunction("BAPI_EMPLOYEE_ENQUEUE");
                    employeeEnqueueFunction.SetValue("NUMBER", Convert.ToInt32(employeeId));
                    employeeEnqueueFunction.Invoke(SAPdestination);


                    foreach (DataRow row in dt.Rows)
                    {
                        string validBegin = row["Validbegin"].ToString();
                        string validEnd = row["Validend"].ToString();
                        Recordnumber = row["Recordnr"].ToString();
                        Objectid = row["Objectid"].ToString();
                        Lockindicator = row["Lockindic"].ToString();

                        if (validEnd == Validityend)
                        {
                            //proxy.Bapi_Emplcomm_Delimit(ValiditybeginLess, employeeId, string.Empty, string.Empty, Objectid, Recordnumber, Subtype, validBegin, validEnd, out key, out ret);

                            IRfcFunction employeeCommDelimitFunction = bapiRepository.CreateFunction("BAPI_EMPLCOMM_DELIMIT");
                            employeeCommDelimitFunction.SetValue("DELIMIT_DATE", Convert.ToDateTime(ValiditybeginLess));
                            employeeCommDelimitFunction.SetValue("EMPLOYEENUMBER", Convert.ToInt32(employeeId));
                            employeeCommDelimitFunction.SetValue("LOCKINDICATOR", Lockindicator);
                            employeeCommDelimitFunction.SetValue("OBJECTID", Objectid);
                            employeeCommDelimitFunction.SetValue("RECORDNUMBER", Convert.ToInt32(Recordnumber));
                            employeeCommDelimitFunction.SetValue("SUBTYPE", Subtype);
                            employeeCommDelimitFunction.SetValue("VALIDITYBEGIN", Convert.ToDateTime(validBegin));
                            employeeCommDelimitFunction.SetValue("VALIDITYEND", Convert.ToDateTime(validEnd));

                            employeeCommDelimitFunction.Invoke(SAPdestination);

                            string Message = employeeCommDelimitFunction.GetStructure("RETURN").GetValue("MESSAGE").ToString();

                            if (!string.IsNullOrEmpty(Message))
                            {
                                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Warning, string.Format("Error while delimiting Email Id for Pernr - {0}", employeeId));
                                //DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Warning, ret.Message);
                            }
                        }
                    }

                    //proxy.Bapi_Employee_Dequeue(employeeId, out ret);

                    IRfcFunction employeeDequeueFunction = bapiRepository.CreateFunction("BAPI_EMPLOYEE_DEQUEUE");
                    employeeDequeueFunction.SetValue("NUMBER", Convert.ToInt32(employeeId));
                    employeeDequeueFunction.Invoke(SAPdestination);


                    //proxy.CommitWork();

                    IRfcFunction employeeTranCommitFunction = bapiRepository.CreateFunction("BAPI_TRANSACTION_COMMIT");
                    employeeTranCommitFunction.Invoke(SAPdestination);


                    return true;
                //}
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
                //using (SAPNEWPRO proxy = new SAPNEWPRO())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                    string Lockindicator = string.Empty;
                    string Objectid = string.Empty;
                    string Recordnumber = string.Empty;
                    string Subtype = "0001";
                    string ValiditybeginLess = delimitDate.ToString("yyyyMMdd");
                    string Validityend = "99991231";

                    //SAPUserGrp.BAPIRETURN1 ret = new SAPUserGrp.BAPIRETURN1();
                    //BAPIPAKEY key = new BAPIPAKEY();
                    //BAPIP0105NLTable p0105Table = new BAPIP0105NLTable();

                    DataTable dt;

                    //proxy.Bapi_Emplcomm_Getdetailedlist(employeeId, Subtype, "99991231", "18000101", out ret, ref p0105Table);

                    //DataTable dt = p0105Table.ToADODataTable();


                    SAPdestination = GetSAPHRMSDestination();
                    RfcRepository bapiRepository = SAPdestination.Repository;

                    IRfcFunction readUsersFunction = bapiRepository.CreateFunction("BAPI_EMPLCOMM_GETDETAILEDLIST");

                    readUsersFunction.SetValue("EMPLOYEENUMBER", employeeId);
                    readUsersFunction.SetValue("SUBTYPE", Subtype);


                    readUsersFunction.Invoke(SAPdestination);

                    IRfcTable COMMUNICATION = readUsersFunction.GetTable("COMMUNICATION");
                    dt = COMMUNICATION.ToDataTable("COMMUNICATION");



                    //proxy.Bapi_Employee_Enqueue(employeeId, out ret);

                    IRfcFunction employeeEnqueueFunction = bapiRepository.CreateFunction("BAPI_EMPLOYEE_ENQUEUE");
                    employeeEnqueueFunction.SetValue("NUMBER", Convert.ToInt32(employeeId));
                    employeeEnqueueFunction.Invoke(SAPdestination);

                    foreach (DataRow row in dt.Rows)
                    {
                        string validBegin = row["Validbegin"].ToString();
                        string validEnd = row["Validend"].ToString();
                        Recordnumber = row["Recordnr"].ToString();
                        Objectid = row["Objectid"].ToString();
                        Lockindicator = row["Lockindic"].ToString();

                        if (validEnd == Validityend)
                        {
                            //proxy.Bapi_Emplcomm_Delimit(ValiditybeginLess, employeeId, string.Empty, string.Empty, Objectid, Recordnumber, Subtype, validBegin, validEnd, out key, out ret);

                            IRfcFunction employeeCommDelimitFunction = bapiRepository.CreateFunction("BAPI_EMPLCOMM_DELIMIT");
                            employeeCommDelimitFunction.SetValue("DELIMIT_DATE", Convert.ToDateTime(ValiditybeginLess));
                            employeeCommDelimitFunction.SetValue("EMPLOYEENUMBER", Convert.ToInt32(employeeId));
                            employeeCommDelimitFunction.SetValue("LOCKINDICATOR", Lockindicator);
                            employeeCommDelimitFunction.SetValue("OBJECTID", Objectid);
                            employeeCommDelimitFunction.SetValue("RECORDNUMBER", Convert.ToInt32(Recordnumber));
                            employeeCommDelimitFunction.SetValue("SUBTYPE", Subtype);
                            employeeCommDelimitFunction.SetValue("VALIDITYBEGIN", Convert.ToDateTime(validBegin));
                            employeeCommDelimitFunction.SetValue("VALIDITYEND", Convert.ToDateTime(validEnd));

                            employeeCommDelimitFunction.Invoke(SAPdestination);

                            string Message = employeeCommDelimitFunction.GetStructure("RETURN").GetValue("MESSAGE").ToString();

                            if (!string.IsNullOrEmpty(Message))
                            {
                                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Warning, string.Format("Error while delimiting Email Id for Pernr - {0}", employeeId));
                                
                            }


                            //if (!string.IsNullOrEmpty(ret.Message))
                            //{
                            //    DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Warning, string.Format("Error while delimiting FLINS Login Id for Pernr - {0}", employeeId));
                            //    //DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Warning, ret.Message);
                            //}



                        }
                    }

                    //proxy.Bapi_Employee_Dequeue(employeeId, out ret);

                    IRfcFunction employeeDequeueFunction = bapiRepository.CreateFunction("BAPI_EMPLOYEE_DEQUEUE");
                    employeeDequeueFunction.SetValue("NUMBER", Convert.ToInt32(employeeId));
                    employeeDequeueFunction.Invoke(SAPdestination);


                    //proxy.CommitWork();

                    IRfcFunction employeeTranCommitFunction = bapiRepository.CreateFunction("BAPI_TRANSACTION_COMMIT");
                    employeeTranCommitFunction.Invoke(SAPdestination);


                    return true;
                //}
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format("Error while delimiting FLINS Login Id in HRMS for Pernr - {0}", employeeId), ex);
                return false;
            }
        }

        //unused function, so didnt upgrade to 3.0 version

        //public override void SyncTelephoneDirectory()
        //{
        //    try
        //    {
        //        var users = User.GetActiveEmployees();
        //        var propagationFields = MasterConfigHelper.PropagationFields;
        //        bool canRunPropagationTrigger = MasterConfigHelper.CanRunPropagationTrigger;

        //        DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, "Started preparing sync data for telephone directory.");

        //        foreach (User usr in users)
        //        {
        //            if (usr.FullTimeEmployee.HasValue && usr.FullTimeEmployee.Value)
        //            {
        //                try
        //                {
        //                    int employeeId;
        //                    bool isValidPernr = int.TryParse(usr.EmployeeId, out employeeId);
        //                    if (isValidPernr)
        //                    {
        //                        using (SAPNEWPRO proxy = new SAPNEWPRO())
        //                        {
        //                            proxy.Connection = this.GetSAPConnection();
        //                            proxy.Connection.Open();

        //                            SAPUserGrp.BAPIRETURN1 return1 = new SAPUserGrp.BAPIRETURN1();
        //                            ZBAPI_PERNRTable pernrTable = new ZBAPI_PERNRTable();
        //                            ZBAPIPA_TEL_DIR_INFO1Table telDirInfo = new ZBAPIPA_TEL_DIR_INFO1Table();

        //                            ZBAPI_PERNR pernrInfo = new ZBAPI_PERNR();
        //                            pernrInfo.Sign = "I";
        //                            pernrInfo.Option0 = "EQ";
        //                            pernrInfo.Low = usr.EmployeeId;

        //                            pernrTable.Add(pernrInfo);

        //                            proxy.Z_Bapipa_Get_Tel_Dir_Info(out return1, ref pernrTable, ref  telDirInfo);

        //                            if (telDirInfo != null && telDirInfo.Count != 0)
        //                            {
        //                                usr.OrgCompany = telDirInfo[0].Company;
        //                                usr.OrgDepartment = telDirInfo[0].Department;
        //                                usr.OrgDivision = telDirInfo[0].Division;
        //                                usr.OrgBranch = telDirInfo[0].Branch;
        //                                usr.OrgSection = telDirInfo[0].Section;

        //                                usr.Fax = telDirInfo[0].Fax_Number;
        //                                usr.TelephoneNumber = telDirInfo[0].Office_Number;
        //                                usr.MobileNumber = telDirInfo[0].Mobile_Number;

        //                                usr.PostalCode = telDirInfo[0].Postal_Code;
        //                                usr.PostalAddress = telDirInfo[0].Office_Address;
        //                                usr.Country = telDirInfo[0].Country;

        //                                if (!string.IsNullOrEmpty(telDirInfo[0].User_Job_Title))
        //                                {
        //                                    usr.PositionName = telDirInfo[0].User_Job_Title;
        //                                }

        //                                User.UpdateUserFromDiscovery(usr, propagationFields, canRunPropagationTrigger);
        //                            }
        //                            else
        //                            {
        //                                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Warning, string.Format("Telephone directory info for the user - {0}, Pernr - {1}. is missing.", usr.KnownAs, usr.EmployeeId));
        //                            }
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format("Error while synchronising data to telephone directory for the user - {0}, Pernr - {1}.", usr.KnownAs, usr.EmployeeId), ex);
        //                }
        //            }
        //        }
        //        DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Info, "Completed preparing sync data for telephone directory.");
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, "Error while synchronising data to telephone directory.", ex);
        //    }
        //}
        /*
        public override bool AddEmailIdInHR(string employeeId, string emailId, DateTime beginDate)
        {
            try
            {
                //using (SAPNEWPRO proxy = new SAPNEWPRO())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                bool isAdded = false;

                string Lockindicator = string.Empty;
                string Objectid = string.Empty;
                string Recordnumber = string.Empty;
                string Subtype = "0010";
                string ValiditybeginLess = beginDate.AddDays(-1).ToString("yyyyMMdd");
                string Validitybegin = beginDate.ToString("yyyyMMdd");
                string Validityend = "99991231";

                //SAPUserGrp.BAPIRETURN1 ret = new SAPUserGrp.BAPIRETURN1();
                //BAPIPAKEY key = new BAPIPAKEY();
                //BAPIP0105NLTable p0105Table = new BAPIP0105NLTable();
                DataTable dt;

                //proxy.Bapi_Emplcomm_Getdetailedlist(employeeId, Subtype, "99991231", "18000101", out ret, ref p0105Table);

                //DataTable dt = p0105Table.ToADODataTable();


                SAPdestination = GetSAPHRMSDestination();
                RfcRepository bapiRepository = SAPdestination.Repository;

                IRfcFunction readUsersFunction = bapiRepository.CreateFunction("BAPI_EMPLCOMM_GETDETAILEDLIST");

                readUsersFunction.SetValue("EMPLOYEENUMBER", employeeId);
                readUsersFunction.SetValue("SUBTYPE", Subtype);


                readUsersFunction.Invoke(SAPdestination);

                IRfcTable COMMUNICATION = readUsersFunction.GetTable("COMMUNICATION");
                dt = COMMUNICATION.ToDataTable("COMMUNICATION");


                //proxy.Bapi_Employee_Enqueue(employeeId, out ret);

                IRfcFunction employeeEnqueueFunction = bapiRepository.CreateFunction("BAPI_EMPLOYEE_ENQUEUE");
                employeeEnqueueFunction.SetValue("NUMBER", Convert.ToInt32(employeeId));
                employeeEnqueueFunction.Invoke(SAPdestination);

                foreach (DataRow row in dt.Rows)
                {
                    string validBegin = row["Validbegin"].ToString();
                    string validEnd = row["Validend"].ToString();
                    Recordnumber = row["Recordnr"].ToString();
                    Objectid = row["Objectid"].ToString();
                    Lockindicator = row["Lockindic"].ToString();

                    if (validBegin == Validitybegin)
                    {
                        //proxy.Bapi_Emplcomm_Delete(employeeId, Lockindicator, string.Empty, Objectid, Recordnumber, Subtype, validBegin, validEnd, out ret);

                        IRfcFunction employeeCommDeleteFunction = bapiRepository.CreateFunction("BAPI_EMPLCOMM_DELETE");
                        employeeCommDeleteFunction.SetValue("EMPLOYEENUMBER", Convert.ToInt32(employeeId));
                        employeeCommDeleteFunction.SetValue("LOCKINDICATOR", Lockindicator);
                        employeeCommDeleteFunction.SetValue("OBJECTID", Objectid);
                        employeeCommDeleteFunction.SetValue("RECORDNUMBER", Convert.ToInt32(Recordnumber));
                        employeeCommDeleteFunction.SetValue("SUBTYPE", Subtype);
                        employeeCommDeleteFunction.SetValue("VALIDITYBEGIN", Convert.ToDateTime(validBegin));
                        employeeCommDeleteFunction.SetValue("VALIDITYEND", Convert.ToDateTime(validEnd));

                        employeeCommDeleteFunction.Invoke(SAPdestination);


                    }
                    else if (validEnd == Validityend)
                    {
                        //proxy.Bapi_Emplcomm_Delimit(ValiditybeginLess, employeeId, Lockindicator, string.Empty, Objectid, Recordnumber, Subtype, validBegin, validEnd, out key, out ret);

                        IRfcFunction employeeCommDelimitFunction = bapiRepository.CreateFunction("BAPI_EMPLCOMM_DELIMIT");
                        employeeCommDelimitFunction.SetValue("DELIMIT_DATE", Convert.ToDateTime(ValiditybeginLess));
                        employeeCommDelimitFunction.SetValue("EMPLOYEENUMBER", Convert.ToInt32(employeeId));
                        employeeCommDelimitFunction.SetValue("LOCKINDICATOR", Lockindicator);
                        employeeCommDelimitFunction.SetValue("OBJECTID", Objectid);
                        employeeCommDelimitFunction.SetValue("RECORDNUMBER", Convert.ToInt32(Recordnumber));
                        employeeCommDelimitFunction.SetValue("SUBTYPE", Subtype);
                        employeeCommDelimitFunction.SetValue("VALIDITYBEGIN", Convert.ToDateTime(validBegin));
                        employeeCommDelimitFunction.SetValue("VALIDITYEND", Convert.ToDateTime(validEnd));

                        employeeCommDelimitFunction.Invoke(SAPdestination);

                    }
                }

                //SAPNEWPRO proxy = new SAPNEWPRO();
                //proxy.Bapi_Emplcomm_Create(emailId, string.Empty, out Lockindicator, out Objectid, out Recordnumber, out ret, ref employeeId, ref Subtype, ref Validitybegin, ref Validityend);

                IRfcFunction employeeCommCreateFunction = bapiRepository.CreateFunction("BAPI_EMPLCOMM_CREATE");
                employeeCommCreateFunction.SetValue("COMMUNICATIONID", Convert.ToInt32(emailId));
                employeeCommCreateFunction.SetValue("VALIDITYEND", Convert.ToDateTime(Validityend));
                employeeCommCreateFunction.SetValue("EMPLOYEENUMBER", Convert.ToInt32(employeeId));
                employeeCommCreateFunction.SetValue("SUBTYPE", Subtype);
                employeeCommCreateFunction.SetValue("VALIDITYBEGIN", Convert.ToDateTime(Validitybegin));
                employeeCommCreateFunction.SetValue("VALIDITYEND", Convert.ToDateTime(Validityend));


                employeeCommCreateFunction.Invoke(SAPdestination);

                string Message = employeeCommCreateFunction.GetStructure("RETURN").GetValue("MESSAGE").ToString();

                //isAdded = string.IsNullOrEmpty(ret.Message);
                isAdded = string.IsNullOrEmpty(Message);

                //if (!string.IsNullOrEmpty(ret.Message))
                if (!string.IsNullOrEmpty(Message))
                {
                    DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Warning, string.Format("Error while adding Email Id for Pernr - {0}", employeeId));
                    //DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Warning, ret.Message);
                }

                //proxy.Bapi_Employee_Dequeue(employeeId, out ret);

                IRfcFunction employeeDequeueFunction = bapiRepository.CreateFunction("BAPI_EMPLOYEE_DEQUEUE");
                employeeDequeueFunction.SetValue("NUMBER", Convert.ToInt32(employeeId));
                employeeDequeueFunction.Invoke(SAPdestination);


                //proxy.CommitWork();

                IRfcFunction employeeTranCommitFunction = bapiRepository.CreateFunction("BAPI_TRANSACTION_COMMIT");
                employeeTranCommitFunction.Invoke(SAPdestination);

                return isAdded;
                //}
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
                //using (SAPNEWPRO proxy = new SAPNEWPRO())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                bool isAdded = false;

                string Lockindicator = string.Empty;
                string Objectid = string.Empty;
                string Recordnumber = string.Empty;
                string Subtype = "0001";
                string Validitybegin = beginDate.ToString("yyyyMMdd");
                string Validityend = "99991231";
                SAPdestination = GetSAPHRMSDestination();
                RfcRepository bapiRepository = SAPdestination.Repository;

                //SAPUserGrp.BAPIRETURN1 ret = new SAPUserGrp.BAPIRETURN1();

                //proxy.Bapi_Employee_Enqueue(employeeId, out ret);

                IRfcFunction employeeEnqueueFunction = bapiRepository.CreateFunction("BAPI_EMPLOYEE_ENQUEUE");
                employeeEnqueueFunction.SetValue("NUMBER", Convert.ToInt32(employeeId));
                employeeEnqueueFunction.Invoke(SAPdestination);


                //proxy.Bapi_Emplcomm_Create(sapLogonId, string.Empty, out Lockindicator, out Objectid, out Recordnumber, out ret, ref employeeId, ref Subtype, ref Validitybegin, ref Validityend);

                IRfcFunction employeeCommCreateFunction = bapiRepository.CreateFunction("BAPI_EMPLCOMM_CREATE");
                employeeCommCreateFunction.SetValue("EMPLOYEENUMBER", Convert.ToInt32(sapLogonId));
                employeeCommCreateFunction.SetValue("SUBTYPE", Subtype);
                employeeCommCreateFunction.SetValue("VALIDITYBEGIN", Convert.ToDateTime(Validitybegin));
                employeeCommCreateFunction.SetValue("VALIDITYEND", Convert.ToDateTime(Validityend));

                employeeCommCreateFunction.Invoke(SAPdestination);

                string Message = employeeCommCreateFunction.GetStructure("RETURN").GetValue("MESSAGE").ToString();

                //isAdded = string.IsNullOrEmpty(ret.Message);
                isAdded = string.IsNullOrEmpty(Message);

                //if (!string.IsNullOrEmpty(ret.Message))
                //{
                //    //.ProvisioningLogger.WriteLog(null,  LogLevelType.Warning, string.Format("Error while adding FLINS Login Id for Pernr - {0}", employeeId));
                //    //DILog.ProvisioningLogger.WriteLog(null,  LogLevelType.Warning, ret.Message);
                //}

                IRfcFunction employeeDequeueFunction = bapiRepository.CreateFunction("BAPI_EMPLOYEE_DEQUEUE");
                employeeDequeueFunction.SetValue("NUMBER", Convert.ToInt32(employeeId));
                employeeDequeueFunction.Invoke(SAPdestination);


                //proxy.CommitWork();

                IRfcFunction employeeTranCommitFunction = bapiRepository.CreateFunction("BAPI_TRANSACTION_COMMIT");
                employeeTranCommitFunction.Invoke(SAPdestination);

                return isAdded;
                //}
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Exception, string.Format("Error while updating FLINS Login Id - {0} in HRMS for Pernr - {1}", sapLogonId, employeeId), ex);
                return false;
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
            //FLD_PERNR = new RFC_DB_FLD();
            //FLD_PERNR.Fieldname = "PERNR";

            //FLD_ENDDA = new RFC_DB_FLD();
            //FLD_ENDDA.Fieldname = "ENDDA";

            //FLD_VORNA = new RFC_DB_FLD();
            //FLD_VORNA.Fieldname = "VORNA";

            //FLD_TITEL = new RFC_DB_FLD();
            //FLD_TITEL.Fieldname = "TITEL";

            //FLD_NATIO = new RFC_DB_FLD();
            //FLD_NATIO.Fieldname = "NATIO";

            //FLD_NACHN = new RFC_DB_FLD();
            //FLD_NACHN.Fieldname = "NACHN";

            //FLD_MIDNM = new RFC_DB_FLD();
            //FLD_MIDNM.Fieldname = "MIDNM";

            //FLD_RUFNM = new RFC_DB_FLD();
            //FLD_RUFNM.Fieldname = "RUFNM";

            //FLD_BEGDA = new RFC_DB_FLD();
            //FLD_BEGDA.Fieldname = "BEGDA";

            //FLD_ICNUM = new RFC_DB_FLD();
            //FLD_ICNUM.Fieldname = "ICNUM";

            //FLD_RACKY = new RFC_DB_FLD();
            //FLD_RACKY.Fieldname = "RACKY";

            //FLD_MASSN = new RFC_DB_FLD();
            //FLD_MASSN.Fieldname = "MASSN";

            //FLD_STAT2 = new RFC_DB_FLD();
            //FLD_STAT2.Fieldname = "STAT2";

            //FLD_ENAME = new RFC_DB_FLD();
            //FLD_ENAME.Fieldname = "ENAME";

            //FLD_KOSTL = new RFC_DB_FLD();
            //FLD_KOSTL.Fieldname = "KOSTL";

            //FLD_BUKRS = new RFC_DB_FLD();
            //FLD_BUKRS.Fieldname = "BUKRS";

            //FLD_WERKS = new RFC_DB_FLD();
            //FLD_WERKS.Fieldname = "WERKS";

            //FLD_BTRTL = new RFC_DB_FLD();
            //FLD_BTRTL.Fieldname = "BTRTL";

            //FLD_ORGEH = new RFC_DB_FLD();
            //FLD_ORGEH.Fieldname = "ORGEH";

            //FLD_PLANS = new RFC_DB_FLD();
            //FLD_PLANS.Fieldname = "PLANS";

            //FLD_PERSG = new RFC_DB_FLD();
            //FLD_PERSG.Fieldname = "PERSG";

            //FLD_USRTY = new RFC_DB_FLD();
            //FLD_USRTY.Fieldname = "USRTY";

            //FLD_USRID_LONG = new RFC_DB_FLD();
            //FLD_USRID_LONG.Fieldname = "USRID_LONG";

            //FLD_USRID = new RFC_DB_FLD();
            //FLD_USRID.Fieldname = "USRID";

            //FLD_KST01 = new RFC_DB_FLD();
            //FLD_KST01.Fieldname = "KST01";

            //FLD_KSTAR = new RFC_DB_FLD();
            //FLD_KSTAR.Fieldname = "KSTAR";

            //FLD_AEDTM = new RFC_DB_FLD();
            //FLD_AEDTM.Fieldname = "AEDTM";
            //delta date 
            if (!this.IsFirstTimeDiscovery)
            {
                deltaDate = this.Process.LastRunOn.Value.ToString("yyyyMMdd");
            }
        }

        //private SAPConnection GetSAPConnection()
        //{
        //    Destination destination = new Destination();
        //    string pwd = RSAHelper.DecryptData(SAPConfiguration.Password);
        //    destination.AppServerHost = this.SAPConfiguration.IPAddress; //Configuration.IPAddress;10.168.15.24";//
        //    destination.Client = Convert.ToInt16(SAPConfiguration.ClientNumber); //Configuration.IntClientNum; 300;//
        //    destination.Password = pwd; //Configuration.Password;"orange123"; //
        //    destination.SystemNumber = Convert.ToInt16(SAPConfiguration.SystemNumber); //Configuration.SystemNumber;00;//
        //    destination.Username = SAPConfiguration.UserName; //Configuration.UserName;sisuis";//"
        //    SAPConnection connection = new SAPConnection(destination);
        //    return connection;
        //}

        private RfcDestination GetSAPHRMSDestination()
        {


            if (configParameters.Count == 0)
            {
                //            string decryptedPassword = RSAHelper.DecryptData(this.SAPConfiguration.Password);

                configParameters.Add(RfcConfigParameters.AppServerHost, "192.168.15.183");
                configParameters.Add(RfcConfigParameters.SystemNumber, "00");
                configParameters.Add(RfcConfigParameters.User, "sapuser");
                configParameters.Add(RfcConfigParameters.Password, "pass1234");
                configParameters.Add(RfcConfigParameters.Client, "800");
                configParameters.Add(RfcConfigParameters.Language, "EN");
                configParameters.Add(RfcConfigParameters.Name, "SAPDestination");


                rfcSAPDestination = RfcDestinationManager.GetDestination(configParameters);


            }

            return rfcSAPDestination;
        }

        private RfcDestination GetSAPHRMSDestination(int temp)
        {


            if (configParameters.Count == 0)
            {
                string decryptedPassword = RSAHelper.DecryptData(Configuration(EndpointKey.Password));

                configParameters.Add(RfcConfigParameters.AppServerHost, Configuration(EndpointKey.IPAddress));
                configParameters.Add(RfcConfigParameters.SystemNumber, Configuration(EndpointKey.SystemNumber));
                configParameters.Add(RfcConfigParameters.User, Configuration(EndpointKey.UserName));
                configParameters.Add(RfcConfigParameters.Password, decryptedPassword);
                configParameters.Add(RfcConfigParameters.Client, Configuration(EndpointKey.ClientNumber));
                configParameters.Add(RfcConfigParameters.Language, "EN");
                configParameters.Add(RfcConfigParameters.Name, "SAPDestination");


                rfcSAPDestination = RfcDestinationManager.GetDestination(configParameters);


            }

            return rfcSAPDestination;
        }


        private int GetCompletePAInformation()
        {

            //TAB512Table dt = new TAB512Table();
            //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();
            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;

            try
            {
                WriteDiscoveryLog(LogLevelType.Info, "Getting Complete PA Information");

                string Pernr = string.Empty;
                bool isNewUser;

                string password = RSAHelper.EncryptData(MasterConfigHelper.DefaultPassword);
                
                var sections = NOrganisation.GetAll();
                var positions = NDesignation.GetAll();

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                    SAPdestination = GetSAPHRMSDestination();
                    bapiRepository = SAPdestination.Repository;
                    string filter; 

                    #region  All Emp PA0000 Table
                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE","PA0000");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "PERNR" );
                    dtFields = fieldTable.ToDataTable("FIELDS");

                    filter = "ENDDA >= '" + today + "' ";
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT",filter);

                    readTableFunction.Invoke(SAPdestination);

                    readTable = readTableFunction.GetTable("DATA");

                    dtTemp = readTable.ToDataTable("PA0000");

                    //dtFlds.Add(FLD_PERNR);
                    //RFC_DB_OPT filter = new RFC_DB_OPT();
                    //filter.Text = "ENDDA >= '" + today + "' ";
                    //dtopts.Add(filter);
                    //SAPProxy1 proxy = new SAPProxy1();
                    //proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000AllEmp = ConvertPipesToColumns(dtFields, dtTemp);
                    #endregion
                //}

                //TRUSTED SOURCE USER INFO                
                NOrganisation section = null;
                NDesignation position = null;

                DataRow[] rows;

                foreach (DataRow person in m_dtPA0000AllEmp.Rows)
                {
                    Pernr = person["PERNR"].ToString().Trim();

                    //RFC_DB_OPT FILTER_PERNR = new RFC_DB_OPT();
                    //FILTER_PERNR.Text = "PERNR = '" + Pernr + "' ";

                    //RFC_DB_OPT FILTER_PERNR_ENDDA = new RFC_DB_OPT();
                    //FILTER_PERNR_ENDDA.Text = "PERNR = '" + Pernr + "' AND ENDDA >= '" + today + "'";


                    #region  PA0000 Table
                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE","PA0000");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "BEGDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "STAT2");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "MASSN");
                    dtFields = fieldTable.ToDataTable("FIELDS");

                    filter = "PERNR = '" + Pernr + "' ";
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT",filter);

                    readTableFunction.Invoke(SAPdestination);

                    readTable = readTableFunction.GetTable("DATA");

                    dtTemp = readTable.ToDataTable("PA0000");


                        

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_BEGDA);
                    //dtFlds.Add(FLD_ENDDA);
                    //dtFlds.Add(FLD_STAT2);
                    //dtFlds.Add(FLD_MASSN);

                    //dtopts.Add(FILTER_PERNR);

                    //proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();
                    m_dtPA0000 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0001 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_BEGDA);
                    //dtFlds.Add(FLD_ENDDA);
                    //dtFlds.Add(FLD_ENAME);
                    //dtFlds.Add(FLD_KOSTL);
                    //dtFlds.Add(FLD_BUKRS);
                    //dtFlds.Add(FLD_WERKS);
                    //dtFlds.Add(FLD_BTRTL);
                    //dtFlds.Add(FLD_ORGEH);
                    //dtFlds.Add(FLD_PLANS);
                    //dtFlds.Add(FLD_PERSG);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0001");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "BEGDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENAME");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "KOSTL");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "BUKRS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "WERKS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "BTRTL");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ORGEH");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "PLANS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "PERSG");

                    dtFields = fieldTable.ToDataTable("FIELDS");

                    filter = "PERNR = '" + Pernr + "' AND ENDDA >= '" + today + "'"; 
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filter);

                    readTableFunction.Invoke(SAPdestination);

                    readTable = readTableFunction.GetTable("DATA");

                    dtTemp = readTable.ToDataTable("PA0001");


                    //dtFields = dtFlds.ToADODataTable();
                    m_dtPA0001 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0002 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_PERNR);
                    //dtFlds.Add(FLD_VORNA);
                    //dtFlds.Add(FLD_TITEL);
                    //dtFlds.Add(FLD_NATIO);
                    //dtFlds.Add(FLD_NACHN);
                    //dtFlds.Add(FLD_MIDNM);
                    //dtFlds.Add(FLD_RUFNM);
                    //dtFlds.Add(FLD_ENDDA);

                    //dtopts.Add(FILTER_PERNR);

                    //proxy.Rfc_Read_Table("|", " ", "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtFields = dtFlds.ToADODataTable();
                    //DataTable dtParameters = dt.ToADODataTable();


                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0002");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "PERNR");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "VORNA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "TITEL");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "NATIO");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "NACHN");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "MIDNM");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "RUFNM");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");

                    dtFields = fieldTable.ToDataTable("FIELDS");

                    filter = "PERNR = '" + Pernr + "' ";
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filter);

                    readTableFunction.Invoke(SAPdestination);

                    readTable = readTableFunction.GetTable("DATA");

                    DataTable dtParameters = readTable.ToDataTable("PA0002");
                    m_dtPA0002 = ConvertPipesToColumns(dtFields, dtParameters);

                    #endregion

                    #region  PA0185 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_ICNUM);
                    //dtFlds.Add(FLD_ENDDA);

                    //dtopts.Add(FILTER_PERNR);

                    //proxy.Rfc_Read_Table("|", null, "PA0185", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0185");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ICNUM");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    dtFields = fieldTable.ToDataTable("FIELDS");

                    filter = "PERNR = '" + Pernr + "' ";
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filter);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtTemp = readTable.ToDataTable("PA0185");


                    m_dtPA0185 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0077 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_RACKY);

                    //dtopts.Add(FILTER_PERNR);

                    //proxy.Rfc_Read_Table("|", null, "PA0077", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0077");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "RACKY");
                        

                    filter = "PERNR = '" + Pernr + "' ";
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filter);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtTemp = readTable.ToDataTable("PA0077");

                    m_dtPA0077 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0105 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_ENDDA);
                    //dtFlds.Add(FLD_USRTY);
                    //dtFlds.Add(FLD_USRID_LONG);
                    //dtFlds.Add(FLD_USRID);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0105");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "USRTY");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "USRID_LONG");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "USRID");

                    filter = "PERNR = '" + Pernr + "' ";
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filter);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtTemp = readTable.ToDataTable("PA0105");


                    m_dtPA0105 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0027 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_ENDDA);
                    //dtFlds.Add(FLD_KST01);
                    //dtFlds.Add(FLD_KSTAR);

                    //dtopts.Add(FILTER_PERNR);

                    //proxy.Rfc_Read_Table("|", null, "PA0027", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();


                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0027");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "KST01");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "KSTAR");
                        

                    filter = "PERNR = '" + Pernr + "' ";
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filter);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtTemp = readTable.ToDataTable("PA0027");

                    m_dtPA0027 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    

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

                    //if (string.IsNullOrEmpty(user.NRIC))
                    //{
                    //    WriteLog( LogLevelType.Error, string.Format("NRIC is empty or missing for the user - {0}", Pernr), null, DiscoveryObjectType.None, 0);
                    //    continue;
                    //}

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

                    var sectionCode = CheckNull.AgainstString(rows[0]["ORGEH"]).Trim();
                    section = sections.FirstOrDefault(item => item.Code == sectionCode);
                    if (section != null)
                    {
                        user.NOrganisationId = Convert.ToInt32(section.Id);
                    }

                    var positionCode = CheckNull.AgainstString(rows[0]["PLANS"]).Trim();
                    position = positions.FirstOrDefault(item => item.Code == positionCode);
                    if (position != null)
                    {
                        user.NBusinessRoleId = Convert.ToInt32(position.Id);
                    }

                    user.Password = password;
                    user.EmployeeId = Pernr;
                    user.FullTimeEmployee = true;
                    user.EmployeeType = EmployeeType.Permanent;

                    bool isRejoinUser = false;
                    long userId = 0;

                    if (isNewUser)
                    {
                        var oldUser = User.GetByNRIC(user.IdentificationNo, user.IdentificationType, user.IdentificationCountry);
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
                            
                            user.AuthenticationMode = oldUser.AuthenticationMode;

                            

                            userId = oldUser.Id;

                            
                            user.Save(MasterConfigHelper.ServiceUser);
                        }
                        else
                        {
                            user.UserName = Pernr;//new logic
                            user.Password = password;
                            
                            //Temp Fix, to avoid exception.
                            if (string.IsNullOrEmpty(user.DisplayName) == false)
                                user.Save(MasterConfigHelper.ServiceUser);
                            userId = user.Id;
                            int RoleId = MasterConfigHelper.RoleUser;
                            User.InsertUserRole(userId, RoleId);
                        }
                    }
                    else
                    {
                        user.Save(MasterConfigHelper.ServiceUser);
                        userId = user.Id;
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

                //dt = null;
                //dtFlds = null;
                //dtopts = null;
                dtTemp = null;
                dtFields = null;
                bapiRepository = null;
                readTableFunction = null;
                readTable = null;
                fieldTable = null;
                optionsTable = null;
                
            }
        }

        private int GetDeltaPAInformation()
        {

            //TAB512Table dt = new TAB512Table();
            //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();

            List<string> employeeIds = new List<string>();            
            List<NOrganisation> sections = new List<NOrganisation>();
            List<NDesignation> positions = new List<NDesignation>();

            triggers = new List<EmployeeEventTrigger>();

            string Pernr = string.Empty;
            bool isNewUser;

            var propagationFields = MasterConfigHelper.PropagationFields;
            bool canRunPropagationTrigger = MasterConfigHelper.CanRunPropagationTrigger;

            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;

            try
            {
                WriteDiscoveryLog(LogLevelType.Info, "Getting Delta PA Information");

                string password = RSAHelper.EncryptData(MasterConfigHelper.DefaultPassword);
                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;
                filter = "AEDTM >= '" + deltaDate + "' ";

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                    //RFC_DB_OPT filter = new RFC_DB_OPT();
                    //filter = "AEDTM >= '" + deltaDate + "' ";

                #region  PA0000 Table

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0000");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");

                //filter = "AEDTM >= '" + deltaDate + "' ";
                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0000");


                //dtFlds.Add(FLD_PERNR);

                //dtopts.Add(filter);

                //proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                #region PA0001 Table

                //proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0001");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");

                //filter = "ENDDA >= '" + today + "' ";
                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0001");


                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                #region PA0002 Table

                //proxy.Rfc_Read_Table("|", null, "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0002");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");

                //filter = "ENDDA >= '" + today + "' ";
                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0002");

                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                #region  PA0185 Table

                //proxy.Rfc_Read_Table("|", null, "PA0185", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0185");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");

                //filter = "ENDDA >= '" + today + "' ";
                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0185");



                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                #region PA0077 Table

                //proxy.Rfc_Read_Table("|", null, "PA0077", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0077");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");

                //filter = "ENDDA >= '" + today + "' ";
                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0077");


                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                #region PA0105 Table

                //proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0105");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");

                //filter = "ENDDA >= '" + today + "' ";
                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0105");




                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                #region PA0027 Table

                //proxy.Rfc_Read_Table("|", null, "PA0027", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0027");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");

                //filter = "ENDDA >= '" + today + "' ";
                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0027");

                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                

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
                NDesignation position = null;

                DataRow[] rows;

                if (employeeIds.Count > 0)
                {
                    sections = NOrganisation.GetAll();
                    positions = NDesignation.GetAll();
                }

                foreach (string person in employeeIds)
                {
                    long userId;

                    bool isRejoinUser = false;
                    bool offBoardExists = false;
                    bool onBoardExists = false;

                    PresentationOrgChangeTrack orgTrack = null;
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

                    //RFC_DB_OPT FILTER_DELTA_DATA = new RFC_DB_OPT();
                    //FILTER_DELTA_DATA.Text = "PERNR = '" + Pernr + "' AND AEDTM >= '" + deltaDate + "'";

                    //RFC_DB_OPT FILTER_PERNR_ENDDA = new RFC_DB_OPT();
                    //FILTER_PERNR_ENDDA.Text = "PERNR = '" + Pernr + "' AND ENDDA >= '" + deltaDate + "'";

                    string filterDeltaData = "PERNR = '" + Pernr + "' AND AEDTM >= '" + deltaDate + "'"; 
                    string filterPernEndDA = "PERNR = '" + Pernr + "' AND ENDDA >= '" + deltaDate + "'"; 

                    //using (SAPProxy1 proxy = new SAPProxy1())
                    //{
                    //    proxy.Connection = this.GetSAPConnection();
                    //    proxy.Connection.Open();

                    #region  PA0000 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_BEGDA);
                    //dtFlds.Add(FLD_ENDDA);
                    //dtFlds.Add(FLD_STAT2);
                    //dtFlds.Add(FLD_MASSN);

                    //dtopts.Add(FILTER_DELTA_DATA);

                    //proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0000");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "BEGDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "STAT2");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "MASSN");
                    dtFields = fieldTable.ToDataTable("FIELDS");

                    //filter = "PERNR = '" + Pernr + "' ";
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterDeltaData);

                    readTableFunction.Invoke(SAPdestination);

                    readTable = readTableFunction.GetTable("DATA");

                    dtTemp = readTable.ToDataTable("PA0000");

                    m_dtPA0000 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0001 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_ENDDA);
                    //dtFlds.Add(FLD_ENAME);
                    //dtFlds.Add(FLD_KOSTL);
                    //dtFlds.Add(FLD_BUKRS);
                    //dtFlds.Add(FLD_WERKS);
                    //dtFlds.Add(FLD_BTRTL);
                    //dtFlds.Add(FLD_ORGEH);
                    //dtFlds.Add(FLD_PLANS);
                    //dtFlds.Add(FLD_PERSG);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();


                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0001");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENAME");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "KOSTL");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "BUKRS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "WERKS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "BTRTL");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ORGEH");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "PLANS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "PERSG");

                    dtFields = fieldTable.ToDataTable("FIELDS");

                    //filter = "PERNR = '" + Pernr + "' AND ENDDA >= '" + today + "'";
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterPernEndDA);

                    readTableFunction.Invoke(SAPdestination);

                    readTable = readTableFunction.GetTable("DATA");

                    dtTemp = readTable.ToDataTable("PA0001");

                    //dtFields = dtFlds.ToADODataTable();

                    m_dtPA0001 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0002 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_PERNR);
                    //dtFlds.Add(FLD_VORNA);
                    //dtFlds.Add(FLD_TITEL);
                    //dtFlds.Add(FLD_NATIO);
                    //dtFlds.Add(FLD_NACHN);
                    //dtFlds.Add(FLD_MIDNM);
                    //dtFlds.Add(FLD_RUFNM);
                    //dtFlds.Add(FLD_ENDDA);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", " ", "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtFields = dtFlds.ToADODataTable();
                    //DataTable dtParameters = dt.ToADODataTable();


                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0002");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "PERNR");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "VORNA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "TITEL");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "NATIO");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "NACHN");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "MIDNM");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "RUFNM");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");

                    dtFields = fieldTable.ToDataTable("FIELDS");

                    //filter = "PERNR = '" + Pernr + "' ";
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterPernEndDA);

                    readTableFunction.Invoke(SAPdestination);

                    readTable = readTableFunction.GetTable("DATA");

                    DataTable dtParameters = readTable.ToDataTable("PA0002");

                    m_dtPA0002 = ConvertPipesToColumns(dtFields, dtParameters);

                    #endregion

                    #region  PA0185 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_ICNUM);
                    //dtFlds.Add(FLD_ENDDA);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", null, "PA0185", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();


                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0185");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ICNUM");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    dtFields = fieldTable.ToDataTable("FIELDS");

                    //filter = "PERNR = '" + Pernr + "' ";
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterPernEndDA);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtTemp = readTable.ToDataTable("PA0185");

                    m_dtPA0185 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0077 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_RACKY);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", null, "PA0077", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();


                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0077");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "RACKY");


                    //filter = "PERNR = '" + Pernr + "' ";
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterPernEndDA);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtTemp = readTable.ToDataTable("PA0077");


                    m_dtPA0077 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0105 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_ENDDA);
                    //dtFlds.Add(FLD_USRTY);
                    //dtFlds.Add(FLD_USRID_LONG);
                    //dtFlds.Add(FLD_USRID);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();


                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0105");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "USRTY");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "USRID_LONG");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "USRID");

                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterPernEndDA);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtTemp = readTable.ToDataTable("PA0105");


                    m_dtPA0105 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0027 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_ENDDA);
                    //dtFlds.Add(FLD_KST01);
                    //dtFlds.Add(FLD_KSTAR);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", null, "PA0027", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0027");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "KST01");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "KSTAR");


                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterPernEndDA);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtTemp = readTable.ToDataTable("PA0027");


                    m_dtPA0027 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    //}

                    rows = (from tmpDataRow in m_dtPA0185.AsEnumerable()
                            where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            select tmpDataRow).ToArray<DataRow>();

                    //skip users without nric
                    if (null != rows && rows.Count() > 0)
                    {
                        user.IdentificationNo = CheckNull.AgainstString(rows[0]["ICNUM"].ToString().Trim());
                    }

                    //if (string.IsNullOrEmpty(user.NRIC))
                    //{
                    //    WriteLog( LogLevelType.Error, string.Format("NRIC is empty or missing for the user - {0}", Pernr), null, DiscoveryObjectType.None, 0);
                    //    continue;
                    //}

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
                            user.JoinDate = UtilityMethods.GetDateFromString(Joindate);
                            onBoardExists = true;
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

                            offBoardExists = true;
                        }
                    }

                    rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                            where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            && MasterConfigHelper.TranferTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
                            select tmpDataRow).ToArray<DataRow>();

                    if (rows != null && rows.Count() > 0)
                    {
                        DateTime tempBeginDate = UtilityMethods.GetDateFromString(CheckNull.AgainstString(rows[0]["BEGDA"].ToString()).Trim());

                        //if (tempBeginDate >= DateTime.Today)
                        //{
                        if (orgTrack == null) orgTrack = new PresentationOrgChangeTrack();
                        orgTrack.Event = EmployeeEvents.OUChange;
                        orgTrack.EffectiveDate = tempBeginDate;
                        //}
                    }

                    rows = (from tmpDataRow in m_dtPA0000.AsEnumerable()
                            where UtilityMethods.GetDateFromString(tmpDataRow.Field<string>("ENDDA")) >= DateTime.Today
                            && MasterConfigHelper.ReOrgTriggers.Contains(tmpDataRow.Field<string>("MASSN").Trim())
                            select tmpDataRow).ToArray<DataRow>();

                    if (rows != null && rows.Count() > 0)
                    {
                        DateTime tempBeginDate = UtilityMethods.GetDateFromString(CheckNull.AgainstString(rows[0]["BEGDA"].ToString()).Trim());

                        //if (tempBeginDate >= DateTime.Today)
                        //{
                        if (orgTrack == null) orgTrack = new PresentationOrgChangeTrack();
                        orgTrack.Event = EmployeeEvents.ReOrganisation;
                        orgTrack.EffectiveDate = tempBeginDate;
                        //}
                    }

                    rows = (from tmpDataRow in m_dtPA0001.AsEnumerable()
                            select tmpDataRow).ToArray<DataRow>();

                    if (null != rows && rows.Count() > 1)
                    {
                        user.DisplayName = CheckNull.AgainstString(rows[0]["ENAME"]).Trim();

                        //if any future org change data; store it in a temp objet.
                        if (rows.Count() > 1)
                        {
                            if (orgTrack != null)
                            {
                                orgTrack.CostCenter = CheckNull.AgainstString(rows[1]["KOSTL"]).Trim();
                                orgTrack.CompanyCode = CheckNull.AgainstString(rows[1]["BUKRS"]).Trim();
                                orgTrack.DepartmentCode = CheckNull.AgainstString(rows[1]["WERKS"]).Trim();
                                orgTrack.DivisionCode = CheckNull.AgainstString(rows[1]["BTRTL"]).Trim();
                                orgTrack.SectionCode = CheckNull.AgainstString(rows[1]["ORGEH"]).Trim();
                            }
                        }
                    }
                    else
                    {
                        user.DisplayName = CheckNull.AgainstString(rows[0]["ENAME"]).Trim();

                        if (orgTrack != null)
                        {
                            orgTrack.CostCenter = CheckNull.AgainstString(rows[0]["KOSTL"]).Trim();
                            orgTrack.CompanyCode = CheckNull.AgainstString(rows[0]["BUKRS"]).Trim();
                            orgTrack.DepartmentCode = CheckNull.AgainstString(rows[0]["WERKS"]).Trim();
                            orgTrack.DivisionCode = CheckNull.AgainstString(rows[0]["BTRTL"]).Trim();
                            orgTrack.SectionCode = CheckNull.AgainstString(rows[0]["ORGEH"]).Trim();
                        }
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

                    var sectionCode = CheckNull.AgainstString(rows[0]["ORGEH"]).Trim();
                    section = sections.FirstOrDefault(item => item.Code == sectionCode);
                    if (section != null)
                    {
                        user.NOrganisationId = Convert.ToInt32(section.Id);
                    }

                    var positionCode = CheckNull.AgainstString(rows[0]["PLANS"]).Trim();
                    position = positions.FirstOrDefault(item => item.Code == positionCode);
                    if (position != null)
                    {
                        user.NBusinessRoleId = Convert.ToInt32(position.Id);
                    }

                    if (orgTrack != null)
                    {
                        section = sections.FirstOrDefault(item => item.Code == sectionCode);
                        if (section != null)
                        {
                            orgTrack.OrganisationId = section.Id;
                            orgTrack.SectionName = section.Name;
                        }
                    }

                    user.EmployeeId = Pernr;
                    user.EmployeeType = EmployeeType.Permanent;
                    user.FullTimeEmployee = true;


                    if (isNewUser)
                    {
                        oldUser = User.GetByNRIC(user.IdentificationNo, user.IdentificationType, user.IdentificationCountry);
                        isRejoinUser = oldUser != null;

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
                            user.AuthenticationMode = oldUser.AuthenticationMode;

                            userId = oldUser.Id;
                        }
                        else
                        {
                            if (!user.JoinDate.HasValue)
                            {
                                continue;
                            }

                            user.UserName = Pernr;//new logic
                            user.Password = password;

                            user.Save(MasterConfigHelper.ServiceUser);
                            userId = user.Id;

                            int RoleId = MasterConfigHelper.RoleUser;
                            User.InsertUserRole(userId, RoleId);
                        }
                    }
                    else
                    {
                        if (orgTrack == null)
                        {
                            User.UpdateUserFromDiscovery(user, propagationFields, canRunPropagationTrigger, MasterConfigHelper.ServiceUser);
                        }
                        userId = user.Id;
                    }

                    user.Id = userId;

                    this.TrackEventTriggers(user, orgTrack, isRejoinUser, oldUser, isNewUser, offBoardExists, onBoardExists);
                }
                EmployeeEventTrigger.ValidateAndCreateTriggers(triggers, MasterConfigHelper.ServiceUser);
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

                //dt = null;
                //dtFlds = null;
                //dtopts = null;
                dtTemp = null;
                dtFields = null;
            }
        }

        private int GetHRMSUsersFromGivenDate(string scanDate)
        {

            //TAB512Table dt = new TAB512Table();
            //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();

            List<string> employeeIds = new List<string>();           
            List<NOrganisation> sections = new List<NOrganisation>();
            List<NDesignation> positions = new List<NDesignation>();

            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;


            string Pernr = string.Empty;
            bool isNewUser;

            try
            {
                WriteDiscoveryLog(LogLevelType.Info, "Downloading users from the selected data : " + scanDate);
                string password = RSAHelper.EncryptData(MasterConfigHelper.DefaultPassword);
                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;
                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //proxy.Connection = this.GetSAPConnection();
                //proxy.Connection.Open();

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "AEDTM >= '" + scanDate + "' ";

                filter = "AEDTM >= '" + scanDate + "' ";
                #region  PA0000 Table

                //dtFlds.Add(FLD_PERNR);

                //dtopts.Add(filter);

                //proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0000");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");

                //filter = "AEDTM >= '" + deltaDate + "' ";
                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0000");


                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                #region PA0001 Table

                //proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0001");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");
                dtTemp = readTable.ToDataTable("PA0001");

                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                #region PA0002 Table

                //proxy.Rfc_Read_Table("|", null, "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0002");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0002");

                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                #region  PA0185 Table

                //proxy.Rfc_Read_Table("|", null, "PA0185", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0185");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");

                //filter = "ENDDA >= '" + today + "' ";
                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0185");


                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                #region PA0077 Table

                //proxy.Rfc_Read_Table("|", null, "PA0077", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();
                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0077");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");
                    
                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0077");

                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                #region PA0105 Table

                //proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0105");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);
                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0105");

                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                #region PA0027 Table

                //proxy.Rfc_Read_Table("|", null, "PA0027", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0027");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                dtFields = fieldTable.ToDataTable("FIELDS");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0027");



                m_dtPA0000AllEmp.Merge(ConvertPipesToColumns(dtFields, dtTemp));

                #endregion

                //}

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
                NDesignation position = null;

                DataRow[] rows;

                if (employeeIds.Count > 0)
                {
                    sections = NOrganisation.GetAll();
                    positions = NDesignation.GetAll();
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

                    //RFC_DB_OPT FILTER_DELTA_DATA = new RFC_DB_OPT();
                    //FILTER_DELTA_DATA.Text = "PERNR = '" + Pernr + "' AND AEDTM >= '" + scanDate + "'";

                    //RFC_DB_OPT FILTER_PERNR_ENDDA = new RFC_DB_OPT();
                    //FILTER_PERNR_ENDDA.Text = "PERNR = '" + Pernr + "' AND ENDDA >= '" + scanDate + "'";

                    string filterDeltaData = "PERNR = '" + Pernr + "' AND AEDTM >= '" + scanDate + "'";
                    string filterPernEndDA = "PERNR = '" + Pernr + "' AND ENDDA >= '" + scanDate + "'"; 


                    //using (SAPProxy1 proxy = new SAPProxy1())
                    //{
                    //    proxy.Connection = this.GetSAPConnection();
                    //    proxy.Connection.Open();

                    #region  PA0000 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_BEGDA);
                    //dtFlds.Add(FLD_ENDDA);
                    //dtFlds.Add(FLD_STAT2);
                    //dtFlds.Add(FLD_MASSN);

                    //dtopts.Add(FILTER_DELTA_DATA);

                    //proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0000");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "BEGDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "STAT2");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "MASSN");
                    dtFields = fieldTable.ToDataTable("FIELDS");

                    //filter = "PERNR = '" + Pernr + "' ";
                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterDeltaData);

                    readTableFunction.Invoke(SAPdestination);

                    readTable = readTableFunction.GetTable("DATA");

                    dtTemp = readTable.ToDataTable("PA0000");

                    m_dtPA0000 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0001 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_ENDDA);
                    //dtFlds.Add(FLD_ENAME);
                    //dtFlds.Add(FLD_KOSTL);
                    //dtFlds.Add(FLD_BUKRS);
                    //dtFlds.Add(FLD_WERKS);
                    //dtFlds.Add(FLD_BTRTL);
                    //dtFlds.Add(FLD_ORGEH);
                    //dtFlds.Add(FLD_PLANS);
                    //dtFlds.Add(FLD_PERSG);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0001");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENAME");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "KOSTL");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "BUKRS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "WERKS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "BTRTL");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ORGEH");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "PLANS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "PERSG");

                    dtFields = fieldTable.ToDataTable("FIELDS");

                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterPernEndDA);

                    readTableFunction.Invoke(SAPdestination);

                    readTable = readTableFunction.GetTable("DATA");

                    dtTemp = readTable.ToDataTable("PA0001");

                    //dtFields = dtFlds.ToADODataTable();

                    m_dtPA0001 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0002 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_PERNR);
                    //dtFlds.Add(FLD_VORNA);
                    //dtFlds.Add(FLD_TITEL);
                    //dtFlds.Add(FLD_NATIO);
                    //dtFlds.Add(FLD_NACHN);
                    //dtFlds.Add(FLD_MIDNM);
                    //dtFlds.Add(FLD_RUFNM);
                    //dtFlds.Add(FLD_ENDDA);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", " ", "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtFields = dtFlds.ToADODataTable();
                    //DataTable dtParameters = dt.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0002");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "PERNR");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "VORNA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "TITEL");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "NATIO");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "NACHN");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "MIDNM");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "RUFNM");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");

                    dtFields = fieldTable.ToDataTable("FIELDS");

                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterPernEndDA);

                    readTableFunction.Invoke(SAPdestination);

                    readTable = readTableFunction.GetTable("DATA");

                    DataTable dtParameters = readTable.ToDataTable("PA0002");

                    m_dtPA0002 = ConvertPipesToColumns(dtFields, dtParameters);

                    #endregion

                    #region  PA0185 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_ICNUM);
                    //dtFlds.Add(FLD_ENDDA);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", null, "PA0185", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0185");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ICNUM");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    dtFields = fieldTable.ToDataTable("FIELDS");

                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterPernEndDA);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtTemp = readTable.ToDataTable("PA0185");

                    m_dtPA0185 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0077 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_RACKY);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", null, "PA0077", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0185");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ICNUM");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    dtFields = fieldTable.ToDataTable("FIELDS");

                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterPernEndDA);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtTemp = readTable.ToDataTable("PA0185");
                    
                    m_dtPA0077 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0105 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_ENDDA);
                    //dtFlds.Add(FLD_USRTY);
                    //dtFlds.Add(FLD_USRID_LONG);
                    //dtFlds.Add(FLD_USRID);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0105");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "USRTY");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "USRID_LONG");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "USRID");

                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterPernEndDA);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtTemp = readTable.ToDataTable("PA0105");

                    m_dtPA0105 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    #region PA0027 Table

                    //dt = new TAB512Table();
                    //dtFlds = new RFC_DB_FLDTable();
                    //dtopts = new RFC_DB_OPTTable();

                    //dtFlds.Add(FLD_ENDDA);
                    //dtFlds.Add(FLD_KST01);
                    //dtFlds.Add(FLD_KSTAR);

                    //dtopts.Add(FILTER_PERNR_ENDDA);

                    //proxy.Rfc_Read_Table("|", null, "PA0027", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //dtTemp = dt.ToADODataTable();
                    //dtFields = dtFlds.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "PA0027");
                    readTableFunction.SetValue("DELIMITER", "|");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "ENDDA");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "KST01");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "KSTAR");


                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filterPernEndDA);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtTemp = readTable.ToDataTable("PA0027");

                    m_dtPA0027 = ConvertPipesToColumns(dtFields, dtTemp);

                    #endregion

                    //}

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

                    var sectionCode = CheckNull.AgainstString(rows[0]["ORGEH"]).Trim();
                    section = sections.FirstOrDefault(item => item.Code == sectionCode);
                    if (section != null)
                    {
                        user.NOrganisationId = Convert.ToInt32(section.Id);
                    }

                    var positionCode = CheckNull.AgainstString(rows[0]["PLANS"]).Trim();
                    position = positions.FirstOrDefault(item => item.Code == positionCode);
                    if (position != null)
                    {
                        user.NBusinessRoleId = Convert.ToInt32(position.Id);
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
                            userId = oldUser.Id;
                            user.Save(MasterConfigHelper.ServiceUser);

                        }
                        else
                        {
                            user.UserName = Pernr;//new logic
                            user.Password = password;
                            user.Save(MasterConfigHelper.ServiceUser);
                            userId = user.Id;
                            int RoleId = MasterConfigHelper.RoleUser;
                            User.InsertUserRole(userId, RoleId);
                        }
                    }
                    else
                    {
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

                //dt = null;
                //dtFlds = null;
                //dtopts = null;
                dtTemp = null;
                dtFields = null;
            }
        }

        private void GetUserByPernr(string Pernr)
        {

            //TAB512Table dt = new TAB512Table();
            //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();

            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filterPernr = "PERNR = '" + Pernr + "' ";
            string filterPernrEndDA = "PERNR = '" + Pernr + "' AND ENDDA >= '" + today + "'";



            try
            {
                WriteDiscoveryLog(LogLevelType.Info, "Getting specific user Information");

                bool isNewUser;

                string password = RSAHelper.EncryptData(MasterConfigHelper.DefaultPassword);
                
                var sections = NOrganisation.GetAll();
                var positions = NDesignation.GetAll();

                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;


                //TRUSTED SOURCE USER INFO                
                NOrganisation section = null;
                NDesignation position = null;

                DataRow[] rows;

                //RFC_DB_OPT FILTER_PERNR = new RFC_DB_OPT();
                //FILTER_PERNR.Text = "PERNR = '" + Pernr + "' ";

                //RFC_DB_OPT FILTER_PERNR_ENDDA = new RFC_DB_OPT();
                //FILTER_PERNR_ENDDA.Text = "PERNR = '" + Pernr + "' AND ENDDA >= '" + today + "'";

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                #region  PA0000 Table

                //dt = new TAB512Table();
                //dtFlds = new RFC_DB_FLDTable();
                //dtopts = new RFC_DB_OPTTable();

                //dtFlds.Add(FLD_BEGDA);
                //dtFlds.Add(FLD_ENDDA);
                //dtFlds.Add(FLD_STAT2);
                //dtFlds.Add(FLD_MASSN);

                //dtopts.Add(FILTER_PERNR);

                //proxy.Rfc_Read_Table("|", null, "PA0000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0000");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "BEGDA");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ENDDA");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "STAT2");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "MASSN");
                dtFields = fieldTable.ToDataTable("FIELDS");

                //filter = "PERNR = '" + Pernr + "' ";
                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filterPernr);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0000");



                m_dtPA0000 = ConvertPipesToColumns(dtFields, dtTemp);

                #endregion

                #region PA0001 Table

                //dt = new TAB512Table();
                //dtFlds = new RFC_DB_FLDTable();
                //dtopts = new RFC_DB_OPTTable();

                //dtFlds.Add(FLD_BEGDA);
                //dtFlds.Add(FLD_ENDDA);
                //dtFlds.Add(FLD_ENAME);
                //dtFlds.Add(FLD_KOSTL);
                //dtFlds.Add(FLD_BUKRS);
                //dtFlds.Add(FLD_WERKS);
                //dtFlds.Add(FLD_BTRTL);
                //dtFlds.Add(FLD_ORGEH);
                //dtFlds.Add(FLD_PLANS);
                //dtFlds.Add(FLD_PERSG);

                //dtopts.Add(FILTER_PERNR_ENDDA);

                //proxy.Rfc_Read_Table("|", null, "PA0001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0001");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ENDDA");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ENAME");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "KOSTL");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "BUKRS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "WERKS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "BTRTL");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ORGEH");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PLANS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERSG");

                dtFields = fieldTable.ToDataTable("FIELDS");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filterPernrEndDA);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("PA0001");

                //dtFields = dtFlds.ToADODataTable();



                m_dtPA0001 = ConvertPipesToColumns(dtFields, dtTemp);

                #endregion

                #region PA0002 Table

                //dt = new TAB512Table();
                //dtFlds = new RFC_DB_FLDTable();
                //dtopts = new RFC_DB_OPTTable();

                //dtFlds.Add(FLD_PERNR);
                //dtFlds.Add(FLD_VORNA);
                //dtFlds.Add(FLD_TITEL);
                //dtFlds.Add(FLD_NATIO);
                //dtFlds.Add(FLD_NACHN);
                //dtFlds.Add(FLD_MIDNM);
                //dtFlds.Add(FLD_RUFNM);
                //dtFlds.Add(FLD_ENDDA);

                //dtopts.Add(FILTER_PERNR);

                //proxy.Rfc_Read_Table("|", " ", "PA0002", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtFields = dtFlds.ToADODataTable();
                //DataTable dtParameters = dt.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0002");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERNR");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "VORNA");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "TITEL");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "NATIO");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "NACHN");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "MIDNM");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "RUFNM");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ENDDA");

                dtFields = fieldTable.ToDataTable("FIELDS");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filterPernr);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtParameters = readTable.ToDataTable("PA0002");

                m_dtPA0002 = ConvertPipesToColumns(dtFields, dtParameters);

                #endregion

                #region  PA0185 Table

                //dt = new TAB512Table();
                //dtFlds = new RFC_DB_FLDTable();
                //dtopts = new RFC_DB_OPTTable();

                //dtFlds.Add(FLD_ICNUM);
                //dtFlds.Add(FLD_ENDDA);

                //dtopts.Add(FILTER_PERNR);

                //proxy.Rfc_Read_Table("|", null, "PA0185", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0185");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ICNUM");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ENDDA");
                dtFields = fieldTable.ToDataTable("FIELDS");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filterPernr);

                readTableFunction.Invoke(SAPdestination);
                readTable = readTableFunction.GetTable("DATA");
                dtTemp = readTable.ToDataTable("PA0185");


                m_dtPA0185 = ConvertPipesToColumns(dtFields, dtTemp);

                #endregion

                #region PA0077 Table

                //dt = new TAB512Table();
                //dtFlds = new RFC_DB_FLDTable();
                //dtopts = new RFC_DB_OPTTable();

                //dtFlds.Add(FLD_RACKY);

                //dtopts.Add(FILTER_PERNR);

                //proxy.Rfc_Read_Table("|", null, "PA0077", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0077");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ICNUM");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ENDDA");
                dtFields = fieldTable.ToDataTable("FIELDS");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filterPernr);

                readTableFunction.Invoke(SAPdestination);
                readTable = readTableFunction.GetTable("DATA");
                dtTemp = readTable.ToDataTable("PA0077");


                m_dtPA0077 = ConvertPipesToColumns(dtFields, dtTemp);

                #endregion

                #region PA0105 Table

                //dt = new TAB512Table();
                //dtFlds = new RFC_DB_FLDTable();
                //dtopts = new RFC_DB_OPTTable();

                //dtFlds.Add(FLD_ENDDA);
                //dtFlds.Add(FLD_USRTY);
                //dtFlds.Add(FLD_USRID_LONG);
                //dtFlds.Add(FLD_USRID);

                //dtopts.Add(FILTER_PERNR_ENDDA);

                //proxy.Rfc_Read_Table("|", null, "PA0105", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();
                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0105");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ENDDA");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "USRTY");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "USRID_LONG");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "USRID");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filterPernrEndDA);

                readTableFunction.Invoke(SAPdestination);
                readTable = readTableFunction.GetTable("DATA");
                dtTemp = readTable.ToDataTable("PA0105");

                m_dtPA0105 = ConvertPipesToColumns(dtFields, dtTemp);

                #endregion

                #region PA0027 Table

                //dt = new TAB512Table();
                //dtFlds = new RFC_DB_FLDTable();
                //dtopts = new RFC_DB_OPTTable();

                //dtFlds.Add(FLD_ENDDA);
                //dtFlds.Add(FLD_KST01);
                //dtFlds.Add(FLD_KSTAR);

                //dtopts.Add(FILTER_PERNR);

                //proxy.Rfc_Read_Table("|", null, "PA0027", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //dtTemp = dt.ToADODataTable();
                //dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "PA0027");
                readTableFunction.SetValue("DELIMITER", "|");
                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ENDDA");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "KST01");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "KSTAR");


                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filterPernr);

                readTableFunction.Invoke(SAPdestination);
                readTable = readTableFunction.GetTable("DATA");
                dtTemp = readTable.ToDataTable("PA0027");

                m_dtPA0027 = ConvertPipesToColumns(dtFields, dtTemp);

                #endregion

                //}

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

                var sectionCode = CheckNull.AgainstString(rows[0]["ORGEH"]).Trim();
                section = sections.FirstOrDefault(item => item.Code == sectionCode);
                if (section != null)
                {
                    user.NOrganisationId = Convert.ToInt32(section.Id);
                }

                var positionCode = CheckNull.AgainstString(rows[0]["PLANS"]).Trim();
                position = positions.FirstOrDefault(item => item.Code == positionCode);
                if (position != null)
                {
                    user.NBusinessRoleId = Convert.ToInt32(position.Id);
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

                        userId = oldUser.Id;

                        user.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        user.UserName = Pernr;//new logic
                        user.Password = password;
                        user.EmployeeId = Pernr;
                        user.Save(MasterConfigHelper.ServiceUser);
                        userId = user.Id;
                        int RoleId = MasterConfigHelper.RoleUser;
                        User.InsertUserRole(userId, RoleId);

                    }
                }
                else
                {
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

                //dt = null;
                //dtFlds = null;
                //dtopts = null;
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

        private void PopulateCompanyCodes()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;
            DataTable dtFields;
            DataTable dtTemp;

            try
            {
                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;

                Dictionary<string, string> companies = new Dictionary<string, string>();

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_FLD FLD_BUKRS = new RFC_DB_FLD();
                //FLD_BUKRS.Fieldname = "BUKRS";

                //RFC_DB_FLD FLD_BUTXT = new RFC_DB_FLD();
                //FLD_BUTXT.Fieldname = "BUTXT";

                //dtFlds.Add(FLD_BUKRS);
                //dtFlds.Add(FLD_BUTXT);

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                filter = "SPRAS = 'E' OR SPRAS = ''";
                //dtopts.Add(filter);

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                    //proxy.Connection = this.GetSAPConnection();
                    //proxy.Connection.Open();

                    //proxy.Rfc_Read_Table("|", null, "T001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //DataTable dtTemp = dt.ToADODataTable();
                    //DataTable dtFields = dtFlds.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "T001");
                    readTableFunction.SetValue("DELIMITER","|");

                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "BUKRS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "BUTXT");

                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT",filter);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtFields = fieldTable.ToDataTable("FIELDS");
                    dtTemp = readTable.ToDataTable("T001");

                    m_dtT001 = ConvertPipesToColumns(dtFields, dtTemp);
                //}

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
                    
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Trusted Source companies populated", companies.Count.ToString()));
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} company(s) populated", companies.Count.ToString()));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate companies", ex);
            }
        }

        private void PopulateCostCenters()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;
            DataTable dtFields;
            DataTable dtTemp;

            
            try
            {
                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_FLD FLD_KOSTL = new RFC_DB_FLD();
                //FLD_KOSTL.Fieldname = "KOSTL";

                //RFC_DB_FLD FLD_BUKRS = new RFC_DB_FLD();
                //FLD_BUKRS.Fieldname = "BUKRS";

                //dtFlds.Add(FLD_KOSTL);
                //dtFlds.Add(FLD_BUKRS);

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                //string today = DateTime.Today.ToString("yyyyMMdd");
                filter = "(SPRAS = 'E' OR SPRAS = '') AND DATBI >= '" + today + "'";
                //dtopts.Add(filter);

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                    //proxy.Connection = this.GetSAPConnection();
                    //proxy.Connection.Open();

                    //proxy.Rfc_Read_Table("|", null, "CSKS", 0, 0, ref dt, ref dtFlds, ref dtopts);
                    //DataTable dtTemp = dt.ToADODataTable();
                    //DataTable dtFields = dtFlds.ToADODataTable();

                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "CSKS");
                    readTableFunction.SetValue("DELIMITER", "|");

                    fieldTable = readTableFunction.GetTable("FIELDS");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "KOSTL");
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", "BUKRS");

                    optionsTable = readTableFunction.GetTable("OPTIONS");
                    optionsTable.Append();
                    optionsTable.SetValue("TEXT", filter);

                    readTableFunction.Invoke(SAPdestination);
                    readTable = readTableFunction.GetTable("DATA");
                    dtFields = fieldTable.ToDataTable("FIELDS");
                    dtTemp = readTable.ToDataTable("CSKS");

                    m_dtCSKS = ConvertPipesToColumns(dtFields, dtTemp);
                //}

                foreach (DataRow row in m_dtCSKS.Rows)
                {
                    string costCenterCode = row["KOSTL"].ToString().Trim();
                    string companyCode = row["BUKRS"].ToString().Trim();

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

            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;
            DataTable dtFields;
            DataTable dtTemp;

            try
            {
                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;
                
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_FLD FLD_KOSTL = new RFC_DB_FLD();
                //FLD_KOSTL.Fieldname = "KOSTL";

                //RFC_DB_FLD FLD_KOKRS = new RFC_DB_FLD();
                //FLD_KOKRS.Fieldname = "KOKRS";

                //RFC_DB_FLD FLD_LTEXT = new RFC_DB_FLD();
                //FLD_LTEXT.Fieldname = "LTEXT";

                //dtFlds.Add(FLD_KOSTL);
                //dtFlds.Add(FLD_KOKRS);
                //dtFlds.Add(FLD_LTEXT);

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                filter = "(SPRAS = 'E' OR SPRAS = '') AND DATBI >= '" + today + "'";
                //dtopts.Add(filter);

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                //    proxy.Rfc_Read_Table("|", null, "CSKT", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //    DataTable dtTemp = dt.ToADODataTable();
                //    DataTable dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "CSKT");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "KOSTL");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "KOKRS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "LTEXT");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);
                readTable = readTableFunction.GetTable("DATA");
                dtFields = fieldTable.ToDataTable("FIELDS");
                dtTemp = readTable.ToDataTable("CSKT");

                m_dtCSKT = ConvertPipesToColumns(dtFields, dtTemp);
                //}

                foreach (DataRow row in m_dtCSKT.Rows)
                {
                    string costCenterCode = row["KOSTL"].ToString().Trim();
                    string companyCode = row["KOKRS"].ToString().Trim();
                    string costCenterText = row["LTEXT"].ToString().Trim();
                    
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate cost centers", ex);
            }
        }

        private void PopulatePersonnelAreas()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable readTable;
            DataTable dtFields;
            DataTable dtTemp;

            try
            {
                Dictionary<string, string> personnelAreas = new Dictionary<string, string>();
                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;


                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_FLD FLD_PERSA = new RFC_DB_FLD();
                //FLD_PERSA.Fieldname = "PERSA";

                //RFC_DB_FLD FLD_NAME1 = new RFC_DB_FLD();
                //FLD_NAME1.Fieldname = "NAME1";

                //dtFlds.Add(FLD_PERSA);
                //dtFlds.Add(FLD_NAME1);

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                //    proxy.Rfc_Read_Table("|", null, "T500P", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //    DataTable dtTemp = dt.ToADODataTable();
                //    DataTable dtFields = dtFlds.ToADODataTable();
                
                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "T500P");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "PERSA");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "NAME1");

                readTableFunction.Invoke(SAPdestination);
                readTable = readTableFunction.GetTable("DATA");
                dtFields = fieldTable.ToDataTable("FIELDS");
                dtTemp = readTable.ToDataTable("T500P");
                
   
                m_dtT500P = ConvertPipesToColumns(dtFields, dtTemp);
                //}

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
                    
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Trusted Source Personnel Areas populated", personnelAreas.Count.ToString()));
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} Personnel Area(s) populated", personnelAreas.Count.ToString()));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate personnel areas", ex);
            }
        }

        private void PopulatePersonnelSubAreas()
        {

            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable readTable;
            DataTable dtFields;
            DataTable dtTemp;

            try
            {                
                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_FLD FLD_WERKS = new RFC_DB_FLD();
                //FLD_WERKS.Fieldname = "WERKS";

                //RFC_DB_FLD FLD_BTRTL = new RFC_DB_FLD();
                //FLD_BTRTL.Fieldname = "BTRTL";

                //RFC_DB_FLD FLD_BTEXT = new RFC_DB_FLD();
                //FLD_BTEXT.Fieldname = "BTEXT";

                //dtFlds.Add(FLD_WERKS);
                //dtFlds.Add(FLD_BTRTL);
                //dtFlds.Add(FLD_BTEXT);

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                //    proxy.Rfc_Read_Table("|", null, "T001P", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //    DataTable dtTemp = dt.ToADODataTable();
                //    DataTable dtFields = dtFlds.ToADODataTable();
                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "T001P");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "WERKS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "BTRTL");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "BTEXT");

                readTableFunction.Invoke(SAPdestination);
                readTable = readTableFunction.GetTable("DATA");
                dtFields = fieldTable.ToDataTable("FIELDS");
                dtTemp = readTable.ToDataTable("T001P");


                m_dtT001P = ConvertPipesToColumns(dtFields, dtTemp);
                //}

                foreach (DataRow row in m_dtT001P.Rows)
                {
                    string code = row["BTRTL"].ToString().Trim();
                    string text = row["BTEXT"].ToString().Trim();
                    string deptCode = row["WERKS"].ToString().Trim();

                    //replace '-' symbol with '---' to avoid UI errors.
                    text = text.Equals("-") ? "---" : text;

                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Trusted Source Personnel Sub Areas populated"));
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Personnel Sub Area(s) populated"));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate personnel sub areas", ex);
            }
        }

        private void PopulateOrganizations()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            DataTable dtFields;
            DataTable dtTemp;
            string filter; 

            try
            {
                Dictionary<string, string> orgUnits = new Dictionary<string, string>();
                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;


                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_FLD FLD_ORGEH = new RFC_DB_FLD();
                //FLD_ORGEH.Fieldname = "OBJID";

                //RFC_DB_FLD FLD_ORGTX = new RFC_DB_FLD();
                //FLD_ORGTX.Fieldname = "STEXT";

                //RFC_DB_FLD FLD_SHORT = new RFC_DB_FLD();
                //FLD_SHORT.Fieldname = "SHORT";

                //dtFlds.Add(FLD_ORGEH);
                //dtFlds.Add(FLD_ORGTX);
                //dtFlds.Add(FLD_SHORT);

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "(LANGU = 'E' OR LANGU = '') AND OTYPE = 'O' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
                filter = "(LANGU = 'E' OR LANGU = '') AND OTYPE = 'O' AND ENDDA >= '" + today + "' ";

                //dtopts.Add(filter);

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                //    proxy.Rfc_Read_Table("|", null, "HRP1000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //    DataTable dtTemp = dt.ToADODataTable();
                //    DataTable dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "HRP1000");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "OBJID");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "STEXT");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "SHORT");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);


                readTableFunction.Invoke(SAPdestination);
                readTable = readTableFunction.GetTable("DATA");
                dtFields = fieldTable.ToDataTable("FIELDS");
                dtTemp = readTable.ToDataTable("HRP1000");


                m_dtHRP1000 = ConvertPipesToColumns(dtFields, dtTemp);
                //}

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
                //NOrganisation.FlushHeadInformation();
                this.PopulateRedHats();
                this.PopulateParentOrganisation();
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error Occured in populate organisation unit heads", ex);
            }
        }

        private void PopulateRedHats()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            DataTable dtFields;
            DataTable dtTemp;
            string filter; 

            try
            {
                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();
                //m_dtHRP1001 = new DataTable();

                //RFC_DB_FLD FLD_SOBID = new RFC_DB_FLD();
                //FLD_SOBID.Fieldname = "SOBID";

                //RFC_DB_FLD FLD_OBJID = new RFC_DB_FLD();
                //FLD_OBJID.Fieldname = "OBJID";

                //dtFlds.Add(FLD_OBJID);
                //dtFlds.Add(FLD_SOBID);

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "RSIGN = 'B' AND RELAT = '012' AND OTYPE = 'O' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
                filter = "RSIGN = 'B' AND RELAT = '012' AND OTYPE = 'O' AND ENDDA >= '" + today + "' ";

                //dtopts.Add(filter);

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                //    proxy.Rfc_Read_Table("|", null, "HRP1001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //    DataTable dtTemp = dt.ToADODataTable();
                //    DataTable dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "HRP1001");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "OBJID");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "STEXT");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "SHORT");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);


                readTableFunction.Invoke(SAPdestination);
                readTable = readTableFunction.GetTable("DATA");
                dtFields = fieldTable.ToDataTable("FIELDS");
                dtTemp = readTable.ToDataTable("HRP1001");

                m_dtHRP1001 = ConvertPipesToColumns(dtFields, dtTemp);
                //}

                DataTable dtPositions = this.GetPernrAndPositionMappings();

                if (dtPositions != null && dtPositions.Rows.Count != 0)
                {
                    foreach (DataRow row in m_dtHRP1001.Rows)
                    {
                        string positionCode = row["SOBID"].ToString().Trim();
                        string sectionCode = row["OBJID"].ToString().Trim();

                        var section = NOrganisation.GetByOrgCode(sectionCode);

                        if (section != null)
                        {
                            string pernr = this.GetEmployeeIdByPosition(positionCode, dtPositions);

                            if (!string.IsNullOrEmpty(pernr))
                            {
                                var user = User.GetByEmployeeId(pernr);

                                if (user != null)
                                {
                                    //section.Head = Convert.ToInt32(user.Id);
                                }
                                user.Save(MasterConfigHelper.ServiceUser);
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

        private DataTable GetPernrAndPositionMappings()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            DataTable dtFields;
            DataTable dtTemp;
            string filter; 

            string today = DateTime.Today.ToString("yyyyMMdd");
            try
            {
                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();
                //DataTable m_dtHRP1001 = new DataTable();

                //RFC_DB_FLD FLD_SOBID = new RFC_DB_FLD();
                //FLD_SOBID.Fieldname = "SOBID";

                //RFC_DB_FLD FLD_OBJID = new RFC_DB_FLD();
                //FLD_OBJID.Fieldname = "OBJID";

                //dtFlds.Add(FLD_OBJID);
                //dtFlds.Add(FLD_SOBID);

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                filter = string.Format("RSIGN = 'A' AND RELAT = '008' AND OTYPE = 'S' AND ENDDA >= '{0}'", today);

                //dtopts.Add(filter);

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                //    proxy.Rfc_Read_Table("|", null, "HRP1001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //    DataTable dtTemp = dt.ToADODataTable();
                //    DataTable dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "HRP1001");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "OBJID");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "STEXT");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "SHORT");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);


                readTableFunction.Invoke(SAPdestination);
                readTable = readTableFunction.GetTable("DATA");
                dtFields = fieldTable.ToDataTable("FIELDS");
                dtTemp = readTable.ToDataTable("HRP1001");

                m_dtHRP1001 = ConvertPipesToColumns(dtFields, dtTemp);


                //}

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
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            DataTable dtFields;
            DataTable dtTemp;
            string filter; 

            try
            {

                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();
                //m_dtHRP1001 = new DataTable();

                //RFC_DB_FLD FLD_SOBID = new RFC_DB_FLD();
                //FLD_SOBID.Fieldname = "SOBID";

                //RFC_DB_FLD FLD_OBJID = new RFC_DB_FLD();
                //FLD_OBJID.Fieldname = "OBJID";

                //dtFlds.Add(FLD_OBJID);
                //dtFlds.Add(FLD_SOBID);

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "RSIGN = 'A' AND SCLAS = 'O' AND OTYPE = 'O' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
                filter = "RSIGN = 'A' AND SCLAS = 'O' AND OTYPE = 'O' AND ENDDA >= '" + today + "' ";

                //dtopts.Add(filter);

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                //    proxy.Rfc_Read_Table("|", null, "HRP1001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //    DataTable dtTemp = dt.ToADODataTable();
                //    DataTable dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "HRP1001");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "OBJID");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "STEXT");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "SHORT");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);


                readTableFunction.Invoke(SAPdestination);
                readTable = readTableFunction.GetTable("DATA");
                dtFields = fieldTable.ToDataTable("FIELDS");
                dtTemp = readTable.ToDataTable("HRP1001");

                m_dtHRP1001 = ConvertPipesToColumns(dtFields, dtTemp);
                //}

                foreach (DataRow row in m_dtHRP1001.Rows)
                {
                    string parentSectionCode = row["SOBID"].ToString().Trim();
                    string sectionCode = row["OBJID"].ToString().Trim();

                    var section = NOrganisation.GetByOrgCode(sectionCode);

                    if (section != null)
                    {
                        section.ParentOrgCode = parentSectionCode;
                        section.Save(MasterConfigHelper.ServiceUser);
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
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;
            DataTable dtFields;
            DataTable dtTemp;

            try
            {
                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;

                Dictionary<string, string> positions = new Dictionary<string, string>();

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_FLD FLD_PLANS = new RFC_DB_FLD();
                //FLD_PLANS.Fieldname = "OBJID";

                //RFC_DB_FLD FLD_STEXT = new RFC_DB_FLD();
                //FLD_STEXT.Fieldname = "STEXT";

                //dtFlds.Add(FLD_PLANS);
                //dtFlds.Add(FLD_STEXT);

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "(LANGU = 'E' OR LANGU = '') AND OTYPE = 'S' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
                filter = "(LANGU = 'E' OR LANGU = '') AND OTYPE = 'S' AND ENDDA >= '" + today + "' ";

                //dtopts.Add(filter);

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                //    proxy.Rfc_Read_Table("|", null, "HRP1000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //    DataTable dtTemp = dt.ToADODataTable();
                //    DataTable dtFields = dtFlds.ToADODataTable();
                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "HRP1000");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "OBJID");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "STEXT");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);


                readTableFunction.Invoke(SAPdestination);
                readTable = readTableFunction.GetTable("DATA");
                dtFields = fieldTable.ToDataTable("FIELDS");
                dtTemp = readTable.ToDataTable("HRP1000");


                m_dtHRP1000 = ConvertPipesToColumns(dtFields, dtTemp);
                //}

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
                    var pos = NDesignation.GetByCode(position.Key);
                    if (pos == null)
                    {
                        NDesignation dsg = new NDesignation()
                        {
                            Code = position.Key,
                            Description = position.Value,
                            IsActive = true,
                        };
                        dsg.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        pos.Description = position.Value;
                        pos.Save(MasterConfigHelper.ServiceUser);
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
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;
            DataTable dtFields;
            DataTable dtTemp;

            try
            {
                SAPdestination = GetSAPHRMSDestination();
                bapiRepository = SAPdestination.Repository;

                Dictionary<string, string> positions = new Dictionary<string, string>();

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_FLD FLD_PLANS = new RFC_DB_FLD();
                //FLD_PLANS.Fieldname = "OBJID";

                //RFC_DB_FLD FLD_STEXT = new RFC_DB_FLD();
                //FLD_STEXT.Fieldname = "STEXT";

                //dtFlds.Add(FLD_PLANS);
                //dtFlds.Add(FLD_STEXT);

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "(LANGU = 'E' OR LANGU = '') AND OTYPE = 'S' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
                filter = "(LANGU = 'E' OR LANGU = '') AND OTYPE = 'S' AND AEDTM >= '" + deltaDate + "' ";

                //dtopts.Add(filter);

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                //    proxy.Rfc_Read_Table("|", null, "HRP1000", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //    DataTable dtTemp = dt.ToADODataTable();
                //    DataTable dtFields = dtFlds.ToADODataTable();

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "HRP1000");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "OBJID");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "STEXT");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);


                readTableFunction.Invoke(SAPdestination);
                readTable = readTableFunction.GetTable("DATA");
                dtFields = fieldTable.ToDataTable("FIELDS");
                dtTemp = readTable.ToDataTable("HRP1000");
                
                m_dtHRP1000 = ConvertPipesToColumns(dtFields, dtTemp);
                //}

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
                    var pos = NDesignation.GetByCode(position.Key);
                    if (pos == null)
                    {
                        NDesignation dsg = new NDesignation()
                        {
                            Code = position.Key,
                            Description = position.Value,
                            IsActive = true,
                        };
                        dsg.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        pos.Description = position.Value;
                        pos.Save(MasterConfigHelper.ServiceUser);
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

        private void TrackEventTriggers(User user, PresentationOrgChangeTrack orgTrack, bool isRejoiningUser, User oldUser, bool isNewUser, bool offBoardExists, bool onBoardExists)
        {
            //trigger on board event if the join date is greater than equal to today during first time discovery
            if (user.JoinDate.HasValue)
            {
                if (isRejoiningUser)
                {
                    var triggerRehire = this.PopulateRehireTrigger(user, oldUser, isNewUser, onBoardExists);

                    if (triggerRehire != null && triggerRehire.Id == 0)
                    {
                        triggers.Add(triggerRehire);
                    }
                }
                else
                {
                    var trigger = this.PopulateOnBoardTrigger(user, oldUser, isNewUser, onBoardExists);

                    if (trigger != null && trigger.Id == 0)
                    {
                        triggers.Add(trigger);
                    }
                }                
            }

            //trigger off board event if the termination date is greater than equal to today during first time discovery
            if (offBoardExists)
            {
                if (user.TerminationDate.Value.Date != Constants.SAP_MAX_DATE)
                {
                    var trigger = this.PopulateOffBoardTrigger(user);

                    if (trigger != null && trigger.Id == 0)
                    {
                        triggers.Add(trigger);
                    }
                }
            }

            //trigger ou change/re-org event
            if (orgTrack != null)
            {
                var trigger = this.PopulateOUChangeTrigger(user, orgTrack);

                if (trigger != null && trigger.Id == 0)
                {
                    triggers.Add(trigger);
                }
            }

        }

        private EmployeeEventTrigger PopulateOnBoardTrigger(User user, User oldUser, bool isNewUser, bool onBoardExists)
        {
            var evnt = EmployeeEvents.NewJoinee;
            var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, evnt, null);
            double onBoardDaysBeforeJoining = MasterConfigHelper.OnBoardDaysBeforeJoining;
            DateTime priorBoardingDate = user.JoinDate.Value.AddDays(-(onBoardDaysBeforeJoining));

            if (trigger != null)
            {
                if (!onBoardExists)
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

                trigger.UserDetails = string.Empty;
                trigger.Save(MasterConfigHelper.ServiceUser);
                return trigger;
            }
            else
            {
                if (!isNewUser)
                {
                    return null;
                }

                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true,
                    UserDetails = string.Empty,
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

        private EmployeeEventTrigger PopulateRehireTrigger(User user, User oldUser, bool isNewUser, bool onBoardExists)
        {
            var evnt = EmployeeEvents.NewJoinee;
            var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, evnt, null);
            double onBoardDaysBeforeJoining = MasterConfigHelper.OnBoardDaysBeforeJoining;
            DateTime priorBoardingDate = user.JoinDate.Value.AddDays(-(onBoardDaysBeforeJoining));

            if (trigger != null)
            {
                if (priorBoardingDate >= DateTime.Today)
                {
                    trigger.EffectiveDate = priorBoardingDate;
                }
                else
                {
                    trigger.EffectiveDate = DateTime.Today;
                }

                trigger.UserDetails = UtilityMethods.Serialize<User>(user);
                trigger.Save(MasterConfigHelper.ServiceUser);
                return trigger;
            }
            else
            {
                if (!isNewUser)
                {
                    return null;
                }

                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true,
                    UserDetails = UtilityMethods.Serialize<User>(user),
                };

                if (priorBoardingDate >= DateTime.Today)
                {
                    newTrigger.EffectiveDate = priorBoardingDate;
                }
                else
                {
                    newTrigger.EffectiveDate = DateTime.Today;
                }

                //INCASE OF REJOINING DELIMIT THE EMAILID AND FLINS LOGIN ID FOR OLD PERNR
                DateTime delimitDate = oldUser.TerminationDate.HasValue ? oldUser.TerminationDate.Value.AddDays(-1) : user.JoinDate.Value.AddDays(-1);
                this.DelimitEmailIdInHR(oldUser.EmployeeId, delimitDate);
                this.DelimitFLINSLogonIdInHR(oldUser.EmployeeId, delimitDate);

                return newTrigger;
            }
        }

        private EmployeeEventTrigger PopulateOffBoardTrigger(User user)
        {
            var evnt = EmployeeEvents.Resignation;
            var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, evnt, null);

            if (trigger != null)
            {

                trigger.EffectiveDate = user.TerminationDate.Value;
                trigger.Save(MasterConfigHelper.ServiceUser);
                return trigger;
            }
            else
            {
                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    EffectiveDate = user.TerminationDate.Value,
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true
                };
                return newTrigger;
            }
        }

        private EmployeeEventTrigger PopulateOUChangeTrigger(User user, PresentationOrgChangeTrack orgTrack)
        {
            var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, orgTrack.Event, null);

            if (trigger != null)
            {
                var tempTrigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, orgTrack.Event, this.Process.LastRunOn.Value.Date);
                if (tempTrigger != null && trigger.IsRequestCreated)
                {
                    return null;
                }

                tempTrigger.EffectiveDate = orgTrack.EffectiveDate;
                tempTrigger.OldUserDetails = UtilityMethods.Serialize<User>(user);
                this.FillUserOrgFromOrgTrack(user, orgTrack);
                tempTrigger.UserDetails = UtilityMethods.Serialize<User>(user);
                tempTrigger.Save(MasterConfigHelper.ServiceUser);
                return tempTrigger;
            }
            else
            {
                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    EffectiveDate = orgTrack.EffectiveDate,
                    EventType = orgTrack.Event,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true
                };

                newTrigger.OldUserDetails = UtilityMethods.Serialize<User>(user);
                this.FillUserOrgFromOrgTrack(user, orgTrack);
                newTrigger.UserDetails = UtilityMethods.Serialize<User>(user);

                return newTrigger;
            }
        }

        private void FillUserOrgFromOrgTrack(User user, PresentationOrgChangeTrack orgTrack)
        {
            user.NOrganisationId = Convert.ToInt32(orgTrack.OrganisationId);            
        }

    }

    //public static class IRfcTableExtentions
    //{
    //    /// <summary>
    //    /// Converts SAP table to .NET DataTable table
    //    /// </summary>
    //    /// <param name="sapTable">The SAP table to convert.</param>
    //    /// <returns></returns>
    //    public static DataTable ToDataTable(this IRfcTable sapTable, string name)
    //    {
    //        DataTable adoTable = new DataTable(name);
    //        //... Create ADO.Net table.
    //        for (int liElement = 0; liElement < sapTable.ElementCount; liElement++)
    //        {
    //            RfcElementMetadata metadata = sapTable.GetElementMetadata(liElement);
    //            adoTable.Columns.Add(metadata.Name, GetDataType(metadata.DataType));
    //        }

    //        //Transfer rows from SAP Table ADO.Net table.
    //        foreach (IRfcStructure row in sapTable)
    //        {
    //            DataRow ldr = adoTable.NewRow();
    //            for (int liElement = 0; liElement < sapTable.ElementCount; liElement++)
    //            {
    //                RfcElementMetadata metadata = sapTable.GetElementMetadata(liElement);

    //                switch (metadata.DataType)
    //                {
    //                    case RfcDataType.DATE:
    //                        ldr[metadata.Name] = row.GetString(metadata.Name).Substring(0, 4) + row.GetString(metadata.Name).Substring(5, 2) + row.GetString(metadata.Name).Substring(8, 2);
    //                        break;
    //                    case RfcDataType.BCD:
    //                        ldr[metadata.Name] = row.GetDecimal(metadata.Name);
    //                        break;
    //                    case RfcDataType.CHAR:
    //                        ldr[metadata.Name] = row.GetString(metadata.Name);
    //                        break;
    //                    case RfcDataType.STRING:
    //                        ldr[metadata.Name] = row.GetString(metadata.Name);
    //                        break;
    //                    case RfcDataType.INT2:
    //                        ldr[metadata.Name] = row.GetInt(metadata.Name);
    //                        break;
    //                    case RfcDataType.INT4:
    //                        ldr[metadata.Name] = row.GetInt(metadata.Name);
    //                        break;
    //                    case RfcDataType.FLOAT:
    //                        ldr[metadata.Name] = row.GetDouble(metadata.Name);
    //                        break;
    //                    default:
    //                        ldr[metadata.Name] = row.GetString(metadata.Name);
    //                        break;
    //                }
    //            }
    //            adoTable.Rows.Add(ldr);
    //        }
    //        return adoTable;
    //    }

    //    private static Type GetDataType(RfcDataType rfcDataType)
    //    {
    //        switch (rfcDataType)
    //        {
    //            case RfcDataType.DATE:
    //                return typeof(string);
    //            case RfcDataType.CHAR:
    //                return typeof(string);
    //            case RfcDataType.STRING:
    //                return typeof(string);
    //            case RfcDataType.BCD:
    //                return typeof(decimal);
    //            case RfcDataType.INT2:
    //                return typeof(int);
    //            case RfcDataType.INT4:
    //                return typeof(int);
    //            case RfcDataType.FLOAT:
    //                return typeof(double);
    //            default:
    //                return typeof(string);
    //        }
    //    }
    //}

}
