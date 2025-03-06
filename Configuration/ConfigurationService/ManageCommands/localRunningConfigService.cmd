@echo off 

echo Getting config8215 config status
..\cURL\curl.exe -X POST http://localhost:5000/replica/status -H "Content-Type: application/json" -d @Requests/localRunningConfigService.json