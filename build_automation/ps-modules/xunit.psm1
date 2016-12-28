function Invoke-XUnit
{
  <#
    .Synopsis
      Run xunit tests for a set of assemblies
    .Description
      Run xunit tests for a set of assemblies
    .Parameter pathToXUnitExe
      Full path to the Xunit.Console.exe
    .Parameter testAssemblies
      Array of full paths to the .dll files that will be used to scan for test cases
    .Parameter pathForTestReports
      Full path to the directory where the xunit-report.xml/html will be output
  #>
    [CmdletBinding()]
    Param
    (
        [String]
        [ValidateScript({Test-Path $_ })]
        [Parameter(Mandatory=$true)]
        $pathToXUnitExe,

        [String[]]
        [Parameter(Mandatory=$true)]
        $testAssemblies,

        [String]
        [Parameter(Mandatory=$true)]
        [ValidateScript({Test-Path $_ })]
        $pathForTestReports
    )

    $assemblies = $testAssemblies -join ";"
    $xmlReport = Join-Path -path $pathForTestReports -childpath "xunit-report.xml"
    $htmlReport = Join-Path -path $pathForTestReports -childpath "xunit-report.html"
    Exec {
        &$pathToXUnitExe $assemblies -parallel all -xml $xmlReport -html $htmlReport
    }

    Write-Host "Generated: $xmlReport"
    Write-Host "Generated: $htmlReport"
}
