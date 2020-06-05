param([String]$versionFilePath)

$libVersion = Get-Content -Path "$versionFilePath"
Write-Host $libVersion
Write-Host "##vso[task.setvariable variable=libVersion;]$libVersion"