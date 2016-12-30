$scriptPath =  $(Split-Path $MyInvocation.MyCommand.Path)

$packageArgs = @{
  packageName = 'Cornerstone.NuGetUpload'
  softwareName   = 'NuGetUpload'
  fileType = 'msi'
  silentArgs = '/quiet'
  file = Join-Path $scriptPath 'NugetUploadSetup.msi'
}

Install-ChocolateyInstallPackage @packageArgs

Remove-Item -Force $packageArgs.file

$appFolder = "%PROGRAMFILES(X86)%\Cornerstone\NugetUpload"

Install-ChocolateyPath -PathToInstall $appFolder -PathType 'Machine'



