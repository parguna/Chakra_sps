/* User Account*/
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=6
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 1, N'UserName', NULL, N'KnownAs', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Emp_Id', NULL, N'IdentificationNo', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Phone', NULL, N'TelephoneNumber', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Fax', NULL, N'Fax', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Designation', NULL, N'PositionName', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Level', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)

/* Secondary Account*/
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=6
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 4, N'UserName', NULL, N'KnownAs', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'Emp_Id', NULL, N'IdentificationNo', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'Phone', NULL, N'TelephoneNumber', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId,4, N'Fax', NULL, N'Fax', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'Designation', NULL, N'PositionName', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Level', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)


