/*
 * Author : Jonny Hofmann
 * File : Server.cs
 * Utility : A server is a host with an InfluxDB instance running
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILBSYS
{
    class Server
    {
        public string Address;
        public string Name;

        public Server(string name, string address)
        {
            this.Address = address;
            this.Name = name;
        }

    }
}
