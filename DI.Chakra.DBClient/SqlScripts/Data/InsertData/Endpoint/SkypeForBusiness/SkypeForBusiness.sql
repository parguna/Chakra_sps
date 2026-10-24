 Set Identity_Insert [dbo].[DI_M_EndpointSystemType] ON

	IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=30)
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id],[EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],[ObjectName],[CanBeTrustedSource]) 
	VALUES (30,30, 1, N'System', getdate(), N'System', getdate(), N'SkypeForBusiness', N'Skype For Business',N'SkypeForBusinessUserInfo',0)
 Set Identity_Insert [dbo].[DI_M_EndpointSystemType] OFF
 GO
DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=30

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'User' )
	INSERT INTO DI_M_EndpointObjects(EndpointSystemTypeId, ObjectName, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, 'User', N'System', getdate(), N'System', getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'ResponseGroup' )
	INSERT INTO DI_M_EndpointObjects(EndpointSystemTypeId, ObjectName, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, 'ResponseGroup', N'System', getdate(), N'System', getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'UnAssignedNumberRange' )
	INSERT INTO DI_M_EndpointObjects(EndpointSystemTypeId, ObjectName, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, 'UnAssignedNumberRange', N'System', getdate(), N'System', getdate())

Set Identity_Insert [dbo].[DI_M_EndpointConfigurationKey] ON
INSERt INTO [DI_M_EndpointConfigurationKey](Id,KeyName,Caption,CreatedBy,CreatedDate)Values(50,'ConnectionUri','ConnectionUri','System',GETDATE())
INSERt INTO [DI_M_EndpointConfigurationKey](Id,KeyName,Caption,CreatedBy,CreatedDate)Values(51,'AzueADUserName','AzueADUserName','System',GETDATE())
INSERt INTO [DI_M_EndpointConfigurationKey](Id,KeyName,Caption,CreatedBy,CreatedDate)Values(52,'AzureADUserPassword','AzureADUserPassword','System',GETDATE())
Set Identity_Insert [dbo].[DI_M_EndpointConfigurationKey] OFF

DECLARE @EndpointSystemTypeId bigint
DECLARE @KeyId bigint

-- SFB
SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='SkypeForBusiness'
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='ConnectionUri'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,1,'varchar','',1,'', 'System',GETDATE(),'System', GETDATE())

SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='UserName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,3,'varchar','',1,'', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='Password'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,4,'varchar','',1,'onkeypress:return Preventspaces(event);', 'System',GETDATE(),'System', GETDATE())


SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='AzueADUserName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,5,'varchar','',1,'', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='AzureADUserPassword'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,6,'varchar','',1,'onkeypress:return Preventspaces(event);', 'System',GETDATE(),'System', GETDATE())

-- End