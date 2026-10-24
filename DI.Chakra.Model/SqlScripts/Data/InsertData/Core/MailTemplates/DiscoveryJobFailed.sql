
INSERT [dbo].[DI_M_MailTemplate] ([TemplateName], [TemplateDescription], [MailFrom], [MailTo], [MailSubject], [MailContent], [CreatedBy], [CreatedDate], [LastModifiedBy], [LastModifiedDate])
VALUES (N'DiscoveryJobFailed', N'Discovery Job Failed', N'devteam@deepidentity.com', NULL, N'Discovery Job Failed - @@ENDPOINTNAME@@ (@@PROCESSNAME@@) ',
N'<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <title></title>
    <style type="text/css">
        .style1
        {
            width: 285px;
        }
         .title
        {
            font-family:"lucida grande", tahoma, verdana, arial, sans-serif;
            font-size:13px;
            color:#1c2a47;
            font-weight:bold;
        }
         .content
        {
            padding-left:10px;
            font-family:"lucida grande", tahoma, verdana, arial, sans-serif;
            font-size:14px;
            color:#000;
            
        }
        .table_results td {
	    border-right: 1px solid #F8F8F8;
	    border-bottom: 1px solid #EEE;
	    padding: 2px 2px;
	    color: #000;
        }

    .table_results {
	font: normal 12px auto Verdana, Arial, Helvetica, sans-serif;
	color: #000;
	border-left: 1px solid #C1DAD7;
    }
    </style>
</head>
<body>
    <div>
        <p>
        <span class="title">Dear Security Operations,</span></p>
        
        <p>
        <span class="title">Discovery Process Failed</span></p>
        
        <p>
        <span class="title">Error Message:</span></p><hr />
        
        <p>        
        <span class="content">@@ERRORMESSAGE@@</span></p>
        
        <p>
        <span class="title">Endpoint system details:</span></p><hr />
        
       
    </div>
    <table class="table_results" cellpadding="3" cellspacing="3" style="width:100%;">
       
        <tr>
            <td class="style1">
                <b>Application Type:</b></td>
            <td>@@ENDPOINTTYPE@@</td>
           
        </tr>   
		<tr>
            <td class="style1">
                <b>Application:</b></td>
            <td>@@ENDPOINTNAME@@</td>
           
        </tr>             
        <tr>
            <td class="style1">
                <b>Process Name: </b></td>
            <td>@@PROCESSNAME@@</td>
          
        </tr>
         
    </table>
    
</body>
</html>', N'System', GETDATE(), NULL, NULL)

GO
