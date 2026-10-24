
DECLARE @PageId bigint
DECLARE @UrlId bigint
DECLARE @TabParentMenuId bigint

-- Reports menu creation
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT','Reports',NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (NULL, 'REPORT','Reports' , @PageId,SYSDATETIME() )

--Report Submenu's
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_GENERAL','General Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_GENERAL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_GENERAL','General' , @PageId,SYSDATETIME() )

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_ENDPOINT','Endpoint Systems Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ENDPOINT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_ENDPOINT','Endpoint System' , @PageId,SYSDATETIME() )


INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_ATTESTATION','Attestation Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_ATTESTATION','Attestation' , @PageId,SYSDATETIME() )

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_PROVISIONING','Provisioning Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_PROVISIONING'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_PROVISIONING','Provisioning' , @PageId,SYSDATETIME() )

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_MANAGEMENT','Management Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MANAGEMENT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_MANAGEMENT','Management' , @PageId,SYSDATETIME() )

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_AUDIT','Audit Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_AUDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_AUDIT','Audit' , @PageId ,SYSDATETIME())

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_COMPLIANCE','Compliance Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_COMPLIANCE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (-1, 'REPORT_COMPLIANCE','Compliance' , @PageId,SYSDATETIME() )

INSERT INTO [dbo].[DI_M_Page]( [UniqueName] , [Name] ,[PageUrlId], [CreatedDate]) VALUES ('REPORT_MYSCHEDULE', 'Report Schedule', Null ,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MYSCHEDULE'
INSERT INTO [dbo].[DI_M_PagePermission] ( PageId , PermissionId, UrlId,CreatedDate) VALUES ( @PageId , 1, NULL, SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], [CreatedDate]) VALUES (@TabParentMenuId,'REPORT_MYSCHEDULE','Report Schedule', @PageId, SYSDATETIME() )


---REPORT_USER Submenu's
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_IDAS','IDAS Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_IDAS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_GENERAL'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_IDAS','IDAS' , @PageId,SYSDATETIME() )


INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_SORA','SORA Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_SORA'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_GENERAL'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_SORA','SO/RA' , @PageId,SYSDATETIME() )


INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Exception','Exception Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Exception'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_GENERAL'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_Exception','Exception' , @PageId,SYSDATETIME() )


INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Notification','Notificaiton Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Notification'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_GENERAL'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_Notification','Notificaiton' , @PageId,SYSDATETIME() )


--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_ALLUSER','All User Report',NULL,SYSDATETIME())
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ALLUSER'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
--SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_IDAS'
--INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_ALLUSER','AllUser' , @PageId,SYSDATETIME() )

--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_USERROLE','UserRole Report',NULL,SYSDATETIME())
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_USERROLE'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
--SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_IDAS'
--INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_USERROLE','User - Role' , @PageId,SYSDATETIME() )

--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_USERGROUP','UserGroup Report',NULL,SYSDATETIME())
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_USERGROUP'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
--SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_IDAS'
--INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_USERGROUP','User - Group' , @PageId,SYSDATETIME() )

--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_ACTIVEEMPLOYEES','All User Report',NULL,SYSDATETIME())
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ACTIVEEMPLOYEES'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
--SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_IDAS'
--INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_ACTIVEEMPLOYEES','Active Employees' , @PageId,SYSDATETIME() )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/User/AllEmployessDetail.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/User/AllEmployessDetail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_ALLUSER','All User Report',@UrlId,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ALLUSER'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_IDAS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_ALLUSER','AllUser' , @PageId,SYSDATETIME() )




INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/User/UserGroup.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/User/UserGroup.aspx'
insert into [dbo].[di_m_page] ( [uniquename] ,  [name] , [pageurlid],[createddate]) values ('REPORT_USERGROUP','User Group',@UrlId,sysdatetime())
select @pageid =id from [di_m_page] where [uniquename] ='REPORT_USERGROUP'
insert into [dbo].[di_m_pagepermission]( pageid , permissionid, urlid,createddate) values( @pageid , 12, @UrlId,sysdatetime())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_IDAS'
insert into [dbo].[di_m_tabmenu] ([parenttabmenuid],[uniquename], [menuname] , [pageid],[createddate]) values (@TabParentMenuId, 'REPORT_USERGROUP','User-Group' , @pageid,sysdatetime() )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/User/UserRole.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/User/UserRole.aspx'
insert into [dbo].[di_m_page] ( [uniquename] ,  [name] , [pageurlid],[createddate]) values ('REPORT_USERROLE','User Role',@UrlId,sysdatetime())
select @pageid =id from [di_m_page] where [uniquename] ='REPORT_USERROLE'
insert into [dbo].[di_m_pagepermission]( pageid , permissionid, urlid,createddate) values( @pageid , 12, @UrlId,sysdatetime())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_IDAS'
insert into [dbo].[di_m_tabmenu] ([parenttabmenuid],[uniquename], [menuname] , [pageid],[createddate]) values (@TabParentMenuId, 'REPORT_USERROLE','User-Role' , @pageid,sysdatetime() )





INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/User/AlertCenterReport.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/User/AlertCenterReport.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_ALERTCENTER','Alert Center' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_Notification'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ALERTCENTER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_ALERTCENTER','Alert Center' , @PageId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/User/AlertCenterReport.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/User/JobAllocationCenterReport.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/User/JobAllocationCenterReport.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_JOBALLOCATIONCENTER','Job Allocation Center' ,@UrlId,SYSDATETIME() )
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_Notification'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_JOBALLOCATIONCENTER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_JOBALLOCATIONCENTER','Job Allocation Center' , @PageId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/User/JobAllocationCenterReport.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/DetailType.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/User/JobAllocationCenterReport.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/MockUpEndpoints/StandingOrder.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/MockUpEndpoints/StandingOrder.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('STANDING_ORDER','Standing Order' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_SORA'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='STANDING_ORDER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'STANDING_ORDER','Standing Order' , @PageId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/MockUpEndpoints/StandingOrder.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/MockUpEndpoints/RiskAssessment.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/MockUpEndpoints/RiskAssessment.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('RISK_ASSESSMENT','Risk Assessment' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_SORA'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RISK_ASSESSMENT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'RISK_ASSESSMENT','Risk Assessment' , @PageId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/MockUpEndpoints/RiskAssessment.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/BackDatedException.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/BackDatedException.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('BACKDATED_EXCEPTION','Backdated Exception' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_Exception'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='BACKDATED_EXCEPTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'BACKDATED_EXCEPTION','Backdated Exception' , @PageId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/BackDatedException.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/OnScreenException.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/OnScreenException.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('ONSCREEN EXCEPTION','CSEL/GoldBar Exception' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_Exception'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ONSCREEN EXCEPTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'ONSCREEN EXCEPTION','CSEL/GoldBar Exception' , @PageId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/OnScreenException.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())



INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/Company/Company.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Company/Company.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPANY','Company' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_IDAS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPANY'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'COMPANY','Company' , @PageId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Company/Company.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())


-- New reports

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Prov','Provisioning' ,NULL,SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Prov'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_PROVISIONING'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Prov'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_Prov','Provisioning Reports' , @PageId,SYSDATETIME() )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/Provisioning/AccessMatrixDetail.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Provisioning/AccessMatrixDetail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Access_Matrix','Access Matrix Details' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_Prov'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Access_Matrix'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (0, 'REPORT_Access_Matrix','Access Matrix Details' , @PageId,SYSDATETIME() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Access_Matrix'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Provisioning/AccessMatrixDetail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 24, NULL,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 25, NULL,SYSDATETIME())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/Provisioning/Organisation.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Provisioning/Organisation.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Organisation','Organization Details' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_Prov'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Organisation'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (0, 'REPORT_Organisation','Organization Details' , @PageId,SYSDATETIME() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Organisation'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Provisioning/Organisation.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 24, NULL,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 25, NULL,SYSDATETIME())


-- Attestation reports
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Att','Attestation Reports' ,NULL,SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Att'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ATTESTATION'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Att'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_Att','Attestation Reports' , @PageId,SYSDATETIME() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/Attestation/Attestations.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Attestation/Attestations.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Attestation_Detail','Attestation Details Report' ,@UrlId,SYSDATETIME() )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_Att'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Attestation_Detail'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_Attestation_Detail','Attestation' , @PageId,SYSDATETIME() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Attestation_Detail'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Attestation/Attestations.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 24, NULL,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 25, NULL,SYSDATETIME())

-- Attestation Campaign Reports
--Campaign Task Detail
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/Attestation/AttestationCampaigntaskDetail.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Attestation/AttestationCampaigntaskDetail.aspx'

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Attestation_Campaign_Task','Attestation Campaign Task' ,@UrlId,SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Attestation_Campaign_Task'

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Attestation_Campaign_Task'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/DetailType.aspx'

INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

--status
--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/Attestation/AttestationCampaignStatus.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/AttestationOverallStatus.aspx'

--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Attestation_Campaign_Status','Attestation Campaign Status' ,@UrlId,SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATION_OVERALLSTATUS'

INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'ADMIN_ATTESTATION_OVERALLSTATUS','Active Attestation' , @PageId,SYSDATETIME() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATION_OVERALLSTATUS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Attestation/AttestationCampaignStatus.aspx'

INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Attestation/AttestationCampaignStatus.aspx'

--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Attestation_Campaign_Status','Attestation Campaign Status' ,@UrlId,SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATION_ACTIVECAMPAIGN'

--INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'ADMIN_ATTESTATION_ACTIVECAMPAIGN','Active Campaign' , @PageId,SYSDATETIME() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATION_ACTIVECAMPAIGN'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Attestation/AttestationCampaigntaskDetail.aspx'

INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

--Report
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/Attestation/AttestationCampaignReport.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Attestation/AttestationCampaignReport.aspx'

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Attestation_Campaign_Report','Attestation Campaign Report' ,@UrlId,SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Attestation_Campaign_Report'

INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_Attestation_Campaign_Report','Attestation Campaign Report' , @PageId,SYSDATETIME() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Attestation_Campaign_Report'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Attestation/AttestationCampaigntaskDetail.aspx'

INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 30, NULL, SYSDATETIME())


-- Compliance reports group header - under Compliance tab
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('COMPLIANCE_REPORTS_GROUP','Compliance Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='COMPLIANCE_REPORTS_GROUP'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_COMPLIANCE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'COMPLIANCE_REPORTS_GROUP','Compliance Reports' , @PageId,SYSDATETIME() )

-- compliance reports under the header - reusing the existing pages and permissions to add undere Compliance reports group
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/SystemSetupReports/List.aspx'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='SYSTEM_SETUP_REPORTS'
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE_REPORTS_GROUP'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='SYSTEM_SETUP_REPORTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'System Setup Reports' ,@PageId ,SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/ManagementReports/List.aspx' 
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='MANAGEMENT_REPORTS'
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE_REPORTS_GROUP'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='MANAGEMENT_REPORTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'Compliance Management Reports' ,@PageId ,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='OPERATIONS_DETAILED_REPORTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Comp/Reports/OperationDetailedReports/List.aspx'
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='COMPLIANCE_REPORTS_GROUP'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='OPERATIONS_DETAILED_REPORTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'Operations Detailed Reports' ,@PageId ,SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/MySchedule/List.aspx',SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate]) VALUES('/Report/MySchedule/View.aspx',SYSDATETIME() )

--My Schedule Report group under the My Schedule Menu

INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('MY_SCHEDULEREPORT','Report Schedule ',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='MY_SCHEDULEREPORT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_MYSCHEDULE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'MY_SCHEDULEREPORT','Report Schedule History' , @PageId,SYSDATETIME() )

-- My  Schedule reports under the header -My Schedule reports group

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/MySchedule/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_MYSCHEDULEHISTORY','My Schedule History' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='MY_SCHEDULEREPORT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MYSCHEDULEHISTORY'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_MYSCHEDULEHISTORY','My Schedule History' , @PageId,SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/MySchedule/View.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 26, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 27, null,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 9, null,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 5, null,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/MySchedule/View.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_MYSCHEDULEHISTORY_VIEW','Reports' ,@UrlId,SYSDATETIME() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_MYSCHEDULEHISTORY_VIEW' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 5, null,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())


----- End of My Schedule Reports---

----- Audit  password letter reports----- 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Pass','Password Letter Reports' ,NULL,SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Pass'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_AUDIT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Pass'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId], [UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_Pass','Password Letter Reports' , @PageId,SYSDATETIME() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/Audit/PasswordLetter.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Audit/PasswordLetter.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_Password_Letter','Password Letter Report' ,@UrlId,SYSDATETIME() )

SELECT @TabParentMenuId =Id 
FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_Pass'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Password_Letter'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_Password_Letter','Password Letter' , @PageId,SYSDATETIME() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_Password_Letter'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Audit/PasswordLetter.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES ('/Report/ReconcilationReport.aspx',SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/ReconcilationReport.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('RECONCILATION_REPORT','Reconcilation Report' ,@UrlId,SYSDATETIME() )
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_AUDIT_ROLEACCESS'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RECONCILATION_REPORT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'RECONCILATION_REPORT','Reconcilation Report' , @PageId,SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/ReconcilationReport.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

---REPORT_MANAGEMENT Submenu's
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ('REPORT_ManagementReport','Management Reports',NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='REPORT_ManagementReport'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,CreatedDate) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_MANAGEMENT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES (@TabParentMenuId, 'REPORT_ManagementReport','Reports' , @PageId,SYSDATETIME() )

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='REPORT_ManagementReport'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ReportResult'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_ReportResult', 'Management Reports' , @PageId , SYSDATETIME())