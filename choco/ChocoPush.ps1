param(
	[string]$version = "1.0.0"
)

$pkgName = "nugetupload." +$version +".nupkg"

choco apiKey -k 06038eb3-b496-4fde-93d1-8c7f60c4cd7d -source https://chocolatey.org/
choco push $pkgName