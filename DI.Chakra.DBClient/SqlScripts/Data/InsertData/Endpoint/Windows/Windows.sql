SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType ON
GO

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=13)
INSERT [dbo].[DI_M_EndpointSystemType] ([Id], [EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],[ObjectName],[CanBeTrustedSource])
 VALUES        (13, 13, 1,N'System', CAST(N'2014-12-16 18:20:27.297' AS DateTime), N'System', CAST(N'2014-12-16 18:20:27.297' AS DateTime), N'Windows', N'Windows','WindowsUserInfo',0)

SET IDENTITY_INSERT [dbo].DI_M_EndpointSystemType OFF

GO 

DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=13
DELETE FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=13

INSERT INTO DI_M_EndpointObjects(EndpointSystemTypeId, ObjectName,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TypeId, 'User',N'System', getdate(), N'System',getdate())
INSERT INTO DI_M_EndpointObjects(EndpointSystemTypeId, ObjectName,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TypeId, 'Group',N'System', getdate(), N'System',getdate())


alter table WindowsUserInfoWindowsGroupInfo add EndpointSystemId bigint not null,Version int null 

DECLARE @EndpointSystemTypeId bigint
DECLARE @KeyId bigint


-- Windows
SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='Windows'
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='IPAddress'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,1,'varchar','[0-9\.\ \-]*',1,'onkeypress:return IPAddressKeyOnly(event);#onblur:return ValidateIPaddress(this)', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='UserName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,2,'varchar','',1,'', 'System',GETDATE(),'System', GETDATE())
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='Password'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,3,'varchar','',1,'onkeypress:"return Preventspaces(event);', 'System',GETDATE(),'System', GETDATE())
-- End
