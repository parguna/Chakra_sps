if not exists(select 1 from DI_M_IMConfigurationMaster where ConfigKey = 'NPACUnits')
begin
	insert into DI_M_IMConfigurationMaster(ConfigKey, ConfigValue,Description,CreatedBy, CreatedDate, IsUiDisplay) values ('NPACUnits','1,2,3,4,5','Zones of NPAC', 'System', GETDATE(), 1)
end