
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) VALUES  
( 
N'Password Change Notification', 
N'Password Change Notification', 
N'devteam@deepidentity.com', 
NULL, 
N'Password changed - @@applicationName@@ - @@userDisplayName@@  (@@pernrNum@@)', 
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <title></title>
    <style type="text/css">
        .body {
            font-family: Courier New;
            font-size: 13px;
            font-weight: normal;
            margin-left: 5px;
        }

        .marlef {
            margin-left: 40px;
        }
         .marlef1 {
            margin-left: 50px;
        }
        .marlef2 {
            margin-left: 50px;
        }
    </style>
</head>
<body class="body">
    
	<p>Dear @@userDisplayName@@,
<br/>
<br/>
This is to inform you that you have requested for password change for @@applicationName@@ - @@userLogonName@@ on @@dateTime@@.
<br/>
<br/>
Regards,
<br/>
IDMS Administrator.
<br/>
<br/>
*** This is a system-generated message. Please do not reply to this message.***</p>

</body>
</html>',
 'System', getdate(), 'SINGAPOREPOWER', getdate())


 GO