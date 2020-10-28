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
using System.Windows.Shapes;

namespace ILBSYS
{
    /// <summary>
    /// Interaction logic for AddServer.xaml
    /// </summary>
    public partial class AddServer : Window
    {
        /// <summary>
        /// Opens the dialog box and puts default values in it
        /// </summary>
        public AddServer()
        {
            InitializeComponent();
            txtAddress.Text = "localhost";
        }

        /// <summary>
        /// Adds a server with the given inputs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnDialogOk_Click(Object sender, RoutedEventArgs e)
        {
            Utilities.AddServer(new Server(txtHostname.Text, "http://" + txtAddress.Text + ":8086"));
            MainWindow mainWindow = (MainWindow)Owner;
            mainWindow.PopulateServerList();
            this.Close();
        }
    }
}
