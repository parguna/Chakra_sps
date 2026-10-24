-- IACM
DELETE FROM [dbo].[DI_M_IACMConfigurationMaster] WHERE [ConfigKey] in (	'SharedMailboxText'			,'RegularMailboxText',
																		'SAPLOCALDiscoveryJobForWintelTeam','ADDiscoveryJobForWintelTeam','WBSAdmin',
																		'IT_DepartmentCode','AdhocLicenseMsg','NetworkManagement','WBSAdmin')


INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('SharedMailboxText', 'License Shared Mail', getdate(), getdate(),1,'')
INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('RegularMailboxText', 'Internal Shared Mail', getdate(), getdate(),1,'')
INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('SAPLOCALDiscoveryJobForWintelTeam', '1156', getdate(), getdate(),1,'')
INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADDiscoveryJobForWintelTeam', '1107', getdate(), getdate(),1,'')
INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('IT_DepartmentCode', 'SP17', getdate(), getdate(),1,'')
INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('AdhocLicenseMsg', '*Important note* O365 Licenses and Charges applied to your cost centers', getdate(), getdate(),1,'')


 INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('NetworkManagement', '5', getdate(), getdate(),1,'')


 INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('WBSAdmin', '11', getdate(), getdate(),1,'')


go

 UPDATE [dbo].[DI_M_IACMConfigurationMaster] set  [ConfigValue]= 2    WHERE [ConfigKey] = 'WintelGroupId' 
 go

--INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('OnBoardTriggers', '01',getdate(), getdate(), 1,'')

-- IM Configuration master

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'ExcludedCostCenters'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ExcludedCostCenters', 'CCT,NCC', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'HRMSDummyEmailId'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('HRMSDummyEmailId', 'TESTEMAIL@SPGROUP.COM.SG', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'ADPostOfficeBoxValue'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADPostOfficeBoxValue', 'office365user', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'ADUPNDomainName'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADUPNDomainName', 'SingaporePower.local', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'ADNonEmployeeAccountOU'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADNonEmployeeAccountOU', 'OU=NonEmployeeAccount,DC=idm,DC=local', getdate(), getdate(),1,'')


DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'O365AllowExternalEmail'
GO
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('O365AllowExternalEmail', 'AllowExtEmail', getdate(), getdate(),1,'')
GO

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'O365DenyExternalEmail'
GO
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('O365DenyExternalEmail', 'DenyExtEmail', getdate(), getdate(),1,'')
GO

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'O365EmailDomainName'
GO
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('O365EmailDomainName', 'SingaporePower.com.sg', getdate(), getdate(),1,'')
GO

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'O365SvcAccountPwdChangeDuration'
GO
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('O365SvcAccountPwdChangeDuration', '60', getdate(), getdate(),1,'')
GO

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'O365ADSyncWaitTime'
GO
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('O365ADSyncWaitTime', '5', getdate(), getdate(),1,'')
GO

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'O365MailBoxWaitTime'
GO
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('O365MailBoxWaitTime', '2', getdate(), getdate(),1,'')
GO

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'ADSharedAccountOU'
GO
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADSharedAccountOU', 'OU=SharedAccount,DC=idm,DC=local', getdate(), getdate(),1,'')
GO

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] in (	'ADSAPLocalUserAccountOU'	,'ADSAPLocalServiceAccountOU',
'ADSAPLocalSharedAccountOU'	,'ADSAPLocalDisabledAccountOU', 'ADSAPLocalSecondaryAccountOU')

INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADSAPLocalUserAccountOU', 'OU=Service accounts,OU=Accounts,DC=Sap,DC=local', getdate(), getdate(),1,'')
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADSAPLocalServiceAccountOU', 'OU=Service accounts,OU=Accounts,DC=Sap,DC=local', getdate(), getdate(),1,'')
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADSAPLocalSharedAccountOU', 'OU=Service accounts,OU=Accounts,DC=Sap,DC=local', getdate(), getdate(),1,'')
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADSAPLocalDisabledAccountOU', 'OU=Disabled,OU=Service accounts,OU=Accounts,DC=Sap,DC=local', getdate(), getdate(),1,'')
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADSAPLocalSecondaryAccountOU', 'OU=Service accounts,OU=Accounts,DC=Sap,DC=local', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'ADServiceAccountOU'
GO
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADServiceAccountOU', 'OU=ServiceAccount,DC=idm,DC=local', getdate(), getdate(),1,'')
GO

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'ADSecondaryAccountOU'
GO
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ADSecondaryAccountOU', 'OU=SecondaryAccount,DC=idm,DC=local', getdate(), getdate(),1,'')
GO




INSERT INTO DI_M_PageUrl VALUES('/Workflow/Clone.aspx',NULL,GETDATE(),NULL,NULL) 
GO

INSERT INTO [dbo].[DI_M_Page]   ([UniqueName],[Name],[PageUrlId] ,[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate])
     select  'ADMIN_WORKFLOW_CLONEWORKFLOW' ,'Clone Workflow' ,id  ,NULL,GETDATE(),NULL,NULL from DI_M_PageUrl where [url] =  '/Workflow/Clone.aspx'
GO

INSERT INTO DI_M_PagePermission SELECT Id,6,PageUrlId,null,GETDATE(),null,null FROM DI_M_Page  WHERE UniqueName =  'ADMIN_WORKFLOW_CLONEWORKFLOW'
GO

INSERT INTO DI_M_PagePermission SELECT 94,16,Id,null,GETDATE(),null,null FROM DI_M_PageUrl WHERE Url='/Workflow/Clone.aspx'
GO

---------------------Bulk Access Request -------------------------------------



 INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/NACR/Create/BulkRequest/BulkAdhocAccessRequest.aspx' , SYSDATETIME())
 
DECLARE @UrlId bigint
DECLARE @PageId bigint
DECLARE @TabParentMenuId bigint

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/NACR/Create/BulkRequest/BulkAdhocAccessRequest.aspx'
 

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_BULK_ACCESSREQUESTS','Bulk Access Request' ,@UrlId , SYSDATETIME())

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='HOME' 

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_BULK_ACCESSREQUESTS'

INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'HOME_BULK_ACCESSREQUESTS', 'Bulk Access Request' , @PageId , SYSDATETIME())

 Go


 DECLARE @UrlId bigint
DECLARE @PageId bigint
DECLARE @TabParentMenuId bigint

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/NACR/Create/BulkRequest/BulkAdhocAccessRequest.aspx'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_BULK_ACCESSREQUESTS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())



DECLARE @PagePermissionId bigint
DECLARE @RIDRole bigint 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR

SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in (
'HOME_BULK_ACCESSREQUESTS'
)

OPEN c1
 
FETCH NEXT FROM c1
INTO @PagePermissionId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDRole ,  @PagePermissionId,'System' , SYSDATETIME(),'System' , SYSDATETIME())

FETCH NEXT FROM c1
INTO @PagePermissionId

END

CLOSE c1
DEALLOCATE c1

GO

------------------------------------------------------------------------------



INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(1 ,  20997,'System' , SYSDATETIME(),'System' , SYSDATETIME())

------------------------------------------------------------------------------------------------------

 UPDATE
    DI_M_PagePermission
SET
    [UrlId] = P.PageUrlId
FROM
   (SELECT [PageUrlId] FROM DI_M_Page WHERE [UniqueName]= 'HOME_MYREQUESTS_DETAIL' ) P 
WHERE
    [PermissionId] IN (SELECT Id FROM DI_M_Permission WHERE [Name]='Detail') AND
    [PageId] IN (SELECT ID FROM DI_M_Page WHERE [UniqueName] ='ADMIN_PROVISIONING_MYREQUESTS')

------------------------------------------------------------------------------------------------------------------------------------------------------------


IF NOT EXISTS (SELECT * FROM [dbo].[DI_M_EndpointConfigurationKey] WHERE KEYNAME = 'IsWindowsAuthentication')
INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName],[Caption] ,[CreatedBy] ,[CreatedDate] ,[LastModifiedBy] ,[LastModifiedDate])
     VALUES ('IsWindowsAuthentication' ,'Is Windows Authentication','System'  ,GETDATE(),'System'  ,GETDATE())

	 IF NOT EXISTS (SELECT * FROM [dbo].[DI_M_EndpointConfigurationKey] WHERE KEYNAME = 'DirectorySyncServerName')
INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName],[Caption] ,[CreatedBy] ,[CreatedDate] ,[LastModifiedBy] ,[LastModifiedDate])
     VALUES ('DirectorySyncServerName' ,'Directory Sync ServerName','System'  ,GETDATE(),'System'  ,GETDATE())

	 IF NOT EXISTS (SELECT * FROM [dbo].[DI_M_EndpointConfigurationKey] WHERE KEYNAME = 'WSDLUrl')
INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName],[Caption] ,[CreatedBy] ,[CreatedDate] ,[LastModifiedBy] ,[LastModifiedDate])
     VALUES ('WSDLUrl' ,'WSDLUrl','System'  ,GETDATE(),'System'  ,GETDATE())
--17/19/2018

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'ExecutiveEmployeeADGroups'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('ExecutiveEmployeeADGroups', 'InternetUsers', getdate(), getdate(),1,'')



DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'MSPDefaultProfile'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('MSPDefaultProfile', 'Z_MSQ100', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'MCPDefaultProfile'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('MCPDefaultProfile', 'Z_MSPCLNT100', getdate(), getdate(),1,'')


DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'NonEmployeeNoticePeriod'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('NonEmployeeNoticePeriod', '7,14,21,30', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'LastPwdChangeTime'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('LastPwdChangeTime', '2016-12-09 10:38:32.273', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'CanSyncSmartDirectory'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanSyncSmartDirectory', '1', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'CanResetO365Pwd'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanResetO365Pwd', '0', getdate(), getdate(),1,'')


DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'SMSCode'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('SMSCode', 'SSIDS-SMS', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'SMSDummyBadgeStartRange'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('SMSDummyBadgeStartRange', '6000000000', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'SMSPassOfficeEmail'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('SMSPassOfficeEmail', 'passoffice@singaporepower.com.sg', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'XL_SMSRoleCatalogue'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('XL_SMSRoleCatalogue', 'Building,Section,RoleName,Description,TechnicalRole,Level1Approver,Level1ApproverType,Level2Approver,Level2ApproverType,Level3Approver,Level3ApproverType', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'XL_SMSProvisioningMatrix'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('XL_SMSProvisioningMatrix', 'PostalCode,Location,DepartmentCode,DepartmentName,DivisionCode,DivisionName,DefaultGroups', getdate(), getdate(),1,'')




--update DI_M_IMConfigurationMaster set ConfigValue = 'passoffice@spgroup.com.sg' where ConfigKey = 'SMSPassOfficeEmail'
--update DI_M_IMConfigurationMaster set ConfigValue = 'AllowInternetMail' where ConfigKey = 'O365AllowExternalEmail'
--update DI_M_IMConfigurationMaster set ConfigValue = 'NoInternetMail' where ConfigKey = 'O365DenyExternalEmail'
--update DI_M_IMConfigurationMaster set ConfigValue = '5' where ConfigKey = 'O365MailBoxWaitTime'
--update DI_M_IMConfigurationMaster set ConfigValue = 'Office365User' where ConfigKey = 'ADPostOfficeBoxValue'
--update DI_M_IMConfigurationMaster set ConfigValue = 'spgroup.com.sg' where ConfigKey = 'O365EmailDomainName'
--update DI_M_IMConfigurationMaster set ConfigValue = 'spgroup.com.sg' where ConfigKey = 'ADUPNDomainName'
--update DI_M_IMConfigurationMaster set ConfigValue = 'KnownAs,TelephoneNumber,EmailId,OrgDepartment,OrgDepartment,OrgDivision,OrgBranch,OrgSection,Fax,TelephoneNumber,MobileNumber,PostalCode,PostalAddress,Country,EmployeeId,NRIC' where ConfigKey = 'PropagationFields'
--update DI_M_MailTemplate set MailFrom = 'securityadmin@spgroup.com.sg'

--DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'PropagationFields'
--INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
--VALUES ('PropagationFields', 'KnownAs,TelephoneNumber,EmailId,OrgDepartment,OrgDepartment,OrgDivision,OrgBranch,OrgSection,Fax,TelephoneNumber,MobileNumber,PostalCode,PostalAddress,Country,EmployeeId,NRIC', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'DefaultMailQuota'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('DefaultMailQuota', '150', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'SharedAccountMailQuota'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('SharedAccountMailQuota', '600', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'MailQuotaThreshold'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('MailQuotaThreshold', '10', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'AccountCreatedNotificationTo'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('AccountCreatedNotificationTo', 'secaidmtest@spgroup.com.sg', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'AccountCreatedNotificationCC'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('AccountCreatedNotificationCC', 'donotrespond@spgroup.com.sg', getdate(), getdate(),1,'')


DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'FailureNotificaionReceiver'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('FailureNotificaionReceiver', 'secaidmtest@spgroup.com.sg', getdate(), getdate(),1,'')

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'AlertNotificaionReceiver'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('AlertNotificaionReceiver', 'secaidmtest@spgroup.com.sg', getdate(), getdate(),1,'')



DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'HRMSTransferTriggers'
GO
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('HRMSTransferTriggers', 'C1,C2,C3,C4,C5', getdate(), getdate(),1,'')
GO


DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'HRMSReOrgTriggers'
GO
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('HRMSReOrgTriggers', 'E0', getdate(), getdate(),1,'')
GO

DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'SPHelpDeskEmail'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('SPHelpDeskEmail', '', getdate(), getdate(),1,'')
GO


DELETE FROM [dbo].[DI_M_IMConfigurationMaster] WHERE [ConfigKey] = 'CanRunUpdateOneCardUsersJob'
INSERT INTO [dbo].[DI_M_IMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('CanRunUpdateOneCardUsersJob', '1', getdate(), getdate(),1,'')
GO





 IF NOT EXISTS (SELECT * FROM [dbo].[DI_M_EndpointConfigurationKey] WHERE KEYNAME = 'SecondaryAccountEnabledDepartments')
INSERT INTO [dbo].[DI_M_IACMConfigurationMaster]([ConfigKey], [ConfigValue],CreatedDate, LastModifiedDate, [IsUiDisplay],[Description])
VALUES ('SecondaryAccountEnabledDepartments', 'SP17', getdate(), getdate(),1,'')
GO