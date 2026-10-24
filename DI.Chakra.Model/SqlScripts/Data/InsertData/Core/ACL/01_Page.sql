
Delete [DI_M_AccessRights]
DBCC CHECKIDENT ([DI_M_AccessRights], reseed, 0)
go
Delete [DI_M_PagePermission]
DBCC CHECKIDENT ([DI_M_PagePermission], reseed, 0)
go
Delete DI_M_Permission
DBCC CHECKIDENT (DI_M_Permission, reseed, 0)
go
Delete [DI_M_TabMenu]
DBCC CHECKIDENT ([DI_M_TabMenu], reseed, 0)
go
Delete DI_M_Page
DBCC CHECKIDENT (DI_M_Page, reseed, 0)
go
Delete DI_M_PageUrl
DBCC CHECKIDENT (DI_M_PageUrl, reseed, 0)

go
Delete UserRole
Delete DI_M_AccessRights

SET IDENTITY_INSERT [DI_M_Permission] ON
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (1, 'View',NULL,NULL)--1
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (2, 'Detail','/Images/action-buttons/detail_2.png',NULL)--2
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (3, 'Add','/Images/action-buttons/add.png',NULL)--3
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (4 ,'Edit','/Images/action-buttons/edit.png',NULL)--4
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (5 ,'Delete','/Images/action-buttons/delete.png','confirmAction(this); return false')--5
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (6 ,'Save','/Images/action-buttons/save.png',NULL)--6
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (7 ,'Customize','/Images/action-buttons/customize.png',NULL)--7
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (8 ,'AccessRights','/Images/action-buttons/customize.png',NULL)--8
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (9 ,'RunNow','/Images/action-buttons/runnow.png',NULL)--9
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (10 ,'Approve','/Images/action-buttons/approve.png',NULL)--10
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (11 ,'Reject','/Images/action-buttons/reject.png',NULL)--11
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (12 ,'Export','/Images/action-buttons/import.png',NULL)--12
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (13 ,'GeneratePair','/Images/action-buttons/pair-conflict.png',NULL) -- 13
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (14 ,'PagePermission','/Images/action-buttons/page-permission.png',NULL) -- 14
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (15 ,'Delegate','/Images/action-buttons/delegate.png',NULL) -- 15
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (16 ,'Clone','/Images/action-buttons/clone.png',NULL) -- 16
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (17 ,'Upload','/Images/action-buttons/upload.png',NULL) -- 17
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (18 ,'DefaultRoleSettings','/Images/action-buttons/default.png',NULL) -- 18
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (19 ,'ReSubmit','/Images/action-buttons/resubmit.png',NULL) -- 19
INSERT INTO [dbo].[DI_M_Permission] (Id, Name, IconURL,ClientScript) values (20 ,'Withdraw','/Images/action-buttons/resubmit.png','return ShowModalPopup();') -- 20
SET IDENTITY_INSERT [DI_M_Permission] OFF

DECLARE @UrlId int
DECLARE @PageId int
DECLARE @TabParentMenuId int

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Home/Home.aspx')

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Profile/Profile.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Request/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Request/Detail.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Task/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Task/GroupTasks.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Task/DetailType.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Task/Detail.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AdvanceDelegation/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AdvanceDelegation/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AdvanceDelegation/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACR/CreateCart.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/SpecialACR/CreateCart.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/ConfigKey/List.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Attestation/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Attestation/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Attestation/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Attestation/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/AppSetting/ApplicationSetting.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/AppText/List.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Endpoint/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Endpoint/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Endpoint/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Endpoint/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Exception/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Exception/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Exception/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Exception/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Group/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Group/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Group/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Group/Edit.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Group/Clone.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Role/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Role/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Role/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Role/Edit.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Role/ChangeAccessRights.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Role/Clone.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Tenant/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Tenant/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Tenant/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Tenant/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/TrustedSrcConfig/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/Admin/Recon/ReconException.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Workflow/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Workflow/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Workflow/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Workflow/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Triggers/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Triggers/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Triggers/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Triggers/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Account/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Account/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Account/Edit.aspx' )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Employee/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Employee/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Employee/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Employee/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Admin/HouseKeeping/List.aspx'  )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Admin/Attestation/List.aspx'  )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Admin/Attestation/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Admin/Attestation/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Admin/Attestation/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Job/Discovery/List.aspx'  )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Job/Discovery/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Job/Discovery/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Job/Discovery/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Job/Attestation/List.aspx'  )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Job/Attestation/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Job/Attestation/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Job/Attestation/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Job/Compliance/List.aspx'  )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Job/Compliance/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Job/Compliance/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Job/Compliance/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Log/Log.aspx'  )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Log/UI.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Mail/Mail/List.aspx'  )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Mail/Mail/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Mail/Mail/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Mail/Mail/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Mail/Server/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Mail/Server/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Mail/Server/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Mail/Server/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Mail/Template/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Mail/Template/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Mail/Template/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Mail/Template/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/NonEmployee/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/NonEmployee/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/NonEmployee/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/NonEmployee/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/PasswordMgmt/ApplicationPolicy/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/PasswordMgmt/ApplicationPolicy/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/PasswordMgmt/ApplicationPolicy/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/PasswordMgmt/ApplicationPolicy/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/PasswordMgmt/GlobalQuestion/List.aspx' )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/PasswordMgmt/PasswordSync/PasswordSync.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/PasswordMgmt/PrivateQuestion/List.aspx' )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/Form/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/Form/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/Form/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/Form/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/Roles/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/Roles/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/Roles/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/Roles/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/RoleMatrix/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/RoleMatrix/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/RoleMatrix/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/RoleMatrix/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/Request/List.aspx')
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/request/detail.aspx')

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/Task/List.aspx')
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/Task/DetailType.aspx')
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/Task/Detail.aspx')

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Prov/SnapShot/List.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/InHouse/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/InHouse/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/InHouse/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/InHouse/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/InHouse/Permission/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/InHouse/Permission/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/InHouse/Permission/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/InHouse/Permission/Edit.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ( '/Preferences/UserPreference.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ([Url])VALUES('Admin/AdminItems/List.aspx')




INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('#' )




SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Home/Home.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('HOME','Home' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMINISTRATION','Administration',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('PASSWORDMANAGEMENT','Password Management',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('USER','User',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING','Provisioning',@UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_MAIL','Mail',@UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_LOG','Log',@UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_JOB','Job',@UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_INH_SYSTEM','Inhouse System',@UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_APP_CONFIG','Application Configuration',@UrlId)


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Profile/Profile.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('HOME_MYPROFILE','My Profile' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('HOME_MYTASK' ,'My Task' ,@UrlId  )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/GroupTasks.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('HOME_MYGROUPTASK' ,'My GroupTask' ,@UrlId  )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('HOME_MYTASKDETAIL','My Task Detail',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AdvanceDelegation/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('HOME_MYDELEGATIONS' ,'My Delegations' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Delegation/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('HOME_MYDELEGATIONS_DETAILDELEGATIONS','Detail Delegation',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Delegation/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('HOME_MYDELEGATIONS_ADDDELEGATIONS','Add Delegation',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Delegation/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('HOME_MYDELEGATIONS_EDITDELEGATIONS','Edit Delegation',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Request/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('HOME_MYREQUESTS' ,'My Requests' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Request/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('HOME_MYREQUESTS_DETAIL','Detail My Request',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACR/CreateCart.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('HOME_ACCESSREQUESTS','Access Request',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/SpecialACR/CreateCart.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('HOME_SPECIALACCESSREQUESTS','Special Access Request',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_GENERALCONFIGURATION','General Configuration',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/AppSetting/ApplicationSetting.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_APPSETTING','AppSetting',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/AppText/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_APPTEXT','AppSetting',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ATTESTATION','Attestation',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_DETAILATTESTATION','Detail Attestation',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ADDATTESTATION','Add Attestation',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_EDITATTESTATION','Edit Attestation',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ENDPOINTSYSTEM','Endpoint System',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ENPOINT_DETAILENDPOINT','Detail Endpoint',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ENPOINT_ADDENDPOINT','Add Endpoint',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ENDPOINT_EDITENDPOINT','Edit Endpoint',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_GROUPS','Group',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_GROUPS_DETAILGROUP','Detail Group',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_GROUPS_ADDGROUP','Add Group',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_GROUP_EDITGROUP','Edit Group',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Clone.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_GROUP_CLONEGROUP','Clone Group',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ROLES','Role',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/ChangeAccessRights.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ROLES_CHANGEACCESSRIGHTS','Change AccessRights',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ROLES_DETAILROLES','Detail Role',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ROLES_ADDROLES','Add Role',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ROLES_EDITROLES','Edit Role',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Clone.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ROLES_CLONEROLES','Clone Role',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Tenant/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_TENANT','Tenant',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Tenant/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_TENANT_DETAILTENANT','Detail Tenant',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Tenant/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_TENANT_ADDTENANT','Add Tenant',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Tenant/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_TENANT_EDITTENANT','Edit Tenant',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/TrustedSrcConfig/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_TRUSTEDSOURCE','Trusted Source',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/HouseKeeping/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_HOUSEKEEPING','House Keeping',@UrlId )






SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Recon/ReconException.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_RECON','Reconciliation',@UrlId )



------------Admin Mode setup-------------

SELECT @UrlId = Id FROM [DI_M_PageUrl] WHERE [URL]='Admin/AdminItems/List.aspx'
INSERT INTO [dbo].[DI_M_Page]([UniqueName],[Name],[PageUrlId]) VALUES('ADMIN_MODE','ADMINMODE',@UrlId)


--------------------------------------------------------------------------------------------------------


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Workflow/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_WORKFLOW','Workflow',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Workflow/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_WORKFLOW_DETAILWORKFLOW','Detail Workflow',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Workflow/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_WORKFLOW_ADDWORKFLOW','Add Workflow',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Workflow/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_WORKFLOW_EDITWORKFLOW','Edit Workflow',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Triggers/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_TRIGGERS','Triggers',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Triggers/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_TRIGGERS_DETAILTRIGGERS','Detail Trigger ',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Triggers/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_TRIGGERS_ADDTRIGGERS','Add Trigger',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Triggers/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_TRIGGERS_EDITTRIGGERS','Edit Trigger',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Discovery/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_DISCOVERYJOB','Discovery Job',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Discovery/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_DISCOVERYJOB_DETAILDISCOVERY','Detail Discovery Job',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Discovery/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_DISCOVERYJOB_ADDDISCOVERY','Add Discovery Job',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Discovery/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_DISCOVERYJOB_EDITDISCOVERY','Edit Discovery Job',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Attestation/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ATTESTATIONJOB','Attestation Job',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Attestation/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ATTESTATIONJOB_DETAILATTESTATION','Detail Attestation Job',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Attestation/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ATTESTATIONJOB_ADDATTESTATION','Add Attestation Job',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Attestation/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ATTESTATIONJOB_EDITATTESTATION','Edit Attestation Job',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Compliance/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_COMPLIANCEJOB','Compliance Job',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Compliance/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_COMPLIANCEJOB_DETAILCOMPLIANCE','Detail Compliance Job',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Compliance/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_COMPLIANCEJOB_ADDCOMPLIANCE','Add Compliance Job',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Job/Compliance/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_COMPLIANCEJOB_EDITCOMPLIANCE','Edit Compliance Job',@UrlId )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Log/Log.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_LOG_SERVICELOG','Service Log',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Log/UI.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_LOG_UILOG','UI Log',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Mail/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_MAIL_VIEWMAILS','Mail',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Mail/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_MAIL_VIEWMAILS_DETAILMAIL','Detail Mail',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Mail/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_MAIL_VIEWMAILS_ADDMAIL','Add Mail',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Mail/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_MAIL_VIEWMAILS_EDITMAIL','Edit Mail',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Server/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_MAIL_SMTPSERVER','Mail Server',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Server/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_MAIL_SMTPSERVER_DETAILSERVER','Detail MailServer',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Server/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_MAIL_SMTPSERVER_ADDSERVER','Add MailServer',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Server/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_MAIL_SMTPSERVER_EDITSERVER','Edit MailServer',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Template/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_MAIL_TEMPLATES','Mail Template',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Template/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_MAIL_TEMPLATES_DETAILTEMPLATE','Detail Mail Template',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Template/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_MAIL_TEMPLATES_ADDTEMPLATE','Add Mail Template',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Mail/Template/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_MAIL_TEMPLATES_EDITTEMPLATE','Edit Mail Template',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Form/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING_FORM','Provisioning Form',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Form/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING_FORM_DETAILFORM','Detail Provisioning Form',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Form/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING_FORM_ADDFORM','Add Provisioning Form',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Form/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING_FORM_EDITFORM','Edit Provisioning Form',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Roles/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING_ROLE','Provisioning Role',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Roles/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING_ROLE_DETAILROLE','Detail Provisioning Role',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Roles/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING_ROLE_ADDROLE','Add Provisioning Role',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Roles/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING_ROLE_EDITROLE','Edit Provisioning Role',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/RoleMatrix/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ROLEMATRIX','Role Matrix',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/RoleMatrix/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ROLEMATRIX_DETAIL_ROLEMATRIX','Detail RoleMatrix',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/RoleMatrix/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ROLEMATRIX_ADD_ROLEMATRIX','Add RoleMatrix',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/RoleMatrix/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ROLEMATRIX_EDIT_ROLEMATRIX','Edit RoleMatrix',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Request/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING_MYREQUESTS','My Request',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='//Prov/request/detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING_MYDETAILREQUESTS','My Request Detail',@UrlId )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING_MYTASK','My Task ',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/DetailType.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING_MYTASKDETAILTYPE' ,'My Task DetailType' ,@UrlId  )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_PROVISIONING_MYTASKDETAIL','My Task Detail',@UrlId )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/SnapShot/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ACCESSSNAPSHOT','Access SnapShot',@UrlId )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/ApplicationPolicy/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('PASSWORDMGMT_APPLICATIONPASSWORDPOLICY','Application Password Policy',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/ApplicationPolicy/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('PASSWORDMGMT_APPLICATIONPASSWORDPOLICY_DETAILAPPLICATIONPOLICY','Detail ApplicationPolicy',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/ApplicationPolicy/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('PASSWORDMGMT_APPLICATIONPASSWORDPOLICY_ADDAPPLICATIONPOLICY','Add ApplicationPolicy',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/ApplicationPolicy/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('PASSWORDMGMT_APPLICATIONPASSWORDPOLICY_EDITAPPLICATIONPOLICY','Edit ApplicationPolicy',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/GlobalQuestion/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('PASSWORDMGMT_GLOBALQUESTION','Global Questions',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/PrivateQuestion/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('PASSWORDMGMT_PRIVATEQUESTION','Security Questions',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/PasswordMgmt/PasswordSync/PasswordSync.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('PASSWORDMANAGEMENT_PASSWORDRESET','Password Synchronization',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Account/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('USER_ACCOUNT','User Account',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Account/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('USER_ACCOUNT_DETAILACCOUNT','Account Detail',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Account/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('USER_ACCOUNT_EDITACCOUNT','Edit UserAccount',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Employee/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('USER_EMPLOYEE','Employee',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Employee/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('USER_EMPLOYEE_DETAILEMPLOYEE','Detail Employee',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Employee/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('USER_EMPLOYEE_ADDEMPLOYEE','Add Employee',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Employee/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('USER_EMPLOYEE_EDITEMPLOYEE','Edit Employee',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/NonEmployee/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('USER_NONEMPLOYEE','NonEmployee',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/NonEmployee/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('USER_NONEMPLOYEE_DETAILNONEMPLOYEE','Detail NonEmployee',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/NonEmployee/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('USER_NONEMPLOYEE_ADDNONEMPLOYEE','Add NonEmployee',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/NonEmployee/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('USER_NONEMPLOYEE_EDITNONEMPLOYEE','Edit NonEmployee',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_INHOUSE_LIST','InHouse',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_INHOUSE_ADD','Add InHouse',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_INHOUSE_EDIT','Edit InHouse',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_INHOUSE_PERMISSION','InHouse Permission',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_INHOUSE_PERMISSION_ADD','Add InHouse Permission',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_INHOUSE_PERMISSION_EDIT','Edit InHouse Permission',@UrlId )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Preferences/UserPreference.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('PREFERENCES_USERPREFERENCE','User Preference',@UrlId )


--- Tab Menu Insertion

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (NULL, 'HOME','Home' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMINISTRATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (NULL,'ADMINISTRATION', 'Administration' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMANAGEMENT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (NULL,'PASSWORDMANAGEMENT', 'Password Management' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (NULL,'USER', 'User' , @PageId )
SELECT @PageId=Id FROM [DI_M_Page] WHERE [UniqueName]='ADMIN_MODE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId])VALUES (NULL,'ADMIN MODE','Admin Mode',@PageId)


--- Home Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='HOME'

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYTASK'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'HOME_MYTASK', 'My Task' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYPROFILE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'HOME_MYPROFILE', 'My Profile' ,@PageId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYDELEGATIONS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'HOME_MYDELEGATIONS', 'My Delegations' ,@PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYREQUESTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'HOME_MYREQUESTS', 'My Requests' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_ACCESSREQUESTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'HOME_ACCESSREQUESTS', 'Access Request' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_SPECIALACCESSREQUESTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'HOME_ACCESSREQUESTS', 'Special Access Request' , @PageId )

--- Administration Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMINISTRATION'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ENDPOINTSYSTEM'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_ENDPOINTSYSTEM', 'Endpoint System' ,@PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_ATTESTATION', 'Attestation' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_PROVISIONING', 'Provisioning' , @PageId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_WORKFLOW'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_WORKFLOW', 'Workflow' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GROUPS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_GROUPS', 'Group' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_ROLES', 'Role' ,@PageId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_LOG'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_LOG', 'Log' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_JOB'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_JOB', 'Job' , @PageId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_MAIL', 'Mail' , @PageId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INH_SYSTEM'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_INH_SYSTEM', 'Inhouse System' , @PageId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_APP_CONFIG'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_APP_CONFIG', 'Application Configuration' , @PageId)


--- Administration (Provisioning) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='ADMIN_PROVISIONING'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_FORM'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_PROVISIONING_FORM', 'Provisioning Form' ,@PageId  )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_ROLE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_PROVISIONING_ROLE', 'Provisioning Role' ,@PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLEMATRIX'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_ROLEMATRIX', 'Role Matrix' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_MYREQUESTS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_PROVISIONING_MYREQUESTS', 'Request Details' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_MYTASK'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_PROVISIONING__MYTASK', 'Task Details' , @PageId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRIGGERS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_TRIGGERS', 'Triggers' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ACCESSSNAPSHOT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_ACCESSSNAPSHOT', 'Access Snapshot' ,@PageId )

--- Administration (Log) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] ='ADMIN_LOG'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_LOG_SERVICELOG'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_LOG_SERVICELOG', 'Service Log' , @PageId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_LOG_UILOG'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_LOG_UILOG', 'UI Log' ,@PageId)


--- Administration (Job) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_JOB'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DISCOVERYJOB'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_DISCOVERYJOB', 'Discovery Job' , @PageId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATIONJOB'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_ATTESTATIONJOB', 'Attestation Job' , @PageId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPLIANCEJOB'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_COMPLIANCEJOB', 'Compliance Job' , @PageId)

--- Administration (Mail) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_MAIL'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_TEMPLATES'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_MAIL_TEMPLATES', 'Mail Template' ,@PageId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_VIEWMAILS'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_MAIL_VIEWMAILS', 'Mail' , @PageId)

--- Administration (Inhouse system) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_INH_SYSTEM'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_LIST'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_INHOUSE_LIST', 'Account' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_PERMISSION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_INHOUSE_PERMISSION', 'Permission' , @PageId )

--- Administration (Application Configuration) Sub Menu's
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_APP_CONFIG'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TENANT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_TENANT', 'Tenant' , @PageId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_APPSETTING'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_APPSETTING', 'AppSetting' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GENERALCONFIGURATION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_GENERALCONFIGURATION', 'General Configuration' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_SMTPSERVER'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_MAIL_SMTPSERVER', 'Mail Server' ,@PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRUSTEDSOURCE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_TRUSTEDSOURCE', 'Trusted Source' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_RECON'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_RECON','Reconciliation' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_HOUSEKEEPING'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'ADMIN_HOUSEKEEPING','House Keeping' , @PageId )



---  =========  ACL Scripts ================

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL','Access Control',@UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

--===========PageURL

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/Permission/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/Permission/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/Permission/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/Permission/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/URL/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/URL/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/URL/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/URL/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/Page/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/Page/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/Page/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/Page/Edit.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/Page/PagePermission.aspx' )


INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/TabMenu/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/TabMenu/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/TabMenu/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/ACL/TabMenu/Edit.aspx' )

--===========Page

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_PERMISSION','Permission',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_DETAILPERMISSION','Detail Permission',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_ADDPERMISSION','Add Permission',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_EDITPERMISSION','Edit Permission',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_URL','URL' ,@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_DETAILURL','Detail URL',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_ADDURL','Add URL',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_EDITURL','Edit URL',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_PAGE','Page',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_DETAILPAGE','Detail Page',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_ADDPAGE','Add Page',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_EDITPAGE','Edit Page',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/PagePermission.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_PAGEPERMISSION','Permission Page',@UrlId )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_MENU','Menu',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_DETAILMENU','Detail Menu',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_ADDMENU','Add Menu',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ACL_EDITMENU','Edit Menu',@UrlId )

--===========TabMenu

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL'
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_APP_CONFIG'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId]) VALUES ('ACL','Access Control' ,@TabParentMenuId,  @PageId )

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ACL'

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_PERMISSION'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId]) VALUES ('ACL_PERMISSION','Permission' ,@TabParentMenuId,  @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_URL'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId]) VALUES ('ACL_URL','URL' ,@TabParentMenuId, @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_PAGE'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId]) VALUES ('ACL_PAGE','Page' ,@TabParentMenuId, @PageId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_MENU'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId]) VALUES ('ACL_MENU','Menu' ,@TabParentMenuId, @PageId)


--=========== Page Permission

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_PERMISSION'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/Detail.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Permission/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_URL'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/Detail.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/URL/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_PAGE'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/Detail.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/Page/PagePermission.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 14, @UrlId)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_MENU'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/Detail.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACL/TabMenu/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_ADDPERMISSION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_EDITPERMISSION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_DETAILPERMISSION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_ADDURL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_EDITURL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_DETAILURL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_ADDPAGE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_EDITPAGE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_PAGEPERMISSION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_DETAILPAGE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_ADDMENU'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_EDITMENU'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACL_DETAILMENU'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)



-------- Admin Mode Item--------------------------
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName]='ADMIN_MODE'
SELECT @UrlId=Id FROM [DI_M_PageUrl] WHERE [Url] ='Admin/AdminItems/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId,PermissionID,UrlId)Values(@PageId,1,@UrlId)


--===========PageURL

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/AdminModeItem/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/AdminModeItem/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/AdminModeItem/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/AdminModeItem/Edit.aspx' )

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMIN_ITEMS','Admin Mode Items',@UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ITEMS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)



--===========Page

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMINITEM_LIST','Admin Mode Items',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMINITEM__DETAIL','Detail Admin Items',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMINITEM__ADD','Add Admin Items',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('ADMINITEM__EDIT','Edit Admin Items',@UrlId )

--===========TabMenu

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ITEMS'
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ACL'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId]) VALUES ('ADMIN_ITEMS','Admin Mode Items' ,@TabParentMenuId,  @PageId )

--=========== Page Permission

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ITEMS'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/Detail.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/AdminModeItem/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMINITEM__ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMINITEM__EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)



--------------- QuickLink

--===========PageURL

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/QuickLink/DefaultRoleQL/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/QuickLink/DefaultRoleQL/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/QuickLink/DefaultRoleQL/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/QuickLink/DefaultRoleQL/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/QuickLink/QL/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/QuickLink/QL/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/QuickLink/QL/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/QuickLink/QL/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/QuickLink/QLAccessRights/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/QuickLink/QLAccessRights/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/QuickLink/QLAccessRights/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/QuickLink/QLAccessRights/Edit.aspx' )

--===========Page

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('QUICKLINK','QuickLink(QL)',@UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QUICKLINK'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('QL_DROLE','Default Roles QL',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('QL_DROLE_DETAIL','Detail Default Role QL',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('QL_DROLE_ADD','Add Default Role QL',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('QL_DROLE_EDIT','Edit Default Role QL',@UrlId )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('QL','QuickLink',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('QL_DETAIL','Detail QuickLink',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('QL_ADD','Add QuickLink',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('QL_EDIT','Edit QuickLink',@UrlId )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('QL_ACCESSRIGHTS','QL AccessRights',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('QL_AR_DETAIL','AccessRights',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('QL_AR_ADD','Add QL AccessRights',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('QL_AR_EDIT','Edit QL AccessRights',@UrlId )

--===========TabMenu

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QUICKLINK'
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ACL'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId]) VALUES ('QUICKLINK','Quick Link' ,@TabParentMenuId,  @PageId )


--=========== Page Permission

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_DROLE'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/Detail.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/Detail.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QL/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/DefaultRoleQL/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 18, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 8, @UrlId)



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_ACCESSRIGHTS'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/Detail.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/QuickLink/QLAccessRights/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_DROLE_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_DROLE_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_AR_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='QL_AR_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


----- Dashboard
--===========PageURL

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/Dashboard/DefaultRoleDB/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/Dashboard/DefaultRoleDB/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/Dashboard/DefaultRoleDB/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/Dashboard/DefaultRoleDB/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/Dashboard/DB/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/Dashboard/DB/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/Dashboard/DB/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/Dashboard/DB/Edit.aspx' )

INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/Dashboard/DBAccessRights/List.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/Dashboard/DBAccessRights/Detail.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/Dashboard/DBAccessRights/Add.aspx' )
INSERT INTO [dbo].[DI_M_PageUrl] ( [Url] ) VALUES ('/AppCustomization/Dashboard/DBAccessRights/Edit.aspx' )

--===========Page

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DASHBOARD','Dashboard(DB)',@UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DASHBOARD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DB_DROLE','Default Roles DB',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DB_DROLE_DETAIL','Detail Default Role DB',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DB_DROLE_ADD','Add Default Role DB',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DB_DROLE_EDIT','Edit Default Role DB',@UrlId )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DB','Dashboard',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DB_DETAIL','Detail Dashboard',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DB_ADD','Add Dashboard',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DB_EDIT','Edit Dashboard',@UrlId )


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/List.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DB_ACCESSRIGHTS','DB AccessRights',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/Detail.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DB_AR_DETAIL','AccessRights',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/Add.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DB_AR_ADD','Add DB AccessRights',@UrlId )
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/Edit.aspx'
INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId]) VALUES ('DB_AR_EDIT','Edit DB AccessRights',@UrlId )

--===========TabMenu

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DASHBOARD'
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ACL'
INSERT INTO [dbo].[DI_M_TabMenu] ([UniqueName],[MenuName],[ParentTabMenuId], [PageId]) VALUES ('DASHBOARD','Dashboard' ,@TabParentMenuId,  @PageId )

--=========== Page Permission

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_DROLE'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/Detail.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/Detail.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DB/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DefaultRoleDB/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 18, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 8, @UrlId)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_ACCESSRIGHTS'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/Detail.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AppCustomization/Dashboard/DBAccessRights/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_DROLE_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_DROLE_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_AR_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DB_AR_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


----User Menu

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE MenuName ='User'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EMPLOYEE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'USER_EMPLOYEE', 'Employees' ,@PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_NONEMPLOYEE'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'USER_NONEMPLOYEE', 'Non-Employees' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_ACCOUNT'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'USER_ACCOUNT', 'Accounts' ,@PageId)

---Password Management

SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE MenuName ='Password Management'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMANAGEMENT_PASSWORDRESET'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'PASSWORDMANAGEMENT_PASSWORDRESET', 'Password Reset' ,@PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_PRIVATEQUESTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'PASSWORDMGMT_PRIVATEQUESTION', 'Security Questions' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_GLOBALQUESTION'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId]) VALUES (@TabParentMenuId,'PASSWORDMGMT_GLOBALQUESTION', 'Global Questions' , @PageId )
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_APPLICATIONPASSWORDPOLICY'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName], [PageId]) VALUES (@TabParentMenuId,'PASSWORDMGMT_APPLICATIONPASSWORDPOLICY', 'Application Password Policy' , @PageId )

