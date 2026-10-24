/*---CREATE PROVISIONING FIELD----*/
INSERT INTO [dbo].[DI_M_NProvisioningField]([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
SELECT B.EndpointType, B.EndpointId, [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], GETDATE(), [LastModifiedBy], GETDATE()  from DI_M_NProvisioningField AS A
CROSS JOIN (SELECT EndpointSystemTypeId as EndpointType,Id as EndpointId FROm DI_M_EndpointSystem WHERE EndpointSystemTypeId = 40 AND ApplicationType=42) AS B
where EndPointSystemTypeId = 3 and EndPointSystemId = 8  order by AccountType,FieldName
