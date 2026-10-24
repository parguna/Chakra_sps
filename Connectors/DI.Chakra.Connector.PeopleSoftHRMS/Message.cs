using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.PeopleSoftHRMS
{
    public static class Message
    {
        public const string PeopleSoftHRMS_001 = "Discovery Process";
        public const string PeopleSoftHRMS_002 = "System";
        public const string PeopleSoftHRMS_100 = "Started PSHCM Connector Configuration";
        public const string PeopleSoftHRMS_101 = "PSHCM Connector Configuration Established SuccessFully.";
        public const string PeopleSoftHRMS_102 = "Error occured while Establishing the Configuration (PSHCM Connector Method).";
        public const string PeopleSoftHRMS_103 = "Started PSHCM Connection Establish";
        public const string PeopleSoftHRMS_104 = "{0} - Connection Established SuccessFully ";
        public const string PeopleSoftHRMS_105 = "Error occured while Establishing the Connection {0}(connect Method).";

        public const string PeopleSoftHRMS_106 = "{0} - PeopleSoft HRMS Users discovery started";
        public const string PeopleSoftHRMS_107 = "{0} - PeopleSoft HRMS Users discovery completed";
        public const string PeopleSoftHRMS_108 = "Error occured while processing the DiscoverHRMSUsers Method.";

        public const string PeopleSoftHRMS_109 = "{0} - PeopleSoft HRMS company discovery started";
        public const string PeopleSoftHRMS_110 = "{0} - PeopleSoft HRMS company discovery completed";
        public const string PeopleSoftHRMS_111 = "Error occured while processing the DiscoverCompany Method.";

        public const string PeopleSoftHRMS_112 = "{0} - PeopleSoft HRMS department discovery started";
        public const string PeopleSoftHRMS_113 = "{0} - PeopleSoft HRMS department discovery completed";
        public const string PeopleSoftHRMS_114 = "Error occured while processing the DiscoverDepartment Method.";

        public const string PeopleSoftHRMS_115 = "{0} - PeopleSoft HRMS designation discovery started";
        public const string PeopleSoftHRMS_116 = "{0} - PeopleSoft HRMS designation discovery completed";
        public const string PeopleSoftHRMS_117 = "Error occured while processing the DiscoverDesignation Method.";

        public const string PeopleSoftHRMS_118 = "{0} - PeopleSoft HRMS salaryGrade discovery started";
        public const string PeopleSoftHRMS_119 = "{0} - PeopleSoft HRMS salaryGrade discovery completed";
        public const string PeopleSoftHRMS_120 = "Error occured while processing the DiscoverSalaryGrade Method.";

        public const string PeopleSoftHRMS_121 = "{0} - PeopleSoft HRMS businessUnit discovery started";
        public const string PeopleSoftHRMS_122 = "{0} - PeopleSoft HRMS businessUnit discovery completed";
        public const string PeopleSoftHRMS_123 = "Error occured while processing the DiscoverBusinessUnit Method.";

        public const string PeopleSoftHRMS_124 = "SELECT COMPANY, RWS_COMPANY_DESCR FROM sysadm.PS_RWS_EMPL_HIRE_V;";//"SELECT JOB.COMPANY, COMPANY_TBL.DESCR FROM PS_Company_TBL WHERE EFF_STATUS = 'A'";
        public const string PeopleSoftHRMS_125 = "Company";
        public const string PeopleSoftHRMS_133 = "COMPANY";


        public const string PeopleSoftHRMS_126 = "SELECT SETID, DEPTID, Descr ,SETID_LOCATION,Manager_ID,Manager_POSN,Company FROM sysadm.PS_DEPT_TBL J WHERE J.Manager_ID<> ' ' AND J.SETID != 'SHARE' AND EFF_STATUS = 'A' AND EFFDT =(SELECT MAX(J1.EFFDT) FROM sysadm.PS_DEPT_TBL J1 WHERE J1.DeptID =J.DeptID AND J1.SETID = J.SETID)";
        public const string PeopleSoftHRMS_127 = "Department";

        public const string PeopleSoftHRMS_128 = "SELECT JOB.BUSINESS_UNIT, BUS_UNIT_TBL_HR.DESCR FROM sysadm.PS_BUS_UNIT_TBL_HR WHERE ACTIVE_INACTIVE = 'A'";
        public const string PeopleSoftHRMS_129 = "BusinessUnit";
        public const string PeopleSoftHRMS_130 = "Location";
        public const string PeopleSoftHRMS_131 = "Descr";
        public const string PeopleSoftHRMS_132 = "Default_SETID";

        public const string PeopleSoftHRMS_134 = "SELECT SETID,Jobcode,Descr FROM sysadm.PS_JOBCODE_TBL WHERE Eff_Status = 'A'";
        public const string PeopleSoftHRMS_135 = "BusinessUnit";
        public const string PeopleSoftHRMS_136 = "Business_Unit";
        public const string PeopleSoftHRMS_137 = "Descr";
        public const string PeopleSoftHRMS_138 = "Default_SETID";

        public const string PeopleSoftHRMS_139 = "SELECT SETID,Grade,Descr FROM sysadm.PS_SAL_GRADE_TBL G where G.Eff_Status = 'A' AND G.EFFDT = (SELECT MAX(G1.EFFDT) FROM sysadm.PS_SAL_GRADE_TBL G1 WHERE G1.Grade =G.Grade and G1.SETID = G.SETID)";
        public const string PeopleSoftHRMS_140 = "GRADE";
        public const string PeopleSoftHRMS_GRADE_DESC = "RWS_GRADE_DESCR";
        public const string PeopleSoftHRMS_SAL_ADMIN_PLAN = "SAL_ADMIN_PLAN";
        public const string PeopleSoftHRMS_PLAN_DESCR = "RWS_SAL_PLAN_DESCR";
        public const string PeopleSoftHRMS_141 = "SETID";
        public const string PeopleSoftHRMS_142 = "DEPTID";
        //  public const string PeopleSoftHRMS_DEPT_DESC = "JOB.DEPTID";
        public const string PeopleSoftHRMS_143 = "SETID_LOCATION";
        public const string PeopleSoftHRMS_144 = "Manager_ID";
        public const string PeopleSoftHRMS_145 = "Manager_POSN";
        public const string PeopleSoftHRMS_146 = "Error occured while processing the GetManagerID Method";
        public const string PeopleSoftHRMS_147 = @"SELECT EMPLID FROM PS_JOB J WHERE J.POSITION_NBR = '{0}'
                                                    AND J.HR_STATUS in ('A') and J.EMPL_STATUS in ('A')
                                                    AND J.EFFDT = (SELECT MAX(J1.EFFDT) FROM PS_JOB J1 WHERE J1.EMPLID = J.EMPLID
                                                    AND J1.EMPL_RCD = J.EMPL_RCD AND J1.EFFDT <= SYSDATE) 
                                                    AND J.EMPL_RCD = (SELECT MAX(J2.EMPL_RCD) FROM PS_JOB J2 WHERE J2.EMPLID = J.EMPLID
                                                    AND J2.EFFDT = J.EFFDT AND J2.EFFDT <= SYSDATE ) AND J.EFFSEQ = (SELECT MAX(J3.EFFSEQ) FROM PS_JOB J3 WHERE J3.EMPLID = J.EMPLID AND J3.EMPL_RCD = J.EMPL_RCD AND J3.EFFDT = J.EFFDT
                                                    AND J3.EFFDT <= SYSDATE)
                                                    ORDER BY J.EMPLID , J.EMPL_RCD, J.EFFDT, J.EFFSEQ";
        public const string PeopleSoftHRMS_148 = "EMPLID";
        public const string PeopleSoftHRMS_149 = "Jobcode";
        public const string PeopleSoftHRMS_150 = @"SELECT J.EMPLID EMPLID, J.EMPL_RCD, J.EFFDT, J.DEPTID DEPTID, J.JOBCODE, J.POSITION_NBR, J.SuperVisor_ID SUPERVISOR_ID, J.JOB_ENTRY_DT,
                                    J.HIRE_DT HIRE_DT, J.BUSINESS_UNIT, J.SETID_DEPT, J.SETID_JOBCODE, J.TERMINATION_DT TERMINATION_DT, J.ACTION,
                                    J.FULL_PART_TIME FULL_PART_TIME, J.COMPANY COMPANY, C.DESCR DEPTNAME,
                                    B.NAME NAME, B.FIRST_NAME FIRST_NAME, B.LAST_NAME LAST_NAME, B.MIDDLE_NAME MIDDLE_NAME, B.NAME_AC, B.NAME_Display NAME_Display,
                                    B.Address1 ADDRESS1, B.ADDRESS2 ADDRESS2, B.ADDRESS3 ADDRESS3, B.ADDRESS4 ADDRESS4, B.CITY, B.NUM1 NUM1, B.BIRTHDATE BIRTHDATE,
                                    B.NUM2 NUM2, B.COUNTY COUNTRY, B.STATE, B.POSTAL POSTAL, B.PHONE HOME_PHONE, B.SEX, J.REPORTS_TO, J.Location, J.Grade,
                                    B.NAME_INITIALS NAME_INITIALS, B.NAME_PREFIX NAME_PREFIX, B.NAME_SUFFIX NAME_SUFFIX FROM PS_JOB J
                                    INNER JOIN PS_PERSONAL_DATA B on B.emplID = J.EmPLID
                                    Inner JOIN ps_dept_tbl C on c.SETID = J.SETID_DEPT and C.DEPTID = J.DEPTID
                                    and C.ROWID in (select MAX(ROWID) from ps_dept_tbl group by SETID, DEPTID )
                                    WHERE J.HR_STATUS in ('A','I') and EMPL_STATUS in ('A','T') 
                                    AND J.EFFDT = (SELECT MAX(J1.EFFDT) FROM PS_JOB J1 WHERE J1.EMPLID = J.EMPLID
                                    AND J1.EMPL_RCD = J.EMPL_RCD) 
                                    AND J.EMPL_RCD = (SELECT MAX(J2.EMPL_RCD) FROM PS_JOB J2 WHERE J2.EMPLID = J.EMPLID
                                    AND J2.EFFDT = J.EFFDT ) AND J.EFFSEQ = (SELECT MAX(J3.EFFSEQ) FROM PS_JOB J3 WHERE J3.EMPLID = J.EMPLID AND J3.EMPL_RCD = J.EMPL_RCD AND J3.EFFDT = J.EFFDT)
                                    ORDER BY J.EMPLID , J.EMPL_RCD, J.EFFDT, J.EFFSEQ ";
  
        public const string PeopleSoftHRMS_ONETIME_USER_QUERY = "SELECT * FROM sysadm.PS_RWS_EMPLOYEE_LIST_V";
        public const string PEOPLESOFTHRMS_USERLIST = "SELECT * FROM sysadm.PS_RWS_EMPLOYEE_V ";
        public const string PeopleSoftHRMS_OffBoard_REG = "SELECT * FROM sysadm.PS_RWS_REG_EMP_TER";
        public const string PeopleSoftHRMS_OffBoard_ROS = "SELECT * FROM sysadm.PS_RWS_ROS_EMP_TER ";    
        public const string PeopleSoftHRMS_153 = " SELECT * FROM sysadm.PS_RWS_EMPL_HIRE_V ";
        public const string PeopleSoftHRMS_154 = " SELECT * FROM sysadm.PS_RWS_EMP_PRO_DEM ";
        public const string PeopleSoftHRMS_155 = " SELECT * FROM sysadm.PS_RWS_EMP_TRF_VW ";
        public const string PeopleSoftHRMS_156 = " SELECT * FROM sysadm.PS_RWS_EMP_CSEL_VW ";
        public const string PeopleSoftHRMS_157 = "NAME_DISPLAY";
        public const string PeopleSoftHRMS_158 = "FIRST_NAME";
        public const string PeopleSoftHRMS_159 = "MIDDLE_NAME";
        public const string PeopleSoftHRMS_160 = "LAST_NAME";
        public const string PeopleSoftHRMS_161 = "PREF_FIRST_NAME";
        public const string PeopleSoftHRMS_162 = "EMPLID";
        public const string PeopleSoftHRMS_163 = "NRIC";
        public const string PeopleSoftHRMS_164 = "USERID";
        public const string PeopleSoftHRMS_165 = "EMPL_CLASS";
        public const string PeopleSoftHRMS_EMPLNTTYPE = "EMPL_CLASS";
        public const string PeopleSoftHRMS_EMPLNTYPE_DESC = "RWS_EMP_TYPE_DESCR";
        public const string PeopleSoftHRMS_HR_STATUS = "HR_STATUS";

        public const string RWSEMPLCLSDESC = "RWS_EMPL_CLS_DESCR";
        public const string PeopleSoftHRMS_166 = "DEPTID";
        public const string PeopleSoftHRMS_DEPT_DESC = "RWS_DEPT_DESCR";
        public const string PeopleSoftHRMS_168 = "COMPANY";
        public const string PeopleSoftHRMS_COMPANY_DESC = "RWS_COMPANY_DESCR";
        public const string PeopleSoftHRMS_169 = "SAL_ADMIN_PLAN";
        public const string PeopleSoftHRMS_SALARY_DESC = "RWS_SAL_PLAN_DESCR";
        public const string PeopleSoftHRMS_170 = "GRADE";
        public const string EMAILADDR = "EMAIL_ADDR";
        public const string PeopleSoftHRMS_171 = "JOBCODE";
        public const string PeopleSoftHRMS_JOBCODE_DESC = "RWS_JOB_CD_DESCR";
        public const string PeopleSoftHRMS_172 = "ESTABID";
        public const string PeopleSoftHRMS_COSTCENTER_DESC = "RWS_COSTC_DESCR";
        public const string PeopleSoftHRMS_173 = "EMAILID";
        public const string PeopleSoftHRMS_174 = "JOB.EFFDT";
        public const string PeopleSoftHRMS_175 = "JOB.EFFDT";
        public const string PeopleSoftHRMS_EMPSTATUS = "HR_STATUS";
        public const string PeopleSoftHRMS_177 = "FULL_PART_TIME";
        public const string PeopleSoftHRMS_178 = "HOME_PHONE";
        public const string PeopleSoftHRMS_179 = "NUM1";
        public const string PeopleSoftHRMS_180 = "NUM2";
        public const string PeopleSoftHRMS_181 = "ADDRESS1";
        public const string PeopleSoftHRMS_182 = "ADDRESS2";
        public const string PeopleSoftHRMS_183 = "ADDRESS3";
        public const string PeopleSoftHRMS_184 = "ADDRESS4";
        public const string PeopleSoftHRMS_185 = "JOB.EFFDT";
        public const string PeopleSoftHRMS_186 = "COUNTRY";
        public const string PeopleSoftHRMS_187 = "JOB.EFFDT";
        public const string PeopleSoftHRMS_188 = "BUSINESS_UNIT";
        public const string PeopleSoftHRMS_BUSINESSUNIT_DESC = "RWS_BU_DESCR";
        public const string PeopleSoftHRMS_191 = "ACTION";
        public const string PeopleSoftHRMS_192 = "BIRTHDATE";
        public const string PeopleSoftHRMS_193 = "NAME_INITIALS";
        public const string PeopleSoftHRMS_194 = "NAME_PREFIX";
        public const string PeopleSoftHRMS_195 = "NAME_SUFFIX";
        public const string PeopleSoftHRMS_196 = "SUPERVISOR_ID";
        public const string PeopleSoftHRMS_197 = "EFFDT";
        public const string PeopleSoftHRMS_198 = "HIR";
        public const string PeopleSoftHRMS_199 = "DTA";
        public const string PeopleSoftHRMS_200 = "XFR";
        public const string PeopleSoftHRMS_201 = "PRO";
        public const string PeopleSoftHRMS_202 = "EFFDT";
        public const string PeopleSoftHRMS_LASTDAYATWORK = "LAST_DAY_AT_WRK";
        public const string PeopleSoftHRMS_TERMINATEDATE = "OFFBOARDDT";
        public const string PeopleSoftHRMS_JOINDATE = "ONBOARDDT";
        public const string PeopleSoftHRMS_203 = "RWS_DESIGNATION";
        public const string PeopleSoftHRMS_DESIGN_DESC = "RWS_DESIGNATION";
        public const string PeopleSoftHRMS_204 = "SELECT * from sysadm.PS_SAL_GRADE_TBL where GRADE ='{0}'";
        public const string PeopleSoftHRMS_205 = "MAX_RT_ANNUAL";
        public const string PeopleSoftHRMS_206 = "MIN_RT_ANNUAL";
        public const string PeopleSoftHRMS_207 = "MID_RT_ANNUAL";
        public const string PeopleSoftHRMS_208 = "{0} - PeopleSoft Onboard HRMS Users discovery started";
        public const string PeopleSoftHRMS_209 = "{0} - PeopleSoft Onboard HRMS Users discovery completed";
        public const string PeopleSoftHRMS_210 = "Error occured while processing the DiscoverOnboardusers Method.";
        public const string PeopleSoftHRMS_DOB = "BIRTHDATE";
        public const string PeopleSoftHRMS_IDENTITY_NO = "NATIONAL_ID";
        public const string PeopleSoftHRMS_IDENTITY_TYPE = "NATIONAL_ID_TYPE";
        public const string PeopleSoftHRMS_IDENTITY_COUNTRY = "NID_COUNTRY";
        public const string PeopleSoftHRMS_SUPERVISOR_ADID = "RWS_SUPV_USERID";
        public const string PeopleSoftHRMS_SUPERVISOR_EMPID = "SUPERVISOR_ID";
        public const string PEOPLESOFTHRMS_POSITIONCODE = "POSITIONCODE";
        public const string PeopleSoftHRMS_DESIGNATION = "RWS_DESIGNATION";
        public const string PeopleSoftHRMS_Location_Code = "LOCATION";
        public const string PeopleSoftHRMS_Location_Desc = "LOCATION_DESCR";
        public const string PeopleSoftHRMS_219 = "{0} - PeopleSoft HRMS OFF-BOARD discovery started";
        public const string PeopleSoftHRMS_220 = "{0} - PeopleSoft HRMS OFF-BOARD discovery completed";
        public const string PeopleSoftHRMS_221 = "{0} - PeopleSoft HRMS TRANSFER discovery started";
        public const string PeopleSoftHRMS_222 = "{0} - PeopleSoft HRMS TRANSFER discovery completed";
        public const string PeopleSoftHRMS_223 = "{0} - PeopleSoft HRMS PROMOTION/DEMOTION discovery started";
        public const string PeopleSoftHRMS_224 = "{0} - PeopleSoft HRMS PROMOTION/DEMOTION discovery completed";
        public const string PeopleSoftHRMS_225 = "TERMINATION_DT";
        public const string PeopleSoftHRMS_226 = "LAST_HIRE_DT";
        public const string PeopleSoftHRMS_EMPLOYEE_ID = "EMPLID";
        public const string PeopleSoftHRMS_CSEL_LICNO = "RWS_CRA_LIC_ID";
        public const string PeopleSoftHRMS_CSEL_DESC = "RWS_CRA_LIC_STAT";
        public const string PeopleSoftHRMS_START_DATE = "RWS_CRA_LIC_STRTDT";
        public const string PeopleSoftHRMS_EXPIRY_DATE = "RWS_CRA_LIC_EXPDT";
        public const string PeopleSoftHRMS_227 = "{0} - PeopleSoft HRMS CSEL discovery started";
        public const string PeopleSoftHRMS_228 = "{0} - PeopleSoft HRMS CSEL discovery completed";
        public const string PeopleSoftHRMS_229 = "Error occured while processing the DiscoverCSEL Method.";
        public const string PeopleSoftHRMS_230 = "Error occured while processing the GetData Method.";
        public const string PeopleSoftHRMS_231 = "INSERT INTO PS_USER(NAME_DISPLAY, FIRST_NAME, MIDDLE_NAME, LAST_NAME, PREFERRED_NAME, EMPL_ID, NRIC, OPR_ID, EMPLOYEE_TYPE, DEPT_ID, BUSINESS_UNIT, COMPANY, CORPORATE_PLAN, SALARY_GRADE, JOB_CODE, COST_CENTER, EMAIL, EFFECTIVE_DATE, DOB, IDENTITY_NO, IDENTITY_TYPE, IDENTITY_COUNTRY, LOCATION_CODE, EMPLOYEEAD_ID, SALARY_PLAN, DESIGNATION, SUPERVISOR_ADID, SUPERVISOR_EMPID) VALUES({0}','{1}',{2},'{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}', {17},{18},'{19}','{20}','{22}',{23},'{24}',{25},'{25}',{27},'{28}')";
        public const string PeopleSoftHRMS_300 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID> 102 </SystemID></InterfaceRequestHeaderDT><createUserVALParamDT><EmployeeID>{0}<EmployeeID> <AD - ID>{1}</AD - ID><FirstName>{2}</FirstName ><LastName>{3}</LastName><Email>{4}</Email ><IDAS_Tnx_ID>{5}</IDAS_Tnx_ID></createUserVALParamDT></UserIntegrationDS>";
        public const string PeopleSoftHRMS_301 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><updateRoleVALParamDT><AD-ID>{0}</AD-ID><Roles actiontype=Add'><RoleCode>{1}</RoleCode></Roles><Roles actiontype=Remove><RoleCode>{2}</RoleCode></Roles></updateRoleVALParamDT><KioskCampaignIntegrationDS>";
        public const string PeopleSoftHRMS_302 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><deleteUserVALParamDT><AD-ID>{0}</AD-ID><IDAS_Tnx_ID>{1}</IDAS_Tnx_ID><deleteUserVALParamDT></UserIntegrationDS>";
        public const string PeopleSoftHRMS_303 = "Error occured while processing the DiscoverOneTimeUsers Method.";
        public const string PeopleSoftHRMS_304 = "Error occured while processing the DiscoverOffboardUsers Method.";
        public const string PeopleSoftHRMS_305 = "Error occured while processing the DiscoverTransferUsers Method.";
        public const string PeopleSoftHRMS_306 = "Error occured while processing the DiscoverPromotionUsers Method.";
        public const string PeopleSoftHRMS_307 = "{0} - PeopleSoft Withdrawn discovery started";
        public const string PeopleSoftHRMS_308 = "{0} - PeopleSoft Withdrawn discovery completed";
        public const string PeopleSoftHRMS_309 = "{0} - PeopleSoft JobCode discovery started";
        public const string PeopleSoftHRMS_310 = "{0} - PeopleSoft JobCode discovery completed";
        public const string PeopleSoftHRMS_311 = "{0} - PeopleSoft CostCenter discovery started";
        public const string PeopleSoftHRMS_312 = "{0} - PeopleSoft CostCenter discovery completed";
        public const string PeopleSoftHRMS_313 = "{0} - PeopleSoft SalaryPlan discovery started";
        public const string PeopleSoftHRMS_314 = "{0} - PeopleSoft SalaryPlan discovery completed";
        public const string PeopleSoftHRMS_315 = "Error occured while processing the DiscoverJobCode Method.";
        public const string PeopleSoftHRMS_316 = "Error occured while processing the DiscoverCostCenter Method.";
        public const string PeopleSoftHRMS_317 = "Error occured while processing the DiscoverSalaryGrade Method.";
        public const string PeopleSoftHRMS_318 = "Error occured while processing the DiscoverSalaryPlan Method.";
    }
}
