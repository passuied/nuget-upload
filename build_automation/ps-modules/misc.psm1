function Test-Paths
{
  <#
  .Synopsis
    Tests validity of file or directory paths in bulk an dprints result
  .Description
    Tests validity of file or directory paths in bulk
  .Parameter pathsToTest
    Array of paths to test for existence
  #>
    [CmdletBinding()]
    Param
    (
        [String[]]
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        $pathsToTest
    )

    foreach($item in $pathsToTest)
    {
        if(Test-Path -path $item)
        {
            Write-Host "$item is a valid path."
        }
        else
        {
            Write-Host "$item is an invalid path."
        }
    }
}

function Invoke-ForceDeleteFolders
{
  <#
    .Synopsis
      Tests validity of directory path and then force delete it with contents
    .Description
      Tests validity of directory path and then force delete it with contents
    .Parameter foldersToDelete
      Array of paths to test for existence and delete with contents
  #>
    [CmdletBinding()]
    Param
    (
        [String[]]
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        $foldersToDelete
    )

    foreach($item in $foldersToDelete)
    {
       if(Test-Path $item){
         Write-Host "Deleting : $item"
         Remove-Item $item -Force -Recurse
       }

    }

}

function Invoke-CreateChildDirs
{
  <#
  .Synopsis
  .Description
  .Parameter rootDirectory
  .Parameter childDirs
  #>
    [CmdletBinding()]
    Param
    (
        [String]
        [Parameter(Mandatory=$true)]
        [ValidateNotNullOrEmpty()]
        $rootDirectory,

        [String[]]
        [Parameter(Mandatory=$true)]
        $childDirs
    )

    foreach($item in $childDirs)
    {
        New-Item -ItemType Directory -Path $item -Force
    }

}

function Get-TaskNotImplementedMessage
{
  <#
    .Synopsis
      Print task not implemented
    .Description
      Print task not implemented
  #>
  Write-Host "Task Not Implemented"
}

function Get-BuildInfo
{
  <#
    .Synopsis
      Generate buildInfo.json
    .Description
      Generate buildInfo.json
    .Parameter component
      Name of the component to be built and distributed by the build system
    .Parameter build
      Number identifying the Team City build number
    .Parameter buildTime
      Date/Time string that stamps when the buildinfo was generated
    .Parameter packageDir
      Full path of the packages directory where NuGet packages are restored
    .Parameter buildInfoCli
      Full path of the location where BuildInfo.Generator.Commandline.exe
  #>
    [CmdletBinding()]
    Param
    (
      [String]
      [Parameter(Mandatory = $true)]
      $component="undefined",

      [Int]
      [Parameter(Mandatory = $true)]
      $build=0,

      [String]
      [Parameter(Mandatory = $true)]
      $buildTime="undefined",

      [String]
      [ValidateScript({ Test-Path $_ })]
      [Parameter(Mandatory=$true)]
      $packageDir,

      [ValidateScript({ Test-Path $_ })]
      [Parameter(Mandatory=$true)]
      $buildInfoCli
    )
    exec {
        &$buildInfoCli -component $component `
                      -build $build `
                      -buildTime $buildTime `
                      -packageDir $packageDir
    }
}

Export-ModuleMember -Function Test-Paths, Invoke-ForceDeleteFolders, Invoke-CreateChildDirs, Invoke-GenerateBuildInfoFile, Get-TaskNotImplementedMessage
