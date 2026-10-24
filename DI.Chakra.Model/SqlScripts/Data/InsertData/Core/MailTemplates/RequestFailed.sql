INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'AccessRequestFailureNotification', N'Access Request Failure Notification', N'securityadmin@singaporepower.com.sg', NULL,N'Execution Failed - @@APPLICATION@@  - @@RequestType@@ - @@RequestNum@@', N'
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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

    <p>The following request has failed to execute. Please view the error details from the Log viewer.</p>

    <p>Request Details:<br />
    -------------------------------------------------------------------<br /></p>
    
<p> Request ID: @@RequestNum@@<br />

      Request Name: @@APPLICATION@@ - @@RequestType@@<br />


      Request For: @@USERDISPLAYNAME@@<br />


Employee ID: @@PERNRNO@@<br />

      Execution Failure Date: @@DATE@@<br />

      <br/>
      <br/>



      Regards, <br/>


      IDMS Administrator.</p>

      <br/>
      <br/>



      <p>*** This is a system-generated message. Please do not reply to this message.***</p>
    </body>
</html>', N'System', GETDATE(), NULL, NULL)
GO

