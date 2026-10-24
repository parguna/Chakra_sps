
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=21)
Begin
 Set Identity_Insert [dbo].[DI_M_EndpointSystemType] ON
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id], [EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],ObjectName,[CanBeTrustedSource]) 
	VALUES (21, 21,1, N'System', getdate(), N'System', getdate(), N'Peoplesoft', N'Peoplesoft',N'PeopleSoftUserInfo','1')
Set Identity_Insert [dbo].[DI_M_EndpointSystemType] OFF
End


DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=21

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'User' )
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'User', N'System', getdate(), N'System', getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Role' )
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Role', N'System', getdate(), N'System', getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Permission' )
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Permission', N'System', getdate(), N'System', getdate())
--IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Menu' )
--INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Menu', N'System', getdate(), N'System', getdate())
--IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Portal' )
--INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Portal', N'System', getdate(), N'System', getdate())
----IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Role class' )
----INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Role class', N'System', getdate(), N'System', getdate())
--IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Access log' )
--INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Access log', N'System', getdate(), N'System', getdate())

ALTER TABLE PeopleSoftUserInfoPeopleSoftRoleInfo  ADD EndpointSystemId BIGINT NULL, [Version] INT NOT NULL  DEFAULT 1

ALTER TABLE PeopleSoftRoleInfoPeopleSoftPermissionInfo  ADD EndpointSystemId BIGINT NULL, [Version] INT NOT NULL  DEFAULT 1

DECLARE @EndpointSystemTypeId bigint
DECLARE @KeyId bigint


-- Peoplesoft User Management
SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='Peoplesoft'
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='IPAddress'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,1,'varchar','[0-9\.\ \-]*',1,'onkeypress:return IPAddressKeyOnly(event);#onblur:return ValidateIPaddress(this)', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='InstanceName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,2,'varchar','',0,'', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='PortNo'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,3,'varchar','',0,'', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='UserName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,4,'varchar','',1,'', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='Password'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,5,'varchar','',1,'onkeypress:return Preventspaces(event);', 'System',GETDATE(),'System', GETDATE())
-- End

INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'OPRID','Account Name','OPRID',1,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'AccountStatus','Account Status','IsLocked',2,'System',GETDATE(),'System', GETDATE())
-- End
