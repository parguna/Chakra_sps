
CREATE VIEW [dbo].[VW_EBSISU_CRM_ROLES]
AS
SELECT Distinct  SAPR.Id as SAP_ROLEID,SAPR.EndpointSystemId, A.BusinessRole,SAPR.ROLE_CODE,SAPR.ROLE_DESC, SAPR.EndpointUserId, SAPR.ValidToDate,BusinessRole as Description
FROM
(SELECT RoleName as BusinessRole,item AS Technicalrole from DI_SAP_EBS_XL_RolesMatrix cross apply dbo.Split(TechnicalRole,',') WHERE item!='') 
AS A INNER JOIN
(SELECT Distinct rinfo.Id, ruinfo.EndpointSystemId, ruinfo.SAPUserinfo_Id as EndpointUserId,rinfo.ROLE_CODE,rinfo.ROLE_DESC,CASE WHEN ISDATE(ruinfo.Role_ToDate)=1 THEN Convert(date,ruinfo.Role_ToDate,111) ELSE NULL End as ValidToDate
FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id = rinfo.Id AND ruinfo.IsDirectlyAttached = 1 
INNER JOIN DI_M_EndpointSystem E on ruinfo.EndpointSystemId = E.Id
WHERE E.ApplicationType IN(9,10))as SAPR on A.Technicalrole = SAPR.ROLE_CODE
GO

CREATE VIEW [dbo].[VW_HRMSFLINS]
AS
select Distinct SAPR.Id as SAP_ROLEID, SAPR.EndpointSystemId, A.BusinessRole,SAPR.ROLE_CODE,ROLE_DESC, SAPR.SAPUserinfo_Id as EndpointUserId, SAPR.ValidToDate as ValidToDate,BusinessRole as Description from 
(select distinct BusinessRole,item as TechnicalRole from DBO.DI_SAP_HRFLINSRoleMatrix CROSS APPLY dbo.Split(TechnicalRule, ',')
where  MainSupp= 'main'  AND item !=''
) as A
inner JOIN(
SELECT rinfo.Id, ruinfo.EndpointSystemId, rinfo.ROLE_CODE,rinfo.ROLE_DESC,CASE WHEN ISDATE(ruinfo.Role_ToDate)=1 THEN Convert(date,ruinfo.Role_ToDate,111) ELSE NULL End as ValidToDate,ruinfo.SAPUserinfo_Id FROM SAPRoleInfoSAPUserinfo ruinfo 
join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id= rinfo.Id AND ruinfo.IsDirectlyAttached=1 
INNER JOIN DI_M_EndpointSystem E on ruinfo.EndpointSystemId = E.Id
WHERE E.ApplicationType = 3) as  SAPR on A.TechnicalRole = SAPR.ROLE_CODE
GO


CREATE VIEW [dbo].[VW_MSSLISU_CRM_ROLES]
AS
SELECT Distinct  SAPR.Id as SAP_ROLEID,SAPR.EndpointSystemId, A.BusinessRole,SAPR.ROLE_CODE,SAPR.ROLE_DESC, SAPR.EndpointUserId, SAPR.ValidToDate,BusinessRole as Description
FROM
(SELECT RoleName as BusinessRole,item AS Technicalrole from DI_SAP_MSSL_XL_RolesMatrix cross apply dbo.Split(TechnicalRole,',') WHERE item!='') 
AS A INNER JOIN
(SELECT Distinct rinfo.Id,ruinfo.EndpointSystemId, ruinfo.SAPUserinfo_Id as EndpointUserId,rinfo.ROLE_CODE,rinfo.ROLE_DESC,
CASE WHEN ISDATE(ruinfo.Role_ToDate)=1 THEN Convert(date,ruinfo.Role_ToDate,111) ELSE NULL End as ValidToDate
FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id = rinfo.Id AND ruinfo.IsDirectlyAttached = 1 
INNER JOIN DI_M_EndpointSystem E on ruinfo.EndpointSystemId = E.Id
WHERE E.ApplicationType IN(7,8))as SAPR on A.Technicalrole = SAPR.ROLE_CODE
GO


CREATE VIEW [dbo].[VW_SAPFLINSBI_ROLES]
AS
SELECT Distinct SAPR.Id as SAP_ROLEID, SAPR.EndpointSystemId, A.BusinessRole,SAPR.ROLE_CODE,ROLE_DESC, SAPR.EndpointUserId, SAPR.ValidToDate,BusinessRole as Description
FROM
(SELECT BusinessRole,item AS Technicalrole from DI_SAP_BIRoleCategories cross apply dbo.Split(TechnicalRole,',') WHERE item!='') 
AS A INNER JOIN
(SELECT Distinct rinfo.Id,ruinfo.EndpointSystemId, ruinfo.SAPUserinfo_Id as EndpointUserId,rinfo.ROLE_CODE,rinfo.ROLE_DESC,Convert(date,ruinfo.Role_ToDate,111) as ValidToDate 
FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id = rinfo.Id AND ruinfo.IsDirectlyAttached = 1 
INNER JOIN DI_M_EndpointSystem E on ruinfo.EndpointSystemId = E.Id
WHERE E.ApplicationType=4)as SAPR on A.Technicalrole = SAPR.ROLE_CODE
GO


CREATE VIEW [dbo].[VW_SAPGRC_ROLES]
AS
SELECT Distinct  SAPR.Id as SAP_ROLEID,SAPR.EndpointSystemId, A.BusinessRole,SAPR.ROLE_CODE,SAPR.ROLE_DESC, SAPR.EndpointUserId, SAPR.ValidToDate,BusinessRole as Description
FROM
(SELECT BusinessRoleName as BusinessRole,item AS Technicalrole from DI_SAP_GRCRole cross apply dbo.Split(TechnicalRole,',') WHERE item!='') 
AS A INNER JOIN
(SELECT Distinct rinfo.Id,ruinfo.EndpointSystemId, ruinfo.SAPUserinfo_Id as EndpointUserId,rinfo.ROLE_CODE,rinfo.ROLE_DESC,Convert(date,ruinfo.Role_ToDate,111) as ValidToDate 
FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id = rinfo.Id AND ruinfo.IsDirectlyAttached = 1 
INNER JOIN DI_M_EndpointSystem E on ruinfo.EndpointSystemId = E.Id
WHERE E.ApplicationType=6 )as SAPR on A.Technicalrole = SAPR.ROLE_CODE
GO

