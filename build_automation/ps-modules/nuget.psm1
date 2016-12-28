function Invoke-NugetRestore
{
  <#
    .Synopsis
      Restore nuget package using a .sln file
    .Description
      Restore nuget package using a .sln file
    .Parameter nugetExePath
      Full path to the nuget commandline tool
    .Parameter restoreSlnPath
      Full pat to the .sln file to be used for NuGet restore
    .Parameter packageSources
      An array of URL to use as package sources
  #>
    [CmdletBinding()]
    Param
    (
        [String]
        [ValidateScript({Test-Path $_ })]
        [Parameter(Mandatory=$true)]
        $nugetExePath,

        [String]
        [ValidateScript({Test-Path $_ })]
        [Parameter(Mandatory=$true)]
        $restoreSlnPath,

        [String[]]
        $packageSources = @("https://www.nuget.org/api/v2/")
    )

    $nugetPackageSources = $packageSources -join ";"

    Exec {
        &$nugetExePath restore $restoreSlnPath -source $nugetPackageSources
    }
}

function Invoke-NugetPackDll
{
  <#
    .Synopsis
      Create nuget package using a .csproj or .nuspec file
    .Description
      Create nuget package using a .csproj file
    .Parameter nugetExePath
      Full path to the nuget commandline tool
    .Parameter nuspecOrCsprojPaths
      Full pat to the .nuspec or .csproj file to be used for NuGet pack
    .Parameter outputDirectory
      Full path where the created nuget package should be generated
  #>
    [CmdletBinding()]
    Param
    (
        [String]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory=$true)]
        [ValidateScript({Test-Path $_ })]
        $nugetExePath,

        [String[]]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory=$true)]
        [ValidateScript({Test-Path $_ })]
        $nuspecOrCsprojPaths,

        [String]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory=$true)]
        [ValidateScript({Test-Path $_ })]
        $outputDirectory
    )

    foreach ($item in $nuspecOrCsprojPaths)
    {
       &$nugetExePath pack $item -IncludeReferencedProjects -Build -OutputDirectory $outputDirectory
    }
}

function Invoke-NugetPackTool
{
  <#
    .Synopsis
      Create nuget package using a .csproj or .nuspec file
    .Description
      Create nuget package using a .csproj file
    .Parameter nugetExePath
      Full path to the nuget commandline tool
    .Parameter nuspecOrCsprojPaths
      Full pat to the .nuspec or .csproj file to be used for NuGet pack
    .Parameter outputDirectory
      Full path where the created nuget package should be generated
  #>
    [CmdletBinding()]
    Param
    (
        [String]
        [ValidateNotNullOrEmpty]
        [ValidateScript({Test-Path $_ })]
        [Parameter(Mandatory=$true)]
        $nugetExePath,

        [String[]]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory=$true)]
        [ValidateScript({Test-Path $_ })]
        $nuspecOrCsprojPaths,

        [String]
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory=$true)]
        [ValidateScript({Test-Path $_ })]
        $outputDirectory
    )

    foreach ($item in $nuspecOrCsprojPaths)
    {
       &$nugetExePath pack $item -IncludeReferencedProjects -Build -Tool -OutputDirectory $outputDirectory
    }

}

#Export-ModuleMember -Function Invoke-NugetRestore, Invoke-NugetPackDll, Invoke-NugetPackTool
