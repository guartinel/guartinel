@echo off 

echo Getting config7248 config status
..\cURL\curl.exe -X POST https://config7248.guartinel.com:5558/replica/status -H "Content-Type: application/json" -d @Requests/replica_status_7248.json
pause