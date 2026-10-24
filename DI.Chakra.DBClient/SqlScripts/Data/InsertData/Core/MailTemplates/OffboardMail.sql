SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName], [ActivityType]) 
VALUES (49, N'OffboardMail', N'This email is sent 7 days (configurable) prior when IDAS is notified, whichever is earlier of the termination of the employee. This is to inform the supervisor that the employee’s access rights will be revoked.', N'devteam@deepidentity.com', N'##MAILTO_SUPERVISOR##', N'itidadmin@rwsentosa.com', NULL,
 N'IDAS - Revocation of user access rights due to off boarding for @@REQUESTEENAME_USRLAST@@ @@REQUESTEENAME_USRFIRST@@', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
    <head>
        <title>IDAS - Changes in user access rights due to promotion</title>
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
			<td ><p>Dear ##SUPERVISOR_NAME##,</p>
        		<p>We received a notification from HR that employment of @@REQUESTEENAME_USRLAST@@ @@REQUESTEENAME_USRFIRST@@ with RWS will be terminated.</p>
				<p>This email is to inform you that <u>all his/her current application access rights will be revoked on <b>@@DATE_REQ@@</b>.</u></p>
				<p>If you require to temporarily hold the access rights, please submit a service request through <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a>
				before the revocation date. It is important to set the effective date of the request to be later than <b>@@DATE_REQ@@</b>, otherwise it will be revoked together with the current access rights.
				</p>
				
			</td></tr>
			<tr>
			  <td ><table border="1" cellpadding="5" cellspacing="0" width="300" bordercolor="#ffffff">
				  <tr bgcolor="aa504c" >
					<td Colspan="3"><span style="color:#FFF"><b>Removed Application / Roles / Entitlements</b></span></td>
				  </tr>
				  @@tableBody@@
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
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 1, 49, 3)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF