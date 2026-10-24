
/*
Name : VW_SAPFLINSBI_ROLES
AUTHOR: SIVANANDAN R
EXAMPLE: select * from dbo.VW_SAPFLINSBI_ROLES WHERE Businessrole='Elearning - Learner (SP:Information Systems)'

*/

Declare @ep varchar(10)=(select  CAST(id as varchar) from DI_M_EndpointSystem where ApplicationType=4 and IsTrustedSource=0)
declare @query varchar(max)='CREATE VIEW dbo.VW_SAPFLINSBI_ROLES
AS
SELECT Distinct SAPR.Id as SAP_ROLEID, SAPR.EndpointSystemId, A.BusinessRole,SAPR.ROLE_CODE,ROLE_DESC, SAPR.EndpointUserId, SAPR.ValidToDate,BusinessRole as Description
FROM
(SELECT BusinessRole,[VALUE] AS Technicalrole from DI_SAP_BIRoleCategories cross apply string_split(TechnicalRole,'','') WHERE [VALUE]!='''') 
AS A INNER JOIN
(SELECT Distinct rinfo.Id,ruinfo.EndpointSystemId, ruinfo.SAPUserinfo_Id as EndpointUserId,rinfo.ROLE_CODE,rinfo.ROLE_DESC,Convert(date,ruinfo.Role_ToDate,111) as ValidToDate 
FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id = rinfo.Id AND ruinfo.IsDirectlyAttached = 1 
WHERE ruinfo.EndpointSystemId ='+@ep+' AND rinfo.EndpointSystemId ='+@ep+')as SAPR on A.Technicalrole = SAPR.ROLE_CODE'


EXEC (@query)

