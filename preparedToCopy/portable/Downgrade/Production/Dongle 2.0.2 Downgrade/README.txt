==========================
= Dongle 2.0.2 Downgrade =
==========================

This recipe downgrades a FittingLINK Bluetooth adaptor to a 14.2 production release.

* FW version becomes 2.0.2.
* BD_ADDR is the same.
* Any pairing is deleted.

> ..\Dongle\DFUMode.exe
> ..\Dongle\CsrUpgrade.exe leyline_wireless_usb_dongle_202_full.dfu

The CsrUpgrade.exe program fails in the manifest phase of the downgrade.

[MANIFEST]
( 10%) Attempting to return to run-time mode. Trying to open ...
(100%) USB error: could not be accessed. Device ready. ...

This is due to the fact that the device changes USB serial number in the downgrade process,
and this confuses Windows.

