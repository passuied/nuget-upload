param(
	[string]$version = "1.0.0"
)

$pkgName = "Nuget.Upload." +$version +".nupkg"

choco pack ../src/Nuget.Upload.Choco/Nuget.Upload.nuspec --version=$version
choco apiKey -k 06038eb3-b496-4fde-93d1-8c7f60c4cd7d -source https://chocolatey.org/
choco push $pkgName


