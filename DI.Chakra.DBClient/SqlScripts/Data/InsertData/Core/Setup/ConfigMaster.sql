-- IACM
TRUNCATE TABLE [dbo].[DI_M_IACMConfigurationMaster]
GO

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('OnBoardTriggers', '01',getdate(), getdate(), 1,'')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('OffBoardTriggers', '10',getdate(), getdate(), 1,'')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('TransferTriggers', '06',getdate(), getdate(), 1,'')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('ReOrgTriggers', '03',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ExecutiveEmployeeFlags', '0,1',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ActiveEmployeeFlags', '3',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('UserRole', '2',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue], CreatedDate, LastModifiedDate, [IsUiDisplay] ,[Description])
VALUES ('HouseKeepingNumberOfDaysTasks', '90', getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],  CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('HouseKeepingNumberOfDaysAttestation', '90', getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],  CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('HouseKeepingNumberOfDaysLog', '90',  getdate(), getdate(), 1,'')


  INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue], CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
  VALUES ('AccountCreatedNotificationTo', 'itidadmin@rwsentosa.com',getdate(), getdate(), 1,'')

  INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue], CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
  VALUES ('AccountCreatedNotificationCC', 'itidadmin@rwsentosa.com', getdate(), getdate(),1,'')

  INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue], CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
  VALUES ('FailureNotificaionReceiver', 'itidadmin@rwsentosa.com',getdate(), getdate(), 1,'')

  INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
  VALUES ('AlertNotificaionReceiver', 'itidadmin@rwsentosa.com',getdate(), getdate(), 1,'')

  --INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
  --VALUES ('LNSServiceAccountCertifier', 'IT-ABCCorp.id',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ITSecurityGroupId', '1', getdate(), getdate(),1,'')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('LNSAdminUserIdPath', 'C:\IACM\EndpointSystems\notes\Lotus_Id_File\admin.id',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('PropagationFields', 'KnownAs,TelephoneNumber,EmailId,OrgDepartment,OrgDepartment,OrgDivision,OrgBranch,OrgSection,Fax,TelephoneNumber,MobileNumber,PostalCode,PostalAddress,Country',getdate(), getdate(), 1,'')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('AD_DISABLED_OU', 'OU=DENY Access,DC=idm,DC=local', getdate(), getdate(),1,'')


INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('XL_VPNConfigurations', 'Application,Prod,Dev,QA',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('XL_SAPEPGroupInfo',  'GroupId,GroupName,GroupDescription,GroupUniqueName,Version', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('XL_SAPEPRoleInfo',  'RoleId,RoleName,RoleDescription,UniqueRoleName,Version',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OnBoardDaysBeforeJoining', '3',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('IACMLoginURL', 'http://192.168.15.76:90/login.aspx',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('Endpoint_Notification', 'devteam@deepidentity.com',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('EmployeeDefaultKey', 'T9',getdate(), getdate(), 1,'')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('LNSTempIdPath',  'C:\Temp',getdate(), getdate(), 1,'')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('CanRunOnBoardTrigger',  '1',getdate(), getdate(), 0,'Can Run OnBoard Trigger')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('CanRunRehireTrigger',  '1',getdate(), getdate(), 0,'Can Run Rehire Trigger')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('CanRunOffBoardTrigger',  '1',getdate(), getdate(), 0,'Can Run OffBoard Trigger')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('CanRunTransferTrigger',  '1',getdate(), getdate(), 0,'Can Run Transfer Trigger')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('CanRunReOrgTrigger',  '0',getdate(), getdate(), 0,'Can Run ReOrg Trigger')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('CanRunPropagationTrigger',  '0',getdate(), getdate(), 0,'Can Run Propagation Trigger')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('RunAttestationRemediation',  '1',getdate(), getdate(), 0,'Run Attestation Remediation')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('AuthenticationMode', 'IACM', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('AuthenticationDomain', '', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ApplicationCulture', 'en-US',getdate(), getdate(), 0,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('AttestationDefaultReviewStatus', '1', getdate(), getdate(),0,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('DefaultDomain', 'rws', getdate(), getdate(),1,'')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('LotusUserIdFilePath', 'C:\IACM\EndpointSystems\notes\LNSUserIds',getdate(), getdate(), 1,'')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('LotusCertifierIdFilePath', 'C:\IACM\EndpointSystems\notes\LNSCertifiers',getdate(), getdate(), 1,'')

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('LotusCertifierPassword', '@bcd1234',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('PurgeTables', 'DI_Log_ServiceLog,DI_Log_UI',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanConsiderDefaultMatrix',  '1',getdate(), getdate(), 1,'Can Consider Default Matrix')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanConsiderBestMatrixChooser',  '0',getdate(), getdate(), 1,'Can Consider Best Matrix Match')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('EmailApprovalLink',  'http://localhost:60471/ApproveTask.aspx',getdate(), getdate(), 1,'Link for Mail task Approval')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('WintelGroupId',  '10',getdate(), getdate(), 1,'Wintel Group Id')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('MaxWorkflowAttachmentsSupported',  '1',getdate(), getdate(), 1,'Maximum number of attachments supported')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ServiceUser',  'System',getdate(), getdate(), 1,'Updates through services')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OrgChartDepthSupported',  '3',getdate(), getdate(), 1,'Maximum organisation levels supported')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('MakeEPEntitlementAlwaysRequestable',  '1-1',getdate(), getdate(), 1,'Endpoint entitlement which can be requested again and again')


INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('DISCOVERY_SVCCONTROLLER',  'DIChakraDiscoveryService',getdate(), getdate(), 1,'Discovery Service Controller')
INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ATTESTATION_SVCCONTROLLER',  'DIChakraAttestationService',getdate(), getdate(), 1,'Attestation Service Controller')
INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('EMAIL_SVCCONTROLLER',  'DIChakraEmailingService ',getdate(), getdate(), 1,'Email Service Controller')
INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('MISC_SVCCONTROLLER',  'DIChakraMiscellaneousService',getdate(), getdate(), 1,'Misc Service Controller')


Update [dbo].[DI_M_IACMConfigurationMaster] Set CreatedBy = 'System'
gO

-- End

TRUNCATE TABLE [dbo].[DI_M_PMConfigurationMaster]
GO

-- Password Management
INSERT INTO [dbo].[DI_M_PMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('DefaultPassword', 'di', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_PMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('PasswordResetOption', 'SecurityQA',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_PMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('NoofSecurityQuestion', '3',getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_PMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OTPOption', 'SMS', getdate(), getdate(),1,'')

Update [dbo].[DI_M_PMConfigurationMaster] Set CreatedBy = 'System'
GO

-- End



TRUNCATE TABLE [dbo].[DI_M_IMConfigurationMaster]
GO

-- IM Configuration master


INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OnBoardTriggers', '01', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OffBoardTriggers', '10', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('TransferTriggers', '06', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ReOrgTriggers', '03', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('AD_DISABLED_OU', 'OU=DENY Access,DC=DII,DC=com', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunOnBoardTrigger', '1', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunRehireTrigger', '1', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunPromotionTrigger', '1', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunDemotionTrigger', '1', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunOffBoardTrigger', '1', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunOffBoard2Trigger', '1', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunOffBoard3Trigger', '1', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunTransferTrigger', '1', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunReOrgTrigger', '0', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunPropagationTrigger', '0', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('LotusUserIdFilePath', 'C:\LNS ID Files\UserIds', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('LotusCertifierIdFilePath', 'C:\LNS ID Files', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('LotusCertifierPassword', 'wwcorp', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('LNSTempIdPath', 'C:\LNS ID Files\temp', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('LNSAdminUserIdPath', 'C:\IACM\EndpointSystems\notes\Lotus_Id_File\admin.id',getdate(), getdate(), 1,'')

 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
  VALUES ('LNSServiceAccountCertifier', 'IT-ABCCorp.id',getdate(), getdate(), 1,'')

 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
  VALUES ('SpecialAccessRequestEnabled', '1',getdate(), getdate(), 1,'')

  INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('PROVISIONING_SVCCONTROLLER',  'DIChakraProvisioningService',getdate(), getdate(), 1,'Provisioning Service Controller')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],  CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('AttestationSnapshotFilePath', 'C:\IMAttestation\',  getdate(), getdate(), 1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('AttestationDefaultReviewStatus', '1', getdate(), getdate(),0,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('SGCCodeExcludedCompanyCode','IRS',GETDATE(),GETDATE(),'1','External Company to have different Company Code for Email')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('RTypeCompanyCode',  'RWS',getdate(), getdate(), 1,'R Type Company Code')

Update [dbo].[DI_M_IMConfigurationMaster] Set CreatedBy = 'System'


GO

-- End

--- RWS Configuration Related Script ---

 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ExtractionNoticeDays',  '7',getdate(), getdate(), 1,'Extraction Mail Notification Days')

 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ExtrationNoticeRemainderDays',  '3',getdate(), getdate(), 1,'Extraction Mail Notification Remainder Days')

 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('TransferMailNotificationWindowPeriod','5',GETDATE(),GETDATE(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('PromotionMailNotificationWindowPeriod','5',GETDATE(),GETDATE(),1,'')

 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('InterimStoreCleanUpDays','1',GETDATE(),GETDATE(),1,'')

 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('SupervisorGrade','001',GETDATE(),GETDATE(),'1','')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OnboardFutureWindowperiodDays','7',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OnboardBackDatedWindowPeriodDays','7',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('AdhocJobAssemblyPath','C:\RWS_Sprint2\SOURCE\DI.Chakra.AdhocJobs\bin\Debug\DI.Chakra.AdhocJobs.dll',GETDATE(),GETDATE(),'1','')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADResetPasswordDays','1',GETDATE(),GETDATE(),'1','')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADResetDefaultPassword','@bcd5678',GETDATE(),GETDATE(),'1','')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster] ([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description]) 
VALUES('ITCCTEAMGROUP','ITCCTEAM',GETDATE(),GETDATE(),'1','')


INSERT INTO [dbo].[DI_M_IMConfigurationMaster] ([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description]) 
VALUES('StandingOrder','StandingOrder',GETDATE(),GETDATE(),'1','')


INSERT INTO [dbo].[DI_M_IMConfigurationMaster] ([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description]) 
VALUES('RiskAssessment','RiskAssessment',GETDATE(),GETDATE(),'1','')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster] ([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description]) 
VALUES('EmailDomainName','rwsentosa.com',GETDATE(),GETDATE(),'1','')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster] ([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description]) 
VALUES('DefaultDomainCompany','Resorts World at Sentosa Pte Ltd',GETDATE(),GETDATE(),'1','')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OffBoardingRegularEmployeeHours','2',GETDATE(),GETDATE(),'1','')

 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OffBoardingRoasterEmployeeHours','2',GETDATE(),GETDATE(),'1','')

 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OffBoardingEmployeeDays','2',GETDATE(),GETDATE(),'1','')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunNonCSELGoldBarTrigger','0',GETDATE(),GETDATE(),'0','')

 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('AccountNotNeededForEndpoints','26',GETDATE(),GETDATE(),'1','Account Not Needed For EndpointTypes')

 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('EndpointNeedSplAccounts','1,19',GETDATE(),GETDATE(),1,'EndpointTypes need to have Special Account')

--- End ---

INSERT INTO DI_M_IMConfigurationMaster([ConfigKey], [ConfigValue],[Description],CreatedDate, LastModifiedDate, [IsUiDisplay]) VALUES('EthinicNamesToReplace',
'Mohammed,Mohammad,Mohamed,Mohamad,Muhammed,Muhammad,Muhamed,Muhamad,Mohd,Abdul,Abd,s/o,d/o,binti,bin,bte,Syed,Jnr,Junior,Snr,Senior,Jr,Sr',
'Malay and Other Names to replace with Empty string',
GETDATE(),GETDATE(),1)

INSERT INTO DI_M_IMConfigurationMaster([ConfigKey], [ConfigValue],[Description],CreatedDate, LastModifiedDate, [IsUiDisplay]) VALUES('ApplyOnlyonWholeWord',
'1', 'Apply Only on Whole Words like abdullah should noe replace with abdul',
GETDATE(),GETDATE(),1)


--INSERT INTO DI_M_NGrade (SetID,Grade,Description,CreatedDate,CreatedBy,LastModifiedBy,LastModifiedDate,SequenceNumber)
--VALUES('RWM-M5','M5','Mgr, Asst Mgr, Snr Mgr (RWM)',	GETDATE(),	'System',	'System',	GETDATE(),	1),
--('RWM-M4','M4','Mgr, Asst Dir (RWM)',	GETDATE(),	'System',	'System',	GETDATE(),	2),
--('RWM-M3','M3','Snr Mgr',	GETDATE(),	'System',	'System',	GETDATE(),	3),
--('RWM-M2','M2','Asst Dir',	GETDATE(),	'System',	'System',	GETDATE(),	4),
--('RWM-M1','M1','Dir, AVP (RWM)',	GETDATE(),	'System',	'System',	GETDATE(),	5),
--('RWM-SM5','SM5','Asst VP',	GETDATE(),	'System',	'System',	GETDATE(),	6),
--('RWM-SM4','SM4','VP',	GETDATE(),	'System',	'System',	GETDATE(),	7),
--('RWM-SM3','SM3','Snr VP, CFO',	GETDATE(),	'System',	'System',	GETDATE(),	8),
--('RWM-SM2','SM2','Exec VP',	GETDATE(),	'System',	'System',	GETDATE(),	9),
--('RWM-SM1','SM1','CEO, COO, President',	GETDATE(),	'System',	'System',	GETDATE(),	10)

INSERT INTO DI_M_IMConfigurationMaster([ConfigKey], [ConfigValue],[Description],CreatedDate, LastModifiedDate, [IsUiDisplay]) 
VALUES('MinimumSupervisorGradToApprove',
'M5',
'Minimum Supervisor Grade to approve'
,GETDATE(),GETDATE(),1)

INSERT INTO DI_M_NGradeReference (Grade,Sequence,CreatedDate,CreatedBy,LastModifiedBy,LastModifiedDate)
VALUES('M5',1,GETDATE(),'SYSTEM','SYSTEM',GETDATE()),('M4',2,GETDATE(),'SYSTEM','SYSTEM',GETDATE()),
('M3',3,GETDATE(),'SYSTEM','SYSTEM',GETDATE()),('M2',4,GETDATE(),'SYSTEM','SYSTEM',GETDATE()),
('M1',5,GETDATE(),'SYSTEM','SYSTEM',GETDATE()),('SM5',6,GETDATE(),'SYSTEM','SYSTEM',GETDATE()),
('SM4',7,GETDATE(),'SYSTEM','SYSTEM',GETDATE()),('SM3',8,GETDATE(),'SYSTEM','SYSTEM',GETDATE()),
('SM2',9,GETDATE(),'SYSTEM','SYSTEM',GETDATE()),('SM1',10,GETDATE(),'SYSTEM','SYSTEM',GETDATE())

INSERT INTO DI_M_IMConfigurationMaster([ConfigKey], [ConfigValue],[Description],CreatedDate, LastModifiedDate, [IsUiDisplay]) 
VALUES('UserExclusionEventTypeList',
'2,3,6,12,15,16',
'List of event types to display in drop down'
,GETDATE(),GETDATE(),0)

INSERT INTO DI_M_IMConfigurationMaster([ConfigKey], [ConfigValue],[Description],CreatedDate, LastModifiedDate, [IsUiDisplay]) 
VALUES('NotificationTypeProvisionActions',
'2,4,6',
'List of Actions for Provision type Notifications'
,GETDATE(),GETDATE(),0)

INSERT INTO DI_M_IMConfigurationMaster([ConfigKey], [ConfigValue],[Description],CreatedDate, LastModifiedDate, [IsUiDisplay]) 
VALUES('NotificationTypeNotificationActions',
'4',
'List of Actions for Notification type Notifications'
,GETDATE(),GETDATE(),0)

INSERT INTO DI_M_IMConfigurationMaster([ConfigKey], [ConfigValue],[Description],CreatedDate, LastModifiedDate, [IsUiDisplay]) 
VALUES('NotificationTypeExceptionActions',
'4',
'List of Actions for Exception type Notifications'
,GETDATE(),GETDATE(),0)



 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('NewJoineeFutureWindowperiodDays','7',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('NewJoineeBackDatedWindowPeriodDays','7',GETDATE(),GETDATE(),0,'')

 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ResignationFutureWindowperiodDays','7',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ResignationBackDatedWindowPeriodDays','7',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OUChangeFutureWindowperiodDays','7',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OUChangeBackDatedWindowPeriodDays','7',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('PromotionFutureWindowperiodDays','7',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('PromotionBackDatedWindowPeriodDays','7',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('NoneFutureWindowperiodDays','7',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('NoneBackDatedWindowPeriodDays','7',GETDATE(),GETDATE(),0,'')




 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('NewJoineeViewBackDatedWindowPeriodDays','30',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('NewJoineeViewFutureWindowperiodDays','14',GETDATE(),GETDATE(),0,'')



 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ResignationViewFutureWindowperiodDays','14',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ResignationViewBackDatedWindowPeriodDays','30',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('PromotionViewFutureWindowperiodDays','14',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('PromotionViewBackDatedWindowPeriodDays','30',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OUChangeViewFutureWindowperiodDays','14',GETDATE(),GETDATE(),0,'')


 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OUChangeViewBackDatedWindowPeriodDays','30',GETDATE(),GETDATE(),0,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('DATETIMEPICKERINTERVAL',  '15',getdate(), getdate(), 1,'DateTime Picker Interval Minitutes')

GO


UPDATE DI_M_ApprovalUserTypes SET ApprovalUserTypeDesc = 'Catalogue Owner' WHERE ID = 4
UPDATE DI_M_RoleTypes set ApplicableApprovalUserTypes = '6' WHERE ID IN (8,9)

--Role Owner
INSERT INTO DI_M_ApprovalUserTypes
SELECT 
'Role Owner',
ApplicableWorkFlowTypes,
IsActive,
CreatedBy,
CreatedDate,
LastModifiedBy,
LastModifiedDate
from DI_M_ApprovalUserTypes
WHERE Id = 1
GO

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('IsOneTimeUserListDiscoverExecute','1',GETDATE(),GETDATE(),0,'')

GO

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('Offboard2Days',  '5',getdate(), getdate(), 1,'Offboard 2 days interval')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('Offboard3Days',  '15',getdate(), getdate(), 1,'Offboard 3 days interval')

GO

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunOffBoard2Trigger', '1', getdate(), getdate(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunOffBoard3Trigger', '1', getdate(), getdate(),1,'')

GO

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('DefaultExclusionDays', '60',getdate(), getdate(), 1,'')

GO

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ExChangeResultSize', '20000', getdate(), getdate(),0,'')
GO

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ReportPageUrl', '/Report/List.aspx', getdate(), Null,1,'Report Page Url')
GO

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('DOMAINCONTROLSERVER',  '',getdate(), getdate(), 0,'Exchange Domain Control Server')
GO

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ISCROSSDOMAIN',  '0',getdate(), getdate(), 0,'Exchange Server is cross domain?')
GO

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ExChangeRequestDelay',  '20',getdate(), getdate(), 1,'Exchange request delay post AD creation')
GO

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('EnvironmentFBI',  'UAT',getdate(), getdate(), 1,'FBI Environment SIT/UAT/PROD')
GO

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('UserNamePrefix',  '',getdate(), getdate(), 0,'UAT UserName Prefix')
GO

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ReviewStatusSummryStartDays', '-10',getdate(), getdate(), 1,'')

GO

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ReviewStatusSummryEndDays', '10',getdate(), getdate(), 1,'')

GO

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('EmployeeTriggerInhouseApplMsg', 'Provision the request role only.',getdate(), getdate(), 1,'')

GO
INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('FSCMDelayHours', 'FSCMDelayHours',getdate(), getdate(), 1,'12')

GO

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('DiscoverBufferForOnBoard', '12', getdate(), getdate(),1,'DiscoverBufferForOnBoard in Hours')
GO

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('StopInhouseAttestationIfDataNotUploaded',  '1',getdate(), getdate(), 1,'Stop Inhouse Attestation if data file not uploaded.')
GO


INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('DormantAccountNoOfDays',  '90',getdate(), getdate(), 1,'Management Report Dormant Account No of Days.')
GO

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('EndpointsRequiresManulaImplementation',  '',getdate(), getdate(), 1,'EndpointsRequiresManulaImplementation')
GO

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('EntitlementsRequiresManulaImplementation',  '',getdate(), getdate(), 1,'EntitlementsRequiresManulaImplementation')
GO


INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADHOC_SVCCONTROLLER',  'DIChakraAdHocJobService',getdate(), getdate(), 1,'AdHoc Service Controller')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ExcludePasswordLetterJobCode',  'T0032,J00534',getdate(), getdate(), 1,'Exclude from generating Password Letter for these Jobcode')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ExcludePasswordLetterDepartment',  'D0373,D0033,D0374,D0375,D0376,D0455,D0496,D0380,D0080,D0351,D0436,D0378,D0475,D0379,D0105,D0032,D0427,D0495,D0034,D0352,D0456',getdate(), getdate(), 1,'Exclude from generating Password Letter for these Departments')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('OffboardMailNotificationWindowPeriod','5',GETDATE(),GETDATE(),1,'')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('DefaultExchangeMailQuota','300 MB',GETDATE(),GETDATE(),1,'')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('EnableGamingRemovalRequestCreate',  '0',getdate(), getdate(), 1,'Enable Gaming Removal AccessRequest Create')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('SkypeForBusinessFrontEndServer',  'pskyfea1.sap.local',getdate(), getdate(), 1,'Enable/Disable Masking')
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('UpdateTelehoneNumberInHRMS',  '1',getdate(), getdate(), 1,'Update the telephone number in HRMS in case of any failure in SFB')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('VPNADAccountCreationState',  '2',getdate(), getdate(), 1,'VPN AD account shall be created upon 2 level approval')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('GEMSADAccountCreationState',  '2',getdate(), getdate(), 1,'R GEMS AD account shall be created upon 2 level approval')

INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('SkipConnectorCall',  '0',getdate(), getdate(), 0,'To Skip the connector call during provisioning')

 INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('PurgeConfigurationTimer',  '0 0 12 1 1 ? *',getdate(), getdate(), 0,' Timer for Purge Configuration generation every year')