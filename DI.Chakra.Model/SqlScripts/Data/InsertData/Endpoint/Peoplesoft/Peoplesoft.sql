ALTER TABLE ADSUserInfoADSGroupInfo  ADD EndpointSystemId INT NULL, [Version] INT NOT NULL  DEFAULT 1
GO
SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType ON
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [DisplayName]='Peoplesoft'AND [CreatedBy] = 'System')
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id], [EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],[ObjectName]) VALUES (8, 8, 1, N'System', getdate(), N'System', getdate(), N'Peoplesoft', N'Peoplesoft','PeoplesoftUserInfo')
SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType OFF

GO

DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [DisplayName]='Active Directory'

INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'User')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'Role')
INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (@TypeId, N'Permission')
--INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (1, N'Computer')
--INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (1, N'Printer')
--INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName]) VALUES (1, N'Folder')



