DECLARE @Eptyp bigint
DECLARE @UserObject bigint
DECLARE @RoleObject bigint
DECLARE @GroupObject bigint
DECLARE @ParentId bigint
Declare @PositionObject bigint
Declare @TCodeObject bigint
DECLARE @EpId bigint

SELECT @Eptyp =Id FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName =3
SELECT @UserObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'User'
SELECT @RoleObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'Role'
SELECT @TCodeObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'T-Code'

select @EpId = Id from DI_M_EndpointSystem where DisplayName='HRMS/FLINS'

IF NOT EXISTS(SELECT * FROM DI_M_EndpointHierarchy WHERE EndpointId=@EpId)
BEGIN

INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp,@EpId, 14, @UserObject, 1, 1, NULL, 1, 1, N'system',  getdate(), NULL, NULL)
SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 14 AND EndpointObjectId=@UserObject;

--Indirect Role
INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, @EpId, 43, @RoleObject, 0, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)
SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 43 AND EndpointObjectId=@RoleObject;


INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, @EpId, 41, @RoleObject, 0, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)

--Technical Role
INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
values (@Eptyp, @EpId, 42, @RoleObject, 0, 1, @ParentId, 3, 1, N'system',  getdate(), NULL, NULL)
SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 42 AND EndpointObjectId=@RoleObject;


END
