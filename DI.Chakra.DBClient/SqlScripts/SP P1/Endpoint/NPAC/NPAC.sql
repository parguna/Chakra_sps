IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'NPACUserInfoNPACLevelInfo'))
	ALTER TABLE NPACUserInfoNPACLevelInfo  ADD EndpointSystemId BIGINT NULL
GO

SET IDENTITY_INSERT [DI_M_EndpointSystemType] ON
GO

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=6)
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id],[EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],[ObjectName],[CanBeTrustedSource]) 
	VALUES (6,6, 1, N'System', getdate(), N'System', getdate(), N'NPAC/NMACS/ENIS/PROTIS/CIMS/MDMS', N'NPAC/NMACS/ENIS/PROTIS/CIMS/IAMS/UOS/MDMS','NPACUserInfo',0)

GO
SET IDENTITY_INSERT [DI_M_EndpointSystemType] OFF

DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=6

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'User' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'User',N'System', GETDATE(), N'System', GETDATE())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Level' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Level',N'System', GETDATE(), N'System', GETDATE())
	
GO


DECLARE @EndpointSystemTypeId bigint
DECLARE @KeyId bigint

-- NPAC
SELECT @EndpointSystemTypeId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE DisplayName ='NPAC/NMACS/ENIS/PROTIS/CIMS/IAMS/UOS/MDMS'
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='ServerName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,1,'varchar','',1,'onkeypress:return allowAlphabetswithdots(event);', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='DatabaseName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,2,'varchar','',1,'onkeypress:return allowAlphabetswithdots(event);', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='UserName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,3,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='Password'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,4,'varchar','',1,'onkeypress:return Preventspaces(event);', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='IsWindowsAuthentication'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,5,'bit','',0,'', 'System',GETDATE(),'System', GETDATE())

