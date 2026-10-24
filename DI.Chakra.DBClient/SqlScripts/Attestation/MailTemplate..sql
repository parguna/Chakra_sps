SET IDENTITY_INSERT [dbo].DI_M_MailTemplate ON 
GO
Delete from DI_M_MailTemplate where [TemplateName]='NotifyAttestationApproval'

INSERT [dbo].DI_M_MailTemplate ( [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], 
[LastModifiedBy], [LastModifiedDate]) VALUES ( N'NotifyAttestationApproval', N'Notify Attestation Approval', N'securityadmin@spgroup.com.sg', N'', N'', NULL, N'IDMS - Perform User Access Rights Certification for @@CAMPAIGN_NAME@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title></title>
        <style type="text/css">
            .body {
            font-family: Courier New;
            font-size: 13px;
            font-weight: normal;
            margin-left: 5px;
            }
            .marlef {
            margin-left: 40px;
            }
            .marlef1 {
            margin-left: 50px;
            }
            .marlef2 {
            margin-left: 50px;
            }
        </style>
    </head>
    <body class="body" data-gr-c-s-loaded="true">
        <p>Dear @@REVIEWER_NAME@@,<br />
        <br />
        Identity Access Management (IAM) team is initiating the access rights review for @@ATTESTATION_NAME@@.&nbsp;<br />
        Please perform online review via IDMS Portal for the user access rights listed.<br />
        <br />
        Attestation Name<span style="white-space: pre;">	</span>: @@ATTESTATION_NAME@@<br />
        Attestation Description<span style="white-space: pre;">	</span>: @@ATTESTATION_DESC@@<br />
        Attestation CampaignName: @@CAMPAIGN_NAME@@<br />
        Due Date<span style="white-space: pre;">		</span>: @@REVIEWLASTDATE@@<br />
		<b>Delegation Comments</b><span style="white-space: pre;">	</span>: @@DELEGATIONCOMMENTS@@<br />
        <br />
        Scope of Works<br />
        Please conduct the review covering the following:<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; - Proper segregation of duties have been considered for users&rsquo; access rights<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; - Users are not granted rights beyond the requirements of their role<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; - Changes to users&rsquo; access rights are properly configured<br />
        <br />
        How to Review<br />
        Log in to IDMS&nbsp; @@IACMURL@@<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;1. Go to Home -&gt; My Task -&gt; Select the task to review&nbsp;<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;2. Click on "Detail" icon to start the review<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;3. For each identified user, to validate that the access is still required and that the assigned role(s) is correct. If access is no longer required, click on ''Revoke'' to remove the access.<br />
        <br />
        Thank You.<br />
        <br />
        Regards<br />
        @@REGARDS@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), N'iacmadmin', CAST(N'2020-09-04T11:50:33.230' AS DateTime))
GO
Delete from DI_M_MailTemplate where [TemplateName]='EscalateAttestationApproval'
INSERT [dbo].DI_M_MailTemplate ( [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate],
 [LastModifiedBy], [LastModifiedDate]) VALUES ( N'EscalateAttestationApproval', N'Escalate Attestation Approval', N'devteam@deepidentity.com', N'@@FRE@@', N'', NULL, N'IDMS ESCALATION - Review of @@CAMPAIGN_NAME@@ was not completed by @@REVIEWER_NAME@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title>IDAS - Escalation: Review of ##ATTTSK_ENDPOINTNAME_DISP## @@ATTE_TYPE@@ ##ATTE_TYPE_DESC## - @@ATTTSK_CAMPAIGN_NAME@@</title>
        <style type="text/css">
            .title {
            font-family: "lucida grande", tahoma, verdana, arial, sans-serif;
            font-size: 13px;
            color: #1c2a47;
            font-weight: bolder;
            }
            .body {
            font-family: "lucida grande", tahoma, verdana, arial, sans-serif;
            font-size: 12px;
            color: InfoText;
            font-weight: normal;
            margin-left: 50px;
            }
            .marlef {
            margin-left: 40px;
            }
            .marlef1 {
            margin-left: 50px;
            }
            .marlef2 {
            margin-left: 50px;
            }
        </style>
    </head>
    <body class="body" data-gr-c-s-loaded="true">
        <div><span style="font-family: ''Courier New''; font-size: 13px;">Dear @@RECEIVER@@,<br />
        <br />
        This is an escalation email to inform you that @@REVIEWER_NAME@@ did not complete the access rights review initiated by Identity &amp; Access Management System (IDMS).<br />
        <br />
        Please perform online review via IDMS Portal for the user access rights listed<br />
        <br />
        Attestation Name<span style="white-space: pre;">	</span>: @@ATTESTATION_NAME@@<br />
        Attestation Description<span style="white-space: pre;">	</span>: @@ATTESTATION_DESC@@<br />
        Attestation CampaignName: @@CAMPAIGN_NAME@@<br />
        Due Date<span style="white-space: pre;">		</span>: @@REVIEWLASTDATE@@<br />
        <br />
        Scope of Works<br />
        Please conduct the review covering the following:<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; - Proper segregation of duties have been considered for users&rsquo; access rights<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; - Users are not granted rights beyond the requirements of their role<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; - Changes to users&rsquo; access rights are properly configured<br />
        <br />
        How to Review<br />
        Log in to IDMS&nbsp; @@IACMURL@@<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;1. Go to Home -&gt; My Task -&gt; Select the task to review&nbsp;<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;2. Click on "Detail" icon to start the review<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;3. For each identified user, to validate that the access is still required and that the assigned role(s) is correct. If access is no longer required, click on ''Revoke'' to remove the access.<br />
        <br />
        Thank You.<br />
        <br />
        Regards<br />
        @@REGARDS@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***</span></div>
    </body>
</html>', NULL, CAST(N'2017-11-15T14:11:58.900' AS DateTime), N'iacmadmin', CAST(N'2020-09-04T11:21:12.347' AS DateTime))
GO
Delete from DI_M_MailTemplate where [TemplateName]='RemindAttestationApproval'
INSERT [dbo].DI_M_MailTemplate ( [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], 
[LastModifiedDate]) VALUES ( N'RemindAttestationApproval', N'Reminder Attestation Approval', N'devteam@deepidentity.com', N'##MAILTO_ATTTSK##', N'##ITCCEmail##', NULL, N'IDMS - Reminder to complete the review for @@CAMPAIGN_NAME@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title>IDAS Reminder - Review of ##ATTTSK_ENDPOINTNAME_DISP## @@ATTE_TYPE@@ ##ATTE_TYPE_DESC## - @@ATTTSK_CAMPAIGN_NAME@@</title>
        <style type="text/css">
            .title {
            font-family: "lucida grande", tahoma, verdana, arial, sans-serif;
            font-size: 13px;
            color: #1c2a47;
            font-weight: bolder;
            }
            .body {
            font-family: "lucida grande", tahoma, verdana, arial, sans-serif;
            font-size: 12px;
            color: InfoText;
            font-weight: normal;
            margin-left: 50px;
            }
        </style>
    </head>
    <body class="body" data-gr-c-s-loaded="true">
        <p><span style="font-family: ''Courier New''; font-size: 13px;">Dear @@REVIEWER_NAME@@,<br />
        <br />
        Please be reminded to complete the access rights review for @@ATTESTATION_NAME@@.&nbsp;<br />
        by @@REVIEWLASTDATE@@.<br />
        <br />
        Please perform online review via IDMS Portal for the user access rights listed<br />
        <br />
        Attestation Name: @@ATTESTATION_NAME@@<br />
        Attestation Description: @@ATTESTATION_DESC@@<br />
        Attestation CampaignName: @@CAMPAIGN_NAME@@<br />
        Due Date: @@REVIEWLASTDATE@@<br />
        <br />
        Scope of Works<br />
        Please conduct the review covering the following:<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; - Proper segregation of duties have been considered for users&rsquo; access rights<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; - Users are not granted rights beyond the requirements of their role<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; - Changes to users&rsquo; access rights are properly configured<br />
        <br />
        How to Review<br />
        Log in to IDMS&nbsp; @@IACMURL@@<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;1. Go to Home -&gt; My Task -&gt; Select the task to review&nbsp;<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;2. Click on "Detail" icon to start the review<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;3. For each identified user, to validate that the access is still required and that the assigned role(s) is correct. If access is no longer required, click on ''Revoke'' to remove the access.<br />
        <br />
        Thank You.<br />
        <br />
        Regards<br />
        @@REGARDS@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***</span></p>
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-14T17:31:28.017' AS DateTime), N'iacmadmin', CAST(N'2020-09-04T11:23:43.520' AS DateTime))
GO
Delete from DI_M_MailTemplate where [TemplateName]='NotifyAttestationComplete'
INSERT [dbo].DI_M_MailTemplate ( [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy],
 [LastModifiedDate]) VALUES ( N'NotifyAttestationComplete', N'Notify Attestation Complete', N'securityadmin@spgroup.com.sg', N'', N'', NULL, N'IDMS: Attestation - {0} Completed', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title></title>
        <style type="text/css">
            .body {
            font-family: Courier New;
            font-size: 13px;
            font-weight: normal;
            margin-left: 5px;
            }
            .marlef {
            margin-left: 40px;
            }
            .marlef1 {
            margin-left: 50px;
            }
            .marlef2 {
            margin-left: 50px;
            }
        </style>
    </head>
    <body class="body" data-gr-c-s-loaded="true">
        <p>Dear IAM Team,
        <br />
        <br />
        This is to notify that the below attestation process is completed successfully.
        <br />
        <br />
        Attestation Name: @@ATTESTATION_NAME@@
        <br />
        Attestation Description: @@ATTESTATION_DESC@@
        <br />
        <br />
        Regards
        <br />
        @@REGARDS@@&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.*** </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), N'iacmadmin', CAST(N'2020-09-04T11:19:34.340' AS DateTime))
GO
Delete from DI_M_MailTemplate where [TemplateName]='AttestationException'
INSERT [dbo].DI_M_MailTemplate ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], 
[LastModifiedDate]) VALUES ( N'AttestationException', N'Email will be send when there is no reviewer found', N'securityadmin@spgroup.com.sg', N'securityadmin@spgroup.com.sg', N'', NULL, N'IDMS - Attestation Exception (@@CAMPAIGN_NAME@@)', N'<p style="margin-left: 40px;">&nbsp;</p>
<p style="margin-left: 40px;"><span style="font-family: ''Courier New'';">Dear IAM Team,&nbsp;&nbsp;<br />
<br />
We cannot locate the reviewer for the below user(s) for the attestation initiated.<br />
<br />
User Name:<br />
@@USERREVIEWERNOTFOUND@@<br />
<br />
Attestation Name<span style="white-space: pre;">	</span>: @@ATTESTATION_NAME@@<br />
Attestation Description<span style="white-space: pre;">	</span>: @@ATTESTATION_DESC@@<br />
Attestation CampaignName: @@CAMPAIGN_NAME@@<br />
Due Date<span style="white-space: pre;">		</span>: @@REVIEWLASTDATE@@<br />
</span>
<div>&nbsp;</div>
</p>
<p style="margin-left: 40px;"><span style="font-family: ''Courier New'';">Regards<br />
@@REGARDS@@</span></p>
<p style="margin-left: 40px;"><span style="font-family: ''Courier New'';">*** This is a system-generated message. Please do not reply to this message.***</span></p>
<span style="font-family: ''Courier New'';">
<br />
</span>', N'iacmadmin', CAST(N'2020-09-03T17:57:09.903' AS DateTime), N'iacmadmin', CAST(N'2020-09-04T11:11:22.993' AS DateTime))
GO
Delete from DI_M_MailTemplate where [TemplateName]='DelegateAttestationTask'
INSERT [dbo].DI_M_MailTemplate ( [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], 
[LastModifiedDate]) VALUES ( N'DelegateAttestationTask', N'This email is sent to the delegated user that is delegated by the approver for the attestation task.', N'securityadmin@spgroup.com.sg', N'@@FRM@@', N'', NULL, N'IDMS - User Access Rights Review Task @@TASKID@@ (Delegated by - @@DELEGATED_USER@@) @@CAMPAIGN_NAME@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title></title>
        <style type="text/css">
            .title {
            font-family: "lucida grande", tahoma, verdana, arial, sans-serif;
            font-size: 13px;
            color: #1c2a47;
            font-weight: bolder;
            }
            .body {
            font-family: "lucida grande", tahoma, verdana, arial, sans-serif;
            font-size: 12px;
            color: InfoText;
            font-weight: normal;
            margin-left: 50px;
            }
        </style>
    </head>
    <body class="body" data-gr-c-s-loaded="true">
        <p><span style="font-family: ''Courier New''; font-size: 13px;">Dear @@REVIEWER_NAME@@,<br />
        <br />
        The following task has been delegated to you by @@DELEGATED_USER@@ and is pending your action.<br />
        <br />
        Delegation Comments: @@DELEGATER_COMMENTS@@<br />
        <br />
        Please perform online review via IDMS Portal for the user access rights listed<br />
        <br />
        Attestation Name<span style="white-space: pre;">	</span>: @@ATTESTATION_NAME@@<br />
        Attestation Description<span style="white-space: pre;">	</span>: @@ATTESTATION_DESC@@<br />
        Attestation CampaignName: @@CAMPAIGN_NAME@@<br />
        Due Date<span style="white-space: pre;">		</span>: @@REVIEWLASTDATE@@<br />
        <br />
        Scope of Works<br />
        Please conduct the review covering the following:<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; - Proper segregation of duties have been considered for users&rsquo; access rights<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; - Users are not granted rights beyond the requirements of their role<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; - Changes to users&rsquo; access rights are properly configured<br />
        <br />
        How to Review<br />
        Log in to IDMS&nbsp; @@IACMURL@@<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;1. Go to Home -&gt; My Task -&gt; Select the task to review&nbsp;<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;2. Click on "Detail" icon to start the review<br />
        &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;3. For each identified user, to validate that the access is still required and that the assigned role(s) is correct. If access is no longer required, click on ''Revoke'' to remove the access.<br />
        <br />
        Thank You.<br />
        <br />
        Regards<br />
        @@REGARDS@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***</span></p>
    </body>
</html>', N'IACMAdmin', CAST(N'2020-09-02T14:00:20.780' AS DateTime), N'iacmadmin', CAST(N'2020-09-04T11:14:09.617' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].DI_M_MailTemplate OFF
GO
INSERT [dbo].[DI_M_MailTemplate] ( [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent],  [CreatedDate],[CreatedBy],
[LastModifiedBy], [LastModifiedDate]) 
VALUES ( N'AttestationJobFailed', N'This is an email notification to IDMS ADMIN about attestation job failed with the error message.',
N'securityteam@spgroup.com.sg', N'securityteam@spgroup.com.sg', N'',Null, N'Attestation Job Failed - @@ENDPOINTNAME@@ ( @@PROCESSNAME@@ )', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title>IDMS - Attestation Job Failed</title>
        <style type="text/css">
            .title {            font-family: "lucida grande", tahoma, verdana, arial, sans-serif;            font-size: 13px;            color: #1c2a47;            font-weight: bolder;            }            .body {            font-family: "lucida grande", tahoma, verdana, arial, sans-serif;            font-size: 12px;            color: InfoText;            font-weight: normal;            margin-left: 50px;            }
        </style>
    </head>
    <body class="body" contenteditable="true">
        <span style="font-family: ''Courier New'';">Dear IAM Team,</span><span style="font-family: ''Courier New'';">               Attestation Process Failed
        </span>
        <p><span style="font-family: ''Courier New'';">        <strong>Error Message:</strong></span></p>
        <p><span style="font-family: ''Courier New'';" class="content">        @@ERRORMESSAGE@@</span></p>
        <p><span style="font-family: ''Courier New'';" class="title">        Endpoint system details:</span></p>
        <p><span class="title" style="font-family: ''Courier New'';">Application Type<span style="white-space: pre;">	</span>:<span style="white-space: pre;">	@@ENDPOINTTYPE@@</span></span></p>
        <p><span class="title" style="white-space: pre; font-family: ''Courier New'';">Application<span style="white-space: pre;">		</span>:<span style="white-space: pre;">	@@ENDPOINTNAME@@</span></span></p>
        <p><span class="title" style="white-space: pre; font-family: ''Courier New'';">Process Name<span style="white-space: pre;">	</span>:<span style="white-space: pre;">	@@PROCESSNAME@@</span></span></p>
        <p><span style="font-family: ''Courier New'';">Best regards,<br />
        @@REGARDS@@</span></p>
        <p><span style="font-family: ''Courier New'';">*** This is a system-generated message. Please do not reply to this message.***</span></p>
    </body>
</html>', SYSDATETIME(), N'IACMAdmin', null ,null)
GO
INSERT [dbo].[DI_M_MailTemplate] ( [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent],  [CreatedDate],[CreatedBy],
[LastModifiedBy], [LastModifiedDate]) 
VALUES ( N'NotifyAttestationWithDrawn', N'This is an email notification about attestation campaign with drawn.',
N'securityteam@spgroup.com.sg', N'securityteam@spgroup.com.sg', N'',Null, N'IDMS:Attestation Campaign(@@CAMPAIGNNAME@@) Withdrawn by @@WITHDRAWBY@@', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">  <html>      <head>          <title></title>          <style type="text/css">              .body {              font-family: Courier New;              font-size: 13px;              font-weight: normal;              margin-left: 5px;              }              .marlef {              margin-left: 40px;              }              .marlef1 {              margin-left: 50px;              }              .marlef2 {              margin-left: 50px;              }          </style>      </head>      <body class="body">          <p>          Dear @@APPROVER@@,<br />          <br />          The following Attestation Campaign has been withdrawn by @@WITHDRAWBY@@ on @@DATE@@<br />          <br />          <br />          Details:<br />          <br />          Attestation Campaign<span style="white-space: pre;"> </span>: @@CAMPAIGNNAME@@<br />          Withdraw Reason&nbsp; <span style="white-space: pre;"> </span>: @@WITHDRAWREASON@@<br />          <br />          Regards,<br />          @@REGARDS@@</p>          <p>          *** This is a system-generated message. Please do not reply to this message.***          </p>      </body>  </html>', SYSDATETIME(), N'IACMAdmin', null ,null)
