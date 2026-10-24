
DECLARE @PageId int
DECLARE @UrlId int

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Admin/AppText/List.aspx' )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/AppText/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('APPTEXTS','Application Texts',@UrlId )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='APPTEXTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/AppText/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)

