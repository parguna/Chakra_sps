DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_Windows','Windows' ,NULL,N'System', getdate(), N'System',getdate() )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Windows'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, NULL,N'System', getdate(), N'System',getdate())

--Windows
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/Windows/Users.aspx' ,N'System', getdate(), N'System',getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/Windows/Groups.aspx',N'System', getdate(), N'System',getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/Windows/UserGroups.aspx' ,N'System', getdate(), N'System',getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/Windows/GroupUsers.aspx',N'System', getdate(), N'System',getdate() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Windows/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_Windows_USERS','Users' ,@UrlId,N'System', getdate(), N'System',getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Windows/Groups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_Windows_GROUPS','Groups' ,@UrlId,N'System', getdate(), N'System',getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Windows/UserGroups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_Windows_USERGROUPS','User Groups' ,@UrlId,N'System', getdate(), N'System',getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Windows/GroupUsers.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_Windows_GROUPUSERS','Group Users' ,@UrlId ,N'System', getdate(), N'System',getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Windows_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Windows/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System',getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, NULL,N'System', getdate(), N'System',getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System',getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Windows_GROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Windows/Groups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System',getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, NULL,N'System', getdate(), N'System',getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System',getdate())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Windows_USERGROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Windows/UserGroups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System',getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, NULL,N'System', getdate(), N'System',getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System',getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Windows_GROUPUSERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/Windows/GroupUsers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System',getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, NULL,N'System', getdate(), N'System',getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System',getdate())

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Windows'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_Windows','Windows' , @PageId,N'System', getdate(), N'System',getdate() )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_Windows'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Windows_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_Windows_USERS','Users' , @PageId,N'System', getdate(), N'System',getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Windows_GROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_Windows_GROUPS','Groups' , @PageId ,N'System', getdate(), N'System',getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Windows_USERGROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_Windows_USERGROUPS','User Groups' , @PageId,N'System', getdate(), N'System',getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Windows_GROUPUSERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId],CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_Windows_GROUPUSERS','Group Users' , @PageId,N'System', getdate(), N'System',getdate() )


---AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('REPORT_Windows','REPORT_Windows_USERS','REPORT_Windows_GROUPS','REPORT_Windows_USERGROUPS','REPORT_Windows_GROUPUSERS')

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
where pag.uniquename in ('REPORT_Windows','REPORT_Windows_USERS','REPORT_Windows_GROUPS','REPORT_Windows_USERGROUPS','REPORT_Windows_GROUPUSERS')

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
