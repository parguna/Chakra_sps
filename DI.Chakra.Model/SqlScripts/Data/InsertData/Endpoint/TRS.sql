SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType ON
INSERT [dbo].[DI_M_EndpointSystemType] ([Id], [EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description]) VALUES (15, 15, 1, N'System', CAST(N'2014-12-16 18:20:27.297' AS DateTime), N'System', CAST(N'2014-12-16 18:20:27.297' AS DateTime), N'Trusted Source', N'Trusted Source')
SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType OFF

INSERT INTO DI_M_EndpointObjects(EndpointSystemTypeId, ObjectName) VALUES (15, 'Employees')
INSERT INTO DI_M_EndpointObjects(EndpointSystemTypeId, ObjectName) VALUES (15, 'Department')
