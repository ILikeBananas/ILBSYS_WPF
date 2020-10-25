/*
 * Author : Jonny Hofmann
 * File : InfluxDB.cs
 * Utility : Class to make all the requests on the InfluxDB server
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vibrant.InfluxDB.Client;
using Vibrant.InfluxDB;
using Vibrant.InfluxDB.Client.Rows;

namespace ILBSYS
{
    /// <summary>
    /// Structure for usage over time, will be used as an array
    /// </summary>
    struct UsageWithTime {
        public string Timestamp { get; set; }
        public double Usage { get; set; }
    }

    static class InfluxDB
    {
        static private string CurrentServerAddress;
        static private string CurrentHost;

        /// <summary>
        /// Sets the current server address
        /// </summary>
        /// <param name="address">The address to set</param>
        static public void SetCurrentServerAddress(string address)
        {
            CurrentServerAddress = address;
            
        }

        /// <summary>
        /// Sets the host to the name given
        /// </summary>
        /// <param name="host">^Name to set as current host</param>
        static public void SetCurrentHost(string host)
        {
            CurrentHost = host;
        }

        /// <summary>
        /// Get the last RAM usage recorded on the Influx server for the current server 
        /// </summary>
        /// <returns></returns>
        static public double GetRamUsage()
        {
            return 0.0;
        }

        /// <summary>
        /// Get the last CPU usage recorded on the Influx server for the current server
        /// </summary>
        /// <returns></returns>
        static public double GetCPUUsage()
        {
            return 0.0;
        }

        /// <summary>
        /// Get the uptime of the current server 
        /// </summary>
        /// <returns></returns>
        static public int GetUptime()
        {
            return 0;
        }

        /// <summary>
        /// Gets in the database the cpu usage over the last 6 hours
        /// </summary>
        /// <returns>Array of UsageWithTime containing the points for the last 6 hours of cpu usage</returns>
        static public UsageWithTime[] GetCPUOverTime()
        {
            return new UsageWithTime[0];
        }

        /// <summary>
        /// Gets in the database the ram usage over the last 6 hours
        /// </summary>
        /// <returns>Array of UsageWithTime containing the points for the last 6 hours of ram usage</returns>
        static public UsageWithTime[] GetRAMOverTime()
        {
            return new UsageWithTime[0];
        }
        
        /// <summary>
        /// Returns all the hosts from the server
        /// </summary>
        /// <returns>string array of hostnames on the server</returns>
        static public async Task<List<string>> GetAllHostsAsync()
        {
            Console.WriteLine("Starting");
            InfluxClient client = new InfluxClient(new Uri(CurrentServerAddress));
            //var response = client.ReadAsync<DynamicInfluxRow>("telegraf", "SHOW TAG VALUES WITH KEY=host");
            var response = await client.ReadAsync<DynamicInfluxRow>("telegraf", "SELECT last(\"uptime\") FROM \"system\" GROUP BY \"host\"");
            var result = response.Results[0];
            var series = result.Series[0];
            List<string> hosts = new List<string>();
            for(int i = 0; i < series.GroupedTags.Count; i++)
            {
                hosts.Add(series.GroupedTags.ElementAt(i).Value.ToString());
            }

            return hosts;
        }

        
    }

    public class ComputerInfo
    {
        [InfluxTimestamp]
        public DateTime TimeStamp { get; set; }

        [InfluxTag("host")]
        public string Host { get; set; }

        [InfluxField("cpu")]
        public double CPU { get; set; }

        [InfluxField("ram")]
        public long RAM { get; set; }
    }
}
