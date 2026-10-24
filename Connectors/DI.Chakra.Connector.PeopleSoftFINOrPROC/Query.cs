namespace DI.Chakra.Connector.PeopleSoftFINOrPROC
{
    public static class Query
    {
        public const string roleDescriptionSQL = "select * from sysadm.PS_RWS_ROL_DSCR_VW";
        public const string roleId = "ROleName";
        public const string roleDescription = "DESCR";

        public const string NameDispaly = "NAME_DISPLAY";
        public const string FirstName = "FIRST_NAME";
        public const string MiddleName = "MIDDLE_NAME";
        public const string LastName = "LAST_NAME";
        public const string DOB = "DOB";
        public const string PreferredName = "PREFERRED_NAME";
        public const string EmployeeId = "EMPL_ID";
        public const string NRIC = "NRIC";
        public const string OprId = "ADID";
        public const string EmployeeType = "EMPLOYEE_TYPE";
        public const string EmployeeTypeDesc = "EMPLOYEE_TYPE_DESC";
        public const string DeptId = "DEPT_ID";
        public const string DepartmentDesc = "DEPARTMENT_DESC";
        public const string BusinessUnit = "BUSINESS_UNIT";
        public const string BusinessUnitDesc = "BUSINESS_UNIT_DESC";
        public const string Company = "COMPANY";
        public const string CompanyCodeDesc = "COMPANY_CODE_DESC";
        public const string CorporatePlab = "CORPORATE_PLAN";
        public const string SalaryPlanDesc = "SALARY_PLAN_DESC";
        public const string SalaryGrade = "SALARY_GRADE";
        public const string SalaryGradeDesc = "SALARY_GRADE_DESC";
        public const string JobCode = "JOB_CODE";
        public const string DesignationDesc = "DESIGNATION_DESC";
        public const string CostCenter = "COST_CENTER";
        public const string CostCenterDesc = "COSTCENTER_DESC";
        public const string Email = "EMAIL";
        public const string JoinDate = "JOIN_DATE";
        public const string TerminationDate = "TERMINATION_DATE";
        public const string Status = "STATUS";
        public const string ADID = "AD-ID";
        public const string RoleID = "RoleID";
        public const string NULL = "NULL";
        public const string InsertUserRoleTable = "INSERT INTO PeopleSoftFINOrPROCRoleInfoPeopleSoftFINOrPROCUserInfo(PeopleSoftFINOrPROCUserInfo_Id,PeopleSoftFINOrPROCRoleInfo_Id,EndpointSystemId,[Version]) VALUES({0},{1},{2},{3})";
        public const string DeleteUserRoleTable = "DELETE FROM PeopleSoftFINOrPROCRoleInfoPeopleSoftFINOrPROCUserInfo WHERE EndpointSystemId={0}";
        public const string Module = "MODULE";
        public const string CatalogueName = "CATALOG";
        public const string Role = "ROLENAME";

        public const string buId = "BUSINESS_UNIT";
        public const string buDescription = "DESCR";

        public const string bankDescriptionSQL = "SELECT * FROM sysadm.PS_RWS_BANK_ACC_VW";
        public const string bankAccount = "BANK_ACCOUNT_NUM";
        public const string bankAccountDesc = "BANK_CD";

        public const string cctoOUSQL = "SELECT * FROM  sysadm.PS_RWS_OU_CC_VW";
        public const string costCenter = "DEPTID";
        public const string operatingUnit = "OPERATING_UNIT";

        public const string outoTaggingSQL = "SELECT * FROM sysadm.OUtoTagging_VW ";
        public const string ouOperatingUnit = "OperatingUnit";
        public const string taggingRole = "TaggingRole";

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
        public const string BUISINESSUNIT_QUERY = "SELECT * FROM sysadm.PS_RWS_BU_DESC_VW";

        public const string AUDITUSER = "AUDITUSER";
        public const string ROLEUSER = "ROLEUSER";
        public const string AUDITSTAMP = "AUDITSTAMP";
        public const string ACTION = "ACTION";
        public const string ROLENAME = "ROLENAME";
        public const string REFCODE = "REFCODE";
        public const string USER2BU_QUERY = "SELECT OPRID,BUSINESS_UNIT FROM sysadm.PS_RWS_USER_BU_VW";
        public const string USER2BANK_QUERY = "SELECT OPRID, BANK_ACCOUNT_NUM FROM sysadm.PS_RWS_BANK_SEC_VW";
        public const string OprIdADID = "OPRID";

        public const string FlushQuery = "DELETE FROM PeopleSoftFINOrPROCFunctionInfoPeopleSoftFINOrPROCRoleInfo WHERE PeopleSoftFINOrPROCFunctionInfo_Id IN (SELECT ID FROM DI_PeopleSoftFINOrPROC_FunctionInfo WHERE EndPointSystemId = {0})"
                                         + "DELETE FROM DI_PeopleSoftFINOrPROC_FunctionInfo WHERE EndPointSystemId = {0}";
        public const string FinInsertQuery = "INSERT INTO DI_PeopleSoftFINOrPROC_FunctionInfo(PermissionName,MenuName,BarName,ContentReference,Description,IsDisplay,ModifiedDate,EndPointSystemId,CreatedBy"
                                         + ",CreatedDate) VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9})";

        public const string InsertRolFunQuery = "INSERT INTO PeopleSoftFINOrPROCFunctionInfoPeopleSoftFINOrPROCRoleInfo" +
                                                "(PeopleSoftFINOrPROCRoleInfo_Id, PeopleSoftFINOrPROCFunctionInfo_Id) VALUES({0},{1})";
    }
}
