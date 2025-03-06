@echo off 

echo Syncing slaves to master config8215 
..\cURL\curl.exe -X POST https://config8215.guartinel.com:5558/replica/sync -H "Content-Type: application/json" -d @Requests/replica_sync_8215.json
pause