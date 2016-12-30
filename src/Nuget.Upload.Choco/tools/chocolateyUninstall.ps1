$packageArgs = @{
  packageName = 'Cornerstone.NuGetUpload'
  fileType = 'msi'
  file = '{53091262-B927-438C-A5B6-ABB8B8B0C0A3}'
  silentArgs = '/quiet'
}

#Uninstall-ChocolateyPackage @packageArgs
msiexec /x  '{53091262-B927-438C-A5B6-ABB8B8B0C0A3}' /quiet

