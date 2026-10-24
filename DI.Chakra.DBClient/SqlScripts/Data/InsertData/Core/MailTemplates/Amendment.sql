
SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) 
VALUES (1, N'AdhocProvisioningAmendment', N'This email is sent immediately after changes in application access due to: onboarding, ad-hoc requests, close-loop remedial actions, transfer, promotion, offboarding', N'devteam@deepidentity.com', N'@@MAILTO_REQ@@', N'##MAILTO_SUPERVISOR##,itidadmin@rwsentosa.com', NULL, N'IDAS - Change in user access rights for @@USERLASTNAME_REQ@@ @@USERFIRSTNAME_REQ@@', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
	<table style="font-family:arial;">
	<tr>
    <td ><!-- Message start -->

		<table style="font-family:arial;">
			<tr>
			  <td ><p>Dear @@USERFIRSTNAME_REQ@@,</p>
				<p> This is to inform you that your accounts and/or application access rights have been changed as shown in the following table(s). The change will take effective immediately.</b>.</p>
				<p> Should you require any modification, please submit a service request through <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a>.</p></td>
			</tr>

			<tr>
			  <td ><table border="1" cellpadding="5" cellspacing="0" width="300" bordercolor="#ffffff">
				  <tr bgcolor="aa504c" >
					<td><span style="color:#FFF"><b>##ENTITLEMENT_REQ_INFO##</b></span></td>
				  </tr>
				  @@ENTITLEMENT_REQ_PRV@@
				</table></td>
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
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 2, 1)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] Off