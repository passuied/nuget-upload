using Nuget.Upload.Configuration;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nuget.Upload.Core
{
    public class NugetUploader
    {
        public NugetUploader(NugetUploadOptions options)
        {
            this.Options = options;
        }

        public NugetUploadOptions Options { get; private set; }

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

        private static async Task CopyToTargetFolders(string targetUploadFolder, IDictionary<string, IEnumerable<string>> filesToUpload)
        {
            foreach (var targetFolder in filesToUpload.Keys)
            {
                var targetPath = Path.Combine(targetUploadFolder, targetFolder);
                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath);

                foreach (var file in filesToUpload[targetFolder])
                {
                    await CopyFileAsync(file, targetPath);
                }

            }
        }

        private static async Task CopyFileAsync(string filename, string targetFolder)
        {
            using (FileStream sourceStream = File.Open(filename, FileMode.Open))
            {
                using (FileStream destinationStream = File.Create(targetFolder + filename.Substring(filename.LastIndexOf('\\'))))
                {
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

        private  IDictionary<string, IEnumerable<string>> GetLibDllsByPackageID(string stagingFolder)
        {
            var dic = new Dictionary<string, IEnumerable<string>>();
            foreach (var subFolder in Directory.EnumerateDirectories(stagingFolder))
            {
                var libFolder = Path.Combine(subFolder, "lib");
                if (Directory.Exists(libFolder))
                {
                    string fwFolder = FindFrameworkFolder(libFolder);
                    dic.Add(subFolder.Substring(subFolder.LastIndexOf('\\')+1), Directory.GetFiles(fwFolder, "*.*"));
                }
            }

            return dic;
        }

        private string FindFrameworkFolder(string libFolder)
        {
            var fwFolders = Directory.EnumerateDirectories(libFolder)
                                .Select(f => f.Substring(f.LastIndexOf('\\')+1))
                                .ToArray();

            var fws = fwFolders
                        .Select(f => NuGetFramework.ParseFolder(f))
                        .ToArray();

            var targetFw = NuGetFramework.ParseFolder(this.Options.TargetFramework);

            var nearest = NuGetFrameworkUtility.GetNearest(
                fwFolders, 
                targetFw,
                f => NuGetFramework.ParseFolder(f)
                );

            return Path.Combine(libFolder, nearest);
        }

        private static void RunNuget(string packageID, string packageVersionID, string stagingFolder)
        {
            Process p = new Process();
            p.StartInfo.FileName = "nuget.exe";
            p.StartInfo.WorkingDirectory = stagingFolder;
            p.StartInfo.Arguments = $" install {packageID} {packageVersionID}";
            p.Start();
            p.WaitForExit();
        }
    }
}
