DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC','PeopleSoftFINOrPROC' ,NULL ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, NULL,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/User.aspx' ,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/Role.aspx' ,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/RoleUser.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/UserRole.aspx' ,N'System', getdate(), N'System', getdate())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/UserWithNoRoles.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/RolesWithNoUser.aspx' ,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/BankAccount.aspx' ,N'System', getdate(), N'System', getdate())



SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/User.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_USER','User' ,@UrlId ,N'System', getdate(), N'System', getdate())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/Role.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_ROLE','Role' ,@UrlId ,N'System', getdate(), N'System', getdate())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/RoleUser.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_ROLEUSER','RoleUser' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/UserRole.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_USERROLE','UserRole' ,@UrlId,N'System', getdate(), N'System', getdate() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/UserWithNoRoles.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_USERWITHNOROLE','User With No Role' ,@UrlId,N'System', getdate(), N'System', getdate() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/RolesWithNoUser.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_ROLEWITHNOUSER','Role With No User' ,@UrlId,N'System', getdate(), N'System', getdate() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/BankAccount.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_BANKACCOUNT','Bank Account' ,@UrlId,N'System', getdate(), N'System', getdate() )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_USER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/User.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_ROLE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/Role.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_ROLEUSER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/RoleUser.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_USERROLE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/UserRole.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_USERWITHNOROLE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/UserWithNoRoles.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_ROLEWITHNOUSER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/RolesWithNoUser.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_BANKACCOUNT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/BankAccount.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ENDPOINT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC','PeopleSoftFINOrPROC' , @PageId,N'System', getdate(), N'System', getdate() )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_PeopleSoftFINOrPROC'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_USER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_USER','User'  , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_PeopleSoftFINOrPROC'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_ROLE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_ROLE','Role'  , @PageId ,N'System', getdate(), N'System', getdate())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_ROLEUSER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_ROLEUSER','RoleUser' , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_USERROLE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_USERROLE','UserRole' , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_USERWITHNOROLE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_USERWITHNOROLE','User With No Role' , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_ROLEWITHNOUSER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_ROLEWITHNOUSER','Role With No User' , @PageId ,N'System', getdate(), N'System', getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_BANKACCOUNT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_BANKACCOUNT','Bank Account' , @PageId ,N'System', getdate(), N'System', getdate())



--- Functions ----


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/Functions.aspx' ,N'System', getdate(), N'System', getdate())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/Functions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_Function','Function' ,@UrlId ,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_Function'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/Functions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_PeopleSoftFINOrPROC'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_Function'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_USER','Functions'  , @PageId ,N'System', getdate(), N'System', getdate())

--- Role RoleFunctions ----


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/RoleFunction.aspx' ,N'System', getdate(), N'System', getdate())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/RoleFunction.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_RoleFunctions','RoleFunctions' ,@UrlId ,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_RoleFunctions'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/RoleFunction.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_PeopleSoftFINOrPROC'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_RoleFunctions'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_RoleFunctions','RoleFunctions'  , @PageId ,N'System', getdate(), N'System', getdate())


--- FunctionRoles ----


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/FunctionRole.aspx' ,N'System', getdate(), N'System', getdate())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/FunctionRole.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_FunctionRoles','FunctionRoles' ,@UrlId ,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_FunctionRoles'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/FunctionRole.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_PeopleSoftFINOrPROC'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_FunctionRoles'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_FunctionRoles','FunctionRoles'  , @PageId ,N'System', getdate(), N'System', getdate())


--- RoleWithNoFunctions ----


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/RolesWithNoFunctions.aspx' ,N'System', getdate(), N'System', getdate())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/RolesWithNoFunctions.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_RolesWithNoFunctions','RolesWithNoFunctions' ,@UrlId ,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_RolesWithNoFunctions'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/RolesWithNoFunctions.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_PeopleSoftFINOrPROC'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_RolesWithNoFunctions'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_RolesWithNoFunctions','RolesWithNoFunctions'  , @PageId ,N'System', getdate(), N'System', getdate())


--- BUDescriptions ----


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] , [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/BUDescription.aspx' ,N'System', getdate(), N'System', getdate())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/BUDescription.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_BUDescription','BUDescription' ,@UrlId ,N'System', getdate(), N'System', getdate())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_BUDescription'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/BUDescription.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_PeopleSoftFINOrPROC'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_BUDescription'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_BUDescription','BUDescription'  , @PageId ,N'System', getdate(), N'System', getdate())



------------ USER BankAccount -------------------

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/UserBankAccount.aspx' ,N'System', getdate(), N'System', getdate())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/UserBankAccount.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_USERBANKACCOUNT','User Bank Account' ,@UrlId,N'System', getdate(), N'System', getdate() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_USERBANKACCOUNT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/UserBankAccount.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_PeopleSoftFINOrPROC'

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_USERBANKACCOUNT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_USERBANKACCOUNT','User Bank Account' , @PageId ,N'System', getdate(), N'System', getdate())


----------------------

-------------- BankAccount User ---------------------

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/BankAccountUser.aspx' ,N'System', getdate(), N'System', getdate())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/BankAccountUser.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_BANKACCOUNTUSER','Bank Account User' ,@UrlId,N'System', getdate(), N'System', getdate() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_BANKACCOUNTUSER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/BankAccountUser.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_PeopleSoftFINOrPROC'

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_BANKACCOUNTUSER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_BANKACCOUNTUSER','Bank Account User' , @PageId ,N'System', getdate(), N'System', getdate())


------------------------------
-------------- User BUDescription ---------------------
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/UserBUDescription.aspx' ,N'System', getdate(), N'System', getdate())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/UserBUDescription.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_USERBUDESCRIPTION','User BUDescription' ,@UrlId,N'System', getdate(), N'System', getdate() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_USERBUDESCRIPTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/UserBUDescription.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_PeopleSoftFINOrPROC'

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_USERBUDESCRIPTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_USERBUDESCRIPTION','User BUDescription' , @PageId ,N'System', getdate(), N'System', getdate())

------------------------------

-------------- BUDescription User ---------------------

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate ) VALUES ('/Report/Endpoint/PeopleSoftFINOrPROC/BUDescriptionUser.aspx' ,N'System', getdate(), N'System', getdate())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/BUDescriptionUser.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES ('REPORT_PeopleSoftFINOrPROC_BUDESCRIPTIONUSER','BUDescription User' ,@UrlId,N'System', getdate(), N'System', getdate() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_BUDESCRIPTIONUSER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Endpoint/PeopleSoftFINOrPROC/BUDescriptionUser.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_PeopleSoftFINOrPROC'

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PeopleSoftFINOrPROC_BUDESCRIPTIONUSER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], CreatedDate , LastModifiedBy , LastModifiedDate) VALUES (@TabParentMenuId, 'REPORT_PeopleSoftFINOrPROC_BUDESCRIPTIONUSER','BUDescription User' , @PageId ,N'System', getdate(), N'System', getdate())

------------------------------


---AccessRights
DECLARE @PagePermissionId int
DECLARE @RIDRole int 

--SELECT @RIDRole = Id from DI_M_Role where RoleName='Endpoint Administrator' 
--DECLARE c1 CURSOR READ_ONLY
--FOR
--SELECT pageperm.Id
--FROM [DI_M_PagePermission] pageperm
--inner join DI_M_Page pag on pag.Id = pageperm.PageId
--where pag.uniquename in ('REPORT_PeopleSoftFINOrPROC','REPORT_PeopleSoftFINOrPROC_USER','REPORT_PeopleSoftFINOrPROC_ROLE','REPORT_PeopleSoftFINOrPROC_ROLEUSER','REPORT_PeopleSoftFINOrPROC_USERROLE', 'REPORT_PeopleSoftFINOrPROC_USERWITHNOROLE', 'REPORT_PeopleSoftFINOrPROC_ROLEWITHNOUSER', 'REPORT_PeopleSoftFINOrPROC_BANKACCOUNT')

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
where pag.uniquename in  ('REPORT_PeopleSoftFINOrPROC','REPORT_PeopleSoftFINOrPROC_USER','REPORT_PeopleSoftFINOrPROC_ROLE','REPORT_PeopleSoftFINOrPROC_ROLEUSER','REPORT_PeopleSoftFINOrPROC_USERROLE', 'REPORT_PeopleSoftFINOrPROC_USERWITHNOROLE', 'REPORT_PeopleSoftFINOrPROC_ROLEWITHNOUSER', 'REPORT_PeopleSoftFINOrPROC_BANKACCOUNT','REPORT_PeopleSoftFINOrPROC_BANKACCOUNTUSER' , 'REPORT_PeopleSoftFINOrPROC_USERBANKACCOUNT','REPORT_PeopleSoftFINOrPROC_BUDESCRIPTIONUSER','REPORT_PeopleSoftFINOrPROC_USERBUDESCRIPTION')


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