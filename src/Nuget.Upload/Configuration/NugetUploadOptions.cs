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
            this.NugetConfigPath = "%APPDATA%\\NuGet\\NuGet.config";
        }

        public string TempFolder { get; set; }

        public string TargetFramework { get; set; }

        public string NugetConfigPath { get; set; }

        public IEnumerable<NuGetUploadRule> UploadRules { get; set; }

    }
}
