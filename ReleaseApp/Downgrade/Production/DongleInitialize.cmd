@echo off
setlocal
REM This script will download Leyline dongle firmware to a BTD800 dongle, which is
REM fresh from SeCom production - i.e. it does NOT already have FittingLINK firmware in it.
REM The FW image downloaded by this script clears all BT pairings.

"%~dp0Dongle\BTD800DFUmode"
REM If the DFU driver is not installed, BTD800DFUmode will fail with the message:

REM Could not connect to dongle after changing to DFU mode.
REM Check that the DFU driver is installed.

REM The return code from the process will be 2.
REM Detect this and attempt to install the driver automatically.
IF %ERRORLEVEL% EQU 0 goto upgrade
IF %ERRORLEVEL% NEQ 2 goto end
ECHO Starting driver installation...
ECHO When prompted if you will allow the program to make changes to your computer, click 'yes'.
ECHO When you are warned that Windows can't verify the publisher of this driver software, click 'Install this driver software anyway'.
CALL "%~dp0..\Driver\install"

:upgrade
REM Check if dongle has a valid BDADDR
"%~dp0Bluetooth\pscli" -trans CSRTRANS=USB query bdaddr.psr "%~dp0Dongle\bdaddr.psq"
if %ERRORLEVEL% NEQ 0 goto end

for /f %%i in ("bdaddr.psr") do set size=%%~zi
if %size% GTR 0 goto download

REM Empty PSR file - BD_ADDR not correctly set.
echo BTD800 dongle has no unique BD_ADDR
goto failure

:download
"%~dp0Dongle\CsrUpgrade" "%~dp0..\Firmware\leyline_wireless_usb_dongle_production.dfu"
timeout /t 5

"%~dp0Dongle\Upgrade\Upgrade.exe" "%~dp0..\Firmware\DongleUpgradePackage.xml"
timeout /t 5

REM Check if dongle has a valid BDADDR after FittingLINK FW installation
"%~dp0Dongle\DFUmode"
IF %ERRORLEVEL% NEQ 0 goto end

"%~dp0Bluetooth\pscli" -trans CSRTRANS=USB query bdaddr_new.psr "%~dp0Dongle\bdaddr.psq"
if %ERRORLEVEL% NEQ 0 goto end

for /f %%i in ("bdaddr_new.psr") do set size=%%~zi
if %size% GTR 0 goto done

REM Empty PSR file - BD_ADDR not correctly set.
echo FittingLINK dongle has no unique BD_ADDR

:done
REM reboot using pscli. Suppress output because it will always report HCI communication failure.
"%~dp0Bluetooth\pscli" -trans CSRTRANS=USB cold_reset >nul 2>&1

:end
