DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_REPORTS','Reports',@UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_REPORTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_REPORTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId,'COMPLIANCE_REPORTS',
'Reports' , @PageId,SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ( '/Comp/Reports/SystemSetupReports/List.aspx' ,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/SystemSetupReports/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('SYSTEM_SETUP_REPORTS','System Setup Reports',@UrlId ,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='SYSTEM_SETUP_REPORTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/SystemSetupReports/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE_REPORTS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='SYSTEM_SETUP_REPORTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'System Setup Reports' ,@PageId ,SYSDATETIME())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ,[CreatedDate]) VALUES ( '/Comp/Reports/ManagementReports/List.aspx' ,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/ManagementReports/List.aspx' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('MANAGEMENT_REPORTS','Compliance Management Reports',@UrlId ,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='MANAGEMENT_REPORTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/ManagementReports/List.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE_REPORTS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='MANAGEMENT_REPORTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'Compliance Management Reports' ,@PageId ,SYSDATETIME())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ,[CreatedDate]) VALUES ( '/Comp/Reports/OperationDetailedReports/List.aspx',SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/OperationDetailedReports/List.aspx' 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('OPERATIONS_DETAILED_REPORTS','Operations Detailed Reports',@UrlId,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='OPERATIONS_DETAILED_REPORTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/OperationDetailedReports/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE_REPORTS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='OPERATIONS_DETAILED_REPORTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'Operations Detailed Reports' ,@PageId ,SYSDATETIME())

--Role Access report
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] ) VALUES ('/Report/Audit/RoleAccess.aspx' ,N'System', getdate(), N'System', getdate())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Audit/RoleAccess.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_AUDIT_ROLEACCESS','Audit Role Accesses' ,null,N'System', getdate(), N'System', getdate() )
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ('REPORT_ROLEACCESS','Audit Role Access' ,@UrlId,N'System', getdate(), N'System', getdate() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_AUDIT_ROLEACCESS'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, null,N'System', getdate(), N'System', getdate())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ROLEACCESS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Audit/RoleAccess.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 1, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 12, @UrlId,N'System', getdate(), N'System', getdate())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId, [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES( @PageId , 24, NULL,N'System', getdate(), N'System', getdate())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/ReconcilationReport.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/ReconcilationReport.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('RECONCILATION_REPORT','Reconcilation Report' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_AUDIT_ROLEACCESS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RECONCILATION_REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'RECONCILATION_REPORT','Reconcilation Report' , @PageId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/ReconcilationReport.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())



INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Login','Login Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Login'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_AUDIT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_Login','Login Reports' , @PageId,SYSDATETIME() )

INSERT INTO DI_M_PageUrl (url,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)values('/Report/Audit/LoginStatus.aspx','System',GETDATE(),'System',GETDATE())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Audit/LoginStatus.aspx'
INSERT INTO DI_M_Page (UniqueName,Name,PageUrlId,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)values('REPORT_LoginStatus','Login Status',@UrlId,'System',GETDATE(),'System',GETDATE())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_LoginStatus'
insert into DI_M_PagePermission(PageId,PermissionId,UrlId,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)values(@PageId,12,@UrlId,'System',GETDATE(),'System',GETDATE())

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='REPORT_Login'

insert into DI_M_TabMenu (UniqueName,MenuName,ParentTabMenuId,PageId,CreatedBy,CreatedDate,LastModifiedBy,
LastModifiedDate)values('REPORT_LoginStatus','Login Status',@TabParentMenuId,@PageId,'System',GETDATE(),'System',GETDATE())

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_AUDIT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_AUDIT_ROLEACCESS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_AUDIT_ROLEACCESS','Audit' , @PageId,N'System', getdate(), N'System' , getdate())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ROLEACCESS'
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_AUDIT_ROLEACCESS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName],[MenuName] , [PageId], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (@TabParentMenuId, 'REPORT_ROLEACCESS','Role Access' , @PageId,N'System', getdate(), N'System' , getdate())
SELECT @TabParentMenuId =Id FROM [DI_M_PagePermission] WHERE [PageId] = @PageId and [PermissionId] = 1

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_AUDIT_ROLEACCESS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='AUDIT_HISTORY'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'AUDIT_HISTORY', 'History' , @PageId , SYSDATETIME())

