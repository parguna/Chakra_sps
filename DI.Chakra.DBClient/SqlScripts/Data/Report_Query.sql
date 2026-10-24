--INSERT INTO DI_M_IMConfigurationMaster(ConfigKey,ConfigValue,Description,CreatedBy,CreatedDate,IsUiDisplay)
--VALUES('DailyHRMSProcessId','4','','System',GETDATE(),1)

---REPORT---
---Entitlement---

Select * from DI_M_EntitlementReconcilation where AccountReconcilationId in(
SELECT A.Id FROM 
(
SELECT E.DisplayName as EndpointSystem,INACT.* FROM DI_M_AccountReconcilation INACT Inner Join DI_M_EndpointSystem E on INACT.EndpointSystemId = E.Id
where INACT.IsDeleted=1
) AS A Inner Join 
(
SELECT A.Id,A.EndpointSystemId,AccountName from DI_M_AccountReconcilation A Inner Join DI_M_EndpointSystem E on A.EndpointSystemId = E.id 
where a.IsDeleted=0
) AS B on A.EndpointSystemId = B.EndpointSystemId AND A.AccountName = B.AccountName
where A.AccountName !='' and B.AccountName != ''
) 

Select * From DI_M_AccountReconcilation WHERE ID IN
(
SELECT A.Id FROM 
(
SELECT E.DisplayName as EndpointSystem,INACT.* FROM DI_M_AccountReconcilation INACT Inner Join DI_M_EndpointSystem E on INACT.EndpointSystemId = E.Id
where INACT.IsDeleted=1
) AS A Inner Join 
(
SELECT A.Id,A.EndpointSystemId,AccountName from DI_M_AccountReconcilation A Inner Join DI_M_EndpointSystem E on A.EndpointSystemId = E.id 
where a.IsDeleted=0
) AS B on A.EndpointSystemId = B.EndpointSystemId AND A.AccountName = B.AccountName
where A.AccountName !='' and B.AccountName != ''
)

GO
update DI_M_CustomReport set ReportQuery='SELECT  U.UserName,U.DisplayName,U.EmployeeId,U.EmailId,UA.extensionAttribute4 as EmailDomain, U.CompanyName,U.DepartmentName,U.DivisionName,U.SectionName,U.PositionName as Position,  U.CostCenter,CASE WHEN ISNULL(UA.extensionAttribute4,'''')=''NoInternetMail'' THEN ''No'' ELSE ''Yes'' End as HasInternetAccess
FROM DI_ADS_UserInfo UA INNER JOIN DI_M_AccountReconcilation A on UA.Id = A.EndpointUserId 
INNER JOIN DI_M_User U on A.UserId = U.Id  INNER JOIN DI_M_EndpointSystem E On E.Id = A.EndpointSystemId
where U.IsActive=1 and A.IsDeleted=0 AND U.id in(select UserId from DI_M_AccountReconcilation WHERE EndpointSystemId = (select Id from DI_M_EndpointSystem where ApplicationType = 18) )
AND E.ApplicationType = 1  AND A.AccountType=1 AND A.UserId IS NOT NULL' where uniqueName='REPORT_USERS_EXTERNAL_EMAIL_ACCESS'

