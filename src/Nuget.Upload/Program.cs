using Cornerstone.Configuration;
using NuGet.Upload.Configuration;
using NuGet.Upload.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Upload
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
                                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                .AddJsonFile("appsettings.json")
                                .Build();

            var options = config.GetSection("NugetUpload").Get<NugetUploadOptions>();

            string packageID = string.Empty;
            string packageVersionID = string.Empty;
            if (args.Count() > 0)
            {
                switch (args[0])
                {
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
