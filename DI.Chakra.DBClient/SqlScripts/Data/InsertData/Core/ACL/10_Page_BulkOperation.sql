DECLARE @PageId Bigint
DECLARE @UrlId Bigint
DECLARE @TabParentMenuId Bigint

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('BulkOperations','Bulk Operations',@UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='BulkOperations'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='BulkOperations'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) 
									VALUES (@TabParentMenuId,'BulkOperations','Bulk Operations' , @PageId,SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ,[CreatedDate]) VALUES ( '/Comp/Bulk/UploadFunction/List.aspx' ,SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadFunction/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('UPLOAD_FUNCTION','Upload Functions',@UrlId,SYSDATETIME() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOAD_FUNCTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadFunction/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 6, @UrlId,SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME() )


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='BulkOperations'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOAD_FUNCTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[ParentTabMenuId], [MenuName] ,[PageId],[CreatedDate]) VALUES 
									('Upload Functions',@TabParentMenuId, 'Upload Functions' ,@PageId ,SYSDATETIME() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ,[CreatedDate]) VALUES ( '/Comp/Bulk/UploadObjects/List.aspx' ,SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadObjects/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('UPLOAD_OBJECT','Upload Objects',@UrlId,SYSDATETIME() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOAD_OBJECT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadObjects/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 6, @UrlId,SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME() )

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='BulkOperations'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOAD_OBJECT'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[ParentTabMenuId], [MenuName] , [PageId],[CreatedDate]) VALUES 
									('Upload Objects',@TabParentMenuId, 'Upload Objects' ,@PageId,SYSDATETIME()  )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ,[CreatedDate]) VALUES ( '/Comp/Bulk/UploadExceptions/List.aspx',SYSDATETIME() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadExceptions/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('UPLOADEXCEPTIONS','Upload Exceptions',@UrlId,SYSDATETIME() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOADEXCEPTIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadExceptions/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 6, @UrlId,SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME() )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='BulkOperations'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOADEXCEPTIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[ParentTabMenuId], [MenuName] , [PageId],[CreatedDate]) VALUES 
								('Upload Exceptions',@TabParentMenuId, 'Upload Exceptions' ,@PageId ,SYSDATETIME() )



