/*
Name : VW_HRMSFLINS
AUTHOR: SIVANANDAN R
EXAMPLE: select * from dbo.VW_HRMSFLINS WHERE ENDPOINTSYSTEMID=5
*/

Declare @ep VARCHAR(10)=(select CAST(ID as varchar) from DI_M_EndpointSystem where ApplicationType=3 and IsTrustedSource=0)
declare @query varchar(max)='CREATE VIEW dbo.VW_HRMSFLINS
AS
select Distinct SAPR.Id as SAP_ROLEID, SAPR.EndpointSystemId, A.BusinessRole,SAPR.ROLE_CODE,ROLE_DESC, SAPR.SAPUserinfo_Id as EndpointUserId, SAPR.ValidToDate as ValidToDate,BusinessRole as Description from 
(select distinct BusinessRole,[value] as TechnicalRole from DBO.DI_SAP_HRFLINSRoleMatrix CROSS APPLY STRING_SPLIT(TechnicalRule, '','')
where  MainSupp= ''main''  AND [value] !=''''
) as A
inner JOIN(
SELECT rinfo.Id, ruinfo.EndpointSystemId, rinfo.ROLE_CODE,rinfo.ROLE_DESC,Convert(date,ruinfo.Role_ToDate,111) as ValidToDate,ruinfo.SAPUserinfo_Id FROM SAPRoleInfoSAPUserinfo ruinfo 
join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id= rinfo.Id AND ruinfo.IsDirectlyAttached=1 
WHERE ruinfo.EndpointSystemId = '+@ep+' AND rinfo.EndpointSystemId = '+@ep+') as  SAPR on A.TechnicalRole = SAPR.ROLE_CODE'
exec (@query)



