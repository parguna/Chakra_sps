/****** Object:  Trigger [dbo].[DI_M_ConfigurationMaster_Trigger]    Script Date: 15/3/2015 5:36:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[DI_M_ConfigurationMaster_Trigger] ON [dbo].[DI_M_ConfigurationMaster]
AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

INSERT INTO [dbo].[DI_M_ConfigurationMasterHistory]
           ([ConfigId]
           ,[ConfigKey]
           ,[ConfigValue]
           ,[IsActive]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
	SELECT [ConfigId]
           ,[ConfigKey]
           ,[ConfigValue]
           ,[IsActive]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
		FROM INSERTED      
END
GO

/****** Object:  Trigger [dbo].[DI_M_RoleAssignment_Trigger]  Script Date: 15/3/2015 5:36:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[DI_M_RoleAssignment_Trigger] ON [dbo].[DI_M_RoleAssignment]
AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

INSERT INTO [dbo].[DI_M_RoleAssignmentHistory]
           ([RoleAssignmentId]
           ,[RoleId]
           ,[UserId]
           ,[GroupId]
           ,[Remarks]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
	SELECT [Id]
           ,[RoleId]
           ,[UserId]
           ,[GroupId]
           ,[Remarks]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
		FROM INSERTED      
END
GO

/****** Object:  Trigger [dbo].[DI_M_Group_Trigger]    Script Date: 15/3/2015 5:36:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[DI_M_Group_Trigger] ON [dbo].[DI_M_Group]
AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

INSERT INTO [dbo].[DI_M_GroupHistory]
           ([GroupId]
           ,[GroupName]
           ,[GroupType]
           ,[GroupOwner]
           ,[GroupDescription]
           ,[IsDefaultGroup]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
	SELECT [GroupId]
           ,[GroupName]
           ,[GroupType]
           ,[GroupOwner]
           ,[GroupDescription]
           ,[IsDefaultGroup]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
		FROM INSERTED      
END
GO

/****** Object:  Trigger [dbo].[[DI_M_AccessRights_Trigger]]    Script Date: 15/3/2015 5:36:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[DI_M_AccessRights_Trigger] ON [dbo].[DI_M_AccessRights]
AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

INSERT INTO [dbo].[DI_M_AccessRightHistory]
           ([Id]
           ,[RoleId]
           ,[TabMenuId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
	SELECT [Id]
           ,[RoleId]
           ,[TabMenuId]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
		FROM INSERTED      
END
GO


/****** Object:  Trigger [dbo].[DI_M_Role_Trigger]  Script Date: 15/3/2015 5:36:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[DI_M_Role_Trigger] ON [dbo].[DI_M_Role]
AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

INSERT INTO [dbo].[DI_M_RoleHistory]
           ([RoleId]
           ,[RoleName]
           ,[RoleDescription]
           ,[IsDefaultRole]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate])
	SELECT [RoleId]
           ,[RoleName]
           ,[RoleDescription]
           ,[IsDefaultRole]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastModifiedBy]
           ,[LastModifiedDate]
		FROM INSERTED      
END
GO