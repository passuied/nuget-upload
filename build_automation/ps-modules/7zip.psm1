function Invoke-7Zip
{

<#
  .Synopsis
    Create a 7Zip archive
  .Description
    Create a 7Zip archive
  .Parameter pathTo7ZipExe
    Path to 7Zip executable
  .Parameter outputPath
    Path where the .7z file should be created
  .Parameter zipAndContents
    Hashtable with
    Key = Name of the 7Zip file to be created.
    Value = Collection of full file names to be added to the 7z file.
#>
    [CmdletBinding()]
    Param
    (
        [String]
        [ValidateScript({ Test-Path $_ })]
        $pathTo7ZipExe,

        [String]
        [ValidateScript({ Test-Path $_ })]
        $outputPath,
        [Hashtable]
        $zipAndContents=@{}
    )

  foreach ($key in $zipAndContents.Keys)
  {
    $list = $zipAndContents[$key]
    foreach ($item in $list)
    {
      Exec { &$pathTo7ZipExe a -t7z -mmt -o$outputPath $key $item }
    }
  }
}

function Invoke-AppendFilesTo7Zip
{
<#
  .Synopsis
    Add file to an existing 7z file
  .Description
    Add file to an existing 7z file
  .Parameter pathTo7ZipExe
    Path to 7z executable
  .Parameter zipFileToAppend
    Full path to the 7z file to be appended
  .Parameter fileToAppend
    Full path to the file to be appended to the 7z file
#>
    [CmdletBinding()]
    Param
    (
        [String]
        [ValidateScript({ Test-Path $_ })]
        $pathTo7ZipExe,

        [String]
        [ValidateScript({ Test-Path $_ })]
        $zipFileToAppend,

        [String]
        [ValidateScript({ Test-Path $_ })]
        $fileToAppend
    )

    Exec { &$pathTo7ZipExe a $zipFileToAppend $item }


}

#Export-ModuleMember -Function Invoke-7Zip, Invoke-AppendFilesTo7Zip
