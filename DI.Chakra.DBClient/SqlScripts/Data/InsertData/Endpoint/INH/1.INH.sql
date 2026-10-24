IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'InhouseUserInfoInhousePermission'))
	ALTER TABLE InhouseUserInfoInhousePermission  ADD EndpointSystemId BIGINT NULL, [Version] INT NOT NULL  DEFAULT 1

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'EndpointSystemId' AND Object_ID = Object_ID(N'InhousePermissionInhouseFunction'))
	ALTER TABLE InhousePermissionInhouseFunction  ADD EndpointSystemId BIGINT NULL, [Version] INT NOT NULL  DEFAULT 1

SET IDENTITY_INSERT [DI_M_EndpointSystemType] ON
GO

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=9)
	INSERT [dbo].[DI_M_EndpointSystemType] ( [Id],[EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],[ObjectName],[CanBeTrustedSource]) 
	VALUES ( 9,9, 1, N'System', getdate(), N'System', getdate(), N'In-house System', N'In-house System','InhouseUserInfo',0)
SET IDENTITY_INSERT [DI_M_EndpointSystemType] OFF
GO

DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=9

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'User' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'User', N'System', getdate(), N'System', getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Permission' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Permission', N'System', getdate(), N'System', getdate())
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Function' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Function', N'System', getdate(), N'System', getdate())

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'PrivillegeAcctTrans' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'PrivillegeAcctTrans', N'System', getdate(), N'System', getdate())





DECLARE @EndpointSystemTypeId bigint
DECLARE @KeyId bigint



-- Inhouse
SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='In-house System'
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='CanRequestMultipleRoles'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,1,'bit','',0,'', 'System',GETDATE(),'System', GETDATE())


GO

--IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystem] WHERE DisplayName='StandingOrder')
--	INSERT [dbo].[DI_M_EndpointSystem] ( [Description], [EndpointSystemTypeId], [DisplayName], [OwnerUserId], [SupportPersonId1], [SupportPersonId2], [CoreSystemIndicator], [IsMockEndPoint], [ApplicationCode], [IsActive], [IsTrustedSource], [IsPasswordResetAllowed], [IncludeInAttestation], [UserObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [SubSystemTypeId], [ITDistributionList], [LastUploadedDate], [MailSentDate])  
--	VALUES (N'',9,N'StandingOrder',1,NUll,NUll,0,1,N'SO',1,0,0,0,N'',N'IACMAdmin',getdate(),N'ÍACMAdmin',getdate(),NULL,N'',NULL,NULL)

	
--IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystem] WHERE DisplayName='RiskAssessment')
--	INSERT [dbo].[DI_M_EndpointSystem] ( [Description], [EndpointSystemTypeId], [DisplayName], [OwnerUserId], [SupportPersonId1], [SupportPersonId2], [CoreSystemIndicator], [IsMockEndPoint], [ApplicationCode], [IsActive], [IsTrustedSource], [IsPasswordResetAllowed], [IncludeInAttestation], [UserObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [SubSystemTypeId], [ITDistributionList], [LastUploadedDate], [MailSentDate])  
--	VALUES (N'',9,N'RiskAssessment',1,NUll,NUll,0,1,N'SO',1,0,0,0,N'',N'IACMAdmin',getdate(),N'ÍACMAdmin',getdate(),NULL,N'',NULL,NULL)


GO

IF NOT EXISTS (SELECT * FROM [DI_M_IMConfigurationMaster] WHERE ConfigKey='StandingOrder')
	INSERT [dbo].[DI_M_IMConfigurationMaster] ( [ConfigKey], [ConfigValue], [Description], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [IsUiDisplay])
	VALUES ( N'StandingOrder', N'StandingOrder', N'', N'System', getdate(), NULL, getdate(), 1)
	
IF NOT EXISTS (SELECT * FROM [DI_M_IMConfigurationMaster] WHERE ConfigKey='RiskAssessment')
	INSERT [dbo].[DI_M_IMConfigurationMaster] ( [ConfigKey], [ConfigValue], [Description], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [IsUiDisplay])
	VALUES ( N'RiskAssessment', N'RiskAssessment', N'', N'System', getdate(), NULL, getdate(), 1)


-- End