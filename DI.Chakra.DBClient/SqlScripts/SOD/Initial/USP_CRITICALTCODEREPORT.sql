USE [V5_IDMS_SP]
GO
/*
NAME		:	USP_CRITICALTCODEREPORT
DESC		:	GENERATE THE CRITICAL TCODE REPORT BASED ON USERNAME;	
EXEC USP_CRITICALTCODEREPORT 10
*/
ALTER proc [dbo].[USP_CRITICALTCODEREPORT]
(
@ENDPOINTID BIGINT,
@USERNAME NVARCHAR(MAX)=''
)
AS
BEGIN

	SET NOCOUNT ON;
	DECLARE @ACCOUNTRECONID BIGINT
	DECLARE @ENDPOINT_USERID BIGINT
	DECLARE @DISPLAYNAME nVARCHAR(MAX)=''
	DECLARE @ACCOUNTNAME nVARCHAR(512)=''
	DECLARE @TECHNICALROLE nVARCHAR(MAX)=''
	DECLARE @DESCRIPTION nVARCHAR(MAX)=''
	DECLARE @POSITION nVARCHAR(MAX)=''
	DECLARE @ACCOUNTTYPE TINYINT
	DECLARE @ENDPOINT_NAME nVARCHAR(1024)=''
	DECLARE @USER_NAME nVARCHAR(MAX)=''
	DECLARE @USERDISPLAYNAME	nVARCHAR(MAX)=''
	DECLARE @AccountOwner BIGINT=0
	DECLARE @MODULE VARCHAR(52)=''
	DECLARE @APPTYPE TINYINT

	DECLARE @CURRENTDATE DATETIME=GETDATE()
	SELECT @APPTYPE= ApplicationType,@ENDPOINT_NAME = DisplayName FROM DI_M_EndpointSystem WHERE ID = @ENDPOINTID 

	IF @APPTYPE = 10
	BEGIN
			SET @MODULE = 'CRP (EBS-CRM)'
	END
	ELSE IF @APPTYPE = 9
	BEGIN
			SET @MODULE = 'ISP (EBS - ISU)'
	END
	ELSE IF @APPTYPE = 8
	BEGIN
			SET @MODULE = 'MCP (MSSL - CRM)'
	END
	ELSE IF @APPTYPE = 7
	BEGIN
			SET @MODULE = 'MSP (MSSL - ISU)'
	END

	IF (LEN(ISNULL(@MODULE,''))<= 0)
	BEGIN
			RETURN;
	END

	DECLARE @TOTALACCOUNT BIGINT=0

	CREATE TABLE #TEMPACCOUNTS(ID BIGINT IDENTITY(1,1),DISPLAYNAME NVARCHAR(MAX) NULL,ACCOUNTNAME NVARCHAR(512) NOT NULL,ACCOUNTRECONID BIGINT NOT NULL,ENDPOINTUSERID BIGINT,ACCOUNTTYPE TINYINT, [UserName] [nvarchar](max) null,
	[UserDisplayName] [nvarchar](max) null,USERID BIGINT NULL)

	IF LEN(ISNULL(@USERNAME,''))> 0
	BEGIN
			INSERT INTO #TEMPACCOUNTS(ACCOUNTRECONID,DISPLAYNAME,ACCOUNTNAME,ENDPOINTUSERID,ACCOUNTTYPE,[UserName],[UserDisplayName],USERID)
			SELECT A.ID,FULLNAME ,ACCOUNTNAME,EndpointUserId,AccountType,B.USERNAME,B.DisplayName,A.UserId  FROM DI_M_AccountReconcilation A LEFT JOIN DI_M_User B on A.UserId = B.ID WHERE A.EndpointSystemId = @ENDPOINTID AND IsDeleted = 0 AND IsLocked = 0 AND AccountName=@USERNAME
			AND A.ID IN(SELECT AccountReconcilationId FROM DI_M_EntitlementReconcilation WHERE EndpointSystemId = @ENDPOINTID AND IsDeleted = 0 AND EntitlementType IN(1,5))
			AND EndpointUserId IN(SELECT ID FROM DI_SAP_UserInfo WHERE EndPointSystemId  = @ENDPOINTID AND UserLogonName=@USERNAME)
			SET @TOTALACCOUNT = @@ROWCOUNT
			
			DELETE FROM DI_SAP_CriticalTcodeReport WHERE EndPointSystemId = @ENDPOINTID AND AccountName=@USERNAME
	END
	ELSE
	BEGIN
			INSERT INTO #TEMPACCOUNTS(ACCOUNTRECONID,DISPLAYNAME,ACCOUNTNAME,ENDPOINTUSERID,ACCOUNTTYPE,[UserName],[UserDisplayName],USERID)
			SELECT A.ID,FULLNAME ,ACCOUNTNAME,EndpointUserId,AccountType,B.USERNAME,B.DisplayName,A.UserId FROM DI_M_AccountReconcilation  A LEFT JOIN DI_M_User B on A.UserId = B.ID WHERE A.EndpointSystemId = @ENDPOINTID AND IsDeleted = 0 AND IsLocked = 0
			AND A.ID IN(SELECT AccountReconcilationId FROM DI_M_EntitlementReconcilation WHERE EndpointSystemId = @ENDPOINTID AND IsDeleted = 0 AND EntitlementType IN(1,5))
			AND EndpointUserId IN(SELECT ID FROM DI_SAP_UserInfo WHERE EndPointSystemId  = @ENDPOINTID)
			SET @TOTALACCOUNT = @@ROWCOUNT
			
			DELETE FROM DI_SAP_CriticalTcodeReport WHERE EndPointSystemId = @ENDPOINTID
	END
	
	--SELECT *FROM #TEMPACCOUNTS

	IF @TOTALACCOUNT > 0
	BEGIN
			DECLARE @INCR BIGINT=1
			WHILE @TOTALACCOUNT >= @INCR
			BEGIN
				
					 SET @AccountOwner = 0
					 SET @ENDPOINT_USERID = 0
					 SET @ACCOUNTRECONID = 0
					 SET @DISPLAYNAME =''
					 SET @TECHNICALROLE =''
					 SET @DESCRIPTION =''
					 SET @POSITION =''
					 SET @ACCOUNTTYPE =0
					 SET @ACCOUNTNAME = ''
					 SET @USER_NAME		= ''
					 SET @USERDISPLAYNAME = ''

					 SELECT @ACCOUNTRECONID		= ACCOUNTRECONID,
							@DISPLAYNAME		= DISPLAYNAME,
							@ACCOUNTNAME		= ACCOUNTNAME,
							@ENDPOINT_USERID	= ENDPOINTUSERID,
							@ACCOUNTTYPE		= ACCOUNTTYPE,
							@USERDISPLAYNAME	= USERDISPLAYNAME,
							@USER_NAME			= USERNAME,
							@AccountOwner		= USERID	
					 FROM	#TEMPACCOUNTS 
					 WHERE	ID = @INCR

					 
					 DECLARE @TEMP_ROLES TABLE(Position NVARCHAR(MAX) NULL,TECHNICAL_ROLE NVARCHAR(MAX),FromValue NVARCHAR(MAX),ToValue NVARCHAR(MAX), [DESCRIPTION] NVARCHAR(MAX))

					--FROM TECHNICALROLE---
					INSERT INTO @TEMP_ROLES(Position,TECHNICAL_ROLE,FromValue,ToValue,[DESCRIPTION])
					SELECT NULL AS Position, Entitlement,FromValue,LTRIM(RTRIM(ISNULL(ToValue,''))) as ToValue,R.ROLE_DESC  FROM DI_M_EntitlementReconcilation E INNER JOIN 
					(
						SELECT RoleName,FromValue,ToValue FROM DI_SAP_RoleToAuthMap WHERE EndPointSystemId = @ENDPOINTID AND ObjectCode = 'S_TCODE' AND FieldCode = 'TCD' AND ProfileGeneratorVariant != 'X'

					)AS SR ON E.Entitlement = SR.RoleName 
					INNER JOIN DI_SAP_ROLEINFO R ON E.Entitlement = R.ROLE_CODE
					WHERE AccountReconcilationId = @ACCOUNTRECONID AND IsDeleted = 0 AND EntitlementType = 1 AND R.EndpointSystemId = @ENDPOINTID

					

					DECLARE @ROLES TABLE(POSITION NVARCHAR(1024),ROLE NVARCHAR(MAX) NULL,DESCR NVARCHAR(MAX) NULL)
					--FROM POSITION---
					DECLARE @POSCOUNT INT = 0
					IF OBJECT_ID('tempdb..#TBLPOST') IS NOT NULL
					BEGIN
							DROP TABLE #TBLPOST
					END

					SELECT IDENTITY(INT,1,1) AS ID, Entitlement  INTO #TBLPOST FROM DI_M_EntitlementReconcilation WHERE AccountReconcilationId = @ACCOUNTRECONID AND IsDeleted = 0 AND EntitlementType = 5
					SET @POSCOUNT = @@ROWCOUNT

					DECLARE @POS NVARCHAR(100)=''
					SET @POS = ''

					WHILE @POSCOUNT > 0
					BEGIN
							SELECT @POS = Entitlement FROM #TBLPOST WHERE ID = @POSCOUNT
							--PRINT @ACCOUNTNAME
							 IF(@APPTYPE = 7 or @APPTYPE = 8)--MSSL--
							 BEGIN
									IF(@APPTYPE=7)
									BEGIN
											SELECT @POSITION = PositionText FROM DI_SAP_MSSL_XL_PositionsMatrix WHERE Position_MSP = @POS
									END
									ELSE IF(@APPTYPE=8)
									BEGIN
											SELECT @POSITION = PositionText FROM DI_SAP_MSSL_XL_PositionsMatrix WHERE Position_MCP = @POS
									END
							
							 END
							 ELSE IF(@APPTYPE = 9 or @APPTYPE = 10)--EBS--
							 BEGIN
									IF(@APPTYPE=9)
									BEGIN
											SELECT @POSITION = PositionText FROM DI_SAP_EBS_XL_PositionsMatrix WHERE Position_ISP = @POS
									END
									ELSE IF(@APPTYPE=10)
									BEGIN
											SELECT @POSITION = PositionText FROM DI_SAP_EBS_XL_PositionsMatrix WHERE Position_CRP = @POS
									END
							 END
							---getrolesby position
							INSERT INTO @ROLES(POSITION,ROLE,DESCR)
							SELECT ISNULL(@POSITION,@POS),RelatedObjectCode,ROLE_DESC FROM (SELECT RelatedObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId = @ENDPOINTID AND ActualObjectCode = @POS AND RelatedObjectType='AG' AND ActualObjectType='S')AS A
							INNER JOIN DI_SAP_ROLEINFO R ON A.RelatedObjectCode = R.ROLE_CODE WHERE EndPointSystemId = @ENDPOINTID
							--GetOrgByPosition--
							DECLARE @ORGUNIT NVARCHAR(512)=''
							SET @ORGUNIT  = (SELECT  TOP 1 RelatedObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId=@ENDPOINTID AND ActualObjectCode = @POS AND RelatedObjectType='O' AND ActualObjectType='S')

							IF LEN(ISNULL(@ORGUNIT,'')) > 0
							BEGIN
							
									--getrolesbyorg--
									INSERT INTO @ROLES(POSITION,ROLE,DESCR)
									SELECT ISNULL(@POSITION,@POS),RelatedObjectCode,ROLE_DESC FROM
									(SELECT RelatedObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId=@ENDPOINTID AND RelatedObjectType='AG' AND ActualObjectType='O' AND ActualObjectCode = @ORGUNIT
									) AS A INNER JOIN DI_SAP_ROLEINFO R ON A.RelatedObjectCode = R.ROLE_CODE WHERE EndPointSystemId = @ENDPOINTID
									--GETPARENTOTGBYORG
									SET @ORGUNIT  = (SELECT  TOP 1 ActualObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId=@ENDPOINTID AND RelatedObjectCode = @ORGUNIT AND RelatedObjectType='O' AND ActualObjectType='O')

							END

							DECLARE @INCRE INT = 0
							DECLARE @ISPARENTEXISTS BIT = 1
							WHILE(@ISPARENTEXISTS = 1 AND LEN(ISNULL(@ORGUNIT,'')) > 0)
							BEGIN
									SET @INCRE = @INCRE + 1
									IF(@INCRE <= 5 AND LEN(ISNULL(@ORGUNIT,'')) > 0)
									BEGIN
											INSERT INTO @ROLES(POSITION, ROLE,DESCR)
											SELECT ISNULL(@POSITION,@POS),RelatedObjectCode,ROLE_DESC FROM
											(SELECT RelatedObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId=@ENDPOINTID AND RelatedObjectType='AG' AND ActualObjectType='O' AND ActualObjectCode = @ORGUNIT
											) AS A INNER JOIN DI_SAP_ROLEINFO R ON A.RelatedObjectCode = R.ROLE_CODE WHERE EndPointSystemId = @ENDPOINTID

											SET @ORGUNIT  = (SELECT  TOP 1 ActualObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId=@ENDPOINTID AND RelatedObjectCode = @ORGUNIT AND RelatedObjectType='O' AND ActualObjectType='O')

									END
									ELSE 
									BEGIN
											SET @ISPARENTEXISTS = 0
									END

							END
							
							SET @POSCOUNT = @POSCOUNT - 1
					END


					IF EXISTS(SELECT 1 FROM @ROLES)
					BEGIN
					INSERT INTO @TEMP_ROLES(Position,TECHNICAL_ROLE,FromValue,[DESCRIPTION],ToValue)
					SELECT POSITION,ROLE AS Entitlement,FromValue,DESCR,ToValue FROM 
					(
						SELECT Distinct POSITION, ROLE,DESCR from @ROLES
					) AS A
					INNER JOIN (
						SELECT RoleName,FromValue,ToValue FROM DI_SAP_RoleToAuthMap WHERE EndPointSystemId = @ENDPOINTID AND ObjectCode = 'S_TCODE' AND FieldCode = 'TCD' AND ProfileGeneratorVariant != 'X'
					)AS SR ON ROLE = SR.RoleName 
					END
					IF EXISTS(SELECT 1 FROM @TEMP_ROLES)
					BEGIN

						IF EXISTS(SELECT 1 FROM @TEMP_ROLES WHERE FromValue = '*')
						BEGIN
							---FOR ALL TCODE---
							IF EXISTS(SELECT 1 FROM DI_M_EndpointSystem WHERE Id = @ENDPOINTID AND ApplicationType IN(9,10))
							BEGIN
								INSERT INTO DBO.DI_SAP_CriticalTcodeReport(EndPointSystemId,EndpointSystemName,AccountName,Displayname,[Transaction],[Description],BusinessRole, TechnicalRole,FromPosition,AccountType,createdBy, CreatedDate, [UserName],[UserDisplayName],[TransactionOwner],AccountOwnerId)
								SELECT distinct
									@ENDPOINTID,@ENDPOINT_NAME,@ACCOUNTNAME,@DISPLAYNAME,M.[Transaction],M.[TransactionDescription],ISNULL(D.RoleName,M.[DESCRIPTION]), M.TECHNICAL_ROLE,M.Position,@ACCOUNTTYPE,'System',@CURRENTDATE,@USER_NAME,@UserDisplayName,TransactionOwner,@AccountOwner FROM  
									(
										SELECT [Transaction],[TransactionDescription],TCODE_Code,TCODE_Desc,TECHNICAL_ROLE,Position,FromValue,[DESCRIPTION],TransactionOwner FROM 
										(
										SELECT M.[Transaction],M.[TransactionDescription],T.TCODE_Code,T.TCODE_Desc,M.TransactionOwner FROM DI_SAP_EBS_XL_CriticalTCodesMatrix M INNER JOIN [dbo].[DI_SAP_TCODEINFO] T ON M.[Transaction] = T.TCODE_Code
										WHERE EndPointSystemId = @ENDPOINTID
										) AS MR 
										CROSS JOIN (SELECT TECHNICAL_ROLE,Position,FromValue,[DESCRIPTION] FROM @TEMP_ROLES WHERE FromValue = '*')AS B
									)AS M
									LEFT JOIN (SELECT ROLENAME,item as TechnicalRole FROM DI_SAP_EBS_XL_RolesMatrix CROSS APPLY dbo.Split(TechnicalRole,',') WHERE Application = @MODULE)AS D ON M.TECHNICAL_ROLE = D.TechnicalRole
							END

							ELSE IF EXISTS(SELECT 1 FROM DI_M_EndpointSystem WHERE Id = @ENDPOINTID AND ApplicationType IN(7,8))
							BEGIN
									INSERT INTO DBO.DI_SAP_CriticalTcodeReport(EndPointSystemId,EndpointSystemName,AccountName,Displayname,[Transaction],[Description], BusinessRole, TechnicalRole,FromPosition,AccountType,createdBy, CreatedDate, [UserName],[UserDisplayName],[TransactionOwner],AccountOwnerId)
									SELECT distinct
									@ENDPOINTID,@ENDPOINT_NAME,@ACCOUNTNAME,@DISPLAYNAME,M.[Transaction],M.[TransactionDescription],ISNULL(D.RoleName,M.[DESCRIPTION]), M.TECHNICAL_ROLE,M.Position,@ACCOUNTTYPE,'System',@CURRENTDATE,@USER_NAME,@UserDisplayName,TransactionOwner,@AccountOwner FROM  
									(
										SELECT [Transaction],[TransactionDescription],TCODE_Code,TCODE_Desc,TECHNICAL_ROLE,Position,FromValue,[DESCRIPTION],TransactionOwner FROM 
										(
										SELECT M.[Transaction],M.[TransactionDescription],T.TCODE_Code,T.TCODE_Desc,M.TransactionOwner FROM DI_SAP_MSSL_XL_CriticalTCodeMatrix M INNER JOIN [dbo].[DI_SAP_TCODEINFO] T ON M.[Transaction] = T.TCODE_Code
										WHERE EndPointSystemId = @ENDPOINTID
										) AS MR 
										CROSS JOIN (SELECT TECHNICAL_ROLE,Position,FromValue,[DESCRIPTION] FROM @TEMP_ROLES WHERE FromValue = '*')AS B
									)AS M
									LEFT JOIN (
									SELECT ROLENAME,item as TechnicalRole FROM DI_SAP_MSSL_XL_RolesMatrix CROSS APPLY dbo.Split(TechnicalRole,',') WHERE Application = @MODULE)AS D ON M.TECHNICAL_ROLE = D.TechnicalRole
							END
						
					END
					
					IF EXISTS(SELECT 1 FROM @TEMP_ROLES WHERE FromValue <> '*' and LEN(LTRIM(RTRIM(ISNULL(ToValue,''))))> 0)
					BEGIN

							--- TCODE with in the Range ---
							IF EXISTS(SELECT 1 FROM DI_M_EndpointSystem WHERE Id = @ENDPOINTID AND ApplicationType IN(7,8,9,10))
							BEGIN
								INSERT INTO DBO.DI_SAP_CriticalTcodeReport(EndPointSystemId,EndpointSystemName,AccountName,Displayname,[Transaction],[Description],BusinessRole, TechnicalRole,FromPosition,AccountType,createdBy, CreatedDate, [UserName],[UserDisplayName],[TransactionOwner],AccountOwnerId)
								SELECT 
								@ENDPOINTID,@ENDPOINT_NAME,@ACCOUNTNAME,@DISPLAYNAME,M.[Transaction],M.[TransactionDescription],ISNULL(D.RoleName,T.DESCRIPTION), T.TECHNICAL_ROLE,T.Position,@ACCOUNTTYPE,'System',@CURRENTDATE,@USER_NAME,@UserDisplayName,M.TransactionOwner,@AccountOwner FROM DI_SAP_EBS_XL_CriticalTCodesMatrix M 
								INNER JOIN [dbo].[DI_SAP_TCODEINFO] B ON M.[Transaction] = B.TCODE_Code
								INNER JOIN @TEMP_ROLES T ON M.[Transaction] between T.FromValue and T.ToValue
								LEFT JOIN (SELECT ROLENAME,TechnicalRole FROM DI_SAP_EBS_XL_RolesMatrix
								WHERE Application = @MODULE)AS D ON T.TECHNICAL_ROLE = D.TechnicalRole AND LEN(LTRIM(RTRIM(ISNULL(ToValue,''))))> 0 AND FromValue <> '*'

								UNION

								SELECT 
								@ENDPOINTID,@ENDPOINT_NAME,@ACCOUNTNAME,@DISPLAYNAME,M.[Transaction],M.[TransactionDescription],ISNULL(D.RoleName,T.DESCRIPTION), T.TECHNICAL_ROLE,T.Position,@ACCOUNTTYPE,'System',@CURRENTDATE,@USER_NAME,@UserDisplayName,M.TransactionOwner,@AccountOwner FROM DI_SAP_EBS_XL_CriticalTCodesMatrix M 
								INNER JOIN [dbo].[DI_SAP_TCODEINFO] B ON M.[Transaction] = B.TCODE_Code
								INNER JOIN @TEMP_ROLES T ON M.[Transaction] = T.FromValue
								LEFT JOIN (SELECT ROLENAME,TechnicalRole FROM DI_SAP_EBS_XL_RolesMatrix
								WHERE Application = @MODULE)AS D ON T.TECHNICAL_ROLE = D.TechnicalRole AND LEN(LTRIM(RTRIM(ISNULL(ToValue,''))))= 0 AND FromValue <> '*'
								
							END
							

					END

					Else IF EXISTS(SELECT 1 FROM @TEMP_ROLES WHERE FromValue <> '*')
					BEGIN

							---FOR ALL TCODE---
							IF EXISTS(SELECT 1 FROM DI_M_EndpointSystem WHERE Id = @ENDPOINTID AND ApplicationType IN(9,10))
							BEGIN
								INSERT INTO DBO.DI_SAP_CriticalTcodeReport(EndPointSystemId,EndpointSystemName,AccountName,Displayname,[Transaction],[Description],BusinessRole, TechnicalRole,FromPosition,AccountType,createdBy, CreatedDate, [UserName],[UserDisplayName],[TransactionOwner],AccountOwnerId)
								SELECT distinct
								@ENDPOINTID,@ENDPOINT_NAME,@ACCOUNTNAME,@DISPLAYNAME,M.[Transaction],M.[TransactionDescription],ISNULL(D.RoleName,T.DESCRIPTION), T.TECHNICAL_ROLE,T.Position,@ACCOUNTTYPE,'System',@CURRENTDATE,@USER_NAME,@UserDisplayName,M.TransactionOwner,@AccountOwner FROM DI_SAP_EBS_XL_CriticalTCodesMatrix M 
								INNER JOIN [dbo].[DI_SAP_TCODEINFO] B ON M.[Transaction] = B.TCODE_Code
								INNER JOIN @TEMP_ROLES T ON M.[Transaction] = T.FromValue
								LEFT JOIN (SELECT ROLENAME,item as TechnicalRole FROM DI_SAP_EBS_XL_RolesMatrix CROSS APPLY dbo.Split(TechnicalRole,',') WHERE Application = @MODULE)AS D ON T.TECHNICAL_ROLE = D.TechnicalRole
								
							END
							ELSE IF EXISTS(SELECT 1 FROM DI_M_EndpointSystem WHERE Id = @ENDPOINTID AND ApplicationType IN(7,8))
							BEGIN
									INSERT INTO DBO.DI_SAP_CriticalTcodeReport(EndPointSystemId,EndpointSystemName,AccountName,Displayname,[Transaction],[Description], BusinessRole, TechnicalRole,FromPosition,AccountType,createdBy, CreatedDate, [UserName],[UserDisplayName],[TransactionOwner],AccountOwnerId)
									SELECT distinct
									@ENDPOINTID,@ENDPOINT_NAME,@ACCOUNTNAME,@DISPLAYNAME,M.[Transaction],M.[TransactionDescription],ISNULL(D.RoleName,T.DESCRIPTION),T.TECHNICAL_ROLE,T.Position,@ACCOUNTTYPE,'System',@CURRENTDATE,@USER_NAME,@UserDisplayName,M.TransactionOwner,@AccountOwner FROM DI_SAP_MSSL_XL_CriticalTCodeMatrix M 
									INNER JOIN [dbo].[DI_SAP_TCODEINFO] B ON M.[Transaction] = B.TCODE_Code
									INNER JOIN @TEMP_ROLES T ON M.[Transaction] = T.FromValue
									LEFT JOIN (SELECT ROLENAME,item as TechnicalRole FROM DI_SAP_MSSL_XL_RolesMatrix CROSS APPLY dbo.Split(TechnicalRole,',')  WHERE Application = @MODULE )AS D ON T.TECHNICAL_ROLE = D.TechnicalRole
									WHERE T.FromValue <> '*'
								
							END

					END
					
					END

					SET @INCR = @INCR + 1
					DELETE FROM @TEMP_ROLES
					DELETE FROM @ROLES
			END
		
	END

	IF OBJECT_ID('tempdb..#TBLPOST') IS NOT NULL
	BEGIN
			DROP TABLE #TBLPOST
	END
	IF OBJECT_ID('tempdb..#TEMPACCOUNTS') IS NOT NULL
	BEGIN
			DROP TABLE #TEMPACCOUNTS
	END

END


