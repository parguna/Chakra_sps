SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) 
VALUES (7, N'EscalateAdhocProvisioningApproval', 
N'This is an escalation email sent to the approval’s supervisor when the request is not approve within the configured timeline.  ',
 N'devteam@deepidentity.com', N'##MAILTO_MANAGER_TSK##', N'##MAILTO_ATTTSK##,itidadmin@rwsentosa.com', NULL,
 N'IDAS - Escalation: Application access change approval required for @@REQUESTEEFIRSTNAME@@ @@REQUESTEELASTNAME@@',
 N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
       <p> Dear ##MANAGER_PRVTSK##,
        <br/>
		<br/>
        This is a escalation email to inform you that ##APPROVER_ATTTSK## didn''t complete the provisioning approval process.
		You are requested to take necessary steps to ensure the provisioning process is complete. 
        <br/>
        <br />
         A request  made for @@REQUESTEELASTNAME@@ @@REQUESTEEFIRSTNAME@@ on ##TASK_CREAT_DATE## is pending your approval.
	    <br />
		 <br />
        Please log into <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a> to approve or reject this request. Your expedited action is most appreciated.
        
        <br/>
        <br/>
		<u>Request Details: </u>
		 <br/>
		Application: @@APPLICATION_PRVTSK@@
        <br/>
        Requester Name: @@REQUESTERDISPLAYNAME_PRVTSK@@
        <br/>
        Date of Request: @@REQUESTEDDATE_PRVTSK@@
        <br/>
        Request: @@REQUESTTYPE_PRVTSK@@ - @@ENTITLEMENT_PRV@@ - @@ACCOUNTTYPE_PRVTSK@@
        <br/>
        Last date for submission: ##REVIEWLASTDATE_PRVTSK##

        <br/>
        <br/>
       <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table></p>
    
</body>
</html>', NULL, NULL, SYSDATETIME() , NULL, NULL, 7, 7)


SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF