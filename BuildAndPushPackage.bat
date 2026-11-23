cd .\src

@ECHO off
cls

FOR /d /r . %%d in (bin,obj) DO (
	IF EXIST "%%d" (		 	 
		ECHO %%d | FIND /I "\node_modules\" > Nul && ( 
			ECHO.Skipping: %%d
		) || (
			ECHO.Deleting: %%d
			rd /s/q "%%d"
		)
	)
)

@ECHO.Building solution...
@dotnet restore
@IF ERRORLEVEL 1 GOTO :failed
@dotnet build -c Release
@IF ERRORLEVEL 1 GOTO :failed
@ECHO.Building nanoFramework library...
@REM dotnet cannot build the nfproj, so it is not part of the solution. It needs nuget.exe on the PATH and the MSBuild
@REM of Visual Studio or the Build Tools with the nanoFramework extension. Without the extension, point the environment
@REM variable NanoFrameworkProjectSystemPath to its extracted MSBuild files, the folder name has to end with a backslash.
@SET "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
@SET "MSBUILD="
@REM -all also returns instances the installer marks as incomplete, their MSBuild works nevertheless.
@FOR /f "usebackq delims=" %%m IN (`"%VSWHERE%" -all -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) DO @SET "MSBUILD=%%m"
@IF NOT DEFINED MSBUILD (
	@ECHO.vswhere found no MSBuild, install Visual Studio or the Visual Studio Build Tools.
	@GOTO :failed
)
@nuget.exe restore .\SparkplugNet.nanoFramework\packages.config -PackagesDirectory ..\packages
@IF ERRORLEVEL 1 GOTO :failed
@"%MSBUILD%" .\SparkplugNet.nanoFramework\SparkplugNet.nanoFramework.nfproj -p:Configuration=Release -nologo -v:minimal
@IF ERRORLEVEL 1 GOTO :failed
@REM The nanoFramework package gets the version GitVersion computes for SparkplugNet. FOR /f turns an unescaped = into
@REM a space, so the command below must not contain one.
@SET "PACKAGE_VERSION="
@FOR /f "usebackq delims=" %%v IN (`dotnet msbuild .\SparkplugNet\SparkplugNet.csproj -nologo -t:GetVersion -getProperty:PackageVersion`) DO @SET "PACKAGE_VERSION=%%v"
@IF NOT DEFINED PACKAGE_VERSION GOTO :failed
@nuget.exe pack .\SparkplugNet.nanoFramework\SparkplugNet.nanoFramework.nuspec -Properties version=%PACKAGE_VERSION% -OutputDirectory .\SparkplugNet.nanoFramework\bin\Release
@IF ERRORLEVEL 1 GOTO :failed
@ECHO.Build successful.
@REM Keep the echo off here, otherwise cmd prints the expanded API keys into the console output.
@REM nuget.org has a symbol server, so pushing the *.nupkg uploads the *.snupkg next to it as well.
@REM The GitHub feed offers PackagePublish only, there is no symbol server to push the *.snupkg to.
@FOR %%p IN (SparkplugNet SparkplugNet.Extensions.Json SparkplugNet.nanoFramework) DO (
	@ECHO.Pushing %%p...
	@dotnet nuget push ".\%%p\bin\Release\*.nupkg" -s "nuget.org" --skip-duplicate -k "%NUGET_API_KEY%"
	@IF ERRORLEVEL 1 GOTO :failed
	@dotnet nuget push ".\%%p\bin\Release\*.nupkg" -s "github" --skip-duplicate --api-key "%GITHUB_API_KEY%"
	@IF ERRORLEVEL 1 GOTO :failed
)
@ECHO.
@ECHO.Upload success. Press any key to exit.
@PAUSE > Nul
@EXIT /B 0

:failed
@ECHO.
@ECHO.The step above failed, nothing after it ran. Press any key to exit.
@PAUSE > Nul
@EXIT /B 1
