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

@ECHO on
@ECHO.Building solution...
@dotnet build -c Release -o bin/publish
@ECHO.Deleting *.pdb files...
@cd bin/publish
@del *.pdb
@ECHO.Build successful.

dotnet nuget push *.nupkg -s "nuget.org" --skip-duplicate -k "%NUGET_API_KEY%"
@ECHO.Upload success. Press any key to exit.
PAUSE