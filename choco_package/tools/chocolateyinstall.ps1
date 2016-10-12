$ErrorActionPreference = 'Stop';

$packageName  = 'GitHubSearch'
$toolsDir     = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url          = 'https://github.com/martijnspaan/GitHubSearch/releases/download/1.2.0/GitHubSearch_1.2.0.zip'
$checksum     = '30BFEEF304FD311AEB880BE6FCF6B513'
$checksumType = 'md5'

Install-ChocolateyZipPackage $packageName $url $toolsDir -checksum $checksum -checksumType $checksumType

Install-ChocolateyPath $toolsDir 'Machine'