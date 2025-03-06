@echo off 

echo echo Do you want to send maintenance email to every user of the PRODUCTION system?
set INPUT=
set /P INPUT=y/n?: %=%
If /I "%INPUT%"=="y" goto send 
If /I "%INPUT%"=="n" goto exit

:send
echo Sending emails..
cURL\curl.exe -X POST https://backend.guartinel.com:9090/admin/sendMaintenanceEmail -H "Content-Type: application/json" -d @send_maintenance_email.json
pause

:exit
echo BYE
pause
exit


