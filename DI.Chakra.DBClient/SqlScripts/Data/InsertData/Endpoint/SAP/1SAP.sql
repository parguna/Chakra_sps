IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'SAPEPUserInfoSAPEPGroupInfo'))
	ALTER TABLE SAPEPUserInfoSAPEPGroupInfo ADD EndpointSystemId BIGINT NULL, [Version] INT NOT NULL DEFAULT 1
GO
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'SAPParameterInfoSAPUserinfo'))
	ALTER TABLE SAPParameterInfoSAPUserinfo ADD EndpointSystemId BIGINT NULL, [Version] INT NOT NULL DEFAULT 1
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'SAPProfileInfoSAPUserinfo'))
	ALTER TABLE SAPProfileInfoSAPUserinfo ADD EndpointSystemId BIGINT NULL, [Version] INT NOT NULL DEFAULT 1
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'SAPRoleInfoSAPUserinfo'))
	ALTER TABLE SAPRoleInfoSAPUserinfo ADD EndpointSystemId BIGINT NULL, [Version] INT NOT NULL  DEFAULT 1
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'Role_FromDate' AND Object_ID = Object_ID(N'SAPRoleInfoSAPUserinfo'))
	ALTER TABLE SAPRoleInfoSAPUserinfo ADD Role_FromDate nvarchar(MAX) NULL, Role_ToDate nvarchar(MAX) NULL
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'SAPRoleInfoSAPUserinfo'))
	alter table SAPRoleInfoSAPUserinfo drop constraint [PK_dbo.SAPRoleInfoSAPUserinfo]
GO
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'SAPSingleRoleInfoSAPCompositeRoleInfo'))
	ALTER TABLE SAPSingleRoleInfoSAPCompositeRoleInfo ADD EndpointSystemId BIGINT NULL, [Version] INT NOT NULL DEFAULT 1
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'SAPTCodeInfoSAPSingleRoleInfo'))
	ALTER TABLE SAPTCodeInfoSAPSingleRoleInfo ADD EndpointSystemId BIGINT NULL, [Version] INT NOT NULL DEFAULT 1
GO
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'SAPUserinfoSAPGroupInfo'))
	ALTER TABLE SAPUserinfoSAPGroupInfo ADD EndpointSystemId BIGINT NULL, [Version] INT NOT NULL DEFAULT 1
GO
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'IsDirectlyAttached' AND Object_ID = Object_ID(N'SAPRoleInfoSAPUserinfo'))
	Alter table SAPRoleInfoSAPUserinfo add IsDirectlyAttached bit null	
GO

SET IDENTITY_INSERT [DI_M_EndpointSystemType] ON
GO


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=3)
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id],[EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],[ObjectName],[CanBeTrustedSource]) 
	VALUES (3,3, 1, N'System', getdate(), N'System',getdate(), N'SAP', N'SAP','SAPUserinfo','1')

SET IDENTITY_INSERT [DI_M_EndpointSystemType] OFF
GO

DECLARE @TypeId BIGINT
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=3


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'User' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ( @TypeId, N'User', N'System', getdate(), N'System',getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Role' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ( @TypeId, N'Role', N'System', getdate(), N'System',getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Group' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ( @TypeId, N'Group', N'System', getdate(), N'System',getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Profile' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ( @TypeId, N'Profile', N'System', getdate(), N'System',getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'T-Code' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ( @TypeId, N'T-Code', N'System', getdate(), N'System',getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Parameter' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ( @TypeId, N'Parameter', N'System', getdate(), N'System',getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Authorization' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ( @TypeId, N'Authorization', N'System', getdate(), N'System',getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Position' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ( @TypeId, N'Position', N'System', getdate(), N'System',getdate())

GO

DECLARE @EndpointSystemTypeId bigint
DECLARE @KeyId bigint



-- SAP
SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='SAP'
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='IPAddress'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,1,'varchar','[0-9\.\ \-]*',1,'onkeypress:return IPAddressKeyOnly(event);#onblur:return ValidateIPaddress(this)', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='UserName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,2,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='Password'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,3,'varchar','',1,'onkeypress:return Preventspaces(event);', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='SystemNumber'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,4,'varchar','',1,'onKeyPress:return allowNumerics(event)', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='ClientNumber'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,5,'varchar','',1,'onKeyPress:return allowNumerics(event)', 'System',GETDATE(),'System', GETDATE())
-- End

INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value], [Order], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'UserLogonName','User Logon Name','UserLogonName',1,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'DisplayName','Display Name','Lastname',2,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'Department','Department','Department',3,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'EMPLOYEEID','EMPLOYEE ID','EMPLOYEEID',4,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'EMPLOYEETYPE','EMPLOYEE TYPE','EMPLOYEETYPE',5,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'Tel1_Numbr','Telephone Numbr','Tel1_Numbr',6,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'E_Mail','E Mail','E_Mail',7,'System',GETDATE(),'System', GETDATE())
-- End
GO
---SOD--- CHANGES
DECLARE @TypeId BIGINT
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=3

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'CriticalTCodeReport' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ( @TypeId, N'CriticalTCodeReport', N'System', getdate(), N'System',getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'ConflictingRoleReport' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ( @TypeId, N'ConflictingRoleReport', N'System', getdate(), N'System',getdate())
