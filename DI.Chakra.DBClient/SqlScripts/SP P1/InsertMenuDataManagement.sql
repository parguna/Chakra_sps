

------------------------ Delete Existing inhouse menu and move it to data management ------------------------------------------------------------------------------------
delete from DI_M_TabMenu where PageId in (select Id from DI_M_Page where PageUrlId IN (select Id from DI_M_PageUrl where Url = '/InHouse/List.aspx'))
delete from DI_M_AccessRights where PagePermissionId in (select Id from DI_M_PagePermission where pageid in (select Id from DI_M_Page where PageUrlId IN (select Id from DI_M_PageUrl where Url = '/InHouse/List.aspx')))
delete from DI_M_PagePermission where pageid in (select Id from DI_M_Page where PageUrlId IN (select Id from DI_M_PageUrl where Url = '/InHouse/List.aspx'))
delete from DI_M_Page where PageUrlId IN (select Id from DI_M_PageUrl where Url = '/InHouse/List.aspx')
------------------------------------------------------------------------------------------------------------------------------------------------------------------------



GO
CREATE FUNCTION Split (
      @InputString                  VARCHAR(8000),
      @Delimiter                    VARCHAR(50)
)

RETURNS @Items TABLE (
	  Id							int identity,
      Item                          VARCHAR(8000)
)

AS
BEGIN
      IF @Delimiter = ' '
      BEGIN
            SET @Delimiter = ','
            SET @InputString = REPLACE(@InputString, ' ', @Delimiter)
      END

      IF (@Delimiter IS NULL OR @Delimiter = '')
            SET @Delimiter = ','

      DECLARE @Item           VARCHAR(8000)
      DECLARE @ItemList       VARCHAR(8000)
      DECLARE @DelimIndex     INT

      SET @ItemList = @InputString
      SET @DelimIndex = CHARINDEX(@Delimiter, @ItemList, 0)
      WHILE (@DelimIndex != 0)
      BEGIN
            SET @Item = SUBSTRING(@ItemList, 0, @DelimIndex)
            INSERT INTO @Items VALUES (@Item)

            -- Set @ItemList = @ItemList minus one less item
            SET @ItemList = SUBSTRING(@ItemList, @DelimIndex+1, LEN(@ItemList)-@DelimIndex)
            SET @DelimIndex = CHARINDEX(@Delimiter, @ItemList, 0)
      END -- End WHILE

      IF @Item IS NOT NULL -- At least one delimiter was encountered in @InputString
      BEGIN
            SET @Item = @ItemList
            INSERT INTO @Items VALUES (@Item)
      END

      -- No delimiters were encountered in @InputString, so just return @InputString
      ELSE INSERT INTO @Items VALUES (@InputString)

      RETURN

END
GO


go
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE dbsp_save_menu_page_with_access
	-- Add the parameters for the stored procedure here
	@pageUrl nvarchar(200),             -- /DataManagement/SAPRoleBasedonOrgMgt/List.aspx
	@pageUniqueName nvarchar(200),      -- ADMIN_DATAMANAGEMENT_SAP_ROLE_ORG
	@pageName nvarchar(200),            -- SAP HRMS Organisation
	@levelUniqueNames nvarchar(max),    -- ADMINISTRATION,ADMIN_DATA,SAP,SAPHRMS
	@levelNames nvarchar(max),		    -- Administration,Data Manangement,SAP,SAP-HRMS
	@permissionIDs nvarchar(200)        -- 1,3,4,5,6,12,17
AS
BEGIN
	SET NOCOUNT ON;

	--set @levelUniqueNames = 'ADMINISTRATION,ADMIN_DATAMANAGEMENT,SAP,SAPHRMS'
	--set @levelNames = 'Administration,Data Manangement,SAP,SAP-HRMS'

	declare @splitter nvarchar(1) = ','
	if not exists (select 1 from DI_M_PageUrl where Url = @pageUrl)
	begin
		INSERT INTO [dbo].[DI_M_PageUrl] ( [Url], CreatedDate  ) VALUES ( @pageUrl, SYSDATETIME())
	end

	declare @pageURLId int
	select @pageURLId = Id from [DI_M_PageUrl] where Url = @pageUrl

	if not exists (select 1 from [DI_M_Page] where [UniqueName] = @pageUniqueName)
	begin
		INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES (@pageUniqueName, @pageName ,@pageURLId , SYSDATETIME())
	end

	declare @pageId int
	select @pageId = Id from [DI_M_Page] where [UniqueName] = @pageUniqueName

	declare @tblTabContent table(id int identity, uname nvarchar(200), name varchar(200), tabId int)

	insert into @tblTabContent(uname, name)
	select A.item, B.Item from dbo.split(@levelUniqueNames, @splitter) A inner join dbo.split(@levelNames, @splitter) B on A.id = B.Id

	DECLARE @RIDRole bigint 
	SELECT @RIDRole = Id from DI_M_Role where RoleName='Administrator' 

	declare @count int
	select @count = count(1) from @tblTabContent

	declare @i int =  2
	declare @mainTabParentId int
	declare @mainTabParentUniqueName nvarchar(200)
	
	declare @tempUniqueName nvarchar(200)
	declare @tempName nvarchar(200)
	--SELECT @mainTabParentId =Id FROM [DI_M_TabMenu] WHERE [UniqueName] =  (select uname from @tblTabContent where id = @i)

	declare @tempPageURLId int
	declare @tempPageId int
	while(@i <= @count)
	begin
		select @mainTabParentUniqueName = uname from @tblTabContent where id = @i - 1
		select @mainTabParentId = id from DI_M_TabMenu where UniqueName = @mainTabParentUniqueName

		select @tempName = name, @tempUniqueName = uname from @tblTabContent where id = @i
		select @tempPageURLId = id from DI_M_PageUrl where Url = '#'

		if not exists (select 1 from [DI_M_Page] where UniqueName = @tempUniqueName)
		begin
			INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId], CreatedDate) VALUES (@tempUniqueName, @tempName ,@tempPageURLId , SYSDATETIME())
		end

		select @tempPageId = id from [DI_M_Page] where UniqueName = @tempUniqueName

		if not exists (select 1 from [DI_M_TabMenu] where UniqueName = @tempUniqueName)
		begin
			INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@mainTabParentId,@tempUniqueName, @tempName , @tempPageId , SYSDATETIME())
		end

		if not exists (SELECT pageperm.Id FROM [DI_M_PagePermission] pageperm
		inner join DI_M_Page pag on pag.Id = pageperm.PageId
		where pag.uniquename = @tempUniqueName)
		begin
			INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate]) VALUES( @tempPageId , 1, @tempPageURLId,SYSDATETIME())
			INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
			select @RIDRole, Id, 'System' , SYSDATETIME(),'System' , SYSDATETIME() from [DI_M_PagePermission] where UrlId = @tempPageURLId and PageId = @tempPageId
		end

		if not exists (select 1 from [DI_M_PagePermission] where UrlId = @pageURLId)
		begin
			INSERT INTO [dbo].[DI_M_PagePermission]( PageId , PermissionId, UrlId,[CreatedDate])
			select @PageId, Item, @pageURLId, SYSDATETIME() from dbo.split(@permissionIDs, @splitter)

			INSERT INTO [dbo].[DI_M_AccessRights](RoleId , PagePermissionId, CreatedBy , CreatedDate , LastModifiedBy , LastModifiedDate )
			select @RIDRole, Id, 'System' , SYSDATETIME(),'System' , SYSDATETIME() from [DI_M_PagePermission] where UrlId = @pageURLId
		end

		set @i = @i + 1
	end
	
	declare @lastLevelParentId int
	select @lastLevelParentId = A.id from [DI_M_TabMenu] A inner join @tblTabContent B on A.UniqueName = B.uname

	if not exists (select 1 from [DI_M_TabMenu] where [ParentTabMenuId] = @lastLevelParentId and UniqueName = @pageUniqueName and PageId = @pageId)
	begin
		INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId], CreatedDate) VALUES (@lastLevelParentId, @pageUniqueName, @pageName, @pageId, SYSDATETIME())
	end
END
GO

-- Add menus here

-- AD
exec dbsp_save_menu_page_with_access '/DataManagement/ADProvisioningMatrix/List.aspx','ADMIN_DATA_ADMATRIX','AD Provisioning Matrix','ADMINISTRATION,ADMIN_DATA','Administration,Data Manangement','1,3,4,5,6,12,17'


-- SAP FLINS
exec dbsp_save_menu_page_with_access '/DataManagement/SAPFLINS/SAPBIRoleCategories/List.aspx','ADMIN_DATA_SAP_BI_ROLE_CAT','SAP HRFLINS BI RoleCategories','ADMINISTRATION,ADMIN_DATA,SAP,SAPHRMS','Administration,Data Manangement,SAP,SAP-HRMS','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPFLINS/SAPRoleBasedonOrgMgt/List.aspx','ADMIN_DATA_SAP_ROLE_ORG','SAP HRMS Organisation','ADMINISTRATION,ADMIN_DATA,SAP,SAPHRMS','Administration,Data Manangement,SAP,SAP-HRMS','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPFLINS/SAPRoleBasedOnEEGroup/List.aspx','ADMIN_DATA_SAP_ROLE_EE_GROUP','SAP HRMS EE Group','ADMINISTRATION,ADMIN_DATA,SAP,SAPHRMS','Administration,Data Manangement,SAP,SAP-HRMS','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPFLINS/SAPRoleBasedonPosition/List.aspx','ADMIN_DATA_SAP_ROLE_BY_POSITION','SAP HRMS Position','ADMINISTRATION,ADMIN_DATA,SAP,SAPHRMS','Administration,Data Manangement,SAP,SAP-HRMS','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPFLINS/SAPHRFLINSRoleMatrix/List.aspx','ADMIN_DATA_SAP_MAIN_ROLE_MATRIX','SAP FLINS Main Role Matrix','ADMINISTRATION,ADMIN_DATA,SAP,SAPHRMS','Administration,Data Manangement,SAP,SAP-HRMS','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPFLINS/SAPHRFLINSSuppRoleMatrix/List.aspx','ADMIN_DATA_SAP_SUPP_ROLE_MATRIX','SAP FLINS Supplementary Role Matrix','ADMINISTRATION,ADMIN_DATA,SAP,SAPHRMS','Administration,Data Manangement,SAP,SAP-HRMS','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPFLINS/SAPRoleMandatoryFileds/List.aspx','ADMIN_DATA_SAP_ROLE_MANDATORY_FIELDS','SAP FLINS Role Mandatory Fields','ADMINISTRATION,ADMIN_DATA,SAP,SAPHRMS','Administration,Data Manangement,SAP,SAP-HRMS','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPFLINS/SAPRoleEligibilityMatrix/List.aspx','ADMIN_DATA_SAP_FLINS_ROLE_MATRIX','SAP FLINS Eligibility Role Matrix','ADMINISTRATION,ADMIN_DATA,SAP,SAPHRMS','Administration,Data Manangement,SAP,SAP-HRMS','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPFLINS/SAPHRFLINSConflictedBusinessRoles/List.aspx','ADMIN_DATA_SAP_FLINS_CONFLICTED_ROLES','SAP HRFLINS Conflicted Roles','ADMINISTRATION,ADMIN_DATA,SAP,SAPHRMS','Administration,Data Manangement,SAP,SAP-HRMS','1,3,4,5,6,12,17'

-- SAP Others
exec dbsp_save_menu_page_with_access '/DataManagement/SAPOthers/SAP_PIP_SAP_PI_Role/List.aspx','ADMIN_DATA_SAP_PIP_SAP_PI_Role','SAP PI Role Matrix','ADMINISTRATION,ADMIN_DATA,SAP,SAPOTHERS','Administration,Data Manangement,SAP,SAP-Others','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPOthers/SAPPGRGRCRoleMatrix/List.aspx','ADMIN_DATA_SAP_PGRGRC_ROLE_MATRIX','SAP GRC Role Matrix','ADMINISTRATION,ADMIN_DATA,SAP,SAPOTHERS','Administration,Data Manangement,SAP,SAP-Others','1,3,4,5,6,12,17'

-- SAP MSSL
exec dbsp_save_menu_page_with_access '/DataManagement/SAPMSSL/SAPMSSLCriticalTCodesMatrix/List.aspx','ADMIN_DATA_SAP_MSSL_CRITICAL_TCODE_MATRIX','SAP MSSL Critical TCodes Matrix','ADMINISTRATION,ADMIN_DATA,SAP,SAPMSSL','Administration,Data Manangement,SAP,SAP-MSSL','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPMSSL/SAPMSSLPositionsMatrix/List.aspx','ADMIN_DATA_SAP_MSSL_POSITIONS_MATRIX','SAP MSSL Positions Matrix','ADMINISTRATION,ADMIN_DATA,SAP,SAPMSSL','Administration,Data Manangement,SAP,SAP-MSSL','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPMSSL/SAPMSSLRolesMatrix/List.aspx','ADMIN_DATA_SAP_MSSL_ROLES_MATRIX','SAP MSSL Roles Matrix','ADMINISTRATION,ADMIN_DATA,SAP,SAPMSSL','Administration,Data Manangement,SAP,SAP-MSSL','1,3,4,5,6,12,17'

-- SAP EBS
exec dbsp_save_menu_page_with_access '/DataManagement/SAPEBS/SAPEBSCriticalTCodesMatrix/List.aspx','ADMIN_DATA_SAP_EBS_CRITICAL_TCODE_MATRIX','SAP EBS Critical TCodes Matrix','ADMINISTRATION,ADMIN_DATA,SAP,SAPEBS','Administration,Data Manangement,SAP,SAP-EBS','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPEBS/SAPEBSPositionsMatrix/List.aspx','ADMIN_DATA_SAP_EBS_POSITIONS_MATRIX','SAP EBS Positions Matrix','ADMINISTRATION,ADMIN_DATA,SAP,SAPEBS','Administration,Data Manangement,SAP,SAP-EBS','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPEBS/SAPEBSRolesMatrix/List.aspx','ADMIN_DATA_SAP_EBS_ROLES_MATRIX','SAP EBS Roles Matrix','ADMINISTRATION,ADMIN_DATA,SAP,SAPEBS','Administration,Data Manangement,SAP,SAP-EBS','1,3,4,5,6,12,17'

-- SAP EP
exec dbsp_save_menu_page_with_access '/DataManagement/SAPEP/SAPEPRoleInfoXLUpload/List.aspx','ADMIN_DATA_SAP_EP_ROLE_INFO','SAP-EP Role Info','ADMINISTRATION,ADMIN_DATA,PORTALS_EP_CE_IP','Administration,Data Manangement,Portals(EP/CE/IP)','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPEP/SAPEPGroupInfoXLUpload/List.aspx','ADMIN_DATA_SAP_EP_GROUP_INFO','SAP-EP Group Info','ADMINISTRATION,ADMIN_DATA,PORTALS_EP_CE_IP','Administration,Data Manangement,Portals(EP/CE/IP)','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPEP/CERoleInfoXLUpload/List.aspx','ADMIN_DATA_SAP_CE_ROLE_INFO','CE Role Info','ADMINISTRATION,ADMIN_DATA,PORTALS_EP_CE_IP','Administration,Data Manangement,Portals(EP/CE/IP)','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPEP/SAPCEGroupInfoXLUpload/List.aspx','ADMIN_DATA_SAP_CE_GROUP_INFO','CE Group Info','ADMINISTRATION,ADMIN_DATA,PORTALS_EP_CE_IP','Administration,Data Manangement,Portals(EP/CE/IP)','1,3,4,5,6,12,17'

-- NPAC
exec dbsp_save_menu_page_with_access '/DataManagement/NPAC/NPACSectionMappingConfiguration/List.aspx','ADMIN_DATA_NPAC_SECTION_MAPPING','NPAC Section','ADMINISTRATION,ADMIN_DATA,NPAC','Administration,Data Manangement,NPAC','1,3,4,5,6,12,17'
--exec dbsp_save_menu_page_with_access '/DataManagement/NPAC/NPACDesignationMappingConfiguration/List.aspx','ADMIN_DATA_NPAC_DESIGNATION_MAPPING','NPAC Designation','ADMINISTRATION,ADMIN_DATA,NPAC','Administration,Data Manangement,NPAC','1,3,4,5,6,12,17'

-- SAP SRM
exec dbsp_save_menu_page_with_access '/DataManagement/SAPSRM/SAPSRMMainRoleMatrix/List.aspx','ADMIN_DATA_SAPSRM_MAIN_ROLE_MATRIX','SRM Main Role Matrix','ADMINISTRATION,ADMIN_DATA,SAPSRM','Administration,Data Manangement,SRM','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPSRM/SAPSRMEnablerRoleMatrix/List.aspx','ADMIN_DATA_SAPSRM_ENABLER_ROLE_MATRIX','SRM Enabler Role Matrix','ADMINISTRATION,ADMIN_DATA,SAPSRM','Administration,Data Manangement,SRM','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SAPSRM/SAPSRMBusinessRoleMatrix/List.aspx','ADMIN_DATA_SAPSRM_BUSINESS_ROLE_MAPPING','SRM Business Role Mapping','ADMINISTRATION,ADMIN_DATA,SAPSRM','Administration,Data Manangement,SRM','1,3,4,5,6,12,17'

-- GEMS
exec dbsp_save_menu_page_with_access '/DataManagement/GEMS/GemsRoleMatrix/List.aspx','ADMIN_DATA_GEMS_ROLE_MATRIX','GEMS','ADMINISTRATION,ADMIN_DATA','Administration,Data Manangement','1,3,4,5,6,12,17'

-- MISCELLANEOUS
exec dbsp_save_menu_page_with_access '/DataManagement/O365/Office365LicenseConfig/List.aspx','ADMIN_DATA_O365_CONFIG_MATRIX','O365 Licenses','ADMINISTRATION,ADMIN_DATA,MISCELLANEOUS','Administration,Data Manangement,Miscellaneous','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/MISC/ApprovalException/List.aspx','ADMIN_DATA_MISC_APPROVAL_EXCEPTION','Approver Exception','ADMINISTRATION,ADMIN_DATA,MISCELLANEOUS','Administration,Data Manangement,Miscellaneous','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/VPN/List.aspx','ADMIN_DATA_MISC_VPN_MATRIX','VPN Matrix','ADMINISTRATION,ADMIN_DATA,MISCELLANEOUS','Administration,Data Manangement,Miscellaneous','1,3,4,5,6,12,17'

-- SMS
exec dbsp_save_menu_page_with_access '/DataManagement/SMS/SMSProvisioningMatrix/List.aspx','ADMIN_DATA_SMS_PROV_MATRIX','OneCard Provisioning Matrix','ADMINISTRATION,ADMIN_DATA,ONECARD','Administration,Data Manangement,OneCard','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SMS/SMSRoleCatalogue/List.aspx','ADMIN_DATA_SMS_ROLE_CATELOGUE','OneCard Role Catalogue','ADMINISTRATION,ADMIN_DATA,ONECARD','Administration,Data Manangement,OneCard','1,3,4,5,6,12,17'

-- Triggers
exec dbsp_save_menu_page_with_access '/Triggers/List.aspx','ADMIN_TRIGGERS','Triggers','ADMINISTRATION,ADMIN_PROVISIONING','Administration,Identity Manager','1,2,3,4,5,12'

-- Inhouse systems
exec dbsp_save_menu_page_with_access '/InHouse/List.aspx','ADMIN_DATA_INHOUSE_USERACCOUNTS','Other Systems Accounts','ADMINISTRATION,ADMIN_DATA,INHOUSE_SYSTEMS','Administration,Data Manangement,Other Systems','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/InhouseSystems/Permissions/List.aspx','ADMIN_DATA_INHOUSE_SYSTEMS_PERMISSIONS','Other Systems Permissions','ADMINISTRATION,ADMIN_DATA,INHOUSE_SYSTEMS','Administration,Data Manangement,Other Systems','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/InhouseSystems/Functions/List.aspx','ADMIN_DATA_INHOUSE_SYSTEMS_FUNCTIONS','Other Systems Functions','ADMINISTRATION,ADMIN_DATA,INHOUSE_SYSTEMS','Administration,Data Manangement,Other Systems','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/InhouseSystems/PermissionFunction/List.aspx','ADMIN_DATA_INHOUSE_SYSTEMS_PERMISSION_FUNCTIONS','User Permission Functions Matrix','ADMINISTRATION,ADMIN_DATA,INHOUSE_SYSTEMS','Administration,Data Manangement,Other Systems','1,3,4,5,6,12,17'

-- SAP NOVA
exec dbsp_save_menu_page_with_access '/DataManagement/NOVA/NOVAUserRoleMatrix/List.aspx','ADMIN_DATA_NOVA_USER_ROLE_MATRIX','NOVA User Role Matrix','ADMINISTRATION,ADMIN_DATA,NOVA','Administration,Data Manangement,NOVA','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/NOVA/NOVAManagementRoleMatrix/List.aspx','ADMIN_DATA_NOVA_MANAGEMENT_ROLE_MATRIX','NOVA Management Role Matrix','ADMINISTRATION,ADMIN_DATA,NOVA','Administration,Data Manangement,NOVA','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/NOVA/NOVAExceptionalRoleMatrix/List.aspx','ADMIN_DATA_NOVA_EXCEPTIONAL_ROLE_MATRIX','NOVA Exceptional Role Matrix','ADMINISTRATION,ADMIN_DATA,NOVA','Administration,Data Manangement,NOVA','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/NOVA/NOVASoDExceptionsMatrix/List.aspx','ADMIN_DATA_NOVA_SOD_EXCEPTIONS_MATRIX','NOVA SOD Exceptions Matrix','ADMINISTRATION,ADMIN_DATA,NOVA','Administration,Data Manangement,NOVA','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/NOVA/NOVACompositeRoleMatrix/List.aspx','ADMIN_DATA_NOVA_COMPOSITE_ROLE_MATRIX','NOVA Composite Role Matrix','ADMINISTRATION,ADMIN_DATA,NOVA','Administration,Data Manangement,NOVA','1,3,4,5,6,12,17'

-- DataViewer
exec dbsp_save_menu_page_with_access '/DataManagement/DataViewer/List.aspx','ADMIN_DATA_VIEWER','Data Viewer','ADMINISTRATION,ADMIN_DATA','Administration,Data Manangement','12'

--Report
--exec dbsp_save_menu_page_with_access '','REPORT_SAP','SAP','REPORT,REPORT_ENDPOINT,REPORT_SAP','Reports,Data Endpoint System,SAP','1'

-- Admin UI Check

 exec  [dbo].[dbsp_save_menu_page_with_access] '/Prov/DepartmentChangeRequest.aspx','ADMIN_CHECK_UI','OU Transfer Requests','ADMINISTRATION,ADMIN_PROVISIONING','Administration,Identity Manager','1'

 --- Admin UI -----------------------------
 exec  [dbo].[dbsp_save_menu_page_with_access] '/Admin/AdminUI/List.aspx','ADMIN_UI','Admin UI','ADMINISTRATION,ADMIN_MAINCONFIGURATION','Administration,Configuration','1'

 
 exec  [dbo].[dbsp_save_menu_page_with_access] '/Prov/SubmitFailedRequests.aspx','SUBMIT_FAILED_REQUEST','Submit Failed Request','ADMINISTRATION,ADMIN_PROVISIONING','Administration,Identity Manager','1,28,39'


 
 exec  [dbo].[dbsp_save_menu_page_with_access] '/Prov/SubmitFailedAdhocRequest.aspx','SUBMIT_FAILED_AdHoc_REQUEST','Submit Failed AdHoc Request','ADMINISTRATION,ADMIN_PROVISIONING','Administration,Identity Manager','1,28,39'


 if not exists(select 1 from DI_M_Permission where Name = 'ManuallyResolved')
 begin
insert into DI_M_Permission values ('ManuallyResolved','/Images/action-buttons/manualProcess.png',null,null,'2017-08-30 15:39:05.507',null,null)
end



 exec  [dbo].[dbsp_save_menu_page_with_access] '/Prov/UserAccessRequest/UserAccessDetailMatrix.aspx','USER_ACCESS_DETAIL_MATRIX','User Access Detail','USER','Users','1,12'
exec dbsp_save_menu_page_with_access '/DataManagement/SkypeForBusiness/ProvisioningMatrix/List.aspx','ADMIN_DATA_SFBPROVISIONINGMATRIX','Provisioning Matrix','ADMINISTRATION,ADMIN_DATA,SFB','Administration,Data Manangement,Skype For Business','1,3,4,5,6,12'
exec dbsp_save_menu_page_with_access '/DataManagement/SkypeForBusiness/NumberRange/List.aspx','ADMIN_DATA_SFBNUMBERRANGE','Extension Range','ADMINISTRATION,ADMIN_DATA,SFB','Administration,Data Manangement,Skype For Business','1,3,4,5,6,12,17'
exec dbsp_save_menu_page_with_access '/DataManagement/SkypeForBusiness/NumberMatrix/List.aspx','ADMIN_DATA_SFBNUMBER','Extension Management','ADMINISTRATION,ADMIN_DATA,SFB','Administration,Data Manangement,Skype For Business','1,3,4,5,6,12'

-- Drop after the menus inserted 
drop procedure dbsp_save_menu_page_with_access
drop function Split

