IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'SAPEPRoleInfoSAPEPUserInfo'))
	ALTER TABLE SAPEPRoleInfoSAPEPUserInfo  ADD EndpointSystemId BIGINT NULL
GO
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'SAPEPUserInfoSAPEPGroupInfo'))
	ALTER TABLE SAPEPUserInfoSAPEPGroupInfo  ADD EndpointSystemId BIGINT NULL
GO

SET IDENTITY_INSERT [DI_M_EndpointSystemType] ON
GO

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=4)
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id],[EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],[ObjectName],[CanBeTrustedSource]) 
	VALUES (4,4, 1, N'System', getdate(), N'System', getdate(), N'Portals (EP / CE)', N'Portals (EP / CE)','SAPEPUserInfo',0)

GO
SET IDENTITY_INSERT [DI_M_EndpointSystemType] OFF

DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=4

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'User' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'User',N'System', GETDATE(), N'System', GETDATE())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Role' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Role',N'System', GETDATE(), N'System', GETDATE())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Group' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Group',N'System', GETDATE(), N'System', GETDATE())
GO


DECLARE @EndpointSystemTypeId bigint
DECLARE @KeyId bigint

-- Portal
SELECT @EndpointSystemTypeId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE DisplayName ='Portals (EP / CE)'
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='DomainName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,1,'varchar','',1,'onkeypress:return allowAlphabetswithdots(event);', 'System',GETDATE(),'System', GETDATE())

SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='UserName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,2,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='Password'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,3,'varchar','',1,'onkeypress:return Preventspaces(event);', 'System',GETDATE(),'System', GETDATE())

SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='WSDLUrl'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,4,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),'System', GETDATE())

