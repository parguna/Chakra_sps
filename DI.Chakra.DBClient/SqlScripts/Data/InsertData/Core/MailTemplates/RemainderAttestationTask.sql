SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] ON 

INSERT [dbo].[DI_M_MailTemplate] ([Id], [TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailCC], [MailBcc], [MailSubject], [MailContent], [ApplicableVariableScopes], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate], [TemplateScope], [MailTemplateName], [ActivityType]) 
VALUES (20, N'RemindAttestationApproval', N'This email is sent every 3 days (configurable) to the reviewers and the endorsers until the review activities are completed.', N'securityadmin@spgroup.com.sg', N'', N'', NULL, 
N'IDMS Reminder - Review of @@ATTTSK_ENDPOINTNAME_DISP@@ @@ATTE_TYPE@@ - @@ATTTSK_CAMPAIGN_NAME@@',
 N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">  <html>      <head>          
 <title>IDAS Reminder - Review of ##ATTTSK_ENDPOINTNAME_DISP## @@ATTE_TYPE@@  - @@ATTTSK_CAMPAIGN_NAME@@</title>          
 <style type="text/css">              .title {              font-family: "lucida grande", tahoma, verdana, arial, sans-serif;              
 font-size: 13px;              color: #1c2a47;              font-weight: bolder;              }              .body {              
 font-family: "lucida grande", tahoma, verdana, arial, sans-serif;              font-size: 12px;              color: InfoText;              
 font-weight: normal;              margin-left: 50px;              }          </style>      </head>      <body class="body">          
 <p>Dear @@APPROVER_ATTTSK@@,          <br />          <br />         
 Please be reminded to complete the ##ATTTSK_ENDPOINTNAME_DISP## @@ATTE_TYPE@@  Review. The following is a recap of our earlier email.          
 <br />          <pre>1. In compliance with the <span style="text-decoration: underline;">User Access Management &amp; Control Policy</span>, 
 you are required to perform a review of the @@ATTE_TYPE@@  of ##ATTTSK_ENDPOINTNAME_DISP## as of @@CAMPIGN_START_DATE@@.  
 2. Please follow the steps below to complete the review:   a. Logon to <a href="@@IACMURL@@">IDMS</a>.  
 b. 1. Go to Home -&gt; My Task -&gt; Select the task to review.    
 2. Click on "Detail" icon to start the review  3. This review shall be completed by @@CAMPIGN_END_DATE@@, and signed-off by your supervisor.    </pre>   
 <pre>You may contact the following support personnel should you require any clarification or assistance:   
 Subsystem Owner - ##ATT_EP_OWNER_DISPLAY_NAME##   Subsystem IT Lead - ##ATT_EP_ITLEAD_DISP_NAME##   
 IT Distribution Group - ##ATT_EP_IT_DIST_MAIL_ID##  </pre>          <br />          
 <br />          </p>          <table style="margin-top: 20px;">             
 <tbody>                  <tr>                      <td>                      <p>Best regards,<br />             
 @@REGARDS@@</p>                      </td>                  </tr>                  <tr>                     
 <td>                      
 <p>*** This is a system-generated message. Please do not reply to this message.***</p>                      
 </td>                  </tr>              </tbody>          </table>      
 </body>  </html>', NULL, N'IACMAdmin', SYSDATETIME() , N'IACMAdmin', NULL, 8, 20, 2)

SET IDENTITY_INSERT [dbo].[DI_M_MailTemplate] OFF