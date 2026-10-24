
----Page--

-----Menus--


----ComapnyMenu--
--DECLARE @UrlId bigint
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CompanyMaster/List.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_COMPANY','Company',@UrlId , SYSDATETIME())
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CompanyMaster/Detail.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_COMPANY_DETAILCOMPANY','Company Detail',@UrlId , SYSDATETIME())
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CompanyMaster/Add.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_COMPANY_ADDCOMPANY','Add Company Detail',@UrlId , SYSDATETIME())
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CompanyMaster/Edit.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_COMPANY_EDITCOMPANY','Edit Company Detail',@UrlId , SYSDATETIME())

----SubSystemTypeMenu--

--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_SUBSYSTEM'
--INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_SUBSYSTEM', 'Sub SystemType' ,@PageId , SYSDATETIME())

--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/SubSystemType/List.aspx' , SYSDATETIME())
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/SubSystemType/List.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_SUBSYSTEM','Sub SystemType',@UrlId , SYSDATETIME())



----UserExclusionMenu--
--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/UserExclusion/List.aspx' , SYSDATETIME())
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/UserExclusion/List.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_EXCLUSION','User Exclusion' ,@UrlId , SYSDATETIME())
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EXCLUSION'
--INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'USER_EXCLUSION', 'User Exclusion' ,@PageId, SYSDATETIME())

----Job Allocation Center--
--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/JobAllocationCenter/List.aspx' , SYSDATETIME())
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/JobAllocationCenter/List.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_JOBALLOCATIONCENTER','Job Allocation Center' ,@UrlId , SYSDATETIME())
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_JOBALLOCATIONCENTER'
--INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_JOBALLOCATIONCENTER', 'Job Allocation Center' ,@PageId, SYSDATETIME())

------Permission---


--DECLARE @PageId bigint

----UserExclusion---
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EXCLUSION'
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='UserExclusion/List.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

----Job Allocation Center---
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_JOBALLOCATIONCENTER'
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='JobAllocationCenter/List.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

----CompanyMaster---
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPANY'
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/CompanyMaster/List.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/CompanyMaster/Detail.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/CompanyMaster/Add.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/CompanyMaster/Edit.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/CompanyMaster/List.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPANY_DETAILCOMPANY'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPANY_ADDCOMPANY'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPANY_EDITCOMPANY'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())


-----SubSytemType--
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_SUBSYSTEM'
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/SubSystemType/List.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())





