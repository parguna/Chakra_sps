/*
NAME		:	DBO.USP_HRMSFLINS_GETTECHNICALROLEFROM_BUSINESSROLE
AUTHOR		:	SIVANANDAN R
DESCRIPTION	:	USED TO FETCH TECHNICAL ROLE ASSOCIATED WITH BUSINESSROLE.MAINLY FOR ATTESTATION

EXAMPLE: 
	declare @ENDPOINTSYSTEMID bigint = 5
	DECLARE @AccountReconId bigint = 0
	declare @br varchar(50)='View Contracts'
	declare @company varchar(max)='SP,SPPA'
	declare @AccountName varchar(512)='Kana'
	EXEC DBO.USP_HRMSFLINS_GETTECHNICALROLEFROM_BUSINESSROLE @ENDPOINTSYSTEMID,@AccountReconId,@BR,@company,@AccountName
*/
ALTER PROC DBO.USP_HRMSFLINS_GETTECHNICALROLEFROM_BUSINESSROLE
(
@ENDPOINTSYSTEMID BIGINT,
@AccountReconId BIGINT,
@BUSINESSROLE VARCHAR(max),
@COMPANY VARCHAR(MAX),
@AccountName varchar(512)=null
)AS
BEGIN
	
	SET NOCOUNT ON;
	
	SET @BUSINESSROLE = TRIM(@BUSINESSROLE)
	SET @COMPANY = TRIM(@COMPANY)
	IF(ISNULL(@AccountName,'') != '')
	BEGIN
			SELECT @AccountReconId = ID FROM DI_M_AccountReconcilation WHERE EndpointSystemId = @ENDPOINTSYSTEMID AND AccountName = @AccountName
	END

	SELECT DISTINCT B.TechnicalRule, B.[Type], B.BusinessRole, E.Entitlement,E.EntitlementDescription,E.ValidToDate,E.LastAttestedOn 
	FROM 
	dbo.DI_M_EntitlementReconcilation E WITH(NOLOCK)
	INNER JOIN 
	(
		SELECT	MainSupp as TechnicalRule, 
				'Main' as [Type], 
				BusinessRole, 
				TechnicalRule as TechnicalRole 
		FROM	dbo.DI_SAP_HRFLINSRoleMatrix 
		WHERE	BusinessRole	=	@BUSINESSROLE AND 
				MainSupp IN('main','general','Supplement') AND (@COMPANY ='' OR Company='' OR Company IN(select item from dbo.Split(@company,',')))
		UNION
 
	SELECT 
	'Supplement',CASE WHEN S.costcenter!='' THEN 'CostCenter' WHEN S.PurchasingOrg !='' THEN 'PurchasingOrg' WHEN S.PurchasingGrp !='' THEN 'PurchasingGrp' WHEN S.MRPControl !='' THEN 'MRPControl'
	WHEN S.StorageLocation !='' THEN 'StorageLocation'ELSE '' End as [Type],
	COALESCE(NULLIF(S.costcenter,''),NULLIF(S.PurchasingOrg,''),NULLIF(S.PurchasingGrp,''),NULLIF(S.Plant,''),NULLIF(S.MRPControl,''),S.StorageLocation) as BusinessRole,Role as TechnicalRole 
	FROM dbo.DI_SAP_HRFLINSSuppRoleMatrix S  WITH(NOLOCK)
	INNER JOIN dbo.DI_SAP_HRFLINSRoleMatrix M WITH(NOLOCK) ON S.[Rule] = M.TechnicalRule WHERE M.BusinessRole=@BUSINESSROLE AND MainSupp ='supplement' AND (@COMPANY ='' OR M.Company=''  
	OR M.Company in(SELECT item FROM dbo.Split(@company,',')))
	)as B ON E.Entitlement = B.TechnicalRole
	WHERE  EndpointSystemId= @ENDPOINTSYSTEMID AND AccountReconcilationId = @AccountReconId AND E.IsDeleted = 0 AND E.Entitlement IS NOT NULL
	ORDER BY E.ENTITLEMENT

END