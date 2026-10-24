SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) 
VALUES (31, N'TransferNotification', N'This email is sent to inform the user once the transfer is completed with the new and removed entitlements and application roles.', N'devteam@deepidentity.com',  N'@@MAILTO_REQ@@',  N'##MAILTO_SUPERVISOR##,itidadmin@rwsentosa.com', NULL, 
N'IDAS - Change in user access rights due to transfer for  @@USERLASTNAME_REQ@@ @@USERFIRSTNAME_REQ@@',
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
    <head>
        <title>IDAS - Change in user access rights for @@USERLASTNAME_REQ@@ @@USERFIRSTNAME_REQ@@</title>
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
				<p> This is to inform you that your accounts and/or application access rights have been changed as shown in the following table(s). The change will take effective immediately.</p>
				<p> Should you require any modification, please submit a service request through <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a>.</p></td>
			</tr>
		<tr>
			  <td ><table border="1" cellpadding="5" cellspacing="0" width="300" bordercolor="#ffffff">
				  <tr bgcolor="aa504c" >
					<td Colspan="3"><span style="color:#FFF"><b>Retained or Added Application / Roles / Entitlements</b></span></td>
				  </tr>
				  @@ENTITLEMENT_REQ_PRV_EVNT@@
				</table></td>
			</tr>
			<tr>
			  <td ><table border="0" cellpadding="0" cellspacing="5" width="300">
				  <tr>
					<td>&nbsp;</td>
				  </tr>
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
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 1, 31)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF