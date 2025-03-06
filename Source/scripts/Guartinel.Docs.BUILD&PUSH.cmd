@echo off
echo Starting guartinel1/docs.website  build.

for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YY=%dt:~2,2%" & set "YYYY=%dt:~0,4%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "HH=%dt:~8,2%" & set "Min=%dt:~10,2%" & set "Sec=%dt:~12,2%"

set "fullstamp=%YYYY%-%MM%-%DD%-%HH%-%Min%"

docker build  -f ..\Guartinel.Docs\guartinel.docs.prod.docker -t guartinel1/docs.website:%fullstamp% -t guartinel1/docs.website ..\Guartinel.Docs\
docker images -q > tmp
set /p idWithCRLF=<tmp
set lastImageId=%idWithCRLF:/13/10=%
del tmp

echo echo Push image guartinel1/docs.website to repository with tag docs.website:%fullstamp%?(Y/N)
set INPUT=
set /P INPUT=y/n?: %=%
If /I "%INPUT%"=="y" goto push_to_repo 
If /I "%INPUT%"=="n" goto exit

:push_to_repo
echo Pushing image to repo
docker push guartinel1/docs.website:%fullstamp%
docker push guartinel1/docs.website

:exit
echo BYE
pause
exit

