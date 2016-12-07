using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;

namespace GRA.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!Directory.Exists("content"))
            {
                try
                {
                    Directory.CreateDirectory("content");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to create directory 'content' in {Directory.GetCurrentDirectory()}");
                    throw (ex);
                }
            }

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
