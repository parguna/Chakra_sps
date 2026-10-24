DECLARE @Eptyp bigint
DECLARE @UserObject bigint
DECLARE @GroupObject bigint
declare @RoleObject  bigint
DECLARE @ParentId bigint

--INSERT INTO [DI_M_EndpointObjects](EndpointSystemTypeId,ObjectName,CreatedBy,CreatedDate,IsTrustedSource)
--values(8,'User','System',GETDATE(),0),(8,'Role','System',GETDATE(),0)
--SELECT * FROM [DI_M_EndpointSystemType] where EndpointSystemTypeId=13
--SELECT * FROM [DI_M_EndpointObjects]where EndpointSystemTypeId =30

SELECT @Eptyp =Id FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName = 30
SELECT @UserObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'User'
SELECT @RoleObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'UnAssignedNumberRange'
SELECT @GroupObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'ResponseGroup'

IF NOT EXISTS(SELECT * FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp)
BEGIN
INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, NULL, 14, @UserObject, 1, 1, NULL, 1, 1, N'system',  getdate(), NULL, NULL)
SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 14 AND EndpointObjectId=@UserObject;

 INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, NULL, 26, @RoleObject, 0, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)

INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, NULL, 27, @RoleObject, 1, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)

INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, NULL, 28, @RoleObject, 1, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)

INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, NULL, 29, @RoleObject, 1, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)

INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, NULL, 30, @GroupObject, 1, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)

END

