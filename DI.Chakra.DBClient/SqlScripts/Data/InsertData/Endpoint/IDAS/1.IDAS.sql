IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=26)
Begin
 Set Identity_Insert [dbo].[DI_M_EndpointSystemType] ON
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id], [EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],ObjectName,[CanBeTrustedSource]) 
	VALUES (26, 26,1, N'System', getdate(), N'System', getdate(), N'IDAS', N'IDAS',N'User','0')
Set Identity_Insert [dbo].[DI_M_EndpointSystemType] OFF
End

--IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystem] WHERE DisplayName='IDAS')
--Begin
--	INSERT [dbo].[DI_M_EndpointSystem] ( [Description], [EndpointSystemTypeId], [DisplayName], [OwnerUserId], [SupportPersonId1], [SupportPersonId2], [CoreSystemIndicator], [IsMockEndPoint], [ApplicationCode], [IsActive], [IsTrustedSource], [IsPasswordResetAllowed], [IncludeInAttestation], [UserObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [SubSystemTypeId], [ITDistributionList], [LastUploadedDate], [MailSentDate])  
--	VALUES (N'',26,N'IDAS',1,NUll,NUll,0,0,N'IDAS',1,0,0,0,N'',N'IACMAdmin',getdate(),N'ÍACMAdmin',getdate(),NULL,N'',NULL,NULL)
--End

DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=26

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'User' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'User', N'System', getdate(), N'System', getdate())

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Role' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Role',N'System', GETDATE(), N'System', GETDATE())

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'Group' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'Group',N'System', GETDATE(), N'System', GETDATE())



--IDAS Endppoint Configuration--

DECLARE @EPId bigint
DECLARE @EPConfigurationId bigint
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE [EndpointSystemTypeId]=26
insert into DI_M_EndpointConfiguration ([EndpointSystemId],[MapAll],[CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate]) values (@EPId,'1','System',getdate(),'System',getdate())

-- IDAS--

--DECLARE @EndpointSystemTypeId bigint
--DECLARE @KeyId bigint

--SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='IDAS'
--SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='Instruction'
--INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
--VALUES											 (@EndpointSystemTypeId,@KeyId,1,'label','',0,'This endpoint will use the default IDAS configuration', 'System',GETDATE(),'System', GETDATE())
---- End


-- IDAS
DECLARE @EndpointSysTypeId bigint

SELECT @EndpointSysTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='IDAS'
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value], [Order], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSysTypeId,NULL,'EmployeeId','Employee ID','EmployeeId',1,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value], [Order],[CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSysTypeId,NULL,'AccountCreatedDate','Account Created Date','AccountCreatedDate',2,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSysTypeId,NULL,'IsDisabled','IsDisabled','IsDisabled',3,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value], [Order],[CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSysTypeId,NULL,'IsDeleted','IsDeleted','IsDeleted',4,'System',GETDATE(),'System', GETDATE())
INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSysTypeId,NULL,'Email','E-Mail','Email',5,'System',GETDATE(),'System', GETDATE())

---------Account Recooncilation entry for IACMADMIN

SET IDENTITY_INSERT [dbo].[DI_M_AccountReconcilation] ON 

--IF NOT EXISTS (SELECT * FROM [DI_M_AccountReconcilation] WHERE Id=1 AND [CreatedBy] = 'System')
--	INSERT [dbo].[DI_M_AccountReconcilation] ([Id], [EndpointSystemId], [EndpointUserId], [UserId], [AccountName], [FullName], [AccountCreatedDate], [IsDeleted], [DeletedDate], [IsResolved], [ResolvedBy], [Comments], [ExceptionType], [IsLocked], [LockScenario], [AccountType], [MappingType], [OffboardingAccountAction], [ProvisionedThrough], [IsSODConflict], [IsSensitiveAccess], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
--	VALUES (1,@EPId,1,1,'IACMAdmin','IACM Administrator',GETDATE(),0,NULL,0,NULL,NULL,0,0,0,0,0,0,NULL,0,0,'System', GETDATE(), 'System', GETDATE())

SET IDENTITY_INSERT [dbo].[DI_M_AccountReconcilation] OFF

---------Entitlement Reconcilation entry for IACMADMIN

SET IDENTITY_INSERT [dbo].[DI_M_EntitlementReconcilation] ON 

DECLARE @AccountRconId bigint

--SELECT @AccountRconId =Id FROM [DI_M_AccountReconcilation] WHERE [EndpointUserId]=1
--IF NOT EXISTS (SELECT * FROM [DI_M_EntitlementReconcilation] WHERE Id=1 AND [CreatedBy] = 'System')
--	INSERT [dbo].[DI_M_EntitlementReconcilation] ([Id], [AccountReconcilationId], [EndpointType], [EndpointSystemId], [EndpointUserId], [UserId], [EntitlementId], [EntitlementType], [AccountName], [Entitlement], [EntitlementDescription], [EnteredDate], [IsDeleted], [DeletedDate], [IsResolved], [ResolvedBy], [Comments], [ExceptionType], [ProvisionedThrough], [IsSODConflict], [IsSensitiveAccess], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
--	VALUES (1,@AccountRconId,26,@EPId,1,1,1,1,'IACMAdmin','Administrator','Administrator',GETDATE(),0,NULL,0,NULL,NULL,0,NULL,0,0,'System', GETDATE(), 'System', GETDATE())

SET IDENTITY_INSERT [dbo].[DI_M_AccountReconcilation] OFF


GO
