/*
exec USP_EAM_TECHNICALROLE_FROMBUSINESSROLE 24,'PMFB996H','Y_FR_ALL_EAM_PS_ECP-EDV_EDIT'

SELECT * FROM DI_SAP_EamNovaUserRoleInfo E inner join DI_M_EntitlementReconcilation B on E.id = B.EntitlementId 

SELECT * FROM DI_M_AccountReconcilation WHERE EndpointSystemId=24 AND EndpointUserId=424
SELECT * FROM DI_M_EntitlementReconcilation WHERE AccountReconcilationId=87110
*/
CREATE PROC DBO.USP_EAM_TECHNICALROLE_FROMBUSINESSROLE
(
@ENDPOINTSYSTEMID BIGINT,
@AccountName varchar(512),
@BUSINESSROLE VARCHAR(2000)
)
AS
BEGIN

	SELECT	ER.[Id], [AccountReconcilationId], A.[EndpointType], A.[EndpointSystemId], [EndpointUserId], [UserId], [EntitlementId], [EntitlementType], [AccountName], [Entitlement], 
			[EntitlementDescription], [EnteredDate], [IsDeleted], [DeletedDate], [IsResolved], [ResolvedBy], [Comments], [ExceptionType], [ProvisionedThrough], [IsSODConflict], [IsSensitiveAccess], ER.[CreatedBy], 
			ER.[CreatedDate], ER.[LastModifiedBy], ER.[LastModifiedDate], [ResolvedStatus], A.[LastAttestedOn], [ValidFromDate], [ValidToDate], [AccessRequestId], [ExceptionOccuredDate], [ResolvedDate], 
			cast(E.ApplicationType as varchar(32)) AS [Functions] 
	FROM
	(
		SELECT	ER.EndpointSystemId,ER.EndpointType,er.AccountName as uName, ur.RoleName,Item as TecRole,ER.LastAttestedOn 
		FROM	DI_SAP_EamNovaUserRoleInfo UR inner join DI_M_EntitlementReconcilation ER on UR.Id = ER.EntitlementId
		CROSS APPLY dbo.Split(UR.FLINS_TCodes,',')
		WHERE ER.EndpointSystemId=@ENDPOINTSYSTEMID and er.IsDeleted=0 and ur.RoleName=@BUSINESSROLE AND ER.AccountName = @AccountName
	) AS A
	INNER JOIN DI_M_EntitlementReconcilation ER ON ER.Entitlement = A.TecRole and ER.AccountName = A.uName
	INNER JOIN DI_M_EndpointSystem E ON ER.EndpointSystemId = E.id
	WHERE E.ApplicationType=3 and IsTrustedSource=0  AND ER.IsDeleted=0


	UNION ALL
	SELECT	ER.[Id], [AccountReconcilationId], a.[EndpointType], a.[EndpointSystemId], [EndpointUserId], [UserId], [EntitlementId], [EntitlementType], [AccountName], [Entitlement], 
			[EntitlementDescription], [EnteredDate], [IsDeleted], [DeletedDate], [IsResolved], [ResolvedBy], [Comments], [ExceptionType], [ProvisionedThrough], [IsSODConflict], [IsSensitiveAccess], ER.[CreatedBy], 
			ER.[CreatedDate], ER.[LastModifiedBy], ER.[LastModifiedDate], [ResolvedStatus], A.[LastAttestedOn], [ValidFromDate], [ValidToDate], [AccessRequestId], [ExceptionOccuredDate], [ResolvedDate],
			cast(E.ApplicationType as varchar(32)) AS  [Functions] 
			FROM
		(
		SELECT DISTINCT ER.EndpointSystemId,ER.EndpointType,er.AccountName AS UName, ur.RoleName,Item AS TecRole,ER.LastAttestedOn 
		FROM DI_SAP_EamNovaUserRoleInfo UR INNER JOIN DI_M_EntitlementReconcilation ER ON UR.ID = ER.EntitlementId
		CROSS APPLY dbo.Split(UR.FLINS_BI_TCodes,',')
		WHERE ER.EndpointSystemId=@ENDPOINTSYSTEMID and er.IsDeleted=0 and Item !=''  and ur.RoleName=@BUSINESSROLE AND ER.AccountName = @AccountName
		) AS A
	INNER JOIN DI_M_EntitlementReconcilation ER ON ER.Entitlement = A.TecRole and ER.AccountName = A.UName
	INNER JOIN DI_M_EndpointSystem E ON ER.EndpointSystemId = E.id
	WHERE E.ApplicationType=4 and IsTrustedSource=0 AND ER.IsDeleted=0
	
	UNION ALL
	
	SELECT Distinct ER.[Id], [AccountReconcilationId], A.[EndpointType], A.[EndpointSystemId], [EndpointUserId], [UserId], [EntitlementId], [EntitlementType], ER.[AccountName], [Entitlement], [EntitlementDescription], 
	[EnteredDate], [IsDeleted], [DeletedDate], [IsResolved], [ResolvedBy], [Comments], [ExceptionType], [ProvisionedThrough], [IsSODConflict], [IsSensitiveAccess], ER.[CreatedBy], ER.[CreatedDate], 
	ER.[LastModifiedBy], ER.[LastModifiedDate], [ResolvedStatus], A.[LastAttestedOn], [ValidFromDate], [ValidToDate], [AccessRequestId], [ExceptionOccuredDate], 
	[ResolvedDate], cast(E.ApplicationType as varchar(32)) AS [Functions]
	FROM DI_M_EntitlementReconcilation ER INNER JOIN DI_M_EndpointSystem E ON ER.EndpointSystemId = E.id
	INNER JOIN (
		SELECT ER.EndpointSystemId,ER.EndpointType, er.AccountName, Item as TecRole,ER.LastAttestedOn 
		FROM DI_SAP_EamNovaUserRoleInfo UR 
		INNER JOIN DI_M_EntitlementReconcilation ER on UR.Id = ER.EntitlementId
		INNER JOIN DI_M_EndpointSystem E ON ER.EndpointSystemId = E.id
		CROSS APPLY dbo.Split(UR.AD_Groups,',')
		WHERE ER.EndpointSystemId=@ENDPOINTSYSTEMID and er.IsDeleted=0 and Item !='' AND ur.RoleName=@BUSINESSROLE AND ER.AccountName = @AccountName
	)AS A on ER.[Entitlement] = A.TecRole AND ER.AccountName = A.AccountName
	WHERE E.ApplicationType=1 and IsTrustedSource=0 AND ER.IsDeleted=0
END
