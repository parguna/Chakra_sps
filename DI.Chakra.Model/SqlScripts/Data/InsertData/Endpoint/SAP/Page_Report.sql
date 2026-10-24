
DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_SAP','SAP' ,NULL )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

---SAP

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/SAP/Users.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/SAP/Groups.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/SAP/Roles.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/SAP/Tcodes.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/SAP/Profiles.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/SAP/UserGroups.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/SAP/GroupUsers.aspx' )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_SAP_USERS','Users' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/Groups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_SAP_GROUPS','Groups' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/Roles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_SAP_ROLES','Roles' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/Tcodes.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_SAP_TCODES','Tcodes' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/Profiles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_SAP_PROFILES','Profiles' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/UserGroups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_SAP_USERGROUPS','User Groups' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/GroupUsers.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_SAP_GROUPUSERS','Group Users' ,@UrlId )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_GROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/Groups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_ROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/Roles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_TCODES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/Tcodes.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_PROFILES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/Profiles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_USERGROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/UserGroups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_GROUPUSERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/SAP/GroupUsers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_SAP','SAP' , @PageId )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_SAP'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_SAP_USERS','Users' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_GROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_SAP_GROUPS','Groups' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_ROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_SAP_ROLES','Roles' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_TCODES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_SAP_TCODES','Tcodes' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_PROFILES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_SAP_PROFILES','Profiles' , @PageId )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_USERGROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_SAP_USERGROUPS','User Groups' , @PageId )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SAP_GROUPUSERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_SAP_GROUPUSERS','Group Users' , @PageId )

---AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('REPORT_SAP','REPORT_SAP_USERS','REPORT_SAP_GROUPS','REPORT_SAP_ROLES','REPORT_SAP_TCODES','REPORT_SAP_PROFILES','REPORT_SAP_USERGROUPS','REPORT_SAP_GROUPUSERS')

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
where pag.uniquename in ('REPORT_SAP','REPORT_SAP_USERS','REPORT_SAP_GROUPS','REPORT_SAP_ROLES','REPORT_SAP_TCODES','REPORT_SAP_PROFILES','REPORT_SAP_USERGROUPS','REPORT_SAP_GROUPUSERS')

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