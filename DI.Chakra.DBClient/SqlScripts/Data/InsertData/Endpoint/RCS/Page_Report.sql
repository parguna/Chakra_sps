DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_RCS','RCS' ,NULL ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, NULL,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/RCS/Users.aspx' ,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/RCS/Roles.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/RCS/Functions.aspx' ,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/RCS/PrivillegeAcctTrans.aspx' ,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/RCS/RoleUser.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/RCS/UserRole.aspx' ,N'System', getdate(), N'System', getdate())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/RCS/RoleFunction.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/RCS/FunctionRole.aspx' ,N'System', getdate(), N'System', getdate())



SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_RCS_USERS','Users' ,@UrlId ,N'System', getdate(), N'System', getdate())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/Roles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_RCS_ROLES','Roles' ,@UrlId ,N'System', getdate(), N'System', getdate())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/Functions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_RCS_FUNCTIONS','Functions' ,@UrlId ,N'System', getdate(), N'System', getdate())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/PrivillegeAcctTrans.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_RCS_PRIVILLEGEACCTTRANS','PrivillegeAcctTrans' ,@UrlId ,N'System', getdate(), N'System', getdate())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/RoleUser.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_RCS_ROLEUSER','RoleUser' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/UserRole.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_RCS_USERROLE','UserRole' ,@UrlId,N'System', getdate(), N'System', getdate() )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/RoleFunction.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_RCS_ROLEFUNCTION','RoleFunction' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/FunctionRole.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_RCS_FUNCTIONROLE','FunctionRole' ,@UrlId,N'System', getdate(), N'System', getdate() )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_ROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/Roles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_FUNCTIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/Functions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_PRIVILLEGEACCTTRANS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/PrivillegeAcctTrans.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())




SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_ROLEUSER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/RoleUser.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_USERROLE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/UserRole.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_ROLEFUNCTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/RoleFunction.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_FUNCTIONROLE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/FunctionRole.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_RCS','Resorts Card System' , @PageId,N'System', getdate(), N'System', getdate() )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_RCS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_RCS_USERS','Users' , @PageId ,N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_ROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_RCS_ROLES','Roles	' , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_FUNCTIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_RCS_FUNCTIONS','Functions' , @PageId ,N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_PRIVILLEGEACCTTRANS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_RCS_PRIVILLEGEACCTTRANS','PrivillegeAcctTrans', @PageId ,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_ROLEUSER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_RCS_ROLEUSER','RoleUser' , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_USERROLE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_RCS_USERROLE','UserRole' , @PageId ,N'System', getdate(), N'System', getdate())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_ROLEFUNCTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_RCS_ROLEFUNCTION','RoleFunction' , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_FUNCTIONROLE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_RCS_FUNCTIONROLE','FunctionRole' , @PageId ,N'System', getdate(), N'System', getdate())




--- Accounts With No  Roles----


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/RCS/AccountsWithNoRoles.aspx',N'System', getdate(), N'System', getdate() )


SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/RCS/AccountsWithNoRoles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_RCS_ACCOUNTSWITHNOROLES', 'CMS Accounts With No Roles', @UrlId, N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_ACCOUNTSWITHNOROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/AccountsWithNoRoles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_RCS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_ACCOUNTSWITHNOROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_RCS_ACCOUNTSWITHNOROLES','Accounts With No Roles' , @PageId,N'System', getdate(), N'System', getdate() )


--- Roles With No Accounts ----

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/RCS/RolesWithNoAccounts.aspx',N'System', getdate(), N'System', getdate() )


SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/RCS/RolesWithNoAccounts.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_RCS_ROLESWITHNOACCOUNTS', 'CMS Roles With No Accounts', @UrlId, N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_ROLESWITHNOACCOUNTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/RolesWithNoAccounts.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_RCS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_ROLESWITHNOACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_RCS_ROLESWITHNOACCOUNTS','Roles With No Accounts' , @PageId,N'System', getdate(), N'System', getdate() )


--- Roles With No Functions ----

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/RCS/RolesWithNoFunctions.aspx',N'System', getdate(), N'System', getdate() )


SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/RCS/RolesWithNoFunctions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_RCS_ROLESWITHNOFUNCTIONS', 'CMS Roles With No Functions', @UrlId, N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_ROLESWITHNOFUNCTIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/RolesWithNoFunctions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_RCS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_ROLESWITHNOFUNCTIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_RCS_ROLESWITHNOFUNCTIONS','Roles With No Functions' , @PageId,N'System', getdate(), N'System', getdate() )


--- Dormant Account ----

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/RCS/DormantAccounts.aspx',N'System', getdate(), N'System', getdate() )


SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/RCS/DormantAccounts.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_RCS_DORMANTACCOUNT', 'CMS Dormant Accounts', @UrlId, N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_DORMANTACCOUNT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/RCS/DormantAccounts.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_RCS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_RCS_DORMANTACCOUNT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_RCS_DORMANTACCOUNT','Dormant Accounts' , @PageId,N'System', getdate(), N'System', getdate() )







---AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
--SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT pageperm.Id
--FROM [DI_M_PagePermission] pageperm
--inner join DI_M_Page pag on pag.Id = pageperm.PageId
--where pag.uniquename in ('REPORT_RCS','REPORT_RCS_USERS','REPORT_RCS_ROLES','REPORT_RCS_FUNCTIONS','REPORT_RCS_PRIVILLEGEACCTTRANS','REPORT_RCS_ROLEUSER','REPORT_RCS_USERROLE','REPORT_RCS_ROLEFUNCTION','REPORT_RCS_FUNCTIONROLE')

--OPEN c1
 
--FETCH NEXT FROM c1
--INTO @PagePermissionId

--WHILE @@FETCH_STATUS = 0
--BEGIN

--INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
--VALUES(@RIDRole ,  @PagePermissionId,'System' , getdate() ,'System' , getdate())

--FETCH NEXT FROM c1
--INTO @PagePermissionId

--END

--CLOSE c1
--DEALLOCATE c1

SELECT @RIDRole = Id from DI_M_Role where RoleName='Administrator' 
DECLARE c1 CURSOR READ_ONLY
FOR
SELECT pageperm.Id
FROM [DI_M_PagePermission] pageperm
inner join DI_M_Page pag on pag.Id = pageperm.PageId
where pag.uniquename in ('REPORT_RCS','REPORT_RCS_USERS','REPORT_RCS_ROLES','REPORT_RCS_FUNCTIONS','REPORT_RCS_PRIVILLEGEACCTTRANS','REPORT_RCS_ROLEUSER','REPORT_RCS_USERROLE','REPORT_RCS_ROLEFUNCTION','REPORT_RCS_FUNCTIONROLE')

OPEN c1
 
FETCH NEXT FROM c1
INTO @PagePermissionId

WHILE @@FETCH_STATUS = 0
BEGIN

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(@RIDRole ,  @PagePermissionId,'System' , getdate() ,'System' , getdate())

FETCH NEXT FROM c1
INTO @PagePermissionId

END

CLOSE c1
DEALLOCATE c1


GO