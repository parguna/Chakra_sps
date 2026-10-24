



DECLARE @PageId int
DECLARE @UrlId int
Declare @pagePer bigint
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Home/Home.aspx'
INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYTASK'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 15, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/DetailType.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYGROUPTASK'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Task/GroupTasks.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYTASKDETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 10, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 11, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYPROFILE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Profile/Profile.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYDELEGATIONS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AdvanceDelegation/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AdvanceDelegation/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/AdvanceDelegation/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYDELEGATIONS_ADDDELEGATIONS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYDELEGATIONS_EDITDELEGATIONS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_ACCESSREQUESTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/ACR/CreateCart.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_SPECIALACCESSREQUESTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/SpecialACR/CreateCart.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)	


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYREQUESTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Request/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Request/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='HOME_MYREQUESTS_DETAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, null)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 20, null)



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMANAGEMENT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMINISTRATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_LOG'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_JOB'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INH_SYSTEM'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_APP_CONFIG'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='#'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GROUPS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Group/Clone.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 16, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GROUPS_DETAILGROUP'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GROUPS_ADDGROUP'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GROUP_EDITGROUP'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GROUP_CLONEGROUP'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_APPSETTING'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/AppSetting/ApplicationSetting.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, null)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_APPTEXT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, null)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRUSTEDSOURCE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/TrustedSrcConfig/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_RECON'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Recon/ReconException.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Attestation/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DETAILATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ADDATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_EDITATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_GENERALCONFIGURATION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/ConfigKey/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ENDPOINTSYSTEM'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Endpoint/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ENPOINT_ADDENDPOINT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ENDPOINT_EDITENDPOINT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ENPOINT_DETAILENDPOINT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DISCOVERYJOB'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Discovery/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Discovery/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Discovery/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Discovery/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 9, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DISCOVERYJOB_ADDDISCOVERY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DISCOVERYJOB_EDITDISCOVERY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_DISCOVERYJOB_DETAILDISCOVERY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

--TODO : YASILA
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATIONJOB'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Attestation/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Attestation/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Attestation/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Attestation/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 9, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATIONJOB_ADDATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATIONJOB_EDITATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ATTESTATIONJOB_DETAILATTESTATION'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
--END TODO : YASILA

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPLIANCEJOB'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Compliance/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Compliance/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Compliance/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Job/Compliance/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 9, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPLIANCEJOB_ADDCOMPLIANCE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPLIANCEJOB_EDITCOMPLIANCE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_COMPLIANCEJOB_DETAILCOMPLIANCE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='Exception'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Exception/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Exception/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Exception/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Exception/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='Add Exception'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='Edit Exception'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/ChangeAccessRights.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 8, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Role/Clone.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 16, @UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES_CHANGEACCESSRIGHTS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES_DETAILROLES'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES_ADDROLES'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES_EDITROLES'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLES_CLONEROLES'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TENANT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Tenant/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Tenant/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Tenant/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Admin/Tenant/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TENANT_DETAILTENANT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TENANT_ADDTENANT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TENANT_EDITTENANT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_WORKFLOW'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Workflow/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Workflow/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Workflow/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Workflow/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_WORKFLOW_DETAILWORKFLOW'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_WORKFLOW_ADDWORKFLOW'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_WORKFLOW_EDITWORKFLOW'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRIGGERS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Triggers/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Triggers/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Triggers/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Triggers/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRIGGERS_DETAILTRIGGERS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRIGGERS_ADDTRIGGERS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_TRIGGERS_EDITTRIGGERS'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_LOG_SERVICELOG'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Log/Log.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_LOG_UILOG'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Log/UI.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_VIEWMAILS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Mail/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Mail/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Mail/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Mail/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_VIEWMAILS_ADDMAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_VIEWMAILS_EDITMAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_VIEWMAILS_DETAILMAIL'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_SMTPSERVER'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Server/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Server/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Server/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Server/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_SMTPSERVER_ADDSERVER'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_SMTPSERVER_EDITSERVER'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_SMTPSERVER_DETAILSERVER'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_TEMPLATES'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Template/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Template/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Template/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Mail/Template/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_TEMPLATES_ADDTEMPLATE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_TEMPLATES_EDITTEMPLATE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_MAIL_TEMPLATES_DETAILTEMPLATE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_HOUSEKEEPING'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Admin/HouseKeeping/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EMPLOYEE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Employee/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Employee/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Employee/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Employee/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EMPLOYEE_DETAILEMPLOYEE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EMPLOYEE_ADDEMPLOYEE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_EMPLOYEE_EDITEMPLOYEE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_NONEMPLOYEE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/NonEmployee/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/NonEmployee/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/NonEmployee/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/NonEmployee/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_NONEMPLOYEE_DETAILNONEMPLOYEE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_NONEMPLOYEE_ADDNONEMPLOYEE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_NONEMPLOYEE_EDITNONEMPLOYEE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_ACCOUNT'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Account/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Account/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Account/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_ACCOUNT_EDITACCOUNT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='USER_ACCOUNT_DETAILACCOUNT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_APPLICATIONPASSWORDPOLICY'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/ApplicationPolicy/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/ApplicationPolicy/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/ApplicationPolicy/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/ApplicationPolicy/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_APPLICATIONPASSWORDPOLICY_DETAILAPPLICATIONPOLICY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_APPLICATIONPASSWORDPOLICY_ADDAPPLICATIONPOLICY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_APPLICATIONPASSWORDPOLICY_EDITAPPLICATIONPOLICY'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_GLOBALQUESTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/GlobalQuestion/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMANAGEMENT_PASSWORDRESET'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/PasswordSync/PasswordSync.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PASSWORDMGMT_PRIVATEQUESTION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/PasswordMgmt/PrivateQuestion/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_FORM'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/Form/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/Form/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/Form/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/Form/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_FORM_DETAILFORM'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_FORM_ADDFORM'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_FORM_EDITFORM'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_ROLE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/Roles/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/Roles/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/Roles/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/Roles/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_ROLE_DETAILROLE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_ROLE_ADDROLE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_ROLE_EDITROLE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLEMATRIX'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/RoleMatrix/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/RoleMatrix/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/RoleMatrix/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/RoleMatrix/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLEMATRIX_DETAIL_ROLEMATRIX'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLEMATRIX_ADD_ROLEMATRIX'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ROLEMATRIX_EDIT_ROLEMATRIX'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)


SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_MYREQUESTS'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Request/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/Request/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 19, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_MYTASK'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 15, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_MYTASKDETAILTYPE'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Prov/Task/DetailType.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_PROVISIONING_MYTASKDETAIL'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='Prov/Task/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 10, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 11, NULL)



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_ACCESSSNAPSHOT'

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/Prov/SnapShot/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)


--SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='DATA_VPN'
--SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/Configuration/Data/Endpoint/VPN/List.aspx'
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
--INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_LIST'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/InHouse/List.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 12, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 17, NULL)

SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/InHouse/Edit.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/InHouse/Detail.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] = '/InHouse/Add.aspx'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, Null)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, Null)



SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_PERMISSION'
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/List.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 1, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/Detail.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 2, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/Add.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 3, @UrlId)
SELECT @UrlId =Id FROM [DI_M_PageUrl] WHERE [Url] ='/InHouse/Permission/Edit.aspx' 
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 4, @UrlId)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 5, NULL)
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 17, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_PERMISSION_ADD'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)
SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='ADMIN_INHOUSE_PERMISSION_EDIT'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, NULL)

SELECT @PageId =Id FROM [DI_M_Page] WHERE [UniqueName] ='PREFERENCES_USERPREFERENCE'
INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId) VALUES( @PageId , 6, Null)




