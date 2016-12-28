using NuGet.Upload.Configuration;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NuGet.Upload.Core
{
    public class NugetUploader
    {
        public NugetUploader(NugetUploadOptions options, Action<string> standardOutputWriter)
        {
            this.Options = options;
            this.StandardOutputWriter = standardOutputWriter;
        }

        public NugetUploadOptions Options { get; private set; }
        public Action<string> StandardOutputWriter { get; private set; }

        public async Task Upload(string targetUploadFolder, string packageID, string packageVersionID = "")
        {
            if (string.IsNullOrWhiteSpace(packageID)) throw new ArgumentNullException(nameof(packageID));

            // create staging folder
            var stagingFolder = $"{Options.TempFolder}{Guid.NewGuid()}\\";
            Directory.CreateDirectory(stagingFolder);
            RunNuget(packageID, packageVersionID, stagingFolder);

            // Now apply rules on staging folder where Nuget and dependencies just downloaded
            IDictionary<string, IEnumerable<string>> filesToUpload = GetLibDllsbyTargetFolder(stagingFolder);

            // Finally copy each dll to the correct folder
            await CopyToTargetFolders(targetUploadFolder, filesToUpload);

            // Finally delete staging folder
            Directory.Delete(stagingFolder, true);

        }

        private async Task CopyToTargetFolders(string targetUploadFolder, IDictionary<string, IEnumerable<string>> filesToUpload)
        {
            foreach (var targetFolder in filesToUpload.Keys)
            {
                var targetPath = Path.Combine(targetUploadFolder, targetFolder);
                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath);

                foreach (var file in filesToUpload[targetFolder])
                {
                    await CopyFileIfNewer(file, targetPath);
                }

            }
        }

        private async Task CopyFileIfNewer(string filePath, string targetFolder)
        {
            var targetLocation = Path.Combine(targetFolder, GetFilename(filePath));
            if (File.Exists(targetLocation))
            {
                var oldFileVersion = FileVersionInfo.GetVersionInfo(targetLocation).FileVersion;
                var newFileVersion = FileVersionInfo.GetVersionInfo(filePath).FileVersion;

                if (oldFileVersion != newFileVersion)
                    await CopyFileAsync(filePath, targetFolder);
                else
                    StandardOutputWriter($"Skipping copy of {GetFilename(filePath)} as version {newFileVersion} already in target folder.");
            }
            else
                await CopyFileAsync(filePath, targetFolder);

        }

        private async Task CopyFileAsync(string filePath, string targetFolder)
        {
            using (FileStream sourceStream = File.Open(filePath, FileMode.Open))
            {
                using (FileStream destinationStream = File.Open(Path.Combine(targetFolder, GetFilename(filePath)), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StandardOutputWriter($"Copying {GetFilename(filePath)} to folder: {targetFolder}");
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }
        }

        private IDictionary<string, IEnumerable<string>> GetLibDllsbyTargetFolder(string stagingFolder)
        {
            var dllsByPackageId = GetLibDllsByPackageID(stagingFolder);
            var dllsByTargetFolder = new Dictionary<string, List<string>>();
            foreach (var rule in this.Options.UploadRules)
            {
                // Find Nuget package IDs matching the current rule
                var matchingPackageIds = FindMatchingPackageIds(rule, dllsByPackageId);

                // For each found, store Dlls against the target folder
                foreach (var key in matchingPackageIds)
                {
                    List<string> dlls;
                    if (dllsByTargetFolder.ContainsKey(rule.TargetFolder))
                        dlls = dllsByTargetFolder[rule.TargetFolder];
                    else
                    {
                        dlls = new List<string>();
                        dllsByTargetFolder.Add(rule.TargetFolder, dlls);
                    }
                    dlls.AddRange(dllsByPackageId[key]);

                    // finally remove from original dictionary so it doesn't get picked up again
                    dllsByPackageId.Remove(key);
                }
            }

            return dllsByTargetFolder.ToDictionary(kv => kv.Key, kv => kv.Value.AsEnumerable());
        }

        private static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                              .Replace(@"\*", ".*")
                              .Replace(@"\?", ".")
                       + "$";
        }

        private static IEnumerable<string> FindMatchingPackageIds(NugetUploadRule rule, IDictionary<string, IEnumerable<string>> dllsByPackageId)
        {
            var regex = new Regex(WildcardToRegex(rule.Pattern));

            var matches = dllsByPackageId.Keys
                            .Where(k => regex.IsMatch(k))
                            .ToArray();

            return matches;
        }

        private IDictionary<string, IEnumerable<string>> GetLibDllsByPackageID(string stagingFolder)
        {
            var dic = new Dictionary<string, IEnumerable<string>>();
            foreach (var subFolder in Directory.EnumerateDirectories(stagingFolder))
            {
                var libFolder = Path.Combine(subFolder, "lib");
                if (Directory.Exists(libFolder))
                {
                    string fwFolder = FindFrameworkFolder(libFolder);
                    var files = Directory.GetFiles(fwFolder, "*.dll").ToList();
                    files.AddRange(Directory.GetFiles(fwFolder, "*.xml"));
                    dic.Add(GetFilename(subFolder), files);
                }
            }

            return dic;
        }

        private string FindFrameworkFolder(string libFolder)
        {
            var fwFolders = Directory.EnumerateDirectories(libFolder)
                                .Select(f => GetFilename(f))
                                .ToArray();

            
            var targetFw = NuGetFramework.ParseFolder(this.Options.TargetFramework);

            var nearest = NuGetFrameworkUtility.GetNearest(
                fwFolders,
                targetFw,
                f => NuGetFramework.ParseFolder(f)
                );

            if (nearest == null)
                throw new InvalidOperationException($"Couldn't find matching framework for library {libFolder} and target framework={this.Options.TargetFramework}.");
            return Path.Combine(libFolder, nearest);
        }

        private void RunNuget(string packageID, string packageVersionID, string stagingFolder)
        {
            this.StandardOutputWriter($"Downloading Package {FormatPackageAndVersion(packageID, packageVersionID)} and dependencies...");
            Process p = new Process();
            p.StartInfo.FileName = "nuget.exe";
            p.StartInfo.WorkingDirectory = stagingFolder;
            var versionOption = !string.IsNullOrEmpty(packageVersionID) ? $" -Version {packageVersionID}" : string.Empty;
            p.StartInfo.Arguments = $" install {packageID}{versionOption}";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            this.StandardOutputWriter(p.StandardOutput.ReadToEnd());

            p.WaitForExit();
            if (p.ExitCode != 0)
                throw new ArgumentException($"Unable to download {FormatPackageAndVersion(packageID, packageVersionID)}...");
        }

        public static string FormatPackageAndVersion(string packageID, string packageVersionID)
        {
            if (!string.IsNullOrEmpty(packageVersionID))
                return $"{packageID} v{packageVersionID}";
            else
                return packageID;
        }

        private static string GetFilename(string filepath)
        {
            return filepath.Substring(filepath.LastIndexOf('\\') + 1);
        }
    }
}
