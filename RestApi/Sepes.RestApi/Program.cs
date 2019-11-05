using System.Diagnostics.CodeAnalysis;
using System.IO;
using dotenv.net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Sepes.RestApi
{
    [ExcludeFromCodeCoverage]
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
