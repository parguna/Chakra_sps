 

DECLARE @UrlId bigint
DECLARE @PageId bigint
DECLARE @TabParentMenuId bigint

IF  NOT  EXISTS ( SELECT 1 FROM [dbo].[DI_M_PageUrl] WHERE [Url]= '/Job/CustomReport/List.aspx') 
   BEGIN
   INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/CustomReport/List.aspx'  , SYSDATETIME())
   END 

IF  NOT  EXISTS ( SELECT 1 FROM [dbo].[DI_M_PageUrl] WHERE [Url]= '/Job/CustomReport/Detail.aspx') 
   BEGIN
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/CustomReport/Detail.aspx' , SYSDATETIME())
 END

IF  NOT  EXISTS ( SELECT 1 FROM [dbo].[DI_M_PageUrl] WHERE [Url]= '/Job/CustomReport/Add.aspx') 
   BEGIN
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/CustomReport/Add.aspx' , SYSDATETIME())
 END

IF  NOT  EXISTS ( SELECT 1 FROM [dbo].[DI_M_PageUrl] WHERE [Url]= '/Job/CustomReport/Edit.aspx') 
   BEGIN
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/CustomReport/Edit.aspx', SYSDATETIME() )
 END


delete from DI_M_TabMenu where PageId in (select Id FROM [DI_M_Page] WHERE [UniqueName] ='Custom_REPORT')
delete from DI_M_AccessRights where PagePermissionId in (SELECT Id from DI_M_PagePermission where PageId in (select Id FROM [DI_M_Page] WHERE [UniqueName] ='Custom_REPORT')) 
delete from DI_M_PagePermission where PageId in (select Id FROM [DI_M_Page] WHERE [UniqueName] ='Custom_REPORT')
delete FROM [DI_M_Page] WHERE [UniqueName] ='Custom_REPORT'



IF EXISTS ( SELECT 1 FROM [dbo].[DI_M_Page] WHERE  [UniqueName] in ( 'ADMIN_CUSTOMREPORTJOB','ADMIN_CUSTOMREPORTJOB_DETAILREPORT','ADMIN_CUSTOMREPORTJOB_ADDREPORT','ADMIN_CUSTOMREPORTJOB_EDITREPORT') ) 
BEGIN

DELETE FROM DI_M_TabMenu where PAGEID IN (SELECT  Id FROM [DI_M_Page] WHERE [UniqueName] in 
         ( 'ADMIN_CUSTOMREPORTJOB','ADMIN_CUSTOMREPORTJOB_DETAILREPORT','ADMIN_CUSTOMREPORTJOB_ADDREPORT','ADMIN_CUSTOMREPORTJOB_EDITREPORT')  )


 DELETE FROM DI_M_AccessRights where PagePermissionId in (
select id from [DI_M_PagePermission] where PageId 
in 
(SELECT  Id FROM [DI_M_Page] WHERE [UniqueName] in ( 'ADMIN_CUSTOMREPORTJOB','ADMIN_CUSTOMREPORTJOB_DETAILREPORT','ADMIN_CUSTOMREPORTJOB_ADDREPORT','ADMIN_CUSTOMREPORTJOB_EDITREPORT')  
 ))

DELETE FROM  [DI_M_PagePermission] where PageId 
in (SELECT  Id FROM [DI_M_Page] WHERE [UniqueName] in ( 'ADMIN_CUSTOMREPORTJOB','ADMIN_CUSTOMREPORTJOB_DETAILREPORT','ADMIN_CUSTOMREPORTJOB_ADDREPORT','ADMIN_CUSTOMREPORTJOB_EDITREPORT')  )

DELETE FROM [DI_M_Page] WHERE [UniqueName] in ( 'ADMIN_CUSTOMREPORTJOB','ADMIN_CUSTOMREPORTJOB_DETAILREPORT','ADMIN_CUSTOMREPORTJOB_ADDREPORT','ADMIN_CUSTOMREPORTJOB_EDITREPORT')  

END


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/CustomReport/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_CUSTOMREPORTJOB','Custom Report Jobs',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/CustomReport/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_CUSTOMREPORTJOB_DETAILREPORT','Custom Report Job Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/CustomReport/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_CUSTOMREPORTJOB_ADDREPORT','Add Custom Report Job',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/CustomReport/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_CUSTOMREPORTJOB_EDITREPORT','Edit Custom Report Job',@UrlId , SYSDATETIME())

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_JOB'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_CUSTOMREPORTJOB'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) 
VALUES (@TabParentMenuId,'ADMIN_CUSTOMREPORTJOB', 'Custom Report Jobs' , @PageId, SYSDATETIME())
 
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/CustomReport/List.aspx'

INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/CustomReport/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/CustomReport/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/CustomReport/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 9, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/CustomReport/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())


INSERT INTO [dbo].[DI_M_PagePermission] select 
 Id , 5, null,NULL,SYSDATETIME(),NULL,NULL from DI_M_Page where [UniqueName] in
 ('ADMIN_CUSTOMREPORTJOB_DETAILREPORT','ADMIN_CUSTOMREPORTJOB_ADDREPORT','ADMIN_CUSTOMREPORTJOB_EDITREPORT')



GO