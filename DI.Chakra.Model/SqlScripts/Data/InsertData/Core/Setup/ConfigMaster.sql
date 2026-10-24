TRUNCATE TABLE [dbo].[DI_M_ConfigurationMaster]
GO

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('OnBoardTriggers', '01', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('OffBoardTriggers', '10', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('TransferTriggers', '06', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('ReOrgTriggers', '03', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('ExecutiveEmployeeFlags', '0,1', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('ActiveEmployeeFlags', '3', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('ExecutiveEmployeeADGroups', 'InternetUsers', 1)



  INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
  VALUES ('ADSPServiceAccountOU', 'OU=Users,OU=TEST,DC=idm,DC=local', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('InternetUsersADGroup', 'InternetUsers', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('MSPDefaultProfile', 'Z_MSQ100', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('MCPDefaultProfile', 'Z_MSPCLNT100', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('CIOFlag', 'Chief Information Officer', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('ExcludedCostCenters', 'ENQY,ICCT,INSN,COFF,TURN,FLDS,REMR', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('DefaultMailQuota', '80', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('SharedAccountMailQuota', '300', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('MailQuotaThreshold', '10', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('UserGroup', '3', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('UserRole', '2', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('DeleteUserAfterNoOfDays', '30', 1)


INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], CreatedDate, LastModifiedDate, [IsUiDisplay] )
VALUES ('HouseKeepingNumberOfDaysTasks', '90', getdate(), getdate(), 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue],  CreatedDate, LastModifiedDate, [IsUiDisplay])
VALUES ('HouseKeepingNumberOfDaysAttestation', '90', getdate(), getdate(), 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue],  CreatedDate, LastModifiedDate, [IsUiDisplay])
VALUES ('HouseKeepingNumberOfDaysLog', '90',  getdate(), getdate(), 1)


  INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
  VALUES ('AccountCreatedNotificationTo', 'devteam@deepidentity.com', 1)

  INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
  VALUES ('AccountCreatedNotificationCC', 'devteam@deepidentity.com', 1)

  INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
  VALUES ('FailureNotificaionReceiver', 'devteam@deepidentity.com', 1)

  INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
  VALUES ('AlertNotificaionReceiver', 'devteam@deepidentity.com', 1)

  INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
  VALUES ('LNSServiceAccountCertifier', 'IT-ABCCorp.id', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('WintelGroupId', '10', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('ITSecurityGroupId', '4', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('NonStaffGroup', 'NonStaff Group', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('LNSAdminUserIdPath', 'C:\IACM\EndpointSystems\notes\ID_FILE\lnsadmin.id', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('PropagationFields', 'KnownAs,TelephoneNumber,EmailId,OrgDepartment,OrgDepartment,OrgDivision,OrgBranch,OrgSection,Fax,TelephoneNumber,MobileNumber,PostalCode,PostalAddress,Country', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('IT_DepartmentCode', 'SP17', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('AD_DISABLED_OU', 'OU=DENY Access,DC=idm,DC=local', 1)


INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('XL_ADSPROVISIONINGMATRIX', 'CompanyCode,DepartmentCode,DivisionCode,OrganisationId,OrganisationName,CostCenterCode,CostDistributionCode,OU,DefaultGroups', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('XL_ADSCATALOG', 'Application,BusinessRole,Description,TechnicalRole,Level1Approver,Level1ApproverType,Level2Approver,Level2ApproverType,Level3Approver,Level3ApproverType', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('XL_LNSPROVISIONINGMATRIX', 'CompanyCode,DepartmentCode,DivisionCode,OrganisationId,OrganisationName,CostCenterCode,CostDistributionCode,MailServer,CertifierIdFile,CertifierOU,DefaultGroups,PolicyName,ReplicatSever', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('XL_LNSCATALOG', 'Application,BusinessRole,Description,TechnicalRole,Level1Approver,Level1ApproverType,Level2Approver,Level2ApproverType,Level3Approver,Level3ApproverType', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('XL_SAPROLECATALOG', 'Application,BusinessRole,Description,TechnicalRole,Level1Approver,Level1ApproverType,Level2Approver,Level2ApproverType,Level3Approver,Level3ApproverType', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('XL_SAPPROVISIONINGMATRIX', 'CompanyCode,CompanyName,DepartmentCode,DepartmentName,DivisionCode,DivisionName,CostCenterCode,CostCenterName,Role', 1)


INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('XL_INHOUSE_USERINFO', 'TenantId,InhouseSystemCode,NRIC,EmployeeId,Email,UserName,DisplayName,FirstName,LastName,AccountCreatedDate,LastLoginDate,AccountType,Remarks1,Remarks2,Remarks3,Remarks4,Remarks5,CreatedBy,CreatedDate,Permission,Version', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('XL_InhousePermissions', 'Id,TenantId,EndPointSystemID,InhouseSystemCode,Name,Description,IsActive,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('XL_VPNConfigurations', 'Application,Prod,Dev,QA', 1)


INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('XL_SAPEPGroupInfo',  'GroupId,GroupName,GroupDescription,GroupUniqueName,Version', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('XL_SAPEPRoleInfo',  'RoleId,RoleName,RoleDescription,UniqueRoleName,Version', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('OnBoardDaysBeforeJoining', '3', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('IACMLoginURL', 'http://10.153.3.108/IACM/login.aspx', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('EmployeeDefaultKey', 'T9', 1)


INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('DefaultPassword', 'di', 1)


INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('MSSLSAPPhoneTaskId', 'TS92000001', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('EBSSAPPhoneTaskId', 'TS92007927', 1)


INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('SkipWeekendsOnRunEveryDaySchedule', 'False', 1)


INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('HRMSDummyEmailId',  'HRMSDUMMY@deepidentity.com', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('EmergencyUserName',  '', 1)
INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('Co-ordinator Role', '4', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('LotusUserLicense',  '2', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('LotusShareLicense',  '', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('LotusServiceLicense',  '', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('LNSTempIdPath',  'C:\Temp', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('CanRunOnBoardTrigger',  '1', 0)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('CanRunOffBoardTrigger',  '1', 0)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('CanRunTransferTrigger',  '1', 0)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('CanRunReOrgTrigger',  '1', 0)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('CanRunPropagationTrigger',  '1', 0)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('RunAttestationRemediation',  '1', 0)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('ResetTempPassword',  '1', 0)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('BackDaysAllowedForHRScan',  '30', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('AuthenticationMode', 'IACM', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('PasswordResetOption', 'SecurityQA', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('NoofSecurityQuestion', '3', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('ApplicationCulture', 'en-US', 0)


INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('LotusUserIdFilePath', 'C:\IACM\EndpointSystems\notes\UserIds', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('LotusCertifierIdFilePath', 'C:\IACM\EndpointSystems\notes\CertIds', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('LotusCertifierPassword', 'password', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('OTPOption', 'SMS,Email', 1)

INSERT INTO [dbo].[DI_M_ConfigurationMaster]([ConfigKey], [ConfigValue], [IsUiDisplay])
VALUES ('PurgeTables', 'DI_M_PurgeConfig,DI_M_ComplianceProcessHistory', 1)

Update [dbo].[DI_M_ConfigurationMaster] Set CreatedDate=getdate(), CreatedBy = 'System'
GO