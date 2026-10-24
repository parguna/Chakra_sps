

/* User Account*/
--EP- ApplicationType = 13
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=4 
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId AND ApplicationType = 13

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 1, N'UserName', NULL, N'UserName', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Roles', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Groups', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)
Go
--CE - ApplicationType = 14
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=4
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId AND ApplicationType = 14

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 1, N'UserName', NULL, N'UserName', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Roles', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 1, N'Groups', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)
Go
--Secondary Account -4
/* User Account*/
--EP- ApplicationType = 13
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=4 
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId AND ApplicationType = 13

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 4, N'UserName', NULL, N'UserName', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'Roles', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'Groups', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)
Go
--CE - ApplicationType = 14
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=4
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId AND ApplicationType = 14

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 4, N'UserName', NULL, N'UserName', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'Roles', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 4, N'Groups', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)
Go
--Service Account
/* User Account*/
--EP- ApplicationType = 13
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=4 
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId AND ApplicationType = 13

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 2, N'UserName', NULL, N'UserName', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 2, N'Roles', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 2, N'Groups', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)
Go
--CE - ApplicationType = 14
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=4
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId AND ApplicationType = 14

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 2, N'UserName', NULL, N'UserName', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 2, N'Roles', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 2, N'Groups', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)
Go
--Shared Account = 3
/* User Account*/
--EP- ApplicationType = 13
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=4 
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId AND ApplicationType = 13

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 3, N'UserName', NULL, N'UserName', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'Roles', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'Groups', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)
Go
--CE - ApplicationType = 14
DECLARE @EPTYPEId bigint
DECLARE @EPId bigint
SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=4
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE EndpointSystemTypeId=@EPTYPEId AND ApplicationType = 14

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( @EPTYPEId, @EPId, 3, N'UserName', NULL, N'UserName', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'Roles', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)

INSERT [dbo].[DI_M_NProvisioningField] ( [EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (@EPTYPEId, @EPId, 3, N'Groups', NULL, N'', 1, 'System', GETDATE(), NULL, NULL)

Go