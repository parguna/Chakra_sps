
delete DI_M_DashboardAccessRights
DBCC CHECKIDENT (DI_M_DashboardAccessRights, reseed, 1)
GO

delete DI_M_AdminModeItem
DBCC CHECKIDENT (DI_M_AdminModeItem, reseed, 1)
GO

delete DI_M_DefaultRoleDashboard
DBCC CHECKIDENT (DI_M_DefaultRoleDashboard, reseed, 1)
GO

delete DI_M_DefaultRoleQuickLink
DBCC CHECKIDENT (DI_M_DefaultRoleQuickLink, reseed, 1)
GO

delete DI_M_QuickLinkAccessRights
DBCC CHECKIDENT (DI_M_QuickLinkAccessRights, reseed, 1)
Go

Delete [DI_M_AccessRights]
DBCC CHECKIDENT ([DI_M_AccessRights], reseed, 1)
go

Delete [DI_M_PagePermission]
DBCC CHECKIDENT ([DI_M_PagePermission], reseed, 1)
go

Delete DI_M_Permission
DBCC CHECKIDENT (DI_M_Permission, reseed, 1)
go

Delete [DI_M_TabMenu]
DBCC CHECKIDENT ([DI_M_TabMenu], reseed, 1)
go

Delete DI_M_Page
DBCC CHECKIDENT (DI_M_Page, reseed, 1)
go

Delete DI_M_PageUrl
DBCC CHECKIDENT (DI_M_PageUrl, reseed, 1)
go

Delete UserRole
GO

SET IDENTITY_INSERT [DI_M_Permission] ON
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (1, 'View',NULL,NULL,SYSDATETIME())--1
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (2, 'Detail','/Images/action-buttons/detail_2.png',NULL,SYSDATETIME())--2
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (3, 'Add','/Images/action-buttons/add.png',NULL,SYSDATETIME())--3
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (4 ,'Edit','/Images/action-buttons/edit.png',NULL,SYSDATETIME())--4
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (5 ,'Delete','/Images/action-buttons/delete.png','confirmAction(this); return false',SYSDATETIME())--5
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (6 ,'Save','/Images/action-buttons/save.png',NULL,SYSDATETIME())--6
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (7 ,'Customize','/Images/action-buttons/customize.png',NULL,SYSDATETIME())--7
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (8 ,'AccessRights','/Images/action-buttons/customize.png',NULL,SYSDATETIME())--8
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (9 ,'RunNow','/Images/action-buttons/runnow.png',NULL,SYSDATETIME())--9
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (10 ,'Approve','/Images/action-buttons/approve.png',NULL,SYSDATETIME())--10
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (11 ,'Reject','/Images/action-buttons/reject.png',NULL,SYSDATETIME())--11
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (12 ,'Export','/Images/action-buttons/import.png','ShowProgress();',SYSDATETIME())--12
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (13 ,'GeneratePair','/Images/action-buttons/pair-conflict.png',NULL,SYSDATETIME()) -- 13
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (14 ,'PagePermission','/Images/action-buttons/page-permission.png',NULL,SYSDATETIME()) -- 14
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (15 ,'Delegate','/Images/action-buttons/delegate.png',NULL,SYSDATETIME()) -- 15
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (16 ,'Clone','/Images/action-buttons/clone.png',NULL,SYSDATETIME()) -- 16
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (17 ,'Upload','/Images/action-buttons/upload.png',NULL,SYSDATETIME()) -- 17
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (18 ,'DefaultRoleSettings','/Images/action-buttons/default.png',NULL,SYSDATETIME()) -- 18
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (19 ,'ReSubmit','/Images/action-buttons/resubmit.png',NULL,SYSDATETIME()) -- 19
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (20 ,'Withdraw','/Images/action-buttons/withdraw.png','return ShowModalPopup();',SYSDATETIME()) -- 20
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (21 ,'Download','/Images/action-buttons/Download.png',NULL,SYSDATETIME()) -- 21
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (22 ,'MarkAsRead','/Images/action-buttons/read-notification.png',NULL,SYSDATETIME()) -- 22
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (23 ,'Reply','/Images/action-buttons/reply-notification.png',NULL,SYSDATETIME()) -- 23
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (24 ,'Schedule','/Images/action-buttons/SchdReport.png',NULL,SYSDATETIME()) --24
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (25 ,'ScheduleHistory','/Images/action-buttons/report-history.png',NULL,SYSDATETIME()) --25
INSERT INTO [dbo].[DI_M_Permission] (id, Name, IconURL,ClientScript, CreatedDate) values (26 ,'ViewSchedule','/Images/action-buttons/view-schd.png',NULL,SYSDATETIME())--26
INSERT INTO [dbo].[DI_M_Permission] (id, Name, IconURL,ClientScript, CreatedDate) values (27 ,'EditSchedule','/Images/action-buttons/edit-schd.png',NULL,SYSDATETIME())--27

INSERT INTO [dbo].[DI_M_Permission] (id, Name, IconURL,ClientScript, CreatedDate) values (28 ,'Submit','/Images/action-buttons/submit.png',NULL,SYSDATETIME())--28
INSERT INTO [dbo].[DI_M_Permission] (id, Name, IconURL,ClientScript, CreatedDate) values (29 ,'Revert','/Images/action-buttons/revert.png',NULL,SYSDATETIME())--29
INSERT INTO [dbo].[DI_M_Permission] (id, Name, IconURL,ClientScript, CreatedDate) values (30 ,'Remark','/Images/action-buttons/remark.png',NULL,SYSDATETIME())--30
INSERT INTO [dbo].[DI_M_Permission] (id, Name, IconURL,ClientScript, CreatedDate) values (31 ,'AttestationView','/Images/action-buttons/view.png',NULL,SYSDATETIME())--31
INSERT INTO [dbo].[DI_M_Permission] (id, Name, IconURL,ClientScript, CreatedDate) values (32 ,'AttestationReport','/Images/action-buttons/export.png','ShowProgress();',SYSDATETIME())--32
INSERT INTO [dbo].[DI_M_Permission] (id, Name, IconURL,ClientScript, CreatedDate) values (33 ,'AttestationDetail','/Images/action-buttons/detail_2.png',NULL,SYSDATETIME())--33
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (34 ,'AssignOwner','/Images/action-buttons/delegate.png',NULL,SYSDATETIME()) -- 34
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (35 ,'AlertActions','/Images/action-buttons/alertaction.png',NULL,SYSDATETIME()) -- 35
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (36 ,'RequestExtend','/Images/action-buttons/RequestExtend.png',NULL,SYSDATETIME()) -- 36
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (37 ,'ExtendNonEmployee','/Images/action-buttons/RequestExtend.png',NULL,SYSDATETIME()) -- 37
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript, CreatedDate) values (38 ,'RehireNonEmployee','/Images/action-buttons/rehire.png',NULL,SYSDATETIME()) -- 38

SET IDENTITY_INSERT [DI_M_Permission] OFF

DECLARE @UrlId bigint
DECLARE @PageId bigint
DECLARE @TabParentMenuId bigint

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Home/Home.aspx', SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Profile/Profile.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Request/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Request/Detail.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Task/List.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Task/GroupTasks.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Task/DetailType.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Task/Detail.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AdvanceDelegation/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AdvanceDelegation/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AdvanceDelegation/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/NACR/CreateCart.aspx' , SYSDATETIME())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/ConfigKey/IACM/List.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/ConfigKey/IM/List.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/ConfigKey/PasswordManagement/List.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/ConfigKey/Compliance/List.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/ConfigKey/PIM/List.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/ConfigKey/DGM/List.aspx', SYSDATETIME() )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Notification/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Notification/Detail.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Attestation/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Attestation/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Attestation/Add.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Attestation/Edit.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Attestation/NReview.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Attestation/AttestationOverallStatus.aspx', SYSDATETIME() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/AppSetting/ApplicationSetting.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/AppText/List.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Endpoint/List.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Endpoint/Detail.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Endpoint/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Endpoint/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Exception/List.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Exception/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Exception/Add.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Exception/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Group/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Group/Detail.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Group/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Group/Edit.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Group/Clone.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Role/List.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Role/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Role/Add.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Role/Edit.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Role/ChangeAccessRights.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Role/Clone.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Admin/Recon/ReconException.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/Recon/ExceptionAttribute.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/TrustedSrcConfig/List.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/SubSystemType/List.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/EntitlementManagement/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/EntitlementManagement/Edit.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/EntitlementManagement/Detail.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/CatalogManagement/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/CatalogManagement/Add.aspx'  , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/CatalogManagement/Edit.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/Admin/CatalogManagement/Detail.aspx',SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/UserExclusion/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/EmailNotificationSetting/List.aspx' , SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/MockUpEndpoints/StandingOrder.aspx' , SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/MockUpEndpoints/RiskAssessment.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/JobAllocationCenter/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AlertCenter/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AlertCenter/Detail.aspx' , SYSDATETIME())



INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Workflow/List.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Workflow/Detail.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Workflow/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Workflow/Edit.aspx', SYSDATETIME() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Admin/CompanyMaster/List.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Admin/CompanyMaster/Detail.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Admin/CompanyMaster/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Admin/CompanyMaster/Edit.aspx', SYSDATETIME() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Triggers/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Triggers/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Triggers/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Triggers/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Account/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Account/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Account/Edit.aspx', SYSDATETIME() )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Employee/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Employee/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Employee/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Employee/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Members/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Members/Detail.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Admin/HouseKeeping/List.aspx' , SYSDATETIME() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/Rules/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/Rules/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/Rules/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/Rules/Edit.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/Rules/UserRule.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/ProvisioningFields/List.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/DefaultApplication/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/DefaultApplication/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/DefaultApplication/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/ProvisioningDefaults/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/ProvisioningDefaults/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/ProvisioningDefaults/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/ProvisioningDefaults/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/TechnicalRole/List.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/TechnicalRole/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/TechnicalRole/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/Catalog/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/Catalog/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ProvRS/Catalog/Edit.aspx' , SYSDATETIME())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Discovery/List.aspx'  , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Discovery/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Discovery/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Discovery/Edit.aspx', SYSDATETIME() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Attestation/List.aspx' , SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Attestation/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Attestation/Add.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Attestation/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Compliance/List.aspx' , SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Compliance/Detail.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Compliance/Add.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Compliance/Edit.aspx', SYSDATETIME() )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/AdHocJob/List.aspx'  , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/AdHocJob/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/AdHocJob/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/AdHocJob/Edit.aspx', SYSDATETIME() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Log/Log.aspx'  , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Log/UI.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Log/Configuration.aspx'  , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Log/History.aspx', SYSDATETIME() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Mail/Mail/List.aspx'  , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Mail/Mail/Detail.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Mail/Mail/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Mail/Mail/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Mail/Server/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Mail/Server/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Mail/Server/Add.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Mail/Server/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Mail/Template/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Mail/Template/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Mail/Template/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Mail/Template/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/NonEmployee/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/NonEmployee/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/NonEmployee/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/NonEmployee/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/PasswordMgmt/ApplicationPolicy/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/PasswordMgmt/ApplicationPolicy/Detail.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/PasswordMgmt/ApplicationPolicy/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/PasswordMgmt/ApplicationPolicy/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/PasswordMgmt/GlobalQuestion/List.aspx' , SYSDATETIME())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/PasswordMgmt/PasswordSync/PasswordSync.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/PasswordMgmt/PrivateQuestion/List.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Prov/Request/List.aspx', SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Prov/Request/Detail.aspx', SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Prov/Task/List.aspx', SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Prov/Task/DetailType.aspx', SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Prov/Task/Detail.aspx', SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Prov/SnapShot/List.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES('/ProvRS/EndpointProcess/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES('/ProvRS/EventProcess/List.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/InHouse/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/InHouse/Detail.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/InHouse/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/InHouse/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/InHouse/Permission/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/InHouse/Permission/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/InHouse/Permission/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/InHouse/Permission/Edit.aspx', SYSDATETIME() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/InHouse/Function/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/InHouse/Function/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/InHouse/Function/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/InHouse/Function/Edit.aspx', SYSDATETIME() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Preferences/UserPreference.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES('Admin/AdminItems/List.aspx', SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES('/Report/List.aspx', SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES('/OrgList/OrgList.aspx', SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES('/Report/Attestation/AttestationCampaignStatus.aspx', SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES('/Admin/Attestation/AttestationOverallStatus.aspx', SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/DepartmentRequest/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/DepartmentRequest/Detail.aspx' , SYSDATETIME())


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('#' , SYSDATETIME())




SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Home/Home.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME','Home' ,@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMINISTRATION','Administration',@UrlId , SYSDATETIME())
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('PASSWORDMANAGEMENT','Password Management',@UrlId , SYSDATETIME())
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACCOUNTRECOVERY','Account Recovery',@UrlId , SYSDATETIME())
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER','Users',@UrlId , SYSDATETIME())
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ENDPOINT','Endpoint',@UrlId, SYSDATETIME()) 
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_MAINCONFIGURATION','Configuration',@UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_AUDIT','Audit',@UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ACCESSGOVERNANCE','Access Governance',@UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING','Provisioning',@UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONINGRULESET','Provisioning Rulset',@UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_MAIL','Mails',@UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_LOG','Log',@UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_JOB','Jobs',@UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_INH_SYSTEM','In-house Systems',@UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_APP_CONFIG','Application Configuration',@UrlId, SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/OrgList/OrgList.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ORGLIST','Organization List',@UrlId, SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Profile/Profile.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYPROFILE','My Profile' ,@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYTASK' ,'My Tasks' ,@UrlId , SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/GroupTasks.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYGROUPTASK' ,'My Group Tasks' ,@UrlId , SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYTASKDETAIL','My Task Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AdvanceDelegation/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYDELEGATIONS' ,'My Delegations' ,@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Delegation/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYDELEGATIONS_DETAILDELEGATIONS','Delegation Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Delegation/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYDELEGATIONS_ADDDELEGATIONS','Add Delegation',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Delegation/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYDELEGATIONS_EDITDELEGATIONS','Edit Delegation',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Request/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYREQUESTS' ,'My Requests' ,@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Request/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYREQUESTS_DETAIL','My Request Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/NACR/CreateCart.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_ACCESSREQUESTS','Access Requests',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Notification/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYNOTIFICATIONS' ,'My Notifications' ,@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Notification/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYNOTIFICATIONS_DETAIL','Notification Detail',@UrlId , SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_GENERALCONFIGURATION','Configuration',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/IACM/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_IACMCONFIGURATION','IACM Configuration',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/IM/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_IMCONFIGURATION','IM Configuration',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/PasswordManagement/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PASSWORDMANAGEMENTCONFIGURATION','Password Management',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/Compliance/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_COMPLIANCECONFIGURATION','Compliance',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/PIM/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PIMCONFIGURATION','PIM Configuration',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/DGM/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_DGMCONFIGURATION','DGM Configuration',@UrlId, SYSDATETIME() )




SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/AppSetting/ApplicationSetting.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_APPSETTING','Application Settings',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/AppText/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_APPTEXT','Application Text',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ATTESTATION_DETAIL','Attestations',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_DETAILATTESTATION','Attestation Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ADDATTESTATION','Add Attestation',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_EDITATTESTATION','Edit Attestation',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/NReview.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_NREVIEWATTESTATION', NULL,@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/AttestationOverallStatus.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],   [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ATTESTATION_OVERALLSTATUS', 'Active Attestation',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/Attestation/AttestationCampaignStatus.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],   [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ATTESTATION_ACTIVECAMPAIGN', 'Active Campaign',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ENDPOINTSYSTEM','Endpoint Systems',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/SubSystemType/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_SUBSYSTEM','Sub SystemType',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ENPOINT_DETAILENDPOINT','Endpoint Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ENPOINT_ADDENDPOINT','Add Endpoint',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ENDPOINT_EDITENDPOINT','Edit Endpoint',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_GROUPS','Groups',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_GROUPS_DETAILGROUP','Group Detail',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_GROUPS_ADDGROUP','Add Group',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_GROUP_EDITGROUP','Edit Group',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Clone.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_GROUP_CLONEGROUP','Clone Group',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ROLES','Roles',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/ChangeAccessRights.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ROLES_CHANGEACCESSRIGHTS','Change Access Rights',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ROLES_DETAILROLES','Role Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ROLES_ADDROLES','Add Role',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ROLES_EDITROLES','Edit Role',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Clone.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ROLES_CLONEROLES','Clone Role',@UrlId, SYSDATETIME() )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/TrustedSrcConfig/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_TRUSTEDSOURCE','Trusted Sources',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/HouseKeeping/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_HOUSEKEEPING','Purge Configuration',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_RECON','Reconciliation',@UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ATTESTATION','Attestation',@UrlId, SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/Rules/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONINGRULESET_RULES','Rules',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/Rules/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONINGRULESET_DETAILRULES','Detail Rules',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/Rules/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONINGRULESET_ADDRULES','Add Rules',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/Rules/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONINGRULESET_EDITRULES','Edit Rules',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/ProvisioningFields/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONINGRULESET_PROVISIONINGFIELDS','Provisioning Fields',@UrlId , SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/DefaultApplication/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_APPLICATIONDEFAULTS','Application Defaults',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/DefaultApplication/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_APPLICATIONDEFAULTS_ADD','Add Application Defaults',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/DefaultApplication/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_APPLICATIONDEFAULTS_EDIT','Edit Application Defaults',@UrlId , SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/ProvisioningDefaults/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONINGDEFAULTS','Provisioning Defaults',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/ProvisioningDefaults/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONINGDEFAULTS_DETAIL','Detail Provisioning Defaults',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/ProvisioningDefaults/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONINGDEFAULTS_ADD','Add Provisioning Defaults',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/ProvisioningDefaults/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONINGDEFAULTS_EDIT','Edit Provisioning Defaults',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/TechnicalRole/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_TECHNICALROLE','Technical Roles',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/TechnicalRole/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_TECHNICALROLE_ADD','Add Technical Roles',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/TechnicalRole/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_TECHNICALROLE_EDIT','Edit Technical Roles',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/Catalog/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_CATALOG','Catalogs',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/Catalog/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_CATALOG_ADD','Add Catalog',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/Catalog/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_CATALOG_EDIT','Edit Catalog',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/Rules/UserRule.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONINGRULESET_USERRULES','User Rules',@UrlId , SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/EntitlementManagement/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ENTITLEMENT_MANAGEMENT','Entitlement Management',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/EntitlementManagement/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ENTITLEMENT_MANAGEMENT_EDIT','Entitlement Edit',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/EntitlementManagement/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ENTITLEMENT_MANAGEMENT_DETAIL','Entitlement Detail',@UrlId , SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CatalogManagement/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('CATALOG_MANAGEMENT','Catalog Management',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CatalogManagement/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],   [Name] , [PageUrlId], CreatedDate) VALUES ('CATALOG_MANAGEMENT_ADD', 'Catalog Add',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CatalogManagement/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],   [Name] , [PageUrlId], CreatedDate) VALUES ('CATALOG_MANAGEMENT_EDIT', 'Catalog Edit',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CatalogManagement/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName],   [Name] , [PageUrlId], CreatedDate) VALUES ('CATALOG_MANAGEMENT_DETAIL', 'Catalog Detail',@UrlId , SYSDATETIME())



------------Admin Mode setup-------------

SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [URL]='Admin/AdminItems/List.aspx'
INSERT INTO [dbo].[DI_M_Page]([UniqueName],[Name],[PageUrlId], CreatedDate) VALUES('ADMIN_MODE','ADMINMODE',@UrlId, SYSDATETIME())


--------------------------------------------------------------------------------------------------------


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Workflow/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_WORKFLOW','Workflows',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Workflow/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_WORKFLOW_DETAILWORKFLOW','Workflow Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Workflow/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_WORKFLOW_ADDWORKFLOW','Add Workflow',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Workflow/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_WORKFLOW_EDITWORKFLOW','Edit Workflow',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/JobAllocationCenter/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_JOBALLOCATIONCENTER','Job Allocation Center' ,@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AlertCenter/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ALERTCENTER','Alert Center' ,@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AlertCenter/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ALERTCENTER_DETAIL','Alert Center Detail' ,@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CompanyMaster/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_COMPANY','Company',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CompanyMaster/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_COMPANY_DETAILCOMPANY','Company Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CompanyMaster/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_COMPANY_ADDCOMPANY','Add Company Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CompanyMaster/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_COMPANY_EDITCOMPANY','Edit Company Detail',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Triggers/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_TRIGGERS','Adhoc Emergency Requests',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Triggers/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_TRIGGERS_DETAILTRIGGERS','Adhoc Emergency Request Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Triggers/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_TRIGGERS_ADDTRIGGERS','Add Adhoc Emergency Request',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Triggers/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_TRIGGERS_EDITTRIGGERS','Edit Adhoc Emergency Request',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Discovery/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_DISCOVERYJOB','Discovery Jobs',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Discovery/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_DISCOVERYJOB_DETAILDISCOVERY','Discovery Job Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Discovery/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_DISCOVERYJOB_ADDDISCOVERY','Add Discovery Job',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Discovery/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_DISCOVERYJOB_EDITDISCOVERY','Edit Discovery Job',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Attestation/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ATTESTATIONJOB','Attestation Jobs',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Attestation/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ATTESTATIONJOB_DETAILATTESTATION','Attestation Job Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Attestation/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ATTESTATIONJOB_ADDATTESTATION','Add Attestation Job',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Attestation/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ATTESTATIONJOB_EDITATTESTATION','Edit Attestation Job',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Compliance/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_COMPLIANCEJOB','Compliance Jobs',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Compliance/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_COMPLIANCEJOB_DETAILCOMPLIANCE','Compliance Job Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Compliance/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_COMPLIANCEJOB_ADDCOMPLIANCE','Add Compliance Job',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Compliance/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_COMPLIANCEJOB_EDITCOMPLIANCE','Edit Compliance Job',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Log/Configuration.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('AUDIT_CONFIGURATION','Audit Configuration',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Log/History.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('AUDIT_HISTORY','Audit History',@UrlId , SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/AdHocJob/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ADHOCJOB','Adhoc Jobs',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/AdHocJob/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ADHOCJOB_DETAILADHOC','Adhoc Job Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/AdHocJob/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ADHOCJOB_ADDADHOC','Add Adhoc Job',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/AdHocJob/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ADHOCJOB_EDITADHOC','Edit Adhoc Job',@UrlId , SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Recon/ReconException.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_RECON_EXCEPTION','Exception',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Recon/ExceptionAttribute.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_RECON_EXCEPTIONATTRIBUTE','Exception Attribute',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Log/Log.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_LOG_SERVICELOG','Service Log',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Log/UI.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_LOG_UILOG','UI Log',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Mail/List.aspx'


INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_MAIL_VIEWMAILS','Mails',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Mail/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_MAIL_VIEWMAILS_DETAILMAIL','Mail Detail',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Mail/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_MAIL_VIEWMAILS_EDITMAIL','Edit Mail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Server/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_MAIL_SMTPSERVER','Mail Servers',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Server/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_MAIL_SMTPSERVER_DETAILSERVER','Mail Server Detail',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Server/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_MAIL_SMTPSERVER_ADDSERVER','Add Mail Server',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Server/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_MAIL_SMTPSERVER_EDITSERVER','Edit Mail Server',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Template/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_MAIL_TEMPLATES','Mail Templates',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Template/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_MAIL_TEMPLATES_DETAILTEMPLATE','Mail Template Detail',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Template/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_MAIL_TEMPLATES_ADDTEMPLATE','Add Mail Template',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Template/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_MAIL_TEMPLATES_EDITTEMPLATE','Edit Mail Template',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Request/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_MYREQUESTS','Requests',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Request/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_MYDETAILREQUESTS','Request Detail',@UrlId , SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_MYTASK','Tasks',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/DetailType.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_MYTASKDETAILTYPE' ,'Task Detail Type' ,@UrlId , SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROVISIONING_MYTASKDETAIL','Task Detail',@UrlId, SYSDATETIME() )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/SnapShot/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ACCESSSNAPSHOT','Access Snapshot',@UrlId, SYSDATETIME() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/EndpointProcess/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROV_ENDPOINTPROCESS','Provisioning Endpoint Process',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/EventProcess/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PROV_EVENTPROCESS','Provisioning Event Process',@UrlId, SYSDATETIME() )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/ApplicationPolicy/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('PASSWORDMGMT_APPLICATIONPASSWORDPOLICY','Application Password Policies',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/ApplicationPolicy/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('PASSWORDMGMT_APPLICATIONPASSWORDPOLICY_DETAILAPPLICATIONPOLICY','Application Password Policy Detail',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/ApplicationPolicy/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('PASSWORDMGMT_APPLICATIONPASSWORDPOLICY_ADDAPPLICATIONPOLICY','Add Application Password Policy',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/ApplicationPolicy/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('PASSWORDMGMT_APPLICATIONPASSWORDPOLICY_EDITAPPLICATIONPOLICY','Edit Application Password Policy',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/GlobalQuestion/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('PASSWORDMGMT_GLOBALQUESTION','Global Questions',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/PrivateQuestion/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('PASSWORDMGMT_PRIVATEQUESTION','Security Questions',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/PasswordSync/PasswordSync.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('PASSWORDMANAGEMENT_PASSWORDRESET','Password Reset',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Account/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_ACCOUNT','Accounts',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Account/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_ACCOUNT_DETAILACCOUNT','Account Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Account/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_ACCOUNT_EDITACCOUNT','Edit Account',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Employee/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_EMPLOYEE','Employees',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Employee/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_EMPLOYEE_DETAILEMPLOYEE','Employee Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Employee/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_EMPLOYEE_ADDEMPLOYEE','Add Employee',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Employee/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_EMPLOYEE_EDITEMPLOYEE','Edit Employee',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/NonEmployee/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_NONEMPLOYEE','Non Employees',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/NonEmployee/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_NONEMPLOYEE_DETAILNONEMPLOYEE','Non Employee Detail',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/NonEmployee/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_NONEMPLOYEE_ADDNONEMPLOYEE','Add Non Employee',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/NonEmployee/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_NONEMPLOYEE_EDITNONEMPLOYEE','Edit Non Employee',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_INHOUSE_LIST','In-house System Accounts',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_INHOUSE_ADD','Add In-house System Account',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_INHOUSE_EDIT','Edit In-house System Account',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/UserExclusion/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_EXCLUSION','User Exclusion' ,@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/EmailNotificationSetting/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_EmailNotificationSetting','Email Notification Setting' ,@UrlId , SYSDATETIME())

--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/MockUpEndpoints/StandingOrder.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('STANDING_ORDER','Standing Order' ,@UrlId , SYSDATETIME())
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/MockUpEndpoints/RiskAssessment.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('RISK_ASSESSMENT','Risk Assessment' ,@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Members/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_MEMBER','IDAS User Listing',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Members/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('USER_MEMBER_DETAILEMPLOYEE','IDAS User Detail',@UrlId , SYSDATETIME())




SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_INHOUSE_PERMISSION','In-house System Roles',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_INHOUSE_PERMISSION_ADD','Add In-house System Role',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_INHOUSE_PERMISSION_EDIT','Edit In-house System Role',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Function/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_INHOUSE_FUNCTION','In-house System Functions',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Function/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_INHOUSE_FUNCTION_ADD','Add In-house System Function',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Function/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_INHOUSE_FUNCTION_EDIT','Edit In-house System Function',@UrlId , SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Preferences/UserPreference.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('PREFERENCES_USERPREFERENCE','User Preferences',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('Reports','Report' ,@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/DepartmentRequest/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYDEPARTMENTREQUESTS' ,'My Department Requests' ,@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/DepartmentRequest/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('HOME_MYDEPARTMENTREQUESTS_DETAIL','My Department Request Detail',@UrlId , SYSDATETIME())


--- Tab Menu Insertion

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (NULL, 'HOME','Home' , @PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMINISTRATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (NULL,'ADMINISTRATION', 'Administration' , @PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMANAGEMENT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (NULL,'PASSWORDMANAGEMENT', 'Password Management' , @PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (NULL,'USER', 'Users' , @PageId, SYSDATETIME() )
SELECT @PageId=Id FROM [DI_M_Page] WHERE [UniqueName]='ADMIN_MODE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate)VALUES (NULL,'ADMIN MODE','Admin Mode',@PageId, SYSDATETIME())


--- Home Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='HOME'

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYTASK'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'HOME_MYTASK', 'My Tasks' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYNOTIFICATIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'HOME_MYNOTIFICATIONS', 'My Notifications' , @PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYPROFILE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'HOME_MYPROFILE', 'My Profile' ,@PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYDELEGATIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'HOME_MYDELEGATIONS', 'My Delegations' ,@PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYREQUESTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'HOME_MYREQUESTS', 'My Requests' , @PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYDEPARTMENTREQUESTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'HOME_MYDEPARTMENTREQUESTS', 'My Department Requests' , @PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_ACCESSREQUESTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'HOME_ACCESSREQUESTS', 'Access Requests' , @PageId , SYSDATETIME())


--- Administration Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMINISTRATION'

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAINCONFIGURATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_MAINCONFIGURATION', 'Configuration' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_AUDIT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_AUDIT', 'Audit' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ACCESSGOVERNANCE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_ACCESSGOVERNANCE', 'Access Governance' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PROVISIONING', 'Identity Manager' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ORGLIST'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_ORGLIST', 'Organization List' , @PageId, SYSDATETIME())



-- Admin- Main Configuration Submenus's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_MAINCONFIGURATION'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ENDPOINT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_ENDPOINT', 'Endpoint' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_WORKFLOW'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_WORKFLOW', 'Workflows' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPANY'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_COMPANY', 'Company' , @PageId , SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_GROUPS', 'Groups' , @PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_ROLES', 'Roles' ,@PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INH_SYSTEM'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_INH_SYSTEM', 'In-house Systems' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_JOB'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_JOB', 'Jobs' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_MAIL', 'Mails' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_APP_CONFIG'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_APP_CONFIG', 'Application Configuration' , @PageId, SYSDATETIME())


--Admin- Audit SubMenus
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_AUDIT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_LOG'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_LOG', 'Log' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='AUDIT_CONFIGURATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'AUDIT_CONFIGURATION', 'Configuration' , @PageId , SYSDATETIME())


--Admin- Access Governance Submenu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_ACCESSGOVERNANCE'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_RECON'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_RECON','Reconciliation' , @PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_ATTESTATION', 'Attestation' , @PageId, SYSDATETIME() )


--- Administration (Endpoint) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='ADMIN_ENDPOINT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ENDPOINTSYSTEM'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_ENDPOINTSYSTEM', 'Endpoint Systems' ,@PageId , SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_SUBSYSTEM'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_SUBSYSTEM', 'Sub SystemType' ,@PageId , SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ENTITLEMENT_MANAGEMENT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ENTITLEMENT_MANAGEMENT', 'Entitlement Management' ,@PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='CATALOG_MANAGEMENT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'CATALOG_MANAGEMENT', 'Catalog Management' ,@PageId , SYSDATETIME())



--- Administration (Provisioning) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='ADMIN_PROVISIONING'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_MYREQUESTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PROVISIONING_MYREQUESTS', 'Requests' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_MYTASK'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PROVISIONING__MYTASK', 'Tasks' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRIGGERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_TRIGGERS', 'Adhoc Emergency Requests' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ACCESSSNAPSHOT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_ACCESSSNAPSHOT', 'Access Snapshot' ,@PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGRULESET'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PROVISIONINGRULESET', 'Provisioning Ruleset' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_JOBALLOCATIONCENTER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_JOBALLOCATIONCENTER', 'Job Allocation Center' ,@PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ALERTCENTER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_ALERTCENTER', 'Alert Center' ,@PageId, SYSDATETIME())


--- Administration (ProvisioningRuleset) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='ADMIN_PROVISIONINGRULESET'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGRULESET'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PROVISIONINGRULESET_RULES', 'Rules' ,@PageId  , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_APPLICATIONDEFAULTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PROVISIONINGRULESET_DEFAULTAPPLICATION', 'Application Defaults' ,@PageId  , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGDEFAULTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PROVISIONINGRULESET_PROVISIONINGDEFAULTS', 'Provisioning Defaults' ,@PageId  , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGRULESET_PROVISIONINGFIELDS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PROVISIONINGRULESET_PROVISIONINGFIELDS', 'Provisoning Fields' ,@PageId  , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_TECHNICALROLE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PROVISIONINGRULESET_TECHNICALROLE', 'Technical Roles' ,@PageId  , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_CATALOG'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PROVISIONINGRULESET_CATALOG', 'Catalog' ,@PageId  , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROV_EVENTPROCESS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PROV_EVENTPROCESS', 'Event Process' ,@PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROV_ENDPOINTPROCESS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PROV_ENDPOINTPROCESS', 'Endpoint Process' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGRULESET_USERRULES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PROVISIONINGRULESET_USERRULES', 'User Rules' ,@PageId  , SYSDATETIME())



--- Administration -Audit (Log) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='ADMIN_LOG'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_LOG_SERVICELOG'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_LOG_SERVICELOG', 'Service Log' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_LOG_UILOG'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_LOG_UILOG', 'UI Log' ,@PageId, SYSDATETIME())

--- Administration- Access Governance (Recon) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='ADMIN_RECON'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_RECON_EXCEPTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_RECON_EXCEPTION', 'Exception' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_RECON_EXCEPTIONATTRIBUTE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (0,'ADMIN_RECON_EXCEPTIONATTRIBUTE', 'Exception Attribute' ,@PageId, SYSDATETIME())

--Attestation Sub menu
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='ADMIN_ATTESTATION'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATION_DETAIL'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_ATTESTATION_DETAIL', 'Attestations' , @PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATION_OVERALLSTATUS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_ATTESTATION_OVERALLSTATUS', 'Active Attestation' , @PageId, SYSDATETIME())

--- Administration (Job) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_JOB'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DISCOVERYJOB'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_DISCOVERYJOB', 'Discovery Jobs' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATIONJOB'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_ATTESTATIONJOB', 'Attestation Jobs' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPLIANCEJOB'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (0,'ADMIN_COMPLIANCEJOB', 'Compliance Jobs' , @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ADHOCJOB'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_ADHOCJOB', 'Adhoc Jobs' , @PageId, SYSDATETIME())

--- Administration (Mail) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_MAIL'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_TEMPLATES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_MAIL_TEMPLATES', 'Mail Templates' ,@PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_VIEWMAILS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_MAIL_VIEWMAILS', 'Mails' , @PageId, SYSDATETIME())

--- Administration (Inhouse system) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_INH_SYSTEM'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_LIST'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_INHOUSE_LIST', 'Accounts' , @PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_PERMISSION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_INHOUSE_PERMISSION', 'Roles' , @PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_FUNCTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_INHOUSE_FUNCTION', 'Functions' , @PageId, SYSDATETIME() )

--- Administration (Application Configuration) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_APP_CONFIG'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_APPSETTING'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_APPSETTING', 'Application Settings' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GENERALCONFIGURATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_GENERALCONFIGURATION', 'Configuration' , @PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_SMTPSERVER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_MAIL_SMTPSERVER', 'Mail Servers' ,@PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRUSTEDSOURCE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_TRUSTEDSOURCE', 'Trusted Sources' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_HOUSEKEEPING'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_HOUSEKEEPING','Purge Configuration' , @PageId , SYSDATETIME())

--- Administration (Application Configuration-> Configuration) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_GENERALCONFIGURATION'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_IACMCONFIGURATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_IACMCONFIGURATION','IACM' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_IMCONFIGURATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_IMCONFIGURATION','IM' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PASSWORDMANAGEMENTCONFIGURATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PASSWORDMANAGEMENTCONFIGURATION','Password Management' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPLIANCECONFIGURATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (0,'ADMIN_COMPLIANCECONFIGURATION','Compliance' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PIMCONFIGURATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (0,'ADMIN_PIMCONFIGURATION','PIM' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DGMCONFIGURATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (0,'ADMIN_DGMCONFIGURATION','DGM' , @PageId , SYSDATETIME())


---  =========  ACL Scripts ================

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL','Access Control',@UrlId, SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, NULL, SYSDATETIME())

--===========PageURL

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/Permission/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/Permission/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/Permission/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/Permission/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/URL/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/URL/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/URL/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/URL/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/Page/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/Page/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/Page/Add.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/Page/Edit.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/Page/PagePermission.aspx', SYSDATETIME() )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/TabMenu/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/TabMenu/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/TabMenu/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/ACL/TabMenu/Edit.aspx' , SYSDATETIME())

--===========Page

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_PERMISSION','Permissions',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_DETAILPERMISSION','Permission Detail',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_ADDPERMISSION','Add Permission',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_EDITPERMISSION','Edit Permission',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_URL','URLs' ,@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_DETAILURL','URL Detail',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_ADDURL','Add URL',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_EDITURL','Edit URL',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_PAGE','Pages',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_DETAILPAGE','Page Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_ADDPAGE','Add Page',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_EDITPAGE','Edit Page',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/PagePermission.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_PAGEPERMISSION','Page Permissions',@UrlId, SYSDATETIME() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_MENU','Menus',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_DETAILMENU','Menu Detail',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_ADDMENU','Add Menu',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ACL_EDITMENU','Edit Menu',@UrlId, SYSDATETIME() )

--===========TabMenu

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL'
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_APP_CONFIG'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId], CreatedDate) VALUES ('ACL','Access Control' ,@TabParentMenuId,  @PageId , SYSDATETIME())

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ACL'

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_PERMISSION'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId], CreatedDate) VALUES ('ACL_PERMISSION','Permissions' ,@TabParentMenuId,  @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_URL'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId], CreatedDate) VALUES ('ACL_URL','URLs' ,@TabParentMenuId, @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_PAGE'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId], CreatedDate) VALUES ('ACL_PAGE','Pages' ,@TabParentMenuId, @PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_MENU'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId], CreatedDate) VALUES ('ACL_MENU','Menus' ,@TabParentMenuId, @PageId, SYSDATETIME())


--=========== Page Permission

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_PERMISSION'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 4, @UrlId, SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_URL'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 3, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 4, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 5, NULL, SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_PAGE'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 3, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 4, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 5, NULL, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/PagePermission.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 14, @UrlId, SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_MENU'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 3, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 4, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 5, NULL, SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_EDITPERMISSION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_ADDURL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_EDITURL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_DETAILURL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, NULL, SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_ADDPAGE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_EDITPAGE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_PAGEPERMISSION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_DETAILPAGE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, NULL, SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_ADDMENU'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_EDITMENU'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_DETAILMENU'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, NULL, SYSDATETIME())



-------- Admin Mode Item--------------------------
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName]='ADMIN_MODE'
SELECT @UrlId=Id FROM [DI_M_PageUrl] WHERE [Url] ='Admin/AdminItems/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId,PermissionID,UrlId, CreatedDate) Values(@PageId,1,@UrlId, SYSDATETIME())


--===========PageURL

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/AdminModeItem/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/AdminModeItem/Detail.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/AdminModeItem/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/AdminModeItem/Edit.aspx' , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ITEMS','Admin Mode Items',@UrlId, SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ITEMS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, NULL, SYSDATETIME())



--===========Page

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMINITEM_LIST','Admin Mode Items',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMINITEM__DETAIL','Admin Mode Item Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMINITEM__ADD','Add Admin Mode Item',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMINITEM__EDIT','Edit Admin Mode Item',@UrlId , SYSDATETIME())

--===========TabMenu

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ITEMS'
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ACL'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId], CreatedDate) VALUES ('ADMIN_ITEMS','Admin Mode Items' ,@TabParentMenuId,  @PageId, SYSDATETIME() )

--=========== Page Permission

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ITEMS'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, @UrlId, SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 3, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 4, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 5, NULL, SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMINITEM__ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMINITEM__EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())



--------------- QuickLink

--===========PageURL

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/QuickLink/DefaultRoleQL/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/QuickLink/DefaultRoleQL/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/QuickLink/DefaultRoleQL/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/QuickLink/DefaultRoleQL/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/QuickLink/QL/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/QuickLink/QL/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/QuickLink/QL/Add.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/QuickLink/QL/Edit.aspx', SYSDATETIME() )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/QuickLink/QLAccessRights/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/QuickLink/QLAccessRights/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/QuickLink/QLAccessRights/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/QuickLink/QLAccessRights/Edit.aspx' , SYSDATETIME())

--===========Page

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('QL','Quick Links(QL)',@UrlId, SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, NULL, SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('QL_DROLE','Default Quick Links For Roles',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('QL_DROLE_DETAIL','Default Quick Link for Role Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('QL_DROLE_ADD','Add Default Quick Link for Role',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('QL_DROLE_EDIT','Edit Default Quick Link for Role',@UrlId , SYSDATETIME())


--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/List.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('QL','Quick Links',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('QL_DETAIL','Quick Link Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('QL_ADD','Add Quick Link',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('QL_EDIT','Edit Quick Link',@UrlId , SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('QL_ACCESSRIGHTS','Quick Link Access Rights',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('QL_AR_DETAIL','Quick Link Access Rights Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('QL_AR_ADD','Add Quick Link Access Rights',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('QL_AR_EDIT','Edit Quick Link Access Rights',@UrlId, SYSDATETIME() )

--===========TabMenu

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QUICKLINK'
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ACL'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId], CreatedDate) VALUES ('QUICKLINK','Quick Links' ,@TabParentMenuId,  @PageId, SYSDATETIME() )


--=========== Page Permission

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_DROLE'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 3, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 4, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 5, NULL, SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 3, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 4, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 5, NULL, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 18, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 8, @UrlId, SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_ACCESSRIGHTS'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 3, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 4, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 5, NULL, SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_DROLE_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_DROLE_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_AR_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_AR_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())


----- Dashboard
--===========PageURL

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/Dashboard/DefaultRoleDB/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/Dashboard/DefaultRoleDB/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/Dashboard/DefaultRoleDB/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/Dashboard/DefaultRoleDB/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/Dashboard/DB/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/Dashboard/DB/Detail.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/Dashboard/DB/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/Dashboard/DB/Edit.aspx' , SYSDATETIME())

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/Dashboard/DBAccessRights/List.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/Dashboard/DBAccessRights/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/Dashboard/DBAccessRights/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/AppCustomization/Dashboard/DBAccessRights/Edit.aspx' , SYSDATETIME())

--===========Page

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('DB','Dashboards(DB)',@UrlId, SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, NULL, SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('DB_DROLE','Default Dashboards for Roles',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('DB_DROLE_DETAIL','Default Dashboard For Role Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('DB_DROLE_ADD','Add Default Dashboard For Role',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('DB_DROLE_EDIT','Edit Default Dashboard For Role',@UrlId , SYSDATETIME())


--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/List.aspx'
--INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('DB','Dashboards',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('DB_DETAIL','Dashboard Detail',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('DB_ADD','Add Dashboard',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('DB_EDIT','Edit Dashboard',@UrlId , SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('DB_ACCESSRIGHTS','Dashboard Access Rights',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('DB_AR_DETAIL','Dashboard Access Rights Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('DB_AR_ADD','Add Dashboard Access Rights',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('DB_AR_EDIT','Edit Dashboard Access Rights',@UrlId, SYSDATETIME() )

--===========TabMenu

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DASHBOARD'
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ACL'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId], CreatedDate) VALUES ('DASHBOARD','Dashboards' ,@TabParentMenuId,  @PageId , SYSDATETIME())

--=========== Page Permission

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_DROLE'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate) VALUES( @PageId , 1, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 3, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 4, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 5, NULL, SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 3, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 4, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 5, NULL, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 18, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 8, @UrlId, SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_ACCESSRIGHTS'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 3, @UrlId, SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 4, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 5, NULL, SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_DROLE_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_DROLE_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_AR_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_AR_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 6, NULL, SYSDATETIME())


----User Menu

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='USER'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EMPLOYEE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'USER_EMPLOYEE', 'Employees' ,@PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_NONEMPLOYEE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'USER_NONEMPLOYEE', 'Non Employees' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_ACCOUNT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'USER_ACCOUNT', 'Accounts' ,@PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EXCLUSION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'USER_EXCLUSION', 'User Exclusion' ,@PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_EmailNotificationSetting'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (-1,'ADMIN_EmailNotificationSetting', 'Email Notification Settings' ,@PageId, SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_MEMBER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'USER_MEMBER', 'IDAS User Listing' ,@PageId, SYSDATETIME() )

--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='STANDING_ORDER'
--INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'STANDING_ORDER', 'Standing Order' ,@PageId, SYSDATETIME())


--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RISK_ASSESSMENT'
--INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'RISK_ASSESSMENT', 'Risk Assessment' ,@PageId, SYSDATETIME())


---Password Management

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='PASSWORDMANAGEMENT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMANAGEMENT_PASSWORDRESET'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (-1,'PASSWORDMANAGEMENT_PASSWORDRESET', 'Password Reset' ,@PageId, SYSDATETIME() )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_APPLICATIONPASSWORDPOLICY'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName], [PageId], CreatedDate) VALUES (@TabParentMenuId,'PASSWORDMGMT_APPLICATIONPASSWORDPOLICY', 'Application Password Policy' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACCOUNTRECOVERY'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName], [PageId], CreatedDate) VALUES (-1,'ACCOUNTRECOVERY', 'Account Recovery' , @PageId , SYSDATETIME())

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='ACCOUNTRECOVERY'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_PRIVATEQUESTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'PASSWORDMGMT_PRIVATEQUESTION', 'Security Questions' , @PageId , SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_GLOBALQUESTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'PASSWORDMGMT_GLOBALQUESTION', 'Global Questions' , @PageId , SYSDATETIME())

-- Report Menu

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/RWS/Report/List.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/RWS/Report/Detail.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/RWS/Report/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/RWS/Report/Edit.aspx', SYSDATETIME() )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/RWS/Report/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_Report','Report',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/RWS/Report/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_Report_DETAIL','Report Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/RWS/Report/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_Report_ADD','Add Report',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/RWS/Report/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_Report_EDIT','Edit Report',@UrlId , SYSDATETIME())

--SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_MAINCONFIGURATION'
--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_Report'
--INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_Report', 'Report Definition' , @PageId , SYSDATETIME())

-- Report Job Menu

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Report/List.aspx'  , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Report/Detail.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Report/Add.aspx' , SYSDATETIME())
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/Job/Report/Edit.aspx', SYSDATETIME() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Report/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_REPORTJOB','Report Jobs',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Report/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_REPORTJOB_DETAILREPORT','Report Job Detail',@UrlId , SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Report/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_REPORTJOB_ADDREPORT','Add Report Job',@UrlId, SYSDATETIME() )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Report/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_REPORTJOB_EDITREPORT','Edit Report Job',@UrlId , SYSDATETIME())

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_JOB'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_REPORTJOB'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_REPORTJOB', 'Report Jobs' , @PageId, SYSDATETIME())


------Report Result Menu


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/RWS/ReportResult/List.aspx', SYSDATETIME() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/RWS/ReportResult/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_ReportResult','ReportResult',@UrlId , SYSDATETIME())


--------Password letter -------------- 
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/RWS/PasswordLetter/List.aspx', SYSDATETIME() )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( '/RWS/PasswordLetter/Edit.aspx', SYSDATETIME() )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/RWS/PasswordLetter/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PasswordLetter','Password Letter',@UrlId , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/RWS/PasswordLetter/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_PasswordLetter_EDIT','Edit Password Letter',@UrlId , SYSDATETIME())

--- Inhouse privilege --
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ('/InHouse/Privilege/List.aspx' , SYSDATETIME())

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Privilege/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES ('ADMIN_INHOUSE_PRIVILEGE','In-house System Privilege',@UrlId, SYSDATETIME() )

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_PRIVILEGE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Privilege/List.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 17, @UrlId,SYSDATETIME())


SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_INH_SYSTEM'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_PRIVILEGE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_INHOUSE_PRIVILEGE', 'Privilege' , @PageId, SYSDATETIME() )



