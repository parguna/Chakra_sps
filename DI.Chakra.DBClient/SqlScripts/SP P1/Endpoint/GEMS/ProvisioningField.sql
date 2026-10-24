DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=7
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Description', NULL, N'KnownAs', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'DISPLAY_NAME', NULL, N'KnownAs', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'EMAIL', NULL, N'EmailId', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Roles', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)


/* Shared Account */
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=7
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'Description', NULL, N'KnownAs', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'DISPLAY_NAME', NULL, N'KnownAs', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'EMAIL', NULL, N'EmailId', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'Roles', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)
GO


/* Service Account */
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=7
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 2, N'Description', NULL, N'KnownAs', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 2, N'DISPLAY_NAME', NULL, N'KnownAs', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 2, N'EMAIL', NULL, N'EmailId', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 2, N'Roles', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)
GO


/* Secondary Account */
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=7
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'Description', NULL, N'KnownAs', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'DISPLAY_NAME', NULL, N'KnownAs', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'EMAIL', NULL, N'EmailId', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'Roles', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)
GO