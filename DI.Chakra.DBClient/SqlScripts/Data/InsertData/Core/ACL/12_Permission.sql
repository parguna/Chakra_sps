DECLARE @PageId bigint
DECLARE @UrlId bigint
Declare @pagePer bigint
Declare @TabParentMenuId bigint

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Home/Home.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='Reports'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Report/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYTASK'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/DetailType.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 15, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYGROUPTASK'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/DetailType.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYTASKDETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 10, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 11, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYPROFILE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Profile/Profile.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYDELEGATIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AdvanceDelegation/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AdvanceDelegation/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AdvanceDelegation/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AdvanceDelegation/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYDELEGATIONS_ADDDELEGATIONS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYDELEGATIONS_EDITDELEGATIONS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_ACCESSREQUESTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/NACR/CreateCart.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYREQUESTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Request/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Request/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Request/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYREQUESTS_DETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, null,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 20, null,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYNOTIFICATIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Notification/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Notification/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Notification/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYNOTIFICATIONS_DETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 22, null,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 23, null,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMANAGEMENT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ACCOUNTRECOVERY'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMINISTRATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ENDPOINT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAINCONFIGURATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_AUDIT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ACCESSGOVERNANCE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_LOG'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_RECON'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_JOB'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INH_SYSTEM'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_APP_CONFIG'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Clone.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 16, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GROUPS_DETAILGROUP'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GROUPS_ADDGROUP'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GROUP_EDITGROUP'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GROUP_CLONEGROUP'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_APPSETTING'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/AppSetting/ApplicationSetting.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, null,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_APPTEXT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, null,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRUSTEDSOURCE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/TrustedSrcConfig/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATION_DETAIL'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/NReview.aspx'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_NREVIEWATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 33, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 30, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 28, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 29, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 31, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 32, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate])  VALUES( @PageId , 1, @UrlId, SYSDATETIME())


SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/List.aspx'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] = 'ADMIN_ATTESTATION_DETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DETAILATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ADDATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_EDITATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GENERALCONFIGURATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_IACMCONFIGURATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/IACM/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_IMCONFIGURATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/IM/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PASSWORDMANAGEMENTCONFIGURATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/PasswordManagement/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPLIANCECONFIGURATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/Compliance/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PIMCONFIGURATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/PIM/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DGMCONFIGURATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/DGM/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ENDPOINT'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ENDPOINTSYSTEM'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_SUBSYSTEM'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/SubSystemType/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())




SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ENTITLEMENT_MANAGEMENT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/EntitlementManagement/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12,@UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/EntitlementManagement/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/EntitlementManagement/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ENTITLEMENT_MANAGEMENT_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ENTITLEMENT_MANAGEMENT_DETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='CATALOG_MANAGEMENT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CatalogManagement/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CatalogManagement/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CatalogManagement/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CatalogManagement/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/CatalogManagement/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5,@UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12,@UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 17,@UrlId,SYSDATETIME())

SELECT @PageId=Id FROM [DI_M_Page] WHERE [UniqueName]='CATALOG_MANAGEMENT_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @PageId=Id FROM [DI_M_Page] WHERE [UniqueName]='CATALOG_MANAGEMENT_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @PageId=Id FROM [DI_M_Page] WHERE [UniqueName]='CATALOG_MANAGEMENT_DETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ENPOINT_ADDENDPOINT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ENDPOINT_EDITENDPOINT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ENPOINT_DETAILENDPOINT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DISCOVERYJOB'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Discovery/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Discovery/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Discovery/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Discovery/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 9, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Discovery/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DISCOVERYJOB_ADDDISCOVERY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DISCOVERYJOB_EDITDISCOVERY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DISCOVERYJOB_DETAILDISCOVERY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

--TODO : YASILA
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATIONJOB'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Attestation/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Attestation/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Attestation/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Attestation/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 9, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Attestation/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATIONJOB_ADDATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATIONJOB_EDITATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATIONJOB_DETAILATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ADHOCJOB'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/AdHocJob/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/AdHocJob/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/AdHocJob/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/AdHocJob/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 9, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/AdHocJob/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ADHOCJOB_ADDADHOC'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ADHOCJOB_EDITADHOC'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ADHOCJOB_DETAILADHOC'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
--END TODO : YASILA

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPLIANCEJOB'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Compliance/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Compliance/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Compliance/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Compliance/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 9, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Compliance/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPLIANCEJOB_ADDCOMPLIANCE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPLIANCEJOB_EDITCOMPLIANCE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPLIANCEJOB_DETAILCOMPLIANCE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='Exception'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Exception/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Exception/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Exception/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Exception/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='Add Exception'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='Edit Exception'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/ChangeAccessRights.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 8, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Clone.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 16, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES_CHANGEACCESSRIGHTS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES_DETAILROLES'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES_ADDROLES'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES_EDITROLES'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES_CLONEROLES'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_WORKFLOW'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Workflow/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Workflow/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Workflow/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Workflow/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Workflow/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_WORKFLOW_DETAILWORKFLOW'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_WORKFLOW_ADDWORKFLOW'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_WORKFLOW_EDITWORKFLOW'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_JOBALLOCATIONCENTER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/JobAllocationCenter/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/DetailType.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/JobAllocationCenter/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ALERTCENTER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AlertCenter/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 35, Null,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/AlertCenter/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, Null,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ALERTCENTER_DETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPANY'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/CompanyMaster/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/CompanyMaster/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/CompanyMaster/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/CompanyMaster/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/CompanyMaster/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPANY_DETAILCOMPANY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPANY_ADDCOMPANY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPANY_EDITCOMPANY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRIGGERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Triggers/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Triggers/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Triggers/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Triggers/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRIGGERS_DETAILTRIGGERS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRIGGERS_ADDTRIGGERS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRIGGERS_EDITTRIGGERS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_LOG_SERVICELOG'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Log/Log.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_LOG_UILOG'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Log/UI.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='AUDIT_CONFIGURATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Log/Configuration.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='AUDIT_HISTORY'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Log/History.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_RECON_EXCEPTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/Recon/ReconException.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_RECON_EXCEPTIONATTRIBUTE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/Recon/ExceptionAttribute.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_VIEWMAILS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Mail/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Mail/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Mail/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Mail/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_VIEWMAILS_EDITMAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_VIEWMAILS_DETAILMAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_SMTPSERVER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Server/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Server/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Server/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Server/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Server/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_SMTPSERVER_ADDSERVER'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_SMTPSERVER_EDITSERVER'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_SMTPSERVER_DETAILSERVER'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_TEMPLATES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Template/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Template/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Template/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Template/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Template/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_TEMPLATES_ADDTEMPLATE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_TEMPLATES_EDITTEMPLATE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_TEMPLATES_DETAILTEMPLATE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_HOUSEKEEPING'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/HouseKeeping/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGRULESET'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGRULESET_RULES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/Rules/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/Rules/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/Rules/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/Rules/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/Rules/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGRULESET_USERRULES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/Rules/UserRule.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGRULESET_ADDRULES'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGRULESET_EDITRULES'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGRULESET_DETAILRULES'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())


-----Default App
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_APPLICATIONDEFAULTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/DefaultApplication/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/DefaultApplication/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/DefaultApplication/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, null,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/DefaultApplication/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_APPLICATIONDEFAULTS_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_APPLICATIONDEFAULTS_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())


---Prov default
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGDEFAULTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/ProvisioningDefaults/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/ProvisioningDefaults/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/ProvisioningDefaults/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, null,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/ProvisioningDefaults/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 17, @UrlId, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGDEFAULTS_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGDEFAULTS_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGDEFAULTS_DETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

-- Techincal Roles
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_TECHNICALROLE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/TechnicalRole/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/TechnicalRole/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/TechnicalRole/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, null,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/TechnicalRole/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_TECHNICALROLE_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_TECHNICALROLE_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())


-- Catalog
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_CATALOG'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/Catalog/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/Catalog/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/Catalog/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, null,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/Catalog/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_CATALOG_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_CATALOG_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_CATALOG_DETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONINGRULESET_PROVISIONINGFIELDS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ProvRS/ProvisioningFields/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, NULL, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 3, NULL, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 4, NULL, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 5, NULL, SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ORGLIST'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/OrgList/OrgList.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, @UrlId, SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EMPLOYEE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Employee/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Employee/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Employee/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Employee/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Employee/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EMPLOYEE_DETAILEMPLOYEE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EMPLOYEE_ADDEMPLOYEE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EMPLOYEE_EDITEMPLOYEE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_NONEMPLOYEE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/NonEmployee/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/NonEmployee/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/NonEmployee/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/NonEmployee/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/NonEmployee/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 37, null,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_NONEMPLOYEE_DETAILNONEMPLOYEE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_NONEMPLOYEE_ADDNONEMPLOYEE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_NONEMPLOYEE_EDITNONEMPLOYEE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_MEMBER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Members/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Members/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Members/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_MEMBER_DETAILEMPLOYEE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_ACCOUNT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Account/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Account/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Account/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Account/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_ACCOUNT_EDITACCOUNT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_ACCOUNT_DETAILACCOUNT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EXCLUSION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='UserExclusion/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_EmailNotificationSetting'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='EmailNotificationSetting/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='STANDING_ORDER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/MockUpEndpoints/StandingOrder.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='RISK_ASSESSMENT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/MockUpEndpoints/RiskAssessment.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_APPLICATIONPASSWORDPOLICY'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/ApplicationPolicy/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/ApplicationPolicy/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/ApplicationPolicy/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/ApplicationPolicy/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/ApplicationPolicy/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_APPLICATIONPASSWORDPOLICY_DETAILAPPLICATIONPOLICY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_APPLICATIONPASSWORDPOLICY_ADDAPPLICATIONPOLICY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_APPLICATIONPASSWORDPOLICY_EDITAPPLICATIONPOLICY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_GLOBALQUESTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/GlobalQuestion/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMANAGEMENT_PASSWORDRESET'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/PasswordSync/PasswordSync.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_PRIVATEQUESTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/PrivateQuestion/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_MYREQUESTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Request/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/Request/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 19, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_MYDETAILREQUESTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/Request/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_MYTASK'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/DetailType.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 15, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_MYTASKDETAILTYPE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/DetailType.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_MYTASKDETAIL'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='Prov/Task/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 10, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 11, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ACCESSSNAPSHOT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/SnapShot/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROV_ENDPOINTPROCESS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/EndpointProcess/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROV_EVENTPROCESS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/ProvRS/EventProcess/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_LIST'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/InHouse/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/InHouse/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/InHouse/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/InHouse/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/InHouse/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 17, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, Null,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, Null,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_PERMISSION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/List.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/Detail.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/Add.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/Edit.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/List.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 17, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_PERMISSION_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_PERMISSION_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_FUNCTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Function/List.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Function/Detail.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Function/Add.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Function/Edit.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Function/List.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 17, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_FUNCTION_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_FUNCTION_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PREFERENCES_USERPREFERENCE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, Null,SYSDATETIME())

--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_NREVIEWATTESTATION'

--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/NReview.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, @UrlId, SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 1, @UrlId, SYSDATETIME())
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId, CreatedDate)  VALUES( @PageId , 20, @UrlId, SYSDATETIME()) -- Delete Permission


-- Report Menu

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_Report'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/RWS/Report/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/RWS/Report/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/RWS/Report/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/RWS/Report/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/RWS/Report/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_Report_DETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_Report_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_Report_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

-- Report Job Menu

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_REPORTJOB'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Report/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Report/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Report/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 3, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Report/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 5, NULL,SYSDATETIME())
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 9, NULL,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Report/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_REPORTJOB_ADDREPORT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_REPORTJOB_EDITREPORT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_REPORTJOB_DETAILREPORT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, NULL,SYSDATETIME())


------Report Result Menu

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ReportResult'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/RWS/ReportResult/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

------ AlertCenter

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ALERTCENTER'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 35, NULL,SYSDATETIME())

delete from DI_M_PagePermission where PermissionId=19 and PageId=@PageId
UPDATE DI_M_PagePermission SET UrlId = NULL WHERE PageId = @PageId AND PermissionId = 2


------- Password Letter -----
SELECT @TabParentMenuId =Id FROM [DI_M_TabMenu] WHERE UniqueName ='ADMIN_MAINCONFIGURATION'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PasswordLetter'
INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@TabParentMenuId,'ADMIN_PasswordLetter', 'Password Letter' , @PageId , SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PasswordLetter'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/RWS/PasswordLetter/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PasswordLetter'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/RWS/PasswordLetter/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 4, @UrlId,SYSDATETIME())


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PasswordLetter_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 6, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ENTITLEMENT_MANAGEMENT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/EntitlementManagement/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 17, @UrlId,SYSDATETIME())

DECLARE  @PermissionId BIGINT 
SELECT @PermissionId = Id FROM DI_M_Permission WHERE Name = 'RequestExtend'
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYREQUESTS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , @PermissionId, NULL,SYSDATETIME())

-- My Department Request

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYDEPARTMENTREQUESTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/DepartmentRequest/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/DepartmentRequest/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 2, @UrlId,SYSDATETIME())
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/DepartmentRequest/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 12, @UrlId,SYSDATETIME())
SELECT @PermissionId = Id FROM DI_M_Permission WHERE Name = 'RequestExtend'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , @PermissionId, NULL,SYSDATETIME())

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYDEPARTMENTREQUESTS_DETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @PageId , 1, null,SYSDATETIME())
