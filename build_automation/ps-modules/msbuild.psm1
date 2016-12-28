function Invoke-MsBuild
{
  <#
    .Synopsis
      Run MsBuild for a solution
    .Description
      Run MsBuild for a solution
    .Parameter solutionPath
      Path to the solution to be built
    .Parameter outputDirectoryPath
      Path to the directory where MsBuild generates all build output
    .Parameter target
      MsBuild target "Build","Clean","Rebuild"
    .Parameter verbosity
      MsBuild verbosity "quiet","minimal","normal","detailed","diagnostic"
    .Parameter buildConfig
      MsBuild configuration "Debug","Release"
  #>
    [CmdletBinding()]
    Param
    (
        [String]
        [ValidateScript({Test-Path $_ })]
        $solutionPath,

        [String]
        [ValidateScript({Test-Path $_ })]
        $outputDirectoryPath,

        [String]
        [ValidateSet("Build","Clean","Rebuild")]
        $target = "Rebuild",

        [String]
        [ValidateSet("quiet","minimal","normal","detailed","diagnostic")]
        $verbosity = "minimal",

        [String]
        [ValidateSet("Debug","Release")]
        $buildConfig = "Release"
    )

    Exec {
        msbuild $solutionPath /t:$target /v:$verbosity /p:Configuration=$buildConfig /p:OutDir=$outputDirectoryPath
    }
}

function Invoke-MsBuildWithOctopack
{
  <#
    .Synopsis
      Run MsBuild for a solution
    .Description
      Run MsBuild for a solution
    .Parameter solutionPath
      Path to the solution to be built
    .Parameter outputDirectoryPath
      Path to the directory where MsBuild generates all build output
    .Parameter target
      MsBuild target "Build","Clean","Rebuild"
    .Parameter verbosity
      MsBuild verbosity "quiet","minimal","normal","detailed","diagnostic"
    .Parameter buildConfig
      MsBuild configuration "Debug","Release"
    .Parameter octopackProperties
      Octopack specific properties as string
  #>
  [CmdletBinding()]
  Param
  (
      [String]
      [ValidateScript({Test-Path $_ })]
      $solutionPath,

      [String]
      [ValidateScript({Test-Path $_ })]
      $outputDirectoryPath,

      [String]
      [ValidateSet("Build","Clean","Rebuild")]
      $target = "Rebuild",

      [String]
      [ValidateSet("quiet","minimal","normal","detailed","diagnostic")]
      $verbosity = "minimal",

      [String]
      [ValidateSet("Debug","Release")]
      $buildConfig = "Release",

      [String]
      $octopackProperties =""
  )

  Exec {
      msbuild $solutionPath /t:$target /v:$verbosity /p:Configuration=$buildConfig /p:OutDir=$outputDirectoryPath /p:RunOctoPack=true $octopackProperties
  }
}

Export-ModuleMember -Function Invoke-MsBuild, Invoke-MsBuildWithOctopack
