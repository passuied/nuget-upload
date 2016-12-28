function Get-GitBranchName
{
  <#
    .Synopsis
      Returns the current branch name of the repository
    .Description
      Returns the current branch name of the repository
    .Parameter repoPath
      Full path to the git repository
  #>

  Param(
    [ValidateScript({ Test-Path $_ })]
    [Parameter(Mandatory=$true)]
    [String] $repoPath
  )

  $pwd = pwd
  cd $repoPath
  $branch = git rev-parse --symbolic-full-name --abbrev-ref HEAD 2> $null
  if (!$branch)
  {
    $branch = git branch 2> $null
  }
  cd $pwd
  return $branch
}

function Invoke-GitMerge
{
  <#
    .Synopsis
      Merge 2 git branches
    .Description
      Merge 2 git branches
    .Parameter branchToMerge
      The name of the git branch that will be merged
    .Parameter branchToBeMeregedInto
      The name of the git branch that will be merged into
    .Parameter repoPath
      The full path to the repo to be managed
  #>
	[CmdletBinding()]
	param
	(
		[String]
		[Parameter(Mandatory=$true)]
		$branchToMerge,

		[String]
		[Parameter(Mandatory=$true)]
		[String]$branchToBeMeregedInto,

    [ValidateScript({ Test-Path $_ })]
    [Parameter(Mandatory=$true)]
    [String] $repoPath
	)
  $pwd = pwd
  cd $repoPath
	git checkout $branchToBeMeregedInto
	git merge $branchToMerge --no-ff
  cd $pwd
}

Export-Module -Function Invoke-GitMerge , Get-GitBranchName
