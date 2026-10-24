IF NOT EXISTS (SELECT 1 FROM DI_M_IACMConfigurationMaster WHERE ConfigKey = 'InhouseSystemsToShowInTaskDetailForAD')
BEGIN
	insert into DI_M_IACMConfigurationMaster (ConfigKey, ConfigValue, IsUiDisplay, CreatedBy, CreatedDate) values
	('InhouseSystemsToShowInTaskDetailForAD', 'TMS,EAM,SRM,AMTS,OMS,PUPM',1,'System', getdate())
END

IF NOT EXISTS (SELECT 1 FROM DI_M_IMConfigurationMaster WHERE ConfigKey = 'RetryTimeSeconds')
BEGIN
	insert into DI_M_IMConfigurationMaster (ConfigKey, ConfigValue, IsUiDisplay, CreatedBy, CreatedDate) values
	('RetryTimeSeconds', '10',1,'System', getdate())
END

IF NOT EXISTS (SELECT 1 FROM DI_M_IMConfigurationMaster WHERE ConfigKey = 'HowManyTimesShouldRetry')
BEGIN
	insert into DI_M_IMConfigurationMaster (ConfigKey, ConfigValue, IsUiDisplay, CreatedBy, CreatedDate) values
	('HowManyTimesShouldRetry', '3',1,'System', getdate())
END

IF NOT EXISTS (SELECT 1 FROM DI_M_IMConfigurationMaster WHERE ConfigKey = 'DefaultManagerRole')
BEGIN
	insert into DI_M_IMConfigurationMaster (ConfigKey, ConfigValue, IsUiDisplay, CreatedBy, CreatedDate) values
	('DefaultManagerRole', '9',1,'System', getdate())
END

IF NOT EXISTS (SELECT 1 FROM DI_M_IMConfigurationMaster WHERE ConfigKey = 'EmailRegards')
BEGIN
	insert into DI_M_IMConfigurationMaster (ConfigKey, ConfigValue, IsUiDisplay, CreatedBy, CreatedDate) values
	('EmailRegards', 'IAM Administrator',1,'System', getdate())
END

IF NOT EXISTS (SELECT 1 FROM DI_M_IACMConfigurationMaster WHERE ConfigKey = 'InvalidApprovalTypeMatrixMessage')
BEGIN
	insert into DI_M_IACMConfigurationMaster (ConfigKey, ConfigValue, IsUiDisplay, CreatedBy, CreatedDate) values
	('InvalidApprovalTypeMatrixMessage', 'Please enter a valid Approver Type. For user enter 1 and for group enter 3.',1,'System', getdate())
END


------------------------------------------------------------End point configuration-------------------------------------------------------------------------

if not exists(select 1 from DI_M_IACMConfigurationMaster where ConfigKey = 'SAPGRC_ENTITLEMENT_INFO')
begin
	insert into DI_M_IACMConfigurationMaster(ConfigKey, ConfigValue, CreatedBy, IsUiDisplay, CreatedDate) 
	values ('SAPGRC_ENTITLEMENT_INFO', 'select BusinessRoleName, TechnicalRole from DI_SAP_GRCRole where BusinessRoleName IN (@@)', 'System', 0, GETDATE())
end

-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------