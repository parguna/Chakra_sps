DECLARE @Eptyp bigint
DECLARE @EpId bigint
DECLARE @UserObject bigint
DECLARE @RoleObject bigint
DECLARE @GroupObject bigint
DECLARE @ParentId bigint
Declare @TCodeObject bigint

select @EpId = Id from DI_M_EndpointSystem where ApplicationCode='SAPGRC'
SELECT @Eptyp =Id FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName =3
SELECT @UserObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'User'
SELECT @RoleObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'Role'
SELECT @TCodeObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'T-Code'

IF NOT EXISTS(SELECT * FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp and EndpointId = @EpId)
BEGIN
--Account
INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, @EpId, 14, @UserObject, 1, 1, NULL, 1, 1, N'system',  getdate(), NULL, NULL)
SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 14 AND EndpointObjectId=@UserObject;

--Indirect Role
INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, @EpId, 43, @RoleObject, 0, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)
SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 43 AND EndpointObjectId=@RoleObject;

--BusinessRole
INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, @EpId, 41, @RoleObject, 1, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)
SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 41 AND EndpointObjectId=@RoleObject;

--Technical Role
INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, @EpId, 42, @RoleObject, 0, 0, @ParentId, 3, 1, N'system',  getdate(), NULL, NULL)
SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 42 AND EndpointObjectId=@RoleObject;



END