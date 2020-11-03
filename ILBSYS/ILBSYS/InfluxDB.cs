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
    static class InfluxDB
    {
        static private string CurrentServerAddress;
        static private string CurrentServerName;
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
        /// Sets the current server name
        /// </summary>
        /// <param name="name">Name for the current server</param>
        static public void SetCurrentServerName(string name)
        {
            CurrentServerName = name;
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
        static public async Task<double> GetRamUsage()
        {
            double ramUsage = 0.0;
            try
            {
                InfluxClient client = new InfluxClient(new Uri(CurrentServerAddress));
                var response = await client.ReadAsync<DynamicInfluxRow>("telegraf", "SELECT last(\"used_percent\") from \"mem\" WHERE \"host\" =\'" + CurrentHost + "\'");
                var result = response.Results[0];
                ramUsage = (double)result.Series[0].Rows[0].Fields.Values.ElementAt(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return ramUsage;
        }

        /// <summary>
        /// Get the last CPU usage recorded on the Influx server for the current server
        /// </summary>
        /// <returns></returns>
        static public async Task<double> GetCPUUsage()
        {
            double cpuUsage = 0.0;
            try
            {
                InfluxClient client = new InfluxClient(new Uri(CurrentServerAddress));
                var response = await client.ReadAsync<DynamicInfluxRow>("telegraf", "SELECT last(\"usage_idle\") from \"cpu\" WHERE (\"cpu\" = \'cpu-total\' AND \"host\" =\'" + CurrentHost + "\')");
                var result = response.Results[0];
                cpuUsage = (double)result.Series[0].Rows[0].Fields.Values.ElementAt(0);
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Change the usage from idle to current usage
            cpuUsage = cpuUsage * -1 + 100;
            return cpuUsage;
        }

        /// <summary>
        /// Get the uptime of the current server 
        /// </summary>
        /// <returns></returns>
        async static public Task<int> GetUptime()
        {
            int uptime = 0;
            try
            {
                InfluxClient client = new InfluxClient(new Uri(CurrentServerAddress));
                //var response = client.ReadAsync<DynamicInfluxRow>("telegraf", "SHOW TAG VALUES WITH KEY=host");
                var response = await client.ReadAsync<DynamicInfluxRow>("telegraf", "SELECT last(\"uptime\") from \"system\" WHERE (\"host\" =\'" + CurrentHost + "\')");
                var result = response.Results[0];
                uptime = Int32.Parse(result.Series[0].Rows[0].Fields.Values.ElementAt(0).ToString());
                //uptime = (int)result.Series[0].Rows[0].Fields.Values.ElementAt(0).ToString();
                Console.WriteLine("Uptime : " + uptime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return uptime;
        }

        /// <summary>
        /// Gets in the database the average cpu usage over 24h
        /// </summary>
        /// <returns>Average CPU usage over 24 hours</returns>
        static public async Task<Double> GetCPUUsageAverage24h()
        {
            double cpuUsage = 0.0;
            try
            {
                InfluxClient client = new InfluxClient(new Uri(CurrentServerAddress));
                var response = await client.ReadAsync<DynamicInfluxRow>("telegraf", "SELECT mean(\"usage_idle\") FROM \"cpu\" WHERE  (\"cpu\" = \'cpu-total\' AND \"host\" =\'" + CurrentHost + "\') AND time >= now() - 24h");
                var result = response.Results[0];
                var series = result.Series[0];
                cpuUsage = (double)result.Series[0].Rows[0].Fields.Values.ElementAt(0);

            }
            catch (Exception e)
            {
                Console.WriteLine("GetRAMUsageAverage24h : " + e.Message);
            }

            // Change the usage from idle to utilization
            cpuUsage = cpuUsage * -1 + 100;
            return cpuUsage;
        }

        /// <summary>
        /// Gets in the database the average RAM usage over 24 hours
        /// </summary>
        /// <returns>Average RAM usage over 24 hours</returns>
        static public async Task<Double> GetRAMUsageAverage24h()
        {
            double ramUsage = 0.0;
            try
            {
                InfluxClient client = new InfluxClient(new Uri(CurrentServerAddress));
                var response = await client.ReadAsync<DynamicInfluxRow>("telegraf", "SELECT mean(\"used_percent\") FROM \"mem\" WHERE time >= now() - 24h GROUP BY \"host\"");
                var result = response.Results[0];
                var series = result.Series[0];
                ramUsage = (double)result.Series[0].Rows[0].Fields.Values.ElementAt(0);

            } catch(Exception e)
            {
                Console.WriteLine("GetRAMUsageAverage24h : " + e.Message);
            }

            return ramUsage;
        }
        
        /// <summary>
        /// Returns all the hosts from the server
        /// </summary>
        /// <returns>string array of hostnames on the server</returns>
        static public async Task<List<Host>> GetAllHostsAsync()
        {
            InfluxClient client = new InfluxClient(new Uri(CurrentServerAddress));
            //var response = client.ReadAsync<DynamicInfluxRow>("telegraf", "SHOW TAG VALUES WITH KEY=host");
            var response = await client.ReadAsync<DynamicInfluxRow>("telegraf", "SELECT last(\"uptime\") FROM \"system\" GROUP BY \"host\"");
            var result = response.Results[0];
            var series = result.Series[0];
            List<Host> hosts = new List<Host>();
            for(int i = 0; i < series.GroupedTags.Count; i++)
            {
                hosts.Add(new Host(series.GroupedTags.ElementAt(i).Value.ToString()));
            }

            return hosts;
        }
    }
}
