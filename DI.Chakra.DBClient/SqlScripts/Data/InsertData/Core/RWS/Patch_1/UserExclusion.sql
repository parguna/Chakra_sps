
DECLARE @PageId bigint
DECLARE @UrlId bigint
Declare @pagePer bigint
Declare @TabParentMenuId bigint
Declare @PagePermissionId bigint

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/UserExclusion/Add.aspx' , SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/UserExclusion/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_EXCLUSION_ADD','User Exclusion Add' ,@UrlId , SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EXCLUSION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/UserExclusion/Add.aspx'

select @PagePermissionId = Id from [DI_M_PagePermission] where [PageId] = @PageId and PermissionId = 6


delete from [DI_M_AccessRights] where [PagePermissionId]=@PagePermissionId
delete from [DI_M_PagePermission] where [Id]=@PagePermissionId

INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EXCLUSION_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, Null,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, Null,SYSDATETIME())





SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EXCLUSION'

select @PagePermissionId = Id from [DI_M_PagePermission] where [PageId] = @PageId and PermissionId = 3

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(1 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(2 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(3 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())


select @PagePermissionId = Id from [DI_M_PagePermission] where [PageId] = @PageId and PermissionId = 5

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(1 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(2 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(3 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EXCLUSION'
select @PagePermissionId = Id from [DI_M_PagePermission] where [PageId] = @PageId and PermissionId = 1

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(1 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(2 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(3 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())

select @PagePermissionId = Id from [DI_M_PagePermission] where [PageId] = @PageId and PermissionId = 6

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(1 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(2 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(3 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())


-- Export User Exclusion

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EXCLUSION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/UserExclusion/List.aspx'


INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EXCLUSION'

select @PagePermissionId = Id from [DI_M_PagePermission] where [PageId] = @PageId and PermissionId = 12

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(1 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(2 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())

INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
VALUES(3 , @PagePermissionId ,'System' , GETDATE() ,'System' , GETDATE())


