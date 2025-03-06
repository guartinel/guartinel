@echo off
cls

Set LABEL=%1
Set VERSION_NUMBER=1.0.2.143

Set ROOT_PATH=C:\SVN\Trunk\Projects\Guartinel
Set SOURCE_ROOT_PATH=%ROOT_PATH%\Source
Set TARGET_ROOT_PATH=%ROOT_PATH%\Build

Set MS_BUILD=msbuild /property:AssemblyVersion=%VERSION_NUMBER% /property:PackageVersion=%VERSION_NUMBER% /property:Version=%VERSION_NUMBER% /property:ApplicationVersion=%VERSION_NUMBER% /property:FileVersion=%VERSION_NUMBER% /property:ProductVersion=%VERSION_NUMBER% /property:Configuration=Release;TargetFrameworkVersion=v4.6.2 /verbosity:quiet /nologo
Set CORE_BUILD=dotnet build /p:Version=%VERSION_NUMBER% /p:FileVersion=%VERSION_NUMBER% /p:ProductVersion=%VERSION_NUMBER% --verbosity:quiet --configuration:Release --framework:netcoreapp2.0 /nologo
:: Set CORE_BUILD=dotnet publish --verbosity:quiet --configuration:Release --framework:netcoreapp2.0 /nologo
Set CORE_BUILD_2_1=dotnet build /p:Version=%VERSION_NUMBER% /p:FileVersion=%VERSION_NUMBER% /p:ProductVersion=%VERSION_NUMBER% --verbosity:quiet --configuration:Release --framework:netcoreapp2.1 /nologo
Set CORE_PUBLISH_2_1=dotnet publish /p:Version=%VERSION_NUMBER% /p:FileVersion=%VERSION_NUMBER% /p:ProductVersion=%VERSION_NUMBER% --verbosity:quiet --configuration:Release --framework:netcoreapp2.1 /nologo

Set COPY=xcopy /q /s /y

echo ----------------------------------------------------
echo                Build Guartinel
echo ----------------------------------------------------

echo --- Cleaning up...

If Exist %TARGET_ROOT_PATH% (
   Del /f /s /q %TARGET_ROOT_PATH%\*.* >nul
   RmDir /q /s %TARGET_ROOT_PATH%
)

If Not "%LABEL%"=="" Goto :%LABEL%

:CLI

echo --------------------------
echo      CLI
echo --------------------------

echo -------------------
echo   Guartinel.CLI
echo -------------------
MkDir %TARGET_ROOT_PATH%\Guartinel.CLI
%CORE_BUILD% --runtime:win-x64 %SOURCE_ROOT_PATH%\Guartinel.CLI\Guartinel.CLI.csproj --output:%TARGET_ROOT_PATH%\Guartinel.CLI\win-x64
%CORE_BUILD% --runtime:win-x86 %SOURCE_ROOT_PATH%\Guartinel.CLI\Guartinel.CLI.csproj --output:%TARGET_ROOT_PATH%\Guartinel.CLI\win-x86
%CORE_BUILD% --runtime:linux-x64 %SOURCE_ROOT_PATH%\Guartinel.CLI\Guartinel.CLI.csproj --output:%TARGET_ROOT_PATH%\Guartinel.CLI\linux-x64
:: %COPY% %SOURCE_ROOT_PATH%\Guartinel.CLI.Win\bin\Release\PublishOutput\*.* %TARGET_ROOT_PATH%\Guartinel.CLI.Win

If Not "%LABEL%"=="" Goto :FINISH

echo -------------------
echo   Guartinel.CLI.Win
echo -------------------
:: MkDir %TARGET_ROOT_PATH%\Guartinel.CLI.Win
:: %CORE_BUILD% --runtime:win-x86 %SOURCE_ROOT_PATH%\Guartinel.CLI.Win\Guartinel.CLI.Win.csproj --output:%TARGET_ROOT_PATH%\Guartinel.CLI.Win --self-contained
:: %COPY% %SOURCE_ROOT_PATH%\Guartinel.CLI.Win\bin\Release\PublishOutput\*.* %TARGET_ROOT_PATH%\Guartinel.CLI.Win
:: --------------------------

If Not "%LABEL%"=="" Goto :FINISH

:WatcherServer

echo --------------------------
echo     Watcher Server
echo --------------------------

:: MkDir %TARGET_ROOT_PATH%\Guartinel.WatcherServer
%MS_BUILD% /property:OutDir=%TARGET_ROOT_PATH%\Guartinel.WatcherServer %SOURCE_ROOT_PATH%\Guartinel.WatcherServer\Guartinel.WatcherServer.csproj
:: %COPY% %SOURCE_ROOT_PATH%\Guartinel.WatcherServer\bin\Release\*.* %TARGET_ROOT_PATH%\Guartinel.WatcherServer

If Not "%LABEL%"=="" Goto :FINISH

:Service.WebsiteChecker

echo --------------------------
echo     Website Checker Service
echo --------------------------

MkDir %TARGET_ROOT_PATH%\Guartinel.Service.WebsiteChecker

%CORE_BUILD_2_1% --runtime:win-x64 %SOURCE_ROOT_PATH%\Guartinel.Service.WebsiteChecker\Guartinel.Service.WebsiteChecker.csproj --output:%TARGET_ROOT_PATH%\Guartinel.Service.WebsiteChecker\win-x64
%CORE_PUBLISH_2_1% --runtime:win-x64 %SOURCE_ROOT_PATH%\Guartinel.Service.WebsiteChecker\Guartinel.Service.WebsiteChecker.csproj --output:%TARGET_ROOT_PATH%\Guartinel.Service.WebsiteChecker\win-x64
%CORE_BUILD_2_1% --runtime:linux-x64 %SOURCE_ROOT_PATH%\Guartinel.Service.WebsiteChecker\Guartinel.Service.WebsiteChecker.csproj --output:%TARGET_ROOT_PATH%\Guartinel.Service.WebsiteChecker\linux-x64
%CORE_PUBLISH_2_1% --runtime:linux-x64 %SOURCE_ROOT_PATH%\Guartinel.Service.WebsiteChecker\Guartinel.Service.WebsiteChecker.csproj --output:%TARGET_ROOT_PATH%\Guartinel.Service.WebsiteChecker\linux-x64

:: %COPY% %SOURCE_ROOT_PATH%\Guartinel.Service.WebsiteChecker\bin\Release\*.* %TARGET_ROOT_PATH%\Guartinel.Service.WebsiteChecker

If Not "%LABEL%"=="" Goto :FINISH

:FINISH

echo ----------------------------------------------------
echo                Guartinel build finished.
echo ----------------------------------------------------
