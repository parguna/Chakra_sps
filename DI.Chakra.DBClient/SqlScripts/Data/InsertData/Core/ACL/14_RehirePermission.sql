
DECLARE @UrlId bigint
DECLARE @PageId bigint

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/NonEmployee/Rehire.aspx' , SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/NonEmployee/Rehire.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_NONEMPLOYEE_REHIRE','Rehire Non Employee',@UrlId , SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_NONEMPLOYEE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/NonEmployee/Rehire.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 38, @UrlId,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_NONEMPLOYEE_REHIRE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
