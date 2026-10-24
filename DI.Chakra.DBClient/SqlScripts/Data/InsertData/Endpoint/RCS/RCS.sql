
SET IDENTITY_INSERT [DI_M_EndpointSystemType] ON
GO

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=35)
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id],[EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],[ObjectName],[CanBeTrustedSource]) 
	VALUES (35,35, 1, N'System', getdate(), N'System', getdate(), N'ResortsCardSystem', N'ResortsCardSystem','RCSUserInfo',1)

GO
SET IDENTITY_INSERT [DI_M_EndpointSystemType] OFF

DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=35

INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@TypeId, N'User', N'System', getdate(), N'System', getdate())
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@TypeId, N'Role', N'System', getdate(), N'System', getdate())
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@TypeId, N'Function', N'System', getdate(), N'System', getdate())
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES (@TypeId, N'PrivillegeAcctTrans', N'System', getdate(), N'System', getdate())
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES (@TypeId, N'CopyFileLocalandRead', N'System', getdate(), N'System', getdate())
GO


DECLARE @EndpointSystemTypeId bigint
DECLARE @KeyId bigint

SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='ResortsCardSystem'


SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='BizServiceUserName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,1,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),NULL, NULL)

SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='BizServicePassword'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,2,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),NULL, NULL)

SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='UserFilePath'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,3,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),NULL, NULL)
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='RoleFilePath'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,4,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),NULL, NULL)
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='FunctionFilePath'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,5,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),NULL, NULL)
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='PrivillegeAcctTransFilePath'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,6,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),NULL, NULL)


--IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'RCSRoleInfoRCSFunctionInfo'))
--	ALTER TABLE BizCMSRoleInfoBizCMSFunctionInfo  ADD EndpointSystemId BIGINT NULL, [Version] INT NOT NULL  DEFAULT 1
--IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'RCSUserInfoRCSRoleInfo'))
--	ALTER TABLE BizCMSUserInfoBizCMSRoleInfo  ADD EndpointSystemId BIGINT NULL, [Version] INT NOT NULL  DEFAULT 1


INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'AccountName','Account Name','ADID',1,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,NULL,'RoleCode','RoleCode','RoleCode',2,'System',GETDATE(),'System', GETDATE())
-- End

GO



DECLARE @EndpointSystemTypeId bigint
DECLARE @KeyId bigint


SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='ResortsCardSystem'
INSERT [dbo].DI_M_EndPointAutoExpiry (EndpointSystemTypeId,EndPointSystemId,IsAccountAutoExpirySupported,IsEntitlementAutoExpirySupported,CreatedDate)VALUES (@EndpointSystemTypeId,NULL,0,0,GETDATE())


GO	
