# NuGet Upload

## Responsibility
- Provides ability to upload a given package and dependencies to DLL folder and relative folders underneath.
- Uses rules based on name pattern to upload packages' corresponding DLLs to the correct folder:
- Default rules: (order matters)
  - 'Cornerstone.Services.*' ==> DLL\ServicesFramework\
  - 'Cornerstone.Configuration.*' ==> DLL\ServicesFramework\
  - 'Cornerstone.*' ==> DLL\
  - '*' ==> DLL\3rd Party Assemblies\
- If the file uploaded already exists in the target folder, it will only overwrite it if the version has changed
- If used within TFSVC working copy, the tool will automatically:
  - add the files as Pending add if the file is new
  - checkout the files as Pending Checkin if the file exists

## Usage
- Open an admin command prompt or an admin powershell window
- Change directory to the root of the DLL folder
- Run the `nuget-upload load` option as below passing the NuGet package ID and optionally the version desired. (will use latest stable version available if omitted) 
```bat
C:>cd c:\tfs\dev\Cornerstone\DevRelease\DLL
C:tfs\dev\Cornerstone\DevRelease\DLL>nuget-upload load <package-id> [<version-id>]
```

## Installation
Installation is easy via [Chocolatey](https://chocolatey.org/):

0. First install chocolatey (if you do not have it installed already):
   * Follow instructions [here](https://chocolatey.org/install)
1. Open an admin powershell window:
```pshell
PS> choco install nugetupload
```
   * This will install the `nuget-upload` command line tool to the folder `%PROGRAMFILES(x86)%\Cornerstone\NuGetUpload`
   * It will also add this folder to the Environment `PATH` variable 


## Configuration
- Default configuration can be overriden by editing the `appsettings.json` file located in the `%PROGRAMFILES(x86)%\Cornerstone\NuGetUpload` application folder
- Format is as follows:
```json
{
  "NugetUpload": {
    "TempFolder": "c:\\temp\\",
    "TargetFramework": "net452",
    "DeleteStagingFolder" :  true,
    "ApplyRulesPerDllName": true,
    "UploadRules": [
      {
        "Pattern": "Cornerstone.Services.*",
        "TargetFolder": "ServicesFramework\\"
      },
      {
        "Pattern": "Cornerstone.Configuration.*",
        "TargetFolder": "ServicesFramework\\"
      },
      {
        "Pattern": "Cornerstone.CorpConnection*",
        "TargetFolder": "ServicesFramework\\"
      },
      {
        "Pattern": "Cornerstone.*",
        "TargetFolder": ""
      },
      {
        "Pattern": "*",
        "TargetFolder": "3rd Party Assemblies\\"
      }
    ]
  }
}
```
- `TempFolder`: Path to location where NuGet packages will be downloaded prior to being uploaded.
- `TargetFramework`: .net framework version targeted
- `DeleteStagingFolder`: whether the tool should delete the staging folder created.
- `UploadRules`: an array of rules defining to which sub folder each package will be uploaded to. The order of rules matter as if a package was selected to go to a certain folder, it will not be picked to go to another folder event if the rule pattern matches its name!
  - `Pattern`: pattern associated with package name
  - `TargetFolder`: target upload sub folder

# Change Log
| Version | Summary 
| ------- | ------- 
| 1.0.2   | Fixing a bug that would always add entry to PATH even if already there!
| 1.0.1   | Now supports applying rules per DLL file name instead of NuGet Package ID. Enabled by default but can be reverted by setting 'ApplyRulesPerDllName' to false.
| 1.0.0   | First stable version
