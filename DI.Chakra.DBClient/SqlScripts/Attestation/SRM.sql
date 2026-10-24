	DECLARE @EpId bigint
	SELECT @EpId = Id from DI_M_EndpointSystem where ApplicationCode='SRM'
	IF NOT EXISTS(SELECT * FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId=13)
	BEGIN
	INSERT INTO [DI_M_EndpointObjects](EndpointSystemTypeId,ObjectName,CreatedBy,CreatedDate,IsTrustedSource)
	values(13,'User','System',GETDATE(),0),(13,'Role','System',GETDATE(),0),(13,'T-Code','System',GETDATE(),0)

	END

	DECLARE @Eptyp bigint
	DECLARE @UserObject bigint
	DECLARE @RoleObject bigint
	DECLARE @ParentId bigint
	Declare @TCodeObject bigint
	SELECT @Eptyp =Id FROM [DI_M_EndpointSystemType] WHERE EndpointSystemTypeName = 13
	SELECT @UserObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'User'
	SELECT @RoleObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'Role'
	SELECT @TCodeObject =Id FROM [DI_M_EndpointObjects] WHERE EndpointSystemTypeId =@Eptyp AND ObjectName = 'T-Code'
	IF NOT EXISTS(SELECT * FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp)
	BEGIN
	INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
	values (@Eptyp, @EpId, 14, @UserObject, 1, 1, NULL, 1, 1, N'system',  getdate(), NULL, NULL)
	SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 14 AND EndpointObjectId=@UserObject;

 

	INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
	values (@Eptyp, @EpId, 41, @RoleObject, 1, 1, @ParentId, 2, 1, N'system',  getdate(), NULL, NULL)
	SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 41 AND EndpointObjectId=@RoleObject;
	--Technical Role
	INSERT [dbo].[DI_M_EndpointHierarchy] ([EndpointType], [EndpointId], [EntitlementType], [EndpointObjectId], [IsActionable], [IsAssociatedWithUser], [ParentId], [Level], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
	values (@Eptyp, @EpId, 42, @RoleObject, 0, 0, @ParentId, 3, 1, N'system',  getdate(), NULL, NULL)
	SELECT @ParentId = ID FROM DI_M_EndpointHierarchy WHERE EndpointType=@Eptyp AND EntitlementType = 42 AND EndpointObjectId=@RoleObject;



	END

