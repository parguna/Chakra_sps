/*
NAME	:	USP_SAPGRC_SAPFLINSBI_TECHNICALROLEWITHOUTBUSINESSROLE
AUTHOR	:	SIVANANDAN R
DATE	:	16-10-2020
EXAMPLE	:	

DECLARE @APPLICATIONTYPE INT= 4
DECLARE @USERNAME VARCHAR(128) ='KANA'
EXEC USP_SAPGRC_SAPFLINSBI_TECHNICALROLEWITHOUTBUSINESSROLE @APPLICATIONTYPE,@USERNAME

DECLARE @APPLICATIONTYPE INT= 6
DECLARE @USERNAME VARCHAR(128) ='KANCHANA'
EXEC USP_SAPGRC_SAPFLINSBI_TECHNICALROLEWITHOUTBUSINESSROLE @APPLICATIONTYPE,@USERNAME

*/
CREATE PROC DBO.USP_SAPGRC_SAPFLINSBI_TECHNICALROLEWITHOUTBUSINESSROLE
(
@APPLICATIONTYPE INT,
@USERNAME VARCHAR(128)
)
AS
BEGIN
	
IF (@APPLICATIONTYPE = 4)
	BEGIN
			SELECT Distinct SAPR.Id,SAPR.Id as SAP_ROLEID, SAPR.EndpointSystemId,SAPR.ROLE_CODE as BusinessRole,ROLE_DESC, SAPR.EndpointUserId,
			NULL AS Company,NULL AS Module,NULL AS SubModule, NULL AS Remarks,ROLE_DESC as Description, SAPR.ValidToDate
			FROM 
			(	SELECT BusinessRole,item AS Technicalrole 
				FROM DI_SAP_BIRoleCategories cross apply dbo.Split(TechnicalRole,',') 
				WHERE item!=''
			) AS A 
			RIGHT JOIN 
			( 
			  SELECT DISTINCT rinfo.Id,ruinfo.EndpointSystemId, ruinfo.SAPUserinfo_Id as EndpointUserId,rinfo.ROLE_CODE,rinfo.ROLE_DESC,
			  CASE WHEN ISDATE(ruinfo.Role_ToDate)=1 THEN Convert(date,ruinfo.Role_ToDate,111) ELSE NULL END as ValidToDate  
			  FROM SAPRoleInfoSAPUserinfo ruinfo JOIN DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id = rinfo.Id 
			  JOIN DI_M_EndpointSystem e ON ruinfo.EndpointSystemId = E.Id 
			  JOIN DI_SAP_UserInfo U ON ruinfo.SAPUserinfo_Id = U.Id 
			  WHERE E.ApplicationType=@APPLICATIONTYPE AND ruinfo.IsDirectlyAttached = 1 AND U.UserLogonName=@USERNAME
			) AS SAPR ON A.Technicalrole = SAPR.ROLE_CODE 
			WHERE A.BusinessRole is null
	END
	ELSE IF(@APPLICATIONTYPE = 6)
	BEGIN
			SELECT Distinct  SAPR.Id,SAPR.Id as SAP_ROLEID,SAPR.EndpointSystemId,SAPR.ROLE_CODE as BusinessRole,SAPR.ROLE_DESC, SAPR.EndpointUserId,
			NULL AS Company,NULL AS Module,NULL AS SubModule, NULL AS Remarks,ROLE_DESC as Description, SAPR.ValidToDate
			FROM
			(	SELECT BusinessRoleName as BusinessRole,item AS Technicalrole 
				FROM DI_SAP_GRCRole cross apply dbo.Split(TechnicalRole,',') 
				WHERE item!=''
			) 
			AS A RIGHT JOIN
			(
				SELECT Distinct rinfo.Id,ruinfo.EndpointSystemId, ruinfo.SAPUserinfo_Id as EndpointUserId,rinfo.ROLE_CODE,rinfo.ROLE_DESC,
				CASE WHEN ISDATE(ruinfo.Role_ToDate)=1 THEN Convert(date,ruinfo.Role_ToDate,111) ELSE NULL END as ValidToDate  
				FROM SAPRoleInfoSAPUserinfo ruinfo join DI_SAP_ROLEINFO rinfo ON ruinfo.SAPRoleInfo_Id = rinfo.Id 
				INNER JOIN DI_M_EndpointSystem E ON ruinfo.EndpointSystemId = E.Id	
				INNER JOIN DI_SAP_UserInfo U ON ruinfo.SAPUserinfo_Id = U.Id 
				WHERE  E.ApplicationType=@APPLICATIONTYPE AND ruinfo.IsDirectlyAttached = 1 AND U.UserLogonName=@USERNAME
			)AS SAPR ON A.Technicalrole = SAPR.ROLE_CODE
			WHERE A.BusinessRole IS NULL
	END
END
	

	