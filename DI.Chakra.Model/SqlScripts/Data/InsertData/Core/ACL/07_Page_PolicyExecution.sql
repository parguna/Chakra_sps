DECLARE @PageId int
DECLARE @UrlId int
DECLARE @TabParentMenuId int

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE'

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Comp/PolicyExecution/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Comp/PolicyExecution/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Comp/PolicyExecution/Edit.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Comp/PolicyExecution/Detail.aspx' )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/PolicyExecution/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('COMPLIANCE_POLICYEXECUTION','PolicyExecution' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/PolicyExecution/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('COMPLIANCE_POLICYEXECUTION_ADD','Add PolicyExecution' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/PolicyExecution/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('COMPLIANCE_POLICYEXECUTION_EDIT','Edit PolicyExecution' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/PolicyExecution/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('COMPLIANCE_POLICYEXECUTION_Detail','Detail PolicyExecution' ,@UrlId )



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_POLICYEXECUTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/PolicyExecution/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/PolicyExecution/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/PolicyExecution/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/PolicyExecution/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_POLICYEXECUTION_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_POLICYEXECUTION_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_POLICYEXECUTION_Detail'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)


 --MENU

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_POLICYEXECUTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId, 'COMPLIANCE_POLICYEXECUTION', 'PolicyExecution' , @PageId )
