DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_LNS','Lotus Notes' ,NULL, N'System', getdate(), N'System', getdate() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, NULL, N'System', getdate(), N'System', getdate())


--LNS
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]  ) VALUES ('/Report/Endpoint/LNS/Users.aspx', N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]  ) VALUES ('/Report/Endpoint/LNS/Groups.aspx' , N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]  ) VALUES ('/Report/Endpoint/LNS/Certifiers.aspx', N'System', getdate(), N'System', getdate()  )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]  ) VALUES ('/Report/Endpoint/LNS/UserGroups.aspx', N'System', getdate(), N'System', getdate()  )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]  ) VALUES ('/Report/Endpoint/LNS/GroupUsers.aspx', N'System', getdate(), N'System', getdate()  )
--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/LNS/LockedUsers.aspx', N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/LNS/UsersWithNoGroups.aspx', N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/LNS/GroupsWithNoUsers.aspx', N'System', getdate(), N'System', getdate() )



SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('REPORT_LNS_USERS','Lotus Notes  Users' ,@UrlId, N'System', getdate(), N'System', getdate()  )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/Groups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('REPORT_LNS_GROUPS','Lotus Notes Groups' ,@UrlId , N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/Certifiers.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('REPORT_LNS_CERTIFIERS','Lotus Notes Certifiers' ,@UrlId, N'System', getdate(), N'System', getdate()  )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/UserGroups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('REPORT_LNS_USERGROUPS','Lotus Notes User Groups' ,@UrlId, N'System', getdate(), N'System', getdate()  )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/GroupUsers.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('REPORT_LNS_GROUPUSERS','Lotus Notes Group Users' ,@UrlId , N'System', getdate(), N'System', getdate() )
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/LockedUsers.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('REPORT_LNS_LOCKEDUSERS','Lotus Notes Locked Users' ,@UrlId , N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/UsersWithNoGroups.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('REPORT_LNS_USERSWITHNOGROUPS','Lotus Notes Users with No Groups' ,@UrlId , N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/GroupsWithNoUsers.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('REPORT_LNS_GROUPSWITHNOUSERS','Lotus Notes Groups with No Users' ,@UrlId , N'System', getdate(), N'System', getdate() )



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 1, @UrlId, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 12, NULL, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL, N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_GROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/Groups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 1, @UrlId, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 12, NULL, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL, N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_CERTIFIERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/Certifiers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 1, @UrlId, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 12, NULL, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL, N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_USERGROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/UserGroups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 1, @UrlId, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 12, NULL, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL, N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_GROUPUSERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/GroupUsers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 1, @UrlId, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 12, NULL, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL, N'System', getdate(), N'System', getdate())

--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_LOCKEDUSERS'
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/LockedUsers.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 1, @UrlId, N'System', getdate(), N'System', getdate() )
--INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 12, NULL, N'System', getdate(), N'System', getdate() )
--INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL, N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_USERSWITHNOGROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/UsersWithNoGroups.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 1, @UrlId, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 12, NULL, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL, N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_GROUPSWITHNOUSERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/LNS/GroupsWithNoUsers.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 1, @UrlId, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES( @PageId , 12, NULL, N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL, N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES (@TabParentMenuId, 'REPORT_LNS','Lotus Notes' , @PageId, N'System', getdate(), N'System', getdate()  )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_LNS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES (@TabParentMenuId, 'REPORT_LNS_USERS','Users' , @PageId, N'System', getdate(), N'System', getdate()  )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_GROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES (@TabParentMenuId, 'REPORT_LNS_GROUPS','Groups' , @PageId, N'System', getdate(), N'System', getdate()  )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_CERTIFIERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES (@TabParentMenuId, 'REPORT_LNS_CERTIFIERS','Certifiers' , @PageId, N'System', getdate(), N'System', getdate()  )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_USERGROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES (@TabParentMenuId, 'REPORT_LNS_USERGROUPS','User Groups' , @PageId , N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_GROUPUSERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES (@TabParentMenuId, 'REPORT_LNS_GROUPUSERS','Group Users' , @PageId, N'System', getdate(), N'System', getdate()  )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_LOCKEDUSERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES (@TabParentMenuId, 'REPORT_LNS_LOCKEDUSERS','Locked Users' , @PageId, N'System', getdate(), N'System', getdate()  )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_USERSWITHNOGROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES (@TabParentMenuId, 'REPORT_LNS_USERSWITHNOGROUPS','Users With No Groups' , @PageId, N'System', getdate(), N'System', getdate()  )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LNS_GROUPSWITHNOUSERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES (@TabParentMenuId, 'REPORT_LNS_GROUPSWITHNOUSERS','Groups With No Users' , @PageId, N'System', getdate(), N'System', getdate()  )


---AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('REPORT_LNS','REPORT_LNS_USERS','REPORT_LNS_GROUPS','REPORT_LNS_CERTIFIERS','REPORT_LNS_USERGROUPS','REPORT_LNS_GROUPUSERS', 'REPORT_LNS_LOCKEDUSERS', 'REPORT_LNS_USERSWITHNOGROUPS', 'REPORT_LNS_GROUPSWITHNOUSERS')

OPEN c1
 
FETCH NEXT FROM c1
INTO @PagePermissionId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDRole ,  @PagePermissionId, N'System', getdate(), N'System', getdate())

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
where pag.uniquename in ('REPORT_LNS','REPORT_LNS_USERS','REPORT_LNS_GROUPS','REPORT_LNS_CERTIFIERS','REPORT_LNS_USERGROUPS','REPORT_LNS_GROUPUSERS','REPORT_LNS_LOCKEDUSERS','REPORT_LNS_USERSWITHNOGROUPS','REPORT_LNS_GROUPSWITHNOUSERS')

OPEN c1
 
FETCH NEXT FROM c1
INTO @PagePermissionId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDRole ,  @PagePermissionId, N'System', getdate(), N'System', getdate())

FETCH NEXT FROM c1
INTO @PagePermissionId

END

CLOSE c1
DEALLOCATE c1

GO
