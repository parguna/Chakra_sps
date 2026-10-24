DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('BulkOperations','Bulk Operations',@UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='BulkOperations'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='BulkOperations'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'BulkOperations',
'Bulk Operations' , @PageId)

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Comp/Bulk/UploadFunction/List.aspx' )
--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Comp/Bulk/UploadFunction/Add.aspx' )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadFunction/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('UPLOAD_FUNCTION','Upload Functions',@UrlId )
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadFunction/Add.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('UPLOAD_FUNCTION_ADD','Add Function' ,@UrlId )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOAD_FUNCTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadFunction/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, @UrlId )
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId )

--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadFunction/Add.aspx'



--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOAD_FUNCTION_ADD'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='BulkOperations'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOAD_FUNCTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[ParentTabMenuId], [MenuName] , [PageId]) VALUES ('Upload Functions',@TabParentMenuId, 'Upload Functions' ,@PageId  )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Comp/Bulk/UploadObjects/List.aspx' )
--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Comp/Bulk/UploadObjects/Add.aspx' )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadObjects/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('UPLOAD_OBJECT','Upload Objects',@UrlId )
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadObjects/Add.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('UPLOAD_OBJECT_ADD','Add Object' ,@UrlId )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOAD_OBJECT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadObjects/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)

--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadObjects/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, @UrlId )
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId )

--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOAD_OBJECT_ADD'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='BulkOperations'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOAD_OBJECT'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[ParentTabMenuId], [MenuName] , [PageId]) VALUES ('Upload Objects',@TabParentMenuId, 'Upload Objects' ,@PageId  )


--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Comp/Bulk/Purge/List.aspx' )
--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Comp/Bulk/Purge/Add.aspx' )

--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/Purge/List.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('Purge','Purge',@UrlId )
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/Purge/Add.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('Purge_ADD','Add Purge' ,@UrlId )


--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='Purge'
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/Purge/List.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)

--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/Purge/Add.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId )


--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='Purge_ADD'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


--SELECT @TabParentMenuId =Id 
--FROM [DI_M_TabMenu] WHERE UniqueName ='BulkOperations'
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='Purge'
--INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'Purge' ,@PageId  )


--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Comp/Bulk/Restore/List.aspx' )
--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Comp/Bulk/Restore/Add.aspx' )

--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/Restore/List.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('RESTORE','Restore',@UrlId )
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/Restore/Add.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('RESTORE_ADD','Add Restore' ,@UrlId )


--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RESTORE'
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/Restore/List.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)

--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/Restore/Add.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId )


--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RESTORE_ADD'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


--SELECT @TabParentMenuId =Id 
--FROM [DI_M_TabMenu] WHERE UniqueName ='BulkOperations'
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RESTORE'
--INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'Restore' ,@PageId  )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Comp/Bulk/UploadExceptions/List.aspx' )
--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Comp/Bulk/UploadExceptions/Add.aspx' )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadExceptions/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('UPLOADEXCEPTIONS','Upload Exceptions',@UrlId )
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadExceptions/Add.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('UPLOADEXCEPTIONS_ADD','Add Exceptions' ,@UrlId )


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOADEXCEPTIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadExceptions/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)

--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Bulk/UploadExceptions/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, @UrlId )
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId )

--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOADEXCEPTIONS_ADD'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='BulkOperations'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='UPLOADEXCEPTIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[ParentTabMenuId], [MenuName] , [PageId]) VALUES ('Upload Exceptions',@TabParentMenuId, 'Upload Exceptions' ,@PageId  )





