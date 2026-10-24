/* User Account*/
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=28
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 1, N'UserName', NULL, N'KnownAs', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'User_Id', NULL, N'IdentificationNo', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'HandPhoneNo', NULL, N'MobileNumber', 1, 'System', GETDATE(), NULL, NULL)


INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Role', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)

/* Secondary Account*/

DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=28
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 4, N'UserName', NULL, N'KnownAs', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'User_Id', NULL, N'IdentificationNo', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'HandPhoneNo', NULL, N'MobileNumber', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'Role', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)



