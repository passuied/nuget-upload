# dot source properties file
. .\properties.ps1

#------------------------------ Default Tasks-----------------------------------#
task default -depends cleanup,init,local -description "Default Task"

task local -depends compile,unittest,inttest,pack,zip -description "Use this task when you dont want cleanup"

#------------------------------ Housekeeping Tasks------------------------------#

task init -description "Initialize output folder structure" {
    Get-TaskNotImplementedMessage
}

task cleanup -description "Performs cleanup of output folders" {
  Get-TaskNotImplementedMessage
}

task info -description "Get info about configured paths" {
  Get-TaskNotImplementedMessage
}

task buildInfo -description "Generate buildInfo.json" {
    Get-TaskNotImplementedMessage
}

#------------------------------ Nuget Tasks-------------------------------------#

task  restore -description "Performs Nuget package restore" {
      Get-TaskNotImplementedMessage
}

task pack -description "Performs NuGet package creation based on csproj or nuspec"{
  Get-TaskNotImplementedMessage
}

#------------------------------ MSBuild Tasks ----------------------------------#

task compile -depends restore -description "Compile solution" {
    Get-TaskNotImplementedMessage
}

#------------------------------ Test Tasks -------------------------------------#

task unittest -description "Runs unit tests using configured test runner"{
  Get-TaskNotImplementedMessage
}

task inttest -description "Runs integration tests using configured test runner" { Get-TaskNotImplementedMessage }

task zip -depends buildInfo -description "Create 7Zip compressed artifacts" {
  Get-TaskNotImplementedMessage
}

#-------------------------- Repo Specific Tasks    --------------------------#

