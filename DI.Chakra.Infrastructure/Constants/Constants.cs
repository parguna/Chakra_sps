
using System;

namespace DI.Chakra.Infrastructure
{
    public class Constants
    {

        #region EmployeeStatus

        public class EmployeeStatus
        {
            public const string OnBoarded = "OnBoarded";
            public const string OffBoarded = "OffBoarded";
            public const string NewEmployee = "NewEmployee";
        }

        #endregion EmployeeStatus
        #region Genral
        #region SAPErrorMessage
        public const string SAPPasswordResetErrorMessage = "Choose a password that is different from the last 5 passwords.";
        #endregion
        public const string DateFormat = "yyyy-MM-dd HH:mm tt";
        public const char COMMA = ',';
        public const string CreatedBy = "System";
        public readonly static DateTime SAP_MAX_DATE = new DateTime(9999, 12, 31);
        #endregion
        #region Login
        public const string IACMADMIN = "IACMADMIN";
        #endregion
        #region EndPointSystem

        public const string ENDPOINTSYSTEMTYPENAME = "EndPointSystemTypeName";

        public const string ENDPOINTSYSTEMTYPEID = "EndpointSystemTypeId";

        public const string HOSTNAME = "HostName";

        public const string ENDPOINTSYSTEMID = "EndPointSystemId";

        public const string NORECORDS = "No Record Found";

        public const string COLUMNNAME = "ColumnName";

        public const string DATATYPE = "DataType";

        public const string LISTENDPOINTSYSTEM = "ListEndPointSystem";

        public const string DISCOVERYJOB = "DiscoveryJob";

        public const string DISCOVERYHISTORY = "DiscoveryHistory";

        public const string NAME = "name";

        public const string DBID = "dbid";

        public const string REQUESTTYPEDESC = "RequestTypeDesc";

        public const string REQUESTTYPEID = "RequestTypeId";

        public const string PROVWFTYPEDESC = "WorkflowTypeDesc";

        public const string PROVWFTYPEID = "WorkflowTypeId";

        public const string USERTYPEDESC = "UserTypeDesc";

        public const string USERTYPEID = "UserTypeId";

        public const string APPROVALUSERTYPEDESC = "ApprovalUserTypeDesc";

        public const string APPROVALUSERTYPEID = "ApprovalUserTypeId";

        public const string ROLETYPEDESC = "RoleTypeDesc";

        public const string ROLETYPEID = "RoleTypeId";
        public const string NPAC = "NPAC/NMACS/ENIS/PROTIS/CIMS/IAMS/UOS/MDMS";
        public const string VPN = "VPN";
        public const string GEMS = "GEMS";

        #endregion
        #region EndPointSystem Type

        public const string ACTIVEDIRECTORY = "Active Directory";
        public const string LOTUSNOTES = "Lotus Notes";
        public const string TRUSTEDSOURCE = "Trusted Source";
        public const string FLATFILE = "Flat File";
        public const string EXCHANGE = "Exchange Server";
        public const string DATABASETABLE = "DatabaseTable";

        public const string SAP = "SAP";
        public const string LINUX = "Linux";
        public const string ORCLUM = "OracleUserManagement";
        public const string SAPEP = "Portals (EP / CE / IP)";

        public const string SCM = "SCM";
        public const string OAS = "OAS";

        public const string WINDOWS = "Windows";
        public const string SQLUM = "SQLUserManagement";
        public const string DBTABLE = "DB Table Connector";
        public const string NISOFT = "NiSoft";
        public const string SIS = "Students Portal";

        public const string In_houseSystems = "Other Systems";
        public const string Miscellaneous = "Miscellaneous";
        public const string PeopleSoftFinanceBI = "PSFBI";
        public const string OracleEBS = "Oracle EBS";

        #endregion
        #region AttributeMapping

        public const string ATTRIBUTEMAPPINGDEFAULTLOADPATH = "~//IACM//IACM_VER_3.5//Source//Web//SqlScripts//DI_InsertAdsAttributeMapping.sql";
        public const string ADSATTRIBUTEMAPPINGPATH = "\\DI_InsertAdsAttributeMapping.sql";
        public const string LNSATTRIBUTEMAPPINGLOADPATH = "\\DI_InsertLnsAttributemapping.sql";
        public const string EXATTRIBUTEMAPPINGLOADPATH = "\\DI_InsertExAttributeMapping.sql";
        public const string SAPATTRIBUTEMAPPINGLOADPATH = "\\DI_InsertSapAttributeMapping.sql";
        public const string WINATTRIBUTEMAPPINGLOADPATH = "\\DI_InsertWinAttributeMapping.sql";
        public const string SCMATTRIBUTEMAPPINGLOADPATH = "\\DI_InsertScmAttributeMapping.sql";
        public const string OASATTRIBUTEMAPPINGLOADPATH = "\\DI_InseretOasAttributeMapping.sql";
        public const string TRSATTRIBUTEMAPPINGLOADPATH = "\\DI_InsertTRSAttributeMapping.sql";
        public const string LINUXATTRIBUTEMAPPINGLOADPATH = "\\DI_InsertLinuxAttributeMapping.sql";
        public const string SQLUMATTRIBUTEMAPPINGLOADPATH = "\\DI_InseretSQLUMAttributeMapping.sql";
        public const string DATABASETABLEATTRIBUTEMAPPINGLOADPATH = "\\DI_InsertDatabaseTableAttributeMapping.sql";
        public const string NISOFTATTRIBUTEMAPPINGLOADPATH = "\\DI_InsertNiSoftAttributeMapping.sql";
        public const string SAPEPATTRIBUTEMAPPINGLOADPATH = "\\DI_InsertSAPEPAttributeMapping.sql";
        public const string SISATTRIBUTEMAPPINGLOADPATH = "\\DI_InsertSISAttributeMapping.sql";
        public const string ORCLUMATTRIBUTEMAPPINGLOADPATH = "\\DI_InsertORCLUMAttributeMapping.sql";

        #endregion
        #region Configuration Tab

        public const string CONFIGURATION = "Configuration";
        public const string ROLEMANAGEMENT = "RoleManagement";
        public const string TABLEUSER = "DI_M_User";
        public const string UNMAPPEDUSERS = "Un-MappedUsers";
        public const string USERMANGEMENT = "UserManagement";
        public const string USERLISTING = "UserListing";
        public const string UILOGVIEWER = "UILogViewer";
        public const string SERVICELOGVIEWER = "ServiceLogViewer";

        #endregion
        #region DropDown

        public const string SELECT = "--Select--";
        public const string ALL = "ALL";

        #endregion
        #region BusinessRole

        public const string BusinessRoleId = "BusinessRoleId";
        public const string BusinessRoleName = "DesignationDesc";

        #endregion
        #region Subsystem
        public const string GamingSubSystem = "Gaming";
        public const string NonGamingSubSystem = "NonGaming";
        #endregion
        #region User

        public const string USERID = "UserId";
        public const string USERNAME = "Username";
        public const string USERNAMECONSTRUCTION = "UserName Construction";
        public const string USERNAMEEXISTS = "AccountName/UserName";
        public const string USERNAMENOTEXISTS = "AccountName/ADID Construction Failed";
        #endregion
        #region EmployeeType

        public const string EmployeeType_EMPLOYEE = "Employee";
        public const string EmployeeType_NONEMPLOYEE = "Non Employee";
        public const string EmployeeType_SERVICE = "Service";

        public const string EMPLOYEETYPEID = "EmployeeTypeId";
        public const string EMPLOYEETYPEDESC = "EmployeeTypeDescription";

        #endregion
        #region Group

        public const string GROUPID = "GroupId";
        public const string GROUPNAME = "GroupName";

        #endregion
        #region Organisation

        public const string ORGANISATIONNAME = "OrganisationName";
        public const string ORGUNITPREFIX = "OrgUnitPrefix";

        #endregion
        #region Attestation

        public const string ATTESTATIONID = "AttestationId";
        public const string ATTESTATIONNAME = "AttestationName";

        #endregion
        #region WORKFLOW

        public const string LISTWORKFLOW = "Workflow";
        public const string LISTWORKFLOWCONFIGURATION = "WorkflowConfiguration";

        #endregion
        #region LogedIn User Info

        public const string UserAccessRights = "LogedInUserAccessRightInfo";
        public const string UserABACList = "LogedInUserABACInfo";

        #endregion
        #region AccessDenied Message

        public const string AccessDeniedMsg = "You do not have permission to view this page!";

        #endregion
        #region TabMenuIDs

        #region Query to Get this List
        //SELECT   
        //'public const int '+ 
        //REPLACE(REPLACE ( REPLACE('TabMenuId_'+TabName +'_'+MenuName,' ',''),'/',''),'-','')
        //+' = '+ CAST(TabMenuId AS VARCHAR)+';' FROM DI_M_TabMenu
        //ORDER BY TabMenuId
        #endregion

        public const string TabItem_Attestation = "Attestation";
        public const string TabItem_Compliance = "Compliance";
        public const string TabItem_Configuration = "Configuration";
        public const string TabItem_Configuration_SAP = "SAP";
        public const string TabItem_Configuration_SAPEP = "Portals(EP/CE/IP)";
        public const string TabItem_Configuration_OTHERS = "Other Systems";
        public const string TabItem_EndpointSystem = "Endpoint System";
        public const string TabItem_Home = "Home";
        public const string TabItem_PasswordManagement = "Password Management";
        public const string TabItem_Provisioning = "Provisioning";
        public const string TabItem_Report = "Report";
        public const string TabItem_User = "User";
        public const string TabItem_Workflow = "Workflow";
        public const string TabItem_OtherSystems = "Other System";
        public const string Tabitem_SAP = "SAP";
        public const string tabitem_SAPHRMS = "SAP-HRMS";
        public const string tabitem_SAPMSSL = "SAP-MSSL";
        public const string tabitem_SAPEBS = "SAP-EBS";
        public const string tabitem_SAPOTHERS = "SAP-OTHERS";
        public const string tabitem_Datamanagement = "Data Management";
        public const string tabitem_lotusnotes = "Lotus Notes";
        public const string tabitem_Npac = "Npac";
        public const string tabitem_Misc = "Miscellaneous";
        public const string TabItem_FailedRequest = "Failed Requests";

        public const int TabItem_Configuration_MailManagement_Parent = 403;
        public const int TabItem_Compliance_FunctionManagement_Parent = 702;
        public const int TabItem_Compliance_ConflictManagement_Parent = 703;
        public const int TabItem_Compliance_RiskManagement_Parent = 701;
        public const int TabItem_Compliance_ReportManagement_Parent = 710;
        public const int TabItem_Compliance_BulkOperation_Parent = 716;
        public const int TabMenuId_Provisioning_PasswordSync = 817;
        public const int TabItem_Provisioning_ProvisioningConfiguration_Parent = 821;
        public const int TabItem_UserAccessMenuId = 305;
        public const int TabItem_UserNonEmployees = 106;
        public const int TabItem_UserList = 101;
        public const int TabItem_Provisioning_FailedRequests = 840;

        #endregion
        #region Attribute Info of ABAC

        public const string ABACType_Tenant = "Institution";
        public const string ABACType_Manager = "Manager";
        public const string AllUsersOfCrntManager = "AllUsersOfCurrentManager";

        #endregion
        #region Trusted Source file Columns

        ////public const int TSColumn_EmployeeId = 0;
        ////public const int TSColumn_DisplayName = 1;
        ////public const int TSColumn_Organisation = 2;
        ////public const int TSColumn_BusinessRole = 3;
        ////public const int TSColumn_ConcurrentApptn=4; //Not used as of now.
        ////public const int TSColumn_JoinDate = 5;
        ////public const int TSTotalColumns = 6;

        public const int TSColumn_NRIC = 0;
        public const int TSColumn_DisplayName = 1;
        public const int TSColumn_CompanyName = 2;
        public const int TSColumn_Organisation = 3;
        public const int TSColumn_DivisionName = 4;
        public const int TSColumn_BranchName = 5;
        public const int TSColumn_SectionName = 6;
        public const int TSColumn_CompanyCode = 7;
        public const int TSColumn_DepartmentCode = 8;
        public const int TSColumn_DivisionCode = 9;
        public const int TSColumn_BranchCode = 10;
        public const int TSColumn_SectionCode = 11;
        public const int TSColumn_BusinessRole = 12;
        public const int TSColumn_TerminationDate = 13;
        public const int TSColumn_JoinDate = 14;
        public const int TSColumn_OldNRIC = 15;
        public const int TSColumn_EmployeeId = 16;
        public const int TSColumn_EmailId = 17;
        public const int TSColumn_TelephoneNo = 18;
        public const int TSColumn_UserName = 19;
        public const int TSColumn_FirstName = 20;
        public const int TSColumn_LastName = 21;
        public const int TSColumn_Location = 22;

        public const int TSTotalColumns = 23;

        #endregion
        #region AccessRequest Session Keys

        public const string AccessRequest_Session_RequestForUserIdKey = "ACCESSREQUEST_SESSION_REQUESTFORUSERID";
        public const string AccessRequest_Session_TicketNo = "ACCESSREQUEST_SESSION_TICKETNO";
        public const string AccessRequest_PrivilegedUserID = "PrivilegedUserID";

        #endregion
        #region ToolTip For User Table

        //public const string USERID = "UserId";

        public const string NRIC = "NRIC";
        public const string NRICTOOLTIP = "National Registration Identity Card";

        public const string FIN = "FIN";
        public const string FINTOOLTIP = "Foreign Identification Number";

        public const string PASSPORTNO = "PassportNo";
        public const string PASSPORTNOTOOLTIP = "Identity and Nationality of holder";

        //public const string USERNAME = "UserName";
        public const string USERNAMETOOLTIP = "Your id name you sign in with";

        public const string PASSWORD = "Password";
        public const string PASSWORDTOOLTIP = "Password";

        public const string FIRSTNAME = "FirstName";
        public const string FIRSTNAMETOOLTIP = "Given Name";

        public const string MIDDLENAME = "MiddleName";
        public const string MIDDLENAMETOOLTIP = "Middle Name of User";

        public const string LASTNAME = "LastName";
        public const string LASTNAMETOOLTIP = "Last name is family name.";


        public const string DISPLAYNAME = "DisplayName";
        public const string DISPLAYNAMETOOLTIP = "Display Name of user";


        public const string EMPLOYEEID = "EmployeeId";
        public const string EMPLOYEEIDTOOLTIP = "Employee Id of User";


        public const string FULLTIMEEMPLOYEE = "FullTimeEmployee";
        public const string FULLTIMEEMPLOYEETOOLTIP = "Full Time Employee";

        public const string ORGANISATIONTID = "OrganisationtId";

        public const string EMAILID = "EmailId";
        public const string EMAILIDTOOLTIP = "Email addresses, ex jsmith@gmail.com";


        public const string MANAGERID = "ManagerId";
        public const string MANAGERIDTOOLTIP = "Manager Id of User";


        public const string LOCATION = "Location";
        public const string LOCATIONTOOLTIP = "Location of User";

        public const string ADDRESS = "Address";
        public const string ADDRESSTOOLTIP = "Address of User";


        public const string COUNTRY = "Country";
        public const string COUNTRYTOOLTIP = "Country of User";

        public const string CITY = "City";
        public const string CITYTOOLTIP = "City of User";

        public const string STATE = "State";
        public const string STATETOOLTIP = "an organized political community, living under a government";

        public const string TELEPHONENUMBER = "TelephoneNumber";
        public const string TELEPHONENUMBERTOOLTIP = "Phone Number";

        public const string HOMEPHONENUMBERTOOLTIP = "Home Phone Number";
        public const string HOMEPHONENUMBER = "Home phone number";

        public const string MOBILENUMBER = "MobileNumber";
        public const string MOBILENUMBERTOOLTIP = "Mobile Number";

        public const string BUSINESSROLE = "BusinessRole";
        public const string BUSINESSROLETOOLTIP = "Business Roles represent collections of users who perform similar functions in an organization.";


        public const string COMPANY = "Company";
        public const string COMPANYTOOLTIP = "Working Organisation";
        //The body of actors employed in a theater or in the production of a play

        public const string JOINDATE = "JoinDate";
        public const string JOINDATETOOLTIP = "Joining Date in Organisation";

        public const string REHIREDATE = "RehireDate";
        public const string REHIREDATETOOLTIP = "Re-Join Date";

        public const string TERMINATIONDATE = "TerminationDate";
        public const string TERMINATIONDATETOOLTIP = "Relieving Date";

        public const string REMARKS1 = "Remarks1";
        public const string REMARKS1TOOLTIP = "Mistakes of User";

        public const string REMARKS2 = "Remarks2";
        public const string REMARKS3 = "Remarks3";
        public const string ISACTIVE = "Isactive";
        public const string ISACTIVETOOLTIP = "Active User";

        public const string DEEPIDENTITYROLE = "DeepidentityRole";
        public const string EMPLOYEESTATUS = "EmployeeStatus";
        public const string EMPLOYEESTATUSTOOLTIP = "Employee Working Status";

        public const string EMPLOYEETYPE = "EmployeeType";
        public const string WORKADDRESS1 = "WorkAddress1";
        public const string WORKADDRESS2 = "WorkAddress2 ";
        public const string WORKADDRESS3 = "WorkAddress3";
        public const string WORKADDRESS4 = "WorkAddress4";
        public const string WORKPOSTALCODE = "WorkPostalCode";
        public const string COUNTRY2CHAR = "Country2char";
        public const string BUILDING = "Building";
        public const string CREATEDDATE = "CreatedDate";
        public const string LASTMODIFIEDBY = "LastModifiedby";
        public const string LASTMODIFIEDDATE = "LastModifiedDate";

        #endregion
        #region Provisioning

        #region Provisioning Role

        public const string MSG_Provisioning_Role_Added_Success = "New Role Added successfully";
        public const string MSG_Provisioning_Role_Updated_Success = "Role Updated successfully";
        public const string MSG_Provisioning_Role_SaveError = "Error occurs while Saving Data.";
        public const string MSG_Provisioning_Role_RoleNameExist = "Data not saved: Role Name already exists.";

        public const string MSG_Provisioning_Role_Delete_Success = "Role deleted successfully";
        public const string MSG_Provisioning_Role_Delete_GeneralError = "Error occurs while deleting Provisioning Role.";
        public const string MSG_Provisioning_Role_Delete_DependantExistError = "Provisioning Role cannot be deleted as Role(s) \"{0}\" are dependant on it.";
        public const string MSG_Provisioning_Role_Delete_PolicyExistError = "Provisioning Role cannot be deleted as Policy \"{0}\" use this Role.";

        public const string ProvisioningRoleId = "ProvisioningRoleId";
        public const string ProvisioningRoleName = "RoleName";

        #endregion

        #region Provisioning Form
        public const string ProvisioningFormId = "ProvisioningFormId";
        public const string ProvisioningFormName = "FormName";
        public const string ConfigMatrix_CompanyCode = "CompanyCode";
        public const string ConfigMatrix_DepartmentCode = "DepartmentCode";
        public const string ConfigMatrix_DivisionCode = "DivisionCode";
        public const string ConfigMatrix_CostCenterCode = "CostCenterCode";
        public const string ConfigMatrix_Position = "PositionText";
        #endregion

        #region Provisioning Policy

        public const string ProvisioningPolicyManagement = "ProvisioningPolicyManagement";

        public const string MSG_Provisioning_Policy_Added_Success = "New Policy Added successfully.";
        public const string MSG_Provisioning_Policy_Updated_Success = "Policy Updated successfully.";
        public const string MSG_Provisioning_Policy_SaveError = "Error occurred while Saving Data.";
        public const string MSG_Provisioning_Policy_PolicyNameExist = "Data not saved: Policy Name already exists.";

        public const string MSG_Provisioning_Policy_Delete_Success = "Policy deleted successfully....";
        public const string MSG_Provisioning_Policy_Delete_GeneralError = "Error occurred while deleting Provisioning Policy.";
        public const string MSG_Provisioning_Policy_Delete_DependantExistError = "Provisioning Policy cannot be deleted as Policy is assigned to Role \"{0}\".";

        public const string ProvisioningPolicyId = "ProvisioningPolicyId";
        public const string ProvisioningPolicyName = "PolicyName";


        #endregion

        #region Provisioning Matrix
        public const string MandatoryField_CostCenter = "CostCenter";
        public const string MandatoryField_PurchaseGroup = "PurchaseGroup";
        public const string MandatoryField_Plant = "Plant";
        public const string MandatoryField_StoreLocation = "StorageLocation";
        public const string MandatoryField_MRPController = "MRPController";
        #endregion

        #region Provisioning Fields
        // SAP
        public const string SAP_System_Roles_Field = "SystemRolesList";
        public const string SAP_HOD_Roles_Field = "HODRolesList";
        public const string SAP_Dialog_User_Type = "A";
        public const string SAP_System_User_Type = "B";
        public const string SAPUserType = "UserType";
        public const string SAP_Default_Roles_Field = "Roles";
        public const string Is_HOD = "IsHOD";
        //Provisioning
        public const string AdHoc_ValidFrom = "AdHocAccessRequest.ValidFrom";
        public const string AdHoc_ValidTo = "AdHocAccessRequest.ValidTo";
        public const string AdHoc_RequesterUserId = "AdHocAccessRequest.RequesterUserId";
        public const string AdHoc_CreatedDate = "AdHocAccessRequest.CreatedDate";
        public const string AdHoc_Comments = "AdHocAccessRequest.Comments";
        public const string AccessRequest_RequesterUserName = "RequesterUserName";
        public const string AccessRequest_Id = "Id";
        #endregion

        #endregion
        #region UIAuditLog Messages
        public const string UIAuditLog_Provisioning_AccessRequest_Added = "New Access Request created for user(s) \"{0}\" Added By \"{1}\"";
        public const string UIAuditLog_Provisioning_Role_Added = "New Provisioning Role \"{0}\" Added By \"{1}\"";
        public const string UIAuditLog_Provisioning_Role_Edited = "Provisioning Role \"{0}\" Edited By \"{1}\"";
        public const string UIAuditLog_Provisioning_Role_Deleted = "Provisioning Role \"{0}\" Deleted By \"{1}\""; // "Provisioning RoleId 
        public const string UIAuditLog_Provisioning_Policy_Added = "New Provisioning Policy \"{0}\" Added By \"{1}\"";
        public const string UIAuditLog_Provisioning_Policy_Edited = "Provisioning Policy \"{0}\" Edited By \"{1}\"";
        public const string UIAuditLog_Provisioning_Policy_Deleted = "Provisioning Policy \"{0}\" Deleted By \"{1}\""; // "Provisioning PolicyId 
        public const string UIAuditLog_Provisioning_Form_Added = "New Provisioning Form \"{0}\" Added By \"{1}\"";
        public const string UIAuditLog_Provisioning_Form_Edited = "Provisioning Form \"{0}\" Edited By \"{1}\"";
        public const string UIAuditLog_Provisioning_Form_Deleted = "Provisioning Form \"{0}\" Deleted By \"{1}\"";
        public const string UIAuditLog_Provisioning_AdminAutoProvisioningCheck = "Auto Provisioning for User(s) \"{0}\" Created By \"{1}\"";
        public const string UIAuditLog_Provisioning_DepartmentChange = "Department Change for User(s) \"{0}\" Created By \"{1}\"";
        public const string UIAuditLog_Provisioning_MassPasswd_Reset = "Mass Password was reset for user(s) \"{0}\" in Endpoint \"{1}\" by user \"{2}\" ";

        //public const string UIAuditLog_Configuration_Role_Added = "New DeepIdentity Role \"{0}\" Added By \"{1}\"";
        //public const string UIAuditLog_Configuration_Role_Edited = "DeepIdentity Role \"{0}\" Edited By \"{1}\"";
        //public const string UIAuditLog_Configuration_Role_Deleted = "DeepIdentity RoleId \"{0}\" Deleted By \"{1}\"";
        //public const string UIAuditLog_Configuration_RoleAssignment_AddedToUser = "DeepIdentity Role \"{0}\" Assigned to user(s) \"{1}\" By \"{2}\"";
        //public const string UIAuditLog_Configuration_RoleAssignment_RemovedForUser = "DeepIdentity Role \"{0}\" Removed for user(s) \"{1}\" By \"{2}\"";
        //public const string UIAuditLog_Configuration_RoleAssignment_AddedToGroup = "DeepIdentity Role \"{0}\" Assigned to Group \"{1}\" By \"{2}\"";

        public const string UIAuditLog_Configuration_Role_Added = "New Role \"{0}\" Added By \"{1}\"";
        public const string UIAuditLog_Configuration_Role_Edited = "Role \"{0}\" Edited By \"{1}\"";
        public const string UIAuditLog_Configuration_Role_Deleted = "Role \"{0}\" Deleted By \"{1}\""; // "RoleId \"{0}\" Deleted By \"{1}\"";
        public const string UIAuditLog_Configuration_Role_RoleNameExist = "Data not saved: Role Name \"{0}\" already exists.";
        public const string UIAuditLog_Configuration_RoleAssignment_AddedToUser = "Role \"{0}\" Assigned to user(s) \"{1}\" By \"{2}\"";
        public const string UIAuditLog_Configuration_RoleAssignment_RemovedForUser = "Role \"{0}\" Removed for user(s) \"{1}\" By \"{2}\"";
        public const string UIAuditLog_Configuration_RoleAssignment_AddedToGroup = "Role \"{0}\" Assigned to Group \"{1}\" By \"{2}\"";

        public const string UIAuditLog_Configuration_RoleAssignment_RemovedForGroup = "Role Assignment to group is removed for role \"{0}\" By \"{1}\"";
        public const string UIAuditLog_Configuration_DefaultTabLoad = "Default values of Tabs and Menus are loaded By \"{0}\"";

        public const string UIAuditLog_Configuration_ABAC_Added = "New Attribute Base Access Control \"{0}\" Added By \"{1}\"";
        public const string UIAuditLog_Configuration_ABAC_Edited = "Attribute Base Access Control \"{0}\" Edited By \"{1}\"";
        public const string UIAuditLog_Configuration_ABAC_Deleted = "Attribute Base Access Control \"{0}\" Deleted By \"{1}\""; // Control ID 
        public const string UIAuditLog_Configuration_ABACAssignment_AddedToUser = "ABAC \"{0}\" Assigned to user(s) \"{1}\" By \"{2}\"";
        public const string UIAuditLog_Configuration_ABACAssignment_RemovedForUser = "ABAC \"{0}\" Removed for user(s) \"{1}\" By \"{2}\"";
        public const string UIAuditLog_Configuration_ABACAssignment_AddedToGroup = "ABAC \"{0}\" Assigned to Group \"{1}\" By \"{2}\"";
        public const string UIAuditLog_Configuration_ABACAssignment_RemovedForGroup = "ABAC Assignment to group is removed for ABAC \"{0}\" By \"{1}\"";

        public const string UIAuditLog_Workflow_Workflow_Added = "New Workflow {0} Added By {1}";
        public const string UIAuditLog_Workflow_Workflow_Deleted = "Workflow {0} Deleted By {1}";
        public const string UIAuditLog_Workflow_Workflow_Edited = "Workflow {0} Edited By {1}";

        public const string UIAuditLog_Attestation_ReviewEndpoint_Saved = "Review ID {0}  of Attestation  {1}  Saved     By  {2}";
        public const string UIAuditLog_Attestation_ReviewEndpoint_Submitted = "Review ID {0}  of Attestation  {1}  Submitted By  {2}";
        public const string UIAuditLog_Attestation_ReviewEndpoint_Updated = "Review Of Attestation {0} Updated By {1}";

        public const string UIAuditLog_User_LogIn = "User {0} LoggedIn in to IACM Application.";
        public const string UIAuditLog_User_LogOut = "User {0} LogOut.";
        public const string UIAuditLog_ExceptionMessage = "<Message>{0}</Message><StackTrace>{1}</StackTrace>";

        #endregion
        #region FSP STATUS MESSAGES
        #region REEQUESTOR STATUS MESSAGES

        #endregion
        public const string PENDING_BO_APPROVAL = "Pending For Business Owner Approval";
        public const string PENDING_IT_APPROVAL = "Pending For IT Owner Approval";
        public const string PENDING_EXECUTION = "Execution Pending";
        public const string REJECTED_BY_BO = "Request Rejected By Business Owner";
        public const string REJECTED_BY_IT = "Request Rejected By IT Owner";
        public const string PENDING = "PENDING";
        public const string APPROVED = "APPROVED";
        public const string REJECTED = "REJECTED";
        public const string Executed = "EXECUTED";
        public const string CREATE = "CREATE";
        public const string UPDATE = "UPDATE";
        public const string DELETE = "DELETE";




        #endregion
        #region SAPROLE Types

        public const string COMPOSITE = "COMPOSITE";
        public const string SINGLE = "SINGLE";

        #endregion
        #region AD GROUP TYPE
        public const string DISTRIBUTION_GROUP = "Distribution Group";
        public const string SECURITY_GROUP = "Security Group";
        public const string GROUP_TYPE = "8";//dISTRIBUTION GROUP.
        #endregion
        #region IACM GroupType

        #endregion
        #region "Attestation Review Constants"

        public const string APPROVE = "Approve";
        public const string REVOKE = "Revoke";
        public const string FULL_ACCESS = "FullAccess";
        public const string DISABLED = "Disabled";
        public const string HIDDEN = "Hidden";

        #endregion
        #region Compliance
        #region Function Upload Constants
        public const int Compliance_FunctionUpload_Column_FunctionCode = 0;
        public const int Compliance_FunctionUpload_Column_FunctionDescription = 1;
        public const int Compliance_FunctionUpload_Column_EndpointSystemType = 1;
        public const int Compliance_FunctionUpload_Column_EndpointSystem = 2;
        public const int Compliance_FunctionUpload_Column_FunctionType = 3;
        public const int Compliance_FunctionUpload_Column_FunctionValue = 4;
        public const int Compliance_FunctionUpload_Column_ObjectCode = 5;
        public const int Compliance_FunctionUpload_Column_FieldCode = 6;
        public const int Compliance_FunctionUpload_Column_FieldValue = 7;
        public const int Compliance_FunctionUpload_Column_Reserved1 = 9;
        public const int Compliance_FunctionUpload_Column_Reserved2 = 10;
        public const int Compliance_FunctionUpload_Column_Reserved3 = 11;
        public const int Compliance_FunctionUpload_Column_Reserved4 = 12;
        public const int Compliance_FunctionUpload_Column_Reserved5 = 13;
        public const int Compliance_FunctionUpload_TotalColumns = 14;

        public const int Compliance_ExceptionUpload_Column_ExceptionCode = 0;
        public const int Compliance_ExceptionUpload_Column_ExceptionDescription = 1;
        public const int Compliance_ExceptionUpload_Column_RiskOwnerId = 2;
        public const int Compliance_ExceptionUpload_Column_UserName = 1;
        public const int Compliance_ExceptionUpload_Column_Function1 = 2;
        public const int Compliance_ExceptionUpload_Column_FuncItemId1 = 3;
        public const int Compliance_ExceptionUpload_Column_Function2 = 4;
        public const int Compliance_ExceptionUpload_Column_FuncItemId2 = 5;
        #endregion
        #endregion
        #region WindowsConnector
        #region objectSID Types

        public const string objectSID_Type_WellKnown = "WellKnowSID";
        public const string objectSID_Type_Domain = "DomainSID";
        public const string objectSID_Type_Local = "LocalSID";

        #endregion
        #endregion        
        #region Mail Templates
        public const string CREATE_ACCOUNT_NOTIFICATION_COMMON_TEMPLATE = "CreateAccountNotificationCommonTemplate";
        public const string CREATE_ACCOUNT_NOTIFICATION_Inhouse_TEMPLATE = "InHouseAccountCreation";//"CreateAccountNotificationInhouseTemplate";
        public const string LNSWelcomeMailTemplate = "LotusNotesWelcomeEmail";
        public const string DiscoveryJobFailed = "DiscoveryJobFailed";
        public const string OnboardingUserEmail = "OnboardingUserEmail";
        public const string PostDiscoveryJobValidation = "PostDiscoveryJobValidation";
        public const string NewAccountSendEmailToRequestBy = "NewAccountSendEmailToRequestByTemplate";
        // Needs to update in Mail Templetes
        public const string AdhocAmendment = "AdhocProvisioningAmendment";
        public const string AdhocUserRemovalMailTemplate = "AdhocProvisioningUserRemoval";
        public const string AdhocUserOffBoardingMailTemplate = "OffBoardingUserNotification";
        public const string AdhocApproverOffBoardingMailTemplate = "OffBoardingApprover";
        public const string OUChangeRequestMailTemplate = "OUChangeRequestNotification";

        public const string AlertNotificationMailTemplate = "AlertNotification";
        public const string FailureAccountNotificationMailTemplate = "FailureAccountNotification";
        public const string PasswordChangeNotification = "Password Change Notification";
        public const string AccessRequestFailureNotification = "AccessRequestFailureNotification";
        public const string AccessRequestRejected = "AccessRequestRejected";
        public const string AccessRequestWithdrawn = "RequestWithdrawn";
        public const string NotificationForNewUser = "NotificationForNewUser";
        public const string NotificationNPACIncomplete = "NotificationNPACIncomplete";
        public const string NotifyDeleteNonConnectAcc = "DeleteNonConnectedSystemNotification";
        public const string AttestationJobFailed = "AttestationJobFailed";
        public const string NotifyReferenceUserChange = "NotifyReferenceUserChange";
        public const string RemediationComplete = "RemediationComplete";
        public const string TriggerDeletedNotificationTemplate = "TriggerDeletedNotification";

        public const string BulkRequestRejected = "BulkRequestRejected";

        public const string BulkRequestWithdrawn = "BulkRequestWithdrawn";
        public const string EmployeeEventWithdrawn = "EmployeeEventWithdrawn";
        public const string NonEmployeeExpiryNotification = "NonEmployeeExpiryNotification";

        public const string RequestRejectAlertToApprovers = "RequestRejectAlertToApprovers";
        public const string BulkRequestRejectAlertToApprovers = "BulkRequestRejectAlertToApprovers";

        public const string NotifyOffice365Failed = "NotifyOffice365Failed";

        public const string PassOfficeNotification = "PassOfficeNotification";
        public const string PassOfficeOffBoardNotification = "PassOfficeOffBoardNotification";

        public const string Office365LicenseRemovalNotification = "Office365LicenseRemovalNotification";
        public const string taskStatusMessageWithdraw = "Task updated with Status";
        #endregion
        #region MailTriggerActions
        public const string LNSNotificationFailure = "Lotus Notes Provisioning";
        public const string ADSNotificationFailure = "Active Directory Provisioning";
        #endregion
        #region ApplicaitonName ForMSSL
        public const string MSSLMSP = "MSP (MSSL - ISU)";
        public const string MSSLMCP = "MCP (MSSL - CRM)";
        public const string MSSLBI = "RBP (MSSL-BI)";
        public const string MSSL_ISU_CREM = "MSSL ISU & CRM";
        public const string USERTYPE_EXTERNAL = "EXTERNAL";
        public const string OUTPUT_DEVICE = "LOCL";
        #endregion
        #region ApplicaitonName ForEBS
        public const string EBSISP = "ISP (EBS - ISU)";
        public const string EBSCRP = "CRP (EBS-CRM)";
        public const string EBSBI = "BIP (EBS-BI)";
        public const string EBS_ISU_CREM = "EBS ISU & CRM";
        #endregion
        #region PageNames
        public const string ACCESSREQUEST = "AdhocAccessRequest.aspx";
        public const string HRMSHostName = "HRMSHostName";
        #endregion
        #region ReqSource
        public const string AccessRequest = "Access Request";
        public const string BulkRequest = "Bulk Request";
        public const string NonEmployee = "Non Employee";
        public const string AttestationRemediation = "Attestation Remediation";
        public const string UndoEvent = "UndoEvent";
        public const string OffBoarding = "OffBoarding";
        public const string OUChange = "OUChange";
        public const string OnBoarding = "OnBoarding";
        public const string ReOrganisation = "ReOrganisation";
        public const string Propagation = "Propagation";
        public const string Rehire = "Rehire";
        #endregion
        #region ConfigurationKey
        public const string Authentication = "Authentication";
        public const string PasswordResetOption = "PasswordResetOption";
        #endregion
        #region PasswordManagement
        public const string IACM_DEFAULT_POLICY = "IACM_DEFAULT_POLICY";
        #endregion
        #region Discovery Objects
        #region SAP
        public const string EndpointObject_SAP_User = "User";
        public const string EndpointObject_SAP_Role = "Role";
        public const string EndpointObject_SAP_Group = "Group";
        public const string EndpointObject_SAP_Profile = "Profile";
        public const string EndpointObject_SAP_TCode = "T-Code";
        public const string EndpointObject_SAP_Transaction = "Transaction";
        public const string EndpointObject_SAP_Authorization = "Authorization";
        public const string EndpointObject_SAP_Parameter = "Parameter";
        public const string EndpointObject_SAP_Position = "Position";
        public const string EndpointObject_SAP_ComplianceTCode = "TCodes";
        public const string EndpointObject_SAP_ComplianceProfile = "Profiles";
        public const string EndpointObject_SAP_ComplianceRole = "Role";
        public const string EndpointObject_SAP_SectionMapping = "SectionMapping";
        public const string EndpointObject_CriticalTCodeReport = "CriticalTCodeReport";
        public const string EndpointObject_ConflictingRoleReport = "ConflictingRoleReport";
        #endregion
        #region AD
        public const string EndpointObject_AD_User = "User";
        public const string EndpointObject_AD_Group = "Group";
        public const string EndpointObject_AD_OU = "OU";
        public const string EndpointObject_AD_Computer = "Computer";
        public const string EndpointObject_AD_Printer = "Printer";
        public const string EndpointObject_AD_Folder = "Folder";
        #endregion
        #region GEMS
        public const string EndpointObject_GEMS_User = "User";
        public const string EndpointObject_GEMS_Role = "Role";
        #endregion
        #region LNS
        public const string EndpointObject_LNS_User = "User";
        public const string EndpointObject_LNS_Group = "Group";
        public const string EndpointObject_LNS_Certifier = "Certifier";
        #endregion
        #region SAP EP
        public const string EndpointObject_SAPEP_User = "User";
        public const string EndpointObject_SAPEP_Group = "Group";
        public const string EndpointObject_SAPEP_Role = "Role";
        #endregion
        #region NISOFT
        public const string EndpointObject_NISOFT_User = "User";
        public const string EndpointObject_NISOFT_Profile = "Profile";
        #endregion
        #region EX
        public const string EndpointObject_EX_User = "User";
        public const string EndpointObject_EX_Group = "Group";
        #endregion
        #region WIN
        public const string EndpointObject_WIN_User = "User";
        public const string EndpointObject_WIN_Group = "Group";
        public const string EndpointObject_WIN_Folder = "Folder";
        #endregion
        #region Email Constants
        public const string EmailIDFromAddress = "noreply@senokoenergy.com";
        public const string SGDomainName = "singaporepower.com.sg";
        #endregion
        #region NPAC
        public const string EndpointObject_NPAC_User = "User";
        #endregion
        #region NPAC
        public const string EndpointObject_SSIDS_User = "User";
        #endregion
        #region GTSS
        public const string EndpointObject_GTSS_User = "User";
        public const string EndpointObject_GTSS_Role = "Role";
        #endregion
        #region CMS
        public const string EndpointObject_CMS_User = "User";
        public const string EndpointObject_CMS_Role = "Role";
        #endregion
        #region IACM
        public const string EndpointObject_IACM_User = "User";
        public const string EndpointObject_IACM_Group = "Group";
        public const string EndpointObject_IACM_Role = "Role";
        #endregion
        #endregion
        #region BizCMS
        public const string EndpointObject_BizCMS_User = "User";
        public const string EndpointObject_BizCMS_Role = "Role";
        public const string EndpointObject_BizCMS_Function = "Function";
        public const string EndpointObject_BizCMS_PrivillegeAcctTrans = "PrivillegeAcctTrans";
        #endregion
        #region CSC
        public const string EndpointObject_CSC_User = "User";
        public const string EndpointObject_CSC_Role = "Role";
        public const string EndpointObject_CSC_Function = "Function";
        public const string EndpointObject_CSC_PrivillegeAcctTrans = "PrivillegeAcctTrans";
        #endregion         
        #region CSC
        public const string EndpointObject_LCS_User = "User";
        public const string EndpointObject_LCS_Role = "Role";
        public const string EndpointObject_LCS_Function = "Function";
        public const string EndpointObject_LCS_PrivillegeAcctTrans = "PrivillegeAcctTrans";
        #endregion
        #region Oracle EBS

        public const string EndpointObject_OEBS_HRMSUsers = "HRMS User";

        public const string EndpointObject_OEBS_Users = "User";
        public const string EndpointObject_OEBS_Roles = "Role";
        public const string EndpointObject_OEBS_Responsibilities = "Responsibility";

        public const string EndpointObject_OEBS_Menus = "Menu";
        public const string EndpointObject_OEBS_Submenus = "Sub Menu";


        //public const string EndpointObject_OEBS_Departments = "Department";

        #endregion
        #region LINUX

        public const string EndpointObject_LINUX_User = "User";
        public const string EndpointObject_LINUX_Group = "Group";
        public const string EndpointObject_LINUX_Mount = "Mount";

        #endregion
        #region MSSQL

        public const string EndpointObject_MSSQL_User = "User";
        public const string EndpointObject_MSSQL_Role = "Role";
        public const string EndpointObject_MSSQL_Login = "Login";
        public const string EndpointObject_MSSQL_Database = "Database";
        public const string EndpointObject_MSSQL_Schemainfo = "Schema";
        public const string EndpointObject_MSSQL_LoginDatabaseMaping = "LoginDatabaseMaping";
        public const string EndpointObject_MSSQL_DatabasePermission = "DatabasePermission";
        public const string EndpointObject_MSSQL_ServerPermission = "ServerPermission";
        public const string EndpointObject_MSSQL_UserRoleSchemaPermission = "UserRoleSchemaPermission";
        public const string EndpointObject_MSSQL_ServerRolePermission = "ServerRolePermission";
        public const string EndpointObject_MSSQL_DatabaseRolePermission = "DatabaseRolePermission";

        #endregion
        #region PeopleSoft

        public const string EndpointObject_PS_Roles = "Role";
        public static string EndpointObject_PS_Users = "User";
        public static string EndpointObject_PS_PermissionList = "Permission";
        public static string EndpointObject_PS_Menus = "Menu";
        public static string EndpointObject_PS_Portal = "Portal";
        public static string EndpointObject_PS_PSClass = "Role class";
        public static string EndpontObject_PS_AccessLog = "Access log";

        #endregion
        #region Oracle llg UM

        public const string EndpointObject_ORCLUM_Users = "User";
        public const string EndpointObject_ORCLUM_Roles = "Role";
        public const string EndpointObject_ORCLUM_TableSpace = "TableSpace";

        #endregion
        #region FileHRMS
        public const string EndpointObject_FILEHRMS_User = "User";
        public const string EndpointObject_FILEHRMS_Organisation = "Organisation";
        public const string EndpointObject_FILEHRMS_Designation = "Designation";
        #endregion
        #region RWS PeopleSoft

        public const string EndpointObject_PSBI_Roles = "Role";
        public static string EndpointObject_PSBI_Users = "User";


        #endregion
        #region BizCMS
        public const string EndpointObject_BIZCMS_User = "User";
        public const string EndpointObject_BIZCMS_ROLE = "Role";
        public const string EndpointObject_BIZCMS_FUNCTION = "Function";
        public const string EndpointObject_BIZCMS_PRIVILLEGEACCTTRANS = "PrivillegeAcctTrans";
        #endregion
        #region WMS
        public const string EndpointObject_WMS_User = "User";
        public const string EndpointObject_WMS_ROLE = "Role";
        public const string EndpointObject_WMS_FUNCTION = "Function";
        public const string EndpointObject_WMS_PRIVILLEGEACCTTRANS = "PrivillegeAcctTrans";
        #endregion
        #region CSC
        public const string EndpointObject_CSC_ROLE = "Role";
        public const string EndpointObject_CSC_FUNCTION = "Function";
        public const string EndpointObject_CSC_PRIVILLEGEACCTTRANS = "PrivillegeAcctTrans";
        #endregion
        #region LCS
        public const string EndpointObject_LCS_ROLE = "Role";
        public const string EndpointObject_LCS_FUNCTION = "Function";
        public const string EndpointObject_LCS_PRIVILLEGEACCTTRANS = "PrivillegeAcctTrans";
        #endregion
        #region Inhouse
        public const string EndpointObject_Inhouse_User = "User";
        public const string EndpointObject_Inhouse_ROLE = "Role";
        public const string EndpointObject_Inhouse_PERMISSION = "Permission";
        public const string EndpointObject_Inhouse_FUNCTION = "Function";
        public const string EndpointObject_Inhouse_PRIVILLEGEACCTTRANS = "PrivillegeAcctTrans";
        public const string SPLITTER_FUCNTIONS = "=>";
        #endregion
        #region RCS
        public const string EndpointObject_RCS_User = "User";
        public const string EndpointObject_RCS_ROLE = "Role";
        public const string EndpointObject_RCS_FUNCTION = "Function";
        public const string EndpointObject_RCS_PRIVILLEGEACCTTRANS = "PrivillegeAcctTrans";
        #endregion
        #region ESV
        public const string EndpointObject_ESV_User = "User";
        public const string EndpointObject_ESV_ROLE = "Role";
        public const string EndpointObject_ESV_FUNCTION = "Function";
        public const string EndpointObject_ESV_PRIVILLEGEACCTTRANS = "PrivillegeAcctTrans";
        #endregion
        #region RWS PeopleSoftHRMS
        public const string EndpointObject_HRMS_USER = "User";
        public const string EndpointObject_HRMS_OnBoard = "OnBoard";
        public const string EndpointObject_HRMS_OffBoard = "OffBoard";
        public const string EndpointObject_HRMS_OffBoardROS = "OffBoardROS";
        public const string EndpointObject_HRMS_Promotion = "Promotion";
        public const string EndpointObject_HRMS_Transfer = "Transfer";
        public const string EndpointObject_HRMS_OneTimeUser = "OneTimeUser";
        public const string EndpointObject_HRMS_CSEL = "CSEL";
        public const string EndpointObject_HRMS_WithDrawn = "WithDrawn";
        public const string EndpointObject_HRMS_UserProcessor = "ProcessUserProcessor";
        #endregion
        #region   RWS ISMS
        public const string EndpointObject_ISMS_User = "User";
        #endregion
        #region RWS PeopleSoftFINOrPROC     
        public const string EndpointObject_PLFINPROC_User2Role = "User2Role";
        public const string EndpointObject_PLFINPROC_Finance = "Finance";
        public const string EndpointObject_PLFINPROC_Procurement = "Procurement";
        public const string EndpointObject_PLFINPROC__BusinessUnitDesc = "BusinessUnitDesc";
        public const string EndpointObject_PLFINPROC__BankDesc = "BankDesc";
        public const string EndpointObject_PLFINPROC__PriviligedTransaction = "PriviligedTransaction";
        public const string EndpointObject_PLFINPROC_CC2Ou = "CC2OU";
        public const string EndpointObject_PLFINPROC_OU2Tagging = "OU2Tagging";
        public const string EndpointObject_PLFINPROC_Role2Function = "Role2Function";
        public const string EndpointObject_PLFINPROC_User2BU = "User2BusinessUnit";
        public const string EndpointObject_PLFINPROC_User2Bank = "User2BankAccount";

        #endregion
        #region PeopleSoftFBI
        public const string EPOBJ_PSFBI_RoleDesc = "RoleDesc";
        public static string EPOBJ_PSFBI_COSTCENTER = "COSTCENTER";
        public static string EPOBJ_PSFBI_User2Role = "User2Role";
        public static string EPOBJ_PSFBI_User2CC = "User2CostCenter";
        #endregion
        #region Office365
        public const string EndpointObject_Office365_User = "User";
        public const string EndpointObject_Office365_Group = "Group";
        public const string EndpointObject_Office365_Licenses = "Licenses";
        public const string EndpointObject_Office365_Policies = "Policies";
        public const string EndpointObject_Office365_MailBoxPermissions = "MailBox Permissions";
        public const string EndpointObject_Office365_MailBoxStatistics = "MailBox Statistics";
        public const string EndpointObject_Office365_RecipientPermissions = "Recipient Permissions";
        public const string RequestAdditionalInfoMailBox = "MailBox";
        public const string RequestAdditionalInfoLicense = "LicenseAssignment";
        public const string UserNotFound = "User Not Created in Office 365";
        #endregion
        #region Matrix Message
        public const string MATRIX_WHO_UPDATED_MESSAGE = "Last updated by the User {0} on {1:dd-MM-yyyy hh:mm:ss tt}";
        #endregion
        #region Lenel
        public const string EndpointObject_Lenel_User = "User";
        public const string EndpointObject_Lenel_AccessLevel = "Access Level";
        public const string EndpointObject_Lenel_Badges = "Badge";
        public const string EndpointObject_Lenel_Company = "Company";
        #endregion
        #region VPN
        public const string EndpointObject_VPN_User = "User";
        #endregion
        #region SFB
        public const string EndpointObject_SFB_User = "User";
        public const string EndpointObject_SFB_ResponseGroup = "ResponseGroup";
        public const string EndpointObject_SFB_UnAssignedNumberRange = "UnAssignedNumberRange";
        public const string NotifySkypeForBusinessNewResponseGroup = "NotifyForNewResponseGroup";
        public const long DontAllowSFBAccountCreation = -1;
        #endregion
        public const string AdvanceDelegationNotification = "AdvanceDelegation";
        public const string DelegationExpiredMessage = "Deactivated Expired Advance Delegation successfully";
        public const string DelegationNoRecords = "No Expired Delegation record found for Deactivation";
        public const string DelegationExpiredfailedMessage = "Error occured while Deactivating Expired Advance Delegation";
        public const string NPACSectionNOTFound = "NPACSectionNOTFound";
        #region AccessRequest Message
        public const string addToGridWarning = "Please click on 'Add' button before proceeding to Step 3";
        #endregion

        public const String MSSL_EBS_DEFAULT_PASSWORD= "@Bcd12345";
        public const string Report_Date_Format = "yyyyMMddHHmmss";

        public const string NotifyAttestationApproval = "NotifyAttestationApproval";
        public const string AttestationException = "AttestationException";
        public const string NotifyAttestationWithDrawn = "NotifyAttestationWithDrawn";
        public const string DateFormatForSP = "dd-MM-yyyy hh:mm tt";

        public const string DiscoverSpecificUserFailed = "DiscoverSpecificUserFailed";
        public const string ADReconcilationNotification = "ADReconcilationNotification";
        public const int MaxHour = 23;
        public const int MaxMinutes = 59;
        public const int MaxSeconds = 59;

        public const string EBS = "EBS";
        public const string MSSL = "MSSL";
    }
    public class ColorConstants
    {
        public const string RED = "#c20000";
        public const string YELLOW = "#ffc700";
        public const string GREEN = "#8dcb2a";
        public const string BLUE = "#00adcc";
    }
    public class OrganaisationScopes
    {
        public const string ORG_COMPANY = "0006";
        public const string ORG_CLUSTER = "0016";
        public const string ORG_GROUP = "0014";
        public const string ORG_DEPARTMENT = "0008";
        public const string ORG_SECTION = "0010";
    }
    
}
