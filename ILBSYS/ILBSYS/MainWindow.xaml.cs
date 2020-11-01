using System;
using System.Collections.Generic;
using System.Data;
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
            //Utilities.AddServer(new Server("tower", "http://192.168.1.3:8086"));
            this.UpdateHostList();
            this.PopulateServerList();


        }

        /// <summary>
        /// Updates the host list in the datagrid
        /// </summary>
        async public void UpdateHostList()
        {
            try
            {
                dgHosts.Items.Clear();
                List<Host> hosts = await Utilities.GetAllHostsAsync(); ;
                dgHosts.ItemsSource = hosts;

            } catch(Exception e)
            {

            }
            
        }

        /// <summary>
        /// Changes the current server as soon as the user clicks on one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Console.WriteLine(cbServers.SelectedItem.ToString());
            if(cbServers.SelectedIndex != -1)
            {
                Utilities.SetSelectedServerIndex(cbServers.SelectedIndex);
                this.UpdateHostList();
            }
        }

        /// <summary>
        /// Handles the button to add a server, opens a modal for it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_AddServer_Click(object sender, RoutedEventArgs e)
        {
            AddServer dlg = new AddServer();
            dlg.Owner = this;
            dlg.ShowDialog();
        }

        /// <summary>
        /// Populates the server list with all the servers that are in the Utilities class
        /// </summary>
        public void PopulateServerList()
        {
            List<Server> serverList = Utilities.GetAllServers();
            
            // Saves the current selected index before clearing the itemList
            int previousIndex = cbServers.SelectedIndex;

            cbServers.Items.Clear();

            // Adding each server to the server combobox
            serverList.ForEach(delegate (Server server)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = server.Name;
                item.Tag = server.Address;
                cbServers.Items.Add(item);
            });

            // Setting the old selected item in the combobox
            cbServers.SelectedIndex = previousIndex;

            if(serverList.Count == 1 && previousIndex == -1)
            {
                cbServers.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Asks for the data and puts it in the information datagrid
        /// </summary>
        async public void UpdateInfoDataGrid()
        {
            List<Info> infos = new List<Info>();
            
            // Uptime
            int uptime = await InfluxDB.GetUptime();
            int uptimeHours = uptime / 60 / 60;
            infos.Add(new Info("Uptime", uptimeHours.ToString() + " hours"));

            // CPU usage
            double cpuUsage = await InfluxDB.GetCPUUsage();
            infos.Add(new Info("CPU Usage", cpuUsage.ToString().Substring(0, 4) + "%"));

            // RAM usage
            double ramUsage = await InfluxDB.GetRamUsage();
            infos.Add(new Info("RAM Usage", ramUsage.ToString().Substring(0, 4) + "%"));

            // Ram usage average over 24h
            double ramUsage24h = await InfluxDB.GetRAMUsageAverage24h();
            infos.Add(new Info("Average RAM usage (24h)", ramUsage24h.ToString().Substring(0, 4) + "%"));

            // CPU usage average over 24h
            double cpuUsage24h = await InfluxDB.GetCPUUsageAverage24h();
            infos.Add(new Info("Average CPU Usage (24h)", cpuUsage24h.ToString().Substring(0, 4) + "%"));

            dgPCInfo.ItemsSource = infos;
        }

        /// <summary>
        /// On selection changed, make the info grid update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgHosts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Host host= (Host)dgHosts.SelectedItem;
            Utilities.SetCurrentHost(host.Name);
            UpdateInfoDataGrid();
        }
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

    public class Info
    {
        public Info(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
