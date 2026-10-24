DECLARE @EndPointSystemTypeId BIGINT
SELECT @EndPointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='Active Directory'
insert into DI_M_EndpointFunctionType([Description],EndPointSystemTypeId,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
values('Group',@EndPointSystemTypeId,'System',GETDATE(),'System',GETDATE())
SELECT @EndPointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='Lotus Notes'
insert into DI_M_EndpointFunctionType([Description],EndPointSystemTypeId,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
values('Group',@EndPointSystemTypeId,'System',GETDATE(),'System',GETDATE())
SELECT @EndPointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='SAP'
insert into DI_M_EndpointFunctionType([Description],EndPointSystemTypeId,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
values('Group',@EndPointSystemTypeId,'System',GETDATE(),'System',GETDATE())
SELECT @EndPointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='SAPEP'
insert into DI_M_EndpointFunctionType([Description],EndPointSystemTypeId,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
values('Role',@EndPointSystemTypeId,'System',GETDATE(),'System',GETDATE())
insert into DI_M_EndpointFunctionType([Description],EndPointSystemTypeId,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
values('Profiles',@EndPointSystemTypeId,'System',GETDATE(),'System',GETDATE())
insert into DI_M_EndpointFunctionType([Description],EndPointSystemTypeId,CreatedBy,CreatedDate,LastModifiedBy,LastModifiedDate)
values('TCodes',@EndPointSystemTypeId,'System',GETDATE(),'System',GETDATE())