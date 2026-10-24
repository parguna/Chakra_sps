SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName])
 VALUES (14, N'OffboardingApprover', N'This email is sent to IDADMIN to notify them that the approver has left the organization and need to change the configuration and workflow.', 
 N'devteam@deepidentity.com', N'ITIDAdministrationOperationTeam@RWSentosa.com,@@CONFIGURATIONMASTER_MAILTO_ACCOUNTCREATED_USR@@', N'itidadmin@rwsentosa.com', NULL,
 N'IDAS - Terminated User Cleanup - @@USERDISPLAYNAME_USR@@  ( @@PERNRNO_USR@@ )',
 N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html>
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
    <body class="body">
        <table style="font-family:arial;">
	<tr>
    <td ><!-- Message start -->

		<table style="font-family:arial;">
			<tr>
			  <td ><p>Dear IT ID Administrator,</p>
        <p>
        The following User has left service on ##TERMINATIONDATE_USR##. Please do the cleanup activity for the following Configuration.</p>
        <br />
        
        <p>For Employee <b>@@USERFIRSTNAME@@ @@USERLASTNAME@@</b></p>
<PRE>
Employee Id: @@PERNRNO_USR@@
Display Name: @@USERDISPLAYNAME_USR@@
AD ID: @@USERNAME_USR@@
Email Id: @@EMAILID_USR@@
</PRE>
        </td></tr>
        <tr>
			  <td ><p><b>Details :</b></p></td></tr>
			  <tr>
			  <td ><table border="1" cellpadding="5" cellspacing="0" width="300" bordercolor="#ffffff">
				  <tr bgcolor="aa504c" >
					<td><span style="color:#FFF"><b>Subsystem Owner</b></span></td>
				  </tr>
				  ##SYSTEMOWNER_USR##
				</table></td>
			</tr>
			<tr>
			  <td ><table border="1" cellpadding="5" cellspacing="0" width="300" bordercolor="#ffffff">
				  <tr bgcolor="aa504c" >
					<td><span style="color:#FFF"><b>Group Membership</b></span></td>
				  </tr>
				  ##APPROVERGROUPS_USR##
				</table></td>
			</tr>
			<tr>
			  <td ><table border="1" cellpadding="5" cellspacing="0" width="300" bordercolor="#ffffff">
				  <tr bgcolor="aa504c" >
					<td><span style="color:#FFF"><b>Workflow Approver</b></span></td>
				  </tr>
				  ##APPROVERCONFIG_USR##
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
              IT ID Administration </p>
			    <p><a href="http://www.rwsentosa.com/" alt="Resorts World Sentosa"></a></p>
            </td>
        </tr>
		<tr><td><p>*** This is a system-generated message. Please do not reply to this message.***</p></td></tr>
      </table></td>
  </tr>
</table>
</body>
</html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin',NULL , 3, 14)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF