delete DI_M_AdminModeItem
DECLARE @TabMenuId int 
DBCC CHECKIDENT (DI_M_AdminModeItem, reseed, 0)

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_ENDPOINTSYSTEM'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Endpoint System','Endpoint System','/Images/tools-icons/access-request-form.png','Endpoint System',4,@TabMenuId,'DI.Chakra.Model.EndPoint')

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_DISCOVERYJOB'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Discovery Job','Discovery Job','/Images/tools-icons/access-request-form.png','Discovery Job',3,@TabMenuId,'DI.Chakra.Model.DiscoveryProcess')

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_ATTESTATION'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Attestation','Attestation','/Images/tools-icons/access-request-form.png','Attestation',3,@TabMenuId,'DI.Chakra.Model.Attestation')

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_TRUSTEDSOURCE'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Trusted Source','Trusted Source','/Images/tools-icons/access-request-form.png','Trusted Source',2,@TabMenuId,'DI.Chakra.Model.ConfigGeneralTrustedSource')

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_WORKFLOW'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Workflow','Workflow','/Images/tools-icons/access-request-form.png','Workflow',3,@TabMenuId,'DI.Chakra.Model.WorkflowConfiguration')


select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_GROUPS'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Group','Group','/Images/tools-icons/access-request-form.png','Group',2,@TabMenuId,'DI.Chakra.Model.Group')

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_ROLES'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Role','Role','/Images/tools-icons/access-request-form.png','Role',2,@TabMenuId,'DI.Chakra.Model.Role')

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_PROVISIONING_FORM'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Provisioning Form','Provisioning Form','/Images/tools-icons/access-request-form.png','Provisioning Form',3,@TabMenuId,'DI.Chakra.Model.ProvisioningForm')

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_PROVISIONING_ROLE'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Provisioning Role','Provisioning Role','/Images/tools-icons/access-request-form.png','Provisioning Role',3,@TabMenuId,'DI.Chakra.Model.ProvisioningRole')

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_ROLEMATRIX'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Role Matrix','Role Matrix','/Images/tools-icons/access-request-form.png','Role Matrix',3,@TabMenuId,'DI.Chakra.Model.RoleMatrix')

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_PROVISIONING_MYREQUESTS'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Request Details','Request Details','/Images/tools-icons/access-request-form.png','Request Details',3,@TabMenuId,'')

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_PROVISIONING__MYTASK'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Task Details','Task Details','/Images/tools-icons/access-request-form.png','Task Details',3,@TabMenuId,'')

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_ACCESSSNAPSHOT'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Access Snapshot','Access Snapshot','/Images/tools-icons/access-request-form.png','Access Snapshot',3,@TabMenuId,'')

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_MAIL_SMTPSERVER'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('SMTP Server','SMTP Server','/Images/tools-icons/access-request-form.png','SMTP Server',3,@TabMenuId,'DI.Chakra.Model.MailServerConfiguration')

select @TabMenuId = Id from DI_M_TabMenu where UniqueName='ADMIN_MAIL_TEMPLATES'
INSERT INTO DI_M_AdminModeItem( Name, Description,  Icon, Information, Type, TabMenuId, TableName)
VALUES ('Templates','Templates','/Images/tools-icons/access-request-form.png','Templates',3,@TabMenuId,'DI.Chakra.Model.MailTemplate')

Update [dbo].[DI_M_AdminModeItem] Set CreatedDate=getdate(), CreatedBy = 'System'