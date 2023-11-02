Param(
    [string]$Port = "COM1"
)
$fqbn = Get-Content FQBN.txt -Raw
arduino-cli compile -b $fqbn -u -p $Port -P serialupdi -t