Param(
    [string]$Port = "COM1"
)

arduino-cli compile -b megaTinyCore:megaavr:atxy2:chip=202,clock=20internal,bodvoltage=4v2,bodmode=disabled,eesave=disable,millis=enabled,startuptime=8,wiremode=mors,printf=default,WDTtimeout=disabled,WDTwindow=disabled -u -p $Port -P serialupdi -t