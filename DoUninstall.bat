@ECHO off

REG DELETE "hkcu\Software\Classes\SQLServer.Engine.PrimaryDataFile\shell\Open with scla" /f

ECHO.
ECHO Context menu handlers were deleted successfully.
PAUSE