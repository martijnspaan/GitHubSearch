$ErrorActionPreference = 'Stop';

$packageName  = 'GitHubSearch'
$toolsDir     = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$url          = 'https://github.com/martijnspaan/GitHubSearch/releases/download/[[packageversion]]/GitHubSearch_[[packageversion]].zip'
$checksum     = '[[checksum]]'
$checksumType = 'md5'

Install-ChocolateyZipPackage $packageName $url $toolsDir -checksum $checksum -checksumType $checksumType

Install-ChocolateyPath $toolsDir 'Machine'