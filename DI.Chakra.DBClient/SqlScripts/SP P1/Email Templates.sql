USE [V5_IDMS_SP_SIT]
GO
SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (1, N'AdhocProvisioningAmendment', N'AdhocProvisioningAmendment', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Amendment Request for @@applicationName@@  Completed - @@USER@@  (@@USERID@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery19102242239357840518="28" jQuery1910949271146236042="28" jQuery1910547530185276222="28" jQuery19106831996142794552="28" jQuery19108528570604055713="28" jQuery19102688303565444697="28">
        Dear @@USER@@,&nbsp;<br />
        <br />
        <span style="font-size: 13px;">Your access request has been completed.</span> <br />
        <br />
        Request Details:<br />
        -------------------------------------------------------------------- <br />
        <br />
        Request ID: @@REQUESTNUMBER@@ <br />
        Request Name: @@applicationName@@ - @@REQUESTTYPE@@ <br />
        Request For: @@USER@@ <br />
        Employee ID: @@EMPLOYEEID@@ <br />
        Login ID: @@userLogonName@@&nbsp;<br />
        <br />
        Additional Information&nbsp;: @@accessName@@ <br />
        <br />
        Regards,<br />
        Security&nbsp;Administrator<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (2, N'AdhocProvisioningUserRemoval', N'AdhocProvisioningUserRemoval', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Request for @@applicationName@@  Completed - @@USER@@  (@@EMPLOYEEID@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery19108825178366830599="28" jQuery19100627836284987715="28" jQuery19102234976823795407="28">
        <p>Dear @@USER@@, <br />
        <br />
        Your access request has been completed. <br />
        <br />
        Request Details:<br />
        --------------------------------------------------------------------<br />
        Request ID: @@REQUESTNUMBER@@<br />
        Request Name: @@applicationName@@ - @@REQUESTTYPE@@<br />
        Request For: @@USER@@<br />
        Employee ID: @@EMPLOYEEID@@<br />
        Login ID: @@userLogonName@@<br />
        <br />
        <br />
        </p>
        <p>Regards,<br />
        Security&nbsp;Administrator<br />
        <br />
        </p>
        <p>*** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (3, N'DiscoveryJobFailed', N'Discovery Job Failed', N'securityadmin@singaporepower.com.sg', NULL, NULL, NULL, N'Discovery Job Failed - @@ENDPOINTNAME@@ (@@PROCESSNAME@@) ', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <title></title>
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
<body>
    <div>
        <p>
        <span class="title">Dear Security Operations,</span></p>
        
        <p>
        <span class="title">Discovery Process Failed</span></p>
        
        <p>
        <span class="title">Error Message:</span></p><hr />
        
        <p>        
        <span class="content">@@ERRORMESSAGE@@</span></p>
        
        <p>
        <span class="title">Endpoint system details:</span></p><hr />
        
       
    </div>
    <table class="table_results" cellpadding="3" cellspacing="3" style="width:100%;">
       
        <tr>
            <td class="style1">
                <b>Application Type:</b></td>
            <td>@@ENDPOINTTYPE@@</td>
           
        </tr>   
		<tr>
            <td class="style1">
                <b>Application:</b></td>
            <td>@@ENDPOINTNAME@@</td>
           
        </tr>             
        <tr>
            <td class="style1">
                <b>Process Name: </b></td>
            <td>@@PROCESSNAME@@</td>
          
        </tr>
         
    </table>
    
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (4, N'EscalateAdhocProvisioningApproval', N'Escalate Adhoc Provisioning Approval', N'securityadmin@singaporepower.com.sg', NULL, NULL, NULL, N'Adhoc Provisioning Approval Request - {0} - Escalation', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
       <p> Dear @@RECEIVER@@,
        <br/>
        This is a escalation email to inform you that @@REVIEWER_NAME@@ didn&#39;t complete the provisioning approval process.
        <br/>
        You are requested to take necessary steps to ensure the provisioning process is complete.
        <br/>
        <br/>
		Please log in to IDMS 
        <br/>
        <br/>
		Application: @@APPLICATION_NAME@@
        <br/>
        Requester Name: @@REQUESTER@@
        <br/>
        Date of Request: @@REQUESTEDDATE@@
        <br/>
        Request: @@REQUEST_DETAIL@@
        <br/>
        Last date for submission: @@REVIEWLASTDATE@@.
        <br/>
        <br/>
       Regards
        <br/>
       IDMS Administrator
    </div>
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (5, N'EscalateAttestationApproval', N'Escalate Attestation Approval', N'securityadmin@singaporepower.com.sg', NULL, NULL, NULL, N'Attestation - Approval Level {0} Escalation', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
<body>
    <div>
        Dear @@RECEIVER@@,
      <br/>
      <br/>
        This is a escalation email to inform you that @@REVIEWER_NAME@@ didn&#39;t complete the attestation approval process.
      <br/>
        You are requested to take necessary steps to ensure the attestation process is complete.
      <br/>
		Please log in to IDMS 
      <br/>
        Attestation Name: @@ATTESTATION_NAME@@
      <br/>
        Attestation Description: @@ATTESTATION_DESC@@
      <br/>
        Required Action: Attestation - Approval Level @@APPROVAL_LEVEL@@
      <br/>
        Last date for submission: @@REVIEWLASTDATE@@.
      <br/><br/><br/>
        Regards
      <br/>
       IDMS Administrator  
	   <br/>
      <br/>
      *** This is a system-generated message. Please do not reply to this message.*** </p>  
</div>
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (6, N'NotificationForNewUser', N'Notification for New User', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: User Profile @@userDisplayName@@ (@@pernrNum@@) created', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery191031071051835591895="28" jQuery19106972244626770776="28">
        Dear @@supervisorName@@ <br />
        <br />
        The IDMS User Profile has been&nbsp;created. <br />
        <br />
        User&nbsp;:&nbsp;@@userDisplayName@@&nbsp;(@@pernrNum@@) <br />
        Company : @@coyText@@ <br />
        Department : @@deptText@@ <br />
        Division : @@divText@@ <br />
        Organisation : @@orgText@@ <br />
        Cost Center : @@cctrCode@@ <br />
        <br />
        <br />
        <br />
        Regards,&nbsp;<br />
        Security Administrator <br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (7, N'NotifyAdhocProvisioningApproval', N'Notify Adhoc Provisioning Approval', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Request for @@applicationName@@ pending for your approval - @@userDisplayName@@ (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery1910061058624796245076="33" jQuery19108204754305343215="33" jQuery191010259010248692357="28" jQuery19107970852199999311="28" jQuery191005408647152655671="33">
        Dear @@approverName@@, <br />
        <br />
        There is an access request for&nbsp;@@userDisplayName@@ pending your approval. <br />
        <br />
        Request ID: @@requestNum@@ <br />
        Request Name: @@applicationName@@ - @@requestType@@ <br />
        Request For: @@userDisplayName@@ <br />
        Employee ID: @@pernrNum@@ <br />
        Account Type: @@AccountType@@ <br />
        Reason for Request: @@COMMENTS@@ <br />
        <br />
        Please <strong><span style="color: blue; text-decoration: underline;">use your Windows UserID and Password to</span></strong> login to IDMS&nbsp;to complete&nbsp;the task. <br />
        @@IACMURL@@ <br />
        <br />
        Regards,&nbsp;<br />
        Security Administrator <br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (8, N'OUChangeRequestNotification', N'OUChangeRequestNotification', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Transfer Notification - @@userDisplayName@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jquery19109912656461533571="33" jquery19104078758837715021="28" jquery19105151957916772674="28" jquery1910763603133311431="33" jquery19109592615030167242="33" jquery1910970336753895991="33" jquery19105218288216822015="33" jquery191036917557197117695="33" jquery19108631961207408471="33">
        <p>Dear @@userDisplayName@@, </p>
        <p>Please be informed&nbsp;your access&nbsp;has been updated based on your new organisation unit.&nbsp;</p>
        <p>Should you require any new access, please submit your access request via IDMS.</p>
        <p><a href="https://pidmwbgrp.singaporepower.local/login.aspx" onclick="return false;" target="blank">https://pidmwbgrp.singaporepower.local/login.aspx</a></p>
        <p>During your&nbsp;next logon to Lotus Notes, you may be prompted to accept New Name Change.&nbsp;<br />
        <strong><em>Please Click on&nbsp;<span style="text-decoration: underline;">"Yes"</span> to accept the&nbsp;changes.</em></strong><br />
        <br />
        Please add&nbsp;your new email address&nbsp;(with the new OU) to the ACL (Access Control List) of all your local archive database/s if you have any.<br />
        <br />
        The instructions on how to add your new username to the ACL (Access Control List) of your local archive database is as below:<br />
        <br />
        1. From the workspace of your Lotus Notes client, select your local archive mail database (example: single click on the database)</p>
        <p>2. Select from the drop down menu, <br />
        [File]-&gt;[Application]-&gt;[Access Control]</p>
        <p>3. From the Access Control List dialog box, click on [Add]</p>
        <p>4. Select the "person" icon, to bring out the global address book</p>
        <p>5. From the global address book, ensure that "SPower&rsquo;s Address Book" is chosen in the "Choose address book" field<br />
        In the "Find names starting with" field, search and select your name follow by clicking on the [Add -&gt;] button,<br />
        Then click on the [OK] button to finish.</p>
        <p>6. Upon adding your name (showing the new OU) in the ACL of the local archive database, select your new username again,<br />
        Change the attributes of your profile to the following:<br />
        "Person"&nbsp;- in the "User type" field, "Manager"&nbsp;- in the "Access" field<br />
        "Delete documents"&nbsp;- should be checked</p>
        <p>Upon completion, click on the [OK] button to finish.</p>
        <p>If you need any help or have any query, please contact our helpdesk at ext8111.&nbsp;</p>
        <p>Regards,&nbsp;</p>
        <p>Security Administrator</p>
        <p>&nbsp;</p>
        <p>&nbsp;</p>
        <p>*** This is a system-generated message. Please do not reply to this message.***</p>
        <p>&nbsp;</p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (9, N'Password Change Notification', N'Password Change Notification', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Login changed for @@applicationName@@ - @@userDisplayName@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery19100004245497371021534="28">
        Dear @@userDisplayName@@, <br />
        <br />
        This is to inform you that you have requested for password change for @@applicationName@@ - @@userLogonName@@ on @@dateTime@@. <br />
        <br />
        Regards,&nbsp;<br />
        Security Administrator<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (10, N'RemindAttestationApproval', N'Reminder Attestation Approval', N'securityadmin@singaporepower.com.sg', NULL, NULL, NULL, N'Attestation - Approval Level {0} Reminder', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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


    <p>  Dear @@RECEIVER@@,
	    <br/>
	   <br/>
        This is a gentle reminder regarding the pending attestation approval proces. You are requested to review your team member&#39;s access using IDMS Portal.
	   <br/>
		Please log in to IDMS 
	   <br/>
		Attestation Name: @@ATTESTATION_NAME@@
	   <br/>
        Attestation Description: @@ATTESTATION_DESC@@
	   <br/>
        Required Action: Attestation - Approval Level @@APPROVAL_LEVEL@@
	   <br/>
        Last date for submission: @@REVIEWLASTDATE@@.
	   <br/><br/><br/>
       Regards
	   <br/>
       IDMS Administrator  
	    <br/>
      <br/>
      *** This is a system-generated message. Please do not reply to this message.*** </p>  
</div>
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (11, N'AccessRequestRejected', N'AccessRequestRejected', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Request for @@ApplicationName@@ Rejected - @@USER@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery19108462597709516246="28">
        Dear @@USER@@,<br />
        <br />
        Your&nbsp;request has been rejected by @@APPROVER@@ on @@DATE@@<br />
        <br />
        <br />
        Request Details:<br />
        --------------------------------------------------------------------<br />
        Request ID: @@REQUESTID@@<br />
        Request Name: @@ApplicationName@@ - @@REQUESTTYPE@@<br />
        Request For: @@USER@@<br />
        Employee ID: @@pernrNum@@<br />
        Comments: @@approverComments@@<br />
        <br />
        <br />
        Regards,<br />
        Security&nbsp;Administrator. <br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (12, N'NotifyAttestationApproval', N'Notify Attestation Approval', N'securityadmin@singaporepower.com.sg', NULL, NULL, NULL, N'Attestation - Approval Level {0} Request', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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


    <p>   Dear @@RECEIVER@@,
	   <br/>
	   <br/>
        IT Security Team has initiated attestation process. You are requested to review your team member&#39;s access using IDMS Portal.
	  <br/>
		Please log in to IDMS 
	   <br/>
        Attestation Name: @@ATTESTATION_NAME@@
	   <br/>
        Attestation Description: @@ATTESTATION_DESC@@
	   <br/>
        Required Action: Attestation - Approval Level @@APPROVAL_LEVEL@@
	   <br/>
        Last date for submission: @@REVIEWLASTDATE@@.
	   <br/>
	   <br/>
      Regards
	   <br/>
      IDMS Administrator
	   <br/>
      <br/>
      *** This is a system-generated message. Please do not reply to this message.*** </p>
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (13, N'NotificationNPACIncomplete', N'Notification for incomplete NPAC Account ', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: NPAC Account Incomplete information @@UserName@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery191011647092777458479="28">
        <p>Security Operations,</p>
        <p>Notification of NPAC Account Created with Incomplete Information</p>
        <p>
        <div class="marlef">User ID : @@UserName@@<br />
        Section : @@Section@@<br />
        Designation : @@Designation@@</div>
        </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (14, N'OffBoardingUserNotification', N'OffBoardingUserNotification', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Offboarding Request - @@userDisplayName@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery191035183702545790435="28">
        Dear Security Operations, <br />
        <br />
        The following user has left service on @@terminationDate@@. All the connected system accounts are deactivated. <br />
        <br />
        User : @@userDisplayName@@ (@@pernrNum@@) <br />
        Company : @@coyCode@@ - @@coyText@@ <br />
        Department : @@deptCode@@ - @@deptText@@ <br />
        Division : @@divCode@@ - @@divText@@ <br />
        Organisation : @@orgCode@@ - @@orgText@@ <br />
        Cost Center : @@cctrCode@@ - @@cctrText@@ <br />
        <br />
        Regards, <br />
        IDMS Administrator. <br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (15, N'OffboardingApprover', N'OffboardingApprover', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Offboarding Approver - @@userDisplayName@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery191012536647933698086="28" jQuery19101335439977692593="28">
        Dear Security Operations, <br />
        <br />
        The following Approver has left service on @@terminationDate@@. Please change the following Configuration. <br />
        <br />
        User : @@userDisplayName@@ (@@pernrNum@@) <br />
        <br />
        Configuration : @@applicationstring@@ Group Name: @@GROUPNAME@@ <br />
        <br />
        Regards,&nbsp;<br />
        Security Administrator <br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (16, N'LotusNotesWelcomeEmail', N'LotusNotesWelcomeEmail', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'Welcome to the Singapore Power Corporate Email System', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
        <title></title>
        <style type="text/css">
            .body {
            font-family: Courier New;
            font-size: 17px;
            font-weight: normal;
            margin-left: 20px;
            }
            .marlef {
            margin-left: 40px;
            }
        </style>
    </head>
    <body class="body marlef" _events="[object Object]" jQuery19107545034936782524="28" jQuery19104600334980618087="28" jQuery19109389021460594964="28" jQuery191013469489963844455="28">
        <p>Welcome to the Singapore Power Corporate Email System.</p>
        <p>Please note that our Email facility is for official use and must not be misused. If you receive non-business related mails, especially those with large attachments or chain mails, please delete them immediately from your mailbox. You are NOT to disseminate non-business related mails as it is against our Corporate Email policy and can lead to disciplinary action. We would also like to remind all that every email is archived and can be retrieved for investigative purposes, where required. A copy of the policy is available from the&nbsp;SPectrum at Resources-&gt;IT Needs-&gt;IT Policies &amp; Guidelines-&gt;Corporate Email Policy&nbsp;<br />
        <br />
        <a href="http://spectrum.singaporepower.local/Resources/IT-Needs/IT-Policies-Guidelines.aspx">http://spectrum.singaporepower.local/Resources/IT-Needs/IT-Policies-Guidelines.aspx<br />
        <br />
        </a>For assistance in using the Lotus Notes client, please visit the IT Needs section in SPectrum located at Resources-&gt;IT Needs-&gt;Tips&amp;Guides-&gt;Lotus Notes&nbsp;tab.<br />
        <br />
        <a href="http://spectrum.singaporepower.local/Resources/IT-Needs/Tips-Guide/Lotus-Notes.aspx">http://spectrum.singaporepower.local/Resources/IT-Needs/Tips-Guide/Lotus-Notes.aspx</a></p>
        <p><span>​</span><span>​</span>You are highly encouraged to find out more about the Enterprise Email Archival System and the Enterprise Fax System from the following documents in this section: </p>
        <p>- Email Archival Guide</p>
        <p>- FAQ for Email Archival</p>
        <p>- User Guide for Enterprise Fax</p>
        <p>- FAQ for Enterprise Fax</p>
        <p>Our anti-spam system, Mimesweeper, will help you filter potential spam emails from your mailbox. Please click the link below on the usage of the Mimesweeper system <br />
        <br />
        <a href="http://spectrum.singaporepower.local/Resources/IT-Needs/Tips-Guide/Miscellaneous.aspx">http://spectrum.singaporepower.local/Resources/IT-Needs/Tips-Guide/Miscellaneous.aspx<br />
        </a><a href="http://spectrum.singaporepower.local/getattachment/1f63d9f5-f79c-4786-ae7a-06bd020311d3/Mimesweeper-ESG-User-Guide-(1).aspx"><br />
        </a>Please be reminded that regular housekeeping by deleting mails no longer required to ensure better system response time, easier mail searching and to reduce the chance of your mailbox database being corrupted due to large mail database size. The free space is immediately "returned" to you after you have emptied your "Trash" folder. Do note that deleted emails can be retrieved via the Email Archival system for a period of up to 7 years. </p>
        <p>The email system will prevent you from sending out mails if your mailbox size is larger than the quota allocated to you. </p>
        <p>Your email quota and current usage can be accessed by the mailbox indicator at the top-hand corner (slightly above the Inbox folder) of your Lotus Notes client. Hover your mouse on the indicator to see more details. </p>
        <p>For further assistance, please call our Helpdesk at 6378 8111.</p>
        <p>You can get more information about the company''s businesses and policies from our SPectrum. These policies cover areas on Email, Financial, Purchasing, HR matters, etc. </p>
        <p>Regards,</p>
        <p>Singapore Power Limited : 199406577N </p>
        <p>________________________________________________________________________________________________________________________________________________________________________________________________________</p>
        <p>This email and any files transmitted with it are confidential and intended for the named recipient only. The contents herein are privileged to the sender and the use thereof is restricted to the intended purpose. Please notify the sender immediately by e-mail if you have received this e-mail by mistake and delete this e-mail from your system. If you are not the intended recipient please do not disclose, copy, distribute or take any action in reliance on the contents of this e-mail. Thank you.</p>
        <p>________________________________________________________________________________________________________________________________________________________________________________________________________</p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (17, N'DeleteNonConnectedSystemNotification', N'DeleteNonConnectedSystemNotification', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Offboarding Request - @@userDisplayName@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery19103433693516084604="28" jQuery19107719995960077419="28">
        Dear Security Operations, <br />
        <br />
        @@eventinfo@@ <br />
        <br />
        User : @@userDisplayName@@ (@@pernrNum@@) <br />
        Company : @@coyCode@@ - @@coyText@@ <br />
        Department : @@deptCode@@ - @@deptText@@ <br />
        Division : @@divCode@@ - @@divText@@ <br />
        Organisation : @@orgCode@@ - @@orgText@@ <br />
        Cost Center : @@cctrCode@@ - @@cctrText@@ <br />
        <br />
        @@applicationstring@@ <br />
        <br />
        Regards,&nbsp;<br />
        Security Administrator <br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (18, N'AlertNotification', N'AlertNotification', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Validation Failed - @@event@@ - @@pernr@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery19102834345089658415="28" jQuery19103463792849892146="28">
        <p>Dear Security Operations,</p>
        <br />
        <p>Provisioning mandatory information is missing for some of the default systems. Please see the detailed information below. </p>
        <p>User : @@USER@@ (@@pernrNum@@)<br />
        Company : @@coyCode@@ - @@coyText@@<br />
        Department : @@deptCode@@ - @@deptText@@<br />
        Division : @@divCode@@ - @@divText@@<br />
        Organisation : @@orgCode@@ - @@orgText@@<br />
        Cost Center : @@cctrCode@@ - @@cctrText@@<br />
        <br />
        </p>
        <p>Error Info : @@MESSAGE@@ </p>
        <p>Regards,<br />
        Security&nbsp;Administrator<br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (19, N'AccessRequestFailureNotification', N'Access Request Failure Notification', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Execution Failed - @@APPLICATION@@  - @@RequestType@@ - @@RequestNum@@ - @@pernr@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery19101079468347808708="28" jQuery19103590868106932588="28">
        <p>Dear Security Operations,</p>
        <br />
        <p>The following request has failed to execute. Please view the error details from the Log viewer.</p>
        <p>Request Details:<br />
        -------------------------------------------------------------------</p>
        <p>Request ID: @@RequestNum@@<br />
        Request Name: @@APPLICATION@@ - @@RequestType@@<br />
        Request For: @@USERDISPLAYNAME@@<br />
        Employee ID: @@PERNRNO@@<br />
        Execution Failure Date: @@DATE@@<br />
        <br />
        <br />
        Regards,&nbsp;<br />
        Security Administrator</p>
        <br />
        <br />
        <p>*** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (20, N'CreateAccountNotificationInhouseTemplate', N'Create account notification for Inhouse System', N'DoNotRespond@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Request for @@APPLICATIONNAME@@ Completed - @@USERDISPLAYNAME@@ (@@USERNAME@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery191027463812997583353="28" jQuery1910621298651324905="28" jQuery191006254650212996582="28">
        <p>PRIVATE &amp; CONFIDENTIAL<br />
        -------------------------------------------------------------------<br />
        To:&nbsp;&nbsp;@@RECEIVER@@<br />
        <div class="marlef">@@COMPANY@@<br />
        @@DEPT@@ @@DIVISION@@<br />
        @@BRANCH@@ @@SECTION@@</div>
        </p>
        <p>Notification of @@APPLICATIONNAME@@ (@@APPLICATIONNAMEFULL@@) User ID and Password</p>
        <p>1. The following @@APPLICATIONNAME@@ ID has been created :-
        <div class="marlef">User ID : @@USERNAME@@<br />
        Password : @@PASSWORD@@<br />
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
        <br />
        <br />
        Regards, <br />
        Security Administrator
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (21, N'CreateAccountNotificationCommonTemplate', N'Create account notification common template for all systems', N'DoNotRespond@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Request for @@APPLICATIONNAME@@ Completed - @@USERDISPLAYNAME@@ (@@EMPLOYEEID@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery19105777307022835423="28" jQuery191031130699542479323="28">
        <p>PRIVATE &amp; CONFIDENTIAL<br />
        -------------------------------------------------------------------<br />
        To:&nbsp;&nbsp;@@USERDISPLAYNAME@@<br />
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
        <br />
        <br />
        Regards, <br />
        Security Administrator
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (22, N'OnboardingUserEmail', N'OnboardingUserEmail', N'DoNotRespond@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Request Completed - A1 - (@@EMPLOYEEID@@) @@APPLICATIONNAME@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery191045868327819865823="28" jQuery19103030169706004316="28">
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
        <p>3. Please ensure that your password is kept confidential and that you do not grant permission to any person to use your User ID or password. Please also ensure that your terminal is not left unattended after you have signed on to the system. </p>
        <p>4. Should you have any query, please do not hesitate to contact Helpdesk at 6378 8111.</p>
        <br />
        <br />
        Regards, <br />
        Security Administrator
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (23, N'NotifyEmployeeEventTask', N'Notify Employee Event Task', N'secaidmtest@singaporepower.com.sg', NULL, NULL, NULL, N'Employee Creation/Extension/Shortening', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
      <br/>
      <br/>
      The following task is pending your approval.
      <br/>
      <br/>
      Request Name: @@requestType@@
      <br/>
      Requested For: @@userDisplayName@@
	   <br/>
      Requested By: @@requesterDisplayName@@
      <br/>
      Employee ID: @@pernrNum@@
      <br/>
      <br/>
      Please click the below link to log in to IDMS portal to action on the task.
      <br/>
	  @@IACMURL@@
      <br/>
      Regards,
      <br/>
      IDMS Administrator.
      <br/>
      <br/>
      *** This is a system-generated message. Please do not reply to this message.***
    </p>
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (24, N'RemindEmployeeEventApproval', N'Remind Employee Event Approval', N'secaidmtest@singaporepower.com.sg', NULL, NULL, NULL, N'Non Employee Creation/Extension/Shortening', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
   
        <p>Dear @@RECEIVER@@,<br/>
        <br/>
		This is a gentle reminder regarding the pending approval process. You are requested to review the below request using IDMS
        <br/>
        <br/>
        Please log in to IDMS 
        <br/>
		 Request Name: @@requestType@@
      <br/>
      Requested For: @@userDisplayName@@
	   <br/>
      Requested By: @@requesterDisplayName@@
      <br/>
        Date of Request: @@REQUESTEDDATE@@
        <br/>
        Last date for submission: @@REVIEWLASTDATE@@.
        <br/>
        <br/>
       Regards
        <br/>
       IDMS Administrator    <p>
</div>
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (25, N'NotifyEmployeeEventTaskApproved', N'Notify Employee Event Task Approved', N'secaidmtest@singaporepower.com.sg', NULL, NULL, NULL, N'Employee Creation/Extension/Shortening Request Approved', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
      <br/>
      <br/>
      Your request for Non Employee Creation / Extension / Shortening has been approved.
      <br/>
      <br/>
      Request Name: @@requestType@@
      <br/>
      Requested For: @@userDisplayName@@
	   <br/>
      Employee ID: @@pernrNum@@
      <br/>
      <br/>
      Regards,
      <br/>
      IDMS Administrator.
      <br/>
      <br/>
      *** This is a system-generated message. Please do not reply to this message.***
    </p>
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (26, N'NotifyEmployeeEventTaskRejected', N'Notify Employee Event Task Rejected', N'secaidmtest@singaporepower.com.sg', NULL, NULL, NULL, N'Employee Creation/Extension/Shortening Request Rejected', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
      <br/>
      <br/>
      Your request for Non Employee Creation / Extension / Shortening has been rejected.
      <br/>
      <br/>
      Request Name: @@requestType@@
      <br/>
      Requested For: @@userDisplayName@@
	   <br/>
      Employee ID: @@pernrNum@@
      <br/>
      <br/>
      Regards,
      <br/>
      IDMS Administrator.
      <br/>
      <br/>
      *** This is a system-generated message. Please do not reply to this message.***
    </p>
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (27, N'TriggerDeletedNotification', N'Trigger Deleted Notification', N'secaidmtest@singaporepower.com.sg', N'', NULL, NULL, N'Trigger missing in HR system - @@event@@ - @@pernr@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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

    <br/>

<p>HR trigger is missing for the below user.
</p>

<p>User : @@USER@@  (@@pernrNum@@)<br/>
Company : @@coyCode@@ - @@coyText@@<br/>
Department : @@deptCode@@ - @@deptText@@<br/>
Division : @@divCode@@ - @@divText@@<br/>
Organisation : @@orgCode@@ - @@orgText@@<br/>
Cost Center : @@cctrCode@@ - @@cctrText@@<br/>
<br/>

<p>Regards,<br/>

IDMS Administrator.

*** This is a system-generated message. Please do not reply to this message.***</p>

</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (28, N'RequestWithdrawn', N'RequestWithdrawn', N'secaidmtest@singaporepower.com.sg', NULL, NULL, NULL, N'Request Withdrawn - @@ApplicationName@@ - @@REQUESTTYPE@@ - @@REQUESTID@@ - @@USER@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
      Dear @@APPROVER@@,<br/>

      <br/>


      The following request has been withdrawn by @@REQUESTER@@ on @@DATE@@<br/>


      <br/>
      <br/>
      Request Details:<br/>

      --------------------------------------------------------------------<br/>

      Request ID: @@REQUESTID@@<br/>
      Request Name: @@ApplicationName@@ - @@REQUESTTYPE@@<br/>
      Request For: @@USER@@<br/>
      Employee ID: @@pernrNum@@<br/>
      <br/>
      <br/>
      Regards,<br/>
      IDMS Administrator.
      <br/>

      *** This is a system-generated message. Please do not reply to this message.***
    </p>
  </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (29, N'NonEmployeeExpiryNotification', N'Non Employee Expiry Notification', N'secaidmtest@singaporepower.com.sg', NULL, NULL, NULL, N'Non Employee Expiry Notification - @@userDisplayName@@ - @@pernr@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
<br/>
      <br/>

      User (@@userDisplayName@@) is going to expire soon. Please see the details below.<br/>

      <br/>
      <br/>
      User Details:<br/>

      --------------------------------------------------------------------<br/>

      User Name: @@userDisplayName@@<br/>
      Employee ID: @@pernr@@<br/>
	  Expiry Days: @@expiry_days@@<br/>
      Expiry Date: @@expiry_date@@<br/>
<br/>
Please use the IDMS portal to extend the validity.
<br/>
@@IACMURL@@
<br/>
<br/>
<br/>
Regards,
<br/>
Security Administrator.
<br/>
<br/>
*** This is a system-generated message. Please do not reply to this message.***</p>

</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (30, N'RemediationComplete', N'RemediationComplete', N'secaidmtest@singaporepower.com.sg', NULL, NULL, NULL, N'Remediation Request Completed - @@applicationName@@  - @@REQUESTTYPE@@ - @@REQUESTNUMBER@@ - @@USER@@  (@@USERID@@) - (@@userLogonName@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (31, N'EmployeeEventWithdrawn', N'EmployeeEventWithdrawn', N'secaidmtest@singaporepower.com.sg', NULL, NULL, NULL, N'Request Withdrawn - Employee Creation/Extension/Shortening', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
      Dear @@approverName@@,<br/>

      <br/>


      The following request has been withdrawn by @@requesterDisplayName@@ on @@DATE@@<br/>


      <br/>
      <br/>
      Request Details:<br/>

      --------------------------------------------------------------------<br/>

     Request Name: @@requestType@@
      <br/>
      Requested For: @@userDisplayName@@
	   <br/>
      Requested By: @@requesterDisplayName@@
      <br/>
      Employee ID: @@pernrNum@@
      <br/>
      <br/>
      Please click the below link to log in to IDMS portal to action on the task.
      <br/>
	  @@IACMURL@@
      <br/>
      <br/>
      Regards,<br/>
      IDMS Administrator.
      <br/>

      *** This is a system-generated message. Please do not reply to this message.***
    </p>
  </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (32, N'BulkRequestWithdrawn', N'BulkRequestWithdrawn', N'secaidmtest@singaporepower.com.sg', NULL, NULL, NULL, N'Bulk Request Withdrawn - @@ApplicationName@@ - @@REQUESTTYPE@@ - @@REQUESTID@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
      Dear @@APPROVER@@,<br/>

      <br/>


      The following request has been withdrawn by @@REQUESTER@@ on @@DATE@@<br/>


      <br/>
      <br/>
      Request Details:<br/>

      --------------------------------------------------------------------<br/>

      Request ID: @@REQUESTID@@<br/>
      Request Name: @@ApplicationName@@ - @@REQUESTTYPE@@<br/>
      Request For: @@USER@@<br/>
      Employee ID: @@pernrNum@@<br/>
      <br/>
      <br/>
      Regards,<br/>
      IDMS Administrator.
      <br/>

      *** This is a system-generated message. Please do not reply to this message.***
    </p>
  </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (33, N'RemindBulkRequestApproval', N'RemindBulkRequestApproval', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Bulk Request for @@applicationName@@ pending for your approval', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery1910061058624796245076="33" jQuery19108204754305343215="33" jQuery191010259010248692357="28" jQuery19107970852199999311="28" jQuery191005408647152655671="33">
        Dear @@approverName@@, <br />
        <br />
        There is an access request for&nbsp;@@userDisplayName@@ pending your approval. <br />
        <br />
        Request ID: @@requestNum@@ <br />
        Request Name: @@applicationName@@ - @@requestType@@ <br />
        Request For: @@userDisplayName@@ <br />
        Employee ID: @@pernrNum@@ <br />
        Account Type: @@AccountType@@ <br />
        Reason for Request: @@COMMENTS@@ <br />
        <br />
        Please <strong><span style="color: blue; text-decoration: underline;">use your Windows UserID and Password to</span></strong> login to IDMS&nbsp;to complete&nbsp;the task. <br />
        @@IACMURL@@ <br />
        <br />
        Regards,&nbsp;<br />
        Security Administrator <br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (34, N'NotifyBulkRequestApproval', N'NotifyBulkRequestApproval', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Bulk Request for @@applicationName@@ pending for your approval', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery1910061058624796245076="33" jQuery19108204754305343215="33" jQuery191010259010248692357="28" jQuery19107970852199999311="28" jQuery191005408647152655671="33">
        Dear @@approverName@@, <br />
        <br />
        There is an access request for&nbsp;@@userDisplayName@@ pending your approval. <br />
        <br />
        Request ID: @@requestNum@@ <br />
        Request Name: @@applicationName@@ - @@requestType@@ <br />
        Request For: @@userDisplayName@@ <br />
        Employee ID: @@pernrNum@@ <br />
        Account Type: @@AccountType@@ <br />
        Reason for Request: @@COMMENTS@@ <br />
        <br />
        Please <strong><span style="color: blue; text-decoration: underline;">use your Windows UserID and Password to</span></strong> login to IDMS&nbsp;to complete&nbsp;the task. <br />
        @@IACMURL@@ <br />
        <br />
        Regards,&nbsp;<br />
        Security Administrator <br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (35, N'RemindAdhocProvisioningApproval', N'Reminder Adhoc Provisioning Approval', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Request for @@applicationName@@ pending for your approval - @@userDisplayName@@ (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery19109055355848943497="33" jQuery191009384992435010081="28" jQuery19105115917409423485="28">
        Dear @@approverName@@,<br />
        <br />
        There is an access request for @@userDisplayName@@ pending for your approval.&nbsp;<br />
        <br />
        Request ID: @@requestNum@@ <br />
        Request Name: @@applicationName@@ - @@requestType@@ <br />
        Request For: @@userDisplayName@@ <br />
        Employee ID: @@pernrNum@@ <br />
        Account Type: @@AccountType@@ <br />
        Reason for Request: @@COMMENTS@@ <br />
        <br />
        Please <strong><span style="color: blue; text-decoration: underline;">use your Windows UserID and Password to</span></strong> login to IDMS to complete the task.<br />
        <a href="https://pidmwbgrp.singaporepower.local/login.aspx" onclick="return false;" target="blank">https://pidmwbgrp.singaporepower.local/login.aspx</a><br />
        <br />
        <br />
        Regards&nbsp;<br />
        Security Administrator
        <p><span style="font-family: Courier New; font-size: 13px;">*** This is a system-generated message. Please do not reply to this message.***</span></p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (36, N'BulkRequestRejected', N'BulkRequestRejected', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Bulk Request for @@ApplicationName@@ Rejected', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    <body class="body" _events="[object Object]" jQuery19108462597709516246="28">
        Dear @@USER@@,<br />
        <br />
        Your&nbsp;request has been rejected by @@APPROVER@@ on @@DATE@@<br />
        <br />
        <br />
        Request Details:<br />
        --------------------------------------------------------------------<br />
        Request ID: @@REQUESTID@@<br />
        Request Name: @@ApplicationName@@ - @@REQUESTTYPE@@<br />
        Request For: @@USER@@<br />
        Employee ID: @@pernrNum@@<br />
        Comments: @@approverComments@@<br />
        <br />
        <br />
        Regards,<br />
        Security&nbsp;Administrator. <br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (37, N'RequestRejectAlertToApprovers', N'RequestRejectAlertToApprovers', N'secaidmtest@singaporepower.com.sg', N'test', NULL, NULL, N'Request Rejected - @@ApplicationName@@ - @@REQUESTTYPE@@ - @@REQUESTID@@ - @@USER@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
        --------------------------------------------------------------------<br />
        Request ID: @@REQUESTID@@<br />
        Request Name: @@ApplicationName@@ - @@REQUESTTYPE@@<br />
        Request For: @@USER@@<br />
        Employee ID: @@pernrNum@@<br />
        <br />
        <br />
        Regards,<br />
        IDMS Administrator.
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
        </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (38, N'BulkRequestRejectAlertToApprovers', N'BulkRequestRejectAlertToApprovers', N'secaidmtest@singaporepower.com.sg', N'test', NULL, NULL, N'Bulk Request Rejected - @@ApplicationName@@ - @@REQUESTTYPE@@ - @@REQUESTID@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
        --------------------------------------------------------------------<br />
        Request ID: @@REQUESTID@@<br />
        Request Name: @@ApplicationName@@ - @@REQUESTTYPE@@<br />
        Request For: @@USER@@<br />
        Employee ID: @@pernrNum@@<br />
        <br />
        <br />
        Regards,<br />
        IDMS Administrator.
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
        </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (39, N'NotifyReferenceUserChange', N'NotifyReferenceUserChange', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: Reference User - @@eventtype@@ - @@userDisplayName@@  (@@pernrNum@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
        Dear Security Operations, <br />
        <br />
        @@eventinfo@@ <br />
        <br />
        User : @@userDisplayName@@ (@@pernrNum@@) <br />
        Company : @@coyCode@@ - @@coyText@@ <br />
        Department : @@deptCode@@ - @@deptText@@ <br />
        Division : @@divCode@@ - @@divText@@ <br />
        Organisation : @@orgCode@@ - @@orgText@@ <br />
        Cost Center : @@cctrCode@@ - @@cctrText@@ <br />
        <br />
        @@applicationstring@@ <br />
        <br />
        Regards,&nbsp;<br />
        Security Administrator <br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.***
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (40, N'NotifyOffice365Failed', N'Notify Office365 Failed Notification', N'secaidmtest@singaporepower.com.sg', N'', NULL, NULL, N'IDMS: O365 License assignment for @@UPN@@ failed.', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
<p>Regards,<br/>
IDMS Administrator.
<br/><br/>
*** This is a system-generated message. Please do not reply to this message.***</p>
</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (41, N'PassOfficeNotification', N'Pass Office Onboard Notification', N'secaidmtest@singaporepower.com.sg', N'', NULL, NULL, N'IDMS: SMS Onboard Notification - (@@EMPLOYEEID@@) @@DISPLAYNAME@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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

<br/>

<p>IDMS has created a new account in SMS system and assigned a dummy badge. 
Please assign the actual badge based on the below user details.
</p>

<p>User : @@USER@@  (@@pernrNum@@)<br/>
Company : @@coyCode@@ - @@coyText@@<br/>
Department : @@deptCode@@ - @@deptText@@<br/>
Division : @@divCode@@ - @@divText@@<br/>
Organisation : @@orgCode@@ - @@orgText@@<br/>
Cost Center : @@cctrCode@@ - @@cctrText@@<br/>
<br/>

<p>Regards,<br/>

IDMS Administrator.

*** This is a system-generated message. Please do not reply to this message.***</p>

</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (42, N'PassOfficeOffBoardNotification', N'Pass Office Offboard Notification', N'secaidmtest@singaporepower.com.sg', N'', NULL, NULL, N'IDMS: SMS Offboard Notification - (@@EMPLOYEEID@@) @@DISPLAYNAME@@', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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

<br/>

<p>IDMS has locked the account of below user in SMS system.
</p>

<p>User : @@USER@@  (@@pernrNum@@)<br/>
Company : @@coyCode@@ - @@coyText@@<br/>
Department : @@deptCode@@ - @@deptText@@<br/>
Division : @@divCode@@ - @@divText@@<br/>
Organisation : @@orgCode@@ - @@orgText@@<br/>
Cost Center : @@cctrCode@@ - @@cctrText@@<br/>
Reason : Off-Boarding
<br/>

<p>Regards,<br/>

IDMS Administrator.

*** This is a system-generated message. Please do not reply to this message.***</p>

</body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (43, N'NotifyAttestationComplete', N'Notify Attestation Complete', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'Attestation - {0} Completed', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES (44, N'SRMSupportTeamNotification', N'SRM Support Team Notification', N'securityadmin@singaporepower.com.sg', N'test', NULL, NULL, N'IDMS: SRM Create Account Request Completed - @@USER@@ (@@USERID@@)', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
        -------------------------------------------------------------------- <br />
        <br />
        Request ID: @@REQUESTNUMBER@@ <br />
        Request Name: SAP SRM <br />
        Request For: @@USER@@ <br />
        SRM Login ID: @@userLogonName@@&nbsp;<br />
        <br />
        Requested Roles &nbsp;: <br />
        @@accessName@@ <br />
        <br />
        <br />
        Regards
        <br />
        Security Administrator
        <br />
        <br />
        *** This is a system-generated message. Please do not reply to this message.*** </p>
    </body>
</html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO


INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES 
(100, N'Office365LicenseRemovalNotification', N'Office365LicenseRemovalNotification', N'securityadmin@singaporepower.com.sg', N'', NULL, NULL, N'IDMS: Office365 External Shared License Removal Notification - @@SharedAccountEmail@@', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
  <html>  <head>  <title></title>  <style type="text/css">  .body {  font-family: Courier New;  font-size: 13px;  font-weight: normal;  margin-left: 5px;  }    .marlef {  margin-left: 40px;  }  .marlef1 {  margin-left: 50px;  }  .marlef2 {  margin-left: 50px;  }  </style>  </head>  
  <body class="body">    
  <p>Dear Help Desk Team,</p>  
  <br/>    
  <p>IDMS has removed the Office 365 External Shared License to the below account.  </p>  
  <p>
  User : @@DISPLAYNAME@@  (@@pernrNum@@)<br/>
  Shared Account : (@@SharedAccountEmail@@)  <br/> 
  Cost Center : @@cctrCode@@ - @@cctrText@@<br/>  
  Reason : {@@RequestType@@}  <br/>  
  <p>Regards,<br/>    IDMS Administrator.    *** This is a system-generated message. Please do not reply to this message.***</p>    </body>  </html>', N'System', CAST(N'2018-07-26T17:45:03.493' AS DateTime), NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF
GO

INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES ( N'PostDiscoveryJobValidation', N'PostDiscoveryJobValidation', N'itidadmin@rwsentosa.com', N'@@MAILTO_DISC@@', N'', NULL, N'IDMS: Post Discovery Validation Report - @@ENDPOINTNAME@@ (@@PROCESSNAME@@) ', N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
        <p><span style="white-space: pre;">	</span>Dear IAM Administrator,<br />
        <br />
        <span style="white-space: pre;">		Post </span>Discovery Validation Report<br />
        <br />
        <span style="white-space: pre;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>Message:<br />
        <br />
        <span style="white-space: pre;">		</span>@@ERRORMESSAGE@@</p>
        <p><span style="white-space: pre;">		</span>Endpoint system details:<br />
        <br />
        <span style="white-space: pre;">		</span>Application Type:<span style="white-space: pre;">&nbsp;&nbsp;</span>@@ENDPOINTTYPE@@<br />
        <span style="white-space: pre;">		</span>Application:<span style="white-space: pre;">&nbsp; </span>@@ENDPOINTNAME@@<br />
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style="white-space: pre;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; @@PROCESSlabel@@: @@PROCESSNAME@@</span></p>
        <div>&nbsp;</div>
    </body>
</html>', N'IACMAdmin', CAST(N'2017-11-09T15:02:32.247' AS DateTime), N'IACMAdmin', CAST(N'2019-11-29T14:02:28.693' AS DateTime))
GO
