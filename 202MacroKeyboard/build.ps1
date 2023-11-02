$fqbn = Get-Content FQBN.txt -Raw
arduino-cli compile -b $fqbn