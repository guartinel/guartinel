@echo off
echo Starting guartinel1/websitechecker.service  build for !!!PRODUCTION!!! environment.

for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YY=%dt:~2,2%" & set "YYYY=%dt:~0,4%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "HH=%dt:~8,2%" & set "Min=%dt:~10,2%" & set "Sec=%dt:~12,2%"

set "fullstamp=%YYYY%-%MM%-%DD%-%HH%-%Min%"

docker build  -f ..\Guartinel.Service.WebsiteChecker\docker\websitechecker.service.docker ..\ -t guartinel1/websitechecker.service:prod-%fullstamp% -t guartinel1/websitechecker.service:prod
docker images -q > tmp
set /p idWithCRLF=<tmp
set lastImageId=%idWithCRLF:/13/10=%
del tmp

echo echo Push image guartinel1/websitechecker.service to repository with tag prod-%fullstamp%?(Y/N)
set INPUT=
set /P INPUT=y/n?: %=%
If /I "%INPUT%"=="y" goto push_to_repo 
If /I "%INPUT%"=="n" goto exit

:push_to_repo
echo Pushing image to repo
docker push guartinel1/websitechecker.service:prod-%fullstamp%
docker push guartinel1/websitechecker.service:prod

:exit
echo BYE
pause
exit

