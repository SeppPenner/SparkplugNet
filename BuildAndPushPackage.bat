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
@cd .\SparkplugNet\bin\Release
@IF ERRORLEVEL 1 GOTO :failed
@ECHO.Build successful.
@REM Keep the echo off here, otherwise cmd prints the expanded API keys into the console output.
@REM nuget.org has a symbol server, so pushing the *.nupkg uploads the *.snupkg next to it as well.
@dotnet nuget push *.nupkg -s "nuget.org" --skip-duplicate -k "%NUGET_API_KEY%"
@IF ERRORLEVEL 1 GOTO :failed
@REM The GitHub feed offers PackagePublish only, there is no symbol server to push the *.snupkg to.
@dotnet nuget push *.nupkg -s "github" --skip-duplicate --api-key "%GITHUB_API_KEY%"
@IF ERRORLEVEL 1 GOTO :failed
@ECHO.
@ECHO.Upload success. Press any key to exit.
@PAUSE > Nul
@EXIT /B 0

:failed
@ECHO.
@ECHO.The step above failed, nothing after it ran. Press any key to exit.
@PAUSE > Nul
@EXIT /B 1
