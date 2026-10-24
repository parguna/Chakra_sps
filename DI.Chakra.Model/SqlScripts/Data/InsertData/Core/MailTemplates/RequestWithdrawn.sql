
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'RequestWithdrawn', N'RequestWithdrawn', N'devteam@deepidentity.com', NULL, N'Request Withdrawn - @@ApplicationName@@ - @@REQUESTTYPE@@ - @@REQUESTID@@ - @@USER@@  (@@pernrNum@@)', 
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
    <p>
      Dear @@USER@@,<br/>

      <br/>


      The following request has been withdrawn by @@REQUESTER@@ on @@DATE@@<br/>


      <br/>
      <br/>
      Request Details:<br/>

      --------------------------------------------------------------------<br/>

      Request ID: @@REQUESTID@@<br/>
      Request Name: @@ApplicationName@@ - @@REQUESTTYPE@@<br/>
      Request For: @@USER@@<br/>
      Employee ID: @@pernrNum@@<br/>
      <br/>
      <br/>
      Regards,<br/>
      IDMS Administrator.
      <br/>

      *** This is a system-generated message. Please do not reply to this message.***
    </p>
  </body>
</html>', N'System', GETDATE(), NULL, NULL)

GO