ALTER TABLE LotusUserInfoLotusGroupInfo  ADD EndpointSystemId INT NULL, [Version] INT NOT NULL  DEFAULT 1
GO
SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType ON
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [DisplayName]='Lotus Notes'AND [CreatedBy] = 'System')
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id], [EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],[ObjectName]) VALUES (2, 2, 1, N'System', getdate(), N'System', getdate(), N'Lotus Notes', N'Lotus Notes','LotusUserInfo')
SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType OFF
GO
DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [DisplayName]='Lotus Notes'

INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'User')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'Group')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'Certifier')
