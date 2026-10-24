DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_LNS','Lotus Notes' ,NULL )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)


--LNS
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/LNS/Users.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/LNS/Groups.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/LNS/Certifiers.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/LNS/UserGroups.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Report/Endpoint/LNS/GroupUsers.aspx' )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_LNS_USERS','Users' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/Groups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_LNS_GROUPS','Groups' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/Certifiers.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_LNS_CERTIFIERS','Certifiers' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/UserGroups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_LNS_USERGROUPS','User Groups' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/GroupUsers.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORT_LNS_GROUPUSERS','Group Users' ,@UrlId )



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_GROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/Groups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_CERTIFIERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/Certifiers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_USERGROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/UserGroups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_GROUPUSERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/GroupUsers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 12, NULL)


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_LNS','Lotus Notes' , @PageId )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_LNS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_LNS_USERS','Users' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_GROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_LNS_GROUPS','Groups' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_CERTIFIERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_LNS_CERTIFIERS','Certifiers' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_USERGROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_LNS_USERGROUPS','User Groups' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_GROUPUSERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId]) VALUES (@TabParentMenuId, 'REPORT_LNS_GROUPUSERS','Group Users' , @PageId )

---AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('REPORT_LNS','REPORT_LNS_USERS','REPORT_LNS_GROUPS','REPORT_LNS_CERTIFIERS','REPORT_LNS_USERGROUPS','REPORT_LNS_GROUPUSERS')

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
where pag.uniquename in ('REPORT_LNS','REPORT_LNS_USERS','REPORT_LNS_GROUPS','REPORT_LNS_CERTIFIERS','REPORT_LNS_USERGROUPS','REPORT_LNS_GROUPUSERS')

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
