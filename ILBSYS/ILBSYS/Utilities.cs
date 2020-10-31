/*
 * Author : Jonny Hofmann
 * File : Utilities.cs
 * Utility : Class to make all the requests on the InfluxDB server
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILBSYS
{
    static class Utilities
    {
        static private List<Server> Servers = new List<Server>();
        static private int SelectedServerIndex = -1;
        static private bool HostsUpdated = false;
        static private List<Host> Hosts = new List<Host>();

        /// <summary>
        /// Returns all the servers
        /// </summary>
        /// <returns>Array of Server</returns>
        static public List<Server> GetAllServers()
        {
            return Servers;
        }

        /// <summary>
        /// Adds a server to the list
        /// </summary>
        /// <param name="server">Server to add</param>
        static public void AddServer(Server server)
        {
            Servers.Add(server);
            if(Servers.Count == 1)
            {
                SetSelectedServerIndex(0);
            }
        }

        /// <summary>
        /// Gets the selected server index (index for the server in the Servers array)
        /// </summary>
        /// <returns>Index of the selected server</returns>
        static public int GetSelectedtServerIndex()
        {
            return SelectedServerIndex;
        }

        /// <summary>
        /// Sets the selected server to the one with given index
        /// </summary>
        /// <param name="index">Index of the server to select</param>
        static public void SetSelectedServerIndex(int index)
        {
            SelectedServerIndex = index;
            InfluxDB.SetCurrentServerName(GetSelectedServer().Name);
            InfluxDB.SetCurrentServerAddress(GetSelectedServer().Address);
            HostsUpdated = false;
        }

        /// <summary>
        /// Returns the selected server
        /// </summary>
        /// <returns>Server selected server</returns>
        static public Server GetSelectedServer()
        {
            return Servers[SelectedServerIndex];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        static public void DeleteServer(int index)
        {
            Servers[index] = null;
        }

        /// <summary>
        /// Returns the list of all hosts from the current server
        /// </summary>
        /// <returns></returns>
        static public async Task<List<Host>> GetAllHostsAsync()
        {
            if(HostsUpdated)
            {
                return Hosts;
            } else
            {
                HostsUpdated = true;
                Hosts = await InfluxDB.GetAllHostsAsync();
                return Hosts;
            }
        }

        /// <summary>
        /// Sets the current host
        /// </summary>
        /// <param name="name">Name of the current host</param>
        static public void SetCurrentHost(string name)
        {
            InfluxDB.SetCurrentHost(name);
        }
    }
}
