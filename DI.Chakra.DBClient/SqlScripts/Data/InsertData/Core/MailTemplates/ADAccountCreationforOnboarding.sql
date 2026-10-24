
SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 
INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) 
VALUES (40, N'ADAccountCreationforOnboarding', N'This email is sent 7 days (configurable) prior to or when IDAS is notified of (whichever is earlier) the onboarding date of the new employee(s). This is to inform the supervisor that the new employees’ account has been created and that application access request for the new employee(s) can be made in IDAS', N'devteam@deepidentity.com', N'##MAILTO_SUPERVISOR_USER##', N'itidadmin@rwsentosa.com', NULL, N'IDAS - Onboarding', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
    <head>
        <title>IDAS - Onboarding</title>
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
			  <td ><p>Dear ##SUPERVISOR_NAME_USER##,</p>
				<p> This is to inform you that an AD Account has been created for the following employee(s), You have been listed as the supervisor.</p>
				<p> You may submit a service request for additional application access rights through <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a>.</p>
				<p> Please note that the following set of access rights have been automatically provisioned.</p></td>
			</tr>

			<tr>
			  <td ><table border="1" cellpadding="5" cellspacing="0" width="300" bordercolor="#ffffff">
				  <tr bgcolor="aa504c" >
					<td><span style="color:#FFF"><b>Added Application / Roles / Entitlements</b></span></td>
				  </tr>
				  ##ONBOARDED_ENTITLEMENTS##
				</table></td>
			</tr>
			<tr>
			  <td ><table border="0" cellpadding="0" cellspacing="5" width="300">
				  <tr>
					<td>&nbsp;</td>
				  </tr>
				</table></td>
			</tr>
			<tr>
			  <td ><table border="1" cellpadding="5" cellspacing="0" width="300" bordercolor="#ffffff">
				  <tr bgcolor="aa504c" >
					<td><span style="color:#FFF"><b>List of new employee(s)</b></span></td>
					<td><span style="color:#FFF"><b>joining Date</b></span></td>
				  </tr>
				  ##ONBOARDED_USERS##
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
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 2, 40)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] Off