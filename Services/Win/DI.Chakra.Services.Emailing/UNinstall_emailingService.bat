@echo off
REM Delete the service

SET SERVICE_NAME=DIChakraEmailingService
NET STOP %SERVICE_NAME%
SC delete %SERVICE_NAME%

echo
