SET IDENTITY_INSERT [DI_M_NRulesetType] ON
GO

INSERT [dbo].[DI_M_NRulesetType] ([RulesetType], [Description], [Score], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'Department', NULL, 4, 1, NULL, GETDATE(), NULL, NULL)
INSERT [dbo].[DI_M_NRulesetType] ([RulesetType], [Description], [Score], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'Designation', NULL, 3, 1, NULL, GETDATE(), NULL, NULL)
INSERT [dbo].[DI_M_NRulesetType] ([RulesetType], [Description], [Score], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'EmployeeType', N'Employee Type', 2, 1, NULL, GETDATE(), NULL, NULL)
INSERT [dbo].[DI_M_NRulesetType] ([RulesetType], [Description], [Score], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'EventTrigger', N'Event Trigger', 1, 1, NULL, GETDATE(), NULL, NULL)
INSERT [dbo].[DI_M_NRulesetType] ([RulesetType], [Description], [Score], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'Grade', N'Grade', 5, 1, NULL, GETDATE(), NULL, NULL)
INSERT [dbo].[DI_M_NRulesetType] ([RulesetType], [Description], [Score], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'Company', N'Company', 1, 1, NULL, GETDATE(), NULL, NULL)
INSERT [dbo].[DI_M_NRulesetType] ([RulesetType], [Description], [Score], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'BusinessUnit', N'BusinessUnit', 6, 1, NULL, GETDATE(), NULL, NULL)
INSERT [dbo].[DI_M_NRulesetType] ([RulesetType], [Description], [Score], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'CostCenter', N'CostCenter', 7, 1, NULL, GETDATE(), NULL, NULL)
INSERT [dbo].[DI_M_NRulesetType] ([RulesetType], [Description], [Score], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'Location', N'Location', 8, 1, NULL, GETDATE(), NULL, NULL)

SET IDENTITY_INSERT [DI_M_NRulesetType] OFF
GO

SET IDENTITY_INSERT [DI_M_SubSystemType] ON
GO

IF NOT EXISTS (SELECT * FROM [DI_M_SubSystemType] WHERE [Name]='Gaming')
insert into DI_M_SubSystemType (id,Name,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
values('1','Gaming','System',getdate(),'','')

IF NOT EXISTS (SELECT * FROM [DI_M_SubSystemType] WHERE [Name]='NonGaming')
insert into DI_M_SubSystemType (id,Name,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
values('2','NonGaming','System',getdate(),'','')

SET IDENTITY_INSERT [DI_M_SubSystemType] OFF