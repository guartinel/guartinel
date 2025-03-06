@echo off 

echo Getting config8215 config status
..\cURL\curl.exe -X POST https://config8215.guartinel.com:5558/replica/status -H "Content-Type: application/json" -d @Requests/replica_status_8215.json
pause