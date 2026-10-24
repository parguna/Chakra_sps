@echo off
REM Delete the service

SET SERVICE_NAME=DIChakraAttestationService
NET STOP %SERVICE_NAME%
SC delete %SERVICE_NAME%

echo
