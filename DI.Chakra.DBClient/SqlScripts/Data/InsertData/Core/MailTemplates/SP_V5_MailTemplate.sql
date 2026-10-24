GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='AdhocProvisioningAmendment')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'AdhocProvisioningAmendment', N'This email is sent immediately after changes in application access due to: onboarding, ad-hoc requests, close-loop remedial actions, transfer, promotion, offboarding', N'devteam@deepidentity.com', N'itidadmin@rwsentosa.com', N'##REQ_BY_EMAIL##,itidadmin@rwsentosa.com', NULL, N'IDMS: @@REQUESTNUMBER@@ - Amendment Request for @@applicationName@@  Completed - @@USER@@  (@@EMPLOYEEID@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title>IDAS - Changes in user access rights for @@USERLASTNAME_REQ@@ @@USERFIRSTNAME_REQ@@</title>
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
    <body class="body">
        <p>Dear @@USER@@,&nbsp;<br />
        <br />
        Your access request has been completed.&nbsp;<br />
        <br />
        Request Details:<br />
        <br />
        Request ID: @@REQUESTNUMBER@@&nbsp;<br />
        Request Name: @@applicationName@@ - @@REQUESTTYPE@@&nbsp;<br />
        Request For: @@USER@@&nbsp;<br />
        Employee ID: @@EMPLOYEEID@@&nbsp;<br />
        Login ID: @@userLogonName@@&nbsp;</p>
        <p>Request By: @@RequestBy@@&nbsp;<br />
        Account Type: @@AccountType@@<br />
        Employee Type: @@EmployeeType@@&nbsp;<br />
        Comments: @@Comments@@&nbsp;</p>
        <p>Additional Information : @@accessName@@&nbsp;<br />
        <br />
        Regards,<br />
        @@Regards@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'IACMAdmin', CAST(N'2018-01-10T14:34:20.433' AS DateTime), N'IACMAdmin', CAST(N'2019-05-24T15:56:36.643' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='AdhocProvisioningUserRemoval')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'AdhocProvisioningUserRemoval', N'AdhocProvisioningUserRemoval', N'itidadmin@rwsentosa.com', N'@@MAILTO_REQ@@', N'', NULL, N'IDMS: @@REQUESTNUMBER@@ - Request for @@applicationName@@  Completed - @@USER@@  (@@EMPLOYEEID@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>Dear @@USER@@,&nbsp;<br />
        <br />
        Your access request has been completed.&nbsp;<br />
        <br />
        Request Details:<br />
        <br />
        Request ID: @@REQUESTNUMBER@@&nbsp;<br />
        Request Name: @@applicationName@@ - @@REQUESTTYPE@@&nbsp;<br />
        Request For: @@USER@@&nbsp;<br />
        Employee ID: @@EMPLOYEEID@@&nbsp;<br />
        Login ID: @@userLogonName@@&nbsp;</p>
        <p>Request By: @@RequestBy@@&nbsp;<br />
        Account Type: @@AccountType@@<br />
        Employee Type: @@EmployeeType@@&nbsp;<br />
        Comments: @@Comments@@&nbsp;</p>
        <p>
        Regards,<br />
        @@Regards@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-09T15:02:32.170' AS DateTime), N'IACMAdmin', CAST(N'2019-05-27T02:16:01.687' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='DeleteNonConnectedSystemNotification')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'DeleteNonConnectedSystemNotification', N'DeleteNonConnectedSystemNotificationOUChange', N'itidadmin@rwsentosa.com', N'@@CONFIGURATIONMASTER_MAILTO_ACCOUNTCREATED_USR@@', N'', NULL, N'IDAS - IDMS: OU Change Request - @@USERDISPLAYNAME_USR@@  ( @@PERNRNO_USR@@ )', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>Dear Security Operations,&nbsp;<br />
        <br />
        @@eventinfo@@&nbsp;<br />
        <br />
        User : @@userDisplayName@@ (@@pernrNum@@)&nbsp;<br />
        Company : @@coyCode@@ - @@coyText@@&nbsp;<br />
        Department : @@deptCode@@ - @@deptText@@&nbsp;<br />
        Division : @@divCode@@ - @@divText@@&nbsp;<br />
        Organisation : @@orgCode@@ - @@orgText@@&nbsp;<br />
        Cost Center : @@cctrCode@@ - @@cctrText@@&nbsp;<br />
        <br />
        @@applicationstring@@&nbsp;<br />
        <br />
        Regards,&nbsp;<br />
        Security Administrator&nbsp;<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-09T15:02:32.177' AS DateTime), N'IACMAdmin', CAST(N'2019-05-28T21:42:18.100' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='DiscoveryJobFailed')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'DiscoveryJobFailed', N'Discovery Job Failed', N'itidadmin@rwsentosa.com', N'@@MAILTO_DISC@@', N'', NULL, N'IDMS: Discovery Job Failed - @@ENDPOINTNAME@@ (@@PROCESSNAME@@) ', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title>IDAS - Discovery Job Failed</title>
        <style type="text/css">
            .style1
            {
            width: 285px;
            }
            .title
            {
            font-family:"lucida grande", tahoma, verdana, arial, sans-serif;
            font-size:13px;
            color:#1c2a47;
            font-weight:bold;
            }
            .content
            {
            padding-left:10px;
            font-family:"lucida grande", tahoma, verdana, arial, sans-serif;
            font-size:14px;
            color:#000;
            }
            .table_results td {
            border-right: 1px solid #F8F8F8;
            border-bottom: 1px solid #EEE;
            padding: 2px 2px;
            color: #000;
            }
            .table_results {
            font: normal 12px auto Verdana, Arial, Helvetica, sans-serif;
            color: #000;
            border-left: 1px solid #C1DAD7;
            }
        </style>
    </head>
    <body class="body" style="cursor: nw-resize;">
        <span style="white-space: pre;">	</span>Dear IAM Administrator,<br />
        <br />
        <span style="white-space: pre;">		</span>Discovery Process Failed<br />
        <br />
        <span style="white-space: pre;">		</span>Error Message:<br />
        <br />
        <span style="white-space: pre;">		</span>@@ERRORMESSAGE@@<br />
        <br />
        <span style="white-space: pre;">		</span>Endpoint system details:<br />
        <br />
        <span style="white-space: pre;">		</span>Application Type:<span style="white-space: pre;">	</span>@@ENDPOINTTYPE@@<br />
        <span style="white-space: pre;">		</span>Application:<span style="white-space: pre;">	</span>@@ENDPOINTNAME@@<br />
        <span style="white-space: pre;">		</span>Process Name:<span style="white-space: pre;">	</span>@@PROCESSNAME@@<br />
        <div>&nbsp;</div>
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-09T15:02:32.247' AS DateTime), N'IACMAdmin', CAST(N'2019-05-27T14:21:12.640' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='EscalateAdhocProvisioningApproval')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'EscalateAdhocProvisioningApproval', N'Escalate Adhoc Provisioning Approval', N'devteam@deepidentity.com', N'##MAILTO_MANAGER_TSK##', N'', NULL, N'IDMS: Adhoc Provisioning Approval Request - {0} - Escalation', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title>IDAS - Escalation: Application access change approval required for @@REQUESTEEFIRSTNAME@@ @@REQUESTEELASTNAME@@</title>
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
    <body class="body">
        <p>Dear @@RECEIVER@@,&nbsp;<br />
        This is a escalation email to inform you that @@REVIEWER_NAME@@ didn''t complete the provisioning approval process.&nbsp;<br />
        You are requested to take necessary steps to ensure the provisioning process is complete.&nbsp;<br />
        <br />
        Please use your Windows UserID and Password to login to IDMS to complete the task.&nbsp;<br />
        @@IACMURL@@<br />
        <br />
        Application: @@APPLICATION_NAME@@&nbsp;<br />
        Requester Name: @@REQUESTER@@&nbsp;<br />
        Date of Request: @@REQUESTEDDATE@@&nbsp;<br />
        Request: @@REQUEST_DETAIL@@&nbsp;<br />
        Last date for submission: @@REVIEWLASTDATE@@</p>
        <p>Request By: @@RequestBy@@&nbsp;<br />
        Account Type: @@AccountType@@<br />
        Employee Type: @@EmployeeType@@&nbsp;<br />
        Comments: @@Comments@@<br />
        <br />
        Regards&nbsp;<br />
        @@Regards@@</p>
    </body>
</html>', NULL, CAST(N'2017-11-15T14:07:42.397' AS DateTime), N'IACMAdmin', CAST(N'2019-05-28T21:24:30.453' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='EscalateAttestationApproval')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'EscalateAttestationApproval', N'Escalate Attestation Approval', N'devteam@deepidentity.com', N'##MAILTO_MANAGER_TSK##', NULL, NULL, N'IDAS - Escalation: Review of ##ATTTSK_ENDPOINTNAME_DISP## @@ATTE_TYPE@@ ##ATTE_TYPE_DESC## - @@ATTTSK_CAMPAIGN_NAME@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
 <body class="body">
    <div>
        Dear ##MANAGER_ATTTSK##,
      <br/>
      <br/>
        This is an escalation email to inform you that ##APPROVER_ATTTSK## didn&#39;t complete the attestation approval process.
      <br/>
<PRE>
1.	In compliance with the <u>User Access Management & Control Policy</u>, you are required to perform a review of the @@ATTE_TYPE@@ ##ATTE_TYPE_DESC## of ##ATTTSK_ENDPOINTNAME_DISP## as of @@CAMPIGN_START_DATE@@.
2.	Please follow the steps below to complete the review:
	a.	Logon to <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a>.
	b.	1. Go to Home -> My Task -> Select the task to review.
		2. Click on "Detail" icon to start the review
3.	This review shall be completed by @@CAMPIGN_END_DATE@@, and signed-off by your supervisor.		
</PRE>
   <br/>

<PRE>
<u>Task Details: </u>
Attestation Name: @@ATTESTATION_NAME_ATTTSK@@
Attestation Description: @@ATTESTATION_DESC_ATTTSK@@
Required Action: Attestation - Approval Level @@APPROVALLEVEL_ATTTSK@@
Last date for submission: ##REVIEWLASTDATE_ATTTSK##
</PRE>
      <br/>
 
<PRE>
You may contact the following support personnel should you require any clarification or assistance:
Subsystem Owner - ##ATT_EP_OWNER_DISPLAY_NAME##
Subsystem IT Lead - ##ATT_EP_ITLEAD_DISP_NAME##
IT Distribution Group - ##ATT_EP_IT_DIST_MAIL_ID##
</PRE>

        <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table> 
</div>
</body>
</html>', NULL, CAST(N'2017-11-15T14:11:58.900' AS DateTime), NULL, NULL)
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='AlertNotification')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'AlertNotification', N'AlertNotification', N'itidadmin@rwsentosa.com', N'@@CONFIGURATIONMASTER_MAILTO_ALERT_REQ@@', N'', NULL, N'IDMS: Validation Failed - @@event@@ - @@pernr@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p><span style="font-size: 14.6667px; white-space: pre-wrap; background-color: #ffffff; font-family: Calibri, ''Segoe UI'', Calibri, Thonburi, Arial, Verdana, sans-serif, ''Mongolian Baiti'', ''Microsoft Yi Baiti'', ''Javanese Text'';">Dear Security Operations,</span></p>
        <p><span style="font-size: 14.6667px; white-space: pre-wrap; background-color: #ffffff; font-family: Calibri, ''Segoe UI'', Calibri, Thonburi, Arial, Verdana, sans-serif, ''Mongolian Baiti'', ''Microsoft Yi Baiti'', ''Javanese Text'';">
        Provisioning mandatory information is missing for some of the default systems. Please see the detailed information below.</span></p>
        <p><span style="font-size: 14.6667px; white-space: pre-wrap; background-color: #ffffff; font-family: Calibri, ''Segoe UI'', Calibri, Thonburi, Arial, Verdana, sans-serif, ''Mongolian Baiti'', ''Microsoft Yi Baiti'', ''Javanese Text'';">
        User : @@USER@@ (@@pernrNum@@)
        Company : @@coyCode@@ - @@coyText@@
        Department : @@deptCode@@ - @@deptText@@
        Division : @@divCode@@ - @@divText@@
        Organisation : @@orgCode@@ - @@orgText@@
        Cost Center : @@cctrCode@@ - @@cctrText@@
        Error Info : @@MESSAGE@@</span></p>
        <p><span style="font-size: 14.6667px; white-space: pre-wrap; background-color: #ffffff; font-family: Calibri, ''Segoe UI'', Calibri, Thonburi, Arial, Verdana, sans-serif, ''Mongolian Baiti'', ''Microsoft Yi Baiti'', ''Javanese Text'';">
        Regards,</span></p>
        <p><span style="font-size: 14.6667px; white-space: pre-wrap; background-color: #ffffff; font-family: Calibri, ''Segoe UI'', Calibri, Thonburi, Arial, Verdana, sans-serif, ''Mongolian Baiti'', ''Microsoft Yi Baiti'', ''Javanese Text'';"></span><span style="background-color: #ffffff; font-size: 14.6667px; white-space: pre-wrap; font-family: Calibri, ''Segoe UI'', Calibri, Thonburi, Arial, Verdana, sans-serif, ''Mongolian Baiti'', ''Microsoft Yi Baiti'', ''Javanese Text''; color: infotext;">        Security Administrator</span></p>
        <p><span style="font-size: 14.6667px; white-space: pre-wrap; background-color: #ffffff; font-family: Calibri, ''Segoe UI'', Calibri, Thonburi, Arial, Verdana, sans-serif, ''Mongolian Baiti'', ''Microsoft Yi Baiti'', ''Javanese Text'';">        *** This is a system-generated message. Please do not reply to this message.***</span></p>
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-09T15:02:32.010' AS DateTime), N'IACMAdmin', CAST(N'2019-05-29T14:08:55.140' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='NotificationForNewUser')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'NotificationForNewUser', N'Notification for New User', N'itidadmin@rwsentosa.com', N'@@CONFIGURATIONMASTER_MAILTO_ACCOUNTCREATED_USR@@', N'', NULL, N'IDMS: Non-Employee User Profile Creation @@userDisplayName@@ (@@pernrNum@@) - Pending for your approval', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>Dear @@supervisorName@@,<br />
        <br />
        IDMS Non Employee Profile has been created and is awaiting your approval.<br />
        <br />
        User : @@userDisplayName@@ (@@pernrNum@@)&nbsp;<br />
        Company : @@coyText@@&nbsp;<br />
        Department : @@deptText@@&nbsp;<br />
        Division : @@divText@@&nbsp;<br />
        Organisation : @@orgText@@&nbsp;<br />
        Cost Center : @@cctrCode@@&nbsp;</p>
        <p>Reference User : @@ReferenceUser@@&nbsp;<br />
        Valid From: @@ValidFrom@@&nbsp;<br />
        Valid To: @@ValidTo@@&nbsp;<br />
        <br />
        <br />
        <br />
        Regards,&nbsp;<br />
        @@Regards@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-09T15:02:32.360' AS DateTime), N'IACMAdmin', CAST(N'2019-05-28T11:45:44.113' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='NotifyAdhocProvisioningApproval')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'NotifyAdhocProvisioningApproval', N'Notify Adhoc Provisioning Approval', N'itidadmin@rwsentosa.com', N'##MAILTO_ATTTSK##', N'@@REQUESTEE_MAILTO_REQ@@,##MAILTO_REQUESTEE_SUPERVISOR_USER##', NULL, N'IDMS: @@requestNum@@ - Request for @@applicationName@@ pending for your approval - @@userDisplayName@@ (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>Dear @@approverName@@,&nbsp;<br />
        <br />
        There is an access request for @@userDisplayName@@ pending for your approval.&nbsp;<br />
        <br />
        Request ID: @@requestNum@@&nbsp;<br />
        Request Name: @@applicationName@@ - @@requestType@@&nbsp;<br />
        Request For: @@userDisplayName@@&nbsp;<br />
        Employee ID: @@pernrNum@@&nbsp;<br />
        Account Type: @@AccountType@@&nbsp;<br />
        Reason for Request: @@COMMENTS@@&nbsp;<br />
        <br />
        Please use your Windows UserID and Password to login to IDMS to complete the task.&nbsp;<br />
        @@IACMURL@@&nbsp;<br />
        <br />
        Regards,&nbsp;<br />
        @@Regards@@&nbsp;<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-09T15:02:32.370' AS DateTime), N'IACMAdmin', CAST(N'2019-05-28T12:01:55.437' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='OffboardingApprover')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'OffboardingApprover', N'OffboardingApprover', N'itidadmin@rwsentosa.com', N'@@CONFIGURATIONMASTER_MAILTO_ACCOUNTCREATED_USR@@', N'', NULL, N'IDMS: Offboarding Approver - @@userDisplayName@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        Dear Security Operations,&nbsp;<br />
        <br />
        The following Approver has left service on @@terminationDate@@. Please change the following Configuration.&nbsp;<br />
        <br />
        User : @@userDisplayName@@ (@@pernrNum@@)&nbsp;<br />
        <br />
        Configuration : @@applicationstring@@ Group Name: @@GROUPNAME@@&nbsp;<br />
        <br />
        Regards,&nbsp;<br />
        Security Administrator&nbsp;<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-09T15:02:32.723' AS DateTime), N'IACMAdmin', CAST(N'2019-05-28T21:38:50.553' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='OffBoardingUserNotification')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'OffBoardingUserNotification', N'OffBoardingUserNotification', N'itidadmin@rwsentosa.com', N'##MAILTO_SUPERVISOR_USER##', N'', NULL, N'IDMS: Offboarding Request - @@userDisplayName@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        Dear Security Operations,&nbsp;<br />
        <br />
        The following user has left service on @@terminationDate@@. All the connected system accounts are deactivated.&nbsp;<br />
        <br />
        User : @@userDisplayName@@ (@@pernrNum@@)&nbsp;<br />
        Company : @@coyCode@@ - @@coyText@@&nbsp;<br />
        Department : @@deptCode@@ - @@deptText@@&nbsp;<br />
        Division : @@divCode@@ - @@divText@@&nbsp;<br />
        Organisation : @@orgCode@@ - @@orgText@@&nbsp;<br />
        Cost Center : @@cctrCode@@ - @@cctrText@@&nbsp;<br />
        <br />
        Regards,&nbsp;<br />
        @@Regards@@&nbsp;<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-09T15:02:32.727' AS DateTime), N'IACMAdmin', CAST(N'2019-05-28T21:30:49.773' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='OUChangeRequestNotification')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'OUChangeRequestNotification', N'OUChangeRequestNotification', N'itidadmin@rwsentosa.com', N'@@MAILTO_USR@@', N'', NULL, N'IDMS: Transfer Notification - @@userDisplayName@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>Dear @@userDisplayName@@,<br />
        <br />
        Please be informed your access has been updated based on your new organisation unit.&nbsp;<br />
        <br />
        Should you require any new access, please submit your access request via IDMS.<br />
        <br />
        @@IACMURL@@<br />
        <br />
        Regards,&nbsp;<br />
        @@Regards@@<br />
        <br />
        &nbsp;<br />
        <br />
        &nbsp;<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-09T15:02:32.740' AS DateTime), N'IACMAdmin', CAST(N'2019-05-28T12:15:51.017' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='RemindAttestationApproval')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'RemindAttestationApproval', N'Reminder Attestation Approval', N'devteam@deepidentity.com', N'##MAILTO_ATTTSK##', N'##ITCCEmail##', NULL, N'IDAS Reminder - Review of ##ATTTSK_ENDPOINTNAME_DISP## @@ATTE_TYPE@@ ##ATTE_TYPE_DESC## - @@ATTTSK_CAMPAIGN_NAME@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
    <body class="body">
        <p>Dear @@APPROVER_ATTTSK@@,
        <br/>     
		 <br />   
		Please be reminded to complete the ##ATTTSK_ENDPOINTNAME_DISP## @@ATTE_TYPE@@ ##ATTE_TYPE_DESC## Review. The following is a recap of our earlier email.
		<br/>  
<PRE>
1.	In compliance with the <u>User Access Management & Control Policy</u>, you are required to perform a review of the @@ATTE_TYPE@@ ##ATTE_TYPE_DESC## of ##ATTTSK_ENDPOINTNAME_DISP## as of @@CAMPIGN_START_DATE@@.
2.	Please follow the steps below to complete the review:
	a.	Logon to <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a>.
	b.	1. Go to Home -> My Task -> Select the task to review.
		2. Click on "Detail" icon to start the review
3.	This review shall be completed by @@CAMPIGN_END_DATE@@, and signed-off by your supervisor.		
</PRE>
<PRE>
You may contact the following support personnel should you require any clarification or assistance:
	Subsystem Owner - ##ATT_EP_OWNER_DISPLAY_NAME##
	Subsystem IT Lead - ##ATT_EP_ITLEAD_DISP_NAME##
	IT Distribution Group - ##ATT_EP_IT_DIST_MAIL_ID##
</PRE>
<br />

        <br />
        <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT Account Review</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table> </p>
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-14T17:31:28.017' AS DateTime), N'IACMAdmin', NULL)
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='AccessRequestFailureNotification')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'AccessRequestFailureNotification', N'Access Request Failure Notification', N'itidadmin@rwsentosa.com', N'@@CONFIGURATIONMASTER_MAILTO_REQ@@', N'', NULL, N'IDMS: @@RequestNum@@ - Execution Failed - @@APPLICATION@@  - @@RequestType@@ - @@pernr@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>Dear Security Operations,</p>
        <p >&nbsp; &nbsp; &nbsp; &nbsp; The following request has failed to execute. Please view the error details from the Log viewer.<br />
        &nbsp; &nbsp; &nbsp; &nbsp; Request Details:<br />
        &nbsp; &nbsp; &nbsp; &nbsp; Request ID: @@RequestNum@@<br />
        &nbsp; &nbsp; &nbsp; &nbsp; Request Name: @@APPLICATION@@ - @@RequestType@@<br />
        &nbsp; &nbsp; &nbsp; &nbsp; Request For: @@USERDISPLAYNAME@@<br />
        &nbsp; &nbsp; &nbsp; &nbsp; Employee ID: @@PERNRNO@@<br />
        &nbsp; &nbsp; &nbsp; &nbsp; Execution Failure Date: @@DATE@@<br />
        <br />
        <span style="white-space: pre;">	</span>Request By: @@RequestBy@@&nbsp;<br />
        <span style="white-space: pre;">	</span>Account Type: @@AccountType@@<br />
        <span style="white-space: pre;">	</span>Employee Type: @@EmployeeType@@&nbsp;<br />
        <span style="white-space: pre;">	</span>Comments: @@Comments@@&nbsp;<br />
        <br />
        <span style="white-space: pre;">	</span>Additional Information : @@accessName@@&nbsp;<br />
        <br />
        &nbsp; &nbsp; &nbsp; &nbsp; Regards,<br />
        &nbsp; &nbsp; &nbsp; &nbsp; Security Administrator<br />
        &nbsp; &nbsp; &nbsp; &nbsp; *** This is a system-generated message. Please do not reply to this message.***<br />
        <br />
        </p>
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-09T15:02:32.773' AS DateTime), N'IACMAdmin', CAST(N'2019-05-29T14:18:26.647' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='AccessRequestRejected')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'AccessRequestRejected', N'AccessRequestRejected', N'devteam@deepidentity.com', N'@@MAILTO_REQ@@', N'##MAILTO_SUPERVISOR##', NULL, N'IDMS: @@REQUESTID@@ - Request for @@ApplicationName@@ Rejected - @@USER@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title>IDAS - Rejected user access changes for @@USERLASTNAME_REQ@@ @@USERFIRSTNAME_REQ@@</title>
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
    <body class="body">
        <p>
        Dear @@USER@@,<br />
        <br />
        Your request has been rejected by @@APPROVER@@ on @@DATE@@<br />
        <br />
        <br />
        Request Details:<br />
        <br />
        Request ID: @@REQUESTID@@<br />
        Request Name: @@ApplicationName@@ - @@REQUESTTYPE@@<br />
        Request For: @@USER@@<br />
        Employee ID: @@pernrNum@@<br />
        Comments: @@approverComments@@</p>
        <p>Request By: @@RequestBy@@&nbsp;<br />
        Account Type: @@AccountType@@<br />
        Employee Type: @@EmployeeType@@&nbsp;<br />
        Comments: @@Comments@@<br />
        <br />
        Regards,<br />
        @@Regards@@&nbsp;<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
        </p>
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-09T18:54:49.470' AS DateTime), N'IACMAdmin', CAST(N'2019-05-28T21:24:57.963' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='CreateAccountNotificationCommonTemplate')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'CreateAccountNotificationCommonTemplate', N'Create account notification common template for all systems', N'securityadmin@spgroup.com.sg', N'test', N'', NULL, N'IDMS:@@REQUESTID@@ - Request for @@APPLICATIONNAME@@ Completed - @@USERDISPLAYNAME@@ (@@EMPLOYEEID@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jquery19105777307022835423="28" jquery191031130699542479323="28">
        <p>PRIVATE &amp; CONFIDENTIAL<br />
        -------------------------------------------------------------------<br />
        To:&nbsp;&nbsp;@@USERDISPLAYNAME@@(@@EMPLOYEEID@@ - @@EMPLOYEETYPE@@)<br />
        <div class="marlef">@@COMPANY@@<br />
        @@DEPT@@ @@DIVISION@@ @@BRANCH@@ <br />
        @@SECTION@@</div>
        </p>
        <p>Notification of @@APPLICATIONNAME@@ (@@APPLICATIONNAMEFULL@@) User ID and Password</p>
        <p>1. The following @@APPLICATIONNAME@@ ID has been created :-
        <div class="marlef">User ID : @@USERNAME@@<br />
        Password : @@PASSWORD@@<br />
        AccountType : @@ACCOUNTTYPE@@<br />
        @@RES1@@<br />
        @@RES2@@</div>
        </p>
        <p>2. Please perform the following steps:
        <div class="marlef">(i) Sign on to system using the user ID and password given above and immediately change the password to one of your choice before using the system.<br />
        (ii) Your password must have the following characteristics:<br />
        <div class="marlef1">1) Cannot contain any or all of the user name<br />
        2) At least 8 characters<br />
        3) Must contain (satisfy) at least 3 of the following criteria:<br />
        <div class="marlef2">- Uppercase English char (A to Z)<br />
        - Lowercase English char (a to z)<br />
        - Number 0 to 9<br />
        - Non-alphanumeric (or special character)</div>
        </div>
        </div>
        </p>
        <p>3. Please ensure that your password is kept confidential and that you do not grant permission to any person to use your User ID or password. Please also ensure that your terminal is not left unattended after you have signed on to the system </p>
        <p>4. Should you have any query, please do not hesitate to contact Helpdesk at 6378 8111.</p>
        <p ><br />
        Regards, <br />
        @@REGARDS@@</p>
        <p>&nbsp;</p>
        <p>*** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='OnboardingUserEmail')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'OnboardingUserEmail', N'OnboardingUserEmail', N'securityadmin@spgroup.com.sg', N'test', N'', NULL, N'IDMS: Request Completed - A1 - (@@EMPLOYEEID@@) @@APPLICATIONNAME@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jquery191045868327819865823="28" jquery19103030169706004316="28">
        <p>PRIVATE &amp; CONFIDENTIAL<br />
        -------------------------------------------------------------------<br />
        To:&nbsp;&nbsp;@@USERDISPLAYNAME@@<br />
        <div class="marlef">@@COMPANY@@<br />
        @@DEPT@@ @@DIVISION@@<br />
        @@BRANCH@@ @@SECTION@@</div>
        </p>
        <br />
        <br />
        <p>Notification of @@APPLICATIONNAME@@ @@APPLICATIONNAMEFULL@@ User ID and Password</p>
        <p>1. The following @@APPLICATIONNAME@@ ID has been created :-
        <div class="marlef">User ID&nbsp; &nbsp; &nbsp;: @@USERNAME@@<br />
        Password&nbsp; &nbsp; : @@PASSWORD@@<br />
        AccountType : @@ACCOUNTTYPE@@<br />
        @@RES1@@<br />
        @@RES2@@</div>
        </p>
        <p>2. Please perform the following steps:
        <div class="marlef">(i) Sign on to system using the user ID and password given above and immediately change the password to one of your choice before using the system.<br />
        (ii) Your password must have the following characteristics:<br />
        <div class="marlef1">1) Cannot contain any or all of the user name<br />
        2) At least 8 characters<br />
        3) Must contain (satisfy) at least 3 of the following criteria:<br />
        <div class="marlef2">- Uppercase English char (A to Z)<br />
        - Lowercase English char (a to z)<br />
        - Number 0 to 9<br />
        - Non-alphanumeric (or special character)</div>
        </div>
        </div>
        </p>
        <p>3. Please ensure that your password is kept confidential and that you do not grant permission to any person to use your User ID or password. Please also ensure that your terminal is not left unattended after you have signed on to the system. </p>
        <p>4.&nbsp;<span style="font-family: ''Courier New''; font-size: 13px; white-space: pre-wrap; background-color: #ffffff;">Should you have any query, please do not hesitate to contact IT Support desk at 6916 8111.</span></p>
        <p >
        Regards, <br />
        @@REGARDS@@&nbsp; &nbsp;&nbsp;</p>
        <p>*** This is a system-generated message. Please do not reply to this message.***</p>
        <p>
        <div>&nbsp;</div>
        </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='NotifyEmployeeEventTask')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'NotifyEmployeeEventTask', N'Notify Employee Event Task', N'securityadmin@spgroup.com.sg', N'empty@spgroup.com.sg', N'', NULL, N'IDMS: Non-Employee Creation/Extension/Shortening', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>
        Dear @@approverName@@,
        <br />
        <br />
        The following task is pending your approval.
        <br />
        <br />
        Request Name&nbsp; : @@requestType@@
        <br />
        Requested For : @@userDisplayName@@
        <br />
        Requested By&nbsp; : @@requesterDisplayName@@&nbsp;<br />
        Employee ID&nbsp; &nbsp;: @@pernrNum@@&nbsp;&nbsp;<br />
        Employee Type&nbsp;:&nbsp;@@EMPTYPE@@<br />
        Valid From&nbsp; &nbsp; :&nbsp;@@VALIDFROM@@<br />
        Valid To&nbsp; &nbsp; &nbsp; :&nbsp;@@VALIDTO@@<br />
        Reference User: @@REFUSER@@</p>
        <p ><span style="font-family: ''Courier New''; font-size: 13px; white-space: pre-wrap; background-color: #ffffff;">Please click the below link to log in to IDMS portal to action on the task.</span><br />
        @@IACMURL@@
        </p>
        <p >
        Regards,
        <br />
        @@REGARDS@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
        </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='RemindEmployeeEventApproval')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'RemindEmployeeEventApproval', N'Remind Employee Event Approval', N'securityadmin@spgroup.com.sg', N'empty@spgroup.com.sg', N'', NULL, N'IDMS:Reminder for Non Employee Creation/Extension/Shortening approval', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>Dear @@RECEIVER@@,<br />
        <br />
        This is a gentle reminder regarding the pending approval process. You are requested to review the below request using IDMS
        <br />
        <br />
        <br />
        Request Name&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;: @@requestType@@
        <br />
        Requested For&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; : @@userDisplayName@@
        <br />
        Requested By&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;: @@requesterDisplayName@@
        <br />
        Employee Type&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; : @@EMPTYPE@@
        <br />
        Date of Request&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; : @@REQUESTEDDATE@@&nbsp;<br />
        Last date for submission : @@REVIEWLASTDATE@@&nbsp;<br />
        <br />
        Please&nbsp;<strong><span style="text-decoration-line: underline; color: blue;">use your Windows UserID and Password to</span></strong>&nbsp;login to IDMS&nbsp;to complete&nbsp;the task.&nbsp;<br />
        @@IACMURL@@&nbsp;<br />
        <br />
        <br />
        Regards,&nbsp;<br />
        @@REGARDS@@&nbsp;&nbsp;</p>
        <p>*** This is a system-generated message. Please do not reply to this message.***</p>
        <p>
        </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='NotifyEmployeeEventTaskApproved')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'NotifyEmployeeEventTaskApproved', N'Notify Employee Event Task Approved', N'securityadmin@spgroup.com.sg', N'empty@spgroup.com.sg', N'', NULL, N'IDMS: Non-Employee Creation/Extension/Shortening Request Approved', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>
        Dear @@requesterDisplayName@@,
        <br />
        <br />
        Your request for Non Employee Creation / Extension / Shortening has been approved by @@APPROVER@@.
        <br />
        <br />
        Request Name&nbsp; : @@requestType@@
        <br />
        Requested By&nbsp; :&nbsp;@@requesterDisplayName@@<br />
        Requested For : @@userDisplayName@@(@@pernrNum@@)&nbsp; &nbsp;<br />
        Employee Type : @@EMPTYPE@@&nbsp;&nbsp;&nbsp;<br />
        Comments&nbsp; &nbsp; &nbsp; : @@COMMENTS@@</p>
        <p><span style="white-space: pre;">	</span><br />
        Regards,
        <br />
        @@REGARDS@@&nbsp; &nbsp; &nbsp; &nbsp;&nbsp;<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
        </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='NotifyEmployeeEventTaskRejected')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'NotifyEmployeeEventTaskRejected', N'Notify Employee Event Task Rejected', N'securityadmin@spgroup.com.sg', N'empty@spgroup.com.sg', N'', NULL, N'IDMS:@@REQUESTNUM@@ - Non-Employee Creation/Extension/Shortening Request Rejected', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>
        Dear @@requesterDisplayName@@,
        <br />
        <br />
        Your request for Non Employee Creation / Extension / Shortening has been rejected by @@REJECTEDBY@@.
        <br />
        <br />
        Request Name&nbsp; &nbsp; &nbsp;: @@requestType@@&nbsp;<br />
        Employee EventId :&nbsp;@@REQUESTNUM@@<br />
        Requested For&nbsp; &nbsp; : @@userDisplayName@@&nbsp;<br />
        Requested By&nbsp; &nbsp; &nbsp;: @@REQUESTER@@<br />
        Employee ID&nbsp; &nbsp; &nbsp; :&nbsp;@@pernrNum@@<br />
        Employee Type&nbsp; &nbsp; :&nbsp;@@EMPTYPE@@<br />
        Reason&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;: @@REASON@@<br />
        <br />
        Regards,
        <br />
        @@REGARDS@@&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
        </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='TriggerDeletedNotification')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'TriggerDeletedNotification', N'Trigger Deleted Notification', N'securityadmin@spgroup.com.sg', N'empty@spgroup.com.sg', N'', NULL, N'IDMS: Trigger missing in HR system - @@event@@ - @@USER@@(@@pernr@@) - @@effectivedate@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>Dear Security Operations,</p>
        <p>HR trigger is missing for the below user.
        </p>
        <p>User&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;: @@USER@@  (@@pernrNum@@)<br />
        Company&nbsp; &nbsp; &nbsp; &nbsp; : @@coyCode@@ - @@coyText@@<br />
        Department&nbsp; &nbsp; &nbsp;: @@deptCode@@ - @@deptText@@<br />
        Division&nbsp; &nbsp; &nbsp; &nbsp;: @@divCode@@ - @@divText@@<br />
        Organisation&nbsp; &nbsp;: @@orgCode@@ - @@orgText@@<br />
        Cost Center&nbsp; &nbsp; : @@cctrCode@@ - @@cctrText@@<br />
        <br />
        </p>
        <p>Regards,<br />
        @@REGARDS@@</p>
        <p>        *** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='RequestWithdrawn')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'RequestWithdrawn', N'RequestWithdrawn', N'securityadmin@spgroup.com.sg', N'empty@spgroup.com.sg', N'', NULL, N'IDMS:@@REQUESTID@@ - Request Withdrawn - @@ApplicationName@@ - @@REQUESTTYPE@@ - @@REQUESTID@@ - @@USER@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>
        Dear @@APPROVER@@,<br />
        <br />
        The following request has been withdrawn by @@REQUESTER@@ on @@DATE@@<br />
        <br />
        <br />
        Request Details:<br />
        <br />
        Request ID&nbsp; &nbsp; &nbsp; &nbsp;: @@REQUESTID@@<br />
        Request Name&nbsp; &nbsp; &nbsp;: @@ApplicationName@@ - @@REQUESTTYPE@@<br />
        Request For&nbsp; &nbsp; &nbsp; : @@USER@@<br />
        Request By&nbsp; &nbsp; &nbsp; &nbsp;:&nbsp;@@REQUESTER@@<br />
        Employee Type&nbsp; &nbsp; : @@EMPTYPE@@<br />
        Employee ID&nbsp; &nbsp; &nbsp; : @@pernrNum@@<br />
        Withdraw Reason&nbsp; : @@REASON@@<br />
        <br />
        Regards,<br />
        @@REGARDS@@</p>
        <p>
        *** This is a system-generated message. Please do not reply to this message.***
        </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='NonEmployeeExpiryNotification')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'NonEmployeeExpiryNotification', N'Non Employee Expiry Notification', N'securityadmin@spgroup.com.sg', N'empty@spgroup.com.sg', N'', NULL, N'IDMS: Non Employee Expiry Notification - @@userDisplayName@@ - @@pernr@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>Dear @@approver@@,
        <br />
        <br />
        User (@@userDisplayName@@) is going to expire soon. Please see the details below.<br />
        <br />
        <br />
        User Details:<br />
        <br />
        User Name&nbsp; &nbsp; &nbsp;: @@userDisplayName@@<br />
        Employee ID&nbsp; &nbsp;: @@pernr@@<br />
        Employee Type : @@EMPTYPE@@<br />
        Expiry Days&nbsp; &nbsp;: @@expiry_days@@<br />
        Expiry Date&nbsp; &nbsp;: @@expiry_date@@<br />
        <br />
        Please&nbsp;<strong><span style="text-decoration-line: underline; color: blue;">use your Windows UserID and Password to</span></strong>&nbsp;login to IDMS&nbsp;to complete&nbsp;the task.&nbsp;<br />
        @@IACMURL@@&nbsp;<br />
        <br />
        <br />
        Regards,
        <br />
        @@REGARDS@@&nbsp; &nbsp; &nbsp; &nbsp;&nbsp;<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='RemediationComplete')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'RemediationComplete', N'RemediationComplete', N'securityadmin@spgroup.com.sg', NULL, NULL, NULL, N'IDMS: Remediation Request Completed - @@applicationName@@  - @@REQUESTTYPE@@ - @@REQUESTNUMBER@@ - @@USER@@  (@@USERID@@) - (@@userLogonName@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
  <body class="body">

   <p> Dear @@USER@@,  
   <br/>  
    <br/>

    The following access has been removed as part of attestation remediation process.
    <br/>
    <br/>
      Request Details:<br/>
    --------------------------------------------------------------------
	<br/>
    <br/>

    Request ID: @@REQUESTNUMBER@@
	<br/>
      Request Name: @@applicationName@@ - @@REQUESTTYPE@@
	  <br/>
      Request For: @@USER@@
	  <br/>
      Employee ID: @@EMPLOYEEID@@
	  <br/>
    Login ID: @@userLogonName@@
	<br/>
	<br/>

    Access Name : @@accessName@@
	<br/>
    <br/>

    Regards,<br/>

    IDMS Administrator.<br/>
    <br/>    
      *** This is a system-generated message. Please do not reply to this message.***   </p>
  </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='EmployeeEventWithdrawn')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'EmployeeEventWithdrawn', N'EmployeeEventWithdrawn', N'securityadmin@spgroup.com.sg', N'norequired@spgroup.com.sg', N'', NULL, N'IDMS:@@REQUESTNUM@@ - Request Withdrawn - Employee Creation/Extension/Shortening', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>
        Dear @@approverName@@,<br />
        <br />
        The following request has been withdrawn by @@requesterDisplayName@@ on @@DATE@@<br />
        <br />
        <br />
        Request Details:<br />
        <br />
        Request Name&nbsp; &nbsp;: @@requestType@@
        <br />
        Request Id&nbsp; &nbsp; &nbsp;:&nbsp;@@REQUESTNUM@@<br />
        Requested For&nbsp; : @@userDisplayName@@&nbsp;<br />
        Requested By&nbsp; &nbsp;: @@requesterDisplayName@@
        <br />
        Employee Type&nbsp; : @@EMPTYPE@@&nbsp;<br />
        &nbsp;<br />
        <br />
        Please use your Windows UserID and Password to login to IDMS to complete the task.&nbsp;<br />
        @@IACMURL@@&nbsp;<br />
        <br />
        Regards,<br />
        @@REGARDS@@</p>
        <p>
        *** This is a system-generated message. Please do not reply to this message.***
        </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='BulkRequestWithdrawn')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'BulkRequestWithdrawn', N'BulkRequestWithdrawn', N'securityadmin@spgroup.com.sg', N'empty@spgroup.com.sg', N'', NULL, N'IDMS:@@REQUESTID@@ - Bulk Request Withdrawn - @@ApplicationName@@ - @@REQUESTTYPE@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>
        Dear @@APPROVER@@,<br />
        <br />
        The following request has been withdrawn by @@REQUESTER@@ on @@DATE@@<br />
        <br />
        <br />
        Request Details:<br />
        <br />
        Request ID&nbsp; &nbsp; &nbsp; &nbsp; : @@REQUESTID@@<br />
        Request Name&nbsp; &nbsp; &nbsp; : @@ApplicationName@@ - @@REQUESTTYPE@@<br />
        Request For&nbsp; &nbsp; &nbsp; &nbsp;: @@USER@@<br />
        Employee ID&nbsp; &nbsp; &nbsp; &nbsp;: @@pernrNum@@<br />
        Requested By&nbsp; &nbsp; &nbsp; :&nbsp;@@REQUESTER@@<br />
        Withdraw Reason&nbsp; &nbsp;: @@WITHDRAWREASON@@<br />
        <br />
        <br />
        <br />
        Regards,<br />
        @@REGARDS@@&nbsp; &nbsp; &nbsp;&nbsp;</p>
        <p >
        *** This is a system-generated message. Please do not reply to this message.***
        </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='RemindBulkRequestApproval')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'RemindBulkRequestApproval', N'RemindBulkRequestApproval', N'securityadmin@spgroup.com.sg', N'test', N'', NULL, N'IDMS:@@requestNum@@ - Bulk Request for @@applicationName@@ pending for your approval', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jquery1910061058624796245076="33" jquery19108204754305343215="33" jquery191010259010248692357="28" jquery19107970852199999311="28" jquery191005408647152655671="33">
        Dear @@approverName@@, <br />
        <br />
        There is an access request for&nbsp;@@userDisplayName@@ pending your approval. <br />
        <br />
        Request ID&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;: @@requestNum@@ <br />
        Request Name&nbsp; &nbsp; &nbsp; &nbsp;: @@applicationName@@ - @@requestType@@ <br />
        Request For&nbsp; &nbsp; &nbsp; &nbsp; : @@userDisplayName@@ <br />
        Request By&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; : @@REQUESTBY@@ <br />
        Employee ID&nbsp; &nbsp; &nbsp; &nbsp; : @@pernrNum@@&nbsp;<br />
        Account Type&nbsp; &nbsp; &nbsp; &nbsp;: @@AccountType@@&nbsp;<br />
        Employee Type&nbsp; &nbsp; &nbsp; : @@EMPTYPE@@&nbsp;<br />
        Reason for Request : @@COMMENTS@@&nbsp;<br />
        <br />
        Please <strong><span style="text-decoration: underline; color: blue;">use your Windows UserID and Password to</span></strong> login to IDMS&nbsp;to complete&nbsp;the task. <br />
        @@IACMURL@@ <br />
        <br />
        Regards,&nbsp;<br />
        @@REGARDS@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='NotifyBulkRequestApproval')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'NotifyBulkRequestApproval', N'NotifyBulkRequestApproval', N'securityadmin@spgroup.com.sg', N'test', N'', NULL, N'IDMS:@@requestNum@@ - Bulk Request for @@applicationName@@ pending for your approval', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jquery1910061058624796245076="33" jquery19108204754305343215="33" jquery191010259010248692357="28" jquery19107970852199999311="28" jquery191005408647152655671="33">
        Dear @@approverName@@, <br />
        <br />
        There is an access request for&nbsp;@@userDisplayName@@ pending your approval. <br />
        <br />
        Request ID&nbsp; &nbsp; &nbsp; &nbsp; : @@requestNum@@ <br />
        Request Name&nbsp; &nbsp; &nbsp; : @@applicationName@@ - @@requestType@@ <br />
        Request For&nbsp; &nbsp; &nbsp; &nbsp;: @@userDisplayName@@ <br />
        Employee ID&nbsp; &nbsp; &nbsp; &nbsp;: @@pernrNum@@ <br />
        Account Type&nbsp; &nbsp; &nbsp; : @@AccountType@@ <br />
        Employee Type&nbsp; &nbsp; &nbsp;: @@EMPTYPE@@&nbsp;<br />
        Reason for Request: @@COMMENTS@@&nbsp;<br />
        <br />
        Please <strong><span style="text-decoration: underline; color: blue;">use your Windows UserID and Password to</span></strong> login to IDMS&nbsp;to complete&nbsp;the task. <br />
        @@IACMURL@@ <br />
        <br />
        Regards,&nbsp;<br />
        @@REGARDS@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='RemindAdhocProvisioningApproval')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'RemindAdhocProvisioningApproval', N'Reminder Adhoc Provisioning Approval', N'securityadmin@spgroup.com.sg', N'test', N'', NULL, N'IDMS: @@requestNum@@ - Request for @@applicationName@@ pending for your approval - @@userDisplayName@@ (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jquery19109055355848943497="33" jquery191009384992435010081="28" jquery19105115917409423485="28">
        <p>
        Dear @@approverName@@,<br />
        <br />
        There is an access request for @@userDisplayName@@ pending for your approval.&nbsp;<br />
        <br />
        Request ID&nbsp; &nbsp; &nbsp; &nbsp; : @@requestNum@@ <br />
        Request Name&nbsp; &nbsp; &nbsp; : @@applicationName@@ - @@requestType@@ <br />
        Request For&nbsp; &nbsp; &nbsp; &nbsp;: @@userDisplayName@@ <br />
        Employee ID&nbsp; &nbsp; &nbsp; &nbsp;: @@pernrNum@@ <br />
        Employee Type&nbsp; &nbsp; &nbsp;: @@EMPTYPE@@&nbsp;<br />
        Request&nbsp; By&nbsp; &nbsp; &nbsp; &nbsp;: @@REQUESTBY@@&nbsp;<br />
        Account Type&nbsp; &nbsp; &nbsp; : @@AccountType@@&nbsp;<br />
        Reason for Request: @@COMMENTS@@&nbsp;</p>
        <p>
        Please use your Windows UserID and Password to login to IDMS to complete the task.&nbsp;<br />
        @@IACMURL@@&nbsp;<br />
        <br />
        <br />
        Regards&nbsp;<br />
        Security Administrator
        </p>
        <p><span style="font-family: ''Courier New''; font-size: 13px;">*** This is a system-generated message. Please do not reply to this message.***</span></p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='BulkRequestRejected')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'BulkRequestRejected', N'BulkRequestRejected', N'securityadmin@spgroup.com.sg', N'test', N'', NULL, N'IDMS:@@REQUESTID@@ - Bulk Request for @@ApplicationName@@ Rejected', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jquery19108462597709516246="28">
        <p>
        Dear @@USER@@,<br />
        <br />
        Your&nbsp;request has been rejected by @@APPROVER@@ on @@DATE@@<br />
        <br />
        <br />
        Request Details:<br />
        <br />
        Request ID&nbsp; &nbsp;: @@REQUESTID@@<br />
        Request Name :&nbsp;@@ApplicationName@@ - @@REQUESTTYPE@@<br />
        Request By&nbsp; &nbsp;:&nbsp;@@RequestBy@@<br />
        Request For&nbsp; : @@USER@@<br />
        Employee Type: @@EMPTYPE@@<br />
        Comments&nbsp; &nbsp; &nbsp;:&nbsp;@@Comments@@</p>
        <p>
        Regards,<br />
        @@REGARDS@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
        </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='RequestRejectAlertToApprovers')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'RequestRejectAlertToApprovers', N'RequestRejectAlertToApprovers', N'securityadmin@spgroup.com.sg', N'test', N'', NULL, N'IDMS:@@REQUESTID@@ - Request Rejected - @@ApplicationName@@ - @@REQUESTTYPE@@ -  @@USER@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>
        Dear @@APPROVER@@,<br />
        <br />
        The following request has been rejected by @@REJECTEDUSER@@ on @@DATE@@<br />
        Hence, it has been removed from your task list.<br />
        <br />
        <br />
        Request Details:<br />
        <br />
        Request ID&nbsp; &nbsp;: @@REQUESTID@@<br />
        Request Name : @@ApplicationName@@ - @@REQUESTTYPE@@<br />
        Request For&nbsp; : @@USER@@<br />
        Request By&nbsp; &nbsp;: @@REQUESTBY@@<br />
        Account Type : @@ACCOUNTTYPE@@<br />
        Employee Type: @@EMPTYPE@@<br />
        Reason&nbsp; &nbsp; &nbsp; &nbsp;: @@REASON@@<br />
        Employee ID&nbsp; : @@pernrNum@@<br />
        <br />
        <br />
        Regards,<br />
        @@REGARDS@@</p>
        <p>
        *** This is a system-generated message. Please do not reply to this message.***
        </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='BulkRequestRejectAlertToApprovers')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'BulkRequestRejectAlertToApprovers', N'BulkRequestRejectAlertToApprovers', N'securityadmin@spgroup.com.sg', N'test', N'', NULL, N'IDMS:@@REQUESTID@@ - Bulk Request Rejected - @@ApplicationName@@ - @@REQUESTTYPE@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>
        Dear @@APPROVER@@,<br />
        <br />
        The following request has been rejected by @@REJECTEDUSER@@ on @@DATE@@<br />
        Hence, it has been removed from your task list.<br />
        <br />
        Request Details:<br />
        <br />
        Request ID&nbsp; &nbsp; &nbsp; : @@REQUESTID@@<br />
        Request Name&nbsp; &nbsp; : @@ApplicationName@@ - @@REQUESTTYPE@@<br />
        Request For&nbsp; &nbsp; &nbsp;: @@USER@@<br />
        Request By&nbsp; &nbsp; &nbsp; :&nbsp;@@REQUESTBY@@<br />
        Account Type&nbsp; &nbsp; :&nbsp;@@ACCOUNTTYPE@@<br />
        Employee Type&nbsp; &nbsp;:&nbsp;@@EMPLOYEETYPE@@<br />
        Rejection Reason:&nbsp;@@REJECTIONREASON@@<br />
        <br />
        Regards,<br />
        @@REGARDS@@</p>
        <p>&nbsp;</p>
        <p>*** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='NotifyReferenceUserChange')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'NotifyReferenceUserChange', N'NotifyReferenceUserChange', N'securityadmin@spgroup.com.sg', N'test', N'', NULL, N'IDMS: Reference User - @@eventtype@@ - @@userDisplayName@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jquery19103433693516084604="28" jquery19107719995960077419="28">
        Dear All, <br />
        <br />
        @@eventinfo@@ <br />
        <br />
        User&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; : @@userDisplayName@@ (@@pernrNum@@) <br />
        Employee Type : @@EMPTYPE@@ <br />
        Company&nbsp; &nbsp; &nbsp; &nbsp;: @@coyCode@@ - @@coyText@@&nbsp;<br />
        Department&nbsp; &nbsp; : @@deptCode@@ - @@deptText@@&nbsp;<br />
        Division&nbsp; &nbsp; &nbsp; : @@divCode@@ - @@divText@@&nbsp;<br />
        Organisation&nbsp; : @@orgCode@@ - @@orgText@@&nbsp;<br />
        Cost Center&nbsp; &nbsp;: @@cctrCode@@ - @@cctrText@@&nbsp;<br />
        <br />
        @@applicationstring@@ <br />
        <br />
        Regards,&nbsp;<br />
        @@REGARDS@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='NotifyOffice365Failed')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'NotifyOffice365Failed', N'Notify Office365 Failed Notification', N'securityadmin@spgroup.com.sg', N'empty@spgroup.com.sg', N'', NULL, N'IDMS:@@REQUESTID@@ - O365 License assignment for @@UPN@@ failed.', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>Dear Security Operations,</p>
        <p>O365 License assignment is failed 5 times for the user @@UPN@@. Please assign it manually.
        </p>
        <p>Request ID&nbsp; &nbsp;:&nbsp;@@REQUESTID@@<br />
        Request By&nbsp; &nbsp;:&nbsp;@@REQUESTBy@@<br />
        Request For&nbsp; :&nbsp;@@REQUESTFOR@@&nbsp;</p>
        <p>Regards,<br />
        @@REGARDS@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='PassOfficeNotification')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'PassOfficeNotification', N'Pass Office Onboard Notification', N'securityadmin@spgroup.com.sg', N'empty@spgroup.com.sg', N'', NULL, N'IDMS: SMS Onboard Notification - (@@EMPLOYEEID@@) @@DISPLAYNAME@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>Dear Pass Office Team,</p>
        <br />
        <p>IDMS has created a new account in SMS system and assigned a dummy badge.
        Please assign the actual badge based on the below user details.
        </p>
        <p>User&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; : @@USER@@  (@@pernrNum@@)<br />
        Company&nbsp; &nbsp; &nbsp; &nbsp;: @@coyCode@@ - @@coyText@@<br />
        Employee Type : @@EMPTYPE@@<br />
        Department&nbsp; &nbsp; : @@deptCode@@ - @@deptText@@<br />
        Division&nbsp; &nbsp; &nbsp; : @@divCode@@ - @@divText@@<br />
        Organisation&nbsp; : @@orgCode@@ - @@orgText@@<br />
        Cost Center&nbsp; &nbsp;: @@cctrCode@@ - @@cctrText@@</p>
        <p>Regards,<br />
        IDMS Administrator.
        </p>
        <p>&nbsp;</p>
        <p>        *** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='PassOfficeOffBoardNotification')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'PassOfficeOffBoardNotification', N'Pass Office Offboard Notification', N'securityadmin@spgroup.com.sg', N'empty@spgroup.com', N'', NULL, N'IDMS: SMS Offboard Notification -  @@DISPLAYNAME@@(@@EMPLOYEEID@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>Dear Pass Office Team,</p>
        <br />
        <p>IDMS has locked the account of below user in SMS system.
        </p>
        <p>User&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; : @@USER@@  (@@pernrNum@@)<br />
        Company&nbsp; &nbsp; &nbsp; &nbsp;: @@coyCode@@ - @@coyText@@<br />
        Employee Type :&nbsp;@@EMPTYPE@@<br />
        Department&nbsp; &nbsp; : @@deptCode@@ - @@deptText@@<br />
        Division&nbsp; &nbsp; &nbsp; : @@divCode@@ - @@divText@@<br />
        Organisation&nbsp; : @@orgCode@@ - @@orgText@@<br />
        Cost Center&nbsp; &nbsp;: @@cctrCode@@ - @@cctrText@@<br />
        Reason&nbsp; &nbsp; &nbsp; &nbsp; : Off-Boarding</p>
        <p>Best Regards,<br />
        @@REGARDS@@</p>
        <span style="font-family: ''Courier New''; font-size: 13px; color: infotext;">*** This is a system-generated message. Please do not reply to this message.***</span>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='NotifyAttestationComplete')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'NotifyAttestationComplete', N'Notify Attestation Complete', N'securityadmin@spgroup.com.sg', N'test', NULL, NULL, N'IDMS: Attestation - {0} Completed', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
    <body class="body">
        <p>Dear Security Operations,
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
        IDMS Administrator
        <br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.*** </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='SRMSupportTeamNotification')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'SRMSupportTeamNotification', N'SRM Support Team Notification', N'securityadmin@spgroup.com.sg', N'test', N'', NULL, N'IDMS:@@REQUESTID@@ - SRM @@REQUESTTYPE@@ Request Completed - @@USER@@ (@@USERID@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body">
        <p>Dear SRM Support Team,
        <br />
        <br />
        An SRM access request has been completed. Please assign the user to the SRM Organisation Structure accordingly.
        <br />
        <br />
        Request Details:<br />
        <br />
        Request ID&nbsp; &nbsp;: @@REQUESTNUMBER@@ <br />
        Request Type :&nbsp;@@REQUEST_TYPE@@<br />
        Request Name : SAP SRM&nbsp;&nbsp;<br />
        Request For&nbsp; :&nbsp;@@USER@@<br />
        Request By&nbsp; &nbsp;:&nbsp;@@REQUESTBY@@<br />
        Account Type :&nbsp;@@ACCOUNTTYPE@@<br />
        Employee Type:&nbsp;@@EMPLOYEETYPE@@<br />
        Comment&nbsp; &nbsp; &nbsp; :&nbsp;@@COMMENTS@@<br />
        SRM Login ID : @@userLogonName@@<br />
        <br />
        Requested Roles &nbsp;: <br />
        @@accessName@@ <br />
        <br />
        <br />
        Regards
        <br />
        @@REGARDS@@<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.*** </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='NotifyForNewResponseGroup')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'NotifyForNewResponseGroup', N'Create account notification skype for business template for all systems', N'securityadmin@spgroup.com.sg', N'test', N'', NULL, N'@@ENDPOINTNAME@@  - New ResponseGroup(s) Discovered', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title></title>
        <style type="text/css">
            .body {              font-family: Courier New;              font-size: 13px;              font-weight: normal;              margin-left: 5px;              }              .marlef {              margin-left: 40px;              }              .marlef1 {              margin-left: 50px;              }              .marlef2 {              margin-left: 50px;              }
        </style>
    </head>
    <body class="body" jquery191031130699542479323="28" jquery19105777307022835423="28" _events="[object Object]">
        <p style="margin-left: 40px;"><span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: #000000;">Dear Email Team,</span></p>
        <p style="margin-left: 40px;"><span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: #000000;">New Response Group(s) found during @@ENDPOINTNAME@@ discovery. Please find the below details.</span></p>
        <p style="margin-left: 40px;"><strong>Response Group(s)</strong></p>
        <p style="margin-left: 40px;">@@RESPONSEGROUPNAMES@@</p>
        <p style="margin-left: 40px;"><strong>Endpoint system details:</strong><br />
        &nbsp;&nbsp;&nbsp; <br />
        Application Type :&nbsp;@@ENDPOINTTYPE@@<br />
        Application&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :&nbsp;@@ENDPOINTNAME@@</p>
        <p style="margin-left: 40px;"><span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: #000000;"><br />
        </span></p>
        <p style="margin-left: 40px;"><span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: infotext;">Please use your Windows UserID and Password to login to IDMS to update <span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: #000000;">the UPN(ADID) for the respective response group(s).</span></span></p>
        <p style="font-family: arial; margin-left: 40px;">&nbsp;<span style="font-size: small; white-space: pre-wrap; background-color: #ffffff; font-family: ''Courier New''; color: infotext;">@@IACMURL@@ </span></p>
        <p style="font-family: arial; margin-left: 40px;"><span style="text-align: left; text-transform: none; text-indent: 0px; letter-spacing: normal; font-style: normal; font-weight: 400; word-spacing: 0px; float: none; white-space: pre-wrap; orphans: 2; widows: 2; background-color: #ffffff; text-decoration-color: initial; font-variant-ligatures: normal; font-variant-caps: normal; -webkit-text-stroke-width: 0px; text-decoration-style: initial; display: inline !important; font-family: ''Courier New''; font-size: 13px; color: #000000;">&nbsp;</span></p>
        <p style="margin-left: 40px;">Best regards,</p>
        <p style="margin-left: 40px;">@@REGARDS@@</p>
        <p style="margin-left: 40px;"><span style="font-family: ''Courier New''; font-size: 13px; color: infotext;">&nbsp;</span></p>
        <p style="margin-left: 40px;"><span style="font-family: ''Courier New''; font-size: 13px; color: infotext;">*** This is a system-generated message. Please do not reply to this message.***</span></p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='AdvanceDelegation')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate]) VALUES (N'AdvanceDelegation', N'This email is sent to the delegated user during out of office setup.', N'securityadmin@spgroup.com.sg', N'@@MAILTO_DELTSK@@', N'', NULL, N'IDMS - Advance Delegation - (@@FROM_DELETAK@@ - @@TO_DELETAK@@) Delegated by - @@ADDELEGATED_USER@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <meta charset="UTF-8" />
        <title>IDAS - Advance Delegation</title>
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
    <body class="body" spellcheck="false">
        <p style="font-family: arial;"><span style="font-family: ''Courier New''; font-size: 13px;">Dear @@APPROVER_DELTSK@@,</span></p>
        <p style="font-family: arial;"><span style="white-space: pre-wrap; background-color: #ffffff; font-family: ''Courier New''; font-size: 13px;">There is an Advance delegation created By @@ADDELEGATED_USER@@&nbsp;<span style="font-family: ''Courier New'';">@@ModifiedByText@@</span>&nbsp;</span></p>
        <p style="font-family: arial;"><span style="white-space: pre-wrap; background-color: #ffffff; font-family: ''Courier New''; font-size: 13px;">Below are the details</span></p>
        <p style="font-family: arial;"><span style="font-family: ''Courier New'';">From&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; : @@FROM_DELETAK@@<br />
        TO&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; : @@TO_DELETAK@@<br />
        By&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; : @@ADDELEGATED_USER@@<br />
        Modified By&nbsp;&nbsp;&nbsp; : @@ModifiedBy@@<br />
        <span style="font-family: ''Courier New'';">Start&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>: <span style="font-family: ''Courier New'';">@@FROMDATE@@</span><br />
        <span style="font-family: ''Courier New'';">End&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; : @@TODATE@@<br />
        Comments&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; : @@DELEGATIONCOMMENT@@&nbsp;</span></span></p>
        <p style="font-family: arial;"><span style="white-space: pre-wrap; background-color: #ffffff; font-family: ''Courier New''; font-size: 13px; color: infotext;">Please use your Windows UserID and Password to login to IDMS to complete the task. </span></p>
        <p style="font-family: arial;">&nbsp;<span style="background-color: #ffffff; white-space: pre-wrap; font-size: small; font-family: ''Courier New''; color: infotext;">@@IACMURL@@ </span></p>
        <p style="font-family: arial;"><span style="background-color: #ffffff; white-space: pre-wrap; font-size: small; font-family: ''Courier New''; color: infotext;">&nbsp;</span></p>
        <p style="font-family: arial;"><span style="white-space: pre-wrap; background-color: #ffffff; font-family: ''Courier New''; font-size: 13px;"></span></p>
        <p><span style="font-family: ''Courier New''; font-size: 13px;">Best regards,</span><br style="font-family: ''Courier New''; font-size: 13px;" />
        <span style="white-space: pre-wrap; background-color: #ffffff; font-family: ''Courier New''; font-size: 13px;">@@REGARDS@@</span></p>
        <p><span style="font-family: ''Courier New''; font-size: 13px; color: infotext;">&nbsp;</span></p>
        <p><span style="font-family: ''Courier New''; font-size: 13px; color: infotext;">*** This is a system-generated message. Please do not reply to this message.***</span></p>
        <p style="font-family: arial;">&nbsp;</p>
        <p><span style="font-family: ''Courier New''; font-size: 13px;"></span></p>
    </body>
</html>', N'IACMAdmin', CAST(N'2019-05-22T10:53:23.517' AS DateTime))
GO
IF NOT EXISTS(SELECT 1 FROM [DI_M_MailTemplate] WHERE [TemplateName]='NPACSectionNOTFound')
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (N'NPACSectionNOTFound', N'NPAC Section NOT Found', N'itadmin@spgroup.com.sg', N'it@npacteam.com', N'', NULL, N'IDMS Notification for Re-organization', N'<p style="color: #000000; font-family: ''lucida grande'', tahoma, verdana, arial, sans-serif; font-size: 12px;">Dear ITD AMS Team,&nbsp;</p>
<p style="color: #000000; font-family: ''lucida grande'', tahoma, verdana, arial, sans-serif; font-size: 12px;">&nbsp;</p>
<p style="color: #000000; font-family: ''lucida grande'', tahoma, verdana, arial, sans-serif; font-size: 12px;"><span style="white-space: pre;">	</span>The following IDMS Section cannot be found in NMACS.</p>
<p style="color: #000000; font-family: ''lucida grande'', tahoma, verdana, arial, sans-serif; font-size: 12px;"><span style="color: #000000; font-family: ''lucida grande'', tahoma, verdana, arial, sans-serif; font-size: 12px;"><span style="white-space: pre;">	</span>@@Section@@</span></p>
<p style="color: #000000; font-family: ''lucida grande'', tahoma, verdana, arial, sans-serif; font-size: 12px;">&nbsp;</p>
<p style="color: #000000; font-family: ''lucida grande'', tahoma, verdana, arial, sans-serif; font-size: 12px;">Regards,<br />
@@REGARDS@@</p>
<p  style="color: #000000; font-family: ''lucida grande'', tahoma, verdana, arial, sans-serif; font-size: 12px;">
<br />
*** This is a system-generated message. Please do not reply to this message.***</p>', N'IACMAdmin', CAST(N'2019-06-20T12:55:19.843' AS DateTime), N'IACMAdmin', CAST(N'2019-06-25T12:14:27.970' AS DateTime))
GO
Update DI_M_MailTemplate Set MailContent ='<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">  <html>      <head>          <title></title>          <style type="text/css">              .body {              font-family: Courier New;              font-size: 13px;              font-weight: normal;              margin-left: 5px;              }              .marlef {              margin-left: 40px;              }              .marlef1 {              margin-left: 50px;              }              .marlef2 {              margin-left: 50px;              }          </style>      </head>      <body class="body" _events="[object Object]" jquery19103433693516084604="28" jquery19107719995960077419="28">          Dear All, <br />          <br />  @@eventinfo@@ <br />          <br />          User&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; : @@userDisplayName@@ (@@pernrNum@@) <br />          Employee Type : @@EMPTYPE@@ <br />          Company&nbsp; &nbsp; &nbsp; &nbsp;: @@coyCode@@ - @@coyText@@&nbsp;<br />          Department&nbsp; &nbsp; : @@deptCode@@ - @@deptText@@&nbsp;<br />          Division&nbsp; &nbsp; &nbsp; : @@divCode@@ - @@divText@@&nbsp;<br />          Organisation&nbsp; : @@orgCode@@ - @@orgText@@&nbsp;<br />          Cost Center&nbsp; &nbsp;: @@cctrCode@@ - @@cctrText@@&nbsp;<br />       <br/>  Please update the reference user value for the non employees listed below.  <br />          @@applicationstring@@ <br />          <br />          Regards,&nbsp;<br />          @@REGARDS@@<br />          <br />          *** This is a system-generated message. Please do not reply to this message.***      </body>  </html>'  where TemplateName like '%NotifyReferenceUserChange%'