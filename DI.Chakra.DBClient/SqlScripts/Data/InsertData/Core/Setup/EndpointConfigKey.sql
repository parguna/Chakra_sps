DBCC CHECKIDENT ('[DI_M_EndpointConfigurationKey]', RESEED, 1);
GO


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'IPAddress' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('IPAddress','IP Address', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'DomainName' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('DomainName','Domain Name', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'UserName' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('UserName','User Name', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'Password' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('Password','Password', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'Ou' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('Ou','OU', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'MailDomain' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('MailDomain','Mail Domain', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'DenyAllAccessGroupName' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('DenyAllAccessGroupName','Deny All Access GroupName', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'IDVaultAdmin' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('IDVaultAdmin','ID Vault Admin Name', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'IDVaultAdminFile' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('IDVaultAdminFile','ID Vault Admin File Value', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'MailQuotaThreshold' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('MailQuotaThreshold','Mail Quota Threshold (MB)', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'SystemNumber' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('SystemNumber','System Number', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'ClientNumber' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('ClientNumber','Client Number', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'CanRequestMultipleRoles' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('CanRequestMultipleRoles','Can Request Multiple Roles', 'System',GETDATE(),'System', GETDATE())



-- ORCALE EBS


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'HostName' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('HostName','Host Name', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'InstanceName' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('InstanceName','Instance Name', 'System',GETDATE(),'System', GETDATE())


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'PortNo' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('PortNo','Port No', 'System',GETDATE(),'System', GETDATE())
-- End

-- SAPEP


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'WSDLURL' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('WSDLURL','WSDLURL', 'System',GETDATE(),'System', GETDATE())
-- End
--INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
--VALUES											 ('','', 'System',GETDATE(),'System', GETDATE())


--FILEHRMS - START


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'EmployeeFilePath' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('EmployeeFilePath','Employee FilePath', 'System',GETDATE(),NULL, NULL)


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'OrganisationFilePath' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('OrganisationFilePath','Organisation FilePath', 'System',GETDATE(),NULL, NULL)


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'BusinessRoleFilePath' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('BusinessRoleFilePath','BusinessRole FilePath', 'System',GETDATE(),NULL, NULL)

--FILEHRMS - END
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'SSL' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('SSL','SSL', 'System',GETDATE(),'System', GETDATE())

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'IsWOG' and [CreatedBy] = 'System')
INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES('IsWOG','Is WOG', 'System',GETDATE(),'System', GETDATE())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'IsOneWayTrust' and [CreatedBy] = 'System')
INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES('IsOneWayTrust','Is OneWay Trust', 'System',GETDATE(),'System', GETDATE())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'TrustSource' and [CreatedBy] = 'System')
INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES('TrustSource','Trust Source', 'System',GETDATE(),'System', GETDATE())

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'ServerName' and [CreatedBy] = 'System')
INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES('ServerName','Server Name', 'System',GETDATE(),'System', GETDATE())

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'DatabaseName' and [CreatedBy] = 'System')
INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES('DatabaseName','Database Name', 'System',GETDATE(),'System', GETDATE())

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'WebAPI' and [CreatedBy] = 'System')
INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES('WebAPI','Web API', 'System',GETDATE(),'System', GETDATE())

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'Instruction' and [CreatedBy] = 'System')
INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES('Instruction','Instruction', 'System',GETDATE(),'System', GETDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[DI_M_EndpointConfigurationKey] WHERE KEYNAME = 'SecondaryIPAddress')
INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName],[Caption] ,[CreatedBy] ,[CreatedDate] ,[LastModifiedBy] ,[LastModifiedDate])
     VALUES ('SecondaryIPAddress' ,'Secondary IP Address','System'  ,GETDATE(),'System'  ,GETDATE())

IF NOT EXISTS (SELECT * FROM [dbo].[DI_M_EndpointConfigurationKey] WHERE KEYNAME = 'IsWindowsAuthentication')
INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName],[Caption] ,[CreatedBy] ,[CreatedDate] ,[LastModifiedBy] ,[LastModifiedDate])
     VALUES ('IsWindowsAuthentication' ,'Is Windows Authentication','System'  ,GETDATE(),'System'  ,GETDATE())