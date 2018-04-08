using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventMonitor.Agent.Windows
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = System.Configuration.ConfigurationManager.AppSettings["EventMonitor.ServiceUrl"];
            var url = host + "/api/event/emit";
            PerformanceCounter cpuPercentage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuPercentage.NextValue();

            HttpClient client = new HttpClient();

            while (true)
            {
                try
                {
                    String json = JsonConvert.SerializeObject(new
                    {
                        origin = new
                        {
                            name = Environment.MachineName,
                            location = "unknown"
                        },
                        type = "cpu.total-percentage",
                        value = new
                        {
                            value = cpuPercentage.NextValue()
                        },
                        timestampUtc = DateTime.UtcNow
                    });

                    Task send = client.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                    send.Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
