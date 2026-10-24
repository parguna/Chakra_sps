SET "projectRootPath=%cd%"
echo %projectRootPath%
SET "SolutionFile=%projectRootPath%\Solution\Chakra.sln"
echo %SolutionFile%
SET "ProjectPath=%projectRootPath%\IACMWeb\DI.Chakra.Web.csproj"
SET "MYDATE=10232026"
SET "BUILDLocation=C:\SP\V5\Artifacts\V5\Prod\IDMS_%MYDATE%"
IF EXIST "%BUILDLocation%"  rmdir /s /q "%BUILDLocation%"

mkdir %BUILDLocation%

SET "DBPath=%BUILDLocation%\DB\"
mkdir %DBPath%

Xcopy "%projectRootPath%\DI.Chakra.DBClient\DeploymentScript\" %DBPath% /E /H /C /I

SET "IACMWeb=%BUILDLocation%\Web\"

mkdir %IACMWeb%

SET "OutputWebPath=%BUILDLocation%\tempBuild\"
set MSBuildEmitSolution=1
set MSBuildPath="C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
set "ExcludePDB=true"
set "includeConfig=false"
@ECHO off
cls
:start
ECHO.
ECHO 1. IncludePDB Press 1
ECHO 2. ExcludePDB
set choice= 
set /p choice=Type the number to print text.


if '%choice%'=='1' set "ExcludePDB=false"
if '%choice%'=='2' (
set "ExcludePDB=true"
)


echo ExcludePDB:%ExcludePDB%

COLOR 09
%MSBuildPath% %ProjectPath% /t:clean /t:Rebuild /t:WebPublish /p:WebPublishMethod=FileSystem /p:publishUrl=%IACMWeb% /ds /v:q /m /p:Configuration=Release /p:OutputPath=%OutputWebPath%    /p:DefineTrace=false /p:Platform="Any CPU" /ignoreprojectextensions:.config,ConnectionString.config /p:DebugSymbols=%ExcludePDB% /p:DebugType=full /p:ExcludeGeneratedDebugSymbol=%ExcludePDB%

if NOT ["%errorlevel%"]==["0"] pause
COLOR 09
SET "ProjectPath=%projectRootPath%\DI.Chakra.DomainApi\DI.Chakra.DomainApi.csproj"
SET "OutputWebAPIPath=%BUILDLocation%\tempBuild\DomainAPI"
SET "IACMWebAPI=%BUILDLocation%\App\DomainAPI"
%MSBuildPath% %ProjectPath% /t:clean /t:Rebuild /t:WebPublish /p:WebPublishMethod=FileSystem /p:publishUrl=%IACMWebAPI% /ds /v:q /m /p:Configuration=Release /p:OutputPath=%OutputWebAPIPath%    /p:DefineTrace=false /p:Platform="Any CPU" /ignoreprojectextensions:.config,ConnectionString.config  /p:DebugSymbols=%ExcludePDB% /p:DebugType=full /p:ExcludeGeneratedDebugSymbol=%ExcludePDB%
pushd %BUILDLocation%\App
rename DomainAPI "Domain Api"
pushd %projectRootPath%
if NOT ["%errorlevel%"]==["0"] pause
COLOR 09
SET "ProjectPath=%projectRootPath%\DI.Chakra.DomainService\DI.Chakra.DomainService.csproj"
SET "OutputWCFPath=%BUILDLocation%\tempBuild\DomainServices"
SET "IACMWCF=%BUILDLocation%\App\DomainServices"
%MSBuildPath% %ProjectPath% /t:clean /t:Rebuild /t:WebPublish /p:WebPublishMethod=FileSystem /p:publishUrl=%IACMWCF% /ds /v:q /m /p:Configuration=Release /p:OutputPath=%OutputWCFPath%    /p:DefineTrace=false /p:Platform="Any CPU" /ignoreprojectextensions:.config,ConnectionString.config  /p:DebugSymbols=%ExcludePDB% /p:DebugType=full /p:ExcludeGeneratedDebugSymbol=%ExcludePDB%
pushd %BUILDLocation%\App
rename DomainServices "Domain Service"
pushd %projectRootPath%
if NOT ["%errorlevel%"]==["0"] pause
COLOR 09
SET "ProjectPath=%projectRootPath%\Workflows\DI.Chakra.Workflow.Services\DI.Chakra.Workflow.Services.csproj"
SET "OutputWorkflowServicesPath=%BUILDLocation%\tempBuild\WorkflowServices"
SET "IACMWorkflowServices=%BUILDLocation%\App\WorkflowServices"
%MSBuildPath% %ProjectPath% /t:clean /t:Rebuild /t:WebPublish /p:WebPublishMethod=FileSystem /p:publishUrl=%IACMWorkflowServices% /ds /v:q /m /p:Configuration=Release /p:OutputPath=%OutputWorkflowServicesPath%    /p:DefineTrace=false /p:Platform="Any CPU" /p:DebugSymbols=%ExcludePDB% /p:DebugType=full /p:ExcludeGeneratedDebugSymbol=%ExcludePDB%
pushd %BUILDLocation%\App
rename WorkflowServices "Workflow Service"
pushd %projectRootPath%
if NOT ["%errorlevel%"]==["0"] pause

@echo off
echo ))))-----------------------------------------------
echo ))))WEB Project done
echo ))))-----------------------------------------------
COLOR 09
SET "AttestationPath=%projectRootPath%\Services\Win\DI.Chakra.Services.Attestation\DI.Chakra.Services.Attestation.csproj"
SET "OutputAttestationPath=%BUILDLocation%\tempBuild\Attestation"
SET "IACMAttestation=%BUILDLocation%\App\Attestation"
%MSBuildPath% %AttestationPath% /t:clean /t:Rebuild  /ds /v:q /m /p:Configuration=Release /p:OutDir=%IACMAttestation% /p:OutputPath=%OutputAttestationPath%    /p:DefineTrace=false /p:Platform="Any CPU"   /p:DebugSymbols=%ExcludePDB% /p:DebugType=full 
pushd %BUILDLocation%\App
rename Attestation "Attestation Service"
pushd %projectRootPath%
if NOT ["%errorlevel%"]==["0"] pause 

SET "DiscoveryPath=%projectRootPath%\Services\Win\DI.Chakra.Services.Discovery\DI.Chakra.Services.Discovery.csproj"
SET "OutputDiscoveryPath=%BUILDLocation%\tempBuild\Discovery"
SET "IACMDiscovery=%BUILDLocation%\App\Discovery"
%MSBuildPath% %DiscoveryPath% /t:clean /t:Rebuild  /ds /v:q /m /p:Configuration=Release /p:OutDir=%IACMDiscovery% /p:OutputPath=%OutputDiscoveryPath%    /p:DefineTrace=false /p:Platform="Any CPU"   /p:DebugSymbols=%ExcludePDB% /p:DebugType=full 
pushd %BUILDLocation%\App
rename Discovery "Discovery Service1"
COLOR 03
xcopy "%BUILDLocation%\App\Discovery Service1"  "%BUILDLocation%\App\Discovery Service2" /i /S /E
if NOT ["%errorlevel%"]==["0"] pause
pushd %projectRootPath%
if NOT ["%errorlevel%"]==["0"] pause
COLOR 09
SET "Emailing=%projectRootPath%\Services\Win\DI.Chakra.Services.Emailing\DI.Chakra.Services.Emailing.csproj"
SET "OutputEmailingPath=%BUILDLocation%\tempBuild\Emailing"
SET "IACMEmailing=%BUILDLocation%\App\Emailing"
%MSBuildPath% %Emailing% /t:clean /t:Rebuild  /ds /v:q /m /p:Configuration=Release /p:OutDir=%IACMEmailing% /p:OutputPath=%OutputEmailingPath%    /p:DefineTrace=false /p:Platform="Any CPU"   /p:DebugSymbols=%ExcludePDB% /p:DebugType=full 
pushd %BUILDLocation%\App
rename Emailing "Email Service"
pushd %projectRootPath%
if NOT ["%errorlevel%"]==["0"] pause
COLOR 09
SET "Provisioning=%projectRootPath%\Services\Win\DI.Chakra.Services.Provisioning\DI.Chakra.Services.Provisioning.csproj"
SET "OutputProvisioningPath=%BUILDLocation%\tempBuild\Provisioning"
SET "IACMProvisioning=%BUILDLocation%\App\Provisioning"
%MSBuildPath% %Provisioning% /t:clean /t:Rebuild  /ds /v:q /m /p:Configuration=Release /p:OutDir=%IACMProvisioning% /p:OutputPath=%OutputProvisioningPath%    /p:DefineTrace=false /p:Platform="Any CPU"   /p:DebugSymbols=%ExcludePDB% /p:DebugType=full 
pushd %BUILDLocation%\App
rename Provisioning "Provisioning Service"
pushd %projectRootPath%
if NOT ["%errorlevel%"]==["0"] pause


SET "Misc=%projectRootPath%\Services\Win\DI.Chakra.Services.Misc\DI.Chakra.Services.Misc.csproj"
SET "OutputMiscPath=%BUILDLocation%\tempBuild\Misc"
SET "IACMMisc=%BUILDLocation%\App\Misc"
%MSBuildPath% %Misc% /t:clean /t:Rebuild  /ds /v:q /m /p:Configuration=Release /p:OutDir=%IACMMisc% /p:OutputPath=%OutputMiscPath%    /p:DefineTrace=false /p:Platform="Any CPU"   /p:DebugSymbols=%ExcludePDB% /p:DebugType=full 
pushd %BUILDLocation%\App
rename Misc "Misc Service"
pushd %projectRootPath%
if NOT ["%errorlevel%"]==["0"] pause	
echo %BUILDLocation%
pushd %BUILDLocation%


if ["%ExcludePDB%"]==["true"] DEL /S /Q *.pdb *.PDB
if NOT ["%errorlevel%"]==["0"] pause	
DEL /S /Q *.config *.CONFIG *ConnectionString.config

for /d /r . %%d in (tempBuild) do @if exist "%%d" rd /s/q "%%d"
pushd %projectRootPath%
COLOR 07


start %BUILDLocation%
pushd %projectRootPath%