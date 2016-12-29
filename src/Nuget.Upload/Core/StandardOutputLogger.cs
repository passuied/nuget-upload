using NuGet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cornerstone.NuGet.Upload.Core
{
    public class StandardOutputLogger : ILogger
    {

        public StandardOutputLogger(Action<string> standardOutput)
        {
            this.StandardOutput = standardOutput;
        }

        public Action<string> StandardOutput { get; private set; }

        public void LogDebug(string data)
        {
            this.StandardOutput($"DEBUG: {data}");
        }

        public void LogError(string data)
        {
            this.StandardOutput($"ERROR: {data}");

        }

        public void LogErrorSummary(string data)
        {
            this.StandardOutput($"ERROR Summary: {data}");

        }

        public void LogInformation(string data)
        {
            this.StandardOutput($"INFO: {data}");

        }

        public void LogInformationSummary(string data)
        {
            this.StandardOutput($"INFO Summary: {data}");

        }

        public void LogMinimal(string data)
        {
            this.StandardOutput($"{data}");

        }

        public void LogVerbose(string data)
        {
            this.StandardOutput($"VERBOSE: {data}");

        }

        public void LogWarning(string data)
        {
            this.StandardOutput($"WARN: {data}");

        }
    }
}
