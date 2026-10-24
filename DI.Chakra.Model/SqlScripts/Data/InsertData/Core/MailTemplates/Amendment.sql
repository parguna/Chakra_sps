
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES (N'AdhocProvisioningAmendment', N'AdhocProvisioningAmendment', N'devteam@deepidentity.com', NULL, N'Request Completed - @@applicationName@@  - @@REQUESTTYPE@@ - @@REQUESTNUMBER@@ - @@USER@@  (@@USERID@@) - (@@userLogonName@@)',
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

   <p> Dear @@USER@@,  
   <br/>  
    <br/>

    The following request has been executed.
    <br/>
    <br/>

    

      Request Details:<br/>
    
    --------------------------------------------------------------------
	<br/>
    <br/>

    Request ID: @@REQUESTNUMBER@@
	<br/>
    
      Request Name: @@applicationName@@ - @@REQUESTTYPE@@
	  <br/>
    
    
      Request For: @@USER@@
	  <br/>
    
    
      Employee ID: @@EMPLOYEEID@@
	  <br/>
    

    Login ID: @@userLogonName@@
	<br/>
	<br/>


    Access Name : @@accessName@@
	<br/>
    <br/>



    Regards,<br/>

    IDMS Administrator.<br/>
    <br/>

 

    
      *** This is a system-generated message. Please do not reply to this message.***   </p>
    




  </body>
</html>', N'System', GETDATE(), NULL, NULL)

GO
