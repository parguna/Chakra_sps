SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName]) 
VALUES (44, N'TransferMail', N'This email is sent 7 days (configurable) prior to the inter-transfer date of the employee, informing the employee that all the employee’s current role will be revoked.',
 N'devteam@deepidentity.com', N'@@NEWSUPRVSIORMAILTO_USR@@,##MAILTO_SUPERVISOR##', N'@@MAILTO_REQ@@,itidadmin@rwsentosa.com', NULL, 
N'IDAS - Change in user access rights due to transfer for @@REQUESTEENAME_USR@@ ',
 N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
    <head>
        <title>IDAS - Change in user access rights due to transfer for @@REQUESTEENAME_USR@@</title>
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
	<td >
        <p>Dear @@NEWSUPERVISORNAME@@/@@OLDSUPERVISORNAME@@,</p>	
        <p>We received a notification from HR that @@REQUESTEENAME_USR@@ will be transferred from @@OLDDEPARTMENTNAME_DISC@@ to @@NEWDEPARTMENTNAME_DISC@@.</p>
		<p>This email is to inform you that <u>all his/her  current application access will be revoked</u> on @@DATE_REQ@@, and the following set of access rights will be provisioned.</p>

		<p>You may request to retain or add new application access rights by submitting a service request through <a href="@@CONFIGURATIONMASTER_IACMURL_ATTTSK@@">IDAS</a>. 
		It is important to set the start date of the request as @@DATE_REQ@@ or later, otherwise it will be revoked together with the current access rights.</p>
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
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 1, 44)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF