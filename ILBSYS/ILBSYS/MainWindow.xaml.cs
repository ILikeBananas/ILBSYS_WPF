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
