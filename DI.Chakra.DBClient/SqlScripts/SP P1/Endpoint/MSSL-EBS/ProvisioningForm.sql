
declare @tblMSSLEBS table(id int identity, endpointId int)
declare @min int = 1
declare @max int

Insert into @tblMSSLEBS
select Id from DI_M_EndpointSystem where DisplayName like '%MSSL%'
union
select Id from DI_M_EndpointSystem where DisplayName like '%EBS%'

select @max = count(1) from @tblMSSLEBS
declare @endpointId int
while (@min <= @max)
begin
select @endpointId = endpointId from @tblMSSLEBS where id = @min
print @endpointId
delete from [DI_M_NProvisioningField] where [EndPointSystemId] = @endpointId
insert into [DI_M_NProvisioningField] ([EndPointSystemTypeId], [EndPointSystemId], [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
select [EndPointSystemTypeId], @endpointId, [AccountType], [FieldName], [Description], [Value], [IsActive], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate] from DI_M_NProvisioningField where EndPointSystemId = 4
set @min += 1
end

