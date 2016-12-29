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
        }

        public string TempFolder { get; set; }

        public string TargetFramework { get; set; }

        public bool DeleteStagingFolder { get; set; }

        public IEnumerable<NuGetUploadRule> UploadRules { get; set; }

    }
}
