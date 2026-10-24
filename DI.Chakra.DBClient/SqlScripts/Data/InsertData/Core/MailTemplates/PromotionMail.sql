SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) 
VALUES (43, N'PromotionMail', N'This email is sent 7 days (configurable) prior to the promotion date of the employee, informing the employee of any new or upgraded entitlement(s).', N'devteam@deepidentity.com', N'@@MAILTO_REQ@@',  N'##MAILTO_SUPERVISOR##,itidadmin@rwsentosa.com', NULL, N'IDAS - Changes in user access rights due to promotion for @@REQUESTEENAME_USRLAST@@ @@REQUESTEENAME_USRFIRST@@', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
    <head>
        <title>IDAS - Changes in user access rights due to promotion @@REQUESTEENAME_USRLAST@@ @@REQUESTEENAME_USRFIRST@@</title>
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
			<td ><p>Dear @@REQUESTEENAME_USRFIRST@@,</p>
        		<p>We received a notification from HR that you will be promoted. Congratulations!</p>
				<p>This email is to inform you that the following set of access rights will be provisioned for you on @@DATE_REQ@@.</p>
				<p>You may request to revoke or add new application access rights by submitting a service request through <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a>.</p>
				<p>Please note that the following base set of employees access rights will be retained / provisioned on your promotion date.</p>
			</td></tr>
			<tr>
			  <td ><table border="1" cellpadding="5" cellspacing="0" width="300" bordercolor="#ffffff">
				  <tr bgcolor="aa504c" >
					<td Colspan="3"><span style="color:#FFF"><b>Retained or Added Application / Roles / Entitlements</b></span></td>
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
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 1, 43)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF