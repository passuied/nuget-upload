$ErrorActionPreference = 'Stop';

$scriptPath =  $(Split-Path $MyInvocation.MyCommand.Path)

$packageArgs = @{
  packageName = 'nugetupload'
  softwareName   = 'NuGetUpload'
  fileType = 'msi'
  silentArgs = '/quiet'
  file = Join-Path $scriptPath 'NugetUploadSetup.msi'
}

Install-ChocolateyInstallPackage @packageArgs

Remove-Item -Force $packageArgs.file

# Need to use hard-coded path as if using %PROGRAMFILES(X86)%, it keeps adding the entry to the path!
$programFiles = "$env:systemdrive\Program Files (x86)"
if (Get-OSArchitectureWidth -Compare 32)
{
	$programFiles = "$env:systemdrive\Program Files"
}
	
$appFolder = "$programFiles\Cornerstone\NugetUpload"

Install-ChocolateyPath -PathToInstall $appFolder -PathType 'Machine'



