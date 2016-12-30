$scriptPath =  $(Split-Path $MyInvocation.MyCommand.Path)

$packageArgs = @{
  packageName = 'Cornerstone.NugetUpload'
  softwareName   = 'NugetUpload'
  fileType = 'msi'
  silentArgs = '/quiet'
  file = Join-Path $scriptPath 'NugetUploadSetup.msi'
}

Install-ChocolateyInstallPackage @packageArgs

Remove-Item -Force $packageArgs.file

