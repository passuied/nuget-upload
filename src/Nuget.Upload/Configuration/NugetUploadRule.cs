using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Upload.Configuration
{
    public class NugetUploadRule
    {
        public string Pattern { get; set; }
        public string TargetFolder { get; set; }
    }
}
