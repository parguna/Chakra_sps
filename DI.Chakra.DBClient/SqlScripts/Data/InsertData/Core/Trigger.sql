--ScenarioID 1	:DI_TRS_USERINFO
--ScenarioID 2	:DI_ADS_USERINFO
--ScenarioID 8	:DI_SIS_USERINFO
--ScenarioID 9	:DI_WIN_USERINFO
--ScenarioID 10	:DI_WIN_GROUPINFO
--ScenarioID 11	:DI_WIN_RESOURCEINFO

CREATE TABLE [dbo].[DI_M_Trigger](
[ScenarioID] [int] NOT NULL,
[ColumnName] [nvarchar](250) NOT NULL,
[ISActive] [bit] NOT NULL
) ON [PRIMARY]
--DI_TRS_USERINFO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (1, N'DisplayName',1)
GO
INSERT [dbo].DI_M_Trigger ([ScenarioID], [ColumnName], [ISActive]) VALUES (1, N'EmployeeId',1)
GO
INSERT [dbo].DI_M_Trigger ([ScenarioID], [ColumnName], [ISActive]) VALUES (1, N'OrganisationId',1)
GO
INSERT [dbo].DI_M_Trigger ([ScenarioID], [ColumnName], [ISActive]) VALUES (1, N'BusinessRole',1)
--DI_TRS_USERINFO  END

--DI_ADS_USERINFO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (2, N'cn',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (2, N'sn',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (2, N'title',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (2, N'telephoneNumber',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (2, N'displayName',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (2, N'department',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (2, N'mail',1)
GO
--DI_ADS_USERINFO   END

--DI_ADS_GROUPINFO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (3, N'cn',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (3, N'description',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (3, N'sAMAccountname',1)
GO
--DI_ADS_GROUPINFO  END
GO
--DI_ADS_OUInfo
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (4, N'ou',1)
GO
--DI_ADS_OUInfo  END


--DI_M_DISCOVERYPROCESS
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (5, N'HistoryId',1)
GO
--DI_M_DISCOVERYPROCESS  END


--DI_EX_GROUPINFO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (7, N'cn',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (7, N'description',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (7, N'sAMAccountname',1)
GO
--DI_ADS_GROUPINFO  END


GO

--DI_SAP_RESOURCEINFO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (12, N'UserLogonName',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (12, N'Firstname',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (12, N'Lastname',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (12, N'Department',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (12, N'E_Mail',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (12, N'Pers_No',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (12, N'Function0',1)
GO
INSERT [dbo].[DI_M_Trigger] ([ScenarioID], [ColumnName], [ISActive]) VALUES (12, N'Title',1)
GO

