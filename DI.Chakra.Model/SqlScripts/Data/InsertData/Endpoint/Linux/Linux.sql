SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType ON
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [DisplayName]='Linux'AND [CreatedBy] = 'System')
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id], [EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description]) VALUES (17, 17, 1, N'System', getdate(), N'System', getdate(), N'Linux', N'Linux')
SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType OFF


DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [DisplayName]='Linux'

INSERT INTO DI_M_EndpointObjects(EndpointSystemTypeId, ObjectName) VALUES (@TypeId, 'User')
INSERT INTO DI_M_EndpointObjects(EndpointSystemTypeId, ObjectName) VALUES (@TypeId, 'Group')
