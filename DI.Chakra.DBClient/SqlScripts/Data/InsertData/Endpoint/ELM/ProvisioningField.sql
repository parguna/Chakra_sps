



DECLARE @EPTYPEId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=32
INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [PrimaryIdentifier], [ResolverType], [FieldScope], [CanDerive], [Description], [Value], [IsMandatory], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (28, NULL, 1, N'ADID', 1, N'UserTypeResolver', N'None', 0, N'User Name', N'UserName', 1, 1, N'IACMAdmin', GETDATE(), N'IACMAdmin', GETDATE())

GO


DECLARE @EPTYPEId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=32

DECLARE @CreateAc_Id bigint, @DeleteAc_Id bigint, @DisableAc_Id bigint, @LockAc_Id bigint, @UnlockAc_Id bigint, @EnableAc_Id bigint, @ResetPwd_Id bigint, @AddEnt_Id bigint, @RemoveEnt_Id bigint, @UpdateAc_Id bigint

SELECT @DeleteAc_Id =Id FROM [DI_M_NProvisioningSubProcess] WHERE [Name]='DeleteAccount'
--INSERT [dbo].[DI_M_NProvisioningEventProcess] ([EndPointSystemTypeId], [EndPointSystemId], [EventType], [SubEventType], [ProvisioningEndpointProcessName], [ProvisioningCategoryType], [ProvisioningCategory], [ProvisioningAction], [AccountType], [Order], [BreakWhenFailed], [IsMandatory], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (28, NULL, N'OffBoarding', N'', N'RetainRole', N'All', N'Role', N'Retain', N'User', 1, 1, 1, 1, N'System', CAST(N'2017-02-17 14:19:07.100' AS DateTime), N'System', CAST(N'2017-02-17 14:19:07.100' AS DateTime))
INSERT [dbo].[DI_M_NProvisioningEventProcess] ([EndPointSystemTypeId], [EndPointSystemId], [EventType], [SubEventType], [ProvisioningEndpointProcessName], [ProvisioningCategoryType], [ProvisioningCategory], [ProvisioningAction], [AccountType], [Order], [BreakWhenFailed], [IsMandatory], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@EPTYPEId, NULL, N'OffBoarding', N'', N'AccountRemove', N'All', N'Account', N'Remove', N'User', 2, 1, 1, 1, N'System', CAST(N'2017-02-17 14:19:07.100' AS DateTime), N'System', CAST(N'2017-02-17 14:19:07.100' AS DateTime))
INSERT [dbo].[DI_M_NProvisioningEndpointTriggerProcess] ([EndPointSystemTypeId], [EndPointSystemId], [ProvisioningEndpointProcessName], [AccountType], [SubProcessId], [CustomProcessId], [SubProcessType], [Order], [BreakWhenFailed], [IsMandatory], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@EPTYPEId, NULL, N'AccountRemove', N'User', @DeleteAc_Id, NULL, N'General', 1, 1, 1, 1, N'IACMAdmin', CAST(N'2017-02-17 14:19:07.110' AS DateTime), N'IACMAdmin', CAST(N'2017-02-17 14:19:07.110' AS DateTime))
GO