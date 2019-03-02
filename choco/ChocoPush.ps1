param(
	[string]$version = "1.0.0",
	[string]$apiKey
)

$pkgName = "nugetupload." +$version +".nupkg"

choco apiKey -k $apiKey -source https://push.chocolatey.org/
choco push $pkgName