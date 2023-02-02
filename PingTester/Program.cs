/*
 * Useful if you have terrible internet like myself :)
 */

using System.Net.NetworkInformation;

namespace PingTester
{
    class Program
    {
        // List of websites we'll ping to generate an average ping.
        static readonly string[] Hostnames = 
        { 
            "google.com",
            "discord.com",
            "cloudflare.com"
        };

        static readonly int PingsPerHost = 5;

        private static readonly Ping ping;
        private static readonly int defaultBufferSize = 32;

        static Program()
        {
            Console.Title = typeof(Program).Namespace + " | Press R to test again.";
            Console.WriteLine("Press R to test again.\n");

            ping = new Ping();
        }

        static async Task Main()
        {
            List<long> pingResults = new();

            foreach (string hostname in Hostnames)
            {
                for (int i = 0; i < PingsPerHost; i++)
                {
                    long ping = await PingHost(hostname);

                    // If -1 then there was an error, so don't add this result to the list.
                    if (ping != -1)
                    {
                        pingResults.Add(ping);
                    }
                }
            }

            // Calculate the mean.
            long average = 0;
            long highest = 0;
            foreach (long result in pingResults)
            {
                if (result > highest)
                {
                    highest = result;
                }

                average += result;
            }
            average /= pingResults.Count;
            
            Console.WriteLine($"\nAverage ping: {average} | Highest ping: {highest} | Total bytes sent: {pingResults.Count * defaultBufferSize}");

            // If the user presses the R key restart the program.
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.KeyChar == 'r' || key.KeyChar == 'R')
            {
                Console.WriteLine("\n");
                await Main();
            }
        }

        /// <summary>
        /// Pings a hostname with 32 bytes of data and measures the RoundtripTime.
        /// </summary>
        /// <param name="hostname">Hostname to ping.</param>
        /// <returns>ping A.K.A RoundtripTime</returns>
        static async Task<long> PingHost(string hostname)
        {
            PingReply reply = await ping.SendPingAsync(hostname);
            if (reply.Status == IPStatus.Success)
            {
                Console.WriteLine($"{hostname} | Ping: {reply.RoundtripTime} | Buffer Len: {reply.Buffer.Length}");
                return reply.RoundtripTime;
            }
            else
            {
                Console.WriteLine($"Error: {reply.Status} | Hostname: {hostname}");
                return -1;
            }
        }
    }
}