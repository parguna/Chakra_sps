namespace DI.Chakra.Connector.PeopleSoftHRMS
{
    public static class Query
    {
       
        public const string NameDispaly = "NAME_DISPLAY";
        public const string FirstName = "FIRST_NAME";
        public const string MiddleName = "MIDDLE_NAME";
        public const string LastName = "LAST_NAME";
        public const string DOB = "BIRTHDATE";
        public const string PreferredName = "PREFERRED_NAME";
        public const string EmployeeId = "EMPLID";        
        public const string IdentityNo = "NATIONAL_ID";
        public const string IdentityType = "NATIONAL_ID_TYPE";
        public const string IdentityCountry = "NID_COUNTRY";
        public const string LocationCode = "LOCATION";
        public const string LocationCodeDescr = "LOCATION_DESC";        
        public const string OprId = "ADID";
        public const string EmployeeType = "EMPLOYEE_TYPE";
        public const string EmployeeTypeDesc = "EMPLOYEE_TYPE_DESC";
        public const string DeptId = "DEPTID";
        public const string DepartmentDesc = "RWS_DEPT_DESCR";
        public const string BusinessUnit = "BUSINESS_UNIT";
        public const string BusinessUnitDesc = "RWS_BU_DESCR";
        public const string Company = "COMPANY";
        public const string CompanyCodeDesc = "RWS_COMPANY_DESCR";
        public const string CorporatePlab = "CORPORATE_PLAN";
        public const string SalaryPlanDesc = "RWS_SAL_PLAN_DESCR";
        public const string SalaryPlan = "SAL_ADMIN_PLAN";
        public const string SalaryGrade = "GRADE";
        public const string SalaryGradeDesc = "RWS_GRADE_DESCR";
        public const string JobCode = "JOBCODE";
        public const string JobCodeDescr = "RWS_JOB_CD_DESCR";
        public const string DesignationDesc = "RWS_DESIGNATION";
        public const string Designation = "RWS_DESIGNATION";
        public const string CostCenter = "ESTABID";
        public const string CostCenterDesc = "RWS_COSTC_DESCR";
        public const string Email = "EMAILID";
        public const string JoinDate = "ONBOARDDT";
        public const string TerminationDate = "OFFBOARDDT";
        public const string Status = "HR_STATUS";
        public const string ADID = "USERID";
        public const string RoleID = "RoleID";
        public const string NULL = "NULL";
        public const string InsertUserRoleTable = "INSERT INTO PeopleSoftFINOrPROCRoleInfoPeopleSoftFINOrPROCUserInfo(PeopleSoftFINOrPROCUserInfo_Id,PeopleSoftFINOrPROCRoleInfo_Id,EndpointSystemId,[Version]) VALUES({0},{1},{2},{3})";
        public const string DeleteUserRoleTable = "DELETE FROM PeopleSoftFINOrPROCRoleInfoPeopleSoftFINOrPROCUserInfo WHERE EndpointSystemId={0}";
        public const string Module = "MODULE";
        public const string CatalogueName = "CATALOG";
        public const string Role = "ROLENAME";

        public const string buId = "BUSINESS_UNIT";
        public const string buDescription = "DESCR";

       
        public const string PermissionName = "PERMNAME";
        public const string MenuName = "MENUNAME";
        public const string BarName = "BARNAME";
        public const string ContentReference = "CONTENTREF";
        public const string Description = "DESCRIPTION";
        public const string Display = "DISPLAY";
        public const string ModifiedAt = "LASTMODIFIED";


        public const string operatorId = "OperatorID";
        public const string ADId = "ADID";
        public const string dateTime = "DateTime";
        public const string action = "Action";
        public const string roleUserID = "RoleUserID";
        public const string IDASRefNo = "IDASRefNo";
        public const string BUISINESSUNIT_QUERY = "SELECT * FROM PS_RWS_BU_DESC_VW";

        public const string AUDITUSER = "AUDITUSER";
        public const string ROLEUSER = "ROLEUSER";
        public const string AUDITSTAMP = "AUDITSTAMP";
        public const string ACTION = "ACTION";
        public const string ROLENAME = "ROLENAME";
        public const string REFCODE = "REFCODE";
        public const string EmploymenttType = "EMPL_CLASS";
        public const string EmploymentTypeDescr = "RWS_EMP_TYPE_DESCR";
        public const string SupervisorADID = "RWS_SUPV_USERID";
        public const string SupervisorEmpID = "SUPERVISOR_ID";
        public const string NRIC = "NRIC";


    }
}
