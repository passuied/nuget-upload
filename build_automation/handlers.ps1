<#
function Get-Info {
  Write-Host $rootDir "Exists : " (test-path -path $rootDir)
  Write-Host $solution "Exists : " (test-path -path $solution)
  Write-Host $artifacts "Exists : " (test-path -path $artifacts)
  Write-Host $logs "Exists : " (test-path -path $logs)
  Write-Host $nuget "Exists : " (test-path -path $nuget)
  Write-Host $zip "Exists : " (test-path -path $zip)
  Write-Host $xunit "Exists : " (test-path -path $xunit)
  
  Write-Host "Nuget will be restoring from $nugetPackageSources"
}

function Invoke-Cleanup {
   if( Test-Path "$artifacts/buildOutput"){
    Remove-Item -Path "$artifacts/buildOutput" -Recurse  -Force -Verbose
   }

   if(Test-Path "$artifacts/dist"){
    Remove-Item -Path "$artifacts/dist" -Recurse  -Force -Verbose
   }
   
   if(Test-Path "$rootDir/packages") {
	Remove-Item -Path "$rootDir/packages" -Recurse  -Force -Verbose
   }


}



function Invoke-Initialize {
    New-Item -ItemType Directory -Force -Path $artifacts/buildOutput
    New-Item -ItemType Directory -Force -Path $artifacts/dist

}


function Invoke-NugetRestore {
    exec {
        &$nuget restore $solution -source $nugetPackageSources
    }

}

function Invoke-NugetPack{
 foreach ($item in $nugetTargets)
 {
   & $nuget pack $item -IncludeReferencedProjects -Build -OutputDirectory $dist
 }
}

function Invoke-Compile{
  Framework $frameworkVersion
  exec { msbuild $solution /t:Rebuild /p:Configuration=Release /v:Minimal /p:OutDir="$artifacts/buildOutput" }
}



function Invoke-UnitTest{
  foreach ($item in $unitTestTargets)
  {
  exec {
    &$xunit $item }
  }
}

function Invoke-IntegrationTest {
  Write-Host "Not available"
}



function Invoke-7Zip {
  foreach ($key in $zipTargets.Keys)
  {
    $list = $zipTargets[$key]
    foreach ($item in $list)
    {
      exec { &$zip a -t7z -mmt -o$artifacts $key $item }
    }
  }
}

function Invoke-AppendFileTo7Zip {
    exec{
        &$zip a $key1 $buildInfo 
    }    
}



function Invoke-InstallTopShelfService {
    exec { &$serviceExecutable install}	
}

function Invoke-RunTopShelfService {
    exec { &$serviceExecutable start}
}

function Invoke-UninstallTopshelfService {
    exec {&$serviceExecutable uninstall}
}


function Invoke-GenerateBuildInfoFile {
    exec {
        &$buildInfoCli -component $component -build $build -buildTime $buildTime -packageDir $packageDir | Out-File $buildInfo
    }
}
#>