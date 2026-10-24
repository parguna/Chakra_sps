namespace DI.Chakra.Infrastructure
{
    public class ProvisioningConstants
    {
        #region Provisioning Engine
        public const string MSG_ProvisioningEngineStarted = "Provisioning Engine execution Started On {0}.";
        public const string MSG_ProvisioningEngineStopped = "Provisioning Engine execution Stop at {0}.";
        public const string MSG_ProvisioningEngineTotalRequestExecuted = "Total Access Request Executed: {0}.";
        public const string MSG_ProvisioningEngineTotalRequestToExecute = "Total Access Request To Execute: {0}.";
        public const string MSG_AccessRequestDependantRoleNotExist = "Access Request Cannot be processed. The Requested Role {0} is dependent on Role Id-{1} and User doesn’t have Role Id-{1}.";
        public const string MSG_AccessRequestDependantRoleDelete = "Access Request Cannot be processed. Cannot delete The Role \"{0}\", this is dependent Role for Role \"{1}\" of User.";
        #endregion
        #region Role, Policy and Form Not Production Message
        public const string MSG_RoleNotInProduction = "The Provisioning Role \"{0}\" is not in production mode which used in AccessRequestId  \"{1}\".";
        public const string MSG_PolicyNotInProduction = "The Provisioning Policy \"{0}\" is not in production mode which used by Provisioning Role \"{1}\".";
        public const string MSG_FormNotInProduction = "The Provisioning Form \"{0}\" is not in production mode which used by Provisioning Role \"{1}\".";
        #endregion
        #region Role, Policy and Form Null Value Message
        public const string MSG_RoleNull = "No Role is attached to AccessRequestId \"{0}\".";
        public const string MSG_PolicyNull = "No Policy is attached to Role \"{0}\".";
        public const string MSG_FormNull = "No Form(s) is attached to Role \"{0}\".";
        public const string MSG_FormUserNull = "User is NULL for Provisioning Form \"{0}\".";
        #endregion
        #region Thread MSG
        public const string MSG_ThreadStarted = "New Thread Started for AccessRequest ID: {0}.";
        public const string MSG_ThreadCompleted = "Thread completed for AccessRequest ID: {0}.";
        public const string MSG_FormCompletedThreadStatusChange = "Thread Progress {0}% : AccessRequestID = {1}, Provisioning FormName = {2}, Provisioning Form No = {3} Of {4} .";

        public const string Thread_SemaphoreName = "ProvisioningSemaphoreLNS";
        public const int Thread_MaxTimeToWait = 600000;
        #endregion
        #region General MSG
        public const string ProcessName = "System";
        public const string GroupAtrbName = "GROUP";
        public const string User_Remarks3_Non_Employee = "Non Employee";
        public const string MSG_ExceptionThrown = "Exception thrown by method \"{0}\", exception message: \"{1}\"";
        #endregion
        #region AD Forms MSG
        public const string MSG_ProvisioningADUserCreated = "New User \"{0}\" is Created in Active Directory - ID:{1}.";
        public const string MSG_ProvisioningADUserCreationFailed = "AD New User Creation is failed for user \"{0}\" in Active Directory - ID:{1}.";
        public const string MSG_ProvisioningADUserDeleted = "User \"{0}\" is Disabled from Active Directory - ID:{1}.";
        public const string MSG_ProvisioningADUserDeletionFailed = "User Disable is failed for user \"{0}\" from Active Directory - ID:{1}.";
        public const string MSG_ProvisioningADUserAddedInToGroup = "User \"{0}\" is added in AD Groups \"{1}\" in Active Directory - ID:{2}.";
        public const string MSG_ProvisioningADUserAddedInToGroupFailed = "User AD group assignment is failed for User {0}, Groups \"{1}\" in Active Directory - ID:{2}.";
        public const string MSG_ProvisioningADUserRemovedFromGroup = "User \"{0}\" is Removed from Entitlements \"{1}\" in Endpointsystem - ID:{2}.";
        public const string MSG_ProvisioningADUserRemoveFromGroupFailed = "User Entitlement Removal is failed for User {0}, Groups \"{1}\" in Endpointsystem - ID:{2}.";

        #endregion        
        #region MSSQL Forms MSG
        public const string MSG_ProvisioningMSSQLUserCreated = "New User \"{0}\" is Created in MSSQL - ID:{1}.";
        public const string MSG_ProvisioningMSSQLUserCreationFailed = "MSSQL New User Creation is failed for user \"{0}\" in MSSQL - ID:{1}.";
        public const string MSG_ProvisioningMSSQLUserDeleted = "User \"{0}\" is Disabled from MSSQL - ID:{1}.";
        public const string MSG_ProvisioningMSSQLUserDeletionFailed = "User Disable is failed for user \"{0}\" from MSSQL - ID:{1}.";
        public const string MSG_ProvisioningMSSQLUserAddedInToRoles = "User \"{0}\" is added inMSSQL ROLES \"{1}\" in MSSQL - ID:{2}.";
        public const string MSG_ProvisioningMSSQLUserAddedInToRolesFailed = "User MSSQL group assignment is failed for User {0}, ROLES \"{1}\" in MSSQL - ID:{2}.";
        public const string MSG_ProvisioningMSSQLUserRemovedFromRoles = "User \"{0}\" is Removed from MSSQL ROLES \"{1}\" in MSSQL- ID:{2}.";
        public const string MSG_ProvisioningMSSQLUserRemoveFromRolesFailed = "User MSSQL ROLES Removal is failed for User {0}, Groups \"{1}\" in MSSQL - ID:{2}.";

        #endregion             
        #region SAP Forms MSG
        public const string MSG_ProvisioningSAPUserCreated = "New User \"{0}\" is Created in SAP Server - ID:{1}.";
        public const string MSG_ProvisioningSAPUserCreationFailed = "SAP New User Creation is failed for user \"{0}\" in SAP Server - ID:{1}.";
        public const string MSG_ProvisioningSAPUserDeleted = "User \"{0}\" is Deleted from SAP Server - ID:{1}.";
        public const string MSG_ProvisioningSAPUserDeletionFailed = "User Deletion is failed for user \"{0}\" from SAP Server - ID:{1}.";
        public const string MSG_ProvisioningSAPUserAddedInToGroup = "User \"{0}\" is added in SAP Groups \"{1}\" in SAP Server - ID:{2}.";
        public const string MSG_ProvisioningSAPUserAddedInToGroupFailed = "User SAP group assignment is failed for User {0}, Groups \"{1}\" in SAP Server - ID:{2}.";
        public const string MSG_ProvisioningSAPUserRemovedFromRoles = "User \"{0}\" is Removed from SAP Roles \"{1}\" in SAP Server - ID:{2}.";
        public const string MSG_ProvisioningSAPUserRemoveFromRoleFailed = "User SAP Role Removal is failed for User {0}, Roles \"{1}\" in SAP Server - ID:{2}.";
        public const string MSG_ProvisioningSAPUserRoleAssigned = "SAP User \"{0}\" is added in SAP Roles \"{1}\" in SAP Server - ID:{2}.";
        public const string MSG_ProvisioningSAPUserRoleAssignmentFailed = "User SAP Role assignment is failed for User {0}, Roles \"{1}\" in SAP Server - ID:{2}.";
        public const string SAPDateFormater = "yyyy.MM.dd";
        #endregion
        #region SAP Forms MSG
        public const string MSG_ProvisioningSAPEPUserCreated = "New User \"{0}\" is Created in SAPEP Server - ID:{1}.";
        public const string MSG_ProvisioningSAPEPUserCreationFailed = "SAPEP New User Creation is failed for user \"{0}\" in SAPEP Server - ID:{1}.";
        public const string MSG_ProvisioningSAPEPUserRoleAssigned = "SAPEP User \"{0}\" is added in SAP Roles/Group  \"{1}\" in SAPEP Server - ID:{2}.";
        public const string MSG_ProvisioningSAPEPUserRoleAssignmentFailed = "User SAPEP Role and Group assignment is failed for User {0}, Roles/Group  \"{1}\" in SAPEP Server - ID:{2}.";
        public const string MSG_ProvisioningSAPEPUserDeleted = "User \"{0}\" is Deleted from SAPEP Server - ID:{1}.";
        public const string MSG_ProvisioningSAPEPUserDeletionFailed = "User Deletion is failed for user \"{0}\" from SAPEP Server - ID:{1}.";
        public const string MSG_ProvisioningSAPEPUserRemovedFromRolesAndGroups = "User \"{0}\" is Removed from SAP Roles/Group \"{1}\" in SAPEP Server - ID:{2}.";
        public const string MSG_ProvisioningSAPEPUserRemoveFromRolesAndGroupsFailed = "User SAP Role/Group Removal is failed for User {0}, Roles/Groups \"{1}\" in SAPEP Server - ID:{2}.";
        #endregion
        #region LotusNotes MSG
        public const string MSG_ProvisioningLNUserCreated = "New User \"{0}\" is Created in Lotus Note - ID:{1}.";
        public const string MSG_ProvisioningLNUserCreationFailed = "Lotus Note New User Creation is failed for user \"{0}\" in Lotus Note - ID:{1}.";
        public const string MSG_ProvisioningLNUserAddedInToGroupFailed = "User Lotus Note group assignment is failed for User {0}, Groups \"{1}\" in Lotus Note - ID:{2}.";
        public const string MSG_ProvisioningLNUserAddedInToGroup = "User \"{0}\" is added in Lotus Note Groups \"{1}\" in Lotus Note - ID:{2}.";
        public const string MSG_ProvisioningLNUserDefaultInfoNull = "Lotus Note New User Default info is null for user \"{0}\" in Lotus Note - ID:{1}.";

        public const string MSG_ProvisioningLNUserDeleted = "User \"{0}\" is Deleted from Lotus Note - ID:{1}.";
        public const string MSG_ProvisioningLNUserDeletionFailed = "User Deletion is failed for user \"{0}\" from Lotus Note - ID:{1}.";
        public const string MSG_ProvisioningLNUserRemovedFromGroup = "User \"{0}\" is Removed from Lotus Note Groups \"{1}\" in Lotus Note - ID:{2}.";
        public const string MSG_ProvisioningLNUserRemoveFromGroupFailed = "User Lotus Note group Removal is failed for User {0}, Groups \"{1}\" in Lotus Note - ID:{2}.";
        #endregion      
        #region Exchnage MSG
        public const string MSG_ProvisioningExUserCreated = "New User \"{0}\" is Created in Exchange Server - ID:{1}.";
        public const string MSG_ProvisioningExUserCreationFailed = "New User Creation is failed for user \"{0}\" in Exchange Server - ID:{1}.";
        public const string MSG_ProvisioningExUserAddedInToGroupFailed = "User Distribution group assignment is failed for User {0}, Groups \"{1}\" in Exchange Server - ID:{2}.";
        public const string MSG_ProvisioningExUserAddedInToGroup = "User \"{0}\" is added to Distrubution Groups \"{1}\" in Exchange Server - ID:{2}.";
        public const string MSG_ProvisioningExUserDefaultInfoNull = "New User Default info is null for user \"{0}\" in Exchange Server - ID:{1}.";

        public const string MSG_ProvisioningExUserDeleted = "User \"{0}\" is Deleted from Exchange Server - ID:{1}.";
        public const string MSG_ProvisioningExUserDeletionFailed = "User Deletion is failed for user \"{0}\" from Exchange Server- ID:{1}.";
        public const string MSG_ProvisioningExUserRemovedFromGroup = "User \"{0}\" is Removed from Lotus Note Groups \"{1}\" in Exchange Server - ID:{2}.";
        public const string MSG_ProvisioningExUserRemoveFromGroupFailed = "User Distribution group Removal is failed for User {0}, Groups \"{1}\" in Exchange Server - ID:{2}.";
        #endregion
        #region Linux Forms MSG
        public const string MSG_ProvisioningLinuxUserCreated = "New User \"{0}\" is Created in Linux S - ID:{1}.";
        public const string MSG_ProvisioningLinuxUserCreationFailed = "Linux New User Creation is failed for user \"{0}\" in Linux - ID:{1}.";
        public const string MSG_ProvisioningLinuxUserDeleted = "User \"{0}\" is Deleted from Linux - ID:{1}.";
        public const string MSG_ProvisioningLinuxUserDeletionFailed = "User Deletion is failed for user \"{0}\" from Linux  - ID:{1}.";
        public const string MSG_ProvisioningLinuxUserAddedInToGroup = "User \"{0}\" is added in Linux Groups \"{1}\" in Linux  - ID:{2}.";
        public const string MSG_ProvisioningLinuxUserAddedInToGroupFailed = "User Linux group assignment is failed for User {0}, Groups \"{1}\" in Linux  - ID:{2}.";
        public const string MSG_ProvisioningLinuxUserRemovedFromRoles = "User \"{0}\" is Removed from Linux Roles \"{1}\" in Linux Server - ID:{2}.";
        public const string MSG_ProvisioningLinuxUserRemoveFromRoleFailed = "User Linux Role Removal is failed for User {0}, Roles \"{1}\" in Linux  - ID:{2}.";
        public const string MSG_ProvisioningLinuxUserRoleAssigned = "Linux User \"{0}\" is added in Linux Roles \"{1}\" in Linux Server - ID:{2}.";
        public const string MSG_ProvisioningLinuxUserRoleAssignmentFailed = "User Linux Role assignment is failed for User {0}, Roles \"{1}\" in Linux  - ID:{2}.";
        // public const string LinuxDateFormater = "{0}.{1}.{2}";
        #endregion
        #region General
        public const string PostActivityXMLAtrb_ADGroupName = "ADGroupName";
        public const string PostActivityXMLAtrb_DefaultPrinterForSAP = "DefaultPrinterForSAP";

        public const string MasterUser_UserInformation_Updated_MSG = "User information updated into endpoints as changes found in trusted source for User - \"{0}\", EmployeeID - \"{1}\"";
        public const string MasterUser_UserDisabled_MSG = "User \"{0}\", EmployeeID \"{1}\" Disabled as user termination date is \"{2}\"";
        public const string MasterUser_UserDeleted_MSG = "User \"{0}\", EmployeeID \"{1}\" Deleted after \"{2}\" days of termination";

        public const string UserDisplayNameFormatter = "{0} {1}";
        #endregion
        #region Email Templates
        public const string EmailTemplate_AutoProvisioning_Admin = "AutoProvisioning-Admin";
        public const string EmailTemplate_AutoProvisioning_HR = "AutoProvisioning-HR";
        public const string EmailTemplate_AutoProvisioning_HelpDesk = "AutoProvisioning-HelpDesk";

        public const string EmailTemplate_EndPtOwner_NewUser = "NewUser-EndPtOwner";
        public const string EmailTemplate_EndPtOwner_ExistingUser = "ExistingUser-EndPtOwner";

        public const string EmailTemplate_EndPtOwner_ToDisableSAPEPUser = "EmailToDisableSAPEPUser";
        public const string EmailTemplate_EndPtOwner_AfterDisablingUser = "EmailAfterDisablingUser";
        public const string EmailTemplate_EndPtOwner_AfterDeletingUser = "EmailAfterDeletingUser";

        public const string Email_SentSuccessfullyMsg = "Email sent successfully to \"{0}\", Subject:\"{1}\"";
        public const string Email_ErrorOnSendingMsg = "Error occur while sending email to \"{0}\", Subject: \"{1}\"";
        #endregion
        #region User Remarks1 Values
        public const string User_Remarks1_New = "New";
        public const string User_Remarks1_Modified = "Modified";
        public const string User_Remarks1_Modifying = "Modifying";
        public const string User_Remarks1_Terminated = "Terminated";
        public const string User_Remarks1_Terminating = "Terminating";
        public const string User_Remarks1_Deleted = "Deleted";
        public const string User_Remarks1_Deleting = "Deleting";
        public const string User_Remarks1_Done = "Done";
        #endregion
        #region Auto Provisioning / Post auto Provisioning
        public const string MSG_PostAutoProvisioningNotFound = "Post auto Provisioning not found";
        public const string MSG_PostAutoProvisioningAccessRequestSaveFailed = "Post Auto Provisioning AccessRequest save failed for User id: {0}";
        public const string MSG_PostAutoProvisioningAccessRequestSaveSuccess = "Post Auto Provisioning AccessRequest created for User id: {0}";
        public const string MSG_PostAutoProvisioningARDesc = "This request created automatically after Auto Provisioning as a Post activity.";
        public const string MSG_PostAutoProvisioningUsrNull = "User is NULL for Post auto Provisioning Access Request.";
        #endregion
        #region OracleUM MSG
        public const string MSG_ProvisioningOracleUserCreated = "New User \"{0}\" is Created in Oracle - ID:{1}.";
        public const string MSG_ProvisioningOracleUserCreationFailed = "Oracle New User Creation is failed for user \"{0}\" in Oracle - ID:{1}.";
        public const string MSG_ProvisioningOracleUserAddedInToRoleFailed = "User Oracle role assignment is failed for User {0}, Roles \"{1}\" in Oracle - ID:{2}.";
        public const string MSG_ProvisioningOracleUserAddedInToRole = "User \"{0}\" is added in Oracle Roles \"{1}\" in Oracle - ID:{2}.";
        public const string MSG_ProvisioningOracleUserDefaultInfoNull = "Oracle New User Default info is null for user \"{0}\" in Oracle - ID:{1}.";

        public const string MSG_ProvisioningOracleUserDeleted = "User \"{0}\" is Deleted from Oracle - ID:{1}.";
        public const string MSG_ProvisioningOracleUserDeletionFailed = "User Deletion is failed for user \"{0}\" from Oracle - ID:{1}.";
        public const string MSG_ProvisioningOracleUserRemovedFromRole = "User \"{0}\" is Removed from Oracle Roles \"{1}\" in Oracle - ID:{2}.";
        public const string MSG_ProvisioningOracleUserRemoveFromRoleFailed = "User Oracle role Removal is failed for User {0}, Roles \"{1}\" in Oracle - ID:{2}.";
        #endregion
        #region Peoplesoft Forms MSG
        public const string MSG_ProvisioningPSUserCreated = "New User \"{0}\" is Created in Peoplesoft - ID:{1}.";
        public const string MSG_ProvisioningPSUserCreationFailed = "AD New User Creation is failed for user \"{0}\" in Peoplesoft - ID:{1}.";
        public const string MSG_ProvisioningPSUserDeleted = "User \"{0}\" is Disabled from Peoplesoft - ID:{1}.";
        public const string MSG_ProvisioningPSUserDeletionFailed = "User Disable is failed for user \"{0}\" from Peoplesoft - ID:{1}.";
        public const string MSG_ProvisioningPSUserAddedInToGroup = "User \"{0}\" is added in Peoplesoft Roles \"{1}\" in Peoplesoft - ID:{2}.";
        public const string MSG_ProvisioningPSUserAddedInToGroupFailed = "User Peoplesoft group assignment is failed for User {0}, Groups \"{1}\" in Peoplesoft - ID:{2}.";
        public const string MSG_ProvisioningPSUserRemovedFromGroup = "User \"{0}\" is Removed from AD Groups \"{1}\" in Peoplesoft - ID:{2}.";
        public const string MSG_ProvisioningPSUserRemoveFromGroupFailed = "User Peoplesoft role Removal is failed for User {0}, Roles \"{1}\" in Peoplesoft - ID:{2}.";

        #endregion
        #region GEMS Forms MSG
        public const string MSG_ProvisioningGEMSUserCreated = "New User \"{0}\" is Created in GEMS Server - ID:{1}.";
        public const string MSG_ProvisioningGEMSUserCreationFailed = "GEMS New User Creation is failed for user \"{0}\" in GEMS Server - ID:{1}.";
        public const string MSG_ProvisioningGEMSUserDeleted = "User \"{0}\" is Deleted from GEMS Server - ID:{1}.";
        public const string MSG_ProvisioningGEMSUserDeletionFailed = "User Deletion is failed for user \"{0}\" from GEMS Server - ID:{1}.";
        public const string MSG_ProvisioningGEMSUserRemovedFromRoles = "User \"{0}\" is Removed from GEMS Roles \"{1}\" in GEMS Server - ID:{2}.";
        public const string MSG_ProvisioningGEMSUserRemoveFromRoleFailed = "User GEMS Role Removal is failed for User {0}, Roles \"{1}\" in GEMS Server - ID:{2}.";
        public const string MSG_ProvisioningGEMSUserRoleAssigned = "GEMS User \"{0}\" is added in GEMS Roles \"{1}\" in GEMS Server - ID:{2}.";
        public const string MSG_ProvisioningGEMSUserRoleAssignmentFailed = "User GEMS Role assignment is failed for User {0}, Roles \"{1}\" in GEMS Server - ID:{2}.";
        public const string GEMSDateFormater = "{0}.{1}.{2}";
        public const string MSG_ProvisioningGEMSUserADGroupAssignFailed = "GEMS New User Creation for user \"{0}\" is failed due to  unavail of AD account in SP active directory.";
        #endregion
        #region NPAC Forms MSG
        public const string MSG_ProvisioningNPACUserCreated = "New User \"{0}\" is Created in NPAC Server - ID:{1}.";
        public const string MSG_ProvisioningNPACUserCreationFailed = "NPAC New User Creation is failed for user \"{0}\" in NPAC Server - ID:{1}.";
        public const string MSG_ProvisioningNPACUserDeleted = "User \"{0}\" is Deleted from NPAC Server - ID:{1}.";
        public const string MSG_ProvisioningNPACUserDeletionFailed = "User Deletion is failed for user \"{0}\" from NPAC Server - ID:{1}.";
        public const string MSG_ProvisioningNPACUserLevelRemoved = "NPAC Level \"{1}\" Removed for User \"{0}\" in NPAC Server - ID:{2}.";
        public const string MSG_ProvisioningNPACUserLevelRemoveFailed = "User NPAC Level Removal is failed for User {0}, Level \"{1}\" in NPAC Server - ID:{2}.";
        public const string MSG_ProvisioningNPACUserLevelAssigned = "NPAC Level \"{0}\" added for NPAC User \"{1}\" in NPAC Server - ID:{2}.";
        public const string MSG_ProvisioningNPACUserLevelAssignmentFailed = "User NPAC Level assignment is failed for User {0}, Level \"{1}\" in NPAC Server - ID:{2}.";
        public const string NPACDateFormater = "{0}.{1}.{2}";
        #endregion
        #region OneCard Forms MSG
        public const string MSG_ProvisioningSMSUserCreated = "New User \"{0}\" is Created in OneCard - ID:{1}.";
        public const string MSG_ProvisioningSMSUserCreationFailed = "New User Creation is failed for user \"{0}\" in OneCard - ID:{1}.";
        public const string MSG_ProvisioningSMSUserDeleted = "User \"{0}\" is disabled in OneCard - ID:{1}.";
        public const string MSG_ProvisioningSMSUserDeletionFailed = "User disable is failed for user \"{0}\" in OneCard - ID:{1}.";
        public const string MSG_ProvisioningSMSUserAddedInToGroup = "User \"{0}\" is added in OneCard Groups \"{1}\" in OneCard - ID:{2}.";
        public const string MSG_ProvisioningSMSUserAddedInToGroupFailed = "User OneCard group assignment is failed for User {0}, Groups \"{1}\" in OneCard - ID:{2}.";
        public const string MSG_ProvisioningSMSUserRemovedFromGroup = "User \"{0}\" is Removed from Groups \"{1}\" in OneCard - ID:{2}.";
        public const string MSG_ProvisioningSMSUserRemoveFromGroupFailed = "User group Removal is failed for User {0}, Groups \"{1}\" in OneCard - ID:{2}.";

        #endregion
        #region SSIDS Forms MSG
        public const string MSG_ProvisioningSSIDSUserCreated = "New User \"{0}\" is Created in SSIDS Server - ID:{1}.";
        public const string MSG_ProvisioningSSIDSUserCreationFailed = "SSIDS New User Creation is failed for user \"{0}\" in SSIDS Server - ID:{1}.";
        public const string MSG_ProvisioningSSIDSUserDeleted = "User \"{0}\" is Deleted from SSIDS Server - ID:{1}.";
        public const string MSG_ProvisioningSSIDSUserDeletionFailed = "User Deletion is failed for user \"{0}\" from SSIDS Server - ID:{1}.";
        public const string MSG_ProvisioningSSIDSUserLevelRemoved = "SSIDS {3} \"{1}\" Removed for User \"{0}\" in SSIDS Server - ID:{2}.";
        public const string MSG_ProvisioningSSIDSUserLevelRemoveFailed = "User SSIDS {3} Removal is failed for User {0}, Level \"{1}\" in SSIDS Server - ID:{2}.";
        public const string MSG_ProvisioningSSIDSUserLevelAssigned = "SSIDS {3} \"{0}\" added for SSIDS User \"{1}\" in SSIDS Server - ID:{2}.";
        public const string MSG_ProvisioningSSIDSUserLevelAssignmentFailed = "User SSIDS {3} assignment is failed for User {0}, Level \"{1}\" in SSIDS Server - ID:{2}.";

        #endregion

        public const string PasswordMessage = "Please use your {0} password to login and ignore the change password instructions below.";
        public const string ReorgCreatedBy = "ProvisioningService";
    }
}
