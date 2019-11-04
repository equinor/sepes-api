using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using dotenv.net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Sepes.RestApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoadDotEnv();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        public static void LoadDotEnv() {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while(currentDirectory != null){
                foreach (var fi in currentDirectory.GetFiles(".env", SearchOption.TopDirectoryOnly))
                {
                    DotEnv.Config(false, fi.FullName);
                    return; // found .env file
                }
                currentDirectory = currentDirectory.Parent;
            }
        }
    }
}
