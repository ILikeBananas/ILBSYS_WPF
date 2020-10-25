using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ILBSYS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InfluxDB.SetCurrentServerAddress("http://192.168.1.3:8086");
            this.UpdateHostList();
            
        }

        /// <summary>
        /// Updates the host list in the datagrid
        /// </summary>
        async public void UpdateHostList()
        {
            string[] hosts = await Utilities.GetAllHostsAsync();
            List<Host> hostsList = new List<Host>();
            
            for(int i = 0; i < hosts.Length; i++)
            {
                hostsList.Add(new Host(hosts[i]));
            }

            dgHosts.ItemsSource = hostsList;
        }

        /// <summary>
        /// Just a class for the hosts
        /// </summary>
        public class Host
        {
            public string Name { get; set; }
            public Host(string name)
            {
                Name = name;
            }
        }
    }
}
