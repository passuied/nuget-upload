using Cornerstone.Configuration;
using Nuget.Upload.Configuration;
using Nuget.Upload.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Upload
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
                                var uploader = new NugetUploader(options);
                                await uploader.Upload(Directory.GetCurrentDirectory(), packageID, packageVersionID);
                                Console.WriteLine($"Package '{packageID}' and dependencies successfully uploaded!");
                                Console.ReadLine();
                            }
                            catch (ArgumentException ae)
                            {
                                Console.WriteLine(ae.Message);
                                WriteHelp();
                            }
                            catch (InvalidOperationException ioe)
                            {
                                Console.WriteLine(ioe.Message);
                                WriteHelp();
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
            Console.WriteLine($"Usage: nuget-upload.exe load <NugetPackageID> [<NugetPackageVersionID>]");
        }

    }
}
