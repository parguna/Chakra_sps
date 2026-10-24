ALTER TABLE SAPEPUserInfoSAPEPGroupInfo ADD EndpointSystemId INT NULL, [Version] INT NOT NULL DEFAULT 1
GO

ALTER TABLE SAPParameterInfoSAPUserinfo ADD EndpointSystemId INT NULL, [Version] INT NOT NULL DEFAULT 1
GO

ALTER TABLE SAPProfileInfoSAPUserinfo ADD EndpointSystemId INT NULL, [Version] INT NOT NULL DEFAULT 1
GO

ALTER TABLE SAPRoleInfoSAPUserinfo ADD EndpointSystemId INT NULL, [Version] INT NOT NULL  DEFAULT 1
ALTER TABLE SAPRoleInfoSAPUserinfo ADD Role_FromDate nvarchar(MAX) NULL, Role_ToDate nvarchar(MAX) NULL
alter table SAPRoleInfoSAPUserinfo drop constraint [PK_dbo.SAPRoleInfoSAPUserinfo]
GO

ALTER TABLE SAPSingleRoleInfoSAPCompositeRoleInfo ADD EndpointSystemId INT NULL, [Version] INT NOT NULL DEFAULT 1
GO

ALTER TABLE SAPTCodeInfoSAPSingleRoleInfo ADD EndpointSystemId INT NULL, [Version] INT NOT NULL DEFAULT 1
GO

ALTER TABLE SAPUserinfoSAPGroupInfo ADD EndpointSystemId INT NULL, [Version] INT NOT NULL DEFAULT 1
GO

Alter table SAPRoleInfoSAPUserinfo add IsDirectlyAttached bit null
GO

SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType ON
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [DisplayName]='SAP'AND [CreatedBy] = 'System')
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id], [EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],[ObjectName]) VALUES (3, 3, 1, N'System', getdate(), N'System',getdate(), N'SAP', N'SAP','SAPUserinfo')
SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType OFF
GO
DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [DisplayName]='SAP'


INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES ( @TypeId, N'User')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES ( @TypeId, N'Role')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES ( @TypeId, N'Group')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES ( @TypeId, N'Profile')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES ( @TypeId, N'T-Code')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES ( @TypeId, N'Parameter')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES ( @TypeId, N'Authorization')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES ( @TypeId, N'Position')

GO