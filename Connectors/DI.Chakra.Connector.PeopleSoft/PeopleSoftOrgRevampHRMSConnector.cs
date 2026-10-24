using DI.Chakra.DBConfigHelper;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckNulls = DI.Chakra.DomainModel.CheckNull;

namespace DI.Chakra.Connector
{
    public class PeopleSoftOrgRevampHRMSConnector : ConnectorBase
    {
        EndpointConfiguration PeoplesoftConfiguration { get; set; }
        public PeopleSoftOrgRevampHRMSConnector(EndPoint _endpoint, AccessRequest currentRequest = null)
        {
            this.PeoplesoftConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            this.EndPoint = _endpoint;
            this.CurrentRequest = currentRequest;
        }
        private string Configuration(EndpointKey Key)
        {
            var config = PeoplesoftConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.EndpointConfigurationKey.KeyName == Key.ToString());
            return config != null ? config.Value : "";
        }

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }

        public override bool Connect()
        {
            OracleConnection OrCon = null;
            try
            {

                string PSConStr = "";
                string portNo = "";
                string InstanceName = "";
                string Password = Infrastructure.RSAHelper.DecryptData(Configuration(EndpointKey.Password));
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

                OrCon = new OracleConnection(PSConStr);
                OrCon.Open();
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Endpoint System {0} connection opened", Configuration(EndpointKey.HostName)));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Endpoint System {0} connection failed", Configuration(EndpointKey.HostName)), ex);
                return false;
            }
            finally
            {
                OrCon.Close();
                OrCon.Dispose();
            }
            return true;
        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override void GetData()
        {
            try
            {
                PopulateCompany();

                PopulateBusinessUnits();

                PopulateLocations();

                PopulateDepartments();

                PopulateDesignations();

                PopulateGrades();

                GetHRMSUsers();

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception Occured at GetData Method. Error Message is {0}", ex.Message), ex);
                throw ex;
            }
        }
        private DataSet getDSFromOracleQuery(string strQry)
        {
            OracleConnection OrCon = null;
            OracleCommand OrCom = null;
            OracleDataAdapter OrAdapter;
            DataSet dsResults = new DataSet();
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
                OrCom.CommandType = CommandType.Text;
                OrCom.CommandText = strQry;
                OrAdapter = new OracleDataAdapter(OrCom.CommandText, OrCom.Connection);
                OrAdapter.Fill(dsResults);

            }
            catch (Exception ex)
            {
                if (OrCon.State == ConnectionState.Open)
                {
                    OrCon.Close();
                    OrCon.Dispose();
                }
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception Occured at getDSFromOracleQuery Method. Error Message is {0}", ex.Message), ex);
                WriteDiscoveryLog(LogLevelType.Exception, "Query which thrown exception is " + strQry);
            }

            return dsResults;
        }
        private void PopulateDesignations()
        {
            try
            {
                string designationQuery = @"select SETID,Jobcode,Descr as Description from PS_JOBCODE_TBL where Eff_Status = 'A'";
                DataSet dsDesignation = getDSFromOracleQuery(designationQuery);
                if (dsDesignation.Tables.Count > 0)
                {
                    foreach (DataRow item in dsDesignation.Tables[0].Rows)
                    {
                        try
                        {
                            string JobCode = CheckNulls.AgainstString(item["Jobcode"]);
                            string Designation = CheckNulls.AgainstString(item["Description"]);
                           // string SetId = CheckNulls.AgainstString(item["SETID"]);
                          //  JobCode = SetId + "-" + JobCode;
                            var nDesignation = NHRMSDesignation.GetByCode(JobCode,this.EndPoint.Id);
                            if (nDesignation != null)
                            {
                                nDesignation.Description = Designation;
                                nDesignation.LastModifiedBy = MasterConfigHelper.ServiceUser;
                                nDesignation.LastModifiedDate = DateTime.Now;
                                nDesignation.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                NHRMSDesignation dsg = new NHRMSDesignation()
                                {
                                    Code = JobCode,
                                    Description = Designation,
                                    IsActive = true,
                                    CreatedBy = Constants.CreatedBy,
                                    CreatedDate = DateTime.Now,
                                    LastModifiedBy = Constants.CreatedBy,
                                    LastModifiedDate = DateTime.Now,
                                    EndpointSystemId = this.EndPoint.Id
                                };
                                dsg.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception Occured while reading Designation. Error Message is {1}", ex.Message), ex);
                        }
                    }
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Peoplesoft HRMS Designation Information Populated", dsDesignation.Tables[0].Rows.Count.ToString()));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<User> GetHRMSUsers()
        {

            List<User> listPSUsers = new List<User>();

            try
            {

                string UserSQL = @"SELECT J.EMPLID EMPLID, J.EMPL_RCD, J.EFFDT, J.DEPTID DEPTID, J.JOBCODE, J.POSITION_NBR, J.SuperVisor_ID SUPERVISOR_ID, J.JOB_ENTRY_DT, 
                                               J.HIRE_DT HIRE_DT, J.BUSINESS_UNIT, J.SETID_DEPT, J.SETID_JOBCODE,  J.TERMINATION_DT TERMINATION_DT, J.ACTION, 
                                               J.FULL_PART_TIME FULL_PART_TIME, J.COMPANY COMPANY, C.DESCR DEPTNAME,
                                               B.NAME NAME, B.FIRST_NAME FIRST_NAME, B.LAST_NAME LAST_NAME, B.MIDDLE_NAME MIDDLE_NAME, B.NAME_AC,B.NAME_Display NAME_Display,
                                               B.Address1 ADDRESS1, B.ADDRESS2 ADDRESS2, B.ADDRESS3 ADDRESS3, B.ADDRESS4 ADDRESS4, B.CITY, B.NUM1 NUM1 ,B.BIRTHDATE BIRTHDATE,
                                               B.NUM2 NUM2, B.COUNTY COUNTRY, B.STATE, B.POSTAL POSTAL, B.PHONE HOME_PHONE, B.SEX,J.REPORTS_TO,J.Location,J.Grade,
                                               B.NAME_INITIALS NAME_INITIALS, B.NAME_PREFIX NAME_PREFIX, B.NAME_SUFFIX NAME_SUFFIX FROM PS_JOB J
                              INNER JOIN PS_PERSONAL_DATA B on B.emplID = J.EmPLID
                              Inner JOIN ps_dept_tbl C on c.SETID = J.SETID_DEPT and C.DEPTID = J.DEPTID
                              and C.ROWID in (select MAX(ROWID) from ps_dept_tbl group by SETID, DEPTID )
                            WHERE J.HR_STATUS in ('A','I') and EMPL_STATUS in ('A','T') 
                             {0}
                            AND J.EFFDT = (SELECT MAX(J1.EFFDT) FROM PS_JOB J1 WHERE J1.EMPLID =J.EMPLID 
                            AND J1.EMPL_RCD = J.EMPL_RCD) 
                            AND J.EMPL_RCD = ( SELECT MAX(J2.EMPL_RCD) FROM PS_JOB J2 WHERE J2.EMPLID = J.EMPLID 
                            AND J2.EFFDT = J.EFFDT ) AND J.EFFSEQ = ( SELECT MAX(J3.EFFSEQ) FROM PS_JOB J3 WHERE J3.EMPLID = J.EMPLID AND J3.EMPL_RCD = J.EMPL_RCD AND J3.EFFDT = J.EFFDT)
                            ORDER BY J.EMPLID , J.EMPL_RCD, J.EFFDT, J.EFFSEQ ";

                List<NHRMSUserEvent> userEvent = null;
                if (this.IsFirstTimeDiscovery)
                {
                    UserSQL = string.Format(UserSQL, string.Empty);
                }
                else
                {
                    var deltaDate = this.Process.LastRunOn.Value.ToString("yyyy-MM-dd HH:MM:ss");
                    UserSQL = string.Format(UserSQL, string.Format(@"and (TO_CHAR(J.LASTUPDDTTM,'yyyy-mm-dd hh24:mi:ss') >= '{0}' OR (TO_CHAR(B.LASTUPDDTTM, 'yyyy-mm-dd hh24:mi:ss') >= '{0}'))", deltaDate));
                    userEvent = new List<NHRMSUserEvent>();
                }

                DataSet dsUserinfo = getDSFromOracleQuery(UserSQL);

                if (dsUserinfo.Tables.Count <= 0)
                    return null;

                foreach (DataRow item in dsUserinfo.Tables[0].Rows)
                {
                    string EmployeeID = CheckNulls.AgainstString(item["EMPLID"]);
                    try
                    {
                        NHRMSUser PSUser = NHRMSUser.GetByEmployeeId(EmployeeID, this.EndPoint.Id) ?? new NHRMSUser();
                        string OldDepartMent = PSUser.OrgUnitCode;
                        string PositionCode = PSUser.PositionCode;
                        string OldGrade = PSUser.Grade;
                        // User PSUser = new User();
                        PSUser.IdentificationNo = EmployeeID;
                        PSUser.FirstName = CheckNulls.AgainstString(item["FIRST_NAME"]);
                        PSUser.EmployeeId = CheckNulls.AgainstString(item["EMPLID"]);
                        PSUser.MiddleName = CheckNulls.AgainstString(item["MIDDLE_NAME"]);
                        PSUser.LastName = CheckNulls.AgainstString(item["LAST_NAME"]);
                        PSUser.PrefferedName = CheckNulls.AgainstString(item["NAME"]);
                        PSUser.DisplayName = CheckNulls.AgainstString(item["NAME_Display"]);
                        PSUser.KnownAs = CheckNulls.AgainstString(item["NAME_Display"]);
                        PSUser.FullTimeEmployee = "F" == CheckNulls.AgainstString(item["FULL_PART_TIME"]).ToUpper().Trim() ? true : false;
                        PSUser.EmployeeType = (bool)PSUser.FullTimeEmployee ? EmployeeType.Permanent : EmployeeType.Contractor;
                        PSUser.HomePhoneNumber = CheckNulls.AgainstString(item["HOME_PHONE"]);
                        PSUser.TelephoneNumber = CheckNulls.AgainstString(item["NUM1"]);
                        PSUser.MobileNumber = CheckNulls.AgainstString(item["NUM2"]);
                        PSUser.PostalAddress = CheckNulls.AgainstString(item["ADDRESS1"]) + ", " + CheckNulls.AgainstString(item["ADDRESS2"]) + ", " +
                                                CheckNulls.AgainstString(item["ADDRESS3"]) + ", " + CheckNulls.AgainstString(item["ADDRESS4"]);

                        string Address = string.Empty;
                        if (CheckNulls.AgainstString(item["ADDRESS1"]) != "")
                        {
                            Address = item["ADDRESS1"].ToString();
                        }
                        if (CheckNulls.AgainstString(item["ADDRESS2"]) != "")
                        {
                            Address = Address + "," + item["ADDRESS2"].ToString();
                        }
                        if (CheckNulls.AgainstString(item["ADDRESS3"]) != "")
                        {
                            Address = Address + "," + item["ADDRESS3"].ToString();
                        }
                        if (CheckNulls.AgainstString(item["ADDRESS4"]) != "")
                        {
                            Address = Address + "," + item["ADDRESS4"].ToString();
                        }
                        PSUser.PostalAddress = Address;
                        PSUser.JoinDate = CheckNulls.AgainstNullableDatetime(item["HIRE_DT"]);
                        PSUser.Ethinicity = CheckNulls.AgainstString(item["COUNTRY"]);
                        PSUser.TerminationDate = CheckNulls.AgainstNullableDatetime(item["TERMINATION_DT"]);
                        PSUser.IsActive = (PSUser.TerminationDate.HasValue && PSUser.TerminationDate.Value.Date < DateTime.Now.Date) ? false : true;
                        string DepartmentCode = CheckNull.AgainstString(item["DEPTID"]);
                        string BusinessUnit = CheckNull.AgainstString(item["BUSINESS_UNIT"]);
                        //PSUser.OrgUnitCode = BusinessUnit + "-" + DepartmentCode;
                        PSUser.OrgUnitCode = DepartmentCode;
                        PSUser.PositionCode = CheckNulls.AgainstString(item["JOBCODE"]);
                        PSUser.Grade = CheckNulls.AgainstString(item["Grade"]);
                        PSUser.Location = CheckNulls.AgainstString(item["Location"]);
                        PSUser.Action = CheckNulls.AgainstString(item["ACTION"]);
                        PSUser.Company = CheckNulls.AgainstString(item["COMPANY"]);
                        PSUser.BusinessUnit = BusinessUnit;
                        PSUser.DOB = CheckNulls.AgainstNullableDatetime(item["BIRTHDATE"]);
                        PSUser.EndpointSystemId = this.EndPoint.Id;
                        PSUser.PrefferedNameInitial = CheckNulls.AgainstString(item["NAME_INITIALS"]);
                        PSUser.FirstNameInitial  = CheckNulls.AgainstString(item["NAME_PREFIX"]);
                        PSUser.LastNameInitial = CheckNulls.AgainstString(item["NAME_SUFFIX"]);
                        PSUser.SupervisorId = CheckNulls.AgainstString(item["SUPERVISOR_ID"]);
                        DateTime? effectiveDate = CheckNulls.AgainstNullableDatetime(item["EFFDT"]); 
                        PSUser.Save(MasterConfigHelper.ServiceUser);


                        #region Trigger Points Rules

                        // OnBoarding Rule
                        if(PSUser.Action.Equals("HIR") && PSUser.JoinDate.HasValue)
                        {
                            // I need to check backdated scenario for honoring the Hire event.
                            if (userEvent != null)
                            {
                                userEvent.Add(new NHRMSUserEvent()
                                {
                                    EmployeeEvent = EmployeeEvents.NewJoinee,
                                    EffectiveDate = PSUser.JoinDate.Value
                                });
                            }
                        }
                        // Transfer Rule 
                        if (PSUser.Action.Equals("XFR") && PSUser.OrgUnitCode != OldDepartMent)
                        {
                            if (userEvent != null)
                            {
                                userEvent.Add(new NHRMSUserEvent()
                                {
                                    EmployeeEvent = EmployeeEvents.OUChange,
                                    EffectiveDate = effectiveDate
                                });
                            }
                        }

                        // Promotion Rule 
                        if (PSUser.Action.Equals("PRO") && PSUser.Grade != OldGrade)
                        {
                            if (userEvent != null)
                            {
                                userEvent.Add(new NHRMSUserEvent()
                                {
                                    EmployeeEvent = EmployeeEvents.Promotion,
                                    EffectiveDate = effectiveDate
                                });
                            }
                        }

                        #region CSEL/GoldBar Change
                        //For testing purpose i am creating this event here. Once ISMS trusted source implemented
                        // We will move this code over there.

                        // CSEL/GoldBar Rule 
                        if (PSUser.Action.Equals("DTA") && PSUser.PositionCode != PositionCode)
                        {
                            if (userEvent != null)
                            {
                                userEvent.Add(new NHRMSUserEvent()
                                {
                                    EmployeeEvent = EmployeeEvents.CSELGoldBar,
                                    EffectiveDate = effectiveDate
                                });
                            }
                        }
                        #endregion

                        #endregion
                        if (userEvent != null)
                        {
                            userEvent.ForEach(evt => evt.UserId = PSUser.Id);
                            NHRMSUserEvent.SaveList(userEvent, MasterConfigHelper.ServiceUser);
                            userEvent.Clear();
                        }
                      
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Error, string.Format("Error while reading the record - {0}", EmployeeID), ex);
                    }
                }
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Peoplesoft HRMS User Information Populated", dsUserinfo.Tables[0].Rows.Count.ToString()));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listPSUsers;
        }
        private void PopulateDepartments()
        {
            try
            {
                //string departmentQuery = @"select SETID,DEPTID,Descr as Description,SETID_LOCATION,Manager_ID,Manager_POSN,Company from PS_DEPT_TBL J where EFF_STATUS = 'A' AND EFFDT = (SELECT MAX(J1.EFFDT) FROM PS_DEPT_TBL J1 WHERE J1.DeptID =J.DeptID and J1.SETID = J.SETID)";

                string departmentQuery = @"select SETID,DEPTID,Descr as Description,SETID_LOCATION,Manager_ID,Manager_POSN,Company from PS_DEPT_TBL J where J.Manager_ID <> ' ' and J.SETID != 'SHARE' and EFF_STATUS = 'A' AND EFFDT = (SELECT MAX(J1.EFFDT) FROM PS_DEPT_TBL J1 WHERE J1.DeptID =J.DeptID and J1.SETID = J.SETID)";
                DataSet dsDepartment = getDSFromOracleQuery(departmentQuery);
                if (dsDepartment.Tables.Count > 0)
                {
                    foreach (DataRow item in dsDepartment.Tables[0].Rows)
                    {
                        try
                        {
                            string DepartmentCode = CheckNulls.AgainstString(item["DEPTID"]);
                            string tempCode = DepartmentCode;
                            string Description = CheckNulls.AgainstString(item["Description"]);
                            //string DepartmentSetID = CheckNulls.AgainstString(item["SETID"]);
                            //string BusinessUnit = CheckNulls.AgainstString(item["SETID_LOCATION"]);
                            string BusinessUnit = CheckNulls.AgainstString(item["SETID"]);
                            string MangerID = CheckNulls.AgainstString(item["Manager_ID"]).Trim();
                            string ManagerPosition = CheckNulls.AgainstString(item["Manager_POSN"]).Trim();
                            string Company = CheckNulls.AgainstString(item["Company"]).Trim();
                            DepartmentCode = BusinessUnit + "-" + DepartmentCode;
                            var nOrgDepartment = NHRMSOrganisation.GetByOrgCode(DepartmentCode, "Department");
                            var nOrgCompany = NHRMSOrganisation.GetByOrgCode(Company, "Company");
                            var nOrgBU = NHRMSOrganisation.GetByOrgCode(BusinessUnit, "BusinessUnit");
                            if (nOrgDepartment != null)
                            {
                                nOrgDepartment.Code = tempCode;
                                nOrgDepartment.Description = Description;
                                nOrgDepartment.Scope = "Department";
                                nOrgDepartment.ParentOrgCode = nOrgBU != null ? nOrgBU.Code : "";
                                nOrgDepartment.Reserved1 = nOrgCompany != null ? nOrgCompany.Code : "";
                                if (MangerID != "")
                                    nOrgDepartment.Head = MangerID;
                                else
                                {
                                    if (ManagerPosition != "")
                                        nOrgDepartment.Head = GetManagerFromPosition(ManagerPosition);
                                }
                                nOrgDepartment.LastModifiedBy = MasterConfigHelper.ServiceUser;
                                nOrgDepartment.LastModifiedDate = DateTime.Now;
                                nOrgDepartment.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                NHRMSOrganisation org = new NHRMSOrganisation()
                                {
                                    IsActive = true,
                                    EndpointSystemId = (int)this.EndPoint.Id,
                                    CreatedBy = Constants.CreatedBy,
                                    CreatedDate = DateTime.Now,
                                    LastModifiedBy = MasterConfigHelper.ServiceUser,
                                    LastModifiedDate = DateTime.Now,
                                    Code = DepartmentCode,
                                    Name = DepartmentCode,
                                    ParentOrgCode = nOrgBU != null ? nOrgBU.Code : "",
                                    Scope = "Department",
                                    Head = MangerID != "" ? MangerID : ManagerPosition != "" ? GetManagerFromPosition(ManagerPosition) : "",
                                    Description = Description,
                                    Reserved1 = nOrgCompany != null ? nOrgCompany.Code : ""
                                };
                                org.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception Occured while reading Department Data. Error Message is {1}", ex.Message), ex);
                        }
                    }
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Peoplesoft HRMS Department Information Populated", dsDepartment.Tables[0].Rows.Count.ToString()));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string GetManagerFromPosition(string PositionNumber)
        {
            string ManagerID = "";
            string ManagerQuery = @"select EMPLID from PS_JOB J where J.POSITION_NBR = '{0}'
                                   and J.HR_STATUS in ('A') and J.EMPL_STATUS in ('A')
                            AND J.EFFDT = (SELECT MAX(J1.EFFDT) FROM PS_JOB J1 WHERE J1.EMPLID = J.EMPLID
                            AND J1.EMPL_RCD = J.EMPL_RCD AND J1.EFFDT <= SYSDATE) 
                            AND J.EMPL_RCD = (SELECT MAX(J2.EMPL_RCD) FROM PS_JOB J2 WHERE J2.EMPLID = J.EMPLID
                            AND J2.EFFDT = J.EFFDT AND J2.EFFDT <= SYSDATE ) AND J.EFFSEQ = (SELECT MAX(J3.EFFSEQ) FROM PS_JOB J3 WHERE J3.EMPLID = J.EMPLID AND J3.EMPL_RCD = J.EMPL_RCD AND J3.EFFDT = J.EFFDT
                            AND J3.EFFDT <= SYSDATE)
                            ORDER BY J.EMPLID , J.EMPL_RCD, J.EFFDT, J.EFFSEQ";
            ManagerQuery = string.Format(ManagerQuery, PositionNumber);
            DataSet dsManager = getDSFromOracleQuery(ManagerQuery);
            if (dsManager.Tables.Count > 0 && dsManager.Tables[0].Rows.Count > 0)
            {
                ManagerID = dsManager.Tables[0].Rows[0]["EMPLID"].ToString();
            }
            return ManagerID;
        }
        private void PopulateCompany()
        {
            try
            {
                string companyQuery = @"select COMPANY,DESCR as Description from PS_Company_TBL where EFF_STATUS = 'A'";
                DataSet dsCompany = getDSFromOracleQuery(companyQuery);
                if (dsCompany.Tables.Count > 0)
                {
                    foreach (DataRow item in dsCompany.Tables[0].Rows)
                    {
                        try
                        {
                            string CompanyCode = CheckNulls.AgainstString(item["COMPANY"]);
                            string Description = CheckNulls.AgainstString(item["Description"]);
                            var nOrgCompany = NHRMSOrganisation.GetByOrgCode(CompanyCode, "Company");
                            if (nOrgCompany != null)
                            {
                                nOrgCompany.Name = CompanyCode;
                                nOrgCompany.Description = Description;
                                nOrgCompany.Scope = "Company";
                                nOrgCompany.LastModifiedBy = MasterConfigHelper.ServiceUser;
                                nOrgCompany.LastModifiedDate = DateTime.Now;
                                nOrgCompany.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                NHRMSOrganisation org = new NHRMSOrganisation()
                                {
                                    IsActive = true,
                                    EndpointSystemId = (int)this.EndPoint.Id,
                                    CreatedBy = Constants.CreatedBy,
                                    CreatedDate = DateTime.Now,
                                    LastModifiedBy = MasterConfigHelper.ServiceUser,
                                    LastModifiedDate = DateTime.Now,
                                    Code = CompanyCode,
                                    Name = CompanyCode,
                                    Scope = "Company",
                                    Description = Description
                                };
                                org.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception Occured while reading Company Data. Error Message is {1}", ex.Message), ex);
                        }
                    }
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Peoplesoft HRMS Company Information Populated", dsCompany.Tables[0].Rows.Count.ToString()));
                   
                }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void PopulateBusinessUnits()
        {
            try
            {
                string businessUnitQuery = @"select Business_Unit,Descr as Description,Default_SETID from PS_BUS_UNIT_TBL_HR where ACTIVE_INACTIVE = 'A'";
                DataSet dsBusinessUnits = getDSFromOracleQuery(businessUnitQuery);
                if (dsBusinessUnits.Tables.Count > 0)
                {
                    foreach (DataRow item in dsBusinessUnits.Tables[0].Rows)
                    {
                        try
                        {
                            string BusinessUnitCode = CheckNulls.AgainstString(item["Business_Unit"]);
                            string Description = CheckNulls.AgainstString(item["Description"]);
                            string Name = CheckNulls.AgainstString(item["Default_SETID"]);
                            //var nOrgBU = NHRMSOrganisation.GetByOrgName(BusinessUnitCode, "BusinessUnit");
                            var nOrgBU = NHRMSOrganisation.GetByOrgCode(BusinessUnitCode, "BusinessUnit");
                            if (nOrgBU != null)
                            {
                                nOrgBU.Name = Name;
                                nOrgBU.Description = Description;
                                nOrgBU.Scope = "BusinessUnit";
                                nOrgBU.LastModifiedBy = MasterConfigHelper.ServiceUser;
                                nOrgBU.LastModifiedDate = DateTime.Now;
                                nOrgBU.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                NHRMSOrganisation org = new NHRMSOrganisation()
                                {
                                    IsActive = true,
                                    EndpointSystemId = (int)this.EndPoint.Id,
                                    CreatedBy = Constants.CreatedBy,
                                    CreatedDate = DateTime.Now,
                                    LastModifiedBy = MasterConfigHelper.ServiceUser,
                                    LastModifiedDate = DateTime.Now,
                                    Code = BusinessUnitCode,
                                    Name = Name,
                                    Scope = "BusinessUnit",
                                    Description = Description
                                };
                                org.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception Occured while reading Business Unit. Error Message is {1}", ex.Message), ex);
                        }
                    }
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Peoplesoft HRMS Business Unit Information Populated", dsBusinessUnits.Tables[0].Rows.Count.ToString()));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void PopulateLocations()
        {
            try
            {
                string locationQuery = @"  SELECT SETID,Location,Descr as Description from PS_LOCATION_TBL where EFF_STATUS = 'A'";
                DataSet dsLocations = getDSFromOracleQuery(locationQuery);
                if (dsLocations.Tables.Count > 0)
                {
                    foreach (DataRow item in dsLocations.Tables[0].Rows)
                    {
                        try
                        {
                            string LocationCode = CheckNulls.AgainstString(item["Location"]);
                            string Description = CheckNulls.AgainstString(item["Description"]);
                            string Name = CheckNulls.AgainstString(item["SETID"]);
                            var nOrgLocation = NHRMSOrganisation.GetByOrgCode(LocationCode, "Location");
                            if (nOrgLocation != null)
                            {
                                nOrgLocation.Name = Name;
                                nOrgLocation.Description = Description;
                                nOrgLocation.Scope = "Location";
                                nOrgLocation.LastModifiedBy = MasterConfigHelper.ServiceUser;
                                nOrgLocation.LastModifiedDate = DateTime.Now;
                                var nOrgBusinessUnit = NHRMSOrganisation.GetByOrgCode(Name, "BusinessUnit");
                                nOrgLocation.ParentOrgCode = nOrgBusinessUnit != null ? nOrgBusinessUnit.Code : "";
                                nOrgLocation.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                var nOrgBusinessUnit = NHRMSOrganisation.GetByOrgCode(Name, "BusinessUnit");
                                NHRMSOrganisation org = new NHRMSOrganisation()
                                {
                                    IsActive = true,
                                    EndpointSystemId = (int)this.EndPoint.Id,
                                    CreatedBy = Constants.CreatedBy,
                                    CreatedDate = DateTime.Now,
                                    LastModifiedBy = MasterConfigHelper.ServiceUser,
                                    LastModifiedDate = DateTime.Now,
                                    Code = LocationCode,
                                    Name = Name,
                                    Scope = "Location",
                                    Description = Description,
                                    ParentOrgCode = nOrgBusinessUnit != null ? nOrgBusinessUnit.Code : ""
                                };
                                org.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception Occured while reading Location Data. Error Message is {1}", ex.Message), ex);
                        }
                    }
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Peoplesoft HRMS Location Information Populated", dsLocations.Tables[0].Rows.Count.ToString()));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void PopulateGrades()
        {
            try
            {
                string salaryGradeQuery = @"select SETID,Grade,Descr as Description from PS_SAL_GRADE_TBL G where G.Eff_Status = 'A' AND G.EFFDT = (SELECT MAX(G1.EFFDT) FROM PS_SAL_GRADE_TBL G1 WHERE G1.Grade =G.Grade and G1.SETID = G.SETID)";
                DataSet dsSalaryGrades = getDSFromOracleQuery(salaryGradeQuery);
                if (dsSalaryGrades.Tables.Count > 0)
                {
                    foreach (DataRow item in dsSalaryGrades.Tables[0].Rows)
                    {
                        try
                        {
                            string Grade = CheckNulls.AgainstString(item["Grade"]);
                            string Description = CheckNulls.AgainstString(item["Description"]);
                            string SetId = CheckNulls.AgainstString(item["SETID"]);
                            var nGrade = NGrade.GetByCode(Grade);
                            if (nGrade != null)
                            {
                                nGrade.Description = Description;
                                nGrade.LastModifiedBy = MasterConfigHelper.ServiceUser;
                                nGrade.LastModifiedDate = DateTime.Now;
                                nGrade.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                NGrade grade = new NGrade()
                                {
                                    Grade = Grade,
                                    Description = Description,
                                    SetID = SetId,
                                    CreatedBy = Constants.CreatedBy,
                                    CreatedDate = DateTime.Now,
                                    LastModifiedBy = Constants.CreatedBy,
                                    LastModifiedDate = DateTime.Now,
                                };
                                grade.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception Occured while reading Salary Grade. Error Message is {1}", ex.Message), ex);
                        }
                    }
                    WriteDiscoveryLog(LogLevelType.Info, string.Format("{0}  Peoplesoft HRMS Grade Information Populated", dsSalaryGrades.Tables[0].Rows.Count.ToString()));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
    }


}
