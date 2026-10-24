
--------------------------------------------------------------Matrix Master data ----------------------------------------------------------------------------------


IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_NOVA_UserRoleMatrix')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_NOVA_UserRoleMatrix','Level1Approver,Level1Approver,Level3Approver','NOVA UserRole Matrix','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_NOVA_ExceptionalRoleMatrix')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_NOVA_ExceptionalRoleMatrix','Level1Approver,Level1Approver,Level3Approver','NOVA ExceptionalRole Matrix','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_NOVA_ManagementRoleMatrix')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_NOVA_ManagementRoleMatrix','Level1Approver,Level1Approver,Level3Approver','NOVA ManagementRole Matrix','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_M_XLApproverExceptionInfo')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_M_XLApproverExceptionInfo','Approver','Approver Exception','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_SAP_EBS_XL_CriticalTCodesMatrix')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_SAP_EBS_XL_CriticalTCodesMatrix','TransactionOwner','SAP EBS Critical TCodes Matrix','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_SAP_EBS_XL_PositionsMatrix')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_SAP_EBS_XL_PositionsMatrix','Level1Approver,Level1Approver,Level3Approver','SAP EBS Positions Matrix','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_SAP_EBS_XL_RolesMatrix')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_SAP_EBS_XL_RolesMatrix','Level1Approver,Level1Approver,Level3Approver','SAP EBS Roles Matrix','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_SAP_HRFLINSRoleMatrix')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_SAP_HRFLINSRoleMatrix','Level1Approver,Level1Approver,Level3Approver','SAP FLINS Main Role Matrix','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_SAP_GRCRole')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_SAP_GRCRole','Level1Approver,Level1Approver,Level3Approver','SAP GRC Role Matrix','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_SAP_MSSL_XL_CriticalTCodeMatrix')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_SAP_MSSL_XL_CriticalTCodeMatrix','TransactionOwner','SAP MSSL Critical TCodes Matrix','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_SAP_MSSL_XL_PositionsMatrix')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_SAP_MSSL_XL_PositionsMatrix','Level1Approver,Level1Approver,Level3Approver','SAP MSSL Positions Matrix','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_SAP_MSSL_XL_RolesMatrix')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_SAP_MSSL_XL_RolesMatrix','Level1Approver,Level1Approver,Level3Approver','SAP MSSL Roles Matrix','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_SAP_BIRoleCategories')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_SAP_BIRoleCategories','Level1Approver,Level1Approver,Level3Approver','SAP HRFLINS BI RoleCategories','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_SAP_PIRole')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_SAP_PIRole','Level1Approver,Level1Approver,Level3Approver','SAP PI Role Matrix','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_SAP_SRMMainRoleMatrix')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_SAP_SRMMainRoleMatrix','Level1Approver,Level1Approver,Level3Approver','SRM Main Role Matrix','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_SMS_XLRoleCatalogue')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_SMS_XLRoleCatalogue','Level1Approver,Level1Approver,Level3Approver','OneCard Role Catalogue','EmployeeId')
END

IF NOT EXISTS (SELECT 1 FROM DI_M_UserTableInfo WHERE TableName = 'DI_M_EndpointSystem')
BEGIN
	INSERT INTO DI_M_UserTableInfo(TableName, Columns, Caption, ColumnType) values ('DI_M_EndpointSystem','OwnerUserId','Endpoint System','UserId')
END
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


--- Head of Section, Branch, Department
IF NOT EXISTS(SELECT 1 FROM [DI_M_RoleTypes] WHERE [DisplayName] = 'Head of Section')
BEGIN
	INSERT [dbo].[DI_M_RoleTypes] ([RoleTypeDesc],[DisplayName], [IsActive], [ApplicableApprovalUserTypes], [ApplicableRequestTypes],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
	VALUES (N'Head of Section', N'Head of Section',1, N'2', '1,2', N'System', GETDATE(), N'System', GETDATE())
END
IF NOT EXISTS(SELECT 1 FROM [DI_M_RoleTypes] WHERE [DisplayName] = 'Head of Branch')
BEGIN
	INSERT [dbo].[DI_M_RoleTypes] ([RoleTypeDesc],[DisplayName], [IsActive], [ApplicableApprovalUserTypes], [ApplicableRequestTypes],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
	VALUES (N'Head of Branch', N'Head of Branch',1, N'2', '1,2', N'System', GETDATE(), N'System', GETDATE())
END
IF NOT EXISTS(SELECT 1 FROM [DI_M_RoleTypes] WHERE [DisplayName] = 'Head of Department')
BEGIN
	INSERT [dbo].[DI_M_RoleTypes] ([RoleTypeDesc],[DisplayName], [IsActive], [ApplicableApprovalUserTypes], [ApplicableRequestTypes],[CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
	VALUES (N'Head of Department', N'Head of Department',1, N'2', '1,2', N'System', GETDATE(), N'System', GETDATE())
END
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


-------------------------- Workflow user types ---------------------------------------------------------------------------------------------------------------------------------------
Delete DI_M_EmployeeTerm
DBCC CHECKIDENT (DI_M_EmployeeTerm, reseed, 1)
go

SET IDENTITY_INSERT DI_M_EmployeeTerm ON
declare @CreatedBy nvarchar(10) = 'System'
declare @CreatedDate datetime = getdate()
insert into DI_M_EmployeeTerm (Id, EmployeeTermDesc, IsActive, CreatedBy, CreatedDate) 
values	(1,'Permanent', 1, @CreatedBy, @CreatedDate),
		(2,'Vendor', 1, @CreatedBy, @CreatedDate),
		(3,'Contractor', 1, @CreatedBy, @CreatedDate),
		(4,'TempStaff', 1, @CreatedBy, @CreatedDate)
SET IDENTITY_INSERT DI_M_EmployeeTerm OFF
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

delete from DI_M_RequestTypes where id = 7

SET IDENTITY_INSERT DI_M_RequestTypes ON
GO  

Declare @CreatedBy nvarchar(10) = 'System'
Declare @CreatedDate datetime = getdate()
insert into DI_M_RequestTypes (Id, RequestTypeDesc, IsActive, CreatedBy, CreatedDate)
select 5, 'Employee Event', 1, @CreatedBy, @CreatedDate
SET IDENTITY_INSERT DI_M_RequestTypes OFF
GO 
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
