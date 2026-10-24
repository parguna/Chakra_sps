SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType ON
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [DisplayName]='OracleOEBS'AND [CreatedBy] = 'System')
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id], [EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description]) VALUES (16, 16, 1, N'System', getdate(), N'System', getdate(), N'OracleOEBS', N'OracleOEBS')
SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType OFF



DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [DisplayName]='Oracle11gUM'

INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'User')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'Role')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'Menu')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'SubMenu')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'HRMSUser')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'Responisbility')