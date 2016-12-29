using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cornerstone.NuGet.Upload.Configuration
{
    public class NuGetUploadRule
    {
        public string Pattern { get; set; }
        public string TargetFolder { get; set; }
    }
}
