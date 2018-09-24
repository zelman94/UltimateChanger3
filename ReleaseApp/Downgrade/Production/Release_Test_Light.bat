@ECHO OFF
:begin 

REM Program ATSAM4S using Segger J-Flash
REM -------------------------------------------
Controller\JFlashARM.exe -openprj"Controller\AT91SAM4S16C.jflash" -open..\Firmware\Controller.bin,0x400000 -auto -startapp -exit
Controller\JLink.exe Controller\ResetScript

REM Apply production/calibration settings
REM -------------------------------------
REM Mcu.Production.SerialNumber (must be != 0)
..\CLI\NautilusCLI.exe ..\Nautilus\Developer.xml Nautilus_application "VID_1a17&PID_6004" "SettingsInterface.Write Target=0 Collection=0 Key=0 Value=1"

REM Mcu.Calibration.AdcCalibrationVoltage (all values must be != 0)
..\CLI\NautilusCLI.exe ..\Nautilus\Developer.xml Nautilus_application "VID_1a17&PID_6004" "SettingsInterface.WriteArray Target=0 Collection=1 Key=3 Offset=0 Count=3 Value={3500, 1, 3800, 0, ...}"

REM Mcu.Calibration.AdcCalibrationValue (all values must be != 0)
..\CLI\NautilusCLI.exe ..\Nautilus\Developer.xml Nautilus_application "VID_1a17&PID_6004" "SettingsInterface.WriteArray Target=0 Collection=1 Key=4 Offset=0 Count=3 Value={3045, 1, 3300, 0, ...}"

REM Program BC5-MM using BlueFlash and PSCli
REM -------------------------------------------
REM Programming Bluetooth Application using BlueFlash
Bluetooth\BlueFlashCmd.exe -trans SPITRANS=USB ..\Firmware\leyline_bluetooth_merge
IF %ERRORLEVEL% NEQ 0 GOTO fail

REM Programming Bluetooth Application using PSCli
Bluetooth\pscli.exe -trans SPITRANS=USB merge ..\Firmware\leyline_bluetooth_production.psr
IF %ERRORLEVEL% NEQ 0 GOTO fail

REM Resetting BlueCore using PSCli
Bluetooth\pscli.exe -trans SPITRANS=USB cold_reset
IF %ERRORLEVEL% NEQ 0 GOTO fail

REM Resetting BlueCore using PSCli
Bluetooth\pscli.exe -trans SPITRANS=USB cold_reset
IF %ERRORLEVEL% NEQ 0 GOTO fail

ECHO ------------------- PROGRAMMING SUCCEDED -------------------
GOTO end

REM Failure during programming
:fail
ECHO.
ECHO.
ECHO.
ECHO !!!!!!!!!!!!!!!!!!! PROGRAMMING FAILED !!!!!!!!!!!!!!!!!!!
GOTO end


REM Program End
:end
ECHO.
ECHO.
ECHO.
ECHO Programming End

PAUSE
CLS
