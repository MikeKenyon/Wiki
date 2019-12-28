using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Wiki.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration( (host, c) => {
                    c.AddJsonFile("appsettings.json");
                    })
                .ConfigureLogging((host, log) => log.AddEventLog(
                    new EventLogSettings
                    {

                    }))
                .UseConsoleLifetime()
                .RunConsoleAsync();
        }
    }
}
