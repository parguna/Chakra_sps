
Delete from DI_M_DefaultRoleQuickLink
DBCC CHECKIDENT (DI_M_DefaultRoleQuickLink, reseed, 0)
go

Delete from DI_M_QuickLinkAccessRights
DBCC CHECKIDENT (DI_M_QuickLinkAccessRights, reseed, 0)
go

Delete from DI_M_QuickLink
DBCC CHECKIDENT (DI_M_QuickLink, reseed, 0)


DECLARE @TabMenuId INT
DECLARE @MenuName NVARCHAR(200)
DECLARE @ParentTabMenuId INT

-------------------QuickLinks---------------------------
--------------------------------------------------------
DECLARE @Cursor CURSOR
SET @Cursor = CURSOR FAST_FORWARD
FOR
SELECT Id,ParentTabMenuId,MenuName
FROM   DI_M_TabMenu 
OPEN @Cursor
FETCH NEXT FROM @Cursor
INTO @TabMenuId,@ParentTabMenuId,@MenuName
WHILE @@FETCH_STATUS = 0
BEGIN
DECLARE @TabMenu INT
DECLARE @Category NVARCHAR(200)
DECLARE @Name NVARCHAR(200)


IF (SELECT COUNT(*) FROM DI_M_TabMenu WHERE ParentTabMenuId = @TabMenuId) = 0

	BEGIN
		SET @Category = (SELECT MenuName from DI_M_TabMenu WHERE Id = (SELECT ParentTabMenuId from DI_M_TabMenu where ID= @TabMenuId))
		SET @TabMenu = (SELECT Id from DI_M_TabMenu WHERE ID= @TabMenuId)
		SET @Name = (SELECT MenuName from DI_M_TabMenu WHERE ID= @TabMenuId)
		INSERT INTO DI_M_QuickLink (Category,QuickLinkId,CreatedDate,LastModifiedDate,Name) VALUES(@Category,@TabMenu,GETDATE(),GETDATE(),@Name)	
	END
	 
FETCH NEXT FROM @Cursor
INTO @TabMenuId,@ParentTabMenuId,@MenuName
END
CLOSE @Cursor
DEALLOCATE @Cursor



------------Administrator--------------------
----------- Access Rights

DECLARE @QuickLinkId int
DECLARE @RIDARole int 
SELECT @RIDARole = Id from DI_M_Role where RoleName='Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT Id
FROM [DI_M_QuickLink] where Category in ('Home','Administration','Password Management','Provisioning','ACL','Compliance')

OPEN c1
 
FETCH NEXT FROM c1
INTO @QuickLinkId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_QuickLinkAccessRights](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDARole , @QuickLinkId,'System' , GETDATE() ,'System' , GETDATE())

FETCH NEXT FROM c1
INTO @QuickLinkId

END

CLOSE c1
DEALLOCATE c1

GO


----- Default QuickLinks 

DECLARE @TabMenuId int
DECLARE @RIDARole int 
SELECT @RIDARole = Id from DI_M_Role where RoleName='Administrator' 

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE  UniqueName ='HOME_MYTASK'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDARole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),1)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='USER_ACCOUNT'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDARole , @TabMenuId,'System' , GETDATE() ,'System' , GETDATE(),2)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_ENDPOINTSYSTEM'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDARole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),3)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_WORKFLOW'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES(@RIDARole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),4)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_TRIGGERS'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDARole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),5)



-------User
---Access Rights


DECLARE @QuickLinkId int
DECLARE @RIDURole int 
SELECT @RIDURole = Id from DI_M_Role where RoleName='User' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT Id
FROM [DI_M_QuickLink] where Category in ('Home','Password Management')

OPEN c1
 
FETCH NEXT FROM c1
INTO @QuickLinkId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_QuickLinkAccessRights](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDURole , @QuickLinkId ,'System' , GETDATE() ,'System' , GETDATE())

FETCH NEXT FROM c1
INTO @QuickLinkId

END

CLOSE c1
DEALLOCATE c1


----- Default QuickLink


SELECT @RIDURole = Id from DI_M_Role where RoleName='User' 

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE  UniqueName ='HOME_MYTASK'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES(@RIDURole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),1)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu]  WHERE UniqueName ='HOME_ACCESSREQUESTS'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES(@RIDURole , @TabMenuId,'System' , GETDATE() ,'System' , GETDATE(),2)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMANAGEMENT_PASSWORDRESET'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES(@RIDURole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),3)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='HOME_MYPROFILE'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDURole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),4)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='HOME_MYREQUESTS'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES(@RIDURole , @TabMenuId,'System' , GETDATE() ,'System' , GETDATE(),5)


-----Provisioning Admin
----Access Rights 

DECLARE @RIDPdAdRole int 
SELECT @RIDPdAdRole = Id from DI_M_Role where RoleName='Provisioning Administrator'  
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT Id
FROM [DI_M_QuickLink] where Category in ('Home','Password Management')

OPEN c1
 
FETCH NEXT FROM c1
INTO @QuickLinkId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_QuickLinkAccessRights](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDPdAdRole , @QuickLinkId ,'System' , GETDATE() ,'System' , GETDATE())

FETCH NEXT FROM c1
INTO @QuickLinkId

END

CLOSE c1
DEALLOCATE c1

----Default QuickLink

SELECT @RIDPdAdRole = Id from DI_M_Role where RoleName='Provisioning Administrator' 

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE  UniqueName ='HOME_MYTASK'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDPdAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),1)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMGMT_APPLICATIONPASSWORDPOLICY'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDPdAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),2)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMANAGEMENT_PASSWORDRESET'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDPdAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),3)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMGMT_PRIVATEQUESTION'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDPdAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),4)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMGMT_GLOBALQUESTION'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDPdAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),5)




-----Attestation  Admin
---- Access Rights For User

DECLARE @RIDAtAdRole int 
SELECT @RIDAtAdRole = Id from DI_M_Role where RoleName='Attestation Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT Id
FROM [DI_M_QuickLink] where Category in ('Home','Administration','Job','Password Management')

OPEN c1
 
FETCH NEXT FROM c1
INTO @QuickLinkId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_QuickLinkAccessRights](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDAtAdRole , @QuickLinkId ,'System' , GETDATE() ,'System' , GETDATE())

FETCH NEXT FROM c1
INTO @QuickLinkId

END

CLOSE c1
DEALLOCATE c1


--- Default Quick Link

SELECT @RIDAtAdRole = Id from DI_M_Role where RoleName='Attestation Administrator' 

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='HOME_MYTASK'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDAtAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),1)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMANAGEMENT_PASSWORDRESET'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES(@RIDAtAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),2)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_ATTESTATION'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDAtAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),3)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_ATTESTATIONJOB'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDAtAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),4)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='HOME_MYPROFILE'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDAtAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),5)



-----EndPoint  Admin
---- Access Rights For User

DECLARE @RIDEpAdRole int 
SELECT @RIDEpAdRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT Id
FROM [DI_M_QuickLink] where Category in ('Home','Administration','Job','Password Management')

OPEN c1
 
FETCH NEXT FROM c1
INTO @QuickLinkId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_QuickLinkAccessRights](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDEpAdRole , @QuickLinkId ,'System' , GETDATE() ,'System' , GETDATE())

FETCH NEXT FROM c1
INTO @QuickLinkId

END

CLOSE c1
DEALLOCATE c1


--- Default Quick Link

SELECT @RIDAtAdRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='HOME_MYTASK'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDEpAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),1)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMANAGEMENT_PASSWORDRESET'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES(@RIDEpAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),2)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_ENDPOINTSYSTEM'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDEpAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),3)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_DISCOVERYJOB'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDEpAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),4)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='HOME_MYPROFILE'
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDEpAdRole , @TabMenuId ,'System' , GETDATE() ,'System' , GETDATE(),5)



