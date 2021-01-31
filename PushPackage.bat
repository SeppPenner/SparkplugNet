dotnet nuget push "src\SparkplugNet\bin\Release\SparkplugNet.*.nupkg" -s "github" --skip-duplicate
dotnet nuget push "src\SparkplugNet\bin\Release\SparkplugNet.*.nupkg" -s "nuget.org" --skip-duplicate -k "%NUGET_API_KEY%"
PAUSE