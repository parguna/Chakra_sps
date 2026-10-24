
/*
Name : VW_EBSISU_CRM_ROLES
AUTHOR: SIVANANDAN R
EXAMPLE: select * from dbo.VW_EBSISU_CRM_ROLES WHERE ENDPOINTSYSTEMID=11
*/

Declare @ep varchar(10)=(select  CAST(id as varchar) from DI_M_EndpointSystem where ApplicationType=9 and IsTrustedSource=0)
set @ep = @ep +','+(select CAST(id as varchar) from DI_M_EndpointSystem where ApplicationType=10 and IsTrustedSource=0)
declare @query varchar(max)='CREATE VIEW dbo.VW_EBSISU_CRM_ROLES
AS
SELECT Distinct  SAPR.Id as SAP_ROLEID,SAPR.EndpointSystemId, A.BusinessRole,SAPR.ROLE_CODE,SAPR.ROLE_DESC, SAPR.EndpointUserId, SAPR.ValidToDate,BusinessRole as Description
FROM
(SELECT RoleName as BusinessRole,[VALUE] AS Technicalrole from DI_SAP_EBS_XL_RolesMatrix cross apply string_split(TechnicalRole,'','') WHERE [VALUE]!='''') 
AS A INNER JOIN
(SELECT Distinct rinfo.Id, ruinfo.EndpointSystemId, ruinfo.SAPUserinfo_Id as EndpointUserId,rinfo.ROLE_CODE,rinfo.ROLE_DESC,Convert(date,ruinfo.Role_ToDate,111) as ValidToDate 
FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id = rinfo.Id AND ruinfo.IsDirectlyAttached = 1 
WHERE ruinfo.EndpointSystemId IN('+@ep+') AND rinfo.EndpointSystemId IN('+@ep+'))as SAPR on A.Technicalrole = SAPR.ROLE_CODE'


EXEC (@query)

