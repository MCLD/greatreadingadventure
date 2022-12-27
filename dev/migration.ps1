param($command, $parameters)

Push-Location -Path ".."
dotnet ef migrations $command --project .\src\GRA.Data.SqlServer\GRA.Data.SqlServer.csproj $parameters
Pop-Location
