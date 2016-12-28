function New-OpenCoverAnalysisWithXUnit {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true, ValueFromPipeline=$true, ValueFromPipelineByPropertyName=$true)]
        [ValidateNotNullOrEmpty()]
        [string[]] $Tests,
        
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $Output,
		
		[Parameter(Mandatory = $true)]
		[string]
		[ValidateScript({ Test-Path $_})]
		$PathToXUnitConsoleRunner,
		
		[Parameter(Mandatory = $true)]
		[string]
		[ValidateScript({ Test-Path $_})]
		$PathToOpenCoverConsole,

        [string] $Filter = '+[*]* -[*]*Exception* -[*Tests]* -[xunit*]*'
    )

    $testAssemblies = $Tests -Join ' '
    Exec {
          &$PathToOpenCoverConsole -register:user `
          -target:PathToXUnitConsoleRunner `
          "-targetargs:$testAssemblies -noshadow -parallel all" '-filter:$Filter' -output:$Output
    }
}

function New-OpenCoverReport {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $CoverageXml,
        
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $Output,
        
		[Parameter(Mandatory = $true)]
		[string]
		[ValidateScript({ Test-Path $_})]
		$PathToReportGenerator,        

        [ValidateSet('Verbose', 'Info', 'Error')]
        [string] $Verbosity = 'Error'
    )

    Exec {
          &$PathToReportGenerator -reports:$CoverageXml -targetdir:$Output `
          -verbosity:$Verbosity
    }
}