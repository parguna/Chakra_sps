
INSERT INTO [dbo].[DI_M_MailTemplate]
           ([TemplateName],[TemplateDescription],[MailFrom],[MailTo],[MailSubject],[MailContent],[CreatedBy],[CreatedDate],[LastModifiedBy],[LastModifiedDate])
     VALUES
           ('AlertNotification','AlertNotification','devteam@deepidentity.com','','Missing Provisioning Matrix Data',
           '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
    
	  <p>Dear Security Operations,</p>

    <br/>

<p>Provisioning mandatory information is missing for some of the default systems.  Please see the detailed information below.
</p>

<p>User : @@USER@@  (@@pernrNum@@)<br/>


Company : @@coyCode@@ - @@coyText@@<br/>

Department : @@deptCode@@ - @@deptText@@<br/>

Division : @@divCode@@ - @@divText@@<br/>

Organisation : @@orgCode@@ - @@orgText@@<br/>
Cost Center : @@cctrCode@@ - @@cctrText@@<br/>



<br/>
<p>Error Info : @@MESSAGE@@
<br/>

<p>Regards,<br/>

IDMS Administrator.

*** This is a system-generated message. Please do not reply to this message.***</p>

</body>
</html>','IACMAdmin',GETDATE(),'IACMAdmin',NULL)