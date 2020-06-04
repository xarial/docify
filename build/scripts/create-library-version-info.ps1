param([String]$version, [String]$outFilePath, [String]$libFilePath, [String]$certPath, [String]$pwd)

$params = @{
    Uri = 'https://api.appcenter.ms/v0.1/apps/xarial/docify/distribution_groups/production/releases/latest'
    ContentType = 'application/json'
}
$newVersion = @{
    MinimumAppVersion = '0.2.0'
    MaximumAppVersion = $null
    Version = ''
    DownloadUrl = ''
    Signature = $null
}
$releaseInfo = Invoke-RestMethod @params

if($version -eq $releaseInfo.version)
{
    $versionsInfo = Invoke-RestMethod 'http://localhost:8081/version.json'

    $cert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2([string]$certPath, [string]$pwd)
    
    $rsa = [System.Security.Cryptography.X509Certificates.RSACertificateExtensions]::GetRSAPrivateKey($cert)
        
    $buffer = [System.IO.File]::ReadAllBytes($libFilePath)
    
    $signature = $rsa.SignData($buffer, [System.Security.Cryptography.HashAlgorithmName]::SHA256, [System.Security.Cryptography.RSASignaturePadding]::Pss)

    $newVersion.DownloadUrl = $releaseInfo.download_url
    $newVersion.MinimumAppVersion = $versionsInfo.Versions[-1].MinimumAppVersion
    $newVersion.Version = $version
    $newVersion.Signature = [System.Convert]::ToBase64String($signature)

    $versionsInfo.Versions += $newVersion
    ConvertTo-Json $versionsInfo | Out-File $outFilePath
}
else
{
    throw "Version mismatch"
}