INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailSubject], [MailContent],  [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate]) 
VALUES (N'OnboardingUserEmail', N'OnboardingUserEmail', N'devteam@deepidentity.com', NULL, N'Request Completed - A1 - @@APPLICATIONNAME@@ (@@EMPLOYEEID@@)', 
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
    <p>PRIVATE & CONFIDENTIAL<br />
    -------------------------------------------------------------------<br />
    To:&nbsp;&nbsp;@@USERDISPLAYNAME@@<br />
    <div class="marlef">
        @@COMPANY@@<br />
        @@DEPT@@ @@DIVISION@@<br />
        @@BRANCH@@ @@SECTION@@</p>
    </div>

    <p>Notification of @@APPLICATIONNAME@@ @@APPLICATIONNAMEFULL@@ User ID and Password</p>


    <p>1. The following @@APPLICATIONNAME@@ ID  has been created :-

    <div class="marlef">
        User ID		  : @@USERNAME@@<br />
        Password	  : @@PASSWORD@@<br />
	      AccountType	  : @@ACCOUNTTYPE@@<br />
        @@RES1@@<br />
        @@RES2@@<br />
    </div>
    </p>
    <p>2. Please perform the following steps:

    <div class="marlef">
        (i) Sign on to system using the login ID and password given above and immediately change the password to one of your choice before using the system.<br />

        (ii) Your password must have the following characteristics:<br />

        <div class="marlef1">
            1) Cannot contain any or all of the users name<br />

            2) At least 8 characters<br />

           
                3) Must contain (satisfy) at least 3 of the following criteria:<br />
           
            <div class="marlef2">
                - Uppercase English char (A to Z)<br />

                - Lowercase English char (a to z)<br />

                - Numbers 0 to 9<br />

                - Non-alphanumeric (or special characters)<br />
            </div>
        </div>

        

    </div>
</p>
    <p>
        3. Please ensure that your password is kept confidential and that you do not grant permission to any person to use your User ID or password.  
		Please also ensure that your terminal is not left unattended after you have signed on to the system
    </p>

    <p>4. Should you have any query, please do not hesitate to contact Helpdesk at 6378 8111.</p>
</body>
</html>'
,'IACMAdmin',GETDATE(),'IACMAdmin',NULL)

GO