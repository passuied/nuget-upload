$packageArgs = @{
  packageName = 'nugetupload'
  fileType = 'msi'
  silentArgs = '{53091262-B927-438C-A5B6-ABB8B8B0C0A3} /quiet'
}

Uninstall-ChocolateyPackage @packageArgs

