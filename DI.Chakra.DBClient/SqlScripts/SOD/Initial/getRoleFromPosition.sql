DROP FUNCTION DBO.getRoleFromPosition
GO
-- =============================================
-- Author:		SIVANANDAN
-- Create date: 23 NOV 2021
-- Description:	GET THE INDIRECT ROLES BY POSITION FOR SPECIFIED ENDPOINT
-- example select * from DBO.getRoleFromPosition(10,50000352) 
-- =============================================
CREATE FUNCTION [dbo].[getRoleFromPosition] 
(	
	@ENDPOINTID bigint,
	@POS nvarchar(512)
)
RETURNS @ROLES TABLE(POSITION NVARCHAR(512),ROLE NVARCHAR(MAX) NULL,DESCR NVARCHAR(MAX) NULL) 
AS
BEGIN

	
	INSERT INTO @ROLES(POSITION,ROLE,DESCR)
	SELECT @POS,RelatedObjectCode,ROLE_DESC FROM (SELECT RelatedObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId = @ENDPOINTID AND ActualObjectCode = @POS AND RelatedObjectType='AG' AND ActualObjectType='S')AS A
	INNER JOIN DI_SAP_ROLEINFO R ON A.RelatedObjectCode = R.ROLE_CODE WHERE EndPointSystemId = @ENDPOINTID
	--GetOrgByPosition--
	DECLARE @ORGUNIT NVARCHAR(512)=''
	SET @ORGUNIT  = (SELECT  TOP 1 RelatedObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId=@ENDPOINTID AND ActualObjectCode = @POS AND RelatedObjectType='O' AND ActualObjectType='S')

	IF LEN(ISNULL(@ORGUNIT,'')) > 0
	BEGIN
							
	--getrolesbyorg--
	INSERT INTO @ROLES(POSITION,ROLE,DESCR)
	SELECT @POS,RelatedObjectCode,ROLE_DESC FROM
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
					SELECT @POS,RelatedObjectCode,ROLE_DESC FROM
					(SELECT RelatedObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId=@ENDPOINTID AND RelatedObjectType='AG' AND ActualObjectType='O' AND ActualObjectCode = @ORGUNIT
					) AS A INNER JOIN DI_SAP_ROLEINFO R ON A.RelatedObjectCode = R.ROLE_CODE WHERE EndPointSystemId = @ENDPOINTID

					SET @ORGUNIT  = (SELECT  TOP 1 ActualObjectCode FROM DI_SAP_SAPHRP1001 WHERE EndpointSystemId=@ENDPOINTID AND RelatedObjectCode = @ORGUNIT AND RelatedObjectType='O' AND ActualObjectType='O')

			END
			ELSE 
			BEGIN
				SET @ISPARENTEXISTS = 0
			END
	END
	RETURN
END