using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentitySamplesNetCore.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IdentitySamplesNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            if (args.Any())
            {
                foreach (var item in args)
                {
                    System.Console.WriteLine(item);
                }
            }

            // dotnet run /seed true 
            var seed = args.Any(x => x == "/seed");
            if (seed)
            {
                args = args.Except(new[] { "/seed" }).ToArray();
            }
            if (seed)
            {
                System.Console.WriteLine("Seeding data");
                SeedData.EnsureSeedDataAsync(host.Services).Wait();
                return;
            }
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
