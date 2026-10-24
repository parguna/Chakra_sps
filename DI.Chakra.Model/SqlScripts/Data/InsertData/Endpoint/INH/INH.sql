
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [DisplayName]='InHouseSystem'AND [CreatedBy] = 'System')
	INSERT [dbo].[DI_M_EndpointSystemType] ( [EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],[ObjectName]) VALUES ( 9, 1, N'System', getdate(), N'System', getdate(), N'InHouseSystem', N'InHouse System','InhouseUserInfo')


DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [DisplayName]='InHouseSystem'

INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'User')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'Entitlements')



