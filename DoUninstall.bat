@ECHO off

REG DELETE "hkcu\Software\Classes\SQLServer.Engine.PrimaryDataFile\shell\Attach Database" /f

ECHO.
ECHO Context menu handlers were deleted successfully.
PAUSE