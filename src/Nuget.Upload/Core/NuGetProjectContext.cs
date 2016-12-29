using NuGet.ProjectManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Packaging;
using System.Xml.Linq;
using System.Globalization;

namespace Cornerstone.NuGet.Upload.Core
{
    public class NuGetProjectContext : INuGetProjectContext
    {
        private readonly global::NuGet.Common.ILogger _logger;

        public NuGetProjectContext(global::NuGet.Common.ILogger logger)
        {
            _logger = logger;
        }

        public ExecutionContext ExecutionContext => null;

        public PackageExtractionContext PackageExtractionContext { get; set; }

        public XDocument OriginalPackagesConfig { get; set; }

        public ISourceControlManagerProvider SourceControlManagerProvider => null;

        public void Log(global::NuGet.ProjectManagement.MessageLevel level, string message, params object[] args)
        {
            if (args.Length > 0)
            {
                message = string.Format(CultureInfo.CurrentCulture, message, args);
            }

            switch (level)
            {
                case global::NuGet.ProjectManagement.MessageLevel.Debug:
                    _logger.LogDebug(message);
                    break;

                case global::NuGet.ProjectManagement.MessageLevel.Info:
                    _logger.LogMinimal(message);
                    break;

                case global::NuGet.ProjectManagement.MessageLevel.Warning:
                    _logger.LogWarning(message);
                    break;

                case global::NuGet.ProjectManagement.MessageLevel.Error:
                    _logger.LogError(message);
                    break;
            }
        }

        public void ReportError(string message)
        {
            _logger.LogError(message);
        }

        public virtual global::NuGet.ProjectManagement.FileConflictAction ResolveFileConflict(string message)
        {
            return global::NuGet.ProjectManagement.FileConflictAction.IgnoreAll;
        }

        public NuGetActionType ActionType { get; set; }
    }
}
