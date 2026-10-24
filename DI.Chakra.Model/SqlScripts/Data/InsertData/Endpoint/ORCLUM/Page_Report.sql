DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ORACLEUM','ORACLEUM' ,NULL )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLEUM'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
 
 INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/ORCLUM/Users.aspx' )
 INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/ORCLUM/Roles.aspx' )
 INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/ORCLUM/UserRoles.aspx' )
 INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/ORCLUM/RoleUsers.aspx' )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORCLUM/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ORACLEUM_USERS','Users' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORCLUM/Roles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ORACLEUM_ROLES','Roles' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORCLUM/UserRoles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ORACLEUM_USERROLES','User Roles' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORCLUM/GroupUsers.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ORACLEUM_ROLEUSERS','Role Users' ,@UrlId )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLEUM_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORCLUM/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLEUM_ROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORCLUM/Roles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLEUM_USERROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORCLUM/UserRoles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLEUM_ROLEUSERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORCLUM/RoleUsers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLEUM'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ORACLEUM','ORACLEUM' , @PageId )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ORACLEUM'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLEUM_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ORACLEUM_USERS','Users' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLEUM_ROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ORACLEUM_ROLES','Roles', @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLEUM_ROLESUSERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ORACLEUM_ROLESUSERS','Role Users' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLEUM_USERROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ORACLEUM_USERROLES','User Roles' , @PageId )

---AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('REPORT_ORACLEUM','REPORT_ORACLEUM_USERS','REPORT_ORACLEUM_ROLES','REPORT_ORACLEUM_ROLESUSERS','REPORT_ORACLEUM_USERROLES')

OPEN c1
 
FETCH NEXT FROM c1
INTO @PagePermissionId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDRole ,  @PagePermissionId,'System' , GETDATE() ,'System' , GETDATE())

FETCH NEXT FROM c1
INTO @PagePermissionId

END

CLOSE c1
DEALLOCATE c1


SELECT @RIDRole = Id from DI_M_Role where RoleName='Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('REPORT_ORACLEUM','REPORT_ORACLEUM_USERS','REPORT_ORACLEUM_ROLES','REPORT_ORACLEUM_ROLESUSERS','REPORT_ORACLEUM_USERROLES')

OPEN c1
 
FETCH NEXT FROM c1
INTO @PagePermissionId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDRole ,  @PagePermissionId,'System' , GETDATE() ,'System' , GETDATE())

FETCH NEXT FROM c1
INTO @PagePermissionId

END

CLOSE c1
DEALLOCATE c1


GO