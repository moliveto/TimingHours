@echo off
@Set curdir=%~dp0%

rem for /d /r %%d in (*) do dotnet pack
cd %~dp0%/src
for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s /q "%%d"
for /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S "%%G"
for /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S "%%G"

setlocal enabledelayedexpansion
endlocal
cd %~dp0%
dotnet restore 

dotnet nuget locals all -c
dotnet nuget locals --clear all