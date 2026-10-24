SET IDENTITY_INSERT [DI_M_EndpointSystemType] ON
GO
IF NOT EXISTS (SELECT * FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=32)
	INSERT [dbo].[DI_M_EndpointSystemType] ([Id],[EndpointSystemTypeName], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [DisplayName], [Description],[ObjectName],[CanBeTrustedSource]) 
	VALUES (32,32, 1, N'System', getdate(), N'System', getdate(), N'ELM', N'ELM','',0)
SET IDENTITY_INSERT [DI_M_EndpointSystemType] OFF
-- End


DECLARE @EndpointSystemTypeId bigint
DECLARE @KeyId bigint
SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='ELM'

INSERT INTO [dbo].[DI_M_EndpointUserViewConfiguration]([EndpointSystemTypeId], [EndpointSystemId], [KeyName], [Caption], [Value],[Order],  [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES	(@EndpointSystemTypeId,NULL,'AccountName','Account Name','ADID',1,'System',GETDATE(),'System', GETDATE())