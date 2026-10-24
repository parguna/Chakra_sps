
DECLARE @PageId bigint
DECLARE @UrlId bigint

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ( '/Admin/AppText/List.aspx' ,SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/AppText/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('APPTEXTS','Application Texts',@UrlId,SYSDATETIME() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='APPTEXTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/AppText/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())

