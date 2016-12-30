param(
	[string]$version = "1.0.0"
)


choco pack ../src/Nuget.Upload.Choco/Cornerstone.NuGetUpload.nuspec --version=$version



