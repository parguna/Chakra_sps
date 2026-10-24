DECLARE @PageId bigint
DECLARE @UrlId bigint
Declare @pagePer bigint
Declare @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Reconciliation/List.aspx'  , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Reconciliation/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_RECONCILIATIONJOB','Reconciliation Jobs',@UrlId , SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_RECONCILIATIONJOB'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Reconciliation/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())


SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_JOB'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_RECONCILIATIONJOB'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_RECONCILIATIONJOB', 'Reconciliation Jobs' , @PageId, SYSDATETIME())













------------ Inhouse connector script --------------------


IF NOT EXISTS (SELECT * FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] = 'CanRunDiscovery' and [CreatedBy] = 'System')

INSERT INTO [dbo].[DI_M_EndpointConfigurationKey]([KeyName], [Caption], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 ('CanRunDiscovery','Can Run Discovery', 'System',GETDATE(),'System', GETDATE())



DECLARE @EndpointSystemTypeId bigint
DECLARE @KeyId bigint

SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='In-house System'


-- Inhouse
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='CanRunDiscovery'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,2,'bit','',0,'', 'System',GETDATE(),'System', GETDATE())




SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='UserName'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,3,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),NULL, NULL)

SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='Password'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,4,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),NULL, NULL)

SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='UserFilePath'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,5,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),NULL, NULL)
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='RoleFilePath'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,6,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),NULL, NULL)
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='FunctionFilePath'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,7,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),NULL, NULL)
SELECT @KeyId =Id FROM [DI_M_EndpointConfigurationKey] WHERE [KeyName] ='PrivillegeAcctTransFilePath'
INSERT INTO [dbo].[DI_M_EndpointKeyConfiguration]([EndpointSystemTypeId], [KeyId], [Order], [DataType], [RegExValidationPattern], [IsRequired], [ClientScript], [CreatedBy],[CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES											 (@EndpointSystemTypeId,@KeyId,8,'varchar','',1,'onkeypress:return preventstartingspace(this);', 'System',GETDATE(),NULL, NULL)


GO




insert into DI_M_EndpointConfigurationDetail values (26,30,'',Null,null,null,null)
insert into DI_M_EndpointConfigurationDetail values (26,31,'',Null,null,null,null)
insert into DI_M_EndpointConfigurationDetail values (26,32,'',Null,null,null,null)
insert into DI_M_EndpointConfigurationDetail values (26,33,'',Null,null,null,null)

insert into DI_M_EndpointConfigurationDetail values (26,3,'',Null,null,null,null)
insert into DI_M_EndpointConfigurationDetail values (26,4,'',Null,null,null,null)


DECLARE @TypeId int
SELECT @TypeId =Id FROM [DI_M_EndpointSystemType] WHERE [EndpointSystemTypeName]=9

IF NOT EXISTS (SELECT * FROM [DI_M_EndpointObjects] WHERE [EndpointSystemTypeId]=@TypeId and [ObjectName]=N'PrivillegeAcctTrans' )
	INSERT [dbo].[DI_M_EndpointObjects] ([EndpointSystemTypeId], [ObjectName], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TypeId, N'PrivillegeAcctTrans', N'System', getdate(), N'System', getdate())


----------------- end of inhouse connector script --------------------



---------- Start Custom Report script --------

DECLARE @UrlId bigint
DECLARE @PageId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/CustomReport/List.aspx'  , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/CustomReport/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/CustomReport/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/CustomReport/Edit.aspx', SYSDATETIME() )

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
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_CUSTOMREPORTJOB', 'Custom Report Jobs' , @PageId, SYSDATETIME())





DECLARE @UrlId bigint
DECLARE @PageId bigint
DECLARE @TabParentMenuId bigint


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_CUSTOMREPORTJOB'
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

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_CUSTOMREPORTJOB_ADDREPORT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_CUSTOMREPORTJOB_EDITREPORT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_CUSTOMREPORTJOB_DETAILREPORT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())




INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('Custom_REPORT','Custom Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='Custom_REPORT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_GENERAL'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'Custom_REPORT','Custom Report' , @PageId,SYSDATETIME() )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/CustomReport/CustomReport.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/CustomReport/CustomReport.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_CustomReport','Custom Report' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='Custom_REPORT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_CustomReport'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_CustomReport','Custom Report' , @PageId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/CustomReport/CustomReport.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())


---------- end custom report script -------------

GO

---------- Start Custom Report Viewer script --------

DECLARE @UrlId bigint
DECLARE @PageId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/CustomReport/CustomReportViewer.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Report/CustomReport/CustomReportViewer.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_CustomReportViewer','Custom Report Viewer' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId = -1
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_CustomReportViewer'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
---------- end Custom Report Viewer script --------
