properties {
$frameworkVersion="4.5.1"
$solution=(Get-Item "..\apiconnector-integrations-w4-service-ci.sln").FullName
$rootDir=(Get-Item $solution).Directory
$artifacts= join-path -path (Get-Item $solution).Directory -childpath artifact
$buildOutput="$artifacts\buildOutput"
$dist="$artifacts\dist"
$logs= join-path -path $rootDir -childpath logs


$nuget= join-path -path $rootDir -childpath "tools\NuGet.CommandLine.2.8.6\tools\NuGet.exe"
$nugetPackageSources=@("https://www.nuget.org/api/v2/","http://ci-master/Nuget/nuget")
$nugetTargets=@()

$zip= join-path -path $rootDir -childpath "tools\7-Zip.CommandLine.9.20.0\tools\7za.exe"

$key1 = "$dist\apiconnector-integrations-w4-service.7z"
$fileList1 = @(
"$buildOutput\Topshelf.dll",
"$buildOutput\ApiConnector.Integrations.W4.Providers.CicPlus.pdb",
"$buildOutput\ApiConnector.Client.pdb",
"$buildOutput\ApiConnector.Framework.pdb",
"$buildOutput\ApiConnector.Integrations.W4.Client.pdb",
"$buildOutput\ApiConnector.Sdk.Client.pdb",
"$buildOutput\ApiConnector.Sdk.Service.pdb",
"$buildOutput\Newtonsoft.Json.xml",
"$buildOutput\RabbitMQ.Client.xml",
"$buildOutput\ApiConnector.Integrations.W4.Service.exe",
"$buildOutput\ApiConnector.Integrations.W4.Service.pdb",
"$buildOutput\ApiConnector.Client.dll",
"$buildOutput\ApiConnector.Framework.dll",
"$buildOutput\ApiConnector.Integrations.W4.Client.dll",
"$buildOutput\ApiConnector.Integrations.W4.Providers.CicPlus.dll",
"$buildOutput\ApiConnector.Sdk.Client.dll",
"$buildOutput\ApiConnector.Sdk.Service.dll",
"$buildOutput\Newtonsoft.Json.dll",
"$buildOutput\RabbitMQ.Client.dll",
"$buildOutput\buildInfo.json"
 )


$zipTargets=@{$key1 = $fileList1}

$xunit= join-path -path $rootDir -childpath "tools/xunit.runner.console.2.1.0/tools/xunit.console.exe"
$xunitTestDll= "$buildOutput/ApiConnector.Integrations.W4.Service.Tests.dll"
$unitTestTargets=@($xunitTestDll)

$serviceExecutable="$buildOutput/ApiConnector.Integrations.W4.Service.exe"

# Build Info information
$buildInfoCli= Join-Path -Path $rootDir -ChildPath "tools\BuildInfo.Generator.CommandLine.0.0.2/tools/BuildInfo.Generator.CommandLine.exe"
$component="apiconnector-integrations-w4-service"
$build=0
$buildTime="local"
$packageDir="$rootDir/packages"
$buildInfo= "$buildOutput/buildInfo.json"

}
