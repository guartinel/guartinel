@echo off 

echo Getting config1903 config status
..\cURL\curl.exe -X POST https://config1903.guartinel.com:5558/replica/status -H "Content-Type: application/json" -d @Requests/replica_status_1903.json
pause