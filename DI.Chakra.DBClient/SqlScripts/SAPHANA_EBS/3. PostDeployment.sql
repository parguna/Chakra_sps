USE [V5_IDMS_SP]
GO

IF NOT EXISTS(SELECT 1 from [DI_M_NProvisioningField] WHERE [EndPointSystemTypeId] = 40)
begin
/*---CREATE PROVISIONING FIELD----*/
INSERT INTO [dbo].[DI_M_NProvisioningField]([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
SELECT B.EndpointType, B.EndpointId, [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], GETDATE(), [LastModifiedBy], GETDATE()  from DI_M_NProvisioningField AS A
CROSS JOIN (SELECT EndpointSystemTypeId as EndpointType,Id as EndpointId FROm DI_M_EndpointSystem WHERE EndpointSystemTypeId = 40) AS B
where EndPointSystemTypeId = 3 and EndPointSystemId = 10  order by AccountType,FieldName

END
GO

DECLARE @parentMenuTabId bigint
select @parentMenuTabId = Id from DI_M_TabMenu WHERE UniqueName='REPORT_ENDPOINT'

DECLARE @PageUrlId bigint
select @PageUrlId = Id from DI_M_PageUrl where URL='#'

DECLARE @PageId bigint
IF NOT EXISTS(SELECT 1 from DI_M_Page WHERE UniqueName='REPORT_SAPHANA')
begin
INSERT INTO DI_M_Page(UniqueName,Name,PageUrlId,CreatedDate,CreatedBy)
select 'REPORT_SAPHANA','SAP HANA',@PageUrlId,GETDATE(),'SYSTEM' from DI_M_Page where UniqueName ='REPORT_SAP'
SET @PageId = SCOPE_IDENTITY()
END

IF NOT EXISTS(SELECT 1 from DI_M_TabMenu WHERE UniqueName='REPORT_SAPHANA')
begin
INSERT INTO DI_M_TabMenu(UniqueName,MenuName,ParentTabMenuId,PageId,CreatedDate,CreatedBy)
VALUES('REPORT_SAPHANA','SAP HANA',@parentMenuTabId,@PageId,GETDATE(),'SYSTEM')
END

DECLARE @pPageId bigint
DECLARE @pPageUrlId bigint
SELECT @pPageId = ID,@pPageUrlId = PageUrlId from DI_M_Page WHERE UniqueName='REPORT_SAPHANA'
IF NOT EXISTS(SELECT 1 FROM  DI_M_PagePermission WHERE  PageId =@pPageId  AND  PermissionId=1 AND UrlId = @pPageUrlId )
BEGIN

	INSERT INTO DI_M_PagePermission(PageId,PermissionId,UrlId,CreatedBy,CreatedDate)
	VALUES(@pPageId,1,@pPageUrlId,'System',GETDATE())

END


IF NOT EXISTS(SELECT 1 from DI_M_IMConfigurationMaster WHERE ConfigKey='EBSSAPHANAPhoneTaskId')
begin
INSERT INTO DI_M_IMConfigurationMaster(ConfigKey,ConfigValue,CreatedDate,CreatedBy,IsUiDisplay,[Description])
values('EBSSAPHANAPhoneTaskId','TS92007927',GETDATE(),'System',1,'EBSSAPHANAPhoneTaskId') 
End

IF NOT EXISTS(SELECT 1 from DI_M_IMConfigurationMaster WHERE ConfigKey='MSSLSAPHANAPhoneTaskId')
begin
INSERT INTO DI_M_IMConfigurationMaster(ConfigKey,ConfigValue,CreatedDate,CreatedBy,IsUiDisplay,[Description])
values('MSSLSAPHANAPhoneTaskId','TS92000001',GETDATE(),'System',1,'MSSLSAPHANAPhoneTaskId') 
End