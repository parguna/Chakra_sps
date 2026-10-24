DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('REPORTS','Reports',@UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'REPORTS',
'Reports' , @PageId)

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Comp/Reports/SystemSetupReports/List.aspx' )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/SystemSetupReports/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('SYSTEM_SETUP_REPORTS','System Setup Reports',@UrlId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='SYSTEM_SETUP_REPORTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/SystemSetupReports/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORTS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='SYSTEM_SETUP_REPORTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'System Setup Reports' ,@PageId  )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Comp/Reports/ManagementReports/List.aspx' )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/ManagementReports/List.aspx' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('MANAGEMENT_REPORTS','Management Reports',@UrlId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='MANAGEMENT_REPORTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/ManagementReports/List.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORTS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='MANAGEMENT_REPORTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'Management Reports' ,@PageId  )




INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Comp/Reports/OperationDetailedReports/List.aspx' )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/OperationDetailedReports/List.aspx' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('OPERATIONS_DETAILED_REPORTS','Operations Detailed Reports',@UrlId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='OPERATIONS_DETAILED_REPORTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/OperationDetailedReports/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORTS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='OPERATIONS_DETAILED_REPORTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'Operations Detailed Reports' ,@PageId  )










