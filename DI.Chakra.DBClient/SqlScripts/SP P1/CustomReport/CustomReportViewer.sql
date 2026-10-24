DECLARE @UrlId bigint
DECLARE @PageId bigint
DECLARE @TabParentMenuId bigint


DELETE FROM [DBO].[DI_M_AccessRights] WHERE PagePermissionId IN (SELECT ID FROM [dbo].[DI_M_PagePermission] WHERE PageId IN ( SELECT ID FROM [dbo].[DI_M_Page] WHERE PageUrlId IN (SELECT ID FROM [dbo].[DI_M_PageUrl] WHERE [Url]= '/Report/CustomReport/CustomReportViewer.aspx'))
)
DELETE FROM [dbo].[DI_M_PagePermission] WHERE PageId IN ( SELECT ID FROM [dbo].[DI_M_Page] WHERE PageUrlId IN (SELECT ID FROM [dbo].[DI_M_PageUrl] WHERE [Url]= '/Report/CustomReport/CustomReportViewer.aspx'))
DELETE FROM [dbo].[DI_M_Page] WHERE PageUrlId IN (SELECT ID FROM [dbo].[DI_M_PageUrl] WHERE [Url]= '/Report/CustomReport/CustomReportViewer.aspx')
DELETE FROM [dbo].[DI_M_PageUrl] WHERE [Url]= '/Report/CustomReport/CustomReportViewer.aspx'



IF  NOT  EXISTS ( SELECT 1 FROM [dbo].[DI_M_PageUrl] WHERE [Url]= '/Report/CustomReport/CustomReportViewer.aspx') 
   BEGIN
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/CustomReport/CustomReportViewer.aspx',SYSDATETIME() )
END


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/CustomReport/CustomReportViewer.aspx'

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_CustomReportViewer','Custom Report Viewer' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId = -1
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_CustomReportViewer'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/CustomReport/CustomReportViewer.aspx'
 
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_CustomReportViewer'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())