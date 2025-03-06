:: @echo off
Set CHECK_APP=C:\SVN\Trunk\Projects\Guartinel\Build\Guartinel.CLI\win-x64\Guartinel.CLI.exe
Set SEND=--send --configurationFile="configuration.config"

%CHECK_APP% configured --id:1 --name:Configured %SEND%
