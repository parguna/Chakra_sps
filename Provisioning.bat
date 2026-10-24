SET "projectRootPath=%cd%"
echo %projectRootPath%
SET "SolutionFile=%projectRootPath%\Solution\Chakra.sln"
echo %SolutionFile%
SET "ProjectPath=%projectRootPath%\IACMWeb\DI.Chakra.Web.csproj"
SET "MYDATE=%DATE:~10,4%%DATE:~7,2%%DATE:~4,2%"
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
set MSBuildPath="C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
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
SET "Provisioning=%projectRootPath%\Services\Win\DI.Chakra.Services.Provisioning\DI.Chakra.Services.Provisioning.csproj"
SET "OutputProvisioningPath=%BUILDLocation%\tempBuild\Provisioning"
SET "IACMProvisioning=%BUILDLocation%\App\Provisioning"
%MSBuildPath% %Provisioning% /t:clean /t:Rebuild  /ds /v:q /m /p:Configuration=Release /p:OutDir=%IACMProvisioning% /p:OutputPath=%OutputProvisioningPath%    /p:DefineTrace=false /p:Platform="Any CPU"   /p:DebugSymbols=%ExcludePDB% /p:DebugType=full 
pushd %BUILDLocation%\App
rename Provisioning "Provisioning Service"
pushd %projectRootPath%
if NOT ["%errorlevel%"]==["0"] pause


if ["%ExcludePDB%"]==["true"] DEL /S /Q *.pdb *.PDB
if NOT ["%errorlevel%"]==["0"] pause	
DEL /S /Q *.config *.CONFIG *ConnectionString.config

for /d /r . %%d in (tempBuild) do @if exist "%%d" rd /s/q "%%d"
pushd %projectRootPath%
COLOR 07


start %BUILDLocation%
pushd %projectRootPath%