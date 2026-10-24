insert into DI_M_Page(UniqueName,Name,PageUrlId,CreatedDate) values('ATTESTATION_CAMPAIGN','Attestation Campaign',174,GETDATE())



declare @pageUrl nvarchar(200)='/Report/Attestation/AttestationCampaignReport.aspx'             -- /DataManagement/SAPRoleBasedonOrgMgt/List.aspx
declare @pageUniqueName nvarchar(200)='ATTESTATION_CAMPAIGN_REPORT'      -- ADMIN_DATAMANAGEMENT_SAP_ROLE_ORG
declare @pageName nvarchar(200)='Attestation Campaign Report'            -- SAP HRMS Organisation
declare @levelUniqueNames nvarchar(max)='REPORT,REPORT_ATTESTATION,ATTESTATION_CAMPAIGN'    -- ADMINISTRATION,ADMIN_DATA,SAP,SAPHRMS
declare @levelNames nvarchar(max)='Report,Attestation,Attestation Campaign'
declare @permissionIDs nvarchar(200) ='1'      


exec [dbo].[dbsp_save_menu_page_with_access] @pageUrl,@pageUniqueName,@pageName,@levelUniqueNames,@levelNames,@permissionIDs

declare @pageUrl nvarchar(200)='/Report/Attestation/AttestationCampaignStatus.aspx'             -- /DataManagement/SAPRoleBasedonOrgMgt/List.aspx
declare @pageUniqueName nvarchar(200)='ATTESTATION_CAMPAIGN_STATUS'      -- ADMIN_DATAMANAGEMENT_SAP_ROLE_ORG
declare @pageName nvarchar(200)='Attestation Campaign Status'            -- SAP HRMS Organisation
declare @levelUniqueNames nvarchar(max)='REPORT,REPORT_ATTESTATION,ATTESTATION_CAMPAIGN'    -- ADMINISTRATION,ADMIN_DATA,SAP,SAPHRMS
declare @levelNames nvarchar(max)='Report,Attestation,Attestation Campaign'
declare @permissionIDs nvarchar(200) ='1'      


select * from DI_M_PagePermission

exec [dbo].[dbsp_save_menu_page_with_access] @pageUrl,@pageUniqueName,@pageName,@levelUniqueNames,@levelNames,@permissionIDs

insert into DI_M_PagePermission(PageId,PermissionId,UrlId,CreatedDate) values(10625,1,172,GETDATE())
insert into DI_M_AccessRights(RoleId,PagePermissionId,CreatedDate) values(1,21557,GETDATE())


insert into DI_M_PagePermission(PageId,PermissionId,UrlId,CreatedDate) values(43,20,Null,GETDATE())
