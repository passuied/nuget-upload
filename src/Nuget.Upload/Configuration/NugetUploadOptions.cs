using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cornerstone.NuGet.Upload.Configuration
{
    public class NuGetUploadOptions
    {
        public NuGetUploadOptions()
        {
            DeleteStagingFolder = true;
            ApplyRulesPerDllName = true;
        }

        public string TempFolder { get; set; }

        public string TargetFramework { get; set; }

        public bool DeleteStagingFolder { get; set; }


        /// <summary>
        /// When true, Apply rules to Dll name, when false, apply rules per NuGet package name
        /// </summary>
        public bool ApplyRulesPerDllName { get; set; }

        public IEnumerable<NuGetUploadRule> UploadRules { get; set; }

    }
}
