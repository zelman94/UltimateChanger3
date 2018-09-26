@echo off
REM This script will upgrade Leyline dongle firmware
REM The dongle must already have Leyline FW in it.
REM The FW image downloaded by this script preserves all BT pairings.

"%~dp0Dongle\DFUmode"
REM If the DFU driver is not installed, DFUmode will fail with the message:

REM 'Could not connect to dongle after changing to DFU mode. Check that the DFU driver is installed.'

REM In this case, the process exit code from DFUmode will be 2.
REM Detect this and attempt to install the driver automatically.
IF %ERRORLEVEL% EQU 0 goto upgrade
IF %ERRORLEVEL% NEQ 2 goto end

ECHO Starting driver installation...
ECHO When prompted if you will allow the program to make changes to your computer, click 'yes'.
ECHO When you are warned that Windows can't verify the publisher of this driver software, click 'Install this driver software anyway'.
call "%~dp0..\Driver\install"

:upgrade
"%~dp0Dongle\Upgrade\Upgrade.exe" "%~dp0..\Firmware\DongleProductionPackage.xml"
IF %ERRORLEVEL% NEQ 0 goto end

:end
