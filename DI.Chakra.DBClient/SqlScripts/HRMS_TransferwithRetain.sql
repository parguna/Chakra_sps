
IF NOT EXISTS(SELECT 1 from DI_M_MailTemplate WHERE TemplateName='OnboardingUserEmailPassword')
BEGIN
insert into DI_M_MailTemplate(TemplateName,TemplateDescription,MailFrom,MailTo,MailCC,MailSubject,MailContent,CreatedBy,CreatedDate)
VALUES( 'OnboardingUserEmailPassword','Used for Onboarding Email password letter','securityadmin@spgroup.com.sg','','','IDMS: Request Completed - (@@EMPLOYEEID@@) @@APPLICATIONNAME@@','<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">  <html>      <head>          <title></title>          <style type="text/css">              .body {              font-family: Courier New;              font-size: 13px;              font-weight: normal;              margin-left: 5px;              }              .marlef {              margin-left: 40px;              }              .marlef1 {              margin-left: 50px;              }              .marlef2 {              margin-left: 50px;              }          </style>      </head>      <body class="body" _events="[object Object]" jquery191045868327819865823="28" jquery19103030169706004316="28">          <p>PRIVATE &amp; CONFIDENTIAL<br />          -------------------------------------------------------------------<br />          To:&nbsp;&nbsp;@@USERDISPLAYNAME@@</p>          <div class="marlef">@@COMPANY@@<br />          @@DEPT@@ @@BRANCH@@<br />          @@SECTION@@ </div>          <div class="marlef">@@UNIT@@</div>          <p>&nbsp;</p>          <p> Notification of @@APPLICATIONNAME@@ Password</p>          <p>1. The Password for your @@APPLICATIONNAME@@ ID is :-          </p>          <div >Password&nbsp; &nbsp; : @@PASSWORD@@<br /></div>          <p>&nbsp;</p>          <p>2. Please perform the following steps:          </p>          <div >(i) Sign on to system using the user ID (provided in another email) and password given above. Change the password immediately before using the system.<br />          (ii) Password shall be at least 15 alphanumeric in length, and shall not be based on repetition and sequences.<br />          (iii) Password shall not contain the user name.<br />          (iv) Password shall contain at least three of the following categories:<br />          <div class="marlef1">          <div class="marlef2"> - Lower case letters<br />- Upper case letters <br />                   - Numbers<br />          - Special characters</div>          </div>            <p>3.&nbsp;&nbsp;Please keep your password confidential and do not grant permission to any person to use your User ID or password. Ensure that your terminal is not left unattended after you have signed on to the system.</p>          <p>4.&nbsp;Should you have any problem accessing the application, please email to IT Service Desk.</p>          <p>          Regards, <br />          @@REGARDS@@&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</p>          <p>*** This is a system-generated message. Please do not reply to this message.***</p>          <p>          </p>          <div>&nbsp;</div>          <p>&nbsp;</p>      </body>  </html>','System',GETDATE())
END


Update DI_M_MailTemplate Set MailSubject='IDMS: Request Completed - (@@EMPLOYEEID@@) @@APPLICATIONNAME@@',MailContent='<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">  <html>      <head>          <title></title>          <style type="text/css">              .body {              font-family: Courier New;              font-size: 13px;              font-weight: normal;              margin-left: 5px;              }              .marlef {              margin-left: 40px;              }              .marlef1 {              margin-left: 50px;              }              .marlef2 {              margin-left: 50px;              }          </style>      </head>      <body class="body" _events="[object Object]" jquery191045868327819865823="28" jquery19103030169706004316="28">          <p>PRIVATE &amp; CONFIDENTIAL<br />          -------------------------------------------------------------------<br />          To:&nbsp;&nbsp;@@USERDISPLAYNAME@@</p>          <div class="marlef">@@COMPANY@@<br />          @@DEPT@@ @@BRANCH@@<br />          @@SECTION@@ </div>          <div class="marlef">@@UNIT@@</div>          <p>&nbsp;</p>          <p> </p>          <p>Your @@APPLICATIONNAME@@ ID has been created :-</p>          <div >User ID&nbsp; &nbsp; &nbsp;: @@USERNAME@@<br /> AccountType : @@ACCOUNTTYPE@@<br />  <p>Should you have any problem accessing the application, please email to IT Service Desk.</p>          <p>          Regards, <br />          @@REGARDS@@&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</p>          <p>*** This is a system-generated message. Please do not reply to this message.***</p>          <p>          </p>          <div>&nbsp;</div>          <p>&nbsp;</p>      </body>  </html>' where TemplateName='OnboardingUserEmail';



IF NOT EXISTS(SELECT 1 from DI_M_MailTemplate WHERE TemplateName='CreateAccountPasswordNotificationCommonTemplate')
BEGIN
insert into DI_M_MailTemplate(TemplateName,TemplateDescription,MailFrom,MailTo,MailCC,MailSubject,MailContent,CreatedBy,CreatedDate)
values('CreateAccountPasswordNotificationCommonTemplate','Used for AdhocAccessRequest password letter','securityadmin@spgroup.com.sg','','','IDMS:@@REQUESTID@@ Request for @@APPLICATIONNAME@@ - @@USERDISPLAYNAME@@ (@@EMPLOYEEID@@)','<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">  <html>      <head>          <title></title>          <style type="text/css">              .body {              font-family: Courier New;              font-size: 13px;              font-weight: normal;              margin-left: 5px;              }              .marlef {              margin-left: 40px;              }              .marlef1 {              margin-left: 50px;              }              .marlef2 {              margin-left: 50px;              }          </style>      </head>      <body class="body" _events="[object Object]" jquery191045868327819865823="28" jquery19103030169706004316="28">          <p>PRIVATE &amp; CONFIDENTIAL<br />          -------------------------------------------------------------------<br />          To:&nbsp;&nbsp;@@USERDISPLAYNAME@@</p>          <div class="marlef">@@COMPANY@@<br />          @@DEPT@@ @@BRANCH@@<br />          @@SECTION@@ </div>          <div class="marlef">@@UNIT@@</div>          <p>&nbsp;</p>          <p> Notification of @@APPLICATIONNAME@@ Password</p>          <p>1. The Password for your @@APPLICATIONNAME@@ ID is :-          </p>          <div >Password&nbsp; &nbsp; : @@PASSWORD@@<br /></div>          <p>&nbsp;</p>          <p>2. Please perform the following steps:          </p>          <div >(i) Sign on to system using the user ID (provided in another email) and password given above. Change the password immediately before using the system.<br />          (ii) Password shall be at least 15 alphanumeric in length, and shall not be based on repetition and sequences.<br />          (iii) Password shall not contain the user name.<br />          (iv) Password shall contain at least three of the following categories:<br />          <div class="marlef1">          <div class="marlef2"> - Lower case letters<br />- Upper case letters <br />                   - Numbers<br />          - Special characters</div>          </div>            <p>3.&nbsp;&nbsp;Please keep your password confidential and do not grant permission to any person to use your User ID or password. Ensure that your terminal is not left unattended after you have signed on to the system.</p>          <p>4.&nbsp;Should you have any problem accessing the application, please email to IT Service Desk.</p>          <p>          Regards, <br />          @@REGARDS@@&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</p>          <p>*** This is a system-generated message. Please do not reply to this message.***</p>          <p>          </p>          <div>&nbsp;</div>          <p>&nbsp;</p>      </body>  </html>','System',GETDATE())
END

Update DI_M_MailTemplate Set MailSubject='IDMS:@@REQUESTID@@ Request for @@APPLICATIONNAME@@ @@REQUESTTYPE@@ Completed - @@USERDISPLAYNAME@@ (@@EMPLOYEEID@@)', MailContent ='<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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

<body class="body" _events="[object Object]" jquery19105777307022835423="28" jquery191031130699542479323="28">
    <p>PRIVATE &amp; CONFIDENTIAL<br /> -------------------------------------------------------------------<br /> To:&nbsp;&nbsp;@@USERDISPLAYNAME@@(@@EMPLOYEEID@@ - @@EMPLOYEETYPE@@)<br />
        <div class="marlef">@@COMPANY@@<br /> @@DEPT@@ </div>
        <div class="marlef">@@BRANCH@@ <br /> @@SECTION@@&nbsp;</div>
        <div class="marlef">@@UNIT@@</div>
    </p>

    <p>Your @@APPLICATIONNAME@@ ID has been created :-
        <div class="marlef">User ID &nbsp;&nbsp;&nbsp;&nbsp;: @@USERNAME@@<br /> AccountType : @@ACCOUNTTYPE@@<br/></div>
    </p>
    
    
    <p>Should you have any problem accessing the application, please email to IT Service Desk.</p>
    <p> Regards, <br /> @@REGARDS@@</p>
    <p>&nbsp;</p>
    <p>*** This is a system-generated message. Please do not reply to this message.***</p>
</body>

</html>' where TemplateName='CreateAccountNotificationCommonTemplate';
