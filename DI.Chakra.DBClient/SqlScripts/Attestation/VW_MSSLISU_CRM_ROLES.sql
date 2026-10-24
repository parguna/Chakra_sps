
/*
Name : VW_EBSISU_CRM_ROLES
AUTHOR: SIVANANDAN R
EXAMPLE: select * from dbo.VW_MSSLISU_CRM_ROLES WHERE ENDPOINTSYSTEMID=9 and Role_Code='SMTMSMA'
*/

Declare @ep varchar(10)=(select  CAST(id as varchar) from DI_M_EndpointSystem where ApplicationType=7 and IsTrustedSource=0)
set @ep = @ep +','+(select CAST(id as varchar) from DI_M_EndpointSystem where ApplicationType=8 and IsTrustedSource=0)
declare @query varchar(max)='CREATE VIEW dbo.VW_MSSLISU_CRM_ROLES
AS
SELECT Distinct  SAPR.Id as SAP_ROLEID,SAPR.EndpointSystemId, A.BusinessRole,SAPR.ROLE_CODE,SAPR.ROLE_DESC, SAPR.EndpointUserId, SAPR.ValidToDate,BusinessRole as Description
FROM
(SELECT RoleName as BusinessRole,[VALUE] AS Technicalrole from DI_SAP_MSSL_XL_RolesMatrix cross apply string_split(TechnicalRole,'','') WHERE [VALUE]!='''') 
AS A INNER JOIN
(SELECT Distinct rinfo.Id,ruinfo.EndpointSystemId, ruinfo.SAPUserinfo_Id as EndpointUserId,rinfo.ROLE_CODE,rinfo.ROLE_DESC,Convert(date,ruinfo.Role_ToDate,111) as ValidToDate 
FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id = rinfo.Id AND ruinfo.IsDirectlyAttached = 1 
WHERE ruinfo.EndpointSystemId IN('+@ep+') AND rinfo.EndpointSystemId IN('+@ep+'))as SAPR on A.Technicalrole = SAPR.ROLE_CODE'


EXEC (@query)

