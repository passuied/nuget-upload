function Invoke-InstallTopShelfService
{
  <#
    .Synopsis
      Install a windows service that uses topshelf infrastructure
    .Description
     Install a windows service that uses topshelf infrastructure
    .Parameter pathToServiceExe
     Full path to the .exe file that encapsulates the windows service
  #>
    [CmdletBinding()]
    Param
    (
        [String]
        [ValidateScript({ Test-Path $_ })]
        $pathToServiceExe
    )
    exec { &$pathToServiceExe install}
}

function Invoke-UninstallTopShelfService
{
  <#
    .Synopsis
      Un-Install a windows service that uses topshelf infrastructure
    .Description
     Un-Install a windows service that uses topshelf infrastructure
    .Parameter pathToServiceExe
     Full path to the .exe file that encapsulates the windows service
  #>
    [CmdletBinding()]
    Param
    (
        [String]
        [ValidateScript({ Test-Path $_ })]
        $pathToServiceExe
    )
    exec {&$pathToServiceExe uninstall}
}

function Invoke-StopTopShelfService
{
  <#
    .Synopsis
      Stop a windows service that uses topshelf infrastructure
    .Description
     Stop a windows service that uses topshelf infrastructure
    .Parameter pathToServiceExe
     Full path to the .exe file that encapsulates the windows service
  #>
    [CmdletBinding()]
    Param
    (
        [String]
        [ValidateScript({ Test-Path $_ })]
        $pathToServiceExe
    )
    exec { &$pathToServiceExe stop}
}

function Invoke-StartTopShelfService
{
  <#
    .Synopsis
      Start a windows service that uses topshelf infrastructure
    .Description
     Start a windows service that uses topshelf infrastructure
    .Parameter pathToServiceExe
     Full path to the .exe file that encapsulates the windows service
  #>
    [CmdletBinding()]
    Param
    (
        [String]
        [ValidateScript({ Test-Path $_ })]
        $pathToServiceExe
    )
    exec { &$pathToServiceExe start}
}

Export-ModuleMember -Function Invoke-InstallTopShelfService, Invoke-UninstallTopShelfService, Invoke-StartTopShelfService, Invoke-StopTopShelfService
