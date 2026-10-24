
Delete from DI_M_DefaultRoleQuickLink
DBCC CHECKIDENT (DI_M_DefaultRoleQuickLink, reseed, 1)
go

Delete from DI_M_QuickLinkAccessRights
DBCC CHECKIDENT (DI_M_QuickLinkAccessRights, reseed, 1)
go

Delete from DI_M_QuickLink
DBCC CHECKIDENT (DI_M_QuickLink, reseed, 1)

DECLARE @TabMenuId BIGINT
DECLARE @MenuName NVARCHAR(200)
DECLARE @ParentTabMenuId BIGINT

-------------------QuickLinks---------------------------------------------------------------------------------------------------------------

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
DECLARE @TabMenu BIGINT
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


-------------------------------------------------------------------------------------------------------------------------------------------------
------------Administrator

----Access Rights For Administrator

DECLARE @QuickLinkId BIGINT
DECLARE @RIDARole BIGINT 
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

----- Default QuickLinks For Administrator

DECLARE @TabMenuId BIGINT
DECLARE @RIDARole BIGINT 
DECLARE @QLID BIGINT 
SELECT @RIDARole = Id from DI_M_Role where RoleName='Administrator' 

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE  UniqueName ='HOME_MYTASK'
SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDARole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),1)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='USER_ACCOUNT'
SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDARole , @QLID,'System' , GETDATE() ,'System' , GETDATE(),2)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_ENDPOINTSYSTEM'
SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDARole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),3)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_WORKFLOW'
SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES(@RIDARole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),4)

SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_TRIGGERS'
SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
VALUES( @RIDARole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),5)


----------------------------------------------------------------------------------------------------------------------------------------------
-------User

---Access Rights For User

--DECLARE @QuickLinkId BIGINT
--DECLARE @RIDURole BIGINT 
--SELECT @RIDURole = Id from DI_M_Role where RoleName='User' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT Id
--FROM [DI_M_QuickLink] where Category in ('Home','Password Management') and Name not in('Global Questions','Application Password Policy')

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @QuickLinkId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_QuickLinkAccessRights](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDURole , @QuickLinkId ,'System' , GETDATE() ,'System' , GETDATE())

--FETCH NEXT FROM c1
--INTO @QuickLinkId

--END

--CLOSE c1
--DEALLOCATE c1


------- Default QuickLink For User

--SELECT @RIDURole = Id from DI_M_Role where RoleName='User' 

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE  UniqueName ='HOME_MYTASK'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES(@RIDURole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),1)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu]  WHERE UniqueName ='HOME_ACCESSREQUESTS'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES(@RIDURole , @QLID,'System' , GETDATE() ,'System' , GETDATE(),2)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMANAGEMENT_PASSWORDRESET'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES(@RIDURole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),3)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='HOME_MYPROFILE'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDURole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),4)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='HOME_MYREQUESTS'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES(@RIDURole , @QLID,'System' , GETDATE() ,'System' , GETDATE(),5)


-------------------------------------------------------------------------------------------------------------------------------------------------
-------For Provisioning Administrator

------Access Rights For Provisioning Administrator

--DECLARE @RIDPdAdRole BIGINT 
--SELECT @RIDPdAdRole = Id from DI_M_Role where RoleName='Provisioning Administrator'  
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT Id
--FROM [DI_M_QuickLink] where Category in ('Home','Password Management')

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @QuickLinkId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_QuickLinkAccessRights](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDPdAdRole , @QuickLinkId ,'System' , GETDATE() ,'System' , GETDATE())

--FETCH NEXT FROM c1
--INTO @QuickLinkId

--END

--CLOSE c1
--DEALLOCATE c1


------Default QuickLink For Provisioning Administrator

--SELECT @RIDPdAdRole = Id from DI_M_Role where RoleName='Provisioning Administrator' 

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE  UniqueName ='HOME_MYTASK'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDPdAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),1)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMGMT_APPLICATIONPASSWORDPOLICY'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDPdAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),2)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMANAGEMENT_PASSWORDRESET'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDPdAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),3)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMGMT_PRIVATEQUESTION'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDPdAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),4)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMGMT_GLOBALQUESTION'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDPdAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),5)


-----------------------------------------------------------------------------------------------------------------------------------------------
-------Attestation Administrator

------ Access Rights For Attestation Administrator

--DECLARE @RIDAtAdRole BIGINT 
--SELECT @RIDAtAdRole = Id from DI_M_Role where RoleName='Attestation Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT Id
--FROM [DI_M_QuickLink] where Category in ('Home','Administration','Job','Password Management')

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @QuickLinkId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_QuickLinkAccessRights](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDAtAdRole , @QuickLinkId ,'System' , GETDATE() ,'System' , GETDATE())

--FETCH NEXT FROM c1
--INTO @QuickLinkId

--END

--CLOSE c1
--DEALLOCATE c1


----- Default Quick Link For Attestation Administrator

--SELECT @RIDAtAdRole = Id from DI_M_Role where RoleName='Attestation Administrator' 

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='HOME_MYTASK'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDAtAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),1)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMANAGEMENT_PASSWORDRESET'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES(@RIDAtAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),2)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_ATTESTATION'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDAtAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),3)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_ATTESTATIONJOB'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDAtAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),4)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='HOME_MYPROFILE'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDAtAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),5)


-------------------------------------------------------------------------------------------------------------------------------------------------
-------Endpoint Administrator

------ Access Rights For Endpoint Administrator

--DECLARE @RIDEpAdRole BIGINT 
--SELECT @RIDEpAdRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT Id
--FROM [DI_M_QuickLink] where Category in ('Home','Administration','Job','Password Management')

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @QuickLinkId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_QuickLinkAccessRights](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDEpAdRole , @QuickLinkId ,'System' , GETDATE() ,'System' , GETDATE())

--FETCH NEXT FROM c1
--INTO @QuickLinkId

--END

--CLOSE c1
--DEALLOCATE c1


----- Default Quick Link For Endpoint Administrator

--SELECT @RIDAtAdRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='HOME_MYTASK'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDEpAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),1)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='PASSWORDMANAGEMENT_PASSWORDRESET'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES(@RIDEpAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),2)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_ENDPOINTSYSTEM'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDEpAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),3)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_DISCOVERYJOB'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDEpAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),4)

--SELECT @TabMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='HOME_MYPROFILE'
--SELECT @QLID =Id FROM [DI_M_QuickLink] WHERE  QuickLinkId =@TabMenuId
--INSERT INTO [dbo].[DI_M_DefaultRoleQuickLink](RoleId , QuickLinkId ,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate,Position )
--VALUES( @RIDEpAdRole , @QLID ,'System' , GETDATE() ,'System' , GETDATE(),5)

----------------------------------------------------------------------------------------------------------------------------------------------



