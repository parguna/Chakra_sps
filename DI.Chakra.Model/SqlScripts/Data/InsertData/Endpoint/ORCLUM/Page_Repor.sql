DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT','Report',NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_USER','User Report',NULL)
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPOT_ENDPOINT','Endpoint Systems Report',NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_USER'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPOT_ENDPOINT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ORACLE UM','ORACLE UM' ,NULL )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLE UM'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
 
 INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/ORACLE UM/Users.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/ORACLE UM/Roles.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/ORACLE UM/UserRoles.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/ORACLE UM/RoleUsers.aspx' )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORACLE UM/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ORACLE UM_USERS','Users' ,@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORACLE UM/Roles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ORACLE UM_ROLES','Roles' ,@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORACLE UM/UserRoles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ORACLE UM_USERROLES','User Roles' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORACLE UM/GroupUsers.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_ORACLE UM_ROLEUSERS','Role Users' ,@UrlId )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLE UM_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORACLE UM/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLE UM_ROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORACLE UM/Roles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLE UM_USERROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORACLE UM/UserRoles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLE UM_ROLEUSERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ORACLE UM/RoleUsers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (NULL, 'REPORT' ,'Report' ,@PageId )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_USER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_USER' ,'User' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ENDPOINT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ENDPOINT' ,'Endpoint Systems' , @PageId )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ORACLE UM'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLE UM_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ORACLE UM_USERS','Users' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLE UM_ROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ORACLE UM_ROLES','Roles', @PageId )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLE UM_ROLESUSERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ORACLE UM_ROLESUSERS','Role Users' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ORACLE UM_USERROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_ORACLE UM_USERROLES','User Roles' , @PageId )

