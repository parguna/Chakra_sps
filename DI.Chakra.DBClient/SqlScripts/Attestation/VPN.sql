IF NOT EXISTS(SELECT * FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId=8)
BEGIN
INSERT INTO [DI_M_EndpointObjects](EndpointSystemTypeId,ObjectName,CreatedBy,CreatedDate,IsTrustedSource)
values(8,'User','System',GETDATE(),0),(8,'Role','System',GETDATE(),0)
END

DECLARE @Eptyp bigint
DECLARE @UserObject bigint
DECLARE @GroupObject bigint
DECLARE @ParentId bigint



SELECT @Eptyp =Id FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName = 8
SELECT @UserObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'User'
SELECT @GroupObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'Role'


IF NOT EXISTS(SELECT * FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp)
BEGIN
INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, NULL, 14, @UserObject, 1, 1, NULL, 1, 1, N'system',  getdate(), NULL, NULL)
SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 14 AND EndpointObjectId=@UserObject;

 

INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, NULL, 23, @GroupObject, 1, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)

INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, NULL, 24, @GroupObject, 1, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)


END

