PRINT 'DI_M_ENdpointSystem'
INSERT INTO IACM_UAT.[dbo].[DI_M_EndpointSystem]
           ([EndPointSystemDescription]
           ,
           ,[EndpointSystemTypeId]
           ,[IsTrustedSource]
           ,[HostName]
           ,[DomainName]
           ,[IpAddress]
           ,[IsActive]
           ,[OwnerName]
           ,[OwnerContact]
           ,[OwnerEmail]
           ,[OwnerUserId]
           ,[SaveCredentials]
           ,[PopulateIacm]
           ,[MapIacm]
           ,[DiscoveryReport]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])

SELECT 
			[EndPointSystemDescription]
           ,
           ,[EndpointSystemTypeId]
           ,[IsTrustedSource]
           ,[HostName]
           ,[DomainName]
           ,[IpAddress]
           ,[IsActive]
           ,[OwnerName]
           ,[OwnerContact]
           ,[OwnerEmail]
           ,[OwnerUserId]
           ,[SaveCredentials]
           ,[PopulateIacm]
           ,[MapIacm]
           ,[DiscoveryReport]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM IACM_DI.dbo.DI_M_EndpointSystem
GO
PRINT '[DI_TRS_Configuration]'
INSERT INTO IACM_UAT.[dbo].[DI_TRS_Configuration]
            ([Endpointsystemid]
           ,[HostName]
           ,[FolderPath]
           ,[ImportDepartment]
           ,[ImportEmployee]
           ,[MapAll]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
  SELECT 
			[Endpointsystemid]
           ,[HostName]
           ,[FolderPath]
           ,[ImportDepartment]
           ,[ImportEmployee]
           ,[MapAll]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM IACM_DI.[dbo].[DI_TRS_Configuration]
GO
PRINT '[DI_ADS_Configuration]'
INSERT INTO IACM_UAT.[dbo].[DI_ADS_Configuration]
           ([Endpointsystemid]
           ,[HostName]
           ,[IPAddress]
           ,[DomainName]
           ,[UserName]
           ,[Password]
           ,[RootCertificate]
           ,[Ou]
           ,[ImportComputerObj]
           ,[ImportFolderObj]
           ,[ImportUser]
           ,[ImportGroup]
           ,[ImportOu]
           ,[ImportPrinterObj]
           ,[MapAll]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
  SELECT 
  
           [Endpointsystemid]
           ,[HostName]
           ,[IPAddress]
           ,[DomainName]
           ,[UserName]
           ,[Password]
           ,[RootCertificate]
           ,[Ou]
           ,[ImportComputerObj]
           ,[ImportFolderObj]
           ,[ImportUser]
           ,[ImportGroup]
           ,[ImportOu]
           ,[ImportPrinterObj]
           ,[MapAll]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]  

FROM IACM_DI.[dbo].[DI_ADS_Configuration]
GO
PRINT '[DI_Ex_Configuration]'
INSERT INTO IACM_UAT.[dbo].[DI_Ex_Configuration]
           ([Endpointsystemid]
           ,[HostName]
           ,[IPAddress]
           ,[DomainName]
           ,[UserName]
           ,[Password]
           ,[RootCertificate]
           ,[Ou]
           ,[ImportComputerObj]
           ,[ImportFolderObj]
           ,[ImportUser]
           ,[ImportGroup]
           ,[ImportOu]
           ,[ImportPrinterObj]
           ,[MapAll]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])

SELECT 

			[Endpointsystemid]
           ,[HostName]
           ,[IPAddress]
           ,[DomainName]
           ,[UserName]
           ,[Password]
           ,[RootCertificate]
           ,[Ou]
           ,[ImportComputerObj]
           ,[ImportFolderObj]
           ,[ImportUser]
           ,[ImportGroup]
           ,[ImportOu]
           ,[ImportPrinterObj]
           ,[MapAll]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM IACM_DI.dbo.DI_Ex_Configuration
GO
PRINT '[DI_SAP_Configuration]'
INSERT INTO IACM_UAT.[dbo].[DI_SAP_Configuration]
           ([EndPointSystemId]
           ,[HostName]
           ,[IPAddress]
           ,[DomainName]
           ,[ServerName]
           ,[DataSourceName]
           ,[ClientNumber]
           ,[SystemNumber]
           ,[UserName]
           ,[Password]
           ,[ImportRoles]
           ,[ImportGroups]
           ,[ImportProfiles]
           ,[ImportTransactionCodes]
           ,[ImportUsers]
           ,[ImportAuthorizations]
           ,[MapAll]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])

SELECT 

			[EndPointSystemId]
           ,[HostName]
           ,[IPAddress]
           ,[DomainName]
           ,[ServerName]
           ,[DataSourceName]
           ,[ClientNumber]
           ,[SystemNumber]
           ,[UserName]
           ,[Password]
           ,[ImportRoles]
           ,[ImportGroups]
           ,[ImportProfiles]
           ,[ImportTransactionCodes]
           ,[ImportUsers]
           ,[ImportAuthorizations]
           ,[MapAll]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM IACM_DI.dbo.DI_SAP_Configuration
GO
PRINT '[DI_SIS_Configuration]'
INSERT INTO IACM_UAT.[dbo].[DI_SIS_Configuration]
           ([EndPointSystemId]
           ,[ServerName]
           ,[DataBaseName]
           ,[UserName]
           ,[Password]
           ,[MapAll]
           ,[ImportUsers]
           ,[ImportRoles]
           ,[ImportPages]
           ,[ImportPageElements]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])

SELECT 

			[EndPointSystemId]
           ,[ServerName]
           ,[DataBaseName]
           ,[UserName]
           ,[Password]
           ,[MapAll]
           ,[ImportUsers]
           ,[ImportRoles]
           ,[ImportPages]
           ,[ImportPageElements]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM IACM_DI.dbo.DI_SIS_Configuration
GO
PRINT '[DI_M_DiscoveryProcess]'
INSERT INTO IACM_UAT.[dbo].[DI_M_DiscoveryProcess]
           ([HistoryId]
           ,[Endpointsystemid]
           ,[ProcessStartAt]
           ,[ProcessEndAt]
           ,[Status]
           ,[Message]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
 SELECT

			[HistoryId]
           ,[Endpointsystemid]
           ,[ProcessStartAt]
           ,[ProcessEndAt]
           ,[Status]
           ,[Message]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM IACM_DI.dbo.DI_M_DiscoveryProcess
GO
PRINT '[DI_M_Scheduler]'

INSERT INTO [IACM_UAT].[dbo].[DI_M_Scheduler]
           ([ProcessName]
           ,[AttestationProcessId]
           ,[DiscoveryProcessId]
           ,[RunOnce]
           ,[RunEveryWeek]
           ,[RunEveryMonth]
           ,[RunEveryDay]
           ,[RunSpecifiedTime]
           ,[RunDate]
           ,[RunEveryWeekDay]
           ,[RunEveryDays]
           ,[SpecifiedFromTime]
           ,[SpecifiedToTime]
           ,[SpecifiedEveryHour]
           ,[StartDate]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
			
SELECT 
			[ProcessName]
           ,NULL
           ,[DiscoveryProcessId]
           ,[RunOnce]
           ,[RunEveryWeek]
           ,[RunEveryMonth]
           ,[RunEveryDay]
           ,[RunSpecifiedTime]
           ,[RunDate]
           ,[RunEveryWeekDay]
           ,[RunEveryDays]
           ,[SpecifiedFromTime]
           ,[SpecifiedToTime]
           ,[SpecifiedEveryHour]
           ,[StartDate]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM  [IACM_DI].[dbo].[DI_M_Scheduler]

GO
PRINT '[DI_M_Organisation]'

INSERT INTO [IACM_UAT].[dbo].[DI_M_Organisation]
           (
           ,[OrgUnitPrefix]
           ,[OrganisationName]
           ,[ParentOrganisation]
           ,[HOD]
           ,[OrganisationHeadUserId]
           ,[DivisionRepresentative]
           ,[DivisionRepresentativeUserId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
SELECT
			
           ,[OrgUnitPrefix]
           ,[OrganisationName]
           ,[ParentOrganisation]
           ,[HOD]
           ,[OrganisationHeadUserId]
           ,[DivisionRepresentative]
           ,[DivisionRepresentativeUserId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
FROM
IACM_DI.[dbo].[DI_M_Organisation]
GO
PRINT '[DI_M_User]'
INSERT INTO [IACM_UAT].[dbo].[DI_M_User]
           ([NRIC]
           ,[FIN]
           ,[PassportNo]
           ,[UserName]
           ,[Password]
           ,
           ,[FirstName]
           ,[MiddleName]
           ,[LastName]
           ,[DisplayName]
           ,[EmployeeId]
           ,[FullTimeEmployee]
           ,[OrganisationId]
           ,[EmailId]
           ,[ManagerId]
           ,[Location]
           ,[Address]
           ,[Country]
           ,[City]
           ,[State]
           ,[TelephoneNumber]
           ,[HomephoneNumber]
           ,[MobileNumber]
           ,[BusinessRole]
           ,[Company]
           ,[JoinDate]
           ,[RehireDate]
           ,[TerminationDate]
           ,[Remarks1]
           ,[Remarks2]
           ,[Remarks3]
           ,[IsActive]
           ,[Ethinicity]
           ,[DeepIdentityRole]
           ,[EmployeeStatus]
           ,[EmployeeType]
           ,[WorkAddress1]
           ,[WorkAddress2]
           ,[WorkAddress3]
           ,[WorkAddress4]
           ,[WorkPostalCode]
           ,[Country2Char]
           ,[Building]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
 SELECT 
			[NRIC]
           ,[FIN]
           ,[PassportNo]
           ,[UserName]
           ,[Password]
           ,
           ,[FirstName]
           ,[MiddleName]
           ,[LastName]
           ,[DisplayName]
           ,[EmployeeId]
           ,[FullTimeEmployee]
           ,[OrganisationId]
           ,[EmailId]
           ,[ManagerId]
           ,[Location]
           ,[Address]
           ,[Country]
           ,[City]
           ,[State]
           ,[TelephoneNumber]
           ,[HomephoneNumber]
           ,[MobileNumber]
           ,[BusinessRole]
           ,[Company]
           ,[JoinDate]
           ,[RehireDate]
           ,[TerminationDate]
           ,[Remarks1]
           ,[Remarks2]
           ,[Remarks3]
           ,[IsActive]
           ,[Ethinicity]
           ,[DeepIdentityRole]
           ,[EmployeeStatus]
           ,[EmployeeType]
           ,[WorkAddress1]
           ,[WorkAddress2]
           ,[WorkAddress3]
           ,[WorkAddress4]
           ,[WorkPostalCode]
           ,[Country2Char]
           ,[Building]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
FROM
IACM_DI.[dbo].[DI_M_User]
WHERE USERID <> 1
GO
PRINT '[DI_ADS_GroupInfo]'
INSERT INTO IACM_UAT.[dbo].[DI_ADS_GroupInfo]
           (
           ,[EndPointSystemId]
           ,[cn]
           ,[description]
           ,[member]
           ,[distinguishedName]
           ,[instanceType]
           ,[whenCreated]
           ,[whenChanged]
           ,[uSNCreated]
           ,[uSNChanged]
           ,[name]
           ,[objectGUID]
           ,[objectSid]
           ,[sAMAccountName]
           ,[sAMAccountType]
           ,[groupType]
           ,[groupTypeDesc]
           ,[objectCategory]
           ,[nTSecurityDescriptor]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])

SELECT 
			
           ,[EndPointSystemId]
           ,[cn]
           ,[description]
           ,[member]
           ,[distinguishedName]
           ,[instanceType]
           ,[whenCreated]
           ,[whenChanged]
           ,[uSNCreated]
           ,[uSNChanged]
           ,[name]
           ,[objectGUID]
           ,[objectSid]
           ,[sAMAccountName]
           ,[sAMAccountType]
           ,[groupType]
           ,[groupTypeDesc]
           ,[objectCategory]
           ,[nTSecurityDescriptor]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM IACM_DI.dbo.DI_ADS_GroupInfo
GO
PRINT '[DI_ADS_UserInfo]'
INSERT INTO IACM_UAT.[dbo].[DI_ADS_UserInfo]
           (
           ,[EndPointSystemId]
           ,[cn]
           ,[sn]
           ,[PwdResetCnt]
           ,[st]
           ,[title]
           ,[description]
           ,[postalAddress]
           ,[postalCode]
           ,[postOfficeBox]
           ,[physicalDeliveryOfficeName]
           ,[telephoneNumber]
           ,[facsimileTelephoneNumber]
           ,[givenName]
           ,[initials]
           ,[distinguishedName]
           ,[instanceType]
           ,[whenCreated]
           ,[whenChanged]
           ,[displayName]
           ,[uSNCreated]
           ,[info]
           ,[uSNChanged]
           ,[co]
           ,[department]
           ,[company]
           ,[manager]
           ,[streetAddress]
           ,[wWWHomePage]
           ,[name]
           ,[objectGUID]
           ,[userAccountControl]
           ,[IsAccountDisabled]
           ,[IsAccountLocked]
           ,[IsPasswordRequired]
           ,[PasswordExpirationDate]
           ,[badPwdCount]
           ,[codePage]
           ,[countryCode]
           ,[badPasswordTime]
           ,[lastLogoff]
           ,[lastLogon]
           ,[pwdLastSet]
           ,[primaryGroupID]
           ,[objectSid]
           ,[accountExpires]
           ,[logonCount]
           ,[sAMAccountName]
           ,[sAMAccountType]
           ,[managedObjects]
           ,[ipPhone]
           ,[objectCategory]
           ,[mail]
           ,[homePhone]
           ,[mobile]
           ,[pager]
           ,[nTSecurityDescriptor]
           ,[MappingType]
           ,[DIUserId]
           ,[ExUserId]
           ,[SpecialAccount]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
           ,[IACMLastPasswordChange]
           ,[IsServiceAccount]
           ,[IfUnmappedMapTo])

SELECT 
			
           ,[EndPointSystemId]
           ,[cn]
           ,[sn]
           ,[PwdResetCnt]
           ,[st]
           ,[title]
           ,[description]
           ,[postalAddress]
           ,[postalCode]
           ,[postOfficeBox]
           ,[physicalDeliveryOfficeName]
           ,[telephoneNumber]
           ,[facsimileTelephoneNumber]
           ,[givenName]
           ,[initials]
           ,[distinguishedName]
           ,[instanceType]
           ,[whenCreated]
           ,[whenChanged]
           ,[displayName]
           ,[uSNCreated]
           ,[info]
           ,[uSNChanged]
           ,[co]
           ,[department]
           ,[company]
           ,[manager]
           ,[streetAddress]
           ,[wWWHomePage]
           ,[name]
           ,[objectGUID]
           ,[userAccountControl]
           ,[IsAccountDisabled]
           ,[IsAccountLocked]
           ,[IsPasswordRequired]
           ,[PasswordExpirationDate]
           ,[badPwdCount]
           ,[codePage]
           ,[countryCode]
           ,[badPasswordTime]
           ,[lastLogoff]
           ,[lastLogon]
           ,[pwdLastSet]
           ,[primaryGroupID]
           ,[objectSid]
           ,[accountExpires]
           ,[logonCount]
           ,[sAMAccountName]
           ,[sAMAccountType]
           ,[managedObjects]
           ,[ipPhone]
           ,[objectCategory]
           ,[mail]
           ,[homePhone]
           ,[mobile]
           ,[pager]
           ,[nTSecurityDescriptor]
           ,[MappingType]
           ,[DIUserId]
           ,[ExUserId]
           ,[SpecialAccount]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
           ,[IACMLastPasswordChange]
           ,[IsServiceAccount]
           ,[IfUnmappedMapTo]

FROM IACM_DI.dbo.DI_ADS_UserInfo
GO
PRINT '[DI_Ex_GroupInfo]'
INSERT INTO [IACM_UAT].[dbo].[DI_Ex_GroupInfo]
           (
           ,[EndPointSystemId]
           ,[cn]
           ,[description]
           ,[member]
           ,[distinguishedName]
           ,[instanceType]
           ,[whenCreated]
           ,[whenChanged]
           ,[uSNCreated]
           ,[uSNChanged]
           ,[name]
           ,[objectGUID]
           ,[objectSid]
           ,[sAMAccountName]
           ,[sAMAccountType]
           ,[groupType]
           ,[objectCategory]
           ,[nTSecurityDescriptor]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
   
SELECT 
			
           ,[EndPointSystemId]
           ,[cn]
           ,[description]
           ,[member]
           ,[distinguishedName]
           ,[instanceType]
           ,[whenCreated]
           ,[whenChanged]
           ,[uSNCreated]
           ,[uSNChanged]
           ,[name]
           ,[objectGUID]
           ,[objectSid]
           ,[sAMAccountName]
           ,[sAMAccountType]
           ,[groupType]
           ,[objectCategory]
           ,[nTSecurityDescriptor]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM IACM_DI.dbo.DI_EX_GroupInfo
GO
PRINT '[DI_Ex_UserInfo]'
INSERT INTO IACM_UAT.[dbo].[DI_Ex_UserInfo]
           (
           ,[EndPointSystemId]
           ,[cn]
           ,[sn]
           ,[st]
           ,[title]
           ,[description]
           ,[postalAddress]
           ,[postalCode]
           ,[postOfficeBox]
           ,[physicalDeliveryOfficeName]
           ,[telephoneNumber]
           ,[facsimileTelephoneNumber]
           ,[givenName]
           ,[initials]
           ,[distinguishedName]
           ,[instanceType]
           ,[whenCreated]
           ,[whenChanged]
           ,[displayName]
           ,[uSNCreated]
           ,[info]
           ,[uSNChanged]
           ,[co]
           ,[department]
           ,[company]
           ,[manager]
           ,[streetAddress]
           ,[wWWHomePage]
           ,[name]
           ,[objectGUID]
           ,[userAccountControl]
           ,[IsAccountDisabled]
           ,[IsAccountLocked]
           ,[IsPasswordRequired]
           ,[PasswordExpirationDate]
           ,[badPwdCount]
           ,[codePage]
           ,[countryCode]
           ,[badPasswordTime]
           ,[lastLogoff]
           ,[lastLogon]
           ,[pwdLastSet]
           ,[primaryGroupID]
           ,[objectSid]
           ,[accountExpires]
           ,[logonCount]
           ,[sAMAccountName]
           ,[sAMAccountType]
           ,[managedObjects]
           ,[ipPhone]
           ,[objectCategory]
           ,[mail]
           ,[homePhone]
           ,[mobile]
           ,[pager]
           ,[nTSecurityDescriptor]
           ,[MaxStorage]
           ,[msExchHSName]
           ,[msExchMailboxSecurity]
           ,[networkAddress]
           ,[msExchHomeServerName]
           ,[msExchMailboxSecurityDescriptor]
           ,[msExchMailboxGuid]
           ,[msExchMobileMailboxFlags]
           ,[msExchPoliciesIncluded]
           ,[msExchRecipientDisplayType]
           ,[msExchRecipientTypeDetails]
           ,[msExchUserAccountControl]
           ,[msExchUserCulture]
           ,[msExchVersion]
           ,[MappingType]
           ,[DIUserId]
           ,[AdUserId]
           ,[SpecialAccount]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
           ,[IACMLastPasswordChange]
           ,[IsServiceAccount]
           ,[IfUnmappedMapTo])

SELECT 

			
           ,[EndPointSystemId]
           ,[cn]
           ,[sn]
           ,[st]
           ,[title]
           ,[description]
           ,[postalAddress]
           ,[postalCode]
           ,[postOfficeBox]
           ,[physicalDeliveryOfficeName]
           ,[telephoneNumber]
           ,[facsimileTelephoneNumber]
           ,[givenName]
           ,[initials]
           ,[distinguishedName]
           ,[instanceType]
           ,[whenCreated]
           ,[whenChanged]
           ,[displayName]
           ,[uSNCreated]
           ,[info]
           ,[uSNChanged]
           ,[co]
           ,[department]
           ,[company]
           ,[manager]
           ,[streetAddress]
           ,[wWWHomePage]
           ,[name]
           ,[objectGUID]
           ,[userAccountControl]
           ,[IsAccountDisabled]
           ,[IsAccountLocked]
           ,[IsPasswordRequired]
           ,[PasswordExpirationDate]
           ,[badPwdCount]
           ,[codePage]
           ,[countryCode]
           ,[badPasswordTime]
           ,[lastLogoff]
           ,[lastLogon]
           ,[pwdLastSet]
           ,[primaryGroupID]
           ,[objectSid]
           ,[accountExpires]
           ,[logonCount]
           ,[sAMAccountName]
           ,[sAMAccountType]
           ,[managedObjects]
           ,[ipPhone]
           ,[objectCategory]
           ,[mail]
           ,[homePhone]
           ,[mobile]
           ,[pager]
           ,[nTSecurityDescriptor]
           ,[MaxStorage]
           ,[msExchHSName]
           ,[msExchMailboxSecurity]
           ,[networkAddress]
           ,[msExchHomeServerName]
           ,[msExchMailboxSecurityDescriptor]
           ,[msExchMailboxGuid]
           ,[msExchMobileMailboxFlags]
           ,[msExchPoliciesIncluded]
           ,[msExchRecipientDisplayType]
           ,[msExchRecipientTypeDetails]
           ,[msExchUserAccountControl]
           ,[msExchUserCulture]
           ,[msExchVersion]
           ,[MappingType]
           ,[DIUserId]
           ,[AdUserId]
           ,[SpecialAccount]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
           ,[IACMLastPasswordChange]
           ,[IsServiceAccount]
           ,[IfUnmappedMapTo]

FROM IACM_DI.dbo.DI_EX_UserInfo

GO
PRINT '[DI_SIS_UserInfo]'

INSERT INTO [IACM_UAT].[dbo].[DI_SIS_UserInfo]
           (
           ,[EndPointSystemId]
           ,[SISUserId]
           ,[UserName]
           ,[Email]
           ,[active]
           ,[roleid]
           ,[MappingType]
           ,[DIUserId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
           ,[IsServiceAccount]
           ,[IfUnmappedMapTo])
   SELECT

   
           ,[EndPointSystemId]
           ,[SISUserId]
           ,[UserName]
           ,[Email]
           ,[active]
           ,[roleid]
           ,[MappingType]
           ,[DIUserId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
           ,[IsServiceAccount]
           ,[IfUnmappedMapTo]

FROM IACM_DI.dbo.DI_SIS_UserInfo

GO
PRINT '[DI_SIS_Role]'

INSERT INTO [IACM_UAT].[dbo].[DI_SIS_Role]
           (
           ,[EndPointSystemId]
           ,[RoleId]
           ,[Role]
           ,[RoleDescription]
           ,[Remarks]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
   SELECT 

   
           ,[EndPointSystemId]
           ,[RoleId]
           ,[Role]
           ,[RoleDescription]
           ,[Remarks]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM IACM_DI.dbo.DI_SIS_Role	
GO
PRINT '[DI_SIS_Groups]'

INSERT INTO [IACM_UAT].[dbo].[DI_SIS_Groups]
           (
           ,[EndPointSystemId]
           ,[GroupId]
           ,[GroupName]
           ,[GroupDescription]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
  
  SELECT 
			
           ,[EndPointSystemId]
           ,[GroupId]
           ,[GroupName]
           ,[GroupDescription]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM [IACM_DI].[dbo].[DI_SIS_Groups]
GO
PRINT '[DI_SIS_Menu]'

INSERT INTO [IACM_UAT].[dbo].[DI_SIS_Menu]
           (
           ,[EndPointSystemId]
           ,[MenuId]
           ,[ParentMenuId]
           ,[MenuGroupCode]
           ,[DisplayName]
           ,[URL]
           ,[MenuLevel]
           ,[SequenceIndex]
           ,[StartDate]
           ,[EndDate]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
    SELECT


           ,[EndPointSystemId]
           ,[MenuId]
           ,[ParentMenuId]
           ,[MenuGroupCode]
           ,[DisplayName]
           ,[URL]
           ,[MenuLevel]
           ,[SequenceIndex]
           ,[StartDate]
           ,[EndDate]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM IACM_DI.dbo.DI_SIS_Menu

GO
PRINT '[DI_SIS_MenuUserRole]'

INSERT INTO [IACM_UAT].[dbo].[DI_SIS_MenuUserRole]
           (
           ,[EndPointSystemId]
           ,[MenuUserRoleId]
           ,[MenuId]
           ,[UserRoleId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
   
SELECT 


           ,[EndPointSystemId]
           ,[MenuUserRoleId]
           ,[MenuId]
           ,[UserRoleId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM [IACM_DI].[dbo].[DI_SIS_MenuUserRole]
GO
PRINT '[DI_SIS_GroupPageTokens]'

INSERT INTO [IACM_UAT].[dbo].[DI_SIS_GroupPageTokens]
           (
           ,[EndPointSystemId]
           ,[GroupPageTokenId]
           ,[GroupId]
           ,[PageId]
           ,[AccessToken]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
    SElECT


           ,[EndPointSystemId]
           ,[GroupPageTokenId]
           ,[GroupId]
           ,[PageId]
           ,[AccessToken]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM [IACM_DI].[dbo].[DI_SIS_GroupPageTokens]
GO
PRINT '[DI_SIS_PageAccessDetails]'

INSERT INTO [IACM_UAT].[dbo].[DI_SIS_PageAccessDetails]
           ([userID]
           ,[userName]
           ,[email]
           ,[role]
           ,[pageid]
           ,[page]
           ,[pagePath]
           ,[pageElement]
           ,[disabled]
           ,[hidden])
   SELECT 

   [userID]
           ,[userName]
           ,[email]
           ,[role]
           ,[pageid]
           ,[page]
           ,[pagePath]
           ,[pageElement]
           ,[disabled]
           ,[hidden]

FROM [IACM_DI].[dbo].[DI_SIS_PageAccessDetails]
GO
PRINT '[DI_SIS_PageElements]'


INSERT INTO [IACM_UAT].[dbo].[DI_SIS_PageElements]
           (
           ,[EndPointSystemId]
           ,[PageElementsId]
           ,[PageId]
           ,[ElementSelector]
           ,[ElementDisabledBitwiseValue]
           ,[ElementHiddenBitwiseValue]
           ,[ElementDescription]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
   SELECT


           ,[EndPointSystemId]
           ,[PageElementsId]
           ,[PageId]
           ,[ElementSelector]
           ,[ElementDisabledBitwiseValue]
           ,[ElementHiddenBitwiseValue]
           ,[ElementDescription]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]


FROM  [IACM_DI].[dbo].[DI_SIS_PageElements]
GO
PRINT '[DI_SIS_Pages]'

INSERT INTO [IACM_UAT].[dbo].[DI_SIS_Pages]
           (
           ,[EndPointSystemId]
           ,[PageId]
           ,[PagePathName]
           ,[PageDescription]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])

SELECT


           ,[EndPointSystemId]
           ,[PageId]
           ,[PagePathName]
           ,[PageDescription]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

		FROM   [IACM_DI].[dbo].[DI_SIS_Pages]
GO
PRINT '[DI_SIS_UserExtraPermissions]'

INSERT INTO [IACM_UAT].[dbo].[DI_SIS_UserExtraPermissions]
           (
           ,[EndPointSystemId]
           ,[UserExtraPermissionID]
           ,[UserName]
           ,[PageId]
           ,[Token]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
  
SELECT


           ,[EndPointSystemId]
           ,[UserExtraPermissionID]
           ,[UserName]
           ,[PageId]
           ,[Token]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]

FROM [IACM_DI].[dbo].[DI_SIS_UserExtraPermissions]


GO
PRINT '[DI_SAP_UserInfo]'


INSERT INTO [IACM_UAT].[dbo].[DI_SAP_UserInfo]
           (
           
           ,[EndPointSystemId]
           ,[Firstname]
           ,[Lastname]
           ,[Initials]
           ,[Department]
           ,[E_Mail]
           ,[Pers_No]
           ,[Tel1_Numbr]
           ,[Function0]
           ,[Title]
           ,[City]
           ,[Namcountry]
           ,[Pboxcit_No]
           ,[Postl_Cod1]
           ,[Street]
           ,[Start_Menu]
           ,[ActivityGroup]
           ,[LastLoginTime]
           ,[UserLogonName]
           ,[AccntLockNoPwd]
           ,[AccntLocDurPwd]
           ,[LOCKEDBYINTRUDER]
           ,[EMPLOYEEID]
           ,[EMPLOYEETYPE]
           ,[PASSWORDEXPTIME]
           ,[LOGINDISABLED]
           ,[IsActive]
           ,[DomainName]
           ,[TargetSysId]
           ,[PwdResetCnt]
           ,[DIUserId]
           ,[SpecialAccount]
           ,[IACMLastPasswordChange]
           ,[CreatedDate]
           ,[LastModifiedDate]
           ,[CreatedBy]
           ,[LastModifiedBy])

SELECT 


           
           ,5
           ,[Firstname]
           ,[Lastname]
           ,[Initials]
           ,[Department]
           ,[E_Mail]
           ,[Pers_No]
           ,[Tel1_Numbr]
           ,[Function0]
           ,[Title]
           ,[City]
           ,[Namcountry]
           ,[Pboxcit_No]
           ,[Postl_Cod1]
           ,[Street]
           ,[Start_Menu]
           ,[ActivityGroup]
           ,[LastLoginTime]
           ,[UserLogonName]
           ,[AccntLockNoPwd]
           ,[AccntLocDurPwd]
           ,[LOCKEDBYINTRUDER]
           ,[EMPLOYEEID]
           ,[EMPLOYEETYPE]
           ,[PASSWORDEXPTIME]
           ,[LOGINDISABLED]
           ,[IsActive]
           ,[DomainName]
           ,[TargetSysId]
           ,[PwdResetCnt]
           ,[DIUserId]
           ,[SpecialAccount]
           ,[IACMLastPasswordChange]
           ,[CreatedDate]
           ,[LastModifiedDate]
           ,[CreatedBy]
           ,[LastModifiedBy]

FROM SAP.dbo.DI_SAP_UserInfo 


GO
PRINT '[DI_SAP_UserProfile]'


INSERT INTO IACM_UAT.[dbo].[DI_SAP_UserProfile]
           ([ProfileName]
           ,[UserName]
           ,[EndPointSystemId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
   
SELECT

[ProfileName]
           ,[UserName]
           ,5
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
FROM SAP.[dbo].[DI_SAP_UserProfile]


GO
PRINT '[DI_SAP_TCodeObject]'


---------------------------

INSERT INTO IACM_UAT.[dbo].[DI_SAP_TCodeObject]
           ([ObjectType]
           ,[FieldValue]
           ,[ObjectCode]
           ,[EndPointSystemId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[FieldCode])
    
SELECT

[ObjectType]
           ,[FieldValue]
           ,[ObjectCode]
           ,5
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[FieldCode]

FROM SAP.[dbo].[DI_SAP_TCodeObject]

GO
PRINT '[DI_SAP_TCODEINFO]'

INSERT INTO IACM_UAT.[dbo].[DI_SAP_TCODEINFO]
           ([TCODE_Code]
           ,[TCODE_Desc]
           ,
           ,[EndPointSystemId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
 
 SELECT 
           
           [TCODE_Code]
           ,[TCODE_Desc]
           ,
           ,5
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
 FROm  SAP.[dbo].[DI_SAP_TCODEINFO]

 GO
PRINT '[DI_SAP_SINGLEROLEINFO]'

INSERT INTO IACM_UAT.[dbo].[DI_SAP_SINGLEROLEINFO]
           ([SingleRole_Code]
           ,[SingleRole_Desc]
           ,
           ,[EndPointSystemId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
    SELECT

	[SingleRole_Code]
           ,[SingleRole_Desc]
           ,
           ,5
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]

		   FROM SAP.[dbo].[DI_SAP_SINGLEROLEINFO]

GO
PRINT '[DI_SAP_RoleToAuthMap]'

INSERT INTO IACM_UAT.[dbo].[DI_SAP_RoleToAuthMap]
           ([RoleName]
           ,[ObjectCode]
           ,[AuthCode]
           ,[Variant]
           ,[BiwMenuCounter]
           ,[FieldCode]
           ,[ProfileGeneratorVariant]
           ,[FromValue]
           ,[ToValue]
           ,
           ,[EndPointSystemId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
    SELECT

	[RoleName]
           ,[ObjectCode]
           ,[AuthCode]
           ,[Variant]
           ,[BiwMenuCounter]
           ,[FieldCode]
           ,[ProfileGeneratorVariant]
           ,[FromValue]
           ,[ToValue]
           ,
           ,5
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]

FROM  SAP.[dbo].[DI_SAP_RoleToAuthMap]

GO
PRINT '[DI_SAP_ROLEINFO]'


INSERT INTO IACM_UAT.[dbo].[DI_SAP_ROLEINFO]
           (
           ,[EndPointSystemId]
           ,[ROLE_CODE]
           ,[ROLE_DESC]
           ,[TargetSysId]
           ,[RoleType]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
    SELECT


           ,5
           ,[ROLE_CODE]
           ,[ROLE_DESC]
           ,[TargetSysId]
           ,[RoleType]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]

FROM  SAP.[dbo].[DI_SAP_ROLEINFO]


GO
PRINT '[DI_SAP_PROFILEINFO]'

INSERT INTO IACM_UAT.[dbo].[DI_SAP_PROFILEINFO]
           (
           ,[EndPointSystemId]
           ,[PROFILE_CODE]
           ,[PROFILE_DESC]
           ,[TargetSysId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
    
SELECT

           ,5
           ,[PROFILE_CODE]
           ,[PROFILE_DESC]
           ,[TargetSysId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
FROM  SAP.[dbo].[DI_SAP_PROFILEINFO]

GO
PRINT '[DI_SAP_PARAMETERINFO]'


INSERT INTO IACM_UAT.[dbo].[DI_SAP_PARAMETERINFO]
           ([PARAM_CODE]
           ,
           ,[EndPointSystemId]
           ,[PARAM_DESC]
           ,[PARAM_VALUE]
           ,[TargetSysId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
    SELECT
	[PARAM_CODE]
           ,
           ,5
           ,[PARAM_DESC]
           ,[PARAM_VALUE]
           ,[TargetSysId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
	FROM  SAP.[dbo].[DI_SAP_PARAMETERINFO]

GO
PRINT '[DI_SAP_ObjectValues]'



INSERT INTO IACM_UAT.[dbo].[DI_SAP_ObjectValues]
           ([SapObjectType]
           ,[SapObjectCode]
           ,[SapOkFlag]
           ,[ModificationID]
           ,[OrgName]
           ,[EndPointSystemId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
   SELECT
   [SapObjectType]
           ,[SapObjectCode]
           ,[SapOkFlag]
           ,[ModificationID]
           ,[OrgName]
           ,5
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]

   FROm   SAP.[dbo].[DI_SAP_ObjectValues]
   GO
PRINT '[DI_SAP_Objects]'


INSERT INTO IACM_UAT.[dbo].[DI_SAP_Objects]
           ([SapObjectCode]
           ,[FieldCode1]
           ,[FieldCode2]
           ,[FieldCode3]
           ,[FieldCode4]
           ,[FieldCode5]
           ,[FieldCode6]
           ,[FieldCode7]
           ,[FieldCode8]
           ,[FieldCode9]
           ,[FieldCode0]
           ,[EndPointSystemId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
    SELECT 
			[SapObjectCode]
           ,[FieldCode1]
           ,[FieldCode2]
           ,[FieldCode3]
           ,[FieldCode4]
           ,[FieldCode5]
           ,[FieldCode6]
           ,[FieldCode7]
           ,[FieldCode8]
           ,[FieldCode9]
           ,[FieldCode0]
           ,5
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
	FROM  SAP.[dbo].[DI_SAP_Objects]

	GO
PRINT '[DI_SAP_GROUPINFO]'

INSERT INTO IACM_UAT.[dbo].[DI_SAP_GROUPINFO]
           (
           ,[EndPointSystemId]
           ,[GROUP_CODE]
           ,[GROUP_DESC]
           ,[TargetSysId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
    SELECT 
	
           ,5
           ,[GROUP_CODE]
           ,[GROUP_DESC]
           ,[TargetSysId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
	FROM  SAP.[dbo].[DI_SAP_GROUPINFO]


	GO
PRINT '[DI_SAP_FIELDVALUEMAPPING]'

INSERT INTO IACM_UAT.[dbo].[DI_SAP_FIELDVALUEMAPPING]
           ([RoleCode]
           ,[FieldCode]
           ,[FromValue]
           ,[ToValue]
           ,[EndPointSystemId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
    SELECT 
	[RoleCode]
           ,[FieldCode]
           ,[FromValue]
           ,[ToValue]
           ,5
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
	FROM  SAP.[dbo].[DI_SAP_FIELDVALUEMAPPING]

	GO
PRINT '[DI_SAP_COMPOSITEROLEINFO]'

	
INSERT INTO IACM_UAT.[dbo].[DI_SAP_COMPOSITEROLEINFO]
           ([CompositeRole_Code]
           ,[CompositeRole_Desc]
           ,
           ,[EndPointSystemId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
   SELECT 
 [CompositeRole_Code]
           ,[CompositeRole_Desc]
           ,
           ,5
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
   FROM  SAP.[dbo].[DI_SAP_COMPOSITEROLEINFO]

   GO
PRINT '[DI_SAP_COMPOSITEPROFILEINFO]'

   
INSERT INTO IACM_UAT.[dbo].[DI_SAP_COMPOSITEPROFILEINFO]
           (
           ,[EndPointSystemId]
           ,[PROFILE_CODE]
           ,[PROFILE_DESC]
           ,[TargetSysId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[SUBProfile_Code])
     SELECT 
	 
           ,5
           ,[PROFILE_CODE]
           ,[PROFILE_DESC]
           ,[TargetSysId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[SUBProfile_Code]
	 FROM  SAP.[dbo].[DI_SAP_COMPOSITEPROFILEINFO]

	 GO
PRINT '[DI_SAP_AuthorizationProfile]'

	 
INSERT INTO IACM_UAT.[dbo].[DI_SAP_AuthorizationProfile]
           ([ProfileCode]
           ,[AuthorizationCode]
           ,[EndPointSystemId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[ObjectCode])
     SELECT 
	 [ProfileCode]
           ,[AuthorizationCode]
           ,5
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
           ,[ObjectCode]
	 FROM   SAP.[dbo].[DI_SAP_AuthorizationProfile]


	 GO
PRINT '[DI_SAP_AuthObjectMap]'

	 
INSERT INTO IACM_UAT.[dbo].[DI_SAP_AuthObjectMap]
           ([AuthorizationCode]
           ,[ObjectCode]
           ,[FieldCode]
           ,[FromValue]
           ,[ToValue]
           ,[EndPointSystemId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate])
    SELECT 
	[AuthorizationCode]
           ,[ObjectCode]
           ,[FieldCode]
           ,[FromValue]
           ,[ToValue]
           ,5
           ,[CreatedBy]
           ,[CreatedDate]
           ,[ModifiedBy]
           ,[ModifiedDate]
	FROM  SAP.[dbo].[DI_SAP_AuthObjectMap] 


	GO
PRINT '[SAPUserinfoSAPGroupInfo]'



INSERT INTO IACM_UAT.[dbo].[SAPUserinfoSAPGroupInfo]
           ([SAPGroupInfo_Id]
           ,[SAPUserinfo_Id]
           ,[EndpointSystemId])
     SELECT 
	 [SAPGroupInfo_Id]
           ,[SAPUserinfo_Id]
           ,5
	 FROM  SAP.[dbo].[SAPGroupInfoSAPUserinfo]

GO
PRINT '[SAPParameterInfoSAPUserinfo]'


INSERT INTO IACM_UAT.[dbo].[SAPParameterInfoSAPUserinfo]
           ([SAPParameterInfo_Id]
           ,[SAPUserinfo_Id]
           ,[EndpointSystemId])
     SELECT
	 [SAPParameterInfo_Id]
           ,[SAPUserinfo_Id]
           ,5
	 FROM  SAP.[dbo].[SAPParameterInfoSAPUserinfo]

	 GO
PRINT '[SAPProfileInfoSAPUserinfo]'

INSERT INTO IACM_UAT.[dbo].[SAPProfileInfoSAPUserinfo]
           ([SAPProfileInfo_Id]
           ,[SAPUserinfo_Id]
           ,[EndpointSystemId])
    SELECT 
	[SAPProfileInfo_Id]
           ,[SAPUserinfo_Id]
           ,5
	FROM  SAP.[dbo].[SAPProfileInfoSAPUserinfo]

GO
PRINT '[SAPSingleRoleInfoSAPCompositeRoleInfo]'


INSERT INTO IACM_UAT.[dbo].[SAPSingleRoleInfoSAPCompositeRoleInfo]
           ([SAPSingleRoleInfo_Id]
           ,[SAPCompositeRoleInfo_CompositeRole_Id]
           ,[EndpointSystemId])
    SELECT 
	[SAPSingleRoleInfo_Id]
           ,[SAPCompositeRoleInfo_CompositeRole_Id]
           ,5
	FROM  SAP.[dbo].[SAPSingleRoleInfoSAPCompositeRoleInfo]

	GO
PRINT '[SAPTCodeInfoSAPSingleRoleInfo]'


INSERT INTO IACM_UAT.[dbo].[SAPTCodeInfoSAPSingleRoleInfo]
           ([SAPTCodeInfo_Id]
           ,[SAPSingleRoleInfo_Id]
           ,[EndpointSystemId])
   SELECT 
			[SAPTCodeInfo_Id]
           ,[SAPSingleRoleInfo_Id]
           ,5
		   FROM  SAP.[dbo].[SAPTCodeInfoSAPSingleRoleInfo]
		   GO
PRINT '[SAPRoleInfoSAPUserinfo]'


INSERT INTO IACM_UAT.[dbo].[SAPRoleInfoSAPUserinfo]
           ([SAPUserinfo_Id]
           ,[SAPRoleInfo_Id]
           ,[EndpointSystemId])
    SELECT 

	[SAPUserinfo_Id]
           ,[SAPRoleInfo_Id]
           ,5

		   FROM  SAP.[dbo].[SAPUserinfoSAPRoleInfo]

