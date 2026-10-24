SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[VWHRMSFLIN_USER_MAIN_BUSINESSROLE]
AS
SELECT E.AccountReconcilationId, E.EndpointSystemId,E.EndpointUserId,E.AccountName, B.BusinessRole,B.Company,Max(E.ValidToDate) as ValidToDate
FROM DBO.DI_M_EntitlementReconcilation E WITH(NOLOCK) INNER JOIN 
(
SELECT DISTINCT BusinessRole,Company,item as TechnicalRole from DBO.DI_SAP_HRFLINSRoleMatrix WITH(NOLOCK) CROSS APPLY dbo.Split(TechnicalRule, ',')
WHERE   MainSupp ='main' --or MainSupp ='supplement'
)AS B on E.Entitlement = B.TechnicalRole
WHERE E.EndpointSystemId=5 AND E.IsDeleted = 0
GROUP BY E.AccountReconcilationId,E.EndpointSystemId,E.EndpointUserId,E.AccountName,B.BusinessRole,B.Company

GO


