using Cornerstone.Configuration;
using Cornerstone.NuGet.Upload.Configuration;
using Cornerstone.NuGet.Upload.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cornerstone.Nuget.Upload
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
            
        }

        static async Task MainAsync(string[] args)
        {
            var config = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .Build();

            var options = config.GetSection("NugetUpload").Get<NuGetUploadOptions>();

            string packageID = string.Empty;
            string packageVersionID = string.Empty;
            if (args.Count() > 0)
            {
                switch (args[0])
                {
                    // Load dlls to target folder
                    case "load":
                        {
                            if (args.Count() >= 2)
                                packageID = args[1];
                            if (args.Count() == 3)
                                packageVersionID = args[2];

                            try
                            {
                                var uploader = new NugetUploader(options, s => Console.WriteLine(s));
                                await uploader.Upload(Directory.GetCurrentDirectory(), packageID, packageVersionID);
                                Console.WriteLine();
                                Console.WriteLine($"Package '{NugetUploader.FormatPackageAndVersion(packageID, packageVersionID)}' and dependencies successfully uploaded!");
                                Console.ReadLine();
                            }
                            catch (ArgumentException ae)
                            {
                                Console.WriteLine(ae.Message);
                            }
                            catch (InvalidOperationException ioe)
                            {
                                Console.WriteLine(ioe.Message);
                            }
                        }
                        break;
                    // Show info about package, includind dependencies
                    case "info":
                        {
                            if (args.Count() >= 2)
                                packageID = args[1];
                            if (args.Count() == 3)
                                packageVersionID = args[2];
                            var uploader = new NugetUploader(options, s => Console.WriteLine(s));
                            await uploader.ShowInfo(packageID, packageVersionID);

                        }
                        break;
                    default:
                        WriteHelp();
                        break;
                }

            }
            else
                WriteHelp();
        }

        static void WriteHelp()
        {
            Console.WriteLine($"Usage: nuget-upload.exe load <NuGetPackageID> [<NuGetPackageVersionID>]");
        }

    }
}
