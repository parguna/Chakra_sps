DECLARE @EPId bigint
SELECT @EPId =Id FROM [DI_M_EndpointSystem] WHERE [EndpointSystemTypeId]=9

DECLARE @EPTYPEId bigint

SELECT @EPTYPEId =EndpointSystemTypeName FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName=9

INSERT [dbo].[DI_M_NProvisioningField] ([EndPointSystemTypeId],[EndpointsystemId], [FieldName], [CanDerive], [Description], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [PrimaryIdentifier], [ResolverType], [FieldScope], [Value],[IsMandatory],[AccountType]) VALUES (@EPTYPEId,NULL, N'Permissions', 1, NULL, 1, 'IACMAdmin', GetDate(), NULL, NULL, 0, N'None', N'None', NULL,1,1)
