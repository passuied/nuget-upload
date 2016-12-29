using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cornerstone.NuGet.Upload.Core
{
    public class NugetUploaderSourceRepositoryProvider : ISourceRepositoryProvider
    {
        private readonly global::NuGet.Configuration.IPackageSourceProvider _packageSourceProvider;
        private readonly List<Lazy<INuGetResourceProvider>> _resourceProviders;
        private readonly List<SourceRepository> _repositories = new List<SourceRepository>();

        // There should only be one instance of the source repository for each package source.
        private static readonly ConcurrentDictionary<global::NuGet.Configuration.PackageSource, SourceRepository> _cachedSources
            = new ConcurrentDictionary<global::NuGet.Configuration.PackageSource, SourceRepository>();

        public NugetUploaderSourceRepositoryProvider(global::NuGet.Configuration.IPackageSourceProvider packageSourceProvider)
        {
            _packageSourceProvider = packageSourceProvider;

            _resourceProviders = new List<Lazy<INuGetResourceProvider>>();
            _resourceProviders.AddRange(global::NuGet.Protocol.Core.v2.FactoryExtensionsV2.GetCoreV2(Repository.Provider));
            _resourceProviders.AddRange(global::NuGet.Protocol.FactoryExtensionsV2.GetCoreV3(Repository.Provider));

            // Create repositories
            _repositories = _packageSourceProvider.LoadPackageSources()
                .Where(s => s.IsEnabled)
                .Select(CreateRepository)
                .ToList();
        }

        /// <summary>
        /// Retrieve repositories that have been cached.
        /// </summary>
        public IEnumerable<SourceRepository> GetRepositories()
        {
            return _repositories;
        }

        /// <summary>
        /// Create a repository and add it to the cache.
        /// </summary>
        public SourceRepository CreateRepository(global::NuGet.Configuration.PackageSource source)
        {
            return CreateRepository(source, FeedType.Undefined);
        }

        public SourceRepository CreateRepository(global::NuGet.Configuration.PackageSource source, FeedType type)
        {
            return _cachedSources.GetOrAdd(source, new SourceRepository(source, _resourceProviders, type));
        }

        public global::NuGet.Configuration.IPackageSourceProvider PackageSourceProvider
        {
            get { return _packageSourceProvider; }
        }
    }
}
