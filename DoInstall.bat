@ECHO off

IF NOT EXIST "%CD%\DatabaseManagementTool.exe" ECHO File "%CD%\DatabaseManagementTool.exe" should exist. Cannot complete operation.
IF NOT EXIST "%CD%\DatabaseManagementTool.exe" PAUSE
IF NOT EXIST "%CD%\DatabaseManagementTool.exe" exit

REG ADD "hkcu\Software\Classes\SQLServer.Engine.PrimaryDataFile\shell\Attach Database" /ve /t REG_SZ /d "Attach Database" /f
REG ADD "hkcu\Software\Classes\SQLServer.Engine.PrimaryDataFile\shell\Attach Database" /v Icon /t REG_SZ /d "%CD%\DatabaseManagementTool.exe" /f
REG ADD "hkcu\Software\Classes\SQLServer.Engine.PrimaryDataFile\shell\Attach Database" /v MultiSelectModel /t REG_SZ /d Single /f
REG ADD "hkcu\Software\Classes\SQLServer.Engine.PrimaryDataFile\shell\Attach Database\command" /ve /t REG_SZ /d "%CD%\DatabaseManagementTool.exe -attach %%1" /f

ECHO.
ECHO Context menu handlers were added successfully.
PAUSE