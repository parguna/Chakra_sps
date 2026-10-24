DELETE FROM [DI_M_MailTemplate] WHERE [TemplateName] = 'DeleteNonConnectedSystemNotification'
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'DeleteNonConnectedSystemNotification', N'DeleteNonConnectedSystemNotification', N'devteam@deepidentity.com', NULL, N'Offboarding Request - @@userDisplayName@@  (@@pernrNum@@)', 
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
    
	<p>Dear Security Operations,
<br/>
<br/>
The following user has left service on @@terminationDate@@. Please deactivate the user''s accounts in the systems listed below.
<br/>
<br/>
User : @@userDisplayName@@  (@@pernrNum@@)
<br/>
Company : @@coyCode@@ - @@coyText@@
<br/>
Department : @@deptCode@@ - @@deptText@@
<br/>
Division : @@divCode@@ - @@divText@@
<br/>
Organisation : @@orgCode@@ - @@orgText@@
<br/>
Cost Center : @@cctrCode@@ - @@cctrText@@
<br/>
<br/>
@@applicationstring@@

<br/>
<br/>
Regards, 
<br/>
IDMS Administrator. 
<br/>
<br/>
*** This is a system-generated message. Please do not reply to this message.***	</p>

</body>
</html>', N'System', GETDATE(), NULL, NULL)
