SET "projectRootPath=%cd%"
echo %projectRootPath%
SET "SolutionFile=%projectRootPath%\Solution\Chakra.sln"
echo %SolutionFile%
SET "ProjectPath=%projectRootPath%\IACMWeb\DI.Chakra.Web.csproj"
SET "MYDATE=%DATE:~10,4%%DATE:~7,2%%DATE:~4,2%"
SET "BUILDLocation=C:\SP\V5\Artifacts\%MYDATE%"
IF EXIST "%BUILDLocation%"  rmdir /s /q "%BUILDLocation%"

mkdir %BUILDLocation%

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


pushd %projectRootPath%
if NOT ["%errorlevel%"]==["0"] pause	
echo %BUILDLocation%
pushd %BUILDLocation%

start %BUILDLocation%
pushd %projectRootPath%