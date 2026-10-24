SET APPLICATION_NAME=@__WEBAPPLICATIONNAME@
SET APPL_POOL_NAME=@__WEBAPPLPOOLNAME@
SET PHYSICAL_PATH=@__WEBPHYSICALPATH@


SET OLD_PROMPT=%PROMPT%
PROMPT $f$f$f$f

@echo on
C:\Windows\System32\inetsrv\appcmd add app /site.name:"Default Web Site" /path:/%APPLICATION_NAME% /physicalPath:%PHYSICAL_PATH%
@IF %ERRORLEVEL% NEQ 0 GOTO ERROR

C:\Windows\System32\inetsrv\appcmd ADD apppool /name:%APPL_POOL_NAME% /managedRuntimeVersion:4.0  /managedPipelineMode:Integrated /enable32BitAppOnWin64:true
@IF %ERRORLEVEL% NEQ 0 GOTO ERROR


C:\Windows\System32\inetsrv\appcmd set app /app.name:"Default Web Site/%APPLICATION_NAME%"  /applicationPool:%APPL_POOL_NAME%
@IF %ERRORLEVEL% NEQ 0 GOTO ERROR

:GOOD
GOTO END

:ERROR
@echo off
echo ))))-----------------------------------------------
echo ))))An error occurred.
echo ))))-----------------------------------------------

:END
PROMPT %OLD_PROMPT%

