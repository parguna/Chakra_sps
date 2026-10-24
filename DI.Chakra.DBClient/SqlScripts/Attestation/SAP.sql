DECLARE @Eptyp bigint
DECLARE @UserObject bigint
DECLARE @RoleObject bigint
DECLARE @GroupObject bigint
DECLARE @ParentId bigint
Declare @PositionObject bigint
Declare @TCodeObject bigint

SELECT @Eptyp =Id FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName =3
SELECT @UserObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'User'
SELECT @RoleObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'Role'
SELECT @GroupObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'Group'
SELECT @PositionObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'Position'
SELECT @TCodeObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'T-Code'

IF NOT EXISTS(SELECT * FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp)
BEGIN
--Account
INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, NULL, 14, @UserObject, 1, 1, NULL, 1, 1, N'system',  getdate(), NULL, NULL)
SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 14 AND EndpointObjectId=@UserObject;

--BusinessRole
INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, NULL, 41, @RoleObject, 1, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)
SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 41 AND EndpointObjectId=@RoleObject;

--Technical Role
INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, NULL, 42, @RoleObject, 0, 0, @ParentId, 3, 1, N'system',  getdate(), NULL, NULL)
SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 42 AND EndpointObjectId=@RoleObject;


----Group
--SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 14 AND EndpointObjectId=@UserObject;
--INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
--values (@Eptyp, NULL, 2, @GroupObject, 1, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)

--Position
INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, NULL, 5, @RoleObject, 1, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)

END