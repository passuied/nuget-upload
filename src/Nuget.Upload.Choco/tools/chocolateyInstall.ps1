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

$programFiles = "%PROGRAMFILES(X86)%"
if (Get-OSArchitectureWidth -Compare 32)
{
	$programFiles = "%PROGRAMFILES%"
}
	
$appFolder = $programFiles +"\Cornerstone\NugetUpload"

Install-ChocolateyPath -PathToInstall $appFolder -PathType 'Machine'



