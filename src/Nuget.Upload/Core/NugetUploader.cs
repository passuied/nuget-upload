using Cornerstone.NuGet.Upload.Configuration;
using NuGet.Frameworks;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NuGet.Protocol.Core.Types;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.Client;
using NuGet.Protocol;
using NuGet.PackageManagement;
using NuGet.Resolver;
using NuGet.Versioning;
using NuGet.ProjectManagement;
using NuGet.Packaging;
using System.Threading;
using NuGet.Packaging.Core;

namespace Cornerstone.NuGet.Upload.Core
{
    public class NugetUploader
    {
        private string _stagingFolder;

        public NugetUploader(NuGetUploadOptions options, Action<string> standardOutputWriter)
        {
            this.Options = options;
            this.StandardOutputWriter = standardOutputWriter;
            this._stagingFolder = $"{Options.TempFolder}{Guid.NewGuid()}\\";

            // Attempt to use NuGet API but very tedious... code below not really in use
            this.Settings = global::NuGet.Configuration.Settings.LoadDefaultSettings(_stagingFolder, configFileName: null, machineWideSettings: new MachineWideSettings());
            this.SourceProvider = new PackageSourceProvider(this.Settings);
            this.Logger = new StandardOutputLogger(StandardOutputWriter);


        }

        public NuGetUploadOptions Options { get; private set; }
        public Action<string> StandardOutputWriter { get; private set; }
        public ISettings Settings { get; private set; }
        public PackageSourceProvider SourceProvider { get; private set; }
        public StandardOutputLogger Logger { get; private set; }

        public async Task Upload(string targetUploadFolder, string packageID, string packageVersionID = "")
        {
            if (string.IsNullOrWhiteSpace(packageID)) throw new ArgumentNullException(nameof(packageID));

            // create staging folder
            Directory.CreateDirectory(_stagingFolder);
            RunNuget(packageID, packageVersionID, _stagingFolder);

            // Now apply rules on staging folder where Nuget and dependencies just downloaded
            IDictionary<string, IEnumerable<string>> filesToUpload = GetLibDllsbyTargetFolder(_stagingFolder);

            // Finally copy each dll to the correct folder
            await CopyToTargetFolders(targetUploadFolder, filesToUpload);

            // Finally delete staging folder
            if (this.Options.DeleteStagingFolder)
                Directory.Delete(_stagingFolder, true);

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
            var newFileVersion = FileVersionInfo.GetVersionInfo(filePath).FileVersion;

            if (File.Exists(targetLocation))
            {
                var oldFileVersion = FileVersionInfo.GetVersionInfo(targetLocation).FileVersion;

                if (oldFileVersion != newFileVersion)
                    await CheckoutAndCopyFileAsync(filePath, targetFolder, newFileVersion);
                else
                    StandardOutputWriter($"Skipping copy of {GetFilename(filePath)} as version {newFileVersion} already in target folder.");
            }
            else
                await CopyFileAndTfsAddAsync(filePath, targetFolder, newFileVersion);

        }

       

        public async Task ShowInfo(string packageID, string packageVersionID = "")
        {
            // Attempt to use the NuGet API... Very complex :(
            //var sourceRepoProvider = new NugetUploaderSourceRepositoryProvider(this.SourceProvider);

            //var packageManager = new NuGetPackageManager(sourceRepoProvider, this.Settings, _stagingFolder);

            //var resolutionContext = new ResolutionContext(
            //   DependencyBehavior.Lowest,
            //   includePrelease: false,
            //   includeUnlisted: true,
            //   versionConstraints: VersionConstraints.None);

            //var folderProject = new FolderNuGetProject(
            //    this._stagingFolder,
            //    new PackagePathResolver(_stagingFolder));


            //var version = !string.IsNullOrEmpty(packageVersionID) ? new NuGetVersion(packageVersionID) : null;
            //if (version == null)
            //{
            //    version = await NuGetPackageManager.GetLatestVersionAsync(
            //        packageID,
            //        NuGetFramework.ParseFolder(this.Options.TargetFramework),
            //        resolutionContext,
            //        sourceRepoProvider.GetRepositories(),
            //        this.Logger,
            //        CancellationToken.None

            //        );
            //}

            //var primaryRepositories = packageSources.Select(sourceRepositoryProvider.CreateRepository);


            //packageManager.PreviewInstallPackageAsync(
            //    folderProject,
            //    new PackageIdentity(packageID, version),
            //    resolutionContext,
            //    new NuGetProjectContext(this.Logger),
            //    CancellationToken.None);
            

            await Task.Run(() => this.StandardOutputWriter("Coming soon..."));
        }

        private async Task CheckoutAndCopyFileAsync(string filePath, string targetFolder, string newVersion)
        {
            var targetLocation = Path.Combine(targetFolder, GetFilename(filePath));

            var workspaceInfo = Workstation.Current.GetLocalWorkspaceInfo(targetLocation);
            if (workspaceInfo != null)
            {
                using (var server = new TfsTeamProjectCollection(workspaceInfo.ServerUri))
                {
                    var workspace = workspaceInfo.GetWorkspace(server);
                    workspace.PendEdit(targetLocation);
                    await CopyFileAsync(filePath, targetFolder, newVersion);
                }
            }
            else
            {
                await CopyFileAsync(filePath, targetFolder, newVersion);
            }
        }

        private async Task CopyFileAndTfsAddAsync(string filePath, string targetFolder, string newVersion)
        {
            var targetLocation = Path.Combine(targetFolder, GetFilename(filePath));
            await CopyFileAsync(filePath, targetFolder, newVersion);

            var workspaceInfo = Workstation.Current.GetLocalWorkspaceInfo(targetFolder);
            if (workspaceInfo != null)
            {
                using (var server = new TfsTeamProjectCollection(workspaceInfo.ServerUri))
                {
                    var workspace = workspaceInfo.GetWorkspace(server);
                    workspace.PendAdd(targetLocation);

                }
            }

        }

        private async Task CopyFileAsync(string filePath, string targetFolder, string newVersion)
        {
            using (FileStream sourceStream = File.Open(filePath, FileMode.Open))
            {
                using (FileStream destinationStream = File.Open(Path.Combine(targetFolder, GetFilename(filePath)), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    StandardOutputWriter($"Copying {FormatPackageAndVersion(GetFilename(filePath), newVersion)} to folder: {targetFolder}");
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }
        }

        private IDictionary<string, IEnumerable<string>> GetLibDllsbyTargetFolder(string stagingFolder)
        {
            var dllsByPackageId = GetLibDllsByPackageID(stagingFolder);

            if (!this.Options.ApplyRulesPerDllName)
            {
                return ApplyRulesPerPackageId(dllsByPackageId);
            }
            else
                return ApplyRulesPerDllName(dllsByPackageId);
        }

        private IDictionary<string, IEnumerable<string>> ApplyRulesPerDllName(IDictionary<string, IEnumerable<string>> dllsByPackageId)
        {
            var dllsByTargetFolder = new Dictionary<string, List<string>>();

            // 1. flatten all Dlls
            var flattenDlls = dllsByPackageId.SelectMany(kv => kv.Value).ToList();

            // 2. Go to each rule
            foreach (var rule in this.Options.UploadRules)
            {
                // 2.a Find matching Dlls
                var matchingDlls = FindMatchingDlls(rule, flattenDlls);

                // 2.b. Add matching dlls to dictionary
                if (matchingDlls.Any())
                {
                    List<string> lstDlls;
                    if (dllsByTargetFolder.ContainsKey(rule.TargetFolder))
                        lstDlls = dllsByTargetFolder[rule.TargetFolder];
                    else
                    {
                        lstDlls = new List<string>();
                        dllsByTargetFolder.Add(rule.TargetFolder, lstDlls);

                    }
                    lstDlls.AddRange(matchingDlls);

                    // 2.c Remove from original list
                    matchingDlls.ToList().ForEach(l => flattenDlls.Remove(l));
                }
            }

            // 3. Return dictionary
            return dllsByTargetFolder.ToDictionary(kv => kv.Key, kv => kv.Value.AsEnumerable());
        }

       

        private IDictionary<string, IEnumerable<string>> ApplyRulesPerPackageId(IDictionary<string, IEnumerable<string>> dllsByPackageId)
        {
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

        private static IEnumerable<string> FindMatchingPackageIds(NuGetUploadRule rule, IDictionary<string, IEnumerable<string>> dllsByPackageId)
        {
            var regex = new Regex(WildcardToRegex(rule.Pattern));

            var matches = dllsByPackageId.Keys
                            .Where(k => regex.IsMatch(k))
                            .ToArray();

            return matches;
        }

        private IEnumerable<string> FindMatchingDlls(NuGetUploadRule rule, IEnumerable<string> dlls)
        {
            var regex = new Regex(WildcardToRegex(rule.Pattern));

            var matches = dlls
                            .Where(l => regex.IsMatch(GetFilename(l)))
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
                    var files = Directory.GetFiles(fwFolder, "*.*").ToList();
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
            p.StartInfo.Arguments = $" install {packageID}{versionOption} -Framework {Options.TargetFramework}";
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
