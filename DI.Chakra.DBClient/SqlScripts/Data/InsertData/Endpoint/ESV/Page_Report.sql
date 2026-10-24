DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_ESV','ESV' ,NULL ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, NULL,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/ESV/Users.aspx' ,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/ESV/Roles.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/ESV/Functions.aspx' ,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/ESV/PrivillegeAcctTrans.aspx' ,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/ESV/RoleUser.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/ESV/UserRole.aspx' ,N'System', getdate(), N'System', getdate())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/ESV/RoleFunction.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/ESV/FunctionRole.aspx' ,N'System', getdate(), N'System', getdate())



SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/Users.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_ESV_USERS','Users' ,@UrlId ,N'System', getdate(), N'System', getdate())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/Roles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_ESV_ROLES','Roles' ,@UrlId ,N'System', getdate(), N'System', getdate())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/Functions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_ESV_FUNCTIONS','Functions' ,@UrlId ,N'System', getdate(), N'System', getdate())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/PrivillegeAcctTrans.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_ESV_PRIVILLEGEACCTTRANS','PrivillegeAcctTrans' ,@UrlId ,N'System', getdate(), N'System', getdate())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/RoleUser.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_ESV_ROLEUSER','RoleUser' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/UserRole.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_ESV_USERROLE','UserRole' ,@UrlId,N'System', getdate(), N'System', getdate() )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/RoleFunction.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_ESV_ROLEFUNCTION','RoleFunction' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/FunctionRole.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_ESV_FUNCTIONROLE','FunctionRole' ,@UrlId,N'System', getdate(), N'System', getdate() )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_USERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/Users.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_ROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/Roles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_FUNCTIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/Functions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_PRIVILLEGEACCTTRANS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/PrivillegeAcctTrans.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())




SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_ROLEUSER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/RoleUser.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_USERROLE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/UserRole.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_ROLEFUNCTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/RoleFunction.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_FUNCTIONROLE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/FunctionRole.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_ESV','EServices' , @PageId,N'System', getdate(), N'System', getdate() )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ESV'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_USERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_ESV_USERS','Users' , @PageId ,N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_ROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_ESV_ROLES','Roles	' , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_FUNCTIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_ESV_FUNCTIONS','Functions' , @PageId ,N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_PRIVILLEGEACCTTRANS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_ESV_PRIVILLEGEACCTTRANS','PrivillegeAcctTrans', @PageId ,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_ROLEUSER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_ESV_ROLEUSER','RoleUser' , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_USERROLE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_ESV_USERROLE','UserRole' , @PageId ,N'System', getdate(), N'System', getdate())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_ROLEFUNCTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_ESV_ROLEFUNCTION','RoleFunction' , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_FUNCTIONROLE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_ESV_FUNCTIONROLE','FunctionRole' , @PageId ,N'System', getdate(), N'System', getdate())




--- Accounts With No  Roles----


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/ESV/AccountsWithNoRoles.aspx',N'System', getdate(), N'System', getdate() )


SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/ESV/AccountsWithNoRoles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_ESV_ACCOUNTSWITHNOROLES', 'CMS Accounts With No Roles', @UrlId, N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_ACCOUNTSWITHNOROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/AccountsWithNoRoles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ESV'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_ACCOUNTSWITHNOROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_ESV_ACCOUNTSWITHNOROLES','Accounts With No Roles' , @PageId,N'System', getdate(), N'System', getdate() )


--- Roles With No Accounts ----

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/ESV/RolesWithNoAccounts.aspx',N'System', getdate(), N'System', getdate() )


SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/ESV/RolesWithNoAccounts.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_ESV_ROLESWITHNOACCOUNTS', 'CMS Roles With No Accounts', @UrlId, N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_ROLESWITHNOACCOUNTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/RolesWithNoAccounts.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ESV'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_ROLESWITHNOACCOUNTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_ESV_ROLESWITHNOACCOUNTS','Roles With No Accounts' , @PageId,N'System', getdate(), N'System', getdate() )


--- Roles With No Functions ----

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/ESV/RolesWithNoFunctions.aspx',N'System', getdate(), N'System', getdate() )


SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/ESV/RolesWithNoFunctions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_ESV_ROLESWITHNOFUNCTIONS', 'CMS Roles With No Functions', @UrlId, N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_ROLESWITHNOFUNCTIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/RolesWithNoFunctions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ESV'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_ROLESWITHNOFUNCTIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_ESV_ROLESWITHNOFUNCTIONS','Roles With No Functions' , @PageId,N'System', getdate(), N'System', getdate() )


--- Dormant Account ----

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Endpoint/ESV/DormantAccounts.aspx',N'System', getdate(), N'System', getdate() )


SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/Endpoint/ESV/DormantAccounts.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],	[Name],  [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])	VALUES ('REPORT_ESV_DORMANTACCOUNT', 'CMS Dormant Accounts', @UrlId, N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_DORMANTACCOUNT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/ESV/DormantAccounts.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ESV'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ESV_DORMANTACCOUNT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_ESV_DORMANTACCOUNT','Dormant Accounts' , @PageId,N'System', getdate(), N'System', getdate() )







---AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 
--SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT pageperm.Id
--FROM [DI_M_PagePermission] pageperm
--inner join DI_M_Page pag on pag.Id = pageperm.PageId
--where pag.uniquename in ('REPORT_ESV','REPORT_ESV_USERS','REPORT_ESV_ROLES','REPORT_ESV_FUNCTIONS','REPORT_ESV_PRIVILLEGEACCTTRANS','REPORT_ESV_ROLEUSER','REPORT_ESV_USERROLE','REPORT_ESV_ROLEFUNCTION','REPORT_ESV_FUNCTIONROLE')

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
where pag.uniquename in ('REPORT_ESV','REPORT_ESV_USERS','REPORT_ESV_ROLES','REPORT_ESV_FUNCTIONS','REPORT_ESV_PRIVILLEGEACCTTRANS','REPORT_ESV_ROLEUSER','REPORT_ESV_USERROLE','REPORT_ESV_ROLEFUNCTION','REPORT_ESV_FUNCTIONROLE')

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