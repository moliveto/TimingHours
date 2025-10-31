@echo off

echo.
echo ===============================================
echo Eliminador de carpetas BIN y OBJ - Version Pro
echo ===============================================
echo.

echo Eliminando directorios...

FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"
for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s /q "%%d"

echo Proceso finalizado. & echo.

rem pause