--SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

--INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) 
--VALUES (29, N'AdvanceDelegation', N'This email is sent to the delegated user during out of office setup.', N'devteam@deepidentity.com', N'@@MAILTO_DELTSK@@', N'itidadmin@rwsentosa.com', NULL, N'IDAS - Advance Delegation - (@@FROM_DELETAK@@ - @@TO_DELETAK@@) Delegated by - @@ADDELEGATED_USER@@', 
--N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
--<html>
--    <head>
--		<meta charset="UTF-8">
--        <title>IDAS - Advance Delegation</title>
--        <style type="text/css">
--            .title {
--            font-family: "lucida grande", tahoma, verdana, arial, sans-serif;
--            font-size: 13px;
--            color: #1c2a47;
--            font-weight: bolder;
--            }
--            .body {
--            font-family: "lucida grande", tahoma, verdana, arial, sans-serif;
--            font-size: 12px;
--            color: InfoText;
--            font-weight: normal;
--            margin-left: 50px;
--            }
--        </style>
--    </head>
--    <body class="body">
--	<table style="font-family:arial;">
--	<tr>
--    <td ><!-- Message start -->

--		<table style="font-family:arial;">
--        <tr>
--          <td ><p>Dear @@APPROVER_DELTSK@@,</p>
--            <p>This email is to notify you on the Approval role has been delegated to you by @@ADDELEGATED_USER@@.</p>
--			<p>For the following Period : <b>@@FROM_DELETAK@@ - @@TO_DELETAK@@</b>.<p>
--			<br />			
--            <p> Please log into <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a> to take action on the task. </p>
--			<br />
--			</td>
--        </tr>
--		</table>        
        
--	</td>
--	</tr>
--    <tr>
--    <td><!-- Message Footer -->
      
--      <table style="margin-top:20px">
--        <tr>
--          <td><p>Best regards,<br/>
--              IT ID Administration</p>
--            </td>
--        </tr>
--		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
--      </table></td>
--  </tr>
--</table>
--</body>
--</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 10, 29)

--SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF
INSERT [dbo].[DI_M_MailTemplate] ( [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES ( N'AdvanceDelegation', N'This email is sent to the delegated user during out of office setup.', N'securityadmin@spgroup.com.sg', N'@@MAILTO_DELTSK@@', NULL, NULL, N'IDMS - Advance Delegation - (@@FROM_DELETAK@@ - @@TO_DELETAK@@) Delegated by - @@ADDELEGATED_USER@@', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head>
		<meta charset="UTF-8">
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
    <body class="body">
	<table style="font-family:arial;">
	<tr>
    <td ><!-- Message start -->

		<table style="font-family:arial;">
        <tr>
          <td ><p>Dear @@APPROVER_DELTSK@@,</p>
            <p>This email is to notify you on the Approval role has been delegated to you by @@ADDELEGATED_USER@@.</p>
			<p>For the following Period : <b>@@FROM_DELETAK@@ - @@TO_DELETAK@@</b>.<p>
			<br />			
            <p> Please log into <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a> to take action on the task. </p>
			<br />
			</td>
        </tr>
		</table>        
        
	</td>
	</tr>
    <tr>
    <td><!-- Message Footer -->
      
      <table style="margin-top:20px">
        <tr>
          <td><p>Best regards,<br/>
              IT ID Administration</p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table></td>
  </tr>
</table>
</body>
</html>',  N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL)