
DECLARE @EndpointSystemTypeId bigint
DECLARE @KeyId bigint

-- AD
SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='Active Directory'
INSERT [dbo].DI_M_EndPointAutoExpiry (EndpointSystemTypeId,EndPointSystemId,IsAccountAutoExpirySupported,IsEntitlementAutoExpirySupported,CreatedDate)VALUES (@EndpointSystemTypeId,NULL,1,1,GETDATE())

SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='Exchange'
INSERT [dbo].DI_M_EndPointAutoExpiry (EndpointSystemTypeId,EndPointSystemId,IsAccountAutoExpirySupported,IsEntitlementAutoExpirySupported,CreatedDate)VALUES (@EndpointSystemTypeId,NULL,0,0,GETDATE())

SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='PeopleSoftFinanceBI'
INSERT [dbo].DI_M_EndPointAutoExpiry (EndpointSystemTypeId,EndPointSystemId,IsAccountAutoExpirySupported,IsEntitlementAutoExpirySupported,CreatedDate)VALUES (@EndpointSystemTypeId,NULL,1,1,GETDATE())

SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='BizCMS'
INSERT [dbo].DI_M_EndPointAutoExpiry (EndpointSystemTypeId,EndPointSystemId,IsAccountAutoExpirySupported,IsEntitlementAutoExpirySupported,CreatedDate)VALUES (@EndpointSystemTypeId,NULL,0,0,GETDATE())

SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='PeoplesoftHRMS'
INSERT [dbo].DI_M_EndPointAutoExpiry (EndpointSystemTypeId,EndPointSystemId,IsAccountAutoExpirySupported,IsEntitlementAutoExpirySupported,CreatedDate)VALUES (@EndpointSystemTypeId,NULL,0,0,GETDATE())

SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='PeopleSoftFINOrPROC'
INSERT [dbo].DI_M_EndPointAutoExpiry (EndpointSystemTypeId,EndPointSystemId,IsAccountAutoExpirySupported,IsEntitlementAutoExpirySupported,CreatedDate)VALUES (@EndpointSystemTypeId,NULL,0,0,GETDATE())

SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='ISMS'
INSERT [dbo].DI_M_EndPointAutoExpiry (EndpointSystemTypeId,EndPointSystemId,IsAccountAutoExpirySupported,IsEntitlementAutoExpirySupported,CreatedDate)VALUES (@EndpointSystemTypeId,NULL,0,0,GETDATE())

SELECT @EndpointSystemTypeId =Id FROM [DI_M_EndpointSystemType] WHERE DisplayName ='IDAS'
INSERT [dbo].DI_M_EndPointAutoExpiry (EndpointSystemTypeId,EndPointSystemId,IsAccountAutoExpirySupported,IsEntitlementAutoExpirySupported,CreatedDate)VALUES (@EndpointSystemTypeId,NULL,0,0,GETDATE())

--SET IDENTITY_INSERT [dbo].DI_M_EndPointAutoExpiry OFF
GO	
