@echo off

REM Installs A service
REM -----------------------------------------
REM Created By DeepIdentity Pte. Ltd.
REM (c) 2011

REM -----------------------------------------
REM CHANGE THESE VARIABLES BELOW TO REFLECT
REM THE SERVICE PARAMETERS
REM Set up the specific variables
REM Assumes that the service is from the same directory as the batch file unless specified as the first parameter

SET PROGRAM_NAME=DI.Chakra.Services.HouseKeeping.exe
SET SERVICE_NAME=DIChakraHouseKeepingService
SET DISPLAY_NAME=DeepIdentity HouseKeeping Service (5.0)
SET INSTALL_DIR=%1

REM -----------------------------------------
REM DO NOT CHANGE ANY TEXT BELOW THIS LINE

SET OLD_PROMPT=%PROMPT%
PROMPT $f$f$f$f

IF "%INSTALL_DIR%"=="" GOTO GET_CURRENTPATH
GOTO INSTALL

:GET_CURRENTPATH
REM Get the directory of the install without destroying the current directory with the new PUSHD command
pushd %0\..
rem cd /d %0\..
SET INSTALL_DIR=%CD%
popd

:INSTALL
REM Now install the service
REM SC does not seem to set the errorlevel, resulting in commands
REM being executed regardless of the success of the SC CREATE.

@echo on
SC CREATE %SERVICE_NAME% binpath= "\"%INSTALL_DIR%\%PROGRAM_NAME%\"" displayname= "%DISPLAY_NAME%" depend= Tcpip start= auto
@IF %ERRORLEVEL% NEQ 0 GOTO ERROR

NET START %SERVICE_NAME%
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

