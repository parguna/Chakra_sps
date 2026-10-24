DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_MSSQL','MS SQL' ,NULL, N'System', getdate(), N'System', getdate() )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MSSQL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, NULL, N'System', getdate(), N'System', getdate())

--MSSQL
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/MSSQL/Users.aspx' , N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/MSSQL/Roles.aspx', N'System', getdate(), N'System', getdate() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/MSSQL/UserRoles.aspx' , N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/MSSQL/RoleUsers.aspx', N'System', getdate(), N'System', getdate() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/MSSQL/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_MSSQL_USERS','Users' ,@UrlId, N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/MSSQL/Roles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_MSSQL_ROLES','Roles' ,@UrlId, N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/MSSQL/UserRoles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_MSSQL_USERROLES','User Roles' ,@UrlId, N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/MSSQL/RoleUsers.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_MSSQL_ROLEUSERS','Role users' ,@UrlId , N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MSSQL_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/MSSQL/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId, N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, NULL, N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL, N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MSSQL_ROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/MSSQL/Roles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId, N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, NULL, N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL, N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MSSQL_USERROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/MSSQL/UserRoles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId, N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, NULL, N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL, N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MSSQL_ROLEUSERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/MSSQL/RoleUsers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId, N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, NULL, N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL, N'System', getdate(), N'System', getdate())

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MSSQL'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_MSSQL','MSSQL' , @PageId, N'System', getdate(), N'System', getdate() )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_MSSQL'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MSSQL_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_MSSQL_USERS','Users' , @PageId, N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MSSQL_ROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_MSSQL_ROLES','Roles' , @PageId , N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MSSQL_USERROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_MSSQL_USERROLES','User Roles' , @PageId, N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MSSQL_ROLEUSERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_MSSQL_ROLEUSERS','Role Users' , @PageId, N'System', getdate(), N'System', getdate() )


---AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('REPORT_MSSQL','REPORT_MSSQL_USERS','REPORT_MSSQL_ROLES','REPORT_MSSQL_USERROLES','REPORT_MSSQL_ROLEUSERS')

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
where pag.uniquename in ('REPORT_MSSQL','REPORT_MSSQL_USERS','REPORT_MSSQL_ROLES','REPORT_MSSQL_USERROLES','REPORT_MSSQL_ROLEUSERS')

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
