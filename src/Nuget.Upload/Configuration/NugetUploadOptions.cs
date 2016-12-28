﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Upload.Configuration
{
    public class NugetUploadOptions
    {
        public string TempFolder { get; set; }

        public string TargetFramework { get; set; }

        public IEnumerable<NugetUploadRule> UploadRules { get; set; }

    }
}