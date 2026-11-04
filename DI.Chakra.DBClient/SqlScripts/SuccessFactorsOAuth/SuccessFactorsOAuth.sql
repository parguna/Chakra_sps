-- SuccessFactors OAuth (SAML bearer, certificate based) endpoint configuration keys.
-- Idempotent: safe to run more than once. Run against the IDMS database.
-- Existing SuccessFactors endpoints are not changed: with no SF_EnableOAuth value they keep using Basic authentication,
-- and the IsRequired setting of the existing Password key is not modified.
-- Only the certificate THUMBPRINT is stored. The private key stays in the Windows certificate store.

SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Key IDs must match DI.Chakra.Infrastructure.Enums.EndpointKey. Stop if an ID is already used by a different key.
IF EXISTS (SELECT 1 FROM DI_M_EndpointConfigurationKey
           WHERE (ID = 75 AND KeyName <> 'SF_EnableOAuth')
              OR (ID = 76 AND KeyName <> 'SF_BaseCompanyUrl')
              OR (ID = 77 AND KeyName <> 'SF_ClientId')
              OR (ID = 78 AND KeyName <> 'SF_CertificateThumbprint')
              OR (ID = 79 AND KeyName <> 'SF_GrantType'))
BEGIN
    RAISERROR('DI_M_EndpointConfigurationKey IDs 75-79 are already used by other keys. Align the EndpointKey enum before running this script.', 16, 1);
    ROLLBACK TRANSACTION;
    RETURN;
END

SET IDENTITY_INSERT DI_M_EndpointConfigurationKey ON;

IF NOT EXISTS (SELECT 1 FROM DI_M_EndpointConfigurationKey WHERE KeyName = 'SF_EnableOAuth')
    INSERT INTO DI_M_EndpointConfigurationKey(ID, KeyName, Caption, CreatedBy, CreatedDate)
    VALUES (75, 'SF_EnableOAuth', 'Use OAuth (Certificate)', 'System', GETDATE());

IF NOT EXISTS (SELECT 1 FROM DI_M_EndpointConfigurationKey WHERE KeyName = 'SF_BaseCompanyUrl')
    INSERT INTO DI_M_EndpointConfigurationKey(ID, KeyName, Caption, CreatedBy, CreatedDate)
    VALUES (76, 'SF_BaseCompanyUrl', 'Base Company URL (API Server)', 'System', GETDATE());

IF NOT EXISTS (SELECT 1 FROM DI_M_EndpointConfigurationKey WHERE KeyName = 'SF_ClientId')
    INSERT INTO DI_M_EndpointConfigurationKey(ID, KeyName, Caption, CreatedBy, CreatedDate)
    VALUES (77, 'SF_ClientId', 'OAuth Client ID (API Key)', 'System', GETDATE());

IF NOT EXISTS (SELECT 1 FROM DI_M_EndpointConfigurationKey WHERE KeyName = 'SF_CertificateThumbprint')
    INSERT INTO DI_M_EndpointConfigurationKey(ID, KeyName, Caption, CreatedBy, CreatedDate)
    VALUES (78, 'SF_CertificateThumbprint', 'OAuth Certificate Thumbprint', 'System', GETDATE());

IF NOT EXISTS (SELECT 1 FROM DI_M_EndpointConfigurationKey WHERE KeyName = 'SF_GrantType')
    INSERT INTO DI_M_EndpointConfigurationKey(ID, KeyName, Caption, CreatedBy, CreatedDate)
    VALUES (79, 'SF_GrantType', 'OAuth Grant Type', 'System', GETDATE());

SET IDENTITY_INSERT DI_M_EndpointConfigurationKey OFF;

DECLARE @TypeId BIGINT;
SELECT @TypeId = Id FROM DI_M_EndpointSystemType WHERE EndpointSystemTypeName = 31;   -- EndpointType.SuccessFactor

IF @TypeId IS NULL
BEGIN
    RAISERROR('SuccessFactor endpoint system type (31) was not found in DI_M_EndpointSystemType.', 16, 1);
    ROLLBACK TRANSACTION;
    RETURN;
END

DECLARE @Order INT;

-- Not required by default; the endpoint configuration screen makes the OAuth fields required when OAuth is ticked.
IF NOT EXISTS (SELECT 1 FROM DI_M_EndpointKeyConfiguration WHERE EndpointSystemTypeId = @TypeId AND KeyId = 75)
BEGIN
    SELECT @Order = ISNULL(MAX([Order]), 0) + 1 FROM DI_M_EndpointKeyConfiguration WHERE EndpointSystemTypeId = @TypeId;
    INSERT INTO DI_M_EndpointKeyConfiguration(EndpointSystemTypeId, KeyId, [Order], DataType, IsRequired, CreatedBy, CreatedDate)
    VALUES (@TypeId, 75, @Order, 'bit', 0, 'System', GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM DI_M_EndpointKeyConfiguration WHERE EndpointSystemTypeId = @TypeId AND KeyId = 76)
BEGIN
    SELECT @Order = ISNULL(MAX([Order]), 0) + 1 FROM DI_M_EndpointKeyConfiguration WHERE EndpointSystemTypeId = @TypeId;
    INSERT INTO DI_M_EndpointKeyConfiguration(EndpointSystemTypeId, KeyId, [Order], DataType, IsRequired, ClientScript, CreatedBy, CreatedDate)
    VALUES (@TypeId, 76, @Order, 'varchar', 0, 'onkeypress:return preventstartingspace(this);', 'System', GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM DI_M_EndpointKeyConfiguration WHERE EndpointSystemTypeId = @TypeId AND KeyId = 77)
BEGIN
    SELECT @Order = ISNULL(MAX([Order]), 0) + 1 FROM DI_M_EndpointKeyConfiguration WHERE EndpointSystemTypeId = @TypeId;
    INSERT INTO DI_M_EndpointKeyConfiguration(EndpointSystemTypeId, KeyId, [Order], DataType, IsRequired, ClientScript, CreatedBy, CreatedDate)
    VALUES (@TypeId, 77, @Order, 'varchar', 0, 'onkeypress:return preventstartingspace(this);', 'System', GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM DI_M_EndpointKeyConfiguration WHERE EndpointSystemTypeId = @TypeId AND KeyId = 78)
BEGIN
    SELECT @Order = ISNULL(MAX([Order]), 0) + 1 FROM DI_M_EndpointKeyConfiguration WHERE EndpointSystemTypeId = @TypeId;
    INSERT INTO DI_M_EndpointKeyConfiguration(EndpointSystemTypeId, KeyId, [Order], DataType, IsRequired, ClientScript, CreatedBy, CreatedDate)
    VALUES (@TypeId, 78, @Order, 'varchar', 0, 'onkeypress:return preventstartingspace(this);', 'System', GETDATE());
END

-- Optional: empty means urn:ietf:params:oauth:grant-type:saml2-bearer, the only grant type supported by the connector.
IF NOT EXISTS (SELECT 1 FROM DI_M_EndpointKeyConfiguration WHERE EndpointSystemTypeId = @TypeId AND KeyId = 79)
BEGIN
    SELECT @Order = ISNULL(MAX([Order]), 0) + 1 FROM DI_M_EndpointKeyConfiguration WHERE EndpointSystemTypeId = @TypeId;
    INSERT INTO DI_M_EndpointKeyConfiguration(EndpointSystemTypeId, KeyId, [Order], DataType, IsRequired, ClientScript, CreatedBy, CreatedDate)
    VALUES (@TypeId, 79, @Order, 'varchar', 0, 'onkeypress:return preventstartingspace(this);', 'System', GETDATE());
END

COMMIT TRANSACTION;
GO
