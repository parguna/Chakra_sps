--DECLARE @EPId bigint
--SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE [EndpointSystemTypeId]=13
--GO

DECLARE @EPId bigint
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE [EndpointSystemTypeId]=13

INSERT [dbo].[DI_M_NProvisioningField] ([EndpointsystemId], [FieldName], [CanDerive], [Description], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [PrimaryIdentifier], [ResolverType], [FieldScope], [Value],[IsMandatory],[AccountType]) VALUES (@EPId, N'AccountName', 0, NULL, 1,  N'System', GETDATE(), N'System', GETDATE(), 1, N'RuleSetResolver', N'None', N'2',1,1)
INSERT [dbo].[DI_M_NProvisioningField] ([EndpointsystemId], [FieldName], [CanDerive], [Description], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [PrimaryIdentifier], [ResolverType], [FieldScope], [Value],[IsMandatory],[AccountType]) VALUES (@EPId, N'FullName', 0, NULL, 1,  N'System', GETDATE(), N'System', GETDATE(), 0, N'', N'', NULL,1,1)
INSERT [dbo].[DI_M_NProvisioningField] ([EndpointsystemId], [FieldName], [CanDerive], [Description], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [PrimaryIdentifier], [ResolverType], [FieldScope], [Value],[IsMandatory],[AccountType]) VALUES (@EPId, N'Password', 0, NULL, 1,  N'System', GETDATE(), N'System', GETDATE(), 0, N'PasswordResolver', N'None', N'@bcd12345',1,1)
INSERT [dbo].[DI_M_NProvisioningField] ([EndpointsystemId], [FieldName], [CanDerive], [Description], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [PrimaryIdentifier], [ResolverType], [FieldScope], [Value],[IsMandatory],[AccountType]) VALUES (@EPId, N'Description',0, NULL, 1,  N'System', GETDATE(), N'System', GETDATE(), 0, N'UserTypeResolver', N'Update', N'NOrganisation.Name',1,1)
INSERT [dbo].[DI_M_NProvisioningField] ([EndpointsystemId], [FieldName], [CanDerive], [Description], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [PrimaryIdentifier], [ResolverType], [FieldScope], [Value],[IsMandatory],[AccountType]) VALUES (@EPId, N'Groups',1, NULL, 1,  N'System', GETDATE(), N'System', GETDATE(), 0, N'None', N'None', NULL,1,1)
GO

DECLARE @EPId bigint
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE [EndpointSystemTypeId]=13

DECLARE @CreateAc_Id bigint, @DeleteAc_Id bigint, @DisableAc_Id bigint, @LockAc_Id bigint, @UnlockAc_Id bigint, @EnableAc_Id bigint, @ResetPwd_Id bigint, @AddEnt_Id bigint, @RemoveEnt_Id bigint, @UpdateAc_Id bigint
SELECT @CreateAc_Id =Id FROM [DI_M_NProvisioningSubProcess] WHERE [Name]='CreateAccount'
SELECT @DeleteAc_Id =Id FROM [DI_M_NProvisioningSubProcess] WHERE [Name]='DeleteAccount'
SELECT @DisableAc_Id =Id FROM [DI_M_NProvisioningSubProcess] WHERE [Name]='DisableAccount'
SELECT @LockAc_Id =Id FROM [DI_M_NProvisioningSubProcess] WHERE [Name]='LockAccount'
SELECT @UnlockAc_Id =Id FROM [DI_M_NProvisioningSubProcess] WHERE [Name]='UnLockAccount'
SELECT @EnableAc_Id =Id FROM [DI_M_NProvisioningSubProcess] WHERE [Name]='EnableAccount'
SELECT @ResetPwd_Id =Id FROM [DI_M_NProvisioningSubProcess] WHERE [Name]='ResetPassword'
SELECT @AddEnt_Id =Id FROM [DI_M_NProvisioningSubProcess] WHERE [Name]='AddEntitlement'
SELECT @RemoveEnt_Id =Id FROM [DI_M_NProvisioningSubProcess] WHERE [Name]='RemoveEntitlement'
SELECT @UpdateAc_Id =Id FROM [DI_M_NProvisioningSubProcess] WHERE [Name]='UpdateAccount'


INSERT [dbo].[DI_M_NProvisioningEndpointTriggerProcess] ([EndpointSystemId], [SubProcessId], [SubProcessType], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [CustomProcessId], [Order], [ProvisioningEndpointProcessName],[BreakWhenFailed],[AccountType]) VALUES (@EPId, @EnableAc_Id, N'General', 1,  N'System', GETDATE(), N'System', GETDATE(), NULL, 1, N'AccountActivation',1,'User')
INSERT [dbo].[DI_M_NProvisioningEndpointTriggerProcess] ([EndpointSystemId], [SubProcessId], [SubProcessType], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [CustomProcessId], [Order], [ProvisioningEndpointProcessName],[BreakWhenFailed],[AccountType]) VALUES (@EPId, @ResetPwd_Id, N'General', 1,  N'System', GETDATE(), N'System', GETDATE(), NULL, 2, N'AccountActivation',1,'User')
INSERT [dbo].[DI_M_NProvisioningEndpointTriggerProcess] ([EndpointSystemId], [SubProcessId], [SubProcessType], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [CustomProcessId], [Order], [ProvisioningEndpointProcessName],[BreakWhenFailed],[AccountType]) VALUES (@EPId, @UpdateAc_Id, N'General', 1,  N'System', GETDATE(), N'System', GETDATE(), NULL, 3, N'AccountActivation',1,'User')
INSERT [dbo].[DI_M_NProvisioningEndpointTriggerProcess] ([EndpointSystemId], [SubProcessId], [SubProcessType], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [CustomProcessId], [Order], [ProvisioningEndpointProcessName],[BreakWhenFailed],[AccountType]) VALUES (@EPId, @CreateAc_Id, N'General', 1,  N'System', GETDATE(), N'System', GETDATE(), NULL, 1, N'AccountAssignment',1,'User')
INSERT [dbo].[DI_M_NProvisioningEndpointTriggerProcess] ([EndpointSystemId], [SubProcessId], [SubProcessType], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [CustomProcessId], [Order], [ProvisioningEndpointProcessName],[BreakWhenFailed],[AccountType]) VALUES (@EPId, @EnableAc_Id, N'General', 1,  N'System', GETDATE(), N'System', GETDATE(), NULL, 2, N'AccountAssignment',1,'User')
INSERT [dbo].[DI_M_NProvisioningEndpointTriggerProcess] ([EndpointSystemId], [SubProcessId], [SubProcessType], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [CustomProcessId], [Order], [ProvisioningEndpointProcessName],[BreakWhenFailed],[AccountType]) VALUES (@EPId, @ResetPwd_Id, N'General', 1,  N'System', GETDATE(), N'System', GETDATE(), NULL, 3, N'AccountAssignment',1,'User')
INSERT [dbo].[DI_M_NProvisioningEndpointTriggerProcess] ([EndpointSystemId], [SubProcessId], [SubProcessType], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [CustomProcessId], [Order], [ProvisioningEndpointProcessName],[BreakWhenFailed],[AccountType]) VALUES (@EPId, @UpdateAc_Id, N'General', 1,  N'System', GETDATE(), N'System', GETDATE(), NULL, 4, N'AccountAssignment',1,'User')
INSERT [dbo].[DI_M_NProvisioningEndpointTriggerProcess] ([EndpointSystemId], [SubProcessId], [SubProcessType], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [CustomProcessId], [Order], [ProvisioningEndpointProcessName],[BreakWhenFailed],[AccountType]) VALUES (@EPId, @DisableAc_Id, N'General', 1, N'System', GETDATE(), N'System', GETDATE(), NULL, 1, N'AccountRemove',1,'User')
INSERT [dbo].[DI_M_NProvisioningEndpointTriggerProcess] ([EndpointSystemId], [SubProcessId], [SubProcessType], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [CustomProcessId], [Order], [ProvisioningEndpointProcessName],[BreakWhenFailed],[AccountType]) VALUES (@EPId, @AddEnt_Id, N'General', 1,  N'System', GETDATE(), N'System', GETDATE(), NULL, 1, N'RoleAssignment',1,'User')
INSERT [dbo].[DI_M_NProvisioningEndpointTriggerProcess] ([EndpointSystemId], [SubProcessId], [SubProcessType], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [CustomProcessId], [Order], [ProvisioningEndpointProcessName],[BreakWhenFailed],[AccountType]) VALUES (@EPId, @RemoveEnt_Id, N'General', 1,  N'System', GETDATE(), N'System', GETDATE(), NULL, 1, N'RoleRemove',1,'User')
INSERT [dbo].[DI_M_NProvisioningEndpointTriggerProcess] ([EndpointSystemId], [SubProcessId], [SubProcessType], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [CustomProcessId], [Order], [ProvisioningEndpointProcessName],[BreakWhenFailed],[AccountType]) VALUES (@EPId, @UpdateAc_Id, N'General', 1,  N'System', GETDATE(), N'System', GETDATE(), NULL, 1, N'TransferAccount',1,'User')
INSERT [dbo].[DI_M_NProvisioningEndpointTriggerProcess] ([EndpointSystemId], [SubProcessId], [SubProcessType], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [CustomProcessId], [Order], [ProvisioningEndpointProcessName],[BreakWhenFailed],[AccountType]) VALUES (@EPId, @UpdateAc_Id, N'General', 1,  N'System', GETDATE(), N'System', GETDATE(), NULL, 1, N'UpdateAccount',1,'User')
