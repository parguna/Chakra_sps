DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_OEBS','OEBS' ,NULL )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_OEBS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/OEBS/Users.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/OEBS/Roles.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/OEBS/Responsibilities.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/OEBS/RoleUsers.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/OEBS/ResponsibilityUsers.aspx' )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/OEBS/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_OEBS_USERS','Users' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/OEBS/Roles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_OEBS_ROLES','Roles' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/OEBS/Responsibilities.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_OEBS_RESPONSIBILITY','Responsibility' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/OEBS/ResponsibilityUsers.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_OEBS_RESPONSIBILITYUSERS','ResponsibilityUsers' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/OEBS/RoleUsers.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_OEBS_RoleUsers','Role Users' ,@UrlId )



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_OEBS_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='//Report/Endpoint/OEBS/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_OEBS_ROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/OEBS/Roles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_OEBS_RESPONSIBILITY'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/OEBS/Responsibilities.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_OEBS_RESPONSIBILITYUSERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/OEBS/ResponsibilityUsers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_OEBS_RoleUsers'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/OEBS/RoleUsers'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_OEBS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_OEBS','OEBS' , @PageId )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_OEBS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_OEBS_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_OEBS_USERS','Users' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_OEBS_ROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_OEBS_ROLES','Roles	' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_OEBS_RESPONSIBILITIES'

INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_OEBS_RESPONSIBILITIES','Responsibilities' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_OEBS_ROLESUSERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_OEBS_ROLESUSERS','Role Users' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_OEBS_RESPONSIBILITYUSER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_OEBS_RESPONSIBILITYUSER','Responsibility Users' , @PageId )

---AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('REPORT_OEBS','REPORT_OEBS_USERS','REPORT_OEBS_ROLES','REPORT_OEBS_RESPONSIBILITIES','REPORT_OEBS_ROLESUSERS','REPORT_OEBS_RESPONSIBILITYUSER')

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


DECLARE @PagePermissionId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('REPORT_OEBS','REPORT_OEBS_USERS','REPORT_OEBS_ROLES','REPORT_OEBS_RESPONSIBILITIES','REPORT_OEBS_ROLESUSERS','REPORT_OEBS_RESPONSIBILITYUSER')

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