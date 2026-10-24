SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) 
VALUES (13, N'NotifyAdhocProvisioningApproval', N'Notify Adhoc Provisioning Approval', N'itidadmin@rwsentosa.com',
 N'##MAILTO_ATTTSK##', '@@REQUESTEE_MAILTO_REQ@@,##MAILTO_REQUESTEE_SUPERVISOR_USER##', NULL, N'IDAS *ACTION REQUIRED* - Application access change approval required for @@REQUESTEEFIRSTNAME@@ @@REQUESTEELASTNAME@@ ',
 N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
    <head>
        <title>IDAS *ACTION REQUIRED* - Application access change approval required for @@REQUESTEEFIRSTNAME@@ @@REQUESTEELASTNAME@@ </title>
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
        Dear @@APPROVER_ATTTSK@@,
        <br />
        <br />
         A request  made for @@REQUESTEELASTNAME@@ @@REQUESTEEFIRSTNAME@@ on ##TASK_CREAT_DATE## is pending your approval.
	    <br />
		 <br />
        Please proceed to <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a> to approve or reject this request. Your expedited action is most appreciated.
        <br />
      
        <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration</p>
			  <p><a href="http://www.rwsentosa.com/" alt="Resorts World Sentosa"></a></p></td>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table>
        </p>
    </body>
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 7, 13)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF