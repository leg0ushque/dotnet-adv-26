$Config = Get-Content "$PSScriptRoot\sonar\sonar-cart.json" | ConvertFrom-Json
$SlnPath = "$PSScriptRoot\Ecommerce.CartService\Ecommerce.CartService.slnx"

Write-Host "Analyzing: $($Config.'sonar.projectName')"

dotnet sonarscanner begin `
  /k:"$($Config.'sonar.projectKey')" `
  /n:"$($Config.'sonar.projectName')" `
  /v:"$($Config.'sonar.projectVersion')" `
  /d:sonar.host.url="$($Config.'sonar.host.url')" `
  /d:sonar.token="$($Config.'sonar.token')" `
  /d:sonar.exclusions="$($Config.'sonar.exclusions')" `
  /d:sonar.cs.opencover.reportsPaths="$($Config.'sonar.cs.opencover.reportsPaths')"

dotnet build $SlnPath --no-incremental

dotnet test $SlnPath `
  --collect:"XPlat Code Coverage" `
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

dotnet sonarscanner end /d:sonar.token="$($Config.'sonar.token')"

Write-Host "Done! See $($Config.'sonar.host.url') for details"